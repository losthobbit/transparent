using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Interfaces;

namespace Transparent.Data.Caches
{
    using Models;
    using System.Data.Entity;

    public class Tags : ITags
    {
        private readonly IUsersContext context;

        private List<IndentedTag> indentedTags;

        public Tags(IUsersContext context)
        {
            this.context = context;

            var tags = context.Tags.Include(tag => tag.Children).Include(tag => tag.Parents).ToList();
            Root = tags.Single(tag => tag.Name == Constants.CriticalThinkingTagName);
        }

        public Tag Root { get; private set; }

        public Tag Find(int id)
        {
            return Find(id, Root);
        }

        public Tag Find(int id, Tag root)
        {
            if (root.Id == id)
                return root;
            if(root.Children != null)
                foreach (var tag in root.Children)
                {
                    var found = Find(id, tag);
                    if (found != null)
                        return found;
                }
            return null;
        }

        public IEnumerable<IndentedTag> IndentedTags
        {
            get 
            {
                if (indentedTags == null)
                    BuildIndentedTags(0);
                return indentedTags;
            }
        }

        private void BuildIndentedTags(int indent, Tag current = null)
        {
            if (current == null)
            {
                indentedTags = new List<IndentedTag>();
                current = Root;
            }
            indentedTags.Add(new IndentedTag { Tag = current, Indent = indent++ });
            foreach (var tag in current.Children.OrderBy(child => child.Children.Count()))
            {
                BuildIndentedTags(indent, tag);
            }
        }
    }
}

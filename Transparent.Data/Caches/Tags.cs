using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Interfaces;

namespace Transparent.Data.Caches
{
    using Common;
    using Models;
    using System.Data.Entity;
    using System.Web;

    /// <summary>
    /// A cache for the tags.
    /// </summary>
    /// <remarks>
    /// Can be a singleton.
    /// </remarks>
    public class Tags : ITags
    {
        private object _lock = new object();

        private readonly IUsersContext context;
        private readonly Dictionary<int, IHtmlString> serializedTags = new Dictionary<int, IHtmlString>();
        private IHtmlString serializedIndentedTags;

        private List<IndentedTag> indentedTags;

        public Tags(IUsersContext context)
        {
            this.context = context;

            Refresh();
        }

        public Tag Root { get; private set; }
        public Tag ApplicationTag { get; private set; }

        /// <summary>
        /// Reload everything from the database.
        /// </summary>
        public void Refresh()
        {
            lock (_lock)
            {
                var tags = context.Tags.Include(tag => tag.Children).Include(tag => tag.Parents).ToList();
                Root = tags.Single(tag => tag.Name == Constants.CriticalThinkingTagName);
                ApplicationTag = tags.Single(tag => tag.Name == Constants.ApplicationName);
                indentedTags = new List<IndentedTag>();
                BuildIndentedTags(indentedTags);
                serializedIndentedTags = JavaScriptRoutines.SerializeObject(IndentedTags);
            }
        }

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
                return indentedTags;
            }
        }

        private void BuildIndentedTags(ICollection<IndentedTag> indentedTags, int indent = 0, Tag current = null, IEnumerable<int> acceptableTagIds = null)
        {
            if (current == null)
            {                
                current = Root;
            }
            if (acceptableTagIds == null || acceptableTagIds.Contains(current.Id))
            {
                indentedTags.Add(new IndentedTag { Id = current.Id, Name = current.Name, Indent = indent++ });
                foreach (var tag in current.Children.OrderBy(child => child.Children.Count()))
                {
                    BuildIndentedTags(indentedTags, indent, tag, acceptableTagIds);
                }
            }
        }

        public class SerializableSubTag
        {
            private Tag tag;

            public SerializableSubTag(Tag tag)
            {
                this.tag = tag;
            }

            public int Id { get { return tag.Id; } }
            public string Name { get { return tag.Name; } }
        }

        public class SerializableTag: SerializableSubTag
        {
            public SerializableTag(Tag tag): base(tag)
            {
                Parents = tag.Parents.Select(parent => new SerializableSubTag(parent)).ToList();
                Children = tag.Children.Select(child => new SerializableSubTag(child)).ToList();
            }

            public ICollection<SerializableSubTag> Parents { get; private set; }
            public ICollection<SerializableSubTag> Children { get; private set; }
        }

        public IHtmlString SerializeTag(Tag tag)
        {
            IHtmlString json;
            if (serializedTags.TryGetValue(tag.Id, out json))
                return json;
            var serializableTag = new SerializableTag(tag);
            json = JavaScriptRoutines.SerializeObject(serializableTag);
            serializedTags.Add(tag.Id, json);
            return json;
        }

        public IHtmlString SerializeAndIndentTags(int[] tagIds)
        {
            if (!tagIds.Any())
                return new HtmlString("[]");
            var indentedTags = new List<IndentedTag>();
            BuildIndentedTags(indentedTags, 0, null, tagIds);
            return JavaScriptRoutines.SerializeObject(indentedTags);
        }

        public IHtmlString SerializedIndentedTags
        {
            get 
            {
                return serializedIndentedTags;
            }
        }
    }
}

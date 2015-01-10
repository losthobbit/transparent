﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Interfaces;

namespace Transparent.Data.Caches
{
    using Models;
    using System.Data.Entity;
    using System.Web;

    public class Tags : ITags
    {
        private readonly IUsersContext context;
        private readonly Dictionary<int, IHtmlString> serializedTags = new Dictionary<int, IHtmlString>();

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
            json = Common.JavaScriptRoutines.SerializeObject(serializableTag);
            serializedTags.Add(tag.Id, json);
            return json;
        }
    }
}

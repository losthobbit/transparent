using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Transparent.Data.Models;

namespace Transparent.Data.Interfaces
{
    public class IndentedTag
    {
        public int Indent { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public interface ITags
    {
        Tag Root { get; }
        Tag Find(int id);

        /// <summary>
        /// List of tags and their indentation level
        /// </summary>
        /// <remarks>
        /// Useful for building a tree view
        /// </remarks>
        IEnumerable<IndentedTag> IndentedTags { get; }
        IHtmlString SerializedIndentedTags { get; }

        IHtmlString SerializeTag(Tag tag);
        IHtmlString SerializeAndIndentTags(int[] tagIds);
    }
}

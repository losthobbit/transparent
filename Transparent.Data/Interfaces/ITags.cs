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
        Tag ApplicationTag { get; }

        /// <summary>
        /// List of tags and their indentation level
        /// </summary>
        /// <remarks>
        /// Useful for building a tree view
        /// </remarks>
        IEnumerable<IndentedTag> IndentedTags { get; }
        IHtmlString SerializedIndentedTags { get; }

        IHtmlString SerializeTag(int id);
        IHtmlString SerializeTag(Tag tag);
        IHtmlString SerializeAndIndentTags(int[] tagIds);

        /// <summary>
        /// Reload everything from the database.
        /// </summary>
        void Refresh();

        KnowledgeLevel GetKnowledgeLevel(UserTag userTag);

        int CreateTag(Tag tag, IEnumerable<int> parentIds);

        /// <summary>
        /// Returns the weighting based on the knowledge level of the user for the tag, and the knowledge weightings.
        /// </summary>
        /// <param name="userTag">The tag that the user has.</param>
        /// <param name="knowledgeLevelWeightings">The number of points per knowledge level.</param>
        /// <returns></returns>
        int GetWeighting(UserTag userTag, IKnowledgeLevelWeightings knowledgeLevelWeightings);

        /// <summary>
        /// Returns the weighting based on the knowledge level of the user for the tag, and the knowledge weightings.
        /// </summary>
        /// <param name="userId">The user's ID.</param>
        /// <param name="tagId">The tag's ID.</param>
        /// <param name="knowledgeLevelWeightings">The number of points per knowledge level.</param>
        /// <returns></returns>
        int GetWeighting(int userId, int tagId, IKnowledgeLevelWeightings knowledgeLevelWeightings);
    }
}

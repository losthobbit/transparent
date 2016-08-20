using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Transparent.Data.Annotations;
using Transparent.Data.Interfaces;
using Transparent.Data.Models.Interfaces;

namespace Transparent.ViewModels.Admin
{
    [MetadataType(typeof(ITag))]
    public class CreateTagViewModel : ITag
    {
        public string Name { get; set; }


        public string Description { get; set; }

        [Display(Name = "Parents")]
        [CannotBeEmpty(ErrorMessage = "A tag must have at least one parent")]
        public ICollection<int> ParentIds { get; set; }

        public IEnumerable<IndentedTag> AllTags { get; set; }

        #region Only included for the interface

        /// <summary>
        /// The number of points a user must have in a tag to be considered competent.
        /// </summary>
        public int CompetentPoints { get; set; }

        /// <summary>
        /// The number of points a user must have in a tag to be considered an expert.
        /// </summary>
        public int ExpertPoints { get; set; }

        #endregion Only included for the interface
    }
}
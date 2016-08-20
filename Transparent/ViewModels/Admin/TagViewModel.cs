using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Transparent.Data.Models.Interfaces;

namespace Transparent.ViewModels.Admin
{
    [MetadataType(typeof(ITag))]
    public class TagViewModel : ITag
    {
        public IHtmlString Json { get; set; }


        public string Name { get; set; }


        public string Description { get; set; }

        /// <summary>
        /// The number of points a user must have in a tag to be considered competent.
        /// </summary>
        public int CompetentPoints { get; set; }

        /// <summary>
        /// The number of points a user must have in a tag to be considered an expert.
        /// </summary>
        public int ExpertPoints { get; set; }
    }
}
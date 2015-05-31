using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Models.Interfaces;

namespace Transparent.Business.ViewModels
{
    [MetadataType(typeof(ITag))]
    public class TagViewModel: ITag
    {
        /// <summary>
        /// Tag ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Tag name
        /// </summary>
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

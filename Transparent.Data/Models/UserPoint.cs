using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Models.Interfaces;

namespace Transparent.Data.Models
{
    /// <summary>
    /// One for each time points were awarded for a particular tag.
    /// </summary>
    /// <remarks>
    /// Also shows the details about a test that was started.
    /// </remarks>
    [MetadataType(typeof(IUserPoint))]
    public class UserPoint: IUserPoint
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int FkUserId { get; set; }
        public virtual UserProfile User { get; set; }

        [ForeignKey("Tag")]
        public int FkTagId { get; set; }
        public virtual Tag Tag { get; set; }

        /// <summary>
        /// Points for the specific event.
        /// </summary>
        /// <remarks>
        /// These should not be set without also modifying the total points for the user tag.
        /// </remarks>
        [Display(Name = "Points")]
        public int Quantity { get; set; }

        public PointReason Reason { get; set; }

        public Badge? Badge { get; set; }

        // This section only applies to points that are gained or lost through taking a test
        // I suppose it would have made more sense to have a class that inherited from UserPoint
        // but I couldn't be bothered.  Feel free to refactor this.
        #region Test points

        [ForeignKey("TestTaken")]
        public int? FkTestId { get; set; }
        public Test TestTaken { get; set; }

        public string Answer { get; set; }

        public virtual ICollection<TestMarking> TestMarkings { get; set; }

        [Required]
        [Index]
        public bool MarkingComplete { get; set; }

        #endregion Test points
    }
}

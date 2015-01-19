﻿using System;
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
    public class UserPoint
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int FkUserId { get; set; }
        public UserProfile User { get; set; }

        [ForeignKey("Tag")]
        public int FkTagId { get; set; }
        public Tag Tag { get; set; }

        [Display(Name = "Points")]
        public int Quantity { get; set; }

        // This section only applies to points that are gained or lost through taking a test
        // I suppose it would have made more sense to have a class that inherited from UserPoint
        // but I couldn't be bothered.  Feel free to refactor this.
        #region Test points

        [ForeignKey("TestTaken")]
        public int FkTestId { get; set; }
        public Test TestTaken { get; set; }

        public string Answer { get; set; }

        public virtual ICollection<TestMarking> TestMarkings { get; set; }

        [Required]
        [Index]
        public bool MarkingComplete { get; set; }

        #endregion Test points
    }
}

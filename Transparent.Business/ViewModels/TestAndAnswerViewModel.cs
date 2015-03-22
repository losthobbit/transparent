using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Models;
using Transparent.Data.Models.Interfaces;

namespace Transparent.Business.ViewModels
{
    [MetadataType(typeof(ITestAnswer))]
    public class TestAndAnswerViewModel: ITestAnswer
    {
        public TestAndAnswerViewModel()
        {

        }

        public TestAndAnswerViewModel(Test test)
        {
            Test = test;
        }

        public Test Test { get; set; }

        [Required]
        public string Answer { get; set; }

        /// <summary>
        /// ID of UserPoint
        /// </summary>
        public int Id { get; set; }

        public bool? Passed { get; set; }
    }
}

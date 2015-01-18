﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.ViewModels;

namespace Transparent.Data.Models
{
    public class AnsweredTests: StatefulPagedList<TestAndAnswerViewModel, AnsweredTests>
    {
        public AnsweredTests()
            : base()
        {
        }

        public AnsweredTests(IQueryable<TestAndAnswerViewModel> items)
            : base(items)
        {
        }
    }
}

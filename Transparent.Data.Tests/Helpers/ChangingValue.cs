using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Data.Tests.Helpers
{
    public class ChangingValue
    {
        public Func<bool> GetValue;

        public ChangingValue()
        {
            this.GetValue = () => false;
        }

        public bool Value 
        {
            get { return GetValue(); }
        }

        public void AssertIsTrue()
        {
            Assert.IsTrue(Value);
        }
    }
}

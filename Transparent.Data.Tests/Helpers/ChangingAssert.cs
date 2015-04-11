using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Data.Tests.Helpers
{
    public class ChangingAssert
    {
        public Action Assertion;

        public ChangingAssert()
        {
            this.Assertion = () => Assert.Fail("Assertion not set");
        }

        public void Try()
        {
            Assertion();
        }
    }
}

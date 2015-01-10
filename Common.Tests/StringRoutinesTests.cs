using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Tests
{
    [TestClass]
    public class StringRoutinesTests
    {
        [TestMethod]
        public void Spaces_with_5_spaces_returns_5_spaces()
        {
            // Act
            var actual = StringRoutines.Spaces(5);

            // Act
            Assert.AreEqual("     ", actual);
        }
    }
}

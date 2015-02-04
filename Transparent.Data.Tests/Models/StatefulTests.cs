using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transparent.Data.Models;

namespace Transparent.Data.Tests.Models
{
    [TestFixture]
    public class StatefulTests
    {
        [Test]
        public void GetState_clones_and_has_same_property_values_as_original()
        {
            //Arrange
            var original = new Stateful();
            original.SetValue("key1", "value1");
            original.SetValue("key2", 25);

            //Act
            var actual = original.GetState();

            //Assert
            Assert.AreEqual("value1", actual.GetValue("key1"));
            Assert.AreEqual(25, actual.GetNullableInt("key2").Value);
        }
    }
}

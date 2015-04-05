using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Tests
{
    [TestFixture]
    public class StringRoutinesTests
    {
        #region Spaces

        [Test]
        public void Spaces_with_5_spaces_returns_5_spaces()
        {
            // Act
            var actual = StringRoutines.Spaces(5);

            // Act
            Assert.AreEqual("     ", actual);
        }

        #endregion Spaces

        #region CamelCaseToSpacedWords

        [TestCase("IDontWorkAtIBM", "I Dont Work At IBM")]
        public void CamelCaseToSpacedWords_with_camel_case_returns_user_readable_text(string camelCase, string expected)
        {
            //Act
            var actual = StringRoutines.CamelCaseToSpacedWords(camelCase);

            //Assert
            Assert.AreEqual(expected, actual);
        }

        #endregion CamelCaseToSpacedWords
    }
}

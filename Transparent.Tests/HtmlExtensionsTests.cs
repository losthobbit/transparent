using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transparent.Tests
{
    [TestFixture]
    public class HtmlExtensionsTests
    {       
        [Test]
        public void TextToHtml_with_line_breaks_replaces_them_with_br_tags()
        {
            //Arrange
            var text = "abcd" + Environment.NewLine + "efgh";

            //Act
            var actual = HtmlExtensions.TextToHtml(null, text).ToHtmlString();

            //Assert
            Assert.AreEqual("abcd<br/>efgh", actual);
        }

        [TestCase("abc.http://google.com.", "abc.<a href=\"http://google.com\" target=\"_blank\">http://google.com</a>.")]
        [TestCase("abc.https://amazon.com ", "abc.<a href=\"https://amazon.com\" target=\"_blank\">https://amazon.com</a> ")]
        [TestCase("abc.http://facebook.com", "abc.<a href=\"http://facebook.com\" target=\"_blank\">http://facebook.com</a>")]
        [TestCase("abc.http://facebook.com\r\n", "abc.<a href=\"http://facebook.com\" target=\"_blank\">http://facebook.com</a><br/>")]
        public void TextToHtml_with_urls_replaces_them_with_hyperlink_tags(string text, string expected)
        {
            //Arrange

            //Act
            var actual = HtmlExtensions.TextToHtml(null, text).ToHtmlString();

            //Assert
            Assert.AreEqual(expected, actual);
        }
    }
}

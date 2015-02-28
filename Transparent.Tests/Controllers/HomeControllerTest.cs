using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transparent;
using Transparent.Controllers;
using Moq;
using System.Web;
using System.Web.Routing;
using Transparent.Data.Interfaces;

namespace Transparent.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        private HomeController target;
        private Mock<HttpRequestBase> mockRequest;
        private Mock<IGeneral> mockGeneral;

        [TestInitialize()]
        public void Initialize()
        {
            mockRequest = new Mock<HttpRequestBase>();
            mockGeneral = new Mock<IGeneral>();

            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(mockRequest.Object);

            target = new HomeController(mockGeneral.Object);
            target.ControllerContext = new ControllerContext(context.Object, new RouteData(), target);
        }

        #region Index

        [TestMethod]
        public void Index_with_unauthenticated_request_redirects_to_account_login()
        {
            // Act
            var result = target.Index() as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("Account", result.RouteValues["controller"]);
            Assert.AreEqual("Login", result.RouteValues["action"]);
        }

        [TestMethod]
        public void Index_with_authenticated_request_redirects_to_highest_ranked_tickets()
        {
            // Arrange
            mockRequest.SetupGet(x => x.IsAuthenticated).Returns(true);

            // Act
            var result = target.Index() as RedirectToRouteResult;

            // Assert
            Assert.AreEqual("Ticket", result.RouteValues["controller"]);
            Assert.AreEqual("HighestRanked", result.RouteValues["action"]);
        }

        #endregion Index

        [TestMethod]
        public void About()
        {
            // Act
            ViewResult result = target.About() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Contact()
        {
            // Act
            ViewResult result = target.Contact() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}

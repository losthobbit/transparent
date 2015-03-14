﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Data.Entity;

namespace Transparent
{
    using Data;
    using Data.Models;
    using WebMatrix.WebData;
    using Castle.Windsor;
    using Castle.Windsor.Installer;
    using Transparent.Windsor;
    using System.Web.Http.Dispatcher;
    using Castle.MicroKernel.Resolvers.SpecializedResolvers;
    
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication, IContainerAccessor
    {
        private static IWindsorContainer container;

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            // TODO: Add error logging
            //System.Diagnostics.Debug.WriteLine(exception);
            var httpException = exception as HttpException;
            if (httpException != null)
            {
                if (httpException.ErrorCode == 404)
                {
                    Response.Redirect("~/Home/PageNotFound");
                    return;
                }
            }
            Response.Redirect("~/Home/Error");
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            InitializeDatabase();

            BootstrapContainer();
        }

        private static void BootstrapContainer()
        {
            container = new WindsorContainer();
            container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel));

            // Just a way of loading the assembly, so that its installer is called
            Business.Windsor.Installer x;

            container.Install
            (
                FromAssembly.InThisApplication(), 
                FromAssembly.Containing<Common.Windsor.Installer>()
            );

            // Setup dependency injection of MVC controllers
            var controllerFactory = new WindsorControllerFactory(container.Kernel);
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);
        }

        private void InitializeDatabase()
        {
            Database.SetInitializer<UsersContext>(new InitDatabase<UsersContext>());
            UsersContext context = new UsersContext();
            context.Database.Initialize(false);
        }

        protected void Application_End()
        {
            container.Dispose();
        }

        public IWindsorContainer Container
        {
            get { return container; }
        }
    }
}
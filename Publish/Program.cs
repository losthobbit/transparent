using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Build.Execution;
using Microsoft.Build.Logging;
using Microsoft.Build.Framework;

namespace Publish
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var projectPath = ConfigurationManager.AppSettings["ProjectPath"];
                var publishProfilePath = ConfigurationManager.AppSettings["PublishProfilePath"];
                var newConnectionString = ConfigurationManager.AppSettings["ConnectionString"];

                if(args.Any(arg => arg == "deploy"))
                    Deploy(projectPath, publishProfilePath);

                Console.WriteLine("Updating configuration...");

                var deployedConfigPath = GetDeployedConfigPath(publishProfilePath);

                SetConnectionString(deployedConfigPath, newConnectionString);

                Console.WriteLine("Configuration updated.");
            }
            catch(Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }

            // If it's running in the IDE...
            if(Debugger.IsAttached)
            {
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectPath"></param>
        /// <param name="publishProfilePath"></param>
        /// <remarks>
        /// This doesn't work... but there's no pressing need to fix it.
        /// </remarks>
        private static void Deploy(string projectPath, string publishProfilePath)
        {
            Console.WriteLine("Deploying...");

            //Process.Start("msbuild", solutionPath + " /p:DeployOnBuild=true"
            //    + " /p:PublishProfile " + publishProfilePath);

            var globalProperties = new Dictionary<string, string>
            {
                // http://msdn.microsoft.com/en-gb/library/ms165431.aspx
                { "PublishProfile", publishProfilePath }
            };
            var projectInstance = new ProjectInstance(projectPath, globalProperties, null);

            var logger = new ConsoleLogger(LoggerVerbosity.Normal);
            var buildManager = BuildManager.DefaultBuildManager;
            var buildParameters = new BuildParameters
            { 
                Loggers = new List<ILogger> { logger }
            };
            var buildRequestData = new BuildRequestData(projectInstance, new[] { "Publish" });
            var buildResult = buildManager.Build(buildParameters, buildRequestData);

            Console.WriteLine("Deployed.");
        }

        private static void SetConnectionString(string deployedConfigPath, string newConnectionString)
        {
            var configFile = XDocument.Load(deployedConfigPath);
            var configurationElement = configFile.Root;
            var connectionStringsElement = configurationElement.Element("connectionStrings");
            var defaultConnectionElement = connectionStringsElement.Elements("add")
                .Single(element => element.Attribute("name").Value == "DefaultConnection");
            defaultConnectionElement.Attribute("connectionString").Value = newConnectionString;
            configFile.Save(deployedConfigPath);
        }

        private static string GetDeployedConfigPath(string publishProfilePath)
        {
            var publishProfile = XDocument.Load(publishProfilePath);
            var projectElement = publishProfile.Root;
            var propertyGroupElement = projectElement.Elements()
                .Single(element => element.Name.LocalName == "PropertyGroup");
            var publishUrlElement = propertyGroupElement.Elements()
                .Single(element => element.Name.LocalName == "publishUrl");
            var publishPath = publishUrlElement.Value;

            return Path.Combine(publishPath, "web.config");
        }
    }
}

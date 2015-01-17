using Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace Transparent.Services
{
    public class Configuration : IConfiguration
    {
        public string GetValue(string key)
        {
            return WebConfigurationManager.AppSettings[key];
        }
    }
}
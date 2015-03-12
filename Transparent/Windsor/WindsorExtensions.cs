using Castle.MicroKernel;
using Castle.MicroKernel.ComponentActivator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Transparent.Windsor
{
    public static class WindsorExtensions
    {   
        /// <summary>
        /// From http://weblogs.asp.net/psteele/using-windsor-to-inject-dependencies-into-asp-net-mvc-actionfilters
        /// </summary>        
        /// <exception cref="ComponentActivatorException">Unable to inject value into property.</exception>
        public static void InjectProperties(this IKernel kernel, object target)
        {
            var type = target.GetType();
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (property.CanWrite && kernel.HasComponent(property.PropertyType))
                {
                    var value = kernel.Resolve(property.PropertyType);
                    try
                    {
                        property.SetValue(target, value, null);
                    }
                    catch (Exception ex)
                    {
                        var message = string.Format("Error setting property {0} on type {1}, See inner exception for more information.", property.Name, type.FullName);
                        throw new ComponentActivatorException(message, ex, null);
                    }
                }
            }
        }
    }
}
using Castle.Windsor;
using System.Web;
using System.Web.Mvc;
using Transparent.Data.Interfaces;
using Transparent.Filters;

namespace Transparent
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new PeriodicFilterAttribute());
        }
    }
}
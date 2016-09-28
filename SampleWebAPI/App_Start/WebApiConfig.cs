using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace SampleWebAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "GetUsersLists",
                routeTemplate: "api/{controller}/{action}/{id}",
                //routeTemplate: "api/{controller}/{action}/{sidx}/{sord}/{page}/{rows}",
                defaults: new {id = RouteParameter.Optional }
                //defaults: new { action = "GetUsersLists",
                //    sidx = RouteParameter.Optional, 
                //    sord = RouteParameter.Optional, 
                //    page = RouteParameter.Optional, 
                //    rows = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Controllers with Actions
            // To handle routes like `/api/VTRouting/route`
            
            
        }
    }
}

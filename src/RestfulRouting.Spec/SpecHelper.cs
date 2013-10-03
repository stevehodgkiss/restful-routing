using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using Machine.Specifications;

namespace RestfulRouting.Spec
{
    public static class SpecExtensions
    {
        public static RouteData WithFormat(this RouteData routeData, string format)
        {
            routeData.Values["format"].ShouldEqual(format);
            return routeData;
        }

        public static RouteData WithName(this RouteData routeData, string name)
        {
            ((NamedRoute) routeData.Route).Name.ShouldEqual(name);
            return routeData;
        }

        public static Route ForAction(this RouteCollection routes, string controller, string action)
        {
            var matches = routes.OfType<Route>().Where(r => 
                controller.Equals((string)r.Defaults["controller"], StringComparison.InvariantCultureIgnoreCase) 
                && action.Equals((string)r.Defaults["action"], StringComparison.InvariantCultureIgnoreCase ))
                .ToList();
                
            matches.Count.ShouldEqual(1);
            return matches.First();
        }

        public static IEnumerable<Route> ForController(this RouteCollection routes, string controller)
        {
            return routes.OfType<Route>().Where(r =>
                controller.Equals((string)r.Defaults["controller"], StringComparison.InvariantCultureIgnoreCase));
        }

        public static T As<T>(this object value) where T : class
        {
            return value as T;
        }
    }
}

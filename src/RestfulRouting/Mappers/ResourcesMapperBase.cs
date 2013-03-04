using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using RestfulRouting.Exceptions;

namespace RestfulRouting.Mappers
{
    public interface IResourcesMapperBase : IMapper
    {
        void As(string resourceName);
        void Except(params string[] actions);
        void Only(params string[] actions);
        void WithFormatRoutes();
        void PathNames(Action<RouteNames> action);
        void ReRoute(Action<RoutePaths> action);
    }

    public class ResourcesMapperBase<TController> : Mapper, IResourcesMapperBase where TController : Controller
    {
        protected string ResourceName;
        protected string ResourcePath;
        protected string ControllerName;
        protected RouteNames Names = new RouteNames();
        protected RoutePaths Paths = new RoutePaths();
        protected Dictionary<string, Func<Route>> IncludedActions;
        protected bool GenerateFormatRoutes;
        protected string SingularResourceName;
        protected string PluralResourceName;

        public ResourcesMapperBase()
        {
            ControllerName = GetControllerName<TController>();
            PluralResourceName = Inflector.Pluralize(ControllerName);
            SingularResourceName = Inflector.Singularize(PluralResourceName);
            As(PluralResourceName);
        }

        public void As(string resourceName)
        {
            ResourceName = resourceName;
            CalculatePath();
        }

        public void Except(params string[] actions)
        {
            foreach (var action in actions)
            {
                IncludedActions.Remove(action);
            }
        }

        public void Only(params string[] actions)
        {
            if (actions.Any(action => !IncludedActions.ContainsKey(action)))
                throw new InvalidRestfulMethodException(GetControllerName<TController>(), IncludedActions.Keys.ToArray());

            IncludedActions = IncludedActions.Where(a => actions.Contains(a.Key, StringComparer.OrdinalIgnoreCase))
                .ToDictionary(k => k.Key, pair => pair.Value, StringComparer.OrdinalIgnoreCase);
        }

        public void WithFormatRoutes()
        {
            GenerateFormatRoutes = true;
        }

        public void PathNames(Action<RouteNames> action)
        {
            action(Names);
        }

        public void ReRoute(Action<RoutePaths> action)
        {
            action(Paths);
        }

        private void CalculatePath()
        {
            ResourcePath = Join(BasePath, ResourceName);
        }

        protected override void SetBasePath(string basePath)
        {
            base.SetBasePath(basePath);
            CalculatePath();
        }

        protected NamedRoute GenerateNamedRoute(string name, string path, string controller, string action, string[] httpMethods)
        {
            return new NamedRoute(name, path,
                             new RouteValueDictionary(new { controller, action }),
                             new RouteValueDictionary(new { httpMethod = new RestfulHttpMethodConstraint(httpMethods) }),
                             new RouteValueDictionary(new { Namespaces = new[] { typeof(TController).Namespace} }),
                             RouteHandler);
        }

        protected Route GenerateRoute(string path, string controller, string action, string[] httpMethods)
        {
            return new Route(path,
                             new RouteValueDictionary(new { controller, action }),
                             new RouteValueDictionary(new { httpMethod = new RestfulHttpMethodConstraint(httpMethods) }),
                             new RouteValueDictionary(new { Namespaces = new [] { typeof(TController).Namespace } }),
                             RouteHandler);
        }

        protected void AddIncludedActions(List<Route> routes)
        {
            routes.AddRange(IncludedActions.Select(x => x.Value.Invoke()).ToArray());
        }

        protected void AddFormatRoutes(List<Route> routes)
        {
            var formatRoutes = routes.Select(WithFormatExtension).ToList();
            routes.InsertRange(0, formatRoutes);
        }

        protected Route WithFormatExtension(Route implicitRoute)
        {
            var namedRoute = implicitRoute as NamedRoute;
            if (namedRoute != null)
            {
                var explicitRoute = new NamedRoute("formatted_" + namedRoute.Name, implicitRoute.Url + ".{format}", implicitRoute.RouteHandler)
                {
                    Constraints = new RouteValueDictionary(implicitRoute.Constraints ?? new RouteValueDictionary()),
                    DataTokens = new RouteValueDictionary(implicitRoute.DataTokens ?? new RouteValueDictionary()),
                    Defaults = new RouteValueDictionary(implicitRoute.Defaults ?? new RouteValueDictionary())
                };

                explicitRoute.Constraints.Add("format", @"[A-Za-z0-9]+");

                return explicitRoute;
            }
            else
            {
                var explicitRoute = new Route(implicitRoute.Url + ".{format}", implicitRoute.RouteHandler)
                {
                    Constraints = new RouteValueDictionary(implicitRoute.Constraints ?? new RouteValueDictionary()),
                    DataTokens = new RouteValueDictionary(implicitRoute.DataTokens ?? new RouteValueDictionary()),
                    Defaults = new RouteValueDictionary(implicitRoute.Defaults ?? new RouteValueDictionary())
                };

                explicitRoute.Constraints.Add("format", @"[A-Za-z0-9]+");

                return explicitRoute;
            }
            
        }

        public void Constrain(string key, object value)
        {
            Constraints[key] = value;
        }
    }
}
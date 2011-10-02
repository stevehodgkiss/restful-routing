using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace RestfulRouting.Mappers
{
    public interface IResourceMapper<TController> : IResourcesMapperBase where TController : Controller
    {
        void Member(Action<AdditionalAction> action);
    }

    public class ResourceMapper<TController> : ResourcesMapperBase<TController>, IResourceMapper<TController> where TController : Controller
    {
        Action<ResourceMapper<TController>> _subMapper;
        Dictionary<string, KeyValuePair<string, HttpVerbs[]>> _members = new Dictionary<string, KeyValuePair<string, HttpVerbs[]>>(StringComparer.OrdinalIgnoreCase);

        public ResourceMapper(Action<ResourceMapper<TController>> subMapper = null)
        {
            As(SingularResourceName);
            IncludedActions = new Dictionary<string, Func<Route>>(StringComparer.OrdinalIgnoreCase)
                                  {
                                      {Names.ShowName, () => GenerateNamedRoute(JoinResources(ResourceName), ResourcePath, ControllerName, Names.ShowName, new[] { "GET" })},
                                      {Names.UpdateName, () => GenerateRoute(ResourcePath, ControllerName, Names.UpdateName, new[] { "PUT" })},
                                      {Names.NewName, () => GenerateNamedRoute("new_" + JoinResources(ResourceName), ResourcePath + "/" + (RouteSet.LowercaseUrls ? Names.NewName.ToLowerInvariant() : Names.NewName), ControllerName, Names.NewName, new[] { "GET" })},
                                      {Names.EditName, () => GenerateNamedRoute("edit_" + JoinResources(ResourceName), ResourcePath + "/" + (RouteSet.LowercaseUrls ? Names.EditName.ToLowerInvariant() : Names.EditName), ControllerName, Names.EditName, new[] { "GET" })},
                                      {Names.DestroyName, () => GenerateRoute(ResourcePath, ControllerName, Names.DestroyName, new[] { "DELETE" })},
                                      {Names.CreateName, () => GenerateRoute(ResourcePath, ControllerName, Names.CreateName, new[] { "POST" })}
                                  };
            if (RouteSet.MapDelete)
            {
                IncludedActions.Add(Names.DeleteName, () => GenerateNamedRoute("delete_" + JoinResources(ResourceName), ResourcePath + "/" + (RouteSet.LowercaseUrls ? Names.DeleteName.ToLowerInvariant() : Names.DeleteName), ControllerName, Names.DeleteName, new[] { "GET" }));
            }
            _subMapper = subMapper;
        }

        public void Member(Action<AdditionalAction> action)
        {
            var additionalAction = new AdditionalAction(_members);
            action(additionalAction);
        }

        private Route MemberRoute(string action, string resource, params HttpVerbs[] methods)
        {
            if (methods.Length == 0)
                methods = new[] { HttpVerbs.Get };

            return GenerateRoute(ResourcePath + "/" + resource, ControllerName, action, methods.Select(x => x.ToString().ToUpperInvariant()).ToArray());
        }

        public override void RegisterRoutes(RouteCollection routeCollection)
        {
            if (_subMapper != null)
            {
                _subMapper.Invoke(this);
            }

            var routes = new List<Route>();

            AddIncludedActions(routes);

            routes.AddRange(_members.Select(member => MemberRoute(member.Key, member.Value.Key, member.Value.Value)));

            if (GenerateFormatRoutes)
                AddFormatRoutes(routes);

            foreach (var route in routes)
            {
                ConfigureRoute(route);
                routeCollection.Add(route);
            }

            if (Mappers.Any())
            {
                BasePath = ResourcePath;

                AddResourcePath(SingularResourceName);
                RegisterNested(routeCollection, mapper => mapper.SetParentResources(ResourcePaths));
            }
        }
    }
}

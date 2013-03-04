using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using RestfulRouting.Exceptions;

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
                                      {Names.ShowName, () => GenerateNamedRoute(JoinResources(ResourceName), BuildPathFor(Paths.Show), ControllerName, Names.ShowName, new[] { "GET" })},
                                      {Names.UpdateName, () => GenerateRoute(BuildPathFor(Paths.Update), ControllerName, Names.UpdateName, new[] { "PUT" })},
                                      {Names.NewName, () => GenerateNamedRoute("new_" + JoinResources(ResourceName), BuildPathFor(Paths.New), ControllerName, Names.NewName, new[] { "GET" })},
                                      {Names.EditName, () => GenerateNamedRoute("edit_" + JoinResources(ResourceName), BuildPathFor(Paths.Edit), ControllerName, Names.EditName, new[] { "GET" })},
                                      {Names.DestroyName, () => GenerateRoute(BuildPathFor(Paths.Destroy), ControllerName, Names.DestroyName, new[] { "DELETE" })},
                                      {Names.CreateName, () => GenerateRoute(BuildPathFor(Paths.Create), ControllerName, Names.CreateName, new[] { "POST" })}
                                  };
            if (RouteSet.MapDelete)
            {
                IncludedActions.Add(Names.DeleteName, () => GenerateNamedRoute("delete_" + JoinResources(ResourceName),BuildPathFor(Paths.Delete), ControllerName, Names.DeleteName, new[] { "GET" }));
            }
            _subMapper = subMapper;
        }

        private string BuildPathFor(string path)
        {
            return path.Replace("{resourcePath}", ResourcePath)
                        .Replace("{indexName}", ProperCaseUrl(Names.IndexName))
                        .Replace("{showName}", ProperCaseUrl(Names.ShowName))
                        .Replace("{newName}", ProperCaseUrl(Names.NewName))
                        .Replace("{createName}", ProperCaseUrl(Names.CreateName))
                        .Replace("{editName}", ProperCaseUrl(Names.EditName))
                        .Replace("{updateName}", ProperCaseUrl(Names.UpdateName))
                        .Replace("{deleteName}", ProperCaseUrl(Names.DeleteName))
                        .Replace("{destroyName}", ProperCaseUrl(Names.DestroyName));
        }

        private string ProperCaseUrl(string url)
        {
            return RouteSet.LowercaseUrls
                       ? url.ToLowerInvariant()
                       : url;
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

    public class RoutePaths
    {
        public RoutePaths()
        {
            InitializeDefaults();
        }

        private void InitializeDefaults()
        {
            Index = "{resourcePath}/{indexName}";
            Show = "{resourcePath}";
            New = "{resourcePath}/{newName}";
            Create = "{resourcePath}";
            Edit = "{resourcePath}/{editName}";
            Update = "{resourcePath}";
            Delete = "{resourcePath}/deleteName";
            Destroy = "{resourcePath}";
        }

        public string Index { get; set; }

        public string Show { get; set; }

        public string New { get; set; }

        public string Create { get; set; }

        public string Edit { get; set; }

        public string Update { get; set; }

        public string Delete { get; set; }

        public string Destroy { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using System.Linq;
using RestfulRouting.Exceptions;

namespace RestfulRouting.Mappers
{
    public interface IResourcesMapper<TController> : IResourcesMapperBase where TController : Controller
    {
        void Collection(Action<AdditionalAction> action);
        void Member(Action<AdditionalAction> action);
        void Constrain(string key, object value);
    }

    public class ResourcesMapper<TController> : ResourcesMapperBase<TController>, IResourcesMapper<TController> where TController : Controller
    {
        private Action<ResourcesMapper<TController>> subMapper;
        private Dictionary<string, KeyValuePair<string, HttpVerbs[]>> collections = new Dictionary<string, KeyValuePair<string, HttpVerbs[]>>(StringComparer.OrdinalIgnoreCase);
        private Dictionary<string, KeyValuePair<string, HttpVerbs[]>> members = new Dictionary<string, KeyValuePair<string, HttpVerbs[]>>(StringComparer.OrdinalIgnoreCase);
        private readonly ResourcesRoutePaths _resourcesRoutePaths = new ResourcesRoutePaths();

        protected override RoutePaths Paths
        {
            get { return _resourcesRoutePaths; }
        }

        public ResourcesMapper(Action<ResourcesMapper<TController>> subMapper = null)
        {
            IncludedActions = new Dictionary<string, Func<Route>>(StringComparer.OrdinalIgnoreCase)
                                  {
                                      {Names.IndexName, () => GenerateNamedRoute(JoinResources(ResourceName), BuildPathFor(Paths.Index) , ControllerName, Names.IndexName, new[] { "GET" })},
                                      {Names.CreateName, () => GenerateRoute(BuildPathFor(Paths.Create), ControllerName, Names.CreateName, new[] { "POST" })},
                                      {Names.NewName, () => GenerateNamedRoute("new_" + JoinResources(SingularResourceName), BuildPathFor(Paths.New), ControllerName, Names.NewName, new[] { "GET" })},
                                      {Names.EditName, () => GenerateNamedRoute("edit_" + JoinResources(SingularResourceName), BuildPathFor(Paths.Edit), ControllerName, Names.EditName, new[] { "GET" })},
                                      {Names.ShowName, () => GenerateNamedRoute(JoinResources(SingularResourceName), BuildPathFor(Paths.Show), ControllerName, Names.ShowName, new[] { "GET" })},
                                      {Names.UpdateName, () => GenerateRoute(BuildPathFor(Paths.Update), ControllerName, Names.UpdateName, new[] { "PUT" })},
                                      {Names.DestroyName, () => GenerateRoute(BuildPathFor(Paths.Destroy), ControllerName, Names.DestroyName, new[] { "DELETE" })}
                                  };
            if (RouteSet.MapDelete)
            {
                IncludedActions.Add(Names.DeleteName, () => GenerateNamedRoute("delete_" + JoinResources(ResourceName), BuildPathFor(Paths.Delete), ControllerName, Names.DeleteName, new[] { "GET" }));
            }
            this.subMapper = subMapper;
        }

        public void Collection(Action<AdditionalAction> action)
        {
            var additionalAction = new AdditionalAction(collections);
            action(additionalAction);
        }

        public void Member(Action<AdditionalAction> action)
        {
            var additionalAction = new AdditionalAction(members);
            action(additionalAction);
        }

        private Route MemberRoute(string action, string resource, params HttpVerbs[] methods)
        {
            if (methods.Length == 0)
                methods = new[] { HttpVerbs.Get };

            return GenerateRoute(ResourcePath + "/{" + IdParameterName + "}/" + resource, ControllerName, action, methods.Select(x => x.ToString().ToUpperInvariant()).ToArray());
        }

        private Route CollectionRoute(string action, string resource, params HttpVerbs[] methods)
        {
            if (methods.Length == 0)
                methods = new[] { HttpVerbs.Get };

            return GenerateRoute(ResourcePath + "/" + resource, ControllerName, action, methods.Select(x => x.ToString().ToUpperInvariant()).ToArray());
        }

        public override void RegisterRoutes(RouteCollection routeCollection)
        {
            if (subMapper != null)
            {
                subMapper.Invoke(this);
            }

            var routes = collections.Select(collection => CollectionRoute(collection.Key, collection.Value.Key, collection.Value.Value)).ToList();

            AddIncludedActions(routes);

            routes.AddRange(members.Select(member => MemberRoute(member.Key, member.Value.Key, member.Value.Value)));

            if (GenerateFormatRoutes)
                AddFormatRoutes(routes);

            foreach (var route in routes)
            {
                ConfigureRoute(route);
                routeCollection.Add(route);
            }

            if (Mappers.Any())
            {
                string parentIdParameterName = SingularResourceName + Inflector.Capitalize(IdParameterName);
                BasePath = Join(ResourcePath, "{" + parentIdParameterName + "}");
                var idConstraint = Constraints[IdParameterName];
                if (idConstraint != null)
                {
                    Constraints.Remove(IdParameterName);
                    Constraints.Add(parentIdParameterName, idConstraint);
                }

                AddResourcePath(SingularResourceName);
                RegisterNested(routeCollection, mapper => mapper.SetParentResources(ResourcePaths));
            }
        }

        
    }
}

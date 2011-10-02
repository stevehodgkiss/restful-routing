using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Machine.Specifications;
using RestfulRouting.Mappers;
using RestfulRouting.Spec.TestObjects;
using MvcContrib.TestHelper;

namespace RestfulRouting.Spec.Mappers
{
    public class ResourcesMapperBaseTester<TController> : ResourcesMapperBase<TController> where TController : Controller
    {
        public ResourcesMapperBaseTester()
        {
            base.IncludedActions = new Dictionary<string, Func<Route>>(StringComparer.OrdinalIgnoreCase)
                                  {
                                      {Names.IndexName.ToLowerInvariant(), SomeRoute},
                                      {Names.CreateName.ToLowerInvariant(), SomeRoute},
                                      {Names.NewName.ToLowerInvariant(), SomeRoute},
                                      {Names.EditName.ToLowerInvariant(), SomeRoute},
                                      {Names.ShowName.ToLowerInvariant(), SomeRoute},
                                      {Names.UpdateName.ToLowerInvariant(), SomeRoute},
                                      {Names.DestroyName.ToLowerInvariant(), SomeRoute}
                                  };
        }

        public Route SomeRoute()
        {
            return GenerateRoute(base.ResourcePath, base.ControllerName, Names.IndexName, new[] { "GET" });
        }

        public string ResourceName()
        {
            return base.ResourceName;
        }

        public string ResourcePath()
        {
            return base.ResourcePath;
        }

        public string ControllerName()
        {
            return base.ControllerName;
        }

        public Dictionary<string, Func<Route>> IncludedActions()
        {
            return base.IncludedActions;
        }

        public void SetBasePathForTest(string path)
        {
            SetBasePath(path);
        }

        public Route GenerateRouteForTesting(string path, string controller, string action, string[] httpMethods)
        {
            return GenerateRoute(path, controller, action, httpMethods);
        }

        public override void RegisterRoutes(RouteCollection routeCollection)
        {
            var routes = new List<Route>();

            AddIncludedActions(routes);

            if (GenerateFormatRoutes)
                AddFormatRoutes(routes);

            foreach (var route in routes)
            {
                routeCollection.Add(route);
            }

            if (Mappers.Any())
            {
                var singular = Inflector.Singularize(base.ResourceName);
                BasePath = Join(base.ResourcePath, "{" + singular + "Id}");

                RegisterNested(routeCollection);
            }
        }
    }

    public class resources_mapper_base : base_context
    {
        protected static ResourcesMapperBaseTester<PostsController> tester;

        Establish context = () => tester = new ResourcesMapperBaseTester<PostsController>();
    }

    public class overriding_resource_name : resources_mapper_base
    {
        Because of = () => tester.As("post");

        It sets_the_resource_name_to_post = () => tester.ResourceName().ShouldEqual("post");
    }

    public class excluding_actions_with_except : resources_mapper_base
    {
        Because of = () => tester.Except("index", "update");

        It removes_the_actions_from_included_actions = () => tester.IncludedActions().Keys.ShouldNotContain("index", "update");

        It includes_the_rest = () => tester.IncludedActions().Keys.ShouldContain("show", "create", "new", "edit", "destroy");
    }

    public class excluding_actions_with_only : resources_mapper_base
    {
        Because of = () => tester.Only("index", "update");

        It includes_the_actions = () => tester.IncludedActions().Keys.ShouldContain("index", "update");

        It removes_the_rest = () => tester.IncludedActions().Keys.ShouldNotContain("show", "create", "new", "edit", "destroy");
    }

    public class resource_path : resources_mapper_base
    {
        Because of = () => tester.SetBasePathForTest("blogs/{blogId}");

        It joins_the_base_path_with_the_resource_name = () => tester.ResourcePath().ShouldEqual("blogs/{blogId}/posts");
    }

    public class generate_route : resources_mapper_base
    {
        static Route route;

        Because of = () => route = tester.GenerateRouteForTesting("blogs/{blogId}/posts", "posts", "index", new[] {"GET"});

        It sets_the_correct_properties = () =>
                                             {
                                                 route.Url.ShouldEqual("blogs/{blogId}/posts");
                                                 route.Defaults["controller"].ShouldEqual("posts");
                                                 route.Defaults["action"].ShouldEqual("index");
                                                 ((RestfulHttpMethodConstraint)route.Constraints["httpMethod"]).AllowedMethods.ShouldContainOnly("GET");
                                             };
    }

    public class format_routes : resources_mapper_base
    {
        Because of = () =>
                         {
                             tester.WithFormatRoutes();
                             tester.RegisterRoutes(routes);
                         };

        It generates_double_the_routes = () => routes.Count.ShouldEqual(14);

        It sets_the_first_7_to_format = () => routes.Take(7).ShouldEachConformTo(x => ((Route)x).Url.Equals("posts.{format}"));

        It sets_the_last_7_to_normal = () => routes.Skip(7).Take(7).ShouldEachConformTo(x => ((Route)x).Url.Equals("posts"));
    }

    public class nested_format_routes : resources_mapper_base
    {
        Because of = () =>
                         {
                             tester.Resources<CommentsController>();
                             tester.WithFormatRoutes();
                             tester.RegisterRoutes(routes);
                         };

        It generates_correct_amount_of_routes = () => routes.Count.ShouldEqual(21);
    }

    public class changing_path_names : resources_mapper_base
    {
         Because of = () =>
                         {
                             tester.PathNames(x => x.IndexName = "latest");
                             tester.RegisterRoutes(routes);
                         };

         It takes_path_names_into_account = () => "~/posts".WithMethod(HttpVerbs.Get).ShouldMapTo<PostsController>(x => x.Latest());
    }
}

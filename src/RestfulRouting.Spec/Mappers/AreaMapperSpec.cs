using System;
using System.Web.Routing;
using Machine.Specifications;
using RestfulRouting.Mappers;
using RestfulRouting.Spec.TestObjects;
using System.Linq;
using System.Web.Mvc;
using MvcContrib.TestHelper;

namespace RestfulRouting.Spec.Mappers
{
    public class area_mapper : base_context
    {
        static AreaMapper areaMapper = new AreaMapper("test", null, typeof(PostsController).Namespace, x => x.Resources<PostsController>());

        Because of = () => areaMapper.RegisterRoutes(routes);

        static Func<RouteBase, RouteValueDictionary> DataTokens = (RouteBase x) => ((Route)x).DataTokens;

        It constrains_the_namespace = () => routes.ShouldEachConformTo(x => ((string[])DataTokens(x)["namespaces"]).Contains(typeof(PostsController).Namespace));

        It sets_the_area = () => routes.ShouldEachConformTo(x => DataTokens(x)["area"].ToString() == "test");

        It sets_namespace_fallback = () => routes.ShouldEachConformTo(x => (bool)DataTokens(x)["UseNamespaceFallback"] == false);
        
        It adds_to_the_resource_path = () => areaMapper.JoinResources("posts").ShouldEqual("test_posts");
    }

    public class area_mapper_with_path_prefix : base_context
    {
        static AreaMapper areaMapper = new AreaMapper("admin", "top/secret", null, map => map.Root<PostsController>(x => x.Index()));
        Because of = () => areaMapper.RegisterRoutes(routes);

        It uses_the_path_prefix_for_the_path = () => "~/top/secret".WithMethod(HttpVerbs.Get).ShouldMapTo<PostsController>(x => x.Index());

        It sets_the_area = () => routes.ShouldEachConformTo(x => ((Route)x).DataTokens["area"].ToString() == "admin");

        It adds_to_the_resource_path = () => areaMapper.JoinResources("posts").ShouldEqual("top/secret_posts");
    }

    public class nested_area_mapper : base_context
    {
        static AreaMapper areaMapper = new AreaMapper("top-area", "top", "TopNamespace", x =>
        {
            x.Root<AvatarsController>(avatars => avatars.Show());
            x.Area<PostsController>("nested-area", "nested", map => map.Root<PostsController>(posts => posts.Index()));
        });

        static Func<RouteBase, RouteValueDictionary> DataTokens = (RouteBase x) => ((Route)x).DataTokens;

        Because of = () => areaMapper.RegisterRoutes(routes);

        It constrains_the_namespace_for_the_top_route = () => DataTokens(routes[0])["namespaces"].As<string[]>().ShouldContain("TopNamespace");
        It constrains_the_namespace_for_the_nested_route = () => DataTokens(routes[1])["namespaces"].As<string[]>().ShouldContain(typeof(PostsController).Namespace);

        It uses_the_path_prefix_for_the_top_path = () => "~/top".WithMethod(HttpVerbs.Get).ShouldMapTo<AvatarsController>(x => x.Show());
        It uses_the_path_prefix_for_the_nested_path = () => "~/top/nested".WithMethod(HttpVerbs.Get).ShouldMapTo<PostsController>(x => x.Index());

        It sets_the_area_for_the_top_route = () => DataTokens(routes[0])["area"].ShouldEqual("top-area");
        It sets_the_area_for_the_nested_route = () => DataTokens(routes[1])["area"].ShouldEqual("nested-area");

        It adds_to_the_resource_path = () => areaMapper.JoinResources("posts").ShouldEqual("top_nested_posts");
    }
}

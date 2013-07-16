using System.Web.Mvc;
using Machine.Specifications;
using RestfulRouting.Mappers;
using RestfulRouting.Spec.TestObjects;
using MvcContrib.TestHelper;

namespace RestfulRouting.Spec.Mappers
{
    public class root_mapper : base_context
    {
        Because of = () => new RootMapper<PostsController>(x => x.Index()).RegisterRoutes(routes);

        It maps_to_posts_index = () => "~/".WithMethod(HttpVerbs.Get).ShouldMapTo<PostsController>(x => x.Index()).WithName("root");
    }

    public class root_mapper_with_base_path : base_context
    {
        Because of = () => new AreaMapper("admin", null, null, map => map.Root<PostsController>(x => x.Index())).RegisterRoutes(routes);

        It maps_to_posts_index = () => "~/admin".WithMethod(HttpVerbs.Get).ShouldMapTo<PostsController>(x => x.Index());
    }
}
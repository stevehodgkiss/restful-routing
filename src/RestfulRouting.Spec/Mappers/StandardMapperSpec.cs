using System.Web.Mvc;
using Machine.Specifications;
using MvcContrib.TestHelper;
using RestfulRouting.Mappers;
using RestfulRouting.Spec.TestObjects;

namespace RestfulRouting.Spec.Mappers
{
    public class standard_mapping : base_context
    {
        Because of = () => new StandardMapper().Path("posts/{year}/{slug}").To<PostsController>(x => x.Post(2009, "")).Named("testName").RegisterRoutes(routes);

        It should_map_posts_post = () => "~/posts/2009/test".WithMethod(HttpVerbs.Get).ShouldMapTo<PostsController>(x => x.Post(2009, "test")).WithName("testName");

        It should_generate_url = () => OutBoundUrl.Of<PostsController>(x => x.Post(2009, "test")).ShouldMapToUrl("/posts/2009/test");
    }

    public class standard_mapping_respects_base_path : base_context
    {
        Because of = () => new ResourcesMapper<PostsController>(map => map.Path("hi").To<CommentsController>(x => x.Index(2))).RegisterRoutes(routes);

        // this defaults to mapping under the member now

        It should_map_posts_post = () => "~/posts/1/hi".WithMethod(HttpVerbs.Get).ShouldMapTo<CommentsController>(x => x.Index(1));

    }

	public class standard_mapping_allows_PUT_and_DELETE_verb_overrides : base_context
	{
		static StandardMapper standardMapper;

		Establish context = () =>
			standardMapper = new StandardMapper();

		Because of = () =>
			standardMapper.Path("posts/{id}").Allow(HttpVerbs.Put);

		It should_use_a_http_method_constraint_that_allows_PUT_as_an_override_in_a_post = () =>
			standardMapper.Route.Constraints["httpMethod"].ShouldBeOfType<RestfulHttpMethodConstraint>();

	}
}

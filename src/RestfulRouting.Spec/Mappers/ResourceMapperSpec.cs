using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Machine.Specifications;
using MvcContrib.TestHelper;
using RestfulRouting.Mappers;
using RestfulRouting.Spec.TestObjects;

namespace RestfulRouting.Spec.Mappers
{
    [Behaviors]
    public class SessionsResource
    {
        It should_map_get_show = () => "~/session".WithMethod(HttpVerbs.Get).ShouldMapTo<SessionsController>(x => x.Show()).WithName("session");

        It should_map_get_new = () => "~/session/new".WithMethod(HttpVerbs.Get).ShouldMapTo<SessionsController>(x => x.New()).WithName("new_session");

        It should_map_post_create = () => "~/session".WithMethod(HttpVerbs.Post).ShouldMapTo<SessionsController>(x => x.Create());

        It should_map_get_edit = () => "~/session/edit".WithMethod(HttpVerbs.Get).ShouldMapTo<SessionsController>(x => x.Edit()).WithName("edit_session");

        It should_map_put_update = () => "~/session".WithMethod(HttpVerbs.Put).ShouldMapTo<SessionsController>(x => x.Update());

        It should_map_delete_destroy = () => "~/session".WithMethod(HttpVerbs.Delete).ShouldMapTo<SessionsController>(x => x.Destroy());


        It should_generate_show = () => OutBoundUrl.Of<SessionsController>(x => x.Show()).ShouldMapToUrl("/session");

        It should_generate_new = () => OutBoundUrl.Of<SessionsController>(x => x.New()).ShouldMapToUrl("/session/new");

        It should_generate_create = () => OutBoundUrl.Of<SessionsController>(x => x.Create()).ShouldMapToUrl("/session");

        It should_generate_edit = () => OutBoundUrl.Of<SessionsController>(x => x.Edit()).ShouldMapToUrl("/session/edit");

        It should_generate_update = () => OutBoundUrl.Of<SessionsController>(x => x.Update()).ShouldMapToUrl("/session");

        It should_generate_destroy = () => OutBoundUrl.Of<SessionsController>(x => x.Destroy()).ShouldMapToUrl("/session");
    }

    [Behaviors]
    public class NestedSessionsAvatarsResource
    {
        It should_map_get_show = () => "~/session/avatar".WithMethod(HttpVerbs.Get).ShouldMapTo<AvatarsController>(x => x.Show()).WithName("session_avatar");

        It should_map_get_new = () => "~/session/avatar/new".WithMethod(HttpVerbs.Get).ShouldMapTo<AvatarsController>(x => x.New()).WithName("new_session_avatar");

        It should_map_post_create = () => "~/session/avatar".WithMethod(HttpVerbs.Post).ShouldMapTo<AvatarsController>(x => x.Create());

        It should_map_get_edit = () => "~/session/avatar/edit".WithMethod(HttpVerbs.Get).ShouldMapTo<AvatarsController>(x => x.Edit()).WithName("edit_session_avatar");

        It should_map_put_update = () => "~/session/avatar".WithMethod(HttpVerbs.Put).ShouldMapTo<AvatarsController>(x => x.Update());

        It should_map_delete_destroy = () => "~/session/avatar".WithMethod(HttpVerbs.Delete).ShouldMapTo<AvatarsController>(x => x.Destroy());


        It should_generate_show = () => OutBoundUrl.Of<AvatarsController>(x => x.Show()).ShouldMapToUrl("/session/avatar");

        It should_generate_new = () => OutBoundUrl.Of<AvatarsController>(x => x.New()).ShouldMapToUrl("/session/avatar/new");

        It should_generate_create = () => OutBoundUrl.Of<AvatarsController>(x => x.Create()).ShouldMapToUrl("/session/avatar");

        It should_generate_edit = () => OutBoundUrl.Of<AvatarsController>(x => x.Edit()).ShouldMapToUrl("/session/avatar/edit");

        It should_generate_update = () => OutBoundUrl.Of<AvatarsController>(x => x.Update()).ShouldMapToUrl("/session/avatar");

        It should_generate_destroy = () => OutBoundUrl.Of<AvatarsController>(x => x.Destroy()).ShouldMapToUrl("/session/avatar");
    }

    [Behaviors]
    public class FormatSessionsResource
    {
        It should_map_get_show = () => "~/session.xml".WithMethod(HttpVerbs.Get).ShouldMapTo<SessionsController>(x => x.Show());

        It should_map_get_new = () => "~/session/new.xml".WithMethod(HttpVerbs.Get).ShouldMapTo<SessionsController>(x => x.New());

        It should_map_post_create = () => "~/session.xml".WithMethod(HttpVerbs.Post).ShouldMapTo<SessionsController>(x => x.Create());

        It should_map_get_edit = () => "~/session/edit.xml".WithMethod(HttpVerbs.Get).ShouldMapTo<SessionsController>(x => x.Edit());

        It should_map_put_update = () => "~/session.xml".WithMethod(HttpVerbs.Put).ShouldMapTo<SessionsController>(x => x.Update());

        It should_map_delete_destroy = () => "~/session.xml".WithMethod(HttpVerbs.Delete).ShouldMapTo<SessionsController>(x => x.Destroy());
    }

    public class mapping_resource : base_context
    {
        Because of = () => new ResourceMapper<SessionsController>().RegisterRoutes(routes);

        Behaves_like<SessionsResource> a_sessions_resource;
    }

    public class mapping_resource_with_reroute_overrides : base_context
    {
        static ResourceMapper<SessionsController> _mapper;

        Establish context = () =>
            {
                _mapper = new ResourceMapper<SessionsController>();
                _mapper.ReRoute(c => c.New = "signup");
                _mapper.ReRoute(c => c.Show = "{resourceName}/something/else");
                _mapper.ReRoute(c => c.Edit = "{resourceName}/fancy/{editName}");
            };

        Because of = () => _mapper.RegisterRoutes(routes);

        It should_map_get_show = () => "~/session/something/else".WithMethod(HttpVerbs.Get).ShouldMapTo<SessionsController>(x => x.Show()).WithName("session");

        It should_map_get_new = () => "~/signup".WithMethod(HttpVerbs.Get).ShouldMapTo<SessionsController>(x => x.New()).WithName("new_session");

        It should_map_post_create = () => "~/session".WithMethod(HttpVerbs.Post).ShouldMapTo<SessionsController>(x => x.Create());

        It should_map_get_edit = () => "~/session/fancy/edit".WithMethod(HttpVerbs.Get).ShouldMapTo<SessionsController>(x => x.Edit()).WithName("edit_session");

        It should_map_put_update = () => "~/session".WithMethod(HttpVerbs.Put).ShouldMapTo<SessionsController>(x => x.Update());

        It should_map_delete_destroy = () => "~/session".WithMethod(HttpVerbs.Delete).ShouldMapTo<SessionsController>(x => x.Destroy());

        It should_generate_show;// TODO - this fails: = () => OutBoundUrl.Of<SessionsController>(x => x.Show()).ShouldMapToUrl("/session/something/else");

        It should_generate_new = () => OutBoundUrl.Of<SessionsController>(x => x.New()).ShouldMapToUrl("/signup");

        It should_generate_create = () => OutBoundUrl.Of<SessionsController>(x => x.Create()).ShouldMapToUrl("/session");

        It should_generate_edit;// TODO - this fails: = () => OutBoundUrl.Of<SessionsController>(x => x.Edit()).ShouldMapToUrl("/session/fancy/edit");

        It should_generate_update = () => OutBoundUrl.Of<SessionsController>(x => x.Update()).ShouldMapToUrl("/session");

        It should_generate_destroy = () => OutBoundUrl.Of<SessionsController>(x => x.Destroy()).ShouldMapToUrl("/session");
    }

    public class mapping_resource_with_member_route : base_context
    {
        Because of = () => new ResourceMapper<SessionsController>(map => map.Member(x => x.Get("hello"))).RegisterRoutes(routes);

        Behaves_like<SessionsResource> a_sessions_resource;

        It maps_get_hello = () => "~/session/hello".WithMethod(HttpVerbs.Get).ShouldMapTo<SessionsController>(x => x.Hello());
    }

    public class mapping_nested_resource : base_context
    {
        Because of = () => new ResourceMapper<SessionsController>(sessions => sessions.Resource<AvatarsController>()).RegisterRoutes(routes);

        Behaves_like<SessionsResource> a_sessions_resource;

        Behaves_like<NestedSessionsAvatarsResource> a_nested_avatars_resource;
    }

    public class mapping_nested_resources : base_context
    {
        Because of = () => new ResourceMapper<SessionsController>(sessions => sessions.Resources<PostsController>()).RegisterRoutes(routes);

        It maps_index = () => "~/session/posts".WithMethod(HttpVerbs.Get).ShouldMapTo<PostsController>(x => x.Index());
    }

    public class mapping_resource_with_format_routes : base_context
    {
        Because of = () => new ResourceMapper<SessionsController>(sessions => sessions.WithFormatRoutes()).RegisterRoutes(routes);

        Behaves_like<FormatSessionsResource> a_format_sessions_resource;

        It generates_double_the_routes = () => routes.Count.ShouldEqual(12);

        It sets_the_first_7_to_format = () => routes.Take(6).ShouldEachConformTo(x => ((Route)x).Url.EndsWith(".{format}"));

        It sets_the_last_7_to_normal = () => routes.Skip(6).Take(6).ShouldEachConformTo(x => !((Route)x).Url.EndsWith("test"));
    }
}

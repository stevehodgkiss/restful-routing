using System.Web.Mvc;
using System.Web.Routing;
using Machine.Specifications;
using RestfulRouting.Mappers;
using RestfulRouting.Spec.TestObjects;
using System.Linq;
using System;
using RestfulRouting.Exceptions;

namespace RestfulRouting.Spec
{
    public class mapper_routeBase : base_context
    {
        protected static Mapper mapper;

        private Establish context = () =>
                                {
                                    mapper = new Mapper();
                                    
                                };
    }

    public class mapper_route : mapper_routeBase
    {
        private static Route route = new Route("test", new MvcRouteHandler());
        Because of = () =>
                         {
                             mapper.Route(route);
                             mapper.RegisterRoutes(routes);
                         };

        It adds_the_route = () => routes.Count.ShouldEqual(1);
    }

    public class mapper_resources : mapper_routeBase
    {
        Because of = () =>
                         {
                             mapper.Resources<PostsController>();
                             mapper.RegisterRoutes(routes);
                         };

        It adds_the_routes = () => routes.Count.ShouldEqual(7);
    }

    public class mapper_resource : mapper_routeBase
    {
        Because of = () =>
        {
            mapper.Resource<PostsController>();
            mapper.RegisterRoutes(routes);
        };

        It adds_the_routes = () => routes.Count.ShouldEqual(6);
    }

    public class mapper_set_route_handler : mapper_routeBase
    {
        Because of = () =>
                         {
                             mapper.WithRouteHandler(null);
                             mapper.Path("test").To<PostsController>(x => x.Index());
                             mapper.Resources<PostsController>();
                             mapper.Resource<SessionsController>();
                             mapper.RegisterRoutes(routes);
                         };

        It uses_the_specified_route_handler = () => routes.ShouldEachConformTo(x => ((Route)x).RouteHandler == null);
    }

    public class debug_route : mapper_routeBase
    {
        Because of = () =>
                         {
                             mapper.DebugRoute("debug");
                             mapper.RegisterRoutes(routes);
                         };

        It maps_the_debug_route = () => ((Route)routes.First()).Url.ShouldEqual("debug");
    }

    public class mapper_area_mixing_scopes : mapper_routeBase
    {
        static Exception Exception;

        Because of = () =>
        {
            mapper.Area("area", area => mapper.Resources<PostsController>());
            Exception = Catch.Exception(() => mapper.RegisterRoutes(routes));
        };
        
        It should_throw_exception = () => Exception.ShouldBeOfType<InvalidMapperConfigurationException>();
    }

    public class mapper_resource_mixing_scopes : mapper_routeBase
    {
        static Exception Exception;

        Because of = () =>
        {
            mapper.Resource<PostsController>(posts => mapper.Resource<PostsController>());
            Exception = Catch.Exception(() => mapper.RegisterRoutes(routes));
        };

        It should_throw_exception = () => Exception.ShouldBeOfType<InvalidMapperConfigurationException>();
    }

    public class mapper_resources_mixing_scopes : mapper_routeBase
    {
        static Exception Exception;

        Because of = () =>
        {
            mapper.Resources<PostsController>(posts => mapper.Resources<PostsController>());
            Exception = Catch.Exception(() => mapper.RegisterRoutes(routes));
        };

        It should_throw_exception = () => Exception.ShouldBeOfType<InvalidMapperConfigurationException>();
    }
}

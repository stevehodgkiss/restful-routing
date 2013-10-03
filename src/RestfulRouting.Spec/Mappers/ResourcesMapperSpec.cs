using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Machine.Specifications;
using MvcContrib.TestHelper;
using RestfulRouting.Mappers;
using RestfulRouting.Spec.TestObjects;
using Machine.Specifications.Utility;

namespace RestfulRouting.Spec.Mappers
{
    [Behaviors]
    public class PostsResource
    {
        It should_map_get_index = () => "~/posts".WithMethod(HttpVerbs.Get).ShouldMapTo<PostsController>(x => x.Index()).WithName("posts");

        It should_map_get_show = () => "~/posts/1".WithMethod(HttpVerbs.Get).ShouldMapTo<PostsController>(x => x.Show(1)).WithName("post");

        It should_map_get_new = () => "~/posts/new".WithMethod(HttpVerbs.Get).ShouldMapTo<PostsController>(x => x.New()).WithName("new_post");

        It should_map_post_create = () => "~/posts".WithMethod(HttpVerbs.Post).ShouldMapTo<PostsController>(x => x.Create());

        It should_map_get_edit = () => "~/posts/1/edit".WithMethod(HttpVerbs.Get).ShouldMapTo<PostsController>(x => x.Edit(1)).WithName("edit_post");

        It should_map_put_update = () => "~/posts/1".WithMethod(HttpVerbs.Put).ShouldMapTo<PostsController>(x => x.Update(1));

        It should_map_delete_destroy = () => "~/posts/1".WithMethod(HttpVerbs.Delete).ShouldMapTo<PostsController>(x => x.Destroy(1));

        It should_generate_index = () => OutBoundUrl.Of<PostsController>(x => x.Index()).ShouldMapToUrl("/posts");

        It should_generate_show = () => OutBoundUrl.Of<PostsController>(x => x.Show(1)).ShouldMapToUrl("/posts/1");

        It should_generate_new = () => OutBoundUrl.Of<PostsController>(x => x.New()).ShouldMapToUrl("/posts/new");

        It should_generate_create = () => OutBoundUrl.Of<PostsController>(x => x.Create()).ShouldMapToUrl("/posts");

        It should_generate_edit = () => OutBoundUrl.Of<PostsController>(x => x.Edit(1)).ShouldMapToUrl("/posts/1/edit");

        It should_generate_update = () => OutBoundUrl.Of<PostsController>(x => x.Update(1)).ShouldMapToUrl("/posts/1");

        It should_generate_destroy = () => OutBoundUrl.Of<PostsController>(x => x.Destroy(1)).ShouldMapToUrl("/posts/1");
    }

    [Behaviors]
    public class ProductsResource
    {
        It should_map_get_show = () => "~/products/prod123".WithMethod(HttpVerbs.Get)
            .ShouldMapTo<ProductsController>(x => x.Show("prod123"));

        It should_map_get_edit = () => "~/products/prod123/edit".WithMethod(HttpVerbs.Get)
            .ShouldMapTo<ProductsController>(x => x.Edit("prod123"));

        It should_map_put_update = () => "~/products/prod123".WithMethod(HttpVerbs.Put)
            .ShouldMapTo<ProductsController>(x => x.Update("prod123"));

        It should_map_delete_destroy = () => "~/products/prod123".WithMethod(HttpVerbs.Delete)
            .ShouldMapTo<ProductsController>(x => x.Destroy("prod123"));

        It should_generate_show = () => OutBoundUrl.Of<ProductsController>(x => x.Show("prod123"))
            .ShouldMapToUrl("/products/prod123");

        It should_generate_edit = () => OutBoundUrl.Of<ProductsController>(x => x.Edit("prod123"))
            .ShouldMapToUrl("/products/prod123/edit");

        It should_generate_update = () => OutBoundUrl.Of<ProductsController>(x => x.Update("prod123"))
            .ShouldMapToUrl("/products/prod123");

        It should_generate_destroy = () => OutBoundUrl.Of<ProductsController>(x => x.Destroy("prod123"))
            .ShouldMapToUrl("/products/prod123");
    }

    public class mapping_resources : base_context
    {
        Because of = () => new ResourcesMapper<PostsController>().RegisterRoutes(routes);

        Behaves_like<PostsResource> a_posts_resource;

        It maps_correct_number_of_routes = () => routes.Count.ShouldEqual(7);
    }

    public class mapping_resources_with_as : base_context
    {
        Because of = () => new ResourcesMapper<PostsController>(posts => posts.As("blogs")).RegisterRoutes(routes);

        It should_map_the_resource_as_blogs = () =>
                                                  {
                                                      foreach (var routeBase in routes)
                                                      {
                                                          var route = routeBase as Route;
                                                          route.Url.ShouldStartWith("blogs");
                                                      }
                                                  };
    }

    public class mapping_resources_with_custom_id_parameter : base_context
    {
        Because of = () => new ResourcesMapper<ProductsController>(products =>
        {
            products.IdParameter("code");
            products.Member(x => x.Get("hello"));
        }).RegisterRoutes(routes);

        Behaves_like<ProductsResource> a_products_resource;

        It should_use_custom_parameter_in_edit_route_url = () => routes.ForAction("products", "Edit").Url.ShouldEndWith("products/{code}/edit");

        It should_use_custom_parameter_in_show_route_url = () => routes.ForAction("products", "Show").Url.ShouldEndWith("products/{code}");

        It should_use_custom_parameter_in_update_route_url = () => routes.ForAction("products", "Update").Url.ShouldEndWith("products/{code}");

        It should_use_custom_parameter_in_destroy_route_url = () => routes.ForAction("products", "Destroy").Url.ShouldEndWith("products/{code}");

        It should_use_custom_parameter_in_member_route_url = () => routes.ForAction("products", "Hello").Url.ShouldEndWith("products/{code}/hello");
    }

    public class mapping_resources_with_constraint_on_custom_id_parameter : base_context
    {
        Because of = () => new ResourcesMapper<ProductsController>(products =>
        {
            products.IdParameter("code");
            products.Constrain("code", @"[a-z]{3,3}\d{3,3}");
        }).RegisterRoutes(routes);

        It constrains_the_id = () => "~/products/abc123".WithMethod(HttpVerbs.Get).ShouldMapTo<ProductsController>(x => x.Show("abc123"));

        It should_add_constraint_for_custom_id_parameter = () => routes.ForController("products")
            .Each(route => route.Constraints["code"].ShouldEqual(@"[a-z]{3,3}\d{3,3}"));
    }


    public class mapping_resources_with_collection_route : base_context
    {
        Because of = () => new ResourcesMapper<PostsController>(posts => posts.Collection(x => x.Get("latest"))).RegisterRoutes(routes);

        It should_map_get_latest = () => "~/posts/latest".WithMethod(HttpVerbs.Get).ShouldMapTo<PostsController>(x => x.Latest());
    }

    public class mapping_resources_with_member_route : base_context
    {
        Because of = () => new ResourcesMapper<PostsController>(posts => posts.Member(x => x.Get("hello"))).RegisterRoutes(routes);

        It should_map_get_hello = () => "~/posts/1/hello".WithMethod(HttpVerbs.Get).ShouldMapTo<PostsController>(x => x.Hello(1));
    }

    public class mapping_resources_with_except : base_context
    {
        Because of = () => new ResourcesMapper<PostsController>(posts => posts.Except("index", "update")).RegisterRoutes(routes);

        It maps_correct_number_of_routes = () => routes.Count.ShouldEqual(5);

        It should_map_get_index = () => "~/posts".WithMethod(HttpVerbs.Get).ShouldBeNull();

        It should_map_get_show = () => "~/posts/1".WithMethod(HttpVerbs.Get).ShouldMapTo<PostsController>(x => x.Show(1));

        It should_map_get_new = () => "~/posts/new".WithMethod(HttpVerbs.Get).ShouldMapTo<PostsController>(x => x.New());

        It should_map_post_create = () => "~/posts".WithMethod(HttpVerbs.Post).ShouldMapTo<PostsController>(x => x.Create());

        It should_map_get_edit = () => "~/posts/1/edit".WithMethod(HttpVerbs.Get).ShouldMapTo<PostsController>(x => x.Edit(1));

        It should_map_put_update = () => "~/posts/1".WithMethod(HttpVerbs.Put).ShouldBeNull();
    }

    public class mapping_resources_with_only : base_context
    {
        Because of = () => new ResourcesMapper<PostsController>(posts => posts.Only("index", "update")).RegisterRoutes(routes);

        It should_map_get_index = () => "~/posts".WithMethod(HttpVerbs.Get).ShouldMapTo<PostsController>(x => x.Index());

        It should_map_get_show = () => "~/posts/1".WithMethod(HttpVerbs.Get).ShouldBeNull();

        It should_map_get_new = () => "~/posts/new".WithMethod(HttpVerbs.Get).ShouldBeNull();

        It should_map_post_create = () => "~/posts".WithMethod(HttpVerbs.Post).ShouldBeNull();

        It should_map_get_edit = () => "~/posts/1/edit".WithMethod(HttpVerbs.Get).ShouldBeNull();

        It should_map_put_update = () => "~/posts/1".WithMethod(HttpVerbs.Put).ShouldMapTo<PostsController>(x => x.Update(1));

        It should_map_delete_destroy = () => "~/posts/1".WithMethod(HttpVerbs.Delete).ShouldBeNull();

    }

    [Behaviors]
    public class NestedCommentsResource
    {
        It should_map_posts_get_index = () => "~/posts/1/comments".WithMethod(HttpVerbs.Get).ShouldMapTo<CommentsController>(x => x.Index(1)).WithName("post_comments");

        It should_map_posts_get_show = () => "~/posts/1/comments/2".WithMethod(HttpVerbs.Get).ShouldMapTo<CommentsController>(x => x.Show(1, 2)).WithName("post_comment");

        It should_map_posts_get_new = () => "~/posts/1/comments/new".WithMethod(HttpVerbs.Get).ShouldMapTo<CommentsController>(x => x.New(1)).WithName("new_post_comment");

        It should_map_posts_post_create = () => "~/posts/1/comments".WithMethod(HttpVerbs.Post).ShouldMapTo<CommentsController>(x => x.Create(1));

        It should_map_posts_get_edit = () => "~/posts/1/comments/2/edit".WithMethod(HttpVerbs.Get).ShouldMapTo<CommentsController>(x => x.Edit(1, 2)).WithName("edit_post_comment");

        It should_map_posts_put_update = () => "~/posts/1/comments/2".WithMethod(HttpVerbs.Put).ShouldMapTo<CommentsController>(x => x.Update(1, 2));

        It should_map_posts_delete_destroy = () => "~/posts/1/comments/2".WithMethod(HttpVerbs.Delete).ShouldMapTo<CommentsController>(x => x.Destroy(1, 2));


        It should_generate_index = () => OutBoundUrl.Of<CommentsController>(x => x.Index(2)).ShouldMapToUrl("/posts/2/comments");

        It should_generate_show = () => OutBoundUrl.Of<CommentsController>(x => x.Show(2, 3)).ShouldMapToUrl("/posts/2/comments/3");

        It should_generate_new = () => OutBoundUrl.Of<CommentsController>(x => x.New(2)).ShouldMapToUrl("/posts/2/comments/new");

        It should_generate_create = () => OutBoundUrl.Of<CommentsController>(x => x.Create(2)).ShouldMapToUrl("/posts/2/comments");

        It should_generate_edit = () => OutBoundUrl.Of<CommentsController>(x => x.Edit(2, 3)).ShouldMapToUrl("/posts/2/comments/3/edit");

        It should_generate_update = () => OutBoundUrl.Of<CommentsController>(x => x.Update(2, 3)).ShouldMapToUrl("/posts/2/comments/3");

        It should_generate_destroy = () => OutBoundUrl.Of<CommentsController>(x => x.Destroy(2, 3)).ShouldMapToUrl("/posts/2/comments/3");
    }

    public class nested_resources : base_context
    {
        Because of = () => new ResourcesMapper<PostsController>(posts => posts.Resources<CommentsController>()).RegisterRoutes(routes);

        Behaves_like<PostsResource> a_posts_resource;

        Behaves_like<NestedCommentsResource> a_nested_comments_resource;
    }

    [Behaviors]
    public class NestedReviewsResource
    {
        It should_map_products_get_index = () => "~/products/abc123/reviews".WithMethod(HttpVerbs.Get)
                                                     .ShouldMapTo<ReviewsController>(x => x.Index("abc123")).WithName("product_reviews");

        It should_map_products_get_show = () => "~/products/abc123/reviews/2".WithMethod(HttpVerbs.Get)
                                                    .ShouldMapTo<ReviewsController>(x => x.Show("abc123", 2)).WithName("product_review");

        It should_map_products_get_new = () => "~/products/abc123/reviews/new".WithMethod(HttpVerbs.Get)
                                                   .ShouldMapTo<ReviewsController>(x => x.New("abc123")).WithName("new_product_review");

        It should_map_products_product_create = () => "~/products/abc123/reviews".WithMethod(HttpVerbs.Post)
                                                          .ShouldMapTo<ReviewsController>(x => x.Create("abc123"));

        It should_map_products_get_edit = () => "~/products/abc123/reviews/2/edit".WithMethod(HttpVerbs.Get)
                                                    .ShouldMapTo<ReviewsController>(x => x.Edit("abc123", 2)).WithName("edit_product_review");

        It should_map_products_put_update = () => "~/products/abc123/reviews/2".WithMethod(HttpVerbs.Put)
                                                      .ShouldMapTo<ReviewsController>(x => x.Update("abc123", 2));

        It should_map_products_delete_destroy = () => "~/products/abc123/reviews/2".WithMethod(HttpVerbs.Delete)
                                                          .ShouldMapTo<ReviewsController>(x => x.Destroy("abc123", 2));


        It should_generate_index = () => OutBoundUrl.Of<ReviewsController>(x => x.Index("abc123"))
                                             .ShouldMapToUrl("/products/abc123/reviews");

        It should_generate_show = () => OutBoundUrl.Of<ReviewsController>(x => x.Show("abc123", 3))
                                            .ShouldMapToUrl("/products/abc123/reviews/3");

        It should_generate_new = () => OutBoundUrl.Of<ReviewsController>(x => x.New("abc123"))
                                           .ShouldMapToUrl("/products/abc123/reviews/new");

        It should_generate_create = () => OutBoundUrl.Of<ReviewsController>(x => x.Create("abc123"))
                                              .ShouldMapToUrl("/products/abc123/reviews");

        It should_generate_edit = () => OutBoundUrl.Of<ReviewsController>(x => x.Edit("abc123", 3))
                                            .ShouldMapToUrl("/products/abc123/reviews/3/edit");

        It should_generate_update = () => OutBoundUrl.Of<ReviewsController>(x => x.Update("abc123", 3))
                                              .ShouldMapToUrl("/products/abc123/reviews/3");

        It should_generate_destroy = () => OutBoundUrl.Of<ReviewsController>(x => x.Destroy("abc123", 3))
                                               .ShouldMapToUrl("/products/abc123/reviews/3");
    }

    public class nested_resources_with_custom_id_parameter_on_parent : base_context
    {
        Because of = () => new ResourcesMapper<ProductsController>(products =>
        {
            products.IdParameter("code");
            products.Resources<ReviewsController>();
        }).RegisterRoutes(routes);

        Behaves_like<ProductsResource> a_products_resource;

        Behaves_like<NestedReviewsResource> a_nested_comments_resource;

        It should_use_custom_id_parameter_to_identify_parent_resource_in_route_urls = () => routes.ForAction("reviews", "Show").Url
            .ShouldStartWith("products/{productCode}/reviews/");

        It should_use_standard_id_parameter_to_identify_nested_resource_in_route_urls = () => routes.ForAction("reviews", "Show").Url
            .ShouldEndWith("/reviews/{id}");
    }

    public class nested_resources_with_custom_id_parameter_and_constraint_on_parent : base_context
    {
        Because of = () => new ResourcesMapper<ProductsController>(products =>
        {
            products.IdParameter("code");
            products.Constrain("code", @"[a-z]{3,3}\d{3,3}");
            products.Resources<ReviewsController>();
        }).RegisterRoutes(routes);

        It constrains_the_id = () => "~/products/abc123/reviews/1".WithMethod(HttpVerbs.Get).ShouldMapTo<ReviewsController>(x => x.Show("abc123", 1));
        It should_add_constraint_with_id_parameter_name_on_parent_resource_routes = () => routes.ForController("products")
            .Each(route => route.Constraints["code"].ShouldEqual(@"[a-z]{3,3}\d{3,3}"));

        It should_add_constraint_with_parent_id_parameter_name_on_nested_resource_routes = () => routes.ForController("reviews")
            .Each(route => route.Constraints["productCode"].ShouldEqual(@"[a-z]{3,3}\d{3,3}"));
    }

    public class deep_nested_resources : base_context
    {
        Because of = () => new ResourcesMapper<PostsController>(posts => posts.Resources<CommentsController>(comments => comments.Resources<LikesController>())).RegisterRoutes(routes);

        Behaves_like<PostsResource> a_posts_resource;

        Behaves_like<NestedCommentsResource> a_nested_comments_resource;

        It should_map_posts_get_index = () => "~/posts/1/comments/2/likes".WithMethod(HttpVerbs.Get).ShouldMapTo<LikesController>(x => x.Index(1, 2)).WithName("post_comment_likes");

        It should_map_posts_get_show = () => "~/posts/1/comments/2/likes/3".WithMethod(HttpVerbs.Get).ShouldMapTo<LikesController>(x => x.Show(1, 2, 3)).WithName("post_comment_like");

        It should_map_posts_get_new = () => "~/posts/1/comments/2/likes/new".WithMethod(HttpVerbs.Get).ShouldMapTo<LikesController>(x => x.New(1, 2)).WithName("new_post_comment_like");

        It should_map_posts_post_create = () => "~/posts/1/comments/2/likes".WithMethod(HttpVerbs.Post).ShouldMapTo<LikesController>(x => x.Create(1, 2));

        It should_map_posts_get_edit = () => "~/posts/1/comments/2/likes/3/edit".WithMethod(HttpVerbs.Get).ShouldMapTo<LikesController>(x => x.Edit(1, 2, 3)).WithName("edit_post_comment_like");

        It should_map_posts_put_update = () => "~/posts/1/comments/2/likes/3".WithMethod(HttpVerbs.Put).ShouldMapTo<LikesController>(x => x.Update(1, 2, 3));

        It should_map_posts_delete_destroy = () => "~/posts/1/comments/2/likes/3".WithMethod(HttpVerbs.Delete).ShouldMapTo<LikesController>(x => x.Destroy(1, 2, 3));


        It should_generate_index = () => OutBoundUrl.Of<LikesController>(x => x.Index(2, 2)).ShouldMapToUrl("/posts/2/comments/2/likes");

        It should_generate_show = () => OutBoundUrl.Of<LikesController>(x => x.Show(2, 2, 3)).ShouldMapToUrl("/posts/2/comments/2/likes/3");

        It should_generate_new = () => OutBoundUrl.Of<LikesController>(x => x.New(2, 2)).ShouldMapToUrl("/posts/2/comments/2/likes/new");

        It should_generate_create = () => OutBoundUrl.Of<LikesController>(x => x.Create(2, 2)).ShouldMapToUrl("/posts/2/comments/2/likes");

        It should_generate_edit = () => OutBoundUrl.Of<LikesController>(x => x.Edit(2, 2, 3)).ShouldMapToUrl("/posts/2/comments/2/likes/3/edit");

        It should_generate_update = () => OutBoundUrl.Of<LikesController>(x => x.Update(2, 2, 3)).ShouldMapToUrl("/posts/2/comments/2/likes/3");

        It should_generate_destroy = () => OutBoundUrl.Of<LikesController>(x => x.Destroy(2, 2, 3)).ShouldMapToUrl("/posts/2/comments/2/likes/3");
    }

    [Behaviors]
    public class FormatPostsResource
    {
        It should_map_get_index = () => "~/posts.json".WithMethod(HttpVerbs.Get).ShouldMapTo<PostsController>(x => x.Index()).WithFormat("json").WithName("formatted_posts");

        It should_map_get_show = () => "~/posts/1.json".WithMethod(HttpVerbs.Get).ShouldMapTo<PostsController>(x => x.Show(1)).WithFormat("json").WithName("formatted_post");

        It should_map_get_new = () => "~/posts/new.json".WithMethod(HttpVerbs.Get).ShouldMapTo<PostsController>(x => x.New()).WithFormat("json").WithName("formatted_new_post");

        It should_map_post_create = () => "~/posts.json".WithMethod(HttpVerbs.Post).ShouldMapTo<PostsController>(x => x.Create()).WithFormat("json");

        It should_map_get_edit = () => "~/posts/1/edit.json".WithMethod(HttpVerbs.Get).ShouldMapTo<PostsController>(x => x.Edit(1)).WithFormat("json").WithName("formatted_edit_post");

        It should_map_put_update = () => "~/posts/1.json".WithMethod(HttpVerbs.Put).ShouldMapTo<PostsController>(x => x.Update(1)).WithFormat("json");

        It should_map_delete_destroy = () => "~/posts/1.json".WithMethod(HttpVerbs.Delete).ShouldMapTo<PostsController>(x => x.Destroy(1)).WithFormat("json");
    }

    public class mapping_resources_with_format_routes : base_context
    {
        Because of = () => new ResourcesMapper<PostsController>(posts => posts.WithFormatRoutes()).RegisterRoutes(routes);

        Behaves_like<FormatPostsResource> a_format_posts_resource;

        It generates_double_the_routes = () => routes.Count.ShouldEqual(14);

        It sets_the_first_7_to_format = () => routes.Take(7).ShouldEachConformTo(x => ((Route)x).Url.EndsWith(".{format}"));

        It sets_the_last_7_to_normal = () => routes.Skip(7).Take(7).ShouldEachConformTo(x => !((Route)x).Url.EndsWith("test"));
    }

    public class mapping_nested_resources_with_constraints : base_context
    {
        Because of = () => new ResourcesMapper<PostsController>(posts =>
                                                                    {
                                                                        posts.Constrain("id", @"\d+");
                                                                        posts.Resources<CommentsController>(comments =>
                                                                                                                {
                                                                                                                    comments.Resources<LikesController>(x => x.Member(m => m.Get("test")));
                                                                                                                    comments.Resource<SessionsController>();
                                                                                                                });
                                                                    }).RegisterRoutes(routes);

        static Func<string, List<Route>> RoutesForController = controller => routes.Select(x => (Route)x).Where(x => x.Defaults["controller"].ToString() == controller).ToList();

        It adds_the_constraint = () =>
                            {
                                var postRoutes = routes.Select(x => (Route)x).Where(x => x.Defaults["controller"].ToString() == "posts").ToList();
                                postRoutes.Count.ShouldBeGreaterThan(1);
                                foreach (var route in postRoutes)
                                {
                                    route.Constraints["id"].ShouldEqual(@"\d+");
                                }
                            };

        It constrains_the_id = () => "~/posts/1".WithMethod(HttpVerbs.Get).ShouldMapTo<PostsController>(x => x.Show(1));

        It adds_it_for_the_comments = () =>
                                          {
                                              var commentRoutes = RoutesForController("comments");
                                              commentRoutes.Count.ShouldBeGreaterThan(1);
                                              foreach (var route in commentRoutes)
                                              {
                                                  route.Constraints["postId"].ShouldEqual(@"\d+");
                                              }
                                          };

        It adds_it_for_likes = () =>
                                   {
                                       var likesRoutes = RoutesForController("likes");
                                       likesRoutes.Count.ShouldBeGreaterThan(1);
                                       foreach (var route in likesRoutes)
                                       {
                                           route.Constraints["postId"].ShouldEqual(@"\d+");
                                       }
                                   };

        It adds_it_for_sessions = () =>
                                   {
                                       var sessionRoutes = RoutesForController("sessions");
                                       sessionRoutes.Count.ShouldBeGreaterThan(1);
                                       foreach (var route in sessionRoutes)
                                       {
                                           route.Constraints["postId"].ShouldEqual(@"\d+");
                                       }
                                   };
    }
}

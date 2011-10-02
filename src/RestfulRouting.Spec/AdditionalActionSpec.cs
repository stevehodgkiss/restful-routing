using System.Collections.Generic;
using System.Web.Mvc;
using Machine.Specifications;
using System;

namespace RestfulRouting.Spec
{
    public class additional_action_context : base_context
    {
        protected static AdditionalAction AdditionalAction;
        protected static Dictionary<string, KeyValuePair<string, HttpVerbs[]>> _actionsAndMethods;

        Establish context = () =>
        {
            _actionsAndMethods = new Dictionary<string, KeyValuePair<string, HttpVerbs[]>>(StringComparer.OrdinalIgnoreCase);
            AdditionalAction = new AdditionalAction(_actionsAndMethods);
        };
    }

    public class get_action : additional_action_context
    {
        Because of = () => AdditionalAction.Get("actionname");

        It should_map_get_actionname = () => _actionsAndMethods["actionname"].Value.ShouldEqual(new[] { HttpVerbs.Get });
    }

    public class post_action : additional_action_context
    {
        Because of = () => AdditionalAction.Post("actionname");

        It should_map_post_actionname = () => _actionsAndMethods["actionname"].Value.ShouldEqual(new[] { HttpVerbs.Post });
    }

    public class put_action : additional_action_context
    {
        Because of = () => AdditionalAction.Put("actionname");

        It should_map_post_actionname = () => _actionsAndMethods["actionname"].Value.ShouldEqual(new[] { HttpVerbs.Put });
    }

    public class delete_action : additional_action_context
    {
        Because of = () => AdditionalAction.Delete("actionname");

        It should_map_delete_actionname = () => _actionsAndMethods["actionname"].Value.ShouldEqual(new[] { HttpVerbs.Delete });
    }

    public class head_action : additional_action_context
    {
        Because of = () => AdditionalAction.Head("actionname");

        It should_map_delete_actionname = () => _actionsAndMethods["actionname"].Value.ShouldEqual(new[] { HttpVerbs.Head });
    }

    public class multiple_actions : additional_action_context
    {
        Because of = () =>
        {
            AdditionalAction.Get("actionname");
            AdditionalAction.Post("actionname");
        };

        It should_map_get_and_post = () => _actionsAndMethods["actionname"].Value.ShouldContain(HttpVerbs.Get, HttpVerbs.Post);
    }
}
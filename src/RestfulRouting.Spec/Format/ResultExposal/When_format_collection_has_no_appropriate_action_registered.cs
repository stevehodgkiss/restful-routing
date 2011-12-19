using System;
using System.Web.Mvc;
using Machine.Specifications;
using RestfulRouting.Exceptions;
using RestfulRouting.Format;
using RestfulRouting.Format.ActionResultExposal;

namespace RestfulRouting.Spec.Format.ResultExposal
{
    [Subject(typeof(ActionResultExposer))]
    public class When_format_collection_has_no_appropriate_action_registered
    {
        static ActionResultExposer exposer;

        static Exception exception;

        Establish context = () => {
            var formats = new FormatCollection {
                Json = () => new JsonResult()
            };
            exposer = new ActionResultExposer(formats);
        };

        Because of = () => exception = Catch.Exception(() => exposer.Html());

        It should_throw_NotRegisteredFormatException = () => exception.ShouldBeOfType<NotRegisteredFormatException>();        
    }
}
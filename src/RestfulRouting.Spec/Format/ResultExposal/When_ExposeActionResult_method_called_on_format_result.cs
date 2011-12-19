using System.Web.Mvc;
using Machine.Specifications;
using RestfulRouting.Format;

namespace RestfulRouting.Spec.Format.ResultExposal
{
    [Subject(typeof(FormatResult))]
    public class When_ExposeActionResult_method_called_on_format_result
    {
        static FormatResult formatResult;
        static ActionResult returnedHtmlActionResult;
        static ActionResult returnedJsonActionResult;
        static ActionResult returnedJsActionResult;
        static ActionResult returnedCsvResult;
        static ActionResult returnedXmlActionResult;

        Establish context = () => {
            formatResult = new FormatResult(format => {
                format.Html = () => new ViewResult();
                format.Json = () => new JsonResult();
                format.Js = () => new JavaScriptResult();
                format.Xml = () => new ContentResult();
                format.Csv = () => new PartialViewResult();
            });
        };

        Because of = () => {
            returnedHtmlActionResult = formatResult.ExposeResult().Html();
            returnedJsonActionResult = formatResult.ExposeResult().Json();
            returnedJsActionResult = formatResult.ExposeResult().Js();
            returnedXmlActionResult = formatResult.ExposeResult().Xml();
            returnedCsvResult = formatResult.ExposeResult().Csv();
        };

        It should_return_associated_with_json_result = () => returnedHtmlActionResult.ShouldBeOfType<ViewResult>();

        It should_return_associated_with_html_result = () => returnedJsonActionResult.ShouldBeOfType<JsonResult>();

        It should_return_associated_with_js_result = () => returnedJsActionResult.ShouldBeOfType<JavaScriptResult>();

        It should_return_associated_with_xml_result = () => returnedXmlActionResult.ShouldBeOfType<ContentResult>();

        It should_return_associated_with_csv_result = () => returnedCsvResult.ShouldBeOfType<PartialViewResult>();
    }
}
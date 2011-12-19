using System;
using System.Web.Mvc;
using System.Web.Routing;
using System.Linq;
using RestfulRouting.Format.ActionResultExposal;

namespace RestfulRouting.Format
{
    public class FormatResult : ActionResult
    {
        public static MimeTypeList MimeTypes;
        static FormatResult()
        {
            MimeTypes = new MimeTypeList();
            MimeTypes.InitializeDefaults();
        }

        Action<FormatCollection> _format;
        FormatCollection _formatCollection = new FormatCollection();

        public FormatResult(Action<FormatCollection> format)
        {
            _format = format;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            _format(this._formatCollection);
            var result = GetResult(this._formatCollection, context.RouteData.Values, context.HttpContext.Request.AcceptTypes);
            result.ExecuteResult(context);
        }

        public static ActionResult GetResult(FormatCollection formatCollection, RouteValueDictionary routeValues, string[] acceptTypes)
        {
            var format = "html"; // default to html if no format extension is specified
            if (routeValues["format"] == null)
            {
                if (acceptTypes.Any())
                {
                    format = GetFormat(formatCollection, acceptTypes);
                    if (string.IsNullOrEmpty(format))
                    {
                        if (formatCollection.Default != null)
                            return formatCollection.Default;
                        if (formatCollection["html"] != null && acceptTypes.Length == 1 && acceptTypes.First() == "*/*")
                            return formatCollection["html"].Invoke();
                        return new HttpStatusCodeResult(406);
                    }
                }
                else if (!formatCollection.Any())
                {
                    return new HttpStatusCodeResult(406);
                }
            }
            else
            {
                format = routeValues["format"].ToString();
            }

            if (!formatCollection.ContainsKey(format))
            {
                if (formatCollection.Default != null)
                    return formatCollection.Default;
                return new HttpNotFoundResult();
            }

            return formatCollection[format].Invoke();
        }

        public static string GetFormat(FormatCollection formatCollection, string[] acceptTypes)
        {
            foreach (var mimeType in MimeTypes.Parse(acceptTypes))
            {
                foreach (var key in formatCollection.Keys)
                {
                    if (mimeType.Format == key)
                        return key;
                }
            }
            return null;
        }

        /// <summary>
        /// Exposes the action result for unit testing.
        /// </summary>
        /// <returns>
        /// ActionResultExposer intance to call required actions on.
        /// </returns>
        /// <example>
        ///     FormatResult formatResult = 
        ///         new FormatResult(format => format.Json = () => new JsonResult());
        ///     JsonResult jsonResult = (JsonResult)formatResult.ExposeResult().Json();
        /// </example>
        public ActionResultExposer ExposeResult()
        {
            _format(_formatCollection);
            return new ActionResultExposer(_formatCollection);
        }
    }
}
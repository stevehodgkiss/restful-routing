using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using RazorEngine;

namespace RestfulRouting.RouteDebug
{
    public class RouteDebugController : Controller
    {
        public class RouteDebugViewModel
        {
            public string DebugPath { get; set; }
            public IList<RouteInfo> RouteInfos { get; set; }

            public string GetPath(string name)
            {
                return DebugPath + name;
            }
        }

        public class RouteInfo
        {
            public int Position { get; set; }
            public string HttpMethod { get; set; }
            public string Path { get; set; }
            public string Endpoint { get; set; }
            public string Area { get; set; }
            public string Namespaces { get; set; }
            public string Name { get; set; }
        }

        public ActionResult Index()
        {
            var model = new RouteDebugViewModel { RouteInfos = new List<RouteInfo>() };
            int position = 1;
            foreach (var route in RouteTable.Routes.Select(x => x as Route).Where(x => x != null))
            {
				// issue: #33 Fix
                var httpMethodConstraint = (route.Constraints ?? new RouteValueDictionary())["httpMethod"] as HttpMethodConstraint;

                ICollection<string> allowedMethods = new string[] { };
                if (httpMethodConstraint != null)
                {
                    allowedMethods = httpMethodConstraint.AllowedMethods;
                }

                var namespaces = new string[] { };
                if (route.DataTokens != null && route.DataTokens["namespaces"] != null)
                    namespaces = route.DataTokens["namespaces"] as string[];
                var defaults = new RouteValueDictionary();
                if (route.Defaults != null)
                    defaults = route.Defaults;
                if (route.DataTokens == null)
                    route.DataTokens = new RouteValueDictionary();

                var namedRoute = route as NamedRoute;
                var routeName = "";
                if (namedRoute != null)
                {
                    routeName = namedRoute.Name;
                }

                model.RouteInfos.Add(new RouteInfo
                {
                    Position = position,
                    HttpMethod = string.Join(" ", allowedMethods.ToArray()),
                    Path = route.Url,
                    Endpoint = defaults["controller"] + "#" + defaults["action"],
                    Area = route.DataTokens["area"] as string,
                    Namespaces = string.Join(" ", namespaces.ToArray()),
                    Name = routeName
                });
                position++;
            }

            var debugPath = (from p in model.RouteInfos
                             where p.Endpoint.Equals("routedebug#resources", StringComparison.InvariantCultureIgnoreCase)
                             select p.Path.Replace("{name}", string.Empty)).FirstOrDefault();
            model.DebugPath = debugPath;

            var template = GetTemplate();
            return Content(Razor.Parse(template, model));
        }
        public ActionResult Resources(string name)
        {
            try
            {
                var content = GetContent(name);
                var contentType = "text/html";

                var extension = Path.GetExtension(name).ToLowerInvariant();

                switch (extension) {
                    case ".css":
                        contentType = "text/css";
                        break;
                    case ".jpg":
                        contentType = "image/jpeg";
                        break;
                    case ".png":
                        contentType = "image/png";
                        break;
                    case ".js":
                        contentType = "text/javascript";
                        break;
                    case ".ttf":
                        contentType = "application/x-font-ttf";
                        break;
                    case ".woff":
                        contentType = "application/x-woff";
                        break;
                    default:
                        break;
                }

                if (string.IsNullOrWhiteSpace(content) || extension == ".cshtml")
                    return HttpNotFound();

                return Content(content, contentType, Encoding.UTF8);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        private string GetTemplate()
        {
            using (var reader = new StreamReader(GetType().Assembly.GetManifestResourceStream("RestfulRouting.RouteDebug.Content.Index.cshtml"))) {
                return reader.ReadToEnd();
            }
        }

        private string GetContent(string name) {
            try {
                using (var reader = new StreamReader(GetType().Assembly.GetManifestResourceStream(string.Format("RestfulRouting.RouteDebug.Content.{0}", name))))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }
    }
}
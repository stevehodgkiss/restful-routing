using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Mvc;

namespace RestfulRouting.Format
{
    [DebuggerDisplay("Formats = {string.Join(\", \", Keys)}")]
    public class FormatCollection : Dictionary<string, Func<ActionResult>>
    {
        public ActionResult Default { get; set; }

        public Func<ActionResult> Html { set { this[HtmlKey] = value; } }

        public Func<ActionResult> Xml { set { this[XmlKey] = value; } }

        public Func<ActionResult> Json { set { this[JsonKey] = value; } }

        public Func<ActionResult> Js { set { this[JsKey] = value; } }

        public Func<ActionResult> Csv { set { this[CsvKey] = value; } }

        internal const string HtmlKey = "html";
        internal const string XmlKey = "xml";
        internal const string JsonKey = "json";
        internal const string JsKey = "js";
        internal const string CsvKey = "csv";
    }
}
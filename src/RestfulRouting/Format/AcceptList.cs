using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace RestfulRouting.Format
{
    public class AcceptList : List<AcceptType>
    {
        private readonly MimeTypeList _types;

        public AcceptList(MimeTypeList types, params string[] acceptTypes)
        {
            _types = types;
            int order = 0;
            foreach (var acceptType in acceptTypes)
            {
                Add(acceptType, ref order);
            }
            CustomSort();
        }

        void Swap(int indexA, int indexB)
        {
            AcceptType tmp = this[indexA];
            this[indexA] = this[indexB];
            this[indexB] = tmp;
        }

        private void CustomSort()
        {
            Sort();

            var textXml = this.FirstOrDefault(x => x.Type == "text/xml");
            var textXmlIndex = IndexOf(textXml);
            var appXml = this.FirstOrDefault(x => x.Type == "application/xml");
            var appXmlIndex = IndexOf(appXml);

            if (textXml != null && appXml != null)
            {
                // swap text/xml with application/xml and remove text/xml
                appXml.Quality = new[] { textXml.Quality, appXml.Quality }.Max();
                if (appXmlIndex > textXmlIndex)
                {
                    Swap(appXmlIndex, textXmlIndex);
                    appXmlIndex = IndexOf(appXml);
                }
                Remove(textXml);
            }
            else if (textXml != null)
            {
                textXml.Type = "application/xml";
            }

            if (appXml != null)
            {
                // prioritize /xxx+xml over /xml
                var idx = appXmlIndex;
                while (idx < Count)
                {
                    var currentAccept = this[idx];
                    if (currentAccept.Quality < appXml.Quality)
                        break;
                    if (new Regex(@"\+xml$").IsMatch(currentAccept.Type))
                    {
                        Swap(appXmlIndex, idx);
                        appXmlIndex = idx;
                    }
                    idx++;
                }
            }
        }

        public void Add(string acceptType, ref int order)
        {
            string type = acceptType;
            float quality = 1;
            if (acceptType.Contains(";"))
            {
                var typeAndQuality = acceptType.Split(';');
                type = typeAndQuality.First();
                var attributes = typeAndQuality.Last().Trim().Split('=');
                if (attributes.Length == 2)
                {
                    float.TryParse(attributes.Last(), NumberStyles.Float, NumberFormatInfo.InvariantInfo,  out quality);
                }
            }
            if (new Regex(@"(application|text|image)/\*").IsMatch(acceptType))
            {
                foreach (var mimeType in _types.Matching(acceptType))
                {
                    var at = new AcceptType(mimeType.Type, quality, order);
                    Add(at);
                    order++;
                }
            }
            else
            {
                var at = new AcceptType(type, quality, order);
                Add(at);
                order++;
            }
        }

        public List<MimeType> Parse()
        {
            return this.Where(x => _types.HasType(x.Type)).Select(acceptType => _types.Lookup(acceptType.Type)).Distinct().ToList();
        }
    }
}
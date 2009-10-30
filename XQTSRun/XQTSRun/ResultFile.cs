using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Reflection;

using DataEngine.XQuery;
using System.IO;
using System.Diagnostics;

namespace XQTSRun
{
    class ResultFile
    {
        public const String XQTSResult = "http://www.w3.org/2005/02/query-test-XQTSResult";

        private struct ResultRecord
        {
            public string id;
            public string result;
        }

        private struct FeatureRecord
        {
            public string name;
            public bool supported;
        }

        private List<ResultRecord> _results = new List<ResultRecord>();
        private List<FeatureRecord> _features = new List<FeatureRecord>();

        public void Create(string fileName)
        {
            Assembly asm = Assembly.GetAssembly(typeof(Translator));
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";
            settings.NewLineHandling = NewLineHandling.Replace;
            using (XmlWriter writer = XmlWriter.Create(fileName, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("", "test-suite-result", XQTSResult);

                writer.WriteStartElement("", "implementation", XQTSResult);
                writer.WriteStartAttribute("name");
                writer.WriteString("QueryMachine.XQuery");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("version");
                writer.WriteString(asm.GetName().Version.ToString());
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("anonymous-result-column");
                writer.WriteString("false");
                writer.WriteEndAttribute();

                writer.WriteStartElement("", "organization", XQTSResult);
                writer.WriteStartAttribute("name");
                writer.WriteString("WmHelp.com");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("website");
                writer.WriteString("http://www.wmhelp.com");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("anonymous");
                writer.WriteString("false");
                writer.WriteEndAttribute();
                writer.WriteEndElement();

                writer.WriteStartElement("", "submittor", XQTSResult);
                writer.WriteStartAttribute("name");
                writer.WriteString("Semyon A. Chertkov");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("email");
                writer.WriteString("semyonc@gmail.com");
                writer.WriteEndAttribute();
                writer.WriteEndElement();

                writer.WriteStartElement("", "description", XQTSResult);
                writer.WriteString("Standalone XQuery Implementation in .NET");
                writer.WriteEndElement();

                writer.WriteStartElement("", "implementation-defined-items", XQTSResult);
                writer.WriteEndElement();

                writer.WriteStartElement("", "features", XQTSResult);
                foreach (FeatureRecord rec in _features)
                {
                    writer.WriteStartElement("", "feature", XQTSResult);
                    writer.WriteStartAttribute("name");
                    writer.WriteString(rec.name);
                    writer.WriteEndAttribute();
                    writer.WriteStartAttribute("supported");
                    writer.WriteString(XmlConvert.ToString(rec.supported));
                    writer.WriteEndAttribute();
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();  // features
                                
                writer.WriteStartElement("", "context-properties", XQTSResult);
                writer.WriteEndElement();
                writer.WriteEndElement(); // Implementation

                writer.WriteStartElement("", "syntax", XQTSResult);
                writer.WriteString("XQuery");
                writer.WriteEndElement();

                writer.WriteStartElement("", "test-run", XQTSResult);
                writer.WriteStartAttribute("dateRun");
                writer.WriteString(XmlConvert.ToString(DateTime.Today, "yyyy-MM-dd"));
                writer.WriteEndAttribute();
                writer.WriteStartElement("", "test-suite", XQTSResult);
                writer.WriteStartAttribute("version");
                writer.WriteString("1.0.2");
                writer.WriteEndAttribute();
                writer.WriteEndElement();
                if (Worktime != null)
                {
                    string elapsed;
                    if (Worktime.Elapsed.Hours > 0)
                        elapsed = String.Format("The test run {0} hr, {1} min, {2} sec.",
                            Worktime.Elapsed.Hours, Worktime.Elapsed.Minutes, Worktime.Elapsed.Seconds);
                    else if (Worktime.Elapsed.Minutes > 0)
                        elapsed = String.Format("The test run {0} min, {1} sec.",
                            Worktime.Elapsed.Minutes, Worktime.Elapsed.Seconds);
                    else
                        elapsed = String.Format("The test run {0} sec ({1} ms).",
                            Worktime.Elapsed.Seconds, Worktime.ElapsedMilliseconds);
                    writer.WriteStartElement("", "otherComments", XQTSResult);
                    writer.WriteString(elapsed);    
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();

                foreach (ResultRecord rec in _results)
                {
                    writer.WriteStartElement("", "test-case", XQTSResult);
                    writer.WriteStartAttribute("name");
                    writer.WriteString(rec.id);
                    writer.WriteEndAttribute();
                    writer.WriteStartAttribute("result");
                    writer.WriteString(rec.result);
                    writer.WriteEndAttribute();
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.Close();
            }

            using (XmlWriter writer = XmlWriter.Create(Path.Combine(
                Path.GetDirectoryName(fileName), "QM_Results.xml"), settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("results");
                writer.WriteStartElement("result");
                writer.WriteString(Path.GetFileName(fileName));
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }

        public void AddResult(string id, string result)
        {
            ResultRecord rec;
            rec.id = id;
            rec.result = result;
            _results.Add(rec);
        }

        public void AddFeature(string name, bool supported)
        {
            FeatureRecord rec;
            rec.name = name;
            rec.supported = supported;
            _features.Add(rec);
        }

        public Stopwatch Worktime { get; set; }
    }
}

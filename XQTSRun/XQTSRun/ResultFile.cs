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
                
                writer.WriteStartElement("", "implementation-defined-item", XQTSResult);
                writer.WriteStartAttribute("name"); // The version of Unicode that is used to construct expressions.
                writer.WriteString("expressionUnicode");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("Dependent on the CLR Runtime");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // implementation-defined-item
                
                writer.WriteStartElement("", "implementation-defined-item", XQTSResult);
                writer.WriteStartAttribute("name"); // The statically-known collations.
                writer.WriteString("collations");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("http://www.w3.org/2005/xpath-functions/collation/codepoint");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // implementation-defined-item
                
                writer.WriteStartElement("", "implementation-defined-item", XQTSResult);
                writer.WriteStartAttribute("name"); // The implicit timezone.
                writer.WriteString("implicitTimezone");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("Taken from the system clock. For this test run, +03:00");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // implementation-defined-item
                
                writer.WriteStartElement("", "implementation-defined-item", XQTSResult);
                writer.WriteStartAttribute("name"); // The circumstances in which warnings are raised, and the ways in which warnings are handled
                writer.WriteString("warningsMethod");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("Unimplemented");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // implementation-defined-item
                
                writer.WriteStartElement("", "implementation-defined-item", XQTSResult);
                writer.WriteStartAttribute("name"); // The method by which errors are reported to the external processing environment
                writer.WriteString("errorsMethod"); 
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("via .NET exception (XQueryException class)");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // implementation-defined-item
                
                writer.WriteStartElement("", "implementation-defined-item", XQTSResult);
                writer.WriteStartAttribute("name"); // Whether the implementation is based on the rules of [XML 1.0] and [XML Names] or the rules of [XML 1.1] and [XML Names 1.1]. One of these sets of rules must be applied consistently by all aspects of the implementation.
                writer.WriteString("XMLVersion");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("XML 1.0 (corresponding to .NET implementation)");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // implementation-defined-item
                
                writer.WriteStartElement("", "implementation-defined-item", XQTSResult);
                writer.WriteStartAttribute("name"); // Any components of the static context or dynamic context that are overwritten or augmented by the implementation
                writer.WriteString("overwrittenContextComponents");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("Added static namespace http://www.wmhelp.com/ext for proprietary functions");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // implementation-defined-item
                
                writer.WriteStartElement("", "implementation-defined-item", XQTSResult);
                writer.WriteStartAttribute("name"); // Which of the optional axes are supported by the implementation, if the Full-Axis Feature is not supported
                writer.WriteString("axes");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("All");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // implementation-defined-item
                
                writer.WriteStartElement("", "implementation-defined-item", XQTSResult);
                writer.WriteStartAttribute("name"); // The default handling of empty sequences returned by an ordering key (sortspec) in an order by clause (empty least or empty greatest)
                writer.WriteString("defaultOrderEmpty");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("Empty least");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // implementation-defined-item
                
                writer.WriteStartElement("", "implementation-defined-item", XQTSResult);
                writer.WriteStartAttribute("name"); // The names and semantics of any extension expressions (pragmas) recognized by the implementation
                writer.WriteString("pragmas");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("None");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // implementation-defined-item
                
                writer.WriteStartElement("", "implementation-defined-item", XQTSResult);
                writer.WriteStartAttribute("name"); // The names and semantics of any option declarations recognized by the implementation
                writer.WriteString("optionDeclarations");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("None. Implemented custom option handling in XQueryCommand.OnPreProcess event");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // implementation-defined-item
                
                writer.WriteStartElement("", "implementation-defined-item", XQTSResult);
                writer.WriteStartAttribute("name"); // Protocols (if any) by which parameters can be passed to an external function, and the result of the function can returned to the invoking query
                writer.WriteString("externalFunctionProtocols");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("Direct passing with special CLR attributes XQuerySignature & XQueryParameter");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // implementation-defined-item
                
                writer.WriteStartElement("", "implementation-defined-item", XQTSResult);
                writer.WriteStartAttribute("name"); // The result of an unsuccessful call to an external function (for example, if the function implementation cannot be found or does not return a value of the declared type)
                writer.WriteString("externalFunctionCall");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("An error is raised");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // implementation-defined-item
                
                writer.WriteStartElement("", "implementation-defined-item", XQTSResult);
                writer.WriteStartAttribute("name"); // Limits on ranges of values for various data types, as enumerated in 5.3 Data Model Conformance
                writer.WriteString("limits");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("xs:decimal, xs:integer: -79228162514264337593543950335 to 79228162514264337593543950335. xs:float: -3.402823E+38 to 3.402823E+38. xs:double -1.79769313486232E+308 to 1.79769313486232E+308");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // implementation-defined-item
                
                writer.WriteStartElement("", "implementation-defined-item", XQTSResult);
                writer.WriteStartAttribute("name"); // The destination of the trace output is implementation-defined. See 4 The Trace Function
                writer.WriteString("traceDestination");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("System.Console.Out");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // implementation-defined-item
                
                writer.WriteStartElement("", "implementation-defined-item", XQTSResult);
                writer.WriteStartAttribute("name"); // For xs:integer operations, implementations that support limited-precision integer operations must either raise an error [err:FOAR0002]...
                writer.WriteString("integerOperations");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("err:FOAR0002 is raised");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // implementation-defined-item
                
                writer.WriteStartElement("", "implementation-defined-item", XQTSResult);
                writer.WriteStartAttribute("name"); // For xs:decimal values the number of digits of precision returned by the numeric operators is implementation-defined...
                writer.WriteString("decimalDigits");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("18 digits are supported");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // implementation-defined-item

                writer.WriteStartElement("", "implementation-defined-item", XQTSResult);
                writer.WriteStartAttribute("name"); // If the number of digits in the result exceeds the number of digits that the implementation supports, the result is truncated or rounded...
                writer.WriteString("roundOrTruncate");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("Results are rounded");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // implementation-defined-item

                writer.WriteStartElement("", "implementation-defined-item", XQTSResult);
                writer.WriteStartAttribute("name"); // For 7.4.6 fn:normalize-unicode, conforming implementations must support normalization form "NFC"...
                writer.WriteString("normalizationForms");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("NFC, NFD, NFKC, NFKD");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // implementation-defined-item

                writer.WriteStartElement("", "implementation-defined-item", XQTSResult);
                writer.WriteStartAttribute("name"); // All minimally conforming processors must support year values with a minimum of 4 digits...
                writer.WriteString("secondsDigits");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("6 digits for fractional seconds");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // implementation-defined-item

                writer.WriteStartElement("", "implementation-defined-item", XQTSResult);
                writer.WriteStartAttribute("name"); // The result of casting a string to xs:decimal, when the resulting value is not too large or too small but nevertheless... 
                writer.WriteString("stringToDecimal");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("Depends on System.Decimal.Parse");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // implementation-defined-item

                writer.WriteStartElement("", "implementation-defined-item", XQTSResult);
                writer.WriteStartAttribute("name"); // Various aspects of the processing provided by 15.5.4 fn:doc are implementation-defined...
                writer.WriteString("docProcessing");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("XQueryCommand.SearchPath property used together with base-uri for local files search");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // implementation-defined-item
                
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
                
                writer.WriteStartElement("", "context-property", XQTSResult);
                writer.WriteStartAttribute("name");
                writer.WriteString("Statically known namespaces");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("default and wmh='http://www.wmhelp.com/ext'");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("context-type");
                writer.WriteString("static");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // context-property

                writer.WriteStartElement("", "context-property", XQTSResult);
                writer.WriteStartAttribute("name");
                writer.WriteString("Default element/type namespace");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("Defined in prolog");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("context-type");
                writer.WriteString("static");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // context-property

                writer.WriteStartElement("", "context-property", XQTSResult);
                writer.WriteStartAttribute("name");
                writer.WriteString("Default function namespace");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("Defined in prolog");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("context-type");
                writer.WriteString("static");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // context-property

                writer.WriteStartElement("", "context-property", XQTSResult);
                writer.WriteStartAttribute("name");
                writer.WriteString("In-scope schema types");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("augmentable in XQueryCommand");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("context-type");
                writer.WriteString("static");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // context-property

                writer.WriteStartElement("", "context-property", XQTSResult);
                writer.WriteStartAttribute("name");
                writer.WriteString("In-scope element declarations");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("none");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("context-type");
                writer.WriteString("static");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // context-property

                writer.WriteStartElement("", "context-property", XQTSResult);
                writer.WriteStartAttribute("name");
                writer.WriteString("In-scope attribute declarations");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("none");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("context-type");
                writer.WriteString("static");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // context-property

                writer.WriteStartElement("", "context-property", XQTSResult);
                writer.WriteStartAttribute("name");
                writer.WriteString("In-scope variables");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("XQuery default");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("context-type");
                writer.WriteString("static");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // context-property

                writer.WriteStartElement("", "context-property", XQTSResult);
                writer.WriteStartAttribute("name");
                writer.WriteString("Context item static type");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("item()");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("context-type");
                writer.WriteString("static");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // context-property

                writer.WriteStartElement("", "context-property", XQTSResult);
                writer.WriteStartAttribute("name");
                writer.WriteString("Function signatures");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("augmentable via .NET attributes");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("context-type");
                writer.WriteString("static");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // context-property

                writer.WriteStartElement("", "context-property", XQTSResult);
                writer.WriteStartAttribute("name");
                writer.WriteString("Statically known collations");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("http://www.w3.org/2005/xpath-functions/collation/codepoint");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("context-type");
                writer.WriteString("static");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // context-property

                writer.WriteStartElement("", "context-property", XQTSResult);
                writer.WriteStartAttribute("name");
                writer.WriteString("Default collation");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("Depends on CultureInfo.CurrentCulture");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("context-type");
                writer.WriteString("static");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // context-property

                writer.WriteStartElement("", "context-property", XQTSResult);
                writer.WriteStartAttribute("name");
                writer.WriteString("Construction mode");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("Strip");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("context-type");
                writer.WriteString("static");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // context-property

                writer.WriteStartElement("", "context-property", XQTSResult);
                writer.WriteStartAttribute("name");
                writer.WriteString("Ordering mode");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("Ordered");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("context-type");
                writer.WriteString("static");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // context-property

                writer.WriteStartElement("", "context-property", XQTSResult);
                writer.WriteStartAttribute("name");
                writer.WriteString("Default order for empty sequences");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("Empty least");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("context-type");
                writer.WriteString("static");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // context-property

                writer.WriteStartElement("", "context-property", XQTSResult);
                writer.WriteStartAttribute("name");
                writer.WriteString("Boundary-space policy");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("Strip");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("context-type");
                writer.WriteString("static");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // context-property

                writer.WriteStartElement("", "context-property", XQTSResult);
                writer.WriteStartAttribute("name");
                writer.WriteString("Copy-namespaces mode");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("Preserve, No-Inherit");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("context-type");
                writer.WriteString("static");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // context-property

                writer.WriteStartElement("", "context-property", XQTSResult);
                writer.WriteStartAttribute("name");
                writer.WriteString("Copy-namespaces mode");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("Preserve, No-Inherit");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("context-type");
                writer.WriteString("static");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // context-property

                writer.WriteStartElement("", "context-property", XQTSResult);
                writer.WriteStartAttribute("name");
                writer.WriteString("Base URI");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("settable in XQueryCommand");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("context-type");
                writer.WriteString("static");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // context-property

                writer.WriteStartElement("", "context-property", XQTSResult);
                writer.WriteStartAttribute("name");
                writer.WriteString("Statically known documents");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("none");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("context-type");
                writer.WriteString("static");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // context-property

                writer.WriteStartElement("", "context-property", XQTSResult);
                writer.WriteStartAttribute("name");
                writer.WriteString("Statically known collections");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("none");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("context-type");
                writer.WriteString("static");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // context-property

                writer.WriteStartElement("", "context-property", XQTSResult);
                writer.WriteStartAttribute("name");
                writer.WriteString("Statically known default collection type");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("none");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("context-type");
                writer.WriteString("static");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // context-property

                writer.WriteStartElement("", "context-property", XQTSResult);
                writer.WriteStartAttribute("name");
                writer.WriteString("Context item");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("settable in XQueryCommand");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("context-type");
                writer.WriteString("dynamic");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // context-property

                writer.WriteStartElement("", "context-property", XQTSResult);
                writer.WriteStartAttribute("name");
                writer.WriteString("Context position");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("1");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("context-type");
                writer.WriteString("dynamic");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // context-property

                writer.WriteStartElement("", "context-property", XQTSResult);
                writer.WriteStartAttribute("name");
                writer.WriteString("Context size");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("1");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("context-type");
                writer.WriteString("dynamic");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // context-property

                writer.WriteStartElement("", "context-property", XQTSResult);
                writer.WriteStartAttribute("name");
                writer.WriteString("Variable values");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("Settable in XQueryCommand");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("context-type");
                writer.WriteString("dynamic");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // context-property

                writer.WriteStartElement("", "context-property", XQTSResult);
                writer.WriteStartAttribute("name");
                writer.WriteString("Function implementations");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("Dynamic binded");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("context-type");
                writer.WriteString("dynamic");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // context-property

                writer.WriteStartElement("", "context-property", XQTSResult);
                writer.WriteStartAttribute("name");
                writer.WriteString("Current dateTime");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("System clock");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("context-type");
                writer.WriteString("dynamic");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // context-property

                writer.WriteStartElement("", "context-property", XQTSResult);
                writer.WriteStartAttribute("name");
                writer.WriteString("Implicit timezone");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("System clock");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("context-type");
                writer.WriteString("dynamic");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // context-property

                writer.WriteStartElement("", "context-property", XQTSResult);
                writer.WriteStartAttribute("name");
                writer.WriteString("Available documents");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("By default any document retrievable with XmlReader.Create.");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("context-type");
                writer.WriteString("dynamic");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // context-property

                writer.WriteStartElement("", "context-property", XQTSResult);
                writer.WriteStartAttribute("name");
                writer.WriteString("Available collections");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("None. Can be set in XQueryCommand.");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("context-type");
                writer.WriteString("dynamic");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // context-property

                writer.WriteStartElement("", "context-property", XQTSResult);
                writer.WriteStartAttribute("name");
                writer.WriteString("Default collection");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("value");
                writer.WriteString("None. Can be set in XQueryCommand.");
                writer.WriteEndAttribute();
                writer.WriteStartAttribute("context-type");
                writer.WriteString("dynamic");
                writer.WriteEndAttribute();
                writer.WriteEndElement(); // context-property

                writer.WriteEndElement(); // context-properties
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
                writer.WriteStartElement("", "transformation", XQTSResult);
                writer.WriteStartElement("", "p", XQTSResult);
                writer.WriteString("All queries, listed in XQTS was used in the test run without any changes and/or modifications, as is was provided by W3C XQTS v.1.0.2.");
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteStartElement("", "comparison", XQTSResult);
                writer.WriteStartElement("", "p", XQTSResult);
                writer.WriteString("In order to compare the test results with expected, the test program perform serialization of the test results," +
                    " insert them under dummy root element, re-parse and apply custom comparsion function. The function normalize the whitespaces " +
                    "before compare the text nodes (where applicable) and ignore any declared namespace prefixes.");
                writer.WriteEndElement();
                writer.WriteStartElement("", "p", XQTSResult);
                writer.WriteString("Current version of the test program does not perform any comparsion for engine-raised exceptions, with codes, provided by the \"expected-error\" tag.");
                writer.WriteEndElement();
                writer.WriteEndElement();
                if (Worktime != null)
                {
                    string elapsed;
                    if (Worktime.Elapsed.Hours > 0)
                        elapsed = String.Format("The test run took {0} hr, {1} min, {2} sec.",
                            Worktime.Elapsed.Hours, Worktime.Elapsed.Minutes, Worktime.Elapsed.Seconds);
                    else if (Worktime.Elapsed.Minutes > 0)
                        elapsed = String.Format("The test run took {0} min, {1} sec.",
                            Worktime.Elapsed.Minutes, Worktime.Elapsed.Seconds);
                    else
                        elapsed = String.Format("The test run took {0} sec ({1} ms).",
                            Worktime.Elapsed.Seconds, Worktime.ElapsedMilliseconds);
                    writer.WriteStartElement("", "otherComments", XQTSResult);
                    writer.WriteStartElement("", "p", XQTSResult);
                    writer.WriteString(elapsed);    
                    writer.WriteEndElement();
                    writer.WriteStartElement("", "p", XQTSResult);
                    writer.WriteString("Report generated by XQTSRun.exe (http://qm.codeplex.com)");
                    writer.WriteEndElement();
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

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Xml;
using System.Xml.XPath;

using DataEngine.CoreServices;
using DataEngine.XQuery;
using System.Xml.Schema;

namespace SimpleTestConsole
{
    public static class PerfTest
    {
        private static double TicksToMilliseconds(long ticks, int n)
        {
            if (Stopwatch.IsHighResolution)
            {
                double tickFrequency = 10000000.0;
                tickFrequency /= (double)Stopwatch.Frequency;
                double num2 = ticks / n;
                num2 *= tickFrequency;
                return num2 / 0x2710L;
            }
            return (double)ticks / TimeSpan.TicksPerMillisecond / n;
        }

        [XQuerySignature("load-test1")]
        public static double TestDocument1([Implict] Executive executive, string name, int iter)
        {
            XQueryContext context = (XQueryContext)executive.Owner;
            string fileName = context.GetFileName(name);
            if (fileName == null)
                throw new ArgumentException(String.Format("File {0} not found", name));
            long time = 0;
            Uri uri = new Uri(context.GetFileName(name));
            for (int k = 0; k < iter; k++)
            {
                Stopwatch wh = new Stopwatch();
                wh.Start();
                XQueryDocument ndoc = new XQueryDocument();
                ndoc.Open(uri, context.GetSettings(), XmlSpace.Default, context.Token);
                ndoc.Fill();
                ndoc.Close();
                wh.Stop();
                time += wh.ElapsedTicks;
            }
            return TicksToMilliseconds(time, iter);
        }

        [XQuerySignature("load-test2")]
        public static double TestDocument2([Implict] Executive executive, string name, int iter)
        {
            XQueryContext context = (XQueryContext)executive.Owner;
            string fileName = context.GetFileName(name);
            if (fileName == null)
                throw new ArgumentException(String.Format("File {0} not found", name));
            long time = 0;
            String filename = context.GetFileName(name);
            for (int k = 0; k < iter; k++)
            {                
                Stopwatch wh = new Stopwatch();
                wh.Start();
                IXPathNavigable nv = new XPathDocument(filename);
                wh.Stop();
                time += wh.ElapsedTicks;
            }
            return TicksToMilliseconds(time, iter);
        }

        [XQuerySignature("load-test3")]
        public static double TestDocument3([Implict] Executive executive, string name, int iter)
        {
            XQueryContext context = (XQueryContext)executive.Owner;
            string fileName = context.GetFileName(name);
            if (fileName == null)
                throw new ArgumentException(String.Format("File {0} not found", name));
            long time = 0;
            String filename = context.GetFileName(name);
            for (int k = 0; k < iter; k++)
            {
                Stopwatch wh = new Stopwatch();
                wh.Start();
                XmlDocument doc = new XmlDocument();
                doc.Load(filename);
                wh.Stop();
                time += wh.ElapsedTicks;
            }
            return TicksToMilliseconds(time, iter);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace DataEngine.CoreServices
{
    public class PerfMonitor
    {
        private Dictionary<Object, Slot> stats = new Dictionary<object, Slot>();

        public static readonly PerfMonitor Global = new PerfMonitor();

        private class Slot
        {
            public long ticks;
            public int count;
            public readonly Stack<long> ts;

            public Slot()
            {
                ticks = 0;
                ts = new Stack<long>();
            }
        }

        public static double TicksToMilliseconds(long ticks)
        {
            if (Stopwatch.IsHighResolution)
            {
                double tickFrequency = 10000000.0;
                tickFrequency /= (double)Stopwatch.Frequency;
                double num2 = ticks;
                num2 *= tickFrequency;
                return num2 / 0x2710L;
            }
            return (double)ticks / TimeSpan.TicksPerMillisecond;
        }

        public void Begin(Object key)
        {
            lock (stats)
            {
                long t = Stopwatch.GetTimestamp();
                Slot slot;
                if (!stats.TryGetValue(key, out slot))
                {
                    slot = new Slot();
                    stats.Add(key, slot);
                }
                slot.ts.Push(t);
            }
        }


        public void End(Object key)
        {
            lock (stats)
            {
                long t = Stopwatch.GetTimestamp();
                Slot slot = stats[key];
                slot.ticks += t - slot.ts.Pop();
                slot.count++;
            }
        }

        public void Clear()
        {
            stats.Clear();
        }

        public void PrintStats(TextWriter output)
        {
            foreach (KeyValuePair<Object, Slot> kvp in stats)
            {
                output.WriteLine("{0}\t{1}\t{2}\t{3}", kvp.Key.ToString(),
                    kvp.Value.count, TicksToMilliseconds(kvp.Value.ticks), 
                        TicksToMilliseconds(kvp.Value.ticks / kvp.Value.count));
            }
        }

        public void TraceStats()
        {
            double total = 0;
            foreach (KeyValuePair<Object, Slot> kvp in stats)
            {
                double ms = TicksToMilliseconds(kvp.Value.ticks);
                total += ms;
                if (kvp.Value.count > 0)
                    Trace.TraceInformation("{0}\t{1}\t{2}\t{3}", kvp.Key.ToString(),
                        kvp.Value.count, ms,
                            TicksToMilliseconds(kvp.Value.ticks / kvp.Value.count));
            }
            Trace.TraceInformation("Total: {0}", total);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace XQTSRun
{
    public class OutputWriter: TextWriter
    {
        private RichTextBox m_textBox;
        private StringBuilder m_textPull;
        private Object m_lockObject;

        public OutputWriter(RichTextBox textBox)
        {
            m_textBox = textBox;
            m_lockObject = new object();
        }

        public override void Write(char value)
        {
            Write(new char[] { value });
        }

        public override void Write(char[] buffer, int index, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            if (index < 0)
                throw new ArgumentOutOfRangeException("index");
            if (count < 0)
                throw new ArgumentOutOfRangeException("count");
            if (buffer.Length - index < count)
                throw new ArgumentException();
            char[] str = new char[count];
            Array.Copy(buffer, index, str, 0, count);
            lock (m_lockObject)
            {
                if (m_textPull == null)
                    m_textPull = new StringBuilder();
                m_textPull.Append(str);
            }
        }

        public override void Flush()
        {
            lock (m_lockObject)
            {
                if (m_textPull != null)
                {
                    m_textBox.SelectionStart = m_textBox.TextLength;
                    m_textBox.SelectionLength = 0;
                    m_textBox.SelectedText = m_textPull.ToString();
                    m_textBox.ScrollToCaret();
                    m_textPull = null;
                }
            }
        }

        public override Encoding Encoding
        {
            get 
            { 
                return Encoding.Default; 
            }
        }
    }
}

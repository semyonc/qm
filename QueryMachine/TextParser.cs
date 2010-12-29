/*    
    SQLXEngine - Implementation of ANSI-SQL specification and 
       SQL-engine for executing the SELECT SQL command across the different data sources.
    Copyright (C) 2008-2009  Semyon A. Chertkov (semyonc@gmail.com)

    This library is free software; you can redistribute it and/or
    modify it under the terms of the GNU Lesser General Public
    License as published by the Free Software Foundation; either
    version 2.1 of the License, or (at your option) any later version.

    This library is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public
    License along with this library; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DataEngine.CoreServices
{
    public abstract class TextParser
    {
        public struct ColRow
        {
            public int line;
            public int col;
        }

        public char CommentChar { get; set; }

        public int LineIndex { get; protected set; }

        public abstract int Get(TextReader reader, string[] values, ColRow[] pos);

        public abstract int Get(string line, string[] values, int lineindex, ColRow[] pos);

        public int Get(TextReader reader, string[] values)
        {
            return Get(reader, values, null);
        }

        public int Get(string line, string[] values)
        {
            return Get(line, values, 0, null);
        }
    }

    public class CsvParser: TextParser
    {
        private StringBuilder m_stringBuilder;
        
        public char Delimiter { get; set; }
        public char QuoteChar { get; set; }

        public CsvParser()
        {
            m_stringBuilder = new StringBuilder();
            Delimiter = ',';
            QuoteChar = '"';
            CommentChar = '#';
        }

        public CsvParser(char delimiter)
            : this()
        {
            Delimiter = delimiter;
        }

        public CsvParser(char delimiter, char commentChar)
            : this(delimiter)
        {
            CommentChar = commentChar;
        }

        public override int Get(TextReader reader, string[] values, TextParser.ColRow[] pos)
        {
            int count = 0;
            for (int k = 0; k < values.Length; k++)
            {
                values[k] = null;
                if (pos != null)
                {
                    pos[k].col = 0;
                    pos[k].line = LineIndex;
                }
            }
            int index = 0;
            String input = reader.ReadLine();
            if (input.Length > 0 && input[0] == CommentChar)
            {
                LineIndex++;
                return -1;
            }
            while (index < input.Length)
            {
                if (pos != null && count < pos.Length)
                {
                    pos[count].line = LineIndex;
                    pos[count].col = index;
                }
                if (input[index] == QuoteChar)
                {
                    index++;
                    m_stringBuilder.Length = 0;
                    while (true)
                    {
                        int k = input.IndexOf(QuoteChar, index);
                        if (k == -1)
                        {
                            m_stringBuilder.Append(input.Substring(index));
                            input = reader.ReadLine();
                            index = 0;
                            LineIndex++;
                        }
                        else
                        {
                            int next_ch;
                            if (k < input.Length -1)
                            {
                                next_ch = input[k + 1];
                                if (next_ch == QuoteChar)
                                {
                                    m_stringBuilder.Append(input.Substring(index, k - index + 1));
                                    if (index == input.Length - 1)
                                    {
                                        input = reader.ReadLine();
                                        index = 0;
                                        LineIndex++;
                                    }
                                    else
                                        index = k + 2;
                                }
                                else
                                {
                                    m_stringBuilder.Append(input.Substring(index, k - index));
                                    index = k + 1;
                                    break;
                                }
                            }
                            else
                            {
                                m_stringBuilder.Append(input.Substring(index, k - index));
                                index = k + 1;
                                break;
                            }
                        }
                    }
                    if (index < input.Length)
                        if (input[index] == Delimiter)
                            index++;
                        else
                            throw new ESQLException(Properties.Resources.InvalidTextFileFormat,
                                LineIndex + 1, index + 1);
                    if (count < values.Length)
                        values[count++] = m_stringBuilder.ToString();
                }
                else
                {
                    int k = input.IndexOf(Delimiter, index);
                    if (k > -1)
                    {
                        if (count < values.Length)
                            values[count++] = input.Substring(index, k - index);
                        index = k + 1;
                    }
                    else
                    {
                        if (count < values.Length)
                            values[count++] = input.Substring(index);
                        break;
                    }
                }
            }
            LineIndex++;
            return count;
        }

        public override int Get(string line, string[] values, int lineindex, ColRow[] pos)
        {
            int count = 0;
            for (int k = 0; k < values.Length; k++)
            {
                values[k] = null;
                if (pos != null)
                {
                    pos[k].col = 0;
                    pos[k].line = lineindex;
                }
            }
            int index = 0;
            if (line.Length > 0 && line[0] == CommentChar)
                return -1;
            while (index < line.Length)
            {
                if (pos != null && count < pos.Length)
                {
                    pos[count].line = lineindex;
                    pos[count].col = index;
                }
                if (line[index] == QuoteChar)
                {
                    index++;
                    StringBuilder sb = new StringBuilder();
                    while (true)
                    {
                        int k = line.IndexOf(QuoteChar, index);
                        if (k == -1)
                            throw new ESQLException(Properties.Resources.UnexpectedTextFileEOL, lineindex + 1, index + 1);
                        else
                        {
                            int next_ch;
                            if (k < line.Length -1)
                            {
                                next_ch = line[k + 1];
                                if (next_ch == QuoteChar)
                                {
                                    sb.Append(line.Substring(index, k - index + 1));
                                    index = k + 2;
                                    if (index > line.Length - 1)
                                        break;
                                }
                                else
                                {
                                    sb.Append(line.Substring(index, k - index));
                                    index = k + 1;
                                    break;
                                }
                            }
                            else
                            {
                                sb.Append(line.Substring(index));
                                index = k + 1;
                                break;
                            }
                        }
                    }
                    if (index < line.Length)
                        if (line[index] == Delimiter)
                            index++;
                        else
                            throw new ESQLException(Properties.Resources.InvalidTextFileFormat,
                                lineindex + 1, index + 1);
                    if (count < values.Length)
                        values[count++] = sb.ToString();
                }
                else
                {
                    int k = line.IndexOf(Delimiter, index);
                    if (k > -1)
                    {
                        if (count < values.Length)
                            values[count++] = line.Substring(index, k - index);
                        index = k + 1;
                    }
                    else
                    {
                        if (count < values.Length)
                            values[count++] = line.Substring(index);
                        break;
                    }
                }
            }
            return count;
        }
    }

    public class FLVParser : TextParser
    {
        private int[] m_length;

        public FLVParser(int[] length)
        {
            m_length = length;
            CommentChar = '#';
        }

        public FLVParser(int[] length, char commentChar)
            : this(length)
        {
            CommentChar = commentChar;
        }

        public override int Get(TextReader reader, string[] values, TextParser.ColRow[] pos)
        {
            return Get(reader.ReadLine(), values, LineIndex++, pos);
        }

        public override int Get(string line, string[] values, int lineindex, ColRow[] pos)
        {
            int count = 0;
            for (int k = 0; k < values.Length; k++)
            {
                values[k] = null;
                if (pos != null)
                {
                    pos[k].line = lineindex;
                    pos[k].col = 0;
                }
            }
            int index = 0;
            if (line.Length > 0 && line[0] == CommentChar)
                return -1;
            while (index < line.Length)
            {
                if (pos != null && count < pos.Length)
                {
                    pos[count].line = lineindex;
                    pos[count].col = index;
                }
                values[count] = line.Substring(index,
                    Math.Min(m_length[count], line.Length - index));
                index += m_length[count++];
            }
            return count;
        }
    }

}

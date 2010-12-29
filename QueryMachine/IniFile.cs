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
    
    Originally written by Alvaro Mendez (AMS.Profile)
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.IO;

namespace DataEngine.CoreServices
{
    public class IniFile
    {
        private string m_name;

        public IniFile(string name)
        {
            m_name = name;
        }

        #region The Win32 API methods
        [DllImport("kernel32", SetLastError = true)]
        static extern int WritePrivateProfileString(string section, string key, string value, string fileName);
        [DllImport("kernel32", SetLastError = true)]
        static extern int WritePrivateProfileString(string section, string key, int value, string fileName);
        [DllImport("kernel32", SetLastError = true)]
        static extern int WritePrivateProfileString(string section, int key, string value, string fileName);
        [DllImport("kernel32")]
        static extern int GetPrivateProfileString(string section, string key, string defaultValue, StringBuilder result, int size, string fileName);
        [DllImport("kernel32")]
        static extern int GetPrivateProfileString(string section, int key, string defaultValue, [MarshalAs(UnmanagedType.LPArray)] byte[] result, int size, string fileName);
        [DllImport("kernel32")]
        static extern int GetPrivateProfileString(int section, string key, string defaultValue, [MarshalAs(UnmanagedType.LPArray)] byte[] result, int size, string fileName);
        #endregion

        public string Name
        {
            get
            {
                return m_name;
            }
            set
            {
                m_name = value;
            }
        }

        protected virtual void VerifyAndAdjustSection(ref string section)
        {
            if (section == null)
                throw new ArgumentNullException("section");

            section = section.Trim();
        }

        protected virtual void VerifyAndAdjustEntry(ref string entry)
        {
            if (entry == null)
                throw new ArgumentNullException("entry");

            entry = entry.Trim();
        }

        protected virtual void VerifyName()
        {
            if (m_name == null || m_name == "")
                throw new InvalidOperationException("Operation not allowed because Name property is null or empty.");
        }

        public bool HasEntry(string section, string entry)
        {
            string[] entries = GetEntryNames(section);

            if (entries == null)
                return false;

            VerifyAndAdjustEntry(ref entry);
            return Array.IndexOf(entries, entry) >= 0;
        }

        public bool HasSection(string section)
        {
            string[] sections = GetSectionNames();

            if (sections == null)
                return false;

            VerifyAndAdjustSection(ref section);
            return Array.IndexOf(sections, section) >= 0;
        }

        public void SetValue(string section, string entry, object value)
        {
            if (value == null)
            {
                RemoveEntry(section, entry);
                return;
            }

            VerifyName();
            VerifyAndAdjustSection(ref section);
            VerifyAndAdjustEntry(ref entry);

            if (WritePrivateProfileString(section, entry, value.ToString(), Name) == 0)
                throw new Win32Exception();
        }

        public object GetValue(string section, string entry)
        {
            VerifyName();
            VerifyAndAdjustSection(ref section);
            VerifyAndAdjustEntry(ref entry);

            // Loop until the buffer has grown enough to fit the value
            for (int maxSize = 250; true; maxSize *= 2)
            {
                StringBuilder result = new StringBuilder(maxSize);
                int size = GetPrivateProfileString(section, entry, "", result, maxSize, Name);

                if (size < maxSize - 1)
                {
                    if (size == 0 && !HasEntry(section, entry))
                        return null;
                    return result.ToString();
                }
            }
        }

        public void RemoveEntry(string section, string entry)
        {
            if (!HasEntry(section, entry))
                return;

            VerifyName();
            VerifyAndAdjustSection(ref section);
            VerifyAndAdjustEntry(ref entry);

            if (WritePrivateProfileString(section, entry, 0, Name) == 0)
                throw new Win32Exception();
        }

        public void RemoveSection(string section)
        {
            if (!HasSection(section))
                return;

            VerifyName();
            VerifyAndAdjustSection(ref section);

            if (WritePrivateProfileString(section, 0, "", Name) == 0)
                throw new Win32Exception();
        }

        public string[] GetEntryNames(string section)
        {
            // Verify the section exists
            if (!HasSection(section))
                return null;

            VerifyAndAdjustSection(ref section);

            // Loop until the buffer has grown enough to fit the value
            for (int maxSize = 500; true; maxSize *= 2)
            {
                byte[] bytes = new byte[maxSize];
                int size = GetPrivateProfileString(section, 0, "", bytes, maxSize, Name);

                if (size < maxSize - 2)
                {
                    // Convert the buffer to a string and split it
                    string entries = Encoding.Default.GetString(bytes, 0, size - (size > 0 ? 1 : 0));
                    if (entries == "")
                        return new string[0];
                    return entries.Split(new char[] { '\0' });
                }
            }
        }

        public string[] GetSectionNames()
        {
            // Verify the file exists
            if (!File.Exists(Name))
                return null;

            // Loop until the buffer has grown enough to fit the value
            for (int maxSize = 500; true; maxSize *= 2)
            {
                byte[] bytes = new byte[maxSize];
                int size = GetPrivateProfileString(0, "", "", bytes, maxSize, Name);

                if (size < maxSize - 2)
                {
                    // Convert the buffer to a string and split it
                    string sections = Encoding.Default.GetString(bytes, 0, size - (size > 0 ? 1 : 0));
                    if (sections == "")
                        return new string[0];
                    return sections.Split(new char[] { '\0' });
                }
            }
        }

    }
}

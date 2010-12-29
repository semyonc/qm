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
using System.Linq;
using System.Text;
using System.Globalization;

namespace DataEngine.CoreServices
{
    public class LikeOperator
    {
        private char[] cLike;
        private int[] wildCardType;
        private int iLen;
        private bool isIgnoreCase;
        private int iFirstWildCard;
        private bool isNull;
        private char escapeChar;
        private CultureInfo cultureInfo;
            
        private const int UNDERSCORE_CHAR = 1;
        private const int PERCENT_CHAR = 2;

        public LikeOperator(string pattern, char escape, 
            bool ignoreCase, CultureInfo culture)
        {
            escapeChar = escape;
            cultureInfo = culture;
            isIgnoreCase = ignoreCase;
            Normalize(pattern);
        }

        public Object Compare(Object s)
        {
            if (s == null || s == DBNull.Value)
                return null;

            String str = (String)s;
            if (isIgnoreCase)
                str = str.ToUpper(cultureInfo);

            return compareAt(str, 0, 0, str.Length);
        }

        public bool Compare(String str)
        {
            if (isIgnoreCase)
                str = str.ToUpper(cultureInfo);
            return compareAt(str, 0, 0, str.Length);
        }

        private void Normalize(String pattern)
        {

            isNull = pattern == null;

            if (!isNull && isIgnoreCase)
                pattern = pattern.ToUpper(cultureInfo);

            iLen = 0;
            iFirstWildCard = -1;

            int l = pattern == null ? 0
                                    : pattern.Length;

            cLike = new char[l];
            wildCardType = new int[l];

            bool bEscaping = false;
            bool bPercent = false;

            for (int i = 0; i < l; i++)
            {
                char c = pattern[i];
                if (bEscaping == false)
                {
                    if (escapeChar != '\0' && escapeChar == c)
                    {
                        bEscaping = true;
                        continue;
                    }
                    else if (c == '_')
                    {
                        wildCardType[iLen] = UNDERSCORE_CHAR;
                        if (iFirstWildCard == -1)
                            iFirstWildCard = iLen;
                    }
                    else if (c == '%')
                    {
                        if (bPercent)
                            continue;
                        bPercent = true;
                        wildCardType[iLen] = PERCENT_CHAR;
                        if (iFirstWildCard == -1)
                            iFirstWildCard = iLen;
                    }
                    else
                        bPercent = false;
                }
                else
                {
                    bPercent = false;
                    bEscaping = false;
                }
                cLike[iLen++] = c;
            }

            for (int i = 0; i < iLen - 1; i++)
            {
                if ((wildCardType[i] == PERCENT_CHAR)
                        && (wildCardType[i + 1] == UNDERSCORE_CHAR))
                {
                    wildCardType[i] = UNDERSCORE_CHAR;
                    wildCardType[i + 1] = PERCENT_CHAR;
                }
            }
        }

        private bool compareAt(String s, int i, int j, int jLen)
        {
            for (; i < iLen; i++)
            {
                switch (wildCardType[i])
                {
                    case 0:                  // general character
                        if ((j >= jLen) || (cLike[i] != s[j++]))
                            return false;
                        break;

                    case UNDERSCORE_CHAR:    // underscore: do not test this character
                        if (j++ >= jLen)
                            return false;
                        break;

                    case PERCENT_CHAR:       // percent: none or any character(s)
                        if (++i >= iLen)
                            return true;
                        while (j < jLen)
                        {
                            if ((cLike[i] == s[j])
                                    && compareAt(s, i, j, jLen))
                                return true;
                            j++;
                        }
                        return false;
                }
            }
            if (j != jLen)
                return false;
            return true;
        }

        bool HasWildcards
        {
            get
            {
                return iFirstWildCard != -1;
            }
        }

        bool IsEquivalentToFalsePredicate
        {
            get
            {
                return isNull;
            }
        }

        bool IsEquivalentToEqualsPredicate
        {
            get
            {
                return iFirstWildCard == -1;
            }
        }

        bool isEquivalentToNotNullPredicate
        {
            get
            {
                if (isNull || !HasWildcards)
                    return false;

                for (int i = 0; i < wildCardType.Length; i++)
                    if (wildCardType[i] != PERCENT_CHAR)
                        return false;

                return true;
            }
        }

        bool IsEquivalentToBetweenPredicate
        {
            get
            {
                return iFirstWildCard > 0
                       && iFirstWildCard == wildCardType.Length - 1
                       && cLike[iFirstWildCard] == '%';
            }
        }

        bool IsEquivalentToBetweenPredicateAugmentedWithLike
        {
            get
            {
                return iFirstWildCard > 0 && cLike[iFirstWildCard] == '%';
            }
        }

    }
}

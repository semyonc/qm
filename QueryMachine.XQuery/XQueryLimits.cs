//        Copyright (c) 2009-2011, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        This program is free software: you can redistribute it and/or modify
//        it under the terms of the GNU General Public License as published by
//        the Free Software Foundation, either version 3 of the License, or
//        any later version.

using System;

namespace DataEngine.XQuery
{
    public static class XQueryLimits
    {        
        public const int DirectAcessBufferSize = 500;
        public const int DirectAccessDelta = 1500;
        
        public const int IteratorPrefetchSize = 5; // 64
        public const long LargeFileLength = 67108864;
        public const int DefaultValueCacheSize = 337;

        //public const long LargeFileLength = 154533888;

        public const int LargeFilePageSize = 10000;  // 400
        public const int LargeFileMinWorkset = 3;   // 3
        public const int LargeFileMaxWorkset = 10;  // 15
        public const int LargeFileWorksetDelta = 2; // 2
        public const int LargeFileMinIncrement = 3; 
        public const int LargeFileMaxDecrement = 1;

        public const int SmallFilePageSize = 300;      // 16
        public const int SmallFileMinWorkset = 100;    // 100
        public const int SmallFileMaxWorkset = 1500;   // 3000
        public const int SmallFileWorksetDelta = 500;  // 500 
        public const int SmallFileMinIncrement = 150;  // 150
        public const int SmallFileMaxDecrement = 300;  // 300     
    }
}

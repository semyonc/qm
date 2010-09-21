//        Copyright (c) 2010, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        Redistribution and use in source and binary forms, with or without
//        modification, are permitted provided that the following conditions are met:
//            * Redistributions of source code must retain the above copyright
//              notice, this list of conditions and the following disclaimer.
//            * Redistributions in binary form must reproduce the above copyright
//              notice, this list of conditions and the following disclaimer in the
//              documentation and/or other materials provided with the distribution.
//            * Neither the name of author nor the
//              names of its contributors may be used to endorse or promote products
//              derived from this software without specific prior written permission.
//
//        THIS SOFTWARE IS PROVIDED ''AS IS'' AND ANY
//        EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//        WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//        DISCLAIMED. IN NO EVENT SHALL  AUTHOR BE LIABLE FOR ANY
//        DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//        (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//        LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//        ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//        (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//        SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;

namespace DataEngine.XQuery
{
    public static class XQueryLimits
    {        
        public const int DirectAcessBufferSize = 500;
        public const int IteratorPrefetchSize = 64;
        public const int MaxParallelTasks = 1000;

        public const long LargeFileLength = 67108864;
        //public const long LargeFileLength = 154533888;
        
        public const int LargeFilePageSize = 400;
        public const int LargeFileMinWorkset = 3;
        public const int LargeFileMaxWorkset = 15;
        public const int LargeFileWorksetDelta = 5;
        public const int LargeFileMinIncrement = 3;
        public const int LargeFileMaxDecrement = 1;

        public const int SmallFilePageSize = 16;
        public const int SmallFileMinWorkset = 100;
        public const int SmallFileMaxWorkset = 3000;
        public const int SmallFileWorksetDelta = 500;
        public const int SmallFileMinIncrement = 150;
        public const int SmallFileMaxDecrement = 300;
    }
}

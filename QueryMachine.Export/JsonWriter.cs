//        Copyright (c) 2012, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        This program is free software: you can redistribute it and/or modify
//        it under the terms of the GNU General Public License as published by
//        the Free Software Foundation, either version 3 of the License, or
//        any later version.

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using MongoDB.Bson;
using MongoDB.Bson.IO;

using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

using DataEngine;
using DataEngine.CoreServices;
using DataEngine.CoreServices.Data;

namespace DataEngine.Export
{
    public class JsonWriter: AbstractWriter
    {
        private string _fileName;
        private bool _compress;
        private bool _createSchemaIni;

        public JsonWriter(string fileName, bool compress, bool createSchemaIni)
        {
            _fileName = fileName;
            _compress = compress;
            _createSchemaIni = createSchemaIni;
        }

        public override void Write(Resultset rs)
        {
            if (_createSchemaIni)
            {
                string iniFileName = Path.Combine(Path.GetDirectoryName(_fileName), "schema.ini");
                string sectionName = Path.GetFileName(_fileName);
                IniFile schema = new IniFile(iniFileName);
                schema.RemoveSection(sectionName);
                int n = 1;
                foreach (RowType.TypeInfo ti in rs.RowType.Fields)
                    schema.SetValue(sectionName, String.Format("Col{0}", n++),
                        String.Format("{0} {1}", ti.Name, JsonDataAccessor.GetSchemaDataType(ti.DataType)));
            }
            using (FileStream fsOut = File.Create(_fileName))
            {
                if (_compress)
                {
                    ZipOutputStream zipStream = new ZipOutputStream(fsOut);
                    zipStream.UseZip64 = UseZip64.On;
                    ZipEntry entry = new ZipEntry(ZipEntry.CleanName(
                        Path.ChangeExtension(Path.GetFileName(_fileName), ".json")));
                    zipStream.PutNextEntry(entry);
                    WriteTo(rs, zipStream);
                    zipStream.Close();
                }
                else
                    WriteTo(rs, fsOut);
                fsOut.Close();
            }
        }

        private void WriteTo(Resultset rs, Stream outStream)
        {
            string script = BsonFormatter.CreateJavascriptTemplate(rs.RowType);
            using (StreamWriter outs = new StreamWriter(outStream))
            {
                JsonWriterSettings settings = new JsonWriterSettings() { Encoding = Encoding.UTF8, 
                    OutputMode = JsonOutputMode.JavaScript, Indent = false };
                using (BsonFormatter formatter = new BsonFormatter(script))
                {
                    while (rs.Begin != null)
                    {
                        BsonWriter bsonWriter = BsonWriter.Create(outs, settings);
                        BsonDocument doc = formatter.Format(rs.RowType, rs.Dequeue());
                        doc.WriteTo(bsonWriter);
                        bsonWriter.Close();
                        outs.WriteLine();
                        RowProceded();
                    }
                }
            }            
        }
    }
}

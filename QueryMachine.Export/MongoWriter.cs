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

using MongoDB.Bson;
using MongoDB.Driver;

using DataEngine;
using DataEngine.CoreServices;
using DataEngine.CoreServices.Data;

namespace DataEngine.Export
{
    public class MongoWriter: AbstractWriter
    {
        private BatchMove m_batchMove;

        public MongoWriter(BatchMove batchMove)
        {
            m_batchMove = batchMove;
        }

        public override void Write(Resultset rs)
        {
            MongoConnectionStringBuilder csb = new MongoConnectionStringBuilder();
            csb.ConnectionString = m_batchMove.ConnectionString;
            MongoServer mongo = MongoServer.Create(csb);
            mongo.Connect();
            try
            {
                MongoDatabaseSettings settings = mongo.CreateDatabaseSettings(csb.DatabaseName);
                if (csb.Username != null)
                    settings.Credentials = new MongoCredentials(csb.Username, csb.Password);
                MongoDatabase database = mongo.GetDatabase(settings);
                MongoCollection<BsonDocument> collection = database.GetCollection(m_batchMove.TableName);
                using (BsonFormatter formatter = new BsonFormatter(m_batchMove.DML))
                {
                    while (rs.Begin != null)
                    {
                        BsonDocument doc = formatter.Format(rs.RowType, rs.Dequeue());
                        collection.Insert(doc);
                        RowProceded();
                    }
                }
            }
            finally
            {
                mongo.Disconnect();
            }
        }
    }
}

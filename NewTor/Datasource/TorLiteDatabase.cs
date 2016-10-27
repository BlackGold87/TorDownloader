using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

namespace NewTor.Datasource
{
    public class TorLiteDatabase : LiteDatabase
    {
        public TorLiteDatabase() : base("Test.db") { }

        public TorLiteDatabase(string connectionString, BsonMapper mapper = null) : base(connectionString, mapper) { }

        public TorLiteDatabase(IDiskService diskService, BsonMapper mapper = null) : base(diskService, mapper) { }

        public TorLiteDatabase(Stream stream, BsonMapper mapper = null) : base(stream, mapper) { }
    }
}

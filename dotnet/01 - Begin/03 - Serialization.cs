using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BeginSerialization : BaseClass
{
    // In MongoDB everything is BsonDocument
    // A BsonDocument is a dictionary of BsonValues
    // BsonDocument can contains BsonDocument
    public class DBInfo
    {
        public ObjectId _id;
        public string name;
        public DBInfo child;
        public BsonDocument catchAll;
    }

    public override Task RunAsync()
    {
        DBInfo o1 = new DBInfo() { _id = ObjectId.GenerateNewId(), name = "o1", catchAll = new BsonDocument().Add("hello", "world") };
        DBInfo o2 = new DBInfo() { _id = ObjectId.GenerateNewId(), name = "o2", child = o1, catchAll = new BsonDocument().Add("hello", "world!") };

        // Serialize .NET object to BsonDocument
        BsonDocument doc = o2.ToBsonDocument();
        // Change the name
        doc["name"] = "o2-modified";
        // Deserialize BsonDocument to .NET object
        DBInfo op2 = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<DBInfo>(doc);

        // Serialize to Json
        string json = o2.ToJson();
        
        // Deserialize from Json
        DBInfo op3 = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<DBInfo>(json);
        BsonDocument doc2 = BsonDocument.Parse(json);
        

        return Task.CompletedTask;
    }
}

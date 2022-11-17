using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class Dictionary : BaseClass
{
    public class DBInfo
    {
        [BsonId]                // With this attribute you can define the _id field name
        public ObjectId recordId;

        [BsonIgnoreIfNull]      // Ignore this variable if it is null
        public string name;

        [BsonElement("sp")]     // Rename this variable
        public string searchPolicy;

        // You can serialize dictionaries with typeof(key) == string
        public Dictionary<string, int> postalCode = new Dictionary<string, int>();

        [BsonExtraElements]     // I always add this attribute on object with Id, this avoid deserialization exception in case of schema change
                                // All element the deserialization can't match will be added here
        public BsonDocument catchAll;
    }
    public override Task RunAsync()
    {
        DBInfo t = new DBInfo();
        t.postalCode["Saint-Omer"] = 62500;
        t.postalCode["Lille"] = 59000;

        var doc = t.ToBsonDocument();

        DBInfo op2 = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<DBInfo>(doc);

        return Task.CompletedTask;
    }
}

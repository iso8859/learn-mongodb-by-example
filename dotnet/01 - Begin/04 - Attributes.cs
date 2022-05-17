using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BeginAttributes : BaseClass
{
    // Here we define the object we use and we save in the database
    public class Info
    {
        public int x, y;
    }

    public class DBInfo
    {
        [BsonId]                // With this attribute you can define the _id field name
        public ObjectId recordId;

        [BsonIgnore]
        public DBInfo child;    // Ignore this variable

        [BsonElement("sp")]     // Rename this variable
        public string searchPolicy;

        [BsonExtraElements]     // I always add this attribute on object with Id, this avoid deserialization exception in case of schema change
                                // All element the deserialization can't match will be added here
        public BsonDocument catchAll; 
    }

    public override Task RunAsync()
    {
        DBInfo test = new DBInfo() { recordId = ObjectId.GenerateNewId(), searchPolicy = "test" };
        Console.WriteLine(test.ToJson());
        // { "_id" : ObjectId("628264c17419f785118d0f3e"), "sp" : "test" }

        DBInfo test2 = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<DBInfo>(@"{ ""_id"" : ObjectId(""628264c17419f785118d0f3e""), ""sp"" : ""test"", ""oldVariable"" : ""hello"" }");
        Console.WriteLine(test2.catchAll["oldVariable"].AsString);
        // hello

        return Task.CompletedTask;
    }
}

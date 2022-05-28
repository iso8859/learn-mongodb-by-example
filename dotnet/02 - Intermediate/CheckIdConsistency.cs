using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CheckIdConsistency : BaseClass
{
    public override async Task RunAsync()
    {
        var client = Config.MongoClient();
        var db = client.GetDatabase("test");
        var collection = db.GetCollection<BsonDocument>("CheckIdConsistency");
        // Drop the collection to have clean db
        db.DropCollection("CheckIdConsistency");

        // Insert inconsistent document
        await collection.InsertOneAsync(new BsonDocument("_id", "1").Add("data", 1));
        await collection.InsertOneAsync(new BsonDocument("_id", ObjectId.GenerateNewId()).Add("data", "bananas"));

        // Check for consistency
        HashSet<BsonType> inconsistentIds = new HashSet<BsonType>();
        var cursor = await collection.Find(new BsonDocument()).Project(Builders<BsonDocument>.Projection.Include("_id")).ToCursorAsync();
        foreach (BsonDocument doc in cursor.ToEnumerable())
            inconsistentIds.Add(doc["_id"].BsonType);

        Console.WriteLine($"There is {inconsistentIds.Count} id type");
        // There is 2 id type
    }
}

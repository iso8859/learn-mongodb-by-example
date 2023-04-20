using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class IndexAndLimit : BaseClass
{
    public override async Task RunAsync()
    {
        var client = Config.MongoClient();
        var db = client.GetDatabase("test");
        string collection = "limit";
        db.DropCollection(collection);
        var dbCol = db.GetCollection<BsonDocument>(collection);

        // Add an index
        var indexOptions = new CreateIndexOptions();
        var indexKeys = Builders<BsonDocument>.IndexKeys.Ascending("index");
        var indexModel = new CreateIndexModel<BsonDocument>(indexKeys, indexOptions);
        string indexName = dbCol.Indexes.CreateOne(indexModel);

        // Insert le lot of big records
        List<BsonDocument> batch = new List<BsonDocument>();
        string bigdata = new string('a', 10000);
        DateTime start = DateTime.Now;
        for (int i=0; i<100000; i++)
        {
            BsonDocument doc = new BsonDocument().Add("index", i).Add("bigdata", bigdata);
            batch.Add(doc);
            if (batch.Count>100)
            {
                await dbCol.InsertManyAsync(batch);
                batch.Clear();
            }
        }
        dbCol.InsertMany(batch);
        batch.Clear();
        var duration = DateTime.Now - start;
        Console.WriteLine($"insert duration = {duration.TotalMilliseconds}ms");
        // Do the query
        start = DateTime.Now;
        var cursor = dbCol.Find(Builders<BsonDocument>.Filter.Gt("index", 90000)).Limit(1);
        BsonDocument a = await cursor.FirstAsync();
        duration = DateTime.Now - start;
        Console.WriteLine(a["index"] + " = " + duration.TotalMilliseconds + "ms");

        // Drop the index
        dbCol.Indexes.DropOne(indexName);
        // Put a breakpoint and restart mongodb to be sure nothing is in memory cache

        // Do the query
        start = DateTime.Now;
        cursor = dbCol.Find(Builders<BsonDocument>.Filter.Gt("index", 90000)).Limit(1);
        a = await cursor.FirstAsync();
        duration = DateTime.Now - start;
        Console.WriteLine(a["index"] + " = " + duration.TotalMilliseconds + "ms");

    }
}

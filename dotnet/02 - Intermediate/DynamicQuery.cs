using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

// Demonstration how to compose query with code
public class DynamicQuery : BaseClass
{
    // Here we define the object we use and we save in the database
    public class Info
    {
        public int x, y;
    }

    public class DBInfo
    {
        public ObjectId id; 
        public string name, type;
        public int count;
        public Info info;
        [BsonExtraElements]     // See 04 Attributes
        public BsonDocument catchAll;
    }

    public override async Task RunAsync()
    {
        var client = Config.MongoClient();
        var db = client.GetDatabase("test");
        var collection = db.GetCollection<DBInfo>("IntermediateDynamicQuery");
        // Drop the collection to have clean db
        db.DropCollection("IntermediateDynamicQuery");
        
        await collection.InsertManyAsync(new List<DBInfo>() {
            new DBInfo { name = "MongoDB", type = "DB", count = 1, info = new Info { x = 1, y = 2 } },
            new DBInfo { name = "sqlite", type = "DB", count = 1, info = new Info { x = 3, y = 4 } },
            new DBInfo { name = "Redis", type = "CACHE", count = 1, info = new Info { x = 11, y = 21 } },
            new DBInfo { name = "Cassandra", type = "CACHE", count = 1, info = new Info { x = 12, y = 22 } }
        });

        // Here we compose the query with code
        var _f = Builders<DBInfo>.Filter;
        List<FilterDefinition<DBInfo>> query = new List<FilterDefinition<DBInfo>>();
        query.Add(_f.Gte(_ => _.info.x, 10));
        query.Add(_f.Lte(_ => _.info.y, 30));
        // You can also use direct name, for example if you use dynamic variable list
        query.Add(_f.Eq("dynamicVar01", 10));

        FilterDefinition<DBInfo> final = _f.And(query);
        
        // Render the query to console
        Console.WriteLine(final.Render(collection.DocumentSerializer, collection.Settings.SerializerRegistry));
        // { "info.x" : { "$gte" : 10 }, "info.y" : { "$lte" : 30 }, "dynamicVar01" : 10 }
        
        // Execute the query
        List<DBInfo> infoList3 = await collection.Find(final).ToListAsync();

    }
}

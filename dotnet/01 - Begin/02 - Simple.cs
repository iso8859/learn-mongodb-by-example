using MongoDB.Bson;
using MongoDB.Driver;

public class BeginSimple : BaseClass
{
    // Here we define the objects we use in the database
    public class Info
    {
        public int x, y;
    }

    public class DBInfo
    {
        public ObjectId id; // You MUST add an id or Id or _id variable or field. See 04 - Attributes to change this name
        // ObjectId is the natural type for id in MongoDB but you can use simple type like int, long, string, etc.
        // ObjectId are set automaticaly during insert, for other types you must set them manually.

        public string name, type; // MongoDB serializer handles all public variables
        public int count { get; set; } // and all public properties
        public Info info;
    }

    public override async Task RunAsync()
    {
        // Same example than 01 - Start    
        var client = Config.MongoClient();
        var db = client.GetDatabase("test");
        var collection = db.GetCollection<DBInfo>("BeginSimple");
        // Drop the collection to clean it
        db.DropCollection("BeginSimple");

        // The syncronous version
        collection.InsertOne(new DBInfo { name = "MongoDB", type = "DB", count = 1, info = new Info { x = 1, y = 2 } });
        // and the asyncronous way
        await collection.InsertOneAsync(new DBInfo { name = "sqlite", type = "DB", count = 1, info = new Info { x = 3, y = 4 } });
        await collection.InsertOneAsync(new DBInfo { name = "Redis", type = "CACHE", count = 1, info = new Info { x = 11, y = 21 } });
        await collection.InsertOneAsync(new DBInfo { name = "Cassandra", type = "CACHE", count = 1, info = new Info { x = 12, y = 22 } });

        // Retrieve using Linq expression
        DBInfo info1 = collection.Find(x => x.name == "MongoDB").FirstOrDefault(); // Look at 05 - Index for setting index for performance
        // The same with discard parameter
        DBInfo info2 = collection.Find(_ => _.name == "sqlite").FirstOrDefault();

        // ** Asynchronous

        // Don't do that 
        // info1 = (await collection.FindAsync(_ => _.name == "MongoDB")).FirstOrDefault();
        // Do this
        info1 = await collection.Find(_ => _.name == "MongoDB").FirstOrDefaultAsync();

        DBInfo infonull = collection.Find(_ => _.name == "Moon").FirstOrDefault(); // Here infonull is null because Moon doesn't exists in the database

        // Retrieve several element
        List<DBInfo> infoList1 = collection.Find(_ => _.type == "DB").ToList();
        List<DBInfo> infoList2 = await collection.Find(_ => _.type == "DB").ToListAsync();


        // If you need to enumerate 
        List<DBInfo> infoListx = new List<DBInfo>();
        var cursor = await collection.Find(_ => _.type == "DB").ToCursorAsync();
        foreach(DBInfo dbi in cursor.ToEnumerable())
        {
            // Do something with dbi
            infoListx.Add(dbi);
            // Exit condition
            if (dbi.info.x == 1)
                break;
        }

        // Short foreach        
        foreach (DBInfo dbi in await collection.Find(_ => _.type == "DB").ToListAsync()) // ToList will load all results in memory, don't use it in foreach if you don't know how many results you have and you want to keep memory low
        {
            // Do something with dbi
        }

        // Let's check info1 in the console
        Console.WriteLine(info1.ToJson(new MongoDB.Bson.IO.JsonWriterSettings() { Indent = true }));
        // Output
        /*
            {
              "_id" : ObjectId("6281f361e22b6ffb50e45adc"),
              "name" : "MongoDB",
              "type" : "DB",
              "info" : {
                "x" : 1,
                "y" : 2
              },
              "count" : 1
            }        
        */
    }
}
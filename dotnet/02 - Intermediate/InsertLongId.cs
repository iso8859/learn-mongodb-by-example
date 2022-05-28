using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class InsertLongId : BaseClass
{
    public class Person
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }

    // FindOneAndUpdate is atomic
    // https://www.mongodb.com/docs/manual/core/write-operations-atomicity/
    static public async Task<long> NextInt64Async(IMongoDatabase db, string seqCollection, long q = 1, CancellationToken cancel = default)
    {
        long result = 1;
        BsonDocument r = await db.GetCollection<BsonDocument>("seq").FindOneAndUpdateAsync<BsonDocument>(
            filter: Builders<BsonDocument>.Filter.Eq("_id", seqCollection),
            update: Builders<BsonDocument>.Update.Inc("seq", q),
            options: new FindOneAndUpdateOptions<BsonDocument, BsonDocument>() { ReturnDocument = ReturnDocument.After, IsUpsert = true },
            cancellationToken: cancel
        );
        if (r != null)
            result = r["seq"].AsInt64;
        return result;
    }


    public override async Task RunAsync()
    {
        var client = Config.MongoClient();
        var db = client.GetDatabase("test");
        var collection = db.GetCollection<Person>("CheckIdConsistency");
        // Drop the collection to have clean db
        db.DropCollection("CheckIdConsistency");
        db.GetCollection<BsonDocument>("seq").DeleteOne(Builders<BsonDocument>.Filter.Eq("_id", "person"));

        CountdownEvent started = new CountdownEvent(16);        
        // Insert 16 document in parallel
        Parallel.For(0, 16, async(i) =>
        {
            await collection.InsertOneAsync(new Person() { Id = await NextInt64Async(db, "person"), Name = "Person" + i });
            started.Signal();
        });

        started.Wait();

        // Check the consistency of the Id
        List<long> ids = (await collection.Find(_ => true).ToListAsync()).Select(p => p.Id).ToList(); // _ => true = all documents
        Console.WriteLine("db order : " + string.Join(',', ids));
        // one possible output : 1,2,4,3,5,7,11,6,13,8,15,16,14,12
        ids.Sort();
        Console.WriteLine(string.Join(',', ids));
        // output : 1,2,3,5,6,7,8,9,10,11,12,13,14,15,16
    }
}

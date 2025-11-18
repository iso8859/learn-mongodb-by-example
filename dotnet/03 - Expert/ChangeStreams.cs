using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Change Streams allow applications to access real-time data changes without the complexity and risk of tailing the oplog.
// ONLY WORKS with replica set or sharded cluster (not standalone server)
public class ChangeStreams : BaseClass
{
    public class Item
    {
        public int Id;
        public string Name;
        public DateTime CreatedAt;
    }

    IMongoCollection<Item> m_collection;
    // Write x items per seconds
    public async Task ThreadWrite(int items, int seconds)
    {
        DateTime end = DateTime.Now.AddSeconds(seconds);
        int sleep = 1000 / items;
        while (DateTime.Now < end)
        {
            for (int i = 0; i < items; i++)
            {
                Item item = new Item
                {
                    Id = new Random().Next(1, 100000),
                    Name = "Item " + Guid.NewGuid().ToString(),
                    CreatedAt = DateTime.Now
                };
                await m_collection.InsertOneAsync(item);
                await Task.Delay(sleep);
            }
        }
    }

    public override async Task RunAsync()
    {
        var client = Config.MongoClientDebug();
        var db = client.GetDatabase("test");
        m_collection = db.GetCollection<Item>("changestreams");
        await m_collection.DeleteManyAsync(FilterDefinition<Item>.Empty);
        var taskWrite = ThreadWrite(10, 30);
        var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<Item>>()
            .Match(x => x.OperationType == ChangeStreamOperationType.Insert);
        var options = new ChangeStreamOptions { FullDocument = ChangeStreamFullDocumentOption.UpdateLookup };
        using (var cursor = await m_collection.WatchAsync(pipeline, options))
        {
            await cursor.ForEachAsync(change =>
            {
                var item = change.FullDocument;
                Console.WriteLine($"New Item: Id={item.Id}, Name={item.Name}, CreatedAt={item.CreatedAt}");
            });
        }
        await taskWrite;
    }
}

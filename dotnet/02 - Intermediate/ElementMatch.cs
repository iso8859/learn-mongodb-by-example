using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ElementMatch : BaseClass
{
    public class Item
    {
        public string name;
        public string value;
    }

    public class Record
    {
        public ObjectId _id;
        public string test;
        public List<Item> items;

    }
    public override async Task RunAsync()
    {
        var client = Config.MongoClient();
        client.DropDatabase("test");
        var db = client.GetDatabase("test");
        var coll = db.GetCollection<Record>("Record");
        coll.InsertOne(new Record
        {
            test = "test1",
            items = new List<Item>
            {
                new Item { name = "item1", value = "a" },
                new Item { name = "item2", value = "b" },
                new Item { name = "item3", value = "c" }
            }
        });
        coll.InsertOne(new Record
        {
            test = "test2",
            items = new List<Item>
            {
                new Item { name = "item1", value = "a" },
                new Item { name = "item2", value = "d" },
                new Item { name = "item3", value = "c" },
                new Item { name = "item4", value = "f" }
            }
        });
        coll.InsertOne(new Record
        {
            test = "test3",
            items = new List<Item>
            {
                new Item { name = "item1", value = "a" },
                new Item { name = "item2", value = "d" },
                new Item { name = "item3", value = "e" },
                new Item { name = "item4", value = "f" }
            }
        });
        //foreach (Record r in await coll.Find(Builders<Record>.Filter.All(_ => _.items, Builders<Record>.Filter.And(
        //    Builders<Record>.Filter.ElemMatch(_ => _.items, i => i.name == "item1" && i.value == "a"),
        //    Builders<Record>.Filter.ElemMatch(_ => _.items, i => i.name == "item3" && i.value == "c"))).ToListAsync())
        foreach (Record r in await coll.Find(Builders<Record>.Filter.And(Builders<Record>.Filter.ElemMatch(_ => _.items, i => i.name == "item1" && i.value == "a"), Builders<Record>.Filter.ElemMatch(_ => _.items, i => i.name == "item3" && i.value == "c"))).ToListAsync())
        {
            Console.WriteLine(r.test);
        }
    }
}

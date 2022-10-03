using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Aggregation;

public class BatchUnicity : BaseClass
{
    public class uid
    {
        public int _id;
        public int batch;
        public DateTime dt;
    }

    public async Task<bool> CreateAndCheckUnicityAsync(IMongoCollection<uid> collection, int batch, List<int> uids)
    {
        bool bResult = false;
        DateTime now = DateTime.Now;
        List<uid> ins = new List<uid>();
        foreach (int uid in uids)
            ins.Add(new uid() { batch = batch, _id = uid, dt = now });
        try
        {
            await collection.InsertManyAsync(ins);
            bResult = true;
        }
        catch (MongoBulkWriteException)
        {
            await collection.DeleteManyAsync(_ => _.batch == batch);
        }
        return bResult;
    }

    public async Task DeleteUnicityAsync(IMongoCollection<uid> collection, List<int> uids)
    {
        await collection.DeleteManyAsync(Builders<uid>.Filter.In(_ => _._id, uids));
    }

    public override async Task RunAsync()
    {
        var client = Config.MongoClientDebug();
        var db = client.GetDatabase("test");
        IMongoCollection<uid> collection = db.GetCollection<uid>("unicity");

        List<int> uids = new List<int>() { 4, 8, 9, 10, 54, 56, 87, 99 };
        List<int> uids2 = new List<int>() {0, 4, 8, 9, 10};

        if (await CreateAndCheckUnicityAsync(collection, 1, uids))
        {
            if (await CreateAndCheckUnicityAsync(collection, 2, uids2))
                await DeleteUnicityAsync(collection, uids2);

            await DeleteUnicityAsync(collection, uids);
        }

    }
}

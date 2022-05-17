using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BeginFindOneAndUpdate : BaseClass
{

    
    public class Task4
    {
        public ObjectId _id;
        public int state = 0;
        public DateTime start;
    }

    // Example several node received a message about a new task with a certain id
    // The first node that can find it, change the state and will consume the task.
    // The other nodes will get a null answer
    public async Task<Task4> TryGetOneAsync(IMongoCollection<Task4> collection, ObjectId id, int searchState = 100, int state = 200, CancellationToken cancel = default)
    {
        var _f = Builders<Task4>.Filter;
        var tmp = await collection.FindOneAndUpdateAsync(
            _f.And(_f.Eq(_ => _._id, id), _f.Eq(_ => _.state, searchState)),
            // _ => _._id == id && _.state == searchState,
            Builders<Task4>.Update
                    .Set(_ => _.state, state)
                    .CurrentDate(_ => _.start, UpdateDefinitionCurrentDateType.Date),
            new FindOneAndUpdateOptions<Task4, Task4>()
            {
                ReturnDocument = ReturnDocument.After
            },
            cancellationToken: cancel
            );
        return tmp;
    }
    
    public override async Task RunAsync()
    {
        var client = Config.MongoClient();
        var db = client.GetDatabase("test");
        MongoDB.Driver.IMongoCollection<Task4> collection = db.GetCollection<Task4>("BeingFindOneAndUpdate");
        // Drop the collection to clean it
        db.DropCollection("BeingFindOneAndUpdate");
        Task4 t4 = new Task4() { state = 100 };
        await collection.InsertOneAsync(t4);
        Task4 myTask = await TryGetOneAsync(collection, t4._id, 100, 200);
        if (myTask!=null)
        {
            // Process the task
        }
        else
        {
            // next time :-)
        }
    }
}
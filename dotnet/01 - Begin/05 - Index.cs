using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BeginIndex : BaseClass
{
    public record Person(ObjectId id, string Name, int Age);
    public override async Task RunAsync()
    {
        var client = Config.MongoClient();
        var db = client.GetDatabase("test");
        var collection = db.GetCollection<Person>("BeginIndex");
        
        var indexOptions = new CreateIndexOptions();
        var indexKeys = Builders<Person>.IndexKeys.Ascending(_=>_.Name);
        var indexModel = new CreateIndexModel<Person>(indexKeys, indexOptions);
        await collection.Indexes.CreateOneAsync(indexModel).ConfigureAwait(false); // Call index creation during Init phase.
        // If you are in micro services create an "Init" node you will call manually.

        // Create all record in memory
        List<Person> persons = new List<Person>();
        for (int i = 0; i < 10000; i++)
            persons.Add(new Person(ObjectId.GenerateNewId(), "John" + i.ToString("00000"), i / 100));

        // Insert
        await collection.InsertManyAsync(persons).ConfigureAwait(false);


        /*

        Query will use the index for fast retrieve
        Keep in mind an index must stay in memory on the server. If you have a lot of data check about memory usage.

        db.BeginIndex.find({ "Age": 10 }).explain()

        "queryPlanner" : {
        "plannerVersion" : 1.0, 
        "namespace" : "test.BeginIndex", 
        "indexFilterSet" : false, 
        "parsedQuery" : {
            "Age" : {
                "$eq" : 10.0
            }
        }, 
        "winningPlan" : {
            "stage" : "COLLSCAN",   <= This mean no index usage, scan the collection
            "filter" : {
                "Age" : {
                    "$eq" : 10.0
                }
            }, 
            "direction" : "forward"
        }, 

        -----------------------------------------------------------------------------------------------------------------

        db.BeginIndex.find({ "Name": "John09999" }).explain()

        "queryPlanner" : {
        "plannerVersion" : 1.0, 
        "namespace" : "test.BeginIndex", 
        "indexFilterSet" : false, 
        "parsedQuery" : {
            "Name" : {
                "$eq" : "John09999"
            }
        }, 
        "winningPlan" : {
            "stage" : "FETCH", 
            "inputStage" : {
                "stage" : "IXSCAN",  <= This mean index usage
                "keyPattern" : {
                    "Name" : 1.0
                }, 
                "indexName" : "Name_1", 
                "isMultiKey" : false, 
                "multiKeyPaths" : {
                    "Name" : [

                    ]
                }, 
                "isUnique" : false, 
                "isSparse" : false, 
                "isPartial" : false, 
                "indexVersion" : 2.0, 
                "direction" : "forward", 
                "indexBounds" : {
                    "Name" : [
                        "[\"John09999\", \"John09999\"]"
                    ]
                }
            }
        }, 
        
        */

    }
}

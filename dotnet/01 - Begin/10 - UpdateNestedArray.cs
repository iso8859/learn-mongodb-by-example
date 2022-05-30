using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BeginUpdateNestedArray : BaseClass
{
    public class Country
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<State> States { get; set; }
    }

    public class State
    {

        public string Name { get; set; }
        public List<District> Districts { get; set; }
    }

    public class District
    {

        public string Name { get; set; }
        public string Population { get; set; }

    }

    public override async Task RunAsync()
    {
        var client = Config.MongoClient();
        var db = client.GetDatabase("test");
        var collection = db.GetCollection<Country>("BeginUpdateNestedArray");
        // Drop the collection to clean it
        db.DropCollection("BeginUpdateNestedArray");

        await collection.InsertOneAsync(new Country
        {
            Id = "USA",
            Name = "United States",
            States = new List<State>
            {
                new State
                {
                    Name = "California",
                    Districts = new List<District>
                    {
                        new District
                        {
                            Name = "Los Angeles",
                            Population = "4,972,000"
                        },
                        new District
                        {
                            Name = "San Francisco",
                            Population = "8,972,000"
                        }
                    }
                },
                new State
                {
                    Name = "New York",
                    Districts = new List<District>
                    {
                        new District
                        {
                            Name = "New York City",
                            Population = "8,972,000"
                        }
                    }
                }
            }
        });

        var update = Builders<Country>.Update.Set(_ => _.States[0].Districts[-1].Population, "5,000,000");
        Console.WriteLine(update.Render(collection.DocumentSerializer, collection.Settings.SerializerRegistry));
        // { "$set" : { "States.0.Districts.$.Population" : "5,000,000" } }

        // Update existing record
        await collection.FindOneAndUpdateAsync<Country>(   // <= you must tell C# this is Country to have linq syntax available
            _ => _.Id == "USA" && _.States.Any(s => s.Name == "California") && _.States.Any(s => s.Districts.Any(d => d.Name == "Los Angeles")),
            update
            );

        /*
         { 
            "_id" : "USA", 
            "Name" : "United States", 
            "States" : [
                {
                    "Name" : "California", 
                    "Districts" : [
                        {
                            "Name" : "Los Angeles", 
                            "Population" : "5,000,000"
                        }, 
                        {
                            "Name" : "San Francisco", 
                            "Population" : "8,972,000"
                        }
                    ]
                }, 
                {
                    "Name" : "New York", 
                    "Districts" : [
                        {
                            "Name" : "New York City", 
                            "Population" : "8,972,000"
                        }
                    ]
                }
            ]
        }

         */
    }


}

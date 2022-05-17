using MongoDB.Bson;
using MongoDB.Driver;

public class BeginStart : BaseClass
{
    // Don't forget to add a reference to MongoDB.Driver in the project properties.

    // This example is very basic and just demonstrates the usage of the MongoDB.Driver.
    public override Task RunAsync()
    {
        // Connect to local mongodb with connection string mongodb://127.0.0.1
        // var client = new MongoClient();
        // Here we use the config class
        var client = Config.MongoClient();

        // Create a BsonDocument and add some values.
        // BsonDocument is like Json.
        // Look at 02 - Simple, it's simpler to use MongoDB than this example

        BsonDocument doc = new BsonDocument()
            .Add("name", "MongoDB")
            .Add("type", "database")
            .Add("count", 1)
            .Add("info", new BsonDocument().Add("x", 203).Add("y", 102));

        // Insert the document
        client.GetDatabase("test").GetCollection<BsonDocument>("BeginStart").InsertOne(doc);

        // Now doc contains the record _id
        Console.WriteLine($"The record id is {doc["_id"]}");

        // Retrieve the document
        BsonDocument doc2 = client.GetDatabase("test").GetCollection<BsonDocument>("BeginStart").Find(Builders<BsonDocument>.Filter.Eq("_id", doc["_id"])).First();

        // Compare the two documents
        if (doc.CompareTo(doc2) == 0)
            Console.WriteLine("The documents are equal");
        else
            Console.WriteLine("The documents are not equal");

        return Task.CompletedTask;
    }
}
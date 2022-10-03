using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MongoDB.Driver.WriteConcern;

public static class Config
{
    public static string connectionString = "mongodb://127.0.0.1";

    public static void Init()
    {
        // Never put connection string in code.
        // If you work in a team or test on multiple environement you can put it in environment variable.
        // Environement variable are also great for Docker or Lambda/Function
        if (Environment.GetEnvironmentVariable("MONGODB_URI") != null)
        {
            connectionString = Environment.GetEnvironmentVariable("MONGODB_URI");
        }
    }

    public static IMongoClient MongoClient()
    {
        return new MongoClient(connectionString);
    }

    public static IMongoClient MongoClientDebug(int timeout = 0)
    {
        var mongoConnectionUrl = new MongoUrl(connectionString);
        var mongoClientSettings = MongoClientSettings.FromUrl(mongoConnectionUrl);
        if (timeout > 0)
            mongoClientSettings.ConnectTimeout = TimeSpan.FromMilliseconds(timeout);
        mongoClientSettings.ClusterConfigurator = cb =>
        {
            cb.Subscribe<CommandStartedEvent>(e =>
            {
                string message = $"{DateTime.Now.ToString("HH:mm:sss:fff")}-{e.CommandName}-{e.Command.ToJson()}";
                Console.WriteLine(message);
            });
        };
        return new MongoClient(mongoClientSettings);
    }
}

abstract public class BaseClass
{
    abstract public Task RunAsync();
}
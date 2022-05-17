using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}

abstract public class BaseClass
{
    abstract public Task RunAsync();
}
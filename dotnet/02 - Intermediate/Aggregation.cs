using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

public class Aggregation : BaseClass
{
    public class Pizza
    {
        public int _id;
        public string name;
        public string size;
        public int price;
        public int quantity;
        public DateTime date;
    }

    public class Order
    {
        public int _id;
        public int pizzaId;
        public DateTime orderDate;
        public DateTime deliveryData;
        public string customerName;
        public string customerPhoneNumber;
    }

    public class PizzaOrders
    {
        public int _id; // Order id
        public string name; // Pizza name
        public string size; // Pizza size
        public int price;
        public int quantity;
        public DateTime date;
        public DateTime orderDate;
        public DateTime deliveryData;
        public string customerName;
        public string customerPhoneNumber;
    }

    public override async Task RunAsync()
    {
        var client = Config.MongoClient();
        var db = client.GetDatabase("test");
        var colPizza = db.GetCollection<Pizza>("Pizza");
        // Drop the collection to have clean db
        db.DropCollection("Pizza");

        // https://www.mongodb.com/docs/upcoming/core/aggregation-pipeline/
        await colPizza.InsertManyAsync(new[] {
            new Pizza() { _id= 0, name= "Pepperoni", size= "small", price= 19, quantity= 10, date= DateTime.Parse( "2021-03-13T08:14:30Z" ) },
            new Pizza() { _id= 1, name= "Pepperoni", size= "medium", price= 20,quantity= 20, date = DateTime.Parse( "2021-03-13T09:13:24Z" ) },
            new Pizza() { _id= 2, name= "Pepperoni", size= "medium", price= 20,quantity= 1, date = DateTime.Parse( "2021-03-13T09:13:24Z" ) },
            new Pizza() { _id= 3, name= "Pepperoni", size= "large", price= 21,quantity= 30, date = DateTime.Parse( "2021-03-17T09:22:12Z" ) },
            new Pizza() { _id= 4, name= "Cheese", size= "small", price= 12,quantity= 15, date = DateTime.Parse( "2021-03-13T11:21:39.736Z" ) },
            new Pizza() { _id= 5, name= "Cheese", size= "medium", price= 13,quantity=50, date = DateTime.Parse( "2022-01-12T21:23:13.331Z" ) },
            new Pizza() { _id= 6, name= "Cheese", size= "large", price= 14,quantity= 10, date = DateTime.Parse( "2022-01-12T05:08:13Z" ) },
            new Pizza() { _id= 7, name= "Vegan", size= "small", price= 17,quantity= 10, date = DateTime.Parse( "2021-01-13T05:08:13Z" ) },
            new Pizza() { _id= 8, name= "Vegan", size= "medium", price= 18,quantity= 10, date = DateTime.Parse( "2021-01-13T05:10:13Z" ) }
        });

        // A good e-book about Aggregation https://www.practical-mongodb-aggregations.com/
        var pizza = colPizza.Aggregate(new AggregateOptions() { AllowDiskUse = true }).
            Match(_ => _.size == "medium").
            Group(_ => _.name, _ => new { totalQuantity = _.Sum(_ => _.quantity), name = _.First().name }).
            SortBy(_ => _.totalQuantity);
        Console.WriteLine((await pizza.ToListAsync()).ToJson(new MongoDB.Bson.IO.JsonWriterSettings() { Indent = true }));

        var colOrder = db.GetCollection<Order>("Order");
        // Drop the collection to have clean db
        db.DropCollection("Order");
        await colOrder.InsertManyAsync(new[]
            { new Order { _id = 1, pizzaId = 0, customerName = "Rob", customerPhoneNumber = "555-1234-456" },
            new Order { _id = 2, pizzaId = 1, customerName = "Bob", customerPhoneNumber = "555-1234-457" },
            new Order { _id = 3, pizzaId = 2, customerName = "John", customerPhoneNumber = "555-1234-458" },
            new Order { _id = 4, pizzaId = 3, customerName = "Rob", customerPhoneNumber = "555-1234-459" }}
            );

        // Find order history for Rob and get pizza name and size
        //var pizzaOrders = colOrder.Aggregate(new AggregateOptions() { AllowDiskUse = true }).
        //            Match(_ => _.customerName == "Rob").
        //            Lookup<Order, Pizza, PizzaOrders>(colPizza, _ => _.pizzaId, _ => _._id, _ => _. new PizzaOrders
        //            {
        //                _id = _.First()._id,
        //                name = _.First().name,
        //                size = _.First().size,
        //                price = _.First().price,
        //                quantity = _.First().quantity,
        //                date = _.First().date,
        //                orderDate = _.First().orderDate,
        //                deliveryData = _.First().deliveryData,
        //                customerName = _.First().customerName,
        //                customerPhoneNumber = _.First().customerPhoneNumber
        //            });

        /*
              [{
                "totalQuantity" : 10,
                "name" : "Vegan"
              }, {
                "totalQuantity" : 21,
                "name" : "Pepperoni"
              }, {
                "totalQuantity" : 50,
                "name" : "Cheese"
              }]
         */
    }
}

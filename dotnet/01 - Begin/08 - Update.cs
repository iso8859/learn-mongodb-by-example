using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BeginUpdate : BaseClass
{
    public class Department
    {       
        public string DepartmentId;
        public string DepartmentName;
        public string DepartmentHead;
        [BsonIgnoreIfNull]
        public string Title;
    }
    public record College(
        ObjectId _id,
        Department Department,
        string Location,
        string CollegeName
        );

    public override async Task RunAsync()
    {
        var client = Config.MongoClient();
        var db = client.GetDatabase("test");
        var collection = db.GetCollection<College>("BeginUpdate");
        // Drop the collection to clean it
        db.DropCollection("BeginUpdate");

        College school = new College(
            ObjectId.Empty,
            new Department() { DepartmentId = "FS-11-140", DepartmentName = "IT", DepartmentHead = "XYZ" },
            "India",
            "MMCOE"
            );

        await collection.InsertOneAsync(school);
        /* At this point
        { 
            "_id" : ObjectId("628387c7b249abfe2a3c4f50"), 
            "Department" : {
                "DepartmentId" : "FS-11-140", 
                "DepartmentName" : "IT", 
                "DepartmentHead" : "XYZ"
            }, 
            "Location" : "India", 
            "CollegeName" : "MMCOE"
        }
        */
        var r = await collection.UpdateOneAsync(
            _ => _._id == school._id,
            Builders<College>.Update.Set(c => c.Department.Title, "Information Technology"));
        
        if (r.ModifiedCount == 0)
            Console.WriteLine("Update failed");
        else
        {
            /*
            { 
                "_id" : ObjectId("628387c7b249abfe2a3c4f50"), 
                "Department" : {
                    "DepartmentId" : "FS-11-140", 
                    "DepartmentName" : "IT", 
                    "DepartmentHead" : "XYZ", 
                    "Title" : "Information Technology"
                }, 
                "Location" : "India", 
                "CollegeName" : "MMCOE"
            }

             */
        }

        school = new College(
            ObjectId.Empty,
            new Department() { DepartmentId = "584-MZ-345", DepartmentName = "LAW", DepartmentHead = "XYZ" },
            "France",
            "Paris"
            );
        await collection.InsertOneAsync(school);

        /*
        { 
            "_id" : ObjectId("628387c7b249abfe2a3c4f51"), 
            "Department" : {
                "DepartmentId" : "584-MZ-345", 
                "DepartmentName" : "LAW", 
                "DepartmentHead" : "XYZ"
            }, 
            "Location" : "France", 
            "CollegeName" : "Paris"
        }
        */

        r = await collection.UpdateOneAsync(
             new FilterDefinitionBuilder<College>().Where(x => x._id == school._id),
            new UpdateDefinitionBuilder<College>().Set(_ => _.Department.Title, "ABCD"),
            new UpdateOptions() { IsUpsert = false });

        if (r.ModifiedCount == 0)
            Console.WriteLine("Update failed");
        else
        {
            /*
            { 
                "_id" : ObjectId("628387c7b249abfe2a3c4f51"), 
                "Department" : {
                    "DepartmentId" : "584-MZ-345", 
                    "DepartmentName" : "LAW", 
                    "DepartmentHead" : "XYZ", 
                    "Title" : "ABCD"
                }, 
                "Location" : "France", 
                "CollegeName" : "Paris"
            }
            */
        }
    }
}
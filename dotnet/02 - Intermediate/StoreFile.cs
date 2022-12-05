using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

public class Storefile : BaseClass
{
    public override async Task RunAsync()
    {
        var client = Config.MongoClient();
        var db = client.GetDatabase("test");

        // Read a file or create it

        MemoryStream ms = new MemoryStream(new byte[] { 0x00, 0x01, 0x02 });
        ms.Position = 0;
        // MemoryStream ms2 = new MemoryStream(await File.ReadAllBytesAsync("test.pdf"));

        // Write to MongoDB
        // There is a limitation of 16 MB in record size for mongodb, this what GridFS handle, unlimited file size.
        // https://www.mongodb.com/docs/manual/core/document/#:~:text=Document%20Size%20Limit,MongoDB%20provides%20the%20GridFS%20API.
        GridFSBucket gridFSBucket = new GridFSBucket(db);
        ObjectId id = await gridFSBucket.UploadFromStreamAsync("exemple.bin", ms);

        // Get the file
        MemoryStream ms3 = new MemoryStream();
        await gridFSBucket.DownloadToStreamAsync(id, ms3);
    }
}

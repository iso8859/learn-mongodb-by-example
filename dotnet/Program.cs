Config.Init();

// In general using a singleton for MongoClient is a good idea

await (new ChangeStreams()).RunAsync();
return;

await (new BeginStart()).RunAsync();
await (new BeginSimple()).RunAsync();
await (new BeginSerialization()).RunAsync();
await (new BeginAttributes()).RunAsync();
await (new BeginIndex()).RunAsync();
await (new BeginFindOneAndUpdate()).RunAsync();
await (new BeginUpdate()).RunAsync();
await (new BeginUpdateNestedArray()).RunAsync();

await (new Aggregation()).RunAsync();
await (new CheckIdConsistency()).RunAsync();
await (new DynamicQuery()).RunAsync();
await (new ElementMatch()).RunAsync();
await (new InsertLongId()).RunAsync();
await (new Dictionary()).RunAsync();
await (new Storefile()).RunAsync();
await (new IndexAndLimit()).RunAsync();

await (new BatchUnicity()).RunAsync();
await (new ChangeStreams()).RunAsync();
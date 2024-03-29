﻿Config.Init();

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
await (new InsertLongId()).RunAsync();
await (new Dictionary()).RunAsync();
await (new Storefile()).RunAsync();
await (new IndexAndLimit()).RunAsync();

await (new BatchUnicity()).RunAsync();
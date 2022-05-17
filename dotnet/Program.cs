﻿Config.Init();

await (new BeginStart()).RunAsync();
await (new BeginSimple()).RunAsync();
await (new BeginSerialization()).RunAsync();
await (new BeginAttributes()).RunAsync();
await (new BeginIndex()).RunAsync();
await (new BeingFindOneAndUpdate()).RunAsync();

await (new DynamicQuery()).RunAsync();
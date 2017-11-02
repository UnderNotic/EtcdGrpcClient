# EtcdGrpcClient ![appveyor status](https://ci.appveyor.com/api/projects/status/github/undernotic/etcdgrpcclient)
Simple yet useful .net etcd v3 grpc client

### Installing
https://www.nuget.org/packages/EtcdGrcpClient/

### Simple use example
``` csharp
var etcdAddress = new Uri("http://localhost:2381");
var etcdClient = new EtcdClient(etcdAddress);

await etcdClient.Put("key", "value");

var res = await etcdClient.Get("key");
Assert.Equal("value", res);
```

### Api
- Get
``` csharp
    await etcdClient.Put("key", "value");
    var res = await etcdClient.Get("test3");
    Assert.AreEqual("value", res); 
```
- GetRange
``` csharp
    //Range with one parameter(prefix) will return all keys with given prefix
    Enumerable.Range(0, 3).Select(async x => await etcdClient.Put("key" + x, "value" + x));
    var res = await etcdClient.GetRange("key");
    Assert.AreEqual(3, res.Count);

    //Range with two parameters will return all keys in given range INCLUDING
    Enumerable.Range(0, 3).Select(async x => await etcdClient.Put("key" + x, "value" + x));
    var res = await etcdClient.GetRange("key0, key1");
    Assert.AreEqual(2, res.Count);
```
- Watch
``` csharp
    var resultList = new List<EtcdWatchEvent[]>();
    var watcher = await etcdClient.Watch("key");
    watcher.Subscribe(e => resultList.Add(e));
    await etcdClient.Put("key", "value");

    Assert.AreEqual("key", resultList[0][0].Key);
    Assert.AreEqual("value", resultList[0][0].Value);
    Assert.AreEqual(EvenType.Put, resultList[0][0].Type);
```
- WatchRange
``` csharp
    //Watch with one parameter(prefix) will watch all keys with given prefix
    var resultList = new List<EtcdWatchEvent[]>();
    var watcher = await etcdClient.WatchRange("key");
    watcher.Subscribe(e => resultList.Add(e));

    Enumerable.Range(0, 3).Select(async x => await etcdClient.Put("key" + x, "value" + x));
    Assert.AreEqual(3, resultList.Count);

    //Watch with two parameters will watch all keys in given range INCLUDING
    var resultList = new List<EtcdWatchEvent[]>();
    Enumerable.Range(0, 3).Select(async x => await etcdClient.Put("key" + x, "value" + x));
    var watcher = await etcdClient.WatchRange("test0, test1");
    watcher.Subscribe(e => resultList.Add(e));
    Assert.AreEqual(2, res.Count);
```
- Delete

- DeleteRange

- Lease

## Authors

* **Piotr Szymura** - [undernotic](https://github.com/undernotic)

See also the list of [contributors](https://github.com/undernotic/etcdgrpcclient/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE) file for details

# <img height="90" alt="etcd grpc client" src="https://raw.githubusercontent.com/undernotic/etcdgrpcclient/master/img/logo.png">
[![appveyor status](https://ci.appveyor.com/api/projects/status/github/undernotic/etcdgrpcclient)](https://ci.appveyor.com/project/UnderNotic/etcdgrpcclient) [![NuGet](https://img.shields.io/nuget/vpre/EtcdGrpcClient.svg?style=plastic)](https://www.nuget.org/packages/EtcdGrpcClient)

Simple yet useful .net etcd v3 grpc client

### Installing
https://www.nuget.org/packages/EtcdGrpcClient/

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
- DeleteRange
``` csharp
    //Delete with one parameter(prefix) will delete all key with given prefix
    await etcdClient.Put("test1", string.Empty);
    await etcdClient.Put("test2", string.Empty);

    var deletedCount = etcdClient.DeleteRange("test");
    var getResult = await etcdClient.GetRange("test");

    Assert.AreEqual(3, deletedCount);
    Assert.AreEqual(0, get.Values.Count);

       //Delete with two parameters will delete keys in given range INCLUDING
    await etcdClient.Put("test1", string.Empty);
    await etcdClient.Put("test2", string.Empty);
    await etcdClient.Put("test3", string.Empty);

    var deletedCount = etcdClient.DeleteRange("test1", "test2");
    var getResult = await etcdClient.GetRange("test");

    Assert.AreEqual(2, deletedCount);
    Assert.AreEqual(1, get.Values.Count);
```

- Lease
``` csharp
    //Put with lease third parameter is TTL(time to live) in seconds
    var leaseId = await etcdClient.PutWithLease("test", "value", 1);
    var get = await etcdClient.Get("test");
    await Task.Delay(1100);

    Assert.ThrowsAsync<Exception>(async () => await etcdClient.Get("test"));
    Assert.AreEqual("value", get);


    //Keep alive lease to refresh TTL
     var leaseId = await etcdClient.PutWithLease("test", "value", 1);
    await Task.Delay(1100);
    await etcdClient.KeepAliveLease(leaseId);
    await Task.Delay(1100);
    await etcdClient.KeepAliveLease(leaseId);
    await Task.Delay(1100);
    var res = await etcdClient.Get("test");

    Assert.AreEqual("value", res);
```

## Authors

* **Piotr Szymura** - [undernotic](https://github.com/undernotic)

See also the list of [contributors](https://github.com/undernotic/etcdgrpcclient/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE) file for details

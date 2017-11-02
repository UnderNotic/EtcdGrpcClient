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

- GetRange

- Put

- Watch

- WatchRange

- Delete

- DeleteRange

- Lease

## Authors

* **Piotr Szymura** - [undernotic](https://github.com/undernotic)

See also the list of [contributors](https://github.com/undernotic/etcdgrpcclient/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE) file for details

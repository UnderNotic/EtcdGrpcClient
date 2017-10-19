using EtcdGrcpClient;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EtcdGrpcClient.Test.integration
{
    [TestFixture]
    public class EtcdWatchTest
    {
        private static Uri etcdAddress { get; } = new Uri("http://localhost:2381");
        private EtcdClient etcdClient = new EtcdClient(etcdAddress);

        [Test]
        public async Task ShouldWatchSingleItem()
        {
            var resp = Enumerable.Range(0, 10).Select(async x => await etcdClient.Put("test" + x, "value" + x));
            await Task.WhenAll(resp);
            
            var resultList = new List<EtcdWatchEvent[]>();
            var watcher = await etcdClient.Watch("test3");
            watcher.Subscribe(e => resultList.Add(e));

            await etcdClient.Put("test3", "value3");
            await etcdClient.Put("test2", "value2");

            await Task.Delay(500);
            watcher.Dispose();

            Assert.AreEqual(1, resultList.Count);
            Assert.AreEqual(1, resultList[0].Length);
        }

        [Test]
        public async Task ShouldWatchSinglMultipleItemsPrefix()
        {
            var resultList = new List<EtcdWatchEvent[]>();
            var watcher = await etcdClient.WatchRange("test");
            watcher.Subscribe(e => resultList.Add(e));

            await Task.WhenAll(Enumerable.Range(0, 10).Select(async x => await etcdClient.Put("test" + x, "value" + x)));
            await Task.Delay(500);
            Assert.AreEqual(10, resultList.Count);

            await Task.WhenAll(Enumerable.Range(0, 5).Select(async x => await etcdClient.Put("test" + x, "value" + x)));
            await Task.Delay(500);
            Assert.AreEqual(15, resultList.Count);

            await Task.WhenAll(Enumerable.Range(5, 5).Select(async x => await etcdClient.Put("test" + x, "value" + x)));
            await Task.Delay(500);
            Assert.AreEqual(20, resultList.Count);

            await Task.WhenAll(Enumerable.Range(1, 2).Select(async x => await etcdClient.Put("test" + x, "value" + x)));
            await etcdClient.Put("t", "t");
            await Task.Delay(500);
            watcher.Dispose();
            Assert.AreEqual(22, resultList.Count);

        }

        [Test]
        public async Task ShouldWatchMutlipleItemsFromTo()
        {
            var resultList = new List<EtcdWatchEvent[]>();
            var watcher = await etcdClient.WatchRange("test0", "test2");
            watcher.Subscribe(e => { Console.WriteLine(e[0].Value); resultList.Add(e); });

            await Task.WhenAll(Enumerable.Range(0, 10).Select(async x => await etcdClient.Put("test" + x, "value" + x)));
            await Task.Delay(1000);
            Assert.AreEqual(2, resultList.Count);

            await Task.WhenAll(Enumerable.Range(0, 5).Select(async x => await etcdClient.Put("test" + x, "value" + x)));
            await Task.Delay(1000);
            Assert.AreEqual(4, resultList.Count);

            await Task.WhenAll(Enumerable.Range(5, 5).Select(async x => await etcdClient.Put("test" + x, "value" + x)));
            await Task.Delay(1000);
            Assert.AreEqual(4, resultList.Count);

            await Task.WhenAll(Enumerable.Range(1, 2).Select(async x => await etcdClient.Put("test" + x, "value" + x)));
            await etcdClient.Put("t", "t");
            await Task.Delay(1000);

            watcher.Dispose();
            Assert.AreEqual(5, resultList.Count);
        }

        [Test]
        public async Task ShouldNotReadAfterDispose()
        {

        }

        [SetUp]
        public async Task CleanEtcd()
        {
            await EtcdTestHelper.DeleteAllRecords();
        }
    }
}

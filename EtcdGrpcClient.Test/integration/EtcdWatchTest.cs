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
        public async Task ShouldWatchMultipleItemsPrefix()
        {
            var resultList = new List<EtcdWatchEvent[]>();
            var watcher = await etcdClient.WatchRange("test");
            watcher.Subscribe(e => resultList.Add(e));

            await Task.WhenAll(Enumerable.Range(0, 10).Select(async x => await etcdClient.Put("test" + x, "value" + x)));
            await Task.WhenAll(Enumerable.Range(0, 5).Select(async x => await etcdClient.Put("test" + x, "value" + x)));
            await Task.WhenAll(Enumerable.Range(5, 5).Select(async x => await etcdClient.Put("test" + x, "value" + x)));
            await Task.WhenAll(Enumerable.Range(1, 2).Select(async x => await etcdClient.Put("test" + x, "value" + x)));
            await etcdClient.Put("t", "t");

            await Task.Delay(1500);
            watcher.Dispose();

            Assert.AreEqual(4, resultList.Count);
            Assert.AreEqual(10, resultList[0].Length);
            Assert.AreEqual(5, resultList[1].Length);
            Assert.AreEqual(5, resultList[2].Length);
            Assert.AreEqual(2, resultList[3].Length);
        }

        [Test]
        public async Task ShouldWatchMutlipleItemsFromTo()
        {
            var resultList = new List<EtcdWatchEvent[]>();
            var watcher = await etcdClient.WatchRange("test0", "test2");
            watcher.Subscribe(e => resultList.Add(e));

            await Task.WhenAll(Enumerable.Range(0, 10).Select(async x => await etcdClient.Put("test" + x, "value" + x)));
            await Task.WhenAll(Enumerable.Range(0, 5).Select(async x => await etcdClient.Put("test" + x, "value" + x)));
            await Task.WhenAll(Enumerable.Range(5, 5).Select(async x => await etcdClient.Put("test" + x, "value" + x)));
            await Task.WhenAll(Enumerable.Range(1, 2).Select(async x => await etcdClient.Put("test" + x, "value" + x)));
            await etcdClient.Put("t", "t");

            await Task.Delay(500);
            watcher.Dispose();
            Assert.AreEqual(3, resultList.Count);
            Assert.AreEqual(3, resultList[0].Length);
            Assert.AreEqual(3, resultList[1].Length);
            Assert.AreEqual(2, resultList[2].Length);
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

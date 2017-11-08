using EtcdGrcpClient;
using EtcdGrpcClient.Test.integration;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EtcdGrpcClient.Test
{
    [TestFixture]
    public class EtcdGetTest
    {
        private static Uri etcdAddress { get; } = new Uri("http://localhost:2381");
        private EtcdClient etcdClient = new EtcdClient(etcdAddress);

        [Test]
        public async Task ShouldReturnOneItem()
        {
            var resp = Enumerable.Range(0, 11).Select(async x => await etcdClient.Put("test" + x, "value" + x));
            await Task.WhenAll(resp);
            var res1 = await etcdClient.Get("test3");
            var res2 = await etcdClient.Get("test10");

            Assert.AreEqual("value3", res1);
            Assert.AreEqual("value10", res2);
        }

        [Test]
        public void ShouldThrownWhenKeyDoesNotExist()
        {
            Assert.ThrowsAsync<Exception>(async () => await etcdClient.Get("test"));
        }

        [Test]
        public async Task ShouldReturnRangePrefix()
        {
            var resp = Enumerable.Range(0, 10).Select(async x => await etcdClient.Put("test" + x, "value" + x))
                .Concat(Enumerable.Range(65, 10).Select(async x => await etcdClient.Put("test" + (char)x, "value" + x)));
            await Task.WhenAll(resp);
            var res1 = await etcdClient.GetRange("test");
            var res2 = await etcdClient.GetRange("testA");

            Assert.AreEqual(20, res1.Count);
            CollectionAssert.AreEquivalent(Enumerable.Range(0, 10).Select(x => "value" + x).Concat(Enumerable.Range(65, 10).Select(x => "value" + x)).ToArray(), res1.Values.ToArray());
            Assert.AreEqual(1, res2.Count);
            Assert.AreEqual("value65", res2["testA"]);
        }

        [Test]
        public async Task ShouldReturnRangeFromTo()
        {
            var resp = Enumerable.Range(0, 10).Select(async x => await etcdClient.Put("test" + x, "value1" + x))
                .Concat(Enumerable.Range(65, 10).Select(async x => await etcdClient.Put("test" + (char)x, "value2" + x)));
            await Task.WhenAll(resp);

            var res1 = await etcdClient.GetRange("test0", "test4");
            var res2 = await etcdClient.GetRange("testB", "testD");

            Assert.AreEqual(5, res1.Count);
            CollectionAssert.AreEquivalent(Enumerable.Range(0, 5).Select(x => "value1" + x).ToArray(), res1.Values.ToArray());
            Assert.AreEqual(3, res2.Count);
            CollectionAssert.AreEquivalent(Enumerable.Range(66, 3).Select(x => "value2" + x).ToArray(), res2.Values.ToArray());
        }

        [SetUp]
        public async Task Cleanup()
        {
            await EtcdTestHelper.DeleteAllRecords();
        }

    }
}

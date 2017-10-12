using EtcdGrcpClient;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace EtcdGrpcClient.Test.integration
{

    [TestFixture]
    public class EtcdFallbackTest
    {
        private static Uri etcdAddress { get; } = new Uri("http://localhost:2381");
        private EtcdClient etcdClient = new EtcdClient(etcdAddress);

        public async Task ShouldDeleteAllKeys()
        {
            var r = new Random();
            await etcdClient.Put("test1", r.Next().ToString());
            await etcdClient.Put("test2", r.Next().ToString());
            await etcdClient.Put("test3", r.Next().ToString());

            var deletedCount = await etcdClient.DeleteRange("test");
            var get = await etcdClient.GetRange("test");

            Assert.AreEqual(3, deletedCount);
            Assert.AreEqual(0, get.Values.Count);
        }

        [SetUp]
        public async Task CleanupAsync()
        {
            await EtcdTestHelper.DeleteAllRecords();
        }
    }
}

using EtcdGrcpClient;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace EtcdGrpcClient.Test.integration
{
    [TestFixture]
    public class EtcdLeaseTest
    {
        private static Uri etcdAddress { get; } = new Uri("http://localhost:2381");
        private EtcdClient etcdClient = new EtcdClient(etcdAddress);

        [Test]
        public async Task ShouldExtendLease()
        {
            var leaseId = await etcdClient.PutWithLease("test", "value", 1);
            await Task.Delay(1100);
            await etcdClient.KeepAliveLease(leaseId);
            await Task.Delay(1100);
            await etcdClient.KeepAliveLease(leaseId);
            await Task.Delay(1100);
            var res = await etcdClient.Get("test");

            Assert.AreEqual("value", res);
        }

        [Test]
        public async Task ShouldDeleteExpiriedKey()
        {
            var leaseId = await etcdClient.PutWithLease("test", "value", 1);
            await Task.Delay(12000);

            Assert.ThrowsAsync<Exception>(async() => await etcdClient.Get("test"));
        }

        [SetUp]
        public async Task CleanEtcd()
        {
            await EtcdTestHelper.DeleteAllRecords();
        }
    }
}

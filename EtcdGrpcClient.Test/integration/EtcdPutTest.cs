using EtcdGrpcClient;
using EtcdGrpcClient.Test.integration;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EtcdGrpcClient.Test
{
    [TestFixture]
    public class EtcdPutTest
    {
        private static Uri etcdAddress { get; } = new Uri("http://localhost:2381");
        private EtcdClient etcdClient = new EtcdClient(etcdAddress);

        [Test]
        public async Task ShouldNotOverwriteKeys()
        {
            var resp = Enumerable.Range(0, 100).Select(async x => await etcdClient.Put("test" + x, "value" + x))
                .Concat(Enumerable.Range(0, 105).Select(async x => await etcdClient.Put("test" + x, "value" + x)))
                .Concat(Enumerable.Range(0, 115).Select(async x => await etcdClient.Put("test" + x, "value" + x)));

            await Task.WhenAll(resp);
            var res = await etcdClient.GetRange("test");

            Assert.AreEqual(115, res.Count);
        }

        [SetUp]
        public async Task CleanEtcd()
        {
            await EtcdTestHelper.DeleteAllRecords();

        }

    }
}

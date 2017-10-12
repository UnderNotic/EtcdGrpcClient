using EtcdGrcpClient;
using EtcdGrpcClient.Test.integration;
using NUnit.Framework;
using System.Threading.Tasks;

namespace EtcdGrpcClient.Test
{
    [TestFixture]
    public class EtcdDeleteTest
    {
        private EtcdClient etcdClient = new EtcdClient(EtcdTestHelper.EtcdAddress);

        [Test]
        public async Task ShouldDeleteAllKeys()
        {
            await etcdClient.Put("test1", string.Empty);
            await etcdClient.Put("test2", string.Empty);
            await etcdClient.Put("test3", string.Empty);

            var deletedCount = await EtcdTestHelper.DeleteAllRecords();
            var get = await etcdClient.GetRange("test");

            Assert.AreEqual(3, deletedCount);
            Assert.AreEqual(0, get.Values.Count);
        }

        [Test]
        public async Task ShouldDeleteRangePrefix()
        {
            await etcdClient.Put("test1", string.Empty);
            await etcdClient.Put("test2", string.Empty);
            await etcdClient.Put("test22", string.Empty);
            await etcdClient.Put("test3", string.Empty);
            await etcdClient.Put("test4", string.Empty);

            var deletedCount1 = await etcdClient.DeleteRange("test2");
            var deletedCount2 = await etcdClient.DeleteRange("test4");

            Assert.AreEqual(2, deletedCount1);
            Assert.AreEqual(1, deletedCount2);
        }

        [Test]
        public async Task ShouldDeleteRangeFromTo()
        {
            await etcdClient.Put("test1", string.Empty);
            await etcdClient.Put("test2", string.Empty);
            await etcdClient.Put("test3", string.Empty);
            await etcdClient.Put("a", string.Empty);
            await etcdClient.Put("b", string.Empty);
            await etcdClient.Put("c", string.Empty);
            await etcdClient.Put("f", string.Empty);

            var deletedCount1 = await etcdClient.DeleteRange("test1", "test3");
            var deletedCount2 = await etcdClient.DeleteRange("a", "b");
            var deletedCount3 = await etcdClient.DeleteRange("e", "h");

            Assert.AreEqual(3, deletedCount1);
            Assert.AreEqual(2, deletedCount2);
            Assert.AreEqual(1, deletedCount3);
        }

        [SetUp]
        public async Task Cleanup()
        {
            await EtcdTestHelper.DeleteAllRecords();
        }
    }
}

using EtcdGrcpClient;
using System;
using Microsoft.Extensions.Configuration;

using System.Threading.Tasks;

namespace EtcdGrpcClient.Test.integration
{
    public static class EtcdTestHelper
    {
        public static Uri EtcdAddress { get; }
        private static EtcdClient etcdClient { get; }

        static EtcdTestHelper()
        {
            var config = new ConfigurationBuilder()
                        .AddJsonFile("App.config.json")
                        .Build();

            EtcdAddress = new Uri(config["ETCD_ADDRESS"]);
            etcdClient = new EtcdClient(EtcdAddress);
        }

        public static Task<long> DeleteAllRecords()
        {
            return etcdClient.DeleteRange("A", "z");
        }
    }
}

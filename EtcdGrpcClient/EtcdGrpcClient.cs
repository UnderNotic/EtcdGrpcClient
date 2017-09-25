using System;
using System.Net;
using System.Threading.Tasks;
using Grpc.Core;
using EtcdGrcpClient;
using System.Collections.Generic;
using Google.Protobuf;
using Etcdserverpb;
using static Etcdserverpb.Lease;
using static Etcdserverpb.Watch;

namespace EtcdGrcpClient
{
    public class EtcdClient : IDisposable
    {
        private readonly DnsEndPoint[] etcdServerEndpoints = new[] { new DnsEndPoint("localhost", 2381), new DnsEndPoint("localhost", 2391) };
        private readonly Channel channel;
        private readonly KV.KVClient kvClient;
        private readonly WatchClient watchClient;
        private readonly LeaseClient leaseClient;
        private readonly EtcdLeaseTTLRefresher leaseTTLRefresher;

        public EtcdClient()
        {
            this.channel = new Channel(etcdServerEndpoints[0].Host, etcdServerEndpoints[0].Port, ChannelCredentials.Insecure);
            this.kvClient = new KV.KVClient(channel);
            this.watchClient = new WatchClient(this.channel);
            this.leaseClient = new LeaseClient(this.channel);
            var asyncDuplexStreamingCall = leaseClient.LeaseKeepAlive();
            this.leaseTTLRefresher = new EtcdLeaseTTLRefresher(asyncDuplexStreamingCall);
        }

        public async Task<PutResponse> Put(string key, string value, long leaseId = 0)
        {
            return await kvClient.PutAsync(new PutRequest() { Key = ByteString.CopyFromUtf8(key), Value = ByteString.CopyFromUtf8(value), Lease = leaseId });
        }

        public async Task<long> PutWithLease(string key, string value, int TTL)
        {
            var leaseResp = await leaseClient.LeaseGrantAsync(new LeaseGrantRequest() { TTL = TTL });
            await Put(key, value, leaseResp.ID);
            return leaseResp.ID;
        }

        public Task KeepAliveLease(long leaseId)
        {
            return this.leaseTTLRefresher.RefreshLease(leaseId); // i think they need to be managed internally
        }

        public async Task<string> Get(string key)
        {
            var resp = await kvClient.RangeAsync(new RangeRequest() { Key = ByteString.CopyFromUtf8(key) });
            try
            {
                return resp.Kvs[0].Value.ToStringUtf8();
            }
            catch (Exception ex)
            {
                throw new Exception($"Key {key} doesn't exist in etcd - {ex.Message}");
            }
        }

        public async Task<IDictionary<string, string>> GetRange(string fromKey, string toKey)
        {
            var resp = await kvClient.RangeAsync(new RangeRequest() { Key = ByteString.CopyFromUtf8(fromKey), RangeEnd = ByteString.CopyFromUtf8(toKey) });
            return RangeRespondToDictionary(resp);
        }

        public async Task<IDictionary<string, string>> GetRange(string prefixKey)
        {
            string rangeEnd = GetRangeEndForPrefix(prefixKey);

            var resp = await kvClient.RangeAsync(new RangeRequest() { Key = ByteString.CopyFromUtf8(prefixKey), RangeEnd = ByteString.CopyFromUtf8(rangeEnd) });
            return RangeRespondToDictionary(resp);
        }


        public async Task<EtcdWatcher> Watch(string key)
        {
            var watchRequest = new WatchRequest() { CreateRequest = new WatchCreateRequest() { Key = ByteString.CopyFromUtf8(key) } };
            var watcher = watchClient.Watch();
            await watcher.RequestStream.WriteAsync(watchRequest);
            return new EtcdWatcher(key, watcher);
        }

        public async Task<EtcdWatcher> WatchRange(string prefixKey)
        {
            string rangeEnd = GetRangeEndForPrefix(prefixKey);

            var watchRequest = new WatchRequest() { CreateRequest = new WatchCreateRequest() { Key = ByteString.CopyFromUtf8(prefixKey), RangeEnd = ByteString.CopyFromUtf8(rangeEnd) } };
            var watcher = watchClient.Watch();
            await watcher.RequestStream.WriteAsync(watchRequest);
            return new EtcdWatcher($"prefix-{prefixKey}", watcher);
        }

        public async void Dispose()
        {
            this.leaseTTLRefresher.Dispose();
            await this.channel.ShutdownAsync();
        }

        private static IDictionary<string, string> RangeRespondToDictionary(RangeResponse resp)
        {
            var resDictionary = new Dictionary<string, string>();
            foreach (var kv in resp.Kvs)
            {
                var key = kv.Key.ToStringUtf8();
                var value = kv.Value.ToStringUtf8();
                resDictionary.Add(key, value);

            }
            return resDictionary;
        }

        private string GetRangeEndForPrefix(string prefixKey)
        {
            var charArray = prefixKey.ToCharArray();
            charArray[charArray.Length - 1] = ++charArray[charArray.Length - 1];
            string rangeEnd = new string(charArray);
            return rangeEnd;
        }
    }
}

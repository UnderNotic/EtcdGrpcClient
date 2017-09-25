using Etcdserverpb;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using static Mvccpb.Event.Types;

namespace EtcdGrcpClient
{
    public class EtcdWatchEvent
    {
        public string Key { get; }
        public string Value { get; }
        public EventType Type { get; }

        public EtcdWatchEvent(string key, string value, EventType type)
        {
            Key = key;
            Value = value;
            Type = type;
        }
    }

    public class EtcdWatcher : IDisposable
    {
        public string WatchedKey { get; }

        private AsyncDuplexStreamingCall<WatchRequest, WatchResponse> duplexCall;
        private List<Action<EtcdWatchEvent[]>> actions = new List<Action<EtcdWatchEvent[]>>();

        public EtcdWatcher(string watchedKey, AsyncDuplexStreamingCall<WatchRequest, WatchResponse> duplexCall)
        {
            WatchedKey = watchedKey;
            this.duplexCall = duplexCall;
            Watch(duplexCall.ResponseStream);
        }

        public async void Watch(IAsyncStreamReader<WatchResponse> responseStream)
        {
            while (await responseStream.MoveNext())
            {
                var watchEvents = responseStream.Current.Events.Select(ev =>
                    {
                        var key = ev.Kv.Key.ToStringUtf8();
                        var value = ev.Kv.Value.ToStringUtf8();
                        var type = ev.Type;
                        return new EtcdWatchEvent(key, value, type);
                    }
                ).ToArray();
                actions.ForEach(a => a(watchEvents));
            }
        }

        public void Subscribe(Action<EtcdWatchEvent[]> action)
        {
            actions.Add(action);
        }

        public void Dispose()
        {
            this.duplexCall.Dispose();
        }
    }
}

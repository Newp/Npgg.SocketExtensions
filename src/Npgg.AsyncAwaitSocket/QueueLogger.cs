using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgg.Elasticsearch.Extensions;
using Npgg.Elasticsearch;
using System.Threading;
using System.Reactive.Linq;

namespace Npgg.AsyncAwaitSocket
{
    public class QueueLogger
    {
        readonly HttpClient httpClient;
        readonly ConcurrentQueue<object> Queue = new();
        readonly CancellationTokenSource cts = new();
        public QueueLogger(HttpClient httpClient)
        {
            Observable
                .Interval(TimeSpan.FromSeconds(1))
                .Subscribe(async ticks => Streaming(), cts.Token);
            this.httpClient = httpClient;
        }

        public async Task Streaming()
        {
            if (Queue.Count == 0)
                return;

            StringBuilder sb = new StringBuilder();

            for (int i =0; i< 100;i++)
            {
                if(Queue.TryDequeue(out var document) == false)
                    break;

                sb.AddIndex("test1", document);
            }

            var response= await httpClient.BulkAsync(sb.ToString());

            if(response.Errors)
            {

            }
        }
    }
}

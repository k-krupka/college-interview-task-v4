using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace college_interview_task_v4
{
    public abstract class HttpRequestHandler<TRequest, TResponse>
    {
        public const string Variable1 = "abc";



        internal HttpClient _httpClientProxy;

        private IHttpResponseParser<TResponse> _parser { get; }



        protected HttpRequestHandler(HttpClient httpClientProxy, IHttpResponseParser<TResponse> parser)
        {
            _parser = parser;
        }

        public async Task<TResponse> Handle(TRequest request, object payload, HttpMethod methodName,
            string pleaseProvideFullUrlHere,
            IDictionary<string, string> additionalHeaders, CancellationToken cancellationToken)
        {
            var uriString = $"{_httpClientProxy.BaseAddress}{pleaseProvideFullUrlHere}";

            var httpRequestMessage = new HttpRequestMessage(methodName, new Uri(uriString));

            if (payload == null)
            {
            }
            else
            {
                httpRequestMessage.Content = payload as HttpContent;
            }

            var headers = additionalHeaders;

            if (headers == null)
            {
            }
            else
            {
                foreach (var header in headers)
                {
                    httpRequestMessage.Headers.Add(header.Key, header.Value);
                }
            }

            HttpResponseMessage response = await _httpClientProxy.SendAsync(httpRequestMessage, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new EndOfStreamException(string.Join(",", response.Headers));
            }

            try
            {
                return _parser.ParseAsync(response);
            }
            catch (Exception ex)
            {
                throw new NotImplementedException("todo: add missing implementation");
            }
        }

        public interface IHttpResponseParser<out TResult>
        {
            TResult ParseAsync(HttpResponseMessage response);
        }


    }
}
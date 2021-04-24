using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LCUSharp.Http
{
    /// <summary>
    /// A request handler that supports authentication.
    /// </summary>
    internal abstract class RequestHandler
    {
        private readonly HttpClientHandler _httpClientHandler;

        /// <summary>
        /// The HttpClient used to make requests.
        /// </summary>
        protected HttpClient HttpClient { get; private set; }

        /// <summary>
        /// Creates a new instance of the <see cref="RequestHandler"/> class.
        /// </summary>
        public RequestHandler()
        {
            _httpClientHandler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual
            };
            _httpClientHandler.ServerCertificateCustomValidationCallback = (response, cert, chain, errors) => true;
            CreateHttpClient();
        }

        /// <summary>
        /// Creates a new <see cref="HttpClient"/>.
        /// </summary>
        protected void CreateHttpClient()
        {
            HttpClient = new HttpClient(_httpClientHandler);
            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Prepares a request by calculating the proper url.
        /// </summary>
        /// <typeparam name="TRequest">The object to serialize into the body.</typeparam>
        /// <param name="relativeUrl">The relative url.</param>
        /// <param name="httpMethod">The <see cref="HttpMethod"/>.</param>
        /// <param name="body">The request's body.</param>
        /// <param name="queryParameters">The query parameters.</param>
        /// <returns></returns>
        protected async Task<HttpRequestMessage> PrepareRequestAsync<TRequest>(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters, TRequest body)
        {
            var url = queryParameters == null
                ? relativeUrl
                : relativeUrl + BuildQueryParameterString(queryParameters);
            var request = new HttpRequestMessage(httpMethod, url);

            if (body != null)
            {
                var json = await Task.Run(() => JsonConvert.SerializeObject(body)).ConfigureAwait(false);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            return request;
        }

        /// <summary>
        /// Builds the query parameter string that's appended to the request url.
        /// </summary>
        /// <param name="queryParameters">The query parameters.</param>
        /// <returns>The query parameter url string.</returns>
        protected string BuildQueryParameterString(IEnumerable<string> queryParameters)
        {
            return "?" + string.Join("&", queryParameters.Where(a => !string.IsNullOrWhiteSpace(a)));
        }

        /// <summary>
        /// Gets a <see cref="HttpResponseMessage"/>'s content.
        /// </summary>
        /// <param name="response">The <see cref="HttpResponseMessage"/>.</param>
        /// <returns>The <see cref="HttpResponseMessage"/>'s content.</returns>
        protected async Task<string> GetResponseContentAsync(HttpResponseMessage response)
        {
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
    }
}

using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace LCUSharp.Http
{
    /// <summary>
    /// A request handler for the league client that requires the client's port and the user's Basic authentication token.
    /// </summary>
    public interface ILeagueRequestHandler
    {
        /// <summary>
        /// The league client's port.
        /// </summary>
        int Port { get; set; }

        /// <summary>
        /// The user's Basic authentication token
        /// </summary>
        string Token { get; set; }

        /// <summary>
        /// Changes the request handler's settings.
        /// </summary>
        /// <param name="port">The league client's port.</param>
        /// <param name="token">The user's Basic authentication token.</param>
        void ChangeSettings(int port, string token);

        /// <summary>
        /// Creates and sends a new <see cref="HttpRequestMessage"/> and returns the <see cref="HttpResponseMessage"/>'s content.
        /// </summary>
        /// <param name="httpMethod">The <see cref="HttpMethod"/>.</param>
        /// <param name="relativeUrl">The relative url.</param>
        /// <param name="queryParameters">The query parameters.</param>
        /// <returns>The <see cref="HttpResponseMessage"/>'s content.</returns>
        Task<string> GetJsonResponseAsync(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters = null);

        /// <summary>
        /// Creates and sends a new <see cref="HttpRequestMessage"/> and returns the <see cref="HttpResponseMessage"/>'s content.
        /// </summary>
        /// <typeparam name="TRequest">The object to serialize into the body.</typeparam>
        /// <param name="httpMethod">The <see cref="HttpMethod"/>.</param>
        /// <param name="relativeUrl">The relative url.</param>
        /// <param name="queryParameters">The query parameters.</param>
        /// <param name="body">The request's body.</param>
        /// <returns>The <see cref="HttpResponseMessage"/>'s content.</returns>
        Task<string> GetJsonResponseAsync<TRequest>(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters, TRequest body);

        /// <summary>
        /// Creates and sends a new <see cref="HttpRequestMessage"/> and deserializes the <see cref="HttpResponseMessage"/>'s content (json) as <typeparamref name="TResponse"/>.
        /// </summary>
        /// <typeparam name="TResponse">The object to deserialize the response into.</typeparam>
        /// <param name="httpMethod">The <see cref="HttpMethod"/>/</param>
        /// <param name="relativeUrl">The relative url.</param>
        /// <param name="queryParameters">The query parameters.</param>
        /// <returns>The deserialized response.</returns>
        Task<TResponse> GetResponseAsync<TResponse>(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters = null);

        /// <summary>
        /// Creates and sends a new <see cref="HttpRequestMessage"/> and deserializes the <see cref="HttpResponseMessage"/>'s content (json) as <typeparamref name="TResponse"/>.
        /// </summary>
        /// <typeparam name="TRequest">The object to serialize into the body.</typeparam>
        /// <typeparam name="TResponse">The object to deserialize the response into.</typeparam>
        /// <param name="httpMethod">The <see cref="HttpMethod"/>/</param>
        /// <param name="relativeUrl">The relative url.</param>
        /// <param name="queryParameters">The query parameters.</param>
        /// <param name="body">The request's body.</param>
        /// <returns>The deserialized response.</returns>
        Task<TResponse> GetResponseAsync<TRequest, TResponse>(HttpMethod httpMethod, string relativeUrl, IEnumerable<string> queryParameters, TRequest body);
    }
}

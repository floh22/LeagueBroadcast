namespace LCUSharp.Http.Endpoints
{
    /// <summary>
    /// An endpoint within the league client's API.
    /// </summary>
    internal abstract class EndpointBase
    {
        /// <summary>
        /// The request handler.
        /// </summary>
        protected ILeagueRequestHandler RequestHandler { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointBase"/> class.
        /// </summary>
        /// <param name="requestHandler">The request handler.</param>
        public EndpointBase(ILeagueRequestHandler requestHandler)
        {
            RequestHandler = requestHandler;
        }
    }
}

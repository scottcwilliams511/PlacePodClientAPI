using System.Threading.Tasks;

namespace PlacePodApiClient.Lib {

    /// <summary>
    /// Http request based interface that supports GET, POST, PUT, and DELETE methods.
    /// </summary>
    public interface IHttpAsync {

        /// <summary>
        /// Base URL of the PlacePod API
        /// </summary>
        string BaseRoute { get; }

        /// <summary>
        /// X-API-KEY authentication token.
        /// </summary>
        string ApiKey { get; }

        /// <summary>
        /// Method for making a HTTP 'Get' request to the API.
        /// </summary>
        /// <param name="path">Path to be appended to the BaseRoute</param>
        Task<string> Get(string path);

        /// <summary>
        /// Method for making a HTTP 'Post' request to the API.
        /// </summary>
        /// <param name="path">Path to be appended to the BaseRoute</param>
        /// <param name="body">JSON string payload</param>
        Task<string> Post(string path, string body);

        /// <summary>
        /// Method for making a HTTP 'Put' request to the API.
        /// </summary>
        /// <param name="path">Path to be appended to the BaseRoute</param>
        /// <param name="body">JSON string payload</param>
        Task<string> Put(string path, string body);

        /// <summary>
        /// Method for making a HTTP 'Delete' request to the API.
        /// </summary>
        /// <param name="path">Path to be appended to the BaseRoute</param>
        /// <param name="body">JSON string payload</param>
        Task<string> Delete(string path, string body);
    }
}

using System;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PlacePodApiClient.Lib {

    /// <summary>
    /// Class for making http requests. Inner logic is abstracted away so that 
    /// one only needs to initially pass in an API url and key. Then the appropriate
    /// method can be called, in which the method route and parameters are passed
    /// 
    /// Supports HTTP GET, POST, PUT, and DELETE methods.
    /// </summary>
    public class HttpAsync : IHttpAsync {

        /// <summary>
        /// Base URL of the PlacePod API
        /// </summary>
        public string BaseRoute { get; }

        /// <summary>
        /// X-API-KEY authentication token.
        /// </summary>
        public string ApiKey { get; }

        /// <summary>
        /// Provides a base class for sending HTTP requests and receiving HTTP responses from a resource identified by a URI.
        /// </summary>
        private readonly HttpClient client;

        /// <summary>
        /// Creates a new instance of HttpAsync. Both parameters should be provided.
        /// </summary>
        public HttpAsync(string baseRoute, string apiKey) {
            BaseRoute = baseRoute;
            ApiKey = apiKey;

            client = new HttpClient();
        }


        /// <summary>
        /// Method for making a HTTP 'Get' request to the API.
        /// </summary>
        /// <param name="path">Path to be appended to the BaseRoute</param>
        /// <returns>Result of the request</returns>
        public Task<string> Get(string path) {
            return RequestAsync(path, null, "GET");
        }


        /// <summary>
        /// Method for making a HTTP 'Post' request to the API.
        /// </summary>
        /// <param name="path">Path to be appended to the BaseRoute</param>
        /// <param name="body">JSON string payload</param>
        /// <returns>Result of the request</returns>
        public Task<string> Post(string path, string body) {
            return RequestAsync(path, body, "POST");
        }


        /// <summary>
        /// Method for making a HTTP 'Put' request to the API.
        /// </summary>
        /// <param name="path">Path to be appended to the BaseRoute</param>
        /// <param name="body">JSON string payload</param>
        /// <returns>Result of the request</returns>
        public Task<string> Put(string path, string body) {
            return RequestAsync(path, body, "PUT");
        }


        /// <summary>
        /// Method for making a HTTP 'Delete' request to the API.
        /// </summary>
        /// <param name="path">Path to be appended to the BaseRoute</param>
        /// <param name="body">JSON string payload</param>
        /// <returns>Result of the request</returns>
        public Task<string> Delete(string path, string body) {
            return RequestAsync(path, body, "DELETE");
        }


        /// <summary>
        /// Generic asynchronous HTTP request method.
        /// </summary>
        /// <param name="path">Route of API method to call</param>
        /// <param name="body">Optional JSON parameters to be sent</param>
        /// <param name="httpMethod">"GET", "POST", "PUT", or "DELETE"</param>
        /// <returns>dynamic JArray</returns>
        private async Task<string> RequestAsync(string path, string body, string httpMethod) {
            try {
                using (HttpRequestMessage request = new HttpRequestMessage()) {
                    request.RequestUri = new Uri(BaseRoute + path);
                    request.Method = new HttpMethod(httpMethod);
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    if (ApiKey != null) {
                        request.Headers.Add("X-API-KEY", ApiKey);
                    }

                    // Only add data if the user passed some, ie POST call
                    if (!string.IsNullOrWhiteSpace(body)) {
                        request.Content = new StringContent(body, Encoding.UTF8, "application/json");
                    }

                    using (HttpResponseMessage response = await client.SendAsync(request)) {
                        string responseMsg = await response.Content.ReadAsStringAsync();

                        int status = (int)response.StatusCode;
                        if (status != 200 && status != 202) {
                            // If the API doesn't return code 200 or 202, then something went wrong
                            string message = "";

                            // Try to make the error message more readable if possible.
                            if (!string.IsNullOrWhiteSpace(responseMsg)) {
                                dynamic errorJson = JsonConvert.DeserializeObject(responseMsg);
                                message = (errorJson.Code != null && errorJson.Message != null)
                                    ? $"\"HTTP ERROR {status} \" - \" {errorJson.Code}: {errorJson.Message}"
                                    : $"\"HTTP ERROR {status} \" - \" {responseMsg}";
                            } else {
                                message = $"\"HTTP ERROR {status} \" - \" {response.ReasonPhrase}";
                            }

                            throw new Exception(message);
                        }
                        return JsonConvert.DeserializeObject(responseMsg).ToString();
                    }
                }
            } catch {
                Console.WriteLine($"Http {httpMethod} error");
                throw;
            }
        }
    }
}

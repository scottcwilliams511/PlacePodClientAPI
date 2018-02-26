using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

/// <author>Scott Williams</author>
namespace Http_Async {
    /// <summary>
    /// Class for making http requests. Inner logic is abstracted away so that 
    /// one only needs to initially pass in an API url and key. Then the appropriate
    /// method can be called, in which the method route and parameters are passed
    /// 
    /// Supports HTTP GET, POST, PUT, and DELETE methods.
    /// </summary>
    internal class HttpAsync {
        public string API_SERVER { private get; set; }
        public string API_KEY { private get; set; }

        /// <summary>
        /// Default constructor. API_SERVER must be set later before making requests. API_KEY must be set
        /// for requests that require a header API key.
        /// </summary>
        public HttpAsync() {
        }


        /// <summary>
        /// Parameterized constructor. Use this constructor if you already know both the
        /// API_SERVER and API_KEY
        /// </summary>
        /// <param name="api_url"></param>
        /// <param name="api_key"></param>
        public HttpAsync(string api_url, string api_key) {
            API_SERVER = api_url;
            API_KEY = api_key;
        }


        /// <summary>
        /// Method for making a HTTP 'Get' request to the API
        /// </summary>
        /// <param name="path">Route for the API method</param>
        /// <returns>Result of the request</returns>
        public Task<string> Get(string path) {
            return AsyncHttpRequest(path, null, "GET");
        }


        /// <summary>
        /// Method for making a HTTP 'Post' request to the API
        /// </summary>
        /// <param name="path">Route for the API method</param>
        /// <returns>Result of the request</returns>
        public async Task<string> Post(string path, string data) {
            return await AsyncHttpRequest(path, data, "POST");
        }


        /// <summary>
        /// Method for making a HTTP 'Put' request to the API
        /// </summary>
        /// <param name="path">Route for the API method</param>
        /// <returns>Result of the request</returns>
        public async Task<string> Put(string path, string data) {
            return await AsyncHttpRequest(path, data, "PUT");
        }


        /// <summary>
        /// Method for making a HTTP 'Delete' request to the API
        /// </summary>
        /// <param name="path">Route for the API method</param>
        /// <returns>Result of the request</returns>
        public async Task<string> Delete(string path, string data) {
            return await AsyncHttpRequest(path, data, "DELETE");
        }


        /// <summary>
        /// Generic asynchronous HTTP request method.
        /// </summary>
        /// <param name="path">Route of API method to call</param>
        /// <param name="data">Optional JSON parameters to be sent</param>
        /// <param name="method">"GET", "POST", "PUT", or "DELETE"</param>
        /// <returns>dynamic JArray</returns>
        private async Task<string> AsyncHttpRequest(string path, string data, string method) {
            try {
                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage() {
                    RequestUri = new Uri(API_SERVER + path),
                    Method = new HttpMethod(method)
                };
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (API_KEY != null) {
                    request.Headers.Add("X-API-KEY", API_KEY);
                }

                // Only add data if the user passed some, ie POST call
                if (data != null) {
                    request.Content = new StringContent(data, Encoding.UTF8, "application/json");
                }

                HttpResponseMessage response = await client.SendAsync(request);
                string responseMsg = await response.Content.ReadAsStringAsync();

                // If the API doesn't return code 200, then something went wrong
                int status = (int)response.StatusCode;
                if (status != 200) {
                    if (status == 401) {
                        throw new Exception("\"HTTP Error 401\" - Unauthorized: Access is denied due to invalid credentials.");
                    } else if (status == 404) {
                        throw new Exception("\"HTTP Error 404\" - Not Found.");
                    } else if (status == 415) {
                        throw new Exception("\"HTTP Error 415\" - Unsupported media type.");
                    } else {
                        string message = null;
                        try {
                            // Try to make the error message more readable if possible.
                            dynamic errorJson = JsonConvert.DeserializeObject(responseMsg);
                            message = "\"HTTP ERROR " + status + "\" - " + errorJson.Code + ": " + errorJson.Message;
                        } catch {
                            message = "\"HTTP ERROR " + status + "\" - " + responseMsg;
                        }
                        throw new Exception(message);
                    }
                }

                return responseMsg;
            } catch {
                Console.WriteLine("Http " + method + " error");
                throw;
            }
        }
    }
}
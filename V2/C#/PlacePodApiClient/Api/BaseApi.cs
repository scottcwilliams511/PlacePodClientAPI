using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

using PlacePodApiClient.Lib;

namespace PlacePodApiClient.Api {

    /// <summary>
    /// Base class containing generic CRUD methods for resources.
    /// </summary>
    public abstract class BaseApi {

        /// <summary>
        /// The base path of the specific resource.
        /// </summary>
        public readonly string Route;

        protected readonly IHttpAsync Http;

        public BaseApi(IHttpAsync http, string route) {
            Http = http;
            Route = route;
        }


        /// <summary>
        /// Get all of the specific resource.
        /// </summary>
        /// <typeparam name="T">Type of resource to get.</typeparam>
        public async Task<ICollection<T>> Get<T>() {
            try {
                string response = await Http.Get(Route);

                try {
                    return JsonConvert.DeserializeObject<ICollection<T>>(response);
                } catch (Exception ex) {
                    Console.WriteLine($"Couldn't create {typeof(T)} object: {ex}");
                    return new List<T>();
                }
            } catch {
                Console.WriteLine($"Couldn't get {GetType().Name.Replace("Api", "s")}.");
                throw;
            }
        }


        /// <summary>
        /// Get a specific resource by id.
        /// </summary>
        /// <typeparam name="T">Type of resource to get.</typeparam>
        /// <param name="id">Id of the resource.</param>
        public async Task<T> Get<T>(string id) {
            try {
                string response = await Http.Get($"{Route}/{id}");

                try {
                    return JsonConvert.DeserializeObject<T>(response);
                } catch (Exception ex) {
                    Console.WriteLine($"Couldn't create {typeof(T)} object: {ex}");
                    return default;
                }
            } catch {
                Console.WriteLine($"Couldn't get {GetType().Name.Replace("Api", "")}.");
                throw;
            }
        }


        /// <summary>
        /// Create a new resource.
        /// </summary>
        /// <param name="json">Request json body.</param>
        /// <returns></returns>
        public Task<string> Create(string json) {
            try {
                return Http.Post(Route, json);
            } catch {
                Console.WriteLine($"Couldn't Create {GetType().Name.Replace("Api", "")}.");
                throw;
            }
        }


        /// <summary>
        /// Update a resource
        /// </summary>
        /// <param name="id">Id of the resource.</param>
        /// <param name="json">Request json body.</param>
        /// <returns></returns>
        public Task<string> Update(string id, string json) {
            try {
                return Http.Put($"{Route}/{id}", json);
            } catch {
                Console.WriteLine($"Couldn't Update {GetType().Name.Replace("Api", "")}.");
                throw;
            }
        }


        /// <summary>
        /// Delete a resource
        /// </summary>
        /// <param name="id">Id of the resource.</param>
        /// <param name="json">Request json body.</param>
        /// <returns></returns>
        public Task<string> Delete(string id, string json = null) {
            try {
                return Http.Delete($"{Route}/{id}", json);
            } catch {
                Console.WriteLine($"Couldn't Delete {GetType().Name.Replace("Api", "")}.");
                throw;
            }
        }
    }
}

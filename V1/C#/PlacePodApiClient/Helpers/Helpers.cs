using Newtonsoft.Json;
using System;
using System.Collections.Generic;


namespace PlacePodApiClient.Helpers {

    /// <summary>
    /// Contains methods for constructing objects.
    /// </summary>
    internal static class Factories {

        /// <summary>
        /// Given a json string, this attempts to make a list of the specified object type.
        /// If it fails, a message will be printed and an empty list of the type will be returned.
        /// </summary>
        /// <typeparam name="T">Model object with JsonParameter members to deserialize into.</typeparam>
        /// <param name="json">Json string that is an array of objects.</param>
        /// <returns>A list containing the deserialize objects or an empty list if it fails.</returns>
        public static List<T> CreateCollection<T>(string json) {
            try {
                List<T> objects = JsonConvert.DeserializeObject<List<T>>(json);
                return objects;
            } catch (Exception ex) {
                Console.WriteLine("Couldn't create ping response logs: " + ex.InnerException);
                return new List<T>();
            }
        }
    }
}

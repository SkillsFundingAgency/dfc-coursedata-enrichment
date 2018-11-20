using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dfc.Coursedata.Enrichment.Data.Interfaces;
using Gremlin.Net.Driver;
using Gremlin.Net.Driver.Exceptions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Dfc.Coursedata.Enrichment.Data.Gremlin
{
    public class GremlinBase : IGremlinBase
    {
        private readonly string _hostname ;
        private readonly int _port ;
        private readonly string _authKey ;
        private readonly string _database ;
        private readonly string _collection ;
        public GremlinServer GremlinServer;

        public GremlinBase(IOptions<GremlinCosmosDbSettings> cosmosDbSettings)
        {
            //var value = cosmosDbSettings.Value;
            //var hostname = cosmosDbSettings.Value.Hostname;
            _hostname = cosmosDbSettings.Value.Hostname;
            _port = cosmosDbSettings.Value.Port;
            _authKey = cosmosDbSettings.Value.AuthKey;
            _database = cosmosDbSettings.Value.Database;
            _collection = cosmosDbSettings.Value.Collection;
            GremlinServer = new GremlinServer(_hostname, _port, enableSsl: true, username: "/dbs/" + _database + "/colls/" + _collection, password: _authKey);
        }

        public void PrintStatusAttributes(IReadOnlyDictionary<string, object> attributes)
        {
            Console.WriteLine($"\tStatusAttributes:");
            Console.WriteLine($"\t[\"x-ms-status-code\"] : { GetValueAsString(attributes, "x-ms-status-code")}");
            Console.WriteLine($"\t[\"x-ms-total-request-charge\"] : { GetValueAsString(attributes, "x-ms-total-request-charge")}");
        }

        public string GetValueAsString(IReadOnlyDictionary<string, object> dictionary, string key)
        {
            return JsonConvert.SerializeObject(GetValueOrDefault(dictionary, key));
        }

        public object GetValueOrDefault(IReadOnlyDictionary<string, object> dictionary, string key)
        {
            if (dictionary.ContainsKey(key))
            {
                return dictionary[key];
            }

            return null;
        }

        public Task<ResultSet<dynamic>> SubmitRequest(GremlinClient gremlinClient, KeyValuePair<string, string> query)
        {
            try
            {
                return gremlinClient.SubmitAsync<dynamic>(query.Value);
            }
            catch (ResponseException e)
            {
                Console.WriteLine("\tRequest Error!");

                // Print the Gremlin status code.
                Console.WriteLine($"\tStatusCode: {e.StatusCode}");

                // On error, ResponseException.StatusAttributes will include the common StatusAttributes for successful requests, as well as
                // additional attributes for retry handling and diagnostics.
                // These include:
                //  x-ms-retry-after-ms         : The number of milliseconds to wait to retry the operation after an initial operation was throttled. This will be populated when
                //                              : attribute 'x-ms-status-code' returns 429.
                //  x-ms-activity-id            : Represents a unique identifier for the operation. Commonly used for troubleshooting purposes.
                PrintStatusAttributes(e.StatusAttributes);
                Console.WriteLine($"\t[\"x-ms-retry-after-ms\"] : { GetValueAsString(e.StatusAttributes, "x-ms-retry-after-ms")}");
                Console.WriteLine($"\t[\"x-ms-activity-id\"] : { GetValueAsString(e.StatusAttributes, "x-ms-activity-id")}");

                throw;
            }
        }
    }
}

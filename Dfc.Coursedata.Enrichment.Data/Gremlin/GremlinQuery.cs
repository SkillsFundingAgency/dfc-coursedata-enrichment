using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using Dfc.Coursedata.Enrichment.Data.Interfaces;
using Dfc.ProviderPortal.Providers;
using Gremlin.Net.Driver;
using Gremlin.Net.Structure.IO.GraphSON;
using Microsoft.Azure.Documents;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Dfc.Coursedata.Enrichment.Data.Gremlin
{
    public class GremlinQuery : GremlinBase, IGremlinQuery
    {
        public bool InsertProviders(IEnumerable<Provider> providers)
        {
            // this will build the provider vertices
            var gremlinQueries = GetGremlinQueries(providers);
            ExecuteGremlinQueries(gremlinQueries);    
            return true;
        }

        private void ExecuteGremlinQueries(Dictionary<string, string> gremlinQueries)
        {
            using (var gremlinClient = new GremlinClient(GremlinServer, new GraphSON2Reader(), new GraphSON2Writer(),
                GremlinClient.GraphSON2MimeType))
            {
                foreach (var query in gremlinQueries)
                {
                    Console.WriteLine($"Running this query: {query.Key}: {query.Value}");

                    // Create async task to execute the Gremlin query.
                    var resultSet = SubmitRequest(gremlinClient, query).Result;
                    if (resultSet.Count > 0)
                    {
                        Console.WriteLine("\tResult:");
                        foreach (var result in resultSet)
                        {
                            // The vertex results are formed as Dictionaries with a nested dictionary for their properties
                            string output = JsonConvert.SerializeObject(result);
                            Console.WriteLine($"\t{output}");
                        }

                        Console.WriteLine();
                    }

                    // Print the status attributes for the result set.
                    // This includes the following:
                    //  x-ms-status-code            : This is the sub-status code which is specific to Cosmos DB.
                    //  x-ms-total-request-charge   : The total request units charged for processing a request.
                    PrintStatusAttributes(resultSet.StatusAttributes);
                    Console.WriteLine();
                }
            }
        }

        // TODO: make this generic
        private Entities.Provider Execute(Dictionary<string, string> gremlinQueries, string ukprn)
        {
            Entities.Provider provider = new Entities.Provider();
            provider.Id = ukprn;
            using (var gremlinClient = new GremlinClient(GremlinServer, new GraphSON2Reader(), new GraphSON2Writer(),
                GremlinClient.GraphSON2MimeType))
            {
                foreach (var query in gremlinQueries)
                {
                    Console.WriteLine($"Running this query: {query.Key}: {query.Value}");

                    // Create async task to execute the Gremlin query.
                    var resultSet = SubmitRequest(gremlinClient, query).Result;

                    //string newOutput = JsonConvert.SerializeObject(resultSet);
                    //MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(newOutput));
                    //DataContractJsonSerializer ser = new DataContractJsonSerializer(genericReturnObject.GetType());
                    //genericReturnObject = ser.ReadObject(ms) as IEnumerable<T>;
                    //ms.Close();

                    if (resultSet.Count > 0)
                    {
                        Console.WriteLine("\tResult:");
                        foreach (var result in resultSet)
                        {
                            // The vertex results are formed as Dictionaries with a nested dictionary for their properties
                            string output = JsonConvert.SerializeObject(result);
                            //var s = JsonConvert.DeserializeObject<IEnumerable<T>>(output);

                            //foreach (var property in result[properties)
                            //{
                               
                            //}

                            var properties = result["properties"];

                            var larsTitle = properties["LARTitle"];

                            //var value = larsTitle.Result;
                            var value = (larsTitle.Value as IEnumerable<object>).ToList();

                            Console.WriteLine($"\t{output}");
                        }

                        Console.WriteLine();
                    }

                    //var properties = result["properties"];
                    //var larsTitle = properties["LARTitle"];
                    //(property.Value as IEnumerable<object>).Cast<object>().ToList()
                    //https://stackoverflow.com/questions/48244787/gremlin-net-casting-from-enumerable-selectilistiterator-object-to-real-type

                    // Print the status attributes for the result set.
                    // This includes the following:
                    //  x-ms-status-code            : This is the sub-status code which is specific to Cosmos DB.
                    //  x-ms-total-request-charge   : The total request units charged for processing a request.
                    PrintStatusAttributes(resultSet.StatusAttributes);
                    Console.WriteLine();
                }
            }

            return provider;

        }

        public Dictionary<string, string> GetGremlinQueries(IEnumerable<Provider> providers)
        {
            Dictionary<string, string> gremlinQueries = new Dictionary<string, string>();
            gremlinQueries.Add("Cleanup", "g.V().drop()");

            //int count = 0;
            foreach (var provider in providers)
            {
                // there are dupes atm, so check dictionary and don't add in if already there
                if (!gremlinQueries.ContainsKey($"Add Vertex Ukprn:{provider.UnitedKingdomProviderReferenceNumber}"))
                {
                    //if (count == 30) break;
                    gremlinQueries.Add($"Add Vertex Ukprn:{provider.UnitedKingdomProviderReferenceNumber}",
                        $@"g.addV('provider').property('id','{
                                provider.UnitedKingdomProviderReferenceNumber
                            }').property('ProviderName', '{provider.ProviderName.Replace("'", string.Empty)//.Replace(")", string.Empty)
                            }')");
                    //count++;
                }
            }

            return gremlinQueries;
        }

        public Entities.Provider GetQualificationsByUkprn(string ukprn)
        {
            var gremlinQueries = GetGremlinQueries(ukprn);
            ExecuteGremlinQueries(gremlinQueries);
            //var providerList = Execute(gremlinQueries, ukprn);

            return new Entities.Provider() ;
        }

        public void AddProviderQualificationEdge(string ukprn, List<string> larsId)
        {
            var gremlinQueries = GetGremlinQueries(ukprn, larsId);
            ExecuteGremlinQueries(gremlinQueries);
        }

        public void DeLinkQualificationsFromProvider(string ukprn, List<string> larsId)
        {
            var gremlinQueries = GetGremlinQueries(ukprn, larsId, true);
            ExecuteGremlinQueries(gremlinQueries);
        }

        public Dictionary<string, string> GetGremlinQueries(string ukprn)
        {
            Dictionary<string, string> gremlinQueries = new Dictionary<string, string>();

            gremlinQueries.Add($"Retrieve {ukprn}", $"g.V('{ukprn}').out('runs').hasLabel('qualification')");

            return gremlinQueries;
        }

        public Dictionary<string, string> GetGremlinQueries(string ukprn, List<string> larsIds, bool delete = false)
        {
            Dictionary<string, string> gremlinQueries = new Dictionary<string, string>();

            if (delete)
            {
                foreach (var larsId in larsIds)
                {
                    gremlinQueries.Add($"Deleting Edge for Ukprn: {ukprn}, larsId: {larsId}",
                        $@"g.V('{ukprn}').outE('runs').where(inV().has('id','{larsId}')).drop()");
                }
            }
            else
            {

                foreach (var larsId in larsIds)
                {
                    gremlinQueries.Add($"Adding Edge for Ukprn: {ukprn}, larsId: {larsId}",
                        $@"g.V('{ukprn}').addE('runs').to(g.V('{larsId}'))");
                }
            }

            return gremlinQueries;

        }


        private void ExecuteGremlinQueriesProvider(Dictionary<string, string> gremlinQueries)
        {
            using (var gremlinClient = new GremlinClient(GremlinServer, new GraphSON2Reader(), new GraphSON2Writer(),
                GremlinClient.GraphSON2MimeType))
            {
                foreach (var query in gremlinQueries)
                {
                    Console.WriteLine($"Running this query: {query.Key}: {query.Value}");

                    // Create async task to execute the Gremlin query.
                    var resultSet = SubmitRequest(gremlinClient, query).Result;
                    if (resultSet.Count > 0)
                    {
                        Console.WriteLine("\tResult:");
                        foreach (var result in resultSet)
                        {
                            // The vertex results are formed as Dictionaries with a nested dictionary for their properties
                            string output = JsonConvert.SerializeObject(result);
                            Console.WriteLine($"\t{output}");
                        }

                        Console.WriteLine();
                    }

                    // Print the status attributes for the result set.
                    // This includes the following:
                    //  x-ms-status-code            : This is the sub-status code which is specific to Cosmos DB.
                    //  x-ms-total-request-charge   : The total request units charged for processing a request.
                    PrintStatusAttributes(resultSet.StatusAttributes);
                    Console.WriteLine();
                }
            }
        }

        public GremlinQuery(IOptions<GremlinCosmosDbSettings> cosmosDbSettings) : base(cosmosDbSettings)
        {
        }
    }
}

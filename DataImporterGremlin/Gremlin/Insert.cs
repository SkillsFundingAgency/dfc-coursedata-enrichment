using System;
using System.Collections.Generic;
using Dfc.ProviderPortal.Providers;
using Gremlin.Net.Driver;
using Gremlin.Net.Structure.IO.GraphSON;
using Newtonsoft.Json;

namespace Dfc.CourseData.Importer.Gremlin
{
    public class Insert : GremlinBase
    {
        public bool InsertProviders(IEnumerable<Provider> providers)
        {

            using (var gremlinClient = new GremlinClient(GremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType))
            {
                var gremlinQueries = GetGremlinQueries(providers);

                foreach (var query in gremlinQueries)
                {
                    Console.WriteLine(String.Format("Running this query: {0}: {1}", query.Key, query.Value));

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
            return true;
        }

        public Dictionary<string, string> GetGremlinQueries(IEnumerable<Provider> providers)
        {
            Dictionary<string, string> gremlinQueries = new Dictionary<string, string>();
            gremlinQueries.Add("Cleanup", "g.V().drop()");

            int count = 0;
            foreach (var provider in providers)
            {
                if (count == 30) break;
                gremlinQueries.Add("Add Vertex Ukprn:" + provider.UnitedKingdomProviderReferenceNumber,
                    "g.addV('provider').property('id','" + provider.UnitedKingdomProviderReferenceNumber +
                    "').property('ProviderName', '" + provider.ProviderName + "')");
                count++;
            }

            return gremlinQueries;
        }
    }
}

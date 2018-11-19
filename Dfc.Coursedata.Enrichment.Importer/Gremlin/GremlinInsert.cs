using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dfc.Coursedata.Enrichment.Common.Interfaces;
using Dfc.Coursedata.Enrichment.Importer.Entities;
using Dfc.Coursedata.Enrichment.Importer.Interfaces;
using Dfc.Coursedata.Enrichment.Services.Interfaces;
using Dfc.ProviderPortal.Providers;
using Gremlin.Net.Driver;
using Gremlin.Net.Driver.Messages;
using Gremlin.Net.Structure.IO.GraphSON;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Dfc.Coursedata.Enrichment.Importer.Gremlin
{
    public class GremlinInsert : GremlinBase, IGremlinInsert
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
        }

        public bool InsertLars(IResult<ILarsSearchResult> larsData)
        {
            // this will build the lars vertices
            var gremlinQueries = GetGremlinQueries(larsData);
            ExecuteGremlinQueries(gremlinQueries);
            return true;
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

        public Dictionary<string, string> GetGremlinQueries(IResult<ILarsSearchResult> larsData)
        {
            Dictionary<string, string> gremlinQueries = new Dictionary<string, string>();

            foreach (var data in larsData.Value.Value)
            {
                gremlinQueries.Add($"Add vertex Lar:{data.LearnAimRef}",
                    $@"g.addV('qualification').property('id','{data.LearnAimRef}').property('LARTitle', '{
                            data.LearnAimRefTitle.Replace("'", string.Empty)
                        }')");

                //gremlinQueries.Add($"Add vertex Lar:{data.LearnAimRef}",
                //    $"g.addV('qualification').property('id','{data.LearnAimRef}').property('LARTitle', '{data.LearnAimRefTitle}')");
            }

            return gremlinQueries;
        }

        public Dictionary<string, string> GetGremlinQueries(IEnumerable<ILR> ilrData)
        {
            Dictionary<string, string> gremlinQueries = new Dictionary<string, string>();

            foreach (var ilr in ilrData)
            {
                gremlinQueries.Add($"AddEdge ukprn:{ilr.UKPRN} larsRef:{ilr.LearnAimRef}",
                    $@"g.V('{ilr.UKPRN}').addE('runs').to(g.V('{ilr.LearnAimRef}'))");
            }

            return gremlinQueries;
        }

        public bool InsertEdges(IEnumerable<ILR> ilrData)
        {
            var gremlinQueries = GetGremlinQueries(ilrData);
            ExecuteGremlinQueries(gremlinQueries);
            return true;
        }

        public GremlinInsert(IOptions<GremlinCosmosDbSettings> cosmosDbSettings) : base(cosmosDbSettings)
        {
        }
    }
}

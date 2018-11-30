using System;
using System.Collections.Generic;
using System.Linq;
using Dfc.Coursedata.Enrichment.Common.Interfaces;
using Dfc.Coursedata.Enrichment.Importer.Entities;
using Dfc.Coursedata.Enrichment.Importer.Interfaces;
using Dfc.Coursedata.Enrichment.Services.Interfaces;
using Dfc.ProviderPortal.Providers;
using Gremlin.Net.Driver;
using Gremlin.Net.Structure.IO.GraphSON;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Microsoft.Azure.CosmosDB.BulkExecutor.Graph;

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
                    try
                    {
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
                    catch(Exception ex)
                    {
                        // ignored as some duplicates in spreadsheet
                    }
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

        public bool InsertIlrData(List<ILR> ilrData)
        {
            // get distinct ukprn's, insert as vertices
            // get distinct larsRefs, insert as vertices
            //var ukprnList = ilrData.Select(x => x.UKPRN).Distinct().ToList();
            //var larsRefList = ilrData.Select(x => x.LearnAimRef).Distinct().ToList();


            // drop 
            //var cleanup = new Dictionary<string, string> { { "Cleanup", "g.V().drop()" } };
            //ExecuteGremlinQueries(cleanup);
            //var ukprnQueries = GetGremlinQueries(ukprnList, true);
            //ExecuteGremlinQueries(ukprnQueries);
            //var larsQueries = GetGremlinQueries(larsRefList, false);
            //ExecuteGremlinQueries(larsQueries);


            // quick fix 
            var ukprnsList = new List<string>
            {
                //"10000020",
                "10000028"
                //"10000054",
                //"10000055",
                //"10000060",
                //"10000080",
                //"10000082",
                //"10000093",
                //"10000099"
            };

            //var ukprnQueries = GetGremlinQueries(ukprnsList, true);
            //ExecuteGremlinQueries(ukprnQueries);



            foreach (var prn in ukprnsList)
            {
                var larsList = ilrData.Where(x => x.UKPRN == prn).Select(ilr1 => ilr1.LearnAimRef).Distinct().ToList();
                // create vertex queries here
                var vertexQueries = GetGremlinQueries(larsList, false);
                ExecuteGremlinQueries(vertexQueries);

                // create edges
                var gremlinQueries = GetGremlinQueries(prn, larsList);
                ExecuteGremlinQueries(gremlinQueries);
            }


            //var checkedUkprnList = new List<string>();
            //foreach (var ilr in ilrData)
            //{
            //    var ukprn = ilr.UKPRN;
            //    List<string> larsList = ilrData.Where(x=>x.UKPRN==ukprn).Select(ilr1 => ilr1.LearnAimRef).Distinct().ToList();

            //    if (!checkedUkprnList.Exists(x => x == ukprn))
            //    {
            //        // create queries here
            //        var gremlinQueries = GetGremlinQueries(ukprn, larsList);
            //        ExecuteGremlinQueries(gremlinQueries);
            //    }

            //    checkedUkprnList.Add(ukprn);
            //}

            return true;
        }

        public Dictionary<string, string> GetGremlinQueries(string ukprn, List<string> larsRefs)
        {
            Dictionary<string, string> gremlinQueries = new Dictionary<string, string>();

            // add vertex ukprn
            //gremlinQueries.Add($"Add Vertex Ukprn:{ukprn}", $@"g.addV('provider').property('id','{ukprn}')");

            foreach (var larsRef in larsRefs)
            {
                // add vertices larsRefs
                //gremlinQueries.Add($"Add Vertex LarsRef:{larsRef}", $@"g.addV('qualification').property('id','{larsRef}')");
                // add edges between them
                gremlinQueries.Add($"AddEdge ukprn:{ukprn} larsRef:{larsRef}",
                    $@"g.V('{ukprn}').addE('runs').to(g.V('{larsRef}'))");
            }

            return gremlinQueries;
        }

        public Dictionary<string, string> GetGremlinQueries(List<string> vertices, bool ukprn)
        {
            Dictionary<string, string> gremlinQueries = new Dictionary<string, string>();
            foreach (var vertex in vertices)
            {
                if (ukprn)
                {
                    gremlinQueries.Add($"Add Vertex Ukprn:{vertex}",
                        $@"g.addV('provider').property('id','{vertex}')");
                }
                else
                {
                    gremlinQueries.Add($"Add Vertex LarsRef:{vertex}",
                        $@"g.addV('qualification').property('id','{vertex}')");
                }
            }

            return gremlinQueries;
        }

        public GremlinInsert(IOptions<GremlinCosmosDbSettings> cosmosDbSettings) : base(cosmosDbSettings)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Dfc.Coursedata.Enrichment.Common.Interfaces;
using Dfc.Coursedata.Enrichment.Importer.Entities;
using Dfc.Coursedata.Enrichment.Importer.Entities.CSV;
using Dfc.Coursedata.Enrichment.Importer.Interfaces;
using Dfc.Coursedata.Enrichment.Services.Interfaces;
using Gremlin.Net.Driver;
using Gremlin.Net.Structure.IO.GraphSON;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Microsoft.Azure.CosmosDB.BulkExecutor.Graph;
using Provider = Dfc.ProviderPortal.Providers.Provider;

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

        // ----------------------------------------------------------------

        


        public void InsertCsvProvider(Entities.CSV.Provider provider)
        {
            var gremlinQueries = GetProviderGremlinQueries(provider);
            ExecuteGremlinQueries(gremlinQueries);
        }

        private Dictionary<string, string> GetProviderGremlinQueries(Entities.CSV.Provider provider)
        {
            var gremlinQueries = new Dictionary<string, string>();

            var properties = new Dictionary<string, string>
            {
                {"id", provider.PROVIDER_ID},
                {"ukprn", provider.UKPRN},
                {"providerName", provider.PROVIDER_NAME}
            };
            var nodeCreationQuery = GetNodeCreationQuery("provider", properties);
            gremlinQueries.Add($"Add Provider: {provider.UKPRN}", nodeCreationQuery);
            return gremlinQueries;
        }

        public List<string> InsertCsvQualification(string ukprn, List<Qualification> qualifications)
        {
            // insert the qualifications
            var larsList = qualifications.Where(x => x.UKPRN == ukprn).Select(ilr => ilr.LearnAimRef).Distinct().ToList();
            var gremlinQueries = GetQualificationGremlinQueries(ukprn, larsList);
            ExecuteGremlinQueries(gremlinQueries);
            return larsList;
        }

        private Dictionary<string, string> GetQualificationGremlinQueries(string ukprn, List<string> larsList)
        {
            var gremlinQueries = new Dictionary<string, string>();
            foreach (var lars in larsList)
            {
                // add the qualification
                gremlinQueries.Add($"Add Qualification {lars}", $@"g.addV('qualification').property('id', '{lars}')");

                // add the relationship "runs"
                gremlinQueries.Add($"Add Edge ukprn: {ukprn} larsRef: {lars}", $@"g.V().hasLabel('provider').has('ukprn','{ukprn}').addE('runs').to(g.V().hasLabel('qualification').has('id','{lars}') )");

                //g.V().hasLabel('person').has('firstName', 'Thomas').addE('knows').to(g.V().hasLabel('person').has('firstName', 'Mary Kay'))
            }

            return gremlinQueries;
        }

        public void InsertCsvCourseDetails(string ukprn, List<CourseDetail> courseDetails)
        {
            // insert a course node
            // insert a coursedetail node
            // insert a relationship between course and ukprn
            // insert a relationship between course and qualification larsref (ladid)

            var gremlinQueries = GetCourseDetailGremlinQueries(ukprn, courseDetails);
            ExecuteGremlinQueries(gremlinQueries);
        }

        private Dictionary<string, string> GetCourseDetailGremlinQueries(string ukprn, List<CourseDetail> courseDetails)
        {
            var gremlinQueries = new Dictionary<string, string>();

            var larsList = courseDetails.Select(q => q.LAD_ID).Distinct().ToList();
            foreach (var lars in larsList)
            {
                // add qualification (larsid)
                gremlinQueries.Add($"Add Qualification {lars}", $@"g.addV('qualification').property('id', '{lars}')");

                // add edge between ukprn and lad_id (larsid)
                gremlinQueries.Add($"Add Edge ukprn: {ukprn} larsRef: {lars}", $@"g.V().hasLabel('provider').has('ukprn','{ukprn}').addE('runs').to(g.V().hasLabel('qualification').has('id','{lars}') )");

            }

            foreach (var courseDetail in courseDetails)
            {

                // add a course
                gremlinQueries.Add($"Add Course {courseDetail.COURSE_ID}", $@"g.addV('course').property('id', '{courseDetail.COURSE_ID}')");

                // add a course detail node
                var courseDetailGuid = Guid.NewGuid();
                gremlinQueries.Add($"Add CourseDetail {courseDetailGuid}", $@"g.addV('courseDetail').property('id', '{courseDetailGuid}').property('courseTitle', '{courseDetail.PROVIDER_COURSE_TITLE}').property('courseSummary', '{courseDetail.COURSE_SUMMARY}')");
                //gremlinQueries.Add($"Add CourseDetail {courseDetail.COURSE_ID}", $@"g.addV('courseDetail').property('id', '{courseDetail.COURSE_ID}').property('courseTitle', '{courseDetail.PROVIDER_COURSE_TITLE}').property('courseSummary', '{courseDetail.COURSE_SUMMARY}')");

                // edge between course and coursedetail
                //gremlinQueries.Add($"Add Edge course {courseDetail.COURSE_ID} and coursedetail {courseDetail.COURSE_ID}", $@"g.V().hasLabel('course').has('id','{courseDetail.COURSE_ID}').addE('has').to(g.V().hasLabel('courseDetail').has('id', '{courseDetail.COURSE_ID}'))");
                gremlinQueries.Add($"Add Edge course {courseDetail.COURSE_ID} and coursedetail {courseDetailGuid}", $@"g.V().hasLabel('course').has('id','{courseDetail.COURSE_ID}').addE('has').to(g.V().hasLabel('courseDetail').has('id', '{courseDetailGuid}'))");

                // edge between course and ukprn
                gremlinQueries.Add($"Add Edge ukprn {ukprn} and course {courseDetail.COURSE_ID}", $@"g.V().hasLabel('provider').has('ukprn','{ukprn}').addE('runs').to(g.V().hasLabel('course').has('id', '{courseDetail.COURSE_ID}'))");

                // edge between course and larsref (ladid)
                gremlinQueries.Add($"Add Edge course {courseDetail.COURSE_ID} and qualification {courseDetail.LAD_ID}", $@"g.V().hasLabel('course').has('id','{courseDetail.COURSE_ID}').addE('hasa').to(g.V().hasLabel('qualification').has('id', '{courseDetail.LAD_ID}'))");


                // https://docs.microsoft.com/en-us/azure/cosmos-db/create-graph-gremlin-console
                // g.V().hasLabel('person').has('firstName', 'Thomas').addE('knows').to(g.V().hasLabel('person').has('firstName', 'Mary Kay'))
            }

            return gremlinQueries;
        }

        public void InsertCsvCourseDetailsOnly(string ukprn, List<CourseDetail> courseDetails)
        {
            var gremlinQueries = GetCourseDetailOnlyGremlinQueries(ukprn, courseDetails);
            ExecuteGremlinQueries(gremlinQueries);
        }

        private Dictionary<string, string> GetCourseDetailOnlyGremlinQueries(string ukprn,
            List<CourseDetail> courseDetails)
        {
            var gremlinQueries = new Dictionary<string, string>();
            foreach (var courseDetail in courseDetails)
            {
                // add a course detail node
                var courseDetailGuid = Guid.NewGuid().ToString();
                gremlinQueries.Add($"Add CourseDetail {courseDetailGuid}", $@"g.addV('courseDetail').property('id', '{courseDetailGuid}').property('courseTitle', '{courseDetail.PROVIDER_COURSE_TITLE}').property('courseSummary', '{courseDetail.COURSE_SUMMARY}')");
                gremlinQueries.Add($"Add Edge course {courseDetail.COURSE_ID} and coursedetail {courseDetailGuid}", $@"g.V().hasLabel('course').has('id','{courseDetail.COURSE_ID}').addE('has').to(g.V().hasLabel('courseDetail').has('id', '{courseDetailGuid}'))");
            }

            return gremlinQueries;
        }


        public void InsertCsvVenues(List<Venue> venues)
        {
            var gremlinQueries = GetVenueGremlinQueries(venues);
            ExecuteGremlinQueries(gremlinQueries);
        }

        private Dictionary<string, string> GetVenueGremlinQueries(List<Venue> venues)
        {
            var gremlinQueries = new Dictionary<string, string>();
            foreach (var venue in venues)
            {
                gremlinQueries.Add($"Add Venue {venue.VENUE_ID}", $@"g.addV('venue').property('id', '{venue.VENUE_ID}').property('venueName','{venue.VENUE_NAME}')");

                // TODO: could link provider -> uses -> venue
            }

            return gremlinQueries;
        }

        public void InsertCsvOpportunities(List<Opportunity> opportunities)
        {
            var gremlinQueries = GetOpportunityGremlinQueries(opportunities);
            ExecuteGremlinQueries(gremlinQueries);
        }

        private Dictionary<string, string> GetOpportunityGremlinQueries(List<Opportunity> opportunities)
        {
            var gremlinQueries = new Dictionary<string, string>();
            foreach (var opportunity in opportunities)
            {
                // edge between courseid and venueid
                gremlinQueries.Add($"Add Edge course {opportunity.COURSE_ID} and venue {opportunity.VENUE_ID}", $@"g.V().hasLabel('course').has('id','{opportunity.COURSE_ID}').addE('runat').to(g.V().hasLabel('venue').has('id', '{opportunity.VENUE_ID}'))");

                // TODO: could do venue -> runs -> course
            }

            return gremlinQueries;
        }

        private string GetEdgeQuery(string label1, KeyValuePair<string, string> node1 , string edge, string label2, KeyValuePair<string, string> node2 )
        {
            string edgeQuery =
                $@"g.V().hasLabel('{label1}').has('{node1.Key}','{node1.Value}').addE('{edge}').to(g.V().hasLabel('{label2
                    }').has('{node2.Value}', '{node2.Value}'))";

            return edgeQuery;
        }

        private string GetNodeCreationQuery(string nodeName, Dictionary<string, string> properties)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append($@"g.addV('{nodeName}')");

            foreach (var property in properties)
            {
                sb.Append($@".property('{property.Key}', '{property.Value}')");
            }

            return sb.ToString();
        }

        public GremlinInsert(IOptions<GremlinCosmosDbSettings> cosmosDbSettings) : base(cosmosDbSettings)
        {
        }
    }
}

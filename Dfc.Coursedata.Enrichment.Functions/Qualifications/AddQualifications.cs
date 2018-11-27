using System.Linq;
using System.Threading.Tasks;
using Dfc.Coursedata.Enrichment.Data.Interfaces;
using Dfc.Coursedata.Enrichment.Functions.Common.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Dfc.Coursedata.Enrichment.Functions.Qualifications
{
    public static class AddQualifications
    {
        [FunctionName("AddQualifications")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log, [Inject] IGremlinQuery gremlinQuery)
        {
            var ukprn = req.Query["ukprn"];
            var larsIds = req.Query["larsIds"];

            gremlinQuery.AddProviderQualificationEdge(ukprn, larsIds.ToList());

            return new OkObjectResult($"Hello");
        }
    }
}

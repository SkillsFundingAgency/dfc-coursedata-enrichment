using System.Threading.Tasks;
using Dfc.Coursedata.Enrichment.Functions.Common.DependencyInjection;
using Dfc.Coursedata.Enrichment.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Dfc.Coursedata.Enrichment.Functions.Qualifications
{
    public static class GetQualificationsByUkprn
    {
        [FunctionName("GetQualificationsByUkprn")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log, [Inject] IGremlinQuery gremlinInserter )
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var qualificationsByUkprn = gremlinInserter.GetQualificationsByUkprn("123");

            return new OkObjectResult($"Hello");
        }
    }
}

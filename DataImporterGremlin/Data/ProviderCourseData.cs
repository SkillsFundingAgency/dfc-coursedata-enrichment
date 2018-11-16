using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Dfc.ProviderPortal.Providers;
using Microsoft.Extensions.Logging;
using UKRLP.Storage;

namespace Dfc.CourseData.Importer.Data
{
    public class ProviderCourseData
    {
        static HttpClient _client = new HttpClient();

        public async Task<IEnumerable<Provider>> GetProviderData()
        {

            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger logger = loggerFactory.CreateLogger<Program>();

            ProviderStorage ps = new ProviderStorage();
            // todo: change to new tracewriter
            IEnumerable<Provider> task = await ps.GetAll(null);
            return task;


            // below needs sorting as getting a 500 on this atm so using above (add as project references)
            //var url = "";
            ////var url = "";
            //HttpResponseMessage response = await _client.GetAsync(url);
            //var result = await response.Content.ReadAsAsync<Provider>();
            //return new List<Provider>();
        }
    }
}

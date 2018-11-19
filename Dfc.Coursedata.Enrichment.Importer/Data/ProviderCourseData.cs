using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.ServiceModel.Description;
using System.Threading.Tasks;
using CsvHelper;
using Dfc.Coursedata.Enrichment.Importer.Entities;
using Dfc.Coursedata.Enrichment.Importer.Interfaces;
using Dfc.ProviderPortal.Providers;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UKRLP.Storage;

namespace Dfc.Coursedata.Enrichment.Importer.Data
{
    public class ProviderCourseData : IProviderCourseData
    {
        static HttpClient _client = new HttpClient();
        private readonly string UrlGetAllProviders;
        private readonly string UrlCode;

        public ProviderCourseData(IOptions<UkrlpSettings> ukrlpSettings)
        {
            UrlGetAllProviders = ukrlpSettings.Value.UrlGetAllProviders;
            UrlCode = ukrlpSettings.Value.Code;
        }

        public async Task<IEnumerable<Provider>> GetProviderData()
        {
            //ILoggerFactory loggerFactory = new LoggerFactory();
            //ILogger logger = loggerFactory.CreateLogger<Program>();
            var logger = new TraceWriterStub(TraceLevel.Verbose);
            ProviderStorage ps = new ProviderStorage();
            IEnumerable<Provider> task = await ps.GetAll(logger);
            return task;


            // below needs sorting as getting a 500 on this atm so using above (add as project references)
            //var url = $"{UrlGetAllProviders}?code={UrlCode}";
            //var response = await _client.GetAsync(url);
            //var result = await response.Content.ReadAsAsync<IEnumerable<Provider>>();
            //return result;
        }

        public IEnumerable<ILR> GetILRData()
        {
            TextReader reader = new StreamReader(@"C:\CareersServiceDocuments\Data Extract for Find A Course SILR SN14 160 (2).csv");
            var csvReader = new CsvReader(reader);
            var records = csvReader.GetRecords<ILR>();
            return records;
        }

        public class TraceWriterStub : TraceWriter
        {
            protected TraceLevel _level;
            protected List<TraceEvent> _traces;

            public TraceWriterStub(TraceLevel level) : base(level)
            {
                _level = level;
                _traces = new List<TraceEvent>();
            }

            public override void Trace(TraceEvent traceEvent)
            {
                _traces.Add(traceEvent);
            }

            public List<TraceEvent> Traces => _traces;
        }
    }
}

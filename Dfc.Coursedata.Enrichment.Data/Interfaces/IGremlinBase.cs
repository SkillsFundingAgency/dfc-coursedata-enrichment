using System.Collections.Generic;
using System.Threading.Tasks;
using Gremlin.Net.Driver;

namespace Dfc.Coursedata.Enrichment.Data.Interfaces
{
    public interface IGremlinBase
    {
        Task<ResultSet<dynamic>> SubmitRequest(GremlinClient gremlinClient, KeyValuePair<string, string> query);
        void PrintStatusAttributes(IReadOnlyDictionary<string, object> attributes);
        string GetValueAsString(IReadOnlyDictionary<string, object> dictionary, string key);
        object GetValueOrDefault(IReadOnlyDictionary<string, object> dictionary, string key);
    }
}

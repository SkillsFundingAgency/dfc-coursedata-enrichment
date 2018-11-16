using Newtonsoft.Json.Serialization;

namespace Dfc.Coursedata.Enrichment.Services
{
    public class LarsSearchCriteriaContractResolver : PrivateSetterContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            return propertyName.ToLower();
        }
    }
}

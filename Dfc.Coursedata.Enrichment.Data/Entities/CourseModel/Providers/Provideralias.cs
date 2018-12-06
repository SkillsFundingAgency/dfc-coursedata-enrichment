using Dfc.Coursedata.Enrichment.Data.Interfaces.Providers;

namespace Dfc.Coursedata.Enrichment.Data.Entities.CourseModel.Providers
{
    public class Provideralias : IProvideralias
    {
        public object ProviderAlias { get; set; }
        public object LastUpdated { get; set; }

        public Provideralias()
        {
        }
    }
}
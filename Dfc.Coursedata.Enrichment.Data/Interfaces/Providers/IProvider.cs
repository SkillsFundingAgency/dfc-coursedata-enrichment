using System;

namespace Dfc.Coursedata.Enrichment.Data.Interfaces.Providers
{
    public interface IProvider
    {
        string UnitedKingdomProviderReferenceNumber { get; set; }
        string ProviderName { get; set; }
        string ProviderStatus { get; set; }
        IProvidercontact[] ProviderContact { get; set; }
        DateTime ProviderVerificationDate { get; set; }
        bool ProviderVerificationDateSpecified { get; set; }
        bool ExpiryDateSpecified { get; set; }
        object ProviderAssociations { get; set; }
        IProvideralias[] ProviderAliases { get; set; }
        IVerificationdetail[] VerificationDetails { get; set; }
    }
}

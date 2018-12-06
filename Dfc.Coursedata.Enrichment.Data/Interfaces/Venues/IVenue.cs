namespace Dfc.Coursedata.Enrichment.Data.Interfaces.Venues
{
    public interface IVenue
    {
        string ID { get; }
        int UKPRN { get; }
        int ProviderID { get; }
        int VenueID { get; }
        string VenueName { get; }
        string ProvVenueID { get; }
        string Address1 { get; }
        string Address2 { get; }
        string Town { get; }
        string County { get; }
        string PostCode { get; }
    }
}

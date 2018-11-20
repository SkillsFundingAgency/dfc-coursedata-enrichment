namespace Dfc.Coursedata.Enrichment.Data.Interfaces
{
    public interface IGremlinCosmosDbSettings
    {
        string Hostname { get; }
        int Port { get; }
        string AuthKey { get; }
        string Database { get; }
        string Collection { get; }
    }
}

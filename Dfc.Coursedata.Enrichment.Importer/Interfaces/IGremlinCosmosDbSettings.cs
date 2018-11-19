namespace Dfc.Coursedata.Enrichment.Importer
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

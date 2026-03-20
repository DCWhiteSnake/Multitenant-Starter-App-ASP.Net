namespace MultitenantStarter.Common.Entities
{
    public class Tenant
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string ConnectionString { get; set; }
        public required string ApiKey { get; set; }
    }
}

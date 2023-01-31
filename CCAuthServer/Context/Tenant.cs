namespace CCAuthServer.Context
{
    public class Tenant
    {
        public Guid TenantId { get; set; }
        public string TenantName { get; set; }
        public string TenantDatabase { get; set; }
    }
}

using Microsoft.Extensions.Configuration;

namespace BOM.CORE
{
    public interface ISessionContext
    {
        string Name { get; set; }
        ISessionDriver SessionDriver { get; set; }
        BomConfigContext ContextConfig { get; set; }
        IConfiguration configuration { get; set; }
    }
    public class SessionContext : ISessionContext
    {
        public string Name { get; set; }
        public ISessionDriver SessionDriver { get; set; }
        public BomConfigContext ContextConfig { get; set; }
        public IConfiguration configuration { get; set; }
    }
}
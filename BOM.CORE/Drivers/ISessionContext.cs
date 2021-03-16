namespace BOM.CORE
{
    public interface ISessionContext
    {
        string Name { get; set; }
        ISessionDriver SessionDriver { get; set; }
    }
    public class SessionContext : ISessionContext
    {
        public string Name { get; set; }
        public ISessionDriver SessionDriver { get; set; }
    }
}
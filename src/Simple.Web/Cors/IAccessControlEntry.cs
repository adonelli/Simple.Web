namespace Simple.Web.Cors
{
    using System.Collections.Generic;

    public interface IAccessControlEntry
    {
        string Origin { get; }
        bool? Credentials { get; }
        ISet<string> Methods { get; }
        string AllowHeaders { get; }
        string ExposeHeaders { get; }
        long? MaxAge { get; }
    }
}
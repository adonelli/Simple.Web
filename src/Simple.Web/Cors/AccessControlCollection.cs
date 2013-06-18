namespace Simple.Web.Cors
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public interface IAccessControlCollection
    {
        void Add(IAccessControlEntry entry);
        bool TryGetEntry(string origin, out IAccessControlEntry entry);
    }

    public class AccessControlCollection : IAccessControlCollection
    {
        private readonly IDictionary<string, IAccessControlEntry> _entries = new Dictionary<string, IAccessControlEntry>(StringComparer.OrdinalIgnoreCase);

        public void Add(IAccessControlEntry entry)
        {
            _entries[entry.Origin] = entry;
        }

        public bool TryGetEntry(string origin, out IAccessControlEntry entry)
        {
            return _entries.TryGetValue(origin, out entry);
        }
    }
}
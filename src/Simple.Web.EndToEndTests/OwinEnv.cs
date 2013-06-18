namespace Simple.Web.EndToEndTests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Fix;

    public class OwinEnv : IDictionary<string, object>
    {
        private readonly IDictionary<string, object> _dictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        public OwinEnv(string method, Uri uri, HeaderDictionary requestHeaders, HeaderDictionary responseHeaders, string requestBody = null)
        {
            _dictionary[OwinKeys.RequestMethod] = method;
            _dictionary[OwinKeys.RequestPath] = uri.AbsolutePath;
            _dictionary[OwinKeys.RequestQueryString] = uri.Query.TrimStart('?');
            _dictionary[OwinKeys.RequestScheme] = uri.Scheme;
            _dictionary[OwinKeys.RequestHeaders] = requestHeaders;
            _dictionary[OwinKeys.RequestBody] = requestBody == null
                                                    ? new MemoryStream()
                                                    : new MemoryStream(Encoding.UTF8.GetBytes(requestBody));

            _dictionary[OwinKeys.ResponseHeaders] = responseHeaders;
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        public void Add(KeyValuePair<string, object> item)
        {
            _dictionary.Add(item);
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return _dictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            _dictionary.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return _dictionary.Remove(item);
        }

        public int Count
        {
            get { return _dictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return _dictionary.IsReadOnly; }
        }

        public bool ContainsKey(string key)
        {
            return _dictionary.ContainsKey(key);
        }

        public void Add(string key, object value)
        {
            _dictionary.Add(key, value);
        }

        public bool Remove(string key)
        {
            return _dictionary.Remove(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        public object this[string key]
        {
            get { return _dictionary[key]; }
            set { _dictionary[key] = value; }
        }

        public ICollection<string> Keys
        {
            get { return _dictionary.Keys; }
        }

        public ICollection<object> Values
        {
            get { return _dictionary.Values; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
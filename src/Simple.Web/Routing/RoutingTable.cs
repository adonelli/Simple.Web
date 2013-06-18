namespace Simple.Web.Routing
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Hosting;

    /// <summary>
    /// Handles routing for hosts.
    /// </summary>
    internal class RoutingTable
    {
        private readonly MatcherCollection _matchers = new MatcherCollection();
        private readonly Dictionary<string, IMatcher> _statics = new Dictionary<string, IMatcher>(StringComparer.OrdinalIgnoreCase);

        public void Add(string template, Type type)
        {
            Add(template, new HandlerTypeInfo(type));
        }

        public void Add(string template, HandlerTypeInfo type)
        {
            var parts = template.Trim('/').Split(new[] {'/'});
            if (parts.Length == 0)
            {
                return;
            }
            IMatcher matcher;
            if (_statics.ContainsKey(parts[0]))
            {
                matcher = _statics[parts[0]];
            }
            else if (_matchers.Contains(parts[0]))
            {
                matcher = _matchers[parts[0]];
            }
            else
            {
                matcher = MatcherFactory.Create(parts[0]);
                if (matcher is StaticMatcher)
                {
                    _statics.Add(parts[0], matcher);
                }
                else
                {
                    _matchers.Add(matcher);
                }
            }

            if (parts.Length == 1)
            {
                matcher.AddTypeInfo(type);
                return;
            }

            matcher.Add(parts, 1, 0).AddTypeInfo(type);
        }

        /// <summary>
        /// Gets the type of handler for the specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="variables">The variables.</param>
        /// <param name="contentType">Value of the Content-Type header from the Request.</param>
        /// <param name="acceptTypes">Values of the Accepts header from the Request.</param>
        /// <returns></returns>
        public Type Get(string url, out IDictionary<string, string> variables, string contentType = null, IList<string> acceptTypes = null)
        {
            MatchData matchData;
            var found = MatchData(url, out variables, out matchData);

            if (!found) return null;

            variables = matchData.Variables;
            if (matchData.Single != null)
            {
                return matchData.Single.HandlerType;
            }

            return matchData.ResolveByMediaTypes(contentType, acceptTypes);
        }

        public IEnumerable<HandlerTypeInfo> GetAll(string url)
        {
            MatchData matchData;
            IDictionary<string, string> variables;
            var found = MatchData(url, out variables, out matchData);
            return found ? matchData.List : Enumerable.Empty<HandlerTypeInfo>();
        }

        private bool MatchData(string url, out IDictionary<string, string> variables, out MatchData matchData)
        {
            variables = null;
            url = url.Trim('/');
            int nextIndex = url.IndexOf('/');
            string part = nextIndex >= 0 ? url.Substring(0, nextIndex) : url;
            IMatcher matcher;
            var foundMatchData = new MatchData();
            bool found = false;
            if (_statics.TryGetValue(part, out matcher))
            {
                found = matcher.Match(part, url, nextIndex, foundMatchData);
            }
            if (!found)
            {
                found = _matchers.Aggregate(false, (current, t) => t.Match(part, url, nextIndex, foundMatchData) || current);
            }
            matchData = foundMatchData;
            return found;
        }

        public HashSet<Type> GetAllTypes()
        {
            var set = new HashSet<Type>();
            AddSub(set, _statics.Values);
            AddSub(set, _matchers);
            return set;
        }

        private void AddSub(HashSet<Type> set, IEnumerable<IMatcher> matchers)
        {
            foreach (var matcher in matchers)
            {
                if (matcher.Items != null)
                {
                    foreach (var typeInfo in matcher.Items)
                    {
                        set.Add(typeInfo.HandlerType);
                    }
                }
                AddSub(set, matcher.Matchers);
                var matcherBase = matcher as MatcherBase;
                if (matcherBase != null)
                {
                    AddSub(set, matcherBase.StaticMatchers);
                }
            }
        }
    }
}

namespace Simple.Web.Owin
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using Http;

    internal class OwinContext : IContext
    {
        public OwinContext(IDictionary<string,object> env)
        {
            Variables = env;
            Request = new OwinRequest(env, (IDictionary<string,string[]>)env[OwinKeys.RequestHeaders], (Stream)env[OwinKeys.RequestBody]);
            object headers;
            if (env.TryGetValue(OwinKeys.ResponseHeaders, out headers))
            {
                Response = new OwinResponse(headers as IDictionary<string, string[]>);
            }
            else
            {
                Response = new OwinResponse();
            }
        }

        public IRequest Request { get; private set; }
        public IResponse Response { get; private set; }
        public IDictionary<string, object> Variables { get; private set; }
    }
}
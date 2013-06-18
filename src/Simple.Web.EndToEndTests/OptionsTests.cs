using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Web.EndToEndTests
{
    using System;
    using Cors;
    using Xunit;

    public class OptionsTests
    {
        private static readonly Uri TestUri = new Uri("http://localhost/spaceships", UriKind.Absolute);

        [Fact]
        public void OptionsReturnsMethods()
        {
            SimpleWeb.Configuration.AccessControl.Add(new AccessControlEntry("http://earth.com", "GET,POST,PUT,DELETE",
                                                                             credentials: true));

            var requestHeaders = new HeaderDictionary
                {
                    {"Origin", "http://earth.com"},
                    {"Accept", "application/json"}
                };
            var responseHeaders = new HeaderDictionary();
            var env = new OwinEnv("OPTIONS", TestUri,
                                  requestHeaders,
                                  responseHeaders);
            Application.Run(env);

            Assert.Contains("Access-Control-Allow-Methods", responseHeaders.Keys);
        }
    }
}

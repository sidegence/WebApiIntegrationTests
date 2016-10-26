using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Owin;
using Microsoft.Owin.Testing;
using System.Net.Http.Formatting;

namespace WebApiIntegrationTests.Tests.Controllers
{
    public abstract class BaseWebApiTest : IDisposable
    {
        public TestServer TestServer;
        public string ServerBaseAddress = "http://localhost:50918/";

        public BaseWebApiTest()
        {
            TestServer = TestServer.Create<Startup>();
        }

        public void Dispose()
        {
            if (TestServer != null) TestServer.Dispose();
        }

        public virtual HttpResponseMessage Get(string url)
        {
            return
                TestServer.CreateRequest(ServerBaseAddress + url)
                    .GetAsync()
                    .Result;
        }

        public virtual HttpResponseMessage Post<TModel>(string url, TModel model)
        {
            return
                TestServer.CreateRequest(ServerBaseAddress + url)
                    .And(request => request.Content = new ObjectContent(typeof(TModel), model, new JsonMediaTypeFormatter()))
                    .PostAsync()
                    .Result;
        }
    }

    public abstract class BaseWebApiAuthenticatedTest : BaseWebApiTest
    {
        public string Token;

        public BaseWebApiAuthenticatedTest()
        {
        }


        public override HttpResponseMessage Get(string url)
        {
            return
                TestServer.CreateRequest(ServerBaseAddress + url)
                    .AddHeader("Authorization", "Bearer " + Token)
                    .GetAsync()
                    .Result;
        }

        public override HttpResponseMessage Post<TModel>(string url, TModel model)
        {
            return
                TestServer.CreateRequest(ServerBaseAddress + url)
                    .And(request => request.Content = new ObjectContent(typeof(TModel), model, new JsonMediaTypeFormatter()))
                    .AddHeader("Authorization", "Bearer " + Token)
                    .PostAsync()
                    .Result;
        }
    }

    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var configuration = new HttpConfiguration();
            WebApiConfig.Register(configuration);
        }
    }

    [TestClass]
    public class ValuesControllerIntegrationTest : BaseWebApiTest
    {
        public string Url = "api/Values";

        [TestInitialize]
        public void FixtureInit()
        {
        }

        [TestCleanup]
        public void FixtureDispose()
        {
        }

        [TestMethod]
        public void PostTestMethod()
        {
            var a = Get(Url);
        }
    }
}

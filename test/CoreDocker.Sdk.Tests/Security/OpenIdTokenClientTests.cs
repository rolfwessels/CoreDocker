using System.Linq;
using System.Threading.Tasks;
using CoreDocker.Sdk.RestApi;
using CoreDocker.Sdk.RestApi.Clients;
using CoreDocker.Sdk.Tests.Shared;
using CoreDocker.Utilities.Helpers;
using FluentAssertions;
using Flurl.Http;
using NUnit.Framework;

namespace CoreDocker.Sdk.Tests.Security
{
    [TestFixture]
    [Category("Integration")]
    public class OpenIdTokenClientTests : IntegrationTestsBase
    {
        private ICoreDockerApi _connection;

        private ProjectApiClient _projectApiClient;

        #region Setup/Teardown

        protected void Setup()
        {
            _connection = _defaultRequestFactory.Value.GetConnection();
            _projectApiClient = _connection.Projects;
        }

        [TearDown]
        public void TearDown()
        {
        }

        #endregion

        [Test]
        public async Task CheckForWellKnowConfig_WhenCalled_ShouldHaveResult()
        {
            // arrange
            Setup();
            // action
            var flurlClient = new FlurlClient(_hostAddress.Value);
            var s = await flurlClient.Request(".well-known/openid-configuration/jwks")
                .GetStringAsync();
            var data = await _connection.Authenticate.GetConfigAsync();
            data.Keys.Dump("data.Keys");
            data.Keys.First().Keys.Should().Contain("kty");
        }

        [Test]
        public async Task GivenAuthorization_WhenCalled_ShouldHaveResult()
        {
            // arrange
            Setup();
            // action
            var data = await _connection.Authenticate.GetToken("coredocker.api",AdminUser,AdminPassword);
            data.Should().BeNull();
            data.AccessToken.Should().NotBeEmpty();
        }
    }
}
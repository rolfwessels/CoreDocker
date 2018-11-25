using System.Linq;
using System.Threading.Tasks;
using CoreDocker.Sdk.RestApi;
using CoreDocker.Utilities.Helpers;
using FluentAssertions;
using NUnit.Framework;

namespace CoreDocker.Api.Tests.Integration
{
    [TestFixture]
    [Category("Integration")]
    public class AuthenticateApiClientTests : IntegrationTestsBase
    {
        private ICoreDockerClient _connection;
        private ICoreDockerClient _connectionAuth;


        #region Setup/Teardown

        protected void Setup()
        {
            _connection = _defaultRequestFactory.Value.GetConnection();
            _connectionAuth = _defaultRequestFactory.Value.GetConnection();
//            _connection = new CoreDockerClient("http://localhost:5000");
//            _connectionAuth = new CoreDockerClient("http://localhost:5000");   
        }

        [TearDown]
        public void TearDown()
        {
        }

        #endregion

        [Test]
        public async Task AfterLogin_WhenUsingApi_ShouldGetResults()
        {
            // arrange
            Setup();
            var pingModel = await _connection.Ping.Get();
            // action
            var data = await _connectionAuth.Authenticate.Login(AdminUser, AdminPassword);
            _connection.SetToken(data);
            var projectsEnumerable = await _connection.Projects.All();
            // assert
            pingModel.Environment.ToLower().Should().Be("development"); //??
            projectsEnumerable.Count().Should().BeGreaterThan(0);
        }

        [Test]
        public async Task CheckForWellKnowConfig_WhenCalled_ShouldHaveResult()
        {
            // arrange
            Setup();
            // action
            var data = await _connection.Authenticate.GetConfigAsync();
            // assert
            data.Keys.Dump("data.Keys");
            data.Keys.First().Keys.Should().Contain("kty");
        }

        [Test]
        public async Task GivenAuthorization_WhenCalled_ShouldHaveResult()
        {
            // arrange
            Setup();
            // action
            var data = await _connection.Authenticate.Login(AdminUser, AdminPassword);
            // assert
            data.AccessToken.Should().NotBeEmpty();
            data.ExpiresIn.Should().BeGreaterThan(30);
            data.TokenType.Should().Be("Bearer");
        }
    }
}
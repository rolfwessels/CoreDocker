using System;
using System.Linq;
using System.Threading.Tasks;
using Bumbershoot.Utilities.Helpers;
using CoreDocker.Sdk.RestApi;
using FluentAssertions;
using NUnit.Framework;
using static HotChocolate.ErrorCodes;

namespace CoreDocker.Api.Tests.Integration
{
    [TestFixture]
    [Category("Integration")]
    public class AuthenticateApiClientTests : IntegrationTestsBase
    {
        private ICoreDockerClient? _connection;
        private ICoreDockerClient? _connectionAuth;

        #region Setup/Teardown

        protected void Setup()
        {
            var connectionFactory = DefaultRequestFactory();
            _connection = connectionFactory.GetConnection();
            _connectionAuth = connectionFactory.GetConnection();  
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
            var pingModel = await _connection!.Ping.Get();
            await _connection!.Authenticate.GetConfiguration();
            // action
            var data = await _connectionAuth!.Authenticate.Login(AdminUser, AdminPassword);
            _connection.SetToken(data);
            var projectsEnumerable = await _connection.Projects.All();
            // assert
            pingModel.Environment.ToLower().Should().NotBeEmpty();
            projectsEnumerable.Count().Should().BeGreaterThan(0);
        }

        [Test]
        public async Task CheckForWellKnowConfig_WhenCalled_ShouldHaveResult()
        {
            // arrange
            Setup();
            // action
            var data = await _connection!.Authenticate.GetConfigAsync();
            // assert
            data.Keys.Dump("data.Keys");
            data.Keys.First().Keys.Should().Contain("kty");
        }

        [Test]
        public async Task GetConfiguration_WhenCalled_ShouldHaveResult()
        {
            // arrange
            Setup();
            // action
            var data = await _connection!.Authenticate.GetConfiguration();
            // assert
            data.Should().Contain("http://localhost/connect/token");
        }

        [Test]
        public async Task GivenAuthorization_WhenCalled_ShouldHaveResult()
        {
            // arrange
            Setup();
            // action
            var auth = await _connection!.Authenticate.Login(AdminUser, AdminPassword);
            
            var userInfoResponse = await _connection!.Authenticate.UserInfo();
            // assert
            auth.AccessToken.Should().NotBeEmpty();
            auth.ExpiresIn.Should().BeGreaterThan(30);
            auth.TokenType.Should().Be("Bearer");
        }
    }
}
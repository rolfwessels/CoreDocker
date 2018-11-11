using System.Collections.Generic;
using System.Threading.Tasks;
using CoreDocker.Sdk.SignalR;
using CoreDocker.Sdk.Tests.Shared;
using CoreDocker.Utilities.Helpers;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace CoreDocker.Sdk.Tests.SignalR
{
    [TestFixture]
    public class ChatHubTests : IntegrationTestsBase
    {
        #region Setup/Teardown

        public void Setup()
        {
        }

        #endregion

        [Test]
        public async Task Send_WhenWithMessage_ShouldBroadCastMessage()
        {
            // arrange
            Setup();

            var hostAddressValue = _hostAddress.Value;
//            var hostAddressValue = "http://localhost:5000";
            var coreDockerSockets = new CoreDockerSockets(hostAddressValue)
            {
                OverrideLogging = builder => builder.AddConsole()
            };
            var expected = "hi " + GetRandom.FirstName();

            // assert
            var list = new List<string>();
            await coreDockerSockets.Chat.OnReceived(s => list.Add(s));
            await coreDockerSockets.Chat.Send(expected);
            await coreDockerSockets.Chat.Send(expected);
            await coreDockerSockets.Chat.Send(expected);
            list.WaitFor(x => x.Count > 3);
            list.Should().HaveCount(3).And.Contain(expected);
        }
    }
}
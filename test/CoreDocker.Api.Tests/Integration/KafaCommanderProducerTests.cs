using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CoreDocker.Core.Components.Users;
using CoreDocker.Core.Framework.CommandQuery;
using CoreDocker.Dal.MongoDb;
using CoreDocker.Utilities.Tests;
using CoreDocker.Utilities.Tests.Helpers;
using FizzWare.NBuilder.Generators;
using MediatR;
using NUnit.Framework;

namespace CoreDocker.Api.Tests.Integration
{
    public class KafaCommanderProducerTests 
    {
        #region Setup/Teardown

        public void Setup()
        {

        }
        
        #endregion
        //
        // [Test]
        // public async Task Somethign()
        // {
        //     TestLoggingHelper.EnsureExists();
        //     var kafaCommanderProducer = new KafaCommanderProducer();
        //     var tokenSource = new CancellationTokenSource();
        //     await kafaCommanderProducer.Execute(UserCreate.Request.From(ObjectIdGenerator.Id,GetRandom.FirstName(),GetRandom.Email(),GetRandom.MacAddress(),new List<string>() {RoleManager.Admin.Name}), tokenSource.Token);
        //     var kafaCommanderConsumer = new KafaCommanderConsumer(new Mediator());
        //     var start = kafaCommanderConsumer.Start(tokenSource.Token);
        //     await Task.Delay(2000, tokenSource.Token);
        //     await start;
        //     tokenSource.Cancel();
        // }
    }
}
using System;
using System.Linq;
using Dwapi.ExtractsManagement.Infrastructure;
using Dwapi.SettingsManagement.Core.Model;
using Dwapi.SharedKernel.DTOs;
using Dwapi.SharedKernel.Exchange;
using Dwapi.SharedKernel.Tests.TestHelpers;
using Dwapi.UploadManagement.Core.Exchange.Hts;
using Dwapi.UploadManagement.Core.Interfaces.Packager.Hts;
using Dwapi.UploadManagement.Core.Interfaces.Reader;
using Dwapi.UploadManagement.Core.Interfaces.Reader.Hts;
using Dwapi.UploadManagement.Core.Interfaces.Services.Hts;
using Dwapi.UploadManagement.Core.Packager.Hts;
using Dwapi.UploadManagement.Core.Services.Hts;
using Dwapi.UploadManagement.Infrastructure.Data;
using Dwapi.UploadManagement.Infrastructure.Reader;
using Dwapi.UploadManagement.Infrastructure.Reader.Hts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Dwapi.UploadManagement.Core.Tests.Services.Hts
{
    [TestFixture]
    [Category("Hts")]
    public class HtsSendServiceTests
    {
        private readonly string _authToken = @"1983aeda-1a96-30e9-adc0-fa7ae01bbebc";
        private readonly string _subId = "DWAPI";
        private readonly string url = "http://localhost:7777";

        private IHtsSendService _htsSendService;
        private IServiceProvider _serviceProvider;
        private ManifestMessageBag _bag;
        private HtsMessageBag _clientBag;
        private CentralRegistry _registry;
        private ExtractsContext _context;

        [OneTimeSetUp]
        public void Init()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            var connectionString = config["ConnectionStrings:DwapiConnection"];

            /*
                22704|TOGONYE DISPENSARY|KIRINYAGA
                22696|HERTLANDS MEDICAL CENTRE|NAROK
            */

            _bag = TestDataFactory.ManifestMessageBag(2,10001,10002);
            _clientBag = TestDataFactory.HtsMessageBag(5, 10001, 10002);
        }

        [SetUp]
        public void SetUp()
        {
            _registry = new CentralRegistry(url, "HTS")
            {
                AuthToken = _authToken,
                SubscriberId = _subId
            };
            _htsSendService = TestInitializer.ServiceProvider.GetService<IHtsSendService>();
            _context=TestInitializer.ServiceProvider.GetService<ExtractsContext>();
        }


        [Test,Order(1)]
        public void should_Send_Manifest()
        {
            var sendTo=new SendManifestPackageDTO(_registry);
            //var responses = _htsSendService.SendManifestAsync(sendTo, _bag).Result;
            var responses = _htsSendService.SendManifestAsync(sendTo).Result;
            Assert.NotNull(responses);
            Assert.False(responses.Select(x=>x.IsValid()).Any(x=>false));

            foreach (var sendManifestResponse in responses)
            {
                var sql = $"select count(Id) from Manifests where Id='{sendManifestResponse.Message.Manifest.Id}'";
                Assert.AreEqual(1,TestInitializer.ExecQuery<int>(sql));
                Console.WriteLine(sendManifestResponse);
            }
        }


        [Test,Order(2)]
        public void should_Send_Clients()
        {


            var sendTo = new SendManifestPackageDTO(_registry);
            //var responses = _htsSendService.SendClientsAsync(sendTo, _clientBag).Result;
            var responses = _htsSendService.SendClientsAsync(sendTo).Result;
            Assert.NotNull(responses);
            Assert.False(responses.Select(x => x.IsValid()).Any(x => false));
            foreach (var sendManifestResponse in responses)
            {
                Console.WriteLine(sendManifestResponse);
            }



        }

        [Test, Order(3)]
        public void should_Send_cLinkages()
        {
             var sendTo = new SendManifestPackageDTO(_registry);
            //var responses = _htsSendService.SendClientsLinkagesAsync(sendTo, _clientBag).Result;
            var responses = _htsSendService.SendClientsLinkagesAsync(sendTo).Result;
            Assert.NotNull(responses);
            Assert.False(responses.Select(x => x.IsValid()).Any(x => false));
            foreach (var sendManifestResponse in responses)
            {
                Console.WriteLine(sendManifestResponse);
            }
        }

        [Test,Order(3)]
        public void should_Send_ClientTest()
        {
            var sendTo = new SendManifestPackageDTO(_registry);
            //var responses = _htsSendService.SendClientTestsAsync(sendTo, _clientBag).Result;
            var responses = _htsSendService.SendClientTestsAsync(sendTo).Result;
            Assert.NotNull(responses);
            Assert.False(responses.Select(x => x.IsValid()).Any(x => false));
            foreach (var sendManifestResponse in responses)
            {
                Console.WriteLine(sendManifestResponse);
            }

        }

        [Test,Order(3)]
        public void should_Send_TestKits()
        {
            var sendTo = new SendManifestPackageDTO(_registry);
            //var responses = _htsSendService.SendTestKitsAsync(sendTo, _clientBag).Result;
            var responses = _htsSendService.SendTestKitsAsync(sendTo).Result;
            Assert.NotNull(responses);
            Assert.False(responses.Select(x => x.IsValid()).Any(x => false));
            foreach (var sendManifestResponse in responses)
            {
                Console.WriteLine(sendManifestResponse);
            }
        }

        [Test,Order(3)]
        public void should_Send_ClientTracing()
        {
            var sendTo = new SendManifestPackageDTO(_registry);
            //var responses = _htsSendService.SendClientTracingAsync(sendTo, _clientBag).Result;
            var responses = _htsSendService.SendClientTracingAsync(sendTo).Result;
            Assert.NotNull(responses);
            Assert.False(responses.Select(x => x.IsValid()).Any(x => false));
            foreach (var sendManifestResponse in responses)
            {
                Console.WriteLine(sendManifestResponse);
            }
        }

        [Test,Order(3)]
        public void should_Send_PartnerTracing()
        {
            var sendTo = new SendManifestPackageDTO(_registry);
            //var responses = _htsSendService.SendPartnerTracingAsync(sendTo, _clientBag).Result;
            var responses = _htsSendService.SendPartnerTracingAsync(sendTo).Result;
            Assert.NotNull(responses);
            Assert.False(responses.Select(x => x.IsValid()).Any(x => false));
            foreach (var sendManifestResponse in responses)
            {
                Console.WriteLine(sendManifestResponse);
            }
         }

        [Test,Order(3)]
        public void should_Send_PartnerNotificationServices()
        {
            var sendTo = new SendManifestPackageDTO(_registry);
            //var responses = _htsSendService.SendPartnerNotificationServicesAsync(sendTo, _clientBag).Result;
            var responses = _htsSendService.SendPartnerNotificationServicesAsync(sendTo).Result;
            Assert.NotNull(responses);
            Assert.False(responses.Select(x => x.IsValid()).Any(x => false));
            foreach (var sendManifestResponse in responses)
            {
                Console.WriteLine(sendManifestResponse);
            }
        }

        [Test,Order(4)]
        public void should_Validate_Sent()
        {
            var sqlCentral6 = $"select count(Id) from ClientLinkages";
            var localCount6 = _context.HtsClientsLinkageExtracts.Select(x => x.Id).Count();
            Assert.AreEqual(localCount6, TestInitializer.ExecQuery<int>(sqlCentral6));

            var sqlCentral5 = $"select count(Id) from Clients";
            var localCount5 = _context.HtsClientsExtracts.Select(x => x.Id).Count();
            Assert.AreEqual(localCount5,TestInitializer.ExecQuery<int>(sqlCentral5));

            var sqlCentral4 = $"select count(Id) from HtsClientTests";
            var localCount4 = _context.HtsClientTestsExtracts.Select(x => x.Id).Count();
            Assert.AreEqual(localCount4, TestInitializer.ExecQuery<int>(sqlCentral4));

            var sqlCentral3 = $"select count(Id) from HtsTestKits";
            var localCount3 = _context.HtsTestKitsExtracts.Select(x => x.Id).Count();
            Assert.AreEqual(localCount3, TestInitializer.ExecQuery<int>(sqlCentral3));

            var sqlCentral2 = $"select count(Id) from HtsClientTracing";
            var localCount2 = _context.HtsClientTracingExtracts.Select(x => x.Id).Count();
            Assert.AreEqual(localCount2, TestInitializer.ExecQuery<int>(sqlCentral2));

            var sqlCentral1 = $"select count(Id) from HtsPartnerTracings";
            var localCount1 = _context.HtsPartnerTracingExtracts.Select(x => x.Id).Count();
            Assert.AreEqual(localCount1, TestInitializer.ExecQuery<int>(sqlCentral1));

            var sqlCentral = $"select count(Id) from HtsPartnerNotificationServices";
            var localCount = _context.HtsPartnerNotificationServicesExtracts.Select(x => x.Id).Count();
            Assert.AreEqual(localCount, TestInitializer.ExecQuery<int>(sqlCentral));
        }
    }
}

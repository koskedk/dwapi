using System;
using System.Linq;
using Dwapi.SettingsManagement.Core.Model;
using Dwapi.SharedKernel.DTOs;
using Dwapi.SharedKernel.Exchange;
using Dwapi.SharedKernel.Tests.TestHelpers;
using Dwapi.UploadManagement.Core.Exchange.Dwh;
using Dwapi.UploadManagement.Core.Interfaces.Packager.Dwh;
using Dwapi.UploadManagement.Core.Interfaces.Reader.Dwh;
using Dwapi.UploadManagement.Core.Interfaces.Services.Dwh;
using Dwapi.UploadManagement.Core.Packager.Dwh;
using Dwapi.UploadManagement.Core.Services.Dwh;
using Dwapi.UploadManagement.Infrastructure.Data;
using Dwapi.UploadManagement.Infrastructure.Reader.Dwh;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Dwapi.UploadManagement.Core.Tests.Services.Dwh
{
    [TestFixture]
    [Category("Dwh")]
    public class CTSendServiceTests
    {
        private readonly string _authToken = @"1ba47c2a-6e05-11e8-adc0-fa7ae01bbebc";
        private readonly string _subId = "DWAPI";
        private readonly string url = "http://192.168.43.15/dwapi";

        private ICTSendService _ctSendService;
        private DwhManifestMessageBag _bag;
        private ArtMessageBag _artBag;
        private CentralRegistry _registry;

        [OneTimeSetUp]
        public void Init()
        {
         //   TestInitializer.ClearDb();
            _bag = TestDataFactory.DwhManifestMessageBag(2,10001);
            _artBag = TestDataFactory.ArtMessageBag(5, 10001);
        }

        [SetUp]
        public void SetUp()
        {
            _registry = new CentralRegistry(url, "NDWH")
            {
                AuthToken = _authToken,
                SubscriberId = _subId
            };
            _ctSendService = TestInitializer.ServiceProvider.GetService<ICTSendService>();
        }

        [Test]
        public void should_Send_Manifest()
        {
            var sendTo=new SendManifestPackageDTO(_registry);

            var responses = _ctSendService.SendManifestAsync(sendTo, _bag).Result;
            Assert.NotNull(responses);
            Assert.False(responses.Select(x=>x.IsValid()).Any(x=>false));
            foreach (var sendManifestResponse in responses)
            {
                Console.WriteLine(sendManifestResponse);
            }
        }

        [Test]
        public void should_Send_ART()
        {
            var sendTo=new SendManifestPackageDTO(_registry);

            var responses = _ctSendService.SendBatchExtractsAsync(sendTo, 200, new ArtMessageBag()).Result;
            Assert.NotNull(responses);
            Assert.False(responses.Select(x=>x.IsValid()).Any(x=>false));
            foreach (var sendManifestResponse in responses)
            {
                Console.WriteLine(sendManifestResponse);
            }
        }


    }
}

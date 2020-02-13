using System;
using System.Linq;
using Dwapi.ExtractsManagement.Infrastructure;
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
        private readonly string url = "http://192.168.2.192:3000";

        private ICTSendService _ctSendService;
        private DwhManifestMessageBag _bag;
        private ArtMessageBag _artBag;
        private CentralRegistry _registry;
        private ExtractsContext _context;

        [OneTimeSetUp]
        public void Init()
        {
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
            _context=TestInitializer.ServiceProvider.GetService<ExtractsContext>();
        }

        [Test,Order(1)]
        public void should_Send_Manifest()
        {
            var sendTo = new SendManifestPackageDTO(_registry);

            var responses = _ctSendService.SendManifestAsync(sendTo).Result;
            Assert.NotNull(responses);
            Assert.False(responses.Select(x => x.IsValid()).Any(x => false));
            foreach (var m in responses)
            {
                Console.WriteLine(m);
            }
        }

        [Test,Order(2)]
        public void should_Send_ART()
        {
            var sendTo=new SendManifestPackageDTO(_registry);

            var responses = _ctSendService.SendBatchExtractsAsync(sendTo, 200, new ArtMessageBag()).Result;
            Assert.NotNull(responses);
            Assert.False(responses.Select(x=>x.IsValid()).Any(x=>false));
        }
        [Test,Order(2)]
        public void should_Send_AdverseEvent()
        {
            var sendTo=new SendManifestPackageDTO(_registry);

            var responses = _ctSendService.SendBatchExtractsAsync(sendTo, 200, new AdverseEventsMessageBag()).Result;
            Assert.NotNull(responses);
            Assert.False(responses.Select(x=>x.IsValid()).Any(x=>false));
        }
        [Test,Order(2)]
        public void should_Send_Baseline()
        {
            var sendTo=new SendManifestPackageDTO(_registry);

            var responses = _ctSendService.SendBatchExtractsAsync(sendTo, 200, new BaselineMessageBag()).Result;
            Assert.NotNull(responses);
            Assert.False(responses.Select(x=>x.IsValid()).Any(x=>false));
        }
        [Test,Order(2)]
        public void should_Send_Lab()
        {
            var sendTo=new SendManifestPackageDTO(_registry);

            var responses = _ctSendService.SendBatchExtractsAsync(sendTo, 200, new LabMessageBag()).Result;
            Assert.NotNull(responses);
            Assert.False(responses.Select(x=>x.IsValid()).Any(x=>false));
        }
        [Test,Order(2)]
        public void should_Send_Pharmacy()
        {
            var sendTo=new SendManifestPackageDTO(_registry);

            var responses = _ctSendService.SendBatchExtractsAsync(sendTo, 200, new PharmacyMessageBag()).Result;
            Assert.NotNull(responses);
            Assert.False(responses.Select(x=>x.IsValid()).Any(x=>false));
        }
        [Test,Order(2)]
        public void should_Send_Status()
        {
            var sendTo=new SendManifestPackageDTO(_registry);

            var responses = _ctSendService.SendBatchExtractsAsync(sendTo, 200, new StatusMessageBag()).Result;
            Assert.NotNull(responses);
            Assert.False(responses.Select(x=>x.IsValid()).Any(x=>false));
        }

        [Test,Order(2)]
        public void should_Send_Visit()
        {
            var sendTo=new SendManifestPackageDTO(_registry);

            var responses = _ctSendService.SendBatchExtractsAsync(sendTo, 200, new VisitsMessageBag()).Result;
            Assert.NotNull(responses);
            Assert.False(responses.Select(x=>x.IsValid()).Any(x=>false));
        }

        [Test,Order(4)]
        public void should_Validate_Sent()
        {
            var sqlCentral6 = $"select count(Id) from PatientArtExtract";
            var localCount6 = _context.PatientArtExtracts.Select(x => x.Id).Count();
            Assert.AreEqual(localCount6, TestInitializer.ExecQuery<int>(sqlCentral6,"Ct"));

            var sqlCentral61 = $"select count(Id) from PatientBaselinesExtract";
            var localCount61 = _context.PatientArtExtracts.Select(x => x.Id).Count();
            Assert.AreEqual(localCount61, TestInitializer.ExecQuery<int>(sqlCentral61,"Ct"));

            var sqlCentral62 = $"select count(Id) from PatientLaboratoryExtract";
            var localCount62 = _context.PatientArtExtracts.Select(x => x.Id).Count();
            Assert.AreEqual(localCount62, TestInitializer.ExecQuery<int>(sqlCentral62,"Ct"));

            var sqlCentral64 = $"select count(Id) from PatientPharmacyExtract";
            var localCount64 = _context.PatientArtExtracts.Select(x => x.Id).Count();
            Assert.AreEqual(localCount64, TestInitializer.ExecQuery<int>(sqlCentral64,"Ct"));

            var sqlCentral65 = $"select count(Id) from PatientStatusExtract";
            var localCount65 = _context.PatientArtExtracts.Select(x => x.Id).Count();
            Assert.AreEqual(localCount65, TestInitializer.ExecQuery<int>(sqlCentral65,"Ct"));

            var sqlCentral66 = $"select count(Id) from PatientVisitExtract";
            var localCount66 = _context.PatientArtExtracts.Select(x => x.Id).Count();
            Assert.AreEqual(localCount66, TestInitializer.ExecQuery<int>(sqlCentral66,"Ct"));

            var sqlCentral67 = $"select count(Id) from PatientAdverseEventExtract";
            var localCount67 = _context.PatientArtExtracts.Select(x => x.Id).Count();
            Assert.AreEqual(localCount67, TestInitializer.ExecQuery<int>(sqlCentral67,"Ct"));


        }
        /*
INSERT INTO sys.tables (name) VALUES ('FacilityManifestCargo');
INSERT INTO sys.tables (name) VALUES ('lkp_USGPartnerMenchanism');
INSERT INTO sys.tables (name) VALUES ('Facility');
INSERT INTO sys.tables (name) VALUES ('PatientExtract');
INSERT INTO sys.tables (name) VALUES ('PatientArtExtract');
INSERT INTO sys.tables (name) VALUES ('PatientBaselinesExtract');
INSERT INTO sys.tables (name) VALUES ('PatientLaboratoryExtract');
INSERT INTO sys.tables (name) VALUES ('PatientPharmacyExtract');
INSERT INTO sys.tables (name) VALUES ('PatientStatusExtract');
INSERT INTO sys.tables (name) VALUES ('PatientVisitExtract');
INSERT INTO sys.tables (name) VALUES ('MasterFacility');
INSERT INTO sys.tables (name) VALUES ('__MigrationHistory');
INSERT INTO sys.tables (name) VALUES ('');
INSERT INTO sys.tables (name) VALUES ('PatientAdverseEventExtract');

         */

    }
}

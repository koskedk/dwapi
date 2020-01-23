using System;
using System.Linq;
using Dwapi.UploadManagement.Core.Interfaces.Packager.Dwh;
using Dwapi.UploadManagement.Core.Interfaces.Reader;
using Dwapi.UploadManagement.Core.Interfaces.Reader.Dwh;
using Dwapi.UploadManagement.Core.Model.Dwh;
using Dwapi.UploadManagement.Core.Packager.Dwh;
using Dwapi.UploadManagement.Infrastructure.Data;
using Dwapi.UploadManagement.Infrastructure.Reader;
using Dwapi.UploadManagement.Infrastructure.Reader.Dwh;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Dwapi.UploadManagement.Core.Tests.Packager.Dwh
{
    [TestFixture]
    [Category("Dwh")]
    public class CTPackagerTests
    {
        private ICTPackager _packager;
        private Guid _pid;

        [OneTimeSetUp]
        public void Init()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            var connectionString = config["ConnectionStrings:DwapiConnection"];


            var ctx =TestInitializer.ServiceProvider.GetService<UploadContext>();
            var art= ctx.ClientPatientArtExtracts.First();
            _pid = ctx.ClientPatientExtracts.First(x => x.PatientPK == art.PatientPK && x.SiteCode == art.SiteCode).Id;
        }


        [SetUp]
        public void SetUp()
        {
            _packager =TestInitializer.ServiceProvider.GetService<ICTPackager>();

        }

        [Test]
        public void should_Generate_Manifest()
        {
            var manfiests = _packager.Generate().ToList();
            Assert.True(manfiests.Any());
            var m = manfiests.First();
            Assert.True(m.PatientPks.Any());
            Console.WriteLine($"{m}");
        }

        [Test]
        public void should_Generate_Manifest_With_Metrics()
        {
            var manfiests = _packager.GenerateWithMetrics().ToList();
            Assert.True(manfiests.Any());
            var m = manfiests.First();
            Assert.True(m.PatientPks.Any());
            Assert.True(m.FacMetrics.Any());
            Console.WriteLine($"{m}");
            Console.WriteLine(m.Metrics);
        }

        [Test]
        public void should_Generate_Art_Extracts()
        {
            var extracts = _packager.GenerateBatchExtracts<PatientArtExtractView,Guid>(1,1);
            Assert.True(extracts.Any());
        }

        [Test]
        public void should_Get_Art_PackageInfo()
        {
            var extracts = _packager.GetPackageInfo<PatientArtExtractView,Guid>(1);
            Assert.True(extracts.PageCount>0);
            Assert.True(extracts.TotalRecords>0);
        }
    }
}

using System;
using System.Linq;
using Dwapi.UploadManagement.Core.Interfaces.Reader.Dwh;
using Dwapi.UploadManagement.Core.Model.Dwh;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Dwapi.UploadManagement.Infrastructure.Tests.Reader
{
    [TestFixture]
    public class CTExtractReaderTests
    {
        private ICTExtractReader _reader;

        [OneTimeSetUp]
        public void Init()
        {

        }

        [SetUp]
        public void SetUp()
        {
            _reader =TestInitializer.ServiceProvider.GetService<ICTExtractReader>();
        }

        [Test]
        public void should_ReadProfiles()
        {
            var profiles = _reader.ReadProfiles().ToList();
            Assert.True(profiles.Any());
        }

        [Test]
        public void should_ReadIds()
        {
            var extractViews = _reader.ReadAllIds().ToList();
            Assert.True(extractViews.Any());
        }


        [Test]
        public void should_ART_ReadPaged()
        {
            var extractViews = _reader.Read<PatientArtExtractView,Guid>(1, 2).ToList();
            Assert.True(extractViews.Any());
            Assert.True(extractViews.Count==2);
            Assert.NotNull(extractViews.First().PatientExtractView);
        }

        [Test]
        public void should_ART_Count()
        {
            var totalRecords = _reader.GetTotalRecords<PatientArtExtractView,Guid>();
            Assert.True(totalRecords>0);
        }
    }
}

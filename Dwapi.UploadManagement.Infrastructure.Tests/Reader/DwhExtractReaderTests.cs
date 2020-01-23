using System.Linq;
using Dwapi.UploadManagement.Core.Interfaces.Reader.Dwh;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Dwapi.UploadManagement.Infrastructure.Tests.Reader
{
    [TestFixture]
    public class DwhExtractReaderTests
    {
        private IDwhExtractReader _reader;

        [SetUp]
        public void SetUp()
        {
            _reader =TestInitializer.ServiceProvider.GetService<IDwhExtractReader>();
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
    }
}

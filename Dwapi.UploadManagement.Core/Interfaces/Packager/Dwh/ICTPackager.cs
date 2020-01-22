using System;
using System.Collections.Generic;
using Dwapi.SharedKernel.Exchange;
using Dwapi.UploadManagement.Core.Model.Dwh;

namespace Dwapi.UploadManagement.Core.Interfaces.Packager.Dwh
{
    public interface ICTPackager
    {
        IEnumerable<DwhManifest> Generate();
        IEnumerable<DwhManifest> GenerateWithMetrics();
        PatientExtractView GenerateExtracts(Guid id);
    }
}
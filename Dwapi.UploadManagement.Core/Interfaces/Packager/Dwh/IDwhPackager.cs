﻿using System;
using System.Collections.Generic;
using Dwapi.SharedKernel.Exchange;
using Dwapi.UploadManagement.Core.Model.Dwh;

namespace Dwapi.UploadManagement.Core.Interfaces.Packager.Dwh
{
    public interface IDwhPackager
    {
        IEnumerable<DwhManifest> Generate();
        IEnumerable<PatientExtractView> GenerateExtracts();
        PatientExtractView GenerateExtracts(Guid id);
    }
}
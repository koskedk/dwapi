using System;
using System.Collections.Generic;
using Dwapi.ExtractsManagement.Core.Model.Destination.Dwh;
using Dwapi.SharedKernel.Exchange;
using Dwapi.SharedKernel.Model;
using Dwapi.UploadManagement.Core.Model.Dwh;

namespace Dwapi.UploadManagement.Core.Interfaces.Packager.Dwh
{
    public interface ICTPackager
    {
        IEnumerable<DwhManifest> Generate();
        IEnumerable<DwhManifest> GenerateWithMetrics();

        IEnumerable<T>  GenerateBatchExtracts<T, TId>(int page,int batchSize) where T : Entity<TId>;
        IEnumerable<T>  GenerateBatchExtracts<T>(int page,int batchSize) where T :ClientExtract;

        PackageInfo GetPackageInfo<T, TId>(int batchSize) where T : Entity<TId>;
        PackageInfo GetPackageInfo<T>(int batchSize) where T : ClientExtract;
    }
}

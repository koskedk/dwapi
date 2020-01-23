using System;
using System.Collections.Generic;
using Dwapi.SharedKernel.Exchange;
using Dwapi.SharedKernel.Model;
using Dwapi.UploadManagement.Core.Model.Dwh;

namespace Dwapi.UploadManagement.Core.Interfaces.Reader.Dwh
{
    public interface ICTExtractReader
    {
        IEnumerable<SitePatientProfile> ReadProfiles();
        IEnumerable<Guid> ReadAllIds();

        IEnumerable<T> Read<T, TId>(int page, int pageSize) where T : Entity<TId>;
        long GetTotalRecords<T, TId>() where T : Entity<TId>;
    }
}

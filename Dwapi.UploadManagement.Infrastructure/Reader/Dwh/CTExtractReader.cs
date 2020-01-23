using System;
using System.Collections.Generic;
using System.Linq;
using Dwapi.SharedKernel.Exchange;
using Dwapi.SharedKernel.Model;
using Dwapi.UploadManagement.Core.Interfaces.Reader.Dwh;
using Dwapi.UploadManagement.Core.Model.Dwh;
using Dwapi.UploadManagement.Core.Packager;
using Dwapi.UploadManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Dwapi.UploadManagement.Infrastructure.Reader.Dwh
{
    public class CTExtractReader:ICTExtractReader
    {
        private readonly UploadContext _context;

        public CTExtractReader(UploadContext context)
        {
            _context = context;
        }

        public IEnumerable<SitePatientProfile> ReadProfiles()
        {
            return
                _context.ClientPatientExtracts.AsNoTracking()
                .Select(x =>
                    new SitePatientProfile(x.SiteCode, x.FacilityName, x.PatientPK)
                );
        }

        public IEnumerable<Guid> ReadAllIds()
        {
            return _context.ClientPatientExtracts.Where(x=>!x.IsSent).AsNoTracking().Select(x=>x.Id);
        }

        public IEnumerable<T> Read<T, TId>(int page, int pageSize) where T : Entity<TId>
        {
            return _context.Set<T>()
                .Include(nameof(PatientExtractView))
                .Skip((page - 1) * pageSize).Take(pageSize)
                .OrderBy(x => x.Id)
                .AsNoTracking();
        }

        public long GetTotalRecords<T, TId>() where T : Entity<TId>
        {
            return _context.Set<T>()
                .Select(x => x.Id)
                .LongCount();
        }
    }
}

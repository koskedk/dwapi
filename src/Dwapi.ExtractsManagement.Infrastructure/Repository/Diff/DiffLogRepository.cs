using System;
using Dapper;
using Dwapi.ExtractsManagement.Core.Interfaces.Repository.Diff;
using Dwapi.ExtractsManagement.Core.Model.Destination.Dwh;
using Dwapi.ExtractsManagement.Core.Model.Diff;
using Dwapi.SharedKernel.Infrastructure.Repository;
using Dwapi.SharedKernel.Model;
using Dwapi.SharedKernel.Utility;
using Microsoft.EntityFrameworkCore;
using Z.Dapper.Plus;

namespace Dwapi.ExtractsManagement.Infrastructure.Repository.Diff
{
    public class DiffLogRepository : BaseRepository<DiffLog, Guid>, IDiffLogRepository
    {
        public DiffLogRepository(ExtractsContext context) : base(context)
        {
        }

        public DiffLog GetLog(string docket, string extract)
        {
            return Get(x =>
                x.Docket.ToLower() == docket.ToLower()
                && x.Extract.ToLower() == extract.ToLower());
        }

        public DiffLog InitLog(string docket, string extract)
        {
            var diffLog = Get(x =>
                x.Docket.ToLower() == docket.ToLower() &&
                x.Extract.ToLower() == extract.ToLower());

            if (null == diffLog)
            {
                diffLog = DiffLog.Create(docket, extract);
                Create(diffLog);
                SaveChanges();
            }

            return diffLog;
        }

        public void SaveLog(DiffLog diffLog)
        {
            Context.Database.GetDbConnection().BulkMerge(diffLog);
        }

        public DiffLog GenerateDiff(string docket,string extract)
        {
            var diffLog = DiffLog.Create(docket, extract);

            var sql =
                $"SELECT MAX({nameof(PatientExtract.Date_Created)}) {nameof(PatientExtract.Date_Created)},MAX({nameof(PatientExtract.Date_Last_Modified)}) {nameof(PatientExtract.Date_Last_Modified)} FROM {extract}";

            var extractDates = Context.Database.GetDbConnection().QuerySingle(sql);

            if (null != extractDates)
            {

                diffLog.MaxCreated = Extentions.CastDateTime(extractDates.Date_Created);
                diffLog.MaxModified = Extentions.CastDateTime(extractDates.Date_Last_Modified);

            }

            return diffLog;
        }
    }
}

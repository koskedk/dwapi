using System;
using Dwapi.ExtractsManagement.Core.Model.Diff;
using Dwapi.SharedKernel.Interfaces;
using Dwapi.SharedKernel.Model;

namespace Dwapi.ExtractsManagement.Core.Interfaces.Repository.Diff
{
    public interface IDiffLogRepository:IRepository<DiffLog,Guid>
    {
        DiffLog GetLog(string docket, string extract);
        DiffLog InitLog(string docket, string extract);
        void SaveLog(DiffLog diffLog);
        DiffLog GenerateDiff(string docket, string extract);
    }
}

﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dwapi.ExtractsManagement.Core.Model.Source.Cbs;
using Dwapi.SharedKernel.Interfaces;

namespace Dwapi.ExtractsManagement.Core.Interfaces.Repository.Cbs
{
    public interface ITempMasterPatientIndexRepository : IRepository<TempMasterPatientIndex,Guid>
    {
        Task Clear();
        bool BatchInsert(IEnumerable<TempMasterPatientIndex> extracts);
    }
}

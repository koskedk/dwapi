﻿using System;

namespace Dwapi.ExtractsManagement.Core.Interfaces.Source
{
    public interface ISource
    {
        Guid Id { get; set; }
        string Emr { get; set; }
        int? FacilityCode { get; set; }
        DateTime? DateExtracted { get; set; }
    }
}
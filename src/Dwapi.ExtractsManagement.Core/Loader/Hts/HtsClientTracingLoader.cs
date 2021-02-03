﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Dwapi.ExtractsManagement.Core.Interfaces.Loaders.Hts;
using Dwapi.ExtractsManagement.Core.Interfaces.Repository.Hts;
using Dwapi.ExtractsManagement.Core.Model.Destination.Hts.NewHts;
using Dwapi.ExtractsManagement.Core.Model.Source.Hts.NewHts;
using Dwapi.ExtractsManagement.Core.Notifications;
using Dwapi.ExtractsManagement.Infrastructure.Repository.Hts.TempExtracts;
using Dwapi.SharedKernel.Events;
using Dwapi.SharedKernel.Model;
using Dwapi.SharedKernel.Utility;
using Serilog;

namespace Dwapi.ExtractsManagement.Core.Loader.Hts
{
    public class HtsClientTracingLoader : IHtsClientTracingLoader
    {
        private readonly IHtsClientTracingExtractRepository _htsClientTracingExtractRepository;
        private readonly ITempHtsClientTracingExtractRepository _tempHtsClientTracingExtractRepository;
        private int Found { get; set; }
        private Guid ExtractId { get; set; }

        public HtsClientTracingLoader(IHtsClientTracingExtractRepository htsClientTracingExtractRepository, ITempHtsClientTracingExtractRepository tempHtsClientTracingExtractRepository)
        {
            _htsClientTracingExtractRepository = htsClientTracingExtractRepository;
            _tempHtsClientTracingExtractRepository = tempHtsClientTracingExtractRepository;
        }

        public async Task<int> Load()
        {
            int count = 0;
            try
            {

                int take = AppConstants.HtsBatch;
                var eCount = await  _tempHtsClientTracingExtractRepository.GetCleanCount();
                var pageCount = _tempHtsClientTracingExtractRepository.PageCount(take, eCount);

                int page = 1;
                while (page <= pageCount)
                {
                    var tempHtsClientTracings =await
                        _tempHtsClientTracingExtractRepository.GetAll(QueryUtil.Tracing, page, take);

                    var batch = tempHtsClientTracings.ToList();
                    count += batch.Count;
                    //Auto mapper
                    var extractRecords = Mapper.Map<List<TempHtsClientTracing>, List<HtsClientTracing>>(batch);
                    foreach (var record in extractRecords)
                    {
                        record.Id = LiveGuid.NewGuid();
                    }
                    //Batch Insert
                    var inserted = _htsClientTracingExtractRepository.BatchInsert(extractRecords);
                    if (!inserted)
                    {
                        Log.Error($"Extract {nameof(HtsClientTracing)} not Loaded");
                        return 0;
                    }
                    Log.Debug("saved batch");
                    page++;
                    /*
                    DomainEvents.Dispatch(
                        new ExtractActivityNotification(extractId, new DwhProgress(
                            nameof(PatientExtract),
                            nameof(ExtractStatus.Loading),
                            found, count, 0, 0, 0)));
                    */
                }
                DomainEvents.Dispatch(new HtsNotification(new ExtractProgress(nameof(HtsClientTracing), "Loading...", Found, 0, 0, 0, 0)));
                return count;

            }
            catch (Exception e)
            {
                Log.Error(e, $"Extract {nameof(HtsClientTracing)} not Loaded");
                throw;
            }
        }

        public Task<int> Load(Guid extractId, int found)
        {
            Found = found;
            ExtractId = extractId;
            return Load();
        }
    }
}

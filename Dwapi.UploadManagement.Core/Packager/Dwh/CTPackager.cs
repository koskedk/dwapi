using System;
using System.Collections.Generic;
using System.Linq;
using Dwapi.SharedKernel.Enum;
using Dwapi.SharedKernel.Exchange;
using Dwapi.UploadManagement.Core.Interfaces.Packager.Dwh;
using Dwapi.UploadManagement.Core.Interfaces.Reader;
using Dwapi.UploadManagement.Core.Interfaces.Reader.Dwh;
using Dwapi.UploadManagement.Core.Model.Dwh;

namespace Dwapi.UploadManagement.Core.Packager.Dwh
{
    public class CTPackager : ICTPackager
    {
        private readonly IDwhExtractReader _reader;
        private readonly IEmrMetricReader _metricReader;

        public CTPackager(IDwhExtractReader reader, IEmrMetricReader metricReader)
        {
            _reader = reader;
            _metricReader = metricReader;
        }


        public IEnumerable<DwhManifest> Generate()
        {
            var patientProfiles = _reader.ReadProfiles();

            return DwhManifest.Create(patientProfiles);
        }

        public IEnumerable<DwhManifest> GenerateWithMetrics()
        {
            var metrics = _metricReader.ReadAll().FirstOrDefault();
            var appMetrics = _metricReader.ReadAppAll().ToList();
            var manifests = Generate().ToList();

            if (null != metrics)
            {
                foreach (var manifest in manifests)
                {
                    manifest.AddCargo(CargoType.Metrics, metrics);
                }
            }

            if (appMetrics.Any())
            {
                foreach (var manifest in manifests)
                {
                    foreach (var m in appMetrics)
                    {
                        manifest.AddCargo(CargoType.AppMetrics, m);
                    }
                }
            }

            return manifests;
        }

        public PatientExtractView GenerateExtracts(Guid id)
        {
            return _reader.Read(id);
        }
    }
}
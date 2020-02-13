using System.ComponentModel.DataAnnotations.Schema;
using Dwapi.ExtractsManagement.Core.Model.Destination.Dwh;

namespace Dwapi.UploadManagement.Core.Model.Dwh
{
    [Table("PatientPharmacyExtracts")]
    public class PatientPharmacyExtractView : PatientPharmacyExtract
    {
        public PatientExtractView PatientExtractView { get; set; }
    }
}

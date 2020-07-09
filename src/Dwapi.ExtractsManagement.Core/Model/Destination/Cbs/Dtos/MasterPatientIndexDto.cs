using Dwapi.ExtractsManagement.Core.Algorithm;

namespace Dwapi.ExtractsManagement.Core.Model.Destination.Cbs.Dtos
{
    public class MasterPatientIndexDto : MasterPatientIndex
    {
        public string sxdmPKValueDoBOther { get; set; }

        public void Score()
        {
            var inputA = sxdmPKValueDoB;
            var inputB = sxdmPKValueDoBOther;

            if ((string.IsNullOrWhiteSpace(inputA) || string.IsNullOrWhiteSpace(inputB)))
                return;

            if ((string.IsNullOrWhiteSpace(inputA) || string.IsNullOrWhiteSpace(inputB)))
                return;

            JaroWinklerScore = new JaroWinklerDistance().GetDistance(inputA.ToLower(), inputB.ToLower());
            // scorer.
        }
    }
}
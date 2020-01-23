using System.Collections.Generic;
using Dwapi.ExtractsManagement.Core.Model.Destination.Dwh;
using Dwapi.SharedKernel.Model;
using Dwapi.UploadManagement.Core.Model.Dwh;

namespace Dwapi.UploadManagement.Core.Exchange.Dwh
{
    public interface IMessage<T>where T:ClientExtract
    {
        Facility Facility { get; }
        PatientExtractView Demographic { get; }
        List<T> Extracts { get; }
        IMessage<T> Generate(T extract);
        List<IMessage<T>> GenerateMessages(List<T> extracts);
    }
}

using System.Collections.Generic;
using Dwapi.ExtractsManagement.Core.Model.Destination.Dwh;
using Dwapi.SharedKernel.Model;

namespace Dwapi.UploadManagement.Core.Exchange.Dwh
{
    public interface IMessageBag<T> where T:ClientExtract
    {
        string EndPoint { get; }
        IMessage<T> Message { get; set; }
        List<IMessage<T>> Messages { get; set; }
        IMessageBag<T> Generate(List<T> messages);
    }
}

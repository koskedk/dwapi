using System;
using System.Collections.Generic;
using Dwapi.ExtractsManagement.Core.Model.Destination.Dwh;

namespace Dwapi.UploadManagement.Core.Interfaces.Exchange
{
    public interface IMessageBag<T> where T:ClientExtract
    {
        string EndPoint { get; }
        IMessage<T> Message { get; set; }
        List<IMessage<T>> Messages { get; set; }
        List<Guid> SendIds { get; }
        IMessageBag<T> Generate(List<T> extracts);
    }
}

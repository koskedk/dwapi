using System.Collections.Generic;

namespace Dwapi.UploadManagement.Core.Exchange.Dwh
{
    public interface IMessageBag<T>
    {
        string EndPoint { get; }
        T Message { get; set; }
        List<T> Messages { get; set; }
    }
}
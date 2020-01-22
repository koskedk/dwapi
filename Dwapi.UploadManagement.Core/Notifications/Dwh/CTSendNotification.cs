using Dwapi.SharedKernel.Events;
using Dwapi.SharedKernel.Model;

namespace Dwapi.UploadManagement.Core.Notifications.Hts
{
    public class CTSendNotification : IDomainEvent
    {
        public SendProgress Progress { get; set; }

        public CTSendNotification(SendProgress progress)
        {
            Progress = progress;
        }
    }
}
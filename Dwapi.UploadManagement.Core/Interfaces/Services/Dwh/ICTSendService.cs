using System.Collections.Generic;
using System.Threading.Tasks;
using Dwapi.SharedKernel.DTOs;
using Dwapi.SharedKernel.Exchange;

namespace Dwapi.UploadManagement.Core.Interfaces.Services.Dwh
{
    public interface ICTSendService
    {
        Task<List<SendDhwManifestResponse>> SendManifestAsync(SendManifestPackageDTO sendTo);

        Task<List<SendDhwManifestResponse>> SendManifestAsync(SendManifestPackageDTO sendTo, DwhManifestMessageBag messageBag);

        Task<List<string>> SendExtractsAsync(SendManifestPackageDTO sendTo);
    }
}
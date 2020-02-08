using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Dwapi.ExtractsManagement.Core.DTOs;
using Dwapi.ExtractsManagement.Core.Interfaces.Services;
using Dwapi.ExtractsManagement.Core.Model.Destination.Dwh;
using Dwapi.ExtractsManagement.Core.Model.Destination.Hts.NewHts;
using Dwapi.ExtractsManagement.Core.Notifications;
using Dwapi.SettingsManagement.Core.Interfaces.Repositories;
using Dwapi.SettingsManagement.Core.Model;
using Dwapi.SharedKernel.DTOs;
using Dwapi.SharedKernel.Enum;
using Dwapi.SharedKernel.Events;
using Dwapi.SharedKernel.Exchange;
using Dwapi.SharedKernel.Model;
using Dwapi.SharedKernel.Utility;
using Dwapi.UploadManagement.Core.Event.Dwh;
using Dwapi.UploadManagement.Core.Event.Hts;
using Dwapi.UploadManagement.Core.Exchange.Cbs;
using Dwapi.UploadManagement.Core.Exchange.Dwh;
using Dwapi.UploadManagement.Core.Interfaces.Exchange;
using Dwapi.UploadManagement.Core.Interfaces.Packager.Dwh;
using Dwapi.UploadManagement.Core.Interfaces.Reader.Dwh;
using Dwapi.UploadManagement.Core.Interfaces.Services.Dwh;
using Dwapi.UploadManagement.Core.Model.Cbs.Dtos;
using Dwapi.UploadManagement.Core.Model.Dwh;
using Dwapi.UploadManagement.Core.Notifications.Dwh;
using Dwapi.UploadManagement.Core.Notifications.Hts;
using Hangfire;
using Newtonsoft.Json;
using Serilog;

namespace Dwapi.UploadManagement.Core.Services.Dwh
{
    public class CTSendService : ICTSendService
    {
        private readonly string _endPoint;
        private readonly ICTPackager _packager;

        public CTSendService(ICTPackager packager)
        {
            _packager = packager;
            _endPoint = "api/";
        }

        public Task<List<SendDhwManifestResponse>> SendManifestAsync(SendManifestPackageDTO sendTo)
        {
            return SendManifestAsync(sendTo, DwhManifestMessageBag.Create(_packager.GenerateWithMetrics().ToList()));
        }

        public async Task<List<SendDhwManifestResponse>> SendManifestAsync(SendManifestPackageDTO sendTo,
            DwhManifestMessageBag messageBag)
        {
            var responses = new List<SendDhwManifestResponse>();
            HttpClientHandler handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            var client = new HttpClient(handler);
            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
            foreach (var message in messageBag.Messages)
            {
                try
                {
                    var response = await client.PostAsJsonAsync(sendTo.GetUrl($"{_endPoint.HasToEndsWith("/")}spot"),
                        message.Manifest);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        responses.Add(new SendDhwManifestResponse(content));
                    }
                    else
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        throw new Exception(error);
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e, $"Send Manifest Error");
                    throw;
                }
            }

            return responses;
        }

        public async Task<List<SendCTResponse>> SendBatchExtractsAsync<T>(
            SendManifestPackageDTO sendTo,
            int batchSize,
            IMessageBag<T> messageBag)
            where T :ClientExtract
        {
            var client = new HttpClient();
            var responses = new List<SendCTResponse>();
            var packageInfo = _packager.GetPackageInfo<T>(batchSize);
            int sendCound = 0;
            int count = 0;
            int total = packageInfo.PageCount;


            DomainEvents.Dispatch(new CTStatusNotification(sendTo.ExtractId, ExtractStatus.Sending));

            for (int page = 1; page <= packageInfo.PageCount; page++)
            {
                count++;
                var extracts = _packager.GenerateBatchExtracts<T>(page, packageInfo.PageSize).ToList();
                messageBag = messageBag.Generate(extracts);
                var message = messageBag.Messages;
                try
                {
                    /*
                    var msg = JsonConvert.SerializeObject(message,new JsonSerializerSettings {ReferenceLoopHandling = ReferenceLoopHandling.Serializ});
                    */
                    var response = await client.PostAsJsonAsync(
                        sendTo.GetUrl($"{_endPoint.HasToEndsWith("/")}v2/{messageBag.EndPoint}"), message);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsJsonAsync<SendCTResponse>();
                        responses.Add(content);

                        var sentIds = messageBag.SendIds;
                        sendCound += sentIds.Count;
                        DomainEvents.Dispatch(new CTExtractSentEvent(sentIds, SendStatus.Sent,
                            sendTo.ExtractName));
                    }
                    else
                    {
                        var sentIds = messageBag.SendIds;
                        var error = await response.Content.ReadAsStringAsync();
                        DomainEvents.Dispatch(new CTExtractSentEvent(
                            sentIds, SendStatus.Failed, sendTo.ExtractName,
                            error));
                        throw new Exception(error);
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e, $"Send Extracts Error");
                    throw;
                }

                DomainEvents.Dispatch(new CTSendNotification(new SendProgress(nameof(HtsClientTests),
                    Common.GetProgress(count, total))));
            }

            DomainEvents.Dispatch(new CTSendNotification(new SendProgress(nameof(HtsClientTests),
                Common.GetProgress(count, total), true)));

            DomainEvents.Dispatch(new CTStatusNotification(sendTo.ExtractId, ExtractStatus.Sent, sendCound));

            return responses;
        }
    }
}

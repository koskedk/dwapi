﻿using System;
using System.Collections.Generic;
using System.Linq;
using Dwapi.UploadManagement.Core.Interfaces.Exchange;
using Dwapi.UploadManagement.Core.Interfaces.Exchange.Dwh;
using Dwapi.UploadManagement.Core.Model.Dwh;

namespace Dwapi.UploadManagement.Core.Exchange.Dwh
{
    public class VisitsMessageBag:IVisitMessageBag
    {
        public string EndPoint => "PatientVisits";
        public IMessage<PatientVisitExtractView> Message { get; set; }
        public List<IMessage<PatientVisitExtractView>> Messages { get; set; }
        public List<Guid> SendIds => GetIds();
        public VisitsMessageBag()
        {
        }

        public VisitsMessageBag(IMessage<PatientVisitExtractView> message)
        {
            Message = message;
        }

        public VisitsMessageBag(List<IMessage<PatientVisitExtractView>> messages)
        {
            Messages = messages;
        }

        public VisitsMessageBag(VisitsMessage message)
        {
            Message = message;
        }

        public static VisitsMessageBag Create(PatientExtractView patient)
        {
            var message = new VisitsMessage(patient);
            return new VisitsMessageBag(message);
        }


        public IMessageBag<PatientVisitExtractView> Generate(List<PatientVisitExtractView> extracts)
        {
            var messages = new List<IMessage<PatientVisitExtractView>>();
            foreach (var artExtractView in extracts)
            {
                var message = new VisitsMessage(artExtractView);
                messages.Add(message);
            }

            return new VisitsMessageBag(messages);
        }

        private List<Guid> GetIds()
        {
            var ids= Messages.SelectMany(x => x.SendIds).ToList();
            return ids;
        }
    }
}

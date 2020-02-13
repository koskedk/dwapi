﻿using System;
using System.Collections.Generic;
using System.Linq;
using Dwapi.UploadManagement.Core.Interfaces.Exchange;
using Dwapi.UploadManagement.Core.Interfaces.Exchange.Dwh;
using Dwapi.UploadManagement.Core.Model.Dwh;

namespace Dwapi.UploadManagement.Core.Exchange.Dwh
{
    public class LabMessageBag:ILaboratoryMessageBag
    {
        public string EndPoint => "PatientLabs";
        public IMessage<PatientLaboratoryExtractView> Message { get; set; }
        public List<IMessage<PatientLaboratoryExtractView>> Messages { get; set; }
        public List<Guid> SendIds => GetIds();
        public LabMessageBag()
        {
        }

        public LabMessageBag(IMessage<PatientLaboratoryExtractView> message)
        {
            Message = message;
        }

        public LabMessageBag(List<IMessage<PatientLaboratoryExtractView>> messages)
        {
            Messages = messages;
        }

        public LabMessageBag(LabMessage message)
        {
            Message = message;
        }

        public static LabMessageBag Create(PatientExtractView patient)
        {
            var message = new LabMessage(patient);
            return new LabMessageBag(message);
        }


        public IMessageBag<PatientLaboratoryExtractView> Generate(List<PatientLaboratoryExtractView> extracts)
        {
            var messages = new List<IMessage<PatientLaboratoryExtractView>>();
            foreach (var artExtractView in extracts)
            {
                var message = new LabMessage(artExtractView);
                messages.Add(message);
            }

            return new LabMessageBag(messages);
        }

        private List<Guid> GetIds()
        {
            var ids= Messages.SelectMany(x => x.SendIds).ToList();
            return ids;
        }

    }
}

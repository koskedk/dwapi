using System;
using System.Collections.Generic;
using System.Linq;
using Dwapi.UploadManagement.Core.Interfaces.Exchange;
using Dwapi.UploadManagement.Core.Interfaces.Exchange.Dwh;
using Dwapi.UploadManagement.Core.Model.Dwh;
using Newtonsoft.Json;

namespace Dwapi.UploadManagement.Core.Exchange.Dwh
{
    public class AdverseEventsMessage:IAdverseEventsMessage
    {
        public Facility Facility
        {
            get
            {
                var facility = Demographic?.GetFacility();
                if (null != facility)
                    return facility;
                return new Facility();
            }
        }
        public PatientExtractView Demographic { get; set; }
        [JsonProperty(PropertyName = "ArtExtracts")]
        public List<PatientAdverseEventView> Extracts { get; }

        public List<Guid> SendIds { get; }
        public IMessage<PatientAdverseEventView> Generate(PatientAdverseEventView extract)
        {
            throw new NotImplementedException();
        }

        public List<IMessage<PatientAdverseEventView>> GenerateMessages(List<PatientAdverseEventView> extracts)
        {
            throw new NotImplementedException();
        }

        public List<PatientAdverseEventView> AdverseEventExtracts { get; set; } = new List<PatientAdverseEventView>();

        public AdverseEventsMessage()
        {
        }

        public AdverseEventsMessage(PatientExtractView patient)
        {
            Demographic = patient;
            AdverseEventExtracts = patient.PatientAdverseEventExtracts.ToList();
        }
        public bool HasContents => null != AdverseEventExtracts && AdverseEventExtracts.Any();
    }
}

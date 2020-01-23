﻿namespace Dwapi.UploadManagement.Core.Exchange.Dwh
{
    public class SendCTResponse
    {
        public string BatchKey { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(BatchKey);
        }
        public override string ToString()
        {
            return $"{BatchKey}";
        }
    }
}

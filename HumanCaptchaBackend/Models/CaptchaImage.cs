using System;

namespace HumanCaptchaBackend.Models
{
    public class CaptchaImage
    {
        public Guid ID { get; set; }
        public string Value { get; set; }
        public byte[] Data { get; set; }
        public string MimeType { get; set; }
    }
}

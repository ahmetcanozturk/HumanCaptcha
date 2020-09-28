using System;

namespace HumanCaptchaBackend.Data
{
    public class Captcha
    {
        public uint ID { get; set; }
        public Guid Guid { get; set; }
        public string Value { get; set; }
        public DateTime Expires { get; set; }
    }
}

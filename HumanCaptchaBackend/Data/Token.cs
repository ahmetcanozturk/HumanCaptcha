using System;

namespace HumanCaptchaBackend.Data
{
    public class Token
    {
        public Guid ID { get; set; }
        public string Value { get; set; }
        public bool Used { get; set; }
        public DateTime Expires { get; set; }
    }
}

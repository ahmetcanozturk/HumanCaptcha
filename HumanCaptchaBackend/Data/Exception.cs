using System;

namespace HumanCaptchaBackend.Data
{
    public partial class Exception
    {
        public uint ID { get; set; }
        public DateTime ExceptionTime { get; set; }
        public string TypeName { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string InnerStack { get; set; }
    }
}

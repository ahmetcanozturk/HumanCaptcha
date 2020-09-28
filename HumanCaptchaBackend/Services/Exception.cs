using System;

namespace HumanCaptchaBackend.Services
{
    public class Exception
    {
        public DateTime Time { get; set; }
        public string TypeName { get; set; }
        public string Message { get; set; }
        public string InnerStack { get; set; }
        public string StackTrace { get; set; }

        public Exception(System.Exception exc)
        {
            if (exc.InnerException != null)
                exc = exc.InnerException;

            this.TypeName = exc.GetType().ToString();
            this.Message = exc.Message;
            this.InnerStack = exc.InnerException != null ? exc.InnerException.ToString() : null;
            this.StackTrace = exc.StackTrace;
            this.Time = DateTime.Now;
        }
    }
}
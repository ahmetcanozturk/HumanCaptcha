namespace HumanCaptchaBackend.Services
{
    public interface IExceptionManager
    {
        void DoException(System.Exception exc);
        void DoException(Exception exc);
    }
}

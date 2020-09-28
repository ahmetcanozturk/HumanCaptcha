using HumanCaptchaBackend.Data;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Linq;

namespace HumanCaptchaBackend.Services
{
    public class ExceptionManager: IExceptionManager
    {
        private readonly HumanCaptchaContext context;
        private readonly IHostEnvironment hostEnvironment;

        public ExceptionManager(HumanCaptchaContext _context, IHostEnvironment _hostEnvironment)
        {
            this.context = _context;
            this.hostEnvironment = _hostEnvironment;
        }

        public void DoException(System.Exception exc)
        {
            doException(exc);
        }

        public void DoException(Exception exception)
        {
            doException(exception);
        }

        private void doException(System.Exception exc)
        {
            var exceptionHandler = constructChain();
            Exception exception = new Exception(exc);
            exceptionHandler.HandleException(exception);
        }

        private void doException(Exception exception)
        {
            var exceptionHandler = constructChain();
            exceptionHandler.HandleException(exception);
        }

        private ExceptionHandler constructChain()
        {
            // construct chain of responsibility
            ExceptionHandler exceptionHandler = new DatabaseHandler(context, hostEnvironment);
            ExceptionHandler exceptionHandlerTxt = new TextFileHandler(hostEnvironment);

            //in case of database handler could not handle
            exceptionHandler.SetSuccessor(exceptionHandlerTxt);

            return exceptionHandler;
        }
    }

    abstract class ExceptionHandler
    {
        protected ExceptionHandler successor;
        protected readonly IHostEnvironment hostEnvironment;

        public ExceptionHandler(IHostEnvironment hostEnvironment)
        {
            this.hostEnvironment = hostEnvironment;
        }

        public void SetSuccessor(ExceptionHandler successor)
        {
            this.successor = successor;
        }

        public abstract void HandleException(Exception exception);
    }

    class DatabaseHandler : ExceptionHandler
    {
        private readonly HumanCaptchaContext context;
        public DatabaseHandler(HumanCaptchaContext _context, IHostEnvironment _hostEnvironment) :base(_hostEnvironment)
        {
            this.context = _context;
        }

        public override void HandleException(Exception exception)
        {
            bool isHandled = false;
            if (isDataBaseExists())
                isHandled = handleException(exception);

            if (!isHandled && successor != null)
                successor.HandleException(exception);
        }

        private bool handleException(Exception exception)
        {
            try
            {
                var exc = new Data.Exception()
                {
                    ExceptionTime = exception.Time,
                    Message = exception.Message,
                    InnerStack = exception.InnerStack,
                    StackTrace = exception.StackTrace,
                    TypeName = exception.TypeName
                };
                context.Exceptions.Add(exc);
                int iResult = context.SaveChanges();
                return System.Convert.ToBoolean(iResult);
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        private bool isDataBaseExists()
        {
            try
            {
                return context.Database.CanConnect();
            }
            catch (System.Exception)
            {
                return false;
            }
        }
    }

    class TextFileHandler : ExceptionHandler
    {
        public TextFileHandler(IHostEnvironment _hostEnvironment):base(_hostEnvironment)
        {
        }

        public override void HandleException(Exception exception)
        {
            bool isHandled = handleException(exception);
            if (!isHandled && successor != null)
                successor.HandleException(exception);
        }

        private bool handleException(Exception exception)
        {
            string contentRootPath = hostEnvironment.ContentRootPath;
            var path = System.IO.Path.Combine(contentRootPath, "Temp\\Exceptions.txt");

            try
            {
                using (var writer = new System.IO.StreamWriter(path, true))
                {
                    writer.WriteLine("-------------------------");
                    writer.WriteLine("Exception @" + exception.Time.ToString("dd.MM.yyyy - HH:mm:ss"));
                    writer.WriteLine("Type: " + exception.TypeName);
                    writer.WriteLine("Message: " + exception.Message);
                    writer.WriteLine("Stack: " + exception.StackTrace);
                    writer.WriteLine("-------------------------");
                    writer.WriteLine();
                }
                return true;
            }
            catch (System.Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
            return false;
        }
    }
}
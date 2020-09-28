using HumanCaptchaBackend.Data;
using HumanCaptchaBackend.Models;
using HumanCaptchaBackend.Services;
using Microsoft.AspNetCore.Mvc;
namespace HumanCaptchaBackend.Controllers
{
    /// <summary>
    /// controller to test our not robot captcha
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly HumanCaptchaContext context;
        private readonly IExceptionManager exceptionManager;

        public TestController(HumanCaptchaContext _context, IExceptionManager _exceptionManager)
        {
            this.context = _context;
            this.exceptionManager = _exceptionManager;
        }

        [TypeFilter(typeof(TokenAuthenticationAttribute))]
        [HttpPost]
        public SubmitResultItem Post(string id)
        {
            return new SubmitResultItem() { Result = true, Message = "Record saved." };
        }
    }
}

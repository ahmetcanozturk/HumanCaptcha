using Microsoft.AspNetCore.Mvc;
using HumanCaptchaBackend.Business;
using HumanCaptchaBackend.Models;
using System.Threading.Tasks;
using HumanCaptchaBackend.Data;
using HumanCaptchaBackend.Services;
using System;

namespace HumanCaptchaBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CaptchaController : ControllerBase
    {
        private readonly CaptchaModel model;
        //default values if not specified
        private readonly int size;
        public CaptchaController(HumanCaptchaContext _context, IExceptionManager _exceptionManager)
        {
            this.model = new CaptchaModel(_context, _exceptionManager);
            //default length
            this.size = 6;
        }

        [HttpGet("{size?}")]
        public async Task<CaptchaImage> Get(int? size)
        {
            int sz = size.HasValue ? size.Value : this.size;
            return await model.GenerateCaptcha(sz);
        }

        [HttpPost]
        public async Task<ResultItem> Post(ValidateItem item)
        {
            return await model.CheckCaptcha(item.Id, item.Value);
        }
    }
}

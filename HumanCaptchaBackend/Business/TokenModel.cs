using HumanCaptchaBackend.Data;
using HumanCaptchaBackend.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HumanCaptchaBackend.Business
{
    public class TokenModel
    {
        private readonly HumanCaptchaContext context;
        private readonly IExceptionManager exceptionManager;
        private const int tokenExpireInMinutes = 5;

        public TokenModel(HumanCaptchaContext _context, IExceptionManager _exceptionManager)
        {
            this.context = _context;
            this.exceptionManager = _exceptionManager;
        }

        public async Task<string> SaveToken(Guid guid)
        {
            return await saveToken(guid);
        }

        public async Task<bool> IsTokenValid(string token)
        {
            return await isTokenValid(token);
        }

        private async Task<bool> isTokenValid(string token)
        {
            try
            {
                var item = await context.Tokens.Where(t => t.Value.Equals(token) && t.Expires > DateTime.Now && t.Used == false).FirstOrDefaultAsync();
                if (item != null)
                {
                    // update token if valid then return valid
                    item.Used = true;
                    int result = context.SaveChanges();
                    return Convert.ToBoolean(result);
                }
            }
            catch (System.Exception e)
            {
                exceptionManager.DoException(e);
            }
            return false;
        }

        private async Task<string> saveToken(Guid guid)
        {
            try
            {
                var token = Guid.NewGuid().ToString().Replace("-", "");
                var newItem = new Token()
                {
                    ID = guid,
                    Value = token,
                    Used = false,
                    Expires = DateTime.Now.AddMinutes(tokenExpireInMinutes)
                };
                context.Tokens.Add(newItem);
                int saved = await context.SaveChangesAsync();
                return Convert.ToBoolean(saved) ? token : string.Empty;
            }
            catch (System.Exception e)
            {
                exceptionManager.DoException(e);
            }
            return string.Empty;
        }
    }
}

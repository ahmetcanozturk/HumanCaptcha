using System;
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using HumanCaptchaBackend.Data;
using HumanCaptchaBackend.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HumanCaptchaBackend.Services;

namespace HumanCaptchaBackend.Business
{
    public class CaptchaModel
    {
        private readonly HumanCaptchaContext context;
        private readonly IExceptionManager exceptionManager;
        private const int captchaExpireInMinutes = 3;

        public CaptchaModel(HumanCaptchaContext _context, IExceptionManager _exceptionManager)
        {
            this.context = _context;
            this.exceptionManager = _exceptionManager;
        }

        public async Task<CaptchaImage> GenerateCaptcha(int size)
        {
            return await generateCaptcha(size);
        }

        public async Task<ResultItem> CheckCaptcha(Guid imageid, string value)
        {
            return await checkCaptcha(imageid, value);
        }

        private async Task<CaptchaImage> generateCaptcha(int size)
        {
            try
            {
                int width = size * 30;
                int height = width / 4;
                var b = new Bitmap(width, height);
                var g = Graphics.FromImage(b);

                var brush = new HatchBrush(HatchStyle.SmallConfetti, Color.LightSteelBlue, Color.White);
                g.FillRectangle(brush, 0, 0, b.Width, b.Height);
                float emSize = width / size;
                var fnt = new Font("Arial", emSize, FontStyle.Italic);

                string value = generateRandomCaptchaValue(size);
                while (string.IsNullOrEmpty(value))
                    value = generateRandomCaptchaValue(size);

                g.DrawString(value, fnt, Brushes.Coral, 0, 0, StringFormat.GenericTypographic);

                // random noise
                brush = new HatchBrush(HatchStyle.LargeConfetti, Color.SlateBlue, Color.SlateGray);
                fillRandomNoise(g, brush, width, height);

                Point[] iP = getRandomPoints(width, height);
                for (int i = 0; i < 3; i++)
                {
                    Brush brs = Brushes.BlueViolet;
                    if (i % 3 == 1)
                        brs = Brushes.DodgerBlue;
                    if (i % 3 == 2)
                        brs = Brushes.MediumVioletRed;
                    Pen pn = new Pen(brs, 2);
                    g.DrawBezier(pn, iP[i * 4], iP[i * 4 + 1], iP[i * 4 + 2], iP[i * 4 + 3]);
                }

                // create image
                byte[] bBuffer = (byte[])System.ComponentModel.TypeDescriptor.GetConverter(b).ConvertTo(b, typeof(byte[]));

                // add image properties to database
                var guid = await saveCaptcha(value);

                // return image with properties
                if (guid.HasValue)
                    return new CaptchaImage() { ID = guid.Value, Value = value, Data = bBuffer, MimeType = "image/jpg" };
            }
            catch(System.Exception e)
            {
                exceptionManager.DoException(e);
            }        
            return null;
        }

        private async Task<ResultItem> checkCaptcha(Guid imageid, string value)
        {
            try
            {
                var item = await context.Captchas.Where(c => c.Guid.Equals(imageid) && c.Value.Equals(value) && c.Expires >= DateTime.Now).FirstOrDefaultAsync();
                if (item != null)
                {
                    var tokenModel = new TokenModel(this.context, this.exceptionManager);
                    string token = await tokenModel.SaveToken(imageid);
                    if (!string.IsNullOrEmpty(token))
                        return new ResultItem() { Result = true, Token = token };
                }
            }
            catch(System.Exception e)
            {
                exceptionManager.DoException(e);
            }
            return new ResultItem() { Result = false, Token = string.Empty };
        }

        private async Task<Guid?> saveCaptcha(string value)
        {
            try
            {
                var guid = Guid.NewGuid();
                var newItem = new Captcha()
                {
                    Value = value,
                    Guid = guid, 
                    Expires = DateTime.Now.AddMinutes(captchaExpireInMinutes)
                };
                context.Captchas.Add(newItem);
                int saved = await context.SaveChangesAsync();
                return Convert.ToBoolean(saved) ? guid : (Guid?)null;
            }
            catch(System.Exception e)
            {
                exceptionManager.DoException(e);
            }
            return null;
        }



        private string generateRandomCaptchaValue(int size)
        {
            var random = new Random(getSeed());
            var builder = new StringBuilder();
            for (int i = 0; i < size; i++)
            {
                var val = random.Next(0, 2);
                if (val == 0)
                    builder.Append(randomString(1, random));
                else
                    builder.Append(randomNumber(0, 9, random));
            }
            return builder.ToString();
        }

        private void fillRandomNoise(Graphics g, HatchBrush hbrs, int width, int height)
        {
            var random = new Random(getSeed());
            int m = Math.Max(width, height);
            for (int i = 0; i < (int)(width * height / 20F); i++)
            {
                int x = random.Next(width);
                int y = random.Next(height);
                int w = random.Next(m / 40);
                int h = random.Next(m / 40);
                g.FillEllipse(hbrs, x, y, w, h);
            }
        }

        private Point[] getRandomPoints(int width, int height)
        {
            Point[] iP = new Point[12];
            var random = new Random(getSeed());
            int midX = width / 2;
            int midY = height / 2;
            for (int i = 0; i < iP.Length; i++)
            {
                if (i % 2 == 0)
                    iP[i] = new Point(random.Next(5, midX), random.Next(5, midY));
                else
                    iP[i] = new Point(random.Next(midX, width), random.Next(midY, height));
            }
            return iP;
        }

        private string randomString(int iLen, Random random)
        {
            var builder = new StringBuilder();
            char ch;
            for (int i = 0; i < iLen; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            return builder.ToString();
        }

        private int randomNumber(int iMin, int iMax, Random random)
        {
            return random.Next(iMin, iMax + 1);
        }

        private int getSeed()
        {
            DateTime dt = DateTime.Now;
            long elapsedTicks = dt.Ticks;
            long let;
            Math.DivRem(elapsedTicks, (long)(elapsedTicks / 100000), out let);
            int iet = (int)let;
            int mins = dt.Minute;
            int secs = dt.Second;
            int ms = dt.Millisecond;
            return iet + mins + secs + ms + (mins * secs * ms);
        }
    }
}

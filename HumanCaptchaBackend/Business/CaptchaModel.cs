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
    /// <summary>
    /// Human Captcha model
    /// </summary>
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

        /// <summary>
        /// generate captcha image
        /// </summary>
        /// <param name="size">alphanumeric length of the captcha</param>
        /// <returns>image properties and image binary data</returns>
        public async Task<CaptchaImage> GenerateCaptcha(int size)
        {
            return await generateCaptcha(size);
        }

        /// <summary>
        /// verify captcha value
        /// </summary>
        /// <param name="imageid">id of the captcha image</param>
        /// <param name="value">user entered value</param>
        /// <returns>result and the Token</returns>
        public async Task<ResultItem> VerifyCaptcha(Guid imageid, string value)
        {
            return await verifyCaptcha(imageid, value);
        }

        /// <summary>
        /// generate captcha image
        /// </summary>
        /// <param name="size">alphanumeric length of the captcha</param>
        /// <returns>image properties and image binary data</returns>
        private async Task<CaptchaImage> generateCaptcha(int size)
        {
            try
            {
                int width = size * 30;
                int height = width / 4;
                var b = new Bitmap(width, height);
                var g = Graphics.FromImage(b);

                //create brush and rectangle
                var brush = new HatchBrush(HatchStyle.SmallConfetti, Color.LightSteelBlue, Color.White);
                g.FillRectangle(brush, 0, 0, b.Width, b.Height);
                float emSize = width / size;
                var fnt = new Font("Arial", emSize, FontStyle.Italic);

                // generate truly random captcha value
                string value = generateRandomCaptchaValue(size);
                while (string.IsNullOrEmpty(value))
                    value = generateRandomCaptchaValue(size);

                // write the generated value on the image
                g.DrawString(value, fnt, Brushes.Coral, 0, 0, StringFormat.GenericTypographic);

                // generate random noise on the image
                brush = new HatchBrush(HatchStyle.LargeConfetti, Color.SlateBlue, Color.SlateGray);
                fillRandomNoise(g, brush, width, height);

                // draw random lines on the image
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

        /// <summary>
        /// verify captcha value
        /// </summary>
        /// <param name="imageid">id of the captcha image</param>
        /// <param name="value">user entered value</param>
        /// <returns>result and the Token</returns>
        private async Task<ResultItem> verifyCaptcha(Guid imageid, string value)
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

        /// <summary>
        /// save captcha properties to database
        /// </summary>
        /// <param name="value">generated value</param>
        /// <returns>guid of the saved data</returns>
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

        /// <summary>
        /// generate truly random captcha values
        /// </summary>
        /// <param name="size">number of alpha-numeric characters to generate</param>
        /// <returns>random value</returns>
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

        /// <summary>
        /// generate random noise on the image
        /// </summary>
        /// <param name="g">graphics</param>
        /// <param name="hbrs">brush</param>
        /// <param name="width">width of the image</param>
        /// <param name="height">height of the image</param>
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

        /// <summary>
        /// generate random points to draw lines on the image
        /// </summary>
        /// <param name="width">width of the image</param>
        /// <param name="height">geight of the image</param>
        /// <returns></returns>
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

        /// <summary>
        /// generate random string
        /// </summary>
        /// <param name="len">length</param>
        /// <param name="random">random number generator</param>
        /// <returns>random string</returns>
        private string randomString(int len, Random random)
        {
            var builder = new StringBuilder();
            char ch;
            for (int i = 0; i < len; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            return builder.ToString();
        }

        /// <summary>
        /// generate random number
        /// </summary>
        /// <param name="iMin"></param>
        /// <param name="iMax"></param>
        /// <param name="random"></param>
        /// <returns>random number</returns>
        private int randomNumber(int iMin, int iMax, Random random)
        {
            return random.Next(iMin, iMax + 1);
        }

        /// <summary>
        /// generate a seed for random number generator
        /// </summary>
        /// <returns>random number seed</returns>
        private int getSeed()
        {
            var dt = DateTime.Now;
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

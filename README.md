# HumanCaptcha
Human Captcha (Not Robot) reusable React Component with .Net Core WebApi back-end

## Project Structure
1. HumanCaptchaBackend: .Net Core 3.1 WebApi
2. HumanCaptcha: React front-end including component and sample site

### HumanCaptchaBackend
Generates a captcha image using System.Drawing and Drawing.Drawing2D namespace
Validates the captcha against user input value and returns a authorization token to be used in front-end Forms

### HumanCaptcha
- Human Captcha component - HumanCaptcha.jsx - is under ./HumanCaptcha/ClientApp/src/components
- Home.jsx is a sample that uses the Human Captcha component with a simple Form

## Usage
<HumanCaptcha size="alphanumeric length of the captca" onChange={method to handle captcha is verified and token is retrieved} />

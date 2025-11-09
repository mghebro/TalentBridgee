using System.Security.Cryptography;
using System.Net.Mail;
using System.Net;

namespace TalentBridge.Common.SMTP;

public class SMTPService
{
    private const string SenderEmail = "giorgimgebrishvili2008@gmail.com";
    private const string AppPassword = "uypn wtuc xcxt tsej";

    public static async Task SendPasswordResetLinkAsync(string toEmail, string userName, string resetLink)
    {
        var subject = "Reset Your Password - TalentBridge";
        var htmlBody = GeneratePasswordResetEmailTemplate(userName, resetLink);

        using var mail = new MailMessage();
        mail.From = new MailAddress(SenderEmail, "TalentBridge");
        mail.To.Add(toEmail);
        mail.Subject = subject;
        mail.Body = htmlBody;
        mail.IsBodyHtml = true;

        using var smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            EnableSsl = true,
            Credentials = new NetworkCredential(SenderEmail, AppPassword),
        };

        await smtpClient.SendMailAsync(mail);
    }

    private static string GeneratePasswordResetEmailTemplate(string userName, string resetLink)
    {
        return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Reset Your Password</title>
</head>
<body style='margin:0; padding:0; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);'>
    <table role='presentation' width='100%' cellpadding='0' cellspacing='0' border='0' style='padding: 40px 20px; font-family: -apple-system, BlinkMacSystemFont, Segoe UI, Roboto, Helvetica, sans-serif;'>
        <tr>
            <td align='center'>
                <table role='presentation' cellpadding='0' cellspacing='0' border='0' style='max-width: 600px; background-color: #ffffff; border-radius: 16px; box-shadow: 0 20px 60px rgba(0,0,0,0.3); overflow: hidden;'>
                    <!-- Header with Brand -->
                    <tr>
                        <td style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 40px 30px; text-align: center;'>
                            <h1 style='font-size: 32px; font-weight: 700; color: #ffffff; margin: 0; letter-spacing: 1px;'>TalentBridge</h1>
                            <p style='font-size: 14px; color: rgba(255,255,255,0.9); margin: 8px 0 0; letter-spacing: 0.5px;'>Connecting Talent with Opportunity</p>
                        </td>
                    </tr>
                    <!-- Content -->
                    <tr>
                        <td style='padding: 40px 30px;'>
                            <h2 style='font-size: 24px; font-weight: 600; color: #1a202c; margin: 0 0 20px;'>Password Reset Request</h2>
                            <p style='font-size: 16px; color: #4a5568; line-height: 1.6; margin: 0 0 10px;'>
                                Hello <strong style='color: #667eea;'>{userName}</strong>,
                            </p>
                            <p style='font-size: 16px; color: #4a5568; line-height: 1.6; margin: 0 0 30px;'>
                                We received a request to reset your TalentBridge account password. Click the button below to create a new password. This link will expire in <strong>1 hour</strong> for security purposes.
                            </p>
                        </td>
                    </tr>
                    <!-- Reset Button -->
                    <tr>
                        <td align='center' style='padding: 0 30px 40px;'>
                            <table role='presentation' cellpadding='0' cellspacing='0' border='0'>
                                <tr>
                                    <td style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); border-radius: 50px; box-shadow: 0 4px 15px rgba(102, 126, 234, 0.4);'>
                                        <a href='{resetLink}' style='display: inline-block; padding: 16px 48px; color: white; text-decoration: none; font-weight: 600; font-size: 16px; letter-spacing: 0.5px;'>
                                            Reset Password
                                        </a>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <!-- Security Notice -->
                    <tr>
                        <td style='padding: 0 30px 30px;'>
                            <div style='background: linear-gradient(135deg, #fef5e7 0%, #fdebd0 100%); border-left: 4px solid #f39c12; border-radius: 8px; padding: 20px;'>
                                <p style='font-size: 14px; color: #9a6e3a; font-weight: 600; margin: 0 0 8px;'>🔒 Security Notice</p>
                                <p style='font-size: 13px; color: #9a6e3a; line-height: 1.5; margin: 0;'>
                                    This link can only be used once and expires in 1 hour. Never share this link with anyone. If you didn't request this reset, please ignore this email or contact our support team.
                                </p>
                            </div>
                        </td>
                    </tr>
                    <!-- Alternative Link -->
                    <tr>
                        <td style='padding: 0 30px 30px;'>
                            <p style='font-size: 14px; color: #718096; margin: 0 0 12px; font-weight: 500;'>Button not working? Copy and paste this link:</p>
                            <div style='background-color: #f7fafc; padding: 16px; border-radius: 8px; border: 1px solid #e2e8f0; font-family: Courier New, monospace; font-size: 12px; word-break: break-all; color: #4a5568;'>
                                {resetLink}
                            </div>
                        </td>
                    </tr>
                    <!-- Footer -->
                    <tr>
                        <td style='background-color: #f7fafc; padding: 30px; border-top: 1px solid #e2e8f0; text-align: center;'>
                            <p style='font-size: 14px; color: #718096; margin: 0 0 20px; line-height: 1.5;'>If you didn't request this password reset, you can safely ignore this email. Your password will remain unchanged.</p>
                            <p style='font-size: 14px; color: #667eea; font-weight: 600; margin: 0;'>Best regards,<br/>The TalentBridge Team</p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
    }

    public static string GenerateVerificationCode()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[4];
        rng.GetBytes(bytes);
        var randomNumber = BitConverter.ToUInt32(bytes, 0);
        return (randomNumber % 900000 + 100000).ToString();
    }

    public static async Task<string> SendVerificationCodeAsync(string toEmail, string userName)
    {
        string verificationCode = GenerateVerificationCode();
        string htmlContent = GenerateVerificationEmailTemplate(userName, verificationCode);

        using var mail = new MailMessage();
        mail.From = new MailAddress(SenderEmail, "TalentBridge");
        mail.To.Add(toEmail);
        mail.Subject = "Your Verification Code - TalentBridge";
        mail.Body = htmlContent;
        mail.IsBodyHtml = true;

        using var smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            EnableSsl = true,
            Credentials = new NetworkCredential(SenderEmail, AppPassword),
        };

        await smtpClient.SendMailAsync(mail);
        return verificationCode;
    }

    public static string SendVerificationCode(string toEmail, string userName)
    {
        string verificationCode = GenerateVerificationCode();
        string htmlContent = GenerateVerificationEmailTemplate(userName, verificationCode);

        using var mail = new MailMessage();
        mail.From = new MailAddress(SenderEmail, "TalentBridge");
        mail.To.Add(toEmail);
        mail.Subject = "Your Verification Code - TalentBridge";
        mail.Body = htmlContent;
        mail.IsBodyHtml = true;

        using var smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            EnableSsl = true,
            Credentials = new NetworkCredential(SenderEmail, AppPassword),
        };

        smtpClient.Send(mail);
        return verificationCode;
    }

    private static string GenerateVerificationEmailTemplate(string userName, string verificationCode)
    {
        return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8' />
    <meta name='viewport' content='width=device-width, initial-scale=1.0' />
    <title>Your Verification Code</title>
</head>
<body style='margin:0; padding:0; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);'>
    <table role='presentation' width='100%' cellpadding='0' cellspacing='0' border='0' style='padding: 40px 20px; font-family: -apple-system, BlinkMacSystemFont, Segoe UI, Roboto, Helvetica, sans-serif;'>
        <tr>
            <td align='center'>
                <table role='presentation' cellpadding='0' cellspacing='0' border='0' style='max-width: 600px; background-color: #ffffff; border-radius: 16px; box-shadow: 0 20px 60px rgba(0,0,0,0.3); overflow: hidden;'>
                    <!-- Header with Brand -->
                    <tr>
                        <td style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 40px 30px; text-align: center;'>
                            <h1 style='font-size: 32px; font-weight: 700; color: #ffffff; margin: 0; letter-spacing: 1px;'>TalentBridge</h1>
                            <p style='font-size: 14px; color: rgba(255,255,255,0.9); margin: 8px 0 0; letter-spacing: 0.5px;'>Connecting Talent with Opportunity</p>
                        </td>
                    </tr>
                    <!-- Content -->
                    <tr>
                        <td style='padding: 40px 30px 30px;'>
                            <h2 style='font-size: 24px; font-weight: 600; color: #1a202c; margin: 0 0 20px; text-align: center;'>Verify Your Identity</h2>
                            <p style='font-size: 16px; color: #4a5568; line-height: 1.6; margin: 0 0 10px; text-align: center;'>
                                Hello <strong style='color: #667eea;'>{userName}</strong>,
                            </p>
                            <p style='font-size: 16px; color: #4a5568; line-height: 1.6; margin: 0 0 30px; text-align: center;'>
                                Enter the verification code below to continue with your TalentBridge account. This code helps us keep your account secure.
                            </p>
                        </td>
                    </tr>
                    <!-- Code Digits -->
                    <tr>
                        <td align='center' style='padding: 0 30px 40px;'>
                            <table role='presentation' cellpadding='0' cellspacing='10' border='0'>
                                <tr>
                                    <td style='background: linear-gradient(135deg, #f0f4ff 0%, #e9ecff 100%); width: 50px; height: 60px; border-radius: 12px; text-align: center; vertical-align: middle; font-size: 28px; font-weight: 700; color: #667eea; border: 2px solid #667eea; box-shadow: 0 4px 12px rgba(102, 126, 234, 0.2);'>{verificationCode[0]}</td>
                                    <td style='background: linear-gradient(135deg, #f0f4ff 0%, #e9ecff 100%); width: 50px; height: 60px; border-radius: 12px; text-align: center; vertical-align: middle; font-size: 28px; font-weight: 700; color: #667eea; border: 2px solid #667eea; box-shadow: 0 4px 12px rgba(102, 126, 234, 0.2);'>{verificationCode[1]}</td>
                                    <td style='background: linear-gradient(135deg, #f0f4ff 0%, #e9ecff 100%); width: 50px; height: 60px; border-radius: 12px; text-align: center; vertical-align: middle; font-size: 28px; font-weight: 700; color: #667eea; border: 2px solid #667eea; box-shadow: 0 4px 12px rgba(102, 126, 234, 0.2);'>{verificationCode[2]}</td>
                                    <td style='background: linear-gradient(135deg, #f0f4ff 0%, #e9ecff 100%); width: 50px; height: 60px; border-radius: 12px; text-align: center; vertical-align: middle; font-size: 28px; font-weight: 700; color: #667eea; border: 2px solid #667eea; box-shadow: 0 4px 12px rgba(102, 126, 234, 0.2);'>{verificationCode[3]}</td>
                                    <td style='background: linear-gradient(135deg, #f0f4ff 0%, #e9ecff 100%); width: 50px; height: 60px; border-radius: 12px; text-align: center; vertical-align: middle; font-size: 28px; font-weight: 700; color: #667eea; border: 2px solid #667eea; box-shadow: 0 4px 12px rgba(102, 126, 234, 0.2);'>{verificationCode[4]}</td>
                                    <td style='background: linear-gradient(135deg, #f0f4ff 0%, #e9ecff 100%); width: 50px; height: 60px; border-radius: 12px; text-align: center; vertical-align: middle; font-size: 28px; font-weight: 700; color: #667eea; border: 2px solid #667eea; box-shadow: 0 4px 12px rgba(102, 126, 234, 0.2);'>{verificationCode[5]}</td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <!-- Security Info -->
                    <tr>
                        <td style='padding: 0 30px 30px;'>
                            <div style='background: linear-gradient(135deg, #e8f5e9 0%, #c8e6c9 100%); border-left: 4px solid #4caf50; border-radius: 8px; padding: 18px; text-align: center;'>
                                <p style='font-size: 14px; color: #2e7d32; margin: 0; line-height: 1.5;'>
                                    ⏱️ This code expires in <strong>10 minutes</strong><br/>
                                    🔐 Never share this code with anyone
                                </p>
                            </div>
                        </td>
                    </tr>
                    <!-- Footer -->
                    <tr>
                        <td style='background-color: #f7fafc; padding: 30px; border-top: 1px solid #e2e8f0; text-align: center;'>
                            <p style='font-size: 14px; color: #718096; margin: 0 0 20px; line-height: 1.5;'>If you didn't request this verification code, you can safely ignore this email.</p>
                            <p style='font-size: 14px; color: #667eea; font-weight: 600; margin: 0;'>Best regards,<br/>The TalentBridge Team</p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
    }
}
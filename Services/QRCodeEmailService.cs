using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using ITP104_FINAL_PROJECT.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using QRCoder;

namespace ITP104_FINAL_PROJECT.Services
{
    /// <summary>
    /// Service for sending QR codes via email to students
    /// </summary>
    public static class QRCodeEmailService
    {
        // Email configuration (same as OTPService)
        private const string SMTP_HOST = "smtp.gmail.com";
        private const int SMTP_PORT = 587;
        private const string SENDER_EMAIL = "jeysixc.aguilan@gmail.com";
        private const string EMAIL_PASSWORD = "sdsagazlqhgcxvig"; // Gmail App Password
        private const string SCHOOL_NAME = "Student Attendance System";

        /// <summary>
        /// Send QR code to student's email after registration
        /// </summary>
        public static async Task<bool> SendQRCodeEmailAsync(Student student, string qrData)
        {
            try
            {
                if (student == null)
                    throw new ArgumentNullException(nameof(student));

                if (string.IsNullOrWhiteSpace(student.Email))
                    throw new Exception("Student email address is required.");

                if (string.IsNullOrWhiteSpace(qrData))
                    throw new ArgumentNullException(nameof(qrData));

                // Check internet connectivity first
                bool isOnline = await NetworkService.IsInternetAvailableAsync();
                if (!isOnline)
                {
                    await ErrorLoggingService.LogInfoAsync(
                        "QR Email Skipped - Offline Mode",
                        $"Student: {student.StudentNumber} - {student.FullName}\n" +
                        $"Email: {student.Email}\n" +
                        $"Reason: No internet connection available",
                        "qr_email_offline"
                    );
                    return false; // Return false but don't throw exception
                }

                // Generate QR code image
                byte[] qrImageBytes = GenerateQRCodeImage(qrData);

                // Send email with QR code
                await SendEmailAsync(student, qrImageBytes);

                // Log success
                await ErrorLoggingService.LogInfoAsync(
                    "QR Code Email Sent",
                    $"Student: {student.StudentNumber} - {student.FullName}\n" +
                    $"Email: {student.Email}\n" +
                    $"QR Data: {qrData}",
                    "qr_email_success"
                );

                return true;
            }
            catch (Exception ex)
            {
                // Log error but don't fail registration
                await ErrorLoggingService.LogErrorAsync(
                    $"QR Code Email Failed - Student: {student.StudentNumber}, Email: {student.Email}",
                    ex
                );

                return false;
            }
        }

        /// <summary>
        /// Generate QR code as byte array
        /// </summary>
        private static byte[] GenerateQRCodeImage(string qrData)
        {
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrData, QRCodeGenerator.ECCLevel.Q);
                using (QRCode qrCode = new QRCode(qrCodeData))
                {
                    // Generate high-quality QR code (500x500 pixels)
                    using (Bitmap qrBitmap = qrCode.GetGraphic(20, Color.Black, Color.White, true))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            qrBitmap.Save(ms, ImageFormat.Png);
                            return ms.ToArray();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Send email with QR code attachment
        /// </summary>
        private static async Task SendEmailAsync(Student student, byte[] qrImageBytes)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(SCHOOL_NAME, SENDER_EMAIL));
            message.To.Add(new MailboxAddress(student.FullName, student.Email));
            message.Subject = $"Your Student QR Code - {SCHOOL_NAME}";

            // Create email body
            var bodyBuilder = new BodyBuilder();

            // HTML email body
            bodyBuilder.HtmlBody = CreateEmailHtmlBody(student);

            // Plain text fallback
            bodyBuilder.TextBody = CreateEmailTextBody(student);

            // Attach QR code image
            bodyBuilder.Attachments.Add($"QRCode_{student.StudentNumber}.png", qrImageBytes, new ContentType("image", "png"));

            // Also embed QR code inline for display in email
            var image = bodyBuilder.LinkedResources.Add($"QRCode_{student.StudentNumber}_inline.png", qrImageBytes, new ContentType("image", "png"));
            image.ContentId = "qrcode-image";

            message.Body = bodyBuilder.ToMessageBody();

            // Send email
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(SMTP_HOST, SMTP_PORT, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(SENDER_EMAIL, EMAIL_PASSWORD);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }

        /// <summary>
        /// Create HTML email body
        /// </summary>
        private static string CreateEmailHtmlBody(Student student)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 20px; }}
        .container {{ max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
        .header {{ background-color: #2c3e50; color: white; padding: 25px; border-radius: 8px 8px 0 0; text-align: center; }}
        .header h1 {{ margin: 0; font-size: 24px; }}
        .content {{ padding: 20px 0; }}
        .info-box {{ background-color: #f8f9fa; border-left: 4px solid #2c3e50; padding: 15px; margin: 15px 0; }}
        .info-row {{ margin: 8px 0; }}
        .label {{ font-weight: bold; color: #333; }}
        .value {{ color: #555; }}
        .qr-container {{ text-align: center; margin: 25px 0; padding: 20px; background-color: #f8f9fa; border-radius: 8px; }}
        .qr-container img {{ max-width: 300px; border: 2px solid #2c3e50; border-radius: 8px; padding: 10px; background: white; }}
        .instructions {{ background-color: #e8f4f8; border: 1px solid #2c3e50; padding: 15px; border-radius: 5px; margin: 20px 0; }}
        .instructions h3 {{ margin-top: 0; color: #2c3e50; }}
        .instructions ul {{ margin: 10px 0; padding-left: 20px; }}
        .instructions li {{ margin: 5px 0; }}
        .footer {{ text-align: center; margin-top: 30px; padding-top: 20px; border-top: 1px solid #ddd; color: #666; font-size: 12px; }}
        .warning {{ background-color: #fff3cd; border: 1px solid #856404; padding: 10px; border-radius: 5px; margin: 15px 0; color: #856404; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Welcome to {SCHOOL_NAME}</h1>
            <p style='margin: 5px 0 0 0; font-size: 14px;'>Your Registration is Complete!</p>
        </div>
        
        <div class='content'>
            <p>Dear <strong>{student.FullName}</strong>,</p>
            <p>Congratulations! Your student registration has been successfully completed. Below are your details and your unique QR code for attendance tracking.</p>
            
            <div class='info-box'>
                <div class='info-row'><span class='label'>Student ID:</span> <span class='value'>{student.StudentNumber}</span></div>
                <div class='info-row'><span class='label'>Full Name:</span> <span class='value'>{student.FullName}</span></div>
                <div class='info-row'><span class='label'>Program:</span> <span class='value'>{student.Program}</span></div>
                <div class='info-row'><span class='label'>Year Level:</span> <span class='value'>{student.YearLevel}</span></div>
                <div class='info-row'><span class='label'>Email:</span> <span class='value'>{student.Email}</span></div>
                <div class='info-row'><span class='label'>Enrollment Date:</span> <span class='value'>{student.EnrollmentDate:MMMM dd, yyyy}</span></div>
            </div>
            
            <div class='qr-container'>
                <h3 style='margin-top: 0; color: #2c3e50;'>Your Attendance QR Code</h3>
                <img src='cid:qrcode-image' alt='QR Code' />
            </div>
            
            <div class='instructions'>
                <h3>How to Use Your QR Code</h3>
                <ul>
                    <li><strong>Save this QR code</strong> to your phone or print it out</li>
                    <li><strong>Present the QR code</strong> at the attendance scanner when entering campus or classrooms</li>
                    <li><strong>Keep it secure</strong> - do not share your QR code with others</li>
                    <li><strong>Always have it ready</strong> for quick and efficient attendance tracking</li>
                    <li>The QR code is also attached as a separate image file for easy access</li>
                </ul>
            </div>
            
            <div class='warning'>
                <strong>Important:</strong> This QR code is unique to you. Sharing or allowing others to use your QR code is prohibited and may result in disciplinary action.
            </div>
            
            <p>If you lose this QR code or need a replacement, please contact the registrar's office immediately.</p>
            
            <p style='margin-top: 25px;'>Welcome aboard, and best wishes for your academic journey!</p>
        </div>
        
        <div class='footer'>
            <p><strong>{SCHOOL_NAME}</strong></p>
            <p>This is an automated message. Please do not reply to this email.</p>
            <p>Generated on {DateTime.Now:MMMM dd, yyyy 'at' hh:mm tt}</p>
        </div>
    </div>
</body>
</html>";
        }

        /// <summary>
        /// Create plain text email body (fallback)
        /// </summary>
        private static string CreateEmailTextBody(Student student)
        {
            return $@"
{SCHOOL_NAME}
STUDENT REGISTRATION CONFIRMATION

Dear {student.FullName},

Your student registration has been successfully completed!

STUDENT INFORMATION:
====================
Student ID: {student.StudentNumber}
Full Name: {student.FullName}
Program: {student.Program}
Year Level: {student.YearLevel}
Email: {student.Email}
Enrollment Date: {student.EnrollmentDate:MMMM dd, yyyy}

YOUR QR CODE:
=============
Your unique attendance QR code is attached to this email as an image file.

HOW TO USE YOUR QR CODE:
- Save the QR code image to your phone or print it
- Present it at the attendance scanner when required
- Keep it secure and do not share with others
- Always have it ready for attendance tracking

IMPORTANT:
This QR code is unique to you. Sharing or allowing others to use your QR code 
is prohibited and may result in disciplinary action.

If you lose this QR code or need assistance, please contact the registrar's office.

Welcome aboard, and best wishes for your academic journey!

---
{SCHOOL_NAME}
This is an automated message. Please do not reply to this email.
Generated on {DateTime.Now:MMMM dd, yyyy 'at' hh:mm tt}
";
        }
    }
}

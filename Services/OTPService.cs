using ITP104_FINAL_PROJECT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace ITP104_FINAL_PROJECT.Services
{
    public class OTPService
    {
        // In-memory OTP sessions (for production, consider using Redis or database)
        private static Dictionary<string, OTPSession> activeSessions = new Dictionary<string, OTPSession>();

        // Email configuration
        private const string SMTP_HOST = "smtp.gmail.com";
        private const int SMTP_PORT = 587; // TLS port
        private const string SENDER_EMAIL = "jeysixc.aguilan@gmail.com";
        private const string EMAIL_PASSWORD = "sdsagazlqhgcxvig"; // Gmail App Password



        // OTP settings
        private const int OTP_LENGTH = 6;
        private const int OTP_EXPIRY_MINUTES = 5;
        private static Random random = new Random();

        /// <summary>
        /// Initiates attendance verification by generating and sending OTP
        /// </summary>
        public static async Task<OTPSession> InitiateAttendanceAsync(Student student, AttendanceType type, string qrData)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student));

            if (string.IsNullOrWhiteSpace(student.Email))
                throw new Exception("Student email address is not registered.");

            // Generate 6-digit OTP
            string otp = GenerateOTP();

            // Create session with 5-minute expiry
            var session = new OTPSession
            {
                SessionId = Guid.NewGuid().ToString(),
                StudentId = student.StudentId.ToString(),
                StudentNumber = student.StudentNumber,
                StudentName = student.FullName,
                Email = student.Email,
                OTP = otp,
                AttendanceType = type,
                QRData = qrData,
                CreatedAt = DateTime.Now,
                ExpiresAt = DateTime.Now.AddMinutes(OTP_EXPIRY_MINUTES),
                IsUsed = false,
                IsVerified = false
            };

            // Store session
            CleanupExpiredSessions();
            activeSessions[session.SessionId] = session;

            // Send OTP via email
            await SendOTPEmailAsync(session);

            return session;
        }

        /// <summary>
        /// Verifies the OTP entered by the student
        /// </summary>
        public static (bool Success, string Message, OTPSession Session) VerifyOTP(string sessionId, string enteredOTP)
        {
            // Get session
            if (!activeSessions.ContainsKey(sessionId))
            {
                return (false, "Invalid or expired session. Please scan QR code again.", null);
            }

            var session = activeSessions[sessionId];

            // Check if expired
            if (DateTime.Now > session.ExpiresAt)
            {
                activeSessions.Remove(sessionId);
                return (false, "OTP expired. Please scan QR code again.", null);
            }

            // Check if already used
            if (session.IsUsed)
            {
                return (false, "OTP already used. Please scan QR code again.", null);
            }

            // Verify OTP (case-insensitive)
            if (!session.OTP.Equals(enteredOTP?.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                session.FailedAttempts++;

                // Lock session after 3 failed attempts
                if (session.FailedAttempts >= 3)
                {
                    activeSessions.Remove(sessionId);
                    return (false, "Too many failed attempts. Please scan QR code again.", null);
                }

                return (false, $"Invalid OTP. {3 - session.FailedAttempts} attempts remaining.", null);
            }

            // Mark session as verified and used
            session.IsVerified = true;
            session.IsUsed = true;
            session.VerifiedAt = DateTime.Now;

            return (true, "OTP verified successfully", session);
        }

        /// <summary>
        /// Resends OTP to the student's email
        /// </summary>
        public static async Task<(bool Success, string Message)> ResendOTPAsync(string sessionId)
        {
            if (!activeSessions.ContainsKey(sessionId))
            {
                return (false, "Session not found. Please scan QR code again.");
            }

            var session = activeSessions[sessionId];

            // Check if expired
            if (DateTime.Now > session.ExpiresAt)
            {
                activeSessions.Remove(sessionId);
                return (false, "Session expired. Please scan QR code again.");
            }

            // Generate new OTP
            string newOTP = GenerateOTP();
            session.OTP = newOTP;
            session.ResendCount++;

            // Limit resend attempts
            if (session.ResendCount > 3)
            {
                activeSessions.Remove(sessionId);
                return (false, "Too many resend attempts. Please scan QR code again.");
            }

            // Send new OTP
            try
            {
                await SendOTPEmailAsync(session);
                return (true, "OTP resent successfully. Please check your email.");
            }
            catch (Exception ex)
            {
                return (false, $"Failed to resend OTP: {ex.Message}");
            }
        }

        /// <summary>
        /// Removes a session from active sessions
        /// </summary>
        public static void RemoveSession(string sessionId)
        {
            if (activeSessions.ContainsKey(sessionId))
            {
                activeSessions.Remove(sessionId);
            }
        }

        /// <summary>
        /// Sends OTP email to the student
        /// </summary>
        private static async Task SendOTPEmailAsync(OTPSession session)
        {
            try
            {
                // Log connection attempt (for debugging)
                System.Diagnostics.Debug.WriteLine($"[OTP] Attempting SMTP connection with MailKit...");
                System.Diagnostics.Debug.WriteLine($"[OTP] Email: {SENDER_EMAIL}");
                System.Diagnostics.Debug.WriteLine($"[OTP] Password Length: {EMAIL_PASSWORD.Length}");
                System.Diagnostics.Debug.WriteLine($"[OTP] Target: {session.Email}");

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Student Attendance System", SENDER_EMAIL));
                message.To.Add(new MailboxAddress("", session.Email));
                message.Subject = $"Attendance {session.AttendanceType} - OTP Verification";

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = GenerateEmailBody(session)
                };
                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    // Connect to SMTP server
                    System.Diagnostics.Debug.WriteLine($"[OTP] Connecting to {SMTP_HOST}:{SMTP_PORT}...");
                    await client.ConnectAsync(SMTP_HOST, SMTP_PORT, SecureSocketOptions.StartTls);

                    // Authenticate
                    System.Diagnostics.Debug.WriteLine($"[OTP] Authenticating...");
                    await client.AuthenticateAsync(SENDER_EMAIL, EMAIL_PASSWORD);

                    // Send email
                    System.Diagnostics.Debug.WriteLine($"[OTP] Sending email...");
                    await client.SendAsync(message);

                    // Disconnect
                    await client.DisconnectAsync(true);

                    System.Diagnostics.Debug.WriteLine($"[OTP] Email sent successfully!");
                }
            }
            catch (MailKit.Security.AuthenticationException authEx)
            {
                string detailedError = "Failed to send OTP email:\n\n";
                detailedError += "Authentication Error - Email provider rejected the login.\n\n" +
                               "Please verify:\n" +
                               "1. Email address is correct: " + SENDER_EMAIL + "\n" +
                               "2. Password/App Password is correct\n" +
                               "3. Account is not locked or suspended\n\n" +
                               "Error details: " + authEx.Message;

                System.Diagnostics.Debug.WriteLine($"[OTP] Authentication error: {authEx.Message}");
                throw new Exception(detailedError);
            }
            catch (Exception ex)
            {
                string detailedError = "Failed to send OTP email:\n\n";
                detailedError += $"Error: {ex.Message}\n\n";
                detailedError += "Please ensure:\n" +
                               "1. Internet connection is active\n" +
                               "2. Email credentials are correct\n" +
                               "3. Student has a valid email address\n\n" +
                               $"Technical details: {ex.GetType().Name}";

                System.Diagnostics.Debug.WriteLine($"[OTP] Error: {ex.Message}");
                throw new Exception(detailedError);
            }
        }
        /// <summary>
        /// Generates the HTML email body for OTP
        /// </summary>
        private static string GenerateEmailBody(OTPSession session)
        {
            string actionText = session.AttendanceType == AttendanceType.TimeIn ? "Time In" : "Time Out";
            string actionColor = session.AttendanceType == AttendanceType.TimeIn ? "#4CAF50" : "#FF9800";

            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background-color: #f4f4f4;
            margin: 0;
            padding: 0;
        }}
        .container {{
            max-width: 600px;
            margin: 20px auto;
            background-color: #ffffff;
            border-radius: 8px;
            overflow: hidden;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }}
        .header {{
            background-color: {actionColor};
            color: white;
            padding: 30px;
            text-align: center;
        }}
        .header h1 {{
            margin: 0;
            font-size: 24px;
        }}
        .content {{
            padding: 40px 30px;
        }}
        .otp-box {{
            background-color: #f8f9fa;
            border: 2px dashed {actionColor};
            border-radius: 8px;
            padding: 30px;
            text-align: center;
            margin: 20px 0;
        }}
        .otp-code {{
            font-size: 48px;
            font-weight: bold;
            color: {actionColor};
            letter-spacing: 8px;
            font-family: 'Courier New', monospace;
        }}
        .info-box {{
            background-color: #fff3cd;
            border-left: 4px solid #ffc107;
            padding: 15px;
            margin: 20px 0;
        }}
        .student-info {{
            background-color: #e7f3ff;
            border-radius: 6px;
            padding: 15px;
            margin: 20px 0;
        }}
        .student-info p {{
            margin: 5px 0;
            color: #333;
        }}
        .footer {{
            background-color: #f8f9fa;
            padding: 20px;
            text-align: center;
            color: #666;
            font-size: 12px;
            border-top: 1px solid #dee2e6;
        }}
        .warning {{
            color: #d32f2f;
            font-weight: bold;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🎓 Student Attendance System</h1>
            <p style='margin: 10px 0 0 0; font-size: 16px;'>{actionText} Verification</p>
        </div>
        
        <div class='content'>
            <h2 style='color: #333; margin-top: 0;'>Hello, {session.StudentName}!</h2>
            
            <p>Your One-Time Password (OTP) for <strong>{actionText}</strong> verification:</p>
            
            <div class='otp-box'>
                <div class='otp-code'>{session.OTP}</div>
                <p style='margin: 15px 0 0 0; color: #666;'>Valid for {OTP_EXPIRY_MINUTES} minutes</p>
            </div>
            
            <div class='student-info'>
                <p><strong>Student Number:</strong> {session.StudentNumber}</p>
                <p><strong>Action:</strong> {actionText}</p>
                <p><strong>Date & Time:</strong> {session.CreatedAt:MMM dd, yyyy hh:mm tt}</p>
                <p><strong>Expires At:</strong> {session.ExpiresAt:hh:mm tt}</p>
            </div>
            
            <div class='info-box'>
                <p style='margin: 0; color: #856404;'>
                    <strong>⚠️ Security Notice:</strong><br>
                    This OTP is for your attendance verification only. 
                    Do not share this code with anyone.
                </p>
            </div>
            
            <p class='warning'>⏱️ This code will expire in {OTP_EXPIRY_MINUTES} minutes.</p>
            
            <p style='color: #666; font-size: 14px; margin-top: 20px;'>
                If you did not request this verification, please contact your administrator immediately.
            </p>
        </div>
        
        <div class='footer'>
            <p>This is an automated email from the Student Attendance System.</p>
            <p>Please do not reply to this email.</p>
            <p style='margin-top: 10px;'>© {DateTime.Now.Year} Student Attendance System. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
        }

        /// <summary>
        /// Generates a random OTP
        /// </summary>
        private static string GenerateOTP()
        {
            return random.Next(100000, 999999).ToString();
        }

        /// <summary>
        /// Cleans up expired sessions
        /// </summary>
        private static void CleanupExpiredSessions()
        {
            var expiredKeys = activeSessions
                .Where(kvp => DateTime.Now > kvp.Value.ExpiresAt || kvp.Value.IsUsed)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in expiredKeys)
            {
                activeSessions.Remove(key);
            }
        }

        /// <summary>
        /// Gets session count for monitoring
        /// </summary>
        public static int GetActiveSessionCount()
        {
            CleanupExpiredSessions();
            return activeSessions.Count;
        }

        /// <summary>
        /// Initiates email change verification - Step 1: Verify OLD email
        /// </summary>
        public static async Task<OTPSession> InitiateEmailChangeVerifyOldAsync(Student student, string newEmail)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student));

            if (string.IsNullOrWhiteSpace(student.Email))
                throw new Exception("Student current email address is not registered.");

            if (string.IsNullOrWhiteSpace(newEmail))
                throw new ArgumentException("New email address is required.");

            // Generate 6-digit OTP
            string otp = GenerateOTP();

            // Create session for OLD email verification
            var session = new OTPSession
            {
                SessionId = Guid.NewGuid().ToString(),
                StudentId = student.StudentId.ToString(),
                StudentNumber = student.StudentNumber,
                StudentName = student.FullName,
                Email = student.Email, // OLD email
                OTP = otp,
                AttendanceType = AttendanceType.EmailChange, // New type
                QRData = newEmail, // Store new email in QRData field temporarily
                CreatedAt = DateTime.Now,
                ExpiresAt = DateTime.Now.AddMinutes(OTP_EXPIRY_MINUTES),
                IsUsed = false,
                IsVerified = false
            };

            // Store session
            CleanupExpiredSessions();
            activeSessions[session.SessionId] = session;

            // Send OTP to OLD email
            await SendEmailChangeOTPAsync(session, isOldEmail: true);

            return session;
        }

        /// <summary>
        /// Initiates email change verification - Step 2: Verify NEW email
        /// </summary>
        public static async Task<OTPSession> InitiateEmailChangeVerifyNewAsync(OTPSession oldEmailSession)
        {
            if (oldEmailSession == null || !oldEmailSession.IsVerified)
                throw new Exception("Old email must be verified first.");

            string newEmail = oldEmailSession.QRData; // New email stored in QRData

            // Generate new OTP for new email
            string otp = GenerateOTP();

            // Create session for NEW email verification
            var session = new OTPSession
            {
                SessionId = Guid.NewGuid().ToString(),
                StudentId = oldEmailSession.StudentId,
                StudentNumber = oldEmailSession.StudentNumber,
                StudentName = oldEmailSession.StudentName,
                Email = newEmail, // NEW email
                OTP = otp,
                AttendanceType = AttendanceType.EmailChange,
                QRData = oldEmailSession.Email, // Store old email for reference
                CreatedAt = DateTime.Now,
                ExpiresAt = DateTime.Now.AddMinutes(OTP_EXPIRY_MINUTES),
                IsUsed = false,
                IsVerified = false
            };

            // Store session
            activeSessions[session.SessionId] = session;

            // Send OTP to NEW email
            await SendEmailChangeOTPAsync(session, isOldEmail: false);

            return session;
        }

        /// <summary>
        /// Sends OTP email for email change verification
        /// </summary>
        private static async Task SendEmailChangeOTPAsync(OTPSession session, bool isOldEmail)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Student Attendance System", SENDER_EMAIL));
                message.To.Add(new MailboxAddress(session.StudentName, session.Email));
                message.Subject = isOldEmail ? "🔐 Verify Current Email - Email Change Request" : "✅ Verify New Email - Email Change Request";

                string stepNumber = isOldEmail ? "1" : "2";
                string stepDescription = isOldEmail ? "Verify Current Email" : "Verify New Email";
                string emailType = isOldEmail ? "current" : "new";

                message.Body = new TextPart("html")
                {
                    Text = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
</head>
<body style='margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f4f4f4;'>
    <div style='max-width: 600px; margin: 20px auto; background: white; border-radius: 10px; overflow: hidden; box-shadow: 0 2px 10px rgba(0,0,0,0.1);'>
        <div style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; text-align: center;'>
            <h1 style='margin: 0; font-size: 28px;'>🔐 Email Change Verification</h1>
            <p style='margin: 10px 0 0 0; font-size: 16px; opacity: 0.9;'>Step {stepNumber} of 2: {stepDescription}</p>
        </div>
        
        <div style='padding: 40px 30px;'>
            <p style='font-size: 16px; color: #333; margin-bottom: 20px;'>
                Hello <strong>{session.StudentName}</strong>,
            </p>
            
            <p style='font-size: 15px; color: #555; line-height: 1.6;'>
                {(isOldEmail ?
                    "You have requested to change your email address. To proceed, please verify your CURRENT email by entering the code below:" :
                    "Almost done! Please verify your NEW email address by entering the code below:")}
            </p>
            
            <div style='background: #f8f9fa; border-left: 4px solid #667eea; padding: 20px; margin: 25px 0; border-radius: 5px;'>
                <p style='margin: 0 0 10px 0; color: #666; font-size: 14px;'>Your Verification Code:</p>
                <div style='font-size: 36px; font-weight: bold; color: #667eea; letter-spacing: 8px; font-family: monospace;'>
                    {session.OTP}
                </div>
            </div>
            
            <div style='background: #fff3cd; border: 1px solid #ffeaa7; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                <p style='margin: 0 0 10px 0; color: #856404; font-size: 14px;'><strong>📧 Email Change Details:</strong></p>
                <p style='margin: 5px 0; font-size: 14px; color: #856404;'><strong>Student:</strong> {session.StudentName}</p>
                <p style='margin: 5px 0; font-size: 14px; color: #856404;'><strong>Student Number:</strong> {session.StudentNumber}</p>
                <p style='margin: 5px 0; font-size: 14px; color: #856404;'><strong>Verifying:</strong> {emailType.ToUpper()} email</p>
                <p style='margin: 5px 0; font-size: 14px; color: #856404;'><strong>Expires At:</strong> {session.ExpiresAt:hh:mm tt}</p>
            </div>
            
            <div style='background: #d1ecf1; border: 1px solid #bee5eb; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                <p style='margin: 0; color: #0c5460; font-size: 14px;'>
                    <strong>🔒 Security Notice:</strong><br>
                    This code is for email change verification only. 
                    Do not share this code with anyone. If you did not request this change, 
                    please contact your administrator immediately.
                </p>
            </div>
            
            <p style='background: #fff3cd; border: 1px solid #ffc107; color: #856404; padding: 12px; border-radius: 5px; font-size: 14px; margin: 20px 0;'>
                ⏱️ This code will expire in {OTP_EXPIRY_MINUTES} minutes.
            </p>
        </div>
        
        <div style='background: #f8f9fa; padding: 20px; text-align: center; border-top: 1px solid #dee2e6;'>
            <p style='margin: 5px 0; color: #6c757d; font-size: 13px;'>This is an automated email from the Student Attendance System.</p>
            <p style='margin: 5px 0; color: #6c757d; font-size: 13px;'>Please do not reply to this email.</p>
            <p style='margin: 10px 0 0 0; color: #6c757d; font-size: 12px;'>© {DateTime.Now.Year} Student Attendance System. All rights reserved.</p>
        </div>
    </div>
</body>
</html>"
                };

                Console.WriteLine($"[OTP-Email Change] Sending OTP to {(isOldEmail ? "OLD" : "NEW")} email: {session.Email}");

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(SMTP_HOST, SMTP_PORT, SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(SENDER_EMAIL, EMAIL_PASSWORD);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                Console.WriteLine($"[OTP-Email Change] Email sent successfully to {session.Email}!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[OTP-Email Change] Error sending email: {ex.Message}");
                throw new Exception($"Failed to send verification email: {ex.Message}");
            }
        }
    }
}

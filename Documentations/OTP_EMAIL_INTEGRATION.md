# Email OTP Verification for Student Attendance

## Overview

This implementation adds **Two-Factor Authentication (2FA)** using **Email OTP (One-Time Password)** for student attendance verification. Every time a student scans their QR code for Time In or Time Out, they must verify their identity by entering a 6-digit OTP sent to their registered email address.

## Features

✅ **Email-based OTP verification** for both Time In and Time Out
✅ **6-digit random OTP** with 5-minute expiration
✅ **Professional HTML email templates** with styling
✅ **Real-time countdown timer** in verification dialog
✅ **OTP resend functionality** (max 3 resend attempts)
✅ **Failed attempt tracking** (max 3 verification attempts)
✅ **Session management** with automatic cleanup
✅ **Secure email delivery** via Gmail SMTP

## How It Works

### Attendance Flow

1. **Student scans QR code** → System identifies student
2. **System determines attendance type** → Time In or Time Out based on previous scan
3. **OTP generation** → 6-digit random code generated
4. **Email sent** → OTP delivered to student's registered email
5. **Verification dialog shown** → Student enters OTP code
6. **OTP verification** → System validates the entered code
7. **Attendance recorded** → If OTP is correct, attendance is marked in database

### Security Features

- **Time-limited OTP**: Expires after 5 minutes
- **One-time use**: Each OTP can only be used once
- **Attempt limiting**: Max 3 failed verification attempts
- **Resend limiting**: Max 3 resend requests per session
- **Session cleanup**: Expired sessions automatically removed
- **Secure SMTP**: SSL/TLS encrypted email delivery

## Email Configuration

### Gmail App Password Setup

The system uses Gmail SMTP to send OTP emails. You need to create an **App Password** (not your regular Gmail password).

**Current Configuration:**

- Email: `aarhonsaludar.official@gmail.com`
- App Password: `ktnv otai xpdf vvni`
- SMTP Server: `smtp.gmail.com:587`

### How to Create Gmail App Password

1. Go to your Google Account: https://myaccount.google.com/
2. Select **Security** from the left menu
3. Under "Signing in to Google", enable **2-Step Verification** (if not already enabled)
4. After enabling 2FA, go back to Security settings
5. Under "Signing in to Google", click **App passwords**
6. Select app: **Mail**
7. Select device: **Windows Computer**
8. Click **Generate**
9. Copy the 16-character password (format: `xxxx xxxx xxxx xxxx`)
10. Use this password in `OTPService.cs`

### Update Email Settings

Edit `Services/OTPService.cs`:

```csharp
// Email configuration
private const string SMTP_HOST = "smtp.gmail.com";
private const int SMTP_PORT = 587;
private const string SENDER_EMAIL = "your-email@gmail.com"; // Change this
private const string APP_PASSWORD = "your-app-password-here"; // Change this
```

## Student Email Requirement

**IMPORTANT**: Students MUST have a valid email address registered in the system.

### Check Student Email

```sql
SELECT student_number, first_name, last_name, email
FROM students
WHERE email IS NULL OR email = '';
```

### Update Student Email

```sql
UPDATE students
SET email = 'student@example.com'
WHERE student_number = '2021-12345';
```

## Files Added/Modified

### New Files Created

1. **Services/OTPService.cs**

   - OTP generation and validation
   - Email sending functionality
   - Session management
   - Cleanup of expired sessions

2. **OTPVerificationDialog.cs**

   - OTP input dialog form
   - Countdown timer display
   - Verify and resend buttons
   - Error handling and UI feedback

3. **OTPVerificationDialog.Designer.cs**

   - Windows Forms designer code
   - UI controls and layout

4. **OTPVerificationDialog.resx**
   - Form resources

### Modified Files

1. **QRScannerForm.cs**

   - Added OTP verification step before recording attendance
   - Integrated OTPVerificationDialog
   - Added `DetermineAttendanceTypeAsync()` method
   - Added `Services` namespace import

2. **Data/ScanHistoryRepository.cs**
   - Added `HasActiveTodayTimeInAsync()` method to check if student has active Time In

## Database Requirements

Students table must have an `email` column:

```sql
ALTER TABLE students
ADD COLUMN email VARCHAR(100) AFTER phone;
```

## Testing the Integration

### Test Scenario 1: Successful Time In

1. Register a student with valid email
2. Scan student QR code
3. Check your email for OTP
4. Enter the 6-digit code in the dialog
5. Click "Verify"
6. Attendance should be recorded

### Test Scenario 2: OTP Expiration

1. Scan QR code
2. Wait for 5 minutes without entering OTP
3. Timer should reach 0:00
4. Verify button should be disabled
5. Must scan QR code again

### Test Scenario 3: Resend OTP

1. Scan QR code
2. Click "Resend OTP" button
3. New OTP should be sent to email
4. Timer should reset to 5:00
5. Enter new OTP to verify

### Test Scenario 4: Failed Attempts

1. Scan QR code
2. Enter wrong OTP 3 times
3. Session should be locked
4. Must scan QR code again

## Troubleshooting

### Email Not Sending

**Issue**: OTP email fails to send

**Solutions**:

1. Check Gmail App Password is correct
2. Verify 2FA is enabled on Gmail account
3. Check internet connection
4. Check Gmail SMTP is not blocked by firewall
5. Verify sender email is correct

### Student Has No Email

**Issue**: Error "Student email address is not registered"

**Solution**:

```sql
UPDATE students
SET email = 'student@example.com'
WHERE student_id = [ID];
```

### OTP Dialog Not Showing

**Issue**: OTP verification dialog doesn't appear

**Solutions**:

1. Check if `OTPVerificationDialog.cs` is compiled
2. Verify `using ITP104_FINAL_PROJECT.Services;` is added
3. Check for exceptions in error logs
4. Ensure student exists and has email

### Timer Not Counting Down

**Issue**: Countdown timer shows 5:00 but doesn't decrease

**Solution**:

- Timer component should be enabled in Designer
- Check `timerCountdown.Start()` is called in `InitializeDialog()`
- Verify `timerCountdown_Tick` event is wired up

## Customization

### Change OTP Expiry Time

Edit `Services/OTPService.cs`:

```csharp
private const int OTP_EXPIRY_MINUTES = 5; // Change to desired minutes
```

### Change OTP Length

Edit `Services/OTPService.cs`:

```csharp
private const int OTP_LENGTH = 6; // Change to 4, 6, or 8
```

And update the `GenerateOTP()` method:

```csharp
private static string GenerateOTP()
{
    // For 4 digits: 1000 to 9999
    // For 6 digits: 100000 to 999999
    // For 8 digits: 10000000 to 99999999
    return random.Next(100000, 999999).ToString();
}
```

### Customize Email Template

Edit the `GenerateEmailBody()` method in `Services/OTPService.cs` to modify:

- Colors
- Layout
- Logo/branding
- Text content
- Styling

### Change Email Provider

To use a different email provider (e.g., Outlook, SendGrid):

Edit `Services/OTPService.cs`:

```csharp
// For Outlook
private const string SMTP_HOST = "smtp-mail.outlook.com";
private const int SMTP_PORT = 587;

// For SendGrid
private const string SMTP_HOST = "smtp.sendgrid.net";
private const int SMTP_PORT = 587;
```

## Production Deployment

### Recommendations

1. **Store credentials securely**

   - Use environment variables or configuration files
   - Don't hardcode passwords in source code
   - Use Azure Key Vault or similar for sensitive data

2. **Use professional email service**

   - Consider SendGrid, AWS SES, or Mailgun for production
   - Better deliverability and tracking
   - Higher sending limits

3. **Implement rate limiting**

   - Limit OTP requests per student per day
   - Prevent abuse and spam

4. **Add logging**

   - Log all OTP send attempts
   - Track failed verifications
   - Monitor for suspicious activity

5. **Database session storage**
   - Current implementation uses in-memory storage
   - For production, store sessions in database or Redis
   - Enables distributed systems and server restarts

## Benefits of Email OTP

✅ **Prevents buddy punching** - Students can't mark attendance for friends
✅ **Easy to implement** - No special hardware required
✅ **Familiar to users** - Everyone knows how to check email
✅ **No app installation** - Works with any email client
✅ **Secure** - Time-limited, one-time use codes
✅ **Audit trail** - Email records provide verification proof
✅ **Cost-effective** - Gmail allows free sending (500 emails/day)

## Alternative Options (Not Implemented)

If email OTP doesn't work for your use case, consider:

1. **SMS OTP** - For students without email (requires SMS gateway)
2. **Daily PIN** - Pre-generated codes printed weekly
3. **Classroom QR** - Teacher displays rotating QR code
4. **Mobile App** - Custom attendance app with push notifications

## Support

For issues or questions:

- Check error logs in the application
- Verify student email addresses are valid
- Test email sending with a known working address
- Check Gmail security settings and app password

## Version History

- **v1.0** - Initial OTP implementation with email verification
- Features: 6-digit OTP, 5-minute expiry, resend functionality, countdown timer

---

**Implementation Date**: December 1, 2025
**Author**: GitHub Copilot
**Status**: ✅ Ready for Testing

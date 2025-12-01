# Quick Setup Guide - Email OTP Verification

## ⚡ Quick Start (5 Minutes)

### Step 1: Verify Gmail Configuration ✅

The OTP service is already configured with:

- **Email**: `aarhonsaludar.official@gmail.com`
- **App Password**: `ktnv otai xpdf vvni`

**No changes needed!** This should work immediately.

### Step 2: Ensure Students Have Email Addresses 📧

Run this SQL query to check which students need emails:

```sql
-- Check students without email
SELECT student_id, student_number, first_name, last_name, email
FROM students
WHERE email IS NULL OR email = '' OR email NOT LIKE '%@%';
```

Update students who need email addresses:

```sql
-- Add email to a student
UPDATE students
SET email = 'student@example.com'
WHERE student_number = '2021-12345';
```

### Step 3: Build and Run 🚀

Your project is ready! Just build and run:

1. Open Visual Studio
2. Build Solution (Ctrl + Shift + B)
3. Run the application (F5)

### Step 4: Test the OTP Flow 🧪

1. **Scan a student QR code** in the QR Scanner
2. **Check the student's email** for OTP (arrives in ~5 seconds)
3. **Enter the 6-digit code** in the dialog that appears
4. **Click Verify** → Attendance is recorded!

---

## 📋 What Was Added

### New Files

- ✅ `Services/OTPService.cs` - OTP generation and email sending
- ✅ `OTPVerificationDialog.cs` - OTP input dialog
- ✅ `OTPVerificationDialog.Designer.cs` - UI layout
- ✅ `OTPVerificationDialog.resx` - Resources

### Modified Files

- ✅ `QRScannerForm.cs` - Added OTP verification step
- ✅ `Data/ScanHistoryRepository.cs` - Added helper method

---

## ✨ Features

### For Students

- 📧 Receive 6-digit OTP via email
- ⏱️ 5-minute expiration timer
- 🔄 Resend OTP if needed (max 3 times)
- ✓ Clean, professional email template

### Security

- 🔒 One-time use codes
- ⏰ Time-limited (5 minutes)
- 🚫 Max 3 verification attempts
- 🔐 Secure SSL/TLS email delivery

---

## 🎯 How It Works

```
Student Scans QR Code
        ↓
System Identifies Student
        ↓
Determines Time In or Time Out
        ↓
Generates 6-Digit OTP
        ↓
Sends Email to Student
        ↓
Shows OTP Verification Dialog
        ↓
Student Enters OTP
        ↓
System Verifies Code
        ↓
✓ Attendance Recorded!
```

---

## 🔧 Configuration (Optional)

If you want to use a different email address, edit `Services/OTPService.cs`:

```csharp
// Line 14-17
private const string SMTP_HOST = "smtp.gmail.com";
private const int SMTP_PORT = 587;
private const string SENDER_EMAIL = "your-email@gmail.com"; // Change here
private const string APP_PASSWORD = "your-app-password"; // Change here
```

### How to Get Gmail App Password

1. Go to https://myaccount.google.com/security
2. Enable **2-Step Verification**
3. Go to **App passwords**
4. Generate password for "Mail" and "Windows Computer"
5. Copy the 16-character password
6. Use it in the code above

---

## 📧 Email Template Preview

Students will receive an email like this:

```
Subject: Attendance Time In - OTP Verification

🎓 Student Attendance System
Time In Verification

Hello, John Doe!

Your One-Time Password (OTP) for Time In verification:

    123456

Valid for 5 minutes

Student Number: 2021-12345
Action: Time In
Date & Time: Dec 01, 2025 10:30 AM
Expires At: 10:35 AM

⚠️ Security Notice:
This OTP is for your attendance verification only.
Do not share this code with anyone.

⏱️ This code will expire in 5 minutes.
```

---

## ✅ Testing Checklist

- [ ] Student has valid email address
- [ ] Email arrives within 5-10 seconds
- [ ] OTP dialog shows student name and type (Time In/Out)
- [ ] Timer counts down from 5:00
- [ ] Entering correct OTP marks attendance
- [ ] Entering wrong OTP shows error
- [ ] Resend button sends new OTP
- [ ] Expired OTP shows error message

---

## ❓ Troubleshooting

### Email Not Arriving?

1. **Check spam folder** - Gmail might filter it
2. **Verify student email** - Must be valid format
3. **Check internet connection** - Required for SMTP
4. **Wait 30 seconds** - Sometimes delayed

### Dialog Not Showing?

1. **Check student has email** in database
2. **Look for error messages** in the app
3. **Verify build succeeded** - No compilation errors

### Can't Send Emails?

1. **Verify App Password** - Must be correct
2. **Check 2FA enabled** on Gmail account
3. **Test internet connection**
4. **Check firewall settings** - Allow SMTP port 587

---

## 🎉 You're All Set!

The OTP email verification is now integrated into your student attendance system. Every scan will require email verification, preventing unauthorized attendance marking.

**Next Steps:**

1. Test with a few students
2. Ensure all students have valid emails
3. Monitor email delivery
4. Gather user feedback

**Support:**

- Check `Documentations/OTP_EMAIL_INTEGRATION.md` for detailed documentation
- Review code comments in `Services/OTPService.cs`
- Check error logs if issues occur

---

**Status**: ✅ Ready to Use
**Last Updated**: December 1, 2025

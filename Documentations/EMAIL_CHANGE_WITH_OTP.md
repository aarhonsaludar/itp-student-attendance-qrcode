# Email Change with OTP Verification

**Date:** December 1, 2025  
**Feature:** Secure Email Change with Two-Step OTP Verification

## Overview

When students need to change their email address in the Edit Student dialog, the system now requires **two-step OTP verification** for security:

1. **Verify OLD email** - Prove they own the current account
2. **Verify NEW email** - Prove they can access the new email

This prevents unauthorized email changes and ensures both emails are valid and accessible.

## How It Works

### User Flow

1. **Student opens Edit Student Dialog**
   - All fields are pre-filled with current information
2. **Student changes email address**

   - Enters a new email in the email field
   - Clicks SAVE button

3. **System detects email change**

   - Shows warning: "Email address has changed!"
   - Asks for verification confirmation

4. **STEP 1: Verify Current Email**

   - System sends OTP to the CURRENT email address
   - Student receives email: "🔐 Verify Current Email - Email Change Request"
   - Student enters 6-digit OTP code
   - Code expires in 5 minutes

5. **STEP 2: Verify New Email**

   - After Step 1 succeeds, system sends OTP to NEW email address
   - Student receives email: "✅ Verify New Email - Email Change Request"
   - Student enters 6-digit OTP code
   - Code expires in 5 minutes

6. **Verification Complete**
   - System shows: "✅ Email Verification Complete!"
   - Student clicks SAVE again to apply changes
   - Email is updated in database

### Security Features

✅ **Two-Factor Verification**

- Requires access to BOTH old and new email accounts
- Prevents unauthorized changes even if someone has the password

✅ **Time-Limited Codes**

- OTP expires in 5 minutes
- Cannot reuse codes

✅ **Separate Sessions**

- Each step has its own OTP session
- Step 2 only works after Step 1 succeeds

✅ **Clear Email Templates**

- Professional emails with security warnings
- Shows which step (1 of 2 or 2 of 2)
- Displays student information for verification

## Technical Implementation

### Files Modified

1. **`Models\AttendanceType.cs`**

   - Added `EmailChange` enum value

2. **`Services\OTPService.cs`**

   - Added `InitiateEmailChangeVerifyOldAsync()` method
   - Added `InitiateEmailChangeVerifyNewAsync()` method
   - Added `SendEmailChangeOTPAsync()` method

3. **`EditStudentDialog.cs`**
   - Added email change detection
   - Added `VerifyEmailChangeAsync()` method
   - Integrated with OTP verification dialog

### Code Flow

```csharp
// Step 1: Send OTP to OLD email
var oldEmailSession = await OTPService.InitiateEmailChangeVerifyOldAsync(student, newEmail);

// Verify OLD email
using (var otpDialog = new OTPVerificationDialog(oldEmailSession))
{
    if (otpDialog.ShowDialog() == DialogResult.OK && otpDialog.IsVerified)
    {
        // Step 2: Send OTP to NEW email
        var newEmailSession = await OTPService.InitiateEmailChangeVerifyNewAsync(oldEmailSession);

        // Verify NEW email
        using (var otpDialog2 = new OTPVerificationDialog(newEmailSession))
        {
            if (otpDialog2.ShowDialog() == DialogResult.OK && otpDialog2.IsVerified)
            {
                // Both verified - allow save
                emailChangeVerified = true;
            }
        }
    }
}
```

## Email Templates

### Step 1: Verify Current Email

```
Subject: 🔐 Verify Current Email - Email Change Request

Hello [Student Name],

You have requested to change your email address. To proceed, please verify
your CURRENT email by entering the code below:

[6-DIGIT OTP CODE]

📧 Email Change Details:
- Student: [Name]
- Student Number: [Number]
- Verifying: CURRENT email
- Expires At: [Time]

🔒 Security Notice:
This code is for email change verification only. Do not share this code
with anyone. If you did not request this change, please contact your
administrator immediately.
```

### Step 2: Verify New Email

```
Subject: ✅ Verify New Email - Email Change Request

Hello [Student Name],

Almost done! Please verify your NEW email address by entering the code below:

[6-DIGIT OTP CODE]

📧 Email Change Details:
- Student: [Name]
- Student Number: [Number]
- Verifying: NEW email
- Expires At: [Time]

🔒 Security Notice:
This code is for email change verification only. Do not share this code
with anyone. If you did not request this change, please contact your
administrator immediately.
```

## Usage Instructions

### For Students

1. Open your student record in Edit Student dialog
2. Change your email address to the new email
3. Click SAVE
4. Follow the two-step verification:
   - Check your CURRENT email for OTP
   - Enter the code
   - Check your NEW email for OTP
   - Enter the code
5. Click SAVE again to finalize the change

### For Administrators

- No special configuration needed
- Email change attempts are logged
- Failed verifications are tracked
- OTP codes expire automatically after 5 minutes

## Error Handling

| Error                                  | Cause                           | Solution                        |
| -------------------------------------- | ------------------------------- | ------------------------------- |
| "Current email verification failed"    | Wrong OTP entered for old email | Request new verification        |
| "New email verification failed"        | Wrong OTP entered for new email | Start over from Step 1          |
| "OTP expired"                          | Took longer than 5 minutes      | Request new verification        |
| "Email change cancelled"               | User clicked Cancel             | Email remains unchanged         |
| "Student current email not registered" | No email on file                | Add email first, then change it |

## Security Considerations

### Why Two-Step Verification?

**Without 2-step:**

- If someone steals a student's password, they can change the email
- Original owner loses access forever
- No way to recover the account

**With 2-step:**

- Attacker needs access to BOTH emails
- Original owner receives notification on current email
- Can prevent unauthorized changes immediately
- Much harder to hijack accounts

### Additional Security

- OTP codes are 6-digit random numbers
- Each code is single-use
- Codes expire in 5 minutes
- Failed attempts are tracked
- Email notifications sent to both addresses
- Clear audit trail in logs

## Future Enhancements

Potential improvements:

1. **SMS Verification** - Add phone number verification as alternative
2. **Admin Approval** - Require admin approval for email changes
3. **Cooling Period** - Add 24-hour waiting period before change takes effect
4. **Email History** - Track all email changes with timestamps
5. **Notification** - Send notification to old email after successful change

## Testing Checklist

- [ ] Change email to valid address
- [ ] Receive OTP on old email
- [ ] Verify old email with correct OTP
- [ ] Receive OTP on new email
- [ ] Verify new email with correct OTP
- [ ] Email updated in database
- [ ] Can login with new email for future OTP
- [ ] Wrong OTP rejected
- [ ] Expired OTP rejected
- [ ] Cancel works at any step
- [ ] Email format validation works

## Notes

- The email change is NOT applied until BOTH verifications succeed AND user clicks SAVE
- Users can cancel at any point - email remains unchanged
- OTP dialog shows current step (Step 1 of 2 or Step 2 of 2)
- Clear visual feedback at each step
- Professional email templates match existing attendance OTP emails

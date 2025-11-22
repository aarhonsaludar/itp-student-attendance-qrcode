# Profile Picture Upload - Quick Start Guide

## What's New?

Students' profile pictures can now be uploaded and managed directly from the **Student Record Screen** when viewing an existing student's details.

## How to Use

### Uploading a Profile Photo:

1. **Open Student Record**

   - Scan a QR code or select a student from the records list
   - The StudentRecordScreen opens showing the student's information

2. **Click on Profile Picture**

   - The default avatar is displayed in the center-left area
   - Click on the profile picture (you'll see the cursor change to a hand)

3. **Select Image File**

   - A file dialog opens automatically
   - Choose an image file from your computer
   - Supported formats: JPG, PNG, BMP, GIF
   - The dialog defaults to "My Pictures" folder

4. **Image Processing**

   - The selected image is automatically copied to: `Images/Students/` folder
   - File is named with student ID and timestamp: `{StudentId}_{yyyyMMdd_HHmmss}.ext`
   - Example: `5_20240115_143045.jpg`

5. **Database Storage**
   - Photo path is saved to the `students.photo_path` column
   - "Profile photo updated successfully!" message appears
   - Photo persists even after closing and reopening the app

### Viewing Stored Photos:

- When you open a student record, if they have a photo in the database, it displays automatically
- If the photo file is missing or corrupted, the default avatar shows instead
- No manual action needed - everything happens automatically

## Technical Details

### Database Column

```sql
-- Already exists in students table
photo_path VARCHAR(500) NULL
```

### Storage Location

```
Application Folder/
├── Images/
│   └── Students/
│       ├── 5_20240115_143045.jpg
│       ├── 5_20240115_143122.png
│       └── 7_20240115_150305.jpg
```

### File Naming Convention

- Pattern: `{studentId}_{yyyyMMdd_HHmmss}.{extension}`
- StudentId: Numeric ID from database
- Timestamp: Prevents filename conflicts when same student uploads multiple photos
- Example: `12_20240115_095430.jpg`

## Error Handling

| Scenario              | Behavior                                           |
| --------------------- | -------------------------------------------------- |
| File cannot be found  | Default avatar displays                            |
| Invalid image format  | Error message shows                                |
| Upload fails          | Error message with details                         |
| Database update fails | Photo still displays locally, logged for debugging |
| User cancels dialog   | No action taken                                    |

## Troubleshooting

### "Error uploading photo" message

- Check that the image file exists and is accessible
- Ensure you have read permissions for the selected file
- Try a different image file

### Photo doesn't persist after reopening

- Check if the file still exists in Images/Students/ folder
- Verify database has write permissions
- Check photo_path value in students table for that student

### Default avatar keeps showing

- Ensure photo_path in database is not NULL
- Verify the file path stored in database is correct
- Check Images/Students/ folder exists and contains the file

## Implementation Details

### Key Methods (StudentRecordScreen.cs):

1. **PicProfilePhoto_Click()**

   - Opens file dialog
   - Copies image to storage
   - Updates UI
   - Triggers database update

2. **UpdateStudentPhotoAsync()**

   - Saves photo path to database
   - Handles async operations
   - Error logging

3. **LoadStudentDataAsync()**
   - Loads photo from database on form open
   - Falls back to default avatar if needed

## Future Enhancements

- [ ] Image cropping tool
- [ ] Photo size validation
- [ ] Bulk photo upload
- [ ] Photo viewing history
- [ ] Cloud storage integration
- [ ] Profile picture in StudentRegistration

## Need More Info?

See: `Documentations/PROFILE_PICTURE_UPLOAD.md` for detailed documentation

using System;
using System.Windows.Forms;
using System.Drawing;
using ITP104_FINAL_PROJECT.Models;
using ITP104_FINAL_PROJECT.Data;
using System.Text.RegularExpressions;

namespace ITP104_FINAL_PROJECT
{
    /// <summary>
    /// Edit Student Dialog - Matches StudentRegistration form fields
    /// Allows editing of student information except Student ID (read-only)
    /// </summary>
    public partial class EditStudentDialog : Form
    {
        private Student originalStudent;
        private StudentRepository studentRepository;
        
        public Student UpdatedStudent { get; private set; }

        public EditStudentDialog(Student student)
        {
            originalStudent = student;
            studentRepository = new StudentRepository();
            
            InitializeComponent();
            InitializeFormData();
            LoadStudentData();
        }

        /// <summary>
        /// Initialize form controls and populate dropdowns
        /// </summary>
        private void InitializeFormData()
        {
            // Populate Year Level dropdown (matches StudentRegistration)
            cmbYearLevel.Items.Clear();
            cmbYearLevel.Items.AddRange(new string[] {
                "1st Year", "2nd Year", "3rd Year", "4th Year"
            });

            // Populate Course dropdown (matches StudentRegistration)
            cmbCourse.Items.Clear();
            cmbCourse.Items.AddRange(new string[] {
                "Computer Science",
                "Information Technology",
                "Nursing",
                "Educ",
                "Psychology"
            });

            // Populate Sex dropdown
            cmbFinalSex.Items.Clear();
            cmbFinalSex.Items.AddRange(new string[] {
                "Male",
                "Female"
            });
        }

        /// <summary>
        /// Load existing student data into form controls
        /// </summary>
        private void LoadStudentData()
        {
            // Student ID - Read-only (visible but disabled)
            txtStudentID.Text = originalStudent.StudentNumber;
            txtStudentID.ReadOnly = true;
            txtStudentID.Enabled = false; // Grayed out appearance
            txtStudentID.FillColor = Color.FromArgb(230, 230, 230);

            // Editable fields
            txtName.Text = GetFullName(originalStudent);
            txtEmail.Text = originalStudent.Email ?? "";
            txtPhone.Text = originalStudent.Phone ?? "";
            txtSection.Text = originalStudent.Section ?? "";

            // Course dropdown
            cmbCourse.SelectedItem = originalStudent.Program;

            // Year Level dropdown - Convert from number to display format
            string yearLevelDisplay = ConvertYearLevelToDisplay(originalStudent.YearLevel);
            cmbYearLevel.SelectedItem = yearLevelDisplay;

            // Sex dropdown
            if (!string.IsNullOrWhiteSpace(originalStudent.Sex))
            {
                cmbFinalSex.SelectedItem = originalStudent.Sex;
            }
        }

        /// <summary>
        /// Get full name from student object (combines first, middle, last)
        /// </summary>
        private string GetFullName(Student student)
        {
            string fullName = student.FirstName;
            
            if (!string.IsNullOrWhiteSpace(student.MiddleName))
            {
                fullName += " " + student.MiddleName;
            }
            
            fullName += " " + student.LastName;
            
            return fullName.Trim();
        }

        /// <summary>
        /// Convert year level number (1,2,3,4) to display format (1st Year, 2nd Year, etc.)
        /// </summary>
        private string ConvertYearLevelToDisplay(string yearLevel)
        {
            switch (yearLevel)
            {
                case "1": return "1st Year";
                case "2": return "2nd Year";
                case "3": return "3rd Year";
                case "4": return "4th Year";
                default: return "1st Year";
            }
        }

        /// <summary>
        /// Convert display format to year level number for database
        /// </summary>
        private string ConvertDisplayToYearLevel(string display)
        {
            if (display.Contains("1st")) return "1";
            if (display.Contains("2nd")) return "2";
            if (display.Contains("3rd")) return "3";
            if (display.Contains("4th")) return "4";
            return "1";
        }

        /// <summary>
        /// Save button click handler - Validates and updates student
        /// </summary>
        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate required fields (matches StudentRegistration validation)
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("Please enter student name.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtName.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtEmail.Text))
                {
                    MessageBox.Show("Please enter email address.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtEmail.Focus();
                    return;
                }

                // Email format validation (matches StudentRegistration)
                if (!IsValidEmail(txtEmail.Text))
                {
                    MessageBox.Show("Please enter a valid email address.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtEmail.Focus();
                    return;
                }

                if (cmbCourse.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select a course.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmbCourse.Focus();
                    return;
                }

                if (cmbYearLevel.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select a year level.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmbYearLevel.Focus();
                    return;
                }

                if (cmbFinalSex.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select sex/gender.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmbFinalSex.Focus();
                    return;
                }

                // Parse name (split into first, middle, last) - matches StudentRegistration logic
                string[] nameParts = txtName.Text.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string firstName = nameParts.Length > 0 ? nameParts[0] : "";
                string middleName = nameParts.Length > 2 ? nameParts[1] : "";
                string lastName = nameParts.Length > 1 ? nameParts[nameParts.Length - 1] : "";

                // Extract year level number
                string yearLevel = ConvertDisplayToYearLevel(cmbYearLevel.Text);

                // Create updated student object
                UpdatedStudent = new Student
                {
                    StudentId = originalStudent.StudentId,
                    StudentNumber = originalStudent.StudentNumber, // Cannot change (read-only)
                    FirstName = firstName,
                    MiddleName = string.IsNullOrWhiteSpace(middleName) ? null : middleName,
                    LastName = lastName,
                    Email = txtEmail.Text.Trim(),
                    Phone = string.IsNullOrWhiteSpace(txtPhone.Text) ? null : txtPhone.Text.Trim(),
                    Sex = cmbFinalSex.SelectedItem?.ToString(), // Sex field
                    YearLevel = yearLevel,
                    Program = cmbCourse.Text,
                    Section = string.IsNullOrWhiteSpace(txtSection.Text) ? null : txtSection.Text.Trim(),
                    Status = originalStudent.Status, // Keep original status
                    QRCodeData = originalStudent.QRCodeData, // Keep original QR code
                    PhotoPath = originalStudent.PhotoPath, // Keep original photo
                    EnrollmentDate = originalStudent.EnrollmentDate, // Keep original enrollment date
                    CreatedAt = originalStudent.CreatedAt
                };

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving changes: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Cancel button click handler
        /// </summary>
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// Email validation (matches StudentRegistration validation)
        /// </summary>
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}

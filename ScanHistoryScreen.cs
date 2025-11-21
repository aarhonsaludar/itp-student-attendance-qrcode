using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using ITP104_FINAL_PROJECT.Data;
using ITP104_FINAL_PROJECT.Models;

namespace ITP104_FINAL_PROJECT
{
    public partial class ScanHistoryScreen : Form
    {
        private Timer animationTimer;
        private int animationStep = 0;
        private int currentPage = 1;
        private int itemsPerPage = 50; // Increased for better display
        private int totalRecords = 0;
        private readonly ScanHistoryRepository scanHistoryRepository;
        
        // Optional: Timer for real-time date updates
        private Timer realTimeDateTimer;

        public ScanHistoryScreen()
        {
            InitializeComponent();
            scanHistoryRepository = new ScanHistoryRepository();
            InitializeForm();
        }

        private void InitializeForm()
        {
            // Initialize animation timer
            animationTimer = new Timer();
            animationTimer.Interval = 30;
            animationTimer.Tick += AnimationTimer_Tick;

            // Initialize Real-Time Date Timer (Optional)
            // This timer updates the DateTimePicker to the current time every second.
            // Use this if you want the filter to always track "Now" when the user hasn't manually selected a past date.
            realTimeDateTimer = new Timer();
            realTimeDateTimer.Interval = 1000; // 1 second
            realTimeDateTimer.Tick += RealTimeDateTimer_Tick;
            // realTimeDateTimer.Start(); // Uncomment to enable real-time clock updates on the date picker

            // Set default date to Today and Checked
            dtpDateFrom.Value = DateTime.Now;
            dtpDateFrom.Checked = true;

            // Setup event handlers
            btnSearch.Click += BtnSearch_Click;
            dtpDateFrom.ValueChanged += DtpDateFrom_ValueChanged;
            btnClearFilter.Click += BtnClearFilter_Click;
            btnExport.Click += BtnExport_Click;
            btnClose.Click += BtnClose_Click;
            btnPreviousPage.Click += BtnPreviousPage_Click;
            btnNextPage.Click += BtnNextPage_Click;

            // Setup DataGridView
            InitializeDataGrid();

            // Setup hover effects
            SetupHoverEffects();

            // Load real data from database
            _ = LoadScanHistoryAsync();

            // Start animation
            animationTimer.Start();

            // Hide unused filters
            dtpDateTo.Visible = false;
            lblDateTo.Visible = false;
            cmbScanType.Visible = false;
            
            // Add static label for Scan Type
            Label lblQrCodeStatic = new Label();
            lblQrCodeStatic.Text = "QR Code";
            lblQrCodeStatic.Font = new Font("Segoe UI", 10F);
            lblQrCodeStatic.ForeColor = Color.FromArgb(68, 88, 112);
            lblQrCodeStatic.Location = cmbScanType.Location;
            lblQrCodeStatic.Size = cmbScanType.Size;
            lblQrCodeStatic.TextAlign = ContentAlignment.MiddleLeft;
            lblQrCodeStatic.BackColor = Color.Transparent; // Or match background
            // Add to panel
            pnlFilters.Controls.Add(lblQrCodeStatic);
        }

        private void RealTimeDateTimer_Tick(object sender, EventArgs e)
        {
            // Update the DateTimePicker value to the current time
            // Note: This might trigger ValueChanged event, so ensure your logic handles that appropriately
            // to avoid excessive database calls if the Date part hasn't changed.
            if (dtpDateFrom.Value.Date != DateTime.Now.Date)
            {
                 dtpDateFrom.Value = DateTime.Now;
            }
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            animationStep++;

            if (animationStep <= 20)
            {
                // Fade in effect
                pnlFilters.Visible = true;
                pnlDataGrid.Visible = true;
            }
            else
            {
                animationTimer.Stop();
            }
        }

        private void SetupHoverEffects()
        {
            // Search button hover
            btnSearch.MouseEnter += (s, e) =>
            {
                btnSearch.FillColor = Color.FromArgb(40, 120, 180);
                btnSearch.Cursor = Cursors.Hand;
            };
            btnSearch.MouseLeave += (s, e) =>
            {
                btnSearch.FillColor = Color.FromArgb(52, 152, 219);
            };

            // Clear filter button hover
            btnClearFilter.MouseEnter += (s, e) =>
            {
                btnClearFilter.FillColor = Color.FromArgb(200, 60, 60);
                btnClearFilter.Cursor = Cursors.Hand;
            };
            btnClearFilter.MouseLeave += (s, e) =>
            {
                btnClearFilter.FillColor = Color.FromArgb(231, 76, 60);
            };

            // Export button hover
            btnExport.MouseEnter += (s, e) =>
            {
                btnExport.FillColor = Color.FromArgb(90, 180, 90);
                btnExport.Cursor = Cursors.Hand;
            };
            btnExport.MouseLeave += (s, e) =>
            {
                btnExport.FillColor = Color.FromArgb(46, 204, 113);
            };

            // Pagination buttons hover
            btnPreviousPage.MouseEnter += (s, e) =>
            {
                btnPreviousPage.FillColor = Color.FromArgb(120, 120, 120);
                btnPreviousPage.Cursor = Cursors.Hand;
            };
            btnPreviousPage.MouseLeave += (s, e) =>
            {
                btnPreviousPage.FillColor = Color.FromArgb(149, 165, 166);
            };

            btnNextPage.MouseEnter += (s, e) =>
            {
                btnNextPage.FillColor = Color.FromArgb(120, 120, 120);
                btnNextPage.Cursor = Cursors.Hand;
            };
            btnNextPage.MouseLeave += (s, e) =>
            {
                btnNextPage.FillColor = Color.FromArgb(149, 165, 166);
            };

            // Panel hover effects
            pnlDataGrid.MouseEnter += (s, e) =>
            {
                pnlDataGrid.ShadowDecoration.Depth = 20;
            };
            pnlDataGrid.MouseLeave += (s, e) =>
            {
                pnlDataGrid.ShadowDecoration.Depth = 10;
            };
        }

        private void InitializeDataGrid()
        {
            dgvScanHistory.Columns.Clear();
            dgvScanHistory.AutoGenerateColumns = false;
            dgvScanHistory.AllowUserToAddRows = false;
            dgvScanHistory.AllowUserToDeleteRows = false;
            dgvScanHistory.ReadOnly = true;

            // Create columns
            DataGridViewTextBoxColumn colDate = new DataGridViewTextBoxColumn
            {
                Name = "Date",
                HeaderText = "Date",
                Width = 150
            };

            DataGridViewTextBoxColumn colTimeIn = new DataGridViewTextBoxColumn
            {
                Name = "TimeIn",
                HeaderText = "Time In",
                Width = 130
            };

            DataGridViewTextBoxColumn colTimeOut = new DataGridViewTextBoxColumn
            {
                Name = "TimeOut",
                HeaderText = "Time Out",
                Width = 130
            };

            DataGridViewTextBoxColumn colStudentID = new DataGridViewTextBoxColumn
            {
                Name = "StudentID",
                HeaderText = "Student ID",
                Width = 130
            };

            DataGridViewTextBoxColumn colName = new DataGridViewTextBoxColumn
            {
                Name = "Name",
                HeaderText = "Student Name",
                Width = 200
            };

            DataGridViewTextBoxColumn colScanType = new DataGridViewTextBoxColumn
            {
                Name = "ScanType",
                HeaderText = "Scan Type",
                Width = 130
            };

            DataGridViewTextBoxColumn colStatus = new DataGridViewTextBoxColumn
            {
                Name = "Status",
                HeaderText = "Status",
                Width = 120
            };

            DataGridViewButtonColumn colAction = new DataGridViewButtonColumn
            {
                Name = "Action",
                HeaderText = "Action",
                Width = 130,
                Text = "View Details",
                UseColumnTextForButtonValue = true
            };

            dgvScanHistory.Columns.AddRange(new DataGridViewColumn[]
            {
                colDate, colTimeIn, colTimeOut, colStudentID, colName, colScanType, colStatus, colAction
            });

            // Style the DataGridView
            dgvScanHistory.BackgroundColor = Color.White;
            dgvScanHistory.BorderStyle = BorderStyle.None;
            dgvScanHistory.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvScanHistory.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvScanHistory.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgvScanHistory.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvScanHistory.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            dgvScanHistory.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 73, 94);
            dgvScanHistory.ColumnHeadersDefaultCellStyle.Padding = new Padding(10, 8, 10, 8);
            dgvScanHistory.ColumnHeadersHeight = 50;
            dgvScanHistory.DefaultCellStyle.BackColor = Color.White;
            dgvScanHistory.DefaultCellStyle.ForeColor = Color.FromArgb(64, 64, 64);
            dgvScanHistory.DefaultCellStyle.SelectionBackColor = Color.FromArgb(189, 195, 199);
            dgvScanHistory.DefaultCellStyle.SelectionForeColor = Color.FromArgb(44, 62, 80);
            dgvScanHistory.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            dgvScanHistory.DefaultCellStyle.Padding = new Padding(10, 5, 10, 5);
            dgvScanHistory.EnableHeadersVisualStyles = false;
            dgvScanHistory.GridColor = Color.FromArgb(231, 231, 231);
            dgvScanHistory.RowHeadersVisible = false;
            dgvScanHistory.RowTemplate.Height = 45;
            dgvScanHistory.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvScanHistory.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);

            // Handle button clicks
            dgvScanHistory.CellContentClick += DgvScanHistory_CellContentClick;

            // Make columns sortable
            foreach (DataGridViewColumn column in dgvScanHistory.Columns)
            {
                if (column.Name != "Action")
                {
                    column.SortMode = DataGridViewColumnSortMode.Automatic;
                }
            }
        }

        private bool isLoading = false;

        private async Task LoadScanHistoryAsync()
        {
            if (isLoading) return;
            isLoading = true;

            try
            {
                dgvScanHistory.Rows.Clear();

                // Get filter criteria - only apply if date pickers are explicitly checked
                DateTime? startDate = null;
                DateTime? endDate = null;

                // Only apply date filter if user has explicitly checked the date picker
                if (dtpDateFrom.Checked)
                {
                    startDate = dtpDateFrom.Value.Date;
                    // Since Date To is hidden, treat Date From as "Specific Date"
                    // Filter for the entire day (00:00:00 to 23:59:59)
                    endDate = startDate.Value.AddDays(1).AddSeconds(-1);
                }

                // Load scan history from database
                var scanHistory = await scanHistoryRepository.GetHistoryAsync(
                    startDate: startDate,
                    endDate: endDate,
                    studentId: null,
                    scanType: null
                );

                // Filter by search text if provided
                string searchText = txtSearch.Text.Trim().ToLower();
                if (!string.IsNullOrEmpty(searchText))
                {
                    scanHistory = scanHistory.Where(s =>
                        s.StudentNumber?.ToLower().Contains(searchText) == true ||
                        s.StudentName?.ToLower().Contains(searchText) == true
                    ).ToList();
                }

                totalRecords = scanHistory.Count;

                // Apply pagination
                var pagedData = scanHistory
                    .OrderByDescending(s => s.ScanDateTime)
                    .Skip((currentPage - 1) * itemsPerPage)
                    .Take(itemsPerPage)
                    .ToList();

                // Populate DataGridView
                foreach (var scan in pagedData)
                {
                    string date = scan.ScanDateTime.ToString("MMM dd, yyyy");
                    string timeIn = scan.ScanDateTime.ToString("hh:mm tt");
                    string timeOut = scan.TimeOut.HasValue ? scan.TimeOut.Value.ToString("hh:mm tt") : "-";
                    string studentNumber = scan.StudentNumber ?? "N/A";
                    string studentName = scan.StudentName ?? "Unknown";
                    string scanType = scan.ScanType ?? "QR";
                    string status = scan.Status ?? "success";

                    // Format status with color coding
                    status = status.ToLower() == "success" ? "Success" :
                             status.ToLower() == "duplicate" ? "Duplicate" : "Failed";

                    dgvScanHistory.Rows.Add(date, timeIn, timeOut, studentNumber, studentName, scanType, status);

                    // Apply row color based on status
                    int rowIndex = dgvScanHistory.Rows.Count - 1;
                    if (status == "Duplicate")
                    {
                        dgvScanHistory.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Orange;
                    }
                    else if (status == "Failed")
                    {
                        dgvScanHistory.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Red;
                    }
                }

                // Update pagination info
                UpdatePaginationInfo();

                // Update total records label
                lblTotalRecords.Text = $"Total Records: {totalRecords}";

                // Only show message if user has applied filters
                // Use a non-blocking notification (e.g. update status label) instead of MessageBox
                // to prevent focus loss and minimizing issues during auto-filtering.
                if (totalRecords == 0 && (startDate.HasValue || endDate.HasValue || !string.IsNullOrEmpty(searchText)))
                {
                    // Optional: Update a status label if you have one, e.g.:
                    // lblStatus.Text = "No records found.";
                    // lblStatus.Visible = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading scan history: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                isLoading = false;
            }
        }

        private void UpdatePaginationInfo()
        {
            int totalPages = (int)Math.Ceiling((double)totalRecords / itemsPerPage);
            totalPages = Math.Max(1, totalPages); // At least 1 page

            lblPageInfo.Text = $"Page {currentPage} of {totalPages}";
            btnPreviousPage.Enabled = currentPage > 1;
            btnNextPage.Enabled = currentPage < totalPages;
        }

        private void DgvScanHistory_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == dgvScanHistory.Columns["Action"].Index)
            {
                var studentID = dgvScanHistory.Rows[e.RowIndex].Cells["StudentID"].Value.ToString();
                var studentName = dgvScanHistory.Rows[e.RowIndex].Cells["Name"].Value.ToString();
                var date = dgvScanHistory.Rows[e.RowIndex].Cells["Date"].Value.ToString();
                var timeIn = dgvScanHistory.Rows[e.RowIndex].Cells["TimeIn"].Value.ToString();
                var timeOut = dgvScanHistory.Rows[e.RowIndex].Cells["TimeOut"].Value.ToString();

                MessageBox.Show(
                    $"Scan Details:\n\n" +
                    $"Student ID: {studentID}\n" +
                    $"Name: {studentName}\n" +
                    $"Date: {date}\n" +
                    $"Time In: {timeIn}\n" +
                    $"Time Out: {timeOut}\n\n" +
                    "Full details screen will be available in Phase 2.",
                    "Scan Details",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }

        private async void DtpDateFrom_ValueChanged(object sender, EventArgs e)
        {
            currentPage = 1;
            await LoadScanHistoryAsync();
        }

        private async void BtnSearch_Click(object sender, EventArgs e)
        {
            currentPage = 1; // Reset to first page on search
            await LoadScanHistoryAsync();
        }

        private async void BtnClearFilter_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            dtpDateFrom.Value = DateTime.Now;
            dtpDateFrom.Checked = true;
            // dtpDateTo.Checked = false; // Removed
            // cmbScanType.SelectedIndex = -1; // Removed
            currentPage = 1;
            await LoadScanHistoryAsync();
        }

        private async void BtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "CSV File (*.csv)|*.csv|Excel File (*.xlsx)|*.xlsx",
                    Title = "Export Scan History",
                    FileName = $"Scan_History_{DateTime.Now:yyyyMMdd_HHmmss}"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    // Show progress
                    this.Cursor = Cursors.WaitCursor;
                    btnExport.Enabled = false;
                    btnExport.Text = "Exporting...";

                    // Get all data (not just current page) with current filters
                    DateTime? startDate = null;
                    DateTime? endDate = null;

                    if (dtpDateFrom.Checked)
                    {
                        startDate = dtpDateFrom.Value.Date;
                        endDate = startDate.Value.AddDays(1).AddSeconds(-1);
                    }

                    var allScanHistory = await scanHistoryRepository.GetHistoryAsync(
                        startDate: startDate,
                        endDate: endDate,
                        studentId: null,
                        scanType: null
                    );

                    // Apply search filter
                    string searchText = txtSearch.Text.Trim().ToLower();
                    if (!string.IsNullOrEmpty(searchText))
                    {
                        allScanHistory = allScanHistory.Where(s =>
                            s.StudentNumber?.ToLower().Contains(searchText) == true ||
                            s.StudentName?.ToLower().Contains(searchText) == true
                        ).ToList();
                    }

                    // Sort by date descending
                    allScanHistory = allScanHistory.OrderByDescending(s => s.ScanDateTime).ToList();

                    // Determine file type and export accordingly
                    string fileExtension = System.IO.Path.GetExtension(saveDialog.FileName).ToLower();

                    if (fileExtension == ".csv")
                    {
                        ExportToCsv(saveDialog.FileName, allScanHistory);
                    }
                    else if (fileExtension == ".xlsx")
                    {
                        // For Excel export, we need additional libraries (EPPlus, ClosedXML, etc.)
                        // For now, we'll export as CSV with .xlsx extension
                        MessageBox.Show(
                            "Excel export requires additional libraries.\n" +
                            "Exporting as CSV format instead.",
                            "Export Format",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                        // Change extension to .csv
                        string csvFileName = System.IO.Path.ChangeExtension(saveDialog.FileName, ".csv");
                        ExportToCsv(csvFileName, allScanHistory);
                    }

                    // Reset button state
                    this.Cursor = Cursors.Default;
                    btnExport.Enabled = true;
                    btnExport.Text = "Export";

                    MessageBox.Show(
                        $"Successfully exported {allScanHistory.Count} records!\n\n" +
                        $"File saved to:\n{saveDialog.FileName}",
                        "Export Successful",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                btnExport.Enabled = true;
                btnExport.Text = "Export";

                MessageBox.Show(
                    $"Error exporting scan history:\n{ex.Message}",
                    "Export Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void ExportToCsv(string filePath, List<ScanHistory> data)
        {
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath))
            {
                // Write header
                writer.WriteLine("Date,Time In,Time Out,Student ID,Student Name,Scan Type,Status,Location,Purpose,Notes");

                // Write data rows
                foreach (var scan in data)
                {
                    string date = scan.ScanDateTime.ToString("yyyy-MM-dd");
                    string timeIn = scan.ScanDateTime.ToString("HH:mm:ss");
                    string timeOut = scan.TimeOut.HasValue ? scan.TimeOut.Value.ToString("HH:mm:ss") : "";
                    string studentNumber = EscapeCsvField(scan.StudentNumber ?? "");
                    string studentName = EscapeCsvField(scan.StudentName ?? "");
                    string scanType = EscapeCsvField(scan.ScanType ?? "QR Code");
                    string status = EscapeCsvField(scan.Status ?? "");
                    string location = EscapeCsvField(scan.Location ?? "");
                    string purpose = EscapeCsvField(scan.ScanPurpose ?? "");
                    string notes = EscapeCsvField(scan.Notes ?? "");

                    writer.WriteLine($"{date},{timeIn},{timeOut},{studentNumber},{studentName},{scanType},{status},{location},{purpose},{notes}");
                }
            }
        }

        private string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field))
                return "";

            // If field contains comma, quote, or newline, wrap in quotes and escape quotes
            if (field.Contains(",") || field.Contains("\"") || field.Contains("\n") || field.Contains("\r"))
            {
                return "\"" + field.Replace("\"", "\"\"") + "\"";
            }

            return field;
        }

        private async void BtnPreviousPage_Click(object sender, EventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                await LoadScanHistoryAsync();
            }
        }

        private async void BtnNextPage_Click(object sender, EventArgs e)
        {
            int totalPages = (int)Math.Ceiling((double)totalRecords / itemsPerPage);
            if (currentPage < totalPages)
            {
                currentPage++;
                await LoadScanHistoryAsync();
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (animationTimer != null)
            {
                animationTimer.Stop();
                animationTimer.Dispose();
            }
            
            if (realTimeDateTimer != null)
            {
                realTimeDateTimer.Stop();
                realTimeDateTimer.Dispose();
            }
        }

        // Public method to refresh data
        public async void RefreshData()
        {
            currentPage = 1;
            await LoadScanHistoryAsync();
        }

        private void lblHeaderTitle_Click(object sender, EventArgs e)
        {

        }

        private void lblHeaderSubtitle_Click(object sender, EventArgs e)
        {

        }

        private void pnlMainContent_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}

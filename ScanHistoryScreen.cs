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

            // Setup event handlers
            btnSearch.Click += BtnSearch_Click;
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

        private async Task LoadScanHistoryAsync()
        {
            try
            {
                dgvScanHistory.Rows.Clear();

                // Get filter criteria - only apply if date pickers are explicitly checked
                DateTime? startDate = null;
                DateTime? endDate = null;

                // Only apply date filter if user has explicitly checked the date picker
                if (dtpDateFrom.Checked && dtpDateTo.Checked)
                {
                    startDate = dtpDateFrom.Value.Date;
                    endDate = dtpDateTo.Value.Date.AddDays(1).AddSeconds(-1);
                }
                else if (dtpDateFrom.Checked)
                {
                    startDate = dtpDateFrom.Value.Date;
                }
                else if (dtpDateTo.Checked)
                {
                    endDate = dtpDateTo.Value.Date.AddDays(1).AddSeconds(-1);
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

                // Filter by scan type if selected
                if (cmbScanType.SelectedIndex > 0) // Index 0 is "All"
                {
                    string selectedType = cmbScanType.Text;
                    scanHistory = scanHistory.Where(s =>
                        s.ScanType?.Equals(selectedType, StringComparison.OrdinalIgnoreCase) == true
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
                    string timeOut = "-"; // Not tracked in current schema
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
                if (totalRecords == 0 && (startDate.HasValue || endDate.HasValue || !string.IsNullOrEmpty(searchText) || cmbScanType.SelectedIndex > 0))
                {
                    MessageBox.Show("No scan records found matching the criteria.", "No Records",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading scan history: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private async void BtnSearch_Click(object sender, EventArgs e)
        {
            currentPage = 1; // Reset to first page on search
            await LoadScanHistoryAsync();
        }

        private async void BtnClearFilter_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            dtpDateFrom.Checked = false;
            dtpDateTo.Checked = false;
            cmbScanType.SelectedIndex = -1;
            currentPage = 1;
            await LoadScanHistoryAsync();
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog
            {
                Filter = "CSV File|*.csv|Excel File|*.xlsx|PDF File|*.pdf",
                Title = "Export Scan History",
                FileName = $"Scan_History_{DateTime.Now:yyyyMMdd_HHmmss}"
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show(
                    $"Scan history will be exported to:\n{saveDialog.FileName}\n\n" +
                    "Export functionality will be fully implemented in Phase 2.",
                    "Export",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
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

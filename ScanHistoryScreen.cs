using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace ITP104_FINAL_PROJECT
{
    public partial class ScanHistoryScreen : Form
    {
        private Timer animationTimer;
        private int animationStep = 0;
        private int currentPage = 1;
        private int itemsPerPage = 10;
        private int totalPages = 2;

        public ScanHistoryScreen()
        {
            InitializeComponent();
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

            // Load sample data
            LoadSampleData();

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

        private void LoadSampleData()
        {
            dgvScanHistory.Rows.Clear();

            // Sample data - 20 entries
            var sampleData = new[]
            {
                new { Date = "Nov 16, 2025", TimeIn = "08:15 AM", TimeOut = "05:00 PM", ID = "2300001", Name = "Juan Dela Cruz", Type = "QR Code", Status = "Success" },
                new { Date = "Nov 16, 2025", TimeIn = "08:12 AM", TimeOut = "05:10 PM", ID = "2300002", Name = "Emilia Santos", Type = "QR Code", Status = "Success" },
                new { Date = "Nov 16, 2025", TimeIn = "08:10 AM", TimeOut = "04:45 PM", ID = "2300003", Name = "Miguel Navarro", Type = "QR Code", Status = "Success" },
                new { Date = "Nov 16, 2025", TimeIn = "08:05 AM", TimeOut = "05:15 PM", ID = "2300004", Name = "Sarah Del Rosario", Type = "QR Code", Status = "Success" },
                new { Date = "Nov 16, 2025", TimeIn = "08:00 AM", TimeOut = "04:30 PM", ID = "2300005", Name = "David Buhain", Type = "QR Code", Status = "Success" },
                new { Date = "Nov 15, 2025", TimeIn = "08:45 AM", TimeOut = "04:45 PM", ID = "2300006", Name = "Jennifer Magbanua", Type = "QR Code", Status = "Success" },
                new { Date = "Nov 15, 2025", TimeIn = "08:30 AM", TimeOut = "04:30 PM", ID = "2300007", Name = "Roberto Galang", Type = "QR Code", Status = "Success" },
                new { Date = "Nov 15, 2025", TimeIn = "08:15 AM", TimeOut = "05:00 PM", ID = "2300008", Name = "Liza Andres", Type = "QR Code", Status = "Success" },
                new { Date = "Nov 15, 2025", TimeIn = "08:00 AM", TimeOut = "04:50 PM", ID = "2300009", Name = "Jaime Tolentino", Type = "QR Code", Status = "Success" },
                new { Date = "Nov 15, 2025", TimeIn = "07:45 AM", TimeOut = "03:45 PM", ID = "2300010", Name = "Maria Rodriguez", Type = "QR Code", Status = "Success" },
                new { Date = "Nov 15, 2025", TimeIn = "09:30 AM", TimeOut = "--", ID = "2300011", Name = "Wilfredo Lim", Type = "QR Code", Status = "Failed" },
                new { Date = "Nov 15, 2025", TimeIn = "08:15 AM", TimeOut = "05:00 PM", ID = "2300012", Name = "Patricia Balagtas", Type = "QR Code", Status = "Success" },
                new { Date = "Nov 15, 2025", TimeIn = "08:00 AM", TimeOut = "04:40 PM", ID = "2300013", Name = "Kristopher Mendoza", Type = "QR Code", Status = "Success" },
                new { Date = "Nov 15, 2025", TimeIn = "07:45 AM", TimeOut = "04:30 PM", ID = "2300014", Name = "Linda Yumul", Type = "QR Code", Status = "Success" },
                new { Date = "Nov 15, 2025", TimeIn = "08:30 AM", TimeOut = "05:10 PM", ID = "2300015", Name = "Daniel Dizon", Type = "QR Code", Status = "Success" },
                new { Date = "Nov 14, 2025", TimeIn = "08:15 AM", TimeOut = "04:45 PM", ID = "2300016", Name = "Nancy Arriola", Type = "QR Code", Status = "Success" },
                new { Date = "Nov 14, 2025", TimeIn = "08:00 AM", TimeOut = "05:00 PM", ID = "2300017", Name = "Mateo Lopez", Type = "QR Code", Status = "Success" },
                new { Date = "Nov 14, 2025", TimeIn = "07:45 AM", TimeOut = "04:30 PM", ID = "2300018", Name = "Sandra Malig", Type = "QR Code", Status = "Success" },
                new { Date = "Nov 14, 2025", TimeIn = "09:30 AM", TimeOut = "--", ID = "2300019", Name = "Antonio Pascual", Type = "QR Code", Status = "Failed" },
                new { Date = "Nov 14, 2025", TimeIn = "08:15 AM", TimeOut = "04:55 PM", ID = "2300020", Name = "Beatrice Vergara", Type = "QR Code", Status = "Success" }
            };


            // Load data for current page
            int startIndex = (currentPage - 1) * itemsPerPage;
            int endIndex = Math.Min(startIndex + itemsPerPage, sampleData.Length);

            for (int i = startIndex; i < endIndex; i++)
            {
                var data = sampleData[i];
                dgvScanHistory.Rows.Add(data.Date, data.TimeIn, data.TimeOut, data.ID, data.Name, data.Type, data.Status);
            }

            // Update pagination info
            UpdatePaginationInfo();

            // Update total records label
            lblTotalRecords.Text = $"Total Records: {sampleData.Length}";
        }

        private void UpdatePaginationInfo()
        {
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

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim().ToLower();
            DateTime? dateFrom = dtpDateFrom.Checked ? dtpDateFrom.Value.Date : (DateTime?)null;
            DateTime? dateTo = dtpDateTo.Checked ? dtpDateTo.Value.Date : (DateTime?)null;
            string scanType = cmbScanType.SelectedIndex >= 0 ? cmbScanType.Text : "";

            if (string.IsNullOrEmpty(searchText) && !dateFrom.HasValue && !dateTo.HasValue && string.IsNullOrEmpty(scanType))
            {
                MessageBox.Show("Please enter search criteria.", "Search", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Show filtering message
            string filters = "Filtering by:\n";
            if (!string.IsNullOrEmpty(searchText)) filters += $"- Search: {searchText}\n";
            if (dateFrom.HasValue) filters += $"- From: {dateFrom.Value:MMM dd, yyyy}\n";
            if (dateTo.HasValue) filters += $"- To: {dateTo.Value:MMM dd, yyyy}\n";
            if (!string.IsNullOrEmpty(scanType)) filters += $"- Type: {scanType}\n";

            MessageBox.Show(
                filters + "\nFiltering functionality will be fully implemented with database integration.",
                "Search Applied",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void BtnClearFilter_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            dtpDateFrom.Checked = false;
            dtpDateTo.Checked = false;
            cmbScanType.SelectedIndex = -1;
            currentPage = 1;
            LoadSampleData();
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

        private void BtnPreviousPage_Click(object sender, EventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                LoadSampleData();
            }
        }

        private void BtnNextPage_Click(object sender, EventArgs e)
        {
            if (currentPage < totalPages)
            {
                currentPage++;
                LoadSampleData();
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
        public void RefreshData()
        {
            currentPage = 1;
            LoadSampleData();
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

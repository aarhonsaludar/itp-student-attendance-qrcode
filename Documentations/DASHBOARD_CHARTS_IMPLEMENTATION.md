# Dashboard Charts Implementation Guide

This guide explains how to add beautiful, data-driven charts to the Main Dashboard, similar to the reference image provided.

## Overview

We'll replace the "Recent Scan Activity" DataGridView with three analytics charts:
1. **Today's Scan Activity** - Line chart showing hourly scan trends
2. **Attendance Trends** - Line chart showing weekly/monthly attendance patterns
3. **Students by Program** - Bar chart showing distribution across programs/courses

## Step 1: Add System.Windows.Forms.DataVisualization Reference

1. Open the project in Visual Studio
2. Right-click on the project → Add → Reference
3. In the Assemblies tab, search for `System.Windows.Forms.DataVisualization`
4. Check the checkbox and click OK

**OR** Manually edit `ITP104-FINAL-PROJECT.csproj`:
- Add this line after line 117 (`<Reference Include="System.Web" />`):
```xml
<Reference Include="System.Windows.Forms.DataVisualization" />
```

## Step 2: Add Using Statement to MainDashboard.cs

At the top of `MainDashboard.cs`, add:
```csharp
using System.Windows.Forms.DataVisualization.Charting;
```

## Step 3: Replace Recent Scans Panel with Charts Panel

### In InitializeDashboard() or MoveDashboardControlsToPanel()

Replace the `dgvRecentScans` DataGridView with a charts panel. Find where `dgvRecentScans` is located and replace it with:

```csharp
// Remove the old Recent Scans panel/grid
if (pnlDashboardContent.Controls.Contains(dgvRecentScans))
{
    pnlDashboardContent.Controls.Remove(dgvRecentScans);
}

// Create Charts Analytics Panel
Panel pnlChartsAnalytics = CreateChartsPanel();
pnlChartsAnalytics.Location = new Point(30, 380); // Adjust based on your layout
pnlChartsAnalytics.Size = new Size(1340, 500); // Adjust based on your layout
pnlDashboardContent.Controls.Add(pnlChartsAnalytics);
```

## Step 4: Create the Charts Panel Method

Add this method to `MainDashboard.cs`:

```csharp
private Panel CreateChartsPanel()
{
    // Main container panel
    Guna.UI2.WinForms.Guna2Panel pnlCharts = new Guna.UI2.WinForms.Guna2Panel
    {
        BackColor = Color.Transparent,
        Dock = DockStyle.None,
        BorderRadius = 15,
        FillColor = Color.White,
        ShadowDecoration = { Enabled = true, Depth = 10, BorderRadius = 15 }
    };

    // Title Label
    Label lblChartsTitle = new Label
    {
        Text = "📊 Dashboard Analytics",
        Font = new Font("Segoe UI", 16F, FontStyle.Bold),
        ForeColor = Color.FromArgb(44, 62, 80),
        Location = new Point(20, 15),
        AutoSize = true
    };
    pnlCharts.Controls.Add(lblChartsTitle);

    // Container for charts (3 columns)
    Panel pnlChartsContainer = new Panel
    {
        Location = new Point(20, 60),
        Size = new Size(pnlCharts.Width - 40, 420),
        BackColor = Color.Transparent
    };

    // Chart 1: Today's Scan Activity (Line Chart)
    Chart chartTodayActivity = CreateTodayActivityChart();
    chartTodayActivity.Location = new Point(0, 0);
    chartTodayActivity.Size = new Size(420, 400);
    pnlChartsContainer.Controls.Add(chartTodayActivity);

    // Chart 2: Attendance Trend (Line Chart)
    Chart chartAttendanceTrend = CreateAttendanceTrendChart();
    chartAttendanceTrend.Location = new Point(440, 0);
    chartAttendanceTrend.Size = new Size(420, 400);
    pnlChartsContainer.Controls.Add(chartAttendanceTrend);

    // Chart 3: Students by Program (Bar Chart)
    Chart chartProgramDistribution = CreateProgramDistributionChart();
    chartProgramDistribution.Location = new Point(880, 0);
    chartProgramDistribution.Size = new Size(420, 400);
    pnlChartsContainer.Controls.Add(chartProgramDistribution);

    pnlCharts.Controls.Add(pnlChartsContainer);

    // Load data into charts
    LoadChartsDataAsync();

    return pnlCharts;
}
```

## Step 5: Create Individual Chart Methods

### Chart 1: Today's Scan Activity (Line Chart)

```csharp
private Chart CreateTodayActivityChart()
{
    Chart chart = new Chart
    {
        BackColor = Color.Transparent,
        Dock = DockStyle.Fill
    };

    // Chart Area
    ChartArea chartArea = new ChartArea
    {
        Name = "ChartArea1",
        BackColor = Color.FromArgb(245, 248, 250),
        BorderColor = Color.Transparent,
        AxisX = {
            LineColor = Color.FromArgb(200, 200, 200),
            MajorGrid = { LineColor = Color.FromArgb(230, 230, 230) },
            LabelStyle = { ForeColor = Color.FromArgb(100, 100, 100), Font = new Font("Segoe UI", 8F) }
        },
        AxisY = {
            LineColor = Color.FromArgb(200, 200, 200),
            MajorGrid = { LineColor = Color.FromArgb(230, 230, 230) },
            LabelStyle = { ForeColor = Color.FromArgb(100, 100, 100), Font = new Font("Segoe UI", 8F) }
        }
    };
    chart.ChartAreas.Add(chartArea);

    // Series
    Series series = new Series
    {
        Name = "Today's Scans",
        ChartType = SeriesChartType.Line,
        Color = Color.FromArgb(46, 204, 113),
        BorderWidth = 3,
        MarkerStyle = MarkerStyle.Circle,
        MarkerSize = 8,
        MarkerColor = Color.FromArgb(46, 204, 113)
    };
    chart.Series.Add(series);

    // Title
    Title title = new Title
    {
        Text = "Today's Scan Activity",
        Font = new Font("Segoe UI", 12F, FontSt ile.Bold),
        ForeColor = Color.FromArgb(44, 62, 80),
        Docking = Docking.Top,
        Alignment = ContentAlignment.MiddleLeft
    };
    chart.Titles.Add(title);

    // Legend
    Legend legend = new Legend
    {
        Name = "Legend1",
        Docking = Docking.Bottom,
        Alignment = StringAlignment.Center,
        Font = new Font("Segoe UI", 8F)
    };
    chart.Legends.Add(legend);

    return chart;
}
```

### Chart 2: Attendance Trend (Line Chart)

```csharp
private Chart CreateAttendanceTrendChart()
{
    Chart chart = new Chart
    {
        BackColor = Color.Transparent,
        Dock = DockStyle.Fill
    };

    // Chart Area
    ChartArea chartArea = new ChartArea
    {
        Name = "ChartArea1",
        BackColor = Color.FromArgb(245, 248, 250),
        BorderColor = Color.Transparent,
        AxisX = {
            LineColor = Color.FromArgb(200, 200, 200),
            MajorGrid = { LineColor = Color.FromArgb(230, 230, 230) },
            LabelStyle = { ForeColor = Color.FromArgb(100, 100, 100), Font = new Font("Segoe UI", 8F) }
        },
        AxisY = {
            LineColor = Color.FromArgb(200, 200, 200),
            MajorGrid = { LineColor = Color.FromArgb(230, 230, 230) },
            LabelStyle = { ForeColor = Color.FromArgb(100, 100, 100), Font = new Font("Segoe UI", 8F) }
        }
    };
    chart.ChartAreas.Add(chartArea);

    // Series
    Series series = new Series
    {
        Name = "Daily Attendance",
        ChartType = SeriesChartType.Line,
        Color = Color.FromArgb(52, 152, 219),
        BorderWidth = 3,
        MarkerStyle = MarkerStyle.Circle,
        MarkerSize = 8,
        MarkerColor = Color.FromArgb(52, 152, 219)
    };
    chart.Series.Add(series);

    // Title
    Title title = new Title
    {
        Text = "7-Day Attendance Trend",
        Font = new Font("Segoe UI", 12F, FontStyle.Bold),
        ForeColor = Color.FromArgb(44, 62, 80),
        Docking = Docking.Top,
        Alignment = ContentAlignment.MiddleLeft
    };
    chart.Titles.Add(title);

    // Legend
    Legend legend = new Legend
    {
        Name = "Legend1",
        Docking = Docking.Bottom,
        Alignment = StringAlignment.Center,
        Font = new Font("Segoe UI", 8F)
    };
    chart.Legends.Add(legend);

    return chart;
}
```

### Chart 3: Program Distribution (Bar Chart)

```csharp
private Chart CreateProgramDistributionChart()
{
    Chart chart = new Chart
    {
        BackColor = Color.Transparent,
        Dock = DockStyle.Fill
    };

    // Chart Area
    ChartArea chartArea = new ChartArea
    {
        Name = "ChartArea1",
        BackColor = Color.FromArgb(245, 248, 250),
        BorderColor = Color.Transparent,
        AxisX = {
            LineColor = Color.FromArgb(200, 200, 200),
            MajorGrid = { LineColor = Color.Transparent },
            LabelStyle = { ForeColor = Color.FromArgb(100, 100, 100), Font = new Font("Segoe UI", 8F), Angle = -45 }
        },
        AxisY = {
            LineColor = Color.FromArgb(200, 200, 200),
            MajorGrid = { LineColor = Color.FromArgb(230, 230, 230) },
            LabelStyle = { ForeColor = Color.FromArgb(100, 100, 100), Font = new Font("Segoe UI", 8F) }
        }
    };
    chart.ChartAreas.Add(chartArea);

    // Series
    Series series = new Series
    {
        Name = "Students",
        ChartType = SeriesChartType.Column,
        Color = Color.FromArgb(255, 152, 0),
        BorderWidth = 0
    };
    chart.Series.Add(series);

    // Title
    Title title = new Title
    {
        Text = "Students by Program",
        Font = new Font("Segoe UI", 12F, FontStyle.Bold),
        ForeColor = Color.FromArgb(44, 62, 80),
        Docking = Docking.Top,
        Alignment = ContentAlignment.MiddleLeft
    };
    chart.Titles.Add(title);

    // Legend
    Legend legend = new Legend
    {
        Name = "Legend1",
        Docking = Docking.Bottom,
        Alignment = StringAlignment.Center,
        Font = new Font("Segoe UI", 8F)
    };
    chart.Legends.Add(legend);

    return chart;
}
```

## Step 6: Load Data from Database

Add these methods to load actual data:

```csharp
private async void LoadChartsDataAsync()
{
    try
    {
        await LoadTodayActivityData();
        await LoadAttendanceTrendData();
        await LoadProgramDistributionData();
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"Error loading charts data: {ex.Message}");
    }
}

private async Task LoadTodayActivityData()
{
    try
    {
        // Get scans for today grouped by hour
        var today = DateTime.Today;
        var scans = await scanHistoryRepository.GetHistoryAsync(
            startDate: today,
            endDate: today.AddDays(1).AddSeconds(-1),
            studentId: null,
            scanType: null
        );

        // Group by hour
        var hourlyData = scans
            .GroupBy(s => s.ScanDateTime.Hour)
            .Select(g => new { Hour = g.Key, Count = g.Count() })
            .OrderBy(x => x.Hour)
            .ToList();

        // Find the chart
        var chart = FindChartByTitle("Today's Scan Activity");
        if (chart != null && chart.Series.Count > 0)
        {
            chart.Series[0].Points.Clear();
            foreach (var data in hourlyData)
            {
                chart.Series[0].Points.AddXY($"{data.Hour:D2}:00", data.Count);
            }
        }
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"Error loading today activity: {ex.Message}");
    }
}

private async Task LoadAttendanceTrendData()
{
    try
    {
        // Get scans for last 7 days
        var endDate = DateTime.Today;
        var startDate = endDate.AddDays(-6);

        var scans = await scanHistoryRepository.GetHistoryAsync(
            startDate: startDate,
            endDate: endDate.AddDays(1),
            studentId: null,
            scanType: null
        );

        // Group by date
        var dailyData = scans
            .GroupBy(s => s.ScanDateTime.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .OrderBy(x => x.Date)
            .ToList();

        // Fill in missing dates with 0
        var allDays = new List<dynamic>();
        for (int i = 0; i < 7; i++)
        {
            var date = startDate.AddDays(i);
            var count = dailyData.FirstOrDefault(d => d.Date == date)?.Count ?? 0;
            allDays.Add(new { Date = date, Count = count });
        }

        // Find the chart
        var chart = FindChartByTitle("7-Day Attendance Trend");
        if (chart != null && chart.Series.Count > 0)
        {
            chart.Series[0].Points.Clear();
            foreach (var data in allDays)
            {
                chart.Series[0].Points.AddXY(data.Date.ToString("MMM dd"), data.Count);
            }
        }
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"Error loading attendance trend: {ex.Message}");
    }
}

private async Task LoadProgramDistributionData()
{
    try
    {
        // Get all active students
        var students = await studentRepository.GetAllAsync(activeOnly: true);

        // Group by program
        var programData = students
            .GroupBy(s => s.Program)
            .Select(g => new { Program = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(10) // Top 10 programs
            .ToList();

        // Find the chart
        var chart = FindChartByTitle("Students by Program");
        if (chart != null && chart.Series.Count > 0)
        {
            chart.Series[0].Points.Clear();
            foreach (var data in programData)
            {
                var point = chart.Series[0].Points.AddXY(data.Program, data.Count);
                // Alternate colors for visual appeal
                chart.Series[0].Points[point].Color = (point % 2 == 0) ? 
                    Color.FromArgb(255, 152, 0) : Color.FromArgb(255, 193, 7);
            }
        }
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"Error loading program distribution: {ex.Message}");
    }
}

private Chart FindChartByTitle(string title)
{
    // Helper method to find chart by title
    foreach (Control control in pnlDashboardContent.Controls)
    {
        if (control is Guna.UI2.WinForms.Guna2Panel panel)
        {
            foreach (Control panelControl in panel.Controls)
            {
                if (panelControl is Panel chartsContainer)
                {
                    foreach (Control chartControl in chartsContainer.Controls)
                    {
                        if (chartControl is Chart chart && chart.Titles.Count > 0 && chart.Titles[0].Text == title)
                        {
                            return chart;
                        }
                    }
                }
            }
        }
    }
    return null;
}
```

## Step 7: Update DashboardRefreshTimer

Add chart refresh to the existing timer:

```csharp
private async void DashboardRefreshTimer_Tick(object sender, EventArgs e)
{
    // Auto-refresh dashboard stats every 5 seconds
    await LoadDashboardStatsAsync();
    // Remove this line: await LoadRecentScansAsync();
    
    // Add charts refresh instead
    LoadChartsDataAsync();
}
```

## Adjustments and Customization

### Colors
- **Green**: `Color.FromArgb(46, 204, 113)` - Today's Activity
- **Blue**: `Color.FromArgb(52, 152, 219)` - Attendance Trend
- **Orange**: `Color.FromArgb(255, 152, 0)` - Program Distribution

### Layout
Adjust the `Location` and `Size` properties in `CreateChartsPanel()` to fit your dashboard layout.

### Data Refresh Rate
The charts will refresh every 5 seconds with the `dashboardRefreshTimer`. Adjust the interval if needed.

## Testing

1. Build the project
2. Run the application
3. Log in and view the dashboard
4. Verify that all three charts display with real data from your database
5. Check that charts update automatically every 5 seconds

## Troubleshooting

**Issue**: Charts not showing
- **Solution**: Verify the System.Windows.Forms.DataVisualization reference is added

**Issue**: No data in charts
- **Solution**: Check database connection and ensure there's data in scan_history and students tables

**Issue**: Layout issues  
- **Solution**: Adjust `Location` and `Size` properties in `CreateChartsPanel()`

## Result

You'll have a modern, data-driven dashboard with:
- Real-time scan activity visualization
- Weekly attendance trends
- Student distribution across programs
- All data fetched directly from your MySQL database
- Auto-refresh every 5 seconds

This matches the modern dashboard design from the reference image you provided!

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ITP104_FINAL_PROJECT.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json;

namespace ITP104_FINAL_PROJECT.Services
{
    /// <summary>
    /// Export Service - Handles data export to PDF, JSON, and CSV formats
    /// </summary>
    public static class ExportService
    {
        #region Student Record Export

        /// <summary>
        /// Export student record with scan history to specified format
        /// </summary>
        public static void ExportStudentRecord(string filePath, Student student, List<ScanHistory> scanHistory, ExportFormat format)
        {
            switch (format)
            {
                case ExportFormat.PDF:
                    ExportStudentToPdf(filePath, student, scanHistory);
                    break;
                case ExportFormat.JSON:
                    ExportStudentToJson(filePath, student, scanHistory);
                    break;
                case ExportFormat.CSV:
                    ExportStudentToCsv(filePath, student, scanHistory);
                    break;
                default:
                    throw new ArgumentException("Unsupported export format");
            }
        }

        private static void ExportStudentToPdf(string filePath, Student student, List<ScanHistory> scanHistory)
        {
            Document document = new Document(PageSize.A4, 50, 50, 50, 50);
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));

            document.Open();

            // Title
            Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, BaseColor.DARK_GRAY);
            Paragraph title = new Paragraph("STUDENT RECORD REPORT", titleFont);
            title.Alignment = Element.ALIGN_CENTER;
            title.SpacingAfter = 20;
            document.Add(title);

            // Student Information Section
            Font sectionFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, BaseColor.BLACK);
            Font labelFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.BLACK);
            Font valueFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.DARK_GRAY);

            Paragraph studentInfoTitle = new Paragraph("Student Information", sectionFont);
            studentInfoTitle.SpacingBefore = 10;
            studentInfoTitle.SpacingAfter = 10;
            document.Add(studentInfoTitle);

            // Student Info Table
            PdfPTable infoTable = new PdfPTable(2);
            infoTable.WidthPercentage = 100;
            infoTable.SetWidths(new float[] { 1, 2 });
            infoTable.SpacingAfter = 20;

            // Helper to add info rows
            Action<string, string> addInfoRow = (label, value) =>
            {
                PdfPCell labelCell = new PdfPCell(new Phrase(label, labelFont));
                labelCell.BackgroundColor = new BaseColor(240, 240, 240);
                labelCell.Padding = 8;
                labelCell.Border = Rectangle.BOX;

                PdfPCell valueCell = new PdfPCell(new Phrase(value ?? "", valueFont));
                valueCell.Padding = 8;
                valueCell.Border = Rectangle.BOX;

                infoTable.AddCell(labelCell);
                infoTable.AddCell(valueCell);
            };

            string fullName = $"{student.FirstName} {student.MiddleName} {student.LastName}".Replace("  ", " ").Trim();

            addInfoRow("Student ID:", student.StudentNumber);
            addInfoRow("Full Name:", fullName);
            addInfoRow("First Name:", student.FirstName);
            addInfoRow("Middle Name:", student.MiddleName ?? "");
            addInfoRow("Last Name:", student.LastName);
            addInfoRow("Program/Course:", student.Program);
            addInfoRow("Year Level:", student.YearLevel.ToString());
            addInfoRow("Email:", student.Email ?? "");
            addInfoRow("Phone:", student.Phone ?? "");
            addInfoRow("Home Address:", student.Address ?? "");
            addInfoRow("Status:", student.Status);
            addInfoRow("Enrollment Date:", student.EnrollmentDate.ToString("MMMM dd, yyyy"));
            addInfoRow("Created At:", student.CreatedAt.ToString("MMMM dd, yyyy HH:mm:ss"));

            document.Add(infoTable);

            // Scan History Section
            Paragraph scanHistoryTitle = new Paragraph("Attendance Scan History", sectionFont);
            scanHistoryTitle.SpacingBefore = 20;
            scanHistoryTitle.SpacingAfter = 10;
            document.Add(scanHistoryTitle);

            if (scanHistory != null && scanHistory.Count > 0)
            {
                // Scan History Table
                PdfPTable scanTable = new PdfPTable(7);
                scanTable.WidthPercentage = 100;
                scanTable.SetWidths(new float[] { 1.2f, 1f, 1f, 1f, 1.2f, 1f, 1.5f });
                scanTable.SpacingAfter = 20;

                // Header
                Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9, BaseColor.WHITE);
                string[] headers = { "Date", "Time In", "Time Out", "Type", "Location", "Status", "Purpose" };

                foreach (string header in headers)
                {
                    PdfPCell headerCell = new PdfPCell(new Phrase(header, headerFont));
                    headerCell.BackgroundColor = new BaseColor(41, 128, 185);
                    headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    headerCell.Padding = 8;
                    scanTable.AddCell(headerCell);
                }

                // Data rows
                Font dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 8, BaseColor.BLACK);
                foreach (var scan in scanHistory.OrderByDescending(s => s.ScanDateTime))
                {
                    scanTable.AddCell(new PdfPCell(new Phrase(scan.ScanDateTime.ToString("MM/dd/yyyy"), dataFont)) { Padding = 5 });
                    scanTable.AddCell(new PdfPCell(new Phrase(scan.ScanDateTime.ToString("HH:mm:ss"), dataFont)) { Padding = 5 });
                    scanTable.AddCell(new PdfPCell(new Phrase(scan.TimeOut?.ToString("HH:mm:ss") ?? "-", dataFont)) { Padding = 5 });
                    scanTable.AddCell(new PdfPCell(new Phrase(scan.ScanType ?? "QR", dataFont)) { Padding = 5 });
                    scanTable.AddCell(new PdfPCell(new Phrase(scan.Location ?? "-", dataFont)) { Padding = 5 });
                    scanTable.AddCell(new PdfPCell(new Phrase(scan.AttendanceStatus ?? "-", dataFont)) { Padding = 5 });
                    scanTable.AddCell(new PdfPCell(new Phrase(scan.ScanPurpose ?? "-", dataFont)) { Padding = 5 });
                }

                document.Add(scanTable);
            }
            else
            {
                Paragraph noData = new Paragraph("No scan history available.", valueFont);
                noData.SpacingAfter = 20;
                document.Add(noData);
            }

            // Footer
            Paragraph footer = new Paragraph($"Generated on: {DateTime.Now:MMMM dd, yyyy HH:mm:ss} | Total Scans: {scanHistory?.Count ?? 0}",
                FontFactory.GetFont(FontFactory.HELVETICA, 8, BaseColor.GRAY));
            footer.Alignment = Element.ALIGN_CENTER;
            footer.SpacingBefore = 20;
            document.Add(footer);

            document.Close();
            writer.Close();
        }

        private static void ExportStudentToJson(string filePath, Student student, List<ScanHistory> scanHistory)
        {
            var exportData = new
            {
                ExportDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                ExportType = "Student Record",
                Student = new
                {
                    StudentId = student.StudentId,
                    StudentNumber = student.StudentNumber,
                    FullName = $"{student.FirstName} {student.MiddleName} {student.LastName}".Replace("  ", " ").Trim(),
                    FirstName = student.FirstName,
                    MiddleName = student.MiddleName,
                    LastName = student.LastName,
                    Program = student.Program,
                    YearLevel = student.YearLevel,
                    Email = student.Email,
                    Phone = student.Phone,
                    Address = student.Address,
                    Status = student.Status,
                    EnrollmentDate = student.EnrollmentDate.ToString("yyyy-MM-dd"),
                    CreatedAt = student.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                    UpdatedAt = student.UpdatedAt?.ToString("yyyy-MM-dd HH:mm:ss")
                },
                ScanHistory = scanHistory?.OrderByDescending(s => s.ScanDateTime).Select(scan => new
                {
                    ScanId = scan.ScanId,
                    Date = scan.ScanDateTime.ToString("yyyy-MM-dd"),
                    TimeIn = scan.ScanDateTime.ToString("HH:mm:ss"),
                    TimeOut = scan.TimeOut?.ToString("HH:mm:ss"),
                    ScanType = scan.ScanType,
                    Location = scan.Location,
                    Status = scan.AttendanceStatus,
                    Purpose = scan.ScanPurpose,
                    Notes = scan.Notes,
                    DeviceId = scan.DeviceId,
                    CreatedAt = scan.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
                }).ToList(),
                Summary = new
                {
                    TotalScans = scanHistory?.Count ?? 0
                }
            };

            string json = JsonConvert.SerializeObject(exportData, Formatting.Indented);
            File.WriteAllText(filePath, json, Encoding.UTF8);
        }

        private static void ExportStudentToCsv(string filePath, Student student, List<ScanHistory> scanHistory)
        {
            using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                // Write Student Information Section
                writer.WriteLine("STUDENT INFORMATION");
                writer.WriteLine("===================");
                writer.WriteLine();

                string fullName = $"{student.FirstName} {student.MiddleName} {student.LastName}".Replace("  ", " ").Trim();

                writer.WriteLine($"Student ID,{EscapeCsvField(student.StudentNumber)}");
                writer.WriteLine($"Full Name,{EscapeCsvField(fullName)}");
                writer.WriteLine($"First Name,{EscapeCsvField(student.FirstName)}");
                writer.WriteLine($"Middle Name,{EscapeCsvField(student.MiddleName ?? "")}");
                writer.WriteLine($"Last Name,{EscapeCsvField(student.LastName)}");
                writer.WriteLine($"Program/Course,{EscapeCsvField(student.Program)}");
                writer.WriteLine($"Year Level,{student.YearLevel}");
                writer.WriteLine($"Email,{EscapeCsvField(student.Email ?? "")}");
                writer.WriteLine($"Phone,{EscapeCsvField(student.Phone ?? "")}");
                writer.WriteLine($"Home Address,{EscapeCsvField(student.Address ?? "")}");
                writer.WriteLine($"Status,{EscapeCsvField(student.Status)}");
                writer.WriteLine($"Enrollment Date,{student.EnrollmentDate:yyyy-MM-dd}");
                writer.WriteLine($"Created At,{student.CreatedAt:yyyy-MM-dd HH:mm:ss}");
                writer.WriteLine();
                writer.WriteLine();

                // Write Scan History Section
                writer.WriteLine("SCAN HISTORY");
                writer.WriteLine("============");
                writer.WriteLine();
                writer.WriteLine("Date,Time In,Time Out,Scan Type,Location,Status,Purpose,Notes");

                if (scanHistory != null && scanHistory.Count > 0)
                {
                    foreach (var scan in scanHistory.OrderByDescending(s => s.ScanDateTime))
                    {
                        string date = scan.ScanDateTime.ToString("yyyy-MM-dd");
                        string timeIn = scan.ScanDateTime.ToString("HH:mm:ss");
                        string timeOut = scan.TimeOut?.ToString("HH:mm:ss") ?? "";
                        string scanType = EscapeCsvField(scan.ScanType ?? "QR Code");
                        string location = EscapeCsvField(scan.Location ?? "");
                        string status = EscapeCsvField(scan.AttendanceStatus ?? "");
                        string purpose = EscapeCsvField(scan.ScanPurpose ?? "");
                        string notes = EscapeCsvField(scan.Notes ?? "");

                        writer.WriteLine($"{date},{timeIn},{timeOut},{scanType},{location},{status},{purpose},{notes}");
                    }
                }
                else
                {
                    writer.WriteLine("No scan history available");
                }

                writer.WriteLine();
                writer.WriteLine();
                writer.WriteLine($"Export Date,{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                writer.WriteLine($"Total Scan Records,{scanHistory?.Count ?? 0}");
            }
        }

        #endregion

        #region Scan History Export

        /// <summary>
        /// Export scan history to specified format
        /// </summary>
        public static void ExportScanHistory(string filePath, List<ScanHistory> scanHistory, ExportFormat format)
        {
            switch (format)
            {
                case ExportFormat.PDF:
                    ExportScanHistoryToPdf(filePath, scanHistory);
                    break;
                case ExportFormat.JSON:
                    ExportScanHistoryToJson(filePath, scanHistory);
                    break;
                case ExportFormat.CSV:
                    ExportScanHistoryToCsv(filePath, scanHistory);
                    break;
                default:
                    throw new ArgumentException("Unsupported export format");
            }
        }

        private static void ExportScanHistoryToPdf(string filePath, List<ScanHistory> scanHistory)
        {
            Document document = new Document(PageSize.A4.Rotate(), 30, 30, 40, 40);
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));

            document.Open();

            // Title
            Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, BaseColor.DARK_GRAY);
            Paragraph title = new Paragraph("ATTENDANCE SCAN HISTORY REPORT", titleFont);
            title.Alignment = Element.ALIGN_CENTER;
            title.SpacingAfter = 20;
            document.Add(title);

            // Summary
            Font summaryFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.BLACK);
            Paragraph summary = new Paragraph($"Total Records: {scanHistory.Count} | Generated: {DateTime.Now:MMMM dd, yyyy HH:mm:ss}", summaryFont);
            summary.Alignment = Element.ALIGN_CENTER;
            summary.SpacingAfter = 15;
            document.Add(summary);

            if (scanHistory != null && scanHistory.Count > 0)
            {
                // Scan History Table
                PdfPTable table = new PdfPTable(9);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 1f, 1.5f, 1.2f, 1f, 1f, 1f, 1.2f, 1f, 1.5f });

                // Header
                Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9, BaseColor.WHITE);
                string[] headers = { "ID", "Student", "Number", "Date", "Time In", "Time Out", "Type", "Status", "Location" };

                foreach (string header in headers)
                {
                    PdfPCell headerCell = new PdfPCell(new Phrase(header, headerFont));
                    headerCell.BackgroundColor = new BaseColor(52, 73, 94);
                    headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    headerCell.Padding = 6;
                    table.AddCell(headerCell);
                }

                // Data rows
                Font dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 7, BaseColor.BLACK);
                foreach (var scan in scanHistory.OrderByDescending(s => s.ScanDateTime))
                {
                    table.AddCell(new PdfPCell(new Phrase(scan.ScanId.ToString(), dataFont)) { Padding = 4, HorizontalAlignment = Element.ALIGN_CENTER });
                    table.AddCell(new PdfPCell(new Phrase(scan.StudentName ?? "-", dataFont)) { Padding = 4 });
                    table.AddCell(new PdfPCell(new Phrase(scan.StudentNumber ?? "-", dataFont)) { Padding = 4 });
                    table.AddCell(new PdfPCell(new Phrase(scan.ScanDateTime.ToString("MM/dd/yyyy"), dataFont)) { Padding = 4 });
                    table.AddCell(new PdfPCell(new Phrase(scan.ScanDateTime.ToString("HH:mm:ss"), dataFont)) { Padding = 4 });
                    table.AddCell(new PdfPCell(new Phrase(scan.TimeOut?.ToString("HH:mm:ss") ?? "-", dataFont)) { Padding = 4 });
                    table.AddCell(new PdfPCell(new Phrase(scan.ScanType ?? "QR", dataFont)) { Padding = 4 });
                    table.AddCell(new PdfPCell(new Phrase(scan.AttendanceStatus ?? "-", dataFont)) { Padding = 4 });
                    table.AddCell(new PdfPCell(new Phrase(scan.Location ?? "-", dataFont)) { Padding = 4 });
                }

                document.Add(table);
            }
            else
            {
                Paragraph noData = new Paragraph("No scan history available.",
                    FontFactory.GetFont(FontFactory.HELVETICA, 12, BaseColor.GRAY));
                noData.Alignment = Element.ALIGN_CENTER;
                document.Add(noData);
            }

            document.Close();
            writer.Close();
        }

        private static void ExportScanHistoryToJson(string filePath, List<ScanHistory> scanHistory)
        {
            var exportData = new
            {
                ExportDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                ExportType = "Scan History",
                TotalRecords = scanHistory.Count,
                ScanHistory = scanHistory.OrderByDescending(s => s.ScanDateTime).Select(scan => new
                {
                    ScanId = scan.ScanId,
                    StudentId = scan.StudentId,
                    StudentNumber = scan.StudentNumber,
                    StudentName = scan.StudentName,
                    Program = scan.Program,
                    Date = scan.ScanDateTime.ToString("yyyy-MM-dd"),
                    TimeIn = scan.ScanDateTime.ToString("HH:mm:ss"),
                    TimeOut = scan.TimeOut?.ToString("HH:mm:ss"),
                    ScanType = scan.ScanType,
                    Location = scan.Location,
                    AttendanceStatus = scan.AttendanceStatus,
                    Purpose = scan.ScanPurpose,
                    Notes = scan.Notes,
                    DeviceId = scan.DeviceId,
                    CreatedAt = scan.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
                }).ToList()
            };

            string json = JsonConvert.SerializeObject(exportData, Formatting.Indented);
            File.WriteAllText(filePath, json, Encoding.UTF8);
        }

        private static void ExportScanHistoryToCsv(string filePath, List<ScanHistory> scanHistory)
        {
            using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                // Write header
                writer.WriteLine("ATTENDANCE SCAN HISTORY REPORT");
                writer.WriteLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                writer.WriteLine($"Total Records: {scanHistory.Count}");
                writer.WriteLine();

                // Column headers
                writer.WriteLine("Scan ID,Student ID,Student Number,Student Name,Program,Date,Time In,Time Out,Scan Type,Location,Status,Purpose,Notes");

                // Data rows
                if (scanHistory != null && scanHistory.Count > 0)
                {
                    foreach (var scan in scanHistory.OrderByDescending(s => s.ScanDateTime))
                    {
                        writer.WriteLine(string.Join(",",
                            scan.ScanId,
                            scan.StudentId,
                            EscapeCsvField(scan.StudentNumber ?? ""),
                            EscapeCsvField(scan.StudentName ?? ""),
                            EscapeCsvField(scan.Program ?? ""),
                            scan.ScanDateTime.ToString("yyyy-MM-dd"),
                            scan.ScanDateTime.ToString("HH:mm:ss"),
                            scan.TimeOut?.ToString("HH:mm:ss") ?? "",
                            EscapeCsvField(scan.ScanType ?? "QR Code"),
                            EscapeCsvField(scan.Location ?? ""),
                            EscapeCsvField(scan.AttendanceStatus ?? ""),
                            EscapeCsvField(scan.ScanPurpose ?? ""),
                            EscapeCsvField(scan.Notes ?? "")
                        ));
                    }
                }
            }
        }

        #endregion

        #region Helper Methods

        private static string EscapeCsvField(string field)
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

        #endregion
    }

    public enum ExportFormat
    {
        PDF,
        JSON,
        CSV
    }
}

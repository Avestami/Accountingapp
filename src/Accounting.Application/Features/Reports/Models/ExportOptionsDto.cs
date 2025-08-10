using System;
using System.Collections.Generic;
using System.Linq;

namespace Accounting.Application.Features.Reports.Models
{
    public class ExportOptionsDto
    {
        public string Format { get; set; } = "excel"; // pdf, excel, csv, json
        public string FileName { get; set; } = "report";
        public string Title { get; set; } = "Report";
        public string? Subtitle { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyLogo { get; set; }
        
        // PDF Options
        public string PdfPageSize { get; set; } = "A4"; // A4, A3, Letter
        public string PdfOrientation { get; set; } = "Portrait"; // Portrait, Landscape
        public bool IncludeHeader { get; set; } = true;
        public bool IncludeFooter { get; set; } = true;
        public bool IncludePageNumbers { get; set; } = true;
        public string? HeaderText { get; set; }
        public string? FooterText { get; set; }
        
        // Excel Options
        public string WorksheetName { get; set; } = "Report";
        public bool AutoFitColumns { get; set; } = true;
        public bool IncludeFilters { get; set; } = true;
        public bool FreezePanes { get; set; } = true;
        public string? ExcelTemplate { get; set; }
        
        // Formatting Options
        public string DateFormat { get; set; } = "yyyy-MM-dd";
        public string NumberFormat { get; set; } = "#,##0.00";
        public string CurrencyFormat { get; set; } = "#,##0.00";
        public bool UseRtlLayout { get; set; } = true;
        public string Language { get; set; } = "fa";
        
        // Content Options
        public bool IncludeSummary { get; set; } = true;
        public bool IncludeDetails { get; set; } = true;
        public bool IncludeCharts { get; set; } = false;
        public bool IncludeMetadata { get; set; } = true;
        public List<string>? ColumnsToInclude { get; set; }
        public List<string>? ColumnsToExclude { get; set; }
        
        // Security Options
        public bool PasswordProtect { get; set; } = false;
        public string? Password { get; set; }
        public bool AllowPrinting { get; set; } = true;
        public bool AllowCopying { get; set; } = true;
        public bool AllowEditing { get; set; } = false;
        
        // Compression
        public bool CompressOutput { get; set; } = false;
        public int CompressionLevel { get; set; } = 6; // 1-9
        
        // Watermark
        public string? WatermarkText { get; set; }
        public string? WatermarkImage { get; set; }
        public double WatermarkOpacity { get; set; } = 0.3;
        
        public void Validate()
        {
            var validFormats = new[] { "pdf", "excel", "csv", "json" };
            if (!validFormats.Contains(Format.ToLower()))
                throw new ArgumentException($"Invalid format. Supported formats: {string.Join(", ", validFormats)}");
                
            if (string.IsNullOrWhiteSpace(FileName))
                FileName = "report";
                
            if (CompressionLevel < 1 || CompressionLevel > 9)
                CompressionLevel = 6;
                
            if (WatermarkOpacity < 0 || WatermarkOpacity > 1)
                WatermarkOpacity = 0.3;
        }
    }
}
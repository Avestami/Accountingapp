using System;
using System.Collections.Generic;

namespace Accounting.Domain.Entities
{
    public class Report : BaseEntity
    {
        public string ReportName { get; set; }
        public string Description { get; set; }
        public ReportType Type { get; set; }
        public string Parameters { get; set; }
        public DateTime GeneratedDate { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public ReportFormat Format { get; set; }
        public long FileSizeBytes { get; set; }
        public ReportStatus Status { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime? ExpiryDate { get; set; }
        
        // Date Range for Report Data
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        
        // Foreign Keys
        public int GeneratedByUserId { get; set; }
        
        // Navigation Properties
        public virtual User GeneratedByUser { get; set; }
        
        public Report()
        {
            GeneratedDate = DateTime.UtcNow;
            Status = ReportStatus.Pending;
            Format = ReportFormat.Pdf;
        }
        
        public void MarkAsCompleted(string filePath, string fileName, long fileSizeBytes)
        {
            Status = ReportStatus.Completed;
            FilePath = filePath;
            FileName = fileName;
            FileSizeBytes = fileSizeBytes;
            ErrorMessage = null;
        }
        
        public void MarkAsFailed(string errorMessage)
        {
            Status = ReportStatus.Failed;
            ErrorMessage = errorMessage;
        }
        
        public bool IsExpired()
        {
            return ExpiryDate.HasValue && DateTime.UtcNow > ExpiryDate.Value;
        }
        
        public void SetExpiry(TimeSpan duration)
        {
            ExpiryDate = DateTime.UtcNow.Add(duration);
        }
    }
    
    public enum ReportType
    {
        TicketSummary = 1,
        FinancialStatement = 2,
        VoucherListing = 3,
        FxPositions = 4,
        AuditTrail = 5,
        UserActivity = 6,
        AccountBalance = 7,
        Custom = 99
    }
    
    public enum ReportFormat
    {
        Pdf = 1,
        Excel = 2,
        Csv = 3,
        Json = 4
    }
    
    public enum ReportStatus
    {
        Pending = 1,
        Processing = 2,
        Completed = 3,
        Failed = 4,
        Expired = 5
    }
}
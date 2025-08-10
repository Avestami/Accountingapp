using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Accounting.Application.Features.Reports.Models
{
    public class ReportFilterDto
    {
        [Required]
        public DateTime DateFrom { get; set; }
        
        [Required]
        public DateTime DateTo { get; set; }
        
        [Required]
        public DateTime StartDate { get; set; }
        
        [Required]
        public DateTime EndDate { get; set; }
        
        public string? Currency { get; set; }
        
        public string? ReportType { get; set; }
        
        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        
        // Search and Filtering
        public string? SearchTerm { get; set; }
        public List<string>? Categories { get; set; }
        public List<string>? Counterparties { get; set; }
        public List<string>? Airlines { get; set; }
        public List<string>? Destinations { get; set; }
        public List<string>? AccountCodes { get; set; }
        
        // Status Filters
        public List<string>? Statuses { get; set; }
        public bool? IsActive { get; set; }
        
        // Amount Filters
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        
        // Sorting
        public string? SortBy { get; set; }
        public string? SortDirection { get; set; } = "asc";
        
        // Grouping
        public string? GroupBy { get; set; }
        
        // Export Options
        public string? ExportFormat { get; set; }
        public bool IncludeDetails { get; set; } = true;
        public bool IncludeSummary { get; set; } = true;
        
        // Date Grouping
        public string? DateGrouping { get; set; } // daily, weekly, monthly, yearly
        
        // Comparison
        public DateTime? ComparisonStartDate { get; set; }
        public DateTime? ComparisonEndDate { get; set; }
        
        public void Validate()
        {
            if (StartDate > EndDate)
                throw new ArgumentException("Start date cannot be greater than end date");
                
            if (PageNumber < 1)
                PageNumber = 1;
                
            if (PageSize < 1 || PageSize > 1000)
                PageSize = 50;
                
            if (ComparisonStartDate.HasValue && ComparisonEndDate.HasValue && ComparisonStartDate > ComparisonEndDate)
                throw new ArgumentException("Comparison start date cannot be greater than comparison end date");
        }
    }
}
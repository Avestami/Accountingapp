using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Accounting.Domain.Enums;

namespace Accounting.Application.DTOs
{
    public class VoucherDto
    {
        public int Id { get; set; }
        public string VoucherNumber { get; set; }
        public VoucherType Type { get; set; }
        public string TypeName { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Reference { get; set; }
        public VoucherStatus Status { get; set; }
        public string StatusName { get; set; }
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
        public bool IsBalanced { get; set; }
        public bool IsPosted { get; set; }
        public DateTime? PostedDate { get; set; }
        public int? PostedByUserId { get; set; }
        public string PostedByUserName { get; set; }
        public int? TicketId { get; set; }
        public string TicketNumber { get; set; }
        public int CreatedByUserId { get; set; }
        public string CreatedByUserName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<VoucherEntryDto> Entries { get; set; } = new();
        public List<VoucherAttachmentDto> Attachments { get; set; } = new();
        
        public decimal GetDebitTotal()
        {
            return Entries?.Where(e => e.TransactionType == TransactionType.Debit).Sum(e => e.Amount) ?? 0;
        }
        
        public decimal GetCreditTotal()
        {
            return Entries?.Where(e => e.TransactionType == TransactionType.Credit).Sum(e => e.Amount) ?? 0;
        }
    }

    public class CreateVoucherDto
    {
        [Required]
        public VoucherType Type { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(100)]
        public string Reference { get; set; }

        public int? TicketId { get; set; }

        [Required]
        [MinLength(2, ErrorMessage = "At least 2 entries are required for double-entry bookkeeping")]
        public List<CreateVoucherEntryDto> Entries { get; set; } = new();
    }

    public class UpdateVoucherDto
    {
        [Required]
        public VoucherType Type { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(100)]
        public string Reference { get; set; }

        public int? TicketId { get; set; }

        [Required]
        [MinLength(2, ErrorMessage = "At least 2 entries are required for double-entry bookkeeping")]
        public List<UpdateVoucherEntryDto> Entries { get; set; } = new();
    }

    public class VoucherEntryDto
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public string AccountType { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public string Description { get; set; }
        public string Currency { get; set; }
        public decimal? ExchangeRate { get; set; }
        public decimal? ForeignDebitAmount { get; set; }
        public decimal? ForeignCreditAmount { get; set; }
        public int? CostCenterId { get; set; }
        public string CostCenterName { get; set; }
        public int? ProjectId { get; set; }
        public string ProjectName { get; set; }
        public int SortOrder { get; set; }
        public TransactionType TransactionType { get; set; }
        public decimal Amount { get; set; }
        public int VoucherId { get; set; }
    }

    public class CreateVoucherEntryDto
    {
        [Required]
        public int AccountId { get; set; }

        [Range(0, double.MaxValue)]
        public decimal DebitAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal CreditAmount { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(3)]
        public string Currency { get; set; } = "USD";

        [Range(0.000001, double.MaxValue)]
        public decimal? ExchangeRate { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? ForeignDebitAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? ForeignCreditAmount { get; set; }

        public int? CostCenterId { get; set; }

        public int? ProjectId { get; set; }

        public int SortOrder { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public TransactionType TransactionType { get; set; }
    }

    public class UpdateVoucherEntryDto
    {
        public int? Id { get; set; }

        [Required]
        public int AccountId { get; set; }

        [Range(0, double.MaxValue)]
        public decimal DebitAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal CreditAmount { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(3)]
        public string Currency { get; set; } = "USD";

        [Range(0.000001, double.MaxValue)]
        public decimal? ExchangeRate { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? ForeignDebitAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? ForeignCreditAmount { get; set; }

        public int? CostCenterId { get; set; }

        public int? ProjectId { get; set; }

        public int SortOrder { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public TransactionType TransactionType { get; set; }
    }

    public class PostVoucherDto
    {
        [StringLength(500)]
        public string Notes { get; set; }
    }

    public class VoucherAttachmentDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string OriginalFileName { get; set; }
        public string ContentType { get; set; }
        public long FileSize { get; set; }
        public string FilePath { get; set; }
        public DateTime UploadedAt { get; set; }
        public string UploadedBy { get; set; }
    }

    public class VoucherSummaryDto
    {
        public int Id { get; set; }
        public string VoucherNumber { get; set; }
        public VoucherType Type { get; set; }
        public string TypeName { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public VoucherStatus Status { get; set; }
        public string StatusName { get; set; }
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
        public bool IsBalanced { get; set; }
        public int EntriesCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
    }

    public class VoucherSearchDto
    {
        public string VoucherNumber { get; set; }
        public VoucherType? Type { get; set; }
        public VoucherStatus? Status { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string Description { get; set; }
        public string Reference { get; set; }
        public int? AccountId { get; set; }
        public decimal? AmountFrom { get; set; }
        public decimal? AmountTo { get; set; }
        public string CreatedBy { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string SortBy { get; set; } = "Date";
        public string SortDirection { get; set; } = "DESC";
    }

    public class VoucherValidationResultDto
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
        public decimal Difference { get; set; }
        public bool IsBalanced { get; set; }
    }
}
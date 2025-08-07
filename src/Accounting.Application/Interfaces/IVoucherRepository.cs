using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Accounting.Domain.Entities;
using Accounting.Domain.Enums;

namespace Accounting.Application.Interfaces
{
    public interface IVoucherRepository : IRepository<Voucher>
    {
        /// <summary>
        /// Get voucher with all related entities (entries, accounts, etc.)
        /// </summary>
        Task<Voucher> GetVoucherWithDetailsAsync(int id);

        /// <summary>
        /// Get vouchers with pagination and filtering
        /// </summary>
        Task<(IEnumerable<Voucher> Items, int TotalCount)> GetVouchersAsync(
            int pageNumber,
            int pageSize,
            string searchTerm = null,
            VoucherType? type = null,
            VoucherStatus? status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string currency = null,
            int? ticketId = null,
            int? createdByUserId = null,
            string sortBy = null,
            bool sortDescending = false);

        /// <summary>
        /// Get next voucher number for a specific type
        /// </summary>
        Task<string> GetNextVoucherNumberAsync(VoucherType type);

        /// <summary>
        /// Check if voucher number exists
        /// </summary>
        Task<bool> VoucherNumberExistsAsync(string voucherNumber);

        /// <summary>
        /// Get vouchers by ticket ID
        /// </summary>
        Task<IEnumerable<Voucher>> GetVouchersByTicketIdAsync(int ticketId);

        /// <summary>
        /// Get vouchers by status
        /// </summary>
        Task<IEnumerable<Voucher>> GetVouchersByStatusAsync(VoucherStatus status);

        /// <summary>
        /// Get vouchers created by user
        /// </summary>
        Task<IEnumerable<Voucher>> GetVouchersByUserAsync(int userId);

        /// <summary>
        /// Get vouchers pending approval
        /// </summary>
        Task<IEnumerable<Voucher>> GetPendingVouchersAsync();

        /// <summary>
        /// Get posted vouchers for a date range
        /// </summary>
        Task<IEnumerable<Voucher>> GetPostedVouchersAsync(DateTime fromDate, DateTime toDate);

        /// <summary>
        /// Save changes to the database
        /// </summary>
        Task<int> SaveChangesAsync();
    }
}
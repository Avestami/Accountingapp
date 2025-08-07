using System.Collections.Generic;

namespace Accounting.Domain.Entities
{
    public class Account : BaseEntity
    {
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public string Description { get; set; }
        public AccountType Type { get; set; }
        public bool IsActive { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; }
        
        // Hierarchy
        public int? ParentAccountId { get; set; }
        public virtual Account ParentAccount { get; set; }
        public virtual ICollection<Account> ChildAccounts { get; set; }
        
        // Navigation Properties
        public virtual ICollection<VoucherEntry> VoucherEntries { get; set; }
        public virtual ICollection<FxTransaction> FxTransactions { get; set; }
        
        public Account()
        {
            ChildAccounts = new HashSet<Account>();
            VoucherEntries = new HashSet<VoucherEntry>();
            FxTransactions = new HashSet<FxTransaction>();
            IsActive = true;
            Balance = 0;
            Currency = "USD";
        }
        
        public string GetFullAccountCode()
        {
            if (ParentAccount != null)
                return $"{ParentAccount.GetFullAccountCode()}.{AccountCode}";
            return AccountCode;
        }
        
        public bool IsLeafAccount() => ChildAccounts?.Count == 0;
    }
    
    public enum AccountType
    {
        Asset = 1,
        Liability = 2,
        Equity = 3,
        Revenue = 4,
        Expense = 5
    }
}
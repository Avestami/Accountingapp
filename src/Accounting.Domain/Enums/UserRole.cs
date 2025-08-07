namespace Accounting.Domain.Enums
{
    public enum UserRole
    {
        Admin = 1,
        Finance = 2,
        Sales = 3,
        User = 4
    }
    
    public enum TicketStatus
    {
        Draft = 1,
        Pending = 2,
        Approved = 3,
        Rejected = 4,
        Completed = 5,
        Cancelled = 6
    }
    
    public enum TicketType
    {
        Travel = 1,
        Accommodation = 2,
        Meal = 3,
        Transportation = 4,
        Other = 5
    }
    
    public enum VoucherType
    {
        Income = 1,
        Expense = 2,
        Transfer = 3
    }
    
    public enum TransactionType
    {
        Debit = 1,
        Credit = 2
    }
    
    public enum AuditAction
    {
        Create = 1,
        Update = 2,
        Delete = 3,
        Login = 4,
        Logout = 5,
        Approve = 6,
        Reject = 7
    }
    
    public enum VoucherStatus
    {
        Draft = 1,
        Pending = 2,
        Approved = 3,
        Rejected = 4,
        Posted = 5,
        Cancelled = 6
    }
}
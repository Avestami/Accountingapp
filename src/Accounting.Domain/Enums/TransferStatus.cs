namespace Accounting.Domain.Enums
{
    public enum TransferStatus
    {
        Pending = 0,
        Processing = 1,
        Completed = 2,
        Failed = 3,
        Cancelled = 4
    }
}
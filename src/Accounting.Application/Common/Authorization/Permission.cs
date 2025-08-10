namespace Accounting.Application.Common.Authorization
{
    public enum Permission
    {
        // Tickets
        CreateTicket,
        UpdateTicket,
        DeleteTicket,
        IssueTicket,
        CancelTicket,
        ViewTickets,

        // Finance
        CreateVoucher,
        UpdateVoucher,
        DeleteVoucher,
        ViewFinance,

        // Accounts
        CreateAccount,
        UpdateAccount,
        DeleteAccount,
        ViewAccounts,

        // Reports
        ViewReports,
        ExportReports,

        // Settings
        ViewSettings,
        EditSettings,
        ManageUsers,

        // Admin
        AdminAccess,
        SystemSettings
    }
}
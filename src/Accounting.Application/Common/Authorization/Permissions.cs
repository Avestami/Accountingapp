namespace Accounting.Application.Common.Authorization
{
    public static class Permissions
    {
        // Tickets
        public const string TicketsView = "tickets.view";
        public const string TicketsCreate = "tickets.create";
        public const string TicketsEdit = "tickets.edit";
        public const string TicketsDelete = "tickets.delete";
        public const string TicketsIssue = "tickets.issue";
        public const string TicketsCancel = "tickets.cancel";

        // Finance
        public const string FinanceView = "finance.view";
        public const string FinanceCreate = "finance.create";
        public const string FinanceEdit = "finance.edit";
        public const string FinanceDelete = "finance.delete";
        public const string FinancePost = "finance.post";
        public const string FinanceApprove = "finance.approve";

        // Vouchers
        public const string VouchersView = "vouchers.view";
        public const string VouchersCreate = "vouchers.create";
        public const string VouchersEdit = "vouchers.edit";
        public const string VouchersDelete = "vouchers.delete";
        public const string VouchersPost = "vouchers.post";
        public const string VouchersApprove = "vouchers.approve";

        // Accounts (Chart of Accounts)
        public const string AccountsView = "accounts.view";
        public const string AccountsCreate = "accounts.create";
        public const string AccountsEdit = "accounts.edit";
        public const string AccountsDelete = "accounts.delete";

        // Reports
        public const string ReportsView = "reports.view";
        public const string ReportsExport = "reports.export";

        // Settings
        public const string SettingsView = "settings.view";
        public const string SettingsEdit = "settings.edit";
        public const string UsersManage = "users.manage";

        // Admin
        public const string AdminAccess = "admin.access";
        public const string SystemSettings = "system.settings";
    }
}
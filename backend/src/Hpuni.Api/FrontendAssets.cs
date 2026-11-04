namespace Hpuni.Api;

public static class FrontendAssets
{
    public static readonly HashSet<string> Paths = new(StringComparer.Ordinal)
    {
        "/notifications.mjs", "/report-details.mjs", "/finance-controls.mjs", "/asset-lifecycle.mjs", "/reports.html", "/server-report-portal.mjs",
        "/live.html", "/live.mjs", "/live.css", "/api-client.mjs", "/equipment-live.mjs",
        "/index.html", "/login.html", "/resources.html", "/admin.html", "/executive.html", "/accounting.html", "/payment-voucher.html",
        "/styles.css", "/login.css", "/resources.css", "/admin.css", "/executive.css", "/accounting.css", "/account-menu.css", "/payment-voucher.css",
        "/page-entry.mjs", "/account-entry.mjs", "/server-login.mjs", "/server-session.mjs", "/server-resources.mjs", "/server-admin.mjs", "/server-finance.mjs", "/server-voucher.mjs", "/server-reports.mjs", "/voucher-files.mjs",
        "/app.js", "/resources.js", "/admin.js", "/executive.js", "/accounting.js", "/payment-voucher.js", "/login.js", "/account-menu.js", "/admin-password.js",
        "/availability.mjs", "/resource-store.mjs", "/resource-seed.mjs", "/admin-data.mjs", "/demo-auth.mjs", "/executive-data.mjs", "/accounting-data.mjs", "/maintenance-finance.mjs"
    };
}

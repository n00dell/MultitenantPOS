using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ApplicationBuilder;
using DevExpress.ExpressApp.Blazor;
using DevExpress.ExpressApp.MultiTenancy;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.ClientServer;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl.MultiTenancy;
using Microsoft.Extensions.DependencyInjection;
using MultitenantPOS.Blazor.Server.Services;
using MultitenantPOS.Module.BusinessObjects;

namespace MultitenantPOS.Blazor.Server;

public class MultitenantPOSBlazorApplication : BlazorApplication {
    public MultitenantPOSBlazorApplication() {
        ApplicationName = "POS";
        CheckCompatibilityType = DevExpress.ExpressApp.CheckCompatibilityType.DatabaseSchema;
        DatabaseVersionMismatch += MultitenantPOSBlazorApplication_DatabaseVersionMismatch;
    }
    protected override void OnSetupStarted() {
        base.OnSetupStarted();
#if DEBUG
        if(System.Diagnostics.Debugger.IsAttached && CheckCompatibilityType == CheckCompatibilityType.DatabaseSchema) {
            DatabaseUpdateMode = DatabaseUpdateMode.UpdateDatabaseAlways;
        }
#endif
    }
    private void MultitenantPOSBlazorApplication_DatabaseVersionMismatch(object sender, DatabaseVersionMismatchEventArgs e) {
#if EASYTEST
        e.Updater.Update();
        e.Handled = true;
#else
        if(System.Diagnostics.Debugger.IsAttached || TenantId != null) {
            e.Updater.Update();
            e.Handled = true;
        }
        else {
            string message = "The application cannot connect to the specified database, " +
                "because the database doesn't exist, its version is older " +
                "than that of the application or its schema does not match " +
                "the ORM data model structure. To avoid this error, use one " +
                "of the solutions from the https://www.devexpress.com/kb=T367835 KB Article.";

            if(e.CompatibilityError != null && e.CompatibilityError.Exception != null) {
                message += "\r\n\r\nInner exception: " + e.CompatibilityError.Exception.Message;
            }
            throw new InvalidOperationException(message);
        }
#endif
    }
    protected override void OnLoggedOn(LogonEventArgs args)
    {
        base.OnLoggedOn(args);

        // Get the current tenant name via the ITenantProvider service
        var tenantProvider = ServiceProvider.GetRequiredService<ITenantProvider>();
        string tenantName = tenantProvider.TenantName;

        if (!string.IsNullOrEmpty(tenantName))
        {
            // Set the application title dynamically (updates browser tab and other UI elements)
            this.Title = $"{tenantName} POS";
        }
    }
    private string GetCurrentCompanyName()
    {
        try
        {
            var tenantProvider = ServiceProvider?.GetService<ITenantProvider>();
            if (tenantProvider?.TenantId == null) return "POS";

            using (var objectSpace = CreateObjectSpace())
            {
                var tenant = objectSpace.GetObjectByKey<Tenant>(tenantProvider.TenantId);
                return tenant?.Name ?? "POS";
            }
        }
        catch
        {
            return "POS"; // Fallback
        }
    }
    Guid? TenantId {
        get {
            return ServiceProvider?.GetService<ITenantProvider>()?.TenantId;
        }
    }
}

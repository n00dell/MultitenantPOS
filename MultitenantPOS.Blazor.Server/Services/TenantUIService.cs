using DevExpress.ExpressApp.MultiTenancy;
using Microsoft.AspNetCore.Http;
using DevExpress.ExpressApp;
using MultitenantPOS.Module.BusinessObjects;

namespace MultitenantPOS.Blazor.Server.Services
{
    
    public class TenantUIService : ITenantUIService
    {
        private const string DefaultCaption = "MultitenantPOS";
        private const string DefaultLogoUrl = "/images/Logo.svg";
        private readonly IObjectSpaceFactory _objectSpaceFactory;

        public TenantUIService(IObjectSpaceFactory objectSpaceFactory)
        {
            _objectSpaceFactory = objectSpaceFactory;
        }

        public string GetSplashCaption(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                return DefaultCaption;
            }

            try
            {
                // Read current tenant via XAF multi-tenancy provider if available
                var tenantProvider = httpContext.RequestServices.GetService(typeof(ITenantProvider)) as ITenantProvider;
                var tenantName = tenantProvider?.TenantName;

                if (!string.IsNullOrWhiteSpace(tenantName))
                {
                    // You might want to customize the display name based on tenant
                    // For example, remove .com from company1.com
                    var displayName = tenantName.Replace(".com", "").Replace(".", " ");
                    return char.ToUpper(displayName[0]) + displayName.Substring(1);
                }

                // Fallback: use subdomain as tentative tenant name (e.g., acme.app.com -> acme)
                var host = httpContext.Request.Host.Host;
                if (!string.IsNullOrWhiteSpace(host))
                {
                    var parts = host.Split('.');
                    if (parts.Length > 2)
                    {
                        return char.ToUpper(parts[0][0]) + parts[0].Substring(1);
                    }
                    return host;
                }
            }
            catch (Exception)
            {
                // In case of any error, return default
            }

            return DefaultCaption;
        }

        public string GetLogoUrl(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                return DefaultLogoUrl;
            }

            try
            {
                var tenantProvider = httpContext.RequestServices.GetService(typeof(ITenantProvider)) as ITenantProvider;
                if (tenantProvider?.TenantId != null)
                {
                    return "/tenant-assets/logo";
                }
            }
            catch (Exception)
            {
                // In case of any error, return default
            }

            return DefaultLogoUrl;
        }

        public bool HasTenantLogo(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                return false;
            }

            try
            {
                var tenantProvider = httpContext.RequestServices.GetService(typeof(ITenantProvider)) as ITenantProvider;
                var tenantId = tenantProvider?.TenantId;

                if (tenantId != null)
                {
                    using var os = _objectSpaceFactory.CreateObjectSpace(typeof(TenantExtended));
                    var tenant = os.GetObjectByKey<TenantExtended>(tenantId);
                    return tenant?.Logo != null && tenant.Logo.Size > 0;
                }
            }
            catch (Exception)
            {
                // In case of any error, assume no tenant logo
            }

            return false;
        }
    }
}
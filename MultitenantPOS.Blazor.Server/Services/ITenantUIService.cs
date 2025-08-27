using Microsoft.AspNetCore.Http;

namespace MultitenantPOS.Blazor.Server.Services
{
    public interface ITenantUIService
    {
        string GetSplashCaption(HttpContext httpContext);
        string GetLogoUrl(HttpContext httpContext);
        bool HasTenantLogo(HttpContext httpContext);
    }
}




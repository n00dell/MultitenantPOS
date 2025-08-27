using DevExpress.ExpressApp;
using DevExpress.ExpressApp.MultiTenancy;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MultitenantPOS.Module.BusinessObjects;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;

namespace MultitenantPOS.Blazor.Server.Controllers
{
    [ApiController]
    [Route("tenant-assets")]
    public class TenantAssetsController : ControllerBase
    {
        private readonly IObjectSpaceFactory _objectSpaceFactory;
        private readonly ITenantProvider _tenantProvider;
        private readonly ILogger<TenantAssetsController> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly IMemoryCache _cache;

        public TenantAssetsController(IObjectSpaceFactory objectSpaceFactory,
            ITenantProvider tenantProvider,
            ILogger<TenantAssetsController> logger,
            IWebHostEnvironment env,
            IMemoryCache cache)
        {
            _objectSpaceFactory = objectSpaceFactory;
            _tenantProvider = tenantProvider;
            _logger = logger;
            _env = env;
            _cache = cache;
        }

        [HttpGet("logo")]
        public IActionResult GetLogo()
        {
            _logger.LogInformation("Logo request received");

            try
            {
                var tenantId = _tenantProvider.TenantId;
                var tenantName = _tenantProvider.TenantName;

                _logger.LogInformation("Tenant context - ID: {TenantId}, Name: {TenantName}", tenantId, tenantName);

                // Set cache headers manually
                Response.Headers.CacheControl = "public,max-age=300"; // Cache for 5 minutes
                Response.Headers.ETag = $"\"{tenantId}_{tenantName}\"";

                // Create cache key
                var cacheKey = $"tenant_logo_{tenantId}_{tenantName}";

                // Try to get from cache first
                if (_cache.TryGetValue(cacheKey, out CachedLogoData cachedLogo))
                {
                    _logger.LogInformation("Returning cached logo");
                    return File(cachedLogo.Data, cachedLogo.ContentType);
                }

                byte[]? bytes = null;
                string? fileName = null;

                if (tenantId != null)
                {
                    try
                    {
                        using var os = _objectSpaceFactory.CreateObjectSpace(typeof(TenantExtended));
                        var tenant = os.GetObjectByKey<TenantExtended>(tenantId);
                        if (tenant?.Logo != null && tenant.Logo.Size > 0)
                        {
                            using var ms = new MemoryStream();
                            tenant.Logo.SaveToStream(ms);
                            bytes = ms.ToArray();
                            fileName = tenant.Logo.FileName;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to load tenant logo from database for tenant {TenantId}", tenantId);
                    }
                }

                // Fallback to default logo if tenant logo is not available
                if (bytes == null)
                {
                    _logger.LogInformation("No tenant logo found, using fallback");
                    var defaultPath = Path.Combine(_env.WebRootPath, "images", "Logo.svg");
                    if (System.IO.File.Exists(defaultPath))
                    {
                        bytes = System.IO.File.ReadAllBytes(defaultPath);
                        fileName = "Logo.svg";
                        _logger.LogInformation("Loaded default logo from {DefaultPath}", defaultPath);
                    }
                    else
                    {
                        // Last resort - create a simple SVG logo
                        var fallbackSvg = CreateFallbackSvgLogo(tenantName ?? "XAF");
                        bytes = System.Text.Encoding.UTF8.GetBytes(fallbackSvg);
                        fileName = "fallback.svg";
                        _logger.LogInformation("Created fallback SVG logo");
                    }
                }
                else
                {
                    _logger.LogInformation("Loaded tenant logo: {FileName}, Size: {Size} bytes", fileName, bytes.Length);
                }

                var contentType = GetContentType(fileName ?? "logo");

                // Cache the result for 5 minutes
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                    Priority = CacheItemPriority.Normal
                };

                _cache.Set(cacheKey, new CachedLogoData { Data = bytes, ContentType = contentType }, cacheOptions);

                return File(bytes, contentType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load tenant logo");

                // Return a fallback SVG in case of error
                var errorSvg = CreateFallbackSvgLogo("XAF");
                var errorBytes = System.Text.Encoding.UTF8.GetBytes(errorSvg);
                return File(errorBytes, "image/svg+xml");
            }
        }

        private static string GetContentType(string fileName)
        {
            var ext = Path.GetExtension(fileName)?.ToLowerInvariant();
            return ext switch
            {
                ".svg" => "image/svg+xml",
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                _ => "application/octet-stream"
            };
        }

        private static string CreateFallbackSvgLogo(string text)
        {
            return $@"<svg xmlns=""http://www.w3.org/2000/svg"" width=""120"" height=""120"" viewBox=""0 0 120 120"">
                <circle cx=""60"" cy=""60"" r=""50"" fill=""#4a90e2"" stroke=""#ffffff"" stroke-width=""2""/>
                <text x=""60"" y=""70"" font-family=""Arial, sans-serif"" font-size=""24"" font-weight=""bold"" 
                      text-anchor=""middle"" fill=""#ffffff"">{text.Substring(0, Math.Min(3, text.Length)).ToUpper()}</text>
            </svg>";
        }

        private class CachedLogoData
        {
            public byte[] Data { get; set; } = Array.Empty<byte>();
            public string ContentType { get; set; } = string.Empty;
        }
    }
}
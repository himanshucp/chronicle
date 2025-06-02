using System.Security.Claims;

namespace Chronicle.Web.Code.Identity
{
    #region Interface

    public interface ICurrentTenant
    {
        int? Id { get; }
        string? Name { get; }

        string? Color { get; }
        string? CurrencySymbol { get; }
    }

    #endregion

    #region Implementation

    public class CurrentTenant(IHttpContextAccessor httpContextAccessor) : ICurrentTenant
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public int? Id { get => GetClaim(ClaimTypes.TenantId)?.GetId(); }
        public string? Name { get => GetClaim(ClaimTypes.TenantName); }
        public string? Color { get => GetClaim(ClaimTypes.Color); }
        public string? CurrencySymbol { get => GetClaim(ClaimTypes.CurrencySymbol); }

        private string? GetClaim(string name)
        {
            var claims = _httpContextAccessor.HttpContext?.User.Claims.Where(c => c.Type == name);
            if (claims != null && claims.Any())
                return claims.First().Value;

            return null;
        }
    }

    #endregion
}

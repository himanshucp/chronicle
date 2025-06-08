using Chronicle.Caching;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Chronicle.Web
{
    public class ClaimsPrincipalFactory(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IOptions<IdentityOptions> optionsAccessor,
     
        ICache cache) : UserClaimsPrincipalFactory<IdentityUser, IdentityRole>(userManager, roleManager, optionsAccessor)
    {

        // ** Factory Method Pattern

        //public async override Task<ClaimsPrincipal> CreateAsync(IdentityUser appUser)
        //{
        //    try
        //    {
        //        //var principal = await base.CreateAsync(appUser);
        //        //var identity = principal.Identity as ClaimsIdentity;

        //        //var user = db.Users.Single(where: "IdentityId = @0", parms: appUser.Id);
        //        //var tenant = db.Tenants.Single(user!.TenantId);

        //        //var symbol = cache.Currencies[tenant!.CurrencyId].Symbol;

        //        // Add Tenant claims

        //        //identity!.AddClaim(new(ClaimTypes.TenantId, tenant.Id.ToString()!));
        //        //identity.AddClaim(new(ClaimTypes.TenantName, tenant.Name));
        //        //identity.AddClaim(new(ClaimTypes.Color, tenant.Color));
        //        //identity.AddClaim(new(ClaimTypes.CurrencySymbol, symbol!));

        //        //// Add User claims

        //        //identity.AddClaim(new(ClaimTypes.UserId, user.Id.ToString()!));
        //        //identity.AddClaim(new(ClaimTypes.FirstName, user.FirstName));
        //        //identity.AddClaim(new(ClaimTypes.LastName, user.LastName));
        //        //identity.AddClaim(new(ClaimTypes.Email, user.Email));

        //        //// Add Culture claims

        //        //identity.AddClaim(new(ClaimTypes.CurrencyId, tenant.CurrencyId.ToString()!));
        //        //identity.AddClaim(new(ClaimTypes.TimeZoneId, (user.TimeZoneId ?? tenant.TimeZoneId ?? 1).ToString()));
        //        //identity.AddClaim(new(ClaimTypes.LocaleId, (user.LocaleId ?? tenant.LocaleId ?? 1).ToString()));
        //        //identity.AddClaim(new(ClaimTypes.LanguageId, (user.LanguageId ?? tenant.LanguageId ?? 1).ToString()));

        //        //// Add .NET Localization claims

        //        //identity.AddClaim(new(ClaimTypes.TimeZoneName, cache.TimeZones[user.TimeZoneId ?? tenant.TimeZoneId].Name));
        //        //identity.AddClaim(new(ClaimTypes.LocaleName, cache.Locales[user.LocaleId ?? tenant.LocaleId].Name));

        //        //return principal;

        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("In CreateAsync. A claim value is possibly null, this is not allowed.", ex);
        //    }
        //}
    }
}

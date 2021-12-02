namespace Mango.Services.Identity;

using Duende.IdentityServer;
using Duende.IdentityServer.Models;

public class SD
{
    public const string Admin = "Admin";
    public const string Customer = "Customer";

    public static IEnumerable<IdentityResource> IdentityResources =>
        new List<IdentityResource>()
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Email(),
            new IdentityResources.Profile()
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope>()
        {
            new("mango", "Mango Server"),
            new("read", "Read your data."),
            new("write", "Write your data."),
            new("delete", "Delete your data.")
        };

    public static IEnumerable<Client> Clients =>
        new List<Client>()
        {
            new Client()
            {
                ClientId = "mango",
                ClientSecrets = { new Secret("secret".Sha256())},
                AllowedGrantTypes = GrantTypes.Code,
                
                RedirectUris = { "https://localhost:7004/signin-oidc" },
                PostLogoutRedirectUris = {"https://localhost:7004/signout-calleback-oidc"},
                AllowedScopes = new List<string>()
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "mango"
                }
            }
        };
}
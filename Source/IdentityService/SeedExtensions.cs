using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IdentityService
{
    public static class SeedExtensions
    {
        public static void Seed(this ConfigurationDbContext context)
        {
            if (!context.Clients.Any())
            {
                foreach (var client in GetClients().ToList())
                {
                    context.Clients.Add(client.ToEntity());
                }

                context.SaveChanges();
            }

            if (!context.IdentityResources.Any())
            {
                foreach (var resource in GetIdentityResources().ToList())
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }

                context.SaveChanges();
            }

            if (!context.ApiResources.Any())
            {
                foreach (var resource in GetApis().ToList())
                {
                    context.ApiResources.Add(resource.ToEntity());
                }

                context.SaveChanges();
            }
        }

        public static void Seed(this UserManager<ApplicationIdentityUser> userManager)
        {
            if (userManager.Users.Any()) return;

            foreach (var user in GetUsers().ToList())
            {
                var result = userManager.CreateAsync(user, "Pass123$").Result;
            }
        }

        private static IEnumerable<ApiResource> GetApis()
        {
            return new[]
            {
                new ApiResource("API", "API Display Name")
            };
        }

        private static IEnumerable<Client> GetClients()
        {
            return new[]
            {
                /// ClientCredentials
                new Client
                {
                    ClientId = "ClientCredentials",
                    ClientName = "ClientCredentials",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },
                    AllowedScopes = { "API" }
                },
                /// ResourceOwnerPassword
                new Client
                {
                    ClientId = "ResourceOwnerPassword",
                    ClientName = "ResourceOwnerPassword",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets = { new Secret("C7A7E86E-876E-4685-8FF3-AF0EEAB77DE7".Sha256()) },
                    AllowedScopes = { "API" }
                },
                /// HybridAndClientCredentials
                new Client
                {
                    ClientId = "HybridAndClientCredentials",
                    ClientName = "HybridAndClientCredentials",
                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                    ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },
                    AllowedScopes = { "openid", "API" },
                    RedirectUris = { "http://localhost:5001/signin-oidc" },
                    FrontChannelLogoutUri = "http://localhost:5001/signout-oidc",
                    PostLogoutRedirectUris = { "http://localhost:5001/signout-callback-oidc" },
                    AllowOfflineAccess = true
                },
                /// Code
                new Client
                {
                    ClientId = "Code",
                    ClientName = "Code",
                    ClientUri = "http://localhost:5002",
                    AllowedGrantTypes = GrantTypes.Code,
                    AllowedScopes = { "openid", "API" },
                    AllowedCorsOrigins = { "http://localhost:5002" },
                    RequirePkce = true,
                    RequireClientSecret = false,
                    RedirectUris =
                    {
                        "http://localhost:5002/index.html",
                        "http://localhost:5002/callback.html",
                        "http://localhost:5002/silent.html",
                        "http://localhost:5002/popup.html",
                    },
                    PostLogoutRedirectUris = { "http://localhost:5002/index.html" }
                }
            };
        }

        private static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new[] { new IdentityResources.OpenId() };
        }

        private static IEnumerable<ApplicationIdentityUser> GetUsers()
        {
            return new[]
            {
                new ApplicationIdentityUser
                {
                    UserName = "alice",
                    Email = "alice.smith@email.com",
                    EmailConfirmed = true
                },
                new ApplicationIdentityUser
                {
                    UserName = "bob",
                    Email = "bob.smith@email.com",
                    EmailConfirmed = true
                }
            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Gcpe.Hub.API.IntegrationTests
{
    public class TestStartup : Startup
    {
        public TestStartup(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
        {
        }

        public override void ConfigureAzureAuth(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Test Scheme";
                options.DefaultChallengeScheme = "Test Scheme";
            }).AddTestAuth(o => { });
        }

        public override void ConfigureKeycloakAuth(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Test Scheme";
                options.DefaultChallengeScheme = "Test Scheme";
            }).AddTestAuth(o => { });
        }

        public override void ConfigureAuthorizationPolicies(IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ReadAccess", policy => { policy.RequireAssertion(p => { return true; }); });
                options.AddPolicy("WriteAccess", policy => { policy.RequireAssertion(p => { return true; }); });
            });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Gcpe.Hub.API.Helpers
{
    public class SecurityRequirementsOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Policy names map to scopes
            var requiredScopes = context.MethodInfo
                .GetCustomAttributes(typeof(AuthorizeAttribute), true)
                .OfType<AuthorizeAttribute>()
                .Select(attr => attr.Policy)
                .Distinct();

            var scopes = requiredScopes.Any();

            if (requiredScopes.Any())
            {
                operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
                operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });
                if (operation.Security == null)
                {
                    operation.Security = new List<OpenApiSecurityRequirement>();
                }

                var scheme = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Name = "oauth2",
                    In = ParameterLocation.Header,
                    Reference = new OpenApiReference
                    {
                        Id = "oauthSecurityScheme",
                        Type = ReferenceType.SecurityScheme
                    }
                };

                operation.Security.Add(new OpenApiSecurityRequirement { { scheme, requiredScopes.ToList() } });
            }
        }
    }
}

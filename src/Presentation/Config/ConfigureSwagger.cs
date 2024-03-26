using System.Reflection;
using Microsoft.OpenApi.Models;
using Presentation.Auth;
using Presentation.Config;
using Presentation.Filters;

[assembly: HostingStartup(typeof(ConfigureSwagger))]

namespace Presentation.Config;

public class ConfigureSwagger : IHostingStartup
{
    private static bool _configured;

    public void Configure(IWebHostBuilder builder)
    {
        if (_configured) return;
        _configured = true;

        builder.ConfigureServices((context, services) =>
        {
            if (!context.HostingEnvironment.IsDevelopment())
                return;

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);

                options.OperationFilter<IdempotencyKeyOperationFilter>();
                options.OperationFilter<PublicKeyAndSignatureOperationFilter>();
                options.OperationFilter<RequestedWithXmlHttpRequest>();

                options.SupportNonNullableReferenceTypes();

                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "mieci.ge",
                    Version = "v1",
                    Description = "cryptographically secure exit polls, with blockchain technology",
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            },
                        },
                        []
                    },
                });
            });
        });
    }
}
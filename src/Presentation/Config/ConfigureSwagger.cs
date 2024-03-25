using System.ComponentModel;
using System.Reflection;
using Microsoft.OpenApi.Models;
using Presentation.Config;
using Presentation.Filters;

[assembly: HostingStartup(typeof(ConfigureSwagger))]

namespace Presentation.Config;

[EditorBrowsable(EditorBrowsableState.Never)]
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
                options.OperationFilter<RequestedWithXmlHttpRequest>();

                options.SupportNonNullableReferenceTypes();

                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "mieci.ge",
                    Version = "v1",
                    Description = "online mock elections",
                    // TODO do we even need this shit?
                    // Contact = new OpenApiContact
                    // {
                    //     Name = "ddjerqq",
                    //     Email = "ddjerqq@gmail.com",
                    //     Url = new Uri("https://mieci.ge"),
                    // },
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme.",
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
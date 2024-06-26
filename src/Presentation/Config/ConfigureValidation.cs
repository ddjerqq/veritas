﻿using Application;
using Domain;
using FluentValidation;
using FluentValidation.AspNetCore;
using Presentation.Config;
using ZymLabs.NSwag.FluentValidation;

[assembly: HostingStartup(typeof(ConfigureValidation))]

namespace Presentation.Config;

public class ConfigureValidation : IHostingStartup
{
    private static bool _configured;

    public void Configure(IWebHostBuilder builder)
    {
        if (_configured) return;
        _configured = true;

        builder.ConfigureServices(services =>
        {
            services.AddValidatorsFromAssembly(DomainAssembly.Assembly, includeInternalTypes: true);
            services.AddValidatorsFromAssembly(ApplicationAssembly.Assembly, includeInternalTypes: true);

            services.AddFluentValidationAutoValidation()
                .AddFluentValidationClientsideAdapters();

            services.AddScoped<FluentValidationSchemaProcessor>(sp =>
            {
                var validationRules = sp.GetService<IEnumerable<FluentValidationRule>>();
                var loggerFactory = sp.GetService<ILoggerFactory>();
                return new FluentValidationSchemaProcessor(sp, validationRules, loggerFactory);
            });
        });
    }
}
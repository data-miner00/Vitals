namespace Vitals.WebApi;

using Asp.Versioning.Conventions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using RabbitMQ.Client;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Security.Cryptography;
using Vitals.Core.Clients;
using Vitals.Core.Models;
using Vitals.Core.Repositories;
using Vitals.Integrations;
using Vitals.Integrations.Clients;
using Vitals.WebApi.Options;
using Vitals.WebApi.Repositories;
using Vitals.WebApi.Services;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        using var hasher = SHA512.Create();

        var dbConnectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Default connection string is missing.");

        var dbContext = new AppDbContext(dbConnectionString);

        builder.Services.AddSingleton(dbContext);
        builder.Services.AddSingleton(hasher);
        builder.Services.AddSingleton<IUserRepository, Integrations.Repositories.UserRepository>();
        builder.Services.AddSingleton<UserRepository>();
        builder.Services.AddSingleton<IPostRepository, Integrations.Repositories.PostRepository>();
        builder.Services.AddSingleton<IVoteRepository, Integrations.Repositories.VoteRepository>();
        builder.Services.AddControllers(opt =>
        {
            // Return 406 Not Acceptable if the client requests a format that is not supported
            opt.ReturnHttpNotAcceptable = true;
        }).AddXmlDataContractSerializerFormatters();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddHealthChecks();
        builder.Services.AddApiVersioning(opt =>
        {
            opt.AssumeDefaultVersionWhenUnspecified = true;
            opt.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
            opt.ReportApiVersions = true;
        }).AddMvc(opt =>
        {
            opt.Conventions.Add(new VersionByNamespaceConvention());
        }).AddApiExplorer(opt =>
        {
            opt.GroupNameFormat = "'v'V";
            opt.SubstituteApiVersionInUrl = true;
        });

        builder.Services.AddProblemDetails(opt =>
        {
            opt.CustomizeProblemDetails = (ctx) =>
            {
                ctx.ProblemDetails.Extensions.Add("additionalInfo", "hello world");
                ctx.ProblemDetails.Extensions.Add("server", Environment.MachineName);
            };
        });

        builder.RegisterLoggings();

        using var greeterMeter = new Meter("Vitals.WebApi", "1.0.0");
        builder.Services.AddSingleton(greeterMeter);

        var otlpEndpoint = builder.Configuration["OTLP_ENDPOINT_URL"];
        var otel = builder.Services.AddOpenTelemetry();

        otel.ConfigureResource(resource => resource
            .AddService(serviceName: builder.Environment.ApplicationName));

        otel.AddMetrics(meterName: builder.Environment.ApplicationName, otlpEndpoint);

        var activitySource = new ActivitySource("Vitals.WebApi");
        builder.Services.AddSingleton(activitySource);

        otel.AddTracing(builder.Environment.ApplicationName, activitySource.Name, otlpEndpoint);

        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

        var logOption = builder.Configuration.GetSection(LogBackgroundServiceOption.SectionName).Get<LogBackgroundServiceOption>()
            ?? throw new InvalidOperationException("LogBackgroundServiceOption is not configured.");

        builder.Services.AddSingleton(logOption);

        builder.Services.AddHostedService<LogBackgroundService>();

        builder.AddRabbitMQ();

        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseStatusCodePages();
        app.MapControllers();
        app.MapHealthChecks();
        app.MapPrometheusScrapingEndpoint();
        app.Run();
    }

    private static IEndpointConventionBuilder MapHealthChecks(this WebApplication app)
    {
        return app.MapHealthChecks("/health", new HealthCheckOptions
        {
            AllowCachingResponses = false,
            ResultStatusCodes =
            {
                [HealthStatus.Healthy] = StatusCodes.Status200OK,
                [HealthStatus.Degraded] = StatusCodes.Status200OK,
                [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable,
            },
        });
    }

    private static WebApplicationBuilder RegisterLoggings(this WebApplicationBuilder builder)
    {
        builder.Logging
            .ClearProviders()
            .AddConsole()
            .AddDebug()
            .AddOpenTelemetry(opt =>
            {
                opt.AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri(builder.Configuration["OTLP_ENDPOINT_URL"]);
                });

                var resourceBuilder = ResourceBuilder.CreateDefault()
                    .AddService(builder.Environment.ApplicationName);

                opt.AddConsoleExporter()
                    .SetResourceBuilder(resourceBuilder);

                opt.IncludeScopes = true;
            });

        return builder;
    }

    private static OpenTelemetryBuilder AddMetrics(
        this OpenTelemetryBuilder otel,
        string meterName,
        string otlpEndpoint)
    {
        // Add Metrics for ASP.NET Core and our custom metrics and export to Prometheus
        otel.WithMetrics(metrics =>
        {
            metrics
                // Metrics provider from OpenTelemetry
                .AddAspNetCoreInstrumentation()
                .AddRuntimeInstrumentation()
                .AddProcessInstrumentation()
                .AddMeter(meterName)
                // Metrics provides by ASP.NET Core in .NET 8
                .AddMeter("Microsoft.AspNetCore.Hosting")
                .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
                // Metrics provided by System.Net libraries
                .AddMeter("System.Net.Http")
                .AddMeter("System.Net.NameResolution")
                .AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri(otlpEndpoint);
                })
                .AddPrometheusExporter();
        });

        return otel;
    }

    private static OpenTelemetryBuilder AddTracing(
        this OpenTelemetryBuilder otel,
        string applicationName,
        string activityName,
        string otlpEndpoint)
    {
        // Add Tracing for ASP.NET Core and our custom ActivitySource and export to Jaeger
        otel.WithTracing(tracing =>
        {
            tracing.AddSource(applicationName)
                .SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddService(applicationName));
            tracing.AddAspNetCoreInstrumentation();
            tracing.AddHttpClientInstrumentation();
            tracing.AddSource(activityName);
            if (otlpEndpoint != null)
            {
                tracing.AddOtlpExporter(otlpOptions =>
                {
                    otlpOptions.Endpoint = new Uri(otlpEndpoint);
                });

                tracing.AddJaegerExporter(); // Remove this for OTLP
            }
            else
            {
                tracing.AddConsoleExporter();
            }
        });

        return otel;
    }
    
    private static WebApplicationBuilder AddRabbitMQ(this WebApplicationBuilder builder)
    {
        var messagingOption = builder.Configuration
            .GetSection(MessagingOption.SectionName)
            .Get<MessagingOption>()
            ?? throw new InvalidOperationException("MessagingOption not set.");

        var emailOption = builder.Configuration
            .GetSection(EmailOption.SectionName)
            .Get<EmailOption>()
            ?? throw new InvalidOperationException("EmailOption not set.");

        builder.Services.AddSingleton(emailOption);

        var factory = new ConnectionFactory
        {
            HostName = messagingOption.HostName,
            Port = messagingOption.Port,
        };
        var connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        var channel = connection.CreateChannelAsync().GetAwaiter().GetResult();

        channel
            .QueueDeclareAsync(
                queue: emailOption.QueueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null)
            .GetAwaiter()
            .GetResult();

        var rmqEventPublisher = new RmqEventPublisher(channel, emailOption.QueueName);

        builder.Services.AddSingleton<IEventPublisher>(rmqEventPublisher);

        return builder;
    }
}

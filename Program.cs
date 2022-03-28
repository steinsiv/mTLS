using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Server.Kestrel.Https;

using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);


builder.WebHost.ConfigureKestrel(options =>
{
    options.ConfigureHttpsDefaults(configureOptions =>
    {
        configureOptions.ClientCertificateMode = ClientCertificateMode.RequireCertificate;
    });
});

builder.Services
    .AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme)
    .AddCertificate(options =>
    {

        options.AllowedCertificateTypes = CertificateTypes.Chained; // Only allow chained certs, no self signed

        options.RevocationMode = X509RevocationMode.NoCheck;        // Online CA required to check revocation
        options.Events = new CertificateAuthenticationEvents()
        {
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices.GetService<ILogger<Program>>();

                logger!.LogError(context.Exception, "Failed auth.");

                return Task.CompletedTask;
            },
            OnCertificateValidated = context =>
            {
                var logger = context.HttpContext.RequestServices.GetService<ILogger<Program>>();

                // Verify that client cert has been issued by the correct CA to narrow down clients
                logger!.LogInformation($"Cert issued by: {context.ClientCertificate.Issuer}");

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

using ACM.DockerArchive;
using ACM.Kernel;
using APC.Kernel;
using APC.Kernel.Constants;
using APC.Kernel.Extensions;
using APC.Kernel.Registrations;
using APC.Skopeo;

ModuleRegistration registration = new(ModuleType.ACM, typeof(Collector));
registration.AddEndpoint("docker-archive", 1);
HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Services.AddTelemetry(registration);
builder.Services.AddStorage();
builder.Services.AddSingleton<SkopeoClient>();
builder.Services.AddSingleton<FileSystem>();
builder.Services.AddSingleton<Docker>();
builder.Services.Register(registration);
builder.Services.AddHostedService<Worker>();

IHost host = builder.Build();
host.Run();
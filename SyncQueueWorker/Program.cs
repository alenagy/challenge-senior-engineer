using SyncQueueWorker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();  // Register the worker as a hosted service

var host = builder.Build();
host.Run();  // Starts the application
using UNIOOP.NotificationWorker;
using UNIOOP.NotificationWorker.Configuration;
using UNIOOP.NotificationWorker.Messaging;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddOptions<RabbitMQOptions>()
    .Bind(builder.Configuration.GetSection("RabbitMQ"))
    .ValidateOnStart();

builder.Services.AddSingleton<IRabbitMQConnection, RabbitMQConnection>();

builder.Services.AddSingleton<INotificationConsumer, NotificationConsumer>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
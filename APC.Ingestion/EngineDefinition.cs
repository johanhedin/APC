using APC.Kernel;
using MassTransit;

namespace APC.Ingestion;

public class EngineDefinition : ConsumerDefinition<Engine> {
  public EngineDefinition() {
    EndpointName = Endpoints.APC_INGEST.ToString().Replace("queue:", "");
    ConcurrentMessageLimit = 1;
  }

  protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
    IConsumerConfigurator<Engine> consumerConfigurator) {
    // configure message retry with millisecond intervals
    endpointConfigurator.UseMessageRetry(r => r.Intervals(100, 200, 500, 800, 1000));
    // use the outbox to prevent duplicate events from being published
    endpointConfigurator.UseInMemoryOutbox();
  }
}
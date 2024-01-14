namespace ACM.Wget;

public class Worker : BackgroundService {
  public Worker(ILogger<Worker> logger) {
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
    while (!stoppingToken.IsCancellationRequested) {
      await Task.Delay(1000, stoppingToken);
    }
  }
}
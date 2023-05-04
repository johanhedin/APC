using System.Text.RegularExpressions;
using APC.Kernel;
using APC.Kernel.Messages;
using APC.Kernel.Models;
using MassTransit;

namespace APM.Helm;

public class Processor : IProcessor {
  private readonly Helm helm_;

  public Processor(Helm helm) {
    helm_ = helm;
  }

  public async Task Consume(ConsumeContext<ArtifactProcessRequest> context) {
    Artifact artifact = context.Message.artifact;
    await helm_.ProcessArtifact(artifact);
    ArtifactProcessedRequest request = new() {
      Artifact = artifact,
      Context = context.Message.ctx
    };
    await context.Send(Endpoints.APC_INGEST_PROCESSED, request);
  }

}
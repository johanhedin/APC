using System.Diagnostics;
using APC.API.Input;
using APC.API.Output;
using APC.Kernel.Models;
using APC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace APC.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProcessorController : ControllerBase {
  private readonly IArtifactService aps_;
  private readonly IApcDatabase database_;

  public ProcessorController(IArtifactService aps, IApcDatabase database) {
    database_ = database;
    aps_ = aps;
  }

  [HttpGet("processors")]
  public async Task<IEnumerable<ProcessorOutput>> GetProcessors() {
    IEnumerable<Processor> processors = await database_.GetProcessors();
    List<ProcessorOutput> proc_out = new List<ProcessorOutput>();

    foreach (Processor processor in processors) {
      proc_out.Add(new ProcessorOutput {
        Id = processor.Id,
        Config = processor.Config.ToJson(),
        Description = processor.Description
      });
    }

    return proc_out;
  }
  
  [HttpPost("update")]
  [Authorize(Roles = "Administrator")]
  public async Task<Processor> UpdateProcessor([FromBody] UpdateProcessorInput input) {
    Processor processor = await database_.GetProcessor(input.ProcessorId);
    
    processor.Description = input.Description;
    await database_.UpdateProcessor(processor); 
    return processor;
  }

  [HttpPost]
  [Authorize(Roles = "Administrator")]
  public async Task<ActionResult> Post([FromBody] AddProcessorInput input) {
    await database_.AddProcessor(new Processor() {
      Id = input.ProcessorId
    });
    return Ok(new {
      Message = $"Added {input.ProcessorId}!"
    });
  }
}
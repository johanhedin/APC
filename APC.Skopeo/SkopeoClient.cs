﻿using System.Text;
using APC.Kernel;
using APC.Kernel.Extensions;
using APC.Skopeo.Models;
using CliWrap;
using Microsoft.Extensions.Logging;

namespace APC.Skopeo;

public class SkopeoClient {
  private readonly ILogger<SkopeoClient> logger_;

  public SkopeoClient(ILogger<SkopeoClient> logger) {
    logger_ = logger;
  }

  public async Task<bool> CopyToRegistry(string remote_image) {
    Image image = new(remote_image);
    string? registry =
      Configuration.GetApcVar(ApcVariable.ACM_CONTAINER_REGISTRY);

    string internal_image = $"docker://{registry}/{image.Repository}";
    StringBuilder std_out = new();
    StringBuilder std_err = new();
    Command cmd = Cli.Wrap("skopeo")
                     .WithArguments(args => {
                       args.Add("copy");
                       args.Add("--dest-tls-verify=false");
                       args.Add(image.Uri);
                       args.Add(internal_image);
                     })
                     .WithStandardOutputPipe(
                       PipeTarget.ToStringBuilder(std_out))
                     .WithStandardErrorPipe(
                       PipeTarget.ToStringBuilder(std_err));
    logger_.LogInformation($"Pull> {image.Uri}=>{internal_image}");
    try {
      CommandResult result =
        await cmd.ExecuteAsync();
    } catch (Exception e) {
      logger_.LogError(std_err.ToString());
      throw;
    }

    return true;
  }
  public async Task<string> CopyToTar(string remote_image, string target_dir) {
    Uri uri = new Uri(remote_image);
    string target = $"{uri.Host}{uri.PathAndQuery}";
    string tar_name = target.Replace("/", "_").Replace(":", "_").Replace(".", "_");
    string tar_path = Path.Join(target_dir, $"{tar_name}.tar");
    string internal_image = $"docker-archive:{tar_path}:{target}";
    StringBuilder std_out = new();
    StringBuilder std_err = new();
    Command cmd = Cli.Wrap("skopeo")
                     .WithArguments(args => {
                       args.Add("copy");
                       args.Add($"docker://{target}");
                       args.Add(internal_image);
                     })
                     .WithStandardOutputPipe(
                       PipeTarget.ToStringBuilder(std_out))
                     .WithStandardErrorPipe(
                       PipeTarget.ToStringBuilder(std_err));
    logger_.LogInformation($"Pull> {remote_image}=>{internal_image}");
    try {
      CommandResult result =
        await cmd.ExecuteAsync();
    } catch (Exception e) {
      logger_.LogError(std_err.ToString());
      throw;
    }
    return tar_path;
  }

  public async Task<SkopeoListTagsOutput?> GetTags(string image) {
    Command cmd = Cli.Wrap("skopeo").WithArguments(args => {
      args.Add("list-tags");
      args.Add($"docker://{image}");
    });
    SkopeoListTagsOutput tags;
    try {
      tags = await cmd.ExecuteWithResult<SkopeoListTagsOutput>();
    } catch (Exception e) {
      logger_.LogError(e.ToString());
      return null;
    }

    return tags;
  }

  public async Task<SkopeoManifest?> ImageExists(string input) {
    Image image = new(input);
    string? registry =
      Configuration.GetApcVar(ApcVariable.ACM_CONTAINER_REGISTRY);
    Command cmd = Cli.Wrap("skopeo")
                     .WithArguments(args => {
                       args.Add("inspect");
                       args.Add("--tls-verify=false");
                       args.Add(
                         $"docker://{registry}/{image.Repository}");
                     });
    SkopeoManifest manifest;
    try {
      manifest = await cmd.ExecuteWithResult<SkopeoManifest>();
    } catch (Exception e) {
      return null;
    }

    return manifest;
  }
}
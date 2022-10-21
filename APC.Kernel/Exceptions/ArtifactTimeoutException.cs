namespace APC.Kernel.Exceptions;

public class ArtifactTimeoutException : Exception {
  public ArtifactTimeoutException() {
  }

  public ArtifactTimeoutException(string? message) : base(message) {
  }

  public ArtifactTimeoutException(string? message, Exception? innerException) : base(message, innerException) {
  }
}

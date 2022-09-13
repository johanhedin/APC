using Dapper.Contrib.Extensions;

namespace APC.Infrastructure.Models;

[Table("artifacts")]
public class Artifact {
  public Artifact() {
    versions = new Dictionary<string, ArtifactVersion>();
    dependencies = new HashSet<string>();
  }

  public int id { get; set; }
  public string name { get; set; }
  public string module { get; set; }
  public ArtifactStatus status { get; set; } = ArtifactStatus.PROCESSING;

  [Computed] public Dictionary<string, ArtifactVersion> versions { get; set; }

  [Computed] public HashSet<string> dependencies { get; set; }

  public bool AddDependency(string id) {
    return dependencies.Add(id);
  }

  public HashSet<string> DepDiff(HashSet<string> dependencies_in_db) {
    HashSet<string> diff = new HashSet<string>();
    foreach (string dep in dependencies) {
      if (!dependencies_in_db.Contains(dep)) {
        diff.Add(dep);
      }
    }
    return diff;
  }

  public bool AddVersion(ArtifactVersion version) {
    if (versions.ContainsKey(version.version)) return false;
    versions.Add(version.version, version);
    return true;
  }

  public bool HasVersion(string version) {
    return versions.ContainsKey(version);
  }
}
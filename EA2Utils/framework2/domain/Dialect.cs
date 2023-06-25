using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EA;

namespace EAUtils.framework2.domain
{
    public class Dialect : JsLoader
    {
        public List<Artifact> artifacts = new List<Artifact>();
        public Dictionary<string, Artifact> artifactsByName = new Dictionary<string, Artifact>();

        public Package package;
        public Element element;

        public Dialect(Package package)
        {
            this.package = package;

            this.name = package.Name;
        }

        public void loadArtifacts( EAUtils eaUtils)
        {
            artifacts.Clear();
            artifactsByName.Clear();
            Artifact artifact;

            foreach ( Package package in this.package.Packages)
            {
                artifact = new Artifact(eaUtils, package);
                artifacts.Add(artifact);
                artifactsByName.Add(artifact.name, artifact);
            }
        }
    }
}

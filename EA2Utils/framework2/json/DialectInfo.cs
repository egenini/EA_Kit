using EA;
using EAUtils.framework2.domain;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAUtils.framework2.json
{
    public class DialectInfo : JsonableCommon
    {
        private Package dialectPackage;
        public List<ArtifactInfo> artifacts = new List<ArtifactInfo>();
        public JObject dialectJson = null;

        public FilePreference filePreference;

        public DialectInfo(Package dialectPackage)
        {
            this.dialectPackage = dialectPackage;
            filePreference = new FilePreference(this);
        }

        internal void loadArtifacts()
        {
            string jsonTemplate;
            // vamos a buscar los artefactos para recabar la info.
            foreach (Package artifactPackage in dialectPackage.Packages)
            {
                foreach(Element element in artifactPackage.Elements)
                {
                    if( element.Stereotype == "Artifact")
                    {
                        jsonTemplate = "{}";

                        foreach (EA.Attribute attr in element.Attributes)
                        {
                            if( attr.Name == Artifact.JSON_TEMPLATE_AATRIBUTE_NAME)
                            {
                                jsonTemplate = attr.Notes;
                                break;
                            }
                        }

                        artifacts.Add( new ArtifactInfo(artifactPackage.Name, jsonTemplate) );
                        break;
                    }
                }
            }
        }

        public void parse(string jsonString)
        {
            dialectJson = JObject.Parse(jsonString);
            dynamic token;
                    // sólo interesa si está 
            foreach (ArtifactInfo info in artifacts)
            {
                token = dialectJson.SelectToken(info.name);
                if( token == null)
                {
                    dialectJson[info.name] = JObject.Parse(info.jsonTemplate);
                }
            }
        }

        public string stringfity()
        {
            string json = "{}";

            if(this.dialectJson != null)
            {
                json = this.dialectJson.ToString();
            }
            else
            {
                this.start();

                foreach (ArtifactInfo info in artifacts)
                {
                    this.writer.WritePropertyName(info.name);
                    this.writer.WriteRawValue(info.jsonTemplate);
                }

                json = this.end();
            }

            return json;
        }
    }

    public class ArtifactInfo
    {
        public string name;
        public string jsonTemplate;

        public ArtifactInfo(string name, string jsonTemplate)
        {
            this.name = name;
            this.jsonTemplate = jsonTemplate;
        }
    }
}

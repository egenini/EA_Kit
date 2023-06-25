using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EA;
using EAUtils;
using UserInterface.frw;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace UserInterface.html
{
    public class Html : HtmlCommon
    {
        private DefaultContractResolver contractResolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() };

        private JsonSerializerSettings jsonSerializerSettings;

        public List<Resource> links   = new List<Resource>();
        public List<Resource> scripts = new List<Resource>();

        public Html(Element element, Framework frameworkInstance, EAUtils.EAUtils eaUtils) : base(element, frameworkInstance, eaUtils)
        {
            jsonSerializerSettings = new JsonSerializerSettings { ContractResolver = contractResolver, Formatting = Formatting.Indented };
        }

        public new string stringfity()
        {
            string json = JsonConvert.SerializeObject(this, jsonSerializerSettings);

            return json;
        }

        public new string walk()
        {
            List <ConnectorUtils.ElementConnectorInfo> infos = this.eaUtils.connectorUtils.get(this.element, null, "IncludeResource", null, null, false, null);

            Resource resource;
            File file;
            string uri = null;

            foreach (ConnectorUtils.ElementConnectorInfo info in infos)
            {
                file = info.element.Files.GetAt(0);

                if(file.Type == "Archivo local")
                {
                    uri = "file:///"+ file.Name.Replace('\\', '/');
                }
                else
                {
                    uri = file.Name;
                }

                resource = new Resource { uri = uri,
                    integrity = this.eaUtils.taggedValuesUtils.get(info.element, "integrity", "").asString(),
                    crossorigin = this.eaUtils.taggedValuesUtils.get(info.element, "crossorigin", "").asString()
                };

                if ( info.element.Stereotype == "Script")
                {
                    scripts.Add(resource);
                }
                else if (info.element.Stereotype == "CSS")
                {
                    links.Add(resource);
                }
            }

            return base.walk();
        }
    }

    public class Resource
    {
        public string uri { set; get; }
        public string integrity { set; get; }
        public string crossorigin { set; get; }
    }
}

using EA;
using EAUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAPI
{
    public class EstructuraSolucion
    {
        private Package principal; // field
        private Package definicion;
        private Package model;
        private Element info;
        private Element licence;
        private Element contact;
        private Element serviceComponent;
        private List<EA.Element> servers = new List<EA.Element>();
        public Dictionary<string, EA.Element> models = new Dictionary<string, Element>();
        public Dictionary<string, EA.Element> reponses = new Dictionary<string, Element>();
        public Package Principal   // property
        {
            get { return principal; }   // get method
            set { principal = value; }  // set method
        }
        public Package Definicion { get => definicion; set => definicion = value; }
        public Package Model { get => model; set => model = value; }
        public Element Info { get => info; set => info = value; }
        public List<Element> Servers { get=> servers; set => servers = value; }
        public Element Licence { get => licence; set => licence = value; }
        public Element Contact { get => contact; set => contact = value; }
        public Element ServiceComponent { get => serviceComponent; set => serviceComponent = value; }
        
        public bool oneOperationXInterface = false;
        public bool establecer(EAUtils.EAUtils eaUtils)
        {
            // El menú no se habilita si no está sobre el MainPackage, por lo tanto sabemos en que paquete estamos parados.
            this.principal = eaUtils.repository.GetTreeSelectedPackage();
            this.definicion = eaUtils.packageUtils.getChildPackageByStereotype(this.principal, "DefPackage");

            oneOperationXInterface = (bool)eaUtils.taggedValuesUtils.get(principal.Element, "1 operation x interface", "false").asBoolean();


            ServiceComponent = eaUtils.packageUtils.getChildElementByType(Definicion, "Component");

            this.establecerElementoInfo(eaUtils);
            this.establecerServers(eaUtils);
            this.establecerPaqueteModel(eaUtils);

            return verificar(eaUtils);
        }
        public bool establecerElementoInfo(EAUtils.EAUtils eaUtils)
        {
            this.info = eaUtils.packageUtils.getChildElementByStereotype(this.principal, "Info");

            if(this.info == null)
            {
                eaUtils.printOut("Error: no se encuentra el elemento Info");
            }

            this.contact = eaUtils.packageUtils.getChildElementByStereotype(this.principal, "Contact");

            if (this.contact == null)
            {
                eaUtils.printOut("Error: no se encuentra el elemento Contact");
            }

            this.licence = eaUtils.packageUtils.getChildElementByStereotype(this.principal, "Licence");

            if (this.licence == null)
            {
                eaUtils.printOut("Error: no se encuentra el elemento Licence");
            }
            return this.info!= null;
        }
        public void establecerServers(EAUtils.EAUtils eaUtils)
        {
            List<Object> ol = eaUtils.packageUtils.getChildrenElementByStereotype(this.principal, "Server");

            foreach(Object o in ol)
            {
                servers.Add((Element)o);
            }
            if (this.servers.Count == 0)
            {
                eaUtils.printOut("no se encuentra ningún elemento Server");
            }
        }
        public bool establecerPaqueteModel(EAUtils.EAUtils eaUtils)
        {
            string modelPackageGuid = eaUtils.taggedValuesUtils.get(this.principal.Element, "model-package", "").asString();

            if (!string.IsNullOrEmpty(modelPackageGuid))
            {
                Package model = eaUtils.repository.GetPackageByGuid(modelPackageGuid);

                if (model != null)
                {
                    this.model = model;
                }
                else
                {
                    eaUtils.printOut("No se encuentra el paquete para el modelo " + modelPackageGuid);
                }
            }
            else
            {
                this.model = eaUtils.packageUtils.getChildPackageByStereotype(this.principal, "ModelPackage"); ;
            }
            return this.model != null;
        }
        public bool verificar(EAUtils.EAUtils eaUtils)
        {
            return this.Model != null && this.Info != null && this.Principal != null && this.Definicion != null 
                && this.Licence != null && this.Contact != null;

        }
        public void establecerModels()
        {
            foreach( EA.Element element in this.model.Elements)
            {
                this.models.Add(element.Name, element);
            }
        }
        public void establecerResponses()
        {
            foreach (EA.Element element in this.definicion.Elements)
            {
                if(element.HasStereotype("Returns"))
                {
                    this.reponses.Add(element.Name, element);
                }
            }
        }
    }
}

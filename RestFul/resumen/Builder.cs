using EA;
using RestFul.resumen.modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestFul.resumen
{
    public class Builder
    {
        public EAUtils.EAUtils eaUtils;
        public ResumenUtils resumenUtils;
        public Builder(EAUtils.EAUtils eaUtils)
        {
            this.eaUtils = eaUtils;
            this.resumenUtils = new ResumenUtils(eaUtils);
        }

        /// <summary>
        /// Crea un amb a partir de una clase "DomainClass" o "LogicClass" que se agrega al diagrama desde el explorador.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="diagram"></param>
        public void makeResource(Element from, Diagram diagram)
        {
            Package package = this.eaUtils.repository.GetPackageByID(diagram.PackageID);

            this.resumenUtils.diagram = diagram;

            // buscamos el último elemento en el diagrama, eso es el diagramObject que tiene el top más alto.

            DiagramObject diagramObject = resumenUtils.ultimoDiagramObject();

            Recurso recurso = new Recurso(from, diagram, this.eaUtils).buscarPadre().crearAbm(package, diagramObject);
        }
    }
}

using EA;
using System;
using System.Collections.Generic;

namespace RestFul
{
    class RestFulController
    {
        private ServiceCommons.ServiceUtils serviceUtils;
        private EAUtils.EAUtils eaUtils;

        public RestFulController( Repository repository )
        {
            this.eaUtils = new EAUtils.EAUtils(repository);
            serviceUtils = new ServiceCommons.ServiceUtils(this.eaUtils);
        }

        public Boolean OnPostNewDiagramObject( Repository repository, DiagramObject objectoDelDiagrama, Diagram diagrama,Element elemento)
        {
            Boolean changed = false;

            // determinar si está el addin de servicios SOA

            // se fija si tiene que generar una instancia de un elemento de mensaje
            changed = serviceUtils.watchWhatWave(eaUtils, elemento, diagrama);

            if ( diagrama.MetaType.Contains("APIRestResumen") || diagrama.MetaType.Contains("APIRestSummary") || diagrama.MetaType.Contains("Servicio") || diagrama.MetaType.Contains("Service") )
            {

                if (elemento.Stereotype == "URI" )
                {
                    // buscar el objectoDelDiagrama y cambiar tamaño y color de la fuente
                    //objectoDelDiagrama = doUtils.findInDiagramObjects(elemento, diagrama);
                    //elemento.Name = "http://api/...";
                    elemento.Update();
                    objectoDelDiagrama.FontBold = true;
                    objectoDelDiagrama.FontColor = 0;
                    objectoDelDiagrama.fontSize = 10;
                    objectoDelDiagrama.fontName = "Calibri";
                    objectoDelDiagrama.Update();

                    changed = true;
                }
                else if (elemento.Stereotype == "Recurso" || elemento.Stereotype == "Resource")
                {
                    bool english = elemento.Stereotype == "Resource";
                    // buscar el objectoDelDiagrama y actualizar el estilo
                    elemento.Name = "/{"+ (english ? "recurso" : "resource") +"...Id}";
                    elemento.Update();
                    //objectoDelDiagrama = doUtils.findInDiagramObjects(elemento, diagrama);
                    objectoDelDiagrama.FontBold = true;
                    objectoDelDiagrama.FontColor = 8421504;
                    objectoDelDiagrama.fontSize = 12;
                    objectoDelDiagrama.fontName = "Calibri";
                    objectoDelDiagrama.Update();

                    changed = true;
                }
                else if (elemento.Stereotype == "GET" || elemento.Stereotype == "POST"
                    || elemento.Stereotype == "PUT" || elemento.Stereotype == "DELETE"
                    || elemento.Stereotype == "OPTIONS" || elemento.Stereotype == "PATCH")
                {
                    elemento.Name = elemento.Stereotype.ToLower();
                    elemento.Update();

                    // buscar el objectoDelDiagrama y poner bold
                    //objectoDelDiagrama = doUtils.findInDiagramObjects(elemento, diagrama);
                    objectoDelDiagrama.FontBold = true;
                    objectoDelDiagrama.fontSize = 8;
                    objectoDelDiagrama.fontName = "Calibri";
                    objectoDelDiagrama.Update();

                    changed = true;
                }
            }

            if (diagrama.MetaType.Contains("Definition") || diagrama.MetaType.Contains("Definición") || diagrama.MetaType.Contains("Definicion"))
            {
                if (elemento.Type == "Interface" && elemento.Stereotype == "")
                {
                    // crear una instancia de una interfaz expuesta. Si no te gusta borrala.

                    // si la interfaz no existe se agrega

                    Element providedInterface = this.serviceUtils.addProvidedInterface(elemento, "RestFul", diagrama);

                    if (providedInterface != null)
                    {
                        changed = true;
                        // una interfaz embebida tiene 40 de alto, y 13 de ancho.
                        // Si la ubicamos arriba el top es el top +40 
                        // mostrar en el diagrama
                        this.eaUtils.diagramUtils.showNewProvidedInterface(providedInterface, diagrama);

                        // si existe elemento uri crear una relación
                        List<Object> uris = eaUtils.diagramUtils.findInDiagramObjectsByStereotype("URI", diagrama);

                        if (uris.Count != 0)
                        {
                            this.eaUtils.connectorUtils.addConnectorAssociation(providedInterface, (Element)uris[0], null, null);

                        }

                        // si existen elementos mediaType crear una relación.
                        List<Object> mediaTypes = eaUtils.diagramUtils.findInDiagramObjectsByStereotype("mediaType", diagrama);

                        foreach (Element mediaType in mediaTypes)
                        {
                            this.eaUtils.connectorUtils.addConnectorRealization(providedInterface, mediaType, null, null);
                            this.eaUtils.connectorUtils.addConnectorDependency(providedInterface, mediaType, null, null);
                        }
                    }
                }
                if (elemento.Stereotype == "mediaType")
                {

                }
            }
            return changed;
        }


    }
}

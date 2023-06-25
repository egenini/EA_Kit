using EA;
using EAUtils;
using Persistencia.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EAUtils.ConnectorUtils;

namespace Persistencia.frw
{
    class RelationshipAnalizer
    {
        public EAUtils.EAUtils eaUtils   = null;
        public Element   tablaEnAnalisis = null;
        public Generator generator       = null;
        public Info      mainInfo        = null;

        List<ElementConnectorInfo> clasesImplementanTablaEnAnalisisListInfo;
        List<ElementConnectorInfo> tablasAsociadasComoOrigenListInfo;
        List<ElementConnectorInfo> tablasAsociadasComoDestinoListInfo;

        public void analizar( Info mainInfo )
        {
            this.mainInfo = mainInfo;

            clasesImplementanTablaEnAnalisisListInfo = generator.classRealization(this.tablaEnAnalisis);

            tablasAsociadasComoOrigenListInfo = eaUtils.connectorUtils.get(this.tablaEnAnalisis, EAUtils.ConnectorUtils.CONNECTOR__ASSOCIATION,
                    null, null, "table", true, null);

            analizarTablasOrigen();

            tablasAsociadasComoDestinoListInfo = eaUtils.connectorUtils.get(this.tablaEnAnalisis, EAUtils.ConnectorUtils.CONNECTOR__ASSOCIATION,
                    null, null, "table", false, null);

            analizarTablasDestino();

        }

        /// <summary>
        /// Las tablas que son origen de la relación tienen como FK a la tabla que se está analizando.
        /// 
        /// Estas FK's pueden ser producto de una relación uno a uno, uno a muchos o muchos a muchos.
        /// 
        /// Para poder establecer cual es el tipo de relación y quien "manda" en la misma es necesario 
        /// buscar en el modelo de clases y analizar las relaciones entre ellas.
        /// 
        /// La relación establece la cardinalidad y puede establecer el nombre del atributo en la clase.
        /// </summary>
        void analizarTablasOrigen()
        {
            foreach(ElementConnectorInfo tablaOrigenInfo in this.tablasAsociadasComoOrigenListInfo)
            {
                analizarTablaOrigen(tablaOrigenInfo.element);
            }
        }

        /// <summary>
        /// Estas tablas son las FK's que están en la tabla que estamos analizando
        /// 
        /// </summary>
        void analizarTablasDestino()
        {
            foreach (ElementConnectorInfo tablaDestinoInfo in this.tablasAsociadasComoDestinoListInfo)
            {
                analizarTablaDestino(tablaDestinoInfo.element);
            }
        }

        void analizarTablaDestino( Element tablaDestino )
        {
            /*
            List<ElementConnectorInfo> clasesQueImplementanListInfo = generator.classRealization(tablaDestino);
            foreach( ElementConnectorInfo claseQueImplementaInfo  in clasesQueImplementanListInfo )
            {
                analizarClaseRealizadaPorTablaDestino(claseQueImplementaInfo, tablaDestino);
            }
            */
        }
        void analizarTablaOrigen( Element tablaOrigen )
        {
            // buscamos las clases que se implementan la tabla relacionada con la sujeta a análisis.
            List<ElementConnectorInfo> clasesQueImplementanListInfo = generator.classRealization( tablaOrigen );

            if(clasesQueImplementanListInfo.Count != 0)
            {
                foreach (ElementConnectorInfo claseQueImplementaInfo in clasesQueImplementanListInfo)
                {
                    analizarClaseRealizadaPorTablaOrigen(claseQueImplementaInfo.element, tablaOrigen);
                }
            }
            else
            {
                // si una tabla no tiene una clase que la realiza es probable que esta sea producto de una relación de muchos a muchos
                // es ese caso hay que buscar las tablas asociadas a esta que son destino de la relación.
                List < ElementConnectorInfo > tablasCandidatasRelacionMuchosMuchosListInfo = eaUtils.connectorUtils.get(tablaOrigen, EAUtils.ConnectorUtils.CONNECTOR__ASSOCIATION, null, null, "table", false, null);
                foreach( ElementConnectorInfo tablaCandidataRelacionMuchosMuchosListInfo in tablasCandidatasRelacionMuchosMuchosListInfo )
                {
                    clasesQueImplementanListInfo = generator.classRealization(tablaCandidataRelacionMuchosMuchosListInfo.element);
                    // si no hay quien la realice entonces puede ser una tabla de muchos a muchos

                    foreach (ElementConnectorInfo claseQueImplementaInfo in clasesQueImplementanListInfo)
                    {
                        analizarClaseRealizadaPorTablaOrigen(claseQueImplementaInfo.element, tablaCandidataRelacionMuchosMuchosListInfo.element);
                    }
                }
            }
        }

        /// <summary>
        /// La clase implementada por la tabla que es origen de la relación de asociación debe tener una 
        /// relación con la clase que es implementada por la tabla que se está analizando, esta relación
        /// será la que nos permita determinar el tipo de relación (uno a uno, uno a muchos, muchos a muchos)
        /// también podría estar definiendo el nombre del atributo.
        /// 
        /// </summary>
        /// <param name="claseQueImplementaInfo"></param>
        void analizarClaseRealizadaPorTablaOrigen(Element claseQueImplementa, Element tablaQueImplementa)
        {
            FkEntity fkEntity = null;
            Info info = null;
            Cardinality cardinalitySupllier;
            Cardinality cardinalityClient;

            this.eaUtils.printOut("Analizando relación con "+ claseQueImplementa.Name );

            foreach ( Connector connectorClaseQueImlpementa in claseQueImplementa.Connectors )
            {
                foreach( ElementConnectorInfo claseImplementaTablaEnAnalisisInfo in clasesImplementanTablaEnAnalisisListInfo )
                {
                    if (
                            //claseQueImplementa.ElementID != claseImplementaTablaEnAnalisisInfo.element.ElementID 
                            //&&
                            (
                                ( connectorClaseQueImlpementa.Direction == EAUtils.ConnectorUtils.DIRECTION__BI_DIRECTIONAL 
                                  && ( claseImplementaTablaEnAnalisisInfo.element.ElementID == connectorClaseQueImlpementa.ClientID
                                       ||
                                       claseImplementaTablaEnAnalisisInfo.element.ElementID == connectorClaseQueImlpementa.SupplierID
                                     )
                                )
                                || claseImplementaTablaEnAnalisisInfo.element.ElementID == connectorClaseQueImlpementa.ClientID 
                            )

                        )
                    {
                        fkEntity = new FkEntity();

                        info = new Info();

                        this.generator.lookingArtifactsNamespace( info, tablaQueImplementa );

                        fkEntity.@namespace           = info.getFullNamespace();
                        fkEntity.namespacesByArtifact = info.getNamespacesByArtifact();
                        fkEntity.entity               = claseQueImplementa;
                        fkEntity.pluralAlias          = this.eaUtils.taggedValuesUtils.getPluralAlias( claseQueImplementa, "").asString();
                        fkEntity.pluralName           = this.eaUtils.taggedValuesUtils.getPluralName(  claseQueImplementa, "").asString();
                        fkEntity.isRelationSource     = claseQueImplementa.ElementID == connectorClaseQueImlpementa.ClientID;
                        fkEntity.table                = tablaQueImplementa;
                        cardinalitySupllier = new Cardinality(connectorClaseQueImlpementa.SupplierEnd.Cardinality);
                        cardinalityClient   = new Cardinality(connectorClaseQueImlpementa.ClientEnd.Cardinality);

                        if ( cardinalitySupllier.isCollection())
                        {
                            if (fkEntity.relationName == "")
                            {
                                connectorClaseQueImlpementa.SupplierEnd.Role = StringUtils.toCamel(fkEntity.pluralName);
                                fkEntity.entity.Connectors.Refresh();
                            }

                            this.mainInfo.addCollection(fkEntity, cardinalityClient.isCollection());
                        }
                        else
                        {
                            if (fkEntity.relationName == "")
                            {
                                connectorClaseQueImlpementa.SupplierEnd.Role = StringUtils.toCamel(claseQueImplementa.Name);
                                fkEntity.entity.Connectors.Refresh();
                            }

                            this.mainInfo.addEntity(fkEntity);
                        }

                        fkEntity.relationName = connectorClaseQueImlpementa.SupplierEnd.Role;
                    }
                }
            }
        }
    }
}

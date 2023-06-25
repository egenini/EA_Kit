using EA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EAUtils
{
    public class ElementUtils
    {
        const string HEREDAN_DE_MI_QUERY = "select start_object_id from t_connector where connector_type = 'Generalization' and end_object_id = ";

        public Repository repository;
        private EAUtils eaUtils;

        public ElementUtils(Repository repository)
        {
            this.repository = repository;
        }
        public void setEaUtils( EAUtils eaUtils)
        {
            this.eaUtils = eaUtils;
        }

        /// <summary>
        /// Uso interno.
        /// Crea una instancia del elemento con el mismo tipo y nombre que el clasificador.
        /// </summary>
        /// <param name="elementoClasificador"></param>
        /// <param name="parentId">El id del elemento parent (elemento o paquete) </param>
        /// <param name="intanceStereoType"></param>
        /// <param name="diagram"></param>
        /// <returns></returns>
        private Element createInstance(EA.Element elementoClasificador, int parentId, string intanceStereoType, Diagram diagram)
        {
            return createInstance(null, elementoClasificador, parentId, intanceStereoType, diagram);
        }

        private Element createInstance(string nombre, EA.Element elementoClasificador, int parentId, string instanceStereoType, Diagram diagram)
        {
            Element instancia = null;
            // control de instancia de instancia
            
            if (elementoClasificador.ClassifierID == 0)
            {
                instancia = elementoClasificador.Clone();
                instancia.Name = ( nombre == null || nombre.Length == 0 ? elementoClasificador.Name : nombre);
                
                if(instanceStereoType != null && instanceStereoType.Length != 0 )
                {
                    instancia.Stereotype = instanceStereoType;
                }

                try
                {
                    instancia.ParentID = parentId;
                }catch(Exception) { }

                instancia.ClassifierID = elementoClasificador.ElementID;

                instancia.Update();

                this.eaUtils.connectorUtils.settingConnectorsForInstanceElement(instancia, elementoClasificador, diagram);
            }
            return instancia;
        }

        /// <summary>
        /// Crea una instancia del mismo tipo que el clasificador y le agrega una relación de instanciación.
        /// </summary>
        /// <param name="nombre">Nombre de la instacia, puede ser null o ""</param>
        /// <param name="elementoClasificador"></param>
        /// <param name="parent">El paquete al cual pertenece la instancia a crear </param>
        /// <param name="instanceStereoType">El estereotipo que se le asigna a la instancia, puede ser null o ""</param>
        /// <param name="diagram"></param>
        /// <returns>El elemento instancia creado</returns>
        public Element createInstance(string nombre, EA.Element elementoClasificador, Package parent, string instanceStereoType, Diagram diagram)
        {
            return createInstance(nombre, elementoClasificador, parent.PackageID, instanceStereoType, diagram);
        }

        /// <summary>
        /// Crea una instancia del mismo tipo que el clasificador y le agrega una relación de instanciacion y nesting.
        /// </summary>
        /// <param name="nombre">Nombre de la instacia, puede ser null o ""</param>
        /// <param name="elementoClasificador"></param>
        /// <param name="parent">El elemento al cual pertenece la instancia a crear </param>
        /// <param name="instanceStereoType">El estereotipo que se le asigna a la instancia, puede ser null o ""</param>
        /// <param name="diagram"></param>
        /// <returns>El elemento instancia creado</returns>
        public Element createInstance(string nombre, EA.Element elementoClasificador, Element parent, string instanceStereoType, Diagram diagram)
        {
            Element instancia = createInstance(nombre, elementoClasificador, parent.ElementID, instanceStereoType, diagram);
            eaUtils.connectorUtils.addConnectorNesting(instancia, parent, null, null);
            return instancia;
        }

        public string enumeration2Csv( Element enumeration, bool useAlias )
        {
            string csv = "";
            //string attrChar = "\"";

            foreach (EA.Attribute attr in enumeration.Attributes )
            {
                csv += (useAlias ? attr.Alias : attr.Name) + ",";
                //csv +=  attrChar + ( useAlias ? attr.Alias : attr.Name ) + attrChar + ",";
            }
            return csv.Substring(0, csv.Length - 1);

        }

        public Element addDefect(Element element, string name, string notes)
        {
            Element defect = element.Elements.AddNew(name, "Defect");
            defect.Notes = notes;
            defect.Update();

            eaUtils.connectorUtils.addConnectorDependency(defect, element, null, null);

            element.Elements.Refresh();
            return defect;
        }

        public Element add(Package package, string name, string type, string stereotype, string technology)
        {
            Element newElement = package.Elements.AddNew(name, type);

            if (stereotype != null && stereotype.Length != 0)
            {
                if( technology != null)
                {
                    newElement.StereotypeEx = technology + "::" + stereotype;
                }else
                {
                    newElement.Stereotype = stereotype;
                }
            }
            newElement.Update();
            package.Elements.Refresh();

            return newElement;
        }

        public Element add(Element parent, string name, string type, string stereotype)
        {
            Element newElement = parent.Elements.AddNew(name, type);

            if (stereotype != null && stereotype.Length != 0)
            {
                newElement.Stereotype = stereotype;
            }
            newElement.Update();
            parent.Elements.Refresh();

            return newElement;
        }

        public Element synchronize( Package package, string name, string type, string stereotype, string technology)
        {
            Element element = null;
            foreach( Element current in package.Elements)
            {
                if( current.Name == name)
                {
                    element = current;
                    break;
                }
            }

            if( element == null)
            {
                element = add(package, name, type, stereotype, technology);
            }
            return element;
        }

        public Element synchronize(Element parent, string name, string type, string stereotype)
        {
            Element element = null;
            foreach (Element current in parent.Elements)
            {
                if (current.Name == name)
                {
                    element = current;
                    break;
                }
            }

            if (element == null)
            {
                element = add(parent, name, type, stereotype);
            }
            return element;
        }

        public bool delete( Element element )
        {
            bool    deleted = false;
            Package package = null;
            Element parent  = null;

            if ( element.PackageID != 0)
            {
                package = repository.GetPackageByID(element.PackageID);
                short index = 0;
                foreach( Element currentElement in package.Elements)
                {
                    if( currentElement.ElementID == element.ElementID)
                    {
                        package.Elements.DeleteAt(index, false);
                        deleted = true;
                        break;
                    }
                    else
                    {
                        index++;
                    }
                }
            }

            if( ! deleted )
            {
                if( element.ParentID != 0 )
                {
                    parent = repository.GetElementByID(element.ParentID);
                    short index = 0;
                    foreach( Element currentElement in parent.Elements)
                    {
                        if (currentElement.ElementID == element.ElementID)
                        {
                            parent.Elements.DeleteAt(index, false);
                            deleted = true;
                            break;
                        }
                        else
                        {
                            index++;
                        }

                    }
                }
            }
            if( deleted)
            {
                if( parent != null)
                {
                    parent.Elements.Refresh();
                }
                else
                {
                    package.Elements.Refresh();
                }
            }
            return deleted;
        }

        public Element search( string name, Collection elements, bool recursive)
        {

            Element found = null;

            foreach ( Element element in elements)
            {
                if( element.Name == name)
                {
                    found = element;
                    break;
                }
            }

            if ( recursive && found == null)
            {
                foreach (Element element in elements)
                {
                    found = this.search(name, element.Elements, recursive);

                    if( found != null)
                    {
                        break;
                    }
                }
            }

            return found;
        }

        public List<object> InstanciasDe(int clasificadorId)
        {
            ResultAsList results = (ResultAsList)this.eaUtils.sqlUtils.excecute("select Object_ID from t_object where Classifier = " + clasificadorId);
            List<object> instancias = new List<object>(results.data.Count);
            List<List<object>> rows = results.data;

            foreach (List<object> row in rows )
            {
                instancias.Add( this.repository.GetElementByID( int.Parse( (string)row[0] ) ) );
            }
            return instancias;
        }

        public List<int> heredanDeMi(int id)
        {
            ResultAsList results = (ResultAsList)this.eaUtils.sqlUtils.excecute( HEREDAN_DE_MI_QUERY + id );
            List<int> herederos = new List<int>(results.data.Count);
            List<List<object>> rows = results.data;

            foreach (List<object> row in rows)
            {
                herederos.Add( int.Parse((string)row[0] ) );  
            }
            return herederos;

        }
    }
}

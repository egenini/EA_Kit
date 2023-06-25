///////////////////////////////////////////////////////////
//  Operation.cs
//  Implementation of the Class Operation
//  Generated by Enterprise Architect
//  Created on:      28-jul.-2017 13:59:40
//  Original author: Edgardo
///////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace Productividad.modelo
{

    /// <summary>
    /// Definici�n de la operaci�n para el path.
    /// </summary>
    public class Operation
    {

        /// <summary>
        /// este atributo es agregado para facilitar la l�gica.
        /// </summary>
        public string _name;

        /// <summary>
        /// Un breve resumen de lo que hace la operaci�n. Para la m�xima legibilidad en el
        /// swagger-ui, este campo DEBE ser menor de 120 caracteres.
        /// </summary>
        public string summary;
        /// <summary>
        /// Una explicaci�n detallada del comportamiento de la operaci�n. La <a href="$inet:
        /// //https://guides.github.com/features/mastering-markdown/#GitHub-flavored-
        /// markdown"><font color="#0000ff"><u>sintaxis de GFM</u></font></a> se puede
        /// utilizar para la representaci�n de texto enriquecido.
        /// </summary>
        public string description;
        /// <summary>
        /// Cadena �nica utilizada para identificar la operaci�n. El ID DEBE ser �nico
        /// entre todas las operaciones descritas en la API. Herramientas y bibliotecas
        /// PUEDO utilizar el operationId para identificar de forma exclusiva una operaci�n,
        /// por lo tanto, se recomienda seguir las convenciones comunes de nomenclatura de
        /// programaci�n.
        /// </summary>
        public string operationId;
        /// <summary>
        /// Una lista de tipos MIME que la operaci�n puede consumir. Esto anula la
        /// definici�n consumes en el objeto Swagger. Un valor vac�o PUEDE ser utilizado
        /// para borrar la definici�n global. El valor DEBE ser como se describe en <a
        /// href="$inet://https://swagger.io/specification/#mimeTypes"><font
        /// color="#0000ff"><u>Mime Types</u></font></a>.
        /// </summary>
        public List<string> consumes = new List<string>();
        /// <summary>
        /// Una lista de tipos MIME que puede producir la operaci�n. Esto anula la
        /// definici�n produce en el objeto Swagger. Un valor vac�o PUEDE ser utilizado
        /// para borrar la definici�n global. El valor DEBE ser como se describe en <a
        /// href="$inet://"><font color="#0000ff"><u>Mime Types</u></font></a>.
        /// </summary>
        public List<string> produces = new List<string>();
        /// <summary>
        /// El protocolo de transferencia para la operaci�n. Los valores DEBEN ser de la
        /// lista: "http", "https", "ws", "wss". El valor reemplaza la definici�n de
        /// esquemas de Swagger Object.
        /// </summary>
        public List<string> schemes;
        /// <summary>
        /// Declara que esta operaci�n est� obsoleta. El uso de la operaci�n declarada debe
        /// ser refrained. El valor predeterminado es false.
        /// </summary>
        public Boolean deprecated = false;
        public List<Parameter> parameters = new List<Parameter>();
        public Tag tags;
        public ExternalDoc externalDocs;

        /// <summary>
        /// Todas las respuestas son c�digos http salvo en el caso de un ejemplo donde es un mimeType.
        /// </summary>
        public Dictionary< string, Response> responses = new Dictionary<string, Response>();
        public SecurityRequirement security;

        public Response currentResponse; 

        public Operation()
        {

        }
        public Parameter addParameter(string name, string type)
        {
            Parameter parametro = new Parameter(name, type);
            parameters.Add(parametro);
            return parametro;
        }

        public Response addResponse( string httpStatusCode, EA.Element element, Boolean isCollection, string description)
        {
            this.currentResponse = new Response(httpStatusCode);
            this.currentResponse.description = description;
            if ( isCollection )
            {
                this.currentResponse.setSchema();
                this.currentResponse.schema.type = "array";
                this.currentResponse.schema.@ref = element.Name;
            }
            else
            {
                this.currentResponse.@ref = element.Name;
            }

            responses.Add(httpStatusCode, currentResponse);

            return this.currentResponse;
        }
    }
}
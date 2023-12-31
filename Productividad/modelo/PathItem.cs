///////////////////////////////////////////////////////////
//  PathItem.cs
//  Implementation of the Class PathItem
//  Generated by Enterprise Architect
//  Created on:      28-jul.-2017 13:59:43
//  Original author: Edgardo
///////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace Productividad.modelo
{

    /// <summary>
    /// Una ruta relativa a un punto final individual. El nombre del campo DEBE
    /// comenzar con una barra. La ruta de acceso se anexa a la basePath para construir
    /// la URL completa. Se permite la plantilla de trayectoria.
    /// </summary>
    public class PathItem {

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
        /// Permite una definici�n externa de este elemento de ruta de acceso. La
        /// estructura de referencia DEBE estar en el formato de un objeto de elemento de
        /// ruta de acceso. Si hay conflictos entre la definici�n de referencia y la
        /// definici�n de este elemento de ruta, el comportamiento es indefinido.
        /// </summary>
        public string path; 
        public Operation get = null;
	    public Operation put = null;
        public Operation post = null;
        public Operation delete = null;
        public Operation options = null;
        public Operation head = null;
        public Operation patch = null;
        public List< Parameter> parameters = new List<Parameter>();
        public Operation currentOperation = null;

        public PathItem(string path)
        {
            this.path = path;
        }

        public PathItem()
        {
        }

        public void setMethod(string method, string operationId)
        {
            switch (method)
            {
                case "get":
                    this.get = new Operation();
                    this.get.operationId = operationId;
                    this.get._name = "get";
                    this. currentOperation = this.get;
                    break;
                case "delete":
                    this.delete = new Operation();
                    this.delete.operationId = operationId;
                    this.delete._name = "delete";
                    this.currentOperation = this.delete;
                    break;
                case "head":
                    this.head = new Operation();
                    this.head.operationId = operationId;
                    this.head._name = "head";
                    this.currentOperation = this.head;
                    break;
                case "options":
                    this.options = new Operation();
                    this.options.operationId = operationId;
                    this.options._name = "options";
                    this.currentOperation = this.options;
                    break;
                case "patch":
                    this.patch = new Operation();
                    this.patch.operationId = operationId;
                    this.patch._name = "patch";
                    this.currentOperation = this.patch;
                    break;
                case "post":
                    this.post = new Operation();
                    this.post.operationId = operationId;
                    this.post._name = "post";
                    this.currentOperation = this.post;
                    break;
                case "put":
                    this.put = new Operation();
                    this.put.operationId = operationId;
                    this.put._name = "put";
                    this.currentOperation = this.put;
                    break;

            }
        }

        public Parameter addParameter(string name, string type)
        {
            Parameter parametro = new Parameter( name, type);
            parameters.Add(parametro);
            return parametro;
        }
}//end PathItem
}

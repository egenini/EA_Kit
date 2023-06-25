///////////////////////////////////////////////////////////
//  Parameter.cs
//  Implementation of the Class Parameter
//  Generated by Enterprise Architect
//  Created on:      28-jul.-2017 13:59:41
//  Original author: Edgardo
///////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace RestFul.modelo
{
    /// <summary>
    /// Objeto que contiene parámetros que se pueden utilizar en todas las operaciones.
    /// Esta propiedad no define parámetros globales para todas las operaciones.
    /// </summary>
    public class Parameter
    {
        public string name;
        public string type;
        public In paramenterIn;
        public string description = null;
        public Boolean required = false;

        public Parameter(string name, string type) : this(name, type, null, false)
        {
        }
        public Parameter(string name, string type, string description) : this(name, type, description, false)
        {
        }
        public Parameter(string name, string type, Boolean required) : this(name, type, null, required)
        {
        }

        public Parameter( string name, string type, string description, Boolean required)
        {
            this.name = name;
            this.type = type;
            this.description = description;
            this.required = required;
        }

        public void inType(string inType)
        {
            paramenterIn = new In(inType);
        }
    }
}

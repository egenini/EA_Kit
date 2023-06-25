///////////////////////////////////////////////////////////
//  Item.
//  Implementation of the Class Item
//  Generated by Enterprise Architect
//  Created on:      30-jul.-2017 12:31:03
//  Original author: Edgardo
///////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;



namespace Productividad.modelo
{
	public class Item : JsonSchemaCommon
    {

        /// <summary>
        /// Valores separados por comas foo,var.
        /// </summary>
        public static string COLLECTION_FORMAT__CSV = "csv";
        /// <summary>
        /// Valores separados por espacio foo var.
        /// </summary>
        public static string COLLECTION_FORMAT__SSV = "ssv";
        /// <summary>
        /// Valores separados por tabulación foo\var.
        /// </summary>
        public static string COLLECTION_FORMAT__TSV = "tsv";
        /// <summary>
        /// Valores separados por pipes foo|var.
        /// </summary>
        public static string COLLECTION_FORMAT__PIPES = "pipes";

        public static string[] COLLECTION_FORMATS =
        {
            COLLECTION_FORMAT__CSV,
            COLLECTION_FORMAT__SSV,
            COLLECTION_FORMAT__TSV,
            COLLECTION_FORMAT__PIPES
        };


		/// <summary>
		/// Determina el formato de la matriz si se utiliza matriz de tipo. Los valores
		/// posibles son:
		/// </summary>
		public string collectionFormat = null;
        /// <summary>
        /// Describe el tipo de elementos de la matriz.
        /// </summary>
        public List<Item> items = new List<Item>();

        public Item(){

		}
	}
}
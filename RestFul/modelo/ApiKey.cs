///////////////////////////////////////////////////////////
//  ApiKey.cs
//  Implementation of the Class ApiKey
//  Generated by Enterprise Architect
//  Created on:      28-jul.-2017 13:59:24
//  Original author: Edgardo
///////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;



public class ApiKey : SecurityScheme {

	public APIKeyInValues where; // en el modelo este atributo se llama in pero no se puede usar porque es palabra reservada.
	/// <summary>
	/// El nombre del parámetro de encabezado o consulta que se va a utilizar.
	/// </summary>
	public string name;

	public ApiKey(){

	}

	~ApiKey(){

	}

}//end ApiKey
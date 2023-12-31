///////////////////////////////////////////////////////////
//  ExternalDoc.cs
//  Implementation of the Class ExternalDoc
//  Generated by Enterprise Architect
//  Created on:      28-jul.-2017 13:59:34
//  Original author: Edgardo
///////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;



/// <summary>
/// Una lista de etiquetas utilizadas por la especificación con metadatos
/// adicionales. El orden de las etiquetas se puede utilizar para reflejar en su
/// orden por las herramientas de análisis. No se deben declarar todas las
/// etiquetas utilizadas por el objeto Operación. Las etiquetas que no se declaran
/// pueden organizarse aleatoriamente o basarse en la lógica de las herramientas.
/// Cada nombre de etiqueta en la lista DEBE ser único.
/// </summary>
public class ExternalDoc {

	/// <summary>
	/// Una breve descripción de la documentación de destino. La <a href="https:
	/// //guides.github.com/features/mastering-markdown/#GitHub-flavored-
	/// markdown"><font color="#0000ff"><u>sintaxis de GFM</u></font></a> se puede
	/// utilizar para la representación de texto enriquecido.
	/// </summary>
	public string description;
	/// <summary>
	/// La URL de la documentación de destino. El valor DEBE estar en el formato de una
	/// URL.
	/// </summary>
	public string url;

	public ExternalDoc(){

	}

	~ExternalDoc(){

	}

}//end ExternalDoc
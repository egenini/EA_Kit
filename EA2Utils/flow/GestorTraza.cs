using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EAUtils.flow
{
    public class GestorTraza
    {
        List<Traza> trazas = new List<Traza>();
        Traza traza = null;

        public void abrirSubtraza()
        {
            traza = trazas.Last();
        }
        public void cerrarSubtraza()
        {
            traza = null;
        }
        public void agregar(EA.Element element)
        {
            if( traza == null )
            {
                trazas.Add(new Traza(element.ElementGUID, element.Name, element.Notes));
            }
            else
            {
                traza.trazas.Add(new Traza(element.ElementGUID, element.Name, element.Notes));
            }
        }
        public string asString()
        {
            return JsonConvert.SerializeObject(trazas, Formatting.Indented);
        }

        public void fromString( string json)
        {
            trazas = JsonConvert.DeserializeObject<List<Traza>>(json);
        }
    }
}

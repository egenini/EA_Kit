using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAUtils.flow
{
    public class Traza
    {
        public List<Traza> trazas = new List<Traza>();
        public string guid;
        public string nombre;
        public string notas;

        public Traza(string guid, string nombre, string notas) 
        { 
            this.guid = guid;
            this.nombre = nombre;
            this.notas = notas;
        }
    }
}

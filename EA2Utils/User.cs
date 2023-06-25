using System;
using System.IO.Pipes;

namespace EAUtils
{
    public class Usuario
    {
        public const string ANONYMOUS = "Anonymous";

        public string apellido;
        public string departamento;
        public string id;
        public string login;
        public string nombre;

        public Usuario()
        {
        }

        public string autor()
        {
            return nombre + " " + apellido;
        }

        public string presentacion()
        {
            return apellido + ", " + nombre;
        }

        public bool esElMismo( string userItem )
        {
            return this.presentacion() == userItem;
        }

        public string presentacionDesdeAutor(string autorConsultado)
        {
            return this.autor() == autorConsultado ? this.presentacion() : null;
        }

        public static string usuarioNombreOAnonimous( EAUtils eaUtils )
        {
            string nombre = ANONYMOUS;

            if (eaUtils.repository.IsSecurityEnabled)
            {
                nombre = eaUtils.repository.GetCurrentLoginUser(false);
            }

            return nombre;
        }
    }
}
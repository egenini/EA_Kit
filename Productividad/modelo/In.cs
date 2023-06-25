using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Productividad.modelo
{
    public class In
    {
        public static string IN__QUERY     = "query";
        public static string IN__HEADER    = "header";
        public static string IN__PATH      = "path";
        public static string IN__FORM_DATA = "formData";
        public static string IN__BODY      = "body";
        public static string[] IN = { IN__BODY, IN__FORM_DATA, IN__HEADER, IN__PATH, IN__QUERY };

        public string inType;

        public ParameterInBody    inBody    = null;
        public ParameterNotInBody notInBody = null;

        public In( string inType )
        {

            if( inType == IN__BODY )
            {
                this.inType = inType;
                inBody = new ParameterInBody();
            }
            else
            {
                this.inType = inType;

                notInBody = new ParameterNotInBody();
            }
        }

        public bool isInBody()
        {
            return inType == IN__BODY;
        }
    }
}

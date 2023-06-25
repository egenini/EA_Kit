using EAUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model2Table
{
    public class PropertiesTabBuilder
    {
        public PropertiesTabBuilder()
        {

        }
        public PropertiesTabUtil build( EA.Element e )
        {
            PropertiesTabUtil prop = new PropertiesTabUtil();

            prop.addGroup(e.Name).addProperty("Table", "nombre tabla", "");

            return prop;
        }
    }
}

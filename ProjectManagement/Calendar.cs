using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ProjectManagement
{
    public class Calendar
    {
        const string MDA = "MMddyyyy";

        List<string> noLaborables = new List<string>();

        EAUtils.EAUtils eaUtils;
        public Calendar( EAUtils.EAUtils eaUtils)
        {
            this.eaUtils = eaUtils;
        }

        public void load()
        {
            var sql = "SELECT strcontent FROM t_document WHERE DocType = 'event_calendar' and StrContent like '%eventtype=\"Feriados\"%'";

            string queryResult = this.eaUtils.repository.SQLQuery(sql);

            if (queryResult.Length > 0)
            {
                XmlDocument xmlDOM = new XmlDocument();
                try
                {
                    xmlDOM.LoadXml(queryResult);

                    XmlNodeList nodeList = xmlDOM.DocumentElement.SelectNodes("/EADATA/Dataset_0/Data/Row/strcontent");

                    //XmlNodeList rowList = rootNode.SelectNodes("/Row");

                    foreach (XmlNode node in nodeList)
                    {
                        XmlDocument xmlChild = new XmlDocument();
                        xmlChild.LoadXml(node.ChildNodes[0].InnerText);
                        string attrValue = xmlChild.DocumentElement.SelectSingleNode("/calendar/appointment/time").Attributes.GetNamedItem("start").Value;

                        noLaborables.Add(DateTime.Parse(attrValue).ToString(MDA));
                    }
                }
                catch (Exception w) { w.ToString(); }
            }
        }

        public DateTime moverSi( DateTime source )
        {
            int cantidad = this.mover(source);

            return cantidad == 0 ? source : source.AddDays(cantidad);
        }

        public DateTime moverSi(DateTime primerDia, DateTime ultimoDia, int days)
        {
            // obtener la cantidad de días que se necesitan mover desde el día siguiente al último
            int cantidadTotal  = 0;
            int cantidadActual = 0;

            for( int i = 1; i <= days; i++ )
            {
                cantidadActual = mover(primerDia.AddDays(1));
                
                if( cantidadActual != 0)
                {
                    cantidadTotal += cantidadActual;
                    primerDia      = primerDia.AddDays(cantidadActual);
                }
            }
            return cantidadTotal == 0 ? moverSi(ultimoDia) : moverSi(ultimoDia.AddDays(cantidadTotal));
        }

        private int mover( DateTime source)
        {
            int cantidad = 0;

            if ((source.DayOfWeek == DayOfWeek.Saturday))
            {
                cantidad = 2;
            }
            else if ((source.DayOfWeek == DayOfWeek.Sunday))
            {
                cantidad = 1;
            }

            // vemos si es feriado
            if (noLaborables.Contains(source.ToString(MDA)))
            {
                cantidad += 1;
            }
            return cantidad;
        }
    }
}

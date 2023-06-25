using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EA;
using EAUtils;
using UserInterface.frw;
using System.IO;

namespace UserInterface.html
{
    public class Table : HtmlCommon
    {
        public List<List<string>> rows = new List<List<string>>();

        public Table(Element element, Framework frameworkInstance, EAUtils.EAUtils eaUtils) : base(element, frameworkInstance, eaUtils)
        {
            this.setProperties();
        }

        public new void setProperties()
        {
            base.setProperties();

            string notes = this.eaUtils.taggedValuesUtils.get(this.element, "data", "").asString();
            string txt = this.eaUtils.notes2Txt(notes);
            StringReader stringReader = new StringReader(txt);
            string line;
            List<string> row;
            string[] lineSplitted;

            while ((line = stringReader.ReadLine()) != null)
            {

                row = new List<string>();

                lineSplitted = line.Split(';');

                foreach (string col in lineSplitted)
                {
                    row.Add(col.Trim());
                }
                rows.Add(row);
            }
        }
    }
}

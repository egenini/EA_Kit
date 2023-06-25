using EA;
using EAUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistencia.classtable
{
    public class Class2Table
    {
        protected EAUtils.EAUtils eaUtils;
        protected Element table;
        protected Attribute2Column attribute2Column;
        protected Framework framework;

        public Class2Table(Repository repository, Element table)
        {
            this.table   = table;
            this.eaUtils = new EAUtils.EAUtils();

            this.eaUtils.setRepositorio(repository);

            this.attribute2Column = new Attribute2Column(repository, table);
        }

        public void toTable(Element toTable)
        {
            eaUtils.repository.BatchAppend = true;

            this.framework = new Framework(eaUtils );

            table.Name = this.buildTableName(table.Name, toTable.Name);
            // todo: buscar como era eso de normalizar las notas de un elemento.
            table.Notes = toTable.Notes;

            table.Update();

            ModelClass2RealClass modelToClass = new ModelClass2RealClass(this.eaUtils);

            modelToClass.go(toTable);

            attribute2Column.classOwner = toTable;

            foreach ( AttributeInfo attrinfo in modelToClass.realClass.getAttributesInfo() )
            {
                attribute2Column.toColumn(attrinfo);
            }

            table.Attributes.Refresh();

            eaUtils.repository.BatchAppend = false;
        }

        public string buildTableName(string name, string mayBeNewName)
        {
            return name.ToLower().Contains("table") ? this.framework.toCase(mayBeNewName) : name;
        }
    }
}

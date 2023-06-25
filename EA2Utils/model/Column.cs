using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAUtils.model
{
    public class Column
    {
        public string Guid { set; get; }
        public string Name { set; get; }
        public string DataType { set; get; }

        public Column( EA.Attribute column, EAUtils eaUtils)
        {
            if (column.Stereotype == "column")
            {
                Guid = column.AttributeGUID;
                DataType = column.Type;
                Name = column.Name;
            }
        }
    }
}

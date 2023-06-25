using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model2Table.model
{
    public class Model
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }

        public List<Attr> Attributes { get; set; }
        public Model(EA.Element e)
        {
            this.Name = e.Name;
            this.Notes = e.Notes;
         
        }
        public Attr find( int id)
        {
            return Attributes.FirstOrDefault(x => x.Id == id);
        }
        public void addAttributes( EA.Element e)
        {
            foreach (EA.Attribute a in e.Attributes)
            {
                Attributes.Add( new Attr(a) );
            }
        }
    }

    public class Table: Model
    {
        public Table(EA.Element e) : base(e) 
        { 
        }    
    }
    public class Attr
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public string Type { get; set; }
        public string Default { get; set; }

        public Attr(EA.Attribute a)
        {
            this.Name = a.Name;
            this.Notes= a.Notes;
            this.Type = a.Type;
        }
    }

    public class Column : Attr
    {
        public bool Nullable { get; set; }
        public string Length { get; set; }
        public string Precision { get; set; }
        public string Scale { get; set; }
        public bool PK { get; set; }


        public Column(EA.Attribute a) : base(a)
        {
            Nullable  = a.AllowDuplicates;
            Length    = a.Length;
            Precision = a.Precision;
            Scale     = a.Scale;
            PK        = a.IsOrdered;

        }
    }
}

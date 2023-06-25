using EA;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using UIResources;

namespace EAUtils
{
    public class Json2EAClass
    {
        JObject json;
        EAUtils eaUtils;
        EA.Package package;

        public void go(EA.Element element, EAUtils eaUtils)
        {
            this.eaUtils = eaUtils;
            this.package = eaUtils.repository.GetPackageByID( element.PackageID );

            try
            {
                json = Newtonsoft.Json.Linq.JObject.Parse(element.Notes);

                this.walk(json.Children(), element);

            }catch(Exception e)
            {
                Clipboard.SetText(e.ToString());
                Alert.Error("Error disponible en el porta papeles");
            }
        }

        private void walk(JEnumerable<JToken> jEnumerable, Element element)
        {
            foreach (dynamic child in jEnumerable)
            {
                if (child.Type == JTokenType.Property)
                {
                    if (child.Value.Type == JTokenType.Array)
                    {
                        buildClass(child, element);

                        //walkArray(child.Value, element);
                    }
                    else if (child.Value.Type == null )
                    {
                        buildClass(child, element);
                    }
                    else
                    {
                        // crear atributo
                        this.addAttribute(child, element);
                    }
                }
                else if (child.Type == JTokenType.Array)
                {
                    this.walk(child.Children(), element);
                    //walkArray(child, element);
                }
                else if (child.Type == null || child.Type == JTokenType.Object)
                {
                    // crear clase
                    buildClass(child, element);
                }
            }
        }

        

        private void buildClass(dynamic child, Element element)
        {
            if( child.Name == null)
            {
                this.walk(child.Children(), element);
            }
            else
            {
                string cname = child.Name; // por algún motivo si llamo por 2da vez a child.Name tira error.

                cname = cname.ToLower();

                Element newElement = package.Elements.AddNew(StringUtils.toPascal(cname.ToLower()), "Class");

                newElement.StereotypeEx = "DAL::DomainClass";

                newElement.Update();

                this.eaUtils.taggedValuesUtils.set(newElement, "useJsonAlias", "true");

                this.eaUtils.taggedValuesUtils.setJsonAlias(newElement, cname);

                package.Elements.Refresh();

                this.walk(child.Children(), newElement);

            }
        }

        private void addAttribute(dynamic jtoken, Element element)
        {
            string jname = jtoken.Name;

            jname = jname.ToLower();

            EA.Attribute attr = element.Attributes.AddNew( StringUtils.toCamel(jname), this.getType(jtoken.Value) );

            attr.StereotypeEx = "DAL::ExtendedAttribute";
            attr.Update();

            this.eaUtils.taggedValuesUtils.setJsonAlias( attr, jname );

            var value = jtoken.Value.Value;

            if (value != null)
            {
                this.eaUtils.taggedValuesUtils.setExample(attr, value.ToString());
            }

            element.Attributes.Refresh();

        }

        private string getType(dynamic value)
        {
            string type = "";
            try
            {
                if (value == JTokenType.Boolean)
                {
                    type = "boolean";
                }
                else if (value == JTokenType.String)
                {
                    type = "String";
                }
                else if (value == JTokenType.Integer)
                {
                    type = "int";
                }
                else if (value == JTokenType.Float)
                {
                    type = "float";
                }
                else if (value == JTokenType.Date)
                {
                    type = "Timestamp";
                }

            }
            catch (Exception)
            {
                type = this.getType(value.Type);
            }
            return type;
        }
    }
}

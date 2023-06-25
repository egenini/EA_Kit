﻿using System;
using EA;
using System.Windows.Forms;
using UIResources;
using EAUtils;

namespace Productividad
{
    internal class BuilderManager
    {
        private Element enumElement;
        private EAUtils.EAUtils eaUtils;
        private string varType = null;
        private bool? useJsonAlias = false;

        public BuilderManager(Repository repository, Element enumElement)
        {
            eaUtils = new EAUtils.EAUtils();
            eaUtils.setRepositorio(repository);
            this.enumElement = enumElement;
        }

        public void go()
        {
            if( this.enumElement.Gentype.ToLower() == "java")
            {
                string code = "";
                
                if( enumElement.Type == "Class")
                {
                    useJsonAlias = eaUtils.taggedValuesUtils.get(enumElement, "useJsonAlias", "false").asBoolean();

                    if (this.enumElement.Attributes.Count != 0)
                    {
                        code += "public class " + this.enumElement.Name + " {\r\n\r\n";

                        short i = 0;
                        foreach (EA.Attribute attr in this.enumElement.Attributes)
                        {
                            code += this.getAttributeDeclarationLine(attr, i, this.enumElement.Attributes.Count);

                            i++;
                        }

                        code += "\r\n";
                        i = 0;
                        foreach (EA.Attribute attr in this.enumElement.Attributes)
                        {
                            code += this.getAttributeSetGetLine(attr, i, this.enumElement.Attributes.Count);

                            i++;
                        }

                    }
                    code += "}\r\n";
                    Clipboard.SetText(code);

                    Alert.Success("La clase ha sido copiada al porta papeles");
                }
                else
                {
                    if (this.enumElement.Attributes.Count != 0)
                    {
                        bool isEnumClass = this.isEnumClass(this.enumElement);

                        code += "public enum " + this.enumElement.Name + " {\r\n";

                        short i = 0;
                        foreach (EA.Attribute attr in this.enumElement.Attributes)
                        {
                            code += this.getEnumLine(attr, i, this.enumElement.Attributes.Count);

                            i++;
                        }

                        code += "\r\n";

                        // si la enumeracion es short hay que castear el argumento (short)

                        if (isEnumClass)
                        {
                            code += "\tprivate " + this.varType + " value;\r\n";
                            code += "\t" + this.enumElement.Name + "( " + this.varType + " value ){\r\n";
                            code += "\t\tthis.value = value;\r\n";
                            code += "\t}";
                            code += "\r\n";

                            code += "\tpublic " + this.varType + " value() {\r\n";
                            code += "\t\treturn this.value;\r\n";
                            code += "\t}\r\n";
                        }

                        code += "}";

                        Clipboard.SetText(code);

                        Alert.Success("La enumeración ha sido copiada al porta papeles");
                    }
                }
            }
        }

        private bool isEnumClass(Element enumElement)
        {
            bool isClass = false;

            foreach (EA.Attribute attr in this.enumElement.Attributes)
            {
                if( attr.Default.Length != 0)
                {
                    this.varType = attr.Type;
                    isClass = true;
                    break;
                }
            }
            return isClass;
        }

        private string getEnumLine(EA.Attribute attr, short i, short count)
        {
            string attrLine = "\t" + attr.Name;
            if (attr.Default.Length != 0)
            {
                attrLine += "( ";
                attrLine += (this.varType.ToLower() == "string" ? "\"" : this.varType.ToLower() == "char" ? "'" : "");
                if( attr.Type.ToLower() == "short")
                {
                    attrLine += "(short) ";
                }
                attrLine += attr.Default;
                attrLine += (this.varType.ToLower() == "string" ? "\"" : this.varType.ToLower() == "char" ? "'" : "");
                attrLine += " )";
            }
            attrLine += (i < count - 1 ? "," : ";") + "\r\n";

            return attrLine;
        }

        private string getAttributeDeclarationLine(EA.Attribute attr, short i, short count)
        {
            string attrLine = "";
            string jsonAlias;

            if ( (bool)useJsonAlias )
            {
                jsonAlias = this.eaUtils.taggedValuesUtils.getJsonAlias(attr, attr.Name).asString();

                if(jsonAlias != attr.Name)
                {
                    attrLine = "\t@JsonAlias({ \"" + jsonAlias + "\"})\r\n";
                }
            }

            attrLine += "\tprivate "+ (attr.Type == "" ? "String" : attr.Type) + " "+ attr.Name +";\r\n";

            return attrLine;
        }

        private string getAttributeSetGetLine(EA.Attribute attr, short i, short count)
        {
            string attrLine = "\tpublic void set" + StringUtils.toPascal(attr.Name) + "(" + (attr.Type == "" ? "String" : attr.Type) + " " + attr.Name + "){\r\n";

            attrLine += "\t\tthis." + attr.Name + " = " + attr.Name + ";\r\n";
            attrLine += "\t}\r\n";

            attrLine += "\tpublic "+ (attr.Type == "" ? "String" : attr.Type ) + " get"+ StringUtils.toPascal(attr.Name) + "(){\r\n";

            attrLine += "\t\treturn this." + attr.Name +";\r\n";
            attrLine += "\t}\r\n";

            return attrLine;
        }
    }
}
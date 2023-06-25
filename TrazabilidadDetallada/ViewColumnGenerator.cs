using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RestFul
{
    public class ViewColumnGenerator
    {
        private EAUtils.EAUtils eaUtils = new EAUtils.EAUtils();

        Regex fromRegex = new Regex(" from ", RegexOptions.IgnoreCase);
        Regex selectRegex = new Regex(" select ", RegexOptions.IgnoreCase);
        Regex replaceRegex = new Regex("^\\s +|\\s +$");

        public ViewColumnGenerator( EAUtils.EAUtils eaUtils )
        {
            this.eaUtils = eaUtils;
        }

        public void analizar( EA.Element vista)
        {
            var memo = eaUtils.taggedValuesUtils.get(vista, "viewdef", "").asString() ;
            var memoOneLine = "";
            List<string> columns = new List<string>();
            string[] memoSplited;

            if (memo.IndexOf("\r") != 1 || memo.IndexOf("\n") != 1)
            {
                memoSplited = memo.Split('\n');

                for (var i = 0; i < memoSplited.Length; i++)
                {
                    if (memoSplited[i].IndexOf("--") == -1)
                    {
                        memoOneLine += " " + memoSplited[i].Replace("\n", "").Replace("\r", string.Empty); ;
                    }
                }
            }
            // busco el primer SELECT y tomo hasta el primer FROM mientras no se encuentre en una columna

            int fromIndex = getFromIndex(memoOneLine, 0);

            Match selectMatch = selectRegex.Match(memoOneLine);

            var selectLine = memoOneLine.Substring( selectMatch.Index + 6, fromIndex - 1);

            // busco la primer coma mientras no este entre "()"
            while (selectLine.Length > 0)
            {
                //Repository.WriteOutput(outputTab, "selectLine "+ selectLine  ,0);

                int commaPos = lookingForComma(selectLine, 0);
                if (commaPos == -1)
                {
                    columns.Add(makeAttributeDefinition(replaceRegex.Replace(selectLine, "")));
                    break;
                }
                // buscar columna
                columns.Add(makeAttributeDefinition(replaceRegex.Replace(selectLine.Substring(0, commaPos), "")));
        
                selectLine = selectLine.Substring(commaPos + 1);
            }
            sincronizarAttr(vista, columns);
        }

        private int getFromIndex( string line, int startFrom )
        {
            Match match = fromRegex.Match(line, startFrom);

            var fromPos = match.Index;
            var endParethesisPos = line.IndexOf(")", fromPos);

            if (fromPos < endParethesisPos && line.Substring(fromPos, endParethesisPos).LastIndexOf("(") == -1)
            {
                fromPos = getFromIndex(line, endParethesisPos + 1);
            }
            return fromPos;
        }

        private int lookingForComma( string line, int startFrom )
        {
            var commaPos = line.IndexOf(",", startFrom);
            //var initParethesisPos =  line.indexOf("(");
            var endParethesisPos = line.IndexOf(")", commaPos);

            if (commaPos < endParethesisPos && line.Substring(commaPos, endParethesisPos).LastIndexOf("(") == -1)
            {
                commaPos = lookingForComma(line, endParethesisPos + 1);
            }
            return commaPos;
        }
        private string makeAttributeDefinition( string columnString )
        {
            string name = "";

            //Repository.WriteOutput(outputTab, "analizando "+ columnString ,0);
            Regex regex = new Regex( " as ", RegexOptions.IgnoreCase);
            Regex regex2 = new Regex("/^[ ] +|[ ] +$/g", RegexOptions.IgnoreCase);

            Match asMatch = regex.Match(columnString );

            if (asMatch != null && asMatch.Length > 0)
            {
                name = columnString.Substring( asMatch.Index + 4 );
            }
            else
            {
                name = regex2.Replace( columnString.Substring(columnString.LastIndexOf(" ")), string.Empty) ;
            }

            if (name.IndexOf(".") != -1)
            {
                name = name.Substring(name.IndexOf(".") + 1);
            }
            return name;
        }

        private void sincronizarAttr(EA.Element vista, List<string> columns)
        {
            EA.Collection attrs;
            EA.Attribute attr;
            attrs = vista.Attributes;

            var attrsActions = AttrActions(vista, columns);

            for (var i = 0; i < attrsActions.Count; i++)
            {
                if (attrsActions[i]["action"] == "add")
                {
                    attr = (EA.Attribute)attrs.AddNew(attrsActions[i]["name"], "string");
                    attr.Stereotype = "column";
                    attr.Update();
                    attrs.Refresh();
                }
                else
                if (attrsActions[i]["action"] == "delete")
                {
                    for (short a = 0; a < attrs.Count; a++)
                    {
                        attr = (EA.Attribute)attrs.GetAt(a);
                        if (attr.Name == attrsActions[i]["name"])
                        {
                            attrs.DeleteAt(a, false);
                            attrs.Refresh();
                            break;
                        }
                    }
                }
            }
        }

        private List<Dictionary<string, string>> AttrActions(EA.Element vista, List<string> columns)
        {
            EA.Collection attrs;
            EA.Attribute attr;
            attrs = vista.Attributes;

            List<Dictionary<string, string>> attrsActions = new List<Dictionary<string, string>>();

            var action = "";

            for (short i = 0; i < attrs.Count; i++)
            {
                action = "delete";
                Dictionary<string, string> attrAction = new Dictionary<string, string>();

                attr = (EA.Attribute)attrs.GetAt(i);

                for (var c = 0; c < columns.Count; c++)
                {
                    if (attr.Name == columns[c])
                    {
                        action = "nothing";
                        break;
                    }
                }

                attrAction.Add("name", attr.Name);
                attrAction.Add("action", action);

                attrsActions.Add(attrAction);

            }
            // determino si hace falta agregar alguno
            for (var c = 0; c < columns.Count; c++)
            {
                action = "add";
                Dictionary<string, string> attrAction = new Dictionary<string, string>();

                for (var i = 0; i < attrsActions.Count; i++)
                {
                    if (attrsActions[i]["name"] == columns[c])
                    {
                        action = "";
                        break;
                    }
                }

                if (action == "add")
                {
                    attrAction.Add("name", columns[c]);
                    attrAction.Add("action", action);

                    attrsActions.Add(attrAction);
                }
            }
            return attrsActions;
        }
    }
}

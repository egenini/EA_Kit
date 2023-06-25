using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Productividad
{
    public static class JsonTreeViewLoader
    {
        public static void LoadJsonToTreeView(this TreeView treeView, string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return;
            }

            try
            {
                //string prejson = System.Text.RegularExpressions.Regex.Unescape(json.Replace("\r", "").Replace("\n", ""));
                string prejson = json.Replace("\r", "").Replace("\n", "");

                if ( prejson.StartsWith("{") )
                {
                    if (!prejson.EndsWith("}"))
                    {
                        prejson = prejson + "}";
                    }

                    var @object = JObject.Parse(prejson);
                    AddObjectNodes(@object, "JSON", treeView.Nodes);
                }
                else if(prejson.StartsWith("["))
                {
                    if (!prejson.EndsWith("]"))
                    {
                        prejson = prejson + "]";
                    }

                    var @object = JArray.Parse(prejson);
                    AddArrayNodes(@object, "JSON", treeView.Nodes);
                }
            }
            catch( Exception )
            {
                
            }
        }

        public static void AddObjectNodes(JObject @object, string name, TreeNodeCollection parent)
        {
            var node = new TreeNode(name);
            parent.Add(node);

            foreach (var property in @object.Properties())
            {
                AddTokenNodes(property.Value, property.Name, node.Nodes);
            }
        }

        private static void AddArrayNodes(JArray array, string name, TreeNodeCollection parent)
        {
            var node = new TreeNode(name);
            parent.Add(node);

            for (var i = 0; i < array.Count; i++)
            {
                AddTokenNodes(array[i], string.Format("[{0}]", i), node.Nodes);
            }
        }

        private static void AddTokenNodes(JToken token, string name, TreeNodeCollection parent)
        {
            if (token is JValue)
            {
                parent.Add(new TreeNode(string.Format("{0}: {1}", name, ((JValue)token).Value)));
            }
            else if (token is JArray)
            {
                AddArrayNodes((JArray)token, name, parent);
            }
            else if (token is JObject)
            {
                AddObjectNodes((JObject)token, name, parent);
            }
        }
    }
}

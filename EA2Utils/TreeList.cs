using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAUtils
{
    public class TreeList
    {
        const short TREE_NODE__LABEL_POS = 0;
        const short TREE_NODE__CHILDREN_POS = 1;

        List<string> strings = new List<string>();
        char stringSeparator = '.';
        public List<TreeNode> tree = new List<TreeNode>();

        public TreeList( char stringSeparator )
        {
            this.stringSeparator = stringSeparator;
        }

        public void addString( string node )
        {
            this.strings.Add(node);
        }

        public void build(List<string> strings)
        {
            this.strings = strings;
            this.strings.Sort();

            List<string[]> list = new List<string[]>();
            string normalized;
            foreach (string currentString in this.strings)
            {
                if (currentString.IndexOf(stringSeparator) == 0)
                {
                    normalized = currentString.Substring(1);
                }
                else
                {
                    normalized = currentString;
                }

                list.Add(normalized.Split(stringSeparator));
            }

            string[] current;
		
		    for(int i = 0; i< list.Count; i++)
            {
                current = list[i];
                //Session.Output( (i +" *** "+ list[i]) );
                for (int l = 0; l < current.Count(); l++)
                {
                    if (l == 0)
                    {
                        //plainList.push({"id": current[l], "parent" : ""});
                        this.addNode(current[0], 0, null, current[l]);
                    }
                    else
                    {
                        //plainList.push({"id": current[l], "parent" : current[l-1]});
                        this.addNode(current[0], (l - 1), current[l - 1], current[l]);
                    }
                    //Session.Output( (i +" "+ l +" "+ list[i][l]) );
                }
            }

        }

        public void addNode(string root, int parentLevel, string parent, string node )
        {
            // buscar el root
            var rootFound = false;
    		for(int r = 0; r < this.tree.Count; r++)
            {
                //Session.Output("buscando Root "+ root +" =? "+ this.tree[0][r][0])
                if (tree[r].label == root)
                {
                    rootFound = true;
                    if (parent == null)
                    {
                        break;
                    }
                    // Si tiene parent y encontre el root busco hasta encontrar el parent
                    // el parent puede ser el mismo root para eso tengo el level
                    // y agrego el nodo a la children list.
                }
            }
            //Session.Output("root "+ root + (rootFound ? " encontrado ": " creando") );
            if (!rootFound)
            {
                //Session.Output("Creando root "+ root);
                //this.tree.Add([root, []]);
                this.tree.Add( new TreeNode(root));

            }
            else if (parent != null)
            {
                this.lookAt(this.tree, parentLevel, 0, parent, node);
            }
        }

        public bool lookAt(List<TreeNode> currentTree, int levelTarget, int currentLevel, string parentNodeName, string nodeName)
	    {
		    bool ready = false;
		
		    if(levelTarget == currentLevel)
		    {
			    // busco en esta rama el parent
                bool foundNode = false;

			    for( int i = 0; i < currentTree.Count; i++ )
			    {
				    if( currentTree[i].label == parentNodeName )
				    {
					    foundNode       = false;
					
					    for(int n = 0; n < currentTree[i].nodes.Count; n++ )
					    {
						    if( currentTree[i].nodes[n].label == nodeName)
						    {
							    foundNode = true;
							    break;
						    }
					    }
					    if( !foundNode )
					    {
						    currentTree[i].nodes.Add(new TreeNode( nodeName ));
						    break;
						    //ready = true;
					    }
					    else
					    {
						    // se ignora por si se repite
						    break;
					    }
				    }
			    }
		    }
		    else
		    {
			    for(int i = 0; i < currentTree.Count; i++ )
			    {
                    ready = this.lookAt( currentTree[i].nodes, levelTarget, currentLevel + 1, parentNodeName, nodeName );
				    if(ready)
                    {
                        break;
                    }
                }
		    }
		    return ready;
	    }
    }

    public class TreeNode
    {
        public string label;
        public List<TreeNode> nodes;


        public TreeNode(string label)
        {
            this.label = label;
            this.nodes = new List<TreeNode>();
        }

        public TreeNode( string label, List<TreeNode> nodes)
        {
            this.label = label;
            this.nodes = nodes;
        }
    }
}

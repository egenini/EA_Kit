using EA;
using EAUtils;
using EAUtils.model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDGBuilder.mdg
{

    /// <summary>
    /// Si el patrón no tiene una caja de herramientas la agrega
    /// </summary>
    internal class DiagramPatternToolboxHelper
    {
        EAUtils.EAUtils eaUtils;
        public DiagramPatternToolboxHelper(EAUtils.EAUtils eAUtils) 
        { 
            this.eaUtils = eAUtils;
        }

        public void verificar(Package mdgPackage, Package pattern, string genFile) 
        {
            Package toolboxPackage = this.eaUtils.packageUtils.getChildPackageByStereotype(mdgPackage, "toolbox profile");
            bool encontrado = false;
            foreach( Element element in toolboxPackage.Elements)
            {
                if( element.Name.Equals(mdgPackage.Name +"::"+ pattern.Name + "(UMLPatternSilent)") ) 
                { 
                    encontrado= true;                    
                }
            }
            if( ! encontrado )
            {
                Element t = toolboxPackage.Elements.AddNew(mdgPackage.Name + "::" + pattern.Name + "(UMLPatternSilent)", "Class");

                t.Stereotype = "stereotype";
                t.Update();

                string imageFile = genFile.Replace(".xml", ".bmp");

                this.eaUtils.repository.GetProjectInterface().PutDiagramImageToFile(pattern.Diagrams.GetAt(0).DiagramGUID, imageFile, 1);

                Bitmap original = (Bitmap)Image.FromFile(imageFile);
                Bitmap resized = new Bitmap((Bitmap)original, new Size(16, 16));

                using (MemoryStream stream = new MemoryStream())
                {
                    resized.Save(stream, ImageFormat.Bmp);
                    original.Dispose();

                    using (FileStream file = new FileStream(imageFile, FileMode.Open, FileAccess.Write))
                    {
                        stream.WriteTo(file);
                    }
                }

                EA.Attribute attr = t.Attributes.AddNew("Icon", "");

                attr.Default = imageFile;

                attr.Update();

                t.Attributes.Refresh();

                Element toolbox = this.eaUtils.packageUtils.getChildElementByName(toolboxPackage, "ToolboxPage");
                // agregar conector a la metaclass
                this.eaUtils.connectorUtils.addConnector(t, toolbox, "Extension", null, null);

            }
        }

    }
}

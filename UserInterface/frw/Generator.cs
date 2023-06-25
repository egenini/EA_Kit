using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EA;
using EAUtils;
using UIResources;
using EAUtils.entity;
using UserInterface.html;

namespace UserInterface.frw
{
    class Generator
    {
        private EAUtils.EAUtils eaUtils;
        private Element element;
        private Framework frameworkInstance;

        public Generator(Element element, Framework frameworkInstance, EAUtils.EAUtils eaUtils)
        {
            this.element = element;
            this.frameworkInstance = frameworkInstance;
            this.eaUtils = eaUtils;
        }

        public bool generarArtefacto()
        {
            bool canDo = false;

            string infoString = this.frameworkInstance.getFrwJsonFromTaggedValue(this.element);

            if (infoString == "{}" || infoString == "")
            {
                generarJSon();

                infoString = this.frameworkInstance.getFrwJsonFromTaggedValue(this.element);
            }

            if (infoString != "{}" && infoString != "")
            {
                canDo = this.frameworkInstance.generate(infoString, this.element);
            }
            else
            {
                Alert.Error("No se encuentra el JSON");
            }

            return canDo;
        }
        
        public void generarJSon()
        {
            Html html = new Html(element, frameworkInstance, eaUtils);

            html.walk();

            this.frameworkInstance.setFrwJson(this.element, html.stringfity());
        }
    }
}

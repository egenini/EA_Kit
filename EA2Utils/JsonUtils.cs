using EA;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EAUtils
{
    public class JsonUtils
    {
        private EAUtils eaUtils;
        private Element currentElement = null;
        private RealClass currentRealClass = null;

        public JsonUtils(EAUtils eaUtils)
        {
            this.eaUtils = eaUtils;
        }

        /// <summary>
        /// Genera un Object de C# para manipular su contenido.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public Object fromString(string json)
        {
            StringReader sreader = new StringReader(json);
            JsonReader reader = new JsonTextReader(sreader);

            reader.Read();

            return reader.Value;
        }

        /// <summary>
        /// Crea un json del elemento buscando atributos propios, por relaciones y ademas si tiene inner clases las agrega.
        /// </summary>
        /// <param name="rootElement"></param>
        /// <returns></returns>
        public string toString(Element rootElement, short stringCase, bool forceBuild)
        {
            string json = "";
            JsonBuilder builder = new JsonBuilder();

            if ( currentElement == null || forceBuild || ( currentElement != null && currentElement.ElementID != rootElement.ElementID ))
            {
                currentElement = rootElement;

                ModelClass2RealClass model2Real = new ModelClass2RealClass(eaUtils);
                model2Real.go(rootElement);
                this.currentRealClass = model2Real.realClass;
            }

            json = builder.build(currentRealClass, true, stringCase);

            return json;
        }

    }
}

using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using EA;
using Jint;
using Jint.Native;
using Jint.Native.Object;
using Jint.Runtime.Interop;
using Microsoft.ClearScript;
using Microsoft.ClearScript.JavaScript;
using Microsoft.ClearScript.V8;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Proceso4Doc.script
{
    public class Test
    {
        public Repository repository = null;

        public void probar()
        {
            /*            
            var engine = new Engine(cfg => cfg.AllowClr(typeof(EA.Repository).Assembly));

            engine.Execute("Repository.EnsureOutputVisible('Script');");
            engine.Execute("Repository.WriteOutput('Script', 'GG', 0);");
            */

            
            using (var engine = new V8ScriptEngine())
            {
                engine.AddHostObject("Repository", repository);

                var global = engine.Script;

                global.COMObjectInstance = new Func<string, object>((progId) =>
                {
                    object comObject = null;

                    try
                    {
                        Type type = Type.GetTypeFromProgID(progId);
                        comObject = Activator.CreateInstance(type);
                    }
                    catch(Exception) { }

                    return comObject;
                });

                engine.Execute("Repository.EnsureOutputVisible('Script');");
                engine.Execute("var COMObject = function (x) { return COMObjectInstance(x); }");
                engine.Execute("var xmlDom = new COMObject('MSXML2.DOMDocument.6.0')");
                engine.Execute("Repository.WriteOutput('Script', 'is null? '+ (xmlDom == null) , 0)");          
            }
            
        }
    }
}

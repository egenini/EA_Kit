using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAUtils
{
    public class SemanticVersioning
    {
        public const string BUILD_SEPARATOR__MINUS = "-";
        public const string BUILD_SEPARATOR__PLUS = "+";

        public bool usingMinusAsBuildSeparator = true;
        /// <summary>
        /// Parseo algún string de version ?
        /// </summary>
        public bool isInitialized = false;

        public int initialMajor = 0;
        public int initialMinor = 0;
        public int initialPatch = 0;
        private string initialBuild = "";

        public int major = 0;
        public int minor = 0;
        public int patch = 0;
        public string build = "";
        public bool useSemanticVersioning = false;

        public void parseVersion( string currentVersion )
        {
            this.isInitialized = true;

            int index = currentVersion.IndexOf(BUILD_SEPARATOR__MINUS);
            string version = currentVersion;

            if ( index > -1 )
            {
                build = currentVersion.Substring(index + 1); // uso iindex + 1 porque el separador lo tengo aparte.
                version   = currentVersion.Substring(0, index);
                usingMinusAsBuildSeparator = true;
            }
            else if( ( index = currentVersion.IndexOf(BUILD_SEPARATOR__PLUS) ) > -1)
            {
                build = currentVersion.Substring(index + 1); // uso iindex + 1 porque el separador lo tengo aparte.
                version   = currentVersion.Substring(0, index);
                usingMinusAsBuildSeparator = true;
            }

            // ahora debería tener 3 posiciones pero no puedo confiar que esto sea así
            string[] versionSplitted = version.Split('.');

            int numberVersion;
            string currentValue;

            for (int i = 0; i < versionSplitted.Length; i++)
            {
                currentValue = versionSplitted[i];

                bool res = int.TryParse(currentValue, out numberVersion);
                if (i == 0)
                {
                    if (res)
                    {
                        major = numberVersion;
                    }
                    else
                    {
                        major = 1;
                    }
                }
                else if (i == 1)
                {
                    if (res)
                    {
                        minor = numberVersion;
                    }
                    else
                    {
                        major = 0;
                    }
                }
                else if (i == 2)
                {
                    if (res)
                    {
                        patch = numberVersion;
                    }
                    else
                    {
                        patch = 0;
                    }
                }
            }

            initialMajor = major;
            initialMinor = minor;
            initialPatch = patch;
            initialBuild = build;
        }

        public string buildStringWithoutBuildPart()
        {
            return major + "." + minor + "." + patch;
        }

        public string buildString()
        {
            return major + "." + minor + "." + patch + (build != "" ? (usingMinusAsBuildSeparator ? "-"+ build : "+"+ build) : "");
        }

        public void reset()
        {
            major = initialMajor;
            minor = initialMinor;
            patch = initialPatch;
            build = initialBuild;
        }

        public bool incrementMajor()
        {
            if( major == initialMajor )
            {
                major++;
                minor = 0;
                patch = 0;
            }
            return major - initialMajor == 1;
        }

        /// <summary>
        /// Se puede incrementar de 1 a la vez y en tanto y en cuanto no se hubiera incrementado major.
        /// </summary>
        /// <returns></returns>
        public bool incrementMinor()
        {
            if( minor == initialMinor && major == initialMajor)
            {
                minor++;
                patch = 0;
            }
            return minor - initialMinor == 1;
        }

        /// <summary>
        /// Se puede incrementar de 1 en 1 y si ni major ni minor se modificaron.
        /// </summary>
        /// <returns></returns>
        public bool incrementPatch()
        {
            if( patch == initialPatch && major == initialMajor && minor == initialMinor )
            {
                patch++;
            }
            return patch - initialPatch == 1;
        }
    }
}

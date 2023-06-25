using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace EAUtils
{
    public class StringUtils
    {
        public const short CAMEL = 1;
        public const short PASCAL = 2;
        public const short SNAKE = 3;
        public const short DOT = 4;
        public const short POINT = 4;
        public const short NONE = 0; // por si se quiere llamar siempre al método usando alguna suerte de menú.

        public static string to( short caseIndex, string toConvert)
        {
            if (caseIndex == CAMEL)
            {
                toConvert = toCamel(toConvert);
            }
            else if (caseIndex == PASCAL)
            {
                toConvert = toPascal (toConvert);
            }
            else if (caseIndex == SNAKE)
            {
                toConvert = toSnake(toConvert);
            }
            else if (caseIndex == DOT || caseIndex == POINT)
            {
                toConvert = toPoint(toConvert);
            }

            return toConvert;
        }
        /// <summary>
        /// camelCase
        /// </summary>
        /// <param name="toConvert"></param>
        /// <returns></returns>
        public static string toCamel( string toConvert )
        {
            toConvert = toCamel(toConvert, true);
            return toCamel(toConvert, false);
        }

        public static string toCamel(string toConvert, bool toUpper)
        {
            string camel = null;
            if (toConvert.Length != 0)
            {
                camel = makeWord(toConvert, "-", toUpper);
                camel = makeWord(camel, "_", toUpper);
                camel = makeWord(camel, ".", toUpper);
                camel = makeWord(camel, "\r\n", toUpper);
            }
            return camel.Substring(0, 1).ToLower() + camel.Substring(1);
        }

        /// <summary>
        /// PascalCase
        /// </summary>
        /// <param name="toConvert"></param>
        /// <returns></returns>
        public static string toPascal(string toConvert)
        {
            toConvert = toCamel(toConvert, true);
            return toConvert.Substring(0,1).ToUpper() + toConvert.Substring(1);
        }

        public static string toSnake(string toConvert)
        {
            for (int j = toConvert.Length - 1; j > 0; j--)
                if ((j > 0 && char.IsUpper(toConvert[j])) || (j > 0 && char.IsNumber(toConvert[j]) && !char.IsNumber(toConvert[j - 1])))
                    toConvert = toConvert.Insert(j, "_");

            return toConvert.ToLower();
        }
        public static string toPoint(string toConvert)
        {
            for (int j = toConvert.Length - 1; j > 0; j--)
                if ((j > 0 && char.IsUpper(toConvert[j])) || (j > 0 && char.IsNumber(toConvert[j]) && !char.IsNumber(toConvert[j - 1])))
                    toConvert = toConvert.Insert(j, ".");

            return toConvert.ToLower();
        }


        public static string toLabel(string toConvert)
        {
            toConvert = toSnake(toConvert);
            return (toConvert.Substring(0, 1).ToUpper() + toConvert.Substring(1).ToLower()).Replace('_', ' ');
        }

        private static string makeWord( string toConvert, string separator, bool toUpper)
        {
            string toReturn = "";
            string whiteSpaces = "";
            String currentWord;
            int startFrom = 0;

            // @ TODO excluir los comentarios  entre estas marcas /* */

            if (toConvert.Contains(separator))
            {
                string[] spaces = toConvert.Split(separator.ToCharArray());
                foreach (string word in spaces)
                {
                    whiteSpaces = "";
                    if (word.Trim().Length > 0)
                    {
                        for (int i = 0; i < word.Length; i++)
                        {
                            if(char.IsWhiteSpace(word[i]))
                            {
                                whiteSpaces += " ";
                            }
                            else
                            {
                                break;
                            }
                        }
                        if( whiteSpaces. Length > 0)
                        {
                            currentWord = word.Replace(" ", "");
                        }
                        else
                        {
                            currentWord = word;
                        }
                        startFrom = 0;
                        if (currentWord.Length > 1 && currentWord[0] == '"' )
                        {
                            startFrom = 1;
                        }
                        if (toUpper)
                        {
                            toReturn += whiteSpaces + ( startFrom == 1 ? "\"" : "" ) + currentWord.Substring(startFrom, 1).ToUpper() + currentWord.Substring(startFrom + 1);

                        }
                        else
                        {
                            toReturn += whiteSpaces + ( startFrom == 1 ? "\"" : "" ) + currentWord.Substring(startFrom, 1).ToLower() + currentWord.Substring(startFrom + 1);
                        }

                        if ( separator == "\r\n")
                        {
                            toReturn += separator;
                        }
                    }
                }
            }
            else
            {
                toReturn = toConvert;
            }
            return toReturn;
        }

        public static string wordBetween( string word, char first, char second)
        {
            int start = word.IndexOf(first) + 1;
            int end = word.IndexOf(second, start);
            return word.Substring(start, end - start);
        }
    }
}

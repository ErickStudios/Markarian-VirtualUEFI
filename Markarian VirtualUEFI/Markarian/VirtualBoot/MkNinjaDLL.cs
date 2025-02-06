// R LIBBAHEBE BB
// SUMMARY LIBRERIA NinjaDLL para emsamblar programas NinjaASM
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

// ninja dll

//
// RESUMEN:
//      no hay resumen...
//      (como que no :O)
namespace MkNinjanamespace
{
    // UL
    public class MkNinja
    {
        // esto se reescribio en el #### NinjaConsole.xaml.cs por que no funcionana
        // hurmp >:(
        private string MkNinja_ReservedWord_Startvar = "#";
        public Dictionary<string, string> variables = new Dictionary<string, string>();

        public MkNinja()
        {
            // es literalmente un "void"
        }

        public string MKNINJA_UNCONVERFROMOOBJET(string text)
        {
            string result;
            result = text.Replace("◊◊◊♫♪ӒӒӦ", "-");
            result = result.Replace("◊◊◊♫♪ӒҴѴ", "_");
            result = result.Replace("◊◊◊♫♪ӒӒӱ", "\"");
            result = result.Replace("◊◊◊♫♪Ӓẏ", "/");
            result = result.Replace("◊◊◊♫♪ủỨ", "{");
            result = result.Replace("◊◊◊♫♪ủA", "}");
            return result;
        }

        public string MKNINJA_CONVERTOOOBJET(string text)
        {
            string result;
            result = text.Replace("-" ,"◊◊◊♫♪ӒӒӦ");
            result = result.Replace("_" ,"◊◊◊♫♪ӒҴѴ");
            result = result.Replace("\"" , "◊◊◊♫♪ӒӒӱ");
            result = result.Replace("/" , "◊◊◊♫♪Ӓẏ");
            result = result.Replace("{" ,"◊◊◊♫♪ủỨ");
            result = result.Replace("}", "◊◊◊♫♪ủA");
            return result;
        }

        //
        // RESUMEN:
        //      esto para obtener valores dentro de un texto en formato NinjaASMDAT o NinDAT
        public string MkNinja_Dat_GetValue(string ofvar, string intext)
        {
            string[] MkNinjaTemp_Get1 = intext.Split(new string[] { MkNinja_ReservedWord_Startvar + ofvar + "= " }, StringSplitOptions.None);
            if (MkNinjaTemp_Get1.Length < 2) return null;
            return MkNinjaTemp_Get1[1].Split('\n')[0];
        }

        //
        // RESUMEN:
        //      Remplazar los > para quw no hagan inteferendia
        public string NinjaSystemUncript(string text)
        {
            string result = text.Replace(">>", ">");
            result = result.Replace(">>", ">");
            result = result.Replace("%e2", ">");
            result = result.Replace("%e1", "\n");
            return result;
        }

        //
        // RESUMEN:
        //      esto es para modificar un valor NinDAT en un texto NinDAT
        public string MkNinja_Dat_ModificValue(string value, string to, string intext)
        {
            return intext.Replace(MkNinja_ReservedWord_Startvar + value + "= " + MkNinja_Dat_GetValue(value, intext), MkNinja_ReservedWord_Startvar + value + "= " + to);
        }

        //
        // RESUMEN:
        //      esto es para obtener una sintaxis no funciono el diccionario asi que lo tuve que reesvribir en el NinjaConsole.xaml.cs
        public string NinjaSyntax(string text)
        {
            if (text == "Null") return "NULL";
            if (text.StartsWith("%")) {
                text = text.Substring(1, text.Length - 1);
                if (variables.ContainsKey(text))
                {
                    return variables[text];
                }
                else
                {
                    // Manejar el caso donde la clave no existe
                    return "NINJA ASM REFERENCE ERROR;\n - ERROR OUTPUT: NONE ;\n - ERROR DETAILS: system key missing or invalid...\n\nin a nushell: the variable is not founded";
                }
            }
            return text;
        }

        public string EnlazeNinjaObjet(string text)
        {
            string result;
            result = "_DECLARATION -TYPE \"efi program\"\n\\\\ \n// do NOT load never this program on a real uefi,it break it\n_DECLARATION -BITS 64\n// Compiled with MBNINJA\n// you can edit this code without broken the program\n_IMPORT \"NINJA ASM CODE\" {\n" + text + "\n}";
            return result;
        }

        //
        // RESUMEN:
        //      este es el interprete obsoleto el interprete ya esta en NinjaConsole.xaml.cs
        public void NinjaLang(string script) {
            // !- eh . script NinjaAsm parser
            string[] lines = script.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            int lineNumber = 0;

            foreach (string line in lines)
            {
                lineNumber++; // Incrementar el número de línea en cada iteración

                // Aquí puedes agregar lógica para analizar y ejecutar cada línea del script
                // Esto es solo un ejemplo de cómo podrías comenzar
                if (line.StartsWith("// ")) // Ejemplo de comentario o metadato
                {
                    // Ignorar la línea o hacer algo específico con ella
                    continue;
                }
                else if (line.StartsWith(MkNinja_ReservedWord_Startvar))
                {
                    // diccionary vars use
                    if (!line.Contains("= "))
                    {
                        MessageBox.Show("script.line " + lineNumber + "\n" + "error : the variable definition cant found the \"= \" in the line", "NinjaASM : uefi shell");
                        continue;
                    }
                    string lineWithoutPrefix = line.Substring(MkNinja_ReservedWord_Startvar.Length, line.Length - MkNinja_ReservedWord_Startvar.Length);
                    string[] vardat = lineWithoutPrefix.Split(new string[] { "= " }, StringSplitOptions.None);
                    string var_name = vardat[0];
                    string var_value = vardat[1];

                    variables[var_name] = var_value; // Almacenar en el diccionario
                } else if (line.StartsWith("print: "))
                {
                    MessageBox.Show(line.Substring(7, line.Length - 7), "NinjaASM : uefi shell");
                }

                // Lógica para analizar e interpretar las instrucciones del script
                // Aquí puedes agregar más lógica para ejecutar el código basado en `line`
            }
        }
    }
}

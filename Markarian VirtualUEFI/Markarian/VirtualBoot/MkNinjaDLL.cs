using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MkNinjanamespace
{
    public class MkNinja
    {
        private string MkNinja_ReservedWord_Startvar = "#";
        private Dictionary<string, string> variables = new Dictionary<string, string>();

        public MkNinja()
        {
            // Constructor vacío, se podría inicializar algo aquí si es necesario.
        }

        public string MkNinja_Dat_GetValue(string ofvar, string intext)
        {
            string[] MkNinjaTemp_Get1 = intext.Split(new string[] { MkNinja_ReservedWord_Startvar + ofvar + "= " }, StringSplitOptions.None);
            if (MkNinjaTemp_Get1.Length < 2) return null;
            return MkNinjaTemp_Get1[1].Split('\n')[0];
        }

        public string MkNinja_Dat_ModificValue(string value, string to, string intext)
        {
            return intext.Replace(MkNinja_ReservedWord_Startvar + value + "= " + MkNinja_Dat_GetValue(value, intext), MkNinja_ReservedWord_Startvar + value + "= " + to);
        }

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

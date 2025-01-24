using MkNinjanamespace;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.IO;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Linq;
using System.Windows.Interop;

namespace Markarian_VirtualUEFI
{
    public partial class NinjaConsole : Window
    {
        private string exePath;
        private string ProgramFolder;
        private string FolderPath;
        MkNinja ninjadll = new MkNinja();

        string shellpathcurrent;

        public void CDfolder(string add)
        {
            string exePath = AppDomain.CurrentDomain.BaseDirectory;
            string ProgramFolder = Path.Combine(exePath, "Markarian");
            string FolderPath = Path.Combine(ProgramFolder);

            string newdir = shellpathcurrent + add + "\n";
            string path = newdir.Replace("\n", "\\");

            string[] pathcomplete = shellpathcurrent.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            if (add == "..") {
                // Move up one directory level
                int lastIndex = pathcomplete.Length;
                if (lastIndex > 0)
                {
                    string pathbeta = pathcomplete[lastIndex - 1];
                    pathbeta = pathbeta.Replace("\\","\n");
                    shellpathcurrent = shellpathcurrent.Substring(0, shellpathcurrent.Length - pathbeta.Length - 1);
                }
            } else {
                if (Directory.Exists(FolderPath + "\\" + path))
                {
                    shellpathcurrent = newdir;
                } else
                {
                    ConsoleOutput.AppendText("not founded dir" + "\n");
                }
            }
        }

        public void makedir(string name)
        {
            string exePath = AppDomain.CurrentDomain.BaseDirectory;
            string ProgramFolder = Path.Combine(exePath, "Markarian");
            string FolderPath = Path.Combine(ProgramFolder);

            string newdir = shellpathcurrent + name + "\n";
            string path = newdir.Replace("\n", "\\");

            Directory.CreateDirectory(FolderPath + "\\" + path);
        }


        public NinjaConsole()
        {
            shellpathcurrent = "Memory\n";
            InitializeComponent();
            string exePath = AppDomain.CurrentDomain.BaseDirectory;
            string ProgramFolder = Path.Combine(exePath, "Markarian");
            string FolderPath = Path.Combine(ProgramFolder, "UEFI", "Config.BIN");
            ConsoleOutput.AppendText("EFI shell" + "\n");
            ConsoleOutput.AppendText("Current Running mode 0.5.0" + "\n");
            ConsoleOutput.AppendText("" + "\n");
            MappingTable();
            ConsoleOutput.AppendText("type \"ver\" to view the version" + "\n");
            ConsoleOutput.AppendText("" + "\n");
            console_update();
        }

        private void MappingTable()
        {
            ConsoleOutput.AppendText("Device mapping table" + "\n");
            ConsoleOutput.AppendText("fs1:\tHardDisk" + "\n");
            ConsoleOutput.AppendText("PCefi/00x01/ : or \"VirtualUEFI HardDisk\"" + "\n");
            ConsoleOutput.AppendText("in: .../Markarian/Memory *" + "\n");
        }

        private async void console_update()
        {
            string dir;
            while (true)
            {
                dir = "MARKARIAN>" + shellpathcurrent.Replace("\n", ">").ToUpper() + " " + ConsoleInput.Text;
                await Task.Delay(50); // Esperar 100 ms antes de verificar de nuevo
                ConsoleOutput.Undo();
                ConsoleOutput.AppendText(dir.Replace("MARKARIAN>MEMORY>", "fs1>") + "\n");
                ConsoleOutput.ScrollToEnd();
            }
        }

        private async void ConsoleInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string input = ConsoleInput.Text;
                ConsoleInput.Clear();

                // Procesar el comando de entrada aquí
                ExecuteNinjaScript(input.Replace("%e1", "\n"));
                if (input == "")
                {
                    ConsoleOutput.AppendText("" + "\n");
                }
            }
        }

        private void ExecuteNinjaScript(string script)
        {
            string[] lines = script.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            int lineNumber = 0;

            foreach (string line in lines)
            {
                lineNumber++;
                ProcessCommand(line);
                ConsoleOutput.AppendText("" + "\n");
            }
        }

        private void ProcessCommand(string line)
        {
            string exePath = AppDomain.CurrentDomain.BaseDirectory;
            string ProgramFolder = Path.Combine(exePath, "Markarian");
            string FolderPath = Path.Combine(ProgramFolder, "UEFI", "Config.BIN");
            string ConfigFileUEFIvirtual = File.ReadAllText(FolderPath);

            // procesing commands
            if (line.StartsWith("restart"))
            {
                // Abre la ventana de carga nuevamente
                startup startup = new startup();
                startup.Show();
                this.Close(); // Cierra la ventana actual
            }
            else if (line.StartsWith("map"))
            {
                MappingTable();
            }
            else if (line.StartsWith("cls"))
            {
                ConsoleOutput.Clear();
                ConsoleOutput.AppendText("" + "\n");
            }
            else if (line.StartsWith("print: "))
            {
                ConsoleOutput.AppendText(line.Substring(7, line.Length - 7) + "\n");
            }
            else if (line.StartsWith("cd: "))
            {
                CDfolder(line.Substring(4, line.Length - 4));
            }
            else if (line.StartsWith("md: "))
            {
                makedir(line.Substring(4, line.Length - 4));
            }
            else if (line.StartsWith("fs1: "))
            {
                shellpathcurrent = "Memory\n";
                CDfolder(line.Substring(5, line.Length - 5));
            }
            else if (line.StartsWith("color: "))
            {
                switch (line.Substring(7, line.Length - 7))
                {
                    case "lime":
                        ConsoleOutput.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD3FF8A"));
                        break;
                    case "green":
                        ConsoleOutput.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1FFF00"));
                        break;
                    case "yellow":
                        ConsoleOutput.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFF500"));
                        break;
                    case "red":
                        ConsoleOutput.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF0000"));
                        break;
                    case "blue":
                        ConsoleOutput.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3695FF"));
                        break;
                    default:
                        ConsoleOutput.AppendText("color:" + "\n");
                        ConsoleOutput.AppendText("\tred" + "\n");
                        ConsoleOutput.AppendText("\tyellow" + "\n");
                        ConsoleOutput.AppendText("\tgreen" + "\n");
                        ConsoleOutput.AppendText("\tlime" + "\n");
                        ConsoleOutput.AppendText("\tblue" + "\n");
                        ConsoleOutput.AppendText("lime is the default color set in the shell" + "\n");
                        ConsoleOutput.AppendText("this only changes the text color" + "\n");
                        break;
                }
            }

            else if (line.StartsWith("ver"))
            {
                ConsoleOutput.AppendText("Markarian UEFI" + "\n");
                ConsoleOutput.AppendText("EFI shell version: 0.3" + "\n");
                ConsoleOutput.AppendText("Markarian version: 0.5" + "\n");
            }
            else if (line.StartsWith("config.BIN: "))
            {
                switch (line.Substring(12, line.Length - 12))
                {
                    case "Safeboot_Y":
                        File.WriteAllText(FolderPath, ninjadll.MkNinja_Dat_ModificValue("SafeBoot", "YES", ConfigFileUEFIvirtual));
                        break;
                    case "Safeboot_N":
                        File.WriteAllText(FolderPath, ninjadll.MkNinja_Dat_ModificValue("SafeBoot", "NO", ConfigFileUEFIvirtual));
                        break;
                    case "Ninja_Y":
                        File.WriteAllText(FolderPath, ninjadll.MkNinja_Dat_ModificValue("NinjaTechnology", "YES", ConfigFileUEFIvirtual));
                        break;
                    case "Ninja_N":
                        File.WriteAllText(FolderPath, ninjadll.MkNinja_Dat_ModificValue("NinjaTechnology", "NO", ConfigFileUEFIvirtual));
                        break;
                    default:
                        ConsoleOutput.AppendText("config.BIN:" + "\n");
                        ConsoleOutput.AppendText("\tSafeboot_Y" + "\n");
                        ConsoleOutput.AppendText("\tSafeboot_N" + "\n");
                        ConsoleOutput.AppendText("\tNinja_Y" + "\n");
                        ConsoleOutput.AppendText("\tNinja_N" + "\n");
                        break;
                }
            }
            else if (line.StartsWith("qeasteregg /negro: true"))
            {
                ConsoleOutput.AppendText("modo negro activado" + "\n");
            }
            else if (line.StartsWith("qeasteregg /negro: true"))
            {
                ConsoleOutput.AppendText("modo negro desactivado" + "\n");
            }
            else if (line.StartsWith("negro"))
            {
                ConsoleOutput.AppendText("?negro, que es negro" + "\n");
            }
            else if (line.Contains("%e1"))
            {
                ninjadll.NinjaLang(line.Replace("%e1", "\n"));
            }
            
            // Desplazar automáticamente el contenido hacia abajo
            ConsoleOutput.ScrollToEnd();
        }

        private void ConsoleOutput_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
    }
}

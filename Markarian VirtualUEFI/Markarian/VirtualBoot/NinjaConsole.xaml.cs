using MkNinjanamespace;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Reflection;
using System.IO;
using System.Windows.Media;

namespace Markarian_VirtualUEFI
{
    public partial class NinjaConsole : Window
    {
        private string exePath;
        private string ProgramFolder;
        private string FolderPath;
        MkNinja ninjadll = new MkNinja();

        public NinjaConsole()
        {
            InitializeComponent();
            string exePath = AppDomain.CurrentDomain.BaseDirectory;
            string ProgramFolder = Path.Combine(exePath, "Markarian");
            string FolderPath = Path.Combine(ProgramFolder, "UEFI", "Config.BIN");
            ConsoleOutput.AppendText("EFI shell" + "\n");
            ConsoleOutput.AppendText("Current Running mode 0.4.0" + "\n");
            ConsoleOutput.AppendText("" + "\n");
            ConsoleOutput.AppendText("type \"ver\" to view the version" + "\n");
            ConsoleOutput.AppendText("C:" + "\n");
            ConsoleOutput.AppendText("PCefi/00x01/ : or \"VirtualUEFI HardDisk\"" + "\n");
            ConsoleOutput.AppendText("in: .../Markarian/Memory *" + "\n");
            ConsoleOutput.AppendText("D:" + "\n");
            ConsoleOutput.AppendText("PCefi/00x02/ : or \"VirtualUEFI Space\"" + "\n");
            ConsoleOutput.AppendText("Unassigned Disk *" + "\n");
            ConsoleOutput.AppendText("E:" + "\n");
            ConsoleOutput.AppendText("PCefi/00x03/ : or \"VirtualUEFI Space\"" + "\n");
            ConsoleOutput.AppendText("Unassigned Disk *" + "\n");
        }

        private void ConsoleInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string input = ConsoleInput.Text;
                ConsoleInput.Clear();

                // Procesar el comando de entrada aquí
                ProcessCommand(input);
            }
        }

        private void ProcessCommand(string line)
        {
            string exePath = AppDomain.CurrentDomain.BaseDirectory;
            string ProgramFolder = Path.Combine(exePath, "Markarian");
            string FolderPath = Path.Combine(ProgramFolder, "UEFI", "Config.BIN");
            ConsoleOutput.AppendText("Shell> " + line.Replace("%e1", "\n") + "\n");
            string ConfigFileUEFIvirtual = File.ReadAllText(FolderPath);

            // procesing commands
            if (line.StartsWith("restart"))
            {
                // Abre la ventana de carga nuevamente
                startup startup = new startup();
                startup.Show();
                this.Close(); // Cierra la ventana actual
            }
            else if (line.StartsWith("print: "))
            {
                ConsoleOutput.AppendText(line.Substring(7, line.Length - 7) + "\n");
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
                ConsoleOutput.AppendText("EFI shell version: 0.2" + "\n");
                ConsoleOutput.AppendText("Markarian version: 0.4" + "\n");
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

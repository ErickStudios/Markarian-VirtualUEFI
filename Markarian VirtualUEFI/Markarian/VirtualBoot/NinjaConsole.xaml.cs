using MkNinjanamespace;
using System;
using MKEFI;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.IO;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Linq;
using System.Windows.Interop;
using System.Collections.Generic;

namespace Markarian_VirtualUEFI
{
    public partial class NinjaConsole : Window
    {
        private string exePath;
        private string ProgramFolder;
        private string FolderPath;
        MkNinja ninjadll = new MkNinja();
        EFI_SERVICES efi_services = new EFI_SERVICES();
        public Dictionary<string, string> variables = new Dictionary<string, string>();
        private string MkNinja_ReservedWord_Startvar = "#";
        public string bootoptions;
        public bool inbootmenu;
        int optionsel;
        int optionsmax;

        public string NinjaSyntax(string text)
        {
            if (text == "Null") return "NULL";
            if (text.StartsWith("%"))
            {
                text = text.Substring(1, text.Length - 1);
                if (variables.ContainsKey(text))
                {
                    return ninjadll.NinjaSystemUncript(variables[text]);
                }
                else
                {
                    // Manejar el caso donde la clave no existe
                    return "NINJA ASM REFERENCE ERROR;\n - ERROR OUTPUT: NONE ;\n - ERROR DETAILS: system key missing or invalid...\n\nin a nushell: the variable is not founded";
                }
            }
            return ninjadll.NinjaSystemUncript(text);
        }

        public void NinjaLang(string script)
        {
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
                }
                else if (line.StartsWith("print: "))
                {
                    MessageBox.Show(line.Substring(7, line.Length - 7), "NinjaASM : uefi shell");
                }

                // Lógica para analizar e interpretar las instrucciones del script
                // Aquí puedes agregar más lógica para ejecutar el código basado en `line`
            }
        }

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
                    pathbeta = pathbeta.Replace("\\", "\n");
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


        public async void addline(string name, string line)
        {
            string exePath = AppDomain.CurrentDomain.BaseDirectory;
            string ProgramFolder = Path.Combine(exePath, "Markarian");
            string FolderPath = Path.Combine(ProgramFolder);

            string newdir = shellpathcurrent + name;
            string path = newdir.Replace("\n", "\\");
            if (!File.Exists(FolderPath + "\\" + path))
            {
                File.WriteAllText(FolderPath + "\\" + path, line);
            }
            else
            {
                File.WriteAllText(FolderPath + "\\" + path, File.ReadAllText(FolderPath + "\\" + path) + "\n" + line);
            }

        }

        public string open_file(string name)
        {
            string exePath = AppDomain.CurrentDomain.BaseDirectory;
            string ProgramFolder = Path.Combine(exePath, "Markarian");
            string FolderPath = Path.Combine(ProgramFolder);

            string newdir = shellpathcurrent + name;
            string path = newdir.Replace("\n", "\\");

            return File.ReadAllText(FolderPath + "\\" + path);
        }

        public void Bootmenuaddoption(string name)
        {
            bootoptions = bootoptions + name + "\n";
            optionsmax = optionsmax + 1;
        }

        public NinjaConsole()
        {
            shellpathcurrent = "";
            InitializeComponent();
            string exePath = AppDomain.CurrentDomain.BaseDirectory;
            string ProgramFolder = Path.Combine(exePath, "Markarian");
            string FolderPath = Path.Combine(ProgramFolder, "UEFI", "Config.BIN");
            string WORCKFILE = Path.Combine(ProgramFolder, "Memory", "main.NinjaASM");
            inbootmenu = false;

            optionsel = 1;
            BootMenuUpdate();
            bootmenufunction();
        }

        public void EFISHELL()
        {
            string exePath = AppDomain.CurrentDomain.BaseDirectory;
            string ProgramFolder = Path.Combine(exePath, "Markarian");
            string FolderPath = Path.Combine(ProgramFolder, "UEFI", "Config.BIN");
            string WORCKFILE = Path.Combine(ProgramFolder, "Memory", "main.NinjaASM");
            ExecuteNinjaScript(File.ReadAllText(WORCKFILE));
            console_update();
        }

        public async void bootmenufunction()
        {
            inbootmenu = true;
            ConsoleOutput.Clear();
            displaybootmenu(optionsel);
        }

        private void displaybootmenu(int optioned)
        {
            ProcessCommand("bgcol Teal");
            ProcessCommand("color white");
            string bootoptionsedit = bootoptions + "\nEFI Internal Shell (Unnsupported Option)";
            string[] options = bootoptionsedit.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            int optiontodisp;
            optiontodisp = 1;

            ConsoleOutput.AppendText("Markarian BootMenu\n\n");
            ConsoleOutput.AppendText("");

            foreach (string option in options)
            {
                ConsoleOutput.AppendText("  ");
                if (optioned == optiontodisp)
                {
                    ConsoleOutput.AppendText("<");
                }
                if (File.Exists(option))
                {
                    ConsoleOutput.AppendText(Path.GetFileName(option));
                }
                else
                {
                    ConsoleOutput.AppendText(option);
                }
                if (optioned == optiontodisp)
                {
                    ConsoleOutput.AppendText(">");
                }
                ConsoleOutput.AppendText("\n");
                optiontodisp += 1;
            }

            ConsoleOutput.AppendText("\nArrow keys not supported use W/S keys");
        }

        private void MappingTable()
        {
            ConsoleOutput.AppendText("Device mapping table" + "\n");
            ConsoleOutput.AppendText("fs1:\tHardDisk" + "\n");
            ConsoleOutput.AppendText("PCefi/00x01/ : or \"VirtualUEFI HardDisk\"" + "\n");
            ConsoleOutput.AppendText("in: .../Markarian/Memory *" + "\n");
        }

        private void executeefiprogram(string efiprogram)
        {
            inbootmenu = false;
            ProcessCommand("color white");
            ProcessCommand("bgcol black");
            ExecuteNinjaScript(efiprogram);
        }

        private void BootMenuUpdate()
        {
            optionsmax = 0;
            bootoptions = "";
            string exePath = AppDomain.CurrentDomain.BaseDirectory;
            string ProgramFolder = Path.Combine(exePath, "Markarian");
            string FolderPath = Path.Combine(ProgramFolder, "UEFI", "Config.BIN");
            string WORCKFILE = Path.Combine(ProgramFolder, "Memory", "main.NinjaASM");
            foreach (string filen in Directory.EnumerateFiles(Path.Combine(ProgramFolder, "Memory"), "*.*", SearchOption.AllDirectories))
            {
                if (filen.EndsWith(".efi") || filen.EndsWith(".efish"))
                {
                    Bootmenuaddoption(filen);
                }
            }
        }

        private async void console_update()
        {
            string dir;
            while (!inbootmenu)
            {
                dir = "Shell>" + shellpathcurrent.Replace("\n", ">").ToUpper() + " " + ConsoleInput.Text;
                await Task.Delay(50); // Esperar 100 ms antes de verificar de nuevo
                ConsoleOutput.Undo();
                ConsoleOutput.AppendText(dir.Replace("Shell>MEMORY>", "fs1>") + "\n");
                ConsoleOutput.ScrollToEnd();
                ConsoleInput.Foreground = ConsoleOutput.Foreground;
            }
        }

        private void ConsoleOutpud_KeyDown(object sender, KeyEventArgs e)
        {
            if (inbootmenu)
            {
                string bootoptionsedit = bootoptions + "\nEFI Internal Shell (Unnsupported Option)";
                string[] options = bootoptionsedit.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                ConsoleOutput.Clear();
                if (e.Key == Key.S)
                {
                    if (optionsel + 1 < optionsmax +2)
                    {
                        optionsel = optionsel + 1;
                    }
                    BootMenuUpdate();
                    displaybootmenu(optionsel);
                }
                else if (e.Key == Key.W)
                {
                    if (optionsel - 1 > 0)
                    {
                        optionsel = optionsel - 1;
                    }
                    BootMenuUpdate();
                    displaybootmenu(optionsel);
                }
                else if (e.Key == Key.Enter)
                {
                    if (options[optionsel - 1] == "EFI Internal Shell (Unnsupported Option)") {
                        inbootmenu = false;
                        EFISHELL();
                    } else
                    {
                        int outpudcode = efi_services.EFI_PRERUNFILE(options[optionsel - 1]);
                        if (outpudcode == 0)
                        {
                            BootMenuUpdate();
                            executeefiprogram(efi_services.EFI_RUNCODE());
                        }
                        else 
                        {
                            ProcessCommand("color aquamarine");
                            ConsoleOutput.AppendText("EFI FILE CANT BE EXECUTED" + "\n\n");
                            ConsoleOutput.AppendText("PLEASE CHECK THE CODE BELOW THIS MESSAGE" + "\n");
                            ConsoleOutput.AppendText(efi_services.EFI_E_CODE_NAME(outpudcode));
                            ConsoleOutput.AppendText("\nPress any key to back to boot menu..." + "\n");
                        }
                    }
                } else
                {
                    displaybootmenu(optionsel);
                }
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
            }

            if (lines.Length > 0)
            {
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
            else if (line.StartsWith("echo "))
            {
                ConsoleOutput.AppendText(NinjaSyntax(line.Substring(5, line.Length - 5)) + "\n");
            }
            else if (line.StartsWith("writel "))
            {
                ConsoleOutput.AppendText(NinjaSyntax(line.Substring(7, line.Length - 7)));
            }
            else if (line.StartsWith("read "))
            {
                ConsoleOutput.AppendText(open_file(NinjaSyntax(line.Substring(5, line.Length - 5))) + "\n");
            }
            else if (line.StartsWith("./"))
            {
                ExecuteNinjaScript(open_file(NinjaSyntax(line.Substring(2, line.Length - 2))) + "\n");
            }
            else if (line.StartsWith("+/"))
            {
                string commandtext = line.Substring(2, line.Length - 2);
                if (line.Contains(" > "))
                {
                    string lineWithoutPrefix = line.Substring(MkNinja_ReservedWord_Startvar.Length, line.Length - MkNinja_ReservedWord_Startvar.Length);
                    string[] vardat = lineWithoutPrefix.Split(new string[] { " > " }, StringSplitOptions.None);
                    string file_name = vardat[0];
                    string text_to_mgr = vardat[1];

                    addline(NinjaSyntax(file_name), NinjaSyntax(text_to_mgr));
                }
            }
            else if (line.StartsWith("MBNINJA -c "))
            {
                string commandtext = line.Substring(11, line.Length - 11);
                addline(NinjaSyntax(commandtext + ".efi"), ninjadll.MKNINJA_CONVERTOOOBJET(ninjadll.EnlazeNinjaObjet(open_file(commandtext))));
            }
            else if (line.StartsWith("cd "))
            {
                CDfolder(NinjaSyntax(line.Substring(3, line.Length - 3)));
            }
            else if (line.StartsWith("md "))
            {
                makedir(NinjaSyntax(line.Substring(3, line.Length - 3)));
            }
            else if (line.StartsWith("fs1:"))
            {
                shellpathcurrent = "Memory\n";
                string Commandtext1 = ninjadll.NinjaSyntax(line.Substring(4, line.Length - 4));
                bool commandtext1empyte = Commandtext1 == "";
                if (!commandtext1empyte)
                {
                    string folder = line.Substring(5, line.Length - 5);
                    CDfolder(NinjaSyntax(folder.Replace("\\", "\n")));
                }
            }
            else if (line.StartsWith("color "))
            {
                switch (ninjadll.NinjaSyntax(line.Substring(6, line.Length - 6)))
                {
                    case "black":
                        ConsoleOutput.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF101010"));
                        break;
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
                    case "aquamarine":
                        ConsoleOutput.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7FFFD4"));
                        break;
                    case "blue":
                        ConsoleOutput.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3695FF"));
                        break;
                    case "Teal":
                        ConsoleOutput.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#008080"));
                        break;
                    case "YellowGreen":
                        ConsoleOutput.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9ACD32"));
                        break;
                    case "white":
                        ConsoleOutput.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD6D6D6"));
                        break;
                    default:
                        ConsoleOutput.AppendText("color" + "\n");
                        ConsoleOutput.AppendText("\tred" + "\n");
                        ConsoleOutput.AppendText("\tyellow" + "\n");
                        ConsoleOutput.AppendText("\tYellowGreen" + "\n");
                        ConsoleOutput.AppendText("\tgreen" + "\n");
                        ConsoleOutput.AppendText("\taquamarine" + "\n");
                        ConsoleOutput.AppendText("\tlime" + "\n");
                        ConsoleOutput.AppendText("\tblue" + "\n");
                        ConsoleOutput.AppendText("\tTeal" + "\n");
                        ConsoleOutput.AppendText("\twhite" + "\n");
                        ConsoleOutput.AppendText("\tblack" + "\n");
                        ConsoleOutput.AppendText("this only changes the text color" + "\n");
                        break;
                }
            }
            else if (line.StartsWith("bgcol "))
            {
                switch (ninjadll.NinjaSyntax(line.Substring(6, line.Length - 6)))
                {
                    case "black":
                        Main.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF101010"));
                        break;
                    case "lime":
                        Main.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD3FF8A"));
                        break;
                    case "green":
                        Main.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1FFF00"));
                        break;
                    case "yellow":
                        Main.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFF500"));
                        break;
                    case "red":
                        Main.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF0000"));
                        break;
                    case "aquamarine":
                        Main.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7FFFD4"));
                        break;
                    case "blue":
                        Main.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3695FF"));
                        break;
                    case "Teal":
                        Main.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#008080"));
                        break;
                    case "YellowGreen":
                        Main.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9ACD32"));
                        break;
                    case "white":
                        Main.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD6D6D6"));
                        break;
                    default:
                        ConsoleOutput.AppendText("bgco" + "\n");
                        ConsoleOutput.AppendText("\tred" + "\n");
                        ConsoleOutput.AppendText("\tyellow" + "\n");
                        ConsoleOutput.AppendText("\tYellowGreen" + "\n");
                        ConsoleOutput.AppendText("\tgreen" + "\n");
                        ConsoleOutput.AppendText("\taquamarine" + "\n");
                        ConsoleOutput.AppendText("\tlime" + "\n");
                        ConsoleOutput.AppendText("\tblue" + "\n");
                        ConsoleOutput.AppendText("\tTeal" + "\n");
                        ConsoleOutput.AppendText("\twhite" + "\n");
                        ConsoleOutput.AppendText("\tblack" + "\n");
                        ConsoleOutput.AppendText("this only changes the bg color" + "\n");
                        break;
                }
            }

            else if (line.StartsWith("ver"))
            {
                ConsoleOutput.AppendText("Markarian UEFI" + "\n");
                ConsoleOutput.AppendText("EFI shell version: 0.4" + "\n");
                ConsoleOutput.AppendText("Markarian version: 0.6" + "\n");
            }
            else if (line.StartsWith(".internal "))
            {
                switch (line.Substring(10, 4))
                {
                    case "-exc":
                        switch (line.Substring(15, line.Length - 15))
                        {
                            case "pefishell":
                                ProcessCommand("color white");
                                ProcessCommand("bgcol black");
                                ConsoleOutput.AppendText("Markarian EFI shell" + "\n");
                                ConsoleOutput.AppendText("Current Running mode 0.6.0" + "\n");
                                ConsoleOutput.AppendText("" + "\n");
                                MappingTable();
                                ConsoleOutput.AppendText("type \"ver\" to view the version" + "\n");
                                ConsoleOutput.AppendText("" + "\n");
                                break;
                        }
                        break;
                    default:
                        ConsoleOutput.AppendText("invalid internal key" + "\n");
                        break;
                }
            }
            else if (line.StartsWith("config.BIN: "))
            {
                switch (NinjaSyntax(line.Substring(12, line.Length - 12)))
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
            else if (line.StartsWith("exit"))
            {
                inbootmenu = true;
            }
            else if (line.StartsWith("negro"))
            {
                ConsoleOutput.AppendText("?negro, que es negro" + "\n");
            }
            else if (line.StartsWith("help"))
            {
                ConsoleOutput.AppendText("cls - clear screen" + "\n");
                ConsoleOutput.AppendText("help - show this" + "\n");
                ConsoleOutput.AppendText("ver - show the current version" + "\n");
                ConsoleOutput.AppendText("color - change the text color" + "\n");
                ConsoleOutput.AppendText("echo - show a text in screen" + "\n");
                ConsoleOutput.AppendText("map - show the mapping devices table" + "\n");
                ConsoleOutput.AppendText("config.BIN: - set a spefic setting" + "\n");
                ConsoleOutput.AppendText("md - create a directory" + "\n");
                ConsoleOutput.AppendText("cd - enter in path , \"..\" to back" + "\n");
                ConsoleOutput.AppendText("restart - restats the computer (Not the real , the VirtualUEFI)" + "\n");
                ProcessCommand("pause");
            }
            else
            {
                NinjaLang(line.Replace("%e1", "\n"));
            }
            
            // Desplazar automáticamente el contenido hacia abajo
            ConsoleOutput.ScrollToEnd();
        }

        private void ConsoleOutput_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
        }
    }
}

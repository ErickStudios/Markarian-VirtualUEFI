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
using System.Windows.Controls;
using System.Windows.Documents;
using System.Xml.Linq;

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
        public bool InIF;
        public string ExecutingBlocks;
        public int BlockNum;
        string COMINPUT;
        Key COMCURKEY;

        public string NinjaSyntax(string text)
        {
            if (text == "Null") return "NULL";
            if (text == "&dir") return shellpathcurrent;
            if (text == "&KeyDown") {
                if (COMCURKEY == Key.None)
                {
                    return "false";
                }
                else
                {
                    return "true";
                }
            }
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
            if (text.StartsWith("KeyPressed: "))
            {
                string keytoread = text.Substring(12, text.Length - 12);
                bool keyiskeytoasing;
                bool isreallypressingkey;
                if (EFI_COLECTIONS.EFI_GET_KEY_FROM_NUM(keytoread) == COMCURKEY)
                {
                    keyiskeytoasing = true;
                } else
                {
                    keyiskeytoasing= false;
                }

                if (COMCURKEY == Key.None)
                {
                    isreallypressingkey = false;
                } else
                {
                    isreallypressingkey= true;
                }

                if (keyiskeytoasing && isreallypressingkey)
                {
                    return "true";
                } else
                {
                    return "false";
                }
            }
            if (text == "&CurrentKey")
            {
                return EFI_COLECTIONS.EFI_GET_KEY_CODE(COMCURKEY);
            }
            if (text == "&CurrentKeyCon")
            {
                return EFI_COLECTIONS.EFI_KEY_TO_CHAR(EFI_COLECTIONS.EFI_GET_KEY_CODE(COMCURKEY));
            }
            if (text.StartsWith("?defined: "))
            {
                string vartoview = text.Substring(10,text.Length - 10);
                if (variables.ContainsKey(vartoview))
                {
                    return "true";
                } else { 
                    return "false"; 
                }
            }
            if (text.StartsWith("<"))
            {
                text = text.Substring(1, text.Length - 2);
                string[] comparate = text.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                if (comparate[1] == " isequal ")
                {
                    if (NinjaSyntax(comparate[0]) == NinjaSyntax(comparate[2]))
                    {
                        return "true";
                    } else
                    {
                        return "false";
                    }
                }
                else if (comparate[1] == " isnequal ")
                {
                    if (NinjaSyntax(comparate[0]) == NinjaSyntax(comparate[2]))
                    {
                        return "false";
                    }
                    else
                    {
                        return "true";
                    }
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

                    variables[var_name] = NinjaSyntax(var_value); // Almacenar en el diccionario
                }
                else if (line.StartsWith("print: "))
                {
                    MessageBox.Show(line.Substring(7, line.Length - 7), "NinjaASM : uefi shell");
                } else
                {
                    echo(line + ":\nis not a internal or of environment command\nI dont blame you , this efi shell is under construcction\n");
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
                    echo("not founded dir " + newdir + "\n");
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

        public async void writetext(string name, string line)
        {
            try
            {
                string exePath = AppDomain.CurrentDomain.BaseDirectory;
                string ProgramFolder = Path.Combine(exePath, "Markarian");
                string FolderPath = Path.Combine(ProgramFolder);

                string newdir = shellpathcurrent + name;
                string path = newdir.Replace("\n", "\\");
                File.WriteAllText(FolderPath + "\\" + path, line);
            } catch {
                echo("ERROR: the file dont cant write");
            }
        }

        public string ls()
        {
            string exePath = AppDomain.CurrentDomain.BaseDirectory;
            string ProgramFolder = Path.Combine(exePath, "Markarian");
            string FolderPath = Path.Combine(ProgramFolder);

            string newdir = shellpathcurrent;
            string path = newdir.Replace("\n", "\\");
            string dirs = "Directory of: " + path + "\n\n";

            foreach (string filen in Directory.EnumerateFileSystemEntries(ProgramFolder + "\\" + path, "*.*", SearchOption.TopDirectoryOnly))
            {
                    dirs += "  " + Path.GetFileName(filen) + "\n";
            }

            return dirs;
        }


        public string open_file(string name)
        {
            string exePath = AppDomain.CurrentDomain.BaseDirectory;
            string ProgramFolder = Path.Combine(exePath, "Markarian");
            string FolderPath = Path.Combine(ProgramFolder);

            string newdir = shellpathcurrent + name;
            string path = newdir.Replace("\n", "\\");

            return File.ReadAllText(FolderPath + "\\" + path.Replace("/","\\"));
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

            BlockNum = 0;
            optionsel = 1;
            BootMenuUpdate();
            bootmenufunction();
        }

        public void debug1()
        {
            ProcessCommand("color white");
            ProcessCommand("bgcol Teal");
            AddRun("a\n", "white", "black");
            AddRun("b", "white", "Teal");
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
            ConsoleOutput.Document.Blocks.Clear();
            displaybootmenu(optionsel);
        }

        public void displaybootmenu(int optioned)
        {
            ProcessCommand("bgcol Teal");
            ProcessCommand("color white");
            ProcessCommand("cls");
            string bootoptionsedit = bootoptions + "\nEFI Internal Shell (Unsupported Option)";
            string[] options = bootoptionsedit.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            int optiontodisp;
            optiontodisp = 1;
            string exePath = AppDomain.CurrentDomain.BaseDirectory;
            string ProgramFolder = Path.Combine(exePath, "Markarian");
            string FolderPath = Path.Combine(ProgramFolder);
            bool isselected;
            string colorbg;

            AddRun("Markarian BootMenu\n\n","white","Teal");
            AddRun("", "white", "Teal");

            foreach (string option in options)
            {
                isselected = false;
                AddRun("   ", "white", "Teal");
                colorbg = "Teal";
                if (optioned == optiontodisp)
                {
                    isselected = true;
                    AddRun("<", "white", "Teal");
                }
                if (isselected)
                {
                    colorbg = "blue";
                }
                if (File.Exists(option))
                {
                    AddRun(Path.GetFileName(option), "white", colorbg);
                }
                else
                {
                    AddRun(option, "white", colorbg);
                }
                if (optioned == optiontodisp)
                {
                    AddRun(">", "white", "Teal");
                }
                AddRun("\n", "white", "Teal");
                optiontodisp += 1;
            }
            AddRun("\nArrow keys not supported use W/S keys", "white", "Teal");
            AddRun("\n\n Path:\n", "white", "Teal");
            if (File.Exists(options[optioned - 1]))
            {
                AddRun(options[optioned - 1].Replace(ProgramFolder + "\\Memory", "PCefi/00x01/::"), "white", "Teal");
            }
            else
            {
                AddRun("CODE:NinjaConsole::public void EFISHELL() {...}", "white", "Teal");
            }
        }

        private void MappingTable()
        {
            echo("Device mapping table" + "\n");
            echo("fs1:\tHardDisk" + "\n");
            echo("PCefi/00x01/ : or \"VirtualUEFI HardDisk\"" + "\n");
            echo("in: .../Markarian/Memory *" + "\n");
            echo("blk0:\tEFI PARTITION" + "\n");
            echo("PCefi/00x02/ : or \"VirtualUEFI EFI PARTITION\"" + "\n");
            echo("in: .../Markarian/UEFI *" + "\n");
        }

        public void executeefiprogram(string efiprogram)
        {
            inbootmenu = false;
            ConsoleOutpudparg.Inlines.Clear();
            ProcessCommand("color white");
            ProcessCommand("bgcol black");
            ExecuteNinjaScript(efiprogram);
        }

        public void BootMenuUpdate()
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

        public async void console_update()
        {

        }

        private async void ConsoleOutpud_KeyDown(object sender, KeyEventArgs e)
        {
            COMCURKEY = e.Key;
            if (inbootmenu)
            {
                string bootoptionsedit = bootoptions + "\nEFI Internal Shell (Unnsupported Option)";
                string[] options = bootoptionsedit.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                ConsoleOutput.Document.Blocks.Clear();
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
                            variables.Clear();
                            executeefiprogram(efi_services.EFI_RUNCODE());
                        }
                        else 
                        {
                            ProcessCommand("color aquamarine");
                            echo("EFI FILE CANT BE EXECUTED" + "\n\n");
                            echo("PLEASE CHECK THE CODE BELOW THIS MESSAGE" + "\n");
                            echo(efi_services.EFI_E_CODE_NAME(outpudcode));
                            echo("\nPress any key to back to boot menu..." + "\n");
                        }
                    }
                } else
                {
                    displaybootmenu(optionsel);
                }
            }
            await Task.Delay(50);
            COMCURKEY = Key.None;
        }

        private async void ConsoleInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            { 
                string dir;
                dir = "Shell>" + shellpathcurrent.Replace("\n", ">").ToUpper() + " " + ConsoleInput.Text;
                await Task.Delay(50); // Esperar 50 ms antes de verificar de nuevo

                echo(dir.Replace("Shell>MEMORY>", "fs1>") + "\n");
                ConsoleOutput.ScrollToEnd();
                ConsoleInput.Foreground = ConsoleOutput.Foreground;

                string input = ConsoleInput.Text;
                ConsoleInput.Clear();


                // Procesar el comando de entrada aquí
                ExecuteNinjaScript(input.Replace("%e1", "\n"));
                if (input == "")
                {
                    echo("" + "\n");
                }
            }
        }

        private async void ExecuteNinjaScript(string script)
        {
            string[] lines = script.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            int lineNumber = 0;
            BlockNum = 0;

            foreach (string line in lines)
            {
                lineNumber++;
                if (line.StartsWith("wait "))
                {
                    if (BlockNum == 0)
                    {
                        string commandtext = line.Substring(5,line.Length - 5);
                        commandtext = NinjaSyntax(commandtext);
                        await Task.Delay(int.Parse(commandtext));
                    } else
                    {
                        ProcessCommand(line);
                    }
                }
                else
                {
                    ProcessCommand(line);
                }
            }
        }

        private string getcolorcodeof(string colorcode)
        {
            switch (colorcode) {
                case "black":
                    return "#FF101010";
                case "lime":
                    return "#FFD3FF8A";
                case "green":
                    return "#FF1FFF00";
                case "yellow":
                    return "#FFFFF500";
                case "red":
                    return "#FFFF0000";
                case "aquamarine":
                    return "#7FFFD4";
                case "blue":
                    return "#FF3695FF";
                case "Teal":
                    return "#008080";
                case "YellowGreen":
                    return "#9ACD32";
                case "white":
                    return "#FFD6D6D6";
                default:
                    return "NoColorCodeDetected";
            }
            return colorcode;
        }

        private void ProcessCommand(string line)
        {
            string exePath = AppDomain.CurrentDomain.BaseDirectory;
            string ProgramFolder = Path.Combine(exePath, "Markarian");
            string FolderPath = Path.Combine(ProgramFolder, "UEFI", "Config.BIN");
            string ConfigFileUEFIvirtual = File.ReadAllText(FolderPath);

            if (BlockNum < 1)
            {
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
                    ConsoleOutpudparg.Inlines.Clear();
                    echo("" + "\n");
                }
                else if (line.StartsWith("echo "))
                {
                    echo(NinjaSyntax(line.Substring(5, line.Length - 5)) + "\n");
                }
                else if (line.StartsWith("writel "))
                {
                    echo(NinjaSyntax(line.Substring(7, line.Length - 7)));
                }
                else if (line.StartsWith("read "))
                {
                    echo(open_file(NinjaSyntax(line.Substring(5, line.Length - 5))) + "\n");
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
                    writetext(NinjaSyntax(commandtext + ".efi"), ninjadll.MKNINJA_CONVERTOOOBJET(ninjadll.EnlazeNinjaObjet(open_file(commandtext))));
                }
                else if (line.StartsWith("cd "))
                {
                    CDfolder(NinjaSyntax(line.Substring(3, line.Length - 3)));
                }
                else if (line.StartsWith("md "))
                {
                    makedir(NinjaSyntax(line.Substring(3, line.Length - 3)));
                }
                else if (line.StartsWith("setabsdir: "))
                {
                    shellpathcurrent = "";
                    string Commandtext1 = ninjadll.NinjaSyntax(line.Substring(11, line.Length - 11));
                    bool commandtext1empyte = Commandtext1 == "";
                    if (!commandtext1empyte)
                    {
                        CDfolder(NinjaSyntax(Commandtext1.Replace("\\", "\n")));
                    }
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
                            echo("color" + "\n");
                            echo("\tred" + "\n");
                            echo("\tyellow" + "\n");
                            echo("\tYellowGreen" + "\n");
                            echo("\tgreen" + "\n");
                            echo("\taquamarine" + "\n");
                            echo("\tlime" + "\n");
                            echo("\tblue" + "\n");
                            echo("\tTeal" + "\n");
                            echo("\twhite" + "\n");
                            echo("\tblack" + "\n");
                            echo("this only changes the text color" + "\n");
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
                            echo("bgcol" + "\n");
                            echo("\tred" + "\n");
                            echo("\tyellow" + "\n");
                            echo("\tYellowGreen" + "\n");
                            echo("\tgreen" + "\n");
                            echo("\taquamarine" + "\n");
                            echo("\tlime" + "\n");
                            echo("\tblue" + "\n");
                            echo("\tTeal" + "\n");
                            echo("\twhite" + "\n");
                            echo("\tblack" + "\n");
                            echo("this only changes the bg color" + "\n");
                            break;
                    }
                }
                else if (line.StartsWith("ver"))
                {
                    echo("Markarian UEFI" + "\n");
                    echo("Markarian version: 0.8" + "\n");
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
                                    echo("Markarian EFI shell" + "\n");
                                    echo("Current Running mode 0.8.0" + "\n");
                                    echo("" + "\n");
                                    MappingTable();
                                    echo("type \"ver\" to view the version" + "\n");
                                    echo("" + "\n");
                                    break;
                            }
                            break;
                        default:
                            echo("invalid internal key" + "\n");
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
                            echo("config.BIN:" + "\n");
                            echo("\tSafeboot_Y" + "\n");
                            echo("\tSafeboot_N" + "\n");
                            echo("\tNinja_Y" + "\n");
                            echo("\tNinja_N" + "\n");
                            break;
                    }
                }
                else if (line.StartsWith("qeasteregg /negro: true"))
                {
                    echo("modo negro activado" + "\n");
                }
                else if (line.StartsWith("qeasteregg /negro: true"))
                {
                    echo("modo negro desactivado" + "\n");
                }
                else if (line.StartsWith("exit"))
                {
                    inbootmenu = true;
                }
                else if (line.StartsWith("ls"))
                {
                    echo(ls() + "\n");
                } 
                else if (line.StartsWith("_DECLARATION"))
                {
                }
                else if (line.StartsWith("\\\\"))
                {
                }
                else if (line.StartsWith("}"))
                {
                }
                else if (line.StartsWith("_IMPORT"))
                {
                }
                else if (line.StartsWith("negro"))
                {
                    echo("?negro, que es negro" + "\n");
                }
                else if (line.StartsWith("help"))
                {
                    echo("cls - clear screen" + "\n");
                    echo("help - show this" + "\n");
                    echo("ver - show the current version" + "\n");
                    echo("color - change the text color" + "\n");
                    echo("echo - show a text in screen" + "\n");
                    echo("map - show the mapping devices table" + "\n");
                    echo("config.BIN: - set a spefic setting" + "\n");
                    echo("md - create a directory" + "\n");
                    echo("cd - enter in path , \"..\" to back" + "\n");
                    echo("restart - restats the computer (Not the real , the VirtualUEFI)" + "\n");
                }
                else if (line.StartsWith("if"))
                {
                    if (line.Contains(" : "))
                    {
                        if (NinjaSyntax("%ifcondition") == "true")
                        {
                            ProcessCommand(line.Substring(5, line.Length - 5));
                        }
                    }
                    else
                    {
                        BlockNum = BlockNum + 1;
                        ExecutingBlocks = "";
                    }
                }
                else if (line.StartsWith("func"))
                {
                    BlockNum = BlockNum + 1;
                    ExecutingBlocks = "";
                }
                else
                {
                    NinjaLang(line.Replace("%e1", "\n"));
                }
            }
            else
            {
                if (line.StartsWith("endif"))
                {
                    if (BlockNum - 1 > 0)
                    {
                        BlockNum = BlockNum - 1;
                        ExecutingBlocks = ExecutingBlocks + line + "\n";
                    }
                    else
                    {
                        if (NinjaSyntax("%ifcondition") == "true")
                        {
                            ExecuteNinjaScript(ExecutingBlocks);
                            BlockNum = 0;
                            ExecutingBlocks = "";
                        }
                    }
                }
                if (line.StartsWith("endfunc"))
                {
                    if (BlockNum - 1 > 0)
                    {
                        BlockNum = BlockNum - 1;
                        ExecutingBlocks = ExecutingBlocks + line + "\n";
                    }
                    else
                    {
                        variables[NinjaSyntax("%functionname")] = ExecutingBlocks;
                        echo(variables[NinjaSyntax("%functionname")] + "\n");
                    }
                }

                else if (line.StartsWith("if"))
                {
                    BlockNum = BlockNum + 1;
                    ExecutingBlocks = ExecutingBlocks + line + "\n";
                }
                else if (line.StartsWith("func"))
                {
                    BlockNum = BlockNum + 1;
                    ExecutingBlocks = ExecutingBlocks + line + "\n";
                }
                else
                {
                    ExecutingBlocks = ExecutingBlocks + line + "\n";
                }

            }
            // Desplazar automáticamente el contenido hacia abajo
            ConsoleOutput.ScrollToEnd();
        }

        private void AddRun(string text, string Colorerb, string colorbg)
        {
            // Crear un nuevo Run con el texto deseado
            Run newRun = new Run(text)
            {
                Foreground = getcolorcodeof(Colorerb) != "NoColorCodeDetected" ?
                    new SolidColorBrush((Color)ColorConverter.ConvertFromString(getcolorcodeof(Colorerb))) : ConsoleOutput.Foreground,
                Background = getcolorcodeof(colorbg) != "NoColorCodeDetected" ?
                    new SolidColorBrush((Color)ColorConverter.ConvertFromString(getcolorcodeof(colorbg))) : ConsoleOutput.Background
            };

            /*
            Paragraph paragraph = new Paragraph();
            paragraph.Inlines.Add(newRun);
            paragraph.Margin = new Thickness(0, 0, 0, 0); // Sin margen
            paragraph.Padding = new Thickness(0); // Ajusta el padding a cero
            */

            // Limpiar el párrafo antes de añadir el nuevo Run (esto es solo una prueba)

            // Añadir el Run al Paragraph existente
            ConsoleOutpudparg.Inlines.Add(newRun);

            ConsoleOutput.Document.Blocks.Remove(ConsoleOutpudparg);
            ConsoleOutput.Document.Blocks.Add(ConsoleOutpudparg);
        }
        private void echo(string text)
        {
            AddRun(text, "no color", "no color");
        }

        private void ConsoleOutput_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
        }
    }
}

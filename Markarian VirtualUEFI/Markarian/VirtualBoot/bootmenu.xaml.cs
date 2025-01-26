using MkNinjanamespace;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Markarian_VirtualUEFI
{
    public partial class bootmenu : Window
    {
        private string exePath;
        private string ProgramFolder;
        private string FolderPath;
        MkNinja ninjadll = new MkNinja();

        public bootmenu()
        {
            InitializeComponent();
            string exePath = AppDomain.CurrentDomain.BaseDirectory;
            string ProgramFolder = Path.Combine(exePath, "Markarian");
            string FolderPath = Path.Combine(ProgramFolder, "UEFI", "Config.BIN");
            string ConfigFile = File.ReadAllText(FolderPath);
            Safebootalternate.Content = ninjadll.MkNinja_Dat_GetValue("SafeBoot", File.ReadAllText(FolderPath));
            NinjaTCN_altern.Content = ninjadll.MkNinja_Dat_GetValue("NinjaTechnology", File.ReadAllText(FolderPath));
            ninjadll.NinjaLang(ConfigFile);
        }
        

        private void Button1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("aaa");
        }

        private async void Safebootalternact(object sender, RoutedEventArgs e)
        {
            string exePath = AppDomain.CurrentDomain.BaseDirectory;
            string ProgramFolder = Path.Combine(exePath, "Markarian");
            string FolderPath = Path.Combine(ProgramFolder, "UEFI", "Config.BIN");
            string ConfigFileUEFIvirtual = File.ReadAllText(FolderPath);

            if (ninjadll.MkNinja_Dat_GetValue("SafeBoot", ConfigFileUEFIvirtual) == "YES")
            {
                File.WriteAllText(FolderPath, ninjadll.MkNinja_Dat_ModificValue("SafeBoot", "NO" , ConfigFileUEFIvirtual));
            }
            else
            {
                File.WriteAllText(FolderPath, ninjadll.MkNinja_Dat_ModificValue("SafeBoot", "YES", ConfigFileUEFIvirtual));
            }

            while (File.ReadAllText(FolderPath) == ConfigFileUEFIvirtual)
            {
                await Task.Delay(100); // Esperar 100 ms antes de verificar de nuevo
            }

            // Actualizar el contenido del botón una vez que el archivo haya cambiado
            Safebootalternate.Content = ninjadll.MkNinja_Dat_GetValue("SafeBoot", File.ReadAllText(FolderPath));
        }

        private async void NinjaTCN_alternate(object sender, RoutedEventArgs e)
        {
            string exePath = AppDomain.CurrentDomain.BaseDirectory;
            string ProgramFolder = Path.Combine(exePath, "Markarian");
            string FolderPath = Path.Combine(ProgramFolder, "UEFI", "Config.BIN");
            string ConfigFileUEFIvirtual = File.ReadAllText(FolderPath);

            if (ninjadll.MkNinja_Dat_GetValue("NinjaTechnology", ConfigFileUEFIvirtual) == "YES")
            {
                File.WriteAllText(FolderPath, ninjadll.MkNinja_Dat_ModificValue("NinjaTechnology", "NO", ConfigFileUEFIvirtual));
            }
            else
            {
                File.WriteAllText(FolderPath, ninjadll.MkNinja_Dat_ModificValue("NinjaTechnology", "YES", ConfigFileUEFIvirtual));
            }

            while (File.ReadAllText(FolderPath) == ConfigFileUEFIvirtual)
            {
                await Task.Delay(100); // Esperar 100 ms antes de verificar de nuevo
            }

            // Actualizar el contenido del botón una vez que el archivo haya cambiado
            NinjaTCN_altern.Content = ninjadll.MkNinja_Dat_GetValue("NinjaTechnology", File.ReadAllText(FolderPath));
        }


        private void BackButton(object sender, RoutedEventArgs e)
        {
            // Abre la ventana de carga nuevamente
            startup startup = new startup();
            startup.Show();
            this.Close(); // Cierra la ventana actual
        }
    }
}

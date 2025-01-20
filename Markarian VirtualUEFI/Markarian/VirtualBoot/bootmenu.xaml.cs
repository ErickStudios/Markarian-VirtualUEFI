using System.Windows;

namespace Markarian_VirtualUEFI
{
    public partial class bootmenu : Window
    {
        public bootmenu()
        {
            InitializeComponent();
        }

        private void Button1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("aaa");
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

using System.Windows;

namespace Markarian_VirtualUEFI
{
    public partial class AnotherWindow : Window
    {
        public AnotherWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Abre la ventana de carga nuevamente
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close(); // Cierra la ventana actual
        }
    }
}

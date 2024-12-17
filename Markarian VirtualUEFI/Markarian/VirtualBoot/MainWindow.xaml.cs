using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Markarian_VirtualUEFI
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;
        private int progressValue;

        public MainWindow()
        {
            InitializeComponent();
            InitializeProgressBar();
        }

        private void InitializeProgressBar()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(10); // Intervalo de 10 ms
            timer.Tick += Timer_Tick;
            progressValue = 0;
            progressBar.Value = progressValue; // Inicializa el valor de la barra de progreso
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (progressValue < 100)
            {
                progressValue++;
                progressBar.Value = progressValue; // Incrementa el valor de la barra de progreso
            }
            else
            {
                timer.Stop(); // Detiene el temporizador cuando llega al 100%
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                // Detiene el temporizador para detener el proceso de carga
                timer.Stop();

                // Redirige a otra página o ventana
                AnotherWindow anotherWindow = new AnotherWindow();
                anotherWindow.Show();
                this.Close(); // Cierra la ventana actual si es necesario
            }
        }
    }
}

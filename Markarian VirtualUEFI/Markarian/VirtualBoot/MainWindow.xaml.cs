using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
            CreateChildWindow("A");
        }

        private void CreateChildWindow(string title)
        {
            // Crear la ventana base
            var window = new Grid
            {
                Name = title,
                Width = 300,
                Height = 200,
                Background = new SolidColorBrush(Color.FromRgb(255, 255, 255))
            };

            // Crear la barra de título de la ventana
            var windowTitleBar = new Grid
            {
                Name = "TitleBar",
                Height = 30,
                Background = new SolidColorBrush(Color.FromRgb(75, 225, 255)),
                VerticalAlignment = VerticalAlignment.Top
            };

            // Crear el texto de la barra de título
            var windowTitleBarText = new TextBlock
            {
                Name = "Title",
                Text = title,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 0, 0, 0)
            };

            // Crear el botón de cerrar
            var windowCloseButton = new Button
            {
                Name = "close",
                Content = "x",
                Width = 30,
                Height = 30,
                Background = new SolidColorBrush(Color.FromRgb(246, 211, 217)),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center
            };

            windowCloseButton.Click += (s, e) => CloseChildWindow(window);

            // Añadir los elementos a la barra de título
            windowTitleBar.Children.Add(windowTitleBarText);
            windowTitleBar.Children.Add(windowCloseButton);

            // Añadir la barra de título a la ventana
            window.Children.Add(windowTitleBar);

            // Añadir la ventana al contenedor principal
            MainGrid.Children.Add(window);
        }

        private void CloseChildWindow(Grid window)
        {
            MainGrid.Children.Remove(window);
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

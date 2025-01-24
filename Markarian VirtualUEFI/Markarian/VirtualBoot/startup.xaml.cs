using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.IO;
using MkNinjanamespace;

// por si las dudas no soy ingles solo me gusta OOOJKKK?

namespace Markarian_VirtualUEFI
{
    public partial class startup : Window
    {
        string exePath;
        string MainFolder;

        private DispatcherTimer timer;
        private int progressValue;

        private bool isDragging = false;
        private Point startPoint;
        MkNinja ninjadll = new MkNinja();

        public void CreateChildWindow(string title, int width, int height)
        {
            // Crear la ventana base
            var window = new Grid
            {
                Name = title.Replace(" ", "_"),
                Width = width,
                Height = height,
                Background = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };

            // Crear la barra de título de la ventana
            var windowTitleBar = new Grid
            {
                Name = title.Replace(" ", "_") + "TitleBar",
                Height = 30,
                Background = new SolidColorBrush(Color.FromRgb(233, 225, 187)),
                VerticalAlignment = VerticalAlignment.Top
            };

            windowTitleBar.MouseLeftButtonDown += (s, e) =>
            {
                isDragging = true;
                startPoint = e.GetPosition(window);
                windowTitleBar.CaptureMouse();
            };

            windowTitleBar.MouseMove += (s, e) =>
            {
                if (isDragging)
                {
                    var currentPosition = e.GetPosition(MainGrid);
                    var offsetX = currentPosition.X - startPoint.X;
                    var offsetY = currentPosition.Y - startPoint.Y;
                    window.Margin = new Thickness(offsetX, offsetY, 0, 0);
                }
            };

            windowTitleBar.MouseLeftButtonUp += (s, e) =>
            {
                isDragging = false;
                windowTitleBar.ReleaseMouseCapture();
            };

            // Crear el texto de la barra de título
            var windowTitleBarText = new TextBlock
            {
                Name = title.Replace(" ", "_") + "Title",
                Text = title,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 0, 0, 0)
            };

            // Crear el botón de cerrar
            var windowCloseButton = new Button
            {
                Name = title.Replace(" ", "_") + "close",
                Content = "x",
                Width = 30,
                Height = 20,
                Background = new SolidColorBrush(Color.FromRgb(246, 211, 217)),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top
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

        public void CloseChildWindow(Grid window)
        {
            MainGrid.Children.Remove(window);
        }

        // esto reserva un
        public void CreatingReservedSpace()
        {
            // variable local que se reutilizara muchas veces
            string FolderPath;

            FolderPath = MainFolder;

            if (!Directory.Exists(FolderPath))
            {
                // crear carpeta principal
                Directory.CreateDirectory(FolderPath);
            }

            FolderPath = Path.Combine(MainFolder, "Memory"); // voy a intentar logica
            if (!Directory.Exists(FolderPath))
            {
                // crear carpeta de discos donde puedes meter tus sistemas
                Directory.CreateDirectory(FolderPath); // ah si, se me olvido
            }

            FolderPath = Path.Combine(MainFolder, "Memory", "BootMgr.BIN");
            if (!File.Exists(FolderPath))
            {
                // crear archivo del terminal
                File.WriteAllText(FolderPath, "#Terminal = terminal.mk");
            }

            FolderPath = Path.Combine(MainFolder, "Memory", "terminal.mk");
            if (!File.Exists(FolderPath))
            {
                // crear carpeta de discos donde puedes meter tus sistemas
                File.WriteAllText(FolderPath, "DECLARATION: bathautoexecute terminal"); // ah si, se me olvido
            }

            FolderPath = Path.Combine(MainFolder, "Memory", "User1");
            if (!Directory.Exists(FolderPath))
            {
                // crear carpeta de una tecnología
                Directory.CreateDirectory(FolderPath);
            }

            FolderPath = Path.Combine(MainFolder, "Memory", "User1", "scripts");
            if (!Directory.Exists(FolderPath))
            {
                // crear carpeta de una tecnología
                Directory.CreateDirectory(FolderPath);
            }

            FolderPath = Path.Combine(MainFolder, "Memory", "User1", "texts");
            if (!Directory.Exists(FolderPath))
            {
                // crear carpeta de una tecnología
                Directory.CreateDirectory(FolderPath);
            }

            FolderPath = Path.Combine(MainFolder, "Ninja");
            if (!Directory.Exists(FolderPath))
            {
                // crear carpeta de una tecnología
                Directory.CreateDirectory(FolderPath);
            }

            // carpeta reservada del UEFI virtual
            FolderPath = Path.Combine(MainFolder, "UEFI");
            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }

            // archivo de un bin no se para qué
            // es lógico que el archivo debe estar dentro de ./UEFI
            FolderPath = Path.Combine(MainFolder, "UEFI", "Config.BIN");
            if (!File.Exists(FolderPath))
            {
                File.WriteAllText(FolderPath, "// el archivo del bin no lo modifiques porfa :(\n\n#SafeBoot= YES\n#NinjaTechnology= YES");
            }
        }

        public startup()
        {
            InitializeComponent();
            exePath = AppDomain.CurrentDomain.BaseDirectory;
            MainFolder = Path.Combine(exePath, "Markarian");
            InitializeProgressBar();
            CreatingReservedSpace();
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
                if (ninjadll.MkNinja_Dat_GetValue("NinjaTechnology", File.ReadAllText(Path.Combine(MainFolder, "UEFI", "Config.BIN"))) == "YES")
                {
                    NinjaConsole ninjaConsole = new NinjaConsole();
                    ninjaConsole.Show();
                    this.Close();
                }
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                // Detiene el temporizador para detener el proceso de carga
                timer.Stop();

                // Redirige a otra página o ventana
                bootmenu bootmenu = new bootmenu();
                bootmenu.Show();
                this.Close(); // Cierra la ventana actual si es necesario
            }
        }
    }
}

using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Compilador_AAA.Views;

namespace Compilador_AAA
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            TranslatorView translatorView = new TranslatorView();
            ContentPanel.Children.Add(translatorView);
            


        }

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hwn, int wMsg, int wParam, int lParam);
        private void WindowBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowInteropHelper helper = new WindowInteropHelper(this);
            SendMessage(helper.Handle, 161, 2, 0);
        }
        private void WindowBar_MouseEnter(object sender, MouseEventArgs e)
        {
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
                this.WindowState = WindowState.Maximized;
            else
                this.WindowState = WindowState.Normal;
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }



        private void Palabras_Reservadas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PalabrasReservadasView palabrasReservadasView = new PalabrasReservadasView();
            ContentPanel.Children.Clear();
            ContentPanel.Children.Add(palabrasReservadasView);

        }

        private void TraductorMenu_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TranslatorView translatorView = new TranslatorView();
            ContentPanel.Children.Clear();
            ContentPanel.Children.Add(translatorView);
        }


        private void Window_Activated(object sender, EventArgs e)
        {
            BrushConverter bc = new BrushConverter();
            MainBorder.BorderBrush = (Brush)bc.ConvertFrom("#878b4f");
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            BrushConverter bc = new BrushConverter();
            MainBorder.BorderBrush = (Brush)bc.ConvertFrom("#434343");
        }
    }
}
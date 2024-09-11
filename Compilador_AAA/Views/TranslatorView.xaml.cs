using Compilador_AAA.Traductor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Compilador_AAA.Views
{
    /// <summary>
    /// Lógica de interacción para TranslatorView.xaml
    /// </summary>
    public partial class TranslatorView : UserControl
    {
        public TranslatorView()
        {
            InitializeComponent();
        }

        

        private void btnTraducir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string code = "int x = 10; if (x - 5) return";
                Lexer lexer = new Lexer(code);
                List<Token> tokens = lexer.Tokenize();

                foreach (Token token in tokens)
                {
                    CodeEditor.Text += string.Format($"{token}\n");
                }
            }
            catch (Exception ex)
            {
                // Mostrar el error en caso de que algo salga mal
                MessageBox.Show($"Error: {ex.Message}");
            }
            
        }
    }
}

using Compilador_AAA.Traductor;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit;
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
using System.Diagnostics.Metrics;
using System.Collections.ObjectModel;
using Compilador_AAA.Models;
using ICSharpCode.AvalonEdit.Highlighting;
using System.Text.RegularExpressions;

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

            lvErrores.ItemsSource = ErrorList;
        }
        public static ObservableCollection<ErrorRow> ErrorList { get; set; } = new ObservableCollection<ErrorRow>();

        // Método para manejar errores
        public static void HandleError(string errorMessage, int position)
        {
            var errorRow = new ErrorRow
            {
                emp1 = "",
                Code = "A001", // Código de error
                Description = errorMessage,
                Line = position.ToString(),
                Image = "../Resources/SVG/cross.png", // Ruta de la imagen, si es necesario
                emp2 = "",
            };

            ErrorList.Add(errorRow);
        }

        private bool _isDragging = false;
        private Point _startPoint;

        private void btnTraducir_Click(object sender, RoutedEventArgs e)
        {
            ErrorList.Clear();
            TranslatedEditor.Text = string.Empty;
            TextDocument document = OriginalEditor.Document;

            // Recorrer todas las líneas del documento
            for (int lineNumber = 1; lineNumber <= document.LineCount; lineNumber++)
            {
                // Obtener la línea específica por su número
                DocumentLine line = document.GetLineByNumber(lineNumber);

                // Obtener el texto de la línea
                string lineText = document.GetText(line);

                try
                {
                    Lexer lexer = new Lexer(lineText);
                    List<Token> tokens = lexer.Tokenize();

                    for (int i = 0; i < tokens.Count; i++)
                    {
                        string temp = tokens[i].ToString();
                        if (i != tokens.Count - 1)
                            TranslatedEditor.Text += "'" + temp + "'" + ", ";
                        else
                            TranslatedEditor.Text += "'" + temp + "'\n";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("error");
                }
            }

            BrushConverter bc = new BrushConverter();
            translatedheadercolor.Background = (Brush)bc.ConvertFrom("#878b4f");

        }

        private void lvErrores_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var gridView = lvErrores.View as GridView;

            if (gridView != null)
            {
                // Ancho total disponible
                double totalWidth = lvErrores.ActualWidth - SystemParameters.VerticalScrollBarWidth;

                // Proporciones de las columnas
                gridView.Columns[0].Width = 25;
                gridView.Columns[1].Width = 25;
                gridView.Columns[2].Width = 55;
                if (totalWidth - 185 > 80)
                    gridView.Columns[3].Width = totalWidth - 185;
                gridView.Columns[4].Width = 45;
                gridView.Columns[5].Width = 25;
            }
        }

        private void OriginalEditor_GotFocus(object sender, RoutedEventArgs e)
        {
            BrushConverter bc = new BrushConverter();
            originalheadercolor.Background = (Brush)bc.ConvertFrom("#878b4f");
            translatedheadercolor.Background = (Brush)bc.ConvertFrom("#313131");
            errorlistheadercolor.Background = (Brush)bc.ConvertFrom("#313131");
        }

        private void OriginalEditor_LostFocus(object sender, RoutedEventArgs e)
        {
            BrushConverter bc = new BrushConverter();
            originalheadercolor.Background = (Brush)bc.ConvertFrom("#313131");
        }

        private void TranslatedEditor_GotFocus(object sender, RoutedEventArgs e)
        {
            BrushConverter bc = new BrushConverter();
            translatedheadercolor.Background = (Brush)bc.ConvertFrom("#878b4f");
            originalheadercolor.Background = (Brush)bc.ConvertFrom("#313131");
            errorlistheadercolor.Background = (Brush)bc.ConvertFrom("#313131");
        }

        private void TranslatedEditor_LostFocus(object sender, RoutedEventArgs e)
        {
            BrushConverter bc = new BrushConverter();
            translatedheadercolor.Background = (Brush)bc.ConvertFrom("#313131");
        }

        private void ErrorsWindow_GotFocus(object sender, RoutedEventArgs e)
        {
            BrushConverter bc = new BrushConverter();
            errorlistheadercolor.Background = (Brush)bc.ConvertFrom("#878b4f");
            originalheadercolor.Background = (Brush)bc.ConvertFrom("#313131");
            translatedheadercolor.Background = (Brush)bc.ConvertFrom("#313131");
        }

        private void ErrorsWindow_LostFocus(object sender, RoutedEventArgs e)
        {
            errorlistheadercolor.Background = new SolidColorBrush(Colors.Transparent);
        }

        private void OriginalEditor_TextChanged(object sender, EventArgs e)
        {
            if (OriginalEditor.Text!=string.Empty)
            {
                ErrorList.Clear();
                TranslatedEditor.Text = string.Empty;
                TextDocument document = OriginalEditor.Document;

                // Recorrer todas las líneas del documento
                for (int lineNumber = 1; lineNumber <= document.LineCount; lineNumber++)
                {
                    // Obtener la línea específica por su número
                    DocumentLine line = document.GetLineByNumber(lineNumber);

                    // Obtener el texto de la línea
                    string lineText = document.GetText(line);

                    try
                    {
                        Lexer lexer = new Lexer(lineText);
                        List<Token> tokens = lexer.Tokenize();
                        TranslatedEditor.Text += "block \n";
                        for (int i = 0; i < tokens.Count; i++)
                        {
                            string temp = tokens[i].ToString();
                            if (i != tokens.Count - 1)
                                TranslatedEditor.Text += "\t'" + temp + "'" + ", \n";
                            else
                                TranslatedEditor.Text += "\t'" + temp + "'\n";
                        }
                        TranslatedEditor.Text += "End \n\n";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("error");
                    }
                }
            }
            

        }
    }
}

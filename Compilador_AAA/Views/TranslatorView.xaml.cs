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
            ObservableCollection<ErrorRow> filas = new ObservableCollection<ErrorRow>
{
    new ErrorRow { emp1="",Image = "../Resources/SVG/cross.png", Code = "A001", Description = "Gravedad Código DescripciónProyectoArchivoLíneaEstado suprimido Aviso (activo)CS8618El elemento propiedad \"Code\" que no acepta valores NULL debe contener un valor distinto de NULL al salir del constructor. Considere la posibilidad de agregar el modificador \"required'\"o declarar el propiedad como un valor que acepta valores NULL.Compilador_AAAC: Users Alejandro source repos alxv3rde Compilador_AAA Compilador_AAA Models ErrorRow.cs15", Line = "42", emp2="" },
    new ErrorRow { emp1="",Image = "../Resources/SVG/cross.png", Code = "A001", Description = "Primera fila", Line = "42", emp2="" },
    new ErrorRow { emp1="",Image = "../Resources/SVG/cross.png", Code = "A001", Description = "Primera fila", Line = "42", emp2="" },
    new ErrorRow { emp1="",Image = "../Resources/SVG/cross.png", Code = "A001", Description = "Primera fila", Line = "42", emp2="" },
    new ErrorRow { emp1="",Image = "../Resources/SVG/cross.png", Code = "A001", Description = "Primera fila", Line = "42", emp2="" },
    new ErrorRow { emp1="",Image = "../Resources/SVG/cross.png", Code = "A001", Description = "Primera fila", Line = "42", emp2="" },
    new ErrorRow { emp1="",Image = "../Resources/SVG/cross.png", Code = "A001", Description = "Primera fila", Line = "42", emp2="" },
    new ErrorRow { emp1="",Image = "../Resources/SVG/cross.png", Code = "A001", Description = "Primera fila", Line = "42", emp2="" },
};

            // Asignar la lista al ListView
            lvErrores.ItemsSource = filas;
        }

        private bool _isDragging = false;
        private Point _startPoint;

        private void btnTraducir_Click(object sender, RoutedEventArgs e)
        {
            CodeEditor.Text = string.Empty;
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
                            CodeEditor.Text += "'" + temp + "'" + ", ";
                        else
                            CodeEditor.Text += "'" + temp + "'\n";
                    }
                }
                catch (Exception ex)
                {
                    // Mostrar el error en caso de que algo salga mal
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }


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

        
    }
}

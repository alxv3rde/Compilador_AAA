using Compilador_AAA.Models;
using Compilador_AAA.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador_AAA.Traductor
{
    public class SemanticAnalyzer : IVisitor
    {
        private readonly HashSet<string> _variables = new HashSet<string>();
        private readonly HashSet<string> _functions = new HashSet<string>();
        private readonly HashSet<string> _classes = new HashSet<string>();

        public void Visit(Program program)
        {
            foreach (var stmt in program.children)
            {
                stmt.Accept(this);
            }
        }

        public void Visit(ClassDeclaration classDeclaration)
        {
            // Añadir la clase al conjunto de clases
            if (!_classes.Add(classDeclaration.Name))
            {
                // Error: clase ya definida
                TranslatorView.HandleError("La clase '" + classDeclaration.Name + "' ya está definida.", classDeclaration.StartLine, "SEM001");
            }

            foreach (var child in classDeclaration.Children)
            {
                child.Accept(this);
            }
        }
        public void Visit(IntegerLiteral integerLiteral) { /* Implementar según sea necesario */ }
        public void Visit(StringLiteral stringLiteral) { /* Implementar según sea necesario */ }
        public void Visit(DoubleLiteral doubleLiteral) { /* Implementar según sea necesario */ }

        public void Visit(VarDeclaration varDeclaration)
        {
            // Verificar si la variable ya está declarada
            if (!_variables.Add(varDeclaration.Identifier.ID))
            {
                // Error: variable ya definida
                TranslatorView.HandleError("La variable '" + varDeclaration.Identifier + "' ya está definida.", varDeclaration.StartLine, "SEM002");
            }
            else if (varDeclaration.Value!=null && varDeclaration.Value.Value.Kind != NodeType.BinaryExpr)
            {
                if (varDeclaration.VarType == "int" && varDeclaration.Value.Value.Kind != NodeType.IntegerLiteral)
                {
                    TranslatorView.HandleError(("Valor incorrecto para la variable entera"), varDeclaration.StartLine, "SEM003");
                }
                else if (varDeclaration.VarType == "string" && varDeclaration.Value.Value.Kind != NodeType.StringLiteral)
                {
                    TranslatorView.HandleError(("Valor incorrecto para la variable cadena"), varDeclaration.StartLine, "SEM004");
                }
                else if (varDeclaration.VarType == "double" && varDeclaration.Value.Value.Kind == NodeType.StringLiteral)
                {
                    TranslatorView.HandleError(("Valor incorrecto para la variable double"), varDeclaration.StartLine, "SEM005");
                }
            }

        }

        public void Visit(FunctionDeclaration functionDeclaration)
        {
            // Verificar si la función ya está definida
            if (!_functions.Add(functionDeclaration.Name))
            {
                // Error: función ya definida
                Console.WriteLine($"La función '{functionDeclaration.Name}' ya está definida.");
            }

            foreach (var param in functionDeclaration.Parameters)
            {
                // Se asume que los parámetros son identificadores
                if (param is Identifier identifier)
                {
                    if (!_variables.Add(identifier.ID))
                    {
                        // Error: parámetro ya definido
                        Console.WriteLine($"El parámetro '{identifier.ID}' ya está definido en la función '{functionDeclaration.Name}'.");
                    }
                }
            }

            foreach (var stmt in functionDeclaration.Children)
            {
                stmt.Accept(this);
            }
        }

        public void Visit(AssignmentExpr assignmentExpr)
        {
            // Verificar que la variable asignada esté declarada
            if (assignmentExpr.Identifier is Identifier identifier)
            {
                if (!_variables.Contains(identifier.ID))
                {
                    Console.WriteLine($"La variable '{identifier.ID}' no está declarada.");
                }
            }

            // Aquí puedes agregar más lógica para verificar el valor asignado
            assignmentExpr.Value.Accept(this);
        }

        public void Visit(BinaryExpr binaryExpr)
        {
            // Verificar expresiones izquierda y derecha
            binaryExpr.Left.Accept(this);
            binaryExpr.Right.Accept(this);
        }

        public void Visit(CallExpr callExpr)
        {
            // Verificar que la función llamada esté definida
            if (callExpr.Caller is Identifier caller)
            {
                if (!_functions.Contains(caller.ID))
                {
                    Console.WriteLine($"La función '{caller.ID}' no está definida.");
                }
            }

            // Verificar argumentos
            foreach (var arg in callExpr.Arguments)
            {
                arg.Accept(this);
            }
        }
        public void Visit(Identifier identifier)
        {
            if (!_variables.Contains(identifier.ID))
            {
                TranslatorView.HandleError($"El nombre '{identifier.ID}' no está definido.", identifier.StartLine, "SEM003");
            }
        }

        // Implementar otros métodos de visita según sea necesario
        public void Visit(MemberExpr memberExpr) { /* Implementar según sea necesario */ }


        public void Visit(Property property) { /* Implementar según sea necesario */ }
        public void Visit(ObjectLiteral objectLiteral) { /* Implementar según sea necesario */ }
    }
}

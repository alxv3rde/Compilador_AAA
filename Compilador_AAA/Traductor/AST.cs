using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador_AAA.Traductor
{
    public enum NodeType
    {
        // PROGRAM STRUCTURE
        Program,

        // DECLARATIONS
        ClassDeclaration,       // Representa una declaración de clase
        VarDeclaration,         // Declaración de variable
        FunctionDeclaration,    // Declaración de función o método

        // STATEMENTS
        ExpressionStatement,    // Sentencias de expresión que envuelven expresiones sin valor de retorno
        IfStatement,            // Sentencia if
        ForStatement,           // Sentencia for
        WhileStatement,         // Sentencia while
        ReturnStatement,        // Sentencia return

        // EXPRESSIONS
        AssignmentExpr,         // Expresión de asignación
        MemberExpr,             // Expresión de acceso a miembro (como obj.prop o obj["prop"])
        CallExpr,               // Expresión de llamada de función o método
        BinaryExpr,             // Expresión binaria (+, -, *, /, etc.)

        // LITERALS
        Property,               // Propiedad (clave-valor en un objeto literal)
        ObjectLiteral,          // Literal de objeto (JSON-like en muchos lenguajes)
        NumericLiteral,         // Literal numérico
        Identifier
    }

    // STATEMENT BASE CLASS
    public abstract class Stmt
    {
        public NodeType Kind { get; protected set; }

        protected Stmt(NodeType kind)
        {
            Kind = kind;

        }
        public abstract void Accept(IVisitor visitor);
    }

    // PROGRAM NODE
    public class Program : Stmt
    {
        public List<Stmt> children { get; set; }

        public Program() : base(NodeType.Program)
        {
            
            children= new List<Stmt>();
        }
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
    public class ClassDeclaration : Stmt
    {
        public TokenType AccessMod {  get; set; }
        public List<string> Parameters { get; set; }
        public string Name { get; set; }
        public List<Stmt> Children { get; set; }

        public ClassDeclaration(string name, List<string> parameters, List<Stmt> children, TokenType accessMod)
                    : base(NodeType.ClassDeclaration)
        {
            Name = name;
            Parameters = parameters;
            Children = children;
            AccessMod = accessMod;
        }

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class VarDeclaration : Stmt
    {
        public bool Constant { get; set; }
        public string Identifier { get; set; }
        public Expr Value { get; set; }

        public VarDeclaration(bool constant, string identifier, Expr value = null)
            : base(NodeType.VarDeclaration)
        {
            Constant = constant;
            Identifier = identifier;
            Value = value;
        }
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class FunctionDeclaration : Stmt
    {
        public List<string> Parameters { get; set; }
        public string Name { get; set; }
        public List<Stmt> Children { get; set; }

        public FunctionDeclaration(string name, List<string> parameters, List<Stmt> children)
            : base(NodeType.FunctionDeclaration)
        {
            Name = name;
            Parameters = parameters;
            Children = children;
        }

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    // EXPRESSION BASE CLASS
    public abstract class Expr : Stmt
    {
        protected Expr(NodeType kind) : base(kind) { }
    }

    public class AssignmentExpr : Expr
    {
        public Expr Assignee { get; set; }
        public Expr Value { get; set; }

        public AssignmentExpr(Expr assignee, Expr value)
            : base(NodeType.AssignmentExpr)
        {
            Assignee = assignee;
            Value = value;
        }
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class BinaryExpr : Expr
    {
        public Expr Left { get; set; }
        public Expr Right { get; set; }
        public string Operator { get; set; } // Consider defining an enum for specific operators

        public BinaryExpr(Expr left, Expr right, string operatorSymbol)
            : base(NodeType.BinaryExpr)
        {
            Left = left;
            Right = right;
            Operator = operatorSymbol;
        }
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class CallExpr : Expr
    {
        public List<Expr> Arguments { get; set; }
        public Expr Caller { get; set; }

        public CallExpr(Expr caller, List<Expr> arguments)
            : base(NodeType.CallExpr)
        {
            Caller = caller;
            Arguments = arguments;
        }
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class MemberExpr : Expr
    {
        public Expr Object { get; set; }
        public Expr Property { get; set; }
        public bool Computed { get; set; }

        public MemberExpr(Expr obj, Expr property, bool computed)
            : base(NodeType.MemberExpr)
        {
            Object = obj;
            Property = property;
            Computed = computed;
        }
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    // LITERALS
    public class Identifier : Expr
    {
        public string Symbol { get; set; }

        public Identifier(string symbol) : base(NodeType.Identifier)
        {
            Symbol = symbol;
        }
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class NumericLiteral : Expr
    {
        public double Value { get; set; }

        public NumericLiteral(double value) : base(NodeType.NumericLiteral)
        {
            Value = value;
        }
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class Property : Expr
    {
        public string Key { get; set; }
        public Expr Value { get; set; }

        public Property(string key, Expr value = null) : base(NodeType.Property)
        {
            Key = key;
            Value = value;
        }
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class ObjectLiteral : Expr
    {
        public List<Property> Properties { get; set; }

        public ObjectLiteral(List<Property> properties) : base(NodeType.ObjectLiteral)
        {
            Properties = properties;
        }
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

}

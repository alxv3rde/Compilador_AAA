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
        ClassDeclaration,       
        VarDeclaration,         
        FunctionDeclaration,    

        // STATEMENTS
        ExpressionStatement,    
        IfStatement,            
        ForStatement,           
        WhileStatement,         
        ReturnStatement,        

        // EXPRESSIONS
        AssignmentExpr,         
        MemberExpr,             
        CallExpr,               
        BinaryExpr,             

        // LITERALS
        Property,               
        ObjectLiteral,          
        NumericLiteral,         
        Identifier
    }
    
    // STATEMENT BASE CLASS
    public abstract class Stmt
    {
        private NodeType program;

        public NodeType Kind { get; protected set; }
        public int StartLine {  get; set; }
        
        protected Stmt(NodeType kind,int startLine)
        {
            Kind = kind;
            StartLine = startLine;
        }

        protected Stmt(NodeType program)
        {
            this.program = program;
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

        public ClassDeclaration(string name, List<string> parameters, List<Stmt> children, TokenType accessMod, int startLine)
                    : base(NodeType.ClassDeclaration,startLine)
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
        public string Value { get; set; }

        public VarDeclaration(int startLine,bool constant, string identifier, string value = null)
            : base(NodeType.VarDeclaration, startLine)
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
        public List<Stmt> Parameters { get; set; }
        public string Name { get; set; }
        public List<Stmt> Children { get; set; }

        public FunctionDeclaration(string name, List<Stmt> parameters, List<Stmt> children)
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
        protected Expr(NodeType kind, int startLine) : base(kind,startLine) { }
    }

    public class AssignmentExpr : Expr
    {
        public Expr Assignee { get; set; }
        public Expr Value { get; set; }

        public AssignmentExpr(Expr assignee, Expr value,int startLine)
            : base(NodeType.AssignmentExpr,startLine)
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

        public BinaryExpr(Expr left, Expr right, string operatorSymbol,int startLine)
            : base(NodeType.BinaryExpr, startLine)
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

        public CallExpr(Expr caller, List<Expr> arguments,int startLine)
            : base(NodeType.CallExpr, startLine)
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

        public MemberExpr(Expr obj, Expr property, bool computed,int startLine)
            : base(NodeType.MemberExpr,startLine)
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

        public Identifier(string symbol, int startLine) : base(NodeType.Identifier,startLine)
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

        public NumericLiteral(double value,int startLine) : base(NodeType.NumericLiteral, startLine)
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

        public Property(int startLine,string key, Expr value = null) : base(NodeType.Property,startLine)
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

        public ObjectLiteral(List<Property> properties,int startLine) : base(NodeType.ObjectLiteral,startLine)
        {
            Properties = properties;
        }
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

}

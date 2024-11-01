﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador_AAA.Traductor
{
    public interface IVisitor
    {
        //Program Structure
        void Visit(Program program);

        //Declarations
        void Visit(VarDeclaration varDeclaration);
        void Visit(ClassDeclaration classDeclaration);
        void Visit(FunctionDeclaration functionDeclaration);
        
        //Statements
        

        //Expresions
        void Visit(AssignmentExpr assignmentExpr);
        void Visit(BinaryExpr binaryExpr);
        void Visit(CallExpr callExpr);
        void Visit(MemberExpr memberExpr);

        //Literals
        void Visit(Identifier identifier);
        void Visit(NumericLiteral numericLiteral);
        void Visit(StringLiteral stringLiteral);
        void Visit(Property property);
        void Visit(ObjectLiteral objectLiteral);
    }
}

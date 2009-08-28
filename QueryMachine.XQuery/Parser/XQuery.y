/*
XQuery 1.0: An XML Query Language
W3C Recommendation 23 January 2007

http://www.w3.org/TR/xquery/
http://www.w3.org/TR/xquery-xpath-parsing

Copyright (c) 2009, Semyon A. Chertkov (semyonc@gmail.com)
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright
      notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright
      notice, this list of conditions and the following disclaimer in the
      documentation and/or other materials provided with the distribution.
    * Neither the name of author nor the
      names of its contributors may be used to endorse or promote products
      derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED ''AS IS'' AND ANY
EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL AUTHOR BE LIABLE FOR ANY
DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

%{

using System;
using System.IO;
using System.Collections;
using DataEngine.CoreServices;

namespace DataEngine.XQuery.Parser
{
	public class YYParser
	{	     
	    private Notation notation;
	            
	    public YYParser(Notation notation)
	    {
	    	errorText = new StringWriter();	    	 
	    	this.notation = notation; 
	    } 
	 
		public object yyparseSafe (TokenizerBase tok)
		{
			return yyparseSafe (tok, null);
		}

		public object yyparseSafe (TokenizerBase tok, object yyDebug)
		{ 
			try
			{
			    notation.Clear();
				return yyparse (tok, yyDebug);    
			}
			catch (XQueryException)
			{
				throw;
			}
			catch (Exception)  
			{
				throw new XQueryException ("{2} at line {1} pos {0}", tok.ColNo, tok.LineNo, errorText.ToString());
			}
		}

		public object yyparseDebug (TokenizerBase tok)
		{
			return yyparseSafe (tok, new yydebug.yyDebugSimple ());
		}	
		
%}

/* Reserved words */
%token ENCODING
%token PRESERVE
%token NO_PRESERVE
%token STRIP
%token INHERIT
%token NO_INHERIT
%token NAMESPACE
%token ORDERED
%token UNORDERED
%token EXTERNAL
%token AT
%token AS
%token IN
%token RETURN
%token FOR
%token LET
%token WHERE
%token ASCENDING
%token DESCENDING
%token COLLATION
%token SOME
%token EVERY
%token SATISFIES
%token TYPESWITCH
%token CASE
%token DEFAULT
%token IF
%token THEN
%token ELSE
%token DOCUMENT
%token ELEMENT
%token ATTRIBUTE
%token TEXT
%token COMMENT
%token AND
%token OR
%token TO
%token DIV
%token IDIV
%token MOD
%token UNION
%token INTERSECT
%token EXCEPT
%token INSTANCE_OF
%token TREAT_AS
%token CASTABLE_AS
%token CAST_AS
%token EQ NE LT GT GE LE
%token IS
%token VALIDATE
%token LAX
%token STRICT
%token NODE
%token DOUBLE_PERIOD

%token StringLiteral
%token IntegerLiteral 
%token DecimalLiteral 
%token DoubleLiteral   

%token NCName
%token QName
%token VarName
%token PragmaContents
%token S
%token Char
%token PredefinedEntityRef
%token CharRef

%token XQUERY_VERSION					   
%token MODULE_NAMESPACE
%token IMPORT_SCHEMA
%token IMPORT_MODULE
%token DECLARE_NAMESPACE
%token DECLARE_BOUNDARY_SPACE
%token DECLARE_DEFAULT_ELEMENT
%token DECLARE_DEFAULT_FUNCTION
%token DECLARE_DEFAULT_ORDER
%token DECLARE_OPTION
%token DECLARE_ORDERING
%token DECLARE_DEFAULT_ORDER
%token DECLARE_COPY_NAMESPACES
%token DECLARE_DEFAULT_COLLATION       
%token DECLARE_BASE_URI
%token DECLARE_VARIABLE
%token DECLARE_CONSTRUCTION
%token DECLARE_FUNCTION
%token EMPTY_GREATEST
%token EMPTY_LEAST
%token DEFAULT_ELEMENT
%token ORDER_BY
%token STABLE_ORDER_BY
%token PROCESSING_INSTRUCTION
%token DOCUMENT_NODE
%token SCHEMA_ELEMENT
%token SCHEMA_ATTRIBUTE

%token DOUBLE_SLASH								/* // */
%token COMMENT_BEGIN							/* <!-- */
%token COMMENT_END								/* --> */
%token PI_BEGIN									/* <? */
%token PI_END									/* ?> */
%token PRAGMA_BEGIN								/* (# */
%token PRAGMA_END								/* #) */
%token CDATA_BEGIN								/* <![CDATA[ */
%token CDATA_END								/* ]]> */
%token VOID										/* void() */
%token ITEM										/* item() */

%token AXIS_CHILD AXIS_DESCENDANT AXIS_ATTRIBUTE AXIS_SELF AXIS_DESCENDANT_OR_SELF
   AXIS_FOLLOWING_SIBLING AXIS_FOLLOWING AXIS_PARENT AXIS_ANCESTOR AXIS_PRECEDING_SIBLING
   AXIS_PRECEDING AXIS_ANCESTOR_OR_SELF AXIS_NAMESPACE

%token ML										/* * multiply operator */
%token Apos
%token BeginTag									/* < */
%token Indicator1								/* ? */
%token Indicator2								/* + */
%token Indicator3								/* * */
%token EscapeQuot                               /* "" */
%token EscapeApos                               /* '' */

%token XQComment
%token XQWhitespace

%start module

%%

module
  : versionDecl MainModule
  {
     notation.ConfirmTag(Tag.Module, Descriptor.Root, $2);
     $$ = notation.ResolveTag(Tag.Module);	 
  }
  | MainModule
  {
	 notation.ConfirmTag(Tag.Module, Descriptor.Root, $1);	 
	 $$ = notation.ResolveTag(Tag.Module);	 
  }  
  | versionDecl LibraryModule
  {
     notation.ConfirmTag(Tag.Module, Descriptor.Root, $2);	 
     $$ = notation.ResolveTag(Tag.Module);	 
  }  
  | LibraryModule
  {
	 notation.ConfirmTag(Tag.Module, Descriptor.Root, $1);	 
	 $$ = notation.ResolveTag(Tag.Module);	 
  }    
  ; 
  
versionDecl
  : XQUERY_VERSION StringLiteral Separator
  {
     notation.ConfirmTag(Tag.Module, Descriptor.Version, $2, null);
  }
  | XQUERY_VERSION StringLiteral ENCODING StringLiteral Separator
  {
     notation.ConfirmTag(Tag.Module, Descriptor.Version, $2, $4);
  }
  ; 
  
MainModule
  : Prolog QueryBody
  {
      $$ = notation.Confirm(new Symbol(Tag.Module), Descriptor.Query, $1, $2);
  }
  ;
  
LibraryModule
  : ModuleDecl Prolog
  {
      $$ = notation.Confirm(new Symbol(Tag.Module), Descriptor.Library, $1, $2);
  }
  ;
  
ModuleDecl
  : MODULE_NAMESPACE NCName '=' URILiteral Separator
  {
     $$ = notation.Confirm(new Symbol(Tag.Module), Descriptor.ModuleNamespace, $2, $4);
  }
  ;
  
Prolog
  : /* Empty */
  {
     $$ = null;
  }
  | decl_block1
  | decl_block2
  | decl_block1 decl_block2
  {
     $$ = Lisp.Append($1, $2);
  }
  ;
   
decl_block1
   : decl1 Separator
   {
      $$ = Lisp.Cons($1);
   }
   | decl_block1 decl1 Separator
   {
      $$ = Lisp.Append($1, Lisp.Cons($2));
   }
   ;
   
decl_block2
   : decl2 Separator
   {
      $$ = Lisp.Cons($1);
   }   
   | decl_block2 decl2 Separator
   {
      $$ = Lisp.Append($1, Lisp.Cons($2));
   }   
   ;
   
decl1
   : Setter
   | Import
   | NamespaceDecl
   | DefaultNamespaceDecl
   ;   
   
decl2
   : VarDecl
   | FunctionDecl
   | OptionDecl
   ;
      
Setter
   : BoundarySpaceDecl 
   | DefaultCollationDecl 
   | BaseURIDecl 
   | ConstructionDecl 
   | OrderingModeDecl 
   | EmptyOrderDecl 
   | CopyNamespacesDecl    
   ;
   
Import
   : SchemaImport 
   | ModuleImport  
   ;
   
Separator
  : ";"
  ;  
  
NamespaceDecl
  : DECLARE_NAMESPACE NCName '=' URILiteral
  {
      $$ = notation.Confirm(new Symbol(Tag.Module), Descriptor.Namespace, $2, $4);
  }
  ;  
  
BoundarySpaceDecl
  : DECLARE_BOUNDARY_SPACE PRESERVE
  {
      $$ = notation.Confirm(new Symbol(Tag.Module), 
		Descriptor.BoundarySpace, new TokenWrapper(Token.PRESERVE));
  }
  | DECLARE_BOUNDARY_SPACE STRIP
  {
      $$ = notation.Confirm(new Symbol(Tag.Module), 
		Descriptor.BoundarySpace, new TokenWrapper(Token.STRIP));  
  }
  ;

DefaultNamespaceDecl
  : DECLARE_DEFAULT_ELEMENT NAMESPACE URILiteral
  {
      $$ = notation.Confirm(new Symbol(Tag.Module), Descriptor.DefaultElement, $3);
  }
  | DECLARE_DEFAULT_FUNCTION NAMESPACE URILiteral
  {
      $$ = notation.Confirm(new Symbol(Tag.Module), Descriptor.DefaultFunction, $3);
  }  
  ;
  
OptionDecl
  : DECLARE_OPTION QName StringLiteral
  {
      $$ = notation.Confirm(new Symbol(Tag.Module), Descriptor.OptionDecl, $2, $3);
  }
  ;
  
OrderingModeDecl
  : DECLARE_ORDERING ORDERED
  {
      $$ = notation.Confirm(new Symbol(Tag.Module), 
		Descriptor.Ordering, new TokenWrapper(Token.ORDERED));  
  }
  | DECLARE_ORDERING UNORDERED
  {
      $$ = notation.Confirm(new Symbol(Tag.Module), 
		Descriptor.Ordering, new TokenWrapper(Token.UNORDERED));  
  }  
  ;
  
EmptyOrderDecl
  : DECLARE_DEFAULT_ORDER EMPTY_GREATEST
  {
      $$ = notation.Confirm(new Symbol(Tag.Module), 
		Descriptor.DefaultOrder, new TokenWrapper(Token.EMPTY_GREATEST));  
  }  
  | DECLARE_DEFAULT_ORDER EMPTY_LEAST
  {
      $$ = notation.Confirm(new Symbol(Tag.Module), 
		Descriptor.DefaultOrder, new TokenWrapper(Token.EMPTY_LEAST));  
  }    
  ;
  
CopyNamespacesDecl
  : DECLARE_COPY_NAMESPACES PreserveMode ',' InheritMode
  {
	  $$ = notation.Confirm(new Symbol(Tag.Module), 
	    Descriptor.CopyNamespace, $1, $3); 
  }
  ;
  
PreserveMode
  : PRESERVE
  {
      $$ = new TokenWrapper(Token.PRESERVE);
  }
  | NO_PRESERVE
  {
      $$ = new TokenWrapper(Token.NO_PRESERVE);
  }
  ;
  
InheritMode
  : INHERIT
  {
      $$ = new TokenWrapper(Token.INHERIT);
  }  
  | NO_INHERIT
  {
      $$ = new TokenWrapper(Token.NO_INHERIT);
  }    
  ;
  
DefaultCollationDecl
  : DECLARE_DEFAULT_COLLATION URILiteral
  {
      $$ = notation.Confirm(new Symbol(Tag.Module), Descriptor.DefaultCollation, $2);
  }
  ;
  
BaseURIDecl
  : DECLARE_BASE_URI URILiteral
  {
     $$ = notation.Confirm(new Symbol(Tag.Module), Descriptor.BaseUri, $2);
  }
  ;
  
SchemaImport
  : IMPORT_SCHEMA opt_SchemaPrefix URILiteral 
  {
     $$ = notation.Confirm(new Symbol(Tag.Module), 
         Descriptor.ImportSchema, $2, $3, null);
  }
  | IMPORT_SCHEMA opt_SchemaPrefix URILiteral AT URILiteralList
  {
     $$ = notation.Confirm(new Symbol(Tag.Module), 
         Descriptor.ImportSchema, $2, $3, $5);  
  }
  ;
  
opt_SchemaPrefix
  : /* Empty */
  { 
     $$ = null;
  }
  | SchemaPrefix
  ;   
  
URILiteralList
  : URILiteral
  {
     $$ = Lisp.Cons($1);
  }
  | URILiteralList ',' URILiteral
  {
     $$ = Lisp.Append($1, Lisp.Cons($3));
  }
  ;
  
SchemaPrefix
  : NAMESPACE NCName '='   
  {
     $$ = notation.Confirm(new Symbol(Tag.Module), Descriptor.Namespace, $2);
  }       
  | DEFAULT_ELEMENT NAMESPACE
  {
     $$ = notation.Confirm(new Symbol(Tag.Module), Descriptor.DefaultElement);
  }         
  ;  
                  
ModuleImport
  : IMPORT_MODULE URILiteral ModuleImportSpec
  {
     $$ = notation.Confirm(new Symbol(Tag.Module), Descriptor.ImportModule, $2, $3);
  }
  | IMPORT_MODULE NAMESPACE NCName '=' URILiteral ModuleImportSpec
  {
     $$ = notation.Confirm(new Symbol(Tag.Module), Descriptor.ImportModule, $3, $5, $6);
  }  
  ; 
  
ModuleImportSpec
  : /* Empty */
  {
     $$ = null;
  }
  |  AT URILiteralList
  {
     $$ = $2;
  };
  
VarDecl
  : DECLARE_VARIABLE '$' VarName opt_TypeDeclaration ':' '=' ExprSingle
  {
     $$ = notation.Confirm(new Symbol(Tag.Module), Descriptor.VarDecl, $3, $4, $7); 
  }
  | DECLARE_VARIABLE '$' VarName opt_TypeDeclaration EXTERNAL 
  {
     $$ = notation.Confirm(new Symbol(Tag.Module), Descriptor.VarDecl, $3, $4); 
  }  
  ;
  
opt_TypeDeclaration
  : /* Empty */
  {
     $$ = null;
  }
  | TypeDeclaration
  ;      
  
ConstructionDecl
  : DECLARE_CONSTRUCTION PRESERVE
  {
     $$ = notation.Confirm(new Symbol(Tag.Module), Descriptor.ConstructionDecl, 
		new TokenWrapper(Token.PRESERVE));
  }
  | DECLARE_CONSTRUCTION STRIP
  {
     $$ = notation.Confirm(new Symbol(Tag.Module), Descriptor.ConstructionDecl, 
		new TokenWrapper(Token.STRIP));
  }  
  ;
  
FunctionDecl
  : DECLARE_FUNCTION QName '(' opt_ParamList ')' FunctionBody
  {
     $$ = notation.Confirm(new Symbol(Tag.Module), Descriptor.DeclareFunction, $2, $4, $6);
  }
  | DECLARE_FUNCTION QName '(' opt_ParamList ')' AS SequenceType FunctionBody
  {
     $$ = notation.Confirm(new Symbol(Tag.Module), Descriptor.DeclareFunction, $2, $4, $7, $8);
  }  
  ;
  
FunctionBody
  : EnclosedExpr 
  | EXTERNAL
  {
     $$ = null;
  }
  ;  
  
opt_ParamList
  : /* Empty */
  {
     $$ = null;
  }
  | ParamList
  ;
  
ParamList
  : Param
  {
     $$ = Lisp.Cons($1);
  }
  | ParamList ',' Param
  {
     $$ = Lisp.Append($1, Lisp.Cons($3));
  }
  ;
  
Param
  : '$' VarName
  {
     $$ = $2;
  }
  | '$' VarName TypeDeclaration
  {
     $$ = $2;
     notation.Confirm((Symbol)$2, Descriptor.TypeDecl, $3);
  }
  ;
  
EnclosedExpr
  : '{' Expr '}'
  {
     $$ = $2;
  }
  ;
  
QueryBody
  : Expr
  ;   
  
Expr
  : ExprSingle
  {
     $$ = Lisp.Cons($1);
  }
  | Expr ',' ExprSingle
  {
     $$ = Lisp.Append($1, Lisp.Cons($3));
  }
  ;     
  
ExprSingle
  : FLWORExpr
  | QuantifiedExpr
  | TypeswitchExpr
  | IfExpr
  | OrExpr      
  ;
    
FLWORExpr
  : FLWORPrefix RETURN ExprSingle
  {
     $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.FLWORExpr, $1, null, null, $3);
  }
  | FLWORPrefix WhereClause RETURN ExprSingle
  {
     $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.FLWORExpr, $1, $2, null, $4);
  }  
  | FLWORPrefix OrderByClause RETURN ExprSingle
  {
     $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.FLWORExpr, $1, null, $2, $4);
  }    
  | FLWORPrefix WhereClause OrderByClause RETURN ExprSingle
  {
     $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.FLWORExpr, $1, $2, $3, $5);
  }      
  ;
  
FLWORPrefix
  : ForClause 
  {
     $$ = Lisp.Cons($1);
  } 
  | LetClause
  {
     $$ = Lisp.Cons($1);
  }   
  | FLWORPrefix ForClause
  {
     $$ = Lisp.Append($1, Lisp.Cons($2));
  }
  | FLWORPrefix LetClause
  {
     $$ = Lisp.Append($1, Lisp.Cons($2));
  }  
  ;
   
ForClause
  : FOR ForClauseBody
  {
     $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.For, $2);
  }
  ;      

ForClauseBody
  : ForClauseOperator
  {
     $$ = Lisp.Cons($1);
  }
  | ForClauseBody ',' ForClauseOperator
  {
     $$ = Lisp.Append($1, Lisp.Cons($3));
  }
  ;

ForClauseOperator
  : '$' VarName opt_TypeDeclaration opt_PositionVar IN ExprSingle
  {
     $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.ForClauseOperator, $2, $3, $4, $6);
  }
  ;
  
opt_PositionVar
  : /* Empty */
  {
     $$ = null;
  }
  | PositionVar
  ;
  
PositionVar
  : AT '$' VarName
  {
     $$ = $3;
  }
  ;   
  
LetClause
  : LET LetClauseBody
  {
     $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Let, $2);
  }  
  ;
  
LetClauseBody
  : LetClauseOperator
  {
     $$ = Lisp.Cons($1);
  }
  | LetClauseBody ',' LetClauseOperator
  {
     $$ = Lisp.Append($1, Lisp.Cons($3));
  }
  ;
  
LetClauseOperator
  : '$' VarName opt_TypeDeclaration ':' '=' ExprSingle
  {
     $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.LetClauseOperator, $2, $3, $6);
  }
  ;
  
WhereClause
  : WHERE ExprSingle
  {
     $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Where, $2);
  }
  ;        
  
OrderByClause
  : ORDER_BY OrderSpecList
  {
     $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.OrderBy, $2);
  }
  | STABLE_ORDER_BY OrderSpecList
  {
     $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.StableOrderBy, $2);
  }  
  ;
  
OrderSpecList
  : OrderSpec
  {
     $$ = Lisp.Cons($1);
  }
  | OrderSpecList ',' OrderSpec
  {
     $$ = Lisp.Append($1, Lisp.Cons($3));
  }
  ;
  
OrderSpec
  : ExprSingle 
  | ExprSingle OrderModifier
  {
     $$ = $1;
     notation.Confirm((Symbol)$1, Descriptor.Modifier, $2);
  }
  ;  
  
OrderModifier
  : OrderDirection
  {
     $$ = Lisp.List($1, null, null);
  }
  | OrderDirection EmptyOrderSpec
  {
     $$ = Lisp.List($1, $2, null);
  }  
  | OrderDirection COLLATION URILiteral
  {
     $$ = Lisp.List($1, null, $3);
  }    
  | OrderDirection EmptyOrderSpec COLLATION URILiteral
  {
     $$ = Lisp.List($1, $2, $4);
  }      
  ;
  
OrderDirection
  : ASCENDING
  {
     $$ = new TokenWrapper(Token.ASCENDING);
  }
  | DESCENDING
  {
     $$ = new TokenWrapper(Token.DESCENDING);
  }
  ;
  
EmptyOrderSpec
  : EMPTY_GREATEST
  {
     $$ = new TokenWrapper(Token.EMPTY_GREATEST); 
  }
  | EMPTY_LEAST
  {
     $$ = new TokenWrapper(Token.EMPTY_LEAST); 
  }  
  ;        
  
QuantifiedExpr
  : SOME QuantifiedExprBody SATISFIES ExprSingle
  {
     $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Some, $2, $4);
  }
  | EVERY QuantifiedExprBody SATISFIES ExprSingle
  {
     $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Every, $2, $4);
  }  
  ; 
  
QuantifiedExprBody
  : QuantifiedExprOper
  {
     $$ = Lisp.Cons($1);
  }
  | QuantifiedExprBody ',' QuantifiedExprOper
  {
     $$ = Lisp.Append($1, Lisp.Cons($3));
  }
  ; 
  
QuantifiedExprOper
  : '$' VarName opt_TypeDeclaration IN ExprSingle
  {
     $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.QuantifiedExprOper, $2, $3, $5);
  }
  ;  
  
TypeswitchExpr
  : TYPESWITCH '(' Expr ')' CaseClauseList DEFAULT RETURN ExprSingle
  {
     $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Typeswitch, $3, $5, $8); 
  }
  | TYPESWITCH '(' Expr ')' CaseClauseList DEFAULT '$' VarName RETURN ExprSingle 
  {
     $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Typeswitch, $3, $5, $8, $10); 
  }
  ;
  
CaseClauseList
  : CaseClause
  {
     $$ = Lisp.Cons($1);
  }
  | CaseClauseList CaseClause
  {
     $$ = Lisp.Append($1, Lisp.Cons($2));
  }
  ;
  
CaseClause
  : CASE '$' VarName AS SequenceType RETURN ExprSingle
  {
     $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Case, $3, $5, $7);
  }
  | CASE SequenceType RETURN ExprSingle
  {
     $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Case, $2, $4);
  }  
  ;
  
IfExpr
  : IF '(' Expr ')' THEN ExprSingle ELSE ExprSingle
  {
     $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.If, $3, $6, $8);
  }
  ;  
  
OrExpr
  : AndExpr
  | OrExpr OR AndExpr
  {
     $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Or, $1, $3);
  }
  ;
  
AndExpr
  : ComparisonExpr
  | AndExpr AND ComparisonExpr
  {
     $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.And, $1, $3);
  }  
  ;
  
ComparisonExpr
  : RangeExpr
  | RangeExpr ValueComp RangeExpr
  {
     $$ = notation.Confirm(new Symbol(Tag.Expr), 
		Descriptor.ValueComp, $1, $2, $3);
  }
  | RangeExpr GeneralComp RangeExpr
  {
     $$ = notation.Confirm(new Symbol(Tag.Expr), 
		Descriptor.GeneralComp, $1, $2, $3);
  }     
  | RangeExpr NodeComp RangeExpr   
  {
     $$ = notation.Confirm(new Symbol(Tag.Expr), 
		Descriptor.NodeComp, $1, $2, $3);
  }       
  ;
  
RangeExpr
  : AdditiveExpr
  | AdditiveExpr TO AdditiveExpr
  {
     $$ = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.Range, $1, $3);
  }
  ;
   
AdditiveExpr
  : MultiplicativeExpr
  | AdditiveExpr '+' MultiplicativeExpr
  {
     $$ = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.Add, $1, new TokenWrapper('+'), $3);
  }  
  | AdditiveExpr '-' MultiplicativeExpr 
  {
     $$ = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.Add, $1, new TokenWrapper('-'), $3);
  }    
  ;
  
MultiplicativeExpr
  : UnionExpr 
  | MultiplicativeExpr ML UnionExpr 
  {
     $$ = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.Mul, $1, new TokenWrapper(Token.ML), $3);
  }      
  | MultiplicativeExpr DIV UnionExpr  
  {
     $$ = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.Mul, $1, new TokenWrapper(Token.DIV), $3);
  }        
  | MultiplicativeExpr IDIV UnionExpr  
  {
     $$ = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.Mul, $1, new TokenWrapper(Token.IDIV), $3);
  }        
  | MultiplicativeExpr MOD UnionExpr  
  {
     $$ = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.Mul, $1, new TokenWrapper(Token.MOD), $3);
  }          
  ;
  
UnionExpr
  : IntersectExceptExpr
  | UnionExpr UNION IntersectExceptExpr
  {
     $$ = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.Union, $1, $3);  
  }
  | UnionExpr '|' IntersectExceptExpr 
  {
     $$ = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.Concatenate, $1, $3);  
  }  
  ;
  
IntersectExceptExpr
  : InstanceofExpr
  | IntersectExceptExpr INTERSECT InstanceofExpr
  {
     $$ = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.IntersectExcept, $1, new TokenWrapper(Token.INTERSECT), $3);  
  }    
  | IntersectExceptExpr EXCEPT InstanceofExpr
  {
     $$ = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.IntersectExcept, $1, new TokenWrapper(Token.EXCEPT), $3);  
  }      
  ;
  
InstanceofExpr
  : TreatExpr
  | TreatExpr INSTANCE_OF SequenceType
  {
     $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.InstanceOf, $1, $3);    
  }
  ;
  
TreatExpr
  : CastableExpr
  | CastableExpr TREAT_AS SequenceType      
  {
     $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.TreatAs, $1, $3);    
  }  
  ;
  
CastableExpr
  : CastExpr  
  | CastExpr CASTABLE_AS SingleType
  {
     $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.CastableAs, $1, $3);    
  }    
  ;
  
CastExpr
  : UnaryExpr
  | UnaryExpr CAST_AS SingleType
  {
     $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.CastAs, $1, $3);    
  }      
  ;
  
UnaryExpr
  : UnaryOperator ValueExpr  
  {
     if ($1 == null)
       $$ = $2;
     else
       $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Unary, $1, $2);
  }   
  ;
  
UnaryOperator
  : /* Empty */
  {
     $$ = null;
  }
  | '+' UnaryOperator 
  {
     $$ = Lisp.Append(Lisp.Cons(new TokenWrapper('+')), $2);
  }
  | '-' UnaryOperator
  {
     $$ = Lisp.Append(Lisp.Cons(new TokenWrapper('-')), $2);
  }  
  ; 
    
GeneralComp
  : '=' 
  {
     $$ = new Literal("=");
  }  
  | '!' '='  
  {
     $$ = new Literal("!=");
  }    
  | '<'
  {
     $$ = new Literal("<");
  }      
  | '<' '='
  {
     $$ = new Literal("<=");
  }        
  | '>'
  {
     $$ = new Literal(">");
  }        
  | '>' '='
  {
     $$ = new Literal(">=");
  }        
  ; 
  
ValueComp
  : EQ
  {
     $$ = new TokenWrapper(Token.EQ);
  }
  | NE
  {
     $$ = new TokenWrapper(Token.NE);
  }  
  | LT
  {
     $$ = new TokenWrapper(Token.LT);
  }  
  | LE
  {
     $$ = new TokenWrapper(Token.LE);
  }  
  | GT
  {
     $$ = new TokenWrapper(Token.GT);
  }  
  | GE
  {
     $$ = new TokenWrapper(Token.GE);
  }  
  ;
  
NodeComp
  : IS
  {
     $$ = new TokenWrapper(Token.IS);
  }    
  | '<' '<'
  {
     $$ = new Literal("<<");
  }
  | '>' '>'
  {
     $$ = new Literal(">>");
  }  
  ;   
  
ValueExpr
  : ValidateExpr 
  | PathExpr 
  | ExtensionExpr     
  ;
  
  
ValidateExpr
  : VALIDATE '{' Expr '}'
  {
     $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Validate, null, $3);
  }
  | VALIDATE ValidationMode '{' Expr '}'
  {
     $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Validate, $2, $4);
  }  
  ;   
  
ValidationMode
  : LAX
  {
     $$ = new TokenWrapper(Token.LAX);
  }
  | STRICT
  {
     $$ = new TokenWrapper(Token.STRICT);
  }  
  ;   
  
ExtensionExpr
  : PragmaList '{' Expr '}'
  {
     $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.ExtensionExpr, $1, $3);
  }
  ;
  
PragmaList
  : Pragma
  {
     $$ = Lisp.Cons($1);
  }
  | PragmaList Pragma
  {
     $$ = Lisp.Append($1, Lisp.Cons($2));
  }
  ;
  
Pragma
   : PRAGMA_BEGIN opt_S QName PragmaContents PRAGMA_END
   {
      $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Pragma, $3, $4);
   }
   ;     
         
PathExpr
  : '/' 
  {
      $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Child, new object[] { null });
  }
  | '/' RelativePathExpr
  {
      $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Child, $2);
  }  
  | DOUBLE_SLASH RelativePathExpr
  {
      $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Descendant, $2);
  }    
  | RelativePathExpr
  ;  
   
RelativePathExpr
  : StepExpr
  | RelativePathExpr '/' StepExpr
  {
      $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Child, $1, $3);
  }
  | RelativePathExpr DOUBLE_SLASH StepExpr
  {
      $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Descendant, $1, $3);
  }  
  ;
  
StepExpr
  : AxisStep 
  {
      $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.AxisStep, $1);
  }
  | FilterExpr
  {
      $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.FilterExpr, $1);
  }  
  ;
  
AxisStep
  : ForwardStep
  | ForwardStep PredicateList   
  {
      $$ = $1;
      notation.Confirm((Symbol)$1, Descriptor.PredicateList, $2);
  }  
  | ReverseStep
  | ReverseStep PredicateList   
  {
      $$ = $1;
      notation.Confirm((Symbol)$1, Descriptor.PredicateList, $2);
  }  
  ;
  
ForwardStep
   : ForwardAxis NodeTest 
   {
      $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.ForwardStep, $1, $2);
   }
   | AbbrevForwardStep  
   ;
   
ForwardAxis
   : AXIS_CHILD
   {
      $$ = new TokenWrapper(Token.AXIS_CHILD);
   }
   | AXIS_DESCENDANT
   {
      $$ = new TokenWrapper(Token.AXIS_DESCENDANT);
   }   
   | AXIS_ATTRIBUTE
   {
      $$ = new TokenWrapper(Token.AXIS_ATTRIBUTE);
   }   
   | AXIS_SELF
   {
      $$ = new TokenWrapper(Token.AXIS_SELF);
   }   
   | AXIS_DESCENDANT_OR_SELF
   {
      $$ = new TokenWrapper(Token.AXIS_DESCENDANT_OR_SELF);
   }   
   | AXIS_FOLLOWING_SIBLING
   {
      $$ = new TokenWrapper(Token.AXIS_FOLLOWING_SIBLING);
   }   
   | AXIS_FOLLOWING
   {
      $$ = new TokenWrapper(Token.AXIS_FOLLOWING);
   }   
   | AXIS_NAMESPACE
   {
      $$ = new TokenWrapper(Token.AXIS_NAMESPACE);
   }      
   ;   
    
AbbrevForwardStep
   : '@' NodeTest
   {  
	  $$ = notation.Confirm((Symbol)$2, Descriptor.AbbrevForward, $2); 
   }
   | NodeTest
   ;    
   
ReverseStep  
   : ReverseAxis NodeTest 
   {
      $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.ReverseStep, $1, $2);
   }   
   | AbbrevReverseStep
   ;
   
ReverseAxis
   : AXIS_PARENT
   {
      $$ = new TokenWrapper(Token.AXIS_PARENT);
   }      
   | AXIS_ANCESTOR
   {
      $$ = new TokenWrapper(Token.AXIS_ANCESTOR);
   }      
   | AXIS_PRECEDING_SIBLING
   {
      $$ = new TokenWrapper(Token.AXIS_PRECEDING_SIBLING);
   }      
   | AXIS_PRECEDING
   {
      $$ = new TokenWrapper(Token.AXIS_PRECEDING);
   }      
   | AXIS_ANCESTOR_OR_SELF
   {
      $$ = new TokenWrapper(Token.AXIS_ANCESTOR_OR_SELF);
   }      
   ;
   
AbbrevReverseStep
   : DOUBLE_PERIOD
   {
      $$ = new TokenWrapper(Token.DOUBLE_PERIOD);
   }
   ;   
   
NodeTest
   : KindTest
   | NameTest
   ;
   
NameTest
   : QName
   | Wildcard 
   ;
    
Wildcard
   : '*'  
   {
      $$ = new TokenWrapper('*');
   }
   | NCName ':' '*'
   {
      $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Wildcard1, $1);
   }
   | '*' ':' NCName 
   {
      $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Wildcard2, $3);
   }   
   ; 
    
FilterExpr
   : PrimaryExpr
   | PrimaryExpr PredicateList
   {
      $$ = $1;
      notation.Confirm((Symbol)$1, Descriptor.PredicateList, $2);
   }
   ;
 
PredicateList
   : Predicate
   {
      $$ = Lisp.Cons($1);
   }
   | PredicateList Predicate
   {
      $$ = Lisp.Append($1, Lisp.Cons($2));
   }
   ;

Predicate
   : '[' Expr ']'
   {
      $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Predicate, $2);
   }
   ;   
   
PrimaryExpr
   : Literal 
   | VarRef 
   | ParenthesizedExpr 
   | ContextItemExpr 
   | FunctionCall 
   | Constructor 
   | OrderedExpr 
   | UnorderedExpr
   ;
   
Literal
   : NumericLiteral 
   | StringLiteral   
   ;
   
NumericLiteral	   
   : IntegerLiteral 
   | DecimalLiteral 
   | DoubleLiteral   
   ;
   
VarRef
   : '$' VarName
   {
      $$ = $2;
   }
   ;   
      
ParenthesizedExpr
   : '(' ')'
   {
      $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.ParenthesizedExpr, new object[] { null });
   }
   | '(' Expr ')'      
   {
      $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.ParenthesizedExpr, $2);
   }   
   ;
   
ContextItemExpr
   : '.'
   {
      $$ = new TokenWrapper('.');
   }
   ; 
   
OrderedExpr
   : ORDERED '{' Expr '}'
   {
      $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Ordered, $3);
   }
   ; 
   
UnorderedExpr        
   : UNORDERED '{' Expr '}'
   {
      $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Unordered, $3);
   }   
   ;
   
FunctionCall
   : QName '(' ')'  
   {
      $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Funcall, $1, null);
   }
   | QName '(' Args ')'
   {
      $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Funcall, $1, $3);
   }   
   ;
   
Args
   : ExprSingle
   {
      $$ = Lisp.Cons($1);
   }
   | Args ',' ExprSingle
   {
      $$ = Lisp.Append($1, Lisp.Cons($3));
   }
   ;      
   
Constructor 
   : DirectConstructor
   | ComputedConstructor   
   ;
 
DirectConstructor
   : DirElemConstructor
   | DirCommentConstructor
   | DirPIConstructor   
   ;
   
DirElemConstructor
   : BeginTag QName opt_DirAttributeList '/' '>'
   {
       $$ = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirElemConstructor, $2, $3);
   }
   | BeginTag QName opt_DirAttributeList '>' '<' '/' QName opt_S '>'
   {
       $$ = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirElemConstructor, 
		 $2, $3, null, $7, $8);
   }   
   | BeginTag QName opt_DirAttributeList '>' DirElemContentList '<' '/' QName opt_S '>'
   {
       $$ = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirElemConstructor, 
		 $2, $3, $5, $8, $9);
   }      
   ;
    
DirElemContentList
   : DirElemContent
   {
      $$ = Lisp.Cons($1);
   }
   | DirElemContentList DirElemContent
   {      
      $$ = Lisp.Append($1, Lisp.Cons($2));
   }
   ;
   
opt_DirAttributeList
   : /* Empty */
   {
      $$ = null;
   }
   | DirAttributeList
   ;  
   
DirAttributeList
   : S DirAttribute
   {
      $$ = Lisp.List($1, $2);   
   }
   | DirAttributeList S 
   {
      $$ = Lisp.Append($1, Lisp.Cons($2));
   }
   | DirAttributeList S DirAttribute
   {
      $$ = Lisp.Append($1, Lisp.List($2, $3));
   }
   ;   
   
DirAttribute 
   : QName opt_S "=" opt_S '"' DirAttributeValueQuot '"'
   {
      $$ = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirAttribute,
		 $1, $2, $4, new Literal("\""), $6);
   }
   | QName opt_S "=" opt_S Apos DirAttributeValueApos Apos
   {
      $$ = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirAttribute,
		 $1, $2, $4, new Literal("\'"), $6);
   }   
   ;
   
DirAttributeValueQuot
   : EscapeQuot
   {
      $$ = Lisp.Cons(new TokenWrapper(Token.EscapeQuot));
   }
   | QuotAttrValueContent  
   {
      $$ = Lisp.Cons($1);
   }
   | DirAttributeValueQuot EscapeQuot
   {
      $$ = Lisp.Append($1, Lisp.Cons(new TokenWrapper(Token.EscapeQuot)));
   }   
   | DirAttributeValueQuot QuotAttrValueContent
   {
      $$ = Lisp.Append($1, Lisp.Cons($2));
   }   
   ;
   
DirAttributeValueApos
   : EscapeApos
   {
      $$ = Lisp.Cons(new TokenWrapper(Token.EscapeApos));
   }   
   | AposAttrValueContent        
   {
      $$ = Lisp.Cons($1);
   }   
   | DirAttributeValueApos EscapeApos  
   {
      $$ = Lisp.Append($1, Lisp.Cons(new TokenWrapper(Token.EscapeApos)));
   }
   | DirAttributeValueApos AposAttrValueContent        
   {
      $$ = Lisp.Append($1, Lisp.Cons($2));
   }   
   ;   
   
QuotAttrValueContent
   : QuotAttrContentChar
   | CommonContent
   ;
   
AposAttrValueContent	   
   : AposAttrContentChar
   | CommonContent      
   ;   
   
DirElemContent
   : DirectConstructor
   | ElementContentChar
   | CDataSection
   | CommonContent   
   ;
   
CommonContent
   : PredefinedEntityRef    
   | CharRef 
   | '{' '{'
   {
      $$ = new Literal("{{");
   }
   | '}' '}'
   {
      $$ = new Literal("}}");
   }   
   | EnclosedExpr 
   {
      $$ = notation.Confirm(new Symbol(Tag.CommonContent), Descriptor.EnclosedExpr, $1); 
   }  
   ;
   
DirCommentConstructor
   : COMMENT_BEGIN StringLiteral COMMENT_END
   {
      $$ = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirCommentConstructor, $2);
   }
   ;
   
DirPIConstructor
   : PI_BEGIN StringLiteral PI_END
   {
      $$ = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirPIConstructor, $2, null);
   }   
   | PI_BEGIN StringLiteral S StringLiteral PI_END
   {
      $$ = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirPIConstructor, $2, $4);
   }      
   ;  
   
CDataSection
   : CDATA_BEGIN StringLiteral CDATA_END
   {
      $$ = notation.Confirm(new Symbol(Tag.CData), Descriptor.CDataSection, $2);
   }   
   ; 
   
ComputedConstructor
   : CompDocConstructor
   | CompElemConstructor
   | CompAttrConstructor
   | CompTextConstructor
   | CompCommentConstructor
   | CompPIConstructor     
   ;
   
CompDocConstructor
   : DOCUMENT '{' Expr '}'
   {
      $$ = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompDocConstructor, $3);
   }
   ;  
   
CompElemConstructor
   : ELEMENT QName '{' ContentExpr '}'
   {
      $$ = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompElemConstructor, $2, $4);   
   }
   | ELEMENT QName '{' '}'   
   {
      $$ = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompElemConstructor, $2, null);   
   }   
   | ELEMENT '{' Expr '}' '{' ContentExpr '}'   
   {
      $$ = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompElemConstructor, $3, $6);   
   }   
   | ELEMENT '{' Expr '}' '{' '}'   
   {
      $$ = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompElemConstructor, $3, null);   
   }      
   ;
   
ContentExpr
   : Expr
   ;   
   
CompAttrConstructor
   : ATTRIBUTE QName '{' Expr '}'
   {
      $$ = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompAttrConstructor, $2, $4);   
   }   
   | ATTRIBUTE QName '{' '}'   
   {
      $$ = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompAttrConstructor, $2, null);   
   }      
   | ATTRIBUTE '{' Expr '}' '{' Expr '}'   
   {
      $$ = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompAttrConstructor, $3, $6);   
   }         
   | ATTRIBUTE '{' Expr '}' '{' '}'   
   {
      $$ = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompAttrConstructor, $3, null);   
   }            
   ;
   
CompTextConstructor
   : TEXT '{' Expr '}'
   {
      $$ = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompTextConstructor, $3);   
   }      
   ;
   
CompCommentConstructor	 
   : COMMENT '{' Expr '}'
   {
      $$ = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompCommentConstructor, $3);   
   }         
   ;   
      
CompPIConstructor
   : PROCESSING_INSTRUCTION NCName '{' Expr '}'      
   {
      $$ = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompPIConstructor, $2, $4);   
   }      
   | PROCESSING_INSTRUCTION NCName '{' '}'      
   {
      $$ = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompPIConstructor, $2, null);   
   }         
   | PROCESSING_INSTRUCTION '{' Expr '}' '{' Expr '}'      
   {
      $$ = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompPIConstructor, $3, $6);   
   }            
   | PROCESSING_INSTRUCTION '{' Expr '}' '{' '}'      
   {
      $$ = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompPIConstructor, $3, null);   
   }               
   ;
   
SingleType
   : AtomicType
   | AtomicType '?'
   {
      $$ = $1;
      notation.Confirm((Symbol)$1, Descriptor.Occurrence, 
		new TokenWrapper(Token.Indicator3));
   }
   ;
   
TypeDeclaration	   
   : AS SequenceType
   {
      $$ = $2;
   }   
   ;   
   
SequenceType
   : ItemType  
   | ItemType OccurrenceIndicator
   {
      $$ = $1;
      notation.Confirm((Symbol)$1, Descriptor.Occurrence, $2);
   }
   | VOID
   {
      $$ = new TokenWrapper(Token.VOID);
   }
   ;   
   
OccurrenceIndicator	   
   : Indicator1  /* * */
   {
      $$ = new TokenWrapper(Token.Indicator1);
   }
   | Indicator2  /* + */
   {
      $$ = new TokenWrapper(Token.Indicator2);
   }  
   | Indicator3  /* ? */
   {
      $$ = new TokenWrapper(Token.Indicator3);
   }   
   ;
   
ItemType	   
   : AtomicType 
   | KindTest 
   | ITEM
   {
      $$ = new TokenWrapper(Token.ITEM);
   }
   ;

AtomicType 
   : QName
   ;
   
KindTest
   : KindTestBody
   {
      $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.KindTest, $1);
   }
   
KindTestBody
   : DocumentTest
   | ElementTest
   | AttributeTest
   | SchemaElementTest
   | SchemaAttributeTest
   | PITest
   | CommentTest
   | TextTest   
   | AnyKindTest   
   ;
   
AnyKindTest	   
   : NODE '(' ')'
   {
       $$ = new TokenWrapper(Token.NODE);
   }
   ;
   
DocumentTest
   : DOCUMENT_NODE '(' ')'
   {
       $$ = new TokenWrapper(Token.DOCUMENT_NODE);
   }
   | DOCUMENT_NODE '(' ElementTest ')'
   {
       $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.DocumentNode, $3);
   }
   | DOCUMENT_NODE '(' SchemaElementTest ')'
   {
       $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.DocumentNode, $3);
   }   
   ; 
   
TextTest
   : TEXT '(' ')'
   {
       $$ = new TokenWrapper(Token.TEXT);
   }
   ;
   
CommentTest
   : COMMENT '(' ')'
   {
      $$ = new TokenWrapper(Token.COMMENT);
   }
   ;   
   ;
   
PITest
   : PROCESSING_INSTRUCTION '(' ')'
   {
       $$ = new TokenWrapper(Token.PROCESSING_INSTRUCTION);
   }
   | PROCESSING_INSTRUCTION '(' NCName ')'
   {
       $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.ProcessingInstruction, $3);
   }   
   | PROCESSING_INSTRUCTION '(' StringLiteral ')'
   {
       $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.ProcessingInstruction, $3);
   }      
   ;
      
ElementTest
   : ELEMENT '(' ')'
   {
       $$ = new TokenWrapper(Token.ELEMENT);
   }   
   | ELEMENT '(' ElementNameOrWildcard ')'   
   {
       $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Element, $3);
   }      
   | ELEMENT '(' ElementNameOrWildcard ',' TypeName ')'   
   {
       $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Element, $3, $5);
   }         
   | ELEMENT '(' ElementNameOrWildcard ',' TypeName '?' ')'   
   {
       $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Element, 
		$3, $5, new TokenWrapper('?'));
   }            
   ;
   
ElementNameOrWildcard
   : ElementName
   | '*'
   {
      $$ = new TokenWrapper('*');
   }   
   ;   
   
AttributeTest
   : ATTRIBUTE '(' ')'
   {
       $$ = new TokenWrapper(Token.ATTRIBUTE);
   }      
   | ATTRIBUTE '(' AttributeOrWildcard ')'
   {
       $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Attribute, $3);
   }         
   | ATTRIBUTE '(' AttributeOrWildcard ',' TypeName ')'
   {
       $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Attribute, $3, $5);
   }            
   ;
         
AttributeOrWildcard
   : AttributeName
   | '*'
   {
      $$ = new TokenWrapper('*');
   }
   ;    
   
SchemaElementTest
   : SCHEMA_ELEMENT '(' ElementName ')'    
   {
      $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.SchemaElement, $3);
   }             
   ;
    
SchemaAttributeTest
   : SCHEMA_ATTRIBUTE '(' AttributeName ')'
   {
      $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.SchemaAttribute, $3);
   }                
   ;    
    
AttributeName	   
   : QName
   ;

ElementName	   
   : QName
   ;
    
TypeName	   
   : QName    
   ;
    
opt_S
   : /* Empty */
   {
      $$ = null;
   }
   | S
   ;    
     
QuotAttrContentChar
   : Char
   ;
   
AposAttrContentChar
   : Char
   ;   
   
ElementContentChar
   : Char
   ;  
   
URILiteral
   : StringLiteral
   ;       

%%
}

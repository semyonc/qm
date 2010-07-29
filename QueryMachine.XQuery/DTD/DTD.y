/*
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

#pragma warning disable 162

using System;
using System.IO;
using System.Collections;
using DataEngine.CoreServices;

namespace DataEngine.XQuery.DTD
{
   internal class DTDID
   {
	    public static readonly object ElementDecl =  ATOM.Create("elementDecl");       
	    public static readonly object AttlistDecl =  ATOM.Create("attrlistDecl");       
	    public static readonly object GEDecl =  ATOM.Create("GEDecl");       
	    public static readonly object PEDecl =  ATOM.Create("PEDecl");       
	    public static readonly object NotationDecl =  ATOM.Create("notationDecl");     
	    public static readonly object Comment =  ATOM.Create("comment");       
	    public static readonly object PI =  ATOM.Create("pi");       
	    public static readonly object EMPTY =  ATOM.Create("dtd:EMPTY");       
	    public static readonly object ANY =  ATOM.Create("dtd:ANY");       
	    public static readonly object MixedContent =  ATOM.Create("mixed");       
	    public static readonly object PCDATA =  ATOM.Create("dtd:PCDATA");       
	    public static readonly object ZeroOrOne = ATOM.Create("ZeroOrOne");       
	    public static readonly object OneOrMore = ATOM.Create("OneOrMore");       
	    public static readonly object ZeroOrMore = ATOM.Create("ZeroOrMore");       	    
	    public static readonly object Choice = ATOM.Create("choice");       	    
	    public static readonly object Seq = ATOM.Create("seq");    
	    public static readonly object AttDef = ATOM.Create("attDef");    
	    public static readonly object CDATA = ATOM.Create("dtd:CDATA");
	    public static readonly object ID = ATOM.Create("dtd:ID");
	    public static readonly object IDREF = ATOM.Create("dtd:IDREF");
	    public static readonly object IDREFS = ATOM.Create("dtd:IDREFS");
	    public static readonly object ENTITY = ATOM.Create("dtd:ENTITY");
	    public static readonly object ENTITIES = ATOM.Create("dtd:ENTITIES");
	    public static readonly object NMTOKEN = ATOM.Create("dtd:NMTOKEN");
	    public static readonly object NMTOKENS = ATOM.Create("dtd:NMTOKENS");	
	    public static readonly object Notation = ATOM.Create("notation");	       
        public static readonly object REQUIRED = ATOM.Create("dtd:REQUIRED");	       
	    public static readonly object IMPLIED = ATOM.Create("dtd:IMPLIED");	       
        public static readonly object FIXED = ATOM.Create("dtd:FIXED");	       
        public static readonly object PUBLIC = ATOM.Create("dtd:PUBLIC");
        public static readonly object SYSTEM = ATOM.Create("dtd:SYSTEM");
        public static readonly object NData = ATOM.Create("dtd:NData");
   }

   internal class DTDParser
   {    
        private DTDParser()
        {
	    	errorText = new StringWriter();	    	             
        }
   
		private object yyparseSafe (Tokenizer tok)
		{
			return yyparseSafe (tok, null);
		}
		
		private Tokenizer _tokenizer;

		private object yyparseSafe (Tokenizer tok, object yyDebug)
		{ 
			try
			{			    
			    _tokenizer = tok;
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

		private object yyparseDebug (Tokenizer tok)
		{
			return yyparseSafe (tok, new yydebug.yyDebugSimple ());
		}	
		
		public static object ParseInline(string documentType, string baseUri)
		{
		    Tokenizer tok = new Tokenizer(baseUri, documentType);
		    DTDParser parser = new DTDParser();
		    return parser.yyparseSafe(tok);		
		}
		
		public static object ParseExternal(string publicId, string systemId, string baseUri)
		{
		    Tokenizer tok = new Tokenizer(publicId, systemId, baseUri);		    
		    DTDParser parser = new DTDParser();
		    return parser.yyparseSafe(tok);		
		}			
				
%}

/* Reserved words */
%token Name
%token Literal
%token ELEMENT_DECL			
%token ATTLIST_DECL  		
%token ENTITY_DECL          
%token NOTATION_DECL        
%token PI                   
%token Comment              
%token EMPTY
%token ANY
%token CDATA
%token ID 
%token IDREF 
%token IDREFS 
%token ENTITY 
%token ENTITIES 
%token NMTOKEN 
%token NMTOKENS
%token NOTATION
%token EMPTY
%token ANY
%token NDATA
%token REQUIRED             
%token IMPLIED              
%token FIXED                
%token SYSTEM
%token PUBLIC
%token PCDATA    
%token INCLUDE
%token IGNORE           

%start intSubset

%%

intSubset
  :  markupdecl
  {
      $$ = Lisp.Cons($1);
  }
  |  intSubset markupdecl  
  {
      $$ = Lisp.Append($1, Lisp.Cons($2));
  }
  |  IGNORE
  {
      $$ = null;
  }
  |  INCLUDE intSubset ']'']' '>' 
  {
      $$ = $2;
  }
  |  intSubset IGNORE
  {
      $$ = $1;
  }
  |  intSubset INCLUDE intSubset ']'']' '>' 
  {
      $$ = Lisp.Append($1, $3);
  }
  ;
   
markupdecl
   : elementdecl | AttlistDecl | EntityDecl | NotationDecl | PI | Comment
   ;
      
elementdecl   
   : ELEMENT_DECL Name contentspec '>'
   {
      $$ = Lisp.List(DTDID.ElementDecl, $2, $3);
   }
   ;
   
AttlistDecl
   : ATTLIST_DECL Name AttDef_List '>'
   {
      $$ = Lisp.List(DTDID.AttlistDecl, $2, $3);
   }   
   ;
   
EntityDecl	     
   : GEDecl 
   | PEDecl   
   ;
   
GEDecl	   
   : ENTITY_DECL Name EntityDef '>'
   {
      $$ = Lisp.List(DTDID.GEDecl, $2, $3);
   }   
   ;
   
PEDecl	   
   : ENTITY_DECL '%' Name PEDef '>' 
   {
      $$ = Lisp.List(DTDID.PEDecl, $3, $4);
      if (Lisp.IsFunctor($4, DTDID.PUBLIC))
        _tokenizer.AddPE((string)$3, Lisp.SArg1($4), Lisp.SArg2($4));
      else if (Lisp.IsFunctor($4, DTDID.SYSTEM))
        _tokenizer.AddPE((string)$3, null, Lisp.SArg1($4));
      else
        _tokenizer.AddPE((string)$3, (string)$4);       
   }      
   ;
   
NotationDecl
   : NOTATION_DECL Name ExternalID '>'
   {
      $$ = Lisp.List(DTDID.NotationDecl, $2, $3);
   }      
   | NOTATION_DECL Name PublicID '>'
   {
      $$ = Lisp.List(DTDID.NotationDecl, $2, $3);
   }         
   ;
            
contentspec	   
   : EMPTY
   {
      $$ = DTDID.EMPTY;
   }
   | ANY 
   {
      $$ = DTDID.ANY;
   }   
   | Mixed   
   {
      $$ = Lisp.List(DTDID.MixedContent, $1);    
   } 
   | children   
   ;

Mixed
   : '(' PCDATA ')'
   {
      $$ = Lisp.Cons(DTDID.PCDATA);
   }
   | '(' PCDATA '|' name_list ')' '*'
   {
      $$ = Lisp.Append(Lisp.Cons(DTDID.PCDATA), $4);
   }
   ;
     
children	   
   : choice 
   | choice spec
   {
      $$ = Lisp.List($2, $1);
   }
   | seq 
   | seq spec
   {
      $$ = Lisp.List($2, $1);
   }   
   ;

cp
  : Name
  | Name spec
  | choice
  | choice spec
  | seq
  | seq spec
  ;
  
choice
  : '(' cp '|' choice_list ')'
  {
     $$ = Lisp.List(DTDID.Choice, Lisp.Append(Lisp.Cons($2), $4));  
  }
  ;
  
seq
  : '(' cp ')'  
  {
     $$ = Lisp.List(DTDID.Seq, Lisp.Cons($2));  
  }
  | '(' cp ',' seq_list ')'
  {
     $$ = Lisp.List(DTDID.Seq, Lisp.Append(Lisp.Cons($2), $4));  
  }  
  ;
  
choice_list
  : cp
  {
      $$ = Lisp.Cons($1);
  }
  | choice_list '|' cp
  {
      $$ = Lisp.Append($1, Lisp.Cons($3));
  }
  ;
  
seq_list
  : cp
  {
      $$ = Lisp.Cons($1);
  } 
  | seq_list ',' cp
  {
      $$ = Lisp.Append($1, Lisp.Cons($3));
  }  
  ;
  
name_list
   : Name
   {
      $$ = Lisp.Cons($1);
   }
   | name_list '|' Name
   {
      $$ = Lisp.Append($1, Lisp.Cons($3));
   }
   ; 
          
spec
   : '?' 
   {
       $$ = DTDID.ZeroOrOne;
   }
   | '*' 
   {
       $$ = DTDID.ZeroOrMore;
   }   
   | '+'
   {
       $$ = DTDID.OneOrMore;
   }      
   ;
   
AttDef_List
   : AttDef
   {   
      $$ = Lisp.Cons($1);
   }
   | AttDef_List AttDef
   {
      $$ = Lisp.Append($1, Lisp.Cons($2));
   }
   ;
   
AttDef
   : Name AttType DefaultDecl
   {
       $$ = Lisp.List(DTDID.AttDef, $1, $2, $3);
   }
   ;
   
AttType
   : StringType | TokenizedType | EnumeratedType
   ;
   
StringType
   : CDATA
   {
       $$ = DTDID.CDATA;
   }
   ;
   
TokenizedType	
   : ID     
   {
        $$ = DTDID.ID;
   }          
   | IDREF 
   {
        $$ = DTDID.IDREF;
   }             
   | IDREFS 
   {
        $$ = DTDID.IDREFS;
   }             
   | ENTITY 
   {
        $$ = DTDID.ENTITY;
   }             
   | ENTITIES 
   {
        $$ = DTDID.ENTITIES;
   }             
   | NMTOKEN 
   {
        $$ = DTDID.NMTOKEN;
   }             
   | NMTOKENS
   {
        $$ = DTDID.NMTOKENS;
   }             
   ;
   
EnumeratedType
   : NotationType 
   | Enumeration   
   ;
   
NotationType 
   : NOTATION '(' Name ')'
   {
       $$ = Lisp.List(DTDID.Notation, $3);
   }
   | NOTATION '(' Name '|' name_list ')'
   {
       $$ = Lisp.List(DTDID.Notation, Lisp.Append(Lisp.Cons($3), $5));
   }
   ;
   
Enumeration   
   : '(' Name ')'
   {
      $$ = Lisp.Cons($2);
   }
   | '(' Name '|' name_list ')'
   {
      $$ = Lisp.Append(Lisp.Cons($2), $4);
   }
   ;   
   
DefaultDecl	
   : REQUIRED
   {
      $$ = DTDID.REQUIRED;
   }
   | IMPLIED
   {
      $$ = DTDID.IMPLIED;
   }   
   | FIXED AttValue
   {
      $$ = Lisp.List(DTDID.FIXED, $2);
   }
   | AttValue
   ;
     
EntityDef	   
   : EntityValue 
   | ExternalID
   | ExternalID NDataDecl
   {
      $$ = Lisp.List(DTDID.NData, $1, $2);
   }
   ;
   
PEDef
   : EntityValue 
   | ExternalID
   ;   
   
ExternalID
   : SYSTEM SystemLiteral
   {
      $$ = Lisp.List(DTDID.SYSTEM, $2);
   }
   | PUBLIC PubidLiteral SystemLiteral 
   {
      $$ = Lisp.List(DTDID.PUBLIC, $2, $3);
   }   
   ;
   
PublicID
   : PUBLIC PubidLiteral
   {
      $$ = Lisp.List(DTDID.PUBLIC, $2);
   }   
   ;  
   
NDataDecl
   : NDATA Name
   {
      $$ = $2;
   }
   ;
   
AttValue
   : Literal
   ;    
   
EntityValue
   : Literal
   ;
   
PubidLiteral
   : Literal
   ;
   
SystemLiteral
   : Literal
   ;   
      
%%
}  
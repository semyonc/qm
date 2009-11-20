
// created by jay 0.7 (c) 1998 Axel.Schreiner@informatik.uni-osnabrueck.de

#line 29 "DTD.y"

#pragma warning disable 162

using System;
using System.IO;
using System.Collections;
using DataEngine.CoreServices;

namespace DataEngine.XQuery.DTD
{
   internal class DTDID
   {
	    public static readonly object ElementDecl =  Lisp.Defatom("elementDecl");       
	    public static readonly object AttlistDecl =  Lisp.Defatom("attrlistDecl");       
	    public static readonly object GEDecl =  Lisp.Defatom("GEDecl");       
	    public static readonly object PEDecl =  Lisp.Defatom("PEDecl");       
	    public static readonly object NotationDecl =  Lisp.Defatom("notationDecl");     
	    public static readonly object Comment =  Lisp.Defatom("comment");       
	    public static readonly object PI =  Lisp.Defatom("pi");       
	    public static readonly object EMPTY =  Lisp.Defatom("dtd:EMPTY");       
	    public static readonly object ANY =  Lisp.Defatom("dtd:ANY");       
	    public static readonly object MixedContent =  Lisp.Defatom("mixed");       
	    public static readonly object PCDATA =  Lisp.Defatom("dtd:PCDATA");       
	    public static readonly object ZeroOrOne = Lisp.Defatom("ZeroOrOne");       
	    public static readonly object OneOrMore = Lisp.Defatom("OneOrMore");       
	    public static readonly object ZeroOrMore = Lisp.Defatom("ZeroOrMore");       	    
	    public static readonly object Choice = Lisp.Defatom("choice");       	    
	    public static readonly object Seq = Lisp.Defatom("seq");    
	    public static readonly object AttDef = Lisp.Defatom("attDef");    
	    public static readonly object CDATA = Lisp.Defatom("dtd:CDATA");
	    public static readonly object ID = Lisp.Defatom("dtd:ID");
	    public static readonly object IDREF = Lisp.Defatom("dtd:IDREF");
	    public static readonly object IDREFS = Lisp.Defatom("dtd:IDREFS");
	    public static readonly object ENTITY = Lisp.Defatom("dtd:ENTITY");
	    public static readonly object ENTITIES = Lisp.Defatom("dtd:ENTITIES");
	    public static readonly object NMTOKEN = Lisp.Defatom("dtd:NMTOKEN");
	    public static readonly object NMTOKENS = Lisp.Defatom("dtd:NMTOKENS");	
	    public static readonly object Notation = Lisp.Defatom("notation");	       
        public static readonly object REQUIRED = Lisp.Defatom("dtd:REQUIRED");	       
	    public static readonly object IMPLIED = Lisp.Defatom("dtd:IMPLIED");	       
        public static readonly object FIXED = Lisp.Defatom("dtd:FIXED");	       
        public static readonly object PUBLIC = Lisp.Defatom("dtd:PUBLIC");
        public static readonly object SYSTEM = Lisp.Defatom("dtd:SYSTEM");
        public static readonly object NData = Lisp.Defatom("dtd:NData");
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
				
#line default
  /** error text **/
  public readonly TextWriter errorText = null;

  /** simplified error message.
      @see <a href="#yyerror(java.lang.String, java.lang.String[])">yyerror</a>
    */
  public void yyerror (string message) {
    yyerror(message, null);
  }

  /** (syntax) error message.
      Can be overwritten to control message format.
      @param message text to be displayed.
      @param expected vector of acceptable tokens, if available.
    */
  public void yyerror (string message, string[] expected) {
    if ((errorText != null) && (expected != null) && (expected.Length  > 0)) {
      errorText.Write (message+", expecting");
      for (int n = 0; n < expected.Length; ++ n)
        errorText.Write (" "+expected[n]);
        errorText.WriteLine ();
    } else
      errorText.WriteLine (message);
  }

  /** debugging support, requires the package jay.yydebug.
      Set to null to suppress debugging messages.
    */
//t  protected yydebug.yyDebug debug;

  protected static  int yyFinal = 9;
//t  public static  string [] yyRule = {
//t    "$accept : intSubset",
//t    "intSubset : markupdecl",
//t    "intSubset : intSubset markupdecl",
//t    "intSubset : IGNORE",
//t    "intSubset : INCLUDE intSubset ']' ']' '>'",
//t    "intSubset : intSubset IGNORE",
//t    "intSubset : intSubset INCLUDE intSubset ']' ']' '>'",
//t    "markupdecl : elementdecl",
//t    "markupdecl : AttlistDecl",
//t    "markupdecl : EntityDecl",
//t    "markupdecl : NotationDecl",
//t    "markupdecl : PI",
//t    "markupdecl : Comment",
//t    "elementdecl : ELEMENT_DECL Name contentspec '>'",
//t    "AttlistDecl : ATTLIST_DECL Name AttDef_List '>'",
//t    "EntityDecl : GEDecl",
//t    "EntityDecl : PEDecl",
//t    "GEDecl : ENTITY_DECL Name EntityDef '>'",
//t    "PEDecl : ENTITY_DECL '%' Name PEDef '>'",
//t    "NotationDecl : NOTATION_DECL Name ExternalID '>'",
//t    "NotationDecl : NOTATION_DECL Name PublicID '>'",
//t    "contentspec : EMPTY",
//t    "contentspec : ANY",
//t    "contentspec : Mixed",
//t    "contentspec : children",
//t    "Mixed : '(' PCDATA ')'",
//t    "Mixed : '(' PCDATA '|' name_list ')' '*'",
//t    "children : choice",
//t    "children : choice spec",
//t    "children : seq",
//t    "children : seq spec",
//t    "cp : Name",
//t    "cp : Name spec",
//t    "cp : choice",
//t    "cp : choice spec",
//t    "cp : seq",
//t    "cp : seq spec",
//t    "choice : '(' cp '|' choice_list ')'",
//t    "seq : '(' cp ')'",
//t    "seq : '(' cp ',' seq_list ')'",
//t    "choice_list : cp",
//t    "choice_list : choice_list '|' cp",
//t    "seq_list : cp",
//t    "seq_list : seq_list ',' cp",
//t    "name_list : Name",
//t    "name_list : name_list '|' Name",
//t    "spec : '?'",
//t    "spec : '*'",
//t    "spec : '+'",
//t    "AttDef_List : AttDef",
//t    "AttDef_List : AttDef_List AttDef",
//t    "AttDef : Name AttType DefaultDecl",
//t    "AttType : StringType",
//t    "AttType : TokenizedType",
//t    "AttType : EnumeratedType",
//t    "StringType : CDATA",
//t    "TokenizedType : ID",
//t    "TokenizedType : IDREF",
//t    "TokenizedType : IDREFS",
//t    "TokenizedType : ENTITY",
//t    "TokenizedType : ENTITIES",
//t    "TokenizedType : NMTOKEN",
//t    "TokenizedType : NMTOKENS",
//t    "EnumeratedType : NotationType",
//t    "EnumeratedType : Enumeration",
//t    "NotationType : NOTATION '(' Name ')'",
//t    "NotationType : NOTATION '(' Name '|' name_list ')'",
//t    "Enumeration : '(' Name ')'",
//t    "Enumeration : '(' Name '|' name_list ')'",
//t    "DefaultDecl : REQUIRED",
//t    "DefaultDecl : IMPLIED",
//t    "DefaultDecl : FIXED AttValue",
//t    "DefaultDecl : AttValue",
//t    "EntityDef : EntityValue",
//t    "EntityDef : ExternalID",
//t    "EntityDef : ExternalID NDataDecl",
//t    "PEDef : EntityValue",
//t    "PEDef : ExternalID",
//t    "ExternalID : SYSTEM SystemLiteral",
//t    "ExternalID : PUBLIC PubidLiteral SystemLiteral",
//t    "PublicID : PUBLIC PubidLiteral",
//t    "NDataDecl : NDATA Name",
//t    "AttValue : Literal",
//t    "EntityValue : Literal",
//t    "PubidLiteral : Literal",
//t    "SystemLiteral : Literal",
//t  };
  protected static  string [] yyName = {    
    "end-of-file",null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,"'%'",null,
    null,"'('","')'","'*'","'+'","','",null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,"'>'","'?'",null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    "']'",null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,"'|'",null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,"Name",
    "Literal","ELEMENT_DECL","ATTLIST_DECL","ENTITY_DECL","NOTATION_DECL",
    "PI","Comment","EMPTY","ANY","CDATA","ID","IDREF","IDREFS","ENTITY",
    "ENTITIES","NMTOKEN","NMTOKENS","NOTATION","NDATA","REQUIRED",
    "IMPLIED","FIXED","SYSTEM","PUBLIC","PCDATA","INCLUDE","IGNORE",
  };

  /** index-checked interface to yyName[].
      @param token single character or %token value.
      @return token name or [illegal] or [unknown].
    */
  public static string yyname (int token) {
    if ((token < 0) || (token > yyName.Length)) return "[illegal]";
    string name;
    if ((name = yyName[token]) != null) return name;
    return "[unknown]";
  }

  /** computes list of expected tokens on error by tracing the tables.
      @param state for which to compute the list.
      @return list of token names.
    */
  protected string[] yyExpecting (int state) {
    int token, n, len = 0;
    bool[] ok = new bool[yyName.Length];

    if ((n = yySindex[state]) != 0)
      for (token = n < 0 ? -n : 0;
           (token < yyName.Length) && (n+token < yyTable.Length); ++ token)
        if (yyCheck[n+token] == token && !ok[token] && yyName[token] != null) {
          ++ len;
          ok[token] = true;
        }
    if ((n = yyRindex[state]) != 0)
      for (token = n < 0 ? -n : 0;
           (token < yyName.Length) && (n+token < yyTable.Length); ++ token)
        if (yyCheck[n+token] == token && !ok[token] && yyName[token] != null) {
          ++ len;
          ok[token] = true;
        }

    string [] result = new string[len];
    for (n = token = 0; n < len;  ++ token)
      if (ok[token]) result[n++] = yyName[token];
    return result;
  }

  /** the generated parser, with debugging messages.
      Maintains a state and a value stack, currently with fixed maximum size.
      @param yyLex scanner.
      @param yydebug debug message writer implementing yyDebug, or null.
      @return result of the last reduction, if any.
      @throws yyException on irrecoverable parse error.
    */
  public Object yyparse (yyParser.yyInput yyLex, Object yyd)
				 {
//t    this.debug = (yydebug.yyDebug)yyd;
    return yyparse(yyLex);
  }

  /** initial size and increment of the state/value stack [default 256].
      This is not final so that it can be overwritten outside of invocations
      of yyparse().
    */
  protected int yyMax;

  /** executed at the beginning of a reduce action.
      Used as $$ = yyDefault($1), prior to the user-specified action, if any.
      Can be overwritten to provide deep copy, etc.
      @param first value for $1, or null.
      @return first.
    */
  protected Object yyDefault (Object first) {
    return first;
  }

  /** the generated parser.
      Maintains a state and a value stack, currently with fixed maximum size.
      @param yyLex scanner.
      @return result of the last reduction, if any.
      @throws yyException on irrecoverable parse error.
    */
  public Object yyparse (yyParser.yyInput yyLex)
				{
    if (yyMax <= 0) yyMax = 256;			// initial size
    int yyState = 0;                                   // state stack ptr
    int [] yyStates = new int[yyMax];	                // state stack 
    Object yyVal = null;                               // value stack ptr
    Object [] yyVals = new Object[yyMax];	        // value stack
    int yyToken = -1;					// current input
    int yyErrorFlag = 0;				// #tks to shift

    int yyTop = 0;
    goto skip;
    yyLoop:
    yyTop++;
    skip:
    for (;; ++ yyTop) {
      if (yyTop >= yyStates.Length) {			// dynamically increase
        int[] i = new int[yyStates.Length+yyMax];
        yyStates.CopyTo (i, 0);
        yyStates = i;
        Object[] o = new Object[yyVals.Length+yyMax];
        yyVals.CopyTo (o, 0);
        yyVals = o;
      }
      yyStates[yyTop] = yyState;
      yyVals[yyTop] = yyVal;
//t      if (debug != null) debug.push(yyState, yyVal);

      yyDiscarded: for (;;) {	// discarding a token does not change stack
        int yyN;
        if ((yyN = yyDefRed[yyState]) == 0) {	// else [default] reduce (yyN)
          if (yyToken < 0) {
            yyToken = yyLex.advance() ? yyLex.token() : 0;
//t            if (debug != null)
//t              debug.lex(yyState, yyToken, yyname(yyToken), yyLex.value());
          }
          if ((yyN = yySindex[yyState]) != 0 && ((yyN += yyToken) >= 0)
              && (yyN < yyTable.Length) && (yyCheck[yyN] == yyToken)) {
//t            if (debug != null)
//t              debug.shift(yyState, yyTable[yyN], yyErrorFlag-1);
            yyState = yyTable[yyN];		// shift to yyN
            yyVal = yyLex.value();
            yyToken = -1;
            if (yyErrorFlag > 0) -- yyErrorFlag;
            goto yyLoop;
          }
          if ((yyN = yyRindex[yyState]) != 0 && (yyN += yyToken) >= 0
              && yyN < yyTable.Length && yyCheck[yyN] == yyToken)
            yyN = yyTable[yyN];			// reduce (yyN)
          else
            switch (yyErrorFlag) {
  
            case 0:
              yyerror("syntax error", yyExpecting(yyState));
//t              if (debug != null) debug.error("syntax error");
              goto case 1;
            case 1: case 2:
              yyErrorFlag = 3;
              do {
                if ((yyN = yySindex[yyStates[yyTop]]) != 0
                    && (yyN += Token.yyErrorCode) >= 0 && yyN < yyTable.Length
                    && yyCheck[yyN] == Token.yyErrorCode) {
//t                  if (debug != null)
//t                    debug.shift(yyStates[yyTop], yyTable[yyN], 3);
                  yyState = yyTable[yyN];
                  yyVal = yyLex.value();
                  goto yyLoop;
                }
//t                if (debug != null) debug.pop(yyStates[yyTop]);
              } while (-- yyTop >= 0);
//t              if (debug != null) debug.reject();
              throw new yyParser.yyException("irrecoverable syntax error");
  
            case 3:
              if (yyToken == 0) {
//t                if (debug != null) debug.reject();
                throw new yyParser.yyException("irrecoverable syntax error at end-of-file");
              }
//t              if (debug != null)
//t                debug.discard(yyState, yyToken, yyname(yyToken),
//t  							yyLex.value());
              yyToken = -1;
              goto yyDiscarded;		// leave stack alone
            }
        }
        int yyV = yyTop + 1-yyLen[yyN];
//t        if (debug != null)
//t          debug.reduce(yyState, yyStates[yyV-1], yyN, yyRule[yyN], yyLen[yyN]);
        yyVal = yyDefault(yyV > yyTop ? null : yyVals[yyV]);
        switch (yyN) {
case 1:
#line 165 "DTD.y"
  {
      yyVal = Lisp.Cons(yyVals[0+yyTop]);
  }
  break;
case 2:
#line 169 "DTD.y"
  {
      yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(yyVals[0+yyTop]));
  }
  break;
case 3:
#line 173 "DTD.y"
  {
      yyVal = null;
  }
  break;
case 4:
#line 177 "DTD.y"
  {
      yyVal = yyVals[-3+yyTop];
  }
  break;
case 5:
#line 181 "DTD.y"
  {
      yyVal = yyVals[-1+yyTop];
  }
  break;
case 6:
#line 185 "DTD.y"
  {
      yyVal = Lisp.Append(yyVals[-5+yyTop], yyVals[-3+yyTop]);
  }
  break;
case 13:
#line 196 "DTD.y"
  {
      yyVal = Lisp.List(DTDID.ElementDecl, yyVals[-2+yyTop], yyVals[-1+yyTop]);
   }
  break;
case 14:
#line 203 "DTD.y"
  {
      yyVal = Lisp.List(DTDID.AttlistDecl, yyVals[-2+yyTop], yyVals[-1+yyTop]);
   }
  break;
case 17:
#line 215 "DTD.y"
  {
      yyVal = Lisp.List(DTDID.GEDecl, yyVals[-2+yyTop], yyVals[-1+yyTop]);
   }
  break;
case 18:
#line 222 "DTD.y"
  {
      yyVal = Lisp.List(DTDID.PEDecl, yyVals[-2+yyTop], yyVals[-1+yyTop]);
      if (Lisp.IsFunctor(yyVals[-1+yyTop], DTDID.PUBLIC))
        _tokenizer.AddPE((string)yyVals[-2+yyTop], Lisp.SArg1(yyVals[-1+yyTop]), Lisp.SArg2(yyVals[-1+yyTop]));
      else if (Lisp.IsFunctor(yyVals[-1+yyTop], DTDID.SYSTEM))
        _tokenizer.AddPE((string)yyVals[-2+yyTop], null, Lisp.SArg1(yyVals[-1+yyTop]));
      else
        _tokenizer.AddPE((string)yyVals[-2+yyTop], (string)yyVals[-1+yyTop]);       
   }
  break;
case 19:
#line 235 "DTD.y"
  {
      yyVal = Lisp.List(DTDID.NotationDecl, yyVals[-2+yyTop], yyVals[-1+yyTop]);
   }
  break;
case 20:
#line 239 "DTD.y"
  {
      yyVal = Lisp.List(DTDID.NotationDecl, yyVals[-2+yyTop], yyVals[-1+yyTop]);
   }
  break;
case 21:
#line 246 "DTD.y"
  {
      yyVal = DTDID.EMPTY;
   }
  break;
case 22:
#line 250 "DTD.y"
  {
      yyVal = DTDID.ANY;
   }
  break;
case 23:
#line 254 "DTD.y"
  {
      yyVal = Lisp.List(DTDID.MixedContent, yyVals[0+yyTop]);    
   }
  break;
case 25:
#line 262 "DTD.y"
  {
      yyVal = Lisp.Cons(DTDID.PCDATA);
   }
  break;
case 26:
#line 266 "DTD.y"
  {
      yyVal = Lisp.Append(Lisp.Cons(DTDID.PCDATA), yyVals[-2+yyTop]);
   }
  break;
case 28:
#line 274 "DTD.y"
  {
      yyVal = Lisp.List(yyVals[0+yyTop], yyVals[-1+yyTop]);
   }
  break;
case 30:
#line 279 "DTD.y"
  {
      yyVal = Lisp.List(yyVals[0+yyTop], yyVals[-1+yyTop]);
   }
  break;
case 37:
#line 295 "DTD.y"
  {
     yyVal = Lisp.List(DTDID.Choice, Lisp.Append(Lisp.Cons(yyVals[-3+yyTop]), yyVals[-1+yyTop]));  
  }
  break;
case 38:
#line 302 "DTD.y"
  {
     yyVal = Lisp.List(DTDID.Seq, Lisp.Cons(yyVals[-1+yyTop]));  
  }
  break;
case 39:
#line 306 "DTD.y"
  {
     yyVal = Lisp.List(DTDID.Seq, Lisp.Append(Lisp.Cons(yyVals[-3+yyTop]), yyVals[-1+yyTop]));  
  }
  break;
case 40:
#line 313 "DTD.y"
  {
      yyVal = Lisp.Cons(yyVals[0+yyTop]);
  }
  break;
case 41:
#line 317 "DTD.y"
  {
      yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
  }
  break;
case 42:
#line 324 "DTD.y"
  {
      yyVal = Lisp.Cons(yyVals[0+yyTop]);
  }
  break;
case 43:
#line 328 "DTD.y"
  {
      yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
  }
  break;
case 44:
#line 335 "DTD.y"
  {
      yyVal = Lisp.Cons(yyVals[0+yyTop]);
   }
  break;
case 45:
#line 339 "DTD.y"
  {
      yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
   }
  break;
case 46:
#line 346 "DTD.y"
  {
       yyVal = DTDID.ZeroOrOne;
   }
  break;
case 47:
#line 350 "DTD.y"
  {
       yyVal = DTDID.ZeroOrMore;
   }
  break;
case 48:
#line 354 "DTD.y"
  {
       yyVal = DTDID.OneOrMore;
   }
  break;
case 49:
#line 361 "DTD.y"
  {   
      yyVal = Lisp.Cons(yyVals[0+yyTop]);
   }
  break;
case 50:
#line 365 "DTD.y"
  {
      yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(yyVals[0+yyTop]));
   }
  break;
case 51:
#line 372 "DTD.y"
  {
       yyVal = Lisp.List(DTDID.AttDef, yyVals[-2+yyTop], yyVals[-1+yyTop], yyVals[0+yyTop]);
   }
  break;
case 55:
#line 383 "DTD.y"
  {
       yyVal = DTDID.CDATA;
   }
  break;
case 56:
#line 390 "DTD.y"
  {
        yyVal = DTDID.ID;
   }
  break;
case 57:
#line 394 "DTD.y"
  {
        yyVal = DTDID.IDREF;
   }
  break;
case 58:
#line 398 "DTD.y"
  {
        yyVal = DTDID.IDREFS;
   }
  break;
case 59:
#line 402 "DTD.y"
  {
        yyVal = DTDID.ENTITY;
   }
  break;
case 60:
#line 406 "DTD.y"
  {
        yyVal = DTDID.ENTITIES;
   }
  break;
case 61:
#line 410 "DTD.y"
  {
        yyVal = DTDID.NMTOKEN;
   }
  break;
case 62:
#line 414 "DTD.y"
  {
        yyVal = DTDID.NMTOKENS;
   }
  break;
case 65:
#line 426 "DTD.y"
  {
       yyVal = Lisp.List(DTDID.Notation, yyVals[-1+yyTop]);
   }
  break;
case 66:
#line 430 "DTD.y"
  {
       yyVal = Lisp.List(DTDID.Notation, Lisp.Append(Lisp.Cons(yyVals[-3+yyTop]), yyVals[-1+yyTop]));
   }
  break;
case 67:
#line 437 "DTD.y"
  {
      yyVal = Lisp.Cons(yyVals[-1+yyTop]);
   }
  break;
case 68:
#line 441 "DTD.y"
  {
      yyVal = Lisp.Append(Lisp.Cons(yyVals[-3+yyTop]), yyVals[-1+yyTop]);
   }
  break;
case 69:
#line 448 "DTD.y"
  {
      yyVal = DTDID.REQUIRED;
   }
  break;
case 70:
#line 452 "DTD.y"
  {
      yyVal = DTDID.IMPLIED;
   }
  break;
case 71:
#line 456 "DTD.y"
  {
      yyVal = Lisp.List(DTDID.FIXED, yyVals[0+yyTop]);
   }
  break;
case 75:
#line 466 "DTD.y"
  {
      yyVal = Lisp.List(DTDID.NData, yyVals[-1+yyTop], yyVals[0+yyTop]);
   }
  break;
case 78:
#line 478 "DTD.y"
  {
      yyVal = Lisp.List(DTDID.SYSTEM, yyVals[0+yyTop]);
   }
  break;
case 79:
#line 482 "DTD.y"
  {
      yyVal = Lisp.List(DTDID.PUBLIC, yyVals[-1+yyTop], yyVals[0+yyTop]);
   }
  break;
case 80:
#line 489 "DTD.y"
  {
      yyVal = Lisp.List(DTDID.PUBLIC, yyVals[0+yyTop]);
   }
  break;
case 81:
#line 496 "DTD.y"
  {
      yyVal = yyVals[0+yyTop];
   }
  break;
#line default
        }
        yyTop -= yyLen[yyN];
        yyState = yyStates[yyTop];
        int yyM = yyLhs[yyN];
        if (yyState == 0 && yyM == 0) {
//t          if (debug != null) debug.shift(0, yyFinal);
          yyState = yyFinal;
          if (yyToken < 0) {
            yyToken = yyLex.advance() ? yyLex.token() : 0;
//t            if (debug != null)
//t               debug.lex(yyState, yyToken,yyname(yyToken), yyLex.value());
          }
          if (yyToken == 0) {
//t            if (debug != null) debug.accept(yyVal);
            return yyVal;
          }
          goto yyLoop;
        }
        if (((yyN = yyGindex[yyM]) != 0) && ((yyN += yyState) >= 0)
            && (yyN < yyTable.Length) && (yyCheck[yyN] == yyState))
          yyState = yyTable[yyN];
        else
          yyState = yyDgoto[yyM];
//t        if (debug != null) debug.shift(yyStates[yyTop], yyState);
	 goto yyLoop;
      }
    }
  }

   static  short [] yyLhs  = {              -1,
    0,    0,    0,    0,    0,    0,    1,    1,    1,    1,
    1,    1,    2,    3,    4,    4,    8,    9,    5,    5,
    6,    6,    6,    6,   14,   14,   15,   15,   15,   15,
   20,   20,   20,   20,   20,   20,   17,   19,   19,   21,
   21,   22,   22,   16,   16,   18,   18,   18,    7,    7,
   23,   24,   24,   24,   26,   27,   27,   27,   27,   27,
   27,   27,   28,   28,   29,   29,   30,   30,   25,   25,
   25,   25,   10,   10,   10,   11,   11,   12,   12,   13,
   33,   31,   32,   35,   34,
  };
   static  short [] yyLen = {           2,
    1,    2,    1,    5,    2,    6,    1,    1,    1,    1,
    1,    1,    4,    4,    1,    1,    4,    5,    4,    4,
    1,    1,    1,    1,    3,    6,    1,    2,    1,    2,
    1,    2,    1,    2,    1,    2,    5,    3,    5,    1,
    3,    1,    3,    1,    3,    1,    1,    1,    1,    2,
    3,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    4,    6,    3,    5,    1,    1,
    2,    1,    1,    1,    2,    1,    1,    2,    3,    2,
    2,    1,    1,    1,    1,
  };
   static  short [] yyDefRed = {            0,
    0,    0,    0,    0,   11,   12,    0,    3,    0,    1,
    7,    8,    9,   10,   15,   16,    0,    0,    0,    0,
    0,    0,    0,    5,    2,   21,   22,    0,    0,   23,
   24,    0,    0,    0,    0,   49,   83,    0,    0,    0,
    0,   73,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   13,   47,   46,   48,   28,   30,
   55,   56,   57,   58,   59,   60,   61,   62,    0,    0,
    0,   52,   53,   54,   63,   64,   14,   50,   85,   78,
   84,    0,   17,    0,   75,    0,   77,   76,    0,   19,
   20,    0,    0,   32,   25,    0,   34,   36,   38,    0,
    0,    0,    0,   82,   69,   70,    0,   51,   72,   79,
   81,   18,    4,    0,   44,    0,   40,    0,   42,    0,
    0,   67,    0,   71,    6,    0,    0,   37,    0,   39,
    0,   65,    0,    0,   26,   45,   41,   43,    0,   68,
   66,
  };
  protected static  short [] yyDgoto  = {             9,
   10,   11,   12,   13,   14,   29,   35,   15,   16,   40,
   86,   41,   46,   30,   31,  116,   52,   59,   53,   54,
  118,  120,   36,   71,  108,   72,   73,   74,   75,   76,
  109,   42,   85,  110,   82,
  };
  protected static  short [] yySindex = {         -230,
 -251, -200,  -35, -190,    0,    0, -230,    0, -224,    0,
    0,    0,    0,    0,    0,    0,  -28, -189, -233, -184,
 -238,  -88, -230,    0,    0,    0,    0,  -40,   14,    0,
    0,    7,    7,  -39,  -58,    0,    0, -214, -181,   16,
 -197,    0, -233, -181,   18,   19,  -11,  -82,    7,  -33,
  -37,    7,    7,  -34,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   43, -173,
 -213,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0, -214,    0, -172,    0,   24,    0,    0, -214,    0,
    0,   25,   -5,    0,    0, -168,    0,    0,    0,  -37,
  -37, -164,  -32,    0,    0,    0, -163,    0,    0,    0,
    0,    0,    0,   32,    0,  -21,    0,  -20,    0,   28,
  -19,    0, -168,    0,    0,   54, -157,    0,  -37,    0,
  -37,    0, -168,  -18,    0,    0,    0,    0,  -17,    0,
    0,
  };
  protected static  short [] yyRindex = {            0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   40,   46,    0,    0,    0,    0,    0,    0,    0,
   47,    0,    0,    0,    0,    0,    0,    0,  -27,    0,
    0,  -26,  -25,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   48,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,
  };
  protected static  short [] yyGindex = {           39,
    4,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   20,    0,    0,    0,  -72,   84,   22,   94,  -73,
    0,    0,   77,    0,    0,    0,    0,    0,    0,    0,
    6,   71,    0,   78,   73,
  };
  protected static  short [] yyTable = {            51,
   70,   20,   51,   77,   47,   17,   99,   95,  122,  101,
   93,   28,   25,   31,   33,   35,   31,   33,   35,  126,
  128,  132,  140,  141,   37,   25,  117,  119,    1,    2,
    3,    4,    5,    6,    1,    2,    3,    4,    5,    6,
   45,   38,   44,   79,  104,   22,   38,   39,   56,   58,
  134,   25,    7,    8,   60,  137,   18,  138,   23,   24,
  139,   48,   87,  105,  106,  107,   21,   34,  130,   57,
   94,  131,   43,   97,   98,   55,   81,   83,   84,   90,
   91,   92,  102,  103,  111,  112,  113,  114,  115,  100,
   96,  123,  121,  125,  104,  135,   31,   33,   35,  136,
   32,   27,  127,  129,  133,  127,  127,   29,   74,   80,
   33,   78,  124,   88,    0,   80,   89,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    1,    2,    3,    4,    5,    6,    1,    2,    3,    4,
    5,    6,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   23,   24,    0,    0,   34,    0,
   23,   24,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,   49,    0,    0,   49,
    0,   19,    0,    0,    0,    0,    0,   61,   62,   63,
   64,   65,   66,   67,   68,   69,   26,   27,    0,    0,
    0,   50,
  };
  protected static  short [] yyCheck = {            40,
   40,   37,   40,   62,   93,  257,   41,   41,   41,   44,
   93,   40,    9,   41,   41,   41,   44,   44,   44,   41,
   41,   41,   41,   41,  258,   22,  100,  101,  259,  260,
  261,  262,  263,  264,  259,  260,  261,  262,  263,  264,
   21,  280,  281,  258,  258,    7,  280,  281,   42,   43,
  123,   48,  283,  284,   33,  129,  257,  131,  283,  284,
  133,   23,   43,  277,  278,  279,  257,  257,   41,   63,
   49,   44,  257,   52,   53,   62,  258,   62,  276,   62,
   62,   93,   40,  257,  257,   62,   62,   93,  257,  124,
  124,  124,  257,   62,  258,   42,  124,  124,  124,  257,
   17,   62,  124,  124,  124,  124,  124,   62,   62,   62,
   17,   35,  107,   43,   -1,   38,   44,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  259,  260,  261,  262,  263,  264,  259,  260,  261,  262,
  263,  264,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  283,  284,   -1,   -1,  257,   -1,
  283,  284,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  257,   -1,   -1,  257,
   -1,  257,   -1,   -1,   -1,   -1,   -1,  267,  268,  269,
  270,  271,  272,  273,  274,  275,  265,  266,   -1,   -1,
   -1,  282,
  };

#line 518 "DTD.y"
}  
#line default
namespace yydebug {
        using System;
	 public interface yyDebug {
		 void push (int state, Object value);
		 void lex (int state, int token, string name, Object value);
		 void shift (int from, int to, int errorFlag);
		 void pop (int state);
		 void discard (int state, int token, string name, Object value);
		 void reduce (int from, int to, int rule, string text, int len);
		 void shift (int from, int to);
		 void accept (Object value);
		 void error (string message);
		 void reject ();
	 }
	 
	 class yyDebugSimple : yyDebug {
		 void println (string s){
			 Console.WriteLine (s);
		 }
		 
		 public void push (int state, Object value) {
			 println ("push\tstate "+state+"\tvalue "+value);
		 }
		 
		 public void lex (int state, int token, string name, Object value) {
			 println("lex\tstate "+state+"\treading "+name+"\tvalue "+value);
		 }
		 
		 public void shift (int from, int to, int errorFlag) {
			 switch (errorFlag) {
			 default:				// normally
				 println("shift\tfrom state "+from+" to "+to);
				 break;
			 case 0: case 1: case 2:		// in error recovery
				 println("shift\tfrom state "+from+" to "+to
					     +"\t"+errorFlag+" left to recover");
				 break;
			 case 3:				// normally
				 println("shift\tfrom state "+from+" to "+to+"\ton error");
				 break;
			 }
		 }
		 
		 public void pop (int state) {
			 println("pop\tstate "+state+"\ton error");
		 }
		 
		 public void discard (int state, int token, string name, Object value) {
			 println("discard\tstate "+state+"\ttoken "+name+"\tvalue "+value);
		 }
		 
		 public void reduce (int from, int to, int rule, string text, int len) {
			 println("reduce\tstate "+from+"\tuncover "+to
				     +"\trule ("+rule+") "+text);
		 }
		 
		 public void shift (int from, int to) {
			 println("goto\tfrom state "+from+" to "+to);
		 }
		 
		 public void accept (Object value) {
			 println("accept\tvalue "+value);
		 }
		 
		 public void error (string message) {
			 println("error\t"+message);
		 }
		 
		 public void reject () {
			 println("reject");
		 }
		 
	 }
}
// %token constants
 public class Token {
  public const int Name = 257;
  public const int Literal = 258;
  public const int ELEMENT_DECL = 259;
  public const int ATTLIST_DECL = 260;
  public const int ENTITY_DECL = 261;
  public const int NOTATION_DECL = 262;
  public const int PI = 263;
  public const int Comment = 264;
  public const int EMPTY = 265;
  public const int ANY = 266;
  public const int CDATA = 267;
  public const int ID = 268;
  public const int IDREF = 269;
  public const int IDREFS = 270;
  public const int ENTITY = 271;
  public const int ENTITIES = 272;
  public const int NMTOKEN = 273;
  public const int NMTOKENS = 274;
  public const int NOTATION = 275;
  public const int NDATA = 276;
  public const int REQUIRED = 277;
  public const int IMPLIED = 278;
  public const int FIXED = 279;
  public const int SYSTEM = 280;
  public const int PUBLIC = 281;
  public const int PCDATA = 282;
  public const int INCLUDE = 283;
  public const int IGNORE = 284;
  public const int yyErrorCode = 256;
 }
 namespace yyParser {
  using System;
  /** thrown for irrecoverable syntax errors and stack overflow.
    */
  public class yyException : System.Exception {
    public yyException (string message) : base (message) {
    }
  }

  /** must be implemented by a scanner object to supply input to the parser.
    */
  public interface yyInput {
    /** move on to next token.
        @return false if positioned beyond tokens.
        @throws IOException on input error.
      */
    bool advance (); // throws java.io.IOException;
    /** classifies current token.
        Should not be called if advance() returned false.
        @return current %token or single character.
      */
    int token ();
    /** associated with current token.
        Should not be called if advance() returned false.
        @return value for token().
      */
    Object value ();
  }
 }
} // close outermost namespace, that MUST HAVE BEEN opened in the prolog
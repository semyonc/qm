
// created by jay 0.7 (c) 1998 Axel.Schreiner@informatik.uni-osnabrueck.de

#line 35 "XQuery.y"

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

  protected static  int yyFinal = 18;
//t  public static  string [] yyRule = {
//t    "$accept : module",
//t    "module : versionDecl MainModule",
//t    "module : MainModule",
//t    "module : versionDecl LibraryModule",
//t    "module : LibraryModule",
//t    "versionDecl : XQUERY_VERSION StringLiteral Separator",
//t    "versionDecl : XQUERY_VERSION StringLiteral ENCODING StringLiteral Separator",
//t    "MainModule : Prolog QueryBody",
//t    "LibraryModule : ModuleDecl Prolog",
//t    "ModuleDecl : MODULE_NAMESPACE NCName '=' URILiteral Separator",
//t    "Prolog :",
//t    "Prolog : decl_block1",
//t    "Prolog : decl_block2",
//t    "Prolog : decl_block1 decl_block2",
//t    "decl_block1 : decl1 Separator",
//t    "decl_block1 : decl_block1 decl1 Separator",
//t    "decl_block2 : decl2 Separator",
//t    "decl_block2 : decl_block2 decl2 Separator",
//t    "decl1 : Setter",
//t    "decl1 : Import",
//t    "decl1 : NamespaceDecl",
//t    "decl1 : DefaultNamespaceDecl",
//t    "decl2 : VarDecl",
//t    "decl2 : FunctionDecl",
//t    "decl2 : OptionDecl",
//t    "Setter : BoundarySpaceDecl",
//t    "Setter : DefaultCollationDecl",
//t    "Setter : BaseURIDecl",
//t    "Setter : ConstructionDecl",
//t    "Setter : OrderingModeDecl",
//t    "Setter : EmptyOrderDecl",
//t    "Setter : CopyNamespacesDecl",
//t    "Import : SchemaImport",
//t    "Import : ModuleImport",
//t    "Separator : ';'",
//t    "NamespaceDecl : DECLARE_NAMESPACE NCName '=' URILiteral",
//t    "BoundarySpaceDecl : DECLARE_BOUNDARY_SPACE PRESERVE",
//t    "BoundarySpaceDecl : DECLARE_BOUNDARY_SPACE STRIP",
//t    "DefaultNamespaceDecl : DECLARE_DEFAULT_ELEMENT NAMESPACE URILiteral",
//t    "DefaultNamespaceDecl : DECLARE_DEFAULT_FUNCTION NAMESPACE URILiteral",
//t    "OptionDecl : DECLARE_OPTION QName StringLiteral",
//t    "OrderingModeDecl : DECLARE_ORDERING ORDERED",
//t    "OrderingModeDecl : DECLARE_ORDERING UNORDERED",
//t    "EmptyOrderDecl : DECLARE_DEFAULT_ORDER EMPTY_GREATEST",
//t    "EmptyOrderDecl : DECLARE_DEFAULT_ORDER EMPTY_LEAST",
//t    "CopyNamespacesDecl : DECLARE_COPY_NAMESPACES PreserveMode ',' InheritMode",
//t    "PreserveMode : PRESERVE",
//t    "PreserveMode : NO_PRESERVE",
//t    "InheritMode : INHERIT",
//t    "InheritMode : NO_INHERIT",
//t    "DefaultCollationDecl : DECLARE_DEFAULT_COLLATION URILiteral",
//t    "BaseURIDecl : DECLARE_BASE_URI URILiteral",
//t    "SchemaImport : IMPORT_SCHEMA opt_SchemaPrefix URILiteral",
//t    "SchemaImport : IMPORT_SCHEMA opt_SchemaPrefix URILiteral AT URILiteralList",
//t    "opt_SchemaPrefix :",
//t    "opt_SchemaPrefix : SchemaPrefix",
//t    "URILiteralList : URILiteral",
//t    "URILiteralList : URILiteralList ',' URILiteral",
//t    "SchemaPrefix : NAMESPACE NCName '='",
//t    "SchemaPrefix : DEFAULT_ELEMENT NAMESPACE",
//t    "ModuleImport : IMPORT_MODULE URILiteral AT URILiteralList",
//t    "ModuleImport : IMPORT_MODULE NAMESPACE NCName '=' URILiteral AT URILiteralList",
//t    "VarDecl : DECLARE_VARIABLE '$' VarName opt_TypeDeclaration ':' '=' ExprSingle",
//t    "VarDecl : DECLARE_VARIABLE '$' VarName opt_TypeDeclaration EXTERNAL",
//t    "opt_TypeDeclaration :",
//t    "opt_TypeDeclaration : TypeDeclaration",
//t    "ConstructionDecl : DECLARE_CONSTRUCTION PRESERVE",
//t    "ConstructionDecl : DECLARE_CONSTRUCTION STRIP",
//t    "FunctionDecl : DECLARE_FUNCTION QName '(' opt_ParamList ')' FunctionBody",
//t    "FunctionDecl : DECLARE_FUNCTION QName '(' opt_ParamList ')' AS SequenceType FunctionBody",
//t    "FunctionBody : EnclosedExpr",
//t    "FunctionBody : EXTERNAL",
//t    "opt_ParamList :",
//t    "opt_ParamList : ParamList",
//t    "ParamList : Param",
//t    "ParamList : ParamList ',' Param",
//t    "Param : '$' VarName",
//t    "Param : '$' VarName TypeDeclaration",
//t    "EnclosedExpr : '{' Expr '}'",
//t    "QueryBody : Expr",
//t    "Expr : ExprSingle",
//t    "Expr : Expr ',' ExprSingle",
//t    "ExprSingle : FLWORExpr",
//t    "ExprSingle : QuantifiedExpr",
//t    "ExprSingle : TypeswitchExpr",
//t    "ExprSingle : IfExpr",
//t    "ExprSingle : OrExpr",
//t    "FLWORExpr : FLWORPrefix RETURN ExprSingle",
//t    "FLWORExpr : FLWORPrefix WhereClause RETURN ExprSingle",
//t    "FLWORExpr : FLWORPrefix OrderByClause RETURN ExprSingle",
//t    "FLWORExpr : FLWORPrefix WhereClause OrderByClause RETURN ExprSingle",
//t    "FLWORPrefix : ForClause",
//t    "FLWORPrefix : LetClause",
//t    "FLWORPrefix : FLWORPrefix ForClause",
//t    "FLWORPrefix : FLWORPrefix LetClause",
//t    "ForClause : FOR ForClauseBody",
//t    "ForClauseBody : ForClauseOperator",
//t    "ForClauseBody : ForClauseBody ',' ForClauseOperator",
//t    "ForClauseOperator : '$' VarName opt_TypeDeclaration opt_PositionVar IN ExprSingle",
//t    "opt_PositionVar :",
//t    "opt_PositionVar : PositionVar",
//t    "PositionVar : AT '$' VarName",
//t    "LetClause : LET LetClauseBody",
//t    "LetClauseBody : LetClauseOperator",
//t    "LetClauseBody : LetClauseBody ',' LetClauseOperator",
//t    "LetClauseOperator : '$' VarName opt_TypeDeclaration ':' '=' ExprSingle",
//t    "WhereClause : WHERE ExprSingle",
//t    "OrderByClause : ORDER_BY OrderSpecList",
//t    "OrderByClause : STABLE_ORDER_BY OrderSpecList",
//t    "OrderSpecList : OrderSpec",
//t    "OrderSpecList : OrderSpecList ',' OrderSpec",
//t    "OrderSpec : ExprSingle",
//t    "OrderSpec : ExprSingle OrderModifier",
//t    "OrderModifier : OrderDirection",
//t    "OrderModifier : OrderDirection EmptyOrderSpec",
//t    "OrderModifier : OrderDirection COLLATION URILiteral",
//t    "OrderModifier : OrderDirection EmptyOrderSpec COLLATION URILiteral",
//t    "OrderDirection : ASCENDING",
//t    "OrderDirection : DESCENDING",
//t    "EmptyOrderSpec : EMPTY_GREATEST",
//t    "EmptyOrderSpec : EMPTY_LEAST",
//t    "QuantifiedExpr : SOME QuantifiedExprBody SATISFIES ExprSingle",
//t    "QuantifiedExpr : EVERY QuantifiedExprBody SATISFIES ExprSingle",
//t    "QuantifiedExprBody : QuantifiedExprOper",
//t    "QuantifiedExprBody : QuantifiedExprBody ',' QuantifiedExprOper",
//t    "QuantifiedExprOper : '$' VarName opt_TypeDeclaration IN ExprSingle",
//t    "TypeswitchExpr : TYPESWITCH '(' Expr ')' CaseClauseList DEFAULT RETURN ExprSingle",
//t    "TypeswitchExpr : TYPESWITCH '(' Expr ')' CaseClauseList DEFAULT '$' VarName RETURN ExprSingle",
//t    "CaseClauseList : CaseClause",
//t    "CaseClauseList : CaseClauseList CaseClause",
//t    "CaseClause : CASE '$' VarName AS SequenceType RETURN ExprSingle",
//t    "CaseClause : CASE SequenceType RETURN ExprSingle",
//t    "IfExpr : IF '(' Expr ')' THEN ExprSingle ELSE ExprSingle",
//t    "OrExpr : AndExpr",
//t    "OrExpr : OrExpr OR AndExpr",
//t    "AndExpr : ComparisonExpr",
//t    "AndExpr : AndExpr AND ComparisonExpr",
//t    "ComparisonExpr : RangeExpr",
//t    "ComparisonExpr : RangeExpr ValueComp RangeExpr",
//t    "ComparisonExpr : RangeExpr GeneralComp RangeExpr",
//t    "ComparisonExpr : RangeExpr NodeComp RangeExpr",
//t    "RangeExpr : AdditiveExpr",
//t    "RangeExpr : AdditiveExpr TO AdditiveExpr",
//t    "AdditiveExpr : MultiplicativeExpr",
//t    "AdditiveExpr : AdditiveExpr '+' MultiplicativeExpr",
//t    "AdditiveExpr : AdditiveExpr '-' MultiplicativeExpr",
//t    "MultiplicativeExpr : UnionExpr",
//t    "MultiplicativeExpr : MultiplicativeExpr ML UnionExpr",
//t    "MultiplicativeExpr : MultiplicativeExpr DIV UnionExpr",
//t    "MultiplicativeExpr : MultiplicativeExpr IDIV UnionExpr",
//t    "MultiplicativeExpr : MultiplicativeExpr MOD UnionExpr",
//t    "UnionExpr : IntersectExceptExpr",
//t    "UnionExpr : UnionExpr UNION IntersectExceptExpr",
//t    "UnionExpr : UnionExpr '|' IntersectExceptExpr",
//t    "IntersectExceptExpr : InstanceofExpr",
//t    "IntersectExceptExpr : IntersectExceptExpr INTERSECT InstanceofExpr",
//t    "IntersectExceptExpr : IntersectExceptExpr EXCEPT InstanceofExpr",
//t    "InstanceofExpr : TreatExpr",
//t    "InstanceofExpr : TreatExpr INSTANCE_OF SequenceType",
//t    "TreatExpr : CastableExpr",
//t    "TreatExpr : CastableExpr TREAT_AS SequenceType",
//t    "CastableExpr : CastExpr",
//t    "CastableExpr : CastExpr CASTABLE_AS SingleType",
//t    "CastExpr : UnaryExpr",
//t    "CastExpr : UnaryExpr CAST_AS SingleType",
//t    "UnaryExpr : UnaryOperator ValueExpr",
//t    "UnaryOperator :",
//t    "UnaryOperator : '+' UnaryOperator",
//t    "UnaryOperator : '-' UnaryOperator",
//t    "GeneralComp : '='",
//t    "GeneralComp : '!' '='",
//t    "GeneralComp : '<'",
//t    "GeneralComp : '<' '='",
//t    "GeneralComp : '>'",
//t    "GeneralComp : '>' '='",
//t    "ValueComp : EQ",
//t    "ValueComp : NE",
//t    "ValueComp : LT",
//t    "ValueComp : LE",
//t    "ValueComp : GT",
//t    "ValueComp : GE",
//t    "NodeComp : IS",
//t    "NodeComp : '<' '<'",
//t    "NodeComp : '>' '>'",
//t    "ValueExpr : ValidateExpr",
//t    "ValueExpr : PathExpr",
//t    "ValueExpr : ExtensionExpr",
//t    "ValidateExpr : VALIDATE '{' Expr '}'",
//t    "ValidateExpr : VALIDATE ValidationMode '{' Expr '}'",
//t    "ValidationMode : LAX",
//t    "ValidationMode : STRICT",
//t    "ExtensionExpr : PragmaList '{' Expr '}'",
//t    "PragmaList : Pragma",
//t    "PragmaList : PragmaList Pragma",
//t    "Pragma : PRAGMA_BEGIN opt_S QName PragmaContents PRAGMA_END",
//t    "PathExpr : '/'",
//t    "PathExpr : '/' RelativePathExpr",
//t    "PathExpr : DOUBLE_SLASH RelativePathExpr",
//t    "PathExpr : RelativePathExpr",
//t    "RelativePathExpr : StepExpr",
//t    "RelativePathExpr : RelativePathExpr '/' StepExpr",
//t    "RelativePathExpr : RelativePathExpr DOUBLE_SLASH StepExpr",
//t    "StepExpr : AxisStep",
//t    "StepExpr : FilterExpr",
//t    "AxisStep : ForwardStep",
//t    "AxisStep : ForwardStep PredicateList",
//t    "AxisStep : ReverseStep",
//t    "AxisStep : ReverseStep PredicateList",
//t    "ForwardStep : ForwardAxis NodeTest",
//t    "ForwardStep : AbbrevForwardStep",
//t    "ForwardAxis : AXIS_CHILD",
//t    "ForwardAxis : AXIS_DESCENDANT",
//t    "ForwardAxis : AXIS_ATTRIBUTE",
//t    "ForwardAxis : AXIS_SELF",
//t    "ForwardAxis : AXIS_DESCENDANT_OR_SELF",
//t    "ForwardAxis : AXIS_FOLLOWING_SIBLING",
//t    "ForwardAxis : AXIS_FOLLOWING",
//t    "ForwardAxis : AXIS_NAMESPACE",
//t    "AbbrevForwardStep : '@' NodeTest",
//t    "AbbrevForwardStep : NodeTest",
//t    "ReverseStep : ReverseAxis NodeTest",
//t    "ReverseStep : AbbrevReverseStep",
//t    "ReverseAxis : AXIS_PARENT",
//t    "ReverseAxis : AXIS_ANCESTOR",
//t    "ReverseAxis : AXIS_PRECEDING_SIBLING",
//t    "ReverseAxis : AXIS_PRECEDING",
//t    "ReverseAxis : AXIS_ANCESTOR_OR_SELF",
//t    "AbbrevReverseStep : DOUBLE_PERIOD",
//t    "NodeTest : KindTest",
//t    "NodeTest : NameTest",
//t    "NameTest : QName",
//t    "NameTest : Wildcard",
//t    "Wildcard : '*'",
//t    "Wildcard : NCName ':' '*'",
//t    "Wildcard : '*' ':' NCName",
//t    "FilterExpr : PrimaryExpr",
//t    "FilterExpr : PrimaryExpr PredicateList",
//t    "PredicateList : Predicate",
//t    "PredicateList : PredicateList Predicate",
//t    "Predicate : '[' Expr ']'",
//t    "PrimaryExpr : Literal",
//t    "PrimaryExpr : VarRef",
//t    "PrimaryExpr : ParenthesizedExpr",
//t    "PrimaryExpr : ContextItemExpr",
//t    "PrimaryExpr : FunctionCall",
//t    "PrimaryExpr : Constructor",
//t    "PrimaryExpr : OrderedExpr",
//t    "PrimaryExpr : UnorderedExpr",
//t    "Literal : NumericLiteral",
//t    "Literal : StringLiteral",
//t    "NumericLiteral : IntegerLiteral",
//t    "NumericLiteral : DecimalLiteral",
//t    "NumericLiteral : DoubleLiteral",
//t    "VarRef : '$' VarName",
//t    "ParenthesizedExpr : '(' ')'",
//t    "ParenthesizedExpr : '(' Expr ')'",
//t    "ContextItemExpr : '.'",
//t    "OrderedExpr : ORDERED '{' Expr '}'",
//t    "UnorderedExpr : UNORDERED '{' Expr '}'",
//t    "FunctionCall : QName '(' ')'",
//t    "FunctionCall : QName '(' Args ')'",
//t    "Args : ExprSingle",
//t    "Args : Args ',' ExprSingle",
//t    "Constructor : DirectConstructor",
//t    "Constructor : ComputedConstructor",
//t    "DirectConstructor : DirElemConstructor",
//t    "DirectConstructor : DirCommentConstructor",
//t    "DirectConstructor : DirPIConstructor",
//t    "DirElemConstructor : BeginTag QName opt_DirAttributeList '/' '>'",
//t    "DirElemConstructor : BeginTag QName opt_DirAttributeList '>' '<' '/' QName opt_S '>'",
//t    "DirElemConstructor : BeginTag QName opt_DirAttributeList '>' DirElemContentList '<' '/' QName opt_S '>'",
//t    "DirElemContentList : DirElemContent",
//t    "DirElemContentList : DirElemContentList DirElemContent",
//t    "opt_DirAttributeList :",
//t    "opt_DirAttributeList : DirAttributeList",
//t    "DirAttributeList : S DirAttribute",
//t    "DirAttributeList : DirAttributeList S",
//t    "DirAttributeList : DirAttributeList S DirAttribute",
//t    "DirAttribute : QName opt_S '=' opt_S '\"' DirAttributeValueQuot '\"'",
//t    "DirAttribute : QName opt_S '=' opt_S Apos DirAttributeValueApos Apos",
//t    "DirAttributeValueQuot : EscapeQuot",
//t    "DirAttributeValueQuot : QuotAttrValueContent",
//t    "DirAttributeValueQuot : DirAttributeValueQuot EscapeQuot",
//t    "DirAttributeValueQuot : DirAttributeValueQuot QuotAttrValueContent",
//t    "DirAttributeValueApos : EscapeApos",
//t    "DirAttributeValueApos : AposAttrValueContent",
//t    "DirAttributeValueApos : DirAttributeValueApos EscapeApos",
//t    "DirAttributeValueApos : DirAttributeValueApos AposAttrValueContent",
//t    "QuotAttrValueContent : QuotAttrContentChar",
//t    "QuotAttrValueContent : CommonContent",
//t    "AposAttrValueContent : AposAttrContentChar",
//t    "AposAttrValueContent : CommonContent",
//t    "DirElemContent : DirectConstructor",
//t    "DirElemContent : ElementContentChar",
//t    "DirElemContent : CDataSection",
//t    "DirElemContent : CommonContent",
//t    "CommonContent : PredefinedEntityRef",
//t    "CommonContent : CharRef",
//t    "CommonContent : '{' '{'",
//t    "CommonContent : '}' '}'",
//t    "CommonContent : EnclosedExpr",
//t    "DirCommentConstructor : COMMENT_BEGIN StringLiteral COMMENT_END",
//t    "DirPIConstructor : PI_BEGIN StringLiteral PI_END",
//t    "DirPIConstructor : PI_BEGIN StringLiteral S StringLiteral PI_END",
//t    "CDataSection : CDATA_BEGIN StringLiteral CDATA_END",
//t    "ComputedConstructor : CompDocConstructor",
//t    "ComputedConstructor : CompElemConstructor",
//t    "ComputedConstructor : CompAttrConstructor",
//t    "ComputedConstructor : CompTextConstructor",
//t    "ComputedConstructor : CompCommentConstructor",
//t    "ComputedConstructor : CompPIConstructor",
//t    "CompDocConstructor : DOCUMENT '{' Expr '}'",
//t    "CompElemConstructor : ELEMENT QName '{' ContentExpr '}'",
//t    "CompElemConstructor : ELEMENT QName '{' '}'",
//t    "CompElemConstructor : ELEMENT '{' Expr '}' '{' ContentExpr '}'",
//t    "CompElemConstructor : ELEMENT '{' Expr '}' '{' '}'",
//t    "ContentExpr : Expr",
//t    "CompAttrConstructor : ATTRIBUTE QName '{' Expr '}'",
//t    "CompAttrConstructor : ATTRIBUTE QName '{' '}'",
//t    "CompAttrConstructor : ATTRIBUTE '{' Expr '}' '{' Expr '}'",
//t    "CompAttrConstructor : ATTRIBUTE '{' Expr '}' '{' '}'",
//t    "CompTextConstructor : TEXT '{' Expr '}'",
//t    "CompCommentConstructor : COMMENT '{' Expr '}'",
//t    "CompPIConstructor : PROCESSING_INSTRUCTION NCName '{' Expr '}'",
//t    "CompPIConstructor : PROCESSING_INSTRUCTION NCName '{' '}'",
//t    "CompPIConstructor : PROCESSING_INSTRUCTION '{' Expr '}' '{' Expr '}'",
//t    "CompPIConstructor : PROCESSING_INSTRUCTION '{' Expr '}' '{' '}'",
//t    "SingleType : AtomicType",
//t    "SingleType : AtomicType '?'",
//t    "TypeDeclaration : AS SequenceType",
//t    "SequenceType : ItemType",
//t    "SequenceType : ItemType OccurrenceIndicator",
//t    "SequenceType : VOID",
//t    "OccurrenceIndicator : Indicator1",
//t    "OccurrenceIndicator : Indicator2",
//t    "OccurrenceIndicator : Indicator3",
//t    "ItemType : AtomicType",
//t    "ItemType : KindTest",
//t    "ItemType : ITEM",
//t    "AtomicType : QName",
//t    "KindTest : KindTestBody",
//t    "KindTestBody : DocumentTest",
//t    "KindTestBody : ElementTest",
//t    "KindTestBody : AttributeTest",
//t    "KindTestBody : SchemaElementTest",
//t    "KindTestBody : SchemaAttributeTest",
//t    "KindTestBody : PITest",
//t    "KindTestBody : CommentTest",
//t    "KindTestBody : TextTest",
//t    "KindTestBody : AnyKindTest",
//t    "AnyKindTest : NODE '(' ')'",
//t    "DocumentTest : DOCUMENT_NODE '(' ')'",
//t    "DocumentTest : DOCUMENT_NODE '(' ElementTest ')'",
//t    "DocumentTest : DOCUMENT_NODE '(' SchemaElementTest ')'",
//t    "TextTest : TEXT '(' ')'",
//t    "CommentTest : COMMENT '(' ')'",
//t    "PITest : PROCESSING_INSTRUCTION '(' ')'",
//t    "PITest : PROCESSING_INSTRUCTION '(' NCName ')'",
//t    "PITest : PROCESSING_INSTRUCTION '(' StringLiteral ')'",
//t    "ElementTest : ELEMENT '(' ')'",
//t    "ElementTest : ELEMENT '(' ElementNameOrWildcard ')'",
//t    "ElementTest : ELEMENT '(' ElementNameOrWildcard ',' TypeName ')'",
//t    "ElementTest : ELEMENT '(' ElementNameOrWildcard ',' TypeName '?' ')'",
//t    "ElementNameOrWildcard : ElementName",
//t    "ElementNameOrWildcard : '*'",
//t    "AttributeTest : ATTRIBUTE '(' ')'",
//t    "AttributeTest : ATTRIBUTE '(' AttributeOrWildcard ')'",
//t    "AttributeTest : ATTRIBUTE '(' AttributeOrWildcard ',' TypeName ')'",
//t    "AttributeOrWildcard : AttributeName",
//t    "AttributeOrWildcard : '*'",
//t    "SchemaElementTest : SCHEMA_ELEMENT '(' ElementName ')'",
//t    "SchemaAttributeTest : SCHEMA_ATTRIBUTE '(' AttributeName ')'",
//t    "AttributeName : QName",
//t    "ElementName : QName",
//t    "TypeName : QName",
//t    "opt_S :",
//t    "opt_S : S",
//t    "QuotAttrContentChar : Char",
//t    "AposAttrContentChar : Char",
//t    "ElementContentChar : Char",
//t    "URILiteral : StringLiteral",
//t  };
  protected static  string [] yyName = {    
    "end-of-file",null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,"'!'","'\"'",null,"'$'",null,null,
    null,"'('","')'","'*'","'+'","','","'-'","'.'","'/'",null,null,null,
    null,null,null,null,null,null,null,"':'","';'","'<'","'='","'>'",
    "'?'","'@'",null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,"'['",null,"']'",null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,"'{'","'|'","'}'",null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,"ENCODING","PRESERVE","NO_PRESERVE","STRIP","INHERIT",
    "NO_INHERIT","NAMESPACE","ORDERED","UNORDERED","EXTERNAL","AT","AS",
    "IN","RETURN","FOR","LET","WHERE","ASCENDING","DESCENDING",
    "COLLATION","SOME","EVERY","SATISFIES","TYPESWITCH","CASE","DEFAULT",
    "IF","THEN","ELSE","DOCUMENT","ELEMENT","ATTRIBUTE","TEXT","COMMENT",
    "AND","OR","TO","DIV","IDIV","MOD","UNION","INTERSECT","EXCEPT",
    "INSTANCE_OF","TREAT_AS","CASTABLE_AS","CAST_AS","EQ","NE","LT","GT",
    "GE","LE","IS","VALIDATE","LAX","STRICT","NODE","DOUBLE_PERIOD",
    "StringLiteral","IntegerLiteral","DecimalLiteral","DoubleLiteral",
    "NCName","QName","VarName","PragmaContents","S","Char",
    "PredefinedEntityRef","CharRef","XQUERY_VERSION","MODULE_NAMESPACE",
    "IMPORT_SCHEMA","IMPORT_MODULE","DECLARE_NAMESPACE",
    "DECLARE_BOUNDARY_SPACE","DECLARE_DEFAULT_ELEMENT",
    "DECLARE_DEFAULT_FUNCTION","DECLARE_DEFAULT_ORDER","DECLARE_OPTION",
    "DECLARE_ORDERING","DECLARE_COPY_NAMESPACES",
    "DECLARE_DEFAULT_COLLATION","DECLARE_BASE_URI","DECLARE_VARIABLE",
    "DECLARE_CONSTRUCTION","DECLARE_FUNCTION","EMPTY_GREATEST",
    "EMPTY_LEAST","DEFAULT_ELEMENT","ORDER_BY","STABLE_ORDER_BY",
    "PROCESSING_INSTRUCTION","DOCUMENT_NODE","SCHEMA_ELEMENT",
    "SCHEMA_ATTRIBUTE","DOUBLE_SLASH","COMMENT_BEGIN","COMMENT_END",
    "PI_BEGIN","PI_END","PRAGMA_BEGIN","PRAGMA_END","CDATA_BEGIN",
    "CDATA_END","VOID","ITEM","AXIS_CHILD","AXIS_DESCENDANT",
    "AXIS_ATTRIBUTE","AXIS_SELF","AXIS_DESCENDANT_OR_SELF",
    "AXIS_FOLLOWING_SIBLING","AXIS_FOLLOWING","AXIS_PARENT",
    "AXIS_ANCESTOR","AXIS_PRECEDING_SIBLING","AXIS_PRECEDING",
    "AXIS_ANCESTOR_OR_SELF","AXIS_NAMESPACE","ML","Apos","BeginTag",
    "Indicator1","Indicator2","Indicator3","EscapeQuot","EscapeApos",
    "XQComment","XQWhitespace",
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
#line 214 "XQuery.y"
  {
     notation.ConfirmTag(Tag.Module, Descriptor.Root, yyVals[0+yyTop]);
     yyVal = notation.ResolveTag(Tag.Module);	 
  }
  break;
case 2:
#line 219 "XQuery.y"
  {
	 notation.ConfirmTag(Tag.Module, Descriptor.Root, yyVals[0+yyTop]);	 
	 yyVal = notation.ResolveTag(Tag.Module);	 
  }
  break;
case 3:
#line 224 "XQuery.y"
  {
     notation.ConfirmTag(Tag.Module, Descriptor.Root, yyVals[0+yyTop]);	 
     yyVal = notation.ResolveTag(Tag.Module);	 
  }
  break;
case 4:
#line 229 "XQuery.y"
  {
	 notation.ConfirmTag(Tag.Module, Descriptor.Root, yyVals[0+yyTop]);	 
	 yyVal = notation.ResolveTag(Tag.Module);	 
  }
  break;
case 5:
#line 237 "XQuery.y"
  {
     notation.ConfirmTag(Tag.Module, Descriptor.Version, yyVals[-1+yyTop], null);
  }
  break;
case 6:
#line 241 "XQuery.y"
  {
     notation.ConfirmTag(Tag.Module, Descriptor.Version, yyVals[-3+yyTop], yyVals[-1+yyTop]);
  }
  break;
case 7:
#line 248 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.Query, yyVals[-1+yyTop], yyVals[0+yyTop]);
  }
  break;
case 8:
#line 255 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.Library, yyVals[-1+yyTop], yyVals[0+yyTop]);
  }
  break;
case 9:
#line 262 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.ModuleNamespace, yyVals[-3+yyTop], yyVals[-1+yyTop]);
  }
  break;
case 10:
#line 269 "XQuery.y"
  {
     yyVal = null;
  }
  break;
case 13:
#line 275 "XQuery.y"
  {
     yyVal = Lisp.Append(yyVals[-1+yyTop], yyVals[0+yyTop]);
  }
  break;
case 14:
#line 282 "XQuery.y"
  {
      yyVal = Lisp.Cons(yyVals[-1+yyTop]);
   }
  break;
case 15:
#line 286 "XQuery.y"
  {
      yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[-1+yyTop]));
   }
  break;
case 16:
#line 293 "XQuery.y"
  {
      yyVal = Lisp.Cons(yyVals[-1+yyTop]);
   }
  break;
case 17:
#line 297 "XQuery.y"
  {
      yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[-1+yyTop]));
   }
  break;
case 35:
#line 336 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.Namespace, yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 36:
#line 343 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Module), 
		Descriptor.BoundarySpace, new TokenWrapper(Token.PRESERVE));
  }
  break;
case 37:
#line 348 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Module), 
		Descriptor.BoundarySpace, new TokenWrapper(Token.STRIP));  
  }
  break;
case 38:
#line 356 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.DefaultElement, yyVals[0+yyTop]);
  }
  break;
case 39:
#line 360 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.DefaultFunction, yyVals[0+yyTop]);
  }
  break;
case 40:
#line 367 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.OptionDecl, yyVals[-1+yyTop], yyVals[0+yyTop]);
  }
  break;
case 41:
#line 374 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Module), 
		Descriptor.Ordering, new TokenWrapper(Token.ORDERED));  
  }
  break;
case 42:
#line 379 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Module), 
		Descriptor.Ordering, new TokenWrapper(Token.UNORDERED));  
  }
  break;
case 43:
#line 387 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Module), 
		Descriptor.DefaultOrder, new TokenWrapper(Token.EMPTY_GREATEST));  
  }
  break;
case 44:
#line 392 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Module), 
		Descriptor.DefaultOrder, new TokenWrapper(Token.EMPTY_LEAST));  
  }
  break;
case 45:
#line 400 "XQuery.y"
  {
	  yyVal = notation.Confirm(new Symbol(Tag.Module), 
	    Descriptor.CopyNamespace, yyVals[-3+yyTop], yyVals[-1+yyTop]); 
  }
  break;
case 46:
#line 408 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.PRESERVE);
  }
  break;
case 47:
#line 412 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.NO_PRESERVE);
  }
  break;
case 48:
#line 419 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.INHERIT);
  }
  break;
case 49:
#line 423 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.NO_INHERIT);
  }
  break;
case 50:
#line 430 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.DefaultCollation, yyVals[0+yyTop]);
  }
  break;
case 51:
#line 437 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.BaseUri, yyVals[0+yyTop]);
  }
  break;
case 52:
#line 444 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Module), 
         Descriptor.ImportSchema, yyVals[-1+yyTop], yyVals[0+yyTop], null);
  }
  break;
case 53:
#line 449 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Module), 
         Descriptor.ImportSchema, yyVals[-3+yyTop], yyVals[-2+yyTop], yyVals[0+yyTop]);  
  }
  break;
case 54:
#line 457 "XQuery.y"
  { 
     yyVal = null;
  }
  break;
case 56:
#line 465 "XQuery.y"
  {
     yyVal = Lisp.Cons(yyVals[0+yyTop]);
  }
  break;
case 57:
#line 469 "XQuery.y"
  {
     yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
  }
  break;
case 58:
#line 476 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.Namespace, yyVals[-1+yyTop]);
  }
  break;
case 59:
#line 480 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.DefaultElement);
  }
  break;
case 60:
#line 487 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.ImportModule, yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 61:
#line 491 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.ImportModule, yyVals[-4+yyTop], yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 62:
#line 498 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.VarDecl, yyVals[-4+yyTop], yyVals[-3+yyTop], yyVals[0+yyTop]); 
  }
  break;
case 63:
#line 502 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.VarDecl, yyVals[-2+yyTop], yyVals[-1+yyTop]); 
  }
  break;
case 64:
#line 509 "XQuery.y"
  {
     yyVal = null;
  }
  break;
case 66:
#line 517 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.ConstructionDecl, 
		new TokenWrapper(Token.PRESERVE));
  }
  break;
case 67:
#line 522 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.ConstructionDecl, 
		new TokenWrapper(Token.STRIP));
  }
  break;
case 68:
#line 530 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.DeclareFunction, yyVals[-4+yyTop], yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 69:
#line 534 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.DeclareFunction, yyVals[-6+yyTop], yyVals[-4+yyTop], yyVals[-1+yyTop], yyVals[0+yyTop]);
  }
  break;
case 71:
#line 542 "XQuery.y"
  {
     yyVal = null;
  }
  break;
case 72:
#line 549 "XQuery.y"
  {
     yyVal = null;
  }
  break;
case 74:
#line 557 "XQuery.y"
  {
     yyVal = Lisp.Cons(yyVals[0+yyTop]);
  }
  break;
case 75:
#line 561 "XQuery.y"
  {
     yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
  }
  break;
case 76:
#line 568 "XQuery.y"
  {
     yyVal = yyVals[0+yyTop];
  }
  break;
case 77:
#line 572 "XQuery.y"
  {
     yyVal = yyVals[-1+yyTop];
     notation.Confirm((Symbol)yyVals[-1+yyTop], Descriptor.TypeDecl, yyVals[0+yyTop]);
  }
  break;
case 78:
#line 580 "XQuery.y"
  {
     yyVal = yyVals[-1+yyTop];
  }
  break;
case 80:
#line 591 "XQuery.y"
  {
     yyVal = Lisp.Cons(yyVals[0+yyTop]);
  }
  break;
case 81:
#line 595 "XQuery.y"
  {
     yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
  }
  break;
case 87:
#line 610 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.FLWORExpr, yyVals[-2+yyTop], null, null, yyVals[0+yyTop]);
  }
  break;
case 88:
#line 614 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.FLWORExpr, yyVals[-3+yyTop], yyVals[-2+yyTop], null, yyVals[0+yyTop]);
  }
  break;
case 89:
#line 618 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.FLWORExpr, yyVals[-3+yyTop], null, yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 90:
#line 622 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.FLWORExpr, yyVals[-4+yyTop], yyVals[-3+yyTop], yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 91:
#line 629 "XQuery.y"
  {
     yyVal = Lisp.Cons(yyVals[0+yyTop]);
  }
  break;
case 92:
#line 633 "XQuery.y"
  {
     yyVal = Lisp.Cons(yyVals[0+yyTop]);
  }
  break;
case 93:
#line 637 "XQuery.y"
  {
     yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(yyVals[0+yyTop]));
  }
  break;
case 94:
#line 641 "XQuery.y"
  {
     yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(yyVals[0+yyTop]));
  }
  break;
case 95:
#line 648 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.For, yyVals[0+yyTop]);
  }
  break;
case 96:
#line 655 "XQuery.y"
  {
     yyVal = Lisp.Cons(yyVals[0+yyTop]);
  }
  break;
case 97:
#line 659 "XQuery.y"
  {
     yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
  }
  break;
case 98:
#line 666 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.ForClauseOperator, yyVals[-4+yyTop], yyVals[-3+yyTop], yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 99:
#line 673 "XQuery.y"
  {
     yyVal = null;
  }
  break;
case 101:
#line 681 "XQuery.y"
  {
     yyVal = yyVals[0+yyTop];
  }
  break;
case 102:
#line 688 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Let, yyVals[0+yyTop]);
  }
  break;
case 103:
#line 695 "XQuery.y"
  {
     yyVal = Lisp.Cons(yyVals[0+yyTop]);
  }
  break;
case 104:
#line 699 "XQuery.y"
  {
     yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
  }
  break;
case 105:
#line 706 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.LetClauseOperator, yyVals[-4+yyTop], yyVals[-3+yyTop], yyVals[0+yyTop]);
  }
  break;
case 106:
#line 713 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Where, yyVals[0+yyTop]);
  }
  break;
case 107:
#line 720 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.OrderBy, yyVals[0+yyTop]);
  }
  break;
case 108:
#line 724 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.StableOrderBy, yyVals[0+yyTop]);
  }
  break;
case 109:
#line 731 "XQuery.y"
  {
     yyVal = Lisp.Cons(yyVals[0+yyTop]);
  }
  break;
case 110:
#line 735 "XQuery.y"
  {
     yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
  }
  break;
case 112:
#line 743 "XQuery.y"
  {
     yyVal = yyVals[-1+yyTop];
     notation.Confirm((Symbol)yyVals[-1+yyTop], Descriptor.Modifier, yyVals[0+yyTop]);
  }
  break;
case 113:
#line 751 "XQuery.y"
  {
     yyVal = Lisp.List(yyVals[0+yyTop], null, null);
  }
  break;
case 114:
#line 755 "XQuery.y"
  {
     yyVal = Lisp.List(yyVals[-1+yyTop], yyVals[0+yyTop], null);
  }
  break;
case 115:
#line 759 "XQuery.y"
  {
     yyVal = Lisp.List(yyVals[-2+yyTop], null, yyVals[0+yyTop]);
  }
  break;
case 116:
#line 763 "XQuery.y"
  {
     yyVal = Lisp.List(yyVals[-3+yyTop], yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 117:
#line 770 "XQuery.y"
  {
     yyVal = new TokenWrapper(Token.ASCENDING);
  }
  break;
case 118:
#line 774 "XQuery.y"
  {
     yyVal = new TokenWrapper(Token.DESCENDING);
  }
  break;
case 119:
#line 781 "XQuery.y"
  {
     yyVal = new TokenWrapper(Token.EMPTY_GREATEST); 
  }
  break;
case 120:
#line 785 "XQuery.y"
  {
     yyVal = new TokenWrapper(Token.EMPTY_LEAST); 
  }
  break;
case 121:
#line 792 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Some, yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 122:
#line 796 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Every, yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 123:
#line 803 "XQuery.y"
  {
     yyVal = Lisp.Cons(yyVals[0+yyTop]);
  }
  break;
case 124:
#line 807 "XQuery.y"
  {
     yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
  }
  break;
case 125:
#line 814 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.QuantifiedExprOper, yyVals[-3+yyTop], yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 126:
#line 821 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Typeswitch, yyVals[-5+yyTop], yyVals[-3+yyTop], yyVals[0+yyTop]); 
  }
  break;
case 127:
#line 825 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Typeswitch, yyVals[-7+yyTop], yyVals[-5+yyTop], yyVals[-2+yyTop], yyVals[0+yyTop]); 
  }
  break;
case 128:
#line 832 "XQuery.y"
  {
     yyVal = Lisp.Cons(yyVals[0+yyTop]);
  }
  break;
case 129:
#line 836 "XQuery.y"
  {
     yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(yyVals[0+yyTop]));
  }
  break;
case 130:
#line 843 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Case, yyVals[-4+yyTop], yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 131:
#line 847 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Case, yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 132:
#line 854 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.If, yyVals[-5+yyTop], yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 134:
#line 862 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Or, yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 136:
#line 870 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.And, yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 138:
#line 878 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), 
		Descriptor.ValueComp, yyVals[-2+yyTop], yyVals[-1+yyTop], yyVals[0+yyTop]);
  }
  break;
case 139:
#line 883 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), 
		Descriptor.GeneralComp, yyVals[-2+yyTop], yyVals[-1+yyTop], yyVals[0+yyTop]);
  }
  break;
case 140:
#line 888 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), 
		Descriptor.NodeComp, yyVals[-2+yyTop], yyVals[-1+yyTop], yyVals[0+yyTop]);
  }
  break;
case 142:
#line 897 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.Range, yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 144:
#line 906 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.Add, yyVals[-2+yyTop], new TokenWrapper('+'), yyVals[0+yyTop]);
  }
  break;
case 145:
#line 911 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.Add, yyVals[-2+yyTop], new TokenWrapper('-'), yyVals[0+yyTop]);
  }
  break;
case 147:
#line 920 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.Mul, yyVals[-2+yyTop], new TokenWrapper(Token.ML), yyVals[0+yyTop]);
  }
  break;
case 148:
#line 925 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.Mul, yyVals[-2+yyTop], new TokenWrapper(Token.DIV), yyVals[0+yyTop]);
  }
  break;
case 149:
#line 930 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.Mul, yyVals[-2+yyTop], new TokenWrapper(Token.IDIV), yyVals[0+yyTop]);
  }
  break;
case 150:
#line 935 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.Mul, yyVals[-2+yyTop], new TokenWrapper(Token.MOD), yyVals[0+yyTop]);
  }
  break;
case 152:
#line 944 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.Union, yyVals[-2+yyTop], yyVals[0+yyTop]);  
  }
  break;
case 153:
#line 949 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.Concatenate, yyVals[-2+yyTop], yyVals[0+yyTop]);  
  }
  break;
case 155:
#line 958 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.IntersectExcept, yyVals[-2+yyTop], new TokenWrapper(Token.INTERSECT), yyVals[0+yyTop]);  
  }
  break;
case 156:
#line 963 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.IntersectExcept, yyVals[-2+yyTop], new TokenWrapper(Token.EXCEPT), yyVals[0+yyTop]);  
  }
  break;
case 158:
#line 972 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.InstanceOf, yyVals[-2+yyTop], yyVals[0+yyTop]);    
  }
  break;
case 160:
#line 980 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.TreatAs, yyVals[-2+yyTop], yyVals[0+yyTop]);    
  }
  break;
case 162:
#line 988 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.CastableAs, yyVals[-2+yyTop], yyVals[0+yyTop]);    
  }
  break;
case 164:
#line 996 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.CastAs, yyVals[-2+yyTop], yyVals[0+yyTop]);    
  }
  break;
case 165:
#line 1003 "XQuery.y"
  {
     if (yyVals[-1+yyTop] == null)
       yyVal = yyVals[0+yyTop];
     else
       yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Unary, yyVals[-1+yyTop], yyVals[0+yyTop]);
  }
  break;
case 166:
#line 1013 "XQuery.y"
  {
     yyVal = null;
  }
  break;
case 167:
#line 1017 "XQuery.y"
  {
     yyVal = Lisp.Append(Lisp.Cons(new TokenWrapper('+')), yyVals[0+yyTop]);
  }
  break;
case 168:
#line 1021 "XQuery.y"
  {
     yyVal = Lisp.Append(Lisp.Cons(new TokenWrapper('-')), yyVals[0+yyTop]);
  }
  break;
case 169:
#line 1028 "XQuery.y"
  {
     yyVal = new Literal("=");
  }
  break;
case 170:
#line 1032 "XQuery.y"
  {
     yyVal = new Literal("!=");
  }
  break;
case 171:
#line 1036 "XQuery.y"
  {
     yyVal = new Literal("<");
  }
  break;
case 172:
#line 1040 "XQuery.y"
  {
     yyVal = new Literal("<=");
  }
  break;
case 173:
#line 1044 "XQuery.y"
  {
     yyVal = new Literal(">");
  }
  break;
case 174:
#line 1048 "XQuery.y"
  {
     yyVal = new Literal(">=");
  }
  break;
case 175:
#line 1055 "XQuery.y"
  {
     yyVal = new TokenWrapper(Token.EQ);
  }
  break;
case 176:
#line 1059 "XQuery.y"
  {
     yyVal = new TokenWrapper(Token.NE);
  }
  break;
case 177:
#line 1063 "XQuery.y"
  {
     yyVal = new TokenWrapper(Token.LT);
  }
  break;
case 178:
#line 1067 "XQuery.y"
  {
     yyVal = new TokenWrapper(Token.LE);
  }
  break;
case 179:
#line 1071 "XQuery.y"
  {
     yyVal = new TokenWrapper(Token.GT);
  }
  break;
case 180:
#line 1075 "XQuery.y"
  {
     yyVal = new TokenWrapper(Token.GE);
  }
  break;
case 181:
#line 1082 "XQuery.y"
  {
     yyVal = new TokenWrapper(Token.IS);
  }
  break;
case 182:
#line 1086 "XQuery.y"
  {
     yyVal = new Literal("<<");
  }
  break;
case 183:
#line 1090 "XQuery.y"
  {
     yyVal = new Literal(">>");
  }
  break;
case 187:
#line 1104 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Validate, null, yyVals[-1+yyTop]);
  }
  break;
case 188:
#line 1108 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Validate, yyVals[-3+yyTop], yyVals[-1+yyTop]);
  }
  break;
case 189:
#line 1115 "XQuery.y"
  {
     yyVal = new TokenWrapper(Token.LAX);
  }
  break;
case 190:
#line 1119 "XQuery.y"
  {
     yyVal = new TokenWrapper(Token.STRICT);
  }
  break;
case 191:
#line 1126 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.ExtensionExpr, yyVals[-3+yyTop], yyVals[-1+yyTop]);
  }
  break;
case 192:
#line 1133 "XQuery.y"
  {
     yyVal = Lisp.Cons(yyVals[0+yyTop]);
  }
  break;
case 193:
#line 1137 "XQuery.y"
  {
     yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(yyVals[0+yyTop]));
  }
  break;
case 194:
#line 1144 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Pragma, yyVals[-2+yyTop], yyVals[-1+yyTop]);
   }
  break;
case 195:
#line 1151 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Child, new object[] { null });
  }
  break;
case 196:
#line 1155 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Child, yyVals[0+yyTop]);
  }
  break;
case 197:
#line 1159 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Descendant, yyVals[0+yyTop]);
  }
  break;
case 200:
#line 1168 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Child, yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 201:
#line 1172 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Descendant, yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 202:
#line 1179 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.AxisStep, yyVals[0+yyTop]);
  }
  break;
case 203:
#line 1183 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.FilterExpr, yyVals[0+yyTop]);
  }
  break;
case 205:
#line 1191 "XQuery.y"
  {
      yyVal = yyVals[-1+yyTop];
      notation.Confirm((Symbol)yyVals[-1+yyTop], Descriptor.PredicateList, yyVals[0+yyTop]);
  }
  break;
case 207:
#line 1197 "XQuery.y"
  {
      yyVal = yyVals[-1+yyTop];
      notation.Confirm((Symbol)yyVals[-1+yyTop], Descriptor.PredicateList, yyVals[0+yyTop]);
  }
  break;
case 208:
#line 1205 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.ForwardStep, yyVals[-1+yyTop], yyVals[0+yyTop]);
   }
  break;
case 210:
#line 1213 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.AXIS_CHILD);
   }
  break;
case 211:
#line 1217 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.AXIS_DESCENDANT);
   }
  break;
case 212:
#line 1221 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.AXIS_ATTRIBUTE);
   }
  break;
case 213:
#line 1225 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.AXIS_SELF);
   }
  break;
case 214:
#line 1229 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.AXIS_DESCENDANT_OR_SELF);
   }
  break;
case 215:
#line 1233 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.AXIS_FOLLOWING_SIBLING);
   }
  break;
case 216:
#line 1237 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.AXIS_FOLLOWING);
   }
  break;
case 217:
#line 1241 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.AXIS_NAMESPACE);
   }
  break;
case 218:
#line 1248 "XQuery.y"
  {  
	  yyVal = notation.Confirm((Symbol)yyVals[0+yyTop], Descriptor.AbbrevForward, yyVals[0+yyTop]); 
   }
  break;
case 220:
#line 1256 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.ReverseStep, yyVals[-1+yyTop], yyVals[0+yyTop]);
   }
  break;
case 222:
#line 1264 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.AXIS_PARENT);
   }
  break;
case 223:
#line 1268 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.AXIS_ANCESTOR);
   }
  break;
case 224:
#line 1272 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.AXIS_PRECEDING_SIBLING);
   }
  break;
case 225:
#line 1276 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.AXIS_PRECEDING);
   }
  break;
case 226:
#line 1280 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.AXIS_ANCESTOR_OR_SELF);
   }
  break;
case 227:
#line 1287 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.DOUBLE_PERIOD);
   }
  break;
case 232:
#line 1304 "XQuery.y"
  {
      yyVal = new TokenWrapper('*');
   }
  break;
case 233:
#line 1308 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Wildcard1, yyVals[-2+yyTop]);
   }
  break;
case 234:
#line 1312 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Wildcard2, yyVals[0+yyTop]);
   }
  break;
case 236:
#line 1320 "XQuery.y"
  {
      yyVal = yyVals[-1+yyTop];
      notation.Confirm((Symbol)yyVals[-1+yyTop], Descriptor.PredicateList, yyVals[0+yyTop]);
   }
  break;
case 237:
#line 1328 "XQuery.y"
  {
      yyVal = Lisp.Cons(yyVals[0+yyTop]);
   }
  break;
case 238:
#line 1332 "XQuery.y"
  {
      yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(yyVals[0+yyTop]));
   }
  break;
case 239:
#line 1339 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Predicate, yyVals[-1+yyTop]);
   }
  break;
case 253:
#line 1368 "XQuery.y"
  {
      yyVal = yyVals[0+yyTop];
   }
  break;
case 254:
#line 1375 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.ParenthesizedExpr, new object[] { null });
   }
  break;
case 255:
#line 1379 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.ParenthesizedExpr, yyVals[-1+yyTop]);
   }
  break;
case 256:
#line 1386 "XQuery.y"
  {
      yyVal = new TokenWrapper('.');
   }
  break;
case 257:
#line 1393 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Ordered, yyVals[-1+yyTop]);
   }
  break;
case 258:
#line 1400 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Unordered, yyVals[-1+yyTop]);
   }
  break;
case 259:
#line 1407 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Funcall, yyVals[-2+yyTop], null);
   }
  break;
case 260:
#line 1411 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Funcall, yyVals[-3+yyTop], yyVals[-1+yyTop]);
   }
  break;
case 261:
#line 1418 "XQuery.y"
  {
      yyVal = Lisp.Cons(yyVals[0+yyTop]);
   }
  break;
case 262:
#line 1422 "XQuery.y"
  {
      yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
   }
  break;
case 268:
#line 1440 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirElemConstructor, yyVals[-3+yyTop], yyVals[-2+yyTop]);
   }
  break;
case 269:
#line 1444 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirElemConstructor, 
		 yyVals[-7+yyTop], yyVals[-6+yyTop], null, yyVals[-2+yyTop], yyVals[-1+yyTop]);
   }
  break;
case 270:
#line 1449 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirElemConstructor, 
		 yyVals[-8+yyTop], yyVals[-7+yyTop], yyVals[-5+yyTop], yyVals[-2+yyTop], yyVals[-1+yyTop]);
   }
  break;
case 271:
#line 1457 "XQuery.y"
  {
      yyVal = Lisp.Cons(yyVals[0+yyTop]);
   }
  break;
case 272:
#line 1461 "XQuery.y"
  {      
      yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(yyVals[0+yyTop]));
   }
  break;
case 273:
#line 1468 "XQuery.y"
  {
      yyVal = null;
   }
  break;
case 275:
#line 1476 "XQuery.y"
  {
      yyVal = Lisp.List(yyVals[-1+yyTop], yyVals[0+yyTop]);   
   }
  break;
case 276:
#line 1480 "XQuery.y"
  {
      yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(yyVals[0+yyTop]));
   }
  break;
case 277:
#line 1484 "XQuery.y"
  {
      yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.List(yyVals[-1+yyTop], yyVals[0+yyTop]));
   }
  break;
case 278:
#line 1491 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirAttribute,
		 yyVals[-6+yyTop], yyVals[-5+yyTop], yyVals[-3+yyTop], new Literal("\""), yyVals[-1+yyTop]);
   }
  break;
case 279:
#line 1496 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirAttribute,
		 yyVals[-6+yyTop], yyVals[-5+yyTop], yyVals[-3+yyTop], new Literal("\'"), yyVals[-1+yyTop]);
   }
  break;
case 280:
#line 1504 "XQuery.y"
  {
      yyVal = Lisp.Cons(new TokenWrapper(Token.EscapeQuot));
   }
  break;
case 281:
#line 1508 "XQuery.y"
  {
      yyVal = Lisp.Cons(yyVals[0+yyTop]);
   }
  break;
case 282:
#line 1512 "XQuery.y"
  {
      yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(new TokenWrapper(Token.EscapeQuot)));
   }
  break;
case 283:
#line 1516 "XQuery.y"
  {
      yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(yyVals[0+yyTop]));
   }
  break;
case 284:
#line 1523 "XQuery.y"
  {
      yyVal = Lisp.Cons(new TokenWrapper(Token.EscapeApos));
   }
  break;
case 285:
#line 1527 "XQuery.y"
  {
      yyVal = Lisp.Cons(yyVals[0+yyTop]);
   }
  break;
case 286:
#line 1531 "XQuery.y"
  {
      yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(new TokenWrapper(Token.EscapeApos)));
   }
  break;
case 287:
#line 1535 "XQuery.y"
  {
      yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(yyVals[0+yyTop]));
   }
  break;
case 298:
#line 1561 "XQuery.y"
  {
      yyVal = new Literal("{{");
   }
  break;
case 299:
#line 1565 "XQuery.y"
  {
      yyVal = new Literal("}}");
   }
  break;
case 300:
#line 1569 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CommonContent), Descriptor.EnclosedExpr, yyVals[0+yyTop]); 
   }
  break;
case 301:
#line 1576 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirCommentConstructor, yyVals[-1+yyTop]);
   }
  break;
case 302:
#line 1583 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirPIConstructor, yyVals[-1+yyTop], null);
   }
  break;
case 303:
#line 1587 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirPIConstructor, yyVals[-3+yyTop], yyVals[-1+yyTop]);
   }
  break;
case 304:
#line 1594 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CData), Descriptor.CDataSection, yyVals[-1+yyTop]);
   }
  break;
case 311:
#line 1610 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompDocConstructor, yyVals[-1+yyTop]);
   }
  break;
case 312:
#line 1618 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompElemConstructor, yyVals[-3+yyTop], yyVals[-1+yyTop]);   
   }
  break;
case 313:
#line 1623 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompElemConstructor, yyVals[-2+yyTop], null);   
   }
  break;
case 314:
#line 1628 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompElemConstructor, yyVals[-4+yyTop], yyVals[-1+yyTop]);   
   }
  break;
case 315:
#line 1633 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompElemConstructor, yyVals[-3+yyTop], null);   
   }
  break;
case 317:
#line 1645 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompAttrConstructor, yyVals[-3+yyTop], yyVals[-1+yyTop]);   
   }
  break;
case 318:
#line 1650 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompAttrConstructor, yyVals[-2+yyTop], null);   
   }
  break;
case 319:
#line 1655 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompAttrConstructor, yyVals[-4+yyTop], yyVals[-1+yyTop]);   
   }
  break;
case 320:
#line 1660 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompAttrConstructor, yyVals[-3+yyTop], null);   
   }
  break;
case 321:
#line 1668 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompTextConstructor, yyVals[-1+yyTop]);   
   }
  break;
case 322:
#line 1676 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompCommentConstructor, yyVals[-1+yyTop]);   
   }
  break;
case 323:
#line 1684 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompPIConstructor, yyVals[-3+yyTop], yyVals[-1+yyTop]);   
   }
  break;
case 324:
#line 1689 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompPIConstructor, yyVals[-2+yyTop], null);   
   }
  break;
case 325:
#line 1694 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompPIConstructor, yyVals[-4+yyTop], yyVals[-1+yyTop]);   
   }
  break;
case 326:
#line 1699 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompPIConstructor, yyVals[-3+yyTop], null);   
   }
  break;
case 328:
#line 1708 "XQuery.y"
  {
      yyVal = yyVals[-1+yyTop];
      notation.Confirm((Symbol)yyVals[-1+yyTop], Descriptor.Occurrence, 
		new TokenWrapper(Token.Indicator3));
   }
  break;
case 329:
#line 1717 "XQuery.y"
  {
      yyVal = yyVals[0+yyTop];
   }
  break;
case 331:
#line 1725 "XQuery.y"
  {
      yyVal = yyVals[-1+yyTop];
      notation.Confirm((Symbol)yyVals[-1+yyTop], Descriptor.Occurrence, yyVals[0+yyTop]);
   }
  break;
case 332:
#line 1730 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.VOID);
   }
  break;
case 333:
#line 1737 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.Indicator1);
   }
  break;
case 334:
#line 1741 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.Indicator2);
   }
  break;
case 335:
#line 1745 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.Indicator3);
   }
  break;
case 338:
#line 1754 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.ITEM);
   }
  break;
case 340:
#line 1765 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.KindTest, yyVals[0+yyTop]);
   }
  break;
case 350:
#line 1783 "XQuery.y"
  {
       yyVal = new TokenWrapper(Token.NODE);
   }
  break;
case 351:
#line 1790 "XQuery.y"
  {
       yyVal = new TokenWrapper(Token.DOCUMENT_NODE);
   }
  break;
case 352:
#line 1794 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.DocumentNode, yyVals[-1+yyTop]);
   }
  break;
case 353:
#line 1798 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.DocumentNode, yyVals[-1+yyTop]);
   }
  break;
case 354:
#line 1805 "XQuery.y"
  {
       yyVal = new TokenWrapper(Token.TEXT);
   }
  break;
case 355:
#line 1812 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.COMMENT);
   }
  break;
case 356:
#line 1820 "XQuery.y"
  {
       yyVal = new TokenWrapper(Token.PROCESSING_INSTRUCTION);
   }
  break;
case 357:
#line 1824 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.ProcessingInstruction, yyVals[-1+yyTop]);
   }
  break;
case 358:
#line 1828 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.ProcessingInstruction, yyVals[-1+yyTop]);
   }
  break;
case 359:
#line 1835 "XQuery.y"
  {
       yyVal = new TokenWrapper(Token.ELEMENT);
   }
  break;
case 360:
#line 1839 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Element, yyVals[-1+yyTop]);
   }
  break;
case 361:
#line 1843 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Element, yyVals[-3+yyTop], yyVals[-1+yyTop]);
   }
  break;
case 362:
#line 1847 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Element, 
		yyVals[-4+yyTop], yyVals[-2+yyTop], new TokenWrapper('?'));
   }
  break;
case 364:
#line 1856 "XQuery.y"
  {
      yyVal = new TokenWrapper('*');
   }
  break;
case 365:
#line 1863 "XQuery.y"
  {
       yyVal = new TokenWrapper(Token.ATTRIBUTE);
   }
  break;
case 366:
#line 1867 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Attribute, yyVals[-1+yyTop]);
   }
  break;
case 367:
#line 1871 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Attribute, yyVals[-3+yyTop], yyVals[-1+yyTop]);
   }
  break;
case 369:
#line 1879 "XQuery.y"
  {
      yyVal = new TokenWrapper('*');
   }
  break;
case 370:
#line 1886 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.SchemaElement, yyVals[-1+yyTop]);
   }
  break;
case 371:
#line 1893 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.SchemaAttribute, yyVals[-1+yyTop]);
   }
  break;
case 375:
#line 1912 "XQuery.y"
  {
      yyVal = null;
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
    0,    0,    0,    0,    1,    1,    2,    3,    7,    5,
    5,    5,    5,    9,    9,   10,   10,   11,   11,   11,
   11,   12,   12,   12,   13,   13,   13,   13,   13,   13,
   13,   14,   14,    4,   15,   20,   20,   16,   16,   19,
   24,   24,   25,   25,   26,   29,   29,   30,   30,   21,
   22,   27,   27,   31,   31,   32,   32,   33,   33,   28,
   28,   17,   17,   34,   34,   23,   23,   18,   18,   38,
   38,   37,   37,   41,   41,   42,   42,   40,    6,   43,
   43,   35,   35,   35,   35,   35,   44,   44,   44,   44,
   49,   49,   49,   49,   52,   54,   54,   55,   56,   56,
   57,   53,   58,   58,   59,   50,   51,   51,   60,   60,
   61,   61,   62,   62,   62,   62,   63,   63,   64,   64,
   45,   45,   65,   65,   66,   46,   46,   67,   67,   68,
   68,   47,   48,   48,   69,   69,   70,   70,   70,   70,
   71,   71,   75,   75,   75,   76,   76,   76,   76,   76,
   77,   77,   77,   78,   78,   78,   79,   79,   80,   80,
   81,   81,   82,   82,   84,   85,   85,   85,   73,   73,
   73,   73,   73,   73,   72,   72,   72,   72,   72,   72,
   74,   74,   74,   86,   86,   86,   87,   87,   90,   90,
   89,   91,   91,   92,   88,   88,   88,   88,   94,   94,
   94,   95,   95,   96,   96,   96,   96,   98,   98,  101,
  101,  101,  101,  101,  101,  101,  101,  103,  103,  100,
  100,  104,  104,  104,  104,  104,  105,  102,  102,  107,
  107,  108,  108,  108,   97,   97,   99,   99,  110,  109,
  109,  109,  109,  109,  109,  109,  109,  111,  111,  119,
  119,  119,  112,  113,  113,  114,  117,  118,  115,  115,
  120,  120,  116,  116,  121,  121,  121,  123,  123,  123,
  127,  127,  126,  126,  129,  129,  129,  130,  130,  131,
  131,  131,  131,  132,  132,  132,  132,  133,  133,  134,
  134,  128,  128,  128,  128,  136,  136,  136,  136,  136,
  124,  125,  125,  139,  122,  122,  122,  122,  122,  122,
  140,  141,  141,  141,  141,  146,  142,  142,  142,  142,
  143,  144,  145,  145,  145,  145,   83,   83,   36,   39,
   39,   39,  149,  149,  149,  148,  148,  148,  147,  106,
  150,  150,  150,  150,  150,  150,  150,  150,  150,  159,
  151,  151,  151,  158,  157,  156,  156,  156,  152,  152,
  152,  152,  160,  160,  153,  153,  153,  163,  163,  154,
  155,  164,  162,  161,   93,   93,  135,  137,  138,    8,
  };
   static  short [] yyLen = {           2,
    2,    1,    2,    1,    3,    5,    2,    2,    5,    0,
    1,    1,    2,    2,    3,    2,    3,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    4,    2,    2,    3,    3,    3,
    2,    2,    2,    2,    4,    1,    1,    1,    1,    2,
    2,    3,    5,    0,    1,    1,    3,    3,    2,    4,
    7,    7,    5,    0,    1,    2,    2,    6,    8,    1,
    1,    0,    1,    1,    3,    2,    3,    3,    1,    1,
    3,    1,    1,    1,    1,    1,    3,    4,    4,    5,
    1,    1,    2,    2,    2,    1,    3,    6,    0,    1,
    3,    2,    1,    3,    6,    2,    2,    2,    1,    3,
    1,    2,    1,    2,    3,    4,    1,    1,    1,    1,
    4,    4,    1,    3,    5,    8,   10,    1,    2,    7,
    4,    8,    1,    3,    1,    3,    1,    3,    3,    3,
    1,    3,    1,    3,    3,    1,    3,    3,    3,    3,
    1,    3,    3,    1,    3,    3,    1,    3,    1,    3,
    1,    3,    1,    3,    2,    0,    2,    2,    1,    2,
    1,    2,    1,    2,    1,    1,    1,    1,    1,    1,
    1,    2,    2,    1,    1,    1,    4,    5,    1,    1,
    4,    1,    2,    5,    1,    2,    2,    1,    1,    3,
    3,    1,    1,    1,    2,    1,    2,    2,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    2,    1,    2,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    3,    3,    1,    2,    1,    2,    3,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    2,    2,    3,    1,    4,    4,    3,    4,
    1,    3,    1,    1,    1,    1,    1,    5,    9,   10,
    1,    2,    0,    1,    2,    2,    3,    7,    7,    1,
    1,    2,    2,    1,    1,    2,    2,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    2,    2,    1,
    3,    3,    5,    3,    1,    1,    1,    1,    1,    1,
    4,    5,    4,    7,    6,    1,    5,    4,    7,    6,
    4,    4,    5,    4,    7,    6,    1,    2,    2,    1,
    2,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    3,
    3,    4,    4,    3,    3,    3,    4,    4,    3,    4,
    6,    7,    1,    1,    3,    4,    6,    1,    1,    4,
    4,    1,    1,    1,    0,    1,    1,    1,    1,    1,
  };
   static  short [] yyDefRed = {            0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    2,
    4,    0,    0,    0,    0,    0,    0,   18,   19,   20,
   21,   22,   23,   24,   25,   26,   27,   28,   29,   30,
   31,   32,   33,    0,    0,    0,    0,    0,   55,    0,
  380,    0,    0,   36,   37,    0,    0,   43,   44,    0,
   41,   42,   46,   47,    0,   50,   51,    0,   66,   67,
    0,    1,    3,    0,    0,    0,    0,    0,    0,    0,
    0,    7,   80,    0,   82,   83,   84,   85,    0,    0,
   91,   92,    0,  135,    0,    0,    0,    0,    0,  154,
    0,    0,    0,    0,    0,    8,    0,    0,    0,   34,
   14,   16,    0,    5,    0,    0,   59,    0,    0,    0,
    0,   38,   39,   40,    0,    0,    0,    0,    0,   96,
    0,    0,  103,    0,    0,  123,    0,    0,    0,  167,
  168,    0,    0,    0,    0,    0,    0,    0,    0,   93,
   94,    0,  175,  176,  177,  179,  180,  178,  181,  169,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  227,  249,  250,  251,  252,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  210,  211,  212,  213,  214,
  215,  216,  222,  223,  224,  225,  226,  217,    0,    0,
    0,    0,    0,    0,  256,  165,  184,  185,  186,    0,
  192,    0,  199,  202,  203,    0,    0,    0,  219,  209,
    0,  221,  228,  229,  231,    0,  240,  241,  242,  243,
  244,  245,  246,  247,  248,  263,  264,  265,  266,  267,
  305,  306,  307,  308,  309,  310,  340,  341,  342,  343,
  344,  345,  346,  347,  348,  349,   15,   17,    0,    0,
   58,    0,    0,   56,    0,   35,   48,   49,   45,    0,
    0,   65,    0,    0,    0,   74,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,   81,    0,   87,  106,
    0,    0,  109,    0,    0,    0,    0,  136,  170,  172,
  182,  174,  183,  138,  139,  140,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  155,  156,    0,    0,    0,
    0,  339,    0,  332,  338,  158,  337,  336,    0,  160,
  162,    0,  164,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  189,  190,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  376,    0,    0,  253,  254,    0,    0,  230,
  218,    0,    0,  193,    0,    0,    0,    0,  237,    0,
  208,  220,    0,    6,    9,    0,    0,    0,  329,   63,
    0,    0,    0,    0,    0,   97,    0,  104,    0,  121,
  124,  122,    0,    0,  117,  118,  112,    0,    0,   88,
    0,   89,  333,  334,  335,  331,  328,    0,    0,    0,
    0,  373,  359,  364,    0,  363,    0,    0,  372,  365,
  369,    0,  368,    0,  354,    0,  355,    0,    0,    0,
  350,  233,  259,  261,    0,    0,    0,    0,  356,    0,
  351,    0,    0,    0,    0,  301,    0,  302,    0,    0,
    0,    0,  255,  234,    0,  201,  200,    0,  238,    0,
   57,    0,   77,   71,    0,    0,   68,   70,   75,    0,
    0,  100,    0,    0,    0,    0,  128,    0,    0,  119,
  120,    0,  110,   90,  257,  258,  311,  313,    0,    0,
    0,  360,    0,  318,    0,    0,  366,    0,  321,  322,
  187,    0,    0,  260,  324,    0,  358,  357,    0,  352,
  353,  370,  371,    0,    0,    0,  275,    0,    0,    0,
  191,  239,    0,   62,    0,    0,    0,    0,    0,  125,
    0,    0,    0,  129,    0,  115,    0,  312,  374,    0,
    0,  317,    0,    0,  188,  262,  323,    0,  303,  194,
    0,  379,  296,  297,    0,    0,    0,    0,  300,  292,
    0,  271,  295,  293,  294,  268,  277,   69,   78,  101,
   98,  105,    0,    0,    0,    0,    0,  116,  361,    0,
  315,    0,  367,  320,    0,  326,    0,    0,    0,  298,
  299,    0,    0,  272,    0,  131,  126,    0,  132,  362,
  314,  319,  325,    0,  304,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  127,  378,  284,    0,  285,  291,
  290,  377,  280,    0,  281,  288,  289,  269,    0,  130,
  279,  286,  287,  282,  278,  283,  270,
  };
  protected static  short [] yyDgoto  = {            18,
   19,   20,   21,  111,   22,   82,   23,  284,   24,   25,
   26,   27,   28,   29,   30,   31,   32,   33,   34,   35,
   36,   37,   38,   39,   40,   41,   42,   43,   65,  289,
   48,  285,   49,  291,   83,  292,  294,  497,  346,  589,
  295,  296,  519,   85,   86,   87,   88,   89,   90,  148,
  149,   91,   92,  129,  130,  501,  502,  132,  133,  312,
  313,  427,  428,  512,  135,  136,  506,  507,   93,   94,
   95,  164,  165,  166,   96,   97,   98,   99,  100,  101,
  102,  103,  351,  104,  105,  226,  227,  228,  229,  370,
  230,  231,  384,  232,  233,  234,  235,  236,  398,  237,
  238,  239,  240,  241,  242,  243,  244,  245,  246,  399,
  247,  248,  249,  250,  251,  252,  253,  254,  255,  465,
  256,  257,  258,  259,  260,  481,  591,  592,  482,  547,
  654,  648,  655,  649,  656,  593,  651,  594,  595,  261,
  262,  263,  264,  265,  266,  520,  348,  349,  436,  267,
  268,  269,  270,  271,  272,  273,  274,  275,  276,  445,
  570,  446,  452,  453,
  };
  protected static  short [] yySindex = {          823,
 -248, -183, -203, -201, -178,  -74, -191, -181, -251, -167,
 -157,  -54, -152, -152,  156,  -48, -125,    0,  848,    0,
    0,  367, 1037, 1037, -197,  170,  170,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  -32,  154,  -84,   20, -152,    0,  -80,
    0,   18,  193,    0,    0, -152, -152,    0,    0,  -27,
    0,    0,    0,    0,  255,    0,    0,  -35,    0,    0,
  269,    0,    0,  284,  305,  330,  330,  328,  336,  176,
  176,    0,    0,  342,    0,    0,    0,    0,  116, -147,
    0,    0,  120,    0,   89,   -9, -237,  -77,  -49,    0,
  114,  122,  132,  134, 4347,    0, -197,  170,  170,    0,
    0,    0,  123,    0, -152,  379,    0,  174,  386, -152,
 -152,    0,    0,    0,   17,  182,  416,  131,  418,    0,
  145,  424,    0,  148,   -6,    0,   -5,  367,  367,    0,
    0,  367,  176,  367,  367,  367,  367, -168,  201,    0,
    0,  176,    0,    0,    0,    0,    0,    0,    0,    0,
  411,  288,  277,  176,  176,  176,  176,  176,  176,  176,
  176,  176,  176,  176,  176,  176,  176, 1068, 1068,  160,
  160,  354,  360,  361,  -24,  -23,   25,   29,  -71,  455,
    0,    0,    0,    0,    0,  438,  461,  -17,  466,  469,
  470, 4605,  192,  195,  188,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  196,  197,
  144, 4605,  320,  460,    0,    0,    0,    0,    0, -104,
    0,  -26,    0,    0,    0,  429,  429,  320,    0,    0,
  320,    0,    0,    0,    0,  429,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  170,  170,
    0, -152, -152,    0,  483,    0,    0,    0,    0, 1068,
   -8,    0,  207,  491,  490,    0,  182,  284,  182,  305,
  182,  367,  330,  367,  162,  189,    0,  120,    0,    0,
   98,  505,    0,  505,  367,  265,  367,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  217, -237, -237,  -77,
  -77,  -77,  -77,  -49,  -49,    0,    0,  510,  513,  514,
  515,    0,  517,    0,    0,    0,    0,    0,    6,    0,
    0,  495,    0,  367,  367,  367,  436,  -13,  367,  437,
  -10,  367,  520,  367,  521,  367,    0,    0,  367,  440,
  525,  526,  222,  444,  -16,  367,  -34,  248,  249,  -26,
  223, -254,    0,  263,  261,    0,    0,  190,  -26,    0,
    0,  253,  367,    0, 4605, 4605,  367,  429,    0,  429,
    0,    0,  429,    0,    0,  483,  309, -152,    0,    0,
  528,  182,  -78,  416,  319,    0,  536,    0,  326,    0,
    0,    0,  321,  317,    0,    0,    0, -213,  367,    0,
  367,    0,    0,    0,    0,    0,    0,   30,   31,   32,
  300,    0,    0,    0,  220,    0,   33,  340,    0,    0,
    0,  227,    0,   34,    0,   35,    0,   37,   40,  367,
    0,    0,    0,    0,  236,  488,  562,  563,    0,   41,
    0,  564,  565,  578,  581,    0,  316,    0,  312,  315,
  136,  313,    0,    0,   43,    0,    0,   46,    0, -152,
    0,  367,    0,    0, 1068,  367,    0,    0,    0,  597,
  373,    0,  582,  367,  468,  119,    0,  367, -152,    0,
    0,  370,    0,    0,    0,    0,    0,    0,  342,  524,
  331,    0,  530,    0,   44,  331,    0,  531,    0,    0,
    0,   45,  367,    0,    0,   47,    0,    0,  532,    0,
    0,    0,    0,  293,  297,  188,    0,    4,  596,  315,
    0,    0,  483,    0,  -75,   48,  344,  367,  367,    0,
  346,  421,    5,    0,  403,    0, -152,    0,    0,   55,
  571,    0,  652,  672,    0,    0,    0,  807,    0,    0,
  633,    0,    0,    0,  383,  867,  570,  653,    0,    0,
   52,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  434,  367,  367,  381,  367,    0,    0,  660,
    0,  584,    0,    0,   53,    0,   54,  188,  350,    0,
    0,  385,  667,    0, 1068,    0,    0,  448,    0,    0,
    0,    0,    0,  -21,    0,  188,  404,  454,  367,  -99,
  -88,  664,  188,  367,    0,    0,    0, -103,    0,    0,
    0,    0,    0,  -20,    0,    0,    0,    0,  665,    0,
    0,    0,    0,    0,    0,    0,    0,
  };
  protected static  short [] yyRindex = {         3935,
    0,    0,  412,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0, 3935,    0,
    0, 4491,  729,  171,  310,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0, 4491,
 4491,    0,    0,  730,    0,    0,    0,    0,   42,    0,
    0,    0,  648,    0,  615, 3862, 3483, 3143, 2742,    0,
 2696, 2608, 2418, 2228,    0,    0,  433,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  673,    0,    0,
    0,    0,    0,    0,    0,   -7,  690,    0,   21,    0,
    0,  243,    0,    0,    0,    0,    0, 4491, 4491,    0,
    0, 4491, 4491, 4491, 4491, 4491, 4491,    0,    0,    0,
    0, 4491,    0,    0,    0,    0,    0,    0,    0,    0,
    0, 4137, 4233, 4491, 4491, 4491, 4491, 4491, 4491, 4491,
 4491, 4491, 4491, 4491, 4491, 4491, 4491,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  734,    0,    0,    0,
    0,    0,    0,    0,  414,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
 4491, 1760,    0,  822,    0,    0,    0,    0,    0,    0,
    0, 1862,    0,    0,    0,  924, 1012,    0,    0,    0,
    0,    0,    0,    0,    0, 1202,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  674,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  695,    0,    3,    0,  679,    0,
  473, 4491,    0, 4491,    0,    0,    0, 1363,    0,    0,
  -14,  475,    0,  476, 4491,    0, 4491,    0,    0,    0,
    0,    0,    0,    0,    0,    0, 4065, 3740, 3812, 3277,
 3400, 3534, 3657, 2886, 3020,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0, 1292,    0,
    0, 2330,    0, 4491, 4491, 4491,    0,    0, 4491,    0,
    0, 4491,    0, 4491,    0, 4491,    0,    0, 4491,    0,
    0,    0, 4491,    0,    0, 4491,    0,    0,    0, 1950,
    0,    0,    0,    0,  146,    0,    0,    0, 2140,    0,
    0,    0, 4491,    0,    0,    0, 4491, 1392,    0, 1482,
    0,    0, 1672,    0,    0,  680,    0,    0,    0,    0,
    0,  278,    0,    0,  474,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,   -4, 4491,    0,
 4491,    0,    0,    0,    0,    0,    0,    0,    0,    0,
 4491,    0,    0,    0,    0,    0,    0, 4491,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0, 4491,
    0,    0,    0,    0,    0, 4491,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  147,    0,    0,    0,    0,    0,    0,    0,    0,
    0, 4491,    0,    0,    0, 4491,    0,    0,    0,    0,
    0,    0,    0, 4491,    0,    0,    0, 4491,    0,    0,
    0,   -1,    0,    0,    0,    0,    0,    0,  613,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0, 4491,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  700,    0,    0,    0,  -29,
    0,    0,  703,    0,    0,    0,    0, 4491, 4491,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
 4491,    0,    0, 4491,    0,    0,    0, 4491,    0,    0,
    0,    0,    0,    0,    0, 4491,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0, 4491, 4491,    0, 4491,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  -19,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  701,    0,    0, 4491,    0,
    0,    0,  701, 4491,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,
  };
  protected static  short [] yyGindex = {            0,
    0,  745,  750,   22,  747,    0,    0,   -2,    0,  748,
  752,   36,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0, -238,    0, -163, -141,  362,    0,  225, -169, -360,
    0,  377,  -22,    0,    0,    0,    0,    0,    0,    0,
  663,  722,  724,    0,  518,    0,    0,    0,  523,  668,
  388,    0,    0,    0,  749,  527,    0,  318,  685,  677,
  216,    0,    0,    0,  666,  234,   75,  230,  252,    0,
    0,    0,  654,    0,  339,    0,    0,    0,    0,    0,
    0,  604, -490, -129,   50,    0,    0,    0, -166,    0,
    0,  -41,    0,    0,    0, -170,    0,    0,    0, -289,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
 -481,    0,    0,    0,    0,    0,    0,  245,    0,  287,
    0,    0,  184,  191,    0, -397,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  270,  250,    0,    0,    0,
    0,  463,    0,  467,    0,    0,    0,    0,    0,    0,
  327,  478,    0,  481,
  };
  protected static  short [] yyTable = {            84,
  307,   52,  309,  310,  311,  311,  471,  347,  347,  350,
   66,   67,  641,  665,  375,  358,  361,  276,  393,  586,
  396,  587,  375,  586,  469,  587,  110,  443,  444,  111,
  450,  451,  276,  168,  586,  169,  587,  303,  303,  113,
  606,   86,  114,  406,  496,  118,  175,  496,  112,  411,
   64,  369,  498,  122,  123,  581,  170,  171,  172,   46,
  109,   50,  509,  588,  363,  114,  590,   44,  365,  477,
  400,   56,  380,  142,  142,  142,  142,  142,  142,  403,
  142,   57,   86,  142,  142,   86,  142,  142,  142,  142,
  142,  142,  389,   58,   59,  609,  142,  142,  359,  362,
   86,  315,  586,  478,  587,  376,   61,   62,  489,  590,
  489,  623,  280,  489,   51,  305,  306,  610,  286,  347,
  409,  161,  144,   74,   75,  145,  586,  634,  587,  277,
  278,  510,  511,  415,   86,  417,   45,  419,  552,   10,
  173,   53,  109,   47,   15,  642,   17,  364,  162,  160,
  163,  366,  659,   60,  515,  516,  517,  523,  528,  529,
  420,  530,  422,   51,  531,  539,   86,  551,  572,  575,
   11,  577,  599,  430,  586,  432,  587,  632,  633,  146,
  147,  391,  549,   54,  387,   55,   80,  494,   81,  495,
  494,   68,  273,  274,  498,   71,  401,  548,  388,  402,
  146,  147,  423,   63,   64,  142,   11,  273,  274,   69,
   11,   70,   11,   11,  115,   11,   11,   11,   80,  174,
   81,  646,  583,  584,  113,  646,  583,  584,  110,  424,
  483,  464,  142,  142,   11,  116,  652,  583,  584,  119,
  367,  368,  650,  657,  330,  331,  332,  333,  176,  177,
  650,  553,  338,  121,  205,  111,  657,  410,   64,  168,
  522,  169,  463,  521,   80,  113,   81,  527,  114,   64,
  526,   64,  302,  304,  605,  661,  534,  287,  288,  533,
  407,  662,  117,  167,  120,  647,  126,  311,  124,  514,
   95,   95,   95,   95,  276,  653,  357,  360,  125,  467,
  404,  405,  374,  468,  652,  583,  584,  442,  127,   12,
  449,   86,   86,   86,   86,   86,   86,  200,   76,  128,
   86,   76,   86,   86,  347,  555,   86,  395,  582,  583,
  584,  438,  439,  440,  347,  562,  447,  322,  323,  454,
  131,  456,   80,  458,   81,   12,  459,  321,  320,   12,
  554,   12,   12,  470,   12,   12,   12,  640,  203,  375,
  204,  224,  560,  664,  585,  134,  565,  138,   95,   95,
  485,  425,  426,   12,  488,  139,  582,  583,  584,  324,
  325,  326,   80,  219,   81,  142,  433,  434,  435,   86,
   86,  576,  153,  154,  155,  156,  157,  158,  159,  505,
  563,  328,  329,  334,  335,  491,  203,  143,  204,   80,
  152,   81,  585,  178,   74,   75,  601,  602,  140,  141,
   76,   77,  179,   78,  518,  525,   79,  336,  337,  352,
  352,  219,   13,  180,   11,   11,  181,  532,  279,  281,
  282,   11,   11,  536,  486,  487,  283,   11,   11,  290,
   11,  293,  297,   11,  347,  638,   11,   11,   11,   11,
   11,  298,  626,  627,  524,  629,  299,  300,   13,  301,
  317,  319,   13,  556,   13,   13,  354,   13,   13,   13,
  342,   11,  355,  356,   11,   11,   11,   11,   11,   11,
   11,   11,   74,   75,  371,  372,   13,  645,   76,   77,
  373,   78,  660,  561,   79,  377,  566,  381,  378,  379,
  382,  383,  102,  102,  102,  102,  385,  392,  386,  397,
   11,   11,   11,   11,   11,   11,  408,   11,  412,   11,
   80,  413,   81,  414,  431,   11,   11,   11,   11,   11,
   11,   11,   11,   11,   11,   11,   11,   11,  429,  358,
   11,  615,  361,  363,  365,  617,  375,  437,  441,  448,
  455,  457,  460,  556,  608,  461,  466,  462,  442,  449,
   74,   75,  484,   12,   12,  490,   76,   77,  476,   78,
   12,   12,   79,  479,  480,  500,   12,   12,  492,   12,
  102,  102,   12,  503,  504,   12,   12,   12,   12,   12,
  508,  505,  537,  538,  540,  541,  338,  339,  340,  341,
   74,   75,  535,   80,  137,   81,   76,   77,  542,   78,
   12,  543,   79,   12,   12,   12,   12,   12,   12,   12,
   12,  544,  557,  190,  545,  546,  550,   74,   75,  196,
  390,  558,  559,   76,   77,  567,   78,  133,  568,   79,
  579,  569,  571,  574,  578,  137,  580,  596,  137,   12,
   12,   12,   12,   12,   12,  600,   12,  603,   12,  343,
  199,  200,  201,  137,   12,   12,   12,   12,   12,   12,
   12,   12,   12,   12,   12,   12,   12,  607,  133,   12,
  604,  133,  613,  618,  621,  611,   13,   13,  619,  622,
  630,  625,  628,   13,   13,  636,  133,  137,  631,   13,
   13,  635,   13,  637,   80,   13,   81,  639,   13,   13,
   13,   13,   13,  644,  643,  658,  667,   54,   10,   79,
   72,   52,   60,  230,  375,   73,   64,  316,   53,  137,
  133,   64,   99,   13,  107,  108,   13,   13,   13,   13,
   13,   13,   13,   13,  338,  339,  340,  341,   74,   75,
  375,   61,  375,   72,   76,   77,  230,   78,   73,  106,
   79,  107,  133,  493,  230,  108,  230,  230,  230,  598,
  230,  190,   13,   13,   13,   13,   13,   13,  342,   13,
  499,   13,  230,  230,  230,  230,  614,   13,   13,   13,
   13,   13,   13,   13,   13,   13,   13,   13,   13,   13,
  316,  150,   13,  151,  314,  416,  513,  343,  199,  200,
  201,  232,  418,  564,  230,  137,  230,  308,  318,  421,
  344,  345,  327,  394,  353,  624,  597,  666,  663,  472,
  612,   74,   75,  473,    0,    0,    0,   76,   77,   80,
   78,   81,  573,   79,  232,  474,    0,  230,  230,  475,
    0,    0,  232,    0,  232,  232,  232,    0,  232,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  232,  232,  232,  232,  137,  137,  137,  137,  137,  137,
    0,    0,    0,  137,    0,  137,  137,    0,    0,  137,
    0,    0,    0,    0,    0,  137,  137,    0,    0,   80,
    0,   81,  232,    0,  232,    0,    0,  133,  133,  133,
  133,  133,  133,  204,    0,    0,  133,    0,  133,  133,
    0,  616,  133,    0,    0,    0,    0,    0,    0,  133,
    0,    0,   74,   75,    0,  232,  232,    0,   76,   77,
    0,   78,    0,    0,   79,    0,  204,    0,    0,    0,
    0,    0,  137,  137,  204,    0,  204,  204,  204,    0,
  204,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  204,  204,  204,  204,    0,    0,    0,  620,
    0,    0,    0,    0,    0,  133,  133,    0,    0,    0,
    0,    0,    0,  230,  230,  230,  230,  230,  230,    0,
    0,  206,  230,    0,  230,  230,  204,    0,  230,    0,
    0,    0,    0,    0,  230,  230,  230,  230,  230,  230,
  230,  230,  230,  230,  230,  230,  230,  230,  230,  230,
  230,  230,  230,  230,  206,    0,    0,  204,  204,    0,
    0,    0,  206,    0,  206,  206,  206,    0,  206,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  206,  206,  206,  206,    0,    0,    0,   74,   75,    0,
    0,  230,  230,   76,   77,    0,   78,  230,    0,   79,
    0,  232,  232,  232,  232,  232,  232,    0,    0,    0,
  232,    0,  232,  232,  206,    0,  232,    0,    0,    0,
    0,  230,  232,  232,  232,  232,  232,  232,  232,  232,
  232,  232,  232,  232,  232,  232,  232,  232,  232,  232,
  232,  232,    0,    0,    0,  206,  206,   74,   75,    0,
    0,    0,    0,   76,   77,    0,   78,    0,    0,   79,
    1,    2,    3,    4,    5,    6,    7,    8,    9,   10,
   11,   12,   13,   14,   15,   16,   17,    0,    0,  232,
  232,    0,    0,    0,    0,  232,    2,    3,    4,    5,
    6,    7,    8,    9,   10,   11,   12,   13,   14,   15,
   16,   17,    0,  204,  204,  204,  204,  204,  204,  232,
    0,  235,  204,    0,  204,  204,    0,    0,  204,    0,
    0,    0,    0,    0,  204,  204,  204,  204,  204,  204,
  204,  204,  204,  204,  204,  204,  204,  204,  204,  204,
  204,  204,  204,  204,  235,    0,    0,    0,    0,    0,
    0,    0,  235,    0,  235,  235,  235,    0,  235,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  235,  235,  235,  235,    0,    0,    0,    0,    0,    0,
    0,  204,  204,    0,    0,    0,    0,  204,    0,    0,
    0,  206,  206,  206,  206,  206,  206,    0,    0,    0,
  206,  330,  206,  206,  235,    0,  206,    0,    0,    0,
    0,  204,  206,  206,  206,  206,  206,  206,  206,  206,
  206,  206,  206,  206,  206,  206,  206,  206,  206,  206,
  206,  206,    0,    0,  330,  235,  235,    0,    0,    0,
    0,    0,  330,    0,  330,  330,  330,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  330,
  330,  330,  330,  330,  338,  339,  340,  341,    0,  206,
  206,    0,  134,    0,    0,  206,    3,    4,    5,    6,
    7,    8,    9,   10,   11,   12,   13,   14,   15,   16,
   17,  190,    0,    0,  330,    0,    0,    0,  342,  206,
    0,  205,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  134,    0,    0,  134,    0,    0,    0,
    0,    0,    0,    0,  330,  330,  330,  343,  199,  200,
  201,  134,    0,    0,  205,    0,    0,    0,    0,    0,
  344,  345,  205,    0,  205,  205,  205,    0,  205,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  205,  205,  205,  205,    0,  134,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  235,  235,  235,  235,  235,  235,    0,    0,    0,
  235,  207,  235,  235,  205,    0,  235,  134,    0,    0,
    0,    0,  235,  235,  235,  235,  235,  235,  235,  235,
  235,  235,  235,  235,  235,  235,  235,  235,  235,  235,
  235,  235,    0,    0,  207,  205,  205,    0,    0,    0,
    0,    0,  207,    0,  207,  207,  207,    0,  207,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  207,  207,  207,  207,    0,    0,    0,    0,    0,  235,
  235,    0,    0,    0,    0,  235,    0,  330,  330,    0,
  330,  330,  330,  330,  330,  330,  330,    0,    0,    0,
  330,    0,  330,  330,  207,    0,  330,    0,    0,  235,
    0,    0,  330,  330,  330,  330,  330,  330,  330,  330,
  330,  330,    0,    0,    0,  330,  330,  330,  330,  330,
  330,  330,    0,    0,    0,  207,  207,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  134,  134,  134,  134,  134,  134,    0,  330,
  330,  134,    0,  134,  134,    0,    0,  134,    0,    0,
    0,    0,    0,    0,  134,    0,    0,    0,    0,    0,
    0,  205,  205,  205,  205,  205,  205,    0,    0,  330,
  205,  236,  205,  205,    0,    0,  205,    0,    0,    0,
    0,    0,  205,  205,  205,  205,  205,  205,  205,  205,
  205,  205,  205,  205,  205,  205,  205,  205,  205,  205,
  205,  205,    0,    0,  236,    0,    0,    0,    0,    0,
  134,  134,  236,    0,  236,  236,  236,    0,  236,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  236,  236,  236,  236,    0,    0,    0,    0,    0,  205,
  205,    0,    0,    0,    0,  205,    0,    0,    0,    0,
    0,  207,  207,  207,  207,  207,  207,    0,    0,  195,
  207,    0,  207,  207,  236,    0,  207,    0,    0,  205,
    0,    0,  207,  207,  207,  207,  207,  207,  207,  207,
  207,  207,  207,  207,  207,  207,  207,  207,  207,  207,
  207,  207,  195,    0,    0,  236,  236,    0,    0,    0,
  195,    0,  195,  195,  195,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  195,  195,
  195,  195,    0,    0,    0,    0,    0,    0,    0,  207,
  207,    0,    0,    0,    0,  207,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  195,    0,    0,    0,    0,    0,    0,  207,
    0,  198,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  195,  195,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  198,    0,    0,    0,    0,    0,
    0,    0,  198,    0,  198,  198,  198,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  198,  198,  198,  198,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  236,  236,  236,  236,  236,  236,    0,    0,  197,
  236,    0,  236,  236,  198,    0,  236,    0,    0,    0,
    0,    0,  236,  236,  236,  236,  236,  236,  236,  236,
  236,  236,  236,  236,  236,  236,  236,  236,  236,  236,
  236,  236,  197,    0,    0,  198,  198,    0,    0,    0,
  197,    0,  197,  197,  197,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  197,  197,
  197,  197,    0,    0,    0,    0,    0,    0,    0,  236,
  236,    0,    0,    0,    0,  236,    0,    0,    0,  195,
  195,  195,  195,  195,  195,    0,    0,    0,  195,    0,
  195,  195,  197,    0,  195,    0,    0,    0,    0,  236,
  195,  195,  195,  195,  195,  195,  195,  195,  195,  195,
  195,  195,  195,  195,  195,  195,  195,  195,  195,  195,
    0,    0,    0,  197,  197,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  195,  195,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  198,  198,  198,  198,  198,  198,  195,    0,  196,
  198,    0,  198,  198,    0,    0,  198,    0,    0,    0,
    0,    0,  198,  198,  198,  198,  198,  198,  198,  198,
  198,  198,  198,  198,  198,  198,  198,  198,  198,  198,
  198,  198,  196,    0,    0,    0,    0,    0,    0,    0,
  196,    0,  196,  196,  196,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  196,  196,
  196,  196,    0,    0,    0,    0,    0,    0,    0,  198,
  198,    0,    0,    0,    0,    0,    0,    0,    0,  197,
  197,  197,  197,  197,  197,    0,    0,  163,  197,    0,
  197,  197,  196,    0,  197,    0,    0,    0,    0,  198,
  197,  197,  197,  197,  197,  197,  197,  197,  197,  197,
  197,  197,  197,  197,  197,  197,  197,  197,  197,  197,
  163,    0,    0,  196,  196,    0,    0,    0,  163,    0,
  163,  163,  163,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  163,  163,  163,  163,
    0,    0,    0,    0,    0,    0,    0,  197,  197,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  163,    0,    0,    0,    0,    0,    0,  197,    0,  327,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  163,  163,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  327,    0,    0,    0,    0,    0,    0,    0,
  327,    0,  327,  327,  327,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  327,  327,
  327,  327,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  196,
  196,  196,  196,  196,  196,    0,    0,  161,  196,    0,
  196,  196,  327,    0,  196,    0,    0,    0,    0,    0,
  196,  196,  196,  196,  196,  196,  196,  196,  196,  196,
  196,  196,  196,  196,  196,  196,  196,  196,  196,  196,
  161,    0,    0,  327,  327,    0,    0,    0,  161,    0,
  161,  161,  161,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  161,  161,  161,  161,
    0,    0,    0,    0,    0,    0,    0,  196,  196,    0,
    0,    0,    0,    0,    0,    0,    0,  163,  163,  163,
  163,  163,  163,    0,    0,    0,  163,    0,  163,  163,
  161,    0,  163,    0,    0,    0,    0,  196,  163,  163,
  163,  163,  163,  163,  163,  163,  163,  163,  163,  163,
    0,  163,  163,  163,  163,  163,  163,  163,    0,    0,
    0,  161,  161,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  163,  163,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  327,
  327,  327,  327,  327,  327,  163,    0,  159,  327,    0,
  327,  327,    0,    0,  327,    0,    0,    0,    0,    0,
  327,  327,  327,  327,  327,  327,  327,  327,  327,  327,
  327,  327,    0,  327,  327,  327,  327,  327,  327,  327,
  159,    0,    0,    0,    0,    0,    0,    0,  159,    0,
  159,  159,  159,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  159,  159,  159,  159,
    0,    0,    0,    0,    0,    0,    0,  327,  327,    0,
    0,    0,    0,    0,    0,    0,    0,  161,  161,  161,
  161,  161,  161,    0,    0,  157,  161,    0,  161,  161,
  159,    0,  161,    0,    0,    0,    0,  327,  161,  161,
  161,  161,  161,  161,  161,  161,  161,  161,  161,    0,
    0,  161,  161,  161,  161,  161,  161,  161,  157,    0,
    0,  159,  159,    0,    0,    0,  157,    0,  157,  157,
  157,  151,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  157,  157,  157,  157,    0,    0,
    0,    0,    0,    0,    0,  161,  161,    0,    0,    0,
    0,    0,    0,    0,  151,    0,    0,    0,    0,    0,
    0,    0,  151,    0,  151,  151,  151,    0,  157,    0,
    0,    0,    0,    0,    0,  161,    0,    0,    0,    0,
  151,  151,  151,  151,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  157,
  157,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  151,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  151,  151,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  159,  159,  159,
  159,  159,  159,    0,    0,  152,  159,    0,  159,  159,
    0,    0,  159,    0,    0,    0,    0,    0,  159,  159,
  159,  159,  159,  159,  159,  159,  159,  159,    0,    0,
    0,  159,  159,  159,  159,  159,  159,  159,  152,    0,
    0,    0,    0,    0,    0,    0,  152,    0,  152,  152,
  152,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  152,  152,  152,  152,    0,    0,
    0,    0,    0,    0,    0,  159,  159,    0,    0,    0,
    0,    0,    0,    0,    0,  157,  157,  157,  157,  157,
  157,    0,    0,    0,  157,    0,  157,  157,  152,    0,
  157,    0,    0,    0,    0,  159,  157,  157,  157,  157,
  157,  157,  157,  157,  157,    0,    0,    0,    0,  157,
  157,  157,  157,  157,  157,  157,    0,    0,    0,  152,
  152,  151,  151,  151,  151,  151,  151,    0,    0,  153,
  151,    0,  151,  151,    0,    0,  151,    0,    0,    0,
    0,    0,  151,  151,  151,  151,  151,  151,  151,    0,
    0,    0,    0,  157,  157,  151,  151,  151,  151,  151,
  151,  151,  153,    0,    0,    0,    0,    0,    0,    0,
  153,    0,  153,  153,  153,    0,    0,    0,    0,    0,
    0,    0,    0,  157,    0,    0,    0,    0,  153,  153,
  153,  153,    0,    0,    0,    0,    0,    0,    0,  151,
  151,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  153,    0,    0,    0,    0,    0,    0,  151,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  146,  153,  153,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  152,  152,  152,  152,  152,
  152,    0,    0,    0,  152,    0,  152,  152,    0,    0,
  152,    0,    0,    0,    0,  146,  152,  152,  152,  152,
  152,  152,  152,  146,    0,  146,  146,  146,    0,  152,
  152,  152,  152,  152,  152,  152,    0,    0,    0,    0,
    0,  146,  146,  146,  146,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  152,  152,  146,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  152,    0,    0,    0,  146,    0,    0,
    0,    0,    0,    0,    0,    0,  148,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  153,
  153,  153,  153,  153,  153,    0,    0,    0,  153,    0,
  153,  153,    0,    0,  153,    0,    0,    0,    0,  148,
  153,  153,  153,  153,  153,  153,  153,  148,    0,  148,
  148,  148,    0,  153,  153,  153,  153,  153,  153,  153,
    0,    0,    0,    0,    0,  148,  148,  148,  148,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  153,  153,  148,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  153,    0,  149,
    0,  148,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  146,  146,  146,  146,  146,  146,    0,    0,
    0,  146,    0,  146,  146,    0,    0,  146,    0,    0,
    0,    0,  149,  146,  146,  146,  146,  146,  146,    0,
  149,    0,  149,  149,  149,    0,  146,  146,  146,  146,
  146,  146,  146,    0,    0,    0,    0,    0,  149,  149,
  149,  149,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  143,    0,    0,    0,    0,    0,    0,    0,
  146,  146,  149,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  143,    0,    0,    0,    0,
  146,    0,    0,  143,  149,  143,  143,  143,    0,    0,
    0,    0,    0,  150,    0,    0,    0,    0,    0,    0,
    0,  143,  143,  143,  143,    0,  148,  148,  148,  148,
  148,  148,    0,    0,    0,  148,    0,  148,  148,    0,
    0,  148,    0,    0,    0,    0,  150,  148,  148,  148,
  148,  148,  148,    0,  150,  143,  150,  150,  150,    0,
  148,  148,  148,  148,  148,  148,  148,    0,    0,    0,
    0,    0,  150,  150,  150,  150,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  143,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  148,  148,  150,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  148,    0,  147,    0,  150,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  149,
  149,  149,  149,  149,  149,    0,    0,    0,  149,    0,
  149,  149,    0,    0,  149,    0,    0,    0,    0,  147,
  149,  149,  149,  149,  149,  149,    0,  147,    0,  147,
  147,  147,    0,  149,  149,  149,  149,  149,  149,  149,
    0,    0,    0,    0,    0,  147,  147,  147,  147,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  144,
    0,    0,    0,    0,    0,    0,    0,  149,  149,  147,
    0,    0,  143,  143,  143,  143,  143,  143,    0,    0,
    0,  143,    0,  143,  143,    0,    0,  143,    0,    0,
    0,    0,  144,  143,  143,  143,    0,  149,    0,    0,
  144,  147,  144,  144,  144,    0,  143,  143,  143,  143,
  143,  143,  143,    0,    0,    0,    0,    0,  144,  144,
  144,  144,    0,  150,  150,  150,  150,  150,  150,    0,
    0,  145,  150,    0,  150,  150,    0,    0,  150,    0,
    0,    0,    0,    0,  150,  150,  150,  150,  150,  150,
  143,  143,  144,    0,    0,    0,    0,  150,  150,  150,
  150,  150,  150,  150,  145,    0,    0,    0,    0,    0,
    0,    0,  145,    0,  145,  145,  145,    0,    0,    0,
    0,  141,    0,    0,  144,    0,    0,    0,    0,    0,
  145,  145,  145,  145,    0,    0,    0,    0,    0,    0,
    0,  150,  150,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  141,    0,    0,    0,    0,    0,
    0,    0,  141,    0,  145,  141,    0,    0,    0,    0,
    0,  150,    0,    0,    0,    0,    0,    0,    0,    0,
  141,  141,  141,  141,    0,    0,  147,  147,  147,  147,
  147,  147,    0,    0,    0,  147,  145,  147,  147,    0,
    0,  147,    0,    0,    0,    0,    0,  147,  147,  147,
  147,  147,  147,    0,  141,    0,    0,    0,    0,    0,
  147,  147,  147,  147,  147,  147,  147,    0,    0,    0,
   10,    0,    0,    0,   10,    0,   10,   10,    0,   10,
   10,   10,    0,    0,    0,    0,  141,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   10,    0,
    0,    0,    0,    0,  147,  147,    0,    0,    0,  144,
  144,  144,  144,  144,  144,    0,    0,    0,  144,    0,
  144,  144,    0,    0,  144,    0,    0,    0,    0,    0,
  144,  144,  144,    0,  147,    0,    0,    0,    0,    0,
    0,    0,    0,  144,  144,  144,  144,  144,  144,  144,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  142,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  145,  145,  145,  145,  145,  145,  144,  144,    0,
  145,    0,  145,  145,    0,    0,  145,  142,    0,    0,
    0,    0,  145,  145,  145,  142,    0,    0,  142,    0,
    0,    0,    0,    0,    0,  145,  145,  145,  145,  145,
  145,  145,    0,  142,  142,  142,  142,    0,    0,    0,
    0,  141,  141,  141,  141,  141,  141,    0,    0,    0,
  141,    0,  141,  141,    0,    0,  141,    0,    0,    0,
    0,    0,  141,  141,    0,    0,    0,  142,    0,  145,
  145,    0,    0,    0,    0,  141,  141,  141,  141,  141,
  141,  141,  171,    0,    0,    0,  171,    0,  171,  171,
    0,  171,  171,  171,    0,    0,    0,    0,    0,  142,
    0,    0,    0,    0,    0,    0,    0,    0,   10,   10,
  171,    0,    0,    0,    0,   10,   10,    0,    0,  141,
  141,   10,   10,    0,   10,    0,    0,   10,    0,    0,
   10,   10,   10,   10,   10,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   10,    0,    0,   10,   10,
   10,   10,   10,   10,   10,   10,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  173,    0,
    0,    0,  173,    0,  173,  173,    0,  173,  173,  173,
    0,    0,    0,    0,   10,   10,   10,   10,   10,   10,
    0,   10,    0,   10,    0,    0,  173,    0,    0,   10,
   10,   10,   10,   10,   10,   10,   10,   10,   10,   10,
   10,   10,    0,    0,   10,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  142,  142,  142,  142,  142,  142,
    0,    0,    0,  142,    0,  142,  142,    0,    0,  142,
    0,    0,    0,    0,    0,  142,  142,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  142,  142,
  142,  142,  142,  142,  142,    0,    0,    0,    0,    0,
    0,    0,  220,    0,    0,    0,  221,    0,  224,    0,
    0,    0,  225,  222,    0,    0,    0,    0,    0,    0,
  171,  171,    0,    0,    0,    0,    0,    0,    0,    0,
  223,    0,  142,  142,    0,    0,    0,    0,    0,    0,
    0,    0,  171,  171,  171,  171,  171,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  171,    0,    0,
  171,  171,  171,  171,  171,  171,  171,  171,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  171,  171,  171,  171,
  171,  171,    0,  171,    0,  171,  173,  173,    0,    0,
    0,  171,  171,  171,  171,  171,  171,  171,  171,  171,
  171,  171,  171,  171,    0,    0,  171,    0,  173,  173,
  173,  173,  173,    0,    0,    0,  166,    0,    0,    0,
  166,    0,  166,    0,    0,    0,  166,  166,    0,    0,
    0,    0,    0,  173,    0,    0,  173,  173,  173,  173,
  173,  173,  173,  173,  166,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  173,  173,  173,  173,  173,  173,    0,  173,
    0,  173,    0,    0,    0,    0,    0,  173,  173,  173,
  173,  173,  173,  173,  173,  173,  173,  173,  173,  173,
  182,  183,  173,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  184,  185,  186,  187,  188,    0,    0,    0,
  220,    0,    0,    0,  221,    0,  224,    0,    0,    0,
  225,    0,    0,    0,    0,    0,    0,  189,    0,    0,
  190,  191,  192,  193,  194,  195,  196,  197,  223,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  198,  199,  200,  201,
  202,  203,    0,  204,    0,  205,    0,    0,    0,    0,
    0,  206,  207,  208,  209,  210,  211,  212,  213,  214,
  215,  216,  217,  218,    0,    0,  219,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  166,  166,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  166,  166,  166,  166,
  166,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  166,    0,    0,  166,  166,  166,  166,  166,  166,
  166,  166,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  166,  166,  166,  166,  166,  166,    0,  166,    0,  166,
    0,    0,    0,    0,    0,  166,  166,  166,  166,  166,
  166,  166,  166,  166,  166,  166,  166,  166,  182,  183,
  166,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  184,  185,  186,  187,  188,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  190,  191,
  192,  193,  194,  195,  196,  197,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  198,  199,  200,  201,    0,  203,
    0,  204,    0,    0,    0,    0,    0,    0,    0,  206,
  207,  208,  209,  210,  211,  212,  213,  214,  215,  216,
  217,  218,    0,    0,  219,
  };
  protected static  short [] yyCheck = {            22,
  142,    4,  144,  145,  146,  147,   41,  178,  179,  179,
   13,   14,   34,   34,   34,   40,   40,   47,  123,  123,
   47,  125,   40,  123,   41,  125,   59,   41,   42,   44,
   41,   42,   62,   43,  123,   45,  125,   44,   44,   44,
   36,    0,   44,  282,  123,   48,  124,  123,   27,   58,
   58,  123,  413,   56,   57,  546,  294,  295,  296,  263,
   25,  263,  276,   60,   40,   44,  548,  316,   40,  324,
  237,  263,  202,   44,   44,   44,   44,   44,   44,  246,
   44,  263,   41,   44,   44,   44,   44,   44,   44,   44,
   44,   44,  222,  345,  346,   41,   44,   44,  123,  123,
   59,  270,  123,  358,  125,  123,  264,  265,  398,  591,
  400,   60,  115,  403,  316,  138,  139,   63,  121,  290,
  290,   33,  270,  271,  272,  273,  123,  618,  125,  108,
  109,  345,  346,  297,   93,  299,  320,  301,   93,  337,
  378,  320,  107,  347,  342,  636,  344,  123,   60,   61,
   62,  123,  643,  321,  125,  125,  125,  125,  125,  125,
  302,  125,  304,  316,  125,  125,  125,  125,  125,  125,
    0,  125,  125,  315,  123,  317,  125,  125,  125,  348,
  349,  223,   47,  258,   41,  260,   43,  266,   45,  268,
  266,   36,   47,   47,  555,  321,  238,   62,  221,  241,
  348,  349,   41,  258,  259,   44,   36,   62,   62,  258,
   40,  260,   42,   43,   61,   45,   46,   47,   43,  297,
   45,  325,  326,  327,  257,  325,  326,  327,   59,   41,
   41,  373,   44,   44,   64,  320,  325,  326,  327,  320,
  312,  313,  640,  641,  170,  171,  172,  173,  298,  299,
  648,  490,  287,   61,  359,  270,  654,  266,  266,   43,
   41,   45,   41,   44,   43,  270,   45,   41,  270,  267,
   44,  269,  279,  279,  270,  379,   41,  261,  262,   44,
  283,  385,  263,  293,  267,  385,  322,  429,  316,  431,
  270,  271,  272,  273,  324,  384,  321,  321,   44,  316,
  279,  280,  320,  320,  325,  326,  327,  321,   40,    0,
  321,  270,  271,  272,  273,  274,  275,  352,   41,   36,
  279,   44,  281,  282,  495,  495,  285,  354,  325,  326,
  327,  354,  355,  356,  505,  505,  359,   61,   62,  362,
   36,  364,   43,  366,   45,   36,  369,   60,   61,   40,
  492,   42,   43,  376,   45,   46,   47,  379,  355,  379,
  357,   42,  504,  384,  361,   36,  508,   40,  348,  349,
  393,  274,  275,   64,  397,   40,  325,  326,  327,  164,
  165,  166,   43,  380,   45,   44,  381,  382,  383,  348,
  349,  533,  304,  305,  306,  307,  308,  309,  310,  281,
  282,  168,  169,  174,  175,  408,  355,  292,  357,   43,
  291,   45,  361,  300,  271,  272,  558,  559,   80,   81,
  277,  278,  301,  280,  125,  448,  283,  176,  177,  180,
  181,  380,    0,  302,  264,  265,  303,  460,  316,   61,
  267,  271,  272,  466,  395,  396,   61,  277,  278,  268,
  280,   36,  322,  283,  625,  625,  286,  287,  288,  289,
  290,   44,  604,  605,  125,  607,  322,   44,   36,  322,
  270,   61,   40,  496,   42,   43,  123,   45,   46,   47,
  321,  311,  123,  123,  314,  315,  316,  317,  318,  319,
  320,  321,  271,  272,   40,   58,   64,  639,  277,  278,
   40,  280,  644,   36,  283,   40,  509,  316,   40,   40,
  316,  324,  270,  271,  272,  273,  321,   58,  322,   91,
  350,  351,  352,  353,  354,  355,   44,  357,  322,  359,
   43,   41,   45,   44,  270,  365,  366,  367,  368,  369,
  370,  371,  372,  373,  374,  375,  376,  377,   44,   40,
  380,  574,   40,   40,   40,  578,   40,   63,  123,  123,
   41,   41,  123,  586,  567,   41,  123,   42,  321,  321,
  271,  272,  320,  264,  265,  267,  277,  278,  356,  280,
  271,  272,  283,  321,  324,  267,  277,  278,   61,  280,
  348,  349,  283,   58,  269,  286,  287,  288,  289,  290,
  284,  281,   41,   41,   41,   41,  287,  288,  289,  290,
  271,  272,  125,   43,    0,   45,  277,  278,   41,  280,
  311,   41,  283,  314,  315,  316,  317,  318,  319,  320,
  321,  316,   36,  314,  323,  321,  324,  271,  272,  320,
  321,  269,   61,  277,  278,  276,  280,    0,  125,  283,
  358,  321,  123,  123,  123,   41,  360,   62,   44,  350,
  351,  352,  353,  354,  355,  322,  357,  322,  359,  350,
  351,  352,  353,   59,  365,  366,  367,  368,  369,  370,
  371,  372,  373,  374,  375,  376,  377,  285,   41,  380,
  270,   44,   41,   61,  125,  125,  264,  265,  316,   47,
   41,  268,  322,  271,  272,  321,   59,   93,  125,  277,
  278,  362,  280,   47,   43,  283,   45,  270,  286,  287,
  288,  289,  290,  270,  321,   62,   62,  316,    0,    0,
   41,   59,   59,    0,  321,   41,   58,  125,   59,  125,
   93,  269,  269,  311,  270,  270,  314,  315,  316,  317,
  318,  319,  320,  321,  287,  288,  289,  290,  271,  272,
   61,   59,   62,   19,  277,  278,   33,  280,   19,   23,
  283,   24,  125,  412,   41,   24,   43,   44,   45,  555,
   47,  314,  350,  351,  352,  353,  354,  355,  321,  357,
  414,  359,   59,   60,   61,   62,  125,  365,  366,  367,
  368,  369,  370,  371,  372,  373,  374,  375,  376,  377,
  148,   90,  380,   90,  147,  298,  429,  350,  351,  352,
  353,    0,  300,  506,   91,   77,   93,  143,  152,  303,
  363,  364,  167,  230,  181,  591,  550,  654,  648,  377,
  571,  271,  272,  377,   -1,   -1,   -1,  277,  278,   43,
  280,   45,  526,  283,   33,  378,   -1,  124,  125,  379,
   -1,   -1,   41,   -1,   43,   44,   45,   -1,   47,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   59,   60,   61,   62,  270,  271,  272,  273,  274,  275,
   -1,   -1,   -1,  279,   -1,  281,  282,   -1,   -1,  285,
   -1,   -1,   -1,   -1,   -1,  291,  292,   -1,   -1,   43,
   -1,   45,   91,   -1,   93,   -1,   -1,  270,  271,  272,
  273,  274,  275,    0,   -1,   -1,  279,   -1,  281,  282,
   -1,  125,  285,   -1,   -1,   -1,   -1,   -1,   -1,  292,
   -1,   -1,  271,  272,   -1,  124,  125,   -1,  277,  278,
   -1,  280,   -1,   -1,  283,   -1,   33,   -1,   -1,   -1,
   -1,   -1,  348,  349,   41,   -1,   43,   44,   45,   -1,
   47,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   59,   60,   61,   62,   -1,   -1,   -1,  123,
   -1,   -1,   -1,   -1,   -1,  348,  349,   -1,   -1,   -1,
   -1,   -1,   -1,  270,  271,  272,  273,  274,  275,   -1,
   -1,    0,  279,   -1,  281,  282,   93,   -1,  285,   -1,
   -1,   -1,   -1,   -1,  291,  292,  293,  294,  295,  296,
  297,  298,  299,  300,  301,  302,  303,  304,  305,  306,
  307,  308,  309,  310,   33,   -1,   -1,  124,  125,   -1,
   -1,   -1,   41,   -1,   43,   44,   45,   -1,   47,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   59,   60,   61,   62,   -1,   -1,   -1,  271,  272,   -1,
   -1,  348,  349,  277,  278,   -1,  280,  354,   -1,  283,
   -1,  270,  271,  272,  273,  274,  275,   -1,   -1,   -1,
  279,   -1,  281,  282,   93,   -1,  285,   -1,   -1,   -1,
   -1,  378,  291,  292,  293,  294,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,  305,  306,  307,  308,
  309,  310,   -1,   -1,   -1,  124,  125,  271,  272,   -1,
   -1,   -1,   -1,  277,  278,   -1,  280,   -1,   -1,  283,
  328,  329,  330,  331,  332,  333,  334,  335,  336,  337,
  338,  339,  340,  341,  342,  343,  344,   -1,   -1,  348,
  349,   -1,   -1,   -1,   -1,  354,  329,  330,  331,  332,
  333,  334,  335,  336,  337,  338,  339,  340,  341,  342,
  343,  344,   -1,  270,  271,  272,  273,  274,  275,  378,
   -1,    0,  279,   -1,  281,  282,   -1,   -1,  285,   -1,
   -1,   -1,   -1,   -1,  291,  292,  293,  294,  295,  296,
  297,  298,  299,  300,  301,  302,  303,  304,  305,  306,
  307,  308,  309,  310,   33,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   41,   -1,   43,   44,   45,   -1,   47,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   59,   60,   61,   62,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  348,  349,   -1,   -1,   -1,   -1,  354,   -1,   -1,
   -1,  270,  271,  272,  273,  274,  275,   -1,   -1,   -1,
  279,    0,  281,  282,   93,   -1,  285,   -1,   -1,   -1,
   -1,  378,  291,  292,  293,  294,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,  305,  306,  307,  308,
  309,  310,   -1,   -1,   33,  124,  125,   -1,   -1,   -1,
   -1,   -1,   41,   -1,   43,   44,   45,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   58,
   59,   60,   61,   62,  287,  288,  289,  290,   -1,  348,
  349,   -1,    0,   -1,   -1,  354,  330,  331,  332,  333,
  334,  335,  336,  337,  338,  339,  340,  341,  342,  343,
  344,  314,   -1,   -1,   93,   -1,   -1,   -1,  321,  378,
   -1,    0,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   41,   -1,   -1,   44,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  123,  124,  125,  350,  351,  352,
  353,   59,   -1,   -1,   33,   -1,   -1,   -1,   -1,   -1,
  363,  364,   41,   -1,   43,   44,   45,   -1,   47,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   59,   60,   61,   62,   -1,   93,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  270,  271,  272,  273,  274,  275,   -1,   -1,   -1,
  279,    0,  281,  282,   93,   -1,  285,  125,   -1,   -1,
   -1,   -1,  291,  292,  293,  294,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,  305,  306,  307,  308,
  309,  310,   -1,   -1,   33,  124,  125,   -1,   -1,   -1,
   -1,   -1,   41,   -1,   43,   44,   45,   -1,   47,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   59,   60,   61,   62,   -1,   -1,   -1,   -1,   -1,  348,
  349,   -1,   -1,   -1,   -1,  354,   -1,  266,  267,   -1,
  269,  270,  271,  272,  273,  274,  275,   -1,   -1,   -1,
  279,   -1,  281,  282,   93,   -1,  285,   -1,   -1,  378,
   -1,   -1,  291,  292,  293,  294,  295,  296,  297,  298,
  299,  300,   -1,   -1,   -1,  304,  305,  306,  307,  308,
  309,  310,   -1,   -1,   -1,  124,  125,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  270,  271,  272,  273,  274,  275,   -1,  348,
  349,  279,   -1,  281,  282,   -1,   -1,  285,   -1,   -1,
   -1,   -1,   -1,   -1,  292,   -1,   -1,   -1,   -1,   -1,
   -1,  270,  271,  272,  273,  274,  275,   -1,   -1,  378,
  279,    0,  281,  282,   -1,   -1,  285,   -1,   -1,   -1,
   -1,   -1,  291,  292,  293,  294,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,  305,  306,  307,  308,
  309,  310,   -1,   -1,   33,   -1,   -1,   -1,   -1,   -1,
  348,  349,   41,   -1,   43,   44,   45,   -1,   47,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   59,   60,   61,   62,   -1,   -1,   -1,   -1,   -1,  348,
  349,   -1,   -1,   -1,   -1,  354,   -1,   -1,   -1,   -1,
   -1,  270,  271,  272,  273,  274,  275,   -1,   -1,    0,
  279,   -1,  281,  282,   93,   -1,  285,   -1,   -1,  378,
   -1,   -1,  291,  292,  293,  294,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,  305,  306,  307,  308,
  309,  310,   33,   -1,   -1,  124,  125,   -1,   -1,   -1,
   41,   -1,   43,   44,   45,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   59,   60,
   61,   62,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  348,
  349,   -1,   -1,   -1,   -1,  354,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   93,   -1,   -1,   -1,   -1,   -1,   -1,  378,
   -1,    0,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  124,  125,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   33,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   41,   -1,   43,   44,   45,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   59,   60,   61,   62,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  270,  271,  272,  273,  274,  275,   -1,   -1,    0,
  279,   -1,  281,  282,   93,   -1,  285,   -1,   -1,   -1,
   -1,   -1,  291,  292,  293,  294,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,  305,  306,  307,  308,
  309,  310,   33,   -1,   -1,  124,  125,   -1,   -1,   -1,
   41,   -1,   43,   44,   45,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   59,   60,
   61,   62,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  348,
  349,   -1,   -1,   -1,   -1,  354,   -1,   -1,   -1,  270,
  271,  272,  273,  274,  275,   -1,   -1,   -1,  279,   -1,
  281,  282,   93,   -1,  285,   -1,   -1,   -1,   -1,  378,
  291,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,  303,  304,  305,  306,  307,  308,  309,  310,
   -1,   -1,   -1,  124,  125,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  348,  349,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  270,  271,  272,  273,  274,  275,  378,   -1,    0,
  279,   -1,  281,  282,   -1,   -1,  285,   -1,   -1,   -1,
   -1,   -1,  291,  292,  293,  294,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,  305,  306,  307,  308,
  309,  310,   33,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   41,   -1,   43,   44,   45,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   59,   60,
   61,   62,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  348,
  349,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  270,
  271,  272,  273,  274,  275,   -1,   -1,    0,  279,   -1,
  281,  282,   93,   -1,  285,   -1,   -1,   -1,   -1,  378,
  291,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,  303,  304,  305,  306,  307,  308,  309,  310,
   33,   -1,   -1,  124,  125,   -1,   -1,   -1,   41,   -1,
   43,   44,   45,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   59,   60,   61,   62,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  348,  349,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   93,   -1,   -1,   -1,   -1,   -1,   -1,  378,   -1,    0,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  124,  125,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   33,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   41,   -1,   43,   44,   45,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   59,   60,
   61,   62,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  270,
  271,  272,  273,  274,  275,   -1,   -1,    0,  279,   -1,
  281,  282,   93,   -1,  285,   -1,   -1,   -1,   -1,   -1,
  291,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,  303,  304,  305,  306,  307,  308,  309,  310,
   33,   -1,   -1,  124,  125,   -1,   -1,   -1,   41,   -1,
   43,   44,   45,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   59,   60,   61,   62,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  348,  349,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  270,  271,  272,
  273,  274,  275,   -1,   -1,   -1,  279,   -1,  281,  282,
   93,   -1,  285,   -1,   -1,   -1,   -1,  378,  291,  292,
  293,  294,  295,  296,  297,  298,  299,  300,  301,  302,
   -1,  304,  305,  306,  307,  308,  309,  310,   -1,   -1,
   -1,  124,  125,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  348,  349,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  270,
  271,  272,  273,  274,  275,  378,   -1,    0,  279,   -1,
  281,  282,   -1,   -1,  285,   -1,   -1,   -1,   -1,   -1,
  291,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,   -1,  304,  305,  306,  307,  308,  309,  310,
   33,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   41,   -1,
   43,   44,   45,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   59,   60,   61,   62,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  348,  349,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  270,  271,  272,
  273,  274,  275,   -1,   -1,    0,  279,   -1,  281,  282,
   93,   -1,  285,   -1,   -1,   -1,   -1,  378,  291,  292,
  293,  294,  295,  296,  297,  298,  299,  300,  301,   -1,
   -1,  304,  305,  306,  307,  308,  309,  310,   33,   -1,
   -1,  124,  125,   -1,   -1,   -1,   41,   -1,   43,   44,
   45,    0,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   59,   60,   61,   62,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  348,  349,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   33,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   41,   -1,   43,   44,   45,   -1,   93,   -1,
   -1,   -1,   -1,   -1,   -1,  378,   -1,   -1,   -1,   -1,
   59,   60,   61,   62,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  124,
  125,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   93,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  124,  125,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  270,  271,  272,
  273,  274,  275,   -1,   -1,    0,  279,   -1,  281,  282,
   -1,   -1,  285,   -1,   -1,   -1,   -1,   -1,  291,  292,
  293,  294,  295,  296,  297,  298,  299,  300,   -1,   -1,
   -1,  304,  305,  306,  307,  308,  309,  310,   33,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   41,   -1,   43,   44,
   45,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   59,   60,   61,   62,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  348,  349,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  270,  271,  272,  273,  274,
  275,   -1,   -1,   -1,  279,   -1,  281,  282,   93,   -1,
  285,   -1,   -1,   -1,   -1,  378,  291,  292,  293,  294,
  295,  296,  297,  298,  299,   -1,   -1,   -1,   -1,  304,
  305,  306,  307,  308,  309,  310,   -1,   -1,   -1,  124,
  125,  270,  271,  272,  273,  274,  275,   -1,   -1,    0,
  279,   -1,  281,  282,   -1,   -1,  285,   -1,   -1,   -1,
   -1,   -1,  291,  292,  293,  294,  295,  296,  297,   -1,
   -1,   -1,   -1,  348,  349,  304,  305,  306,  307,  308,
  309,  310,   33,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   41,   -1,   43,   44,   45,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  378,   -1,   -1,   -1,   -1,   59,   60,
   61,   62,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  348,
  349,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   93,   -1,   -1,   -1,   -1,   -1,   -1,  378,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,    0,  124,  125,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  270,  271,  272,  273,  274,
  275,   -1,   -1,   -1,  279,   -1,  281,  282,   -1,   -1,
  285,   -1,   -1,   -1,   -1,   33,  291,  292,  293,  294,
  295,  296,  297,   41,   -1,   43,   44,   45,   -1,  304,
  305,  306,  307,  308,  309,  310,   -1,   -1,   -1,   -1,
   -1,   59,   60,   61,   62,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  348,  349,   93,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  378,   -1,   -1,   -1,  125,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,    0,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  270,
  271,  272,  273,  274,  275,   -1,   -1,   -1,  279,   -1,
  281,  282,   -1,   -1,  285,   -1,   -1,   -1,   -1,   33,
  291,  292,  293,  294,  295,  296,  297,   41,   -1,   43,
   44,   45,   -1,  304,  305,  306,  307,  308,  309,  310,
   -1,   -1,   -1,   -1,   -1,   59,   60,   61,   62,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  348,  349,   93,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  378,   -1,    0,
   -1,  125,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  270,  271,  272,  273,  274,  275,   -1,   -1,
   -1,  279,   -1,  281,  282,   -1,   -1,  285,   -1,   -1,
   -1,   -1,   33,  291,  292,  293,  294,  295,  296,   -1,
   41,   -1,   43,   44,   45,   -1,  304,  305,  306,  307,
  308,  309,  310,   -1,   -1,   -1,   -1,   -1,   59,   60,
   61,   62,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,    0,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  348,  349,   93,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   33,   -1,   -1,   -1,   -1,
  378,   -1,   -1,   41,  125,   43,   44,   45,   -1,   -1,
   -1,   -1,   -1,    0,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   59,   60,   61,   62,   -1,  270,  271,  272,  273,
  274,  275,   -1,   -1,   -1,  279,   -1,  281,  282,   -1,
   -1,  285,   -1,   -1,   -1,   -1,   33,  291,  292,  293,
  294,  295,  296,   -1,   41,   93,   43,   44,   45,   -1,
  304,  305,  306,  307,  308,  309,  310,   -1,   -1,   -1,
   -1,   -1,   59,   60,   61,   62,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  125,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  348,  349,   93,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  378,   -1,    0,   -1,  125,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  270,
  271,  272,  273,  274,  275,   -1,   -1,   -1,  279,   -1,
  281,  282,   -1,   -1,  285,   -1,   -1,   -1,   -1,   33,
  291,  292,  293,  294,  295,  296,   -1,   41,   -1,   43,
   44,   45,   -1,  304,  305,  306,  307,  308,  309,  310,
   -1,   -1,   -1,   -1,   -1,   59,   60,   61,   62,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,    0,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  348,  349,   93,
   -1,   -1,  270,  271,  272,  273,  274,  275,   -1,   -1,
   -1,  279,   -1,  281,  282,   -1,   -1,  285,   -1,   -1,
   -1,   -1,   33,  291,  292,  293,   -1,  378,   -1,   -1,
   41,  125,   43,   44,   45,   -1,  304,  305,  306,  307,
  308,  309,  310,   -1,   -1,   -1,   -1,   -1,   59,   60,
   61,   62,   -1,  270,  271,  272,  273,  274,  275,   -1,
   -1,    0,  279,   -1,  281,  282,   -1,   -1,  285,   -1,
   -1,   -1,   -1,   -1,  291,  292,  293,  294,  295,  296,
  348,  349,   93,   -1,   -1,   -1,   -1,  304,  305,  306,
  307,  308,  309,  310,   33,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   41,   -1,   43,   44,   45,   -1,   -1,   -1,
   -1,    0,   -1,   -1,  125,   -1,   -1,   -1,   -1,   -1,
   59,   60,   61,   62,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  348,  349,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   33,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   41,   -1,   93,   44,   -1,   -1,   -1,   -1,
   -1,  378,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   59,   60,   61,   62,   -1,   -1,  270,  271,  272,  273,
  274,  275,   -1,   -1,   -1,  279,  125,  281,  282,   -1,
   -1,  285,   -1,   -1,   -1,   -1,   -1,  291,  292,  293,
  294,  295,  296,   -1,   93,   -1,   -1,   -1,   -1,   -1,
  304,  305,  306,  307,  308,  309,  310,   -1,   -1,   -1,
   36,   -1,   -1,   -1,   40,   -1,   42,   43,   -1,   45,
   46,   47,   -1,   -1,   -1,   -1,  125,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   64,   -1,
   -1,   -1,   -1,   -1,  348,  349,   -1,   -1,   -1,  270,
  271,  272,  273,  274,  275,   -1,   -1,   -1,  279,   -1,
  281,  282,   -1,   -1,  285,   -1,   -1,   -1,   -1,   -1,
  291,  292,  293,   -1,  378,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  304,  305,  306,  307,  308,  309,  310,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,    0,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  270,  271,  272,  273,  274,  275,  348,  349,   -1,
  279,   -1,  281,  282,   -1,   -1,  285,   33,   -1,   -1,
   -1,   -1,  291,  292,  293,   41,   -1,   -1,   44,   -1,
   -1,   -1,   -1,   -1,   -1,  304,  305,  306,  307,  308,
  309,  310,   -1,   59,   60,   61,   62,   -1,   -1,   -1,
   -1,  270,  271,  272,  273,  274,  275,   -1,   -1,   -1,
  279,   -1,  281,  282,   -1,   -1,  285,   -1,   -1,   -1,
   -1,   -1,  291,  292,   -1,   -1,   -1,   93,   -1,  348,
  349,   -1,   -1,   -1,   -1,  304,  305,  306,  307,  308,
  309,  310,   36,   -1,   -1,   -1,   40,   -1,   42,   43,
   -1,   45,   46,   47,   -1,   -1,   -1,   -1,   -1,  125,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  264,  265,
   64,   -1,   -1,   -1,   -1,  271,  272,   -1,   -1,  348,
  349,  277,  278,   -1,  280,   -1,   -1,  283,   -1,   -1,
  286,  287,  288,  289,  290,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  311,   -1,   -1,  314,  315,
  316,  317,  318,  319,  320,  321,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   36,   -1,
   -1,   -1,   40,   -1,   42,   43,   -1,   45,   46,   47,
   -1,   -1,   -1,   -1,  350,  351,  352,  353,  354,  355,
   -1,  357,   -1,  359,   -1,   -1,   64,   -1,   -1,  365,
  366,  367,  368,  369,  370,  371,  372,  373,  374,  375,
  376,  377,   -1,   -1,  380,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  270,  271,  272,  273,  274,  275,
   -1,   -1,   -1,  279,   -1,  281,  282,   -1,   -1,  285,
   -1,   -1,   -1,   -1,   -1,  291,  292,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  304,  305,
  306,  307,  308,  309,  310,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   36,   -1,   -1,   -1,   40,   -1,   42,   -1,
   -1,   -1,   46,   47,   -1,   -1,   -1,   -1,   -1,   -1,
  264,  265,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   64,   -1,  348,  349,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  286,  287,  288,  289,  290,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  311,   -1,   -1,
  314,  315,  316,  317,  318,  319,  320,  321,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  350,  351,  352,  353,
  354,  355,   -1,  357,   -1,  359,  264,  265,   -1,   -1,
   -1,  365,  366,  367,  368,  369,  370,  371,  372,  373,
  374,  375,  376,  377,   -1,   -1,  380,   -1,  286,  287,
  288,  289,  290,   -1,   -1,   -1,   36,   -1,   -1,   -1,
   40,   -1,   42,   -1,   -1,   -1,   46,   47,   -1,   -1,
   -1,   -1,   -1,  311,   -1,   -1,  314,  315,  316,  317,
  318,  319,  320,  321,   64,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  350,  351,  352,  353,  354,  355,   -1,  357,
   -1,  359,   -1,   -1,   -1,   -1,   -1,  365,  366,  367,
  368,  369,  370,  371,  372,  373,  374,  375,  376,  377,
  264,  265,  380,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  286,  287,  288,  289,  290,   -1,   -1,   -1,
   36,   -1,   -1,   -1,   40,   -1,   42,   -1,   -1,   -1,
   46,   -1,   -1,   -1,   -1,   -1,   -1,  311,   -1,   -1,
  314,  315,  316,  317,  318,  319,  320,  321,   64,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  350,  351,  352,  353,
  354,  355,   -1,  357,   -1,  359,   -1,   -1,   -1,   -1,
   -1,  365,  366,  367,  368,  369,  370,  371,  372,  373,
  374,  375,  376,  377,   -1,   -1,  380,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  264,  265,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  286,  287,  288,  289,
  290,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  311,   -1,   -1,  314,  315,  316,  317,  318,  319,
  320,  321,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  350,  351,  352,  353,  354,  355,   -1,  357,   -1,  359,
   -1,   -1,   -1,   -1,   -1,  365,  366,  367,  368,  369,
  370,  371,  372,  373,  374,  375,  376,  377,  264,  265,
  380,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  286,  287,  288,  289,  290,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  314,  315,
  316,  317,  318,  319,  320,  321,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  350,  351,  352,  353,   -1,  355,
   -1,  357,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  365,
  366,  367,  368,  369,  370,  371,  372,  373,  374,  375,
  376,  377,   -1,   -1,  380,
  };

#line 1935 "XQuery.y"
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
  public const int ENCODING = 257;
  public const int PRESERVE = 258;
  public const int NO_PRESERVE = 259;
  public const int STRIP = 260;
  public const int INHERIT = 261;
  public const int NO_INHERIT = 262;
  public const int NAMESPACE = 263;
  public const int ORDERED = 264;
  public const int UNORDERED = 265;
  public const int EXTERNAL = 266;
  public const int AT = 267;
  public const int AS = 268;
  public const int IN = 269;
  public const int RETURN = 270;
  public const int FOR = 271;
  public const int LET = 272;
  public const int WHERE = 273;
  public const int ASCENDING = 274;
  public const int DESCENDING = 275;
  public const int COLLATION = 276;
  public const int SOME = 277;
  public const int EVERY = 278;
  public const int SATISFIES = 279;
  public const int TYPESWITCH = 280;
  public const int CASE = 281;
  public const int DEFAULT = 282;
  public const int IF = 283;
  public const int THEN = 284;
  public const int ELSE = 285;
  public const int DOCUMENT = 286;
  public const int ELEMENT = 287;
  public const int ATTRIBUTE = 288;
  public const int TEXT = 289;
  public const int COMMENT = 290;
  public const int AND = 291;
  public const int OR = 292;
  public const int TO = 293;
  public const int DIV = 294;
  public const int IDIV = 295;
  public const int MOD = 296;
  public const int UNION = 297;
  public const int INTERSECT = 298;
  public const int EXCEPT = 299;
  public const int INSTANCE_OF = 300;
  public const int TREAT_AS = 301;
  public const int CASTABLE_AS = 302;
  public const int CAST_AS = 303;
  public const int EQ = 304;
  public const int NE = 305;
  public const int LT = 306;
  public const int GT = 307;
  public const int GE = 308;
  public const int LE = 309;
  public const int IS = 310;
  public const int VALIDATE = 311;
  public const int LAX = 312;
  public const int STRICT = 313;
  public const int NODE = 314;
  public const int DOUBLE_PERIOD = 315;
  public const int StringLiteral = 316;
  public const int IntegerLiteral = 317;
  public const int DecimalLiteral = 318;
  public const int DoubleLiteral = 319;
  public const int NCName = 320;
  public const int QName = 321;
  public const int VarName = 322;
  public const int PragmaContents = 323;
  public const int S = 324;
  public const int Char = 325;
  public const int PredefinedEntityRef = 326;
  public const int CharRef = 327;
  public const int XQUERY_VERSION = 328;
  public const int MODULE_NAMESPACE = 329;
  public const int IMPORT_SCHEMA = 330;
  public const int IMPORT_MODULE = 331;
  public const int DECLARE_NAMESPACE = 332;
  public const int DECLARE_BOUNDARY_SPACE = 333;
  public const int DECLARE_DEFAULT_ELEMENT = 334;
  public const int DECLARE_DEFAULT_FUNCTION = 335;
  public const int DECLARE_DEFAULT_ORDER = 336;
  public const int DECLARE_OPTION = 337;
  public const int DECLARE_ORDERING = 338;
  public const int DECLARE_COPY_NAMESPACES = 339;
  public const int DECLARE_DEFAULT_COLLATION = 340;
  public const int DECLARE_BASE_URI = 341;
  public const int DECLARE_VARIABLE = 342;
  public const int DECLARE_CONSTRUCTION = 343;
  public const int DECLARE_FUNCTION = 344;
  public const int EMPTY_GREATEST = 345;
  public const int EMPTY_LEAST = 346;
  public const int DEFAULT_ELEMENT = 347;
  public const int ORDER_BY = 348;
  public const int STABLE_ORDER_BY = 349;
  public const int PROCESSING_INSTRUCTION = 350;
  public const int DOCUMENT_NODE = 351;
  public const int SCHEMA_ELEMENT = 352;
  public const int SCHEMA_ATTRIBUTE = 353;
  public const int DOUBLE_SLASH = 354;
  public const int COMMENT_BEGIN = 355;
  public const int COMMENT_END = 356;
  public const int PI_BEGIN = 357;
  public const int PI_END = 358;
  public const int PRAGMA_BEGIN = 359;
  public const int PRAGMA_END = 360;
  public const int CDATA_BEGIN = 361;
  public const int CDATA_END = 362;
  public const int VOID = 363;
  public const int ITEM = 364;
  public const int AXIS_CHILD = 365;
  public const int AXIS_DESCENDANT = 366;
  public const int AXIS_ATTRIBUTE = 367;
  public const int AXIS_SELF = 368;
  public const int AXIS_DESCENDANT_OR_SELF = 369;
  public const int AXIS_FOLLOWING_SIBLING = 370;
  public const int AXIS_FOLLOWING = 371;
  public const int AXIS_PARENT = 372;
  public const int AXIS_ANCESTOR = 373;
  public const int AXIS_PRECEDING_SIBLING = 374;
  public const int AXIS_PRECEDING = 375;
  public const int AXIS_ANCESTOR_OR_SELF = 376;
  public const int AXIS_NAMESPACE = 377;
  public const int ML = 378;
  public const int Apos = 379;
  public const int BeginTag = 380;
  public const int Indicator1 = 381;
  public const int Indicator2 = 382;
  public const int Indicator3 = 383;
  public const int EscapeQuot = 384;
  public const int EscapeApos = 385;
  public const int XQComment = 386;
  public const int XQWhitespace = 387;
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
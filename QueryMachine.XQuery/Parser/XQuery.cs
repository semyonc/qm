
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
//t    "ModuleImport : IMPORT_MODULE URILiteral ModuleImportSpec",
//t    "ModuleImport : IMPORT_MODULE NAMESPACE NCName '=' URILiteral ModuleImportSpec",
//t    "ModuleImportSpec :",
//t    "ModuleImportSpec : AT URILiteralList",
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
     yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.ImportModule, yyVals[-1+yyTop], yyVals[0+yyTop]);
  }
  break;
case 61:
#line 491 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.ImportModule, yyVals[-3+yyTop], yyVals[-1+yyTop], yyVals[0+yyTop]);
  }
  break;
case 62:
#line 498 "XQuery.y"
  {
     yyVal = null;
  }
  break;
case 63:
#line 502 "XQuery.y"
  {
     yyVal = yyVals[0+yyTop];
  }
  break;
case 64:
#line 508 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.VarDecl, yyVals[-4+yyTop], yyVals[-3+yyTop], yyVals[0+yyTop]); 
  }
  break;
case 65:
#line 512 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.VarDecl, yyVals[-2+yyTop], yyVals[-1+yyTop]); 
  }
  break;
case 66:
#line 519 "XQuery.y"
  {
     yyVal = null;
  }
  break;
case 68:
#line 527 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.ConstructionDecl, 
		new TokenWrapper(Token.PRESERVE));
  }
  break;
case 69:
#line 532 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.ConstructionDecl, 
		new TokenWrapper(Token.STRIP));
  }
  break;
case 70:
#line 540 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.DeclareFunction, yyVals[-4+yyTop], yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 71:
#line 544 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.DeclareFunction, yyVals[-6+yyTop], yyVals[-4+yyTop], yyVals[-1+yyTop], yyVals[0+yyTop]);
  }
  break;
case 73:
#line 552 "XQuery.y"
  {
     yyVal = null;
  }
  break;
case 74:
#line 559 "XQuery.y"
  {
     yyVal = null;
  }
  break;
case 76:
#line 567 "XQuery.y"
  {
     yyVal = Lisp.Cons(yyVals[0+yyTop]);
  }
  break;
case 77:
#line 571 "XQuery.y"
  {
     yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
  }
  break;
case 78:
#line 578 "XQuery.y"
  {
     yyVal = yyVals[0+yyTop];
  }
  break;
case 79:
#line 582 "XQuery.y"
  {
     yyVal = yyVals[-1+yyTop];
     notation.Confirm((Symbol)yyVals[-1+yyTop], Descriptor.TypeDecl, yyVals[0+yyTop]);
  }
  break;
case 80:
#line 590 "XQuery.y"
  {
     yyVal = yyVals[-1+yyTop];
  }
  break;
case 82:
#line 601 "XQuery.y"
  {
     yyVal = Lisp.Cons(yyVals[0+yyTop]);
  }
  break;
case 83:
#line 605 "XQuery.y"
  {
     yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
  }
  break;
case 89:
#line 620 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.FLWORExpr, yyVals[-2+yyTop], null, null, yyVals[0+yyTop]);
  }
  break;
case 90:
#line 624 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.FLWORExpr, yyVals[-3+yyTop], yyVals[-2+yyTop], null, yyVals[0+yyTop]);
  }
  break;
case 91:
#line 628 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.FLWORExpr, yyVals[-3+yyTop], null, yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 92:
#line 632 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.FLWORExpr, yyVals[-4+yyTop], yyVals[-3+yyTop], yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 93:
#line 639 "XQuery.y"
  {
     yyVal = Lisp.Cons(yyVals[0+yyTop]);
  }
  break;
case 94:
#line 643 "XQuery.y"
  {
     yyVal = Lisp.Cons(yyVals[0+yyTop]);
  }
  break;
case 95:
#line 647 "XQuery.y"
  {
     yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(yyVals[0+yyTop]));
  }
  break;
case 96:
#line 651 "XQuery.y"
  {
     yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(yyVals[0+yyTop]));
  }
  break;
case 97:
#line 658 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.For, yyVals[0+yyTop]);
  }
  break;
case 98:
#line 665 "XQuery.y"
  {
     yyVal = Lisp.Cons(yyVals[0+yyTop]);
  }
  break;
case 99:
#line 669 "XQuery.y"
  {
     yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
  }
  break;
case 100:
#line 676 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.ForClauseOperator, yyVals[-4+yyTop], yyVals[-3+yyTop], yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 101:
#line 683 "XQuery.y"
  {
     yyVal = null;
  }
  break;
case 103:
#line 691 "XQuery.y"
  {
     yyVal = yyVals[0+yyTop];
  }
  break;
case 104:
#line 698 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Let, yyVals[0+yyTop]);
  }
  break;
case 105:
#line 705 "XQuery.y"
  {
     yyVal = Lisp.Cons(yyVals[0+yyTop]);
  }
  break;
case 106:
#line 709 "XQuery.y"
  {
     yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
  }
  break;
case 107:
#line 716 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.LetClauseOperator, yyVals[-4+yyTop], yyVals[-3+yyTop], yyVals[0+yyTop]);
  }
  break;
case 108:
#line 723 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Where, yyVals[0+yyTop]);
  }
  break;
case 109:
#line 730 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.OrderBy, yyVals[0+yyTop]);
  }
  break;
case 110:
#line 734 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.StableOrderBy, yyVals[0+yyTop]);
  }
  break;
case 111:
#line 741 "XQuery.y"
  {
     yyVal = Lisp.Cons(yyVals[0+yyTop]);
  }
  break;
case 112:
#line 745 "XQuery.y"
  {
     yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
  }
  break;
case 114:
#line 753 "XQuery.y"
  {
     yyVal = yyVals[-1+yyTop];
     notation.Confirm((Symbol)yyVals[-1+yyTop], Descriptor.Modifier, yyVals[0+yyTop]);
  }
  break;
case 115:
#line 761 "XQuery.y"
  {
     yyVal = Lisp.List(yyVals[0+yyTop], null, null);
  }
  break;
case 116:
#line 765 "XQuery.y"
  {
     yyVal = Lisp.List(yyVals[-1+yyTop], yyVals[0+yyTop], null);
  }
  break;
case 117:
#line 769 "XQuery.y"
  {
     yyVal = Lisp.List(yyVals[-2+yyTop], null, yyVals[0+yyTop]);
  }
  break;
case 118:
#line 773 "XQuery.y"
  {
     yyVal = Lisp.List(yyVals[-3+yyTop], yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 119:
#line 780 "XQuery.y"
  {
     yyVal = new TokenWrapper(Token.ASCENDING);
  }
  break;
case 120:
#line 784 "XQuery.y"
  {
     yyVal = new TokenWrapper(Token.DESCENDING);
  }
  break;
case 121:
#line 791 "XQuery.y"
  {
     yyVal = new TokenWrapper(Token.EMPTY_GREATEST); 
  }
  break;
case 122:
#line 795 "XQuery.y"
  {
     yyVal = new TokenWrapper(Token.EMPTY_LEAST); 
  }
  break;
case 123:
#line 802 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Some, yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 124:
#line 806 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Every, yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 125:
#line 813 "XQuery.y"
  {
     yyVal = Lisp.Cons(yyVals[0+yyTop]);
  }
  break;
case 126:
#line 817 "XQuery.y"
  {
     yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
  }
  break;
case 127:
#line 824 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.QuantifiedExprOper, yyVals[-3+yyTop], yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 128:
#line 831 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Typeswitch, yyVals[-5+yyTop], yyVals[-3+yyTop], yyVals[0+yyTop]); 
  }
  break;
case 129:
#line 835 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Typeswitch, yyVals[-7+yyTop], yyVals[-5+yyTop], yyVals[-2+yyTop], yyVals[0+yyTop]); 
  }
  break;
case 130:
#line 842 "XQuery.y"
  {
     yyVal = Lisp.Cons(yyVals[0+yyTop]);
  }
  break;
case 131:
#line 846 "XQuery.y"
  {
     yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(yyVals[0+yyTop]));
  }
  break;
case 132:
#line 853 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Case, yyVals[-4+yyTop], yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 133:
#line 857 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Case, yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 134:
#line 864 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.If, yyVals[-5+yyTop], yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 136:
#line 872 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Or, yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 138:
#line 880 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.And, yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 140:
#line 888 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), 
		Descriptor.ValueComp, yyVals[-2+yyTop], yyVals[-1+yyTop], yyVals[0+yyTop]);
  }
  break;
case 141:
#line 893 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), 
		Descriptor.GeneralComp, yyVals[-2+yyTop], yyVals[-1+yyTop], yyVals[0+yyTop]);
  }
  break;
case 142:
#line 898 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), 
		Descriptor.NodeComp, yyVals[-2+yyTop], yyVals[-1+yyTop], yyVals[0+yyTop]);
  }
  break;
case 144:
#line 907 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.Range, yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 146:
#line 916 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.Add, yyVals[-2+yyTop], new TokenWrapper('+'), yyVals[0+yyTop]);
  }
  break;
case 147:
#line 921 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.Add, yyVals[-2+yyTop], new TokenWrapper('-'), yyVals[0+yyTop]);
  }
  break;
case 149:
#line 930 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.Mul, yyVals[-2+yyTop], new TokenWrapper(Token.ML), yyVals[0+yyTop]);
  }
  break;
case 150:
#line 935 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.Mul, yyVals[-2+yyTop], new TokenWrapper(Token.DIV), yyVals[0+yyTop]);
  }
  break;
case 151:
#line 940 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.Mul, yyVals[-2+yyTop], new TokenWrapper(Token.IDIV), yyVals[0+yyTop]);
  }
  break;
case 152:
#line 945 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.Mul, yyVals[-2+yyTop], new TokenWrapper(Token.MOD), yyVals[0+yyTop]);
  }
  break;
case 154:
#line 954 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.Union, yyVals[-2+yyTop], yyVals[0+yyTop]);  
  }
  break;
case 155:
#line 959 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.Concatenate, yyVals[-2+yyTop], yyVals[0+yyTop]);  
  }
  break;
case 157:
#line 968 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.IntersectExcept, yyVals[-2+yyTop], new TokenWrapper(Token.INTERSECT), yyVals[0+yyTop]);  
  }
  break;
case 158:
#line 973 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.IntersectExcept, yyVals[-2+yyTop], new TokenWrapper(Token.EXCEPT), yyVals[0+yyTop]);  
  }
  break;
case 160:
#line 982 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.InstanceOf, yyVals[-2+yyTop], yyVals[0+yyTop]);    
  }
  break;
case 162:
#line 990 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.TreatAs, yyVals[-2+yyTop], yyVals[0+yyTop]);    
  }
  break;
case 164:
#line 998 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.CastableAs, yyVals[-2+yyTop], yyVals[0+yyTop]);    
  }
  break;
case 166:
#line 1006 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.CastAs, yyVals[-2+yyTop], yyVals[0+yyTop]);    
  }
  break;
case 167:
#line 1013 "XQuery.y"
  {
     if (yyVals[-1+yyTop] == null)
       yyVal = yyVals[0+yyTop];
     else
       yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Unary, yyVals[-1+yyTop], yyVals[0+yyTop]);
  }
  break;
case 168:
#line 1023 "XQuery.y"
  {
     yyVal = null;
  }
  break;
case 169:
#line 1027 "XQuery.y"
  {
     yyVal = Lisp.Append(Lisp.Cons(new TokenWrapper('+')), yyVals[0+yyTop]);
  }
  break;
case 170:
#line 1031 "XQuery.y"
  {
     yyVal = Lisp.Append(Lisp.Cons(new TokenWrapper('-')), yyVals[0+yyTop]);
  }
  break;
case 171:
#line 1038 "XQuery.y"
  {
     yyVal = new Literal("=");
  }
  break;
case 172:
#line 1042 "XQuery.y"
  {
     yyVal = new Literal("!=");
  }
  break;
case 173:
#line 1046 "XQuery.y"
  {
     yyVal = new Literal("<");
  }
  break;
case 174:
#line 1050 "XQuery.y"
  {
     yyVal = new Literal("<=");
  }
  break;
case 175:
#line 1054 "XQuery.y"
  {
     yyVal = new Literal(">");
  }
  break;
case 176:
#line 1058 "XQuery.y"
  {
     yyVal = new Literal(">=");
  }
  break;
case 177:
#line 1065 "XQuery.y"
  {
     yyVal = new TokenWrapper(Token.EQ);
  }
  break;
case 178:
#line 1069 "XQuery.y"
  {
     yyVal = new TokenWrapper(Token.NE);
  }
  break;
case 179:
#line 1073 "XQuery.y"
  {
     yyVal = new TokenWrapper(Token.LT);
  }
  break;
case 180:
#line 1077 "XQuery.y"
  {
     yyVal = new TokenWrapper(Token.LE);
  }
  break;
case 181:
#line 1081 "XQuery.y"
  {
     yyVal = new TokenWrapper(Token.GT);
  }
  break;
case 182:
#line 1085 "XQuery.y"
  {
     yyVal = new TokenWrapper(Token.GE);
  }
  break;
case 183:
#line 1092 "XQuery.y"
  {
     yyVal = new TokenWrapper(Token.IS);
  }
  break;
case 184:
#line 1096 "XQuery.y"
  {
     yyVal = new Literal("<<");
  }
  break;
case 185:
#line 1100 "XQuery.y"
  {
     yyVal = new Literal(">>");
  }
  break;
case 189:
#line 1114 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Validate, null, yyVals[-1+yyTop]);
  }
  break;
case 190:
#line 1118 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Validate, yyVals[-3+yyTop], yyVals[-1+yyTop]);
  }
  break;
case 191:
#line 1125 "XQuery.y"
  {
     yyVal = new TokenWrapper(Token.LAX);
  }
  break;
case 192:
#line 1129 "XQuery.y"
  {
     yyVal = new TokenWrapper(Token.STRICT);
  }
  break;
case 193:
#line 1136 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.ExtensionExpr, yyVals[-3+yyTop], yyVals[-1+yyTop]);
  }
  break;
case 194:
#line 1143 "XQuery.y"
  {
     yyVal = Lisp.Cons(yyVals[0+yyTop]);
  }
  break;
case 195:
#line 1147 "XQuery.y"
  {
     yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(yyVals[0+yyTop]));
  }
  break;
case 196:
#line 1154 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Pragma, yyVals[-2+yyTop], yyVals[-1+yyTop]);
   }
  break;
case 197:
#line 1161 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Child, new object[] { null });
  }
  break;
case 198:
#line 1165 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Child, yyVals[0+yyTop]);
  }
  break;
case 199:
#line 1169 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Descendant, yyVals[0+yyTop]);
  }
  break;
case 202:
#line 1178 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Child, yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 203:
#line 1182 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Descendant, yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 204:
#line 1189 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.AxisStep, yyVals[0+yyTop]);
  }
  break;
case 205:
#line 1193 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.FilterExpr, yyVals[0+yyTop]);
  }
  break;
case 207:
#line 1201 "XQuery.y"
  {
      yyVal = yyVals[-1+yyTop];
      notation.Confirm((Symbol)yyVals[-1+yyTop], Descriptor.PredicateList, yyVals[0+yyTop]);
  }
  break;
case 209:
#line 1207 "XQuery.y"
  {
      yyVal = yyVals[-1+yyTop];
      notation.Confirm((Symbol)yyVals[-1+yyTop], Descriptor.PredicateList, yyVals[0+yyTop]);
  }
  break;
case 210:
#line 1215 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.ForwardStep, yyVals[-1+yyTop], yyVals[0+yyTop]);
   }
  break;
case 212:
#line 1223 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.AXIS_CHILD);
   }
  break;
case 213:
#line 1227 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.AXIS_DESCENDANT);
   }
  break;
case 214:
#line 1231 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.AXIS_ATTRIBUTE);
   }
  break;
case 215:
#line 1235 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.AXIS_SELF);
   }
  break;
case 216:
#line 1239 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.AXIS_DESCENDANT_OR_SELF);
   }
  break;
case 217:
#line 1243 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.AXIS_FOLLOWING_SIBLING);
   }
  break;
case 218:
#line 1247 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.AXIS_FOLLOWING);
   }
  break;
case 219:
#line 1251 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.AXIS_NAMESPACE);
   }
  break;
case 220:
#line 1258 "XQuery.y"
  {  
	  yyVal = notation.Confirm((Symbol)yyVals[0+yyTop], Descriptor.AbbrevForward, yyVals[0+yyTop]); 
   }
  break;
case 222:
#line 1266 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.ReverseStep, yyVals[-1+yyTop], yyVals[0+yyTop]);
   }
  break;
case 224:
#line 1274 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.AXIS_PARENT);
   }
  break;
case 225:
#line 1278 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.AXIS_ANCESTOR);
   }
  break;
case 226:
#line 1282 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.AXIS_PRECEDING_SIBLING);
   }
  break;
case 227:
#line 1286 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.AXIS_PRECEDING);
   }
  break;
case 228:
#line 1290 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.AXIS_ANCESTOR_OR_SELF);
   }
  break;
case 229:
#line 1297 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.DOUBLE_PERIOD);
   }
  break;
case 234:
#line 1314 "XQuery.y"
  {
      yyVal = new TokenWrapper('*');
   }
  break;
case 235:
#line 1318 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Wildcard1, yyVals[-2+yyTop]);
   }
  break;
case 236:
#line 1322 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Wildcard2, yyVals[0+yyTop]);
   }
  break;
case 238:
#line 1330 "XQuery.y"
  {
      yyVal = yyVals[-1+yyTop];
      notation.Confirm((Symbol)yyVals[-1+yyTop], Descriptor.PredicateList, yyVals[0+yyTop]);
   }
  break;
case 239:
#line 1338 "XQuery.y"
  {
      yyVal = Lisp.Cons(yyVals[0+yyTop]);
   }
  break;
case 240:
#line 1342 "XQuery.y"
  {
      yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(yyVals[0+yyTop]));
   }
  break;
case 241:
#line 1349 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Predicate, yyVals[-1+yyTop]);
   }
  break;
case 255:
#line 1378 "XQuery.y"
  {
      yyVal = yyVals[0+yyTop];
   }
  break;
case 256:
#line 1385 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.ParenthesizedExpr, new object[] { null });
   }
  break;
case 257:
#line 1389 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.ParenthesizedExpr, yyVals[-1+yyTop]);
   }
  break;
case 258:
#line 1396 "XQuery.y"
  {
      yyVal = new TokenWrapper('.');
   }
  break;
case 259:
#line 1403 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Ordered, yyVals[-1+yyTop]);
   }
  break;
case 260:
#line 1410 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Unordered, yyVals[-1+yyTop]);
   }
  break;
case 261:
#line 1417 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Funcall, yyVals[-2+yyTop], null);
   }
  break;
case 262:
#line 1421 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Funcall, yyVals[-3+yyTop], yyVals[-1+yyTop]);
   }
  break;
case 263:
#line 1428 "XQuery.y"
  {
      yyVal = Lisp.Cons(yyVals[0+yyTop]);
   }
  break;
case 264:
#line 1432 "XQuery.y"
  {
      yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
   }
  break;
case 270:
#line 1450 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirElemConstructor, yyVals[-3+yyTop], yyVals[-2+yyTop]);
   }
  break;
case 271:
#line 1454 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirElemConstructor, 
		 yyVals[-7+yyTop], yyVals[-6+yyTop], null, yyVals[-2+yyTop], yyVals[-1+yyTop]);
   }
  break;
case 272:
#line 1459 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirElemConstructor, 
		 yyVals[-8+yyTop], yyVals[-7+yyTop], yyVals[-5+yyTop], yyVals[-2+yyTop], yyVals[-1+yyTop]);
   }
  break;
case 273:
#line 1467 "XQuery.y"
  {
      yyVal = Lisp.Cons(yyVals[0+yyTop]);
   }
  break;
case 274:
#line 1471 "XQuery.y"
  {      
      yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(yyVals[0+yyTop]));
   }
  break;
case 275:
#line 1478 "XQuery.y"
  {
      yyVal = null;
   }
  break;
case 277:
#line 1486 "XQuery.y"
  {
      yyVal = Lisp.List(yyVals[-1+yyTop], yyVals[0+yyTop]);   
   }
  break;
case 278:
#line 1490 "XQuery.y"
  {
      yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(yyVals[0+yyTop]));
   }
  break;
case 279:
#line 1494 "XQuery.y"
  {
      yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.List(yyVals[-1+yyTop], yyVals[0+yyTop]));
   }
  break;
case 280:
#line 1501 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirAttribute,
		 yyVals[-6+yyTop], yyVals[-5+yyTop], yyVals[-3+yyTop], new Literal("\""), yyVals[-1+yyTop]);
   }
  break;
case 281:
#line 1506 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirAttribute,
		 yyVals[-6+yyTop], yyVals[-5+yyTop], yyVals[-3+yyTop], new Literal("\'"), yyVals[-1+yyTop]);
   }
  break;
case 282:
#line 1514 "XQuery.y"
  {
      yyVal = Lisp.Cons(new TokenWrapper(Token.EscapeQuot));
   }
  break;
case 283:
#line 1518 "XQuery.y"
  {
      yyVal = Lisp.Cons(yyVals[0+yyTop]);
   }
  break;
case 284:
#line 1522 "XQuery.y"
  {
      yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(new TokenWrapper(Token.EscapeQuot)));
   }
  break;
case 285:
#line 1526 "XQuery.y"
  {
      yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(yyVals[0+yyTop]));
   }
  break;
case 286:
#line 1533 "XQuery.y"
  {
      yyVal = Lisp.Cons(new TokenWrapper(Token.EscapeApos));
   }
  break;
case 287:
#line 1537 "XQuery.y"
  {
      yyVal = Lisp.Cons(yyVals[0+yyTop]);
   }
  break;
case 288:
#line 1541 "XQuery.y"
  {
      yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(new TokenWrapper(Token.EscapeApos)));
   }
  break;
case 289:
#line 1545 "XQuery.y"
  {
      yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(yyVals[0+yyTop]));
   }
  break;
case 300:
#line 1571 "XQuery.y"
  {
      yyVal = new Literal("{{");
   }
  break;
case 301:
#line 1575 "XQuery.y"
  {
      yyVal = new Literal("}}");
   }
  break;
case 302:
#line 1579 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CommonContent), Descriptor.EnclosedExpr, yyVals[0+yyTop]); 
   }
  break;
case 303:
#line 1586 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirCommentConstructor, yyVals[-1+yyTop]);
   }
  break;
case 304:
#line 1593 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirPIConstructor, yyVals[-1+yyTop], null);
   }
  break;
case 305:
#line 1597 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirPIConstructor, yyVals[-3+yyTop], yyVals[-1+yyTop]);
   }
  break;
case 306:
#line 1604 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CData), Descriptor.CDataSection, yyVals[-1+yyTop]);
   }
  break;
case 313:
#line 1620 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompDocConstructor, yyVals[-1+yyTop]);
   }
  break;
case 314:
#line 1628 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompElemConstructor, yyVals[-3+yyTop], yyVals[-1+yyTop]);   
   }
  break;
case 315:
#line 1633 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompElemConstructor, yyVals[-2+yyTop], null);   
   }
  break;
case 316:
#line 1638 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompElemConstructor, yyVals[-4+yyTop], yyVals[-1+yyTop]);   
   }
  break;
case 317:
#line 1643 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompElemConstructor, yyVals[-3+yyTop], null);   
   }
  break;
case 319:
#line 1655 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompAttrConstructor, yyVals[-3+yyTop], yyVals[-1+yyTop]);   
   }
  break;
case 320:
#line 1660 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompAttrConstructor, yyVals[-2+yyTop], null);   
   }
  break;
case 321:
#line 1665 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompAttrConstructor, yyVals[-4+yyTop], yyVals[-1+yyTop]);   
   }
  break;
case 322:
#line 1670 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompAttrConstructor, yyVals[-3+yyTop], null);   
   }
  break;
case 323:
#line 1678 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompTextConstructor, yyVals[-1+yyTop]);   
   }
  break;
case 324:
#line 1686 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompCommentConstructor, yyVals[-1+yyTop]);   
   }
  break;
case 325:
#line 1694 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompPIConstructor, yyVals[-3+yyTop], yyVals[-1+yyTop]);   
   }
  break;
case 326:
#line 1699 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompPIConstructor, yyVals[-2+yyTop], null);   
   }
  break;
case 327:
#line 1704 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompPIConstructor, yyVals[-4+yyTop], yyVals[-1+yyTop]);   
   }
  break;
case 328:
#line 1709 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompPIConstructor, yyVals[-3+yyTop], null);   
   }
  break;
case 330:
#line 1718 "XQuery.y"
  {
      yyVal = yyVals[-1+yyTop];
      notation.Confirm((Symbol)yyVals[-1+yyTop], Descriptor.Occurrence, 
		new TokenWrapper(Token.Indicator3));
   }
  break;
case 331:
#line 1727 "XQuery.y"
  {
      yyVal = yyVals[0+yyTop];
   }
  break;
case 333:
#line 1735 "XQuery.y"
  {
      yyVal = yyVals[-1+yyTop];
      notation.Confirm((Symbol)yyVals[-1+yyTop], Descriptor.Occurrence, yyVals[0+yyTop]);
   }
  break;
case 334:
#line 1740 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.VOID);
   }
  break;
case 335:
#line 1747 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.Indicator1);
   }
  break;
case 336:
#line 1751 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.Indicator2);
   }
  break;
case 337:
#line 1755 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.Indicator3);
   }
  break;
case 340:
#line 1764 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.ITEM);
   }
  break;
case 342:
#line 1775 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.KindTest, yyVals[0+yyTop]);
   }
  break;
case 352:
#line 1793 "XQuery.y"
  {
       yyVal = new TokenWrapper(Token.NODE);
   }
  break;
case 353:
#line 1800 "XQuery.y"
  {
       yyVal = new TokenWrapper(Token.DOCUMENT_NODE);
   }
  break;
case 354:
#line 1804 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.DocumentNode, yyVals[-1+yyTop]);
   }
  break;
case 355:
#line 1808 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.DocumentNode, yyVals[-1+yyTop]);
   }
  break;
case 356:
#line 1815 "XQuery.y"
  {
       yyVal = new TokenWrapper(Token.TEXT);
   }
  break;
case 357:
#line 1822 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.COMMENT);
   }
  break;
case 358:
#line 1830 "XQuery.y"
  {
       yyVal = new TokenWrapper(Token.PROCESSING_INSTRUCTION);
   }
  break;
case 359:
#line 1834 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.ProcessingInstruction, yyVals[-1+yyTop]);
   }
  break;
case 360:
#line 1838 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.ProcessingInstruction, yyVals[-1+yyTop]);
   }
  break;
case 361:
#line 1845 "XQuery.y"
  {
       yyVal = new TokenWrapper(Token.ELEMENT);
   }
  break;
case 362:
#line 1849 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Element, yyVals[-1+yyTop]);
   }
  break;
case 363:
#line 1853 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Element, yyVals[-3+yyTop], yyVals[-1+yyTop]);
   }
  break;
case 364:
#line 1857 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Element, 
		yyVals[-4+yyTop], yyVals[-2+yyTop], new TokenWrapper('?'));
   }
  break;
case 366:
#line 1866 "XQuery.y"
  {
      yyVal = new TokenWrapper('*');
   }
  break;
case 367:
#line 1873 "XQuery.y"
  {
       yyVal = new TokenWrapper(Token.ATTRIBUTE);
   }
  break;
case 368:
#line 1877 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Attribute, yyVals[-1+yyTop]);
   }
  break;
case 369:
#line 1881 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Attribute, yyVals[-3+yyTop], yyVals[-1+yyTop]);
   }
  break;
case 371:
#line 1889 "XQuery.y"
  {
      yyVal = new TokenWrapper('*');
   }
  break;
case 372:
#line 1896 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.SchemaElement, yyVals[-1+yyTop]);
   }
  break;
case 373:
#line 1903 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.SchemaAttribute, yyVals[-1+yyTop]);
   }
  break;
case 377:
#line 1922 "XQuery.y"
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
   28,   34,   34,   17,   17,   35,   35,   23,   23,   18,
   18,   39,   39,   38,   38,   42,   42,   43,   43,   41,
    6,   44,   44,   36,   36,   36,   36,   36,   45,   45,
   45,   45,   50,   50,   50,   50,   53,   55,   55,   56,
   57,   57,   58,   54,   59,   59,   60,   51,   52,   52,
   61,   61,   62,   62,   63,   63,   63,   63,   64,   64,
   65,   65,   46,   46,   66,   66,   67,   47,   47,   68,
   68,   69,   69,   48,   49,   49,   70,   70,   71,   71,
   71,   71,   72,   72,   76,   76,   76,   77,   77,   77,
   77,   77,   78,   78,   78,   79,   79,   79,   80,   80,
   81,   81,   82,   82,   83,   83,   85,   86,   86,   86,
   74,   74,   74,   74,   74,   74,   73,   73,   73,   73,
   73,   73,   75,   75,   75,   87,   87,   87,   88,   88,
   91,   91,   90,   92,   92,   93,   89,   89,   89,   89,
   95,   95,   95,   96,   96,   97,   97,   97,   97,   99,
   99,  102,  102,  102,  102,  102,  102,  102,  102,  104,
  104,  101,  101,  105,  105,  105,  105,  105,  106,  103,
  103,  108,  108,  109,  109,  109,   98,   98,  100,  100,
  111,  110,  110,  110,  110,  110,  110,  110,  110,  112,
  112,  120,  120,  120,  113,  114,  114,  115,  118,  119,
  116,  116,  121,  121,  117,  117,  122,  122,  122,  124,
  124,  124,  128,  128,  127,  127,  130,  130,  130,  131,
  131,  132,  132,  132,  132,  133,  133,  133,  133,  134,
  134,  135,  135,  129,  129,  129,  129,  137,  137,  137,
  137,  137,  125,  126,  126,  140,  123,  123,  123,  123,
  123,  123,  141,  142,  142,  142,  142,  147,  143,  143,
  143,  143,  144,  145,  146,  146,  146,  146,   84,   84,
   37,   40,   40,   40,  150,  150,  150,  149,  149,  149,
  148,  107,  151,  151,  151,  151,  151,  151,  151,  151,
  151,  160,  152,  152,  152,  159,  158,  157,  157,  157,
  153,  153,  153,  153,  161,  161,  154,  154,  154,  164,
  164,  155,  156,  165,  163,  162,   94,   94,  136,  138,
  139,    8,
  };
   static  short [] yyLen = {           2,
    2,    1,    2,    1,    3,    5,    2,    2,    5,    0,
    1,    1,    2,    2,    3,    2,    3,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    4,    2,    2,    3,    3,    3,
    2,    2,    2,    2,    4,    1,    1,    1,    1,    2,
    2,    3,    5,    0,    1,    1,    3,    3,    2,    3,
    6,    0,    2,    7,    5,    0,    1,    2,    2,    6,
    8,    1,    1,    0,    1,    1,    3,    2,    3,    3,
    1,    1,    3,    1,    1,    1,    1,    1,    3,    4,
    4,    5,    1,    1,    2,    2,    2,    1,    3,    6,
    0,    1,    3,    2,    1,    3,    6,    2,    2,    2,
    1,    3,    1,    2,    1,    2,    3,    4,    1,    1,
    1,    1,    4,    4,    1,    3,    5,    8,   10,    1,
    2,    7,    4,    8,    1,    3,    1,    3,    1,    3,
    3,    3,    1,    3,    1,    3,    3,    1,    3,    3,
    3,    3,    1,    3,    3,    1,    3,    3,    1,    3,
    1,    3,    1,    3,    1,    3,    2,    0,    2,    2,
    1,    2,    1,    2,    1,    2,    1,    1,    1,    1,
    1,    1,    1,    2,    2,    1,    1,    1,    4,    5,
    1,    1,    4,    1,    2,    5,    1,    2,    2,    1,
    1,    3,    3,    1,    1,    1,    2,    1,    2,    2,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    2,
    1,    2,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    3,    3,    1,    2,    1,    2,
    3,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    2,    2,    3,    1,    4,    4,
    3,    4,    1,    3,    1,    1,    1,    1,    1,    5,
    9,   10,    1,    2,    0,    1,    2,    2,    3,    7,
    7,    1,    1,    2,    2,    1,    1,    2,    2,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    2,
    2,    1,    3,    3,    5,    3,    1,    1,    1,    1,
    1,    1,    4,    5,    4,    7,    6,    1,    5,    4,
    7,    6,    4,    4,    5,    4,    7,    6,    1,    2,
    2,    1,    2,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    3,    3,    4,    4,    3,    3,    3,    4,    4,
    3,    4,    6,    7,    1,    1,    3,    4,    6,    1,
    1,    4,    4,    1,    1,    1,    0,    1,    1,    1,
    1,    1,
  };
   static  short [] yyDefRed = {            0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    2,
    4,    0,    0,    0,    0,    0,    0,   18,   19,   20,
   21,   22,   23,   24,   25,   26,   27,   28,   29,   30,
   31,   32,   33,    0,    0,    0,    0,    0,   55,    0,
  382,    0,    0,   36,   37,    0,    0,   43,   44,    0,
   41,   42,   46,   47,    0,   50,   51,    0,   68,   69,
    0,    1,    3,    0,    0,    0,    0,    0,    0,    0,
    0,    7,   82,    0,   84,   85,   86,   87,    0,    0,
   93,   94,    0,  137,    0,    0,    0,    0,    0,  156,
    0,    0,    0,    0,    0,    8,    0,    0,    0,   34,
   14,   16,    0,    5,    0,    0,   59,    0,    0,    0,
   60,    0,   38,   39,   40,    0,    0,    0,    0,    0,
   98,    0,    0,  105,    0,    0,  125,    0,    0,    0,
  169,  170,    0,    0,    0,    0,    0,    0,    0,    0,
   95,   96,    0,  177,  178,  179,  181,  182,  180,  183,
  171,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  229,  251,  252,  253,  254,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  212,  213,  214,  215,
  216,  217,  218,  224,  225,  226,  227,  228,  219,    0,
    0,    0,    0,    0,    0,  258,  167,  186,  187,  188,
    0,  194,    0,  201,  204,  205,    0,    0,    0,  221,
  211,    0,  223,  230,  231,  233,    0,  242,  243,  244,
  245,  246,  247,  248,  249,  250,  265,  266,  267,  268,
  269,  307,  308,  309,  310,  311,  312,  342,  343,  344,
  345,  346,  347,  348,  349,  350,  351,   15,   17,    0,
    0,   58,    0,    0,   56,    0,   35,   48,   49,   45,
    0,    0,   67,    0,    0,    0,   76,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,   83,    0,   89,
  108,    0,    0,  111,    0,    0,    0,    0,  138,  172,
  174,  184,  176,  185,  140,  141,  142,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  157,  158,    0,    0,
    0,    0,  341,    0,  334,  340,  160,  339,  338,    0,
  162,  164,    0,  166,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  191,  192,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  378,    0,    0,  255,  256,    0,    0,
  232,  220,    0,    0,  195,    0,    0,    0,    0,  239,
    0,  210,  222,    0,    6,    9,    0,    0,    0,  331,
   65,    0,    0,    0,    0,    0,   99,    0,  106,    0,
  123,  126,  124,    0,    0,  119,  120,  114,    0,    0,
   90,    0,   91,  335,  336,  337,  333,  330,    0,    0,
    0,    0,  375,  361,  366,    0,  365,    0,    0,  374,
  367,  371,    0,  370,    0,  356,    0,  357,    0,    0,
    0,  352,  235,  261,  263,    0,    0,    0,    0,  358,
    0,  353,    0,    0,    0,    0,  303,    0,  304,    0,
    0,    0,    0,  257,  236,    0,  203,  202,    0,  240,
   61,   57,    0,   79,   73,    0,    0,   70,   72,   77,
    0,    0,  102,    0,    0,    0,    0,  130,    0,    0,
  121,  122,    0,  112,   92,  259,  260,  313,  315,    0,
    0,    0,  362,    0,  320,    0,    0,  368,    0,  323,
  324,  189,    0,    0,  262,  326,    0,  360,  359,    0,
  354,  355,  372,  373,    0,    0,    0,  277,    0,    0,
    0,  193,  241,   64,    0,    0,    0,    0,    0,  127,
    0,    0,    0,  131,    0,  117,    0,  314,  376,    0,
    0,  319,    0,    0,  190,  264,  325,    0,  305,  196,
    0,  381,  298,  299,    0,    0,    0,    0,  302,  294,
    0,  273,  297,  295,  296,  270,  279,   71,   80,  103,
  100,  107,    0,    0,    0,    0,    0,  118,  363,    0,
  317,    0,  369,  322,    0,  328,    0,    0,    0,  300,
  301,    0,    0,  274,    0,  133,  128,    0,  134,  364,
  316,  321,  327,    0,  306,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  129,  380,  286,    0,  287,  293,
  292,  379,  282,    0,  283,  290,  291,  271,    0,  132,
  281,  288,  289,  284,  280,  285,  272,
  };
  protected static  short [] yyDgoto  = {            18,
   19,   20,   21,  111,   22,   82,   23,  285,   24,   25,
   26,   27,   28,   29,   30,   31,   32,   33,   34,   35,
   36,   37,   38,   39,   40,   41,   42,   43,   65,  290,
   48,  286,   49,  121,  292,   83,  293,  295,  498,  347,
  589,  296,  297,  520,   85,   86,   87,   88,   89,   90,
  149,  150,   91,   92,  130,  131,  502,  503,  133,  134,
  313,  314,  428,  429,  513,  136,  137,  507,  508,   93,
   94,   95,  165,  166,  167,   96,   97,   98,   99,  100,
  101,  102,  103,  352,  104,  105,  227,  228,  229,  230,
  371,  231,  232,  385,  233,  234,  235,  236,  237,  399,
  238,  239,  240,  241,  242,  243,  244,  245,  246,  247,
  400,  248,  249,  250,  251,  252,  253,  254,  255,  256,
  466,  257,  258,  259,  260,  261,  482,  591,  592,  483,
  548,  654,  648,  655,  649,  656,  593,  651,  594,  595,
  262,  263,  264,  265,  266,  267,  521,  349,  350,  437,
  268,  269,  270,  271,  272,  273,  274,  275,  276,  277,
  446,  570,  447,  453,  454,
  };
  protected static  short [] yySindex = {          967,
 -254, -227, -214, -181, -223,  -63, -144, -138,    4, -189,
  104,  116, -174, -174,  126,  -36, -111,    0, 1086,    0,
    0,  730, 1109, 1109, -158,  159,  159,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   14,  140,  -80,   56, -174,    0,  -67,
    0,   19,  196,    0,    0, -174, -174,    0,    0,  -52,
    0,    0,    0,    0,  236,    0,    0,    6,    0,    0,
  304,    0,    0,  311,  316,  318,  318,  321,  324,  202,
  202,    0,    0,  312,    0,    0,    0,    0,   66, -171,
    0,    0,   87,    0,  142,  -12, -235,  -92,   88,    0,
   81,  102,  124,  103, 4513,    0, -158,  159,  159,    0,
    0,    0,  112,    0, -174,  369,    0,  170,  380, -174,
    0, -174,    0,    0,    0,  138,  206,  425,  148,  420,
    0,  151,  433,    0,  156,  -24,    0,   -7,  730,  730,
    0,    0,  730,  202,  730,  730,  730,  730, -156,  209,
    0,    0,  202,    0,    0,    0,    0,    0,    0,    0,
    0,  419,  349,  350,  202,  202,  202,  202,  202,  202,
  202,  202,  202,  202,  202,  202,  202,  202,  879,  879,
  161,  161,  360,  363,  366,  -19,  -16,   13,   17, -105,
  452,    0,    0,    0,    0,    0,  446,  465,  -14,  467,
  474,  475, 4771,  203,  204,  201,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  195,
  207,  213, 4771,  434,  460,    0,    0,    0,    0,    0,
  -85,    0,  -32,    0,    0,    0,  431,  431,  434,    0,
    0,  434,    0,    0,    0,    0,  431,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  159,
  159,    0, -174, -174,    0,  486,    0,    0,    0,    0,
  879,   10,    0,  210,  490,  489,    0,  206,  311,  206,
  316,  206,  730,  318,  730,  173,  192,    0,   87,    0,
    0,  139,  491,    0,  491,  730,  266,  730,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  246, -235, -235,
  -92,  -92,  -92,  -92,   88,   88,    0,    0,  506,  507,
  508,  509,    0,  510,    0,    0,    0,    0,    0, -193,
    0,    0,  488,    0,  730,  730,  730,  430,  -13,  730,
  432,   -6,  730,  513,  730,  516,  730,    0,    0,  730,
  435,  518,  519,  395,  437,    1,  730,  -25,  241,  242,
  -32,  214, -253,    0,  250,  248,    0,    0,  193,  -32,
    0,    0,  263,  730,    0, 4771, 4771,  730,  431,    0,
  431,    0,    0,  431,    0,    0,  486,   19, -174,    0,
    0,  523,  206,  -83,  425,  319,    0,  527,    0,  335,
    0,    0,    0,  306,  322,    0,    0,    0, -207,  730,
    0,  730,    0,    0,    0,    0,    0,    0,   23,   28,
   30,  536,    0,    0,    0,  194,    0,   31,  591,    0,
    0,    0,  198,    0,   32,    0,   33,    0,   34,   35,
  730,    0,    0,    0,    0,  200,  734,  564,  566,    0,
   40,    0,  569,  570,  571,  572,    0,  300,    0,  295,
  298,   75,  297,    0,    0,   41,    0,    0,   36,    0,
    0,    0,  730,    0,    0,  879,  730,    0,    0,    0,
  586,  359,    0,  568,  730,  601,  137,    0,  730, -174,
    0,    0,  354,    0,    0,    0,    0,    0,    0,  312,
  514,  310,    0,  512,    0,   43,  310,    0,  517,    0,
    0,    0,   44,  730,    0,    0,   45,    0,    0,  520,
    0,    0,    0,    0,  274,  273,  201,    0,  -15,  579,
  298,    0,    0,    0,  -79,   46,  320,  730,  730,    0,
  325,  379,   -9,    0,  361,    0, -174,    0,    0,   53,
  767,    0,  609,  855,    0,    0,    0,  989,    0,    0,
  598,    0,    0,    0,  344, 1006,  537,  616,    0,    0,
    5,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  396,  730,  730,  343,  730,    0,    0,  627,
    0,  544,    0,    0,   47,    0,   48,  201,  308,    0,
    0,  353,  629,    0,  879,    0,    0,  407,    0,    0,
    0,    0,    0,  -26,    0,  201,  365,  410,  730, -100,
  -75,  622,  201,  730,    0,    0,    0, -106,    0,    0,
    0,    0,    0,  -27,    0,    0,    0,    0,  631,    0,
    0,    0,    0,    0,    0,    0,    0,
  };
  protected static  short [] yyRindex = {         4101,
    0,    0,  385,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0, 4101,    0,
    0, 4657,  695,  223,  337,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  656,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0, 4657,
 4657,    0,    0,  718,    0,    0,    0,    0, 1581,    0,
    0,    0,  123,    0,   22, 4028, 3649, 3309, 2908,    0,
 2862, 2774, 2496, 2450,    0,    0,  481,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  660,    0,    0,
    0,    0,    0,    0,    0,    0,   12,  679,    0,  118,
    0,    0,  296,    0,    0,    0,    0,    0, 4657, 4657,
    0,    0, 4657, 4657, 4657, 4657, 4657, 4657,    0,    0,
    0,    0, 4657,    0,    0,    0,    0,    0,    0,    0,
    0,    0, 4303, 4399, 4657, 4657, 4657, 4657, 4657, 4657,
 4657, 4657, 4657, 4657, 4657, 4657, 4657, 4657,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  638,    0,    0,
    0,    0,    0,    0,    0,  404,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0, 4657, 1894,    0,  782,    0,    0,    0,    0,    0,
    0,    0, 2028,    0,    0,    0,  916, 1060,    0,    0,
    0,    0,    0,    0,    0,    0, 1194,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  667,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  686,    0,   49,    0,  670,
    0,  461, 4657,    0, 4657,    0,    0,    0,  150,    0,
    0,  -10,  462,    0,  463, 4657,    0, 4657,    0,    0,
    0,    0,    0,    0,    0,    0,    0, 4231, 3906, 3978,
 3443, 3566, 3700, 3823, 3052, 3186,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0, 1338,
    0,    0, 2584,    0, 4657, 4657, 4657,    0,    0, 4657,
    0,    0, 4657,    0, 4657,    0, 4657,    0,    0, 4657,
    0,    0,    0, 4657,    0,    0, 4657,    0,    0,    0,
 2172,    0,    0,    0,    0,   84,    0,    0,    0, 2306,
    0,    0,    0, 4657,    0,    0,    0, 4657, 1472,    0,
 1616,    0,    0, 1750,    0,    0,  675,  656,    0,    0,
    0,    0,  205,    0,    0,  466,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   -3, 4657,
    0, 4657,    0,    0,    0,    0,    0,    0,    0,    0,
    0, 4657,    0,    0,    0,    0,    0,    0, 4657,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
 4657,    0,    0,    0,    0,    0, 4657,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  134,    0,    0,    0,    0,    0,    0,    0,
    0,    0, 4657,    0,    0,    0, 4657,    0,    0,    0,
    0,    0,    0,    0, 4657,    0,    0,    0, 4657,    0,
    0,    0,    7,    0,    0,    0,    0,    0,    0,  611,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0, 4657,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  676,    0,    0,    0,
   -4,    0,    0,    0,    0,    0,    0, 4657, 4657,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
 4657,    0,    0, 4657,    0,    0,    0, 4657,    0,    0,
    0,    0,    0,    0,    0, 4657,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0, 4657, 4657,    0, 4657,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  -20,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  677,    0,    0, 4657,    0,
    0,    0,  677, 4657,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,
  };
  protected static  short [] yyGindex = {            0,
    0,  719,  721,    3,  720,    0,    0,   -2,    0,  717,
  723,   27,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  459,    0,  336,  -87, -142,  338,    0,  211, -167,
 -375,    0,  334,  -22,    0,    0,    0,    0,    0,    0,
    0,  607,  682,  684,    0,  451,    0,    0,    0,  456,
  612,  346,    0,    0,    0,  688,  476,    0,  271,  637,
  630,  158,    0,    0,    0,  620,  264,   57,  268,  276,
    0,    0,    0,  608,    0,  376,    0,    0,    0,    0,
    0,    0,  558, -491, -120,   63,    0,    0,    0, -152,
    0,    0,  -98,    0,    0,    0, -170,    0,    0,    0,
 -250,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0, -485,    0,    0,    0,    0,    0,    0,  212,    0,
  240,    0,    0,  152,  145,    0, -442,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  233,  287,    0,    0,
    0,    0,  416,    0,  427,    0,    0,    0,    0,    0,
    0,  282,  438,    0,  440,
  };
  protected static  short [] yyTable = {            84,
  308,   52,  310,  311,  312,  312,  665,  641,  348,  348,
   66,   67,  351,  377,  397,  472,  586,  370,  587,  304,
  359,  139,  586,  362,  587,  376,  606,  444,  445,  112,
  169,  176,  170,  113,  451,  452,  304,  394,  499,  497,
  115,  470,  278,  497,  588,  118,  114,  586,   46,  587,
  116,  109,  364,  123,  124,  581,  366,  278,  171,  172,
  173,   44,  139,  590,  623,  139,  143,  412,  510,   66,
  478,  143,  110,  143,  143,  143,  143,  143,  143,  143,
  139,   50,  381,  143,  143,  401,  143,  143,  143,  143,
  143,  143,   45,  609,  404,  586,   53,  587,  145,   74,
   75,  146,  390,  360,  479,  590,  363,  586,  377,  587,
  278,  279,  281,  316,  139,  610,  306,  307,   56,  287,
  348,  550,  135,  410,   57,  392,  634,  586,  553,  587,
  275,   60,   47,  109,   51,  365,  549,  511,  512,  367,
  402,   51,  174,  403,  642,  275,  139,  516,  490,  136,
  490,  659,  517,  490,  518,  524,  529,  530,  531,  532,
  421,   68,  423,  135,  540,  552,  135,  572,  575,  577,
  599,  632,  633,  431,  162,  433,  147,  148,   10,  499,
  276,  135,  495,   15,  496,   17,  495,  434,  435,  436,
  136,  147,  148,  136,   54,  276,   55,  650,  657,  389,
  115,  163,  161,  164,  175,  650,  368,  369,  136,   71,
  416,  657,  418,  424,  420,  135,  143,  110,  646,  583,
  584,   69,   11,   70,  646,  583,  584,  331,  332,  333,
  334,  465,  425,  484,  523,  143,  143,  522,  528,  116,
  535,  527,  136,  534,   80,   78,   81,  135,   78,  652,
  583,  584,  119,  388,  303,   80,  122,   81,   11,  113,
  605,  339,   11,  125,   11,   11,  115,   11,   11,   11,
  113,  305,  661,  206,  136,  411,  116,   66,  662,  126,
  168,  408,  405,  406,  647,  120,   11,  312,  169,  515,
  170,  139,  139,  139,  139,  139,  139,  652,  583,  584,
  139,  358,  139,  139,  361,  375,  139,  443,  653,  582,
  583,  584,  139,  139,  450,   66,  468,   66,  117,  278,
  469,  396,  325,  326,  327,  348,  201,  127,  555,  582,
  583,  584,  439,  440,  441,  348,   12,  448,  562,  204,
  455,  205,  457,  128,  459,  585,  129,  460,   58,   59,
  554,  132,  640,  135,  471,  143,  664,  144,  377,  204,
  139,  205,  560,  140,  220,  585,  565,   61,   62,  139,
  139,  486,   12,   63,   64,  489,   12,  153,   12,   12,
  179,   12,   12,   12,  220,  177,  178,   97,   97,   97,
   97,  576,  135,  135,  135,  135,  135,  135,  288,  289,
   12,  135,  180,  135,  135,  182,  492,  135,  322,  321,
  323,  324,  426,  427,  135,  601,  602,  506,  563,  136,
  136,  136,  136,  136,  136,  181,  526,  280,  136,  282,
  136,  136,  329,  330,  136,  464,  283,   80,  533,   81,
  284,  136,  335,  336,  537,  154,  155,  156,  157,  158,
  159,  160,  337,  338,  348,  141,  142,  638,  487,  488,
  294,  626,  627,  299,  629,   97,   97,  353,  353,  298,
  135,  135,  300,  291,  556,  225,  301,  302,  318,  320,
   13,  343,  355,   74,   75,  356,   11,   11,  357,   76,
   77,  372,   78,   11,   11,   79,  645,  136,  136,   11,
   11,  660,   11,  373,  374,   11,  378,  566,   11,   11,
   11,   11,   11,  379,  380,  386,   13,  393,  382,  383,
   13,  398,   13,   13,  384,   13,   13,   13,  387,  409,
  414,  413,  415,   11,  430,  432,   11,   11,   11,   11,
   11,   11,   11,   11,   13,  359,  362,  364,  366,  376,
  438,  615,  442,  456,  449,  617,  458,  461,  462,  467,
  463,  443,  450,  556,  608,  104,  104,  104,  104,  477,
  480,  481,   11,   11,   11,   11,   11,   11,   80,   11,
   81,   11,  485,  493,  504,  501,  506,   11,   11,   11,
   11,   11,   11,   11,   11,   11,   11,   11,   11,   11,
   12,   12,   11,  505,  538,  509,  539,   12,   12,  541,
  542,  543,  544,   12,   12,  545,   12,  546,  547,   12,
  551,  557,   12,   12,   12,   12,   12,  558,  559,  567,
  569,  579,  580,   80,  571,   81,  561,  232,  568,  574,
  596,  600,  578,  104,  104,  607,  603,   12,  604,  613,
   12,   12,   12,   12,   12,   12,   12,   12,  618,  619,
  519,  621,  622,  625,  628,   74,   75,  630,  631,  635,
  232,   76,   77,  636,   78,  637,  639,   79,  232,  644,
  232,  232,  232,  658,  232,  643,   12,   12,   12,   12,
   12,   12,  667,   12,   10,   12,  232,  232,  232,  232,
   54,   12,   12,   12,   12,   12,   12,   12,   12,   12,
   12,   12,   12,   12,   62,  525,   12,   81,   52,   74,
  339,  340,  341,  342,  377,   63,   75,   66,  232,   66,
  232,  109,  110,   53,  101,  318,  377,   72,  377,   73,
  107,  407,  106,  491,   13,   13,  108,  191,  500,  417,
  494,   13,   13,  197,  391,  317,  419,   13,   13,  315,
   13,  232,  232,   13,  138,  598,   13,   13,   13,   13,
   13,  151,   80,  152,   81,  514,   80,  564,   81,  422,
  309,  234,  319,  344,  200,  201,  202,  328,  395,  354,
  597,   13,  663,  473,   13,   13,   13,   13,   13,   13,
   13,   13,  624,  612,  474,  666,   74,   75,  573,   80,
    0,   81,   76,   77,  234,   78,  475,    0,   79,  476,
    0,    0,  234,    0,  234,  234,  234,    0,  234,    0,
   13,   13,   13,   13,   13,   13,    0,   13,    0,   13,
  234,  234,  234,  234,    0,   13,   13,   13,   13,   13,
   13,   13,   13,   13,   13,   13,   13,   13,  536,    0,
   13,   74,   75,    0,    0,    0,    0,   76,   77,    0,
   78,    0,  234,   79,  234,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  339,  340,  341,
  342,  611,    0,    0,    0,    0,    0,   80,    0,   81,
    0,    0,    0,    0,    0,  234,  234,  232,  232,  232,
  232,  232,  232,    0,  191,  206,  232,    0,  232,  232,
    0,  343,  232,    0,    0,    0,    0,    0,  232,  232,
  232,  232,  232,  232,  232,  232,  232,  232,  232,  232,
  232,  232,  232,  232,  232,  232,  232,  232,  206,    0,
  344,  200,  201,  202,    0,    0,  206,    0,  206,  206,
  206,    0,  206,  345,  346,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  206,  206,  206,  206,    0,  614,
    0,    0,    0,    0,    0,  232,  232,    0,    0,    0,
    0,  232,    0,    0,    0,    0,    0,    0,    0,    0,
   74,   75,    0,    0,   74,   75,   76,   77,  206,   78,
   76,   77,   79,   78,    0,  232,   79,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   80,    0,   81,    0,    0,    0,   74,   75,  206,
  206,    0,    0,   76,   77,    0,   78,    0,   80,   79,
   81,  234,  234,  234,  234,  234,  234,    0,    0,  208,
  234,    0,  234,  234,    0,    0,  234,    0,    0,    0,
    0,    0,  234,  234,  234,  234,  234,  234,  234,  234,
  234,  234,  234,  234,  234,  234,  234,  234,  234,  234,
  234,  234,  208,    0,    0,    0,    0,    0,    0,    0,
  208,    0,  208,  208,  208,    0,  208,    0,    0,    0,
    0,    0,    0,  616,    0,    0,    0,    0,  208,  208,
  208,  208,    0,    0,    0,   74,   75,    0,  620,  234,
  234,   76,   77,    0,   78,  234,    0,   79,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  208,    0,    0,    0,    0,    0,    0,  234,
    0,    0,    0,    0,    0,  339,  340,  341,  342,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  208,  208,  206,  206,  206,  206,  206,
  206,    0,  191,  237,  206,    0,  206,  206,    0,  343,
  206,    0,    0,    0,    0,    0,  206,  206,  206,  206,
  206,  206,  206,  206,  206,  206,  206,  206,  206,  206,
  206,  206,  206,  206,  206,  206,  237,    0,  344,  200,
  201,  202,    0,    0,  237,    0,  237,  237,  237,    0,
  237,  345,  346,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  237,  237,  237,  237,    0,    0,    0,   74,
   75,    0,    0,  206,  206,   76,   77,    0,   78,  206,
    0,   79,    0,    0,    0,    0,   74,   75,    0,    0,
    0,    0,   76,   77,    0,   78,  237,    0,   79,    0,
    0,    0,    0,  206,    1,    2,    3,    4,    5,    6,
    7,    8,    9,   10,   11,   12,   13,   14,   15,   16,
   17,    0,    0,    0,    0,    0,    0,  237,  237,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  208,
  208,  208,  208,  208,  208,    0,    0,  332,  208,    0,
  208,  208,    0,    0,  208,    0,    0,    0,    0,    0,
  208,  208,  208,  208,  208,  208,  208,  208,  208,  208,
  208,  208,  208,  208,  208,  208,  208,  208,  208,  208,
  332,    0,    0,    0,    0,    0,    0,    0,  332,    0,
  332,  332,  332,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  332,  332,  332,  332,  332,
    0,    0,    0,    0,    0,    0,    0,  208,  208,    0,
    0,    0,    0,  208,    2,    3,    4,    5,    6,    7,
    8,    9,   10,   11,   12,   13,   14,   15,   16,   17,
  332,    0,    0,    0,    0,    0,    0,  208,    3,    4,
    5,    6,    7,    8,    9,   10,   11,   12,   13,   14,
   15,   16,   17,    0,    0,    0,    0,    0,    0,    0,
  332,  332,  332,  237,  237,  237,  237,  237,  237,    0,
    0,  207,  237,    0,  237,  237,    0,    0,  237,    0,
    0,    0,    0,    0,  237,  237,  237,  237,  237,  237,
  237,  237,  237,  237,  237,  237,  237,  237,  237,  237,
  237,  237,  237,  237,  207,    0,    0,    0,    0,    0,
    0,    0,  207,    0,  207,  207,  207,    0,  207,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  207,  207,  207,  207,    0,    0,    0,    0,    0,    0,
    0,  237,  237,    0,    0,    0,    0,  237,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  207,    0,    0,    0,    0,    0,
    0,  237,    0,    0,    0,    0,    0,    0,    0,    0,
   88,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  207,  207,    0,    0,    0,
    0,    0,    0,  332,  332,    0,  332,  332,  332,  332,
  332,  332,  332,    0,    0,  209,  332,    0,  332,  332,
    0,   88,  332,    0,   88,    0,    0,    0,  332,  332,
  332,  332,  332,  332,  332,  332,  332,  332,    0,   88,
    0,  332,  332,  332,  332,  332,  332,  332,  209,    0,
    0,    0,    0,    0,    0,    0,  209,    0,  209,  209,
  209,    0,  209,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   88,  209,  209,  209,  209,    0,    0,
    0,    0,    0,    0,    0,  332,  332,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   88,    0,    0,  209,    0,
    0,    0,    0,    0,    0,  332,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  209,
  209,  207,  207,  207,  207,  207,  207,    0,    0,  238,
  207,    0,  207,  207,    0,    0,  207,    0,    0,    0,
    0,    0,  207,  207,  207,  207,  207,  207,  207,  207,
  207,  207,  207,  207,  207,  207,  207,  207,  207,  207,
  207,  207,  238,    0,    0,    0,    0,    0,    0,    0,
  238,    0,  238,  238,  238,    0,  238,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  238,  238,
  238,  238,    0,    0,    0,    0,    0,    0,    0,  207,
  207,    0,    0,    0,    0,  207,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  238,    0,    0,    0,    0,    0,    0,  207,
   88,   88,   88,   88,   88,   88,    0,    0,    0,   88,
    0,   88,   88,    0,    0,   88,    0,    0,    0,    0,
    0,    0,    0,  238,  238,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  209,  209,  209,  209,  209,
  209,    0,    0,  197,  209,    0,  209,  209,    0,    0,
  209,    0,    0,    0,    0,    0,  209,  209,  209,  209,
  209,  209,  209,  209,  209,  209,  209,  209,  209,  209,
  209,  209,  209,  209,  209,  209,  197,    0,   88,   88,
    0,    0,    0,    0,  197,    0,  197,  197,  197,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  197,  197,  197,  197,    0,    0,    0,    0,
    0,    0,    0,  209,  209,    0,    0,    0,    0,  209,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  197,    0,    0,    0,
    0,    0,    0,  209,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  197,  197,  238,
  238,  238,  238,  238,  238,    0,    0,  200,  238,    0,
  238,  238,    0,    0,  238,    0,    0,    0,    0,    0,
  238,  238,  238,  238,  238,  238,  238,  238,  238,  238,
  238,  238,  238,  238,  238,  238,  238,  238,  238,  238,
  200,    0,    0,    0,    0,    0,    0,    0,  200,    0,
  200,  200,  200,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  200,  200,  200,  200,
    0,    0,    0,    0,    0,    0,    0,  238,  238,    0,
    0,    0,    0,  238,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  200,    0,    0,    0,    0,    0,    0,  238,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  200,  200,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  197,  197,  197,  197,  197,  197,    0,
    0,  199,  197,    0,  197,  197,    0,    0,  197,    0,
    0,    0,    0,    0,  197,  197,  197,  197,  197,  197,
  197,  197,  197,  197,  197,  197,  197,  197,  197,  197,
  197,  197,  197,  197,  199,    0,    0,    0,    0,    0,
    0,    0,  199,    0,  199,  199,  199,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  199,  199,  199,  199,    0,    0,    0,    0,    0,    0,
    0,  197,  197,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  199,    0,    0,    0,    0,    0,
    0,  197,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  199,  199,  200,  200,  200,
  200,  200,  200,    0,    0,  198,  200,    0,  200,  200,
    0,    0,  200,    0,    0,    0,    0,    0,  200,  200,
  200,  200,  200,  200,  200,  200,  200,  200,  200,  200,
  200,  200,  200,  200,  200,  200,  200,  200,  198,    0,
    0,    0,    0,    0,    0,    0,  198,    0,  198,  198,
  198,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  198,  198,  198,  198,    0,    0,
    0,    0,    0,    0,    0,  200,  200,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  198,    0,
    0,    0,    0,    0,    0,  200,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  198,
  198,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  199,  199,  199,  199,  199,  199,    0,    0,  165,
  199,    0,  199,  199,    0,    0,  199,    0,    0,    0,
    0,    0,  199,  199,  199,  199,  199,  199,  199,  199,
  199,  199,  199,  199,  199,  199,  199,  199,  199,  199,
  199,  199,  165,    0,    0,    0,    0,    0,    0,    0,
  165,    0,  165,  165,  165,  163,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  165,  165,
  165,  165,    0,    0,    0,    0,    0,    0,    0,  199,
  199,    0,    0,    0,    0,    0,    0,    0,  163,    0,
    0,    0,    0,    0,    0,    0,  163,    0,  163,  163,
  163,    0,  165,    0,    0,    0,    0,    0,    0,  199,
    0,    0,    0,    0,  163,  163,  163,  163,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  165,  165,  198,  198,  198,  198,  198,
  198,    0,    0,  329,  198,    0,  198,  198,  163,    0,
  198,    0,    0,    0,    0,    0,  198,  198,  198,  198,
  198,  198,  198,  198,  198,  198,  198,  198,  198,  198,
  198,  198,  198,  198,  198,  198,  329,    0,    0,  163,
  163,    0,    0,    0,  329,    0,  329,  329,  329,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  329,  329,  329,  329,    0,    0,    0,    0,
    0,    0,    0,  198,  198,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  329,    0,    0,    0,
    0,    0,    0,  198,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  329,  329,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  165,
  165,  165,  165,  165,  165,    0,    0,    0,  165,    0,
  165,  165,    0,    0,  165,    0,    0,    0,    0,    0,
  165,  165,  165,  165,  165,  165,  165,  165,  165,  165,
  165,  165,    0,  165,  165,  165,  165,  165,  165,  165,
    0,    0,    0,    0,    0,  163,  163,  163,  163,  163,
  163,    0,    0,  161,  163,    0,  163,  163,    0,    0,
  163,    0,    0,    0,    0,    0,  163,  163,  163,  163,
  163,  163,  163,  163,  163,  163,  163,  165,  165,  163,
  163,  163,  163,  163,  163,  163,  161,    0,    0,    0,
    0,    0,    0,    0,  161,    0,  161,  161,  161,    0,
    0,    0,    0,    0,    0,    0,    0,  165,    0,    0,
    0,    0,  161,  161,  161,  161,    0,    0,    0,    0,
    0,    0,    0,  163,  163,    0,    0,    0,    0,    0,
    0,    0,    0,  329,  329,  329,  329,  329,  329,    0,
    0,  159,  329,    0,  329,  329,  161,    0,  329,    0,
    0,    0,    0,  163,  329,  329,  329,  329,  329,  329,
  329,  329,  329,  329,  329,  329,    0,  329,  329,  329,
  329,  329,  329,  329,  159,    0,    0,  161,  161,    0,
    0,    0,  159,    0,  159,  159,  159,  153,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  159,  159,  159,  159,    0,    0,    0,    0,    0,    0,
    0,  329,  329,    0,    0,    0,    0,    0,    0,    0,
  153,    0,    0,    0,    0,    0,    0,    0,  153,    0,
  153,  153,  153,    0,  159,    0,    0,    0,    0,    0,
    0,  329,    0,    0,    0,    0,  153,  153,  153,  153,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  159,  159,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  153,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  153,  153,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  161,  161,  161,  161,  161,  161,    0,
    0,  154,  161,    0,  161,  161,    0,    0,  161,    0,
    0,    0,    0,    0,  161,  161,  161,  161,  161,  161,
  161,  161,  161,  161,    0,    0,    0,  161,  161,  161,
  161,  161,  161,  161,  154,    0,    0,    0,    0,    0,
    0,    0,  154,    0,  154,  154,  154,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  154,  154,  154,  154,    0,    0,    0,    0,    0,    0,
    0,  161,  161,    0,    0,    0,    0,    0,    0,    0,
    0,  159,  159,  159,  159,  159,  159,    0,    0,    0,
  159,    0,  159,  159,  154,    0,  159,    0,    0,    0,
    0,  161,  159,  159,  159,  159,  159,  159,  159,  159,
  159,    0,    0,    0,    0,  159,  159,  159,  159,  159,
  159,  159,    0,    0,    0,  154,  154,  153,  153,  153,
  153,  153,  153,    0,    0,  155,  153,    0,  153,  153,
    0,    0,  153,    0,    0,    0,    0,    0,  153,  153,
  153,  153,  153,  153,  153,    0,    0,    0,    0,  159,
  159,  153,  153,  153,  153,  153,  153,  153,  155,    0,
    0,    0,    0,    0,    0,    0,  155,    0,  155,  155,
  155,    0,    0,    0,    0,    0,    0,    0,    0,  159,
    0,    0,    0,    0,  155,  155,  155,  155,    0,    0,
    0,    0,    0,    0,    0,  153,  153,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  155,    0,
    0,    0,    0,    0,    0,  153,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  148,  155,
  155,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  154,  154,  154,  154,  154,  154,    0,    0,    0,
  154,    0,  154,  154,    0,    0,  154,    0,    0,    0,
    0,  148,  154,  154,  154,  154,  154,  154,  154,  148,
    0,  148,  148,  148,    0,  154,  154,  154,  154,  154,
  154,  154,    0,    0,    0,    0,    0,  148,  148,  148,
  148,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  154,
  154,  148,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  154,
    0,    0,    0,  148,    0,    0,    0,    0,    0,    0,
    0,    0,  150,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  155,  155,  155,  155,  155,
  155,    0,    0,    0,  155,    0,  155,  155,    0,    0,
  155,    0,    0,    0,    0,  150,  155,  155,  155,  155,
  155,  155,  155,  150,    0,  150,  150,  150,    0,  155,
  155,  155,  155,  155,  155,  155,    0,    0,    0,    0,
    0,  150,  150,  150,  150,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  155,  155,  150,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  155,    0,  151,    0,  150,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  148,  148,
  148,  148,  148,  148,    0,    0,    0,  148,    0,  148,
  148,    0,    0,  148,    0,    0,    0,    0,  151,  148,
  148,  148,  148,  148,  148,    0,  151,    0,  151,  151,
  151,    0,  148,  148,  148,  148,  148,  148,  148,    0,
    0,    0,    0,    0,  151,  151,  151,  151,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  145,    0,
    0,    0,    0,    0,    0,    0,  148,  148,  151,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  145,    0,    0,    0,    0,  148,    0,    0,  145,
  151,  145,  145,  145,    0,    0,    0,    0,    0,  152,
    0,    0,    0,    0,    0,    0,    0,  145,  145,  145,
  145,    0,  150,  150,  150,  150,  150,  150,    0,    0,
    0,  150,    0,  150,  150,    0,    0,  150,    0,    0,
    0,    0,  152,  150,  150,  150,  150,  150,  150,    0,
  152,  145,  152,  152,  152,    0,  150,  150,  150,  150,
  150,  150,  150,    0,    0,    0,    0,    0,  152,  152,
  152,  152,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  145,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  150,  150,  152,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  150,    0,  149,    0,  152,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  151,  151,  151,  151,  151,
  151,    0,    0,    0,  151,    0,  151,  151,    0,    0,
  151,    0,    0,    0,    0,  149,  151,  151,  151,  151,
  151,  151,    0,  149,    0,  149,  149,  149,    0,  151,
  151,  151,  151,  151,  151,  151,    0,    0,    0,    0,
    0,  149,  149,  149,  149,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  146,    0,    0,    0,    0,
    0,    0,    0,  151,  151,  149,    0,    0,  145,  145,
  145,  145,  145,  145,    0,    0,    0,  145,    0,  145,
  145,    0,    0,  145,    0,    0,    0,    0,  146,  145,
  145,  145,    0,  151,    0,    0,  146,  149,  146,  146,
  146,    0,  145,  145,  145,  145,  145,  145,  145,    0,
    0,    0,    0,    0,  146,  146,  146,  146,    0,  152,
  152,  152,  152,  152,  152,    0,    0,  147,  152,    0,
  152,  152,    0,    0,  152,    0,    0,    0,    0,    0,
  152,  152,  152,  152,  152,  152,  145,  145,  146,    0,
    0,    0,    0,  152,  152,  152,  152,  152,  152,  152,
  147,    0,    0,    0,    0,    0,    0,    0,  147,    0,
  147,  147,  147,    0,    0,    0,    0,  143,    0,    0,
  146,    0,    0,    0,    0,    0,  147,  147,  147,  147,
    0,    0,    0,    0,    0,    0,    0,  152,  152,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  143,    0,    0,    0,    0,    0,    0,    0,  143,    0,
  147,  143,    0,    0,    0,    0,    0,  152,    0,    0,
    0,    0,    0,    0,    0,    0,  143,  143,  143,  143,
    0,    0,  149,  149,  149,  149,  149,  149,    0,    0,
    0,  149,  147,  149,  149,    0,    0,  149,    0,    0,
    0,    0,    0,  149,  149,  149,  149,  149,  149,    0,
  143,    0,    0,    0,    0,    0,  149,  149,  149,  149,
  149,  149,  149,    0,    0,    0,   10,    0,    0,    0,
   10,    0,   10,   10,    0,   10,   10,   10,    0,    0,
    0,    0,  143,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   10,    0,    0,    0,    0,    0,
  149,  149,    0,    0,    0,  146,  146,  146,  146,  146,
  146,    0,    0,    0,  146,    0,  146,  146,    0,    0,
  146,    0,    0,    0,    0,    0,  146,  146,  146,    0,
  149,    0,    0,    0,    0,    0,    0,    0,    0,  146,
  146,  146,  146,  146,  146,  146,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  144,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  147,  147,  147,
  147,  147,  147,  146,  146,    0,  147,    0,  147,  147,
    0,    0,  147,  144,    0,    0,    0,    0,  147,  147,
  147,  144,    0,    0,  144,    0,    0,    0,    0,    0,
    0,  147,  147,  147,  147,  147,  147,  147,    0,  144,
  144,  144,  144,    0,    0,    0,    0,  143,  143,  143,
  143,  143,  143,    0,    0,    0,  143,    0,  143,  143,
    0,    0,  143,    0,    0,    0,    0,    0,  143,  143,
    0,    0,    0,  144,    0,  147,  147,    0,    0,    0,
    0,  143,  143,  143,  143,  143,  143,  143,  173,    0,
    0,    0,  173,    0,  173,  173,    0,  173,  173,  173,
    0,    0,    0,    0,    0,  144,    0,    0,    0,    0,
    0,    0,    0,    0,   10,   10,  173,    0,    0,    0,
    0,   10,   10,    0,    0,  143,  143,   10,   10,    0,
   10,    0,    0,   10,    0,    0,   10,   10,   10,   10,
   10,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   10,    0,    0,   10,   10,   10,   10,   10,   10,
   10,   10,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  175,    0,    0,    0,  175,    0,
  175,  175,    0,  175,  175,  175,    0,    0,    0,    0,
   10,   10,   10,   10,   10,   10,    0,   10,    0,   10,
    0,    0,  175,    0,    0,   10,   10,   10,   10,   10,
   10,   10,   10,   10,   10,   10,   10,   10,    0,    0,
   10,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  144,  144,  144,  144,  144,  144,    0,    0,    0,  144,
    0,  144,  144,    0,    0,  144,    0,    0,    0,    0,
    0,  144,  144,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  144,  144,  144,  144,  144,  144,
  144,    0,    0,    0,    0,    0,    0,    0,  221,    0,
    0,    0,  222,    0,  225,    0,    0,    0,  226,  223,
    0,    0,    0,    0,    0,    0,  173,  173,    0,    0,
    0,    0,    0,    0,    0,    0,  224,    0,  144,  144,
    0,    0,    0,    0,    0,    0,    0,    0,  173,  173,
  173,  173,  173,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  173,    0,    0,  173,  173,  173,  173,
  173,  173,  173,  173,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  173,  173,  173,  173,  173,  173,    0,  173,
    0,  173,  175,  175,    0,    0,    0,  173,  173,  173,
  173,  173,  173,  173,  173,  173,  173,  173,  173,  173,
    0,    0,  173,    0,  175,  175,  175,  175,  175,    0,
    0,    0,  168,    0,    0,    0,  168,    0,  168,    0,
    0,    0,  168,  168,    0,    0,    0,    0,    0,  175,
    0,    0,  175,  175,  175,  175,  175,  175,  175,  175,
  168,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  175,  175,
  175,  175,  175,  175,    0,  175,    0,  175,    0,    0,
    0,    0,    0,  175,  175,  175,  175,  175,  175,  175,
  175,  175,  175,  175,  175,  175,  183,  184,  175,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  185,  186,
  187,  188,  189,    0,    0,    0,  221,    0,    0,    0,
  222,    0,  225,    0,    0,    0,  226,    0,    0,    0,
    0,    0,    0,  190,    0,    0,  191,  192,  193,  194,
  195,  196,  197,  198,  224,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  199,  200,  201,  202,  203,  204,    0,  205,
    0,  206,    0,    0,    0,    0,    0,  207,  208,  209,
  210,  211,  212,  213,  214,  215,  216,  217,  218,  219,
    0,    0,  220,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  168,  168,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  168,  168,  168,  168,  168,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  168,    0,    0,
  168,  168,  168,  168,  168,  168,  168,  168,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  168,  168,  168,  168,
  168,  168,    0,  168,    0,  168,    0,    0,    0,    0,
    0,  168,  168,  168,  168,  168,  168,  168,  168,  168,
  168,  168,  168,  168,  183,  184,  168,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  185,  186,  187,  188,
  189,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  191,  192,  193,  194,  195,  196,
  197,  198,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  199,  200,  201,  202,    0,  204,    0,  205,    0,    0,
    0,    0,    0,    0,    0,  207,  208,  209,  210,  211,
  212,  213,  214,  215,  216,  217,  218,  219,    0,    0,
  220,
  };
  protected static  short [] yyCheck = {            22,
  143,    4,  145,  146,  147,  148,   34,   34,  179,  180,
   13,   14,  180,   34,   47,   41,  123,  123,  125,   44,
   40,    0,  123,   40,  125,   40,   36,   41,   42,   27,
   43,  124,   45,   44,   41,   42,   44,  123,  414,  123,
   44,   41,   47,  123,   60,   48,   44,  123,  263,  125,
   44,   25,   40,   56,   57,  547,   40,   62,  294,  295,
  296,  316,   41,  549,   60,   44,   44,   58,  276,   58,
  324,   44,   59,   44,   44,   44,   44,   44,   44,   44,
   59,  263,  203,   44,   44,  238,   44,   44,   44,   44,
   44,   44,  320,   41,  247,  123,  320,  125,  270,  271,
  272,  273,  223,  123,  358,  591,  123,  123,  123,  125,
  108,  109,  115,  270,   93,   63,  139,  140,  263,  122,
  291,   47,    0,  291,  263,  224,  618,  123,   93,  125,
   47,  321,  347,  107,  316,  123,   62,  345,  346,  123,
  239,  316,  378,  242,  636,   62,  125,  125,  399,    0,
  401,  643,  125,  404,  125,  125,  125,  125,  125,  125,
  303,   36,  305,   41,  125,  125,   44,  125,  125,  125,
  125,  125,  125,  316,   33,  318,  348,  349,  337,  555,
   47,   59,  266,  342,  268,  344,  266,  381,  382,  383,
   41,  348,  349,   44,  258,   62,  260,  640,  641,  222,
   61,   60,   61,   62,  297,  648,  312,  313,   59,  321,
  298,  654,  300,   41,  302,   93,   44,   59,  325,  326,
  327,  258,    0,  260,  325,  326,  327,  171,  172,  173,
  174,  374,   41,   41,   41,   44,   44,   44,   41,  320,
   41,   44,   93,   44,   43,   41,   45,  125,   44,  325,
  326,  327,  320,   41,  279,   43,   61,   45,   36,  270,
  270,  287,   40,  316,   42,   43,  270,   45,   46,   47,
  257,  279,  379,  359,  125,  266,  270,  266,  385,   44,
  293,  284,  280,  281,  385,  267,   64,  430,   43,  432,
   45,  270,  271,  272,  273,  274,  275,  325,  326,  327,
  279,  321,  281,  282,  321,  320,  285,  321,  384,  325,
  326,  327,  291,  292,  321,  267,  316,  269,  263,  324,
  320,  354,  165,  166,  167,  496,  352,  322,  496,  325,
  326,  327,  355,  356,  357,  506,    0,  360,  506,  355,
  363,  357,  365,   40,  367,  361,   36,  370,  345,  346,
  493,   36,  379,   36,  377,   44,  384,  292,  379,  355,
   40,  357,  505,   40,  380,  361,  509,  264,  265,  348,
  349,  394,   36,  258,  259,  398,   40,  291,   42,   43,
  300,   45,   46,   47,  380,  298,  299,  270,  271,  272,
  273,  534,  270,  271,  272,  273,  274,  275,  261,  262,
   64,  279,  301,  281,  282,  303,  409,  285,   60,   61,
   61,   62,  274,  275,  292,  558,  559,  281,  282,  270,
  271,  272,  273,  274,  275,  302,  449,  316,  279,   61,
  281,  282,  169,  170,  285,   41,  267,   43,  461,   45,
   61,  292,  175,  176,  467,  304,  305,  306,  307,  308,
  309,  310,  177,  178,  625,   80,   81,  625,  396,  397,
   36,  604,  605,   44,  607,  348,  349,  181,  182,  322,
  348,  349,  322,  268,  497,   42,   44,  322,  270,   61,
    0,  321,  123,  271,  272,  123,  264,  265,  123,  277,
  278,   40,  280,  271,  272,  283,  639,  348,  349,  277,
  278,  644,  280,   58,   40,  283,   40,  510,  286,  287,
  288,  289,  290,   40,   40,  321,   36,   58,  316,  316,
   40,   91,   42,   43,  324,   45,   46,   47,  322,   44,
   41,  322,   44,  311,   44,  270,  314,  315,  316,  317,
  318,  319,  320,  321,   64,   40,   40,   40,   40,   40,
   63,  574,  123,   41,  123,  578,   41,  123,   41,  123,
   42,  321,  321,  586,  567,  270,  271,  272,  273,  356,
  321,  324,  350,  351,  352,  353,  354,  355,   43,  357,
   45,  359,  320,   61,   58,  267,  281,  365,  366,  367,
  368,  369,  370,  371,  372,  373,  374,  375,  376,  377,
  264,  265,  380,  269,   41,  284,   41,  271,  272,   41,
   41,   41,   41,  277,  278,  316,  280,  323,  321,  283,
  324,   36,  286,  287,  288,  289,  290,  269,   61,  276,
  321,  358,  360,   43,  123,   45,   36,    0,  125,  123,
   62,  322,  123,  348,  349,  285,  322,  311,  270,   41,
  314,  315,  316,  317,  318,  319,  320,  321,   61,  316,
  125,  125,   47,  268,  322,  271,  272,   41,  125,  362,
   33,  277,  278,  321,  280,   47,  270,  283,   41,  270,
   43,   44,   45,   62,   47,  321,  350,  351,  352,  353,
  354,  355,   62,  357,    0,  359,   59,   60,   61,   62,
  316,  365,  366,  367,  368,  369,  370,  371,  372,  373,
  374,  375,  376,  377,   59,  125,  380,    0,   59,   41,
  287,  288,  289,  290,  321,   59,   41,   58,   91,  269,
   93,  270,  270,   59,  269,  125,   61,   19,   62,   19,
   24,  283,   23,  408,  264,  265,   24,  314,  415,  299,
  413,  271,  272,  320,  321,  149,  301,  277,  278,  148,
  280,  124,  125,  283,   77,  555,  286,  287,  288,  289,
  290,   90,   43,   90,   45,  430,   43,  507,   45,  304,
  144,    0,  153,  350,  351,  352,  353,  168,  231,  182,
  551,  311,  648,  378,  314,  315,  316,  317,  318,  319,
  320,  321,  591,  571,  378,  654,  271,  272,  527,   43,
   -1,   45,  277,  278,   33,  280,  379,   -1,  283,  380,
   -1,   -1,   41,   -1,   43,   44,   45,   -1,   47,   -1,
  350,  351,  352,  353,  354,  355,   -1,  357,   -1,  359,
   59,   60,   61,   62,   -1,  365,  366,  367,  368,  369,
  370,  371,  372,  373,  374,  375,  376,  377,  125,   -1,
  380,  271,  272,   -1,   -1,   -1,   -1,  277,  278,   -1,
  280,   -1,   91,  283,   93,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  287,  288,  289,
  290,  125,   -1,   -1,   -1,   -1,   -1,   43,   -1,   45,
   -1,   -1,   -1,   -1,   -1,  124,  125,  270,  271,  272,
  273,  274,  275,   -1,  314,    0,  279,   -1,  281,  282,
   -1,  321,  285,   -1,   -1,   -1,   -1,   -1,  291,  292,
  293,  294,  295,  296,  297,  298,  299,  300,  301,  302,
  303,  304,  305,  306,  307,  308,  309,  310,   33,   -1,
  350,  351,  352,  353,   -1,   -1,   41,   -1,   43,   44,
   45,   -1,   47,  363,  364,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   59,   60,   61,   62,   -1,  125,
   -1,   -1,   -1,   -1,   -1,  348,  349,   -1,   -1,   -1,
   -1,  354,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  271,  272,   -1,   -1,  271,  272,  277,  278,   93,  280,
  277,  278,  283,  280,   -1,  378,  283,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   43,   -1,   45,   -1,   -1,   -1,  271,  272,  124,
  125,   -1,   -1,  277,  278,   -1,  280,   -1,   43,  283,
   45,  270,  271,  272,  273,  274,  275,   -1,   -1,    0,
  279,   -1,  281,  282,   -1,   -1,  285,   -1,   -1,   -1,
   -1,   -1,  291,  292,  293,  294,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,  305,  306,  307,  308,
  309,  310,   33,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   41,   -1,   43,   44,   45,   -1,   47,   -1,   -1,   -1,
   -1,   -1,   -1,  125,   -1,   -1,   -1,   -1,   59,   60,
   61,   62,   -1,   -1,   -1,  271,  272,   -1,  123,  348,
  349,  277,  278,   -1,  280,  354,   -1,  283,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   93,   -1,   -1,   -1,   -1,   -1,   -1,  378,
   -1,   -1,   -1,   -1,   -1,  287,  288,  289,  290,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  124,  125,  270,  271,  272,  273,  274,
  275,   -1,  314,    0,  279,   -1,  281,  282,   -1,  321,
  285,   -1,   -1,   -1,   -1,   -1,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,  307,  308,  309,  310,   33,   -1,  350,  351,
  352,  353,   -1,   -1,   41,   -1,   43,   44,   45,   -1,
   47,  363,  364,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   59,   60,   61,   62,   -1,   -1,   -1,  271,
  272,   -1,   -1,  348,  349,  277,  278,   -1,  280,  354,
   -1,  283,   -1,   -1,   -1,   -1,  271,  272,   -1,   -1,
   -1,   -1,  277,  278,   -1,  280,   93,   -1,  283,   -1,
   -1,   -1,   -1,  378,  328,  329,  330,  331,  332,  333,
  334,  335,  336,  337,  338,  339,  340,  341,  342,  343,
  344,   -1,   -1,   -1,   -1,   -1,   -1,  124,  125,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  270,
  271,  272,  273,  274,  275,   -1,   -1,    0,  279,   -1,
  281,  282,   -1,   -1,  285,   -1,   -1,   -1,   -1,   -1,
  291,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,  303,  304,  305,  306,  307,  308,  309,  310,
   33,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   41,   -1,
   43,   44,   45,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   58,   59,   60,   61,   62,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  348,  349,   -1,
   -1,   -1,   -1,  354,  329,  330,  331,  332,  333,  334,
  335,  336,  337,  338,  339,  340,  341,  342,  343,  344,
   93,   -1,   -1,   -1,   -1,   -1,   -1,  378,  330,  331,
  332,  333,  334,  335,  336,  337,  338,  339,  340,  341,
  342,  343,  344,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  123,  124,  125,  270,  271,  272,  273,  274,  275,   -1,
   -1,    0,  279,   -1,  281,  282,   -1,   -1,  285,   -1,
   -1,   -1,   -1,   -1,  291,  292,  293,  294,  295,  296,
  297,  298,  299,  300,  301,  302,  303,  304,  305,  306,
  307,  308,  309,  310,   33,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   41,   -1,   43,   44,   45,   -1,   47,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   59,   60,   61,   62,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  348,  349,   -1,   -1,   -1,   -1,  354,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   93,   -1,   -1,   -1,   -1,   -1,
   -1,  378,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
    0,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  124,  125,   -1,   -1,   -1,
   -1,   -1,   -1,  266,  267,   -1,  269,  270,  271,  272,
  273,  274,  275,   -1,   -1,    0,  279,   -1,  281,  282,
   -1,   41,  285,   -1,   44,   -1,   -1,   -1,  291,  292,
  293,  294,  295,  296,  297,  298,  299,  300,   -1,   59,
   -1,  304,  305,  306,  307,  308,  309,  310,   33,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   41,   -1,   43,   44,
   45,   -1,   47,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   93,   59,   60,   61,   62,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  348,  349,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  125,   -1,   -1,   93,   -1,
   -1,   -1,   -1,   -1,   -1,  378,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  124,
  125,  270,  271,  272,  273,  274,  275,   -1,   -1,    0,
  279,   -1,  281,  282,   -1,   -1,  285,   -1,   -1,   -1,
   -1,   -1,  291,  292,  293,  294,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,  305,  306,  307,  308,
  309,  310,   33,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   41,   -1,   43,   44,   45,   -1,   47,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   59,   60,
   61,   62,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  348,
  349,   -1,   -1,   -1,   -1,  354,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   93,   -1,   -1,   -1,   -1,   -1,   -1,  378,
  270,  271,  272,  273,  274,  275,   -1,   -1,   -1,  279,
   -1,  281,  282,   -1,   -1,  285,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  124,  125,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  270,  271,  272,  273,  274,
  275,   -1,   -1,    0,  279,   -1,  281,  282,   -1,   -1,
  285,   -1,   -1,   -1,   -1,   -1,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,  307,  308,  309,  310,   33,   -1,  348,  349,
   -1,   -1,   -1,   -1,   41,   -1,   43,   44,   45,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   59,   60,   61,   62,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  348,  349,   -1,   -1,   -1,   -1,  354,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   93,   -1,   -1,   -1,
   -1,   -1,   -1,  378,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  124,  125,  270,
  271,  272,  273,  274,  275,   -1,   -1,    0,  279,   -1,
  281,  282,   -1,   -1,  285,   -1,   -1,   -1,   -1,   -1,
  291,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,  303,  304,  305,  306,  307,  308,  309,  310,
   33,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   41,   -1,
   43,   44,   45,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   59,   60,   61,   62,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  348,  349,   -1,
   -1,   -1,   -1,  354,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   93,   -1,   -1,   -1,   -1,   -1,   -1,  378,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  124,  125,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  270,  271,  272,  273,  274,  275,   -1,
   -1,    0,  279,   -1,  281,  282,   -1,   -1,  285,   -1,
   -1,   -1,   -1,   -1,  291,  292,  293,  294,  295,  296,
  297,  298,  299,  300,  301,  302,  303,  304,  305,  306,
  307,  308,  309,  310,   33,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   41,   -1,   43,   44,   45,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   59,   60,   61,   62,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  348,  349,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   93,   -1,   -1,   -1,   -1,   -1,
   -1,  378,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  124,  125,  270,  271,  272,
  273,  274,  275,   -1,   -1,    0,  279,   -1,  281,  282,
   -1,   -1,  285,   -1,   -1,   -1,   -1,   -1,  291,  292,
  293,  294,  295,  296,  297,  298,  299,  300,  301,  302,
  303,  304,  305,  306,  307,  308,  309,  310,   33,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   41,   -1,   43,   44,
   45,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   59,   60,   61,   62,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  348,  349,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   93,   -1,
   -1,   -1,   -1,   -1,   -1,  378,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  124,
  125,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  270,  271,  272,  273,  274,  275,   -1,   -1,    0,
  279,   -1,  281,  282,   -1,   -1,  285,   -1,   -1,   -1,
   -1,   -1,  291,  292,  293,  294,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,  305,  306,  307,  308,
  309,  310,   33,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   41,   -1,   43,   44,   45,    0,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   59,   60,
   61,   62,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  348,
  349,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   33,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   41,   -1,   43,   44,
   45,   -1,   93,   -1,   -1,   -1,   -1,   -1,   -1,  378,
   -1,   -1,   -1,   -1,   59,   60,   61,   62,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  124,  125,  270,  271,  272,  273,  274,
  275,   -1,   -1,    0,  279,   -1,  281,  282,   93,   -1,
  285,   -1,   -1,   -1,   -1,   -1,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,  307,  308,  309,  310,   33,   -1,   -1,  124,
  125,   -1,   -1,   -1,   41,   -1,   43,   44,   45,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   59,   60,   61,   62,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  348,  349,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   93,   -1,   -1,   -1,
   -1,   -1,   -1,  378,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  124,  125,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  270,
  271,  272,  273,  274,  275,   -1,   -1,   -1,  279,   -1,
  281,  282,   -1,   -1,  285,   -1,   -1,   -1,   -1,   -1,
  291,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,   -1,  304,  305,  306,  307,  308,  309,  310,
   -1,   -1,   -1,   -1,   -1,  270,  271,  272,  273,  274,
  275,   -1,   -1,    0,  279,   -1,  281,  282,   -1,   -1,
  285,   -1,   -1,   -1,   -1,   -1,  291,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  348,  349,  304,
  305,  306,  307,  308,  309,  310,   33,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   41,   -1,   43,   44,   45,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  378,   -1,   -1,
   -1,   -1,   59,   60,   61,   62,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  348,  349,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  270,  271,  272,  273,  274,  275,   -1,
   -1,    0,  279,   -1,  281,  282,   93,   -1,  285,   -1,
   -1,   -1,   -1,  378,  291,  292,  293,  294,  295,  296,
  297,  298,  299,  300,  301,  302,   -1,  304,  305,  306,
  307,  308,  309,  310,   33,   -1,   -1,  124,  125,   -1,
   -1,   -1,   41,   -1,   43,   44,   45,    0,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   59,   60,   61,   62,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  348,  349,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   33,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   41,   -1,
   43,   44,   45,   -1,   93,   -1,   -1,   -1,   -1,   -1,
   -1,  378,   -1,   -1,   -1,   -1,   59,   60,   61,   62,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  124,  125,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   93,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  124,  125,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  270,  271,  272,  273,  274,  275,   -1,
   -1,    0,  279,   -1,  281,  282,   -1,   -1,  285,   -1,
   -1,   -1,   -1,   -1,  291,  292,  293,  294,  295,  296,
  297,  298,  299,  300,   -1,   -1,   -1,  304,  305,  306,
  307,  308,  309,  310,   33,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   41,   -1,   43,   44,   45,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   59,   60,   61,   62,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  348,  349,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  270,  271,  272,  273,  274,  275,   -1,   -1,   -1,
  279,   -1,  281,  282,   93,   -1,  285,   -1,   -1,   -1,
   -1,  378,  291,  292,  293,  294,  295,  296,  297,  298,
  299,   -1,   -1,   -1,   -1,  304,  305,  306,  307,  308,
  309,  310,   -1,   -1,   -1,  124,  125,  270,  271,  272,
  273,  274,  275,   -1,   -1,    0,  279,   -1,  281,  282,
   -1,   -1,  285,   -1,   -1,   -1,   -1,   -1,  291,  292,
  293,  294,  295,  296,  297,   -1,   -1,   -1,   -1,  348,
  349,  304,  305,  306,  307,  308,  309,  310,   33,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   41,   -1,   43,   44,
   45,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  378,
   -1,   -1,   -1,   -1,   59,   60,   61,   62,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  348,  349,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   93,   -1,
   -1,   -1,   -1,   -1,   -1,  378,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,    0,  124,
  125,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  270,  271,  272,  273,  274,  275,   -1,   -1,   -1,
  279,   -1,  281,  282,   -1,   -1,  285,   -1,   -1,   -1,
   -1,   33,  291,  292,  293,  294,  295,  296,  297,   41,
   -1,   43,   44,   45,   -1,  304,  305,  306,  307,  308,
  309,  310,   -1,   -1,   -1,   -1,   -1,   59,   60,   61,
   62,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  348,
  349,   93,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  378,
   -1,   -1,   -1,  125,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,    0,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
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
   -1,   -1,   -1,  378,   -1,    0,   -1,  125,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  270,  271,
  272,  273,  274,  275,   -1,   -1,   -1,  279,   -1,  281,
  282,   -1,   -1,  285,   -1,   -1,   -1,   -1,   33,  291,
  292,  293,  294,  295,  296,   -1,   41,   -1,   43,   44,
   45,   -1,  304,  305,  306,  307,  308,  309,  310,   -1,
   -1,   -1,   -1,   -1,   59,   60,   61,   62,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,    0,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  348,  349,   93,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   33,   -1,   -1,   -1,   -1,  378,   -1,   -1,   41,
  125,   43,   44,   45,   -1,   -1,   -1,   -1,   -1,    0,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   59,   60,   61,
   62,   -1,  270,  271,  272,  273,  274,  275,   -1,   -1,
   -1,  279,   -1,  281,  282,   -1,   -1,  285,   -1,   -1,
   -1,   -1,   33,  291,  292,  293,  294,  295,  296,   -1,
   41,   93,   43,   44,   45,   -1,  304,  305,  306,  307,
  308,  309,  310,   -1,   -1,   -1,   -1,   -1,   59,   60,
   61,   62,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  125,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  348,  349,   93,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  378,   -1,    0,   -1,  125,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  270,  271,  272,  273,  274,
  275,   -1,   -1,   -1,  279,   -1,  281,  282,   -1,   -1,
  285,   -1,   -1,   -1,   -1,   33,  291,  292,  293,  294,
  295,  296,   -1,   41,   -1,   43,   44,   45,   -1,  304,
  305,  306,  307,  308,  309,  310,   -1,   -1,   -1,   -1,
   -1,   59,   60,   61,   62,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,    0,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  348,  349,   93,   -1,   -1,  270,  271,
  272,  273,  274,  275,   -1,   -1,   -1,  279,   -1,  281,
  282,   -1,   -1,  285,   -1,   -1,   -1,   -1,   33,  291,
  292,  293,   -1,  378,   -1,   -1,   41,  125,   43,   44,
   45,   -1,  304,  305,  306,  307,  308,  309,  310,   -1,
   -1,   -1,   -1,   -1,   59,   60,   61,   62,   -1,  270,
  271,  272,  273,  274,  275,   -1,   -1,    0,  279,   -1,
  281,  282,   -1,   -1,  285,   -1,   -1,   -1,   -1,   -1,
  291,  292,  293,  294,  295,  296,  348,  349,   93,   -1,
   -1,   -1,   -1,  304,  305,  306,  307,  308,  309,  310,
   33,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   41,   -1,
   43,   44,   45,   -1,   -1,   -1,   -1,    0,   -1,   -1,
  125,   -1,   -1,   -1,   -1,   -1,   59,   60,   61,   62,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  348,  349,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   33,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   41,   -1,
   93,   44,   -1,   -1,   -1,   -1,   -1,  378,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   59,   60,   61,   62,
   -1,   -1,  270,  271,  272,  273,  274,  275,   -1,   -1,
   -1,  279,  125,  281,  282,   -1,   -1,  285,   -1,   -1,
   -1,   -1,   -1,  291,  292,  293,  294,  295,  296,   -1,
   93,   -1,   -1,   -1,   -1,   -1,  304,  305,  306,  307,
  308,  309,  310,   -1,   -1,   -1,   36,   -1,   -1,   -1,
   40,   -1,   42,   43,   -1,   45,   46,   47,   -1,   -1,
   -1,   -1,  125,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   64,   -1,   -1,   -1,   -1,   -1,
  348,  349,   -1,   -1,   -1,  270,  271,  272,  273,  274,
  275,   -1,   -1,   -1,  279,   -1,  281,  282,   -1,   -1,
  285,   -1,   -1,   -1,   -1,   -1,  291,  292,  293,   -1,
  378,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  304,
  305,  306,  307,  308,  309,  310,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
    0,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  270,  271,  272,
  273,  274,  275,  348,  349,   -1,  279,   -1,  281,  282,
   -1,   -1,  285,   33,   -1,   -1,   -1,   -1,  291,  292,
  293,   41,   -1,   -1,   44,   -1,   -1,   -1,   -1,   -1,
   -1,  304,  305,  306,  307,  308,  309,  310,   -1,   59,
   60,   61,   62,   -1,   -1,   -1,   -1,  270,  271,  272,
  273,  274,  275,   -1,   -1,   -1,  279,   -1,  281,  282,
   -1,   -1,  285,   -1,   -1,   -1,   -1,   -1,  291,  292,
   -1,   -1,   -1,   93,   -1,  348,  349,   -1,   -1,   -1,
   -1,  304,  305,  306,  307,  308,  309,  310,   36,   -1,
   -1,   -1,   40,   -1,   42,   43,   -1,   45,   46,   47,
   -1,   -1,   -1,   -1,   -1,  125,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  264,  265,   64,   -1,   -1,   -1,
   -1,  271,  272,   -1,   -1,  348,  349,  277,  278,   -1,
  280,   -1,   -1,  283,   -1,   -1,  286,  287,  288,  289,
  290,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  311,   -1,   -1,  314,  315,  316,  317,  318,  319,
  320,  321,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   36,   -1,   -1,   -1,   40,   -1,
   42,   43,   -1,   45,   46,   47,   -1,   -1,   -1,   -1,
  350,  351,  352,  353,  354,  355,   -1,  357,   -1,  359,
   -1,   -1,   64,   -1,   -1,  365,  366,  367,  368,  369,
  370,  371,  372,  373,  374,  375,  376,  377,   -1,   -1,
  380,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  270,  271,  272,  273,  274,  275,   -1,   -1,   -1,  279,
   -1,  281,  282,   -1,   -1,  285,   -1,   -1,   -1,   -1,
   -1,  291,  292,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  304,  305,  306,  307,  308,  309,
  310,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   36,   -1,
   -1,   -1,   40,   -1,   42,   -1,   -1,   -1,   46,   47,
   -1,   -1,   -1,   -1,   -1,   -1,  264,  265,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   64,   -1,  348,  349,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  286,  287,
  288,  289,  290,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  311,   -1,   -1,  314,  315,  316,  317,
  318,  319,  320,  321,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  350,  351,  352,  353,  354,  355,   -1,  357,
   -1,  359,  264,  265,   -1,   -1,   -1,  365,  366,  367,
  368,  369,  370,  371,  372,  373,  374,  375,  376,  377,
   -1,   -1,  380,   -1,  286,  287,  288,  289,  290,   -1,
   -1,   -1,   36,   -1,   -1,   -1,   40,   -1,   42,   -1,
   -1,   -1,   46,   47,   -1,   -1,   -1,   -1,   -1,  311,
   -1,   -1,  314,  315,  316,  317,  318,  319,  320,  321,
   64,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  350,  351,
  352,  353,  354,  355,   -1,  357,   -1,  359,   -1,   -1,
   -1,   -1,   -1,  365,  366,  367,  368,  369,  370,  371,
  372,  373,  374,  375,  376,  377,  264,  265,  380,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  286,  287,
  288,  289,  290,   -1,   -1,   -1,   36,   -1,   -1,   -1,
   40,   -1,   42,   -1,   -1,   -1,   46,   -1,   -1,   -1,
   -1,   -1,   -1,  311,   -1,   -1,  314,  315,  316,  317,
  318,  319,  320,  321,   64,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  350,  351,  352,  353,  354,  355,   -1,  357,
   -1,  359,   -1,   -1,   -1,   -1,   -1,  365,  366,  367,
  368,  369,  370,  371,  372,  373,  374,  375,  376,  377,
   -1,   -1,  380,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  264,  265,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  286,  287,  288,  289,  290,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  311,   -1,   -1,
  314,  315,  316,  317,  318,  319,  320,  321,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  350,  351,  352,  353,
  354,  355,   -1,  357,   -1,  359,   -1,   -1,   -1,   -1,
   -1,  365,  366,  367,  368,  369,  370,  371,  372,  373,
  374,  375,  376,  377,  264,  265,  380,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  286,  287,  288,  289,
  290,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  314,  315,  316,  317,  318,  319,
  320,  321,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  350,  351,  352,  353,   -1,  355,   -1,  357,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  365,  366,  367,  368,  369,
  370,  371,  372,  373,  374,  375,  376,  377,   -1,   -1,
  380,
  };

#line 1945 "XQuery.y"
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
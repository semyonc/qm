
// created by jay 0.7 (c) 1998 Axel.Schreiner@informatik.uni-osnabrueck.de

#line 35 "XQuery.y"

#pragma warning disable 162

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
//t    "OrderModifier : COLLATION URILiteral",
//t    "OrderModifier : OrderDirection EmptyOrderSpec",
//t    "OrderModifier : OrderDirection COLLATION URILiteral",
//t    "OrderModifier : OrderDirection EmptyOrderSpec COLLATION URILiteral",
//t    "OrderModifier : EmptyOrderSpec",
//t    "OrderModifier : EmptyOrderSpec COLLATION URILiteral",
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
//t    "DirElemConstructor : BeginTag QName S '/' '>'",
//t    "DirElemConstructor : BeginTag QName opt_DirAttributeList '>' '<' '/' QName opt_S '>'",
//t    "DirElemConstructor : BeginTag QName S '>' '<' '/' QName opt_S '>'",
//t    "DirElemConstructor : BeginTag QName opt_DirAttributeList '>' DirElemContentList '<' '/' QName opt_S '>'",
//t    "DirElemConstructor : BeginTag QName S '>' DirElemContentList '<' '/' QName opt_S '>'",
//t    "DirElemContentList : DirElemContent",
//t    "DirElemContentList : DirElemContentList DirElemContent",
//t    "opt_DirAttributeList :",
//t    "opt_DirAttributeList : DirAttributeList",
//t    "DirAttributeList : S DirAttribute",
//t    "DirAttributeList : DirAttributeList S",
//t    "DirAttributeList : DirAttributeList S DirAttribute",
//t    "DirAttribute : QName opt_S '=' opt_S '\"' '\"'",
//t    "DirAttribute : QName opt_S '=' opt_S '\"' DirAttributeValueQuot '\"'",
//t    "DirAttribute : QName opt_S '=' opt_S Apos Apos",
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
//t    "SequenceType : EMPTY_SEQUENCE",
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
    "CDATA_END","EMPTY_SEQUENCE","ITEM","AXIS_CHILD","AXIS_DESCENDANT",
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
#line 216 "XQuery.y"
  {
     notation.ConfirmTag(Tag.Module, Descriptor.Root, yyVals[0+yyTop]);
     yyVal = notation.ResolveTag(Tag.Module);	 
  }
  break;
case 2:
#line 221 "XQuery.y"
  {
	 notation.ConfirmTag(Tag.Module, Descriptor.Root, yyVals[0+yyTop]);	 
	 yyVal = notation.ResolveTag(Tag.Module);	 
  }
  break;
case 3:
#line 226 "XQuery.y"
  {
     notation.ConfirmTag(Tag.Module, Descriptor.Root, yyVals[0+yyTop]);	 
     yyVal = notation.ResolveTag(Tag.Module);	 
  }
  break;
case 4:
#line 231 "XQuery.y"
  {
	 notation.ConfirmTag(Tag.Module, Descriptor.Root, yyVals[0+yyTop]);	 
	 yyVal = notation.ResolveTag(Tag.Module);	 
  }
  break;
case 5:
#line 239 "XQuery.y"
  {
     notation.ConfirmTag(Tag.Module, Descriptor.Version, yyVals[-1+yyTop], null);
  }
  break;
case 6:
#line 243 "XQuery.y"
  {
     notation.ConfirmTag(Tag.Module, Descriptor.Version, yyVals[-3+yyTop], yyVals[-1+yyTop]);
  }
  break;
case 7:
#line 250 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.Query, yyVals[-1+yyTop], yyVals[0+yyTop]);
  }
  break;
case 8:
#line 257 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.Library, yyVals[-1+yyTop], yyVals[0+yyTop]);
  }
  break;
case 9:
#line 264 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.ModuleNamespace, yyVals[-3+yyTop], yyVals[-1+yyTop]);
  }
  break;
case 10:
#line 271 "XQuery.y"
  {
     yyVal = null;
  }
  break;
case 13:
#line 277 "XQuery.y"
  {
     yyVal = Lisp.Append(yyVals[-1+yyTop], yyVals[0+yyTop]);
  }
  break;
case 14:
#line 284 "XQuery.y"
  {
      yyVal = Lisp.Cons(yyVals[-1+yyTop]);
   }
  break;
case 15:
#line 288 "XQuery.y"
  {
      yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[-1+yyTop]));
   }
  break;
case 16:
#line 295 "XQuery.y"
  {
      yyVal = Lisp.Cons(yyVals[-1+yyTop]);
   }
  break;
case 17:
#line 299 "XQuery.y"
  {
      yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[-1+yyTop]));
   }
  break;
case 35:
#line 338 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.Namespace, yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 36:
#line 345 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Module), 
		Descriptor.BoundarySpace, new TokenWrapper(Token.PRESERVE));
  }
  break;
case 37:
#line 350 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Module), 
		Descriptor.BoundarySpace, new TokenWrapper(Token.STRIP));  
  }
  break;
case 38:
#line 358 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.DefaultElement, yyVals[0+yyTop]);
  }
  break;
case 39:
#line 362 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.DefaultFunction, yyVals[0+yyTop]);
  }
  break;
case 40:
#line 369 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.OptionDecl, yyVals[-1+yyTop], yyVals[0+yyTop]);
  }
  break;
case 41:
#line 376 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Module), 
		Descriptor.Ordering, new TokenWrapper(Token.ORDERED));  
  }
  break;
case 42:
#line 381 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Module), 
		Descriptor.Ordering, new TokenWrapper(Token.UNORDERED));  
  }
  break;
case 43:
#line 389 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Module), 
		Descriptor.DefaultOrder, new TokenWrapper(Token.EMPTY_GREATEST));  
  }
  break;
case 44:
#line 394 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Module), 
		Descriptor.DefaultOrder, new TokenWrapper(Token.EMPTY_LEAST));  
  }
  break;
case 45:
#line 402 "XQuery.y"
  {
	  yyVal = notation.Confirm(new Symbol(Tag.Module), 
	    Descriptor.CopyNamespace, yyVals[-2+yyTop], yyVals[0+yyTop]); 
  }
  break;
case 46:
#line 410 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.PRESERVE);
  }
  break;
case 47:
#line 414 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.NO_PRESERVE);
  }
  break;
case 48:
#line 421 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.INHERIT);
  }
  break;
case 49:
#line 425 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.NO_INHERIT);
  }
  break;
case 50:
#line 432 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.DefaultCollation, yyVals[0+yyTop]);
  }
  break;
case 51:
#line 439 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.BaseUri, yyVals[0+yyTop]);
  }
  break;
case 52:
#line 446 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Module), 
         Descriptor.ImportSchema, yyVals[-1+yyTop], yyVals[0+yyTop], null);
  }
  break;
case 53:
#line 451 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Module), 
         Descriptor.ImportSchema, yyVals[-3+yyTop], yyVals[-2+yyTop], yyVals[0+yyTop]);  
  }
  break;
case 54:
#line 459 "XQuery.y"
  { 
     yyVal = null;
  }
  break;
case 56:
#line 467 "XQuery.y"
  {
     yyVal = Lisp.Cons(yyVals[0+yyTop]);
  }
  break;
case 57:
#line 471 "XQuery.y"
  {
     yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
  }
  break;
case 58:
#line 478 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.Namespace, yyVals[-1+yyTop]);
  }
  break;
case 59:
#line 482 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.DefaultElement);
  }
  break;
case 60:
#line 489 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.ImportModule, yyVals[-1+yyTop], yyVals[0+yyTop]);
  }
  break;
case 61:
#line 493 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.ImportModule, yyVals[-3+yyTop], yyVals[-1+yyTop], yyVals[0+yyTop]);
  }
  break;
case 62:
#line 500 "XQuery.y"
  {
     yyVal = null;
  }
  break;
case 63:
#line 504 "XQuery.y"
  {
     yyVal = yyVals[0+yyTop];
  }
  break;
case 64:
#line 510 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.VarDecl, yyVals[-4+yyTop], yyVals[-3+yyTop], yyVals[0+yyTop]); 
  }
  break;
case 65:
#line 514 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.VarDecl, yyVals[-2+yyTop], yyVals[-1+yyTop]); 
  }
  break;
case 66:
#line 521 "XQuery.y"
  {
     yyVal = null;
  }
  break;
case 68:
#line 529 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.ConstructionDecl, 
		new TokenWrapper(Token.PRESERVE));
  }
  break;
case 69:
#line 534 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.ConstructionDecl, 
		new TokenWrapper(Token.STRIP));
  }
  break;
case 70:
#line 542 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.DeclareFunction, yyVals[-4+yyTop], yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 71:
#line 546 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.DeclareFunction, yyVals[-6+yyTop], yyVals[-4+yyTop], yyVals[-1+yyTop], yyVals[0+yyTop]);
  }
  break;
case 73:
#line 554 "XQuery.y"
  {
     yyVal = null;
  }
  break;
case 74:
#line 561 "XQuery.y"
  {
     yyVal = null;
  }
  break;
case 76:
#line 569 "XQuery.y"
  {
     yyVal = Lisp.Cons(yyVals[0+yyTop]);
  }
  break;
case 77:
#line 573 "XQuery.y"
  {
     yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
  }
  break;
case 78:
#line 580 "XQuery.y"
  {
     yyVal = yyVals[0+yyTop];
  }
  break;
case 79:
#line 584 "XQuery.y"
  {
     yyVal = yyVals[-1+yyTop];
     notation.Confirm((Symbol)yyVals[-1+yyTop], Descriptor.TypeDecl, yyVals[0+yyTop]);
  }
  break;
case 80:
#line 592 "XQuery.y"
  {
     yyVal = yyVals[-1+yyTop];
  }
  break;
case 82:
#line 603 "XQuery.y"
  {
     yyVal = Lisp.Cons(yyVals[0+yyTop]);
  }
  break;
case 83:
#line 607 "XQuery.y"
  {
     yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
  }
  break;
case 89:
#line 622 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.FLWORExpr, yyVals[-2+yyTop], null, null, yyVals[0+yyTop]);
  }
  break;
case 90:
#line 626 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.FLWORExpr, yyVals[-3+yyTop], yyVals[-2+yyTop], null, yyVals[0+yyTop]);
  }
  break;
case 91:
#line 630 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.FLWORExpr, yyVals[-3+yyTop], null, yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 92:
#line 634 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.FLWORExpr, yyVals[-4+yyTop], yyVals[-3+yyTop], yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 93:
#line 641 "XQuery.y"
  {
     yyVal = Lisp.Cons(yyVals[0+yyTop]);
  }
  break;
case 94:
#line 645 "XQuery.y"
  {
     yyVal = Lisp.Cons(yyVals[0+yyTop]);
  }
  break;
case 95:
#line 649 "XQuery.y"
  {
     yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(yyVals[0+yyTop]));
  }
  break;
case 96:
#line 653 "XQuery.y"
  {
     yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(yyVals[0+yyTop]));
  }
  break;
case 97:
#line 660 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.For, yyVals[0+yyTop]);
  }
  break;
case 98:
#line 667 "XQuery.y"
  {
     yyVal = Lisp.Cons(yyVals[0+yyTop]);
  }
  break;
case 99:
#line 671 "XQuery.y"
  {
     yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
  }
  break;
case 100:
#line 678 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.ForClauseOperator, yyVals[-4+yyTop], yyVals[-3+yyTop], yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 101:
#line 685 "XQuery.y"
  {
     yyVal = null;
  }
  break;
case 103:
#line 693 "XQuery.y"
  {
     yyVal = yyVals[0+yyTop];
  }
  break;
case 104:
#line 700 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Let, yyVals[0+yyTop]);
  }
  break;
case 105:
#line 707 "XQuery.y"
  {
     yyVal = Lisp.Cons(yyVals[0+yyTop]);
  }
  break;
case 106:
#line 711 "XQuery.y"
  {
     yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
  }
  break;
case 107:
#line 718 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.LetClauseOperator, yyVals[-4+yyTop], yyVals[-3+yyTop], yyVals[0+yyTop]);
  }
  break;
case 108:
#line 725 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Where, yyVals[0+yyTop]);
  }
  break;
case 109:
#line 732 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.OrderBy, yyVals[0+yyTop]);
  }
  break;
case 110:
#line 736 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.StableOrderBy, yyVals[0+yyTop]);
  }
  break;
case 111:
#line 743 "XQuery.y"
  {
     yyVal = Lisp.Cons(yyVals[0+yyTop]);
  }
  break;
case 112:
#line 747 "XQuery.y"
  {
     yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
  }
  break;
case 114:
#line 755 "XQuery.y"
  {
     yyVal = yyVals[-1+yyTop];
     notation.Confirm((Symbol)yyVals[-1+yyTop], Descriptor.Modifier, yyVals[0+yyTop]);
  }
  break;
case 115:
#line 763 "XQuery.y"
  {
     yyVal = Lisp.List(yyVals[0+yyTop], null, null);
  }
  break;
case 116:
#line 767 "XQuery.y"
  {
     yyVal = Lisp.List(null, null, yyVals[0+yyTop]);
  }
  break;
case 117:
#line 771 "XQuery.y"
  {
     yyVal = Lisp.List(yyVals[-1+yyTop], yyVals[0+yyTop], null);
  }
  break;
case 118:
#line 775 "XQuery.y"
  {
     yyVal = Lisp.List(yyVals[-2+yyTop], null, yyVals[0+yyTop]);
  }
  break;
case 119:
#line 779 "XQuery.y"
  {
     yyVal = Lisp.List(yyVals[-3+yyTop], yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 120:
#line 783 "XQuery.y"
  {
     yyVal = Lisp.List(null, yyVals[0+yyTop], null);
  }
  break;
case 121:
#line 787 "XQuery.y"
  {
     yyVal = Lisp.List(null, yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 122:
#line 794 "XQuery.y"
  {
     yyVal = new TokenWrapper(Token.ASCENDING);
  }
  break;
case 123:
#line 798 "XQuery.y"
  {
     yyVal = new TokenWrapper(Token.DESCENDING);
  }
  break;
case 124:
#line 805 "XQuery.y"
  {
     yyVal = new TokenWrapper(Token.EMPTY_GREATEST); 
  }
  break;
case 125:
#line 809 "XQuery.y"
  {
     yyVal = new TokenWrapper(Token.EMPTY_LEAST); 
  }
  break;
case 126:
#line 816 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Some, yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 127:
#line 820 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Every, yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 128:
#line 827 "XQuery.y"
  {
     yyVal = Lisp.Cons(yyVals[0+yyTop]);
  }
  break;
case 129:
#line 831 "XQuery.y"
  {
     yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
  }
  break;
case 130:
#line 838 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.QuantifiedExprOper, yyVals[-3+yyTop], yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 131:
#line 845 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Typeswitch, yyVals[-5+yyTop], yyVals[-3+yyTop], yyVals[0+yyTop]); 
  }
  break;
case 132:
#line 849 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Typeswitch, yyVals[-7+yyTop], yyVals[-5+yyTop], yyVals[-2+yyTop], yyVals[0+yyTop]); 
  }
  break;
case 133:
#line 856 "XQuery.y"
  {
     yyVal = Lisp.Cons(yyVals[0+yyTop]);
  }
  break;
case 134:
#line 860 "XQuery.y"
  {
     yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(yyVals[0+yyTop]));
  }
  break;
case 135:
#line 867 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Case, yyVals[-4+yyTop], yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 136:
#line 871 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Case, yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 137:
#line 878 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.If, yyVals[-5+yyTop], yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 139:
#line 886 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Or, yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 141:
#line 894 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.And, yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 143:
#line 902 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), 
		Descriptor.ValueComp, yyVals[-2+yyTop], yyVals[-1+yyTop], yyVals[0+yyTop]);
  }
  break;
case 144:
#line 907 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), 
		Descriptor.GeneralComp, yyVals[-2+yyTop], yyVals[-1+yyTop], yyVals[0+yyTop]);
  }
  break;
case 145:
#line 912 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), 
		Descriptor.NodeComp, yyVals[-2+yyTop], yyVals[-1+yyTop], yyVals[0+yyTop]);
  }
  break;
case 147:
#line 921 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.Range, yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 149:
#line 930 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.Add, yyVals[-2+yyTop], new TokenWrapper('+'), yyVals[0+yyTop]);
  }
  break;
case 150:
#line 935 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.Add, yyVals[-2+yyTop], new TokenWrapper('-'), yyVals[0+yyTop]);
  }
  break;
case 152:
#line 944 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.Mul, yyVals[-2+yyTop], new TokenWrapper(Token.ML), yyVals[0+yyTop]);
  }
  break;
case 153:
#line 949 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.Mul, yyVals[-2+yyTop], new TokenWrapper(Token.DIV), yyVals[0+yyTop]);
  }
  break;
case 154:
#line 954 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.Mul, yyVals[-2+yyTop], new TokenWrapper(Token.IDIV), yyVals[0+yyTop]);
  }
  break;
case 155:
#line 959 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.Mul, yyVals[-2+yyTop], new TokenWrapper(Token.MOD), yyVals[0+yyTop]);
  }
  break;
case 157:
#line 968 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.Union, yyVals[-2+yyTop], yyVals[0+yyTop]);  
  }
  break;
case 158:
#line 973 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.Concatenate, yyVals[-2+yyTop], yyVals[0+yyTop]);  
  }
  break;
case 160:
#line 982 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.IntersectExcept, yyVals[-2+yyTop], new TokenWrapper(Token.INTERSECT), yyVals[0+yyTop]);  
  }
  break;
case 161:
#line 987 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.IntersectExcept, yyVals[-2+yyTop], new TokenWrapper(Token.EXCEPT), yyVals[0+yyTop]);  
  }
  break;
case 163:
#line 996 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.InstanceOf, yyVals[-2+yyTop], yyVals[0+yyTop]);    
  }
  break;
case 165:
#line 1004 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.TreatAs, yyVals[-2+yyTop], yyVals[0+yyTop]);    
  }
  break;
case 167:
#line 1012 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.CastableAs, yyVals[-2+yyTop], yyVals[0+yyTop]);    
  }
  break;
case 169:
#line 1020 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.CastAs, yyVals[-2+yyTop], yyVals[0+yyTop]);    
  }
  break;
case 170:
#line 1027 "XQuery.y"
  {
     if (yyVals[-1+yyTop] == null)
       yyVal = yyVals[0+yyTop];
     else
       yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Unary, yyVals[-1+yyTop], yyVals[0+yyTop]);
  }
  break;
case 171:
#line 1037 "XQuery.y"
  {
     yyVal = null;
  }
  break;
case 172:
#line 1041 "XQuery.y"
  {
     yyVal = Lisp.Append(Lisp.Cons(new TokenWrapper('+')), yyVals[0+yyTop]);
  }
  break;
case 173:
#line 1045 "XQuery.y"
  {
     yyVal = Lisp.Append(Lisp.Cons(new TokenWrapper('-')), yyVals[0+yyTop]);
  }
  break;
case 174:
#line 1052 "XQuery.y"
  {
     yyVal = new Literal("=");
  }
  break;
case 175:
#line 1056 "XQuery.y"
  {
     yyVal = new Literal("!=");
  }
  break;
case 176:
#line 1060 "XQuery.y"
  {
     yyVal = new Literal("<");
  }
  break;
case 177:
#line 1064 "XQuery.y"
  {
     yyVal = new Literal("<=");
  }
  break;
case 178:
#line 1068 "XQuery.y"
  {
     yyVal = new Literal(">");
  }
  break;
case 179:
#line 1072 "XQuery.y"
  {
     yyVal = new Literal(">=");
  }
  break;
case 180:
#line 1079 "XQuery.y"
  {
     yyVal = new TokenWrapper(Token.EQ);
  }
  break;
case 181:
#line 1083 "XQuery.y"
  {
     yyVal = new TokenWrapper(Token.NE);
  }
  break;
case 182:
#line 1087 "XQuery.y"
  {
     yyVal = new TokenWrapper(Token.LT);
  }
  break;
case 183:
#line 1091 "XQuery.y"
  {
     yyVal = new TokenWrapper(Token.LE);
  }
  break;
case 184:
#line 1095 "XQuery.y"
  {
     yyVal = new TokenWrapper(Token.GT);
  }
  break;
case 185:
#line 1099 "XQuery.y"
  {
     yyVal = new TokenWrapper(Token.GE);
  }
  break;
case 186:
#line 1106 "XQuery.y"
  {
     yyVal = new TokenWrapper(Token.IS);
  }
  break;
case 187:
#line 1110 "XQuery.y"
  {
     yyVal = new Literal("<<");
  }
  break;
case 188:
#line 1114 "XQuery.y"
  {
     yyVal = new Literal(">>");
  }
  break;
case 192:
#line 1128 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Validate, null, yyVals[-1+yyTop]);
  }
  break;
case 193:
#line 1132 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Validate, yyVals[-3+yyTop], yyVals[-1+yyTop]);
  }
  break;
case 194:
#line 1139 "XQuery.y"
  {
     yyVal = new TokenWrapper(Token.LAX);
  }
  break;
case 195:
#line 1143 "XQuery.y"
  {
     yyVal = new TokenWrapper(Token.STRICT);
  }
  break;
case 196:
#line 1150 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.ExtensionExpr, yyVals[-3+yyTop], yyVals[-1+yyTop]);
  }
  break;
case 197:
#line 1157 "XQuery.y"
  {
     yyVal = Lisp.Cons(yyVals[0+yyTop]);
  }
  break;
case 198:
#line 1161 "XQuery.y"
  {
     yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(yyVals[0+yyTop]));
  }
  break;
case 199:
#line 1168 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Pragma, yyVals[-2+yyTop], new Literal(yyVals[-1+yyTop]));
   }
  break;
case 200:
#line 1175 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Child, new object[] { null });
  }
  break;
case 201:
#line 1179 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Child, yyVals[0+yyTop]);
  }
  break;
case 202:
#line 1183 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Descendant, yyVals[0+yyTop]);
  }
  break;
case 205:
#line 1192 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Child, yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 206:
#line 1196 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Descendant, yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 207:
#line 1203 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.AxisStep, yyVals[0+yyTop]);
  }
  break;
case 208:
#line 1207 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.FilterExpr, yyVals[0+yyTop]);
  }
  break;
case 210:
#line 1215 "XQuery.y"
  {
      yyVal = yyVals[-1+yyTop];
      notation.Confirm((Symbol)yyVals[-1+yyTop], Descriptor.PredicateList, yyVals[0+yyTop]);
  }
  break;
case 212:
#line 1221 "XQuery.y"
  {
      yyVal = yyVals[-1+yyTop];
      notation.Confirm((Symbol)yyVals[-1+yyTop], Descriptor.PredicateList, yyVals[0+yyTop]);
  }
  break;
case 213:
#line 1229 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.ForwardStep, yyVals[-1+yyTop], yyVals[0+yyTop]);
   }
  break;
case 215:
#line 1237 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.AXIS_CHILD);
   }
  break;
case 216:
#line 1241 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.AXIS_DESCENDANT);
   }
  break;
case 217:
#line 1245 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.AXIS_ATTRIBUTE);
   }
  break;
case 218:
#line 1249 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.AXIS_SELF);
   }
  break;
case 219:
#line 1253 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.AXIS_DESCENDANT_OR_SELF);
   }
  break;
case 220:
#line 1257 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.AXIS_FOLLOWING_SIBLING);
   }
  break;
case 221:
#line 1261 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.AXIS_FOLLOWING);
   }
  break;
case 222:
#line 1265 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.AXIS_NAMESPACE);
   }
  break;
case 223:
#line 1272 "XQuery.y"
  {  
	  yyVal = notation.Confirm((Symbol)yyVals[0+yyTop], Descriptor.AbbrevForward, yyVals[0+yyTop]); 
   }
  break;
case 225:
#line 1280 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.ReverseStep, yyVals[-1+yyTop], yyVals[0+yyTop]);
   }
  break;
case 227:
#line 1288 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.AXIS_PARENT);
   }
  break;
case 228:
#line 1292 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.AXIS_ANCESTOR);
   }
  break;
case 229:
#line 1296 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.AXIS_PRECEDING_SIBLING);
   }
  break;
case 230:
#line 1300 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.AXIS_PRECEDING);
   }
  break;
case 231:
#line 1304 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.AXIS_ANCESTOR_OR_SELF);
   }
  break;
case 232:
#line 1311 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.DOUBLE_PERIOD);
   }
  break;
case 237:
#line 1328 "XQuery.y"
  {
      yyVal = new TokenWrapper('*');
   }
  break;
case 238:
#line 1332 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Wildcard1, yyVals[-2+yyTop]);
   }
  break;
case 239:
#line 1336 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Wildcard2, yyVals[0+yyTop]);
   }
  break;
case 241:
#line 1344 "XQuery.y"
  {
      yyVal = yyVals[-1+yyTop];
      notation.Confirm((Symbol)yyVals[-1+yyTop], Descriptor.PredicateList, yyVals[0+yyTop]);
   }
  break;
case 242:
#line 1352 "XQuery.y"
  {
      yyVal = Lisp.Cons(yyVals[0+yyTop]);
   }
  break;
case 243:
#line 1356 "XQuery.y"
  {
      yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(yyVals[0+yyTop]));
   }
  break;
case 244:
#line 1363 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Predicate, yyVals[-1+yyTop]);
   }
  break;
case 258:
#line 1392 "XQuery.y"
  {
      yyVal = yyVals[0+yyTop];
   }
  break;
case 259:
#line 1399 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.ParenthesizedExpr, new object[] { null });
   }
  break;
case 260:
#line 1403 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.ParenthesizedExpr, yyVals[-1+yyTop]);
   }
  break;
case 261:
#line 1410 "XQuery.y"
  {
      yyVal = new TokenWrapper('.');
   }
  break;
case 262:
#line 1417 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Ordered, yyVals[-1+yyTop]);
   }
  break;
case 263:
#line 1424 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Unordered, yyVals[-1+yyTop]);
   }
  break;
case 264:
#line 1431 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Funcall, yyVals[-2+yyTop], null);
   }
  break;
case 265:
#line 1435 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Funcall, yyVals[-3+yyTop], yyVals[-1+yyTop]);
   }
  break;
case 266:
#line 1442 "XQuery.y"
  {
      yyVal = Lisp.Cons(yyVals[0+yyTop]);
   }
  break;
case 267:
#line 1446 "XQuery.y"
  {
      yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
   }
  break;
case 273:
#line 1464 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirElemConstructor, yyVals[-3+yyTop], yyVals[-2+yyTop]);
   }
  break;
case 274:
#line 1468 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirElemConstructor, yyVals[-3+yyTop], null);
   }
  break;
case 275:
#line 1472 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirElemConstructor, 
		 yyVals[-7+yyTop], yyVals[-6+yyTop], null, yyVals[-2+yyTop], yyVals[-1+yyTop]);
   }
  break;
case 276:
#line 1477 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirElemConstructor, 
		 yyVals[-7+yyTop], null, null, yyVals[-2+yyTop], yyVals[-1+yyTop]);
   }
  break;
case 277:
#line 1482 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirElemConstructor, 
		 yyVals[-8+yyTop], yyVals[-7+yyTop], yyVals[-5+yyTop], yyVals[-2+yyTop], yyVals[-1+yyTop]);
   }
  break;
case 278:
#line 1487 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirElemConstructor, 
		 yyVals[-8+yyTop], null, yyVals[-5+yyTop], yyVals[-2+yyTop], yyVals[-1+yyTop]);
   }
  break;
case 279:
#line 1495 "XQuery.y"
  {
      yyVal = Lisp.Cons(yyVals[0+yyTop]);
   }
  break;
case 280:
#line 1499 "XQuery.y"
  {      
      yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(yyVals[0+yyTop]));
   }
  break;
case 281:
#line 1506 "XQuery.y"
  {
      yyVal = null;
   }
  break;
case 283:
#line 1514 "XQuery.y"
  {
      yyVal = Lisp.List(yyVals[-1+yyTop], yyVals[0+yyTop]);   
   }
  break;
case 284:
#line 1518 "XQuery.y"
  {
      yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(yyVals[0+yyTop]));
   }
  break;
case 285:
#line 1522 "XQuery.y"
  {
      yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.List(yyVals[-1+yyTop], yyVals[0+yyTop]));
   }
  break;
case 286:
#line 1529 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirAttribute,
		 yyVals[-5+yyTop], yyVals[-4+yyTop], yyVals[-2+yyTop], new Literal("\""), Lisp.Cons(new Literal("")));   
   }
  break;
case 287:
#line 1534 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirAttribute,
		 yyVals[-6+yyTop], yyVals[-5+yyTop], yyVals[-3+yyTop], new Literal("\""), yyVals[-1+yyTop]);
   }
  break;
case 288:
#line 1539 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirAttribute,
		 yyVals[-5+yyTop], yyVals[-4+yyTop], yyVals[-2+yyTop], new Literal("\'"), Lisp.Cons(new Literal("")));   
   }
  break;
case 289:
#line 1544 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirAttribute,
		 yyVals[-6+yyTop], yyVals[-5+yyTop], yyVals[-3+yyTop], new Literal("\'"), yyVals[-1+yyTop]);
   }
  break;
case 290:
#line 1552 "XQuery.y"
  {
      yyVal = Lisp.Cons(new TokenWrapper(Token.EscapeQuot));
   }
  break;
case 291:
#line 1556 "XQuery.y"
  {
      yyVal = Lisp.Cons(yyVals[0+yyTop]);
   }
  break;
case 292:
#line 1560 "XQuery.y"
  {
      yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(new TokenWrapper(Token.EscapeQuot)));
   }
  break;
case 293:
#line 1564 "XQuery.y"
  {
      yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(yyVals[0+yyTop]));
   }
  break;
case 294:
#line 1571 "XQuery.y"
  {
      yyVal = Lisp.Cons(new TokenWrapper(Token.EscapeApos));
   }
  break;
case 295:
#line 1575 "XQuery.y"
  {
      yyVal = Lisp.Cons(yyVals[0+yyTop]);
   }
  break;
case 296:
#line 1579 "XQuery.y"
  {
      yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(new TokenWrapper(Token.EscapeApos)));
   }
  break;
case 297:
#line 1583 "XQuery.y"
  {
      yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(yyVals[0+yyTop]));
   }
  break;
case 308:
#line 1609 "XQuery.y"
  {
      yyVal = new Literal("{");
   }
  break;
case 309:
#line 1613 "XQuery.y"
  {
      yyVal = new Literal("}");
   }
  break;
case 310:
#line 1617 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CommonContent), Descriptor.EnclosedExpr, yyVals[0+yyTop]); 
   }
  break;
case 311:
#line 1624 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirCommentConstructor, yyVals[-1+yyTop]);
   }
  break;
case 312:
#line 1631 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirPIConstructor, yyVals[-1+yyTop], null);
   }
  break;
case 313:
#line 1635 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirPIConstructor, yyVals[-3+yyTop], yyVals[-1+yyTop]);
   }
  break;
case 314:
#line 1642 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CData), Descriptor.CDataSection, yyVals[-1+yyTop]);
   }
  break;
case 321:
#line 1658 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompDocConstructor, yyVals[-1+yyTop]);
   }
  break;
case 322:
#line 1666 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompElemConstructor, yyVals[-3+yyTop], yyVals[-1+yyTop]);   
   }
  break;
case 323:
#line 1671 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompElemConstructor, yyVals[-2+yyTop], null);   
   }
  break;
case 324:
#line 1676 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompElemConstructor, yyVals[-4+yyTop], yyVals[-1+yyTop]);   
   }
  break;
case 325:
#line 1681 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompElemConstructor, yyVals[-3+yyTop], null);   
   }
  break;
case 327:
#line 1693 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompAttrConstructor, yyVals[-3+yyTop], yyVals[-1+yyTop]);   
   }
  break;
case 328:
#line 1698 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompAttrConstructor, yyVals[-2+yyTop], null);   
   }
  break;
case 329:
#line 1703 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompAttrConstructor, yyVals[-4+yyTop], yyVals[-1+yyTop]);   
   }
  break;
case 330:
#line 1708 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompAttrConstructor, yyVals[-3+yyTop], null);   
   }
  break;
case 331:
#line 1716 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompTextConstructor, yyVals[-1+yyTop]);   
   }
  break;
case 332:
#line 1724 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompCommentConstructor, yyVals[-1+yyTop]);   
   }
  break;
case 333:
#line 1732 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompPIConstructor, yyVals[-3+yyTop], yyVals[-1+yyTop]);   
   }
  break;
case 334:
#line 1737 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompPIConstructor, yyVals[-2+yyTop], null);   
   }
  break;
case 335:
#line 1742 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompPIConstructor, yyVals[-4+yyTop], yyVals[-1+yyTop]);   
   }
  break;
case 336:
#line 1747 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompPIConstructor, yyVals[-3+yyTop], null);   
   }
  break;
case 338:
#line 1756 "XQuery.y"
  {
      yyVal = yyVals[-1+yyTop];
      notation.Confirm((Symbol)yyVals[-1+yyTop], Descriptor.Occurrence, 
		new TokenWrapper(Token.Indicator3));
   }
  break;
case 339:
#line 1765 "XQuery.y"
  {
      yyVal = yyVals[0+yyTop];
   }
  break;
case 341:
#line 1773 "XQuery.y"
  {
      yyVal = yyVals[-1+yyTop];
      notation.Confirm((Symbol)yyVals[-1+yyTop], Descriptor.Occurrence, yyVals[0+yyTop]);
   }
  break;
case 342:
#line 1778 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.EMPTY_SEQUENCE);
   }
  break;
case 343:
#line 1785 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.Indicator1);
   }
  break;
case 344:
#line 1789 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.Indicator2);
   }
  break;
case 345:
#line 1793 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.Indicator3);
   }
  break;
case 348:
#line 1802 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.ITEM);
   }
  break;
case 350:
#line 1813 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.KindTest, yyVals[0+yyTop]);
   }
  break;
case 360:
#line 1831 "XQuery.y"
  {
       yyVal = new TokenWrapper(Token.NODE);
   }
  break;
case 361:
#line 1838 "XQuery.y"
  {
       yyVal = new TokenWrapper(Token.DOCUMENT_NODE);
   }
  break;
case 362:
#line 1842 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.DocumentNode, yyVals[-1+yyTop]);
   }
  break;
case 363:
#line 1846 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.DocumentNode, yyVals[-1+yyTop]);
   }
  break;
case 364:
#line 1853 "XQuery.y"
  {
       yyVal = new TokenWrapper(Token.TEXT);
   }
  break;
case 365:
#line 1860 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.COMMENT);
   }
  break;
case 366:
#line 1868 "XQuery.y"
  {
       yyVal = new TokenWrapper(Token.PROCESSING_INSTRUCTION);
   }
  break;
case 367:
#line 1872 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.ProcessingInstruction, yyVals[-1+yyTop]);
   }
  break;
case 368:
#line 1876 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.ProcessingInstruction, yyVals[-1+yyTop]);
   }
  break;
case 369:
#line 1883 "XQuery.y"
  {
       yyVal = new TokenWrapper(Token.ELEMENT);
   }
  break;
case 370:
#line 1887 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Element, yyVals[-1+yyTop]);
   }
  break;
case 371:
#line 1891 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Element, yyVals[-3+yyTop], yyVals[-1+yyTop]);
   }
  break;
case 372:
#line 1895 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Element, 
		yyVals[-4+yyTop], yyVals[-2+yyTop], new TokenWrapper('?'));
   }
  break;
case 374:
#line 1904 "XQuery.y"
  {
      yyVal = new TokenWrapper('*');
   }
  break;
case 375:
#line 1911 "XQuery.y"
  {
       yyVal = new TokenWrapper(Token.ATTRIBUTE);
   }
  break;
case 376:
#line 1915 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Attribute, yyVals[-1+yyTop]);
   }
  break;
case 377:
#line 1919 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Attribute, yyVals[-3+yyTop], yyVals[-1+yyTop]);
   }
  break;
case 379:
#line 1927 "XQuery.y"
  {
      yyVal = new TokenWrapper('*');
   }
  break;
case 380:
#line 1934 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.SchemaElement, yyVals[-1+yyTop]);
   }
  break;
case 381:
#line 1941 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.SchemaAttribute, yyVals[-1+yyTop]);
   }
  break;
case 385:
#line 1960 "XQuery.y"
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
   61,   61,   62,   62,   63,   63,   63,   63,   63,   63,
   63,   64,   64,   65,   65,   46,   46,   66,   66,   67,
   47,   47,   68,   68,   69,   69,   48,   49,   49,   70,
   70,   71,   71,   71,   71,   72,   72,   76,   76,   76,
   77,   77,   77,   77,   77,   78,   78,   78,   79,   79,
   79,   80,   80,   81,   81,   82,   82,   83,   83,   85,
   86,   86,   86,   74,   74,   74,   74,   74,   74,   73,
   73,   73,   73,   73,   73,   75,   75,   75,   87,   87,
   87,   88,   88,   91,   91,   90,   92,   92,   93,   89,
   89,   89,   89,   95,   95,   95,   96,   96,   97,   97,
   97,   97,   99,   99,  102,  102,  102,  102,  102,  102,
  102,  102,  104,  104,  101,  101,  105,  105,  105,  105,
  105,  106,  103,  103,  108,  108,  109,  109,  109,   98,
   98,  100,  100,  111,  110,  110,  110,  110,  110,  110,
  110,  110,  112,  112,  120,  120,  120,  113,  114,  114,
  115,  118,  119,  116,  116,  121,  121,  117,  117,  122,
  122,  122,  124,  124,  124,  124,  124,  124,  128,  128,
  127,  127,  130,  130,  130,  131,  131,  131,  131,  132,
  132,  132,  132,  133,  133,  133,  133,  134,  134,  135,
  135,  129,  129,  129,  129,  137,  137,  137,  137,  137,
  125,  126,  126,  140,  123,  123,  123,  123,  123,  123,
  141,  142,  142,  142,  142,  147,  143,  143,  143,  143,
  144,  145,  146,  146,  146,  146,   84,   84,   37,   40,
   40,   40,  150,  150,  150,  149,  149,  149,  148,  107,
  151,  151,  151,  151,  151,  151,  151,  151,  151,  160,
  152,  152,  152,  159,  158,  157,  157,  157,  153,  153,
  153,  153,  161,  161,  154,  154,  154,  164,  164,  155,
  156,  165,  163,  162,   94,   94,  136,  138,  139,    8,
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
    1,    3,    1,    2,    1,    2,    2,    3,    4,    1,
    3,    1,    1,    1,    1,    4,    4,    1,    3,    5,
    8,   10,    1,    2,    7,    4,    8,    1,    3,    1,
    3,    1,    3,    3,    3,    1,    3,    1,    3,    3,
    1,    3,    3,    3,    3,    1,    3,    3,    1,    3,
    3,    1,    3,    1,    3,    1,    3,    1,    3,    2,
    0,    2,    2,    1,    2,    1,    2,    1,    2,    1,
    1,    1,    1,    1,    1,    1,    2,    2,    1,    1,
    1,    4,    5,    1,    1,    4,    1,    2,    5,    1,
    2,    2,    1,    1,    3,    3,    1,    1,    1,    2,
    1,    2,    2,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    2,    1,    2,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    3,    3,    1,
    2,    1,    2,    3,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    2,    2,    3,
    1,    4,    4,    3,    4,    1,    3,    1,    1,    1,
    1,    1,    5,    5,    9,    9,   10,   10,    1,    2,
    0,    1,    2,    2,    3,    6,    7,    6,    7,    1,
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
  390,    0,    0,   36,   37,    0,    0,   43,   44,    0,
   41,   42,   46,   47,    0,   50,   51,    0,   68,   69,
    0,    1,    3,    0,    0,    0,    0,    0,    0,    0,
    0,    7,   82,    0,   84,   85,   86,   87,    0,    0,
   93,   94,    0,  140,    0,    0,    0,    0,    0,  159,
    0,    0,    0,    0,    0,    8,    0,    0,    0,   34,
   14,   16,    0,    5,    0,    0,   59,    0,    0,    0,
   60,    0,   38,   39,   40,    0,    0,    0,    0,    0,
   98,    0,    0,  105,    0,    0,  128,    0,    0,    0,
  172,  173,    0,    0,    0,    0,    0,    0,    0,    0,
   95,   96,    0,  180,  181,  182,  184,  185,  183,  186,
  174,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  232,  254,  255,  256,  257,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  215,  216,  217,  218,
  219,  220,  221,  227,  228,  229,  230,  231,  222,    0,
    0,    0,    0,    0,    0,  261,  170,  189,  190,  191,
    0,  197,    0,  204,  207,  208,    0,    0,    0,  224,
  214,    0,  226,  233,  234,  236,    0,  245,  246,  247,
  248,  249,  250,  251,  252,  253,  268,  269,  270,  271,
  272,  315,  316,  317,  318,  319,  320,  350,  351,  352,
  353,  354,  355,  356,  357,  358,  359,   15,   17,    0,
    0,   58,    0,    0,   56,    0,   35,   48,   49,   45,
    0,    0,   67,    0,    0,    0,   76,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,   83,    0,   89,
  108,    0,    0,  111,    0,    0,    0,    0,  141,  175,
  177,  187,  179,  188,  143,  144,  145,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  160,  161,    0,    0,
    0,    0,  349,    0,  342,  348,  163,  347,  346,    0,
  165,  167,    0,  169,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  194,  195,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  386,    0,    0,  258,  259,    0,    0,
  235,  223,    0,    0,  198,    0,    0,    0,    0,  242,
    0,  213,  225,    0,    6,    9,    0,    0,    0,  339,
   65,    0,    0,    0,    0,    0,   99,    0,  106,    0,
  126,  129,  127,    0,    0,  122,  123,    0,  124,  125,
  114,    0,    0,    0,   90,    0,   91,  343,  344,  345,
  341,  338,    0,    0,    0,    0,  383,  369,  374,    0,
  373,    0,    0,  382,  375,  379,    0,  378,    0,  364,
    0,  365,    0,    0,    0,  360,  238,  264,  266,    0,
    0,    0,    0,  366,    0,  361,    0,    0,    0,    0,
  311,    0,  312,    0,    0,    0,    0,  260,  239,    0,
  206,  205,    0,  243,   61,   57,    0,   79,   73,    0,
    0,   70,   72,   77,    0,    0,  102,    0,    0,    0,
    0,  133,    0,  116,    0,    0,    0,  112,   92,  262,
  263,  321,  323,    0,    0,    0,  370,    0,  328,    0,
    0,  376,    0,  331,  332,  192,    0,    0,  265,  334,
    0,  368,  367,    0,  362,  363,  380,  381,    0,    0,
    0,    0,    0,  283,    0,    0,    0,  196,  244,   64,
    0,    0,    0,    0,    0,  130,    0,    0,    0,  134,
    0,  118,    0,  121,  322,  384,    0,    0,  327,    0,
    0,  193,  267,  333,    0,  313,  199,    0,  389,  306,
  307,    0,    0,    0,    0,  310,  302,    0,  279,  305,
  303,  304,  274,    0,    0,  273,  285,   71,   80,  103,
  100,  107,    0,    0,    0,    0,    0,  119,  371,    0,
  325,    0,  377,  330,    0,  336,    0,    0,    0,  308,
  309,    0,    0,  280,    0,    0,    0,  136,  131,    0,
  137,  372,  324,  329,  335,    0,  314,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  132,  388,  288,  294,    0,  295,  301,  300,  387,  290,
  286,    0,  291,  298,  299,  276,    0,  275,    0,  135,
  289,  296,  297,  292,  287,  293,  278,  277,
  };
  protected static  short [] yyDgoto  = {            18,
   19,   20,   21,  111,   22,   82,   23,  285,   24,   25,
   26,   27,   28,   29,   30,   31,   32,   33,   34,   35,
   36,   37,   38,   39,   40,   41,   42,   43,   65,  290,
   48,  286,   49,  121,  292,   83,  293,  295,  502,  347,
  596,  296,  297,  524,   85,   86,   87,   88,   89,   90,
  149,  150,   91,   92,  130,  131,  506,  507,  133,  134,
  313,  314,  431,  432,  433,  136,  137,  511,  512,   93,
   94,   95,  165,  166,  167,   96,   97,   98,   99,  100,
  101,  102,  103,  352,  104,  105,  227,  228,  229,  230,
  371,  231,  232,  385,  233,  234,  235,  236,  237,  399,
  238,  239,  240,  241,  242,  243,  244,  245,  246,  247,
  400,  248,  249,  250,  251,  252,  253,  254,  255,  256,
  470,  257,  258,  259,  260,  261,  486,  598,  599,  487,
  554,  672,  665,  673,  666,  674,  600,  668,  601,  602,
  262,  263,  264,  265,  266,  267,  525,  349,  350,  441,
  268,  269,  270,  271,  272,  273,  274,  275,  276,  277,
  450,  577,  451,  457,  458,
  };
  protected static  short [] yySindex = {          840,
 -256, -210, -214, -179, -204, -182, -119, -102,  -66, -153,
   66,    6, -133, -133,  156,  -18, -115,    0, 1119,    0,
    0,  158, 1226, 1226, -113,  159,  159,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  -37,  164,  -74,   26, -133,    0,  -68,
    0,   -6,  252,    0,    0, -133, -133,    0,    0,    3,
    0,    0,    0,    0,  272,    0,    0,   15,    0,    0,
  285,    0,    0,  310,  324,  328,  328,  335,  337,  304,
  304,    0,    0,  352,    0,    0,    0,    0,   87, -141,
    0,    0,   97,    0,  166,   -5, -232,  -82,   54,    0,
  102,  110,  115,  120, 4849,    0, -113,  159,  159,    0,
    0,    0,  104,    0, -133,  367,    0,  165,  376, -133,
    0, -133,    0,    0,    0,  109,  172,  409,  131,  410,
    0,  133,  412,    0,  136,   -1,    0,   11,  158,  158,
    0,    0,  158,  304,  158,  158,  158,  158, -226,  194,
    0,    0,  304,    0,    0,    0,    0,    0,    0,    0,
    0,  406,  313,  331,  304,  304,  304,  304,  304,  304,
  304,  304,  304,  304,  304,  304,  304,  304,  686,  686,
  147,  147,  354,  355,  357,  -19,  -17,   12,   17,  -90,
  441,    0,    0,    0,    0,    0,  424,  447,  -14,  449,
  450,  452, 5107,  177,  180,  175,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  179,
  183,  141, 5107,  443,  453,    0,    0,    0,    0,    0,
 -105,    0,  -34,    0,    0,    0,  430,  430,  443,    0,
    0,  443,    0,    0,    0,    0,  430,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  159,
  159,    0, -133, -133,    0,  471,    0,    0,    0,    0,
  686,  -33,    0,  204,  484,  483,    0,  172,  310,  172,
  324,  172,  158,  328,  158,  161,  169,    0,   97,    0,
    0, -207,  492,    0,  492,  158,  262,  158,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  316, -232, -232,
  -82,  -82,  -82,  -82,   54,   54,    0,    0,  497,  500,
  502,  503,    0,  513,    0,    0,    0,    0,    0, -126,
    0,    0,  491,    0,  158,  158,  158,  432,  -13,  158,
  433,  -11,  158,  516,  158,  517,  158,    0,    0,  158,
  437,  520,  522,  226,  439,   -9,  158,  -38,  244,  245,
  -34,  211, -253,    0,  247,  253,    0,    0,  195,  -34,
    0,    0,  249,  158,    0, 5107, 5107,  158,  430,    0,
  430,    0,    0,  430,    0,    0,  471,   -6, -133,    0,
    0,  515,  172,  -77,  409,  311,    0,  521,    0,  321,
    0,    0,    0,  312,  307,    0,    0, -133,    0,    0,
    0, -169,  318,  158,    0,  158,    0,    0,    0,    0,
    0,    0,   22,   28,   29,  340,    0,    0,    0,  197,
    0,   30,  598,    0,    0,    0,  203,    0,   31,    0,
   33,    0,   35,   37,  158,    0,    0,    0,    0,  207,
  741,  551,  572,    0,   44,    0,  573,  578,  584,  585,
    0,  319,    0,  305,  -12,   39,  314,    0,    0,   45,
    0,    0,   21,    0,    0,    0,  158,    0,    0,  686,
  158,    0,    0,    0,  593,  368,    0,  575,  158,  608,
  113,    0,  158,    0, -133,  363, -133,    0,    0,    0,
    0,    0,    0,  352,  523,  325,    0,  519,    0,   46,
  325,    0,  524,    0,    0,    0,   48,  158,    0,    0,
   50,    0,    0,  526,    0,    0,    0,    0,  282,  292,
  175,  -40,  591,    0,    1,  592,  336,    0,    0,    0,
  -76,   53,  334,  158,  158,    0,  345,  405,   -2,    0,
  391,    0, -133,    0,    0,    0,   62,  774,    0,  640,
 1032,    0,    0,    0, 1164,    0,    0,  621,    0,    0,
    0,  369,  543,  559,  644,    0,    0,   42,    0,    0,
    0,    0,    0,  646,   89,    0,    0,    0,    0,    0,
    0,    0,  419,  158,  158,  378,  158,    0,    0,  661,
    0,  583,    0,    0,   55,    0,   56,  175,  360,    0,
    0,  404,  679,    0,  407,  680,  686,    0,    0,  459,
    0,    0,    0,    0,    0,  -29,    0,  175,  413,  175,
  414,  467,  158,  -67,  -30,  677,  175,  678,  175,  158,
    0,    0,    0,    0,  -43,    0,    0,    0,    0,    0,
    0,  -27,    0,    0,    0,    0,  683,    0,  687,    0,
    0,    0,    0,    0,    0,    0,    0,    0,
  };
  protected static  short [] yyRindex = {         4492,
    0,    0,  425,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0, 4492,    0,
    0, 4980,  742,  230,  344,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  689,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0, 4980,
 4980,    0,    0,  750,    0,    0,    0,    0,  398,    0,
    0,    0, 4387,    0, 4347, 4212, 3955, 3425, 3059,    0,
 3013, 2869, 2735, 2457,    0,    0,  488,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  692,    0,    0,
    0,    0,    0,    0,    0,    0,  -31,  713,    0,  190,
    0,    0,  302,    0,    0,    0,    0,    0, 4980, 4980,
    0,    0, 4980, 4980, 4980, 4980, 4980, 4980,    0,    0,
    0,    0, 4980,    0,    0,    0,    0,    0,    0,    0,
    0,    0, 4635, 4753, 4980, 4980, 4980, 4980, 4980, 4980,
 4980, 4980, 4980, 4980, 4980, 4980, 4980, 4980,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  645,    0,    0,
    0,    0,    0,    0,    0,  434,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0, 4980, 1901,    0,  789,    0,    0,    0,    0,    0,
    0,    0, 2035,    0,    0,    0,  923, 1067,    0,    0,
    0,    0,    0,    0,    0,    0, 1201,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  697,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  717,    0,   96,    0,  703,
    0,  493, 4980,    0, 4980,    0,    0,    0, 4470,    0,
    0,  -25,  509,    0,  510, 4980,    0, 4980,    0,    0,
    0,    0,    0,    0,    0,    0,    0, 4302, 4006, 4129,
 3471, 3615, 3749, 3872, 3147, 3337,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0, 1345,
    0,    0, 2591,    0, 4980, 4980, 4980,    0,    0, 4980,
    0,    0, 4980,    0, 4980,    0, 4980,    0,    0, 4980,
    0,    0,    0, 4980,    0,    0, 4980,    0,    0,    0,
 2179,    0,    0,    0,    0,   74,    0,    0,    0, 2313,
    0,    0,    0, 4980,    0,    0,    0, 4980, 1479,    0,
 1623,    0,    0, 1757,    0,    0,  708,  689,    0,    0,
    0,    0,  209,    0,    0,  504,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   -8,   -7, 4980,    0, 4980,    0,    0,    0,    0,
    0,    0,    0,    0,    0, 4980,    0,    0,    0,    0,
    0,    0, 4980,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0, 4980,    0,    0,    0,    0,    0,
 4980,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  117,    0,    0,    0,
    0,    0,    0,    0,    0,    0, 4980,    0,    0,    0,
 4980,    0,    0,    0,    0,    0,    0,    0, 4980,    0,
    0,    0, 4980,    0,    0,    4,    0,    0,    0,    0,
    0,    0,    0,  647,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0, 4980,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  720,    0,    0,    0,    0,    0,  -23,    0,    0,    0,
    0,    0,    0, 4980, 4980,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0, 4980,    0,    0,
 4980,    0,    0,    0, 4980,    0,    0,    0,    0,    0,
    0,    0, 4980,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0, 4980, 4980,    0, 4980,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  -28,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  721,    0,  721,
    0,    0, 4980,    0,    0,    0,  721,    0,  721, 4980,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,
  };
  protected static  short [] yyGindex = {            0,
    0,  763,  766,   43,  764,    0,    0,   -3,    0,  767,
  768,   34,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  505,    0,  382,  -81, -131,  384,    0,  237, -178,
 -373,    0,  385,  -22,    0,    0,    0,    0,    0,    0,
    0,  652,  722,  723,    0,  511,    0,    0,    0,  527,
  663,  390,    0,    0,  386,  739,  525,    0,  320,  681,
  674,  126,    0,    0,    0,  667,  231,   24,  229,  232,
    0,    0,    0,  655,    0,  346,    0,    0,    0,    0,
    0,    0,  613, -500, -112,   51,    0,    0,    0, -104,
    0,    0,  -97,    0,    0,    0, -171,    0,    0,    0,
 -167,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0, -389,    0,    0,    0,    0,    0,  291, -490,    0,
  295,    0,    0,  199,  202,    0, -461,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  294,  270,    0,    0,
    0,    0,  495,    0,  496,    0,    0,    0,    0,    0,
    0,  348,  498,    0,  506,
  };
  protected static  short [] yyTable = {            84,
   52,  351,  476,  671,  655,  385,  685,  348,  348,   66,
   67,  308,  397,  310,  311,  312,  312,  394,  113,  595,
  359,  110,  362,  284,  412,  376,   66,  448,  449,  455,
  456,  474,  370,  616,  553,  115,  120,  169,  284,  170,
  503,  176,  304,  316,  118,  501,  501,  117,   46,  552,
  588,  364,  123,  124,  304,  593,  366,  594,  109,   44,
  604,  171,  172,  173,  143,  143,  426,  427,  428,  112,
  482,  143,  143,  143,  143,   54,  143,   55,  143,  593,
  143,  594,  593,   50,  594,  556,  114,  143,  143,  143,
  381,  143,  593,  143,  594,  593,  143,  594,  143,  143,
  555,  633,  619,  360,  483,  363,  515,  634,  377,   45,
  390,  281,  410,  559,  634,   53,  306,  307,  287,  348,
  281,  147,  148,  593,  620,  594,  392,  646,  145,   74,
   75,  146,   47,  401,  365,  281,   51,  429,  430,  367,
  109,  402,  404,   56,  403,  174,  520,  656,  636,  658,
  278,  279,  521,  522,  528,  533,  677,  534,  679,  535,
   57,  536,  597,  282,  593,  597,  594,   60,  544,  558,
  579,  421,  582,  423,  584,  429,  430,  609,  282,  644,
  645,  388,   51,   80,  435,   81,  437,  503,  499,  499,
  500,   68,  667,  675,  331,  332,  333,  334,  162,  389,
   80,  424,   81,  667,  143,   71,  147,  148,  597,  425,
  675,  593,  143,  594,  175,  597,  416,  110,  418,  113,
  420,  368,  369,   10,  115,  163,  161,  164,   15,   11,
   17,  494,  411,  494,   66,  488,  494,  527,  143,   69,
  526,   70,  469,  532,  113,  116,  531,  539,  339,   78,
  538,  119,   78,  206,  438,  439,  440,  662,  590,  591,
  120,  115,  120,   63,   64,   11,  468,  615,   80,   11,
   81,   11,   11,  117,   11,   11,   11,  303,   58,   59,
  408,  662,  590,  591,  589,  590,  591,  168,  117,  305,
  325,  326,  327,   11,  669,  590,  591,  669,  590,  591,
  284,  358,  312,  361,  519,  375,  472,  447,  551,  454,
  473,  663,  122,  201,  204,  126,  205,  664,  125,  396,
  592,  561,  405,  406,  128,  589,  590,  591,  348,   61,
   62,  568,  443,  444,  445,  681,  127,  452,  348,  220,
  459,  682,  461,   12,  463,  129,   80,  464,   81,  654,
  385,  177,  178,  670,  475,  204,  684,  205,  169,  132,
  170,  592,   66,  135,   66,  560,  589,  590,  591,  288,
  289,  490,  322,  321,  139,  493,  140,  566,  144,   12,
  220,  571,   80,   12,   81,   12,   12,  153,   12,   12,
   12,  323,  324,  510,  569,  143,  204,   88,  205,  329,
  330,  179,  592,  335,  336,  496,  583,   12,  337,  338,
  180,   74,   75,  589,  590,  591,  181,   76,   77,  280,
   78,  220,  182,   79,  514,  141,  142,  282,   74,   75,
  530,  283,  611,  612,   76,   77,  284,   78,   88,  291,
   79,   88,  537,  204,  294,  205,  491,  492,  541,  592,
  353,  353,  298,  299,  300,  301,   88,  302,  652,   97,
   97,   97,   97,  318,  523,  348,  320,  343,  220,  154,
  155,  156,  157,  158,  159,  160,  355,  356,  562,  357,
  372,  373,  638,  639,  225,  641,  374,   13,  378,  379,
   88,  380,  382,   11,   11,  383,   74,   75,  384,  386,
   11,   11,   76,   77,  387,   78,   11,   11,   79,   11,
  393,  572,   11,  574,  409,   11,   11,   11,   11,   11,
  398,  661,   88,   13,  414,  413,  415,   13,  680,   13,
   13,  436,   13,   13,   13,  434,  359,   97,   97,  362,
   11,  364,  366,   11,   11,   11,   11,   11,   11,   11,
   11,   13,  376,  442,  446,  453,  460,  462,  625,  465,
  466,  471,  627,  467,  447,  454,  481,  484,  489,  618,
  562,  104,  104,  104,  104,  497,  485,  505,  508,   11,
   11,   11,   11,   11,   11,   80,   11,   81,   11,  509,
  513,  542,  510,  517,   11,   11,   11,   11,   11,   11,
   11,   11,   11,   11,   11,   11,   11,   12,   12,   11,
   74,   75,  543,  545,   12,   12,   76,   77,  546,   78,
   12,   12,   79,   12,  547,  548,   12,  550,  563,   12,
   12,   12,   12,   12,  549,  565,  564,  557,  573,  586,
   80,  578,   81,  567,  235,  576,  581,  575,  585,  104,
  104,  587,  603,  606,   12,  610,  551,   12,   12,   12,
   12,   12,   12,   12,   12,  630,  613,   88,   88,   88,
   88,   88,   88,   88,  614,  617,   88,  235,   88,   88,
  623,  628,   88,  631,  629,  235,  637,  235,  235,  235,
  632,  235,  635,   12,   12,   12,   12,   12,   12,  640,
   12,  642,   12,  235,  235,  235,  235,  643,   12,   12,
   12,   12,   12,   12,   12,   12,   12,   12,   12,   12,
   12,  647,  529,   12,  648,  649,  651,  650,  653,  339,
  340,  341,  342,  657,  659,  235,  660,  235,  676,  678,
   54,   10,   88,   88,  687,   88,   88,   62,  688,   81,
   52,   13,   13,   74,  385,   63,  191,   75,   13,   13,
   66,   66,  197,  391,   13,   13,   53,   13,  235,  235,
   13,  326,  101,   13,   13,   13,   13,   13,  109,  110,
  385,   72,  385,   80,   73,   81,  106,  407,  237,  495,
  107,  108,  344,  200,  201,  202,  498,  608,   13,  504,
  317,   13,   13,   13,   13,   13,   13,   13,   13,  417,
  315,  151,  152,   74,   75,  138,   80,  516,   81,   76,
   77,  237,   78,  518,  309,   79,  319,  419,  422,  237,
  570,  237,  237,  237,  328,  237,  354,   13,   13,   13,
   13,   13,   13,  395,   13,  605,   13,  237,  237,  237,
  237,  607,   13,   13,   13,   13,   13,   13,   13,   13,
   13,   13,   13,   13,   13,  540,  683,   13,   74,   75,
  686,  622,  477,  478,   76,   77,  479,   78,  580,  237,
   79,  237,    0,    0,    0,  480,    0,    0,    0,    0,
    0,    0,    0,    0,  339,  340,  341,  342,  621,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  237,  237,  235,  235,  235,  235,  235,  235,
  235,  191,  209,  235,    0,  235,  235,    0,  343,  235,
    0,    0,    0,    0,    0,  235,  235,  235,  235,  235,
  235,  235,  235,  235,  235,  235,  235,  235,  235,  235,
  235,  235,  235,  235,  235,  209,    0,  344,  200,  201,
  202,    0,    0,  209,    0,  209,  209,  209,    0,  209,
  345,  346,  339,  340,  341,  342,    0,    0,    0,    0,
    0,  209,  209,  209,  209,    0,    0,    0,    0,  235,
  235,    0,  235,  235,    0,    0,    0,    0,  235,  191,
    0,    0,    0,    0,    0,    0,  343,    0,    0,    0,
    0,   74,   75,    0,    0,  209,    0,   76,   77,    0,
   78,    0,  235,   79,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  344,  200,  201,  202,    0,
    0,    0,    0,    0,   74,   75,  209,  209,  345,  346,
   76,   77,    0,   78,    0,    0,   79,    0,  237,  237,
  237,  237,  237,  237,  237,    0,  211,  237,    0,  237,
  237,    0,    0,  237,   80,    0,   81,    0,    0,  237,
  237,  237,  237,  237,  237,  237,  237,  237,  237,  237,
  237,  237,  237,  237,  237,  237,  237,  237,  237,  211,
    0,    0,    0,    0,    0,    0,    0,  211,    0,  211,
  211,  211,    0,  211,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  211,  211,  211,  211,    0,
    0,    0,    0,  237,  237,    0,  237,  237,    0,    0,
    0,    0,  237,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  624,    0,    0,  211,
    0,    0,    0,    0,    0,    0,  237,    1,    2,    3,
    4,    5,    6,    7,    8,    9,   10,   11,   12,   13,
   14,   15,   16,   17,    0,    0,    0,    0,    0,    0,
  211,  211,  209,  209,  209,  209,  209,  209,  209,    0,
  240,  209,    0,  209,  209,    0,   80,  209,   81,    0,
    0,    0,    0,  209,  209,  209,  209,  209,  209,  209,
  209,  209,  209,  209,  209,  209,  209,  209,  209,  209,
  209,  209,  209,  240,    0,    0,    0,    0,    0,    0,
    0,  240,    0,  240,  240,  240,    0,  240,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  240,
  240,  240,  240,    0,    0,    0,    0,  209,  209,    0,
  209,  209,    0,    0,    0,    0,  209,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  626,    0,
    0,    0,    0,  240,    0,    0,    0,    0,    0,    0,
  209,    0,   74,   75,    0,    0,    0,    0,   76,   77,
    0,   78,    0,    0,   79,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  240,  240,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  211,  211,  211,  211,
  211,  211,  211,    0,  340,  211,    0,  211,  211,    0,
    0,  211,    0,    0,    0,    0,    0,  211,  211,  211,
  211,  211,  211,  211,  211,  211,  211,  211,  211,  211,
  211,  211,  211,  211,  211,  211,  211,  340,    0,    0,
    0,    0,    0,    0,    0,  340,    0,  340,  340,  340,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  340,  340,  340,  340,  340,    0,    0,    0,
    0,  211,  211,    0,  211,  211,    0,    0,    0,    0,
  211,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   74,   75,    0,  340,    0,    0,
   76,   77,    0,   78,  211,    0,   79,    2,    3,    4,
    5,    6,    7,    8,    9,   10,   11,   12,   13,   14,
   15,   16,   17,    0,    0,    0,    0,  340,  340,  340,
  240,  240,  240,  240,  240,  240,  240,    0,  210,  240,
    0,  240,  240,    0,    0,  240,    0,    0,    0,    0,
    0,  240,  240,  240,  240,  240,  240,  240,  240,  240,
  240,  240,  240,  240,  240,  240,  240,  240,  240,  240,
  240,  210,    0,    0,    0,    0,    0,    0,    0,  210,
    0,  210,  210,  210,    0,  210,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  210,  210,  210,
  210,    0,    0,    0,    0,  240,  240,    0,  240,  240,
    0,    0,    0,    0,  240,    3,    4,    5,    6,    7,
    8,    9,   10,   11,   12,   13,   14,   15,   16,   17,
    0,  210,    0,    0,    0,    0,    0,    0,  240,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  210,  210,    0,    0,    0,    0,    0,    0,
  340,  340,    0,  340,  340,  340,  340,  340,  340,  340,
  340,    0,  212,  340,    0,  340,  340,    0,    0,  340,
    0,    0,    0,    0,    0,  340,  340,  340,  340,  340,
  340,  340,  340,  340,  340,    0,    0,    0,  340,  340,
  340,  340,  340,  340,  340,  212,    0,    0,    0,    0,
    0,    0,    0,  212,    0,  212,  212,  212,    0,  212,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  212,  212,  212,  212,    0,    0,    0,    0,  340,
  340,    0,  340,  340,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  212,    0,    0,    0,    0,
    0,    0,  340,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  212,  212,  210,  210,
  210,  210,  210,  210,  210,    0,  241,  210,    0,  210,
  210,    0,    0,  210,    0,    0,    0,    0,    0,  210,
  210,  210,  210,  210,  210,  210,  210,  210,  210,  210,
  210,  210,  210,  210,  210,  210,  210,  210,  210,  241,
    0,    0,    0,    0,    0,    0,    0,  241,    0,  241,
  241,  241,    0,  241,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  241,  241,  241,  241,    0,
    0,    0,    0,  210,  210,    0,  210,  210,    0,    0,
    0,    0,  210,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  241,
    0,    0,    0,    0,    0,    0,  210,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  241,  241,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  212,  212,  212,  212,  212,  212,  212,    0,
  200,  212,    0,  212,  212,    0,    0,  212,    0,    0,
    0,    0,    0,  212,  212,  212,  212,  212,  212,  212,
  212,  212,  212,  212,  212,  212,  212,  212,  212,  212,
  212,  212,  212,  200,    0,    0,    0,    0,    0,    0,
    0,  200,    0,  200,  200,  200,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  200,
  200,  200,  200,    0,    0,    0,    0,  212,  212,    0,
  212,  212,    0,    0,    0,    0,  212,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  200,    0,    0,    0,    0,    0,    0,
  212,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  200,  200,  241,  241,  241,  241,
  241,  241,  241,    0,  203,  241,    0,  241,  241,    0,
    0,  241,    0,    0,    0,    0,    0,  241,  241,  241,
  241,  241,  241,  241,  241,  241,  241,  241,  241,  241,
  241,  241,  241,  241,  241,  241,  241,  203,    0,    0,
    0,    0,    0,    0,    0,  203,    0,  203,  203,  203,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  203,  203,  203,  203,    0,    0,    0,
    0,  241,  241,    0,  241,  241,    0,    0,    0,    0,
  241,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  203,    0,    0,
    0,    0,    0,    0,  241,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  203,  203,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  200,  200,  200,  200,  200,  200,  200,    0,  202,  200,
    0,  200,  200,    0,    0,  200,    0,    0,    0,    0,
    0,  200,  200,  200,  200,  200,  200,  200,  200,  200,
  200,  200,  200,  200,  200,  200,  200,  200,  200,  200,
  200,  202,    0,    0,    0,    0,    0,    0,    0,  202,
    0,  202,  202,  202,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  202,  202,  202,
  202,    0,    0,    0,    0,  200,  200,    0,  200,  200,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  202,    0,    0,    0,    0,    0,    0,  200,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  202,  202,  203,  203,  203,  203,  203,  203,
  203,    0,  201,  203,    0,  203,  203,    0,    0,  203,
    0,    0,    0,    0,    0,  203,  203,  203,  203,  203,
  203,  203,  203,  203,  203,  203,  203,  203,  203,  203,
  203,  203,  203,  203,  203,  201,    0,    0,    0,    0,
    0,    0,    0,  201,    0,  201,  201,  201,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  201,  201,  201,  201,    0,    0,    0,    0,  203,
  203,    0,  203,  203,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  201,    0,    0,    0,    0,
    0,    0,  203,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  201,  201,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  202,  202,
  202,  202,  202,  202,  202,    0,  168,  202,    0,  202,
  202,    0,    0,  202,    0,    0,    0,    0,    0,  202,
  202,  202,  202,  202,  202,  202,  202,  202,  202,  202,
  202,  202,  202,  202,  202,  202,  202,  202,  202,  168,
    0,    0,    0,    0,    0,    0,    0,  168,    0,  168,
  168,  168,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  168,  168,  168,  168,    0,
    0,    0,    0,  202,  202,    0,  202,  202,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  168,
    0,    0,    0,    0,    0,    0,  202,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  168,  168,  201,  201,  201,  201,  201,  201,  201,    0,
  337,  201,    0,  201,  201,    0,    0,  201,    0,    0,
    0,    0,    0,  201,  201,  201,  201,  201,  201,  201,
  201,  201,  201,  201,  201,  201,  201,  201,  201,  201,
  201,  201,  201,  337,    0,    0,    0,    0,    0,    0,
    0,  337,    0,  337,  337,  337,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  337,
  337,  337,  337,    0,    0,    0,    0,  201,  201,    0,
  201,  201,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  337,    0,    0,    0,    0,    0,    0,
  201,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  337,  337,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  168,  168,  168,  168,
  168,  168,  168,    0,  166,  168,    0,  168,  168,    0,
    0,  168,    0,    0,    0,    0,    0,  168,  168,  168,
  168,  168,  168,  168,  168,  168,  168,  168,  168,    0,
  168,  168,  168,  168,  168,  168,  168,  166,    0,    0,
    0,    0,    0,    0,    0,  166,    0,  166,  166,  166,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  166,  166,  166,  166,    0,    0,    0,
    0,  168,  168,    0,  168,  168,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  166,    0,    0,
    0,    0,    0,    0,  168,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  166,  166,
  337,  337,  337,  337,  337,  337,  337,    0,  164,  337,
    0,  337,  337,    0,    0,  337,    0,    0,    0,    0,
    0,  337,  337,  337,  337,  337,  337,  337,  337,  337,
  337,  337,  337,    0,  337,  337,  337,  337,  337,  337,
  337,  164,    0,    0,    0,    0,    0,    0,    0,  164,
    0,  164,  164,  164,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  164,  164,  164,
  164,    0,    0,    0,    0,  337,  337,    0,  337,  337,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  164,    0,    0,    0,    0,    0,    0,  337,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  164,  164,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  166,  166,  166,  166,  166,  166,
  166,    0,  162,  166,    0,  166,  166,    0,    0,  166,
    0,    0,    0,    0,    0,  166,  166,  166,  166,  166,
  166,  166,  166,  166,  166,  166,    0,    0,  166,  166,
  166,  166,  166,  166,  166,  162,    0,    0,    0,    0,
    0,    0,    0,  162,    0,  162,  162,  162,  156,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  162,  162,  162,  162,    0,    0,    0,    0,  166,
  166,    0,  166,  166,    0,    0,    0,    0,    0,    0,
    0,  156,    0,    0,    0,    0,    0,    0,    0,  156,
    0,  156,  156,  156,    0,  162,    0,    0,    0,    0,
    0,    0,  166,    0,    0,    0,    0,  156,  156,  156,
  156,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  162,  162,  164,  164,
  164,  164,  164,  164,  164,    0,  157,  164,    0,  164,
  164,  156,    0,  164,    0,    0,    0,    0,    0,  164,
  164,  164,  164,  164,  164,  164,  164,  164,  164,    0,
    0,    0,  164,  164,  164,  164,  164,  164,  164,  157,
    0,    0,  156,  156,    0,    0,    0,  157,    0,  157,
  157,  157,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  157,  157,  157,  157,    0,
    0,    0,    0,  164,  164,    0,  164,  164,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  157,
    0,    0,    0,    0,    0,    0,  164,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  157,  157,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  162,  162,  162,  162,  162,  162,  162,    0,
    0,  162,    0,  162,  162,    0,    0,  162,    0,    0,
    0,    0,    0,  162,  162,  162,  162,  162,  162,  162,
  162,  162,    0,    0,    0,    0,  162,  162,  162,  162,
  162,  162,  162,    0,    0,    0,    0,    0,  156,  156,
  156,  156,  156,  156,  156,    0,  158,  156,    0,  156,
  156,    0,    0,  156,    0,    0,    0,    0,    0,  156,
  156,  156,  156,  156,  156,  156,    0,  162,  162,    0,
  162,  162,  156,  156,  156,  156,  156,  156,  156,  158,
    0,    0,    0,    0,    0,    0,    0,  158,    0,  158,
  158,  158,    0,    0,    0,    0,    0,    0,    0,    0,
  162,    0,    0,    0,    0,  158,  158,  158,  158,    0,
    0,    0,    0,  156,  156,    0,  156,  156,    0,    0,
    0,    0,    0,    0,    0,    0,  157,  157,  157,  157,
  157,  157,  157,    0,  151,  157,    0,  157,  157,  158,
    0,  157,    0,    0,    0,    0,  156,  157,  157,  157,
  157,  157,  157,  157,    0,    0,    0,    0,    0,    0,
  157,  157,  157,  157,  157,  157,  157,  151,    0,    0,
  158,  158,    0,    0,    0,  151,    0,  151,  151,  151,
  153,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  151,  151,  151,  151,    0,    0,    0,
    0,  157,  157,    0,  157,  157,    0,    0,    0,    0,
    0,    0,    0,  153,    0,    0,    0,    0,    0,    0,
    0,  153,    0,  153,  153,  153,    0,  151,    0,    0,
    0,    0,    0,    0,  157,    0,    0,    0,    0,  153,
  153,  153,  153,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  151,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  153,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  153,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  158,  158,  158,  158,
  158,  158,  158,    0,  154,  158,    0,  158,  158,    0,
    0,  158,    0,    0,    0,    0,    0,  158,  158,  158,
  158,  158,  158,  158,    0,    0,    0,    0,    0,    0,
  158,  158,  158,  158,  158,  158,  158,  154,    0,    0,
    0,    0,    0,    0,    0,  154,    0,  154,  154,  154,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  154,  154,  154,  154,    0,    0,    0,
    0,  158,  158,    0,  158,  158,    0,    0,    0,    0,
    0,    0,    0,    0,  151,  151,  151,  151,  151,  151,
  151,    0,    0,  151,    0,  151,  151,  154,    0,  151,
    0,    0,    0,    0,  158,  151,  151,  151,  151,  151,
  151,    0,    0,    0,    0,    0,    0,    0,  151,  151,
  151,  151,  151,  151,  151,    0,    0,    0,    0,  154,
  153,  153,  153,  153,  153,  153,  153,    0,  155,  153,
    0,  153,  153,    0,    0,  153,    0,    0,    0,    0,
    0,  153,  153,  153,  153,  153,  153,    0,    0,  151,
  151,    0,  151,  151,  153,  153,  153,  153,  153,  153,
  153,  155,    0,    0,    0,    0,    0,    0,    0,  155,
    0,  155,  155,  155,    0,    0,    0,    0,    0,    0,
    0,    0,  151,    0,    0,    0,    0,  155,  155,  155,
  155,    0,    0,    0,    0,  153,  153,    0,  153,  153,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  155,    0,    0,    0,    0,    0,    0,  153,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  152,    0,  155,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  154,  154,  154,  154,  154,  154,
  154,    0,    0,  154,    0,  154,  154,    0,    0,  154,
    0,    0,    0,    0,  152,  154,  154,  154,  154,  154,
  154,    0,  152,    0,  152,  152,  152,    0,  154,  154,
  154,  154,  154,  154,  154,    0,    0,    0,    0,    0,
  152,  152,  152,  152,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  148,    0,    0,    0,    0,  154,
  154,    0,  154,  154,  152,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  148,    0,    0,
    0,    0,  154,    0,    0,  148,  152,  148,  148,  148,
    0,    0,    0,    0,    0,  149,    0,    0,    0,    0,
    0,    0,    0,  148,  148,  148,  148,    0,  155,  155,
  155,  155,  155,  155,  155,    0,    0,  155,    0,  155,
  155,    0,    0,  155,    0,    0,    0,    0,  149,  155,
  155,  155,  155,  155,  155,    0,  149,  148,  149,  149,
  149,    0,  155,  155,  155,  155,  155,  155,  155,    0,
    0,    0,    0,    0,  149,  149,  149,  149,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  148,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  155,  155,    0,  155,  155,  149,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  155,    0,  150,    0,
  149,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  152,  152,  152,  152,  152,  152,  152,    0,    0,
  152,    0,  152,  152,    0,    0,  152,    0,    0,    0,
    0,  150,  152,  152,  152,  152,  152,  152,    0,  150,
    0,  150,  150,  150,    0,  152,  152,  152,  152,  152,
  152,  152,    0,    0,    0,    0,    0,  150,  150,  150,
  150,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  146,    0,    0,    0,    0,  152,  152,    0,  152,
  152,  150,    0,    0,  148,  148,  148,  148,  148,  148,
  148,    0,    0,  148,    0,  148,  148,    0,    0,  148,
    0,    0,    0,    0,  146,  148,  148,  148,    0,  152,
    0,    0,  146,  150,    0,  146,    0,    0,  148,  148,
  148,  148,  148,  148,  148,    0,    0,    0,    0,    0,
  146,  146,  146,  146,    0,  149,  149,  149,  149,  149,
  149,  149,    0,    0,  149,    0,  149,  149,    0,    0,
  149,    0,    0,    0,    0,    0,  149,  149,  149,  148,
  148,  147,  148,  148,  146,    0,    0,    0,    0,  149,
  149,  149,  149,  149,  149,  149,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  147,    0,  146,    0,    0,    0,
    0,    0,  147,    0,    0,  147,  142,    0,    0,    0,
  149,  149,    0,  149,  149,    0,    0,    0,    0,    0,
  147,  147,  147,  147,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  138,  142,    0,    0,
  142,    0,    0,    0,  147,    0,    0,    0,  150,  150,
  150,  150,  150,  150,  150,  142,    0,  150,    0,  150,
  150,    0,    0,  150,    0,    0,    0,    0,    0,  150,
  150,  150,    0,    0,    0,    0,  147,  138,    0,    0,
  138,    0,  150,  150,  150,  150,  150,  150,  150,  142,
    0,    0,    0,    0,    0,  138,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  139,
    0,  142,    0,  150,  150,    0,  150,  150,    0,  138,
    0,  146,  146,  146,  146,  146,  146,  146,    0,    0,
  146,    0,  146,  146,    0,    0,  146,    0,    0,    0,
    0,    0,  146,  146,    0,    0,    0,    0,    0,    0,
  139,  138,    0,  139,    0,  146,  146,  146,  146,  146,
  146,  146,    0,    0,    0,    0,    0,   10,  139,    0,
    0,   10,    0,   10,   10,    0,   10,   10,   10,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   10,  146,  146,    0,  146,
  146,    0,  139,    0,    0,    0,    0,    0,    0,    0,
    0,  147,  147,  147,  147,  147,  147,  147,    0,    0,
  147,    0,  147,  147,    0,    0,  147,    0,    0,    0,
    0,    0,  147,  147,  139,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  147,  147,  147,  147,  147,
  147,  147,    0,    0,    0,    0,  142,  142,  142,  142,
  142,  142,  142,    0,    0,  142,    0,  142,  142,    0,
    0,  142,    0,    0,    0,    0,    0,  142,  142,    0,
    0,    0,    0,    0,    0,    0,  147,  147,    0,  147,
  147,    0,    0,    0,    0,    0,  138,  138,  138,  138,
  138,  138,  138,    0,    0,  138,    0,  138,  138,    0,
  176,  138,    0,    0,  176,    0,  176,  176,  138,  176,
  176,  176,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  142,  142,    0,  142,  142,    0,    0,  176,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  138,  138,    0,  138,  138,    0,    0,    0,  139,
  139,  139,  139,  139,  139,  139,    0,    0,  139,    0,
  139,  139,    0,    0,  139,   10,   10,    0,    0,    0,
    0,  139,   10,   10,    0,    0,    0,    0,   10,   10,
    0,   10,    0,    0,   10,    0,    0,   10,   10,   10,
   10,   10,    0,    0,    0,    0,    0,    0,  178,    0,
    0,    0,  178,    0,  178,  178,    0,  178,  178,  178,
    0,    0,   10,    0,    0,   10,   10,   10,   10,   10,
   10,   10,   10,    0,  139,  139,  178,  139,  139,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   10,   10,   10,   10,   10,   10,    0,   10,    0,
   10,    0,    0,    0,    0,    0,   10,   10,   10,   10,
   10,   10,   10,   10,   10,   10,   10,   10,   10,    0,
    0,   10,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  221,    0,    0,    0,  222,    0,
  225,    0,    0,    0,  226,  223,    0,    0,  176,  176,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  224,    0,    0,    0,    0,    0,    0,    0,
  176,  176,  176,  176,  176,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  176,    0,    0,  176,  176,
  176,  176,  176,  176,  176,  176,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  176,  176,  176,  176,  176,  176,
    0,  176,    0,  176,    0,    0,    0,    0,    0,  176,
  176,  176,  176,  176,  176,  176,  176,  176,  176,  176,
  176,  176,    0,    0,  176,  171,  178,  178,    0,  171,
    0,  171,    0,    0,    0,  171,  171,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  178,  178,
  178,  178,  178,  171,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  178,    0,    0,  178,  178,  178,  178,
  178,  178,  178,  178,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  178,  178,  178,  178,  178,  178,    0,  178,
    0,  178,  183,  184,    0,    0,    0,  178,  178,  178,
  178,  178,  178,  178,  178,  178,  178,  178,  178,  178,
    0,    0,  178,    0,  185,  186,  187,  188,  189,    0,
    0,    0,  221,    0,    0,    0,  222,    0,  225,    0,
    0,    0,  226,    0,    0,    0,    0,    0,    0,  190,
    0,    0,  191,  192,  193,  194,  195,  196,  197,  198,
  224,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  199,  200,
  201,  202,  203,  204,    0,  205,    0,  206,    0,    0,
    0,    0,    0,  207,  208,  209,  210,  211,  212,  213,
  214,  215,  216,  217,  218,  219,    0,    0,  220,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  171,  171,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  171,  171,  171,  171,  171,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  171,    0,    0,  171,  171,  171,  171,  171,  171,  171,
  171,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  171,
  171,  171,  171,  171,  171,    0,  171,    0,  171,    0,
    0,    0,    0,    0,  171,  171,  171,  171,  171,  171,
  171,  171,  171,  171,  171,  171,  171,    0,    0,  171,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  183,  184,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  185,  186,  187,  188,  189,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  191,  192,  193,  194,  195,  196,  197,  198,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  199,  200,  201,  202,
    0,  204,    0,  205,    0,    0,    0,    0,    0,    0,
    0,  207,  208,  209,  210,  211,  212,  213,  214,  215,
  216,  217,  218,  219,    0,    0,  220,
  };
  protected static  short [] yyCheck = {            22,
    4,  180,   41,   34,   34,   34,   34,  179,  180,   13,
   14,  143,   47,  145,  146,  147,  148,  123,   44,   60,
   40,   59,   40,   47,   58,   40,   58,   41,   42,   41,
   42,   41,  123,   36,   47,   44,   44,   43,   62,   45,
  414,  124,   44,  270,   48,  123,  123,   44,  263,   62,
  551,   40,   56,   57,   44,  123,   40,  125,   25,  316,
   60,  294,  295,  296,   44,   44,  274,  275,  276,   27,
  324,   44,   44,   44,   44,  258,   44,  260,   44,  123,
   44,  125,  123,  263,  125,   47,   44,   44,   44,   44,
  203,   44,  123,   44,  125,  123,   44,  125,   44,   44,
   62,   60,   41,  123,  358,  123,  276,  598,  123,  320,
  223,  115,  291,   93,  605,  320,  139,  140,  122,  291,
   47,  348,  349,  123,   63,  125,  224,  628,  270,  271,
  272,  273,  347,  238,  123,   62,  316,  345,  346,  123,
  107,  239,  247,  263,  242,  378,  125,  648,   60,  650,
  108,  109,  125,  125,  125,  125,  657,  125,  659,  125,
  263,  125,  552,   47,  123,  555,  125,  321,  125,  125,
  125,  303,  125,  305,  125,  345,  346,  125,   62,  125,
  125,   41,  316,   43,  316,   45,  318,  561,  266,  266,
  268,   36,  654,  655,  171,  172,  173,  174,   33,  222,
   43,   41,   45,  665,   44,  321,  348,  349,  598,   41,
  672,  123,   44,  125,  297,  605,  298,   59,  300,  257,
  302,  312,  313,  337,   61,   60,   61,   62,  342,    0,
  344,  399,  266,  401,  266,   41,  404,   41,   44,  258,
   44,  260,  374,   41,  270,  320,   44,   41,  287,   41,
   44,  320,   44,  359,  381,  382,  383,  325,  326,  327,
  267,  270,  270,  258,  259,   36,   41,  270,   43,   40,
   45,   42,   43,  270,   45,   46,   47,  279,  345,  346,
  284,  325,  326,  327,  325,  326,  327,  293,  263,  279,
  165,  166,  167,   64,  325,  326,  327,  325,  326,  327,
  324,  321,  434,  321,  436,  320,  316,  321,  321,  321,
  320,  379,   61,  352,  355,   44,  357,  385,  316,  354,
  361,  500,  280,  281,   40,  325,  326,  327,  500,  264,
  265,  510,  355,  356,  357,  379,  322,  360,  510,  380,
  363,  385,  365,    0,  367,   36,   43,  370,   45,  379,
  379,  298,  299,  384,  377,  355,  384,  357,   43,   36,
   45,  361,  267,   36,  269,  497,  325,  326,  327,  261,
  262,  394,   60,   61,   40,  398,   40,  509,  292,   36,
  380,  513,   43,   40,   45,   42,   43,  291,   45,   46,
   47,   61,   62,  281,  282,   44,  355,    0,  357,  169,
  170,  300,  361,  175,  176,  409,  538,   64,  177,  178,
  301,  271,  272,  325,  326,  327,  302,  277,  278,  316,
  280,  380,  303,  283,  428,   80,   81,   61,  271,  272,
  453,  267,  564,  565,  277,  278,   61,  280,   41,  268,
  283,   44,  465,  355,   36,  357,  396,  397,  471,  361,
  181,  182,  322,   44,  322,   44,   59,  322,  637,  270,
  271,  272,  273,  270,  125,  637,   61,  321,  380,  304,
  305,  306,  307,  308,  309,  310,  123,  123,  501,  123,
   40,   58,  614,  615,   42,  617,   40,    0,   40,   40,
   93,   40,  316,  264,  265,  316,  271,  272,  324,  321,
  271,  272,  277,  278,  322,  280,  277,  278,  283,  280,
   58,  515,  283,  517,   44,  286,  287,  288,  289,  290,
   91,  653,  125,   36,   41,  322,   44,   40,  660,   42,
   43,  270,   45,   46,   47,   44,   40,  348,  349,   40,
  311,   40,   40,  314,  315,  316,  317,  318,  319,  320,
  321,   64,   40,   63,  123,  123,   41,   41,  581,  123,
   41,  123,  585,   42,  321,  321,  356,  321,  320,  573,
  593,  270,  271,  272,  273,   61,  324,  267,   58,  350,
  351,  352,  353,  354,  355,   43,  357,   45,  359,  269,
  284,   41,  281,  276,  365,  366,  367,  368,  369,  370,
  371,  372,  373,  374,  375,  376,  377,  264,  265,  380,
  271,  272,   41,   41,  271,  272,  277,  278,   41,  280,
  277,  278,  283,  280,   41,   41,  283,  323,   36,  286,
  287,  288,  289,  290,  316,   61,  269,  324,  276,  358,
   43,  123,   45,   36,    0,  321,  123,  125,  123,  348,
  349,  360,   62,   62,  311,  322,  321,  314,  315,  316,
  317,  318,  319,  320,  321,  123,  322,  270,  271,  272,
  273,  274,  275,  276,  270,  285,  279,   33,  281,  282,
   41,   61,  285,  125,  316,   41,  268,   43,   44,   45,
   47,   47,   47,  350,  351,  352,  353,  354,  355,  322,
  357,   41,  359,   59,   60,   61,   62,  125,  365,  366,
  367,  368,  369,  370,  371,  372,  373,  374,  375,  376,
  377,  362,  125,  380,  321,   47,   47,  321,  270,  287,
  288,  289,  290,  321,  321,   91,  270,   93,   62,   62,
  316,    0,  345,  346,   62,  348,  349,   59,   62,    0,
   59,  264,  265,   41,  321,   59,  314,   41,  271,  272,
   58,  269,  320,  321,  277,  278,   59,  280,  124,  125,
  283,  125,  269,  286,  287,  288,  289,  290,  270,  270,
   61,   19,   62,   43,   19,   45,   23,  283,    0,  408,
   24,   24,  350,  351,  352,  353,  413,  561,  311,  415,
  149,  314,  315,  316,  317,  318,  319,  320,  321,  299,
  148,   90,   90,  271,  272,   77,   43,  432,   45,  277,
  278,   33,  280,  434,  144,  283,  153,  301,  304,   41,
  511,   43,   44,   45,  168,   47,  182,  350,  351,  352,
  353,  354,  355,  231,  357,  555,  359,   59,   60,   61,
   62,  557,  365,  366,  367,  368,  369,  370,  371,  372,
  373,  374,  375,  376,  377,  125,  665,  380,  271,  272,
  672,  578,  378,  378,  277,  278,  379,  280,  531,   91,
  283,   93,   -1,   -1,   -1,  380,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  287,  288,  289,  290,  125,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  124,  125,  270,  271,  272,  273,  274,  275,
  276,  314,    0,  279,   -1,  281,  282,   -1,  321,  285,
   -1,   -1,   -1,   -1,   -1,  291,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,  305,
  306,  307,  308,  309,  310,   33,   -1,  350,  351,  352,
  353,   -1,   -1,   41,   -1,   43,   44,   45,   -1,   47,
  363,  364,  287,  288,  289,  290,   -1,   -1,   -1,   -1,
   -1,   59,   60,   61,   62,   -1,   -1,   -1,   -1,  345,
  346,   -1,  348,  349,   -1,   -1,   -1,   -1,  354,  314,
   -1,   -1,   -1,   -1,   -1,   -1,  321,   -1,   -1,   -1,
   -1,  271,  272,   -1,   -1,   93,   -1,  277,  278,   -1,
  280,   -1,  378,  283,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  350,  351,  352,  353,   -1,
   -1,   -1,   -1,   -1,  271,  272,  124,  125,  363,  364,
  277,  278,   -1,  280,   -1,   -1,  283,   -1,  270,  271,
  272,  273,  274,  275,  276,   -1,    0,  279,   -1,  281,
  282,   -1,   -1,  285,   43,   -1,   45,   -1,   -1,  291,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,  305,  306,  307,  308,  309,  310,   33,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   41,   -1,   43,
   44,   45,   -1,   47,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   59,   60,   61,   62,   -1,
   -1,   -1,   -1,  345,  346,   -1,  348,  349,   -1,   -1,
   -1,   -1,  354,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  125,   -1,   -1,   93,
   -1,   -1,   -1,   -1,   -1,   -1,  378,  328,  329,  330,
  331,  332,  333,  334,  335,  336,  337,  338,  339,  340,
  341,  342,  343,  344,   -1,   -1,   -1,   -1,   -1,   -1,
  124,  125,  270,  271,  272,  273,  274,  275,  276,   -1,
    0,  279,   -1,  281,  282,   -1,   43,  285,   45,   -1,
   -1,   -1,   -1,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,  305,  306,  307,
  308,  309,  310,   33,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   41,   -1,   43,   44,   45,   -1,   47,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   59,
   60,   61,   62,   -1,   -1,   -1,   -1,  345,  346,   -1,
  348,  349,   -1,   -1,   -1,   -1,  354,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  125,   -1,
   -1,   -1,   -1,   93,   -1,   -1,   -1,   -1,   -1,   -1,
  378,   -1,  271,  272,   -1,   -1,   -1,   -1,  277,  278,
   -1,  280,   -1,   -1,  283,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  124,  125,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  270,  271,  272,  273,
  274,  275,  276,   -1,    0,  279,   -1,  281,  282,   -1,
   -1,  285,   -1,   -1,   -1,   -1,   -1,  291,  292,  293,
  294,  295,  296,  297,  298,  299,  300,  301,  302,  303,
  304,  305,  306,  307,  308,  309,  310,   33,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   41,   -1,   43,   44,   45,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   58,   59,   60,   61,   62,   -1,   -1,   -1,
   -1,  345,  346,   -1,  348,  349,   -1,   -1,   -1,   -1,
  354,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  271,  272,   -1,   93,   -1,   -1,
  277,  278,   -1,  280,  378,   -1,  283,  329,  330,  331,
  332,  333,  334,  335,  336,  337,  338,  339,  340,  341,
  342,  343,  344,   -1,   -1,   -1,   -1,  123,  124,  125,
  270,  271,  272,  273,  274,  275,  276,   -1,    0,  279,
   -1,  281,  282,   -1,   -1,  285,   -1,   -1,   -1,   -1,
   -1,  291,  292,  293,  294,  295,  296,  297,  298,  299,
  300,  301,  302,  303,  304,  305,  306,  307,  308,  309,
  310,   33,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   41,
   -1,   43,   44,   45,   -1,   47,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   59,   60,   61,
   62,   -1,   -1,   -1,   -1,  345,  346,   -1,  348,  349,
   -1,   -1,   -1,   -1,  354,  330,  331,  332,  333,  334,
  335,  336,  337,  338,  339,  340,  341,  342,  343,  344,
   -1,   93,   -1,   -1,   -1,   -1,   -1,   -1,  378,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  124,  125,   -1,   -1,   -1,   -1,   -1,   -1,
  266,  267,   -1,  269,  270,  271,  272,  273,  274,  275,
  276,   -1,    0,  279,   -1,  281,  282,   -1,   -1,  285,
   -1,   -1,   -1,   -1,   -1,  291,  292,  293,  294,  295,
  296,  297,  298,  299,  300,   -1,   -1,   -1,  304,  305,
  306,  307,  308,  309,  310,   33,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   41,   -1,   43,   44,   45,   -1,   47,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   59,   60,   61,   62,   -1,   -1,   -1,   -1,  345,
  346,   -1,  348,  349,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   93,   -1,   -1,   -1,   -1,
   -1,   -1,  378,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  124,  125,  270,  271,
  272,  273,  274,  275,  276,   -1,    0,  279,   -1,  281,
  282,   -1,   -1,  285,   -1,   -1,   -1,   -1,   -1,  291,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,  305,  306,  307,  308,  309,  310,   33,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   41,   -1,   43,
   44,   45,   -1,   47,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   59,   60,   61,   62,   -1,
   -1,   -1,   -1,  345,  346,   -1,  348,  349,   -1,   -1,
   -1,   -1,  354,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   93,
   -1,   -1,   -1,   -1,   -1,   -1,  378,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  124,  125,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  270,  271,  272,  273,  274,  275,  276,   -1,
    0,  279,   -1,  281,  282,   -1,   -1,  285,   -1,   -1,
   -1,   -1,   -1,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,  305,  306,  307,
  308,  309,  310,   33,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   41,   -1,   43,   44,   45,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   59,
   60,   61,   62,   -1,   -1,   -1,   -1,  345,  346,   -1,
  348,  349,   -1,   -1,   -1,   -1,  354,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   93,   -1,   -1,   -1,   -1,   -1,   -1,
  378,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  124,  125,  270,  271,  272,  273,
  274,  275,  276,   -1,    0,  279,   -1,  281,  282,   -1,
   -1,  285,   -1,   -1,   -1,   -1,   -1,  291,  292,  293,
  294,  295,  296,  297,  298,  299,  300,  301,  302,  303,
  304,  305,  306,  307,  308,  309,  310,   33,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   41,   -1,   43,   44,   45,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   59,   60,   61,   62,   -1,   -1,   -1,
   -1,  345,  346,   -1,  348,  349,   -1,   -1,   -1,   -1,
  354,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   93,   -1,   -1,
   -1,   -1,   -1,   -1,  378,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  124,  125,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  270,  271,  272,  273,  274,  275,  276,   -1,    0,  279,
   -1,  281,  282,   -1,   -1,  285,   -1,   -1,   -1,   -1,
   -1,  291,  292,  293,  294,  295,  296,  297,  298,  299,
  300,  301,  302,  303,  304,  305,  306,  307,  308,  309,
  310,   33,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   41,
   -1,   43,   44,   45,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   59,   60,   61,
   62,   -1,   -1,   -1,   -1,  345,  346,   -1,  348,  349,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   93,   -1,   -1,   -1,   -1,   -1,   -1,  378,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  124,  125,  270,  271,  272,  273,  274,  275,
  276,   -1,    0,  279,   -1,  281,  282,   -1,   -1,  285,
   -1,   -1,   -1,   -1,   -1,  291,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,  305,
  306,  307,  308,  309,  310,   33,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   41,   -1,   43,   44,   45,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   59,   60,   61,   62,   -1,   -1,   -1,   -1,  345,
  346,   -1,  348,  349,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   93,   -1,   -1,   -1,   -1,
   -1,   -1,  378,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  124,  125,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  270,  271,
  272,  273,  274,  275,  276,   -1,    0,  279,   -1,  281,
  282,   -1,   -1,  285,   -1,   -1,   -1,   -1,   -1,  291,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,  305,  306,  307,  308,  309,  310,   33,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   41,   -1,   43,
   44,   45,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   59,   60,   61,   62,   -1,
   -1,   -1,   -1,  345,  346,   -1,  348,  349,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   93,
   -1,   -1,   -1,   -1,   -1,   -1,  378,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  124,  125,  270,  271,  272,  273,  274,  275,  276,   -1,
    0,  279,   -1,  281,  282,   -1,   -1,  285,   -1,   -1,
   -1,   -1,   -1,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,  305,  306,  307,
  308,  309,  310,   33,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   41,   -1,   43,   44,   45,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   59,
   60,   61,   62,   -1,   -1,   -1,   -1,  345,  346,   -1,
  348,  349,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   93,   -1,   -1,   -1,   -1,   -1,   -1,
  378,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  124,  125,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  270,  271,  272,  273,
  274,  275,  276,   -1,    0,  279,   -1,  281,  282,   -1,
   -1,  285,   -1,   -1,   -1,   -1,   -1,  291,  292,  293,
  294,  295,  296,  297,  298,  299,  300,  301,  302,   -1,
  304,  305,  306,  307,  308,  309,  310,   33,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   41,   -1,   43,   44,   45,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   59,   60,   61,   62,   -1,   -1,   -1,
   -1,  345,  346,   -1,  348,  349,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   93,   -1,   -1,
   -1,   -1,   -1,   -1,  378,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  124,  125,
  270,  271,  272,  273,  274,  275,  276,   -1,    0,  279,
   -1,  281,  282,   -1,   -1,  285,   -1,   -1,   -1,   -1,
   -1,  291,  292,  293,  294,  295,  296,  297,  298,  299,
  300,  301,  302,   -1,  304,  305,  306,  307,  308,  309,
  310,   33,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   41,
   -1,   43,   44,   45,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   59,   60,   61,
   62,   -1,   -1,   -1,   -1,  345,  346,   -1,  348,  349,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   93,   -1,   -1,   -1,   -1,   -1,   -1,  378,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  124,  125,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  270,  271,  272,  273,  274,  275,
  276,   -1,    0,  279,   -1,  281,  282,   -1,   -1,  285,
   -1,   -1,   -1,   -1,   -1,  291,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,   -1,   -1,  304,  305,
  306,  307,  308,  309,  310,   33,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   41,   -1,   43,   44,   45,    0,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   59,   60,   61,   62,   -1,   -1,   -1,   -1,  345,
  346,   -1,  348,  349,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   33,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   41,
   -1,   43,   44,   45,   -1,   93,   -1,   -1,   -1,   -1,
   -1,   -1,  378,   -1,   -1,   -1,   -1,   59,   60,   61,
   62,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  124,  125,  270,  271,
  272,  273,  274,  275,  276,   -1,    0,  279,   -1,  281,
  282,   93,   -1,  285,   -1,   -1,   -1,   -1,   -1,  291,
  292,  293,  294,  295,  296,  297,  298,  299,  300,   -1,
   -1,   -1,  304,  305,  306,  307,  308,  309,  310,   33,
   -1,   -1,  124,  125,   -1,   -1,   -1,   41,   -1,   43,
   44,   45,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   59,   60,   61,   62,   -1,
   -1,   -1,   -1,  345,  346,   -1,  348,  349,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   93,
   -1,   -1,   -1,   -1,   -1,   -1,  378,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  124,  125,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  270,  271,  272,  273,  274,  275,  276,   -1,
   -1,  279,   -1,  281,  282,   -1,   -1,  285,   -1,   -1,
   -1,   -1,   -1,  291,  292,  293,  294,  295,  296,  297,
  298,  299,   -1,   -1,   -1,   -1,  304,  305,  306,  307,
  308,  309,  310,   -1,   -1,   -1,   -1,   -1,  270,  271,
  272,  273,  274,  275,  276,   -1,    0,  279,   -1,  281,
  282,   -1,   -1,  285,   -1,   -1,   -1,   -1,   -1,  291,
  292,  293,  294,  295,  296,  297,   -1,  345,  346,   -1,
  348,  349,  304,  305,  306,  307,  308,  309,  310,   33,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   41,   -1,   43,
   44,   45,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  378,   -1,   -1,   -1,   -1,   59,   60,   61,   62,   -1,
   -1,   -1,   -1,  345,  346,   -1,  348,  349,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  270,  271,  272,  273,
  274,  275,  276,   -1,    0,  279,   -1,  281,  282,   93,
   -1,  285,   -1,   -1,   -1,   -1,  378,  291,  292,  293,
  294,  295,  296,  297,   -1,   -1,   -1,   -1,   -1,   -1,
  304,  305,  306,  307,  308,  309,  310,   33,   -1,   -1,
  124,  125,   -1,   -1,   -1,   41,   -1,   43,   44,   45,
    0,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   59,   60,   61,   62,   -1,   -1,   -1,
   -1,  345,  346,   -1,  348,  349,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   33,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   41,   -1,   43,   44,   45,   -1,   93,   -1,   -1,
   -1,   -1,   -1,   -1,  378,   -1,   -1,   -1,   -1,   59,
   60,   61,   62,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  125,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   93,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  125,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  270,  271,  272,  273,
  274,  275,  276,   -1,    0,  279,   -1,  281,  282,   -1,
   -1,  285,   -1,   -1,   -1,   -1,   -1,  291,  292,  293,
  294,  295,  296,  297,   -1,   -1,   -1,   -1,   -1,   -1,
  304,  305,  306,  307,  308,  309,  310,   33,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   41,   -1,   43,   44,   45,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   59,   60,   61,   62,   -1,   -1,   -1,
   -1,  345,  346,   -1,  348,  349,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  270,  271,  272,  273,  274,  275,
  276,   -1,   -1,  279,   -1,  281,  282,   93,   -1,  285,
   -1,   -1,   -1,   -1,  378,  291,  292,  293,  294,  295,
  296,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  304,  305,
  306,  307,  308,  309,  310,   -1,   -1,   -1,   -1,  125,
  270,  271,  272,  273,  274,  275,  276,   -1,    0,  279,
   -1,  281,  282,   -1,   -1,  285,   -1,   -1,   -1,   -1,
   -1,  291,  292,  293,  294,  295,  296,   -1,   -1,  345,
  346,   -1,  348,  349,  304,  305,  306,  307,  308,  309,
  310,   33,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   41,
   -1,   43,   44,   45,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  378,   -1,   -1,   -1,   -1,   59,   60,   61,
   62,   -1,   -1,   -1,   -1,  345,  346,   -1,  348,  349,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   93,   -1,   -1,   -1,   -1,   -1,   -1,  378,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,    0,   -1,  125,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  270,  271,  272,  273,  274,  275,
  276,   -1,   -1,  279,   -1,  281,  282,   -1,   -1,  285,
   -1,   -1,   -1,   -1,   33,  291,  292,  293,  294,  295,
  296,   -1,   41,   -1,   43,   44,   45,   -1,  304,  305,
  306,  307,  308,  309,  310,   -1,   -1,   -1,   -1,   -1,
   59,   60,   61,   62,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,    0,   -1,   -1,   -1,   -1,  345,
  346,   -1,  348,  349,   93,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   33,   -1,   -1,
   -1,   -1,  378,   -1,   -1,   41,  125,   43,   44,   45,
   -1,   -1,   -1,   -1,   -1,    0,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   59,   60,   61,   62,   -1,  270,  271,
  272,  273,  274,  275,  276,   -1,   -1,  279,   -1,  281,
  282,   -1,   -1,  285,   -1,   -1,   -1,   -1,   33,  291,
  292,  293,  294,  295,  296,   -1,   41,   93,   43,   44,
   45,   -1,  304,  305,  306,  307,  308,  309,  310,   -1,
   -1,   -1,   -1,   -1,   59,   60,   61,   62,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  125,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  345,  346,   -1,  348,  349,   93,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  378,   -1,    0,   -1,
  125,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  270,  271,  272,  273,  274,  275,  276,   -1,   -1,
  279,   -1,  281,  282,   -1,   -1,  285,   -1,   -1,   -1,
   -1,   33,  291,  292,  293,  294,  295,  296,   -1,   41,
   -1,   43,   44,   45,   -1,  304,  305,  306,  307,  308,
  309,  310,   -1,   -1,   -1,   -1,   -1,   59,   60,   61,
   62,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,    0,   -1,   -1,   -1,   -1,  345,  346,   -1,  348,
  349,   93,   -1,   -1,  270,  271,  272,  273,  274,  275,
  276,   -1,   -1,  279,   -1,  281,  282,   -1,   -1,  285,
   -1,   -1,   -1,   -1,   33,  291,  292,  293,   -1,  378,
   -1,   -1,   41,  125,   -1,   44,   -1,   -1,  304,  305,
  306,  307,  308,  309,  310,   -1,   -1,   -1,   -1,   -1,
   59,   60,   61,   62,   -1,  270,  271,  272,  273,  274,
  275,  276,   -1,   -1,  279,   -1,  281,  282,   -1,   -1,
  285,   -1,   -1,   -1,   -1,   -1,  291,  292,  293,  345,
  346,    0,  348,  349,   93,   -1,   -1,   -1,   -1,  304,
  305,  306,  307,  308,  309,  310,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   33,   -1,  125,   -1,   -1,   -1,
   -1,   -1,   41,   -1,   -1,   44,    0,   -1,   -1,   -1,
  345,  346,   -1,  348,  349,   -1,   -1,   -1,   -1,   -1,
   59,   60,   61,   62,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,    0,   41,   -1,   -1,
   44,   -1,   -1,   -1,   93,   -1,   -1,   -1,  270,  271,
  272,  273,  274,  275,  276,   59,   -1,  279,   -1,  281,
  282,   -1,   -1,  285,   -1,   -1,   -1,   -1,   -1,  291,
  292,  293,   -1,   -1,   -1,   -1,  125,   41,   -1,   -1,
   44,   -1,  304,  305,  306,  307,  308,  309,  310,   93,
   -1,   -1,   -1,   -1,   -1,   59,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,    0,
   -1,  125,   -1,  345,  346,   -1,  348,  349,   -1,   93,
   -1,  270,  271,  272,  273,  274,  275,  276,   -1,   -1,
  279,   -1,  281,  282,   -1,   -1,  285,   -1,   -1,   -1,
   -1,   -1,  291,  292,   -1,   -1,   -1,   -1,   -1,   -1,
   41,  125,   -1,   44,   -1,  304,  305,  306,  307,  308,
  309,  310,   -1,   -1,   -1,   -1,   -1,   36,   59,   -1,
   -1,   40,   -1,   42,   43,   -1,   45,   46,   47,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   64,  345,  346,   -1,  348,
  349,   -1,   93,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  270,  271,  272,  273,  274,  275,  276,   -1,   -1,
  279,   -1,  281,  282,   -1,   -1,  285,   -1,   -1,   -1,
   -1,   -1,  291,  292,  125,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  304,  305,  306,  307,  308,
  309,  310,   -1,   -1,   -1,   -1,  270,  271,  272,  273,
  274,  275,  276,   -1,   -1,  279,   -1,  281,  282,   -1,
   -1,  285,   -1,   -1,   -1,   -1,   -1,  291,  292,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  345,  346,   -1,  348,
  349,   -1,   -1,   -1,   -1,   -1,  270,  271,  272,  273,
  274,  275,  276,   -1,   -1,  279,   -1,  281,  282,   -1,
   36,  285,   -1,   -1,   40,   -1,   42,   43,  292,   45,
   46,   47,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  345,  346,   -1,  348,  349,   -1,   -1,   64,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  345,  346,   -1,  348,  349,   -1,   -1,   -1,  270,
  271,  272,  273,  274,  275,  276,   -1,   -1,  279,   -1,
  281,  282,   -1,   -1,  285,  264,  265,   -1,   -1,   -1,
   -1,  292,  271,  272,   -1,   -1,   -1,   -1,  277,  278,
   -1,  280,   -1,   -1,  283,   -1,   -1,  286,  287,  288,
  289,  290,   -1,   -1,   -1,   -1,   -1,   -1,   36,   -1,
   -1,   -1,   40,   -1,   42,   43,   -1,   45,   46,   47,
   -1,   -1,  311,   -1,   -1,  314,  315,  316,  317,  318,
  319,  320,  321,   -1,  345,  346,   64,  348,  349,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  350,  351,  352,  353,  354,  355,   -1,  357,   -1,
  359,   -1,   -1,   -1,   -1,   -1,  365,  366,  367,  368,
  369,  370,  371,  372,  373,  374,  375,  376,  377,   -1,
   -1,  380,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   36,   -1,   -1,   -1,   40,   -1,
   42,   -1,   -1,   -1,   46,   47,   -1,   -1,  264,  265,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   64,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  286,  287,  288,  289,  290,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  311,   -1,   -1,  314,  315,
  316,  317,  318,  319,  320,  321,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  350,  351,  352,  353,  354,  355,
   -1,  357,   -1,  359,   -1,   -1,   -1,   -1,   -1,  365,
  366,  367,  368,  369,  370,  371,  372,  373,  374,  375,
  376,  377,   -1,   -1,  380,   36,  264,  265,   -1,   40,
   -1,   42,   -1,   -1,   -1,   46,   47,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  286,  287,
  288,  289,  290,   64,   -1,   -1,   -1,   -1,   -1,   -1,
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
   -1,   -1,   46,   -1,   -1,   -1,   -1,   -1,   -1,  311,
   -1,   -1,  314,  315,  316,  317,  318,  319,  320,  321,
   64,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  350,  351,
  352,  353,  354,  355,   -1,  357,   -1,  359,   -1,   -1,
   -1,   -1,   -1,  365,  366,  367,  368,  369,  370,  371,
  372,  373,  374,  375,  376,  377,   -1,   -1,  380,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  264,  265,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  286,  287,  288,  289,  290,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  311,   -1,   -1,  314,  315,  316,  317,  318,  319,  320,
  321,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  350,
  351,  352,  353,  354,  355,   -1,  357,   -1,  359,   -1,
   -1,   -1,   -1,   -1,  365,  366,  367,  368,  369,  370,
  371,  372,  373,  374,  375,  376,  377,   -1,   -1,  380,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  264,  265,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  286,  287,  288,  289,  290,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  314,  315,  316,  317,  318,  319,  320,  321,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  350,  351,  352,  353,
   -1,  355,   -1,  357,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  365,  366,  367,  368,  369,  370,  371,  372,  373,
  374,  375,  376,  377,   -1,   -1,  380,
  };

#line 1983 "XQuery.y"
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
  public const int EMPTY_SEQUENCE = 363;
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
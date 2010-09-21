
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
//t    "ForClause : PFOR ForClauseBody",
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
//t    "DirElemConstructor : BeginTag QName opt_S '[' PathExpr ']' opt_DirAttributeList '/' '>'",
//t    "DirElemConstructor : BeginTag QName opt_S '[' PathExpr ']' S '/' '>'",
//t    "DirElemConstructor : BeginTag QName opt_S '[' PathExpr ']' opt_DirAttributeList '>' '<' '/' QName opt_S '>'",
//t    "DirElemConstructor : BeginTag QName opt_S '[' PathExpr ']' S '>' '<' '/' QName opt_S '>'",
//t    "DirElemConstructor : BeginTag QName opt_S '[' PathExpr ']' opt_DirAttributeList '>' DirElemContentList '<' '/' QName opt_S '>'",
//t    "DirElemConstructor : BeginTag QName opt_S '[' PathExpr ']' S '>' DirElemContentList '<' '/' QName opt_S '>'",
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
    "IN","RETURN","FOR","PFOR","LET","WHERE","ASCENDING","DESCENDING",
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
#line 217 "XQuery.y"
  {
     notation.ConfirmTag(Tag.Module, Descriptor.Root, yyVals[0+yyTop]);
     yyVal = notation.ResolveTag(Tag.Module);	 
  }
  break;
case 2:
#line 222 "XQuery.y"
  {
	 notation.ConfirmTag(Tag.Module, Descriptor.Root, yyVals[0+yyTop]);	 
	 yyVal = notation.ResolveTag(Tag.Module);	 
  }
  break;
case 3:
#line 227 "XQuery.y"
  {
     notation.ConfirmTag(Tag.Module, Descriptor.Root, yyVals[0+yyTop]);	 
     yyVal = notation.ResolveTag(Tag.Module);	 
  }
  break;
case 4:
#line 232 "XQuery.y"
  {
	 notation.ConfirmTag(Tag.Module, Descriptor.Root, yyVals[0+yyTop]);	 
	 yyVal = notation.ResolveTag(Tag.Module);	 
  }
  break;
case 5:
#line 240 "XQuery.y"
  {
     notation.ConfirmTag(Tag.Module, Descriptor.Version, yyVals[-1+yyTop], null);
  }
  break;
case 6:
#line 244 "XQuery.y"
  {
     notation.ConfirmTag(Tag.Module, Descriptor.Version, yyVals[-3+yyTop], yyVals[-1+yyTop]);
  }
  break;
case 7:
#line 251 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.Query, yyVals[-1+yyTop], yyVals[0+yyTop]);
  }
  break;
case 8:
#line 258 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.Library, yyVals[-1+yyTop], yyVals[0+yyTop]);
  }
  break;
case 9:
#line 265 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.ModuleNamespace, yyVals[-3+yyTop], yyVals[-1+yyTop]);
  }
  break;
case 10:
#line 272 "XQuery.y"
  {
     yyVal = null;
  }
  break;
case 13:
#line 278 "XQuery.y"
  {
     yyVal = Lisp.Append(yyVals[-1+yyTop], yyVals[0+yyTop]);
  }
  break;
case 14:
#line 285 "XQuery.y"
  {
      yyVal = Lisp.Cons(yyVals[-1+yyTop]);
   }
  break;
case 15:
#line 289 "XQuery.y"
  {
      yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[-1+yyTop]));
   }
  break;
case 16:
#line 296 "XQuery.y"
  {
      yyVal = Lisp.Cons(yyVals[-1+yyTop]);
   }
  break;
case 17:
#line 300 "XQuery.y"
  {
      yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[-1+yyTop]));
   }
  break;
case 35:
#line 339 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.Namespace, yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 36:
#line 346 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Module), 
		Descriptor.BoundarySpace, new TokenWrapper(Token.PRESERVE));
  }
  break;
case 37:
#line 351 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Module), 
		Descriptor.BoundarySpace, new TokenWrapper(Token.STRIP));  
  }
  break;
case 38:
#line 359 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.DefaultElement, yyVals[0+yyTop]);
  }
  break;
case 39:
#line 363 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.DefaultFunction, yyVals[0+yyTop]);
  }
  break;
case 40:
#line 370 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.OptionDecl, yyVals[-1+yyTop], yyVals[0+yyTop]);
  }
  break;
case 41:
#line 377 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Module), 
		Descriptor.Ordering, new TokenWrapper(Token.ORDERED));  
  }
  break;
case 42:
#line 382 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Module), 
		Descriptor.Ordering, new TokenWrapper(Token.UNORDERED));  
  }
  break;
case 43:
#line 390 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Module), 
		Descriptor.DefaultOrder, new TokenWrapper(Token.EMPTY_GREATEST));  
  }
  break;
case 44:
#line 395 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Module), 
		Descriptor.DefaultOrder, new TokenWrapper(Token.EMPTY_LEAST));  
  }
  break;
case 45:
#line 403 "XQuery.y"
  {
	  yyVal = notation.Confirm(new Symbol(Tag.Module), 
	    Descriptor.CopyNamespace, yyVals[-2+yyTop], yyVals[0+yyTop]); 
  }
  break;
case 46:
#line 411 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.PRESERVE);
  }
  break;
case 47:
#line 415 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.NO_PRESERVE);
  }
  break;
case 48:
#line 422 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.INHERIT);
  }
  break;
case 49:
#line 426 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.NO_INHERIT);
  }
  break;
case 50:
#line 433 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.DefaultCollation, yyVals[0+yyTop]);
  }
  break;
case 51:
#line 440 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.BaseUri, yyVals[0+yyTop]);
  }
  break;
case 52:
#line 447 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Module), 
         Descriptor.ImportSchema, yyVals[-1+yyTop], yyVals[0+yyTop], null);
  }
  break;
case 53:
#line 452 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Module), 
         Descriptor.ImportSchema, yyVals[-3+yyTop], yyVals[-2+yyTop], yyVals[0+yyTop]);  
  }
  break;
case 54:
#line 460 "XQuery.y"
  { 
     yyVal = null;
  }
  break;
case 56:
#line 468 "XQuery.y"
  {
     yyVal = Lisp.Cons(yyVals[0+yyTop]);
  }
  break;
case 57:
#line 472 "XQuery.y"
  {
     yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
  }
  break;
case 58:
#line 479 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.Namespace, yyVals[-1+yyTop]);
  }
  break;
case 59:
#line 483 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.DefaultElement);
  }
  break;
case 60:
#line 490 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.ImportModule, yyVals[-1+yyTop], yyVals[0+yyTop]);
  }
  break;
case 61:
#line 494 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.ImportModule, yyVals[-3+yyTop], yyVals[-1+yyTop], yyVals[0+yyTop]);
  }
  break;
case 62:
#line 501 "XQuery.y"
  {
     yyVal = null;
  }
  break;
case 63:
#line 505 "XQuery.y"
  {
     yyVal = yyVals[0+yyTop];
  }
  break;
case 64:
#line 511 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.VarDecl, yyVals[-4+yyTop], yyVals[-3+yyTop], yyVals[0+yyTop]); 
  }
  break;
case 65:
#line 515 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.VarDecl, yyVals[-2+yyTop], yyVals[-1+yyTop]); 
  }
  break;
case 66:
#line 522 "XQuery.y"
  {
     yyVal = null;
  }
  break;
case 68:
#line 530 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.ConstructionDecl, 
		new TokenWrapper(Token.PRESERVE));
  }
  break;
case 69:
#line 535 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.ConstructionDecl, 
		new TokenWrapper(Token.STRIP));
  }
  break;
case 70:
#line 543 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.DeclareFunction, yyVals[-4+yyTop], yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 71:
#line 547 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Module), Descriptor.DeclareFunction, yyVals[-6+yyTop], yyVals[-4+yyTop], yyVals[-1+yyTop], yyVals[0+yyTop]);
  }
  break;
case 73:
#line 555 "XQuery.y"
  {
     yyVal = null;
  }
  break;
case 74:
#line 562 "XQuery.y"
  {
     yyVal = null;
  }
  break;
case 76:
#line 570 "XQuery.y"
  {
     yyVal = Lisp.Cons(yyVals[0+yyTop]);
  }
  break;
case 77:
#line 574 "XQuery.y"
  {
     yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
  }
  break;
case 78:
#line 581 "XQuery.y"
  {
     yyVal = yyVals[0+yyTop];
  }
  break;
case 79:
#line 585 "XQuery.y"
  {
     yyVal = yyVals[-1+yyTop];
     notation.Confirm((Symbol)yyVals[-1+yyTop], Descriptor.TypeDecl, yyVals[0+yyTop]);
  }
  break;
case 80:
#line 593 "XQuery.y"
  {
     yyVal = yyVals[-1+yyTop];
  }
  break;
case 82:
#line 604 "XQuery.y"
  {
     yyVal = Lisp.Cons(yyVals[0+yyTop]);
  }
  break;
case 83:
#line 608 "XQuery.y"
  {
     yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
  }
  break;
case 89:
#line 623 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.FLWORExpr, yyVals[-2+yyTop], null, null, yyVals[0+yyTop]);
  }
  break;
case 90:
#line 627 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.FLWORExpr, yyVals[-3+yyTop], yyVals[-2+yyTop], null, yyVals[0+yyTop]);
  }
  break;
case 91:
#line 631 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.FLWORExpr, yyVals[-3+yyTop], null, yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 92:
#line 635 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.FLWORExpr, yyVals[-4+yyTop], yyVals[-3+yyTop], yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 93:
#line 642 "XQuery.y"
  {
     yyVal = Lisp.Cons(yyVals[0+yyTop]);
  }
  break;
case 94:
#line 646 "XQuery.y"
  {
     yyVal = Lisp.Cons(yyVals[0+yyTop]);
  }
  break;
case 95:
#line 650 "XQuery.y"
  {
     yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(yyVals[0+yyTop]));
  }
  break;
case 96:
#line 654 "XQuery.y"
  {
     yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(yyVals[0+yyTop]));
  }
  break;
case 97:
#line 661 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.For, yyVals[0+yyTop]);
  }
  break;
case 98:
#line 665 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.For, yyVals[0+yyTop]);
	 notation.Confirm((Symbol)yyVal, Descriptor.Parallel);
  }
  break;
case 99:
#line 673 "XQuery.y"
  {
     yyVal = Lisp.Cons(yyVals[0+yyTop]);
  }
  break;
case 100:
#line 677 "XQuery.y"
  {
     yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
  }
  break;
case 101:
#line 684 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.ForClauseOperator, yyVals[-4+yyTop], yyVals[-3+yyTop], yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 102:
#line 691 "XQuery.y"
  {
     yyVal = null;
  }
  break;
case 104:
#line 699 "XQuery.y"
  {
     yyVal = yyVals[0+yyTop];
  }
  break;
case 105:
#line 706 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Let, yyVals[0+yyTop]);
  }
  break;
case 106:
#line 713 "XQuery.y"
  {
     yyVal = Lisp.Cons(yyVals[0+yyTop]);
  }
  break;
case 107:
#line 717 "XQuery.y"
  {
     yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
  }
  break;
case 108:
#line 724 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.LetClauseOperator, yyVals[-4+yyTop], yyVals[-3+yyTop], yyVals[0+yyTop]);
  }
  break;
case 109:
#line 731 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Where, yyVals[0+yyTop]);
  }
  break;
case 110:
#line 738 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.OrderBy, yyVals[0+yyTop]);
  }
  break;
case 111:
#line 742 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.StableOrderBy, yyVals[0+yyTop]);
  }
  break;
case 112:
#line 749 "XQuery.y"
  {
     yyVal = Lisp.Cons(yyVals[0+yyTop]);
  }
  break;
case 113:
#line 753 "XQuery.y"
  {
     yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
  }
  break;
case 115:
#line 761 "XQuery.y"
  {
     yyVal = yyVals[-1+yyTop];
     notation.Confirm((Symbol)yyVals[-1+yyTop], Descriptor.Modifier, yyVals[0+yyTop]);
  }
  break;
case 116:
#line 769 "XQuery.y"
  {
     yyVal = Lisp.List(yyVals[0+yyTop], null, null);
  }
  break;
case 117:
#line 773 "XQuery.y"
  {
     yyVal = Lisp.List(null, null, yyVals[0+yyTop]);
  }
  break;
case 118:
#line 777 "XQuery.y"
  {
     yyVal = Lisp.List(yyVals[-1+yyTop], yyVals[0+yyTop], null);
  }
  break;
case 119:
#line 781 "XQuery.y"
  {
     yyVal = Lisp.List(yyVals[-2+yyTop], null, yyVals[0+yyTop]);
  }
  break;
case 120:
#line 785 "XQuery.y"
  {
     yyVal = Lisp.List(yyVals[-3+yyTop], yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 121:
#line 789 "XQuery.y"
  {
     yyVal = Lisp.List(null, yyVals[0+yyTop], null);
  }
  break;
case 122:
#line 793 "XQuery.y"
  {
     yyVal = Lisp.List(null, yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 123:
#line 800 "XQuery.y"
  {
     yyVal = new TokenWrapper(Token.ASCENDING);
  }
  break;
case 124:
#line 804 "XQuery.y"
  {
     yyVal = new TokenWrapper(Token.DESCENDING);
  }
  break;
case 125:
#line 811 "XQuery.y"
  {
     yyVal = new TokenWrapper(Token.EMPTY_GREATEST); 
  }
  break;
case 126:
#line 815 "XQuery.y"
  {
     yyVal = new TokenWrapper(Token.EMPTY_LEAST); 
  }
  break;
case 127:
#line 822 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Some, yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 128:
#line 826 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Every, yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 129:
#line 833 "XQuery.y"
  {
     yyVal = Lisp.Cons(yyVals[0+yyTop]);
  }
  break;
case 130:
#line 837 "XQuery.y"
  {
     yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
  }
  break;
case 131:
#line 844 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.QuantifiedExprOper, yyVals[-3+yyTop], yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 132:
#line 851 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Typeswitch, yyVals[-5+yyTop], yyVals[-3+yyTop], yyVals[0+yyTop]); 
  }
  break;
case 133:
#line 855 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Typeswitch, yyVals[-7+yyTop], yyVals[-5+yyTop], yyVals[-2+yyTop], yyVals[0+yyTop]); 
  }
  break;
case 134:
#line 862 "XQuery.y"
  {
     yyVal = Lisp.Cons(yyVals[0+yyTop]);
  }
  break;
case 135:
#line 866 "XQuery.y"
  {
     yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(yyVals[0+yyTop]));
  }
  break;
case 136:
#line 873 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Case, yyVals[-4+yyTop], yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 137:
#line 877 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Case, yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 138:
#line 884 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.If, yyVals[-5+yyTop], yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 140:
#line 892 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Or, yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 142:
#line 900 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.And, yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 144:
#line 908 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), 
		Descriptor.ValueComp, yyVals[-2+yyTop], yyVals[-1+yyTop], yyVals[0+yyTop]);
  }
  break;
case 145:
#line 913 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), 
		Descriptor.GeneralComp, yyVals[-2+yyTop], yyVals[-1+yyTop], yyVals[0+yyTop]);
  }
  break;
case 146:
#line 918 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), 
		Descriptor.NodeComp, yyVals[-2+yyTop], yyVals[-1+yyTop], yyVals[0+yyTop]);
  }
  break;
case 148:
#line 927 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.Range, yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 150:
#line 936 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.Add, yyVals[-2+yyTop], new TokenWrapper('+'), yyVals[0+yyTop]);
  }
  break;
case 151:
#line 941 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.Add, yyVals[-2+yyTop], new TokenWrapper('-'), yyVals[0+yyTop]);
  }
  break;
case 153:
#line 950 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.Mul, yyVals[-2+yyTop], new TokenWrapper(Token.ML), yyVals[0+yyTop]);
  }
  break;
case 154:
#line 955 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.Mul, yyVals[-2+yyTop], new TokenWrapper(Token.DIV), yyVals[0+yyTop]);
  }
  break;
case 155:
#line 960 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.Mul, yyVals[-2+yyTop], new TokenWrapper(Token.IDIV), yyVals[0+yyTop]);
  }
  break;
case 156:
#line 965 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.Mul, yyVals[-2+yyTop], new TokenWrapper(Token.MOD), yyVals[0+yyTop]);
  }
  break;
case 158:
#line 974 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.Union, yyVals[-2+yyTop], yyVals[0+yyTop]);  
  }
  break;
case 159:
#line 979 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.Concatenate, yyVals[-2+yyTop], yyVals[0+yyTop]);  
  }
  break;
case 161:
#line 988 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.IntersectExcept, yyVals[-2+yyTop], new TokenWrapper(Token.INTERSECT), yyVals[0+yyTop]);  
  }
  break;
case 162:
#line 993 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr),
        Descriptor.IntersectExcept, yyVals[-2+yyTop], new TokenWrapper(Token.EXCEPT), yyVals[0+yyTop]);  
  }
  break;
case 164:
#line 1002 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.InstanceOf, yyVals[-2+yyTop], yyVals[0+yyTop]);    
  }
  break;
case 166:
#line 1010 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.TreatAs, yyVals[-2+yyTop], yyVals[0+yyTop]);    
  }
  break;
case 168:
#line 1018 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.CastableAs, yyVals[-2+yyTop], yyVals[0+yyTop]);    
  }
  break;
case 170:
#line 1026 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.CastAs, yyVals[-2+yyTop], yyVals[0+yyTop]);    
  }
  break;
case 171:
#line 1033 "XQuery.y"
  {
     if (yyVals[-1+yyTop] == null)
       yyVal = yyVals[0+yyTop];
     else
       yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Unary, yyVals[-1+yyTop], yyVals[0+yyTop]);
  }
  break;
case 172:
#line 1043 "XQuery.y"
  {
     yyVal = null;
  }
  break;
case 173:
#line 1047 "XQuery.y"
  {
     yyVal = Lisp.Append(Lisp.Cons(new TokenWrapper('+')), yyVals[0+yyTop]);
  }
  break;
case 174:
#line 1051 "XQuery.y"
  {
     yyVal = Lisp.Append(Lisp.Cons(new TokenWrapper('-')), yyVals[0+yyTop]);
  }
  break;
case 175:
#line 1058 "XQuery.y"
  {
     yyVal = new Literal("=");
  }
  break;
case 176:
#line 1062 "XQuery.y"
  {
     yyVal = new Literal("!=");
  }
  break;
case 177:
#line 1066 "XQuery.y"
  {
     yyVal = new Literal("<");
  }
  break;
case 178:
#line 1070 "XQuery.y"
  {
     yyVal = new Literal("<=");
  }
  break;
case 179:
#line 1074 "XQuery.y"
  {
     yyVal = new Literal(">");
  }
  break;
case 180:
#line 1078 "XQuery.y"
  {
     yyVal = new Literal(">=");
  }
  break;
case 181:
#line 1085 "XQuery.y"
  {
     yyVal = new TokenWrapper(Token.EQ);
  }
  break;
case 182:
#line 1089 "XQuery.y"
  {
     yyVal = new TokenWrapper(Token.NE);
  }
  break;
case 183:
#line 1093 "XQuery.y"
  {
     yyVal = new TokenWrapper(Token.LT);
  }
  break;
case 184:
#line 1097 "XQuery.y"
  {
     yyVal = new TokenWrapper(Token.LE);
  }
  break;
case 185:
#line 1101 "XQuery.y"
  {
     yyVal = new TokenWrapper(Token.GT);
  }
  break;
case 186:
#line 1105 "XQuery.y"
  {
     yyVal = new TokenWrapper(Token.GE);
  }
  break;
case 187:
#line 1112 "XQuery.y"
  {
     yyVal = new TokenWrapper(Token.IS);
  }
  break;
case 188:
#line 1116 "XQuery.y"
  {
     yyVal = new Literal("<<");
  }
  break;
case 189:
#line 1120 "XQuery.y"
  {
     yyVal = new Literal(">>");
  }
  break;
case 193:
#line 1134 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Validate, null, yyVals[-1+yyTop]);
  }
  break;
case 194:
#line 1138 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Validate, yyVals[-3+yyTop], yyVals[-1+yyTop]);
  }
  break;
case 195:
#line 1145 "XQuery.y"
  {
     yyVal = new TokenWrapper(Token.LAX);
  }
  break;
case 196:
#line 1149 "XQuery.y"
  {
     yyVal = new TokenWrapper(Token.STRICT);
  }
  break;
case 197:
#line 1156 "XQuery.y"
  {
     yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.ExtensionExpr, yyVals[-3+yyTop], yyVals[-1+yyTop]);
  }
  break;
case 198:
#line 1163 "XQuery.y"
  {
     yyVal = Lisp.Cons(yyVals[0+yyTop]);
  }
  break;
case 199:
#line 1167 "XQuery.y"
  {
     yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(yyVals[0+yyTop]));
  }
  break;
case 200:
#line 1174 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Pragma, yyVals[-2+yyTop], new Literal(yyVals[-1+yyTop]));
   }
  break;
case 201:
#line 1181 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Child, new object[] { null });
  }
  break;
case 202:
#line 1185 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Child, yyVals[0+yyTop]);
  }
  break;
case 203:
#line 1189 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Descendant, yyVals[0+yyTop]);
  }
  break;
case 206:
#line 1198 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Child, yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 207:
#line 1202 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Descendant, yyVals[-2+yyTop], yyVals[0+yyTop]);
  }
  break;
case 208:
#line 1209 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.AxisStep, yyVals[0+yyTop]);
  }
  break;
case 209:
#line 1213 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.FilterExpr, yyVals[0+yyTop]);
  }
  break;
case 211:
#line 1221 "XQuery.y"
  {
      yyVal = yyVals[-1+yyTop];
      notation.Confirm((Symbol)yyVals[-1+yyTop], Descriptor.PredicateList, yyVals[0+yyTop]);
  }
  break;
case 213:
#line 1227 "XQuery.y"
  {
      yyVal = yyVals[-1+yyTop];
      notation.Confirm((Symbol)yyVals[-1+yyTop], Descriptor.PredicateList, yyVals[0+yyTop]);
  }
  break;
case 214:
#line 1235 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.ForwardStep, yyVals[-1+yyTop], yyVals[0+yyTop]);
   }
  break;
case 216:
#line 1243 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.AXIS_CHILD);
   }
  break;
case 217:
#line 1247 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.AXIS_DESCENDANT);
   }
  break;
case 218:
#line 1251 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.AXIS_ATTRIBUTE);
   }
  break;
case 219:
#line 1255 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.AXIS_SELF);
   }
  break;
case 220:
#line 1259 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.AXIS_DESCENDANT_OR_SELF);
   }
  break;
case 221:
#line 1263 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.AXIS_FOLLOWING_SIBLING);
   }
  break;
case 222:
#line 1267 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.AXIS_FOLLOWING);
   }
  break;
case 223:
#line 1271 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.AXIS_NAMESPACE);
   }
  break;
case 224:
#line 1278 "XQuery.y"
  {  
	  yyVal = notation.Confirm((Symbol)yyVals[0+yyTop], Descriptor.AbbrevForward, yyVals[0+yyTop]); 
   }
  break;
case 226:
#line 1286 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.ReverseStep, yyVals[-1+yyTop], yyVals[0+yyTop]);
   }
  break;
case 228:
#line 1294 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.AXIS_PARENT);
   }
  break;
case 229:
#line 1298 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.AXIS_ANCESTOR);
   }
  break;
case 230:
#line 1302 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.AXIS_PRECEDING_SIBLING);
   }
  break;
case 231:
#line 1306 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.AXIS_PRECEDING);
   }
  break;
case 232:
#line 1310 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.AXIS_ANCESTOR_OR_SELF);
   }
  break;
case 233:
#line 1317 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.DOUBLE_PERIOD);
   }
  break;
case 238:
#line 1334 "XQuery.y"
  {
      yyVal = new TokenWrapper('*');
   }
  break;
case 239:
#line 1338 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Wildcard1, yyVals[-2+yyTop]);
   }
  break;
case 240:
#line 1342 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Wildcard2, yyVals[0+yyTop]);
   }
  break;
case 242:
#line 1350 "XQuery.y"
  {
      yyVal = yyVals[-1+yyTop];
      notation.Confirm((Symbol)yyVals[-1+yyTop], Descriptor.PredicateList, yyVals[0+yyTop]);
   }
  break;
case 243:
#line 1358 "XQuery.y"
  {
      yyVal = Lisp.Cons(yyVals[0+yyTop]);
   }
  break;
case 244:
#line 1362 "XQuery.y"
  {
      yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(yyVals[0+yyTop]));
   }
  break;
case 245:
#line 1369 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Predicate, yyVals[-1+yyTop]);
   }
  break;
case 259:
#line 1398 "XQuery.y"
  {
      yyVal = yyVals[0+yyTop];
   }
  break;
case 260:
#line 1405 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.ParenthesizedExpr, new object[] { null });
   }
  break;
case 261:
#line 1409 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.ParenthesizedExpr, yyVals[-1+yyTop]);
   }
  break;
case 262:
#line 1416 "XQuery.y"
  {
      yyVal = new TokenWrapper('.');
   }
  break;
case 263:
#line 1423 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Ordered, yyVals[-1+yyTop]);
   }
  break;
case 264:
#line 1430 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Unordered, yyVals[-1+yyTop]);
   }
  break;
case 265:
#line 1437 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Funcall, yyVals[-2+yyTop], null);
   }
  break;
case 266:
#line 1441 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Funcall, yyVals[-3+yyTop], yyVals[-1+yyTop]);
   }
  break;
case 267:
#line 1448 "XQuery.y"
  {
      yyVal = Lisp.Cons(yyVals[0+yyTop]);
   }
  break;
case 268:
#line 1452 "XQuery.y"
  {
      yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
   }
  break;
case 274:
#line 1470 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirElemConstructor, yyVals[-3+yyTop], yyVals[-2+yyTop]);
   }
  break;
case 275:
#line 1474 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirElemConstructor, yyVals[-3+yyTop], null);
   }
  break;
case 276:
#line 1478 "XQuery.y"
  {
       if (!yyVals[-7+yyTop].Equals(yyVals[-2+yyTop]))
	     throw new XQueryException(Properties.Resources.XPST0003, 
		   String.Format("The end tag '{0}' does not match the start tag '{1}'", yyVals[-2+yyTop], yyVals[-7+yyTop]));
       yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirElemConstructor, 
		 yyVals[-7+yyTop], yyVals[-6+yyTop], null, yyVals[-2+yyTop], yyVals[-1+yyTop]);
   }
  break;
case 277:
#line 1486 "XQuery.y"
  {
       if (!yyVals[-7+yyTop].Equals(yyVals[-2+yyTop]))
	     throw new XQueryException(Properties.Resources.XPST0003, 
		   String.Format("The end tag '{0}' does not match the start tag '{1}'", yyVals[-2+yyTop], yyVals[-7+yyTop]));
       yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirElemConstructor, 
		 yyVals[-7+yyTop], null, null, yyVals[-2+yyTop], yyVals[-1+yyTop]);
   }
  break;
case 278:
#line 1494 "XQuery.y"
  {
       if (!yyVals[-8+yyTop].Equals(yyVals[-2+yyTop]))
	     throw new XQueryException(Properties.Resources.XPST0003, 
		   String.Format("The end tag '{0}' does not match the start tag '{1}'", yyVals[-2+yyTop], yyVals[-8+yyTop]));
       yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirElemConstructor, 
		 yyVals[-8+yyTop], yyVals[-7+yyTop], yyVals[-5+yyTop], yyVals[-2+yyTop], yyVals[-1+yyTop]);
   }
  break;
case 279:
#line 1502 "XQuery.y"
  {
       if (!yyVals[-8+yyTop].Equals(yyVals[-2+yyTop]))
	     throw new XQueryException(Properties.Resources.XPST0003, 
		   String.Format("The end tag '{0}' does not match the start tag '{1}'", yyVals[-2+yyTop], yyVals[-8+yyTop]));
       yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirElemConstructor, 
		 yyVals[-8+yyTop], null, yyVals[-5+yyTop], yyVals[-2+yyTop], yyVals[-1+yyTop]);
   }
  break;
case 280:
#line 1511 "XQuery.y"
  { 
       yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirElemConstructor, yyVals[-7+yyTop], yyVals[-2+yyTop]);
       notation.Confirm((Symbol)yyVal, Descriptor.MappingExpr, yyVals[-4+yyTop]);
   }
  break;
case 281:
#line 1516 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirElemConstructor, yyVals[-7+yyTop], null);
       notation.Confirm((Symbol)yyVal, Descriptor.MappingExpr, yyVals[-4+yyTop]);
   }
  break;
case 282:
#line 1521 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirElemConstructor, 
		 yyVals[-11+yyTop], yyVals[-6+yyTop], null, yyVals[-3+yyTop], yyVals[-2+yyTop]); 
	   notation.Confirm((Symbol)yyVal, Descriptor.MappingExpr, yyVals[-8+yyTop]);	 
   }
  break;
case 283:
#line 1527 "XQuery.y"
  {
       if (!yyVals[-11+yyTop].Equals(yyVals[-2+yyTop]))
	     throw new XQueryException(Properties.Resources.XPST0003, 
		   String.Format("The end tag '{0}' does not match the start tag '{1}'", yyVals[-2+yyTop], yyVals[-11+yyTop]));
       yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirElemConstructor, 
		 yyVals[-11+yyTop], null, null, yyVals[-3+yyTop], yyVals[-2+yyTop]);
	   notation.Confirm((Symbol)yyVal, Descriptor.MappingExpr, yyVals[-8+yyTop]); 
   }
  break;
case 284:
#line 1536 "XQuery.y"
  {
       if (!yyVals[-12+yyTop].Equals(yyVals[-2+yyTop]))
	     throw new XQueryException(Properties.Resources.XPST0003, 
		   String.Format("The end tag '{0}' does not match the start tag '{1}'", yyVals[-2+yyTop], yyVals[-12+yyTop]));
       yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirElemConstructor, 
		 yyVals[-12+yyTop], yyVals[-7+yyTop], yyVals[-5+yyTop], yyVals[-2+yyTop], yyVals[-1+yyTop]);
	   notation.Confirm((Symbol)yyVal, Descriptor.MappingExpr, yyVals[-9+yyTop]);
   }
  break;
case 285:
#line 1545 "XQuery.y"
  {
       if (!yyVals[-12+yyTop].Equals(yyVals[-2+yyTop]))
	     throw new XQueryException(Properties.Resources.XPST0003, 
		   String.Format("The end tag '{0}' does not match the start tag '{1}'", yyVals[-2+yyTop], yyVals[-12+yyTop]));
       yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirElemConstructor, 
		 yyVals[-12+yyTop], null, yyVals[-5+yyTop], yyVals[-2+yyTop], yyVals[-1+yyTop]);
	   notation.Confirm((Symbol)yyVal, Descriptor.MappingExpr, yyVals[-9+yyTop]);	 
   }
  break;
case 286:
#line 1557 "XQuery.y"
  {
      yyVal = Lisp.Cons(yyVals[0+yyTop]);
   }
  break;
case 287:
#line 1561 "XQuery.y"
  {      
      yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(yyVals[0+yyTop]));
   }
  break;
case 288:
#line 1568 "XQuery.y"
  {
      yyVal = null;
   }
  break;
case 290:
#line 1576 "XQuery.y"
  {
      yyVal = Lisp.List(yyVals[-1+yyTop], yyVals[0+yyTop]);   
   }
  break;
case 291:
#line 1580 "XQuery.y"
  {
      yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(yyVals[0+yyTop]));
   }
  break;
case 292:
#line 1584 "XQuery.y"
  {
      yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.List(yyVals[-1+yyTop], yyVals[0+yyTop]));
   }
  break;
case 293:
#line 1591 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirAttribute,
		 yyVals[-5+yyTop], yyVals[-4+yyTop], yyVals[-2+yyTop], new Literal("\""), Lisp.Cons(new Literal("")));   
   }
  break;
case 294:
#line 1596 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirAttribute,
		 yyVals[-6+yyTop], yyVals[-5+yyTop], yyVals[-3+yyTop], new Literal("\""), yyVals[-1+yyTop]);
   }
  break;
case 295:
#line 1601 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirAttribute,
		 yyVals[-5+yyTop], yyVals[-4+yyTop], yyVals[-2+yyTop], new Literal("\'"), Lisp.Cons(new Literal("")));   
   }
  break;
case 296:
#line 1606 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirAttribute,
		 yyVals[-6+yyTop], yyVals[-5+yyTop], yyVals[-3+yyTop], new Literal("\'"), yyVals[-1+yyTop]);
   }
  break;
case 297:
#line 1614 "XQuery.y"
  {
      yyVal = Lisp.Cons(new TokenWrapper(Token.EscapeQuot));
   }
  break;
case 298:
#line 1618 "XQuery.y"
  {
      yyVal = Lisp.Cons(yyVals[0+yyTop]);
   }
  break;
case 299:
#line 1622 "XQuery.y"
  {
      yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(new TokenWrapper(Token.EscapeQuot)));
   }
  break;
case 300:
#line 1626 "XQuery.y"
  {
      yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(yyVals[0+yyTop]));
   }
  break;
case 301:
#line 1633 "XQuery.y"
  {
      yyVal = Lisp.Cons(new TokenWrapper(Token.EscapeApos));
   }
  break;
case 302:
#line 1637 "XQuery.y"
  {
      yyVal = Lisp.Cons(yyVals[0+yyTop]);
   }
  break;
case 303:
#line 1641 "XQuery.y"
  {
      yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(new TokenWrapper(Token.EscapeApos)));
   }
  break;
case 304:
#line 1645 "XQuery.y"
  {
      yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(yyVals[0+yyTop]));
   }
  break;
case 315:
#line 1671 "XQuery.y"
  {
      yyVal = new Literal("{");
   }
  break;
case 316:
#line 1675 "XQuery.y"
  {
      yyVal = new Literal("}");
   }
  break;
case 317:
#line 1679 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CommonContent), Descriptor.EnclosedExpr, yyVals[0+yyTop]); 
   }
  break;
case 318:
#line 1686 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirCommentConstructor, yyVals[-1+yyTop]);
   }
  break;
case 319:
#line 1693 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirPIConstructor, yyVals[-1+yyTop], null);
   }
  break;
case 320:
#line 1697 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirPIConstructor, yyVals[-3+yyTop], yyVals[-1+yyTop]);
   }
  break;
case 321:
#line 1704 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CData), Descriptor.CDataSection, yyVals[-1+yyTop]);
   }
  break;
case 328:
#line 1720 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompDocConstructor, yyVals[-1+yyTop]);
   }
  break;
case 329:
#line 1728 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompElemConstructor, yyVals[-3+yyTop], yyVals[-1+yyTop]);   
   }
  break;
case 330:
#line 1733 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompElemConstructor, yyVals[-2+yyTop], null);   
   }
  break;
case 331:
#line 1738 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompElemConstructor, yyVals[-4+yyTop], yyVals[-1+yyTop]);   
   }
  break;
case 332:
#line 1743 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompElemConstructor, yyVals[-3+yyTop], null);   
   }
  break;
case 334:
#line 1755 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompAttrConstructor, yyVals[-3+yyTop], yyVals[-1+yyTop]);   
   }
  break;
case 335:
#line 1760 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompAttrConstructor, yyVals[-2+yyTop], null);   
   }
  break;
case 336:
#line 1765 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompAttrConstructor, yyVals[-4+yyTop], yyVals[-1+yyTop]);   
   }
  break;
case 337:
#line 1770 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompAttrConstructor, yyVals[-3+yyTop], null);   
   }
  break;
case 338:
#line 1778 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompTextConstructor, yyVals[-1+yyTop]);   
   }
  break;
case 339:
#line 1786 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompCommentConstructor, yyVals[-1+yyTop]);   
   }
  break;
case 340:
#line 1794 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompPIConstructor, yyVals[-3+yyTop], yyVals[-1+yyTop]);   
   }
  break;
case 341:
#line 1799 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompPIConstructor, yyVals[-2+yyTop], null);   
   }
  break;
case 342:
#line 1804 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompPIConstructor, yyVals[-4+yyTop], yyVals[-1+yyTop]);   
   }
  break;
case 343:
#line 1809 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompPIConstructor, yyVals[-3+yyTop], null);   
   }
  break;
case 345:
#line 1818 "XQuery.y"
  {
      yyVal = yyVals[-1+yyTop];
      notation.Confirm((Symbol)yyVals[-1+yyTop], Descriptor.Occurrence, 
		new TokenWrapper(Token.Indicator3));
   }
  break;
case 346:
#line 1827 "XQuery.y"
  {
      yyVal = yyVals[0+yyTop];
   }
  break;
case 348:
#line 1835 "XQuery.y"
  {
      yyVal = yyVals[-1+yyTop];
      notation.Confirm((Symbol)yyVals[-1+yyTop], Descriptor.Occurrence, yyVals[0+yyTop]);
   }
  break;
case 349:
#line 1840 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.EMPTY_SEQUENCE);
   }
  break;
case 350:
#line 1847 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.Indicator1);
   }
  break;
case 351:
#line 1851 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.Indicator2);
   }
  break;
case 352:
#line 1855 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.Indicator3);
   }
  break;
case 355:
#line 1864 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.ITEM);
   }
  break;
case 357:
#line 1875 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.KindTest, yyVals[0+yyTop]);
   }
  break;
case 367:
#line 1893 "XQuery.y"
  {
       yyVal = new TokenWrapper(Token.NODE);
   }
  break;
case 368:
#line 1900 "XQuery.y"
  {
       yyVal = new TokenWrapper(Token.DOCUMENT_NODE);
   }
  break;
case 369:
#line 1904 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.DocumentNode, yyVals[-1+yyTop]);
   }
  break;
case 370:
#line 1908 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.DocumentNode, yyVals[-1+yyTop]);
   }
  break;
case 371:
#line 1915 "XQuery.y"
  {
       yyVal = new TokenWrapper(Token.TEXT);
   }
  break;
case 372:
#line 1922 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.COMMENT);
   }
  break;
case 373:
#line 1930 "XQuery.y"
  {
       yyVal = new TokenWrapper(Token.PROCESSING_INSTRUCTION);
   }
  break;
case 374:
#line 1934 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.ProcessingInstruction, yyVals[-1+yyTop]);
   }
  break;
case 375:
#line 1938 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.ProcessingInstruction, yyVals[-1+yyTop]);
   }
  break;
case 376:
#line 1945 "XQuery.y"
  {
       yyVal = new TokenWrapper(Token.ELEMENT);
   }
  break;
case 377:
#line 1949 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Element, yyVals[-1+yyTop]);
   }
  break;
case 378:
#line 1953 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Element, yyVals[-3+yyTop], yyVals[-1+yyTop]);
   }
  break;
case 379:
#line 1957 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Element, 
		yyVals[-4+yyTop], yyVals[-2+yyTop], new TokenWrapper('?'));
   }
  break;
case 381:
#line 1966 "XQuery.y"
  {
      yyVal = new TokenWrapper('*');
   }
  break;
case 382:
#line 1973 "XQuery.y"
  {
       yyVal = new TokenWrapper(Token.ATTRIBUTE);
   }
  break;
case 383:
#line 1977 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Attribute, yyVals[-1+yyTop]);
   }
  break;
case 384:
#line 1981 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Attribute, yyVals[-3+yyTop], yyVals[-1+yyTop]);
   }
  break;
case 386:
#line 1989 "XQuery.y"
  {
      yyVal = new TokenWrapper('*');
   }
  break;
case 387:
#line 1996 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.SchemaElement, yyVals[-1+yyTop]);
   }
  break;
case 388:
#line 2003 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.SchemaAttribute, yyVals[-1+yyTop]);
   }
  break;
case 392:
#line 2022 "XQuery.y"
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
   45,   45,   50,   50,   50,   50,   53,   53,   55,   55,
   56,   57,   57,   58,   54,   59,   59,   60,   51,   52,
   52,   61,   61,   62,   62,   63,   63,   63,   63,   63,
   63,   63,   64,   64,   65,   65,   46,   46,   66,   66,
   67,   47,   47,   68,   68,   69,   69,   48,   49,   49,
   70,   70,   71,   71,   71,   71,   72,   72,   76,   76,
   76,   77,   77,   77,   77,   77,   78,   78,   78,   79,
   79,   79,   80,   80,   81,   81,   82,   82,   83,   83,
   85,   86,   86,   86,   74,   74,   74,   74,   74,   74,
   73,   73,   73,   73,   73,   73,   75,   75,   75,   87,
   87,   87,   88,   88,   91,   91,   90,   92,   92,   93,
   89,   89,   89,   89,   95,   95,   95,   96,   96,   97,
   97,   97,   97,   99,   99,  102,  102,  102,  102,  102,
  102,  102,  102,  104,  104,  101,  101,  105,  105,  105,
  105,  105,  106,  103,  103,  108,  108,  109,  109,  109,
   98,   98,  100,  100,  111,  110,  110,  110,  110,  110,
  110,  110,  110,  112,  112,  120,  120,  120,  113,  114,
  114,  115,  118,  119,  116,  116,  121,  121,  117,  117,
  122,  122,  122,  124,  124,  124,  124,  124,  124,  124,
  124,  124,  124,  124,  124,  128,  128,  127,  127,  130,
  130,  130,  131,  131,  131,  131,  132,  132,  132,  132,
  133,  133,  133,  133,  134,  134,  135,  135,  129,  129,
  129,  129,  137,  137,  137,  137,  137,  125,  126,  126,
  140,  123,  123,  123,  123,  123,  123,  141,  142,  142,
  142,  142,  147,  143,  143,  143,  143,  144,  145,  146,
  146,  146,  146,   84,   84,   37,   40,   40,   40,  150,
  150,  150,  149,  149,  149,  148,  107,  151,  151,  151,
  151,  151,  151,  151,  151,  151,  160,  152,  152,  152,
  159,  158,  157,  157,  157,  153,  153,  153,  153,  161,
  161,  154,  154,  154,  164,  164,  155,  156,  165,  163,
  162,   94,   94,  136,  138,  139,    8,
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
    4,    5,    1,    1,    2,    2,    2,    2,    1,    3,
    6,    0,    1,    3,    2,    1,    3,    6,    2,    2,
    2,    1,    3,    1,    2,    1,    2,    2,    3,    4,
    1,    3,    1,    1,    1,    1,    4,    4,    1,    3,
    5,    8,   10,    1,    2,    7,    4,    8,    1,    3,
    1,    3,    1,    3,    3,    3,    1,    3,    1,    3,
    3,    1,    3,    3,    3,    3,    1,    3,    3,    1,
    3,    3,    1,    3,    1,    3,    1,    3,    1,    3,
    2,    0,    2,    2,    1,    2,    1,    2,    1,    2,
    1,    1,    1,    1,    1,    1,    1,    2,    2,    1,
    1,    1,    4,    5,    1,    1,    4,    1,    2,    5,
    1,    2,    2,    1,    1,    3,    3,    1,    1,    1,
    2,    1,    2,    2,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    2,    1,    2,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    3,    3,
    1,    2,    1,    2,    3,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    2,    2,
    3,    1,    4,    4,    3,    4,    1,    3,    1,    1,
    1,    1,    1,    5,    5,    9,    9,   10,   10,    9,
    9,   13,   13,   14,   14,    1,    2,    0,    1,    2,
    2,    3,    6,    7,    6,    7,    1,    1,    2,    2,
    1,    1,    2,    2,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    2,    2,    1,    3,    3,    5,
    3,    1,    1,    1,    1,    1,    1,    4,    5,    4,
    7,    6,    1,    5,    4,    7,    6,    4,    4,    5,
    4,    7,    6,    1,    2,    2,    1,    2,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    3,    3,    4,    4,
    3,    3,    3,    4,    4,    3,    4,    6,    7,    1,
    1,    3,    4,    6,    1,    1,    4,    4,    1,    1,
    1,    0,    1,    1,    1,    1,    1,
  };
   static  short [] yyDefRed = {            0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    2,
    4,    0,    0,    0,    0,    0,    0,   18,   19,   20,
   21,   22,   23,   24,   25,   26,   27,   28,   29,   30,
   31,   32,   33,    0,    0,    0,    0,    0,   55,    0,
  397,    0,    0,   36,   37,    0,    0,   43,   44,    0,
   41,   42,   46,   47,    0,   50,   51,    0,   68,   69,
    0,    1,    3,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    7,   82,    0,   84,   85,   86,   87,    0,
    0,   93,   94,    0,  141,    0,    0,    0,    0,    0,
  160,    0,    0,    0,    0,    0,    8,    0,    0,    0,
   34,   14,   16,    0,    5,    0,    0,   59,    0,    0,
    0,   60,    0,   38,   39,   40,    0,    0,    0,    0,
    0,   99,    0,    0,    0,  106,    0,    0,  129,    0,
    0,    0,  173,  174,    0,    0,    0,    0,    0,    0,
    0,    0,   95,   96,    0,  181,  182,  183,  185,  186,
  184,  187,  175,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  233,  255,  256,  257,  258,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  216,  217,
  218,  219,  220,  221,  222,  228,  229,  230,  231,  232,
  223,    0,    0,    0,    0,    0,    0,  262,  171,  190,
  191,  192,    0,  198,    0,  205,  208,  209,    0,    0,
    0,  225,  215,    0,  227,  234,  235,  237,    0,  246,
  247,  248,  249,  250,  251,  252,  253,  254,  269,  270,
  271,  272,  273,  322,  323,  324,  325,  326,  327,  357,
  358,  359,  360,  361,  362,  363,  364,  365,  366,   15,
   17,    0,    0,   58,    0,    0,   56,    0,   35,   48,
   49,   45,    0,    0,   67,    0,    0,    0,   76,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   83,
    0,   89,  109,    0,    0,  112,    0,    0,    0,    0,
  142,  176,  178,  188,  180,  189,  144,  145,  146,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  161,  162,
    0,    0,    0,    0,  356,    0,  349,  355,  164,  354,
  353,    0,  166,  168,    0,  170,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  195,
  196,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  393,    0,    0,  259,  260,
    0,    0,  236,  224,    0,    0,  199,    0,    0,    0,
    0,  243,    0,  214,  226,    0,    6,    9,    0,    0,
    0,  346,   65,    0,    0,    0,    0,    0,  100,    0,
  107,    0,  127,  130,  128,    0,    0,  123,  124,    0,
  125,  126,  115,    0,    0,    0,   90,    0,   91,  350,
  351,  352,  348,  345,    0,    0,    0,    0,  390,  376,
  381,    0,  380,    0,    0,  389,  382,  386,    0,  385,
    0,  371,    0,  372,    0,    0,    0,  367,  239,  265,
  267,    0,    0,    0,    0,  373,    0,  368,    0,    0,
    0,    0,  318,    0,  319,    0,    0,    0,    0,    0,
  261,  240,    0,  207,  206,    0,  244,   61,   57,    0,
   79,   73,    0,    0,   70,   72,   77,    0,    0,  103,
    0,    0,    0,    0,  134,    0,  117,    0,    0,    0,
  113,   92,  263,  264,  328,  330,    0,    0,    0,  377,
    0,  335,    0,    0,  383,    0,  338,  339,  193,    0,
    0,  266,  341,    0,  375,  374,    0,  369,  370,  387,
  388,    0,    0,    0,    0,    0,  290,    0,    0,    0,
    0,  197,  245,   64,    0,    0,    0,    0,    0,  131,
    0,    0,    0,  135,    0,  119,    0,  122,  329,  391,
    0,    0,  334,    0,    0,  194,  268,  340,    0,  320,
  200,    0,  396,  313,  314,    0,    0,    0,    0,  317,
  309,    0,  286,  312,  310,  311,  275,    0,    0,    0,
  274,  292,   71,   80,  104,  101,  108,    0,    0,    0,
    0,    0,  120,  378,    0,  332,    0,  384,  337,    0,
  343,    0,    0,    0,  315,  316,    0,    0,  287,    0,
    0,    0,    0,  137,  132,    0,  138,  379,  331,  336,
  342,    0,  321,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  133,  395,  295,  301,    0,  302,  308,  307,
  394,  297,  293,    0,  298,  305,  306,  277,    0,    0,
    0,  281,    0,    0,  280,  276,    0,  136,  296,  303,
  304,  299,  294,  300,  279,    0,    0,    0,    0,  278,
    0,    0,    0,    0,    0,    0,    0,    0,  283,    0,
  282,    0,  285,  284,
  };
  protected static  short [] yyDgoto  = {            18,
   19,   20,   21,  112,   22,   83,   23,  287,   24,   25,
   26,   27,   28,   29,   30,   31,   32,   33,   34,   35,
   36,   37,   38,   39,   40,   41,   42,   43,   65,  292,
   48,  288,   49,  122,  294,   84,  295,  297,  505,  349,
  600,  298,  299,  527,   86,   87,   88,   89,   90,   91,
  151,  152,   92,   93,  131,  132,  509,  510,  135,  136,
  315,  316,  433,  434,  435,  138,  139,  514,  515,   94,
   95,   96,  167,  168,  169,   97,   98,   99,  100,  101,
  102,  103,  104,  354,  105,  106,  229,  230,  231,  232,
  373,  233,  234,  387,  235,  236,  237,  238,  239,  401,
  240,  241,  242,  243,  244,  245,  246,  247,  248,  249,
  402,  250,  251,  252,  253,  254,  255,  256,  257,  258,
  472,  601,  260,  261,  262,  263,  489,  602,  603,  490,
  557,  684,  677,  685,  678,  686,  604,  680,  605,  606,
  264,  265,  266,  267,  268,  269,  528,  351,  352,  443,
  270,  271,  272,  273,  274,  275,  276,  277,  278,  279,
  452,  581,  453,  459,  460,
  };
  protected static  short [] yySindex = {          986,
 -264, -228, -217, -193, -222, -179, -158, -154,   41, -199,
   57,   10, -166, -166,  146, -113, -131,    0,  821,    0,
    0,  548,  -95,  -95, -122,  141,  141,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  -32,  143, -103,   25, -166,    0,  -70,
    0,   -5,  192,    0,    0, -166, -166,    0,    0,  -18,
    0,    0,    0,    0,  238,    0,    0,   11,    0,    0,
  261,    0,    0,  310,  310,  312,  347,  347,  346,  353,
  133,  133,    0,    0,  365,    0,    0,    0,    0,  123,
  -78,    0,    0,  139,    0,  337,   26, -230,  -88,  108,
    0,  142,  144,  155,  145, 4887,    0, -122,  141,  141,
    0,    0,    0,  147,    0, -166,  404,    0,  199,  407,
 -166,    0, -166,    0,    0,    0,   77,  201,  434,  149,
  430,    0,  430,  152,  432,    0,  154,   -2,    0,    7,
  548,  548,    0,    0,  548,  133,  548,  548,  548,  548,
 -169,  210,    0,    0,  133,    0,    0,    0,    0,    0,
    0,    0,    0,  422,  341,  350,  133,  133,  133,  133,
  133,  133,  133,  133,  133,  133,  133,  133,  133,  133,
  893,  893,  166,  166,  361,  363,  366,   -7,   -6,   17,
   21,  -94,  450,    0,    0,    0,    0,    0,  436,  452,
  -19,  457,  460,  461, 5254,  185,  186,  179,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  187,  188,  747, 5254,  587,  456,    0,    0,    0,
    0,    0, -105,    0,  -28,    0,    0,    0,  417,  417,
  587,    0,    0,  587,    0,    0,    0,    0,  417,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  141,  141,    0, -166, -166,    0,  472,    0,    0,
    0,    0,  893,  -35,    0,  203,  486,  488,    0,  201,
  310,  201,  312,  201,  548,  347,  548,   51,  102,    0,
  139,    0,    0, -149,  490,    0,  490,  548,  260,  548,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  184,
 -230, -230,  -88,  -88,  -88,  -88,  108,  108,    0,    0,
  497,  501,  502,  503,    0,  504,    0,    0,    0,    0,
    0, -170,    0,    0,  482,    0,  548,  548,  548,  424,
  -16,  548,  425,  -10,  548,  518,  548,  520,  548,    0,
    0,  548,  439,  523,  524, 1176,  442,  -21,  548,  -25,
  246,  247,  -28,  216, -229,    0,  252,  254,    0,    0,
  220,  -28,    0,    0,  263,  548,    0, 5254, 5254,  548,
  417,    0,  417,    0,    0,  417,    0,    0,  472,   -5,
 -166,    0,    0,  521,  201,  -93,  434,  314,    0,  537,
    0,  327,    0,    0,    0,  315,  313,    0,    0, -166,
    0,    0,    0, -159,  322,  548,    0,  548,    0,    0,
    0,    0,    0,    0,    4,   28,   29,  299,    0,    0,
    0,  285,    0,   30, 1294,    0,    0,    0,  317,    0,
   31,    0,   32,    0,   33,   36,  548,    0,    0,    0,
    0,  328, 1318,  575,  576,    0,   38,    0,  577,  578,
  582,  583,    0,  308,    0,  302,   -8,  540,  112,  307,
    0,    0,   40,    0,    0,   39,    0,    0,    0,  548,
    0,    0,  893,  548,    0,    0,    0,  598,  372,    0,
  574,  548,    2,  132,    0,  548,    0, -166,  373, -166,
    0,    0,    0,    0,    0,    0,  365,  532,  336,    0,
  526,    0,   42,  336,    0,  536,    0,    0,    0,   43,
  548,    0,    0,   44,    0,    0,  539,    0,    0,    0,
    0,  301,  311,  179,   78,  601,    0, 5076,   92,  620,
  359,    0,    0,    0,  -83,   45,  364,  548,  548,    0,
  367,  418,  -14,    0,  405,    0, -166,    0,    0,    0,
   62, 1455,    0,  652, 1573,    0,    0,    0, 1597,    0,
    0,  636,    0,    0,    0,  389, 1852,  589,  661,    0,
    0,  129,    0,    0,    0,    0,    0,  606,  682,  435,
    0,    0,    0,    0,    0,    0,    0,  465,  548,  548,
  411,  548,    0,    0,  687,    0,  610,    0,    0,   46,
    0,   47,  179,  374,    0,    0,  416,  692,    0,  415,
  421,  698,  893,    0,    0,  476,    0,    0,    0,    0,
    0,  -29,    0,  179,  426,   -4,  160,  179,  429,  477,
  548,  -61,  -23,  693,  179,  676,  694,  681,  697,  705,
  179,  548,    0,    0,    0,    0,   37,    0,    0,    0,
    0,    0,    0,  -17,    0,    0,    0,    0,  706,  707,
  758,    0,  713,  933,    0,    0,  708,    0,    0,    0,
    0,    0,    0,    0,    0,  447,  726,  463,  731,    0,
  179,  464,  179,  467,  717,  179,  725,  179,    0,  732,
    0,  734,    0,    0,
  };
  protected static  short [] yyRindex = {         4694,
    0,    0,  481,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0, 4694,    0,
    0, 4961,  800,  234,  349,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  743,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
 4961, 4961,    0,    0,  803,    0,    0,    0,    0,  403,
    0,    0,    0, 4522,    0, 4434, 4349, 4091, 3441, 3074,
    0, 3027, 2883, 2748, 2469,    0,    0,  493,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  748,    0,
    0,    0,    0,    0,    0,    0,    0,  -34,  776,    0,
  382,    0,  619,    0,  633,    0,    0,    0,    0,    0,
 4961, 4961,    0,    0, 4961, 4961, 4961, 4961, 4961, 4961,
    0,    0,    0,    0, 4961,    0,    0,    0,    0,    0,
    0,    0,    0,    0, 3634, 4813, 4961, 4961, 4961, 4961,
 4961, 4961, 4961, 4961, 4961, 4961, 4961, 4961, 4961, 4961,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  651,
    0,    0,    0,    0,    0,    0,    0,  500,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0, 4961, 1911,    0,  795,    0,    0,    0,
    0,    0,    0,    0, 2046,    0,    0,    0,  930, 1074,
    0,    0,    0,    0,    0,    0,    0,    0, 1209,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  764,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  783,    0,   63,
    0,  767,    0,  561, 4961,    0, 4961,    0,    0,    0,
 4568,    0,    0,    5,  563,    0,  564, 4961,    0, 4961,
    0,    0,    0,    0,    0,    0,    0,    0,    0, 4480,
 4179, 4225, 3487, 3766, 3812, 3900, 3162, 3353,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0, 1353,    0,    0, 2604,    0, 4961, 4961, 4961,    0,
    0, 4961,    0,    0, 4961,    0, 4961,    0, 4961,    0,
    0, 4961,    0,    0,    0, 4961,    0,    0, 4961,    0,
    0,    0, 2190,    0,    0,    0,    0,   50,    0,    0,
    0, 2325,    0,    0,    0, 4961,    0,    0,    0, 4961,
 1488,    0, 1632,    0,    0, 1767,    0,    0,  772,  743,
    0,    0,    0,    0,  335,    0,    0,  566,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   16,   19, 4961,    0, 4961,    0,    0,
    0,    0,    0,    0,    0,    0,    0, 4961,    0,    0,
    0,    0,    0,    0, 4961,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0, 4961,    0,    0,    0,
    0,    0, 4961,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  746,    0,    0,  162,
    0,    0,    0,    0,    0,    0,    0,    0,    0, 4961,
    0,    0,    0, 4961,    0,    0,    0,    0,    0,    0,
    0, 4961,    0,    0,    0, 4961,    0,    0,   24,    0,
    0,    0,    0,    0,    0,    0,  716,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
 4961,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  782,    0,    0,    0,    0,    0,    0,
  -12,    0,    0,    0,    0,    0,    0, 4961, 4961,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0, 4961,    0,    0, 4961,    0,    0,    0, 4961,    0,
    0,    0,    0,    0,    0,    0, 4961,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0, 4961, 4961,
    0, 4961,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  -20,    0,    0,    0,    0,    0,    0,  164,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  788,    0,    0,    0,  788,    0,    0,
 4961,    0,    0,    0,  788,    0,    0,    0,    0,    0,
  788, 4961,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  788,    0,  788,    0,    0,  788,    0,  788,    0,    0,
    0,    0,    0,    0,
  };
  protected static  short [] yyGindex = {            0,
    0,  833,  839,    1,  849,    0,    0,   -1,    0,  855,
  856,   34,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  588,    0,  474, -165, -141,  470,    0,  329, -172,
 -379,    0,  478,  -22,    0,    0,    0,    0,    0,    0,
    0,  736,  791,  805,  822,  597,    0,    0,    0,  596,
  750,  475,    0,    0,  479,  823,  604,    0,  398,  768,
  760,  213,    0,    0,    0,  759,  250,   84,  248,  259,
    0,    0,    0,  733,    0,  360,    0,    0,  358,    0,
    0,    0,  685, -281, -127,   54,    0,    0,    0, -155,
    0,    0,  -36,    0,    0,    0, -180,    0,    0,    0,
 -173,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  -91,    0,    0,    0,    0,  292, -518, -558,    0,
  375,    0,    0,  251,  265,    0, -478,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  383,  277,    0,    0,
    0,    0,  584,    0,  586,    0,    0,    0,    0,    0,
    0,  433,  591,    0,  594,
  };
  protected static  short [] yyTable = {            85,
  350,  350,   52,  310,  663,  312,  313,  314,  314,  353,
  683,   66,   67,  392,  259,  478,  703,  396,  399,  476,
  378,  621,  414,   66,  450,  451,  111,  113,  372,  504,
  457,  458,  361,  364,  291,  178,  506,  571,  556,  504,
  610,  306,  667,  639,  115,   46,  119,  145,  114,  291,
  306,  639,   44,  555,  124,  125,  366,  666,  110,  116,
  368,  597,  121,  598,  173,  174,  175,  118,  171,   50,
  172,  145,  145,  145,  145,  145,  145,  383,   54,  145,
   55,  145,  145,  145,  403,  145,  145,  145,  145,  145,
  145,  426,   45,  406,  145,  484,  288,  392,   53,  597,
  318,  598,  624,  379,   56,  597,  488,  598,   57,  280,
  281,  288,  350,  259,  283,  362,  365,  518,  308,  309,
  412,  289,   60,   51,  625,  428,  429,  430,  523,  485,
   47,  563,  639,  259,  418,  639,  420,  599,  422,  367,
  392,  110,  427,  369,   69,  145,   70,  691,  176,  694,
   51,  609,  524,  525,  531,  536,  537,  538,  560,  597,
  539,  598,  547,  423,  562,  425,  583,  586,  588,  614,
  650,  651,  502,  559,  503,   81,  437,   82,  439,  149,
  150,   68,  502,  679,  687,  506,  431,  432,  638,  394,
   71,  147,   74,   75,   76,  148,  431,  432,  679,  111,
  597,  391,  598,  116,  404,  687,  669,  405,  289,  177,
  288,  440,  441,  442,  597,   10,  598,  117,  370,  371,
   15,  668,   17,  289,  114,  288,  171,  497,  172,  497,
  413,   66,  497,   11,  471,    3,    4,    5,    6,    7,
    8,    9,   10,   11,   12,   13,   14,   15,   16,   17,
  120,  597,  123,  598,  208,  620,  333,  334,  335,  336,
  491,  121,  341,  145,  674,  594,  595,   63,   64,   11,
  149,  150,  592,   11,  114,   11,   11,  305,   11,   11,
   11,  127,  407,  408,  410,  116,  307,  118,  121,  341,
  342,  343,  344,  118,  314,  474,  522,   11,  126,  475,
  129,  377,  681,  594,  595,  449,  259,  259,  681,  594,
  595,  456,  291,  554,  360,  363,  193,  554,  675,  170,
   61,   62,  350,  345,  676,  530,  398,  203,  529,   66,
  565,   66,  350,  128,  445,  446,  447,  290,  291,  454,
  572,   81,  461,   82,  463,  130,  465,  134,   12,  466,
  662,  652,  346,  202,  203,  204,  477,  535,  564,  392,
  534,  682,  674,  594,  595,  347,  348,  702,  542,  164,
  570,  541,  664,  493,  575,   78,  670,  496,   78,  327,
  328,  329,  137,  689,   12,  141,   58,   59,   12,  697,
   12,   12,  142,   12,   12,   12,  165,  163,  166,  587,
  324,  323,   88,  593,  594,  595,  179,  180,  145,  499,
  325,  326,   12,  513,  573,  146,  699,  593,  594,  595,
  331,  332,  700,  526,  337,  338,  616,  617,  517,  715,
  155,  717,  533,  206,  720,  207,  722,  339,  340,  596,
  143,  144,  181,   88,  540,  182,   88,  206,  184,  207,
  544,  494,  495,  596,  593,  594,  595,  183,  222,  355,
  355,   88,  350,  282,  284,  285,  259,  286,  293,  296,
  660,  300,  222,  301,  302,  303,  304,  644,  645,  320,
  647,  566,  322,  357,  206,  358,  207,  345,  359,  374,
  596,  376,   13,  375,  642,   88,  380,   11,   11,  381,
  382,  384,  385,  386,   11,   11,   11,  400,  388,  222,
  389,   11,   11,  395,   11,  411,  576,   11,  578,  673,
   11,   11,   11,   11,   11,  415,  416,   88,   13,  438,
  698,  417,   13,  436,   13,   13,  361,   13,   13,   13,
  364,  366,  368,  378,  444,   11,  448,  455,   11,   11,
   11,   11,   11,   11,   11,   11,   13,  597,  462,  598,
  464,  467,  630,  468,  473,  469,  632,  449,  456,   74,
   75,   76,  483,  486,  566,  623,   77,   78,  487,   79,
  508,  500,   80,  492,   11,   11,   11,   11,   11,   11,
   81,   11,   82,   11,  511,  512,  513,  516,  520,   11,
   11,   11,   11,   11,   11,   11,   11,   11,   11,   11,
   11,   11,   12,   12,   11,  545,  546,  548,  549,   12,
   12,   12,  550,  551,  552,  553,   12,   12,  227,   12,
  558,  561,   12,  567,  569,   12,   12,   12,   12,   12,
  568,  156,  157,  158,  159,  160,  161,  162,  582,  577,
  236,   97,   97,   97,   97,   97,  579,  580,  585,  590,
   12,  589,  607,   12,   12,   12,   12,   12,   12,   12,
   12,  591,   88,   88,   88,   88,   88,   88,   88,   88,
  554,  611,   88,  236,   88,   88,  615,  619,   88,  618,
  622,  236,  628,  236,  236,  236,  633,  236,  640,   12,
   12,   12,   12,   12,   12,  634,   12,  637,   12,  236,
  236,  236,  236,  636,   12,   12,   12,   12,   12,   12,
   12,   12,   12,   12,   12,   12,   12,  648,  641,   12,
   97,   97,  643,  646,  649,  690,  653,  654,  655,  656,
  693,  236,  658,  236,  659,  661,  672,  665,   88,   88,
  671,   88,   88,  706,  688,  692,   13,   13,  695,  708,
  593,  594,  595,   13,   13,   13,  696,  705,  711,  710,
   13,   13,  712,   13,  236,  236,   13,  714,  719,   13,
   13,   13,   13,   13,  713,  716,  721,  390,  718,   81,
  206,   82,  207,  723,  238,  724,  596,   54,  597,   10,
  598,   62,   81,  597,   13,  598,   52,   13,   13,   13,
   13,   13,   13,   13,   13,  222,   74,  707,   74,   75,
   76,  392,   63,   75,   66,   77,   78,  238,   79,   66,
   53,   80,  110,  111,  102,  238,  393,  238,  238,  238,
  333,  238,  392,   13,   13,   13,   13,   13,   13,  392,
   13,   72,   13,  238,  238,  238,  238,   73,   13,   13,
   13,   13,   13,   13,   13,   13,   13,   13,   13,   13,
   13,  107,  409,   13,  341,  342,  343,  344,  108,  109,
  597,  153,  598,  498,  501,  238,  319,  238,   98,   98,
   98,   98,   98,  613,  507,  154,  133,  419,  421,  317,
  140,  193,  105,  105,  105,  105,  105,  199,  393,  424,
  521,  574,  519,  311,  321,  608,  356,  397,  238,  238,
  236,  236,  236,  236,  236,  236,  236,  236,  330,  210,
  236,  657,  236,  236,  704,  612,  236,  346,  202,  203,
  204,  701,  236,  236,  236,  236,  236,  236,  236,  236,
  236,  236,  236,  236,  236,  236,  236,  236,  236,  236,
  236,  236,  210,  479,  627,  480,  584,   98,   98,    0,
  210,  481,  210,  210,  210,  482,  210,    0,    0,    0,
    0,  105,  105,    0,    0,    0,    0,    0,  210,  210,
  210,  210,  709,    0,    0,    0,  236,  236,    0,  236,
  236,  593,  594,  595,    0,  236,  593,  594,  595,    0,
    0,    0,    0,    0,    0,    0,    0,   74,   75,   76,
    0,    0,  210,    0,   77,   78,    0,   79,    0,  236,
   80,  206,    0,  207,    0,    0,  206,  596,  207,    0,
    0,    0,  596,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  210,  210,  597,  222,  598,    0,    0,
    0,  222,    0,    0,  238,  238,  238,  238,  238,  238,
  238,  238,    0,  212,  238,    0,  238,  238,    0,    0,
  238,    0,    0,  593,  594,  595,  238,  238,  238,  238,
  238,  238,  238,  238,  238,  238,  238,  238,  238,  238,
  238,  238,  238,  238,  238,  238,  212,    0,    0,    0,
    0,    0,    0,  206,  212,  207,  212,  212,  212,  596,
  212,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  212,  212,  212,  212,    0,    0,  222,    0,
  238,  238,    0,  238,  238,    0,    0,    0,    0,  238,
    2,    3,    4,    5,    6,    7,    8,    9,   10,   11,
   12,   13,   14,   15,   16,   17,  212,    0,    0,    0,
    0,    0,    0,  238,    0,    0,    0,    0,    0,    0,
  341,  342,  343,  344,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  212,  212,  210,
  210,  210,  210,  210,  210,  210,  210,  193,  241,  210,
    0,  210,  210,    0,  345,  210,  470,    0,   81,    0,
   82,  210,  210,  210,  210,  210,  210,  210,  210,  210,
  210,  210,  210,  210,  210,  210,  210,  210,  210,  210,
  210,  241,    0,  346,  202,  203,  204,    0,    0,  241,
    0,  241,  241,  241,    0,  241,  347,  348,  593,  594,
  595,    0,    0,    0,    0,    0,    0,  241,  241,  241,
  241,    0,    0,    0,    0,  210,  210,    0,  210,  210,
    0,    0,    0,    0,  210,    0,    0,    0,  206,    0,
  207,    0,    0,    0,  596,    0,    0,    0,    0,    0,
    0,  241,    0,    0,    0,    0,    0,    0,  210,    0,
    0,    0,    0,  222,    1,    2,    3,    4,    5,    6,
    7,    8,    9,   10,   11,   12,   13,   14,   15,   16,
   17,    0,  241,  241,    0,    0,   81,    0,   82,    0,
    0,    0,    0,  212,  212,  212,  212,  212,  212,  212,
  212,    0,  347,  212,    0,  212,  212,    0,    0,  212,
   81,    0,   82,    0,    0,  212,  212,  212,  212,  212,
  212,  212,  212,  212,  212,  212,  212,  212,  212,  212,
  212,  212,  212,  212,  212,  347,    0,    0,    0,    0,
    0,    0,    0,  347,    0,  347,  347,  347,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  347,  347,  347,  347,  347,    0,    0,    0,  532,  212,
  212,    0,  212,  212,    0,    0,    0,    0,  212,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  543,    0,    0,  347,   74,   75,   76,    0,
    0,    0,  212,   77,   78,    0,   79,    0,    0,   80,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  347,  347,  347,  241,  241,
  241,  241,  241,  241,  241,  241,    0,  211,  241,    0,
  241,  241,    0,    0,  241,    0,    0,   81,    0,   82,
  241,  241,  241,  241,  241,  241,  241,  241,  241,  241,
  241,  241,  241,  241,  241,  241,  241,  241,  241,  241,
  211,    0,    0,    0,    0,    0,    0,    0,  211,    0,
  211,  211,  211,    0,  211,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  211,  211,  211,  211,
    0,    0,    0,    0,  241,  241,    0,  241,  241,    0,
    0,    0,    0,  241,   74,   75,   76,    0,    0,    0,
    0,   77,   78,    0,   79,    0,    0,   80,    0,  626,
  211,    0,    0,    0,    0,    0,    0,  241,   74,   75,
   76,    0,    0,    0,    0,   77,   78,    0,   79,    0,
    0,   80,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  211,  211,    0,    0,   81,    0,   82,  347,  347,
    0,  347,  347,  347,  347,  347,  347,  347,  347,  347,
    0,  213,  347,    0,  347,  347,    0,    0,  347,   81,
    0,   82,    0,    0,  347,  347,  347,  347,  347,  347,
  347,  347,  347,  347,    0,    0,    0,  347,  347,  347,
  347,  347,  347,  347,  213,    0,    0,    0,    0,    0,
    0,    0,  213,    0,  213,  213,  213,    0,  213,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  213,  213,  213,  213,    0,    0,    0,  629,  347,  347,
    0,  347,  347,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  631,    0,    0,  213,   74,   75,   76,    0,    0,
    0,  347,   77,   78,    0,   79,    0,    0,   80,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  213,  213,  211,  211,  211,
  211,  211,  211,  211,  211,    0,  242,  211,    0,  211,
  211,    0,    0,  211,    0,    0,    0,    0,    0,  211,
  211,  211,  211,  211,  211,  211,  211,  211,  211,  211,
  211,  211,  211,  211,  211,  211,  211,  211,  211,  242,
    0,    0,    0,    0,    0,    0,    0,  242,    0,  242,
  242,  242,    0,  242,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  242,  242,  242,  242,    0,
    0,    0,    0,  211,  211,    0,  211,  211,    0,    0,
    0,    0,  211,   74,   75,   76,    0,    0,    0,    0,
   77,   78,    0,   79,    0,    0,   80,    0,    0,  242,
    0,    0,    0,    0,    0,    0,  211,   74,   75,   76,
    0,    0,    0,    0,   77,   78,    0,   79,    0,    0,
   80,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  242,  242,    0,    0,   81,    0,   82,    0,    0,    0,
    0,  213,  213,  213,  213,  213,  213,  213,  213,    0,
  201,  213,    0,  213,  213,    0,    0,  213,    0,    0,
    0,    0,    0,  213,  213,  213,  213,  213,  213,  213,
  213,  213,  213,  213,  213,  213,  213,  213,  213,  213,
  213,  213,  213,  201,    0,    0,    0,    0,    0,    0,
    0,  201,    0,  201,  201,  201,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  201,
  201,  201,  201,    0,  635,    0,    0,  213,  213,    0,
  213,  213,    0,    0,    0,    0,  213,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  201,    0,    0,    0,    0,    0,    0,
  213,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  201,  201,  242,  242,  242,  242,
  242,  242,  242,  242,    0,  204,  242,    0,  242,  242,
    0,    0,  242,    0,    0,    0,    0,    0,  242,  242,
  242,  242,  242,  242,  242,  242,  242,  242,  242,  242,
  242,  242,  242,  242,  242,  242,  242,  242,  204,    0,
    0,    0,    0,    0,    0,    0,  204,    0,  204,  204,
  204,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  204,  204,  204,  204,    0,    0,
    0,    0,  242,  242,    0,  242,  242,    0,    0,    0,
    0,  242,   74,   75,   76,    0,    0,    0,    0,   77,
   78,    0,   79,    0,    0,   80,    0,    0,  204,    0,
    0,    0,    0,    0,    0,  242,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  204,
  204,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  201,  201,  201,  201,  201,  201,  201,  201,    0,  203,
  201,    0,  201,  201,    0,    0,  201,    0,    0,    0,
    0,    0,  201,  201,  201,  201,  201,  201,  201,  201,
  201,  201,  201,  201,  201,  201,  201,  201,  201,  201,
  201,  201,  203,    0,    0,    0,    0,    0,    0,    0,
  203,    0,  203,  203,  203,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  203,  203,
  203,  203,    0,    0,    0,    0,  201,  201,    0,  201,
  201,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  203,    0,    0,    0,    0,    0,    0,  201,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  203,  203,  204,  204,  204,  204,  204,
  204,  204,  204,    0,  202,  204,    0,  204,  204,    0,
    0,  204,    0,    0,    0,    0,    0,  204,  204,  204,
  204,  204,  204,  204,  204,  204,  204,  204,  204,  204,
  204,  204,  204,  204,  204,  204,  204,  202,    0,    0,
    0,    0,    0,    0,    0,  202,    0,  202,  202,  202,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  202,  202,  202,  202,    0,    0,    0,
    0,  204,  204,    0,  204,  204,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  202,    0,    0,
    0,    0,    0,    0,  204,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  202,  202,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  203,
  203,  203,  203,  203,  203,  203,  203,    0,  169,  203,
    0,  203,  203,    0,    0,  203,    0,    0,    0,    0,
    0,  203,  203,  203,  203,  203,  203,  203,  203,  203,
  203,  203,  203,  203,  203,  203,  203,  203,  203,  203,
  203,  169,    0,    0,    0,    0,    0,    0,    0,  169,
    0,  169,  169,  169,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  169,  169,  169,
  169,    0,    0,    0,    0,  203,  203,    0,  203,  203,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  169,    0,    0,    0,    0,    0,    0,  203,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  169,  169,  202,  202,  202,  202,  202,  202,
  202,  202,    0,  344,  202,    0,  202,  202,    0,    0,
  202,    0,    0,    0,    0,    0,  202,  202,  202,  202,
  202,  202,  202,  202,  202,  202,  202,  202,  202,  202,
  202,  202,  202,  202,  202,  202,  344,    0,    0,    0,
    0,    0,    0,    0,  344,    0,  344,  344,  344,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  344,  344,  344,  344,    0,    0,    0,    0,
  202,  202,    0,  202,  202,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  344,    0,    0,    0,
    0,    0,    0,  202,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  344,  344,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  169,  169,
  169,  169,  169,  169,  169,  169,    0,  167,  169,    0,
  169,  169,    0,    0,  169,    0,    0,    0,    0,    0,
  169,  169,  169,  169,  169,  169,  169,  169,  169,  169,
  169,  169,    0,  169,  169,  169,  169,  169,  169,  169,
  167,    0,    0,    0,    0,    0,    0,    0,  167,    0,
  167,  167,  167,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  167,  167,  167,  167,
    0,    0,    0,    0,  169,  169,    0,  169,  169,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  167,    0,    0,    0,    0,    0,    0,  169,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  167,  167,  344,  344,  344,  344,  344,  344,  344,
  344,    0,  165,  344,    0,  344,  344,    0,    0,  344,
    0,    0,    0,    0,    0,  344,  344,  344,  344,  344,
  344,  344,  344,  344,  344,  344,  344,    0,  344,  344,
  344,  344,  344,  344,  344,  165,    0,    0,    0,    0,
    0,    0,    0,  165,    0,  165,  165,  165,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  165,  165,  165,  165,    0,    0,    0,    0,  344,
  344,    0,  344,  344,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  165,    0,    0,    0,    0,
    0,    0,  344,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  165,  165,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  167,  167,  167,
  167,  167,  167,  167,  167,    0,  163,  167,    0,  167,
  167,    0,    0,  167,    0,    0,    0,    0,    0,  167,
  167,  167,  167,  167,  167,  167,  167,  167,  167,  167,
    0,    0,  167,  167,  167,  167,  167,  167,  167,  163,
    0,    0,    0,    0,    0,    0,    0,  163,    0,  163,
  163,  163,    0,  157,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  163,  163,  163,  163,    0,
    0,    0,    0,  167,  167,    0,  167,  167,    0,    0,
    0,    0,    0,    0,    0,    0,  157,    0,    0,    0,
    0,    0,    0,    0,  157,    0,  157,  157,  157,  163,
    0,    0,    0,    0,    0,    0,  167,    0,    0,    0,
    0,    0,  157,  157,  157,  157,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  163,  163,  165,  165,  165,  165,  165,  165,  165,  165,
    0,  158,  165,    0,  165,  165,  157,    0,  165,    0,
    0,    0,    0,    0,  165,  165,  165,  165,  165,  165,
  165,  165,  165,  165,    0,    0,    0,  165,  165,  165,
  165,  165,  165,  165,  158,    0,    0,  157,  157,    0,
    0,    0,  158,    0,  158,  158,  158,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  158,  158,  158,  158,    0,    0,    0,    0,  165,  165,
    0,  165,  165,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  158,    0,    0,    0,    0,    0,
    0,  165,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  158,  158,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  163,  163,  163,  163,
  163,  163,  163,  163,    0,    0,  163,    0,  163,  163,
    0,    0,  163,    0,    0,    0,    0,    0,  163,  163,
  163,  163,  163,  163,  163,  163,  163,    0,    0,    0,
    0,  163,  163,  163,  163,  163,  163,  163,    0,    0,
    0,    0,    0,  157,  157,  157,  157,  157,  157,  157,
  157,    0,  159,  157,    0,  157,  157,    0,    0,  157,
    0,    0,    0,    0,    0,  157,  157,  157,  157,  157,
  157,  157,  163,  163,    0,  163,  163,    0,  157,  157,
  157,  157,  157,  157,  157,  159,    0,    0,    0,    0,
    0,    0,    0,  159,    0,  159,  159,  159,    0,    0,
    0,    0,    0,    0,    0,  163,    0,    0,    0,    0,
    0,  159,  159,  159,  159,    0,    0,    0,    0,  157,
  157,    0,  157,  157,    0,    0,    0,    0,    0,    0,
    0,  158,  158,  158,  158,  158,  158,  158,  158,    0,
  152,  158,    0,  158,  158,  159,    0,  158,    0,    0,
    0,    0,  157,  158,  158,  158,  158,  158,  158,  158,
    0,    0,    0,    0,    0,    0,  158,  158,  158,  158,
  158,  158,  158,  152,    0,    0,  159,  159,    0,    0,
    0,  152,    0,  152,  152,  152,  154,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  152,
  152,  152,  152,    0,    0,    0,    0,  158,  158,    0,
  158,  158,    0,    0,    0,    0,    0,    0,    0,  154,
    0,    0,    0,    0,    0,    0,    0,  154,    0,  154,
  154,  154,    0,  152,    0,    0,    0,    0,    0,    0,
  158,    0,    0,    0,    0,  154,  154,  154,  154,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  152,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  154,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  154,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  159,  159,  159,  159,  159,  159,  159,  159,
    0,    0,  159,    0,  159,  159,    0,    0,  159,    0,
    0,    0,    0,    0,  159,  159,  159,  159,  159,  159,
  159,    0,    0,    0,    0,    0,    0,  159,  159,  159,
  159,  159,  159,  159,    0,    0,    0,    0,    0,  177,
    0,    0,    0,  177,    0,  177,  177,    0,  177,  177,
  177,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  177,  159,  159,
    0,  159,  159,    0,    0,    0,    0,    0,    0,    0,
  152,  152,  152,  152,  152,  152,  152,  152,    0,    0,
  152,    0,  152,  152,    0,    0,  152,    0,    0,    0,
    0,  159,  152,  152,  152,  152,  152,  152,    0,    0,
    0,    0,    0,    0,    0,  152,  152,  152,  152,  152,
  152,  152,    0,    0,    0,    0,  154,  154,  154,  154,
  154,  154,  154,  154,    0,  155,  154,    0,  154,  154,
    0,    0,  154,    0,    0,    0,    0,    0,  154,  154,
  154,  154,  154,  154,    0,    0,  152,  152,    0,  152,
  152,  154,  154,  154,  154,  154,  154,  154,  155,    0,
    0,    0,    0,    0,    0,    0,  155,    0,  155,  155,
  155,  156,    0,    0,    0,    0,    0,    0,    0,  152,
    0,    0,    0,    0,  155,  155,  155,  155,    0,    0,
    0,    0,  154,  154,    0,  154,  154,    0,    0,    0,
    0,    0,    0,    0,  156,    0,    0,    0,    0,    0,
    0,    0,  156,    0,  156,  156,  156,    0,  155,    0,
    0,    0,    0,    0,    0,  154,    0,    0,    0,    0,
  156,  156,  156,  156,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  155,    0,    0,    0,    0,    0,    0,  177,  177,  153,
    0,    0,    0,    0,  156,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  177,  177,  177,  177,  177,    0,    0,    0,    0,    0,
    0,    0,  153,    0,    0,    0,  156,    0,    0,    0,
  153,    0,  153,  153,  153,  177,    0,    0,  177,  177,
  177,  177,  177,  177,  177,  177,    0,    0,  153,  153,
  153,  153,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  177,  177,  177,  177,  177,  177,
    0,  177,  153,  177,    0,    0,    0,    0,    0,  177,
  177,  177,  177,  177,  177,  177,  177,  177,  177,  177,
  177,  177,    0,    0,  177,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  153,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  155,  155,  155,  155,  155,
  155,  155,  155,    0,    0,  155,    0,  155,  155,    0,
    0,  155,    0,    0,    0,    0,    0,  155,  155,  155,
  155,  155,  155,    0,    0,    0,    0,    0,    0,    0,
  155,  155,  155,  155,  155,  155,  155,    0,    0,    0,
    0,  156,  156,  156,  156,  156,  156,  156,  156,    0,
  149,  156,    0,  156,  156,    0,    0,  156,    0,    0,
    0,    0,    0,  156,  156,  156,  156,  156,  156,    0,
    0,  155,  155,    0,  155,  155,  156,  156,  156,  156,
  156,  156,  156,  149,    0,    0,    0,    0,    0,    0,
    0,  149,    0,  149,  149,  149,    0,    0,    0,    0,
    0,    0,    0,    0,  155,    0,    0,    0,    0,  149,
  149,  149,  149,    0,    0,    0,    0,  156,  156,    0,
  156,  156,    0,    0,    0,    0,    0,    0,    0,  153,
  153,  153,  153,  153,  153,  153,  153,    0,  150,  153,
    0,  153,  153,  149,    0,  153,    0,    0,    0,    0,
  156,  153,  153,  153,  153,  153,  153,    0,    0,    0,
    0,    0,    0,    0,  153,  153,  153,  153,  153,  153,
  153,  150,    0,    0,    0,  149,    0,    0,    0,  150,
    0,  150,  150,  150,  151,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  150,  150,  150,
  150,    0,    0,    0,    0,  153,  153,    0,  153,  153,
    0,    0,    0,    0,    0,    0,    0,  151,    0,    0,
    0,    0,    0,    0,    0,  151,    0,  151,  151,  151,
    0,  150,    0,    0,    0,    0,    0,    0,  153,    0,
    0,    0,    0,  151,  151,  151,  151,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  150,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  151,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  147,  151,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  149,  149,  149,  149,  149,  149,  149,  149,    0,    0,
  149,    0,  149,  149,    0,    0,  149,    0,    0,    0,
    0,  147,  149,  149,  149,    0,    0,    0,    0,  147,
    0,    0,  147,    0,    0,  149,  149,  149,  149,  149,
  149,  149,    0,    0,    0,    0,    0,  147,  147,  147,
  147,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  143,    0,    0,  149,  149,    0,  149,
  149,  147,    0,    0,    0,    0,    0,    0,  150,  150,
  150,  150,  150,  150,  150,  150,    0,    0,  150,    0,
  150,  150,    0,    0,  150,    0,    0,    0,    0,    0,
  150,  150,  150,  147,  143,    0,    0,  143,    0,  148,
    0,    0,    0,  150,  150,  150,  150,  150,  150,  150,
    0,    0,  143,    0,  151,  151,  151,  151,  151,  151,
  151,  151,    0,    0,  151,    0,  151,  151,    0,    0,
  151,    0,  148,    0,    0,    0,  151,  151,  151,    0,
  148,  139,    0,  148,  150,  150,  143,  150,  150,  151,
  151,  151,  151,  151,  151,  151,    0,    0,  148,  148,
  148,  148,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  143,    0,
    0,    0,  139,    0,    0,  139,    0,  140,    0,    0,
  151,  151,  148,  151,  151,    0,    0,    0,    0,    0,
  139,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  148,    0,    0,    0,  140,    0,
    0,  140,    0,    0,  139,    0,    0,    0,  147,  147,
  147,  147,  147,  147,  147,  147,  140,    0,  147,    0,
  147,  147,    0,    0,  147,    0,    0,    0,    0,    0,
  147,  147,    0,    0,    0,    0,  139,    0,    0,    0,
    0,    0,    0,  147,  147,  147,  147,  147,  147,  147,
  140,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  140,    0,  147,  147,    0,  147,  147,    0,
    0,    0,    0,  143,  143,  143,  143,  143,  143,  143,
  143,    0,    0,  143,    0,  143,  143,    0,    0,  143,
    0,    0,    0,    0,    0,  143,  143,    0,    0,   10,
    0,    0,    0,   10,    0,   10,   10,    0,   10,   10,
   10,    0,    0,    0,    0,    0,    0,    0,    0,  148,
  148,  148,  148,  148,  148,  148,  148,   10,    0,  148,
    0,  148,  148,    0,    0,  148,    0,    0,    0,    0,
    0,  148,  148,    0,    0,    0,    0,    0,    0,  143,
  143,    0,  143,  143,  148,  148,  148,  148,  148,  148,
  148,  139,  139,  139,  139,  139,  139,  139,  139,    0,
    0,  139,    0,  139,  139,    0,    0,  139,    0,    0,
    0,    0,    0,    0,  139,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  148,  148,    0,  148,  148,
    0,    0,    0,    0,    0,    0,    0,  140,  140,  140,
  140,  140,  140,  140,  140,    0,    0,  140,  179,  140,
  140,    0,  179,  140,  179,  179,    0,  179,  179,  179,
  140,    0,    0,    0,    0,    0,    0,  139,  139,    0,
  139,  139,    0,    0,    0,    0,  179,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  140,  140,    0,  140,  140,    0,    0,
    0,    0,  223,    0,    0,    0,  224,    0,  227,    0,
    0,    0,  228,  225,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  226,    0,    0,    0,    0,    0,    0,   10,   10,    0,
    0,    0,    0,    0,   10,   10,   10,    0,    0,    0,
    0,   10,   10,    0,   10,    0,    0,   10,    0,    0,
   10,   10,   10,   10,   10,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  172,    0,    0,    0,
  172,    0,  172,    0,    0,   10,  172,  172,   10,   10,
   10,   10,   10,   10,   10,   10,    0,    0,    0,    0,
    0,    0,    0,    0,  172,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   10,   10,   10,   10,   10,   10,
    0,   10,    0,   10,    0,    0,    0,    0,    0,   10,
   10,   10,   10,   10,   10,   10,   10,   10,   10,   10,
   10,   10,    0,    0,   10,    0,  179,  179,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  179,
  179,  179,  179,  179,    0,    0,    0,    0,    0,    0,
    0,  223,    0,    0,    0,  224,    0,  227,    0,    0,
    0,  228,  225,    0,  179,    0,    0,  179,  179,  179,
  179,  179,  179,  179,  179,    0,    0,    0,    0,  226,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  185,  186,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  179,  179,  179,  179,  179,  179,    0,
  179,    0,  179,  187,  188,  189,  190,  191,  179,  179,
  179,  179,  179,  179,  179,  179,  179,  179,  179,  179,
  179,    0,    0,  179,    0,    0,    0,    0,  192,    0,
    0,  193,  194,  195,  196,  197,  198,  199,  200,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  172,  172,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  201,  202,  203,
  204,  205,  206,    0,  207,    0,  208,  172,  172,  172,
  172,  172,  209,  210,  211,  212,  213,  214,  215,  216,
  217,  218,  219,  220,  221,    0,    0,  222,    0,    0,
    0,    0,  172,    0,    0,  172,  172,  172,  172,  172,
  172,  172,  172,    0,    0,    0,    0,    0,    0,  223,
    0,    0,    0,  224,    0,  227,    0,    0,    0,  228,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  172,  172,  172,  172,  172,  172,  226,  172,    0,
  172,    0,    0,    0,    0,    0,  172,  172,  172,  172,
  172,  172,  172,  172,  172,  172,  172,  172,  172,  185,
  186,  172,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  187,  188,  189,  190,  191,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  193,  194,  195,  196,  197,  198,  199,  200,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  201,  202,  203,  204,
  205,  206,    0,  207,    0,    0,    0,    0,    0,    0,
    0,  209,  210,  211,  212,  213,  214,  215,  216,  217,
  218,  219,  220,  221,    0,    0,  222,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  185,  186,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  187,  188,  189,  190,  191,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  193,  194,
  195,  196,  197,  198,  199,  200,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  201,  202,  203,  204,    0,  206,
    0,  207,    0,    0,    0,    0,    0,    0,    0,  209,
  210,  211,  212,  213,  214,  215,  216,  217,  218,  219,
  220,  221,    0,    0,  222,
  };
  protected static  short [] yyCheck = {            22,
  181,  182,    4,  145,   34,  147,  148,  149,  150,  182,
   34,   13,   14,   34,  106,   41,   34,  123,   47,   41,
   40,   36,   58,   58,   41,   42,   59,   27,  123,  123,
   41,   42,   40,   40,   47,  124,  416,   36,   47,  123,
  559,   44,   47,  602,   44,  263,   48,   44,   44,   62,
   44,  610,  317,   62,   56,   57,   40,   62,   25,   44,
   40,  123,   44,  125,  295,  296,  297,   44,   43,  263,
   45,   44,   44,   44,   44,   44,   44,  205,  258,   44,
  260,   44,   44,   44,  240,   44,   44,   44,   44,   44,
   44,   41,  321,  249,   44,  325,   47,  225,  321,  123,
  270,  125,   41,  123,  263,  123,  388,  125,  263,  109,
  110,   62,  293,  205,  116,  123,  123,  277,  141,  142,
  293,  123,  322,  317,   63,  275,  276,  277,  125,  359,
  348,   93,  691,  225,  300,  694,  302,   60,  304,  123,
   91,  108,   41,  123,  258,   44,  260,  666,  379,  668,
  317,   60,  125,  125,  125,  125,  125,  125,   47,  123,
  125,  125,  125,  305,  125,  307,  125,  125,  125,  125,
  125,  125,  266,   62,  268,   43,  318,   45,  320,  349,
  350,   36,  266,  662,  663,  565,  346,  347,   60,  226,
  322,  270,  271,  272,  273,  274,  346,  347,  677,   59,
  123,  224,  125,   61,  241,  684,   47,  244,   47,  298,
   47,  382,  383,  384,  123,  338,  125,  321,  313,  314,
  343,   62,  345,   62,  257,   62,   43,  401,   45,  403,
  266,  266,  406,    0,  376,  331,  332,  333,  334,  335,
  336,  337,  338,  339,  340,  341,  342,  343,  344,  345,
  321,  123,   61,  125,  360,  270,  173,  174,  175,  176,
   41,  267,  288,   44,  326,  327,  328,  258,  259,   36,
  349,  350,  554,   40,  270,   42,   43,  280,   45,   46,
   47,   44,  282,  283,  286,  270,  280,  263,  270,  288,
  289,  290,  291,  270,  436,  317,  438,   64,  317,  321,
   40,  321,  326,  327,  328,  322,  398,  399,  326,  327,
  328,  322,  325,  322,  322,  322,  315,  322,  380,  294,
  264,  265,  503,  322,  386,   41,  355,  353,   44,  267,
  503,  269,  513,  323,  357,  358,  359,  261,  262,  362,
  513,   43,  365,   45,  367,   36,  369,   36,    0,  372,
  380,  633,  351,  352,  353,  354,  379,   41,  500,  380,
   44,  385,  326,  327,  328,  364,  365,  385,   41,   33,
  512,   44,  654,  396,  516,   41,  658,  400,   44,  167,
  168,  169,   36,  665,   36,   40,  346,  347,   40,  671,
   42,   43,   40,   45,   46,   47,   60,   61,   62,  541,
   60,   61,    0,  326,  327,  328,  299,  300,   44,  411,
   61,   62,   64,  282,  283,  293,  380,  326,  327,  328,
  171,  172,  386,  125,  177,  178,  568,  569,  430,  711,
  292,  713,  455,  356,  716,  358,  718,  179,  180,  362,
   81,   82,  301,   41,  467,  302,   44,  356,  304,  358,
  473,  398,  399,  362,  326,  327,  328,  303,  381,  183,
  184,   59,  643,  317,   61,  267,  558,   61,  268,   36,
  643,  323,  381,   44,  323,   44,  323,  619,  620,  270,
  622,  504,   61,  123,  356,  123,  358,  322,  123,   40,
  362,   40,    0,   58,   60,   93,   40,  264,  265,   40,
   40,  317,  317,  325,  271,  272,  273,   91,  322,  381,
  323,  278,  279,   58,  281,   44,  518,  284,  520,  661,
  287,  288,  289,  290,  291,  323,   41,  125,   36,  270,
  672,   44,   40,   44,   42,   43,   40,   45,   46,   47,
   40,   40,   40,   40,   63,  312,  123,  123,  315,  316,
  317,  318,  319,  320,  321,  322,   64,  123,   41,  125,
   41,  123,  585,   41,  123,   42,  589,  322,  322,  271,
  272,  273,  357,  322,  597,  577,  278,  279,  325,  281,
  267,   61,  284,  321,  351,  352,  353,  354,  355,  356,
   43,  358,   45,  360,   58,  269,  282,  285,  277,  366,
  367,  368,  369,  370,  371,  372,  373,  374,  375,  376,
  377,  378,  264,  265,  381,   41,   41,   41,   41,  271,
  272,  273,   41,   41,  317,  324,  278,  279,   42,  281,
   91,  325,  284,   36,   61,  287,  288,  289,  290,  291,
  269,  305,  306,  307,  308,  309,  310,  311,  123,  277,
    0,  270,  271,  272,  273,  274,  125,  322,  123,  359,
  312,  123,   62,  315,  316,  317,  318,  319,  320,  321,
  322,  361,  270,  271,  272,  273,  274,  275,  276,  277,
  322,   62,  280,   33,  282,  283,  323,  270,  286,  323,
  286,   41,   41,   43,   44,   45,   61,   47,   93,  351,
  352,  353,  354,  355,  356,  317,  358,   47,  360,   59,
   60,   61,   62,  125,  366,  367,  368,  369,  370,  371,
  372,  373,  374,  375,  376,  377,  378,   41,   47,  381,
  349,  350,  268,  323,  125,   60,  363,  322,   47,  325,
   60,   91,  322,   93,   47,  270,  270,  322,  346,  347,
  322,  349,  350,   47,   62,   62,  264,  265,   62,   47,
  326,  327,  328,  271,  272,  273,   62,   62,  322,   62,
  278,  279,   47,  281,  124,  125,  284,   47,   62,  287,
  288,  289,  290,  291,  322,  322,   62,   41,  322,   43,
  356,   45,  358,   62,    0,   62,  362,  317,  123,    0,
  125,   59,    0,  123,  312,  125,   59,  315,  316,  317,
  318,  319,  320,  321,  322,  381,   41,   60,  271,  272,
  273,  322,   59,   41,   58,  278,  279,   33,  281,  269,
   59,  284,  270,  270,  269,   41,   91,   43,   44,   45,
  125,   47,   61,  351,  352,  353,  354,  355,  356,   62,
  358,   19,  360,   59,   60,   61,   62,   19,  366,  367,
  368,  369,  370,  371,  372,  373,  374,  375,  376,  377,
  378,   23,  285,  381,  288,  289,  290,  291,   24,   24,
  123,   91,  125,  410,  415,   91,  151,   93,  270,  271,
  272,  273,  274,  565,  417,   91,   75,  301,  303,  150,
   78,  315,  270,  271,  272,  273,  274,  321,  322,  306,
  436,  514,  434,  146,  155,  558,  184,  233,  124,  125,
  270,  271,  272,  273,  274,  275,  276,  277,  170,    0,
  280,  640,  282,  283,  684,  561,  286,  351,  352,  353,
  354,  677,  292,  293,  294,  295,  296,  297,  298,  299,
  300,  301,  302,  303,  304,  305,  306,  307,  308,  309,
  310,  311,   33,  380,  582,  380,  534,  349,  350,   -1,
   41,  381,   43,   44,   45,  382,   47,   -1,   -1,   -1,
   -1,  349,  350,   -1,   -1,   -1,   -1,   -1,   59,   60,
   61,   62,   60,   -1,   -1,   -1,  346,  347,   -1,  349,
  350,  326,  327,  328,   -1,  355,  326,  327,  328,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  271,  272,  273,
   -1,   -1,   93,   -1,  278,  279,   -1,  281,   -1,  379,
  284,  356,   -1,  358,   -1,   -1,  356,  362,  358,   -1,
   -1,   -1,  362,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  124,  125,  123,  381,  125,   -1,   -1,
   -1,  381,   -1,   -1,  270,  271,  272,  273,  274,  275,
  276,  277,   -1,    0,  280,   -1,  282,  283,   -1,   -1,
  286,   -1,   -1,  326,  327,  328,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,  305,
  306,  307,  308,  309,  310,  311,   33,   -1,   -1,   -1,
   -1,   -1,   -1,  356,   41,  358,   43,   44,   45,  362,
   47,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   59,   60,   61,   62,   -1,   -1,  381,   -1,
  346,  347,   -1,  349,  350,   -1,   -1,   -1,   -1,  355,
  330,  331,  332,  333,  334,  335,  336,  337,  338,  339,
  340,  341,  342,  343,  344,  345,   93,   -1,   -1,   -1,
   -1,   -1,   -1,  379,   -1,   -1,   -1,   -1,   -1,   -1,
  288,  289,  290,  291,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  124,  125,  270,
  271,  272,  273,  274,  275,  276,  277,  315,    0,  280,
   -1,  282,  283,   -1,  322,  286,   41,   -1,   43,   -1,
   45,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,  303,  304,  305,  306,  307,  308,  309,  310,
  311,   33,   -1,  351,  352,  353,  354,   -1,   -1,   41,
   -1,   43,   44,   45,   -1,   47,  364,  365,  326,  327,
  328,   -1,   -1,   -1,   -1,   -1,   -1,   59,   60,   61,
   62,   -1,   -1,   -1,   -1,  346,  347,   -1,  349,  350,
   -1,   -1,   -1,   -1,  355,   -1,   -1,   -1,  356,   -1,
  358,   -1,   -1,   -1,  362,   -1,   -1,   -1,   -1,   -1,
   -1,   93,   -1,   -1,   -1,   -1,   -1,   -1,  379,   -1,
   -1,   -1,   -1,  381,  329,  330,  331,  332,  333,  334,
  335,  336,  337,  338,  339,  340,  341,  342,  343,  344,
  345,   -1,  124,  125,   -1,   -1,   43,   -1,   45,   -1,
   -1,   -1,   -1,  270,  271,  272,  273,  274,  275,  276,
  277,   -1,    0,  280,   -1,  282,  283,   -1,   -1,  286,
   43,   -1,   45,   -1,   -1,  292,  293,  294,  295,  296,
  297,  298,  299,  300,  301,  302,  303,  304,  305,  306,
  307,  308,  309,  310,  311,   33,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   41,   -1,   43,   44,   45,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   58,   59,   60,   61,   62,   -1,   -1,   -1,  125,  346,
  347,   -1,  349,  350,   -1,   -1,   -1,   -1,  355,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  125,   -1,   -1,   93,  271,  272,  273,   -1,
   -1,   -1,  379,  278,  279,   -1,  281,   -1,   -1,  284,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  123,  124,  125,  270,  271,
  272,  273,  274,  275,  276,  277,   -1,    0,  280,   -1,
  282,  283,   -1,   -1,  286,   -1,   -1,   43,   -1,   45,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,  304,  305,  306,  307,  308,  309,  310,  311,
   33,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   41,   -1,
   43,   44,   45,   -1,   47,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   59,   60,   61,   62,
   -1,   -1,   -1,   -1,  346,  347,   -1,  349,  350,   -1,
   -1,   -1,   -1,  355,  271,  272,  273,   -1,   -1,   -1,
   -1,  278,  279,   -1,  281,   -1,   -1,  284,   -1,  125,
   93,   -1,   -1,   -1,   -1,   -1,   -1,  379,  271,  272,
  273,   -1,   -1,   -1,   -1,  278,  279,   -1,  281,   -1,
   -1,  284,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  124,  125,   -1,   -1,   43,   -1,   45,  266,  267,
   -1,  269,  270,  271,  272,  273,  274,  275,  276,  277,
   -1,    0,  280,   -1,  282,  283,   -1,   -1,  286,   43,
   -1,   45,   -1,   -1,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,   -1,   -1,   -1,  305,  306,  307,
  308,  309,  310,  311,   33,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   41,   -1,   43,   44,   45,   -1,   47,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   59,   60,   61,   62,   -1,   -1,   -1,  125,  346,  347,
   -1,  349,  350,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  125,   -1,   -1,   93,  271,  272,  273,   -1,   -1,
   -1,  379,  278,  279,   -1,  281,   -1,   -1,  284,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  124,  125,  270,  271,  272,
  273,  274,  275,  276,  277,   -1,    0,  280,   -1,  282,
  283,   -1,   -1,  286,   -1,   -1,   -1,   -1,   -1,  292,
  293,  294,  295,  296,  297,  298,  299,  300,  301,  302,
  303,  304,  305,  306,  307,  308,  309,  310,  311,   33,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   41,   -1,   43,
   44,   45,   -1,   47,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   59,   60,   61,   62,   -1,
   -1,   -1,   -1,  346,  347,   -1,  349,  350,   -1,   -1,
   -1,   -1,  355,  271,  272,  273,   -1,   -1,   -1,   -1,
  278,  279,   -1,  281,   -1,   -1,  284,   -1,   -1,   93,
   -1,   -1,   -1,   -1,   -1,   -1,  379,  271,  272,  273,
   -1,   -1,   -1,   -1,  278,  279,   -1,  281,   -1,   -1,
  284,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  124,  125,   -1,   -1,   43,   -1,   45,   -1,   -1,   -1,
   -1,  270,  271,  272,  273,  274,  275,  276,  277,   -1,
    0,  280,   -1,  282,  283,   -1,   -1,  286,   -1,   -1,
   -1,   -1,   -1,  292,  293,  294,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,  305,  306,  307,  308,
  309,  310,  311,   33,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   41,   -1,   43,   44,   45,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   59,
   60,   61,   62,   -1,  123,   -1,   -1,  346,  347,   -1,
  349,  350,   -1,   -1,   -1,   -1,  355,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   93,   -1,   -1,   -1,   -1,   -1,   -1,
  379,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  124,  125,  270,  271,  272,  273,
  274,  275,  276,  277,   -1,    0,  280,   -1,  282,  283,
   -1,   -1,  286,   -1,   -1,   -1,   -1,   -1,  292,  293,
  294,  295,  296,  297,  298,  299,  300,  301,  302,  303,
  304,  305,  306,  307,  308,  309,  310,  311,   33,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   41,   -1,   43,   44,
   45,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   59,   60,   61,   62,   -1,   -1,
   -1,   -1,  346,  347,   -1,  349,  350,   -1,   -1,   -1,
   -1,  355,  271,  272,  273,   -1,   -1,   -1,   -1,  278,
  279,   -1,  281,   -1,   -1,  284,   -1,   -1,   93,   -1,
   -1,   -1,   -1,   -1,   -1,  379,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  124,
  125,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  270,  271,  272,  273,  274,  275,  276,  277,   -1,    0,
  280,   -1,  282,  283,   -1,   -1,  286,   -1,   -1,   -1,
   -1,   -1,  292,  293,  294,  295,  296,  297,  298,  299,
  300,  301,  302,  303,  304,  305,  306,  307,  308,  309,
  310,  311,   33,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   41,   -1,   43,   44,   45,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   59,   60,
   61,   62,   -1,   -1,   -1,   -1,  346,  347,   -1,  349,
  350,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   93,   -1,   -1,   -1,   -1,   -1,   -1,  379,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  124,  125,  270,  271,  272,  273,  274,
  275,  276,  277,   -1,    0,  280,   -1,  282,  283,   -1,
   -1,  286,   -1,   -1,   -1,   -1,   -1,  292,  293,  294,
  295,  296,  297,  298,  299,  300,  301,  302,  303,  304,
  305,  306,  307,  308,  309,  310,  311,   33,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   41,   -1,   43,   44,   45,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   59,   60,   61,   62,   -1,   -1,   -1,
   -1,  346,  347,   -1,  349,  350,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   93,   -1,   -1,
   -1,   -1,   -1,   -1,  379,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  124,  125,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  270,
  271,  272,  273,  274,  275,  276,  277,   -1,    0,  280,
   -1,  282,  283,   -1,   -1,  286,   -1,   -1,   -1,   -1,
   -1,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,  303,  304,  305,  306,  307,  308,  309,  310,
  311,   33,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   41,
   -1,   43,   44,   45,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   59,   60,   61,
   62,   -1,   -1,   -1,   -1,  346,  347,   -1,  349,  350,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   93,   -1,   -1,   -1,   -1,   -1,   -1,  379,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  124,  125,  270,  271,  272,  273,  274,  275,
  276,  277,   -1,    0,  280,   -1,  282,  283,   -1,   -1,
  286,   -1,   -1,   -1,   -1,   -1,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,  305,
  306,  307,  308,  309,  310,  311,   33,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   41,   -1,   43,   44,   45,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   59,   60,   61,   62,   -1,   -1,   -1,   -1,
  346,  347,   -1,  349,  350,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   93,   -1,   -1,   -1,
   -1,   -1,   -1,  379,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  124,  125,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  270,  271,
  272,  273,  274,  275,  276,  277,   -1,    0,  280,   -1,
  282,  283,   -1,   -1,  286,   -1,   -1,   -1,   -1,   -1,
  292,  293,  294,  295,  296,  297,  298,  299,  300,  301,
  302,  303,   -1,  305,  306,  307,  308,  309,  310,  311,
   33,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   41,   -1,
   43,   44,   45,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   59,   60,   61,   62,
   -1,   -1,   -1,   -1,  346,  347,   -1,  349,  350,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   93,   -1,   -1,   -1,   -1,   -1,   -1,  379,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  124,  125,  270,  271,  272,  273,  274,  275,  276,
  277,   -1,    0,  280,   -1,  282,  283,   -1,   -1,  286,
   -1,   -1,   -1,   -1,   -1,  292,  293,  294,  295,  296,
  297,  298,  299,  300,  301,  302,  303,   -1,  305,  306,
  307,  308,  309,  310,  311,   33,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   41,   -1,   43,   44,   45,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   59,   60,   61,   62,   -1,   -1,   -1,   -1,  346,
  347,   -1,  349,  350,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   93,   -1,   -1,   -1,   -1,
   -1,   -1,  379,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  124,  125,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  270,  271,  272,
  273,  274,  275,  276,  277,   -1,    0,  280,   -1,  282,
  283,   -1,   -1,  286,   -1,   -1,   -1,   -1,   -1,  292,
  293,  294,  295,  296,  297,  298,  299,  300,  301,  302,
   -1,   -1,  305,  306,  307,  308,  309,  310,  311,   33,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   41,   -1,   43,
   44,   45,   -1,    0,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   59,   60,   61,   62,   -1,
   -1,   -1,   -1,  346,  347,   -1,  349,  350,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   33,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   41,   -1,   43,   44,   45,   93,
   -1,   -1,   -1,   -1,   -1,   -1,  379,   -1,   -1,   -1,
   -1,   -1,   59,   60,   61,   62,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  124,  125,  270,  271,  272,  273,  274,  275,  276,  277,
   -1,    0,  280,   -1,  282,  283,   93,   -1,  286,   -1,
   -1,   -1,   -1,   -1,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,   -1,   -1,   -1,  305,  306,  307,
  308,  309,  310,  311,   33,   -1,   -1,  124,  125,   -1,
   -1,   -1,   41,   -1,   43,   44,   45,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   59,   60,   61,   62,   -1,   -1,   -1,   -1,  346,  347,
   -1,  349,  350,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   93,   -1,   -1,   -1,   -1,   -1,
   -1,  379,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  124,  125,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  270,  271,  272,  273,
  274,  275,  276,  277,   -1,   -1,  280,   -1,  282,  283,
   -1,   -1,  286,   -1,   -1,   -1,   -1,   -1,  292,  293,
  294,  295,  296,  297,  298,  299,  300,   -1,   -1,   -1,
   -1,  305,  306,  307,  308,  309,  310,  311,   -1,   -1,
   -1,   -1,   -1,  270,  271,  272,  273,  274,  275,  276,
  277,   -1,    0,  280,   -1,  282,  283,   -1,   -1,  286,
   -1,   -1,   -1,   -1,   -1,  292,  293,  294,  295,  296,
  297,  298,  346,  347,   -1,  349,  350,   -1,  305,  306,
  307,  308,  309,  310,  311,   33,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   41,   -1,   43,   44,   45,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  379,   -1,   -1,   -1,   -1,
   -1,   59,   60,   61,   62,   -1,   -1,   -1,   -1,  346,
  347,   -1,  349,  350,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  270,  271,  272,  273,  274,  275,  276,  277,   -1,
    0,  280,   -1,  282,  283,   93,   -1,  286,   -1,   -1,
   -1,   -1,  379,  292,  293,  294,  295,  296,  297,  298,
   -1,   -1,   -1,   -1,   -1,   -1,  305,  306,  307,  308,
  309,  310,  311,   33,   -1,   -1,  124,  125,   -1,   -1,
   -1,   41,   -1,   43,   44,   45,    0,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   59,
   60,   61,   62,   -1,   -1,   -1,   -1,  346,  347,   -1,
  349,  350,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   33,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   41,   -1,   43,
   44,   45,   -1,   93,   -1,   -1,   -1,   -1,   -1,   -1,
  379,   -1,   -1,   -1,   -1,   59,   60,   61,   62,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  125,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   93,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  125,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  270,  271,  272,  273,  274,  275,  276,  277,
   -1,   -1,  280,   -1,  282,  283,   -1,   -1,  286,   -1,
   -1,   -1,   -1,   -1,  292,  293,  294,  295,  296,  297,
  298,   -1,   -1,   -1,   -1,   -1,   -1,  305,  306,  307,
  308,  309,  310,  311,   -1,   -1,   -1,   -1,   -1,   36,
   -1,   -1,   -1,   40,   -1,   42,   43,   -1,   45,   46,
   47,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   64,  346,  347,
   -1,  349,  350,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  270,  271,  272,  273,  274,  275,  276,  277,   -1,   -1,
  280,   -1,  282,  283,   -1,   -1,  286,   -1,   -1,   -1,
   -1,  379,  292,  293,  294,  295,  296,  297,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  305,  306,  307,  308,  309,
  310,  311,   -1,   -1,   -1,   -1,  270,  271,  272,  273,
  274,  275,  276,  277,   -1,    0,  280,   -1,  282,  283,
   -1,   -1,  286,   -1,   -1,   -1,   -1,   -1,  292,  293,
  294,  295,  296,  297,   -1,   -1,  346,  347,   -1,  349,
  350,  305,  306,  307,  308,  309,  310,  311,   33,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   41,   -1,   43,   44,
   45,    0,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  379,
   -1,   -1,   -1,   -1,   59,   60,   61,   62,   -1,   -1,
   -1,   -1,  346,  347,   -1,  349,  350,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   33,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   41,   -1,   43,   44,   45,   -1,   93,   -1,
   -1,   -1,   -1,   -1,   -1,  379,   -1,   -1,   -1,   -1,
   59,   60,   61,   62,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  125,   -1,   -1,   -1,   -1,   -1,   -1,  264,  265,    0,
   -1,   -1,   -1,   -1,   93,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  287,  288,  289,  290,  291,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   33,   -1,   -1,   -1,  125,   -1,   -1,   -1,
   41,   -1,   43,   44,   45,  312,   -1,   -1,  315,  316,
  317,  318,  319,  320,  321,  322,   -1,   -1,   59,   60,
   61,   62,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  351,  352,  353,  354,  355,  356,
   -1,  358,   93,  360,   -1,   -1,   -1,   -1,   -1,  366,
  367,  368,  369,  370,  371,  372,  373,  374,  375,  376,
  377,  378,   -1,   -1,  381,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  125,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  270,  271,  272,  273,  274,
  275,  276,  277,   -1,   -1,  280,   -1,  282,  283,   -1,
   -1,  286,   -1,   -1,   -1,   -1,   -1,  292,  293,  294,
  295,  296,  297,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  305,  306,  307,  308,  309,  310,  311,   -1,   -1,   -1,
   -1,  270,  271,  272,  273,  274,  275,  276,  277,   -1,
    0,  280,   -1,  282,  283,   -1,   -1,  286,   -1,   -1,
   -1,   -1,   -1,  292,  293,  294,  295,  296,  297,   -1,
   -1,  346,  347,   -1,  349,  350,  305,  306,  307,  308,
  309,  310,  311,   33,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   41,   -1,   43,   44,   45,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  379,   -1,   -1,   -1,   -1,   59,
   60,   61,   62,   -1,   -1,   -1,   -1,  346,  347,   -1,
  349,  350,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  270,
  271,  272,  273,  274,  275,  276,  277,   -1,    0,  280,
   -1,  282,  283,   93,   -1,  286,   -1,   -1,   -1,   -1,
  379,  292,  293,  294,  295,  296,  297,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  305,  306,  307,  308,  309,  310,
  311,   33,   -1,   -1,   -1,  125,   -1,   -1,   -1,   41,
   -1,   43,   44,   45,    0,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   59,   60,   61,
   62,   -1,   -1,   -1,   -1,  346,  347,   -1,  349,  350,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   33,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   41,   -1,   43,   44,   45,
   -1,   93,   -1,   -1,   -1,   -1,   -1,   -1,  379,   -1,
   -1,   -1,   -1,   59,   60,   61,   62,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  125,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   93,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,    0,  125,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  270,  271,  272,  273,  274,  275,  276,  277,   -1,   -1,
  280,   -1,  282,  283,   -1,   -1,  286,   -1,   -1,   -1,
   -1,   33,  292,  293,  294,   -1,   -1,   -1,   -1,   41,
   -1,   -1,   44,   -1,   -1,  305,  306,  307,  308,  309,
  310,  311,   -1,   -1,   -1,   -1,   -1,   59,   60,   61,
   62,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,    0,   -1,   -1,  346,  347,   -1,  349,
  350,   93,   -1,   -1,   -1,   -1,   -1,   -1,  270,  271,
  272,  273,  274,  275,  276,  277,   -1,   -1,  280,   -1,
  282,  283,   -1,   -1,  286,   -1,   -1,   -1,   -1,   -1,
  292,  293,  294,  125,   41,   -1,   -1,   44,   -1,    0,
   -1,   -1,   -1,  305,  306,  307,  308,  309,  310,  311,
   -1,   -1,   59,   -1,  270,  271,  272,  273,  274,  275,
  276,  277,   -1,   -1,  280,   -1,  282,  283,   -1,   -1,
  286,   -1,   33,   -1,   -1,   -1,  292,  293,  294,   -1,
   41,    0,   -1,   44,  346,  347,   93,  349,  350,  305,
  306,  307,  308,  309,  310,  311,   -1,   -1,   59,   60,
   61,   62,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  125,   -1,
   -1,   -1,   41,   -1,   -1,   44,   -1,    0,   -1,   -1,
  346,  347,   93,  349,  350,   -1,   -1,   -1,   -1,   -1,
   59,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  125,   -1,   -1,   -1,   41,   -1,
   -1,   44,   -1,   -1,   93,   -1,   -1,   -1,  270,  271,
  272,  273,  274,  275,  276,  277,   59,   -1,  280,   -1,
  282,  283,   -1,   -1,  286,   -1,   -1,   -1,   -1,   -1,
  292,  293,   -1,   -1,   -1,   -1,  125,   -1,   -1,   -1,
   -1,   -1,   -1,  305,  306,  307,  308,  309,  310,  311,
   93,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  125,   -1,  346,  347,   -1,  349,  350,   -1,
   -1,   -1,   -1,  270,  271,  272,  273,  274,  275,  276,
  277,   -1,   -1,  280,   -1,  282,  283,   -1,   -1,  286,
   -1,   -1,   -1,   -1,   -1,  292,  293,   -1,   -1,   36,
   -1,   -1,   -1,   40,   -1,   42,   43,   -1,   45,   46,
   47,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  270,
  271,  272,  273,  274,  275,  276,  277,   64,   -1,  280,
   -1,  282,  283,   -1,   -1,  286,   -1,   -1,   -1,   -1,
   -1,  292,  293,   -1,   -1,   -1,   -1,   -1,   -1,  346,
  347,   -1,  349,  350,  305,  306,  307,  308,  309,  310,
  311,  270,  271,  272,  273,  274,  275,  276,  277,   -1,
   -1,  280,   -1,  282,  283,   -1,   -1,  286,   -1,   -1,
   -1,   -1,   -1,   -1,  293,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  346,  347,   -1,  349,  350,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  270,  271,  272,
  273,  274,  275,  276,  277,   -1,   -1,  280,   36,  282,
  283,   -1,   40,  286,   42,   43,   -1,   45,   46,   47,
  293,   -1,   -1,   -1,   -1,   -1,   -1,  346,  347,   -1,
  349,  350,   -1,   -1,   -1,   -1,   64,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  346,  347,   -1,  349,  350,   -1,   -1,
   -1,   -1,   36,   -1,   -1,   -1,   40,   -1,   42,   -1,
   -1,   -1,   46,   47,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   64,   -1,   -1,   -1,   -1,   -1,   -1,  264,  265,   -1,
   -1,   -1,   -1,   -1,  271,  272,  273,   -1,   -1,   -1,
   -1,  278,  279,   -1,  281,   -1,   -1,  284,   -1,   -1,
  287,  288,  289,  290,  291,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   36,   -1,   -1,   -1,
   40,   -1,   42,   -1,   -1,  312,   46,   47,  315,  316,
  317,  318,  319,  320,  321,  322,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   64,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  351,  352,  353,  354,  355,  356,
   -1,  358,   -1,  360,   -1,   -1,   -1,   -1,   -1,  366,
  367,  368,  369,  370,  371,  372,  373,  374,  375,  376,
  377,  378,   -1,   -1,  381,   -1,  264,  265,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  287,
  288,  289,  290,  291,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   36,   -1,   -1,   -1,   40,   -1,   42,   -1,   -1,
   -1,   46,   47,   -1,  312,   -1,   -1,  315,  316,  317,
  318,  319,  320,  321,  322,   -1,   -1,   -1,   -1,   64,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  264,  265,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  351,  352,  353,  354,  355,  356,   -1,
  358,   -1,  360,  287,  288,  289,  290,  291,  366,  367,
  368,  369,  370,  371,  372,  373,  374,  375,  376,  377,
  378,   -1,   -1,  381,   -1,   -1,   -1,   -1,  312,   -1,
   -1,  315,  316,  317,  318,  319,  320,  321,  322,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  264,  265,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  351,  352,  353,
  354,  355,  356,   -1,  358,   -1,  360,  287,  288,  289,
  290,  291,  366,  367,  368,  369,  370,  371,  372,  373,
  374,  375,  376,  377,  378,   -1,   -1,  381,   -1,   -1,
   -1,   -1,  312,   -1,   -1,  315,  316,  317,  318,  319,
  320,  321,  322,   -1,   -1,   -1,   -1,   -1,   -1,   36,
   -1,   -1,   -1,   40,   -1,   42,   -1,   -1,   -1,   46,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  351,  352,  353,  354,  355,  356,   64,  358,   -1,
  360,   -1,   -1,   -1,   -1,   -1,  366,  367,  368,  369,
  370,  371,  372,  373,  374,  375,  376,  377,  378,  264,
  265,  381,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  287,  288,  289,  290,  291,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  315,  316,  317,  318,  319,  320,  321,  322,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  351,  352,  353,  354,
  355,  356,   -1,  358,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  366,  367,  368,  369,  370,  371,  372,  373,  374,
  375,  376,  377,  378,   -1,   -1,  381,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  264,  265,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  287,  288,  289,  290,  291,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  315,  316,
  317,  318,  319,  320,  321,  322,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  351,  352,  353,  354,   -1,  356,
   -1,  358,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  366,
  367,  368,  369,  370,  371,  372,  373,  374,  375,  376,
  377,  378,   -1,   -1,  381,
  };

#line 2045 "XQuery.y"
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
  public const int PFOR = 272;
  public const int LET = 273;
  public const int WHERE = 274;
  public const int ASCENDING = 275;
  public const int DESCENDING = 276;
  public const int COLLATION = 277;
  public const int SOME = 278;
  public const int EVERY = 279;
  public const int SATISFIES = 280;
  public const int TYPESWITCH = 281;
  public const int CASE = 282;
  public const int DEFAULT = 283;
  public const int IF = 284;
  public const int THEN = 285;
  public const int ELSE = 286;
  public const int DOCUMENT = 287;
  public const int ELEMENT = 288;
  public const int ATTRIBUTE = 289;
  public const int TEXT = 290;
  public const int COMMENT = 291;
  public const int AND = 292;
  public const int OR = 293;
  public const int TO = 294;
  public const int DIV = 295;
  public const int IDIV = 296;
  public const int MOD = 297;
  public const int UNION = 298;
  public const int INTERSECT = 299;
  public const int EXCEPT = 300;
  public const int INSTANCE_OF = 301;
  public const int TREAT_AS = 302;
  public const int CASTABLE_AS = 303;
  public const int CAST_AS = 304;
  public const int EQ = 305;
  public const int NE = 306;
  public const int LT = 307;
  public const int GT = 308;
  public const int GE = 309;
  public const int LE = 310;
  public const int IS = 311;
  public const int VALIDATE = 312;
  public const int LAX = 313;
  public const int STRICT = 314;
  public const int NODE = 315;
  public const int DOUBLE_PERIOD = 316;
  public const int StringLiteral = 317;
  public const int IntegerLiteral = 318;
  public const int DecimalLiteral = 319;
  public const int DoubleLiteral = 320;
  public const int NCName = 321;
  public const int QName = 322;
  public const int VarName = 323;
  public const int PragmaContents = 324;
  public const int S = 325;
  public const int Char = 326;
  public const int PredefinedEntityRef = 327;
  public const int CharRef = 328;
  public const int XQUERY_VERSION = 329;
  public const int MODULE_NAMESPACE = 330;
  public const int IMPORT_SCHEMA = 331;
  public const int IMPORT_MODULE = 332;
  public const int DECLARE_NAMESPACE = 333;
  public const int DECLARE_BOUNDARY_SPACE = 334;
  public const int DECLARE_DEFAULT_ELEMENT = 335;
  public const int DECLARE_DEFAULT_FUNCTION = 336;
  public const int DECLARE_DEFAULT_ORDER = 337;
  public const int DECLARE_OPTION = 338;
  public const int DECLARE_ORDERING = 339;
  public const int DECLARE_COPY_NAMESPACES = 340;
  public const int DECLARE_DEFAULT_COLLATION = 341;
  public const int DECLARE_BASE_URI = 342;
  public const int DECLARE_VARIABLE = 343;
  public const int DECLARE_CONSTRUCTION = 344;
  public const int DECLARE_FUNCTION = 345;
  public const int EMPTY_GREATEST = 346;
  public const int EMPTY_LEAST = 347;
  public const int DEFAULT_ELEMENT = 348;
  public const int ORDER_BY = 349;
  public const int STABLE_ORDER_BY = 350;
  public const int PROCESSING_INSTRUCTION = 351;
  public const int DOCUMENT_NODE = 352;
  public const int SCHEMA_ELEMENT = 353;
  public const int SCHEMA_ATTRIBUTE = 354;
  public const int DOUBLE_SLASH = 355;
  public const int COMMENT_BEGIN = 356;
  public const int COMMENT_END = 357;
  public const int PI_BEGIN = 358;
  public const int PI_END = 359;
  public const int PRAGMA_BEGIN = 360;
  public const int PRAGMA_END = 361;
  public const int CDATA_BEGIN = 362;
  public const int CDATA_END = 363;
  public const int EMPTY_SEQUENCE = 364;
  public const int ITEM = 365;
  public const int AXIS_CHILD = 366;
  public const int AXIS_DESCENDANT = 367;
  public const int AXIS_ATTRIBUTE = 368;
  public const int AXIS_SELF = 369;
  public const int AXIS_DESCENDANT_OR_SELF = 370;
  public const int AXIS_FOLLOWING_SIBLING = 371;
  public const int AXIS_FOLLOWING = 372;
  public const int AXIS_PARENT = 373;
  public const int AXIS_ANCESTOR = 374;
  public const int AXIS_PRECEDING_SIBLING = 375;
  public const int AXIS_PRECEDING = 376;
  public const int AXIS_ANCESTOR_OR_SELF = 377;
  public const int AXIS_NAMESPACE = 378;
  public const int ML = 379;
  public const int Apos = 380;
  public const int BeginTag = 381;
  public const int Indicator1 = 382;
  public const int Indicator2 = 383;
  public const int Indicator3 = 384;
  public const int EscapeQuot = 385;
  public const int EscapeApos = 386;
  public const int XQComment = 387;
  public const int XQWhitespace = 388;
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
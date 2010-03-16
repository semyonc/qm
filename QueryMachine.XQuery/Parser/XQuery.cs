
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
#line 1493 "XQuery.y"
  { 
       yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirElemConstructor, yyVals[-7+yyTop], yyVals[-2+yyTop]);
       notation.Confirm((Symbol)yyVal, Descriptor.MappingExpr, yyVals[-4+yyTop]);
   }
  break;
case 280:
#line 1498 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirElemConstructor, yyVals[-7+yyTop], null);
       notation.Confirm((Symbol)yyVal, Descriptor.MappingExpr, yyVals[-4+yyTop]);
   }
  break;
case 281:
#line 1503 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirElemConstructor, 
		 yyVals[-11+yyTop], yyVals[-6+yyTop], null, yyVals[-3+yyTop], yyVals[-2+yyTop]); 
	   notation.Confirm((Symbol)yyVal, Descriptor.MappingExpr, yyVals[-8+yyTop]);	 
   }
  break;
case 282:
#line 1509 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirElemConstructor, 
		 yyVals[-11+yyTop], null, null, yyVals[-3+yyTop], yyVals[-2+yyTop]);
	   notation.Confirm((Symbol)yyVal, Descriptor.MappingExpr, yyVals[-8+yyTop]); 
   }
  break;
case 283:
#line 1515 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirElemConstructor, 
		 yyVals[-12+yyTop], yyVals[-7+yyTop], yyVals[-5+yyTop], yyVals[-2+yyTop], yyVals[-1+yyTop]);
	   notation.Confirm((Symbol)yyVal, Descriptor.MappingExpr, yyVals[-9+yyTop]);
   }
  break;
case 284:
#line 1521 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirElemConstructor, 
		 yyVals[-12+yyTop], null, yyVals[-5+yyTop], yyVals[-2+yyTop], yyVals[-1+yyTop]);
	   notation.Confirm((Symbol)yyVal, Descriptor.MappingExpr, yyVals[-9+yyTop]);	 
   }
  break;
case 285:
#line 1530 "XQuery.y"
  {
      yyVal = Lisp.Cons(yyVals[0+yyTop]);
   }
  break;
case 286:
#line 1534 "XQuery.y"
  {      
      yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(yyVals[0+yyTop]));
   }
  break;
case 287:
#line 1541 "XQuery.y"
  {
      yyVal = null;
   }
  break;
case 289:
#line 1549 "XQuery.y"
  {
      yyVal = Lisp.List(yyVals[-1+yyTop], yyVals[0+yyTop]);   
   }
  break;
case 290:
#line 1553 "XQuery.y"
  {
      yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(yyVals[0+yyTop]));
   }
  break;
case 291:
#line 1557 "XQuery.y"
  {
      yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.List(yyVals[-1+yyTop], yyVals[0+yyTop]));
   }
  break;
case 292:
#line 1564 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirAttribute,
		 yyVals[-5+yyTop], yyVals[-4+yyTop], yyVals[-2+yyTop], new Literal("\""), Lisp.Cons(new Literal("")));   
   }
  break;
case 293:
#line 1569 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirAttribute,
		 yyVals[-6+yyTop], yyVals[-5+yyTop], yyVals[-3+yyTop], new Literal("\""), yyVals[-1+yyTop]);
   }
  break;
case 294:
#line 1574 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirAttribute,
		 yyVals[-5+yyTop], yyVals[-4+yyTop], yyVals[-2+yyTop], new Literal("\'"), Lisp.Cons(new Literal("")));   
   }
  break;
case 295:
#line 1579 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirAttribute,
		 yyVals[-6+yyTop], yyVals[-5+yyTop], yyVals[-3+yyTop], new Literal("\'"), yyVals[-1+yyTop]);
   }
  break;
case 296:
#line 1587 "XQuery.y"
  {
      yyVal = Lisp.Cons(new TokenWrapper(Token.EscapeQuot));
   }
  break;
case 297:
#line 1591 "XQuery.y"
  {
      yyVal = Lisp.Cons(yyVals[0+yyTop]);
   }
  break;
case 298:
#line 1595 "XQuery.y"
  {
      yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(new TokenWrapper(Token.EscapeQuot)));
   }
  break;
case 299:
#line 1599 "XQuery.y"
  {
      yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(yyVals[0+yyTop]));
   }
  break;
case 300:
#line 1606 "XQuery.y"
  {
      yyVal = Lisp.Cons(new TokenWrapper(Token.EscapeApos));
   }
  break;
case 301:
#line 1610 "XQuery.y"
  {
      yyVal = Lisp.Cons(yyVals[0+yyTop]);
   }
  break;
case 302:
#line 1614 "XQuery.y"
  {
      yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(new TokenWrapper(Token.EscapeApos)));
   }
  break;
case 303:
#line 1618 "XQuery.y"
  {
      yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(yyVals[0+yyTop]));
   }
  break;
case 314:
#line 1644 "XQuery.y"
  {
      yyVal = new Literal("{");
   }
  break;
case 315:
#line 1648 "XQuery.y"
  {
      yyVal = new Literal("}");
   }
  break;
case 316:
#line 1652 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CommonContent), Descriptor.EnclosedExpr, yyVals[0+yyTop]); 
   }
  break;
case 317:
#line 1659 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirCommentConstructor, yyVals[-1+yyTop]);
   }
  break;
case 318:
#line 1666 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirPIConstructor, yyVals[-1+yyTop], null);
   }
  break;
case 319:
#line 1670 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Constructor), Descriptor.DirPIConstructor, yyVals[-3+yyTop], yyVals[-1+yyTop]);
   }
  break;
case 320:
#line 1677 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CData), Descriptor.CDataSection, yyVals[-1+yyTop]);
   }
  break;
case 327:
#line 1693 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompDocConstructor, yyVals[-1+yyTop]);
   }
  break;
case 328:
#line 1701 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompElemConstructor, yyVals[-3+yyTop], yyVals[-1+yyTop]);   
   }
  break;
case 329:
#line 1706 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompElemConstructor, yyVals[-2+yyTop], null);   
   }
  break;
case 330:
#line 1711 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompElemConstructor, yyVals[-4+yyTop], yyVals[-1+yyTop]);   
   }
  break;
case 331:
#line 1716 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompElemConstructor, yyVals[-3+yyTop], null);   
   }
  break;
case 333:
#line 1728 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompAttrConstructor, yyVals[-3+yyTop], yyVals[-1+yyTop]);   
   }
  break;
case 334:
#line 1733 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompAttrConstructor, yyVals[-2+yyTop], null);   
   }
  break;
case 335:
#line 1738 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompAttrConstructor, yyVals[-4+yyTop], yyVals[-1+yyTop]);   
   }
  break;
case 336:
#line 1743 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompAttrConstructor, yyVals[-3+yyTop], null);   
   }
  break;
case 337:
#line 1751 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompTextConstructor, yyVals[-1+yyTop]);   
   }
  break;
case 338:
#line 1759 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompCommentConstructor, yyVals[-1+yyTop]);   
   }
  break;
case 339:
#line 1767 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompPIConstructor, yyVals[-3+yyTop], yyVals[-1+yyTop]);   
   }
  break;
case 340:
#line 1772 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompPIConstructor, yyVals[-2+yyTop], null);   
   }
  break;
case 341:
#line 1777 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompPIConstructor, yyVals[-4+yyTop], yyVals[-1+yyTop]);   
   }
  break;
case 342:
#line 1782 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.CompConstructor), 
		Descriptor.CompPIConstructor, yyVals[-3+yyTop], null);   
   }
  break;
case 344:
#line 1791 "XQuery.y"
  {
      yyVal = yyVals[-1+yyTop];
      notation.Confirm((Symbol)yyVals[-1+yyTop], Descriptor.Occurrence, 
		new TokenWrapper(Token.Indicator3));
   }
  break;
case 345:
#line 1800 "XQuery.y"
  {
      yyVal = yyVals[0+yyTop];
   }
  break;
case 347:
#line 1808 "XQuery.y"
  {
      yyVal = yyVals[-1+yyTop];
      notation.Confirm((Symbol)yyVals[-1+yyTop], Descriptor.Occurrence, yyVals[0+yyTop]);
   }
  break;
case 348:
#line 1813 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.EMPTY_SEQUENCE);
   }
  break;
case 349:
#line 1820 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.Indicator1);
   }
  break;
case 350:
#line 1824 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.Indicator2);
   }
  break;
case 351:
#line 1828 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.Indicator3);
   }
  break;
case 354:
#line 1837 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.ITEM);
   }
  break;
case 356:
#line 1848 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.KindTest, yyVals[0+yyTop]);
   }
  break;
case 366:
#line 1866 "XQuery.y"
  {
       yyVal = new TokenWrapper(Token.NODE);
   }
  break;
case 367:
#line 1873 "XQuery.y"
  {
       yyVal = new TokenWrapper(Token.DOCUMENT_NODE);
   }
  break;
case 368:
#line 1877 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.DocumentNode, yyVals[-1+yyTop]);
   }
  break;
case 369:
#line 1881 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.DocumentNode, yyVals[-1+yyTop]);
   }
  break;
case 370:
#line 1888 "XQuery.y"
  {
       yyVal = new TokenWrapper(Token.TEXT);
   }
  break;
case 371:
#line 1895 "XQuery.y"
  {
      yyVal = new TokenWrapper(Token.COMMENT);
   }
  break;
case 372:
#line 1903 "XQuery.y"
  {
       yyVal = new TokenWrapper(Token.PROCESSING_INSTRUCTION);
   }
  break;
case 373:
#line 1907 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.ProcessingInstruction, yyVals[-1+yyTop]);
   }
  break;
case 374:
#line 1911 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.ProcessingInstruction, yyVals[-1+yyTop]);
   }
  break;
case 375:
#line 1918 "XQuery.y"
  {
       yyVal = new TokenWrapper(Token.ELEMENT);
   }
  break;
case 376:
#line 1922 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Element, yyVals[-1+yyTop]);
   }
  break;
case 377:
#line 1926 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Element, yyVals[-3+yyTop], yyVals[-1+yyTop]);
   }
  break;
case 378:
#line 1930 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Element, 
		yyVals[-4+yyTop], yyVals[-2+yyTop], new TokenWrapper('?'));
   }
  break;
case 380:
#line 1939 "XQuery.y"
  {
      yyVal = new TokenWrapper('*');
   }
  break;
case 381:
#line 1946 "XQuery.y"
  {
       yyVal = new TokenWrapper(Token.ATTRIBUTE);
   }
  break;
case 382:
#line 1950 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Attribute, yyVals[-1+yyTop]);
   }
  break;
case 383:
#line 1954 "XQuery.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Attribute, yyVals[-3+yyTop], yyVals[-1+yyTop]);
   }
  break;
case 385:
#line 1962 "XQuery.y"
  {
      yyVal = new TokenWrapper('*');
   }
  break;
case 386:
#line 1969 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.SchemaElement, yyVals[-1+yyTop]);
   }
  break;
case 387:
#line 1976 "XQuery.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.SchemaAttribute, yyVals[-1+yyTop]);
   }
  break;
case 391:
#line 1995 "XQuery.y"
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
  122,  122,  124,  124,  124,  124,  124,  124,  124,  124,
  124,  124,  124,  124,  128,  128,  127,  127,  130,  130,
  130,  131,  131,  131,  131,  132,  132,  132,  132,  133,
  133,  133,  133,  134,  134,  135,  135,  129,  129,  129,
  129,  137,  137,  137,  137,  137,  125,  126,  126,  140,
  123,  123,  123,  123,  123,  123,  141,  142,  142,  142,
  142,  147,  143,  143,  143,  143,  144,  145,  146,  146,
  146,  146,   84,   84,   37,   40,   40,   40,  150,  150,
  150,  149,  149,  149,  148,  107,  151,  151,  151,  151,
  151,  151,  151,  151,  151,  160,  152,  152,  152,  159,
  158,  157,  157,  157,  153,  153,  153,  153,  161,  161,
  154,  154,  154,  164,  164,  155,  156,  165,  163,  162,
   94,   94,  136,  138,  139,    8,
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
    1,    1,    5,    5,    9,    9,   10,   10,    9,    9,
   13,   13,   14,   14,    1,    2,    0,    1,    2,    2,
    3,    6,    7,    6,    7,    1,    1,    2,    2,    1,
    1,    2,    2,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    2,    2,    1,    3,    3,    5,    3,
    1,    1,    1,    1,    1,    1,    4,    5,    4,    7,
    6,    1,    5,    4,    7,    6,    4,    4,    5,    4,
    7,    6,    1,    2,    2,    1,    2,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    3,    3,    4,    4,    3,
    3,    3,    4,    4,    3,    4,    6,    7,    1,    1,
    3,    4,    6,    1,    1,    4,    4,    1,    1,    1,
    0,    1,    1,    1,    1,    1,
  };
   static  short [] yyDefRed = {            0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    2,
    4,    0,    0,    0,    0,    0,    0,   18,   19,   20,
   21,   22,   23,   24,   25,   26,   27,   28,   29,   30,
   31,   32,   33,    0,    0,    0,    0,    0,   55,    0,
  396,    0,    0,   36,   37,    0,    0,   43,   44,    0,
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
  272,  321,  322,  323,  324,  325,  326,  356,  357,  358,
  359,  360,  361,  362,  363,  364,  365,   15,   17,    0,
    0,   58,    0,    0,   56,    0,   35,   48,   49,   45,
    0,    0,   67,    0,    0,    0,   76,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,   83,    0,   89,
  108,    0,    0,  111,    0,    0,    0,    0,  141,  175,
  177,  187,  179,  188,  143,  144,  145,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  160,  161,    0,    0,
    0,    0,  355,    0,  348,  354,  163,  353,  352,    0,
  165,  167,    0,  169,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  194,  195,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  392,    0,    0,  258,  259,    0,    0,
  235,  223,    0,    0,  198,    0,    0,    0,    0,  242,
    0,  213,  225,    0,    6,    9,    0,    0,    0,  345,
   65,    0,    0,    0,    0,    0,   99,    0,  106,    0,
  126,  129,  127,    0,    0,  122,  123,    0,  124,  125,
  114,    0,    0,    0,   90,    0,   91,  349,  350,  351,
  347,  344,    0,    0,    0,    0,  389,  375,  380,    0,
  379,    0,    0,  388,  381,  385,    0,  384,    0,  370,
    0,  371,    0,    0,    0,  366,  238,  264,  266,    0,
    0,    0,    0,  372,    0,  367,    0,    0,    0,    0,
  317,    0,  318,    0,    0,    0,    0,    0,  260,  239,
    0,  206,  205,    0,  243,   61,   57,    0,   79,   73,
    0,    0,   70,   72,   77,    0,    0,  102,    0,    0,
    0,    0,  133,    0,  116,    0,    0,    0,  112,   92,
  262,  263,  327,  329,    0,    0,    0,  376,    0,  334,
    0,    0,  382,    0,  337,  338,  192,    0,    0,  265,
  340,    0,  374,  373,    0,  368,  369,  386,  387,    0,
    0,    0,    0,    0,  289,    0,    0,    0,    0,  196,
  244,   64,    0,    0,    0,    0,    0,  130,    0,    0,
    0,  134,    0,  118,    0,  121,  328,  390,    0,    0,
  333,    0,    0,  193,  267,  339,    0,  319,  199,    0,
  395,  312,  313,    0,    0,    0,    0,  316,  308,    0,
  285,  311,  309,  310,  274,    0,    0,    0,  273,  291,
   71,   80,  103,  100,  107,    0,    0,    0,    0,    0,
  119,  377,    0,  331,    0,  383,  336,    0,  342,    0,
    0,    0,  314,  315,    0,    0,  286,    0,    0,    0,
    0,  136,  131,    0,  137,  378,  330,  335,  341,    0,
  320,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  132,  394,  294,  300,    0,  301,  307,  306,  393,  296,
  292,    0,  297,  304,  305,  276,    0,    0,    0,  280,
    0,    0,  279,  275,    0,  135,  295,  302,  303,  298,
  293,  299,  278,    0,    0,    0,    0,  277,    0,    0,
    0,    0,    0,    0,    0,    0,  282,    0,  281,    0,
  284,  283,
  };
  protected static  short [] yyDgoto  = {            18,
   19,   20,   21,  111,   22,   82,   23,  285,   24,   25,
   26,   27,   28,   29,   30,   31,   32,   33,   34,   35,
   36,   37,   38,   39,   40,   41,   42,   43,   65,  290,
   48,  286,   49,  121,  292,   83,  293,  295,  503,  347,
  598,  296,  297,  525,   85,   86,   87,   88,   89,   90,
  149,  150,   91,   92,  130,  131,  507,  508,  133,  134,
  313,  314,  431,  432,  433,  136,  137,  512,  513,   93,
   94,   95,  165,  166,  167,   96,   97,   98,   99,  100,
  101,  102,  103,  352,  104,  105,  227,  228,  229,  230,
  371,  231,  232,  385,  233,  234,  235,  236,  237,  399,
  238,  239,  240,  241,  242,  243,  244,  245,  246,  247,
  400,  248,  249,  250,  251,  252,  253,  254,  255,  256,
  470,  599,  258,  259,  260,  261,  487,  600,  601,  488,
  555,  682,  675,  683,  676,  684,  602,  678,  603,  604,
  262,  263,  264,  265,  266,  267,  526,  349,  350,  441,
  268,  269,  270,  271,  272,  273,  274,  275,  276,  277,
  450,  579,  451,  457,  458,
  };
  protected static  short [] yySindex = {         1108,
 -219, -171, -205, -194, -145, -175,  -56,  -51, -128, -206,
   18,   94,  -82,  -82,  214,  -73,  -83,    0, 1494,    0,
    0,  920, 1577, 1577, -129,  193,  193,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   -8,  195,  -54,   12,  -82,    0,  -40,
    0,   20,  228,    0,    0,  -82,  -82,    0,    0,   -9,
    0,    0,    0,    0,  252,    0,    0,  -31,    0,    0,
  259,    0,    0,  265,  292,  296,  296,  302,  309,   86,
   86,    0,    0,  312,    0,    0,    0,    0,   70, -138,
    0,    0,   90,    0,  130,  -20, -234,  -94,  -36,    0,
   77,   89,   96,   93, 4886,    0, -129,  193,  193,    0,
    0,    0,   76,    0,  -82,  340,    0,  136,  344,  -82,
    0,  -82,    0,    0,    0,  107,  139,  373,  104,  371,
    0,  110,  377,    0,  120,  -26,    0,  -12,  920,  920,
    0,    0,  920,   86,  920,  920,  920,  920, -160,  147,
    0,    0,   86,    0,    0,    0,    0,    0,    0,    0,
    0,  391,  313,  358,   86,   86,   86,   86,   86,   86,
   86,   86,   86,   86,   86,   86,   86,   86, 1351, 1351,
  135,  135,  339,  345,  362,  -16,    1,   24,   27,  -81,
  447,    0,    0,    0,    0,    0,  432,  465,    7,  481,
  482,  483, 5292,  212,  213,  208,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  205,
  219,  247, 5292,  425,  489,    0,    0,    0,    0,    0,
 -111,    0,  -25,    0,    0,    0,  448,  448,  425,    0,
    0,  425,    0,    0,    0,    0,  448,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  193,
  193,    0,  -82,  -82,    0,  518,    0,    0,    0,    0,
 1351,  -21,    0,  242,  525,  523,    0,  139,  265,  139,
  292,  139,  920,  296,  920,   55,   95,    0,   90,    0,
    0,  119,  524,    0,  524,  920,  299,  920,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  199, -234, -234,
  -94,  -94,  -94,  -94,  -36,  -36,    0,    0,  530,  534,
  535,  537,    0,  538,    0,    0,    0,    0,    0,    2,
    0,    0,  516,    0,  920,  920,  920,  457,  -15,  920,
  458,   -6,  920,  545,  920,  546,  920,    0,    0,  920,
  474,  547,  552,  471,  476,    5,  920,  -28,  279,  281,
  -25,  253, -253,    0,  282,  286,    0,    0,   99,  -25,
    0,    0,  291,  920,    0, 5292, 5292,  920,  448,    0,
  448,    0,    0,  448,    0,    0,  518,   20,  -82,    0,
    0,  553,  139,  -64,  373,  346,    0,  558,    0,  348,
    0,    0,    0,  337,  335,    0,    0,  -82,    0,    0,
    0, -148,  349,  920,    0,  920,    0,    0,    0,    0,
    0,    0,   26,   28,   29,  841,    0,    0,    0,  192,
    0,   31,  890,    0,    0,    0,  202,    0,   32,    0,
   35,    0,   36,   37,  920,    0,    0,    0,    0,  210,
 1029,  579,  583,    0,   42,    0,  585,  587,  590,  599,
    0,  306,    0,  319,  -19,  554,  152,  323,    0,    0,
   43,    0,    0,   21,    0,    0,    0,  920,    0,    0,
 1351,  920,    0,    0,    0,  607,  379,    0,  588,  920,
  540,  142,    0,  920,    0,  -82,  374,  -82,    0,    0,
    0,    0,    0,    0,  312,  526,  331,    0,  531,    0,
   44,  331,    0,  532,    0,    0,    0,   45,  920,    0,
    0,   46,    0,    0,  536,    0,    0,    0,    0,  295,
  297,  208,  -57,  598,    0, 5178,  -41,  600,  342,    0,
    0,    0,  -85,   47,  336,  920,  920,    0,  343,  396,
   -5,    0,  376,    0,  -82,    0,    0,    0,   62, 1046,
    0,  626, 1106,    0,    0,    0, 1133,    0,    0,  613,
    0,    0,    0,  360, 1059,  555,  631,    0,    0,   53,
    0,    0,    0,    0,    0,  586,  634,   61,    0,    0,
    0,    0,    0,    0,    0,  414,  920,  920,  375,  920,
    0,    0,  658,    0,  575,    0,    0,   48,    0,   49,
  208,  341,    0,    0,  380,  655,    0,  381,  387,  669,
 1351,    0,    0,  450,    0,    0,    0,    0,    0,  -29,
    0,  208,  401,  -18,  154,  208,  404,  461,  920,  132,
  -17,  665,  208,  266,  675,  581,  676,  678,  208,  920,
    0,    0,    0,    0,  605,    0,    0,    0,    0,    0,
    0,  -14,    0,    0,    0,    0,  679,  697,  584,    0,
  703,  596,    0,    0,  690,    0,    0,    0,    0,    0,
    0,    0,    0,  434,  706,  435,  711,    0,  208,  438,
  208,  449,  707,  208,  709,  208,    0,  710,    0,  712,
    0,    0,
  };
  protected static  short [] yyRindex = {         1110,
    0,    0,  452,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0, 1110,    0,
    0, 5000,  773,  183,  318,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  720,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0, 5000,
 5000,    0,    0,  781,    0,    0,    0,    0, 2508,    0,
    0,    0, 1813,    0,  571, 4599, 4342, 3812, 3446,    0,
 3400, 3122, 3013, 2647,    0,    0,  446,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  723,    0,    0,
    0,    0,    0,    0,    0,    0,   -2,  742,    0,  -77,
    0,    0,  236,    0,    0,    0,    0,    0, 5000, 5000,
    0,    0, 5000, 5000, 5000, 5000, 5000, 5000,    0,    0,
    0,    0, 5000,    0,    0,    0,    0,    0,    0,    0,
    0,    0, 4738, 4812, 5000, 5000, 5000, 5000, 5000, 5000,
 5000, 5000, 5000, 5000, 5000, 5000, 5000, 5000,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  747,    0,    0,
    0,    0,    0,    0,    0,  463,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0, 5000, 2063,    0,  835,    0,    0,    0,    0,    0,
    0,    0, 2172,    0,    0,    0,  944, 1222,    0,    0,
    0,    0,    0,    0,    0,    0, 1310,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  726,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  745,    0,   10,    0,  729,
    0,  520, 5000,    0, 5000,    0,    0,    0, 2110,    0,
    0,  -10,  561,    0,  562, 5000,    0, 5000,    0,    0,
    0,    0,    0,    0,    0,    0,    0, 4689, 4393, 4516,
 3858, 4002, 4136, 4259, 3534, 3724,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0, 1500,
    0,    0, 2925,    0, 5000, 5000, 5000,    0,    0, 5000,
    0,    0, 5000,    0, 5000,    0, 5000,    0,    0, 5000,
    0,    0,    0, 5000,    0,    0, 5000,    0,    0,    0,
 2450,    0,    0,    0,    0,  118,    0,    0,    0, 2538,
    0,    0,    0, 5000,    0,    0,    0, 5000, 1588,    0,
 1697,    0,    0, 1975,    0,    0,  734,  720,    0,    0,
    0,    0,  217,    0,    0,  533,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    4,    6, 5000,    0, 5000,    0,    0,    0,    0,
    0,    0,    0,    0,    0, 5000,    0,    0,    0,    0,
    0,    0, 5000,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0, 5000,    0,    0,    0,    0,    0,
 5000,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  704,    0,    0,  158,    0,    0,
    0,    0,    0,    0,    0,    0,    0, 5000,    0,    0,
    0, 5000,    0,    0,    0,    0,    0,    0,    0, 5000,
    0,    0,    0, 5000,    0,    0,    8,    0,    0,    0,
    0,    0,    0,    0,  685,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0, 5000,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  743,    0,    0,    0,    0,    0,    0,   -7,    0,
    0,    0,    0,    0,    0, 5000, 5000,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0, 5000,
    0,    0, 5000,    0,    0,    0, 5000,    0,    0,    0,
    0,    0,    0,    0, 5000,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0, 5000, 5000,    0, 5000,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  -13,    0,    0,    0,    0,    0,    0,  159,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  762,    0,    0,    0,  762,    0,    0, 5000,    0,
    0,    0,  762,    0,    0,    0,    0,    0,  762, 5000,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  762,    0,
  762,    0,    0,  762,    0,  762,    0,    0,    0,    0,
    0,    0,
  };
  protected static  short [] yyGindex = {            0,
    0,  806,  814,   50,  811,    0,    0,   -3,    0,  812,
  813,   38,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  556,    0,  440,  -61, -139,  436,    0,  288, -164,
 -381,    0,  442,  -22,    0,    0,    0,    0,    0,    0,
    0,  715,  765,  768,    0,  560,    0,    0,    0,  559,
  717,  433,    0,    0,  437,  789,  566,    0,  361,  730,
  722,  246,    0,    0,    0,  713,  260,  372,  269,  273,
    0,    0,    0,  695,    0,  394,    0,    0,  327,    0,
    0,    0,  654, -312, -125,   87,    0,    0,    0, -143,
    0,    0,  -87,    0,    0,    0, -165,    0,    0,    0,
 -177,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0, -103,    0,    0,    0,    0,  249, -518, -551,    0,
  329,    0,    0,  207,  223,    0, -559,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  320,  314,    0,    0,
    0,    0,  521,    0,  527,    0,    0,    0,    0,    0,
    0,  369,  539,    0,  522,
  };
  protected static  short [] yyTable = {            84,
   52,  257,  597,  308,  661,  310,  311,  312,  312,   66,
   67,  394,  476,  348,  348,  351,  681,  304,  607,  701,
  391,  397,  169,  359,  170,  448,  449,  554,  665,  176,
  619,  304,  504,  113,  455,  456,  412,  502,  608,  290,
  362,  370,  553,  664,  118,  474,  376,  115,  637,  120,
  110,  117,  123,  124,  290,   66,  637,   46,  502,  171,
  172,  173,  109,  364,  143,  595,  366,  596,   50,  143,
  482,  143,  143,  486,  143,  143,  112,  381,  143,  143,
  143,  595,   54,  596,   55,  143,  143,  143,  143,  143,
  143,  143,  143,  114,  401,  424,   44,  390,  143,  257,
  677,  685,  622,  404,  483,  595,  360,  596,  595,  316,
  596,  281,  636,  561,   60,  677,  306,  307,  287,  257,
  640,   51,  685,  363,  623,  348,  410,  516,   80,  377,
   81,  145,   74,   75,  146,  425,  392,  637,  143,  489,
  637,   47,  143,  174,  109,  689,  365,  692,   45,  367,
  521,  402,  522,  523,  403,  529,  534,  278,  279,  535,
  536,  537,  162,  421,  287,  423,  545,  560,  581,  584,
  586,  612,  648,  649,   53,  595,  435,  596,  437,  287,
  500,  504,   11,  595,   69,  596,   70,  147,  148,  163,
  161,  164,   97,   97,   97,   97,  429,  430,  558,  389,
  667,  500,  175,  501,  288,  287,   56,   10,  391,  147,
  148,   57,   15,  557,   17,  666,   58,   59,   11,  288,
  287,  495,   11,  495,   11,   11,  495,   11,   11,   11,
  368,  369,  528,   51,  469,  527,  416,   71,  418,  590,
  420,  169,  533,  170,  411,  532,   11,  206,  113,   68,
  540,  110,  303,  539,  595,  115,  596,   78,  339,  113,
   78,  177,  178,   66,  618,  116,  305,  591,  592,  593,
   97,   97,  168,  115,  117,  120,   66,  117,   66,  119,
  408,   61,   62,  591,  592,  593,  120,  388,  122,   80,
  127,   81,  257,  257,  312,  126,  520,  204,  128,  205,
  129,  552,  552,  594,  358,  447,  125,  679,  592,  593,
  679,  592,  593,  204,  454,  205,  290,   12,  650,  594,
  472,  361,  220,  201,  473,  688,  375,  132,  396,  405,
  406,  135,  443,  444,  445,  348,  563,  452,  220,  662,
  459,  139,  461,  668,  463,  348,  570,  464,  140,  660,
  687,   63,   64,   12,  475,  143,  695,   12,  562,   12,
   12,  144,   12,   12,   12,  391,  680,  288,  289,  700,
  568,  491,  322,  321,  573,  494,  179,  591,  592,  593,
  153,   12,  438,  439,  440,  591,  592,  593,  595,  180,
  596,  280,  426,  427,  428,  182,  713,  181,  715,  585,
  282,  718,  283,  720,  284,  497,  291,  204,  294,  205,
  325,  326,  327,  594,  299,  204,  318,  205,  323,  324,
  301,  594,  511,  571,  515,  298,  614,  615,  329,  330,
  531,  300,  220,  154,  155,  156,  157,  158,  159,  160,
  220,  302,  538,  335,  336,   13,   11,   11,  542,  337,
  338,  320,  257,   11,   11,  343,  672,  592,  593,   11,
   11,  355,   11,  429,  430,   11,  225,  356,   11,   11,
   11,   11,   11,  141,  142,  348,  658,  642,  643,  564,
  645,   13,  492,  493,  357,   13,  372,   13,   13,  373,
   13,   13,   13,   11,  353,  353,   11,   11,   11,   11,
   11,   11,   11,   11,  374,  104,  104,  104,  104,   13,
  673,  468,  574,   80,  576,   81,  674,   74,   75,  671,
  378,  379,  380,   76,   77,  386,   78,  382,  383,   79,
  696,  384,   11,   11,   11,   11,   11,   11,  398,   11,
  387,   11,  331,  332,  333,  334,  393,   11,   11,   11,
   11,   11,   11,   11,   11,   11,   11,   11,   11,   11,
  628,  409,   11,  413,  630,  414,  415,  434,  436,  359,
  142,  621,  564,  362,  364,  569,  366,  376,  442,  446,
  453,   12,   12,  104,  104,  460,  462,  466,   12,   12,
  591,  592,  593,  467,   12,   12,  465,   12,  471,  447,
   12,  454,  484,   12,   12,   12,   12,   12,  481,  485,
  490,  142,  506,  498,  142,  509,  510,  511,  514,  543,
  204,  550,  205,  544,  518,  546,  594,  547,   12,  142,
  548,   12,   12,   12,   12,   12,   12,   12,   12,  549,
  691,  551,  565,  705,  556,  220,  559,  566,  567,  575,
  577,  578,  588,  580,  583,  707,  589,  613,  587,  605,
  620,  609,  552,  142,  616,  617,  626,   12,   12,   12,
   12,   12,   12,  631,   12,  632,   12,  635,  638,  634,
  639,  641,   12,   12,   12,   12,   12,   12,   12,   12,
   12,   12,   12,   12,   12,  142,  644,   12,  646,  647,
  652,  653,  651,  595,  654,  596,  595,  656,  596,   13,
   13,  339,  340,  341,  342,  657,   13,   13,  595,  659,
  596,  663,   13,   13,  669,   13,  686,  595,   13,  596,
  670,   13,   13,   13,   13,   13,  690,  693,  191,  694,
  703,   74,   75,  704,  197,  391,  235,   76,   77,  706,
   78,  708,  710,   79,  709,  711,   13,  712,  714,   13,
   13,   13,   13,   13,   13,   13,   13,   54,  717,  716,
  719,  721,   10,  722,  344,  200,  201,  202,   62,  235,
   81,   52,   74,  391,   63,   75,   66,  235,   66,  235,
  235,  235,   53,  235,  392,   13,   13,   13,   13,   13,
   13,  101,   13,  391,   13,  235,  235,  235,  235,  332,
   13,   13,   13,   13,   13,   13,   13,   13,   13,   13,
   13,   13,   13,  391,   72,   13,  339,  340,  341,  342,
  109,  110,   73,  106,  237,  107,  108,  235,  407,  235,
  142,  142,  142,  142,  142,  142,  142,  496,  499,  142,
  611,  142,  142,  191,  151,  142,  505,  152,  417,  419,
  343,  142,  142,  317,  315,  138,  519,  237,  517,  422,
  235,  235,  572,  309,  319,  237,  354,  237,  237,  237,
  328,  237,  606,   80,  395,   81,  655,  610,  702,  344,
  200,  201,  202,  237,  237,  237,  237,  699,  477,  625,
  582,  480,  345,  346,  478,  591,  592,  593,  591,  592,
  593,    0,    0,    0,    0,  142,  142,  479,  142,  142,
  591,  592,  593,    0,    0,  237,    0,  237,    0,  672,
  592,  593,   80,    0,   81,  204,    0,  205,  204,    0,
  205,  594,    0,  209,  594,    0,    0,    0,    0,    0,
  204,    0,  205,    0,    0,    0,  594,    0,  237,  237,
  220,    0,   80,  220,   81,  524,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  220,  209,    0,    0,    0,
    0,    0,    0,  697,  209,    0,  209,  209,  209,  698,
  209,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  209,  209,  209,  209,    0,    0,    0,    0,
    0,    0,    0,    0,  530,    0,  235,  235,  235,  235,
  235,  235,  235,    0,    0,  235,    0,  235,  235,    0,
    0,  235,    0,    0,    0,    0,  209,  235,  235,  235,
  235,  235,  235,  235,  235,  235,  235,  235,  235,  235,
  235,  235,  235,  235,  235,  235,  235,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  209,  209,    0,
    0,   80,    0,   81,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   80,    0,
   81,  235,  235,    0,  235,  235,    0,    0,    0,    0,
  235,   80,    0,   81,  237,  237,  237,  237,  237,  237,
  237,   74,   75,  237,    0,  237,  237,   76,   77,  237,
   78,    0,    0,   79,  235,  237,  237,  237,  237,  237,
  237,  237,  237,  237,  237,  237,  237,  237,  237,  237,
  237,  237,  237,  237,  237,   10,    0,    0,   80,   10,
   81,   10,   10,  541,   10,   10,   10,    0,    0,    0,
   74,   75,    0,    0,    0,    0,   76,   77,    0,   78,
  624,    0,   79,   10,    0,   80,    0,   81,    0,  237,
  237,  633,  237,  237,    0,    0,    0,    0,  237,    0,
   74,   75,    0,    0,    0,    0,   76,   77,    0,   78,
    0,    0,   79,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  237,  209,  209,  209,  209,  209,  209,  209,
    0,  211,  209,    0,  209,  209,    0,    0,  209,    0,
  627,    0,    0,    0,  209,  209,  209,  209,  209,  209,
  209,  209,  209,  209,  209,  209,  209,  209,  209,  209,
  209,  209,  209,  209,  211,    0,    0,  629,    0,    0,
    0,    0,  211,    0,  211,  211,  211,    0,  211,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  211,  211,  211,  211,    0,    0,    0,    0,  209,  209,
    0,  209,  209,    0,    0,    0,    0,  209,    0,   74,
   75,    0,    0,    0,    0,   76,   77,    0,   78,  240,
    0,   79,    0,    0,  211,    0,   74,   75,    0,    0,
    0,  209,   76,   77,    0,   78,    0,    0,   79,   74,
   75,    0,    0,    0,    0,   76,   77,    0,   78,    0,
    0,   79,  240,    0,    0,  211,  211,    0,    0,    0,
  240,    0,  240,  240,  240,    0,  240,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  240,  240,
  240,  240,    0,   10,   10,    0,   74,   75,    0,    0,
   10,   10,   76,   77,    0,   78,   10,   10,   79,   10,
    0,    0,   10,    0,    0,   10,   10,   10,   10,   10,
    0,    0,  240,   74,   75,    0,    0,    0,    0,   76,
   77,    0,   78,    0,    0,   79,    0,    0,    0,    0,
   10,    0,    0,   10,   10,   10,   10,   10,   10,   10,
   10,    0,    0,  240,  240,    1,    2,    3,    4,    5,
    6,    7,    8,    9,   10,   11,   12,   13,   14,   15,
   16,   17,    0,    0,    0,    0,    0,    0,    0,   10,
   10,   10,   10,   10,   10,    0,   10,    0,   10,    0,
    0,    0,    0,    0,   10,   10,   10,   10,   10,   10,
   10,   10,   10,   10,   10,   10,   10,    0,    0,   10,
    0,  211,  211,  211,  211,  211,  211,  211,    0,  346,
  211,    0,  211,  211,    0,    0,  211,    0,    0,    0,
    0,    0,  211,  211,  211,  211,  211,  211,  211,  211,
  211,  211,  211,  211,  211,  211,  211,  211,  211,  211,
  211,  211,  346,    0,    0,    0,    0,    0,    0,    0,
  346,    0,  346,  346,  346,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  346,  346,  346,
  346,  346,    0,    0,    0,    0,  211,  211,    0,  211,
  211,    0,    0,    0,    0,  211,    0,    0,    0,  240,
  240,  240,  240,  240,  240,  240,    0,  210,  240,    0,
  240,  240,  346,    0,  240,    0,    0,    0,    0,  211,
  240,  240,  240,  240,  240,  240,  240,  240,  240,  240,
  240,  240,  240,  240,  240,  240,  240,  240,  240,  240,
  210,    0,  346,  346,  346,    0,    0,    0,  210,    0,
  210,  210,  210,    0,  210,    0,    0,  339,  340,  341,
  342,    0,    0,    0,    0,    0,  210,  210,  210,  210,
    0,    0,    0,    0,  240,  240,    0,  240,  240,    0,
    0,    0,    0,  240,  191,    0,    0,    0,    0,    0,
    0,  343,    0,    0,    0,    0,    0,    0,    0,    0,
  210,    0,    0,    0,    0,    0,    0,  240,    0,    0,
    0,    0,    0,    0,    0,    0,  212,    0,    0,    0,
  344,  200,  201,  202,    0,    0,    0,    0,    0,    0,
    0,  210,  210,  345,  346,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  212,
    0,    0,    0,    0,    0,    0,    0,  212,    0,  212,
  212,  212,    0,  212,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  212,  212,  212,  212,    0,
    0,    0,    0,    0,    0,  346,  346,    0,  346,  346,
  346,  346,  346,  346,  346,  346,    0,    0,  346,    0,
  346,  346,    0,    0,  346,    0,    0,    0,    0,  212,
  346,  346,  346,  346,  346,  346,  346,  346,  346,  346,
    0,    0,    0,  346,  346,  346,  346,  346,  346,  346,
    0,    0,  138,    0,    0,    0,    0,    0,    0,    0,
  212,  212,    2,    3,    4,    5,    6,    7,    8,    9,
   10,   11,   12,   13,   14,   15,   16,   17,    0,    0,
    0,    0,    0,    0,  346,  346,    0,  346,  346,    0,
    0,    0,    0,  138,    0,    0,  138,  210,  210,  210,
  210,  210,  210,  210,    0,    0,  210,    0,  210,  210,
    0,  138,  210,    0,    0,    0,    0,  346,  210,  210,
  210,  210,  210,  210,  210,  210,  210,  210,  210,  210,
  210,  210,  210,  210,  210,  210,  210,  210,    0,    0,
    0,    0,    0,    0,    0,  138,    3,    4,    5,    6,
    7,    8,    9,   10,   11,   12,   13,   14,   15,   16,
   17,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  210,  210,    0,  210,  210,  138,    0,    0,
    0,  210,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  210,  212,  212,  212,  212,
  212,  212,  212,    0,  241,  212,    0,  212,  212,    0,
    0,  212,    0,    0,    0,    0,    0,  212,  212,  212,
  212,  212,  212,  212,  212,  212,  212,  212,  212,  212,
  212,  212,  212,  212,  212,  212,  212,  241,    0,    0,
    0,    0,    0,    0,    0,  241,    0,  241,  241,  241,
    0,  241,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  241,  241,  241,  241,    0,    0,    0,
    0,  212,  212,    0,  212,  212,    0,    0,    0,    0,
  212,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  200,    0,    0,    0,    0,  241,    0,    0,
    0,    0,    0,    0,  212,    0,    0,    0,    0,    0,
    0,    0,  138,  138,  138,  138,  138,  138,  138,    0,
    0,  138,    0,  138,  138,  200,    0,  138,  241,  241,
    0,    0,    0,  200,  138,  200,  200,  200,    0,  139,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  200,  200,  200,  200,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  139,    0,    0,  139,    0,  200,    0,  138,  138,    0,
  138,  138,    0,    0,    0,    0,    0,    0,  139,    0,
    0,  203,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  200,  200,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  139,    0,  203,    0,    0,    0,    0,    0,
    0,    0,  203,    0,  203,  203,  203,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  203,  203,  203,  203,  139,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  241,  241,  241,  241,  241,  241,
  241,    0,    0,  241,    0,  241,  241,    0,    0,  241,
    0,    0,    0,    0,  203,  241,  241,  241,  241,  241,
  241,  241,  241,  241,  241,  241,  241,  241,  241,  241,
  241,  241,  241,  241,  241,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  203,  203,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  241,
  241,    0,  241,  241,    0,    0,    0,    0,  241,    0,
    0,    0,  200,  200,  200,  200,  200,  200,  200,    0,
    0,  200,    0,  200,  200,    0,    0,  200,    0,    0,
    0,    0,  241,  200,  200,  200,  200,  200,  200,  200,
  200,  200,  200,  200,  200,  200,  200,  200,  200,  200,
  200,  200,  200,    0,    0,    0,    0,    0,    0,  139,
  139,  139,  139,  139,  139,  139,    0,    0,  139,    0,
  139,  139,    0,    0,  139,    0,    0,    0,    0,    0,
    0,  139,    0,    0,    0,    0,    0,  200,  200,    0,
  200,  200,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  200,  203,  203,  203,  203,  203,  203,  203,    0,  202,
  203,    0,  203,  203,  139,  139,  203,  139,  139,    0,
    0,    0,  203,  203,  203,  203,  203,  203,  203,  203,
  203,  203,  203,  203,  203,  203,  203,  203,  203,  203,
  203,  203,  202,    0,    0,    0,    0,    0,    0,    0,
  202,    0,  202,  202,  202,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,   88,  202,  202,
  202,  202,    0,    0,    0,    0,  203,  203,    0,  203,
  203,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  201,    0,    0,
    0,    0,  202,    0,    0,    0,    0,    0,   88,  203,
    0,   88,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,   88,    0,    0,    0,
  201,    0,    0,  202,  202,    0,    0,    0,  201,    0,
  201,  201,  201,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  201,  201,  201,  201,
   88,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  201,    0,   88,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  168,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  201,  201,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  168,
    0,    0,    0,    0,    0,    0,    0,  168,    0,  168,
  168,  168,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  168,  168,  168,  168,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  202,
  202,  202,  202,  202,  202,  202,    0,    0,  202,    0,
  202,  202,    0,    0,  202,    0,    0,    0,    0,  168,
  202,  202,  202,  202,  202,  202,  202,  202,  202,  202,
  202,  202,  202,  202,  202,  202,  202,  202,  202,  202,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  168,  168,    0,    0,    0,    0,    0,   88,   88,   88,
   88,   88,   88,   88,    0,    0,   88,    0,   88,   88,
    0,    0,   88,    0,  202,  202,    0,  202,  202,    0,
    0,    0,    0,    0,    0,    0,    0,  201,  201,  201,
  201,  201,  201,  201,    0,    0,  201,    0,  201,  201,
    0,    0,  201,    0,    0,    0,    0,  202,  201,  201,
  201,  201,  201,  201,  201,  201,  201,  201,  201,  201,
  201,  201,  201,  201,  201,  201,  201,  201,    0,    0,
    0,    0,   88,   88,    0,   88,   88,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  201,  201,    0,  201,  201,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  201,  168,  168,  168,  168,
  168,  168,  168,    0,  343,  168,    0,  168,  168,    0,
    0,  168,    0,    0,    0,    0,    0,  168,  168,  168,
  168,  168,  168,  168,  168,  168,  168,  168,  168,    0,
  168,  168,  168,  168,  168,  168,  168,  343,    0,    0,
    0,    0,    0,    0,    0,  343,    0,  343,  343,  343,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  343,  343,  343,  343,    0,    0,    0,
    0,  168,  168,    0,  168,  168,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  166,    0,    0,    0,    0,  343,    0,    0,
    0,    0,    0,    0,  168,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  166,    0,    0,  343,  343,
    0,    0,    0,  166,    0,  166,  166,  166,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  166,  166,  166,  166,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  166,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  164,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  166,  166,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  164,    0,    0,    0,    0,    0,
    0,    0,  164,    0,  164,  164,  164,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  164,  164,  164,  164,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  343,  343,  343,  343,  343,  343,
  343,    0,    0,  343,    0,  343,  343,    0,    0,  343,
    0,    0,    0,    0,  164,  343,  343,  343,  343,  343,
  343,  343,  343,  343,  343,  343,  343,    0,  343,  343,
  343,  343,  343,  343,  343,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  164,  164,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  343,
  343,    0,  343,  343,    0,    0,    0,    0,    0,    0,
    0,    0,  166,  166,  166,  166,  166,  166,  166,    0,
    0,  166,    0,  166,  166,    0,    0,  166,    0,    0,
    0,    0,  343,  166,  166,  166,  166,  166,  166,  166,
  166,  166,  166,  166,    0,    0,  166,  166,  166,  166,
  166,  166,  166,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  166,  166,    0,
  166,  166,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  166,  164,  164,  164,  164,  164,  164,  164,    0,  162,
  164,    0,  164,  164,    0,    0,  164,    0,    0,    0,
    0,    0,  164,  164,  164,  164,  164,  164,  164,  164,
  164,  164,    0,    0,    0,  164,  164,  164,  164,  164,
  164,  164,  162,    0,    0,    0,    0,    0,    0,    0,
  162,    0,  162,  162,  162,  156,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  162,  162,
  162,  162,    0,    0,    0,    0,  164,  164,    0,  164,
  164,    0,    0,    0,    0,    0,    0,    0,  156,    0,
    0,    0,    0,    0,    0,    0,  156,    0,  156,  156,
  156,    0,  162,    0,    0,    0,    0,    0,    0,  164,
    0,    0,    0,    0,  156,  156,  156,  156,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  162,  162,    0,    0,    0,    0,    0,
    0,    0,    0,  157,    0,    0,    0,    0,  156,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  157,    0,    0,  156,
  156,    0,    0,    0,  157,    0,  157,  157,  157,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  157,  157,  157,  157,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  157,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  157,  157,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  162,
  162,  162,  162,  162,  162,  162,    0,    0,  162,    0,
  162,  162,    0,    0,  162,    0,    0,    0,    0,    0,
  162,  162,  162,  162,  162,  162,  162,  162,  162,    0,
    0,    0,    0,  162,  162,  162,  162,  162,  162,  162,
    0,    0,    0,    0,    0,  156,  156,  156,  156,  156,
  156,  156,    0,  158,  156,    0,  156,  156,    0,    0,
  156,    0,    0,    0,    0,    0,  156,  156,  156,  156,
  156,  156,  156,    0,  162,  162,    0,  162,  162,  156,
  156,  156,  156,  156,  156,  156,  158,    0,    0,    0,
    0,    0,    0,    0,  158,    0,  158,  158,  158,    0,
    0,    0,    0,    0,    0,    0,    0,  162,    0,    0,
    0,    0,  158,  158,  158,  158,    0,    0,    0,    0,
  156,  156,    0,  156,  156,    0,    0,    0,    0,    0,
    0,    0,    0,  157,  157,  157,  157,  157,  157,  157,
    0,  151,  157,    0,  157,  157,  158,    0,  157,    0,
    0,    0,    0,  156,  157,  157,  157,  157,  157,  157,
  157,    0,    0,    0,    0,    0,    0,  157,  157,  157,
  157,  157,  157,  157,  151,    0,    0,  158,  158,    0,
    0,    0,  151,    0,  151,  151,  151,  153,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  151,  151,  151,  151,    0,    0,    0,    0,  157,  157,
    0,  157,  157,    0,    0,    0,    0,    0,    0,    0,
  153,    0,    0,    0,    0,    0,    0,    0,  153,    0,
  153,  153,  153,    0,  151,    0,    0,    0,    0,    0,
    0,  157,    0,    0,    0,    0,  153,  153,  153,  153,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  151,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  153,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  153,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  158,  158,  158,  158,  158,  158,  158,
    0,  154,  158,    0,  158,  158,    0,    0,  158,    0,
    0,    0,    0,    0,  158,  158,  158,  158,  158,  158,
  158,    0,    0,    0,    0,    0,    0,  158,  158,  158,
  158,  158,  158,  158,  154,    0,    0,    0,    0,    0,
    0,    0,  154,    0,  154,  154,  154,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  154,  154,  154,  154,    0,    0,    0,    0,  158,  158,
    0,  158,  158,    0,    0,    0,    0,    0,    0,    0,
    0,  151,  151,  151,  151,  151,  151,  151,    0,    0,
  151,    0,  151,  151,  154,    0,  151,    0,    0,    0,
    0,  158,  151,  151,  151,  151,  151,  151,    0,    0,
    0,    0,    0,    0,    0,  151,  151,  151,  151,  151,
  151,  151,    0,    0,    0,    0,  154,  153,  153,  153,
  153,  153,  153,  153,    0,  155,  153,    0,  153,  153,
    0,    0,  153,    0,    0,    0,    0,    0,  153,  153,
  153,  153,  153,  153,    0,    0,  151,  151,    0,  151,
  151,  153,  153,  153,  153,  153,  153,  153,  155,    0,
    0,    0,    0,    0,    0,    0,  155,    0,  155,  155,
  155,    0,    0,    0,    0,    0,    0,    0,    0,  151,
    0,    0,    0,    0,  155,  155,  155,  155,    0,    0,
    0,    0,  153,  153,    0,  153,  153,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  155,    0,
    0,    0,    0,    0,    0,  153,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  152,    0,
  155,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  154,  154,  154,  154,  154,  154,  154,    0,    0,
  154,    0,  154,  154,    0,    0,  154,    0,    0,    0,
    0,  152,  154,  154,  154,  154,  154,  154,    0,  152,
    0,  152,  152,  152,    0,  154,  154,  154,  154,  154,
  154,  154,    0,    0,    0,    0,    0,  152,  152,  152,
  152,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  148,    0,    0,    0,    0,  154,  154,    0,  154,
  154,  152,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  148,    0,    0,    0,    0,  154,
    0,    0,  148,  152,  148,  148,  148,    0,    0,    0,
    0,    0,  149,    0,    0,    0,    0,    0,    0,    0,
  148,  148,  148,  148,    0,  155,  155,  155,  155,  155,
  155,  155,    0,    0,  155,    0,  155,  155,    0,    0,
  155,    0,    0,    0,    0,  149,  155,  155,  155,  155,
  155,  155,    0,  149,  148,  149,  149,  149,    0,  155,
  155,  155,  155,  155,  155,  155,    0,    0,    0,    0,
    0,  149,  149,  149,  149,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  148,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  155,  155,    0,  155,  155,  149,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  155,    0,  150,    0,  149,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  152,  152,
  152,  152,  152,  152,  152,    0,    0,  152,    0,  152,
  152,    0,    0,  152,    0,    0,    0,    0,  150,  152,
  152,  152,  152,  152,  152,    0,  150,    0,  150,  150,
  150,    0,  152,  152,  152,  152,  152,  152,  152,    0,
    0,    0,    0,    0,  150,  150,  150,  150,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  146,    0,
    0,    0,    0,  152,  152,    0,  152,  152,  150,    0,
    0,  148,  148,  148,  148,  148,  148,  148,    0,    0,
  148,    0,  148,  148,    0,    0,  148,    0,    0,    0,
    0,  146,  148,  148,  148,    0,  152,    0,    0,  146,
  150,    0,  146,    0,    0,  148,  148,  148,  148,  148,
  148,  148,    0,    0,    0,    0,    0,  146,  146,  146,
  146,    0,  149,  149,  149,  149,  149,  149,  149,    0,
    0,  149,    0,  149,  149,    0,    0,  149,    0,    0,
    0,    0,    0,  149,  149,  149,  148,  148,  147,  148,
  148,  146,    0,    0,    0,    0,  149,  149,  149,  149,
  149,  149,  149,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  147,    0,  146,    0,    0,    0,    0,    0,  147,
    0,    0,  147,    0,    0,    0,    0,  149,  149,    0,
  149,  149,    0,    0,    0,    0,    0,  147,  147,  147,
  147,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  176,    0,    0,    0,  176,    0,  176,
  176,  147,  176,  176,  176,  150,  150,  150,  150,  150,
  150,  150,    0,    0,  150,    0,  150,  150,    0,    0,
  150,  176,    0,    0,    0,    0,  150,  150,  150,    0,
    0,    0,    0,  147,    0,    0,    0,    0,    0,  150,
  150,  150,  150,  150,  150,  150,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  178,    0,    0,
    0,  178,    0,  178,  178,    0,  178,  178,  178,    0,
  150,  150,    0,  150,  150,    0,    0,    0,  146,  146,
  146,  146,  146,  146,  146,  178,    0,  146,    0,  146,
  146,    0,    0,  146,    0,    0,    0,    0,    0,  146,
  146,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  146,  146,  146,  146,  146,  146,  146,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  221,    0,    0,    0,  222,    0,  225,    0,    0,
    0,  226,  223,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  146,  146,    0,  146,  146,    0,  224,
    0,    0,    0,    0,    0,    0,    0,    0,  147,  147,
  147,  147,  147,  147,  147,    0,    0,  147,    0,  147,
  147,    0,    0,  147,    0,    0,    0,    0,    0,  147,
  147,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  147,  147,  147,  147,  147,  147,  147,    0,
    0,  176,  176,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  176,  176,  176,  176,  176,    0,    0,
    0,    0,    0,  147,  147,  171,  147,  147,    0,  171,
    0,  171,    0,    0,    0,  171,  171,    0,  176,    0,
    0,  176,  176,  176,  176,  176,  176,  176,  176,    0,
    0,    0,    0,  171,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  178,  178,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  176,  176,  176,
  176,  176,  176,    0,  176,    0,  176,  178,  178,  178,
  178,  178,  176,  176,  176,  176,  176,  176,  176,  176,
  176,  176,  176,  176,  176,    0,    0,  176,    0,    0,
    0,    0,  178,    0,    0,  178,  178,  178,  178,  178,
  178,  178,  178,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  183,
  184,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  178,  178,  178,  178,  178,  178,    0,  178,    0,
  178,  185,  186,  187,  188,  189,  178,  178,  178,  178,
  178,  178,  178,  178,  178,  178,  178,  178,  178,    0,
    0,  178,    0,    0,    0,    0,  190,    0,    0,  191,
  192,  193,  194,  195,  196,  197,  198,    0,    0,    0,
    0,    0,    0,  221,    0,    0,    0,  222,    0,  225,
    0,    0,    0,  226,  223,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  199,  200,  201,  202,  203,
  204,  224,  205,    0,  206,    0,    0,    0,    0,    0,
  207,  208,  209,  210,  211,  212,  213,  214,  215,  216,
  217,  218,  219,  171,  171,  220,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  171,  171,  171,  171,  171,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  171,    0,    0,  171,  171,  171,  171,  171,  171,  171,
  171,    0,    0,    0,    0,    0,    0,  221,    0,    0,
    0,  222,    0,  225,    0,    0,    0,  226,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  171,
  171,  171,  171,  171,  171,  224,  171,    0,  171,    0,
    0,    0,    0,    0,  171,  171,  171,  171,  171,  171,
  171,  171,  171,  171,  171,  171,  171,    0,    0,  171,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  183,  184,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  185,  186,  187,  188,  189,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  191,  192,  193,  194,  195,  196,  197,  198,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  199,  200,  201,
  202,  203,  204,    0,  205,    0,    0,    0,    0,    0,
    0,    0,  207,  208,  209,  210,  211,  212,  213,  214,
  215,  216,  217,  218,  219,  183,  184,  220,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  185,  186,  187,
  188,  189,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  191,  192,  193,  194,  195,
  196,  197,  198,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  199,  200,  201,  202,    0,  204,    0,  205,    0,
    0,    0,    0,    0,    0,    0,  207,  208,  209,  210,
  211,  212,  213,  214,  215,  216,  217,  218,  219,    0,
    0,  220,
  };
  protected static  short [] yyCheck = {            22,
    4,  105,   60,  143,   34,  145,  146,  147,  148,   13,
   14,  123,   41,  179,  180,  180,   34,   44,   60,   34,
   34,   47,   43,   40,   45,   41,   42,   47,   47,  124,
   36,   44,  414,   44,   41,   42,   58,  123,  557,   47,
   40,  123,   62,   62,   48,   41,   40,   44,  600,   44,
   59,   44,   56,   57,   62,   58,  608,  263,  123,  294,
  295,  296,   25,   40,   44,  123,   40,  125,  263,   44,
  324,   44,   44,  386,   44,   44,   27,  203,   44,   44,
   44,  123,  258,  125,  260,   44,   44,   44,   44,   44,
   44,   44,   44,   44,  238,   41,  316,  223,   44,  203,
  660,  661,   41,  247,  358,  123,  123,  125,  123,  270,
  125,  115,   60,   93,  321,  675,  139,  140,  122,  223,
   60,  316,  682,  123,   63,  291,  291,  276,   43,  123,
   45,  270,  271,  272,  273,   41,  224,  689,   44,   41,
  692,  347,   44,  378,  107,  664,  123,  666,  320,  123,
  125,  239,  125,  125,  242,  125,  125,  108,  109,  125,
  125,  125,   33,  303,   47,  305,  125,  125,  125,  125,
  125,  125,  125,  125,  320,  123,  316,  125,  318,   62,
  266,  563,    0,  123,  258,  125,  260,  348,  349,   60,
   61,   62,  270,  271,  272,  273,  345,  346,   47,  222,
   47,  266,  297,  268,   47,   47,  263,  337,   91,  348,
  349,  263,  342,   62,  344,   62,  345,  346,   36,   62,
   62,  399,   40,  401,   42,   43,  404,   45,   46,   47,
  312,  313,   41,  316,  374,   44,  298,  321,  300,  552,
  302,   43,   41,   45,  266,   44,   64,  359,  257,   36,
   41,   59,  279,   44,  123,   61,  125,   41,  287,  270,
   44,  298,  299,  266,  270,  320,  279,  325,  326,  327,
  348,  349,  293,  270,  263,  270,  267,  270,  269,  320,
  284,  264,  265,  325,  326,  327,  267,   41,   61,   43,
  322,   45,  396,  397,  434,   44,  436,  355,   40,  357,
   36,  321,  321,  361,  321,  321,  316,  325,  326,  327,
  325,  326,  327,  355,  321,  357,  324,    0,  631,  361,
  316,  321,  380,  352,  320,   60,  320,   36,  354,  280,
  281,   36,  355,  356,  357,  501,  501,  360,  380,  652,
  363,   40,  365,  656,  367,  511,  511,  370,   40,  379,
  663,  258,  259,   36,  377,   44,  669,   40,  498,   42,
   43,  292,   45,   46,   47,  379,  384,  261,  262,  384,
  510,  394,   60,   61,  514,  398,  300,  325,  326,  327,
  291,   64,  381,  382,  383,  325,  326,  327,  123,  301,
  125,  316,  274,  275,  276,  303,  709,  302,  711,  539,
   61,  714,  267,  716,   61,  409,  268,  355,   36,  357,
  165,  166,  167,  361,   44,  355,  270,  357,   61,   62,
   44,  361,  281,  282,  428,  322,  566,  567,  169,  170,
  453,  322,  380,  304,  305,  306,  307,  308,  309,  310,
  380,  322,  465,  175,  176,    0,  264,  265,  471,  177,
  178,   61,  556,  271,  272,  321,  325,  326,  327,  277,
  278,  123,  280,  345,  346,  283,   42,  123,  286,  287,
  288,  289,  290,   80,   81,  641,  641,  617,  618,  502,
  620,   36,  396,  397,  123,   40,   40,   42,   43,   58,
   45,   46,   47,  311,  181,  182,  314,  315,  316,  317,
  318,  319,  320,  321,   40,  270,  271,  272,  273,   64,
  379,   41,  516,   43,  518,   45,  385,  271,  272,  659,
   40,   40,   40,  277,  278,  321,  280,  316,  316,  283,
  670,  324,  350,  351,  352,  353,  354,  355,   91,  357,
  322,  359,  171,  172,  173,  174,   58,  365,  366,  367,
  368,  369,  370,  371,  372,  373,  374,  375,  376,  377,
  583,   44,  380,  322,  587,   41,   44,   44,  270,   40,
    0,  575,  595,   40,   40,   36,   40,   40,   63,  123,
  123,  264,  265,  348,  349,   41,   41,   41,  271,  272,
  325,  326,  327,   42,  277,  278,  123,  280,  123,  321,
  283,  321,  321,  286,  287,  288,  289,  290,  356,  324,
  320,   41,  267,   61,   44,   58,  269,  281,  284,   41,
  355,  316,  357,   41,  276,   41,  361,   41,  311,   59,
   41,  314,  315,  316,  317,  318,  319,  320,  321,   41,
   60,  323,   36,   60,   91,  380,  324,  269,   61,  276,
  125,  321,  358,  123,  123,   60,  360,  322,  123,   62,
  285,   62,  321,   93,  322,  270,   41,  350,  351,  352,
  353,  354,  355,   61,  357,  316,  359,   47,   93,  125,
   47,  268,  365,  366,  367,  368,  369,  370,  371,  372,
  373,  374,  375,  376,  377,  125,  322,  380,   41,  125,
  321,   47,  362,  123,  324,  125,  123,  321,  125,  264,
  265,  287,  288,  289,  290,   47,  271,  272,  123,  270,
  125,  321,  277,  278,  321,  280,   62,  123,  283,  125,
  270,  286,  287,  288,  289,  290,   62,   62,  314,   62,
   62,  271,  272,   47,  320,  321,    0,  277,  278,   47,
  280,   62,   47,  283,  321,  321,  311,   47,  321,  314,
  315,  316,  317,  318,  319,  320,  321,  316,   62,  321,
   62,   62,    0,   62,  350,  351,  352,  353,   59,   33,
    0,   59,   41,  321,   59,   41,   58,   41,  269,   43,
   44,   45,   59,   47,   91,  350,  351,  352,  353,  354,
  355,  269,  357,   61,  359,   59,   60,   61,   62,  125,
  365,  366,  367,  368,  369,  370,  371,  372,  373,  374,
  375,  376,  377,   62,   19,  380,  287,  288,  289,  290,
  270,  270,   19,   23,    0,   24,   24,   91,  283,   93,
  270,  271,  272,  273,  274,  275,  276,  408,  413,  279,
  563,  281,  282,  314,   90,  285,  415,   90,  299,  301,
  321,  291,  292,  149,  148,   77,  434,   33,  432,  304,
  124,  125,  512,  144,  153,   41,  182,   43,   44,   45,
  168,   47,  556,   43,  231,   45,  638,  559,  682,  350,
  351,  352,  353,   59,   60,   61,   62,  675,  378,  580,
  532,  380,  363,  364,  378,  325,  326,  327,  325,  326,
  327,   -1,   -1,   -1,   -1,  345,  346,  379,  348,  349,
  325,  326,  327,   -1,   -1,   91,   -1,   93,   -1,  325,
  326,  327,   43,   -1,   45,  355,   -1,  357,  355,   -1,
  357,  361,   -1,    0,  361,   -1,   -1,   -1,   -1,   -1,
  355,   -1,  357,   -1,   -1,   -1,  361,   -1,  124,  125,
  380,   -1,   43,  380,   45,  125,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  380,   33,   -1,   -1,   -1,
   -1,   -1,   -1,  379,   41,   -1,   43,   44,   45,  385,
   47,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   59,   60,   61,   62,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  125,   -1,  270,  271,  272,  273,
  274,  275,  276,   -1,   -1,  279,   -1,  281,  282,   -1,
   -1,  285,   -1,   -1,   -1,   -1,   93,  291,  292,  293,
  294,  295,  296,  297,  298,  299,  300,  301,  302,  303,
  304,  305,  306,  307,  308,  309,  310,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  124,  125,   -1,
   -1,   43,   -1,   45,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   43,   -1,
   45,  345,  346,   -1,  348,  349,   -1,   -1,   -1,   -1,
  354,   43,   -1,   45,  270,  271,  272,  273,  274,  275,
  276,  271,  272,  279,   -1,  281,  282,  277,  278,  285,
  280,   -1,   -1,  283,  378,  291,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,  305,
  306,  307,  308,  309,  310,   36,   -1,   -1,   43,   40,
   45,   42,   43,  125,   45,   46,   47,   -1,   -1,   -1,
  271,  272,   -1,   -1,   -1,   -1,  277,  278,   -1,  280,
  125,   -1,  283,   64,   -1,   43,   -1,   45,   -1,  345,
  346,  123,  348,  349,   -1,   -1,   -1,   -1,  354,   -1,
  271,  272,   -1,   -1,   -1,   -1,  277,  278,   -1,  280,
   -1,   -1,  283,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  378,  270,  271,  272,  273,  274,  275,  276,
   -1,    0,  279,   -1,  281,  282,   -1,   -1,  285,   -1,
  125,   -1,   -1,   -1,  291,  292,  293,  294,  295,  296,
  297,  298,  299,  300,  301,  302,  303,  304,  305,  306,
  307,  308,  309,  310,   33,   -1,   -1,  125,   -1,   -1,
   -1,   -1,   41,   -1,   43,   44,   45,   -1,   47,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   59,   60,   61,   62,   -1,   -1,   -1,   -1,  345,  346,
   -1,  348,  349,   -1,   -1,   -1,   -1,  354,   -1,  271,
  272,   -1,   -1,   -1,   -1,  277,  278,   -1,  280,    0,
   -1,  283,   -1,   -1,   93,   -1,  271,  272,   -1,   -1,
   -1,  378,  277,  278,   -1,  280,   -1,   -1,  283,  271,
  272,   -1,   -1,   -1,   -1,  277,  278,   -1,  280,   -1,
   -1,  283,   33,   -1,   -1,  124,  125,   -1,   -1,   -1,
   41,   -1,   43,   44,   45,   -1,   47,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   59,   60,
   61,   62,   -1,  264,  265,   -1,  271,  272,   -1,   -1,
  271,  272,  277,  278,   -1,  280,  277,  278,  283,  280,
   -1,   -1,  283,   -1,   -1,  286,  287,  288,  289,  290,
   -1,   -1,   93,  271,  272,   -1,   -1,   -1,   -1,  277,
  278,   -1,  280,   -1,   -1,  283,   -1,   -1,   -1,   -1,
  311,   -1,   -1,  314,  315,  316,  317,  318,  319,  320,
  321,   -1,   -1,  124,  125,  328,  329,  330,  331,  332,
  333,  334,  335,  336,  337,  338,  339,  340,  341,  342,
  343,  344,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  350,
  351,  352,  353,  354,  355,   -1,  357,   -1,  359,   -1,
   -1,   -1,   -1,   -1,  365,  366,  367,  368,  369,  370,
  371,  372,  373,  374,  375,  376,  377,   -1,   -1,  380,
   -1,  270,  271,  272,  273,  274,  275,  276,   -1,    0,
  279,   -1,  281,  282,   -1,   -1,  285,   -1,   -1,   -1,
   -1,   -1,  291,  292,  293,  294,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,  305,  306,  307,  308,
  309,  310,   33,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   41,   -1,   43,   44,   45,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   58,   59,   60,
   61,   62,   -1,   -1,   -1,   -1,  345,  346,   -1,  348,
  349,   -1,   -1,   -1,   -1,  354,   -1,   -1,   -1,  270,
  271,  272,  273,  274,  275,  276,   -1,    0,  279,   -1,
  281,  282,   93,   -1,  285,   -1,   -1,   -1,   -1,  378,
  291,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,  303,  304,  305,  306,  307,  308,  309,  310,
   33,   -1,  123,  124,  125,   -1,   -1,   -1,   41,   -1,
   43,   44,   45,   -1,   47,   -1,   -1,  287,  288,  289,
  290,   -1,   -1,   -1,   -1,   -1,   59,   60,   61,   62,
   -1,   -1,   -1,   -1,  345,  346,   -1,  348,  349,   -1,
   -1,   -1,   -1,  354,  314,   -1,   -1,   -1,   -1,   -1,
   -1,  321,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   93,   -1,   -1,   -1,   -1,   -1,   -1,  378,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,    0,   -1,   -1,   -1,
  350,  351,  352,  353,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  124,  125,  363,  364,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   33,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   41,   -1,   43,
   44,   45,   -1,   47,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   59,   60,   61,   62,   -1,
   -1,   -1,   -1,   -1,   -1,  266,  267,   -1,  269,  270,
  271,  272,  273,  274,  275,  276,   -1,   -1,  279,   -1,
  281,  282,   -1,   -1,  285,   -1,   -1,   -1,   -1,   93,
  291,  292,  293,  294,  295,  296,  297,  298,  299,  300,
   -1,   -1,   -1,  304,  305,  306,  307,  308,  309,  310,
   -1,   -1,    0,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  124,  125,  329,  330,  331,  332,  333,  334,  335,  336,
  337,  338,  339,  340,  341,  342,  343,  344,   -1,   -1,
   -1,   -1,   -1,   -1,  345,  346,   -1,  348,  349,   -1,
   -1,   -1,   -1,   41,   -1,   -1,   44,  270,  271,  272,
  273,  274,  275,  276,   -1,   -1,  279,   -1,  281,  282,
   -1,   59,  285,   -1,   -1,   -1,   -1,  378,  291,  292,
  293,  294,  295,  296,  297,  298,  299,  300,  301,  302,
  303,  304,  305,  306,  307,  308,  309,  310,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   93,  330,  331,  332,  333,
  334,  335,  336,  337,  338,  339,  340,  341,  342,  343,
  344,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  345,  346,   -1,  348,  349,  125,   -1,   -1,
   -1,  354,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  378,  270,  271,  272,  273,
  274,  275,  276,   -1,    0,  279,   -1,  281,  282,   -1,
   -1,  285,   -1,   -1,   -1,   -1,   -1,  291,  292,  293,
  294,  295,  296,  297,  298,  299,  300,  301,  302,  303,
  304,  305,  306,  307,  308,  309,  310,   33,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   41,   -1,   43,   44,   45,
   -1,   47,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   59,   60,   61,   62,   -1,   -1,   -1,
   -1,  345,  346,   -1,  348,  349,   -1,   -1,   -1,   -1,
  354,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,    0,   -1,   -1,   -1,   -1,   93,   -1,   -1,
   -1,   -1,   -1,   -1,  378,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  270,  271,  272,  273,  274,  275,  276,   -1,
   -1,  279,   -1,  281,  282,   33,   -1,  285,  124,  125,
   -1,   -1,   -1,   41,  292,   43,   44,   45,   -1,    0,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   59,   60,   61,   62,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   41,   -1,   -1,   44,   -1,   93,   -1,  345,  346,   -1,
  348,  349,   -1,   -1,   -1,   -1,   -1,   -1,   59,   -1,
   -1,    0,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  124,  125,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   93,   -1,   33,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   41,   -1,   43,   44,   45,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   59,   60,   61,   62,  125,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  270,  271,  272,  273,  274,  275,
  276,   -1,   -1,  279,   -1,  281,  282,   -1,   -1,  285,
   -1,   -1,   -1,   -1,   93,  291,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,  303,  304,  305,
  306,  307,  308,  309,  310,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  124,  125,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  345,
  346,   -1,  348,  349,   -1,   -1,   -1,   -1,  354,   -1,
   -1,   -1,  270,  271,  272,  273,  274,  275,  276,   -1,
   -1,  279,   -1,  281,  282,   -1,   -1,  285,   -1,   -1,
   -1,   -1,  378,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,  302,  303,  304,  305,  306,  307,
  308,  309,  310,   -1,   -1,   -1,   -1,   -1,   -1,  270,
  271,  272,  273,  274,  275,  276,   -1,   -1,  279,   -1,
  281,  282,   -1,   -1,  285,   -1,   -1,   -1,   -1,   -1,
   -1,  292,   -1,   -1,   -1,   -1,   -1,  345,  346,   -1,
  348,  349,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  378,  270,  271,  272,  273,  274,  275,  276,   -1,    0,
  279,   -1,  281,  282,  345,  346,  285,  348,  349,   -1,
   -1,   -1,  291,  292,  293,  294,  295,  296,  297,  298,
  299,  300,  301,  302,  303,  304,  305,  306,  307,  308,
  309,  310,   33,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   41,   -1,   43,   44,   45,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,    0,   59,   60,
   61,   62,   -1,   -1,   -1,   -1,  345,  346,   -1,  348,
  349,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,    0,   -1,   -1,
   -1,   -1,   93,   -1,   -1,   -1,   -1,   -1,   41,  378,
   -1,   44,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   59,   -1,   -1,   -1,
   33,   -1,   -1,  124,  125,   -1,   -1,   -1,   41,   -1,
   43,   44,   45,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   59,   60,   61,   62,
   93,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   93,   -1,  125,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,    0,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  124,  125,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   33,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   41,   -1,   43,
   44,   45,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   59,   60,   61,   62,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  270,
  271,  272,  273,  274,  275,  276,   -1,   -1,  279,   -1,
  281,  282,   -1,   -1,  285,   -1,   -1,   -1,   -1,   93,
  291,  292,  293,  294,  295,  296,  297,  298,  299,  300,
  301,  302,  303,  304,  305,  306,  307,  308,  309,  310,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  124,  125,   -1,   -1,   -1,   -1,   -1,  270,  271,  272,
  273,  274,  275,  276,   -1,   -1,  279,   -1,  281,  282,
   -1,   -1,  285,   -1,  345,  346,   -1,  348,  349,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  270,  271,  272,
  273,  274,  275,  276,   -1,   -1,  279,   -1,  281,  282,
   -1,   -1,  285,   -1,   -1,   -1,   -1,  378,  291,  292,
  293,  294,  295,  296,  297,  298,  299,  300,  301,  302,
  303,  304,  305,  306,  307,  308,  309,  310,   -1,   -1,
   -1,   -1,  345,  346,   -1,  348,  349,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  345,  346,   -1,  348,  349,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  378,  270,  271,  272,  273,
  274,  275,  276,   -1,    0,  279,   -1,  281,  282,   -1,
   -1,  285,   -1,   -1,   -1,   -1,   -1,  291,  292,  293,
  294,  295,  296,  297,  298,  299,  300,  301,  302,   -1,
  304,  305,  306,  307,  308,  309,  310,   33,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   41,   -1,   43,   44,   45,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   59,   60,   61,   62,   -1,   -1,   -1,
   -1,  345,  346,   -1,  348,  349,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,    0,   -1,   -1,   -1,   -1,   93,   -1,   -1,
   -1,   -1,   -1,   -1,  378,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   33,   -1,   -1,  124,  125,
   -1,   -1,   -1,   41,   -1,   43,   44,   45,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   59,   60,   61,   62,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   93,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,    0,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  124,  125,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   33,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   41,   -1,   43,   44,   45,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   59,   60,   61,   62,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  270,  271,  272,  273,  274,  275,
  276,   -1,   -1,  279,   -1,  281,  282,   -1,   -1,  285,
   -1,   -1,   -1,   -1,   93,  291,  292,  293,  294,  295,
  296,  297,  298,  299,  300,  301,  302,   -1,  304,  305,
  306,  307,  308,  309,  310,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  124,  125,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  345,
  346,   -1,  348,  349,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  270,  271,  272,  273,  274,  275,  276,   -1,
   -1,  279,   -1,  281,  282,   -1,   -1,  285,   -1,   -1,
   -1,   -1,  378,  291,  292,  293,  294,  295,  296,  297,
  298,  299,  300,  301,   -1,   -1,  304,  305,  306,  307,
  308,  309,  310,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  345,  346,   -1,
  348,  349,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  378,  270,  271,  272,  273,  274,  275,  276,   -1,    0,
  279,   -1,  281,  282,   -1,   -1,  285,   -1,   -1,   -1,
   -1,   -1,  291,  292,  293,  294,  295,  296,  297,  298,
  299,  300,   -1,   -1,   -1,  304,  305,  306,  307,  308,
  309,  310,   33,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   41,   -1,   43,   44,   45,    0,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   59,   60,
   61,   62,   -1,   -1,   -1,   -1,  345,  346,   -1,  348,
  349,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   33,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   41,   -1,   43,   44,
   45,   -1,   93,   -1,   -1,   -1,   -1,   -1,   -1,  378,
   -1,   -1,   -1,   -1,   59,   60,   61,   62,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  124,  125,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,    0,   -1,   -1,   -1,   -1,   93,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   33,   -1,   -1,  124,
  125,   -1,   -1,   -1,   41,   -1,   43,   44,   45,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   59,   60,   61,   62,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   93,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  124,  125,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  270,
  271,  272,  273,  274,  275,  276,   -1,   -1,  279,   -1,
  281,  282,   -1,   -1,  285,   -1,   -1,   -1,   -1,   -1,
  291,  292,  293,  294,  295,  296,  297,  298,  299,   -1,
   -1,   -1,   -1,  304,  305,  306,  307,  308,  309,  310,
   -1,   -1,   -1,   -1,   -1,  270,  271,  272,  273,  274,
  275,  276,   -1,    0,  279,   -1,  281,  282,   -1,   -1,
  285,   -1,   -1,   -1,   -1,   -1,  291,  292,  293,  294,
  295,  296,  297,   -1,  345,  346,   -1,  348,  349,  304,
  305,  306,  307,  308,  309,  310,   33,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   41,   -1,   43,   44,   45,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  378,   -1,   -1,
   -1,   -1,   59,   60,   61,   62,   -1,   -1,   -1,   -1,
  345,  346,   -1,  348,  349,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  270,  271,  272,  273,  274,  275,  276,
   -1,    0,  279,   -1,  281,  282,   93,   -1,  285,   -1,
   -1,   -1,   -1,  378,  291,  292,  293,  294,  295,  296,
  297,   -1,   -1,   -1,   -1,   -1,   -1,  304,  305,  306,
  307,  308,  309,  310,   33,   -1,   -1,  124,  125,   -1,
   -1,   -1,   41,   -1,   43,   44,   45,    0,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   59,   60,   61,   62,   -1,   -1,   -1,   -1,  345,  346,
   -1,  348,  349,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   33,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   41,   -1,
   43,   44,   45,   -1,   93,   -1,   -1,   -1,   -1,   -1,
   -1,  378,   -1,   -1,   -1,   -1,   59,   60,   61,   62,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  125,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   93,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  125,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  270,  271,  272,  273,  274,  275,  276,
   -1,    0,  279,   -1,  281,  282,   -1,   -1,  285,   -1,
   -1,   -1,   -1,   -1,  291,  292,  293,  294,  295,  296,
  297,   -1,   -1,   -1,   -1,   -1,   -1,  304,  305,  306,
  307,  308,  309,  310,   33,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   41,   -1,   43,   44,   45,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   59,   60,   61,   62,   -1,   -1,   -1,   -1,  345,  346,
   -1,  348,  349,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  270,  271,  272,  273,  274,  275,  276,   -1,   -1,
  279,   -1,  281,  282,   93,   -1,  285,   -1,   -1,   -1,
   -1,  378,  291,  292,  293,  294,  295,  296,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  304,  305,  306,  307,  308,
  309,  310,   -1,   -1,   -1,   -1,  125,  270,  271,  272,
  273,  274,  275,  276,   -1,    0,  279,   -1,  281,  282,
   -1,   -1,  285,   -1,   -1,   -1,   -1,   -1,  291,  292,
  293,  294,  295,  296,   -1,   -1,  345,  346,   -1,  348,
  349,  304,  305,  306,  307,  308,  309,  310,   33,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   41,   -1,   43,   44,
   45,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  378,
   -1,   -1,   -1,   -1,   59,   60,   61,   62,   -1,   -1,
   -1,   -1,  345,  346,   -1,  348,  349,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   93,   -1,
   -1,   -1,   -1,   -1,   -1,  378,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,    0,   -1,
  125,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  270,  271,  272,  273,  274,  275,  276,   -1,   -1,
  279,   -1,  281,  282,   -1,   -1,  285,   -1,   -1,   -1,
   -1,   33,  291,  292,  293,  294,  295,  296,   -1,   41,
   -1,   43,   44,   45,   -1,  304,  305,  306,  307,  308,
  309,  310,   -1,   -1,   -1,   -1,   -1,   59,   60,   61,
   62,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,    0,   -1,   -1,   -1,   -1,  345,  346,   -1,  348,
  349,   93,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   33,   -1,   -1,   -1,   -1,  378,
   -1,   -1,   41,  125,   43,   44,   45,   -1,   -1,   -1,
   -1,   -1,    0,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   59,   60,   61,   62,   -1,  270,  271,  272,  273,  274,
  275,  276,   -1,   -1,  279,   -1,  281,  282,   -1,   -1,
  285,   -1,   -1,   -1,   -1,   33,  291,  292,  293,  294,
  295,  296,   -1,   41,   93,   43,   44,   45,   -1,  304,
  305,  306,  307,  308,  309,  310,   -1,   -1,   -1,   -1,
   -1,   59,   60,   61,   62,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  125,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  345,  346,   -1,  348,  349,   93,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  378,   -1,    0,   -1,  125,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  270,  271,
  272,  273,  274,  275,  276,   -1,   -1,  279,   -1,  281,
  282,   -1,   -1,  285,   -1,   -1,   -1,   -1,   33,  291,
  292,  293,  294,  295,  296,   -1,   41,   -1,   43,   44,
   45,   -1,  304,  305,  306,  307,  308,  309,  310,   -1,
   -1,   -1,   -1,   -1,   59,   60,   61,   62,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,    0,   -1,
   -1,   -1,   -1,  345,  346,   -1,  348,  349,   93,   -1,
   -1,  270,  271,  272,  273,  274,  275,  276,   -1,   -1,
  279,   -1,  281,  282,   -1,   -1,  285,   -1,   -1,   -1,
   -1,   33,  291,  292,  293,   -1,  378,   -1,   -1,   41,
  125,   -1,   44,   -1,   -1,  304,  305,  306,  307,  308,
  309,  310,   -1,   -1,   -1,   -1,   -1,   59,   60,   61,
   62,   -1,  270,  271,  272,  273,  274,  275,  276,   -1,
   -1,  279,   -1,  281,  282,   -1,   -1,  285,   -1,   -1,
   -1,   -1,   -1,  291,  292,  293,  345,  346,    0,  348,
  349,   93,   -1,   -1,   -1,   -1,  304,  305,  306,  307,
  308,  309,  310,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   33,   -1,  125,   -1,   -1,   -1,   -1,   -1,   41,
   -1,   -1,   44,   -1,   -1,   -1,   -1,  345,  346,   -1,
  348,  349,   -1,   -1,   -1,   -1,   -1,   59,   60,   61,
   62,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   36,   -1,   -1,   -1,   40,   -1,   42,
   43,   93,   45,   46,   47,  270,  271,  272,  273,  274,
  275,  276,   -1,   -1,  279,   -1,  281,  282,   -1,   -1,
  285,   64,   -1,   -1,   -1,   -1,  291,  292,  293,   -1,
   -1,   -1,   -1,  125,   -1,   -1,   -1,   -1,   -1,  304,
  305,  306,  307,  308,  309,  310,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   36,   -1,   -1,
   -1,   40,   -1,   42,   43,   -1,   45,   46,   47,   -1,
  345,  346,   -1,  348,  349,   -1,   -1,   -1,  270,  271,
  272,  273,  274,  275,  276,   64,   -1,  279,   -1,  281,
  282,   -1,   -1,  285,   -1,   -1,   -1,   -1,   -1,  291,
  292,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  304,  305,  306,  307,  308,  309,  310,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   36,   -1,   -1,   -1,   40,   -1,   42,   -1,   -1,
   -1,   46,   47,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  345,  346,   -1,  348,  349,   -1,   64,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  270,  271,
  272,  273,  274,  275,  276,   -1,   -1,  279,   -1,  281,
  282,   -1,   -1,  285,   -1,   -1,   -1,   -1,   -1,  291,
  292,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  304,  305,  306,  307,  308,  309,  310,   -1,
   -1,  264,  265,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  286,  287,  288,  289,  290,   -1,   -1,
   -1,   -1,   -1,  345,  346,   36,  348,  349,   -1,   40,
   -1,   42,   -1,   -1,   -1,   46,   47,   -1,  311,   -1,
   -1,  314,  315,  316,  317,  318,  319,  320,  321,   -1,
   -1,   -1,   -1,   64,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  264,  265,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  350,  351,  352,
  353,  354,  355,   -1,  357,   -1,  359,  286,  287,  288,
  289,  290,  365,  366,  367,  368,  369,  370,  371,  372,
  373,  374,  375,  376,  377,   -1,   -1,  380,   -1,   -1,
   -1,   -1,  311,   -1,   -1,  314,  315,  316,  317,  318,
  319,  320,  321,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  264,
  265,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  350,  351,  352,  353,  354,  355,   -1,  357,   -1,
  359,  286,  287,  288,  289,  290,  365,  366,  367,  368,
  369,  370,  371,  372,  373,  374,  375,  376,  377,   -1,
   -1,  380,   -1,   -1,   -1,   -1,  311,   -1,   -1,  314,
  315,  316,  317,  318,  319,  320,  321,   -1,   -1,   -1,
   -1,   -1,   -1,   36,   -1,   -1,   -1,   40,   -1,   42,
   -1,   -1,   -1,   46,   47,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  350,  351,  352,  353,  354,
  355,   64,  357,   -1,  359,   -1,   -1,   -1,   -1,   -1,
  365,  366,  367,  368,  369,  370,  371,  372,  373,  374,
  375,  376,  377,  264,  265,  380,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  286,  287,  288,  289,  290,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  311,   -1,   -1,  314,  315,  316,  317,  318,  319,  320,
  321,   -1,   -1,   -1,   -1,   -1,   -1,   36,   -1,   -1,
   -1,   40,   -1,   42,   -1,   -1,   -1,   46,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  350,
  351,  352,  353,  354,  355,   64,  357,   -1,  359,   -1,
   -1,   -1,   -1,   -1,  365,  366,  367,  368,  369,  370,
  371,  372,  373,  374,  375,  376,  377,   -1,   -1,  380,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  264,  265,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  286,  287,  288,  289,  290,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  314,  315,  316,  317,  318,  319,  320,  321,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  350,  351,  352,
  353,  354,  355,   -1,  357,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  365,  366,  367,  368,  369,  370,  371,  372,
  373,  374,  375,  376,  377,  264,  265,  380,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  286,  287,  288,
  289,  290,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  314,  315,  316,  317,  318,
  319,  320,  321,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  350,  351,  352,  353,   -1,  355,   -1,  357,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  365,  366,  367,  368,
  369,  370,  371,  372,  373,  374,  375,  376,  377,   -1,
   -1,  380,
  };

#line 2018 "XQuery.y"
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
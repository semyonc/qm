
// created by jay 0.7 (c) 1998 Axel.Schreiner@informatik.uni-osnabrueck.de

#line 28 "SQL92-min.y"

#pragma warning disable 162

using System;
using System.IO;  
using System.Collections;
using DataEngine.CoreServices;

namespace DataEngine.Parser 
{
	public class YYParser
	{	     
	    private Notation notation;
	            
	    public YYParser(Notation notation)
	    {
	    	errorText = new StringWriter();	    	
	    	this.notation = notation;
	    }
	
		public object yyparseSafe (Tokenizer tok)
		{
			return yyparseSafe (tok, null);
		}

		public object yyparseSafe (Tokenizer tok, object yyDebug)
		{
			try
			{
			    notation.Clear();
				return yyparse (tok, yyDebug);
			}
			catch (Exception e)
			{
				throw new ESQLException ("Error during parse:\n"+ errorText.ToString(), e);
			}
		}

		public object yyparseDebug (Tokenizer tok)
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

  protected static  int yyFinal = 2;
//t  public static  string [] yyRule = {
//t    "$accept : direct_select_stmt_n_rows",
//t    "direct_select_stmt_n_rows : opt_optimizer_hint query_exp",
//t    "direct_select_stmt_n_rows : opt_optimizer_hint query_exp order_by_clause",
//t    "as_clause : opt_AS column_name",
//t    "between_predicate : row_value_constructor opt_not_tag BETWEEN row_value_constructor AND row_value_constructor",
//t    "opt_not_tag :",
//t    "opt_not_tag : not_tag",
//t    "not_tag : NOT",
//t    "case_abbreviation : nullif_case_abbreviation",
//t    "case_abbreviation : coalesce_case_abbreviation",
//t    "nullif_case_abbreviation : NULLIF '(' value_exp ',' value_exp ')'",
//t    "coalesce_case_abbreviation : COALESCE '(' value_exp_list ')'",
//t    "value_exp_list : value_exp",
//t    "value_exp_list : value_exp_list ',' value_exp",
//t    "case_exp : case_abbreviation",
//t    "case_exp : case_spec",
//t    "case_operand : value_exp",
//t    "case_spec : simple_case",
//t    "case_spec : searched_case",
//t    "null_tag : NULL",
//t    "char_factor : num_value_exp",
//t    "char_factor : string_value_fct",
//t    "char_substring_fct : SUBSTRING '(' char_value_exp FROM start_position for_string_length ')'",
//t    "for_string_length :",
//t    "for_string_length : FOR string_length",
//t    "char_value_exp : char_factor",
//t    "char_value_exp : char_value_exp concatenation_operator char_factor",
//t    "char_value_fct : char_substring_fct",
//t    "char_value_fct : fold",
//t    "char_value_fct : form_conversion",
//t    "char_value_fct : trim_fct",
//t    "column_name_list : column_name",
//t    "column_name_list : column_name_list ',' column_name",
//t    "comp_op : '='",
//t    "comp_op : not_equals_operator",
//t    "comp_op : '<'",
//t    "comp_op : '>'",
//t    "comp_op : less_than_or_equals_operator",
//t    "comp_op : greater_than_or_equals_operator",
//t    "comp_predicate : row_value_constructor comp_op row_value_constructor",
//t    "all_tag : ALL",
//t    "corresponding_column_list : column_name_list",
//t    "corresponding_spec : CORRESPONDING",
//t    "corresponding_spec : CORRESPONDING by_corresponding_column_list",
//t    "by_corresponding_column_list : BY '(' corresponding_column_list ')'",
//t    "cross_join : CROSSJOIN table_ref",
//t    "union_join : UNIONJOIN table_ref",
//t    "natural_join : NATURAL opt_join_type JOIN table_ref",
//t    "default_spec : DEFAULT",
//t    "derived_column : derived_value",
//t    "derived_column : derived_value as_clause",
//t    "derived_value : value_exp",
//t    "derived_value : TABLE subquery",
//t    "derived_column_list : column_name_list",
//t    "opt_AS :",
//t    "opt_AS : AS",
//t    "dyn_parameter_spec : '?'",
//t    "else_clause : ELSE result",
//t    "escape_char : char_value_exp",
//t    "exists_predicate : EXISTS subquery",
//t    "factor : num_primary",
//t    "factor : sign num_primary",
//t    "fold : upper_or_lower '(' char_value_exp ')'",
//t    "upper_or_lower : UPPER",
//t    "upper_or_lower : LOWER",
//t    "form_conversion : CONVERT '(' char_value_exp USING form_conversion_name ')'",
//t    "form_conversion_name : qualified_name",
//t    "from_clause : FROM table_ref_list",
//t    "table_ref_list : table_ref",
//t    "table_ref_list : table_ref_list ',' table_ref",
//t    "general_set_fct : set_fct_type '(' value_exp ')'",
//t    "general_set_fct : set_fct_type '(' set_quantifier value_exp ')'",
//t    "general_value_spec : parameter_spec",
//t    "general_value_spec : dyn_parameter_spec",
//t    "general_value_spec : value_tag",
//t    "value_tag : VALUE",
//t    "grouping_column_ref : column_ref",
//t    "grouping_column_ref_list : grouping_column_ref",
//t    "grouping_column_ref_list : grouping_column_ref_list ',' grouping_column_ref",
//t    "opt_group_by_clause :",
//t    "opt_group_by_clause : GROUP BY grouping_column_ref_list",
//t    "opt_having_clause :",
//t    "opt_having_clause : HAVING search_condition",
//t    "indicator_parameter : parameter_name",
//t    "indicator_parameter : indicator_tag parameter_name",
//t    "indicator_tag : INDICATOR",
//t    "in_predicate : row_value_constructor opt_not_tag IN in_predicate_value",
//t    "in_predicate_value : subquery",
//t    "in_predicate_value : '(' in_value_list ')'",
//t    "in_value_list : value_exp",
//t    "in_value_list : in_value_list ',' value_exp",
//t    "joined_table : table_ref_simple cross_join",
//t    "joined_table : table_ref_simple union_join",
//t    "joined_table : table_ref_simple natural_join",
//t    "joined_table : table_ref_simple qualified_join",
//t    "joined_table : '(' table_ref ')'",
//t    "join_column_list : column_name_list",
//t    "join_condition : ON search_condition",
//t    "join_spec : join_condition",
//t    "join_spec : named_columns_join",
//t    "join_spec : constraint_join",
//t    "join_type : INNER",
//t    "join_type : outer_join_type opt_OUTER",
//t    "opt_OUTER :",
//t    "opt_OUTER : OUTER",
//t    "like_predicate : char_value_exp LIKE pattern",
//t    "like_predicate : char_value_exp NOTLIKE pattern",
//t    "like_predicate : char_value_exp LIKE pattern ESCAPE escape_char",
//t    "like_predicate : char_value_exp NOTLIKE pattern ESCAPE escape_char",
//t    "match_predicate : row_value_constructor MATCH opt_unique_tag opt_match_type subquery",
//t    "opt_unique_tag :",
//t    "opt_unique_tag : UNIQUE",
//t    "opt_match_type :",
//t    "opt_match_type : match_type",
//t    "match_type : FULL",
//t    "match_type : PARTIAL",
//t    "named_columns_join : USING '(' join_column_list ')'",
//t    "constraint_join : USING PRIMARY KEY",
//t    "constraint_join : USING FOREIGN KEY",
//t    "constraint_join : USING CONSTRAINT qualified_name",
//t    "query_exp : query_term",
//t    "query_exp : query_exp query_exp_tag opt_all_tag opt_corresponding_spec query_term",
//t    "query_exp_tag : UNION",
//t    "query_exp_tag : EXCEPT",
//t    "opt_all_tag :",
//t    "opt_all_tag : all_tag",
//t    "opt_corresponding_spec :",
//t    "opt_corresponding_spec : corresponding_spec",
//t    "query_term : simple_table",
//t    "query_term : query_term INTERSECT opt_all_tag opt_corresponding_spec simple_table",
//t    "null_predicate : row_value_constructor IS opt_not_tag null_tag",
//t    "num_primary : value_exp_primary",
//t    "num_primary : num_value_fct",
//t    "num_primary : xml_value_func",
//t    "num_value_fct : position_exp",
//t    "num_value_exp : term",
//t    "num_value_exp : num_value_exp term_op term",
//t    "term_op : '+'",
//t    "term_op : '-'",
//t    "ordering_spec : ASC",
//t    "ordering_spec : DESC",
//t    "order_by_clause : ORDER BY sort_spec_list",
//t    "outer_join_type : LEFT",
//t    "outer_join_type : RIGHT",
//t    "outer_join_type : FULL",
//t    "overlaps_predicate : row_value_constructor OVERLAPS row_value_constructor",
//t    "parameter_spec : parameter_name",
//t    "parameter_spec : parameter_name indicator_parameter",
//t    "pattern : char_value_exp",
//t    "position_exp : POSITION '(' char_value_exp IN char_value_exp ')'",
//t    "predicate : comp_predicate",
//t    "predicate : between_predicate",
//t    "predicate : in_predicate",
//t    "predicate : like_predicate",
//t    "predicate : null_predicate",
//t    "predicate : quantified_comp_predicate",
//t    "predicate : exists_predicate",
//t    "predicate : unique_predicate",
//t    "predicate : match_predicate",
//t    "predicate : overlaps_predicate",
//t    "qualified_join : opt_join_type JOIN table_ref join_spec",
//t    "opt_join_type :",
//t    "opt_join_type : join_type",
//t    "quantified_comp_predicate : row_value_constructor comp_op quantifier subquery",
//t    "quantifier : ALL",
//t    "quantifier : SOME",
//t    "quantifier : ANY",
//t    "query_spec : SELECT opt_top_quantifier opt_set_quantifier select_list table_exp",
//t    "opt_top_quantifier :",
//t    "opt_top_quantifier : TOP '(' unsigned_integer ')'",
//t    "result : result_exp",
//t    "result : null_tag",
//t    "result_exp : value_exp",
//t    "row_value_constructor : row_value_constructor_elem",
//t    "row_value_constructor : '(' row_value_constructor_elem ',' row_value_const_list ')'",
//t    "row_value_constructor_elem : value_exp",
//t    "row_value_constructor_elem : null_tag",
//t    "row_value_constructor_elem : default_spec",
//t    "row_value_const_list : row_value_constructor_elem",
//t    "row_value_const_list : row_value_const_list ',' row_value_constructor_elem",
//t    "searched_case : CASE searched_when_clause_list END",
//t    "searched_case : CASE searched_when_clause_list else_clause END",
//t    "searched_when_clause_list : searched_when_clause",
//t    "searched_when_clause_list : searched_when_clause_list searched_when_clause",
//t    "searched_when_clause : WHEN search_condition THEN result",
//t    "search_condition : boolean_value_exp",
//t    "boolean_value_exp : boolean_term",
//t    "boolean_value_exp : boolean_value_exp boolean_term_op boolean_term",
//t    "boolean_term_op : OR",
//t    "boolean_term : boolean_factor",
//t    "boolean_term : boolean_term boolean_factor_op boolean_factor",
//t    "boolean_factor_op : AND",
//t    "boolean_factor : boolean_test",
//t    "boolean_factor : not_tag boolean_test",
//t    "boolean_test : boolean_primary",
//t    "boolean_test : boolean_primary IS opt_not_tag truth_value",
//t    "boolean_primary : predicate",
//t    "boolean_primary : '(' search_condition ')'",
//t    "truth_value : TRUE",
//t    "truth_value : FALSE",
//t    "truth_value : UNKNOWN",
//t    "select_list : '*'",
//t    "select_list : select_sublist_list",
//t    "select_sublist_list : select_sublist",
//t    "select_sublist_list : select_sublist_list ',' select_sublist",
//t    "select_sublist : qualified_name asterisk_tag",
//t    "select_sublist : derived_column",
//t    "set_fct_spec : count_all_fct",
//t    "set_fct_spec : general_set_fct",
//t    "set_fct_spec : xml_agg_set_fct",
//t    "set_fct_type : AVG",
//t    "set_fct_type : MAX",
//t    "set_fct_type : MIN",
//t    "set_fct_type : SUM",
//t    "set_fct_type : COUNT",
//t    "opt_set_quantifier :",
//t    "opt_set_quantifier : set_quantifier",
//t    "set_quantifier : DISTINCT",
//t    "set_quantifier : ALL",
//t    "sign : '+'",
//t    "sign : '-'",
//t    "simple_case : CASE case_operand simple_when_clause_list END",
//t    "simple_case : CASE case_operand simple_when_clause_list else_clause END",
//t    "simple_when_clause_list : simple_when_clause",
//t    "simple_when_clause_list : simple_when_clause_list simple_when_clause",
//t    "simple_table : query_spec",
//t    "simple_table : table_value_constructor",
//t    "simple_when_clause : WHEN when_operand THEN result",
//t    "sort_key : column_ref",
//t    "sort_key : unsigned_integer",
//t    "sort_spec : sort_key ordering_spec_opt",
//t    "ordering_spec_opt :",
//t    "ordering_spec_opt : ordering_spec",
//t    "sort_spec_list : sort_spec",
//t    "sort_spec_list : sort_spec_list ',' sort_spec",
//t    "start_position : num_value_exp",
//t    "string_length : num_value_exp",
//t    "string_value_fct : char_value_fct",
//t    "subquery : '(' opt_optimizer_hint query_exp ')'",
//t    "subquery : '(' opt_optimizer_hint query_exp order_by_clause ')'",
//t    "table_exp :",
//t    "table_exp : from_clause opt_where_clause opt_group_by_clause opt_having_clause",
//t    "table_ref : table_ref_simple",
//t    "table_ref : joined_table",
//t    "table_ref_simple : table_ref_spec",
//t    "table_ref_simple : table_ref_spec table_correlation",
//t    "table_ref_spec : table_name",
//t    "table_ref_spec : dynamic_table",
//t    "table_ref_spec : subquery",
//t    "dynamic_table : TABLE funcall",
//t    "dynamic_table : TABLE column_ref",
//t    "dynamic_table : TABLE '(' value_exp ')'",
//t    "dynamic_table : TABLE xml_query",
//t    "table_correlation : opt_AS id",
//t    "table_correlation : opt_AS id '(' derived_column_list ')'",
//t    "table_value_constructor : VALUES table_value_const_list",
//t    "table_value_const_list : row_value_constructor",
//t    "table_value_const_list : table_value_const_list ',' row_value_constructor",
//t    "term : factor",
//t    "term : term '*' factor",
//t    "term : term '/' factor",
//t    "trim_fct : TRIM '(' trim_operands ')'",
//t    "trim_operands : trim_source",
//t    "trim_operands : FROM trim_source",
//t    "trim_operands : trim_char FROM trim_source",
//t    "trim_operands : trim_spec FROM trim_source",
//t    "trim_operands : trim_spec trim_char FROM trim_source",
//t    "trim_char : char_value_exp",
//t    "trim_source : char_value_exp",
//t    "trim_spec : LEADING",
//t    "trim_spec : TRAILING",
//t    "trim_spec : BOTH",
//t    "unique_predicate : UNIQUE subquery",
//t    "unsigned_value_spec : unsigned_lit",
//t    "unsigned_value_spec : general_value_spec",
//t    "value_exp : char_value_exp",
//t    "value_exp_primary : unsigned_value_spec",
//t    "value_exp_primary : set_fct_spec",
//t    "value_exp_primary : column_ref",
//t    "value_exp_primary : subquery",
//t    "value_exp_primary : case_exp",
//t    "value_exp_primary : funcall",
//t    "value_exp_primary : prefixed_table_name",
//t    "value_exp_primary : cast_specification",
//t    "value_exp_primary : '(' value_exp ')'",
//t    "funcall : func '(' ')'",
//t    "funcall : func '(' row_value_const_list ')'",
//t    "when_operand : value_exp",
//t    "opt_where_clause :",
//t    "opt_where_clause : WHERE search_condition",
//t    "opt_optimizer_hint :",
//t    "opt_optimizer_hint : optimizer_hint_list",
//t    "optimizer_hint_list : optimizer_hint",
//t    "optimizer_hint_list : optimizer_hint_list optimizer_hint",
//t    "column_name : id",
//t    "table_name : prefixed_table_name",
//t    "table_name : implict_table_name",
//t    "prefixed_table_name : id ':' implict_table_name",
//t    "implict_table_name : qualified_name",
//t    "implict_table_name : qualified_name '.' id",
//t    "column_ref : column_ref_primary",
//t    "column_ref_primary : qualified_name",
//t    "column_ref_primary : column_ref_primary '.' id",
//t    "column_ref_primary : column_ref_primary '.' funcall",
//t    "column_ref_primary : column_ref_primary '[' value_exp ']'",
//t    "column_ref_primary : column_ref_primary double_slash id",
//t    "qualified_name : id",
//t    "unsigned_lit : unsigned_integer",
//t    "unsigned_lit : unsigned_float",
//t    "unsigned_lit : unsigned_double",
//t    "unsigned_lit : string_literal",
//t    "xml_value_func : xml_pi",
//t    "xml_value_func : xml_comment",
//t    "xml_value_func : xml_concatenation",
//t    "xml_value_func : xml_element",
//t    "xml_value_func : xml_parse",
//t    "xml_value_func : xml_forest",
//t    "xml_value_func : xml_cdata",
//t    "xml_value_func : xml_root",
//t    "xml_value_func : xml_query",
//t    "xml_query : XMLQUERY '(' string_literal xml_returning_clause ')'",
//t    "xml_query : XMLQUERY '(' string_literal PASSING xml_passing_clause xml_returning_clause ')'",
//t    "xml_query : XMLQUERY '(' string_literal PASSING BY VALUE xml_passing_clause xml_returning_clause ')'",
//t    "xml_returning_clause :",
//t    "xml_returning_clause : RETURNING_CONTENT",
//t    "xml_returning_clause : RETURNING_SEQUENCE",
//t    "xml_passing_clause : derived_column",
//t    "xml_passing_clause : xml_passing_clause ',' derived_column",
//t    "xml_agg_set_fct : XMLAGG '(' value_exp ')'",
//t    "xml_agg_set_fct : XMLAGG '(' value_exp order_by_clause ')'",
//t    "xml_concatenation : XMLCONCAT '(' value_exp_list ')'",
//t    "xml_forest : xml_forest_all",
//t    "xml_forest : XMLFOREST '(' content_value_list ')'",
//t    "xml_forest : XMLFOREST '(' namespace_decl ',' content_value_list ')'",
//t    "xml_parse : XMLPARSE '(' id value_exp opt_xml_whilespace_clause ')'",
//t    "xml_pi : XMLPI '(' id column_name ')'",
//t    "xml_pi : XMLPI '(' id column_name ',' value_exp ')'",
//t    "xml_comment : XMLCOMMENT '(' value_exp ')'",
//t    "xml_cdata : XMLCDATA '(' value_exp ')'",
//t    "xml_root : XMLROOT '(' value_exp ')'",
//t    "xml_root : XMLROOT '(' value_exp ',' id version_expr ')'",
//t    "xml_root : XMLROOT '(' value_exp ',' id version_expr ',' id standalone_expr ')'",
//t    "version_expr : string_literal",
//t    "version_expr : NO_VALUE",
//t    "standalone_expr : id",
//t    "standalone_expr : NO_VALUE",
//t    "xml_element : XMLELEMENT '(' column_name ')'",
//t    "xml_element : XMLELEMENT '(' column_name ',' namespace_decl ')'",
//t    "xml_element : XMLELEMENT '(' column_name ',' xml_attributes ')'",
//t    "xml_element : XMLELEMENT '(' column_name ',' namespace_decl ',' xml_attributes ')'",
//t    "xml_element : XMLELEMENT '(' column_name ',' value_exp_list ')'",
//t    "xml_element : XMLELEMENT '(' column_name ',' namespace_decl ',' value_exp_list ')'",
//t    "xml_element : XMLELEMENT '(' column_name ',' xml_attributes ',' value_exp_list ')'",
//t    "xml_element : XMLELEMENT '(' column_name ',' namespace_decl ',' xml_attributes ',' value_exp_list ')'",
//t    "namespace_decl : XMLNAMESPACES '(' namespace_decl_list ')'",
//t    "namespace_decl_list : namespace_decl_item",
//t    "namespace_decl_list : namespace_decl_list ',' namespace_decl_item",
//t    "namespace_decl_item : regular_decl_item",
//t    "namespace_decl_item : default_decl_item",
//t    "regular_decl_item : string_literal opt_AS column_name",
//t    "default_decl_item : DEFAULT string_literal",
//t    "default_decl_item : NO_DEFAULT",
//t    "xml_attributes : xml_attributes_all",
//t    "xml_attributes : XMLATTRIBUTES '(' content_value_list ')'",
//t    "content_value_list : content_value",
//t    "content_value_list : content_value_list ',' content_value",
//t    "content_value : named_value",
//t    "content_value : named_value xml_content_option",
//t    "content_value : qualified_name asterisk_tag",
//t    "content_value : qualified_name asterisk_tag xml_content_option",
//t    "named_value : value_exp",
//t    "named_value : value_exp as_clause",
//t    "opt_xml_whilespace_clause :",
//t    "opt_xml_whilespace_clause : xml_whilespace_clause",
//t    "xml_whilespace_clause : PRESERVE_WHITESPACE",
//t    "xml_whilespace_clause : STRIP_WHITESPACE",
//t    "xml_content_option : OPTION_NULL opt_ON_NULL",
//t    "xml_content_option : OPTION_EMPTY opt_ON_NULL",
//t    "xml_content_option : OPTION_ABSENT opt_ON_NULL",
//t    "xml_content_option : OPTION_NIL opt_ON_NULL",
//t    "xml_content_option : OPTION_NIL ON NO_CONTENT",
//t    "opt_ON_NULL :",
//t    "opt_ON_NULL : ON NULL",
//t    "cast_specification : CAST '(' cast_operand AS data_type ')'",
//t    "cast_operand : NULL",
//t    "cast_operand : value_exp",
//t    "data_type : string_type",
//t    "data_type : numeric_type",
//t    "data_type : DATE",
//t    "data_type : domain_name",
//t    "string_type : string_type_name",
//t    "string_type : string_type_name '(' unsigned_integer ')'",
//t    "string_type_name : CHARACTER",
//t    "string_type_name : CHAR",
//t    "string_type_name : CHARACTER_VARYING",
//t    "string_type_name : CHAR_VARYING",
//t    "string_type_name : VARCHAR",
//t    "numeric_type : decimal_type",
//t    "numeric_type : float_type",
//t    "numeric_type : other_numeric_type",
//t    "other_numeric_type : INTEGER",
//t    "other_numeric_type : INT",
//t    "other_numeric_type : SMALLINT",
//t    "other_numeric_type : REAL",
//t    "other_numeric_type : DOUBLE_PRECISION",
//t    "decimal_type : decimal_type_name",
//t    "decimal_type : decimal_type_name '(' unsigned_integer ')'",
//t    "decimal_type : decimal_type_name '(' unsigned_integer ',' unsigned_integer ')'",
//t    "decimal_type_name : NUMERIC",
//t    "decimal_type_name : DECIMAL",
//t    "decimal_type_name : DEC",
//t    "float_type : FLOAT",
//t    "float_type : FLOAT '(' unsigned_integer ')'",
//t    "domain_name : id",
//t  };
  protected static  string [] yyName = {    
    "end-of-file",null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    "'('","')'","'*'","'+'","','","'-'","'.'","'/'",null,null,null,null,
    null,null,null,null,null,null,"':'",null,"'<'","'='","'>'","'?'",null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,"'['",
    null,"']'",null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,null,null,
    null,null,null,null,null,null,null,null,null,null,null,null,
    "_RS_START","ALL","AND","ANY","ARE","AS","ASC","AT","AVG","BETWEEN",
    "BIT","BIT_LENGTH","BOTH","BY","CASE","CAST","CHAR","CHARACTER",
    "CHAR_LENGTH","CHARACTER_LENGTH","CONNECT","CONNECTION","CONSTRAINT",
    "CONSTRAINTS","CONTINUE","CONVERT","CORRESPONDING","COUNT","CREATE",
    "CROSS","COALESCE","CURRENT","CURRENT_DATE","CURRENT_TIME",
    "CURRENT_TIMESTAMP","CURRENT_USER","CURSOR","DATE","DAY","DEALLOCATE",
    "DEC","DECIMAL","DECLARE","DEFAULT","DEFERRABLE","DEFERRED","DELETE",
    "DESC","DESCRIBE","DESCRIPTOR","DIAGNOSTICS","DISCONNECT","DISTINCT",
    "DOMAIN","DOUBLE","DROP","ELSE","END","END_EXEC","ESCAPE","EXCEPT",
    "EXCEPTION","EXEC","EXECUTE","EXISTS","EXTERNAL","EXTRACT","FALSE",
    "FETCH","FIRST","FLOAT","FOR","FOREIGN","FOUND","FROM","FULL","GET",
    "GLOBAL","GO","GOTO","GRANT","GROUP","HAVING","HOUR","IDENTITY",
    "IMMEDIATE","IN","INDICATOR","INITIALLY","INNER","INPUT",
    "INSENSITIVE","INSERT","INT","INTEGER","INTERSECT","INTERVAL","INTO",
    "IS","ISOLATION","JOIN","KEY","LANGUAGE","LAST","LEADING","LEFT",
    "LEVEL","LIKE","LOCAL","LOWER","MATCH","MAX","MIN","MINUTE","MONTH",
    "NAMES","NATIONAL","NATURAL","NCHAR","NEXT","NOT","NULL","NULLIF",
    "NUMERIC","OCTET_LENGTH","OF","ON","ONLY","OPEN","OPTION","OR",
    "ORDER","OUTER","OUTPUT","OVERLAPS","PAD","PARTIAL","POSITION",
    "PREPARE","PRIMARY","PRIOR","PRIVILEGES","PROCEDURE","PUBLIC","READ",
    "REAL","REFERENCES","RELATIVE","RESTRICT","REVOKE","RIGHT","ROLLBACK",
    "ROWS","SCHEMA","SCROLL","SECOND","SECTION","SELECT","SESSION",
    "SESSION_USER","SET","SIZE","SMALLINT","SOME","SPACE","SQL","SQLCODE",
    "SQLERROR","SQLSTATE","SUBSTRING","SUM","SYSTEM_USER","TABLE",
    "TEMPORARY","THEN","TIME","TIMESTAMP","TIMEZONE_HOUR",
    "TIMEZONE_MINUTE","TO","TRAILING","TRANSACTION","TRANSLATE",
    "TRANSLATION","TRIM","TRUE","UNION","UNIQUE","UNKNOWN","UPDATE",
    "UPPER","USAGE","USER","USING","VALUE","VALUES","VARCHAR","WHEN",
    "WHENEVER","WHERE","WITH","WRITE","XMLPI","XMLPARSE","XMLQUERY",
    "XMLCOMMENT","XMLELEMENT","XMLATTRIBUTES","XMLCONCAT","XMLFOREST",
    "XMLAGG","XMLNAMESPACES","XMLCDATA","XMLROOT","PASSING","TOP",
    "_RS_END","CHARACTER_VARYING","\"CHARACTER VARYING\"","CHAR_VARYING",
    "\"CHAR VARYING\"","DOUBLE_PRECISION","\"DOUBLE PRECISION\"",
    "count_all_fct","\"COUNT(*)\"","xml_forest_all","\"XMLFOREST(*)\"",
    "xml_attributes_all","\"XMLATTRIBUTES(*)\"","NOTLIKE","\"NOT LIKE\"",
    "CROSSJOIN","\"CROSS JOIN\"","UNIONJOIN","\"UNION JOIN\"",
    "OPTION_NULL","\"OPTION NULL\"","OPTION_EMPTY","\"OPTION EMPTY\"",
    "OPTION_ABSENT","\"OPTION ABSENT\"","OPTION_NIL","\"OPTION NIL\"",
    "NO_VALUE","\"NO VALUE\"","NO_DEFAULT","\"NO DEFAULT\"","NO_CONTENT",
    "\"NO CONTENT\"","PRESERVE_WHITESPACE","\"PRESERVE WHITESPACE\"",
    "STRIP_WHITESPACE","\"STRIP WHITESPACE\"","RETURNING_CONTENT",
    "\"RETURNING CONTENT\"","RETURNING_SEQUENCE","\"RETURNING SEQUENCE\"",
    "not_equals_operator","\"<>\"","greater_than_or_equals_operator",
    "\">=\"","less_than_or_equals_operator","\"<=\"",
    "concatenation_operator","\"||\"","double_colon","\"::\"",
    "asterisk_tag","\".*\"","double_slash","\"//\"","parameter_name",
    "embdd_variable_name","id","unsigned_integer","unsigned_float",
    "unsigned_double","string_literal","func","optimizer_hint",
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
#line 345 "SQL92-min.y"
  {            
            notation.ConfirmTag(Tag.Stmt, Descriptor.Root, yyVals[0+yyTop]);
            yyVal = notation.ResolveTag(Tag.Stmt);
            if (yyVals[-1+yyTop] != null)
                notation.Confirm((Symbol)yyVal, Descriptor.OptimizerHint, yyVals[-1+yyTop]);
      }
  break;
case 2:
#line 352 "SQL92-min.y"
  {      
            notation.ConfirmTag(Tag.Stmt, Descriptor.Root, yyVals[-1+yyTop]);
			notation.ConfirmTag(Tag.Stmt, Descriptor.Order, yyVals[0+yyTop]);						
			yyVal = notation.ResolveTag(Tag.Stmt);
            if (yyVals[-2+yyTop] != null)
                notation.Confirm((Symbol)yyVal, Descriptor.OptimizerHint, yyVals[-2+yyTop]);			
      }
  break;
case 3:
#line 363 "SQL92-min.y"
  {
         yyVal = yyVals[0+yyTop];
      }
  break;
case 4:
#line 371 "SQL92-min.y"
  {
         Symbol sym = new Symbol(Tag.Predicate);
         yyVal = notation.Confirm(sym, Descriptor.Between, yyVals[-5+yyTop], yyVals[-2+yyTop], yyVals[0+yyTop]);
         if (yyVals[-4+yyTop] != null)
           notation.Confirm(sym, Descriptor.Inverse);
      }
  break;
case 5:
#line 381 "SQL92-min.y"
  {
        yyVal = null;
      }
  break;
case 10:
#line 398 "SQL92-min.y"
  {
          yyVal = notation.Confirm(new Symbol(Tag.Expr), 
            Descriptor.NullIf, yyVals[-3+yyTop], yyVals[-1+yyTop]);
      }
  break;
case 11:
#line 406 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.Expr),
            Descriptor.Coalesce, yyVals[-1+yyTop]);
      }
  break;
case 12:
#line 414 "SQL92-min.y"
  {
		yyVal = Lisp.Cons(yyVals[0+yyTop]);
      }
  break;
case 13:
#line 418 "SQL92-min.y"
  {
		yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
	  }
  break;
case 22:
#line 449 "SQL92-min.y"
  {
         Symbol sym = new Symbol(Tag.CExpr);
         if (yyVals[-1+yyTop] == null)
			yyVal = notation.Confirm(sym, Descriptor.Substring, yyVals[-4+yyTop], yyVals[-2+yyTop]);
		 else
			yyVal = notation.Confirm(sym, Descriptor.Substring, yyVals[-4+yyTop], yyVals[-2+yyTop], yyVals[-1+yyTop]);
      }
  break;
case 23:
#line 460 "SQL92-min.y"
  {
         yyVal = null;
      }
  break;
case 24:
#line 464 "SQL92-min.y"
  {
         yyVal = yyVals[0+yyTop];
      }
  break;
case 26:
#line 472 "SQL92-min.y"
  {
           yyVal = notation.Confirm(new Symbol(Tag.CExpr), Descriptor.Concat, yyVals[-2+yyTop], yyVals[0+yyTop]);
      }
  break;
case 31:
#line 486 "SQL92-min.y"
  {
		yyVal = Lisp.Cons(yyVals[0+yyTop]);
      }
  break;
case 32:
#line 490 "SQL92-min.y"
  {
		yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
	  }
  break;
case 39:
#line 507 "SQL92-min.y"
  {
          yyVal = notation.Confirm(new Symbol(Tag.Predicate), Descriptor.Pred, yyVals[-2+yyTop], yyVals[-1+yyTop], yyVals[0+yyTop]);
      }
  break;
case 44:
#line 527 "SQL92-min.y"
  {
		yyVal = yyVals[-1+yyTop];
    }
  break;
case 45:
#line 534 "SQL92-min.y"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 46:
#line 541 "SQL92-min.y"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 47:
#line 548 "SQL92-min.y"
  {
		yyVal = yyVals[0+yyTop];
		if (yyVals[-2+yyTop] != null)
			notation.ConfirmTag(Tag.Join, Descriptor.JoinType, new TokenWrapper(yyVals[-2+yyTop]));
    }
  break;
case 50:
#line 562 "SQL92-min.y"
  {
          yyVal = yyVals[-1+yyTop];          
          notation.Confirm((Symbol)yyVal, Descriptor.Alias, yyVals[0+yyTop]);          
      }
  break;
case 52:
#line 571 "SQL92-min.y"
  {
          yyVal = yyVals[0+yyTop];
          notation.Confirm((Symbol)yyVal,  Descriptor.Dynatable);
      }
  break;
case 57:
#line 592 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.CaseExpr), Descriptor.ElseBranch, yyVals[0+yyTop]);
      }
  break;
case 59:
#line 603 "SQL92-min.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Predicate), Descriptor.Exists, yyVals[0+yyTop]);
    }
  break;
case 61:
#line 618 "SQL92-min.y"
  {
         if (yyVals[-1+yyTop].Equals("-"))
			yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.UnaryMinus, yyVals[0+yyTop]);
		 else
			yyVal = 2;
       }
  break;
case 62:
#line 629 "SQL92-min.y"
  {
         Symbol sym = new Symbol(Tag.CExpr);
         switch((int)yyVals[-3+yyTop])
         {
			case Token.UPPER:
				yyVal = notation.Confirm(sym, Descriptor.StringUpper, yyVals[-1+yyTop]);
				break;
			case Token.LOWER:
				yyVal = notation.Confirm(sym, Descriptor.StringLower, yyVals[-1+yyTop]);
				break;
         }
      }
  break;
case 65:
#line 651 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.CExpr), 
			Descriptor.StringConvert,  yyVals[-3+yyTop], yyVals[-1+yyTop]);
      }
  break;
case 67:
#line 663 "SQL92-min.y"
  {
		notation.ConfirmTag(Tag.SQuery, Descriptor.From, yyVals[0+yyTop]);
      }
  break;
case 68:
#line 670 "SQL92-min.y"
  {
        yyVal = Lisp.Cons(yyVals[0+yyTop]);
      }
  break;
case 69:
#line 674 "SQL92-min.y"
  {
		yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
	  }
  break;
case 70:
#line 682 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.AggExpr), Descriptor.Aggregate, new TokenWrapper(yyVals[-3+yyTop]), yyVals[-1+yyTop]);
      }
  break;
case 71:
#line 687 "SQL92-min.y"
  {
         Symbol sym = new Symbol(Tag.AggExpr);
         yyVal = notation.Confirm(sym, Descriptor.Aggregate, new TokenWrapper(yyVals[-4+yyTop]), yyVals[-1+yyTop]);
         if ((int)yyVals[-2+yyTop] == Token.DISTINCT)
            notation.Confirm(sym, Descriptor.Distinct);
      }
  break;
case 72:
#line 697 "SQL92-min.y"
  {
         yyVal = new Parameter(yyVals[0+yyTop].ToString()); 
      }
  break;
case 76:
#line 710 "SQL92-min.y"
  {
        yyVal = yyVals[0+yyTop];
		notation.ConfirmTag(Tag.SQuery, Descriptor.GroupingColumnRef, yyVals[0+yyTop]);
      }
  break;
case 77:
#line 718 "SQL92-min.y"
  {
		yyVal = Lisp.Cons(yyVals[0+yyTop]);
      }
  break;
case 78:
#line 722 "SQL92-min.y"
  {
		yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
      }
  break;
case 80:
#line 731 "SQL92-min.y"
  {
		notation.ConfirmTag(Tag.SQuery, Descriptor.GroupBy, yyVals[0+yyTop]);
      }
  break;
case 82:
#line 739 "SQL92-min.y"
  {
		notation.ConfirmTag(Tag.SQuery, Descriptor.Having, yyVals[0+yyTop]);
      }
  break;
case 86:
#line 756 "SQL92-min.y"
  {         
         Symbol sym = new Symbol(Tag.Predicate);
		 yyVal = notation.Confirm(sym, Descriptor.InSet, yyVals[-3+yyTop], yyVals[0+yyTop]);            
		 if (yyVals[-2+yyTop] != null)
		   notation.Confirm(sym, Descriptor.Inverse);
      }
  break;
case 88:
#line 767 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.ValueList), Descriptor.ValueList, yyVals[-1+yyTop]);
      }
  break;
case 89:
#line 774 "SQL92-min.y"
  {
         yyVal = Lisp.Cons(yyVals[0+yyTop]);
      }
  break;
case 90:
#line 778 "SQL92-min.y"
  {
         yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
      }
  break;
case 91:
#line 785 "SQL92-min.y"
  {
		notation.ConfirmTag(Tag.Join, Descriptor.CrossJoin, yyVals[-1+yyTop], yyVals[0+yyTop]);
		yyVal = notation.ResolveTag(Tag.Join);
      }
  break;
case 92:
#line 790 "SQL92-min.y"
  {
		notation.ConfirmTag(Tag.Join, Descriptor.UnionJoin, yyVals[-1+yyTop], yyVals[0+yyTop]);
		yyVal = notation.ResolveTag(Tag.Join);      
      }
  break;
case 93:
#line 795 "SQL92-min.y"
  {
		notation.ConfirmTag(Tag.Join, Descriptor.NaturalJoin, yyVals[-1+yyTop], yyVals[0+yyTop]);
		yyVal = notation.ResolveTag(Tag.Join);      
      }
  break;
case 94:
#line 800 "SQL92-min.y"
  {
		notation.ConfirmTag(Tag.Join, Descriptor.QualifiedJoin, yyVals[-1+yyTop], yyVals[0+yyTop]);
		yyVal = notation.ResolveTag(Tag.Join);      
      }
  break;
case 95:
#line 805 "SQL92-min.y"
  {
		yyVal = notation.Confirm(new Symbol(Tag.Join), Descriptor.Branch, yyVals[-1+yyTop]);
      }
  break;
case 97:
#line 816 "SQL92-min.y"
  {
		yyVal = yyVals[0+yyTop];
      }
  break;
case 104:
#line 835 "SQL92-min.y"
  {
		  notation.ConfirmTag(Tag.Join, Descriptor.Outer);
	  }
  break;
case 105:
#line 842 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.Predicate), 
			Descriptor.Like, yyVals[-2+yyTop], yyVals[0+yyTop]);           
      }
  break;
case 106:
#line 847 "SQL92-min.y"
  {               
         Symbol sym = new Symbol(Tag.Predicate);
         yyVal = notation.Confirm(sym, Descriptor.Like, yyVals[-2+yyTop], yyVals[0+yyTop]);           
		 notation.Confirm(sym, Descriptor.Inverse);
      }
  break;
case 107:
#line 853 "SQL92-min.y"
  {
         Symbol sym = new Symbol(Tag.Predicate);
         yyVal = notation.Confirm(sym, Descriptor.Like, yyVals[-4+yyTop], yyVals[-2+yyTop]);           
		 notation.Confirm(sym, Descriptor.Escape, yyVals[0+yyTop]);
      }
  break;
case 108:
#line 859 "SQL92-min.y"
  {
         Symbol sym = new Symbol(Tag.Predicate);
         yyVal = notation.Confirm(sym, Descriptor.Like, yyVals[-4+yyTop], yyVals[-2+yyTop]);           
         notation.Confirm(sym, Descriptor.Inverse);
		 notation.Confirm(sym, Descriptor.Escape, yyVals[0+yyTop]);
      }
  break;
case 109:
#line 871 "SQL92-min.y"
  {
          Symbol sym = new Symbol(Tag.Predicate);          
          yyVal = notation.Confirm(sym, Descriptor.Match, yyVals[-4+yyTop], yyVals[0+yyTop]);
		  if (yyVals[-2+yyTop] != null)
			notation.Confirm(sym, Descriptor.Unique);          
		  if (yyVals[-1+yyTop] != null)
			notation.Confirm(sym, Descriptor.MatchType, new TokenWrapper(yyVals[-1+yyTop]));
      }
  break;
case 110:
#line 883 "SQL92-min.y"
  {
        yyVal = null;
      }
  break;
case 112:
#line 891 "SQL92-min.y"
  {
        yyVal = null;
      }
  break;
case 116:
#line 904 "SQL92-min.y"
  {
		yyVal = notation.Confirm(new Symbol(Tag.Join), Descriptor.Using, yyVals[-1+yyTop]);
      }
  break;
case 117:
#line 911 "SQL92-min.y"
  {
		yyVal = notation.Confirm(new Symbol(Tag.Join), Descriptor.Constraint, new TokenWrapper(yyVals[-1+yyTop]));
      }
  break;
case 118:
#line 915 "SQL92-min.y"
  {
        yyVal = notation.Confirm(new Symbol(Tag.Join), Descriptor.Constraint, new TokenWrapper(yyVals[-1+yyTop]));
      }
  break;
case 119:
#line 919 "SQL92-min.y"
  {
        yyVal = notation.Confirm(new Symbol(Tag.Join), Descriptor.Constraint, yyVals[0+yyTop]);
      }
  break;
case 121:
#line 927 "SQL92-min.y"
  {         
         Symbol sym = new Symbol(Tag.QueryExp);     
         switch((int)yyVals[-3+yyTop])
         {
            case Token.UNION:
               notation.Confirm(sym, Descriptor.Union, yyVals[-4+yyTop], yyVals[0+yyTop]);
               break;
            case Token.EXCEPT:
               notation.Confirm(sym, Descriptor.Except, yyVals[-4+yyTop], yyVals[0+yyTop]);
               break;
         }
         if (yyVals[-2+yyTop] != null)
           notation.Confirm(sym, Descriptor.Distinct);
         yyVal = sym;
      }
  break;
case 124:
#line 951 "SQL92-min.y"
  {
        yyVal = null;
      }
  break;
case 126:
#line 959 "SQL92-min.y"
  {
		yyVal = null;
      }
  break;
case 129:
#line 968 "SQL92-min.y"
  {
         Symbol sym = new Symbol(Tag.QueryTerm);
         notation.Confirm(sym, Descriptor.Intersect, yyVals[-4+yyTop], yyVals[0+yyTop]); 
         if (yyVals[-2+yyTop] != null)
           notation.Confirm(sym, Descriptor.Distinct);
         yyVal = sym;
      }
  break;
case 130:
#line 980 "SQL92-min.y"
  {
        Symbol sym = new Symbol(Tag.Predicate);
        yyVal = notation.Confirm(sym, Descriptor.IsNull, yyVals[-3+yyTop]);
        if (yyVals[-1+yyTop] != null)
          notation.Confirm(sym, Descriptor.Inverse);
      }
  break;
case 136:
#line 1001 "SQL92-min.y"
  {
		  Symbol sym = new Symbol(Tag.Expr);
          if (yyVals[-1+yyTop].Equals("+"))
			yyVal = notation.Confirm(sym, Descriptor.Add, yyVals[-2+yyTop], yyVals[0+yyTop]);
		  else
		    yyVal = notation.Confirm(sym, Descriptor.Sub, yyVals[-2+yyTop], yyVals[0+yyTop]);
      }
  break;
case 141:
#line 1022 "SQL92-min.y"
  {
			yyVal  = yyVals[0+yyTop];
	  }
  break;
case 145:
#line 1036 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.Predicate), Descriptor.Overlaps, yyVals[-2+yyTop], yyVals[0+yyTop]);
      }
  break;
case 149:
#line 1053 "SQL92-min.y"
  {
           yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.PosString, yyVals[-1+yyTop], yyVals[-3+yyTop]);
       }
  break;
case 160:
#line 1073 "SQL92-min.y"
  {
		yyVal = yyVals[-1+yyTop];
		if (yyVals[-3+yyTop] != null)
			notation.ConfirmTag(Tag.Join, Descriptor.JoinType, new TokenWrapper(yyVals[-3+yyTop]));		
		notation.ConfirmTag(Tag.Join, Descriptor.JoinSpec, yyVals[0+yyTop]);		
	  }
  break;
case 161:
#line 1083 "SQL92-min.y"
  {
		yyVal = null;
	  }
  break;
case 163:
#line 1092 "SQL92-min.y"
  {
		  yyVal = notation.Confirm(new Symbol(Tag.Predicate), 
		    Descriptor.QuantifiedPred, yyVals[-3+yyTop], yyVals[-2+yyTop], new TokenWrapper(yyVals[-1+yyTop]), yyVals[0+yyTop]);	  
      }
  break;
case 167:
#line 1107 "SQL92-min.y"
  {
           if ((int)yyVals[-2+yyTop] == Token.DISTINCT)
              notation.ConfirmTag(Tag.SQuery, Descriptor.Distinct);      
           if (yyVals[-3+yyTop] != null)
              notation.ConfirmTag(Tag.SQuery, Descriptor.Top, yyVals[-3+yyTop]);      
           yyVal = notation.ResolveTag(Tag.SQuery);           
           notation.LeaveContext();
      }
  break;
case 168:
#line 1119 "SQL92-min.y"
  {
         yyVal = null;
      }
  break;
case 169:
#line 1123 "SQL92-min.y"
  {
         yyVal = yyVals[-1+yyTop]; 
      }
  break;
case 174:
#line 1141 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.RowValue), 
            Descriptor.RowValue, Lisp.Append(Lisp.Cons(yyVals[-3+yyTop]), yyVals[-1+yyTop]));
      }
  break;
case 176:
#line 1151 "SQL92-min.y"
  { 
		yyVal = new TokenWrapper(yyVals[0+yyTop]);
	  }
  break;
case 177:
#line 1155 "SQL92-min.y"
  {
        yyVal = new TokenWrapper(yyVals[0+yyTop]);
      }
  break;
case 178:
#line 1162 "SQL92-min.y"
  {
        yyVal = Lisp.Cons(yyVals[0+yyTop]);
      }
  break;
case 179:
#line 1166 "SQL92-min.y"
  {
        yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
      }
  break;
case 180:
#line 1175 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.CaseExpr), Descriptor.Case, yyVals[-1+yyTop]);         
      }
  break;
case 181:
#line 1182 "SQL92-min.y"
  {
         object clause_list = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[-1+yyTop]));
         yyVal = notation.Confirm(new Symbol(Tag.CaseExpr), Descriptor.Case, clause_list);         
      }
  break;
case 182:
#line 1190 "SQL92-min.y"
  {
         yyVal = Lisp.Cons(yyVals[0+yyTop]);
      }
  break;
case 183:
#line 1194 "SQL92-min.y"
  {
         yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(yyVals[0+yyTop]));
      }
  break;
case 184:
#line 1201 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.CaseExpr), Descriptor.CaseBranch, yyVals[-2+yyTop], yyVals[0+yyTop]);
      }
  break;
case 187:
#line 1213 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.BooleanExpr), Descriptor.LogicalOR, yyVals[-2+yyTop], yyVals[0+yyTop]);
      }
  break;
case 190:
#line 1225 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.BooleanExpr), Descriptor.LogicalAND, yyVals[-2+yyTop], yyVals[0+yyTop]);
      }
  break;
case 193:
#line 1237 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.BooleanExpr), Descriptor.Inverse, yyVals[0+yyTop]);
      }
  break;
case 195:
#line 1245 "SQL92-min.y"
  {
         Symbol sym = new Symbol(Tag.BooleanExpr);
         yyVal = notation.Confirm(sym, Descriptor.BooleanTest, new TokenWrapper(yyVals[0+yyTop]), yyVals[-3+yyTop]);
		 if (yyVals[-1+yyTop] != null)
		   notation.Confirm(sym, Descriptor.Inverse);                  
      }
  break;
case 197:
#line 1256 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.BooleanExpr),
           Descriptor.Branch, yyVals[-1+yyTop]);
      }
  break;
case 201:
#line 1270 "SQL92-min.y"
  {
          notation.ConfirmTag(Tag.SQuery, Descriptor.Select, null); 
      }
  break;
case 202:
#line 1274 "SQL92-min.y"
  {
          notation.ConfirmTag(Tag.SQuery, Descriptor.Select, yyVals[0+yyTop]); 
      }
  break;
case 203:
#line 1281 "SQL92-min.y"
  {
          yyVal = Lisp.Cons(yyVals[0+yyTop]);
      }
  break;
case 204:
#line 1285 "SQL92-min.y"
  {
         yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
      }
  break;
case 205:
#line 1292 "SQL92-min.y"
  {		
		 yyVal = notation.Confirm(new Symbol(Tag.TableFields), Descriptor.TableFields, yyVals[-1+yyTop]);   
      }
  break;
case 207:
#line 1300 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.AggExpr), Descriptor.AggCount);
      }
  break;
case 215:
#line 1317 "SQL92-min.y"
  {
           notation.EnterContext();
           yyVal = Token.ALL;
        }
  break;
case 216:
#line 1322 "SQL92-min.y"
  {
           notation.EnterContext();
           yyVal = yyVals[0+yyTop];
        }
  break;
case 221:
#line 1342 "SQL92-min.y"
  {
          yyVal = notation.Confirm(new Symbol(Tag.CaseExpr), Descriptor.Case, yyVals[-2+yyTop], yyVals[-1+yyTop]);              
      }
  break;
case 222:
#line 1349 "SQL92-min.y"
  {
         object clause_list = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[-1+yyTop]));
         yyVal = notation.Confirm(new Symbol(Tag.CaseExpr), Descriptor.Case, yyVals[-3+yyTop], clause_list);
      }
  break;
case 223:
#line 1357 "SQL92-min.y"
  {
         yyVal = Lisp.Cons(yyVals[0+yyTop]);
      }
  break;
case 224:
#line 1361 "SQL92-min.y"
  {
         yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(yyVals[0+yyTop]));
      }
  break;
case 227:
#line 1374 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.CaseExpr), Descriptor.CaseBranch, yyVals[-2+yyTop], yyVals[0+yyTop]);
      }
  break;
case 229:
#line 1382 "SQL92-min.y"
  {
			yyVal = new IntegerValue(yyVals[0+yyTop]);
      }
  break;
case 230:
#line 1390 "SQL92-min.y"
  {
        yyVal = yyVals[-1+yyTop]; 
        if ((int)yyVals[0+yyTop] == Token.DESC) 
			notation.Confirm((Symbol)yyVals[-1+yyTop], Descriptor.Desc);
      }
  break;
case 231:
#line 1399 "SQL92-min.y"
  {
        yyVal = Token.ASC;
      }
  break;
case 233:
#line 1407 "SQL92-min.y"
  {
			yyVal = Lisp.Cons(yyVals[0+yyTop]);
      }
  break;
case 234:
#line 1411 "SQL92-min.y"
  {
            yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
      }
  break;
case 238:
#line 1430 "SQL92-min.y"
  {
         yyVal = yyVals[-1+yyTop];
         if (yyVals[-2+yyTop] != null)
           notation.Confirm((Symbol)yyVals[-1+yyTop], Descriptor.OptimizerHint, yyVals[-2+yyTop]);
      }
  break;
case 239:
#line 1436 "SQL92-min.y"
  {
         yyVal = yyVals[-2+yyTop];
         notation.Confirm((Symbol)yyVals[-2+yyTop], Descriptor.Order, yyVals[-1+yyTop]);  
         if (yyVals[-3+yyTop] != null)
           notation.Confirm((Symbol)yyVals[-2+yyTop], Descriptor.OptimizerHint, yyVals[-3+yyTop]);         
      }
  break;
case 245:
#line 1460 "SQL92-min.y"
  {
		yyVal = yyVals[-1+yyTop];
		notation.Confirm((Symbol)yyVal, Descriptor.Alias, yyVals[0+yyTop]);
    }
  break;
case 249:
#line 1474 "SQL92-min.y"
  {
        yyVal = notation.Confirm(new Symbol(Tag.Dynatable), Descriptor.Dynatable, yyVals[0+yyTop]);
    }
  break;
case 250:
#line 1478 "SQL92-min.y"
  {
        yyVal = notation.Confirm(new Symbol(Tag.Dynatable), Descriptor.Dynatable, yyVals[0+yyTop]);
    }
  break;
case 251:
#line 1482 "SQL92-min.y"
  {
        yyVal = notation.Confirm(new Symbol(Tag.Dynatable), Descriptor.Dynatable, yyVals[-2+yyTop]);
    }
  break;
case 252:
#line 1486 "SQL92-min.y"
  {
		yyVal = notation.Confirm(new Symbol(Tag.Dynatable), Descriptor.Dynatable, yyVals[0+yyTop]);
    }
  break;
case 253:
#line 1493 "SQL92-min.y"
  {
		yyVal = new Qname(yyVals[0+yyTop]);
    }
  break;
case 254:
#line 1498 "SQL92-min.y"
  {
		yyVal = new Qname(yyVals[-3+yyTop]);
		notation.Confirm((Symbol)yyVal, Descriptor.DerivedColumns, yyVals[-1+yyTop]);
    }
  break;
case 255:
#line 1506 "SQL92-min.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.TableConstructor), 
          Descriptor.TableValue, yyVals[0+yyTop]);
    }
  break;
case 256:
#line 1514 "SQL92-min.y"
  {
      yyVal = Lisp.Cons(yyVals[0+yyTop]);
    }
  break;
case 257:
#line 1518 "SQL92-min.y"
  {
      yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
    }
  break;
case 259:
#line 1526 "SQL92-min.y"
  {
        yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Mul, yyVals[-2+yyTop], yyVals[0+yyTop]);
    }
  break;
case 260:
#line 1530 "SQL92-min.y"
  {
        yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Div, yyVals[-2+yyTop], yyVals[0+yyTop]);
    }
  break;
case 261:
#line 1537 "SQL92-min.y"
  { 
        yyVal = yyVals[-1+yyTop];
      }
  break;
case 262:
#line 1544 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.CExpr), Descriptor.StringTrim, 
			new TokenWrapper(Token.BOTH), new Literal(" "), yyVals[0+yyTop]);
      }
  break;
case 263:
#line 1549 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.CExpr), Descriptor.StringTrim, 
            new TokenWrapper(Token.BOTH), new Literal(" "), yyVals[0+yyTop]);
      }
  break;
case 264:
#line 1554 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.CExpr), Descriptor.StringTrim, 
            new TokenWrapper(Token.BOTH), yyVals[-2+yyTop], yyVals[0+yyTop]);
      }
  break;
case 265:
#line 1559 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.CExpr), Descriptor.StringTrim, 
			new TokenWrapper(yyVals[-2+yyTop]), new Literal(" "), yyVals[0+yyTop]);
      }
  break;
case 266:
#line 1564 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.CExpr), Descriptor.StringTrim, 
			new TokenWrapper(yyVals[-3+yyTop]), yyVals[-2+yyTop], yyVals[0+yyTop]);
      }
  break;
case 272:
#line 1586 "SQL92-min.y"
  {
        yyVal = notation.Confirm(new Symbol(Tag.Predicate), Descriptor.Unique, yyVals[0+yyTop]);
      }
  break;
case 284:
#line 1610 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Branch, yyVals[-1+yyTop]);
      }
  break;
case 285:
#line 1617 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.Funcall), Descriptor.Funcall, new Qname(yyVals[-2+yyTop]), null);
      }
  break;
case 286:
#line 1621 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.Funcall), Descriptor.Funcall, new Qname(yyVals[-3+yyTop]), yyVals[-1+yyTop]);
      }
  break;
case 289:
#line 1633 "SQL92-min.y"
  {
        notation.ConfirmTag(Tag.SQuery, Descriptor.Where, yyVals[0+yyTop]);
    }
  break;
case 290:
#line 1640 "SQL92-min.y"
  {
       yyVal = null;
    }
  break;
case 292:
#line 1648 "SQL92-min.y"
  {
       yyVal = Lisp.Cons(yyVals[0+yyTop]);
    }
  break;
case 293:
#line 1652 "SQL92-min.y"
  {
       yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(yyVals[0+yyTop])); 
    }
  break;
case 294:
#line 1659 "SQL92-min.y"
  {
	   yyVal = new Qname(yyVals[0+yyTop]);
    }
  break;
case 295:
#line 1666 "SQL92-min.y"
  {
       yyVal = yyVals[0+yyTop];
       notation.ConfirmTag(Tag.SQuery, Descriptor.TableName, yyVals[0+yyTop]);
    }
  break;
case 296:
#line 1671 "SQL92-min.y"
  {      
       notation.ConfirmTag(Tag.SQuery, Descriptor.TableName, yyVals[0+yyTop]);
    }
  break;
case 297:
#line 1678 "SQL92-min.y"
  {
       yyVal = yyVals[0+yyTop];
       notation.Confirm((Symbol)yyVal, Descriptor.Prefix, new Literal(yyVals[-2+yyTop]));       
    }
  break;
case 299:
#line 1687 "SQL92-min.y"
  {
       yyVal = yyVals[-2+yyTop];
       ((Qname)yyVals[-2+yyTop]).Append((String)yyVals[0+yyTop]);
    }
  break;
case 300:
#line 1695 "SQL92-min.y"
  {
       yyVal = yyVals[0+yyTop];
       notation.Confirm((Symbol)yyVals[0+yyTop], Descriptor.ColumnRef);
    }
  break;
case 302:
#line 1704 "SQL92-min.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Dref), Descriptor.Dref, yyVals[-2+yyTop], new Literal((String)yyVals[0+yyTop]));
    }
  break;
case 303:
#line 1708 "SQL92-min.y"
  {       
       Notation.Record[] recs = notation.Select((Symbol)yyVals[0+yyTop], Descriptor.Funcall, 2);       
       if (recs.Length > 0)
       {
          yyVal = notation.Confirm(new Symbol(Tag.Funcall), Descriptor.Funcall, 
             recs[0].Arg0, Lisp.Append(Lisp.Cons(yyVals[-2+yyTop]), recs[0].args[1]));
          notation.Remove(recs);
       }
       else
          throw new InvalidOperationException();
    }
  break;
case 304:
#line 1720 "SQL92-min.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Dref), Descriptor.At, yyVals[-3+yyTop], yyVals[-1+yyTop]);
    }
  break;
case 305:
#line 1724 "SQL92-min.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Dref), Descriptor.Wref, yyVals[-2+yyTop], new Literal((String)yyVals[0+yyTop]));    
    }
  break;
case 306:
#line 1731 "SQL92-min.y"
  {
      yyVal = new Qname(yyVals[0+yyTop]);
    }
  break;
case 307:
#line 1738 "SQL92-min.y"
  {
      yyVal = new IntegerValue(yyVals[0+yyTop]);
    }
  break;
case 308:
#line 1742 "SQL92-min.y"
  {
      yyVal = new DecimalValue(yyVals[0+yyTop]);
    }
  break;
case 309:
#line 1746 "SQL92-min.y"
  {
      yyVal = new DoubleValue(yyVals[0+yyTop]);
    }
  break;
case 310:
#line 1750 "SQL92-min.y"
  {
      yyVal = new Literal(yyVals[0+yyTop]);
    }
  break;
case 320:
#line 1770 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLQuery, yyVals[-2+yyTop], null, yyVals[-1+yyTop]);     
      }
  break;
case 321:
#line 1774 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLQuery, yyVals[-4+yyTop], yyVals[-2+yyTop], yyVals[-1+yyTop]);     
      }
  break;
case 322:
#line 1778 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLQuery, yyVals[-6+yyTop], yyVals[-2+yyTop], yyVals[-1+yyTop]);     
      }
  break;
case 323:
#line 1785 "SQL92-min.y"
  {
         yyVal = null;
      }
  break;
case 324:
#line 1789 "SQL92-min.y"
  {
         yyVal = new TokenWrapper(Token.RETURNING_CONTENT);
      }
  break;
case 325:
#line 1793 "SQL92-min.y"
  {
         yyVal = new TokenWrapper(Token.RETURNING_SEQUENCE);
      }
  break;
case 326:
#line 1800 "SQL92-min.y"
  {
         yyVal = Lisp.Cons(yyVals[0+yyTop]);
      }
  break;
case 327:
#line 1804 "SQL92-min.y"
  {
         yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
      }
  break;
case 328:
#line 1811 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.AggExpr), Descriptor.Aggregate, 
            new TokenWrapper(Token.XMLAGG), yyVals[-1+yyTop]);
      }
  break;
case 329:
#line 1816 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.AggExpr), Descriptor.Aggregate, 
            new TokenWrapper(Token.XMLAGG), yyVals[-2+yyTop]);
         notation.Confirm((Symbol)yyVal, Descriptor.Order, yyVals[-1+yyTop]);
      }
  break;
case 330:
#line 1825 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.SQLX), 
            Descriptor.XMLConcat, yyVals[-1+yyTop]); 
      }
  break;
case 331:
#line 1833 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLForestAll);               
      }
  break;
case 332:
#line 1837 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.SQLX), 
            Descriptor.XMLForest, yyVals[-1+yyTop]);         
      }
  break;
case 333:
#line 1842 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.SQLX), 
            Descriptor.XMLForest, yyVals[-1+yyTop]);  
         notation.Confirm((Symbol)yyVal, Descriptor.XMLNamespaces, yyVals[-3+yyTop]);
      }
  break;
case 334:
#line 1851 "SQL92-min.y"
  {
          String spec = yyVals[-3+yyTop].ToString().ToUpperInvariant();
          if (spec.Equals("DOCUMENT") || spec.Equals("CONTENT"))        
			 yyVal = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLParse, new Literal(spec), yyVals[-2+yyTop], yyVals[-1+yyTop]);
	      else
	      {
	         yyerror(String.Format(Properties.Resources.SyntaxError, 
	            String.Format("XMLPARSE({0}...", spec)));
	         throw new yyParser.yyException("Syntax error");
	      }      
      }
  break;
case 335:
#line 1866 "SQL92-min.y"
  {
          String spec = (String)yyVals[-2+yyTop];  
          if (! spec.Equals("NAME", StringComparison.InvariantCultureIgnoreCase))
          {
	         yyerror(String.Format(Properties.Resources.SyntaxError, 
	            String.Format("XMLPI({0}...", spec)));          
             throw new yyParser.yyException("Syntax error");	      
          }
          yyVal = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLPI, yyVals[-1+yyTop], null);
      }
  break;
case 336:
#line 1877 "SQL92-min.y"
  {
          String spec = (String)yyVals[-4+yyTop];  
          if (! spec.Equals("NAME", StringComparison.InvariantCultureIgnoreCase))
          {
	         yyerror(String.Format(Properties.Resources.SyntaxError, 
	            String.Format("XMLPI({0}...", spec)));                    
             throw new yyParser.yyException("Syntax error");	            
          }
          yyVal = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLPI, yyVals[-3+yyTop], yyVals[-1+yyTop]);      
      }
  break;
case 337:
#line 1891 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLComment, yyVals[-1+yyTop]);
      }
  break;
case 338:
#line 1898 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLCDATA, yyVals[-1+yyTop]);
      }
  break;
case 339:
#line 1905 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLRoot, yyVals[-1+yyTop], null, null);
      }
  break;
case 340:
#line 1909 "SQL92-min.y"
  {
         String tok = (String)yyVals[-2+yyTop];
         if (! tok.Equals("VERSION", StringComparison.InvariantCultureIgnoreCase))
         {
	         yyerror(String.Format(Properties.Resources.SyntaxError, 
	            String.Format("XMLRoot(... {0} ...", tok)));                             
              throw new yyParser.yyException("Syntax error");	                     
         }
         yyVal = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLRoot, yyVals[-4+yyTop], new Literal(yyVals[-1+yyTop]), null);
      }
  break;
case 341:
#line 1920 "SQL92-min.y"
  {
         String tok = (String)yyVals[-5+yyTop];
         if (! tok.Equals("VERSION", StringComparison.InvariantCultureIgnoreCase))
         {
	         yyerror(String.Format(Properties.Resources.SyntaxError, 
	            String.Format("XMLRoot(... {0} ...", tok)));                             
              throw new yyParser.yyException("Syntax error");	                     
         }
         tok = (String)yyVals[-2+yyTop];
         if (! tok.Equals("STANDALONE", StringComparison.InvariantCultureIgnoreCase))
         {
	         yyerror(String.Format(Properties.Resources.SyntaxError, 
	            String.Format("XMLRoot(... {0} ...", tok)));                             
              throw new yyParser.yyException("Syntax error");	                     
         }
         yyVal = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLRoot, yyVals[-7+yyTop], new Literal(yyVals[-4+yyTop]), yyVals[-1+yyTop]);
      }
  break;
case 343:
#line 1942 "SQL92-min.y"
  {
         yyVal = null;
      }
  break;
case 344:
#line 1949 "SQL92-min.y"
  {
         String tok = (String)yyVals[0+yyTop];
         if (tok.Equals("NO", StringComparison.InvariantCultureIgnoreCase) ||
             tok.Equals("YES", StringComparison.InvariantCultureIgnoreCase)) 
			yyVal = new Qname(tok.ToUpperInvariant());
         else
            throw new yyParser.yyException(String.Format(Properties.Resources.SyntaxError, 
               "STANDALONE value must be YES|NO|NO VALUE"));			
      }
  break;
case 345:
#line 1959 "SQL92-min.y"
  {
         yyVal = null;
      }
  break;
case 346:
#line 1966 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLElement, yyVals[-1+yyTop], null);
      }
  break;
case 347:
#line 1970 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLElement, yyVals[-3+yyTop], null);
         notation.Confirm((Symbol)yyVal, Descriptor.XMLNamespaces, yyVals[-1+yyTop]);
      }
  break;
case 348:
#line 1975 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLElement, yyVals[-3+yyTop], null);
         notation.Confirm((Symbol)yyVal, Descriptor.XMLAttributes, yyVals[-1+yyTop]);
      }
  break;
case 349:
#line 1981 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLElement, yyVals[-5+yyTop], null);
         notation.Confirm((Symbol)yyVal, Descriptor.XMLNamespaces, yyVals[-3+yyTop]);
         notation.Confirm((Symbol)yyVal, Descriptor.XMLAttributes, yyVals[-1+yyTop]);
      }
  break;
case 350:
#line 1987 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLElement, yyVals[-3+yyTop], yyVals[-1+yyTop]);         
      }
  break;
case 351:
#line 1991 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLElement, yyVals[-5+yyTop], yyVals[-1+yyTop]);
         notation.Confirm((Symbol)yyVal, Descriptor.XMLNamespaces, yyVals[-3+yyTop]);
      }
  break;
case 352:
#line 1996 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLElement, yyVals[-5+yyTop], yyVals[-1+yyTop]);
         notation.Confirm((Symbol)yyVal, Descriptor.XMLAttributes, yyVals[-3+yyTop]);
      }
  break;
case 353:
#line 2002 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLElement, yyVals[-7+yyTop], yyVals[-1+yyTop]);
         notation.Confirm((Symbol)yyVal, Descriptor.XMLNamespaces, yyVals[-5+yyTop]);
         notation.Confirm((Symbol)yyVal, Descriptor.XMLAttributes, yyVals[-3+yyTop]);      
      }
  break;
case 354:
#line 2011 "SQL92-min.y"
  {
         yyVal = yyVals[-1+yyTop];
      }
  break;
case 355:
#line 2018 "SQL92-min.y"
  {
         yyVal = Lisp.Cons(yyVals[0+yyTop]);
      }
  break;
case 356:
#line 2022 "SQL92-min.y"
  {
         yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
      }
  break;
case 359:
#line 2034 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.SQLX), 
            Descriptor.DeclNamespace, new Literal(yyVals[-2+yyTop]), yyVals[0+yyTop]);  
      }
  break;
case 360:
#line 2042 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.SQLX), 
            Descriptor.DeclNamespace, new Literal(yyVals[0+yyTop]), null);        
      }
  break;
case 361:
#line 2047 "SQL92-min.y"
  {
         yyVal = null;
      }
  break;
case 362:
#line 2054 "SQL92-min.y"
  {
         yyVal = null;
      }
  break;
case 363:
#line 2058 "SQL92-min.y"
  {
         yyVal = yyVals[-1+yyTop];
      }
  break;
case 364:
#line 2065 "SQL92-min.y"
  {
          yyVal = Lisp.Cons(yyVals[0+yyTop]);
      }
  break;
case 365:
#line 2069 "SQL92-min.y"
  {
          yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
      }
  break;
case 367:
#line 2077 "SQL92-min.y"
  {
         yyVal = yyVals[-1+yyTop];
         notation.Confirm((Symbol)yyVal, Descriptor.ContentOption, yyVals[0+yyTop]);
      }
  break;
case 368:
#line 2082 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.TableFields), Descriptor.TableFields, yyVals[-1+yyTop]);   
      }
  break;
case 369:
#line 2086 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.TableFields), Descriptor.TableFields, yyVals[-2+yyTop]);   
         notation.Confirm((Symbol)yyVal, Descriptor.ContentOption, yyVals[0+yyTop]);
      }
  break;
case 371:
#line 2095 "SQL92-min.y"
  {
         yyVal = yyVals[-1+yyTop];
         notation.Confirm((Symbol)yyVal, Descriptor.Alias, yyVals[0+yyTop]);
      }
  break;
case 372:
#line 2103 "SQL92-min.y"
  { 
         yyVal = null;
      }
  break;
case 374:
#line 2111 "SQL92-min.y"
  {
          yyVal = new TokenWrapper(Token.PRESERVE_WHITESPACE);
      }
  break;
case 375:
#line 2115 "SQL92-min.y"
  {
          yyVal = new TokenWrapper(Token.STRIP_WHITESPACE);
      }
  break;
case 376:
#line 2122 "SQL92-min.y"
  {
         yyVal = new TokenWrapper(Token.OPTION_NULL);
      }
  break;
case 377:
#line 2126 "SQL92-min.y"
  {
         yyVal = new TokenWrapper(Token.OPTION_EMPTY);
      }
  break;
case 378:
#line 2130 "SQL92-min.y"
  {
         yyVal = new TokenWrapper(Token.OPTION_ABSENT);
      }
  break;
case 379:
#line 2134 "SQL92-min.y"
  {
         yyVal = new TokenWrapper(Token.OPTION_NIL);
      }
  break;
case 380:
#line 2138 "SQL92-min.y"
  {
         yyVal = new TokenWrapper(Token.NO_CONTENT);
      }
  break;
case 383:
#line 2150 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.Expr), 
            Descriptor.Cast, yyVals[-3+yyTop], yyVals[-1+yyTop]);                 
      }
  break;
case 384:
#line 2158 "SQL92-min.y"
  {
         yyVal = null;
      }
  break;
case 388:
#line 2168 "SQL92-min.y"
  {
         yyVal = new TokenWrapper(yyVals[0+yyTop]);
      }
  break;
case 390:
#line 2176 "SQL92-min.y"
  {
         yyVal = new TokenWrapper(yyVals[0+yyTop]);
      }
  break;
case 391:
#line 2180 "SQL92-min.y"
  {
         yyVal = new TokenWrapper(yyVals[-3+yyTop]);
         notation.Confirm((Symbol)yyVal, Descriptor.Typelen, yyVals[-1+yyTop]);
      }
  break;
case 399:
#line 2198 "SQL92-min.y"
  {
         yyVal = new TokenWrapper(yyVals[0+yyTop]);
      }
  break;
case 405:
#line 2213 "SQL92-min.y"
  {
         yyVal = new TokenWrapper(yyVals[0+yyTop]);
      }
  break;
case 406:
#line 2217 "SQL92-min.y"
  {
         yyVal = new TokenWrapper(yyVals[-3+yyTop]);
         notation.Confirm((Symbol)yyVal, Descriptor.Typeprec, yyVals[-1+yyTop]);         
      }
  break;
case 407:
#line 2222 "SQL92-min.y"
  {
         yyVal = new TokenWrapper(yyVals[-5+yyTop]);
         notation.Confirm((Symbol)yyVal, Descriptor.Typeprec, yyVals[-3+yyTop]);
         notation.Confirm((Symbol)yyVal, Descriptor.Typescale, yyVals[-1+yyTop]);         
      }
  break;
case 411:
#line 2237 "SQL92-min.y"
  {
         yyVal = new TokenWrapper(Token.FLOAT);
      }
  break;
case 412:
#line 2241 "SQL92-min.y"
  {
         yyVal = new TokenWrapper(Token.FLOAT);
         notation.Confirm((Symbol)yyVal, Descriptor.Typeprec, yyVals[-1+yyTop]);         
      }
  break;
case 413:
#line 2249 "SQL92-min.y"
  {
         yyVal = new Qname(yyVals[0+yyTop]);
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
    0,    0,    4,    7,    9,    9,   10,   11,   11,   12,
   13,   15,   15,   16,   16,   18,   17,   17,   21,   22,
   22,   25,   28,   28,   26,   26,   30,   30,   30,   30,
   34,   34,   35,   35,   35,   35,   35,   35,   36,   37,
   38,   39,   39,   40,   41,   43,   44,   46,   47,   47,
   48,   48,   50,    5,    5,   51,   52,   54,   55,   56,
   56,   31,   59,   59,   32,   60,   62,   63,   63,   64,
   64,   67,   67,   67,   69,   70,   72,   72,   73,   73,
   74,   74,   76,   76,   77,   78,   79,   79,   80,   80,
   81,   81,   81,   81,   81,   84,   85,   86,   86,   86,
   89,   89,   91,   91,   92,   92,   92,   92,   94,   95,
   95,   96,   96,   97,   97,   87,   88,   88,   88,    2,
    2,   99,   99,  100,  100,  101,  101,   98,   98,  103,
   57,   57,   57,  105,   23,   23,  109,  109,  110,  110,
    3,   90,   90,   90,  112,   68,   68,   93,  107,  113,
  113,  113,  113,  113,  113,  113,  113,  113,  113,   83,
   45,   45,  114,  116,  116,  116,  117,  118,  118,   53,
   53,  122,    8,    8,  123,  123,  123,  124,  124,   20,
   20,  125,  125,  126,   75,  127,  127,  129,  128,  128,
  131,  130,  130,  132,  132,  133,  133,  134,  134,  134,
  120,  120,  135,  135,  136,  136,  137,  137,  137,   65,
   65,   65,   65,   65,  119,  119,   66,   66,   58,   58,
   19,   19,  139,  139,  102,  102,  140,  143,  143,  144,
  145,  145,  111,  111,   27,   29,   24,   49,   49,  121,
  121,   42,   42,   82,   82,  147,  147,  147,  150,  150,
  150,  150,  148,  148,  141,  153,  153,  108,  108,  108,
   33,  154,  154,  154,  154,  154,  156,  155,  157,  157,
  157,  115,  158,  158,   14,  104,  104,  104,  104,  104,
  104,  104,  104,  104,  151,  151,  142,  146,  146,    1,
    1,  162,  162,    6,  149,  149,  160,  163,  163,   71,
  164,  164,  164,  164,  164,   61,  159,  159,  159,  159,
  106,  106,  106,  106,  106,  106,  106,  106,  106,  152,
  152,  152,  173,  173,  173,  174,  174,  138,  138,  167,
  170,  170,  170,  169,  165,  165,  166,  171,  172,  172,
  172,  178,  178,  179,  179,  168,  168,  168,  168,  168,
  168,  168,  168,  176,  181,  181,  182,  182,  183,  184,
  184,  180,  180,  175,  175,  185,  185,  185,  185,  186,
  186,  177,  177,  188,  188,  187,  187,  187,  187,  187,
  189,  189,  161,  190,  190,  191,  191,  191,  191,  192,
  192,  195,  195,  195,  195,  195,  193,  193,  193,  198,
  198,  198,  198,  198,  196,  196,  196,  199,  199,  199,
  197,  197,  194,
  };
   static  short [] yyLen = {           2,
    2,    3,    2,    6,    0,    1,    1,    1,    1,    6,
    4,    1,    3,    1,    1,    1,    1,    1,    1,    1,
    1,    7,    0,    2,    1,    3,    1,    1,    1,    1,
    1,    3,    1,    1,    1,    1,    1,    1,    3,    1,
    1,    1,    2,    4,    2,    2,    4,    1,    1,    2,
    1,    2,    1,    0,    1,    1,    2,    1,    2,    1,
    2,    4,    1,    1,    6,    1,    2,    1,    3,    4,
    5,    1,    1,    1,    1,    1,    1,    3,    0,    3,
    0,    2,    1,    2,    1,    4,    1,    3,    1,    3,
    2,    2,    2,    2,    3,    1,    2,    1,    1,    1,
    1,    2,    0,    1,    3,    3,    5,    5,    5,    0,
    1,    0,    1,    1,    1,    4,    3,    3,    3,    1,
    5,    1,    1,    0,    1,    0,    1,    1,    5,    4,
    1,    1,    1,    1,    1,    3,    1,    1,    1,    1,
    3,    1,    1,    1,    3,    1,    2,    1,    6,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    4,
    0,    1,    4,    1,    1,    1,    5,    0,    4,    1,
    1,    1,    1,    5,    1,    1,    1,    1,    3,    3,
    4,    1,    2,    4,    1,    1,    3,    1,    1,    3,
    1,    1,    2,    1,    4,    1,    3,    1,    1,    1,
    1,    1,    1,    3,    2,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    0,    1,    1,    1,    1,    1,
    4,    5,    1,    2,    1,    1,    4,    1,    1,    2,
    0,    1,    1,    3,    1,    1,    1,    4,    5,    0,
    4,    1,    1,    1,    2,    1,    1,    1,    2,    2,
    4,    2,    2,    5,    2,    1,    3,    1,    3,    3,
    4,    1,    2,    3,    3,    4,    1,    1,    1,    1,
    1,    2,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    3,    3,    4,    1,    0,    2,    0,
    1,    1,    2,    1,    1,    1,    3,    1,    3,    1,
    1,    3,    3,    4,    3,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    5,
    7,    9,    0,    1,    1,    1,    3,    4,    5,    4,
    1,    4,    6,    6,    5,    7,    4,    4,    4,    7,
   10,    1,    1,    1,    1,    4,    6,    6,    8,    6,
    8,    8,   10,    4,    1,    3,    1,    1,    3,    2,
    1,    1,    4,    1,    3,    1,    2,    2,    3,    1,
    2,    0,    1,    1,    1,    2,    2,    2,    2,    3,
    0,    2,    6,    1,    1,    1,    1,    1,    1,    1,
    4,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    4,    6,    1,    1,    1,
    1,    4,    1,
  };
   static  short [] yyDefRed = {            0,
  292,    0,    0,    0,    0,    0,    0,    0,  128,  225,
  226,  293,    0,    0,  210,    0,    0,    0,  214,    0,
   48,   64,  211,  212,   19,    0,    0,    0,  213,    0,
   63,   75,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  207,  331,    0,    0,  307,  308,  309,  310,
    0,    0,   56,  219,  220,  256,   14,    8,    9,  175,
  280,   15,   17,   18,  176,   25,    0,   21,   27,    0,
  237,   28,   29,   30,  177,  279,   73,  258,   60,    0,
    0,  301,  208,    0,  274,   72,   74,  278,  131,  132,
  133,  134,    0,  173,  277,  209,  281,  319,    0,  276,
  273,  282,  283,    0,  311,  312,  313,  314,  315,  316,
  317,  318,  123,    0,  122,    2,    0,    0,    0,  218,
  217,  216,    0,    0,    0,   16,    0,    0,  182,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,   85,   83,  147,    0,
    0,    0,    0,    0,    0,  137,  138,    0,    0,   61,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   40,
  125,    0,    0,    0,    0,  201,   51,  206,    0,    0,
    0,    0,  203,    0,    7,    0,    0,  151,    0,    0,
    0,  150,  156,    0,  152,  153,  158,  154,  159,  196,
  155,  157,    0,    0,  189,  192,    0,    0,    0,    0,
  223,    0,  180,    0,  183,  384,  385,    0,    0,   12,
    0,    0,    0,    0,  271,    0,  269,  270,    0,    0,
  262,    0,    0,    0,    0,    0,    0,  294,    0,    0,
    0,    0,    0,    0,    0,  364,    0,    0,    0,    0,
   84,  306,    0,  297,  285,  178,    0,    0,  284,    0,
    0,   26,    0,    0,    0,  259,  260,  257,  305,  302,
  303,    0,  229,  228,    0,    0,  233,    0,  127,    0,
    0,  169,    0,   52,   55,   50,    0,  205,    0,    0,
  167,    0,   59,  272,    0,    0,    0,    0,    0,   34,
   38,   37,   33,   35,   36,    0,    6,    0,  193,    0,
    0,    0,  188,    0,  191,    0,    0,  287,    0,  221,
    0,  224,  172,  171,   57,  170,  181,    0,    0,    0,
   11,    0,    0,    0,    0,  263,  261,    0,    0,    0,
    0,    0,    0,    0,  324,  325,    0,  337,    0,  346,
  330,    0,  371,    0,    0,  332,    0,    0,    0,    0,
    0,  367,  328,    0,  338,    0,  339,    0,    0,  286,
  238,    0,    0,   62,   70,    0,  304,    0,  139,  140,
  232,  230,    0,   43,    0,  129,    3,    0,    0,   68,
  248,    0,  243,    0,    0,  246,  247,  295,  296,    0,
    0,  204,  197,    0,  111,    0,  145,    0,    0,  164,
  166,  165,   39,    0,    0,    0,    0,  184,    0,  190,
    0,    0,  222,  393,  392,  388,  410,  409,    0,  401,
  400,  408,  403,  402,  396,  394,  395,  404,  413,    0,
  386,  387,  389,    0,  397,  398,  399,    0,    0,   66,
   13,    0,    0,    0,    0,  264,  265,    0,    0,  335,
  374,  375,    0,  373,    0,  326,    0,  320,    0,  362,
    0,    0,    0,    0,  361,    0,    0,  355,  357,  358,
  369,  365,    0,    0,  376,  377,  378,    0,  379,  329,
    0,  299,  179,  239,  174,   71,  234,    0,    0,  250,
  249,  252,    0,    0,  144,  101,  142,    0,  143,    0,
    0,   91,   92,   93,    0,   94,  162,    0,    0,  245,
  289,    0,    0,  130,  114,  115,    0,  113,    0,    0,
   87,   86,  163,    0,    0,  199,  198,  200,  195,  227,
    0,  383,    0,    0,   65,   10,  149,    0,    0,  266,
    0,  334,    0,    0,    0,    0,  350,    0,  347,    0,
  348,  360,    0,    0,  354,  333,  382,  380,  343,  342,
    0,   31,    0,    0,    0,   95,   69,    0,   45,   46,
    0,  104,  102,    0,    0,    0,  241,  109,    0,   89,
    0,    0,  107,  108,    0,    0,    0,    0,   24,   22,
  336,    0,  327,  321,    0,    0,    0,    0,  359,  356,
    0,  340,    0,   44,  251,    0,    0,    0,   77,   76,
    0,   82,    4,    0,   88,  412,  391,    0,  406,    0,
  363,  351,    0,  349,  352,    0,   32,   47,    0,    0,
   98,  160,   99,  100,    0,    0,    0,   90,    0,  322,
    0,  345,  344,    0,   97,    0,    0,    0,    0,  254,
   78,  407,  353,  341,  119,  118,  117,    0,    0,  116,
  };
  protected static  short [] yyDgoto  = {             2,
  153,    7,  116,  286,  287,  572,  188,  189,  306,  190,
   57,   58,   59,   60,  221,   61,   62,  127,   63,   64,
   65,   66,   67,   68,   69,   70,  455,  549,  599,   71,
   72,   73,   74,  573,  308,  192,  171,  574,  279,  384,
  512,  390,  513,  514,  515,   75,  178,  179,   76,  646,
   77,  214,  325,  593,  193,   78,   79,   80,   81,  449,
   82,  290,  392,   83,   84,  122,   85,   86,   87,  619,
   88,  621,  523,  587,  194,  149,  150,  195,  532,  591,
  393,  394,  516,  669,  641,  642,  643,  644,  517,  518,
  583,  196,  416,  197,  406,  527,  528,    8,  117,  172,
  280,    9,  198,   89,   90,   91,   92,   93,  158,  381,
  275,  199,  200,  201,  202,  414,   10,   14,  123,  181,
  291,  326,   94,  257,  128,  129,  203,  204,  314,  205,
  316,  206,  207,  539,  182,  183,   95,   96,  210,  211,
   11,  319,  276,  277,  382,  401,  395,  520,  396,  397,
   97,   98,   99,  230,  231,  232,  233,  100,  101,  102,
  103,    4,  399,  104,  105,  106,  107,  108,  109,  110,
  111,  112,  347,  467,  244,  245,  463,  571,  654,  473,
  477,  478,  479,  480,  246,  247,  362,  464,  485,  218,
  440,  441,  442,  443,  444,  445,  446,  447,  448,
  };
  protected static  short [] yySindex = {         -451,
    0,    0, -255, -430, -296, 3056, -265, -132,    0,    0,
    0,    0,  202, -190,    0, 3938,  222,  223,    0,  245,
    0,    0,    0,    0,    0,  251,  255,  267,    0,  272,
    0,    0,  273,  277,  285,  290,  296,  307,  310,  311,
  314,  315,    0,    0, -305,  192,    0,    0,    0,    0,
  317, 1794,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,   51,    0,    0, -162,
    0,    0,    0,    0,    0,    0,    0,    0,    0, 4876,
  323,    0,    0,  325,    0,    0,    0,    0,    0,    0,
    0,    0,   41,    0,    0,    0,    0,    0,  327,    0,
    0,    0,    0,  -35,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  102,    0,    0,  116,  116, -156,    0,
    0,    0, 3191, 2147, 1909,    0,  -78, -253,    0, 4059,
 4648, 4648, 4648, 4648, 4648, 2418, -153, -152, -154, 4648,
 -145, 4648, 4174, 4648, 4648, 4648,    0,    0,    0, -142,
 -141, 2778, -255,  342,  343,    0,    0, 4767, 4648,    0,
 4648, 3310, 4767, 4767, 3056, -137, -322, 4648, -455,    0,
    0,  105,  105,  354,  358,    0,    0,    0,  137, -123,
   74,  364,    0,  358,    0,  358, 1673,    0,  155, 2542,
 -329,    0,    0,  -18,    0,    0,    0,    0,    0,    0,
    0,    0,   23,  152,    0,    0,   59,  342, 4648, -247,
    0, 4293,    0,  101,    0,    0,    0,  166, -387,    0,
   71,  373, -316, -293,    0, 4648,    0,    0, -162,  391,
    0,  103, 4412, -145, 4648, -399,  394,    0,  133,  161,
  402,  137,  -80,  184,  401,    0, -412,  -21,  405,  210,
    0,    0,  404,    0,    0,    0,  211,  -24,    0, 3427,
   41,    0,  -28,  407, 4648,    0,    0,    0,    0,    0,
    0,  374,    0,    0,  424, -173,    0,  201,    0, -255,
 -255,    0, -451,    0,    0,    0, -145,    0,  -22,   18,
    0, 4533,    0,    0,  434,  343,  109,   44, 3056,    0,
    0,    0,    0,    0,    0, -207,    0, 2271,    0, 4648,
 4648, 4293,    0, 2147,    0, 2147,  109,    0,   58,    0,
  193,    0,    0,    0,    0,    0,    0,  629, -141, 4648,
    0, 4648, 4648, 4767, -162,    0,    0, 4648, 4648, -162,
  178,  226, -320, 3585,    0,    0,  470,    0, 2898,    0,
    0, -290,    0, -412, 4648,    0, 4648,  134,  134,  134,
  135,    0,    0,  475,    0,  -11,    0,   -7, 3427,    0,
    0,  486,  227,    0,    0,  487,    0, -455,    0,    0,
    0,    0,  499,    0, -132,    0,    0,  -31,  -33,    0,
    0,  497,    0, -254,  137,    0,    0,    0,    0, 2147,
  205,    0,    0,  169,    0, -258,    0, 3056,  518,    0,
    0,    0,    0,  358, -162,  243,  249,    0,  152,    0,
 -281, 4293,    0,    0,    0,    0,    0,    0,  521,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  522,
    0,    0,    0,  528,    0,    0,    0,  531,  532,    0,
    0,  533,  -19,   51,  244,    0,    0, 4648, 4648,    0,
    0,    0,  534,    0,  111,    0,  -25,    0,  537,    0,
  234,  236,  238,   46,    0,  137,  240,    0,    0,    0,
    0,    0,  242,  207,    0,    0,    0, -314,    0,    0,
 -342,    0,    0,    0,    0,    0,    0, -145, 4648,    0,
    0,    0,  542,  -22,    0,    0,    0, -241,    0,  -22,
  -22,    0,    0,    0,  230,    0,    0,  204,   61,    0,
    0,  331,  258,    0,    0,    0,  358,    0,  336, 1909,
    0,    0,    0, 4648, 4648,    0,    0,    0,    0,    0,
   73,    0,   76,   78,    0,    0,    0, 4767,  561,    0,
  564,    0, 4533, 4533,  569, 4648,    0, 3700,    0, 4648,
    0,    0, -145, -290,    0,    0,    0,    0,    0,    0,
  246,    0,  567,  571,  572,    0,    0,  257,    0,    0,
  -22,    0,    0,  575, -141, 2147,    0,    0, 3056,    0,
  248, -162,    0,    0,  578,  579,  256,   51,    0,    0,
    0,  -25,    0,    0,  264,  265,  278,  282,    0,    0,
   91,    0, -145,    0,    0,  -22, -313, -145,    0,    0,
  583,    0,    0, 4648,    0,    0,    0,   92,    0,  581,
    0,    0, 4648,    0,    0, -443,    0,    0, 2147,  -14,
    0,    0,    0,    0,  567,  588, -141,    0,  590,    0,
  283,    0,    0,  592,    0, -141,  279,  289, -145,    0,
    0,    0,    0,    0,    0,    0,    0,  567,  608,    0,
  };
  protected static  short [] yyRindex = {         -220,
    0,    0,    0, -219, 2748,    0,  657,   77,    0,    0,
    0,    0,    0, 3819,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  870,  138,    0,    0,    0,    0,
    0, -220,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0, 1497,    0,    0, 1579,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0, 1079,    0,    0,    0,    0,    0,  299,    0,
    0,    0,    0,  379,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0, -246, -246,    0,    0,
    0,    0,    0,    0, -220,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  300,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0, -211, -211,    0,    0,    0,    0,    0,   21,    4,
  412,  247,    0,    0,    0,    0, -220,    0, -195,    0,
  212,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0, 1581, 1089,    0,    0,  301,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  -29,    0,
    0,    0,    0,    0,    0,  619,    0,    0,    0,    0,
    0,  -36,   56,    0,    0,    0,  288,    0,    0,    0,
    0,    0,  659,    0,    0,    0,    0,    0,    0,    0,
 1288,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  104,   81,    0, -204,    0,    0,
    0,    0, -220,    0,    0,    0,    0,    0,    0,  438,
    0,    0,    0,    0,    0,   82,  287,  -34,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0, -280,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  622,    0,    0,    0,    0,  339,
    0,    0,  631,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  293,    0,    0,    0,  302,  302,  302,
  302,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  110,    0,    0,    0, -220,    0,
    0,  218,    0, 2305,  754,    0,    0,    0,    0,    0,
   14,    0,    0,    0,    0,  635,    0,    0,    0,    0,
    0,    0,    0,    0, 1859, 2028, 2590,    0, 1298,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  637,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  638,    0,    0,    0,  639,    0,    0,
    0,    0,    0,   -8,  640,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  619,    0,    0,    0,
    0,    0,    0,    0,    0,  146,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  328,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  329,    0,    0,
    0,    0,  625,    0,    0,    0,    0,    0,    0, -220,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  641,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0, 2182,    0,    0,    0,    0,    0,    0,
    0, 3212,    0,    0,    0,    0,    0,  653,    0,    0,
    0,  619,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  549,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  656,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  658,    0,    0,
  };
  protected static  short [] yyGindex = {            0,
  698,  552, -179,  466, -337, -100,    0,   -5, -224, -147,
    0,    0,    0,  -16, -127,    0,    0,    0,    0,    0,
 -158,  550, -318,    0,    0,  696,    0,    0,    0,    0,
    0,    0,    0, -445,    0,    0,    0,    0,    0,    0,
    0, -355,    0,    0,  206,    0, -315,    0, 1239,    0,
    0,  502, -259,  180,    0,   94,  633,    0,    0,    0,
 -120,    0,    0,    0,    0,  554,    0,    0,    0,   80,
 -167,    0,    0,    0, -157,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  413,    0,    0,    0,    0,  443,    0,  615,
  562,  456,    0,    0,    0,    0,    0,  580,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  -20,  479,    0,  612,    0,  431,    0,  432,
    0,  557,    0,    0,    0,  457,    0,    0,    0,  540,
    0,    0,    0,  380,    0,    0,    0,    0,    0,    0,
 -143,  371,    0,    0, -198,  527,    0,    0,    0, -200,
    0,    0,  610,    0,    0,    0,    0,    0,    0,    0,
    0,    0, -427,  215, -332,  414,    0,    0,    0,  213,
    0,  208,    0,    0,  410,    0,  415,    0,    9,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,
  };
  protected static  short [] yyTable = {           126,
   56,  274,  180,  301,  370,  110,  389,  370,  499,  474,
  167,  268,  374,   79,  240,  454,  371,  389,  554,  363,
   49,  547,  243,  271,  483,  659,  333,  336,  466,  295,
  253,  155,  235,  503,  310,  154,  124,  334,  147,  555,
  239,  307,  536,    5,  301,  301,  301,  301,  301,  301,
  301,  113,  418,  324,   79,  168,  652,  519,  408,  212,
  213,   49,  329,  567,   49,  212,  320,  120,  364,  639,
    5,  344,  404,  525,  252,  273,  120,  505,  372,  358,
  231,  359,  163,  360,    1,  361,  653,  164,  398,  379,
  505,  506,  421,  156,  301,  157,  301,  301,  301,  301,
  301,  301,  301,  141,  506,   12,  177,  507,  208,  121,
  345,  331,  346,  217,  330,  220,  222,  120,  121,  508,
  507,  231,  114,  237,  231,  220,  242,  248,  249,  250,
  380,  256,  159,  342,  526,  409,  640,  306,  563,  456,
  457,  173,  173,  173,  141,  264,  301,    5,  577,  307,
  121,  272,  509,  324,  579,  580,  311,  569,    5,  268,
  537,    5,  540,  538,    5,  509,  296,  124,  253,  307,
  154,  180,  645,  350,  630,   13,  349,  115,  306,  306,
  306,  306,  306,  306,  306,  461,  387,  462,  398,  568,
  159,  570,  318,  290,  291,  323,    6,  306,  306,  306,
  124,  351,  126,  159,  330,  124,  209,  270,  450,   42,
  274,  475,   51,  668,  304,  303,  305,   67,  343,  118,
  500,  471,  148,  605,  356,  617,  159,  355,  306,  598,
  306,  290,  291,  510,  243,  511,  243,  466,  603,  256,
  126,  119,  521,  476,  501,  524,  202,   42,  376,  151,
  367,  370,  275,  366,  369,  275,  266,  267,   67,  550,
  638,  130,  131,  324,  656,  301,  460,  495,  253,  459,
  369,  275,  275,  275,  557,  177,  559,  330,  561,  558,
  565,  560,  566,  564,  132,  355,  612,  202,  625,  611,
  133,  624,  113,  407,  134,  323,  629,  110,  255,  628,
  194,  267,  413,  398,  631,  632,  135,  355,  330,  398,
  398,  136,  137,  451,  657,  452,  138,  301,  634,  235,
  301,  633,  635,  663,  139,  330,  330,  177,  366,  140,
   79,  366,  220,  368,  301,  141,  368,   49,  242,  255,
  242,  194,  381,  175,  194,  381,  142,  173,  493,  143,
  144,   49,   79,  145,  146,  301,  152,  159,  110,  175,
  175,  175,  161,  114,  162,   79,  114,  486,  487,  489,
  165,  169,   49,  170,  174,  209,  234,  235,  300,  236,
  398,  658,  259,  253,  238,  251,  260,  278,  252,  253,
  253,  301,  269,  120,  282,  388,  306,  283,  285,  306,
  288,   79,  529,  306,  289,  323,  388,  292,   49,  313,
  315,  240,  312,  317,  327,  398,  332,  620,  115,  300,
  300,  300,  300,  300,  173,  300,  121,  328,  622,   35,
  606,  337,  608,  338,  348,  243,  173,  288,  300,  300,
  300,  352,  551,  354,  357,  365,  301,  375,  173,  368,
  306,  306,  240,  306,  306,  370,   79,  370,  173,  370,
  253,  370,  609,   49,  120,  306,  377,  378,  306,  306,
  383,  300,  173,  400,  403,  306,  306,  275,  288,  620,
  306,  655,  575,  306,  345,  185,  346,  405,  422,  306,
  166,  159,  306,   54,  306,  253,   46,  121,  252,  306,
  159,  306,    1,   51,  306,  651,  423,   46,  458,  297,
  468,  306,  637,  590,  306,  490,  484,  488,  491,  120,
  306,  298,  492,  301,  306,  306,  494,  496,  306,  301,
   49,  185,   49,  301,   67,  665,  177,  177,  498,  242,
  504,  220,  522,  220,  306,  299,   25,  301,   80,  301,
   54,  301,  121,  301,  275,   67,   67,  530,  534,  194,
  541,  553,  542,  202,  535,  175,  275,  543,  306,   67,
  544,  548,  545,  546,  552,  301,  556,  202,  275,  562,
  306,  301,  576,  623,  567,  301,  581,  306,  275,   80,
  584,  306,  582,  306,  589,  173,  586,  173,  202,  173,
  585,  600,  275,  595,  601,   67,  596,  648,  597,  604,
  613,  614,  615,  616,  618,  255,  220,  194,  626,  627,
  636,  650,  649,  306,   81,  306,  647,  306,  660,  306,
  662,  306,  664,  306,  202,  306,  666,  300,  194,  194,
  300,  300,  175,  306,  300,  306,  667,  306,  670,  306,
  255,  306,  194,  306,  175,  306,    1,  306,  298,  323,
   67,  306,  268,  306,    5,   81,  175,  306,  300,  267,
  301,  372,  302,   67,  112,   54,  175,  411,  390,  405,
   23,   41,  300,  194,  161,  103,  255,  194,  194,  202,
  175,  300,  300,  236,  300,  300,   53,    3,   96,  298,
  298,  298,  298,  298,  258,  298,  300,  353,  262,  300,
  300,  321,  160,  578,  594,  265,  300,  300,  298,  298,
  298,  300,  385,  417,  300,  275,  661,  275,  240,  275,
  300,  194,  173,  300,  281,  300,  386,  261,  373,  215,
  300,  255,  300,  194,  419,  300,  309,  420,  402,  322,
  194,  298,  300,  244,  288,  300,  194,  497,  502,  341,
  254,  300,  472,  240,  482,  300,  300,  602,  481,  300,
  607,  610,    0,    0,    0,  288,  288,    0,    0,    0,
    0,    0,    0,    0,    0,  300,    0,    0,    0,  288,
    0,    0,    0,    0,  244,    0,    0,  244,    0,  240,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  300,
    0,    0,    0,  175,    0,  175,    0,  175,    0,  191,
    0,  300,    0,    0,    0,  288,  219,    0,  300,  223,
  224,  229,  300,    0,  300,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  240,    0,  263,    0,    0,    0,
    0,    0,    0,    0,  300,   80,  300,    0,  300,  146,
  300,    0,  300,    0,  300,    0,  300,    0,    0,    0,
  288,    0,  191,    0,  300,  191,  300,   80,  300,    0,
  300,    0,  300,    0,  300,    0,  300,    0,  300,    0,
   80,  424,  425,    0,    0,    0,    0,    0,  300,    0,
  146,  146,  146,  146,  146,    0,  146,  298,    0,    0,
  298,  335,  426,    0,  298,  427,  428,    0,  340,  146,
  146,  146,    0,    0,    0,    0,   80,    0,    0,    0,
    0,   81,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  429,    0,    0,    0,    0,
    0,    0,  146,    0,    0,    0,    0,    0,    0,    0,
    0,  298,  298,    0,  298,  298,   81,    0,  430,  431,
    0,    0,    0,    0,    0,    0,  298,    0,    0,  298,
  298,   80,    0,    0,    0,    0,  298,  298,    0,    0,
    0,  298,    0,    0,  298,  415,  415,    0,  432,  191,
  298,  191,   81,  298,    0,  298,    0,    0,    0,    0,
  298,    0,  298,    0,    0,  298,    0,    0,  453,    0,
  433,    0,  298,  335,  335,  298,    0,    0,    0,    0,
    0,  298,    0,    0,    0,  298,  298,  434,    0,  298,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  298,    0,   81,    0,    0,
  244,    0,    0,    0,    0,    0,    0,    0,  135,    0,
    0,  435,    0,    0,    0,  244,    0,    0,  186,  298,
    0,  244,  244,    0,    0,  191,    0,    0,    0,  244,
    0,  298,  436,    0,  437,  244,  438,    0,  298,    0,
  244,    0,  298,    0,  298,  244,    0,    0,    0,  135,
    0,  135,  135,  135,    0,    0,    0,  244,  146,  186,
    0,  146,  186,    0,    0,  146,  244,    0,  135,  135,
  135,  244,    0,    0,  298,    0,  298,    0,  298,    0,
  298,    0,  298,  335,  298,    0,  298,    0,  439,    0,
  244,    0,    0,    0,  298,    0,  298,    0,  298,    0,
  298,  135,  298,    0,  298,    0,  298,    0,  298,    0,
    0,    0,  146,  146,    0,  146,  146,    0,  298,    0,
    0,    0,    0,    0,    0,    0,  244,  146,    0,    0,
  146,    0,    0,  244,    0,    0,    0,  146,  146,  244,
    0,    0,  146,    0,    0,    0,    0,    0,    0,    0,
    0,  146,    0,    0,  146,    0,    0,    0,    0,  592,
  592,    0,    0,  146,    0,    0,  146,    0,    0,    0,
    0,  244,    0,  244,    0,    0,  146,    0,    0,    0,
    0,    0,  146,    0,    0,    0,  146,  146,    0,    0,
  146,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  191,    0,   54,    0,    0,    0,  136,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  187,    0,    0,
  146,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  146,    0,    0,    0,    0,    0,    0,  146,
    0,    0,    0,  146,    0,  146,    0,    0,  136,    0,
  136,  136,  136,    0,  191,    0,    0,  135,  187,    0,
  135,  187,    0,    0,  135,    0,    0,  136,  136,  136,
    0,    0,    0,    0,    0,  146,    0,    0,    0,    0,
    0,  146,    0,  146,    0,  146,    0,  146,    0,    0,
    0,    0,    0,    0,    0,  146,    0,  146,    0,  146,
  136,  146,    0,  146,    0,  146,    0,  146,    0,  146,
    0,  135,  135,    0,  135,  135,    0,    0,    0,  146,
    0,    0,    0,    0,    0,  186,  135,    0,    0,  135,
    0,    0,    0,  284,    0,    0,  135,  135,    0,    0,
    0,  135,  293,    0,  294,    0,  186,  186,    0,    0,
  135,    0,    0,  135,    0,    0,    0,    0,    0,    0,
  186,    0,  135,    0,    0,  135,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  135,    0,    0,    0,    0,
    0,  135,    0,    0,    0,  135,  135,    0,    0,  135,
    0,  186,    0,    0,    0,  186,  186,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,   20,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  135,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  186,
    0,  135,    0,    0,    0,    0,    0,  391,  135,    0,
    0,  186,  135,    0,  135,    0,    0,   20,  186,    0,
   20,    0,    0,    0,  186,    0,  136,    0,    0,  136,
    0,    0,    0,  136,    0,    0,   20,   20,   20,    0,
    0,    0,    0,    0,  135,    0,    0,    0,    0,    0,
  135,    0,  135,    0,  135,    0,  135,    0,  275,    0,
  185,    0,    0,    0,  135,    0,  135,    0,  135,   20,
  135,    0,  135,    0,  135,    0,  135,    0,  135,    0,
  136,  136,    0,  136,  136,    0,    0,    0,  135,    0,
    0,    0,    0,    0,  187,  136,    0,    0,  136,  275,
    0,  185,  275,    0,  185,  136,  136,  391,    0,    0,
  136,    0,    0,    0,    0,  187,  187,    0,    0,  136,
    0,    0,  136,    0,    0,    0,    0,  531,    0,  187,
    0,  136,  533,    0,  136,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  136,    0,    0,    0,    0,    0,
  136,  275,    0,    0,  136,  136,    0,    0,  136,    0,
  187,    0,    0,    0,  187,  187,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  187,    0,    0,   54,    0,   55,  136,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  187,    0,
  136,    0,    0,    0,    0,   53,    0,  136,    0,    0,
  187,  136,  391,  136,    0,    0,    0,  187,  391,  391,
    0,    0,    0,  187,    0,   20,    0,    0,   20,    0,
    0,    0,   20,    0,    0,  588,    0,    0,    0,    0,
    0,    0,    0,  136,    0,    0,    0,    0,    0,  136,
    0,  136,    0,  136,    0,  136,    0,    0,    0,    0,
    0,    0,    0,  136,    0,  136,    0,  136,    0,  136,
    0,  136,    0,  136,    0,  136,    0,  136,    0,   20,
   20,    0,   20,   20,    0,    0,    0,  136,    0,  391,
    0,    0,    0,    0,    0,    0,    0,   20,    0,    0,
    0,    0,    0,  125,   20,   20,   54,  275,   55,   20,
  275,    0,    0,    0,    0,    0,    0,    0,   20,    0,
    0,   20,    0,    0,  391,    0,   53,    0,  148,    0,
   20,    0,    0,   20,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   20,    0,    0,    0,    0,    0,   20,
    0,    0,    0,   20,   20,    0,    0,   20,    0,    0,
    0,  275,  275,    0,    0,  275,    0,  185,    0,  148,
    0,    0,  148,    0,    0,    0,    0,    0,    0,  275,
    0,    0,    0,    0,    0,    0,  275,  275,  185,  185,
    0,    0,    0,    0,    0,    0,    0,   20,    0,    0,
  275,    0,  185,  275,    0,    0,    0,   15,    0,   20,
    0,    0,    0,   16,   17,    0,   20,    0,  125,    0,
   20,   54,   20,   55,   18,    0,   19,    0,    0,   20,
    0,  275,    0,  185,    0,  275,  275,    0,  185,    0,
    0,   53,   21,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   20,    0,    0,    0,    0,    0,   20,    0,
   20,    0,   20,  184,   20,    0,    0,    0,    0,    0,
    0,    0,   20,    0,   20,    0,   20,    0,   20,  275,
   20,  185,   20,    0,   20,    0,   20,    0,    0,    0,
    0,  275,    0,  185,    0,    0,   20,  105,  275,    0,
  185,    0,  275,    0,  275,    0,  185,    0,   22,    0,
   23,   24,    0,    0,    0,    0,    0,    0,    0,  185,
   25,   26,    0,    0,    0,    0,    0,    0,   15,    0,
    0,    0,    0,    0,   16,   17,   27,    0,  105,    0,
  275,  105,  275,    0,  275,   18,  275,   19,    0,    0,
   20,    0,    0,    0,  275,    0,  275,    0,  275,    0,
  275,    0,    0,   21,    0,    0,    0,    0,   28,   29,
    0,    0,    0,    0,    0,    0,    0,    0,  275,    0,
    0,    0,    0,   30,    0,    0,  186,  148,    0,   31,
    0,    0,    0,   32,    0,    0,    0,    0,    0,    0,
    0,   33,   34,   35,   36,   37,    0,   38,   39,   40,
    0,   41,   42,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   43,    0,   44,    0,    0,    0,    0,   22,
    0,   23,   24,    0,    0,    0,    0,    0,    0,    0,
    0,   25,   26,   15,  148,  148,    0,    0,    0,   16,
   17,  253,    0,    0,    0,    0,  187,   27,    0,   54,
   18,   55,   19,    0,    0,   20,  148,  148,    0,    0,
   45,    0,   46,   47,   48,   49,   50,   51,    1,   53,
  148,    0,    0,  148,    0,    0,    0,    0,    0,   28,
   29,    0,  253,    0,    0,  253,    0,    0,    0,    0,
    0,    0,    0,    0,   30,    0,    0,    0,    0,    0,
   31,  148,    0,    0,   32,  148,  148,    0,    0,    0,
    0,    0,   33,   34,   35,   36,   37,    0,   38,   39,
   40,    0,   41,   42,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   43,   22,   44,   23,   24,    0,    0,
    0,    0,    0,    0,    0,    0,  105,   26,    0,  148,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  148,   27,    0,  242,    0,    0,    0,  148,    0,
   52,    0,    0,   54,  148,   55,    0,    0,    0,    0,
    0,   45,    0,   46,   47,   48,   49,   50,   51,    1,
    0,    0,    0,   53,   28,   29,    0,    0,    0,    0,
    0,    0,    0,    0,  105,  242,    0,    0,  242,   30,
    0,    0,    0,    0,    0,   31,    0,    0,    0,   32,
    0,    0,    0,    0,    0,  105,  105,   33,   34,   35,
   36,   37,    0,   38,   39,   40,    0,   41,   42,  105,
    0,    0,  105,    0,    0,    0,    0,    0,   43,    0,
   44,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  105,   15,    0,    0,  105,  105,    0,   16,   17,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   18,    0,
   19,    0,    0,   20,    0,    0,   45,    0,   46,   47,
   48,   49,   50,   51,    1,    0,   21,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  125,  105,    0,
   54,    0,   55,    0,    0,    0,    0,  184,    0,    0,
  105,    0,    0,    0,    0,    0,    0,  105,    0,    0,
   53,    0,    0,  105,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  253,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   22,  253,   23,   24,    0,    0,    0,  253,
  253,    0,    0,  185,   25,   26,    0,  253,  410,    0,
  411,    0,    0,  253,    0,   15,    0,    0,  253,    0,
   27,   16,   17,  253,    0,    0,    0,    0,    0,    0,
    0,    0,   18,    0,   19,  253,    0,   20,    0,    0,
    0,    0,    0,    0,  253,    0,    0,    0,    0,  253,
   21,    0,   28,   29,    0,    0,    0,    0,    0,    0,
    0,  187,    0,    0,   54,    0,   55,   30,  253,  106,
  186,    0,    0,   31,    0,    0,    0,   32,    0,    0,
    0,    0,    0,    0,   53,   33,   34,   35,   36,   37,
    0,   38,   39,   40,    0,   41,   42,    0,    0,    0,
    0,  242,    0,    0,  253,    0,   43,    0,   44,    0,
  106,  253,    0,  106,    0,    0,   22,  253,   23,   24,
    0,    0,  242,  242,    0,    0,    0,    0,   25,   26,
    0,    0,    0,    0,    0,    0,  242,    0,    0,    0,
    0,  161,    0,    0,   27,    0,    0,    0,    0,  253,
    0,  253,    0,    0,   45,    0,   46,   47,   48,   49,
   50,   51,   15,    0,    0,    0,  225,  242,   16,   17,
  412,    0,  242,    0,    0,    0,   28,   29,    0,   18,
    0,   19,    0,    0,   20,    0,    0,    0,    0,    0,
    0,   30,    0,    0,    0,    0,    0,   31,    0,    0,
    0,   32,    0,    0,    0,    0,    0,    0,    0,   33,
   34,   35,   36,   37,    0,   38,   39,   40,    0,   41,
   42,    0,    0,    0,    0,    0,    0,  242,  226,    0,
   43,    0,   44,    0,  242,    0,    0,    0,    0,    0,
  242,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  227,    0,
    0,    0,    0,   22,    0,   23,   24,  168,    0,  168,
  168,    0,  168,    0,    0,    0,   26,    0,   45,    0,
   46,   47,   48,   49,   50,   51,   15,    0,    0,    0,
  168,   27,   16,   17,    0,    0,    0,  125,  255,    0,
   54,    0,   55,   18,    0,   19,    0,    0,   20,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   53,   21,    0,   28,   29,    0,    0,    0,  106,    0,
    0,    0,    0,    0,  228,    0,    0,    0,   30,    0,
    0,    0,  184,    0,   31,    0,    0,    0,   32,    0,
    0,    0,    0,    0,    0,    0,   33,   34,   35,   36,
   37,    0,   38,   39,   40,    0,   41,   42,    0,    0,
    0,    0,    0,    0,    0,    0,    0,   43,    0,   44,
    0,    0,    0,    0,    0,    0,  106,   22,    0,   23,
   24,    0,    0,    0,    0,    0,    0,    0,    0,   25,
   26,    0,    0,    0,    0,    0,    0,  106,  106,    0,
    0,    0,    0,    0,    0,   27,    0,  125,    0,    0,
   54,  106,   55,    0,  106,   45,    0,   46,   47,   48,
   49,   50,   51,    0,    0,    0,    0,    0,    0,    0,
   53,    0,    0,    0,    0,    0,    0,   28,   29,    0,
    0,    0,  106,    0,    0,    0,  106,  106,    0,    0,
    0,    0,   30,    0,    0,  186,    0,    0,   31,    0,
    0,    0,   32,    0,    0,    0,    0,    0,    0,    0,
   33,   34,   35,   36,   37,  168,   38,   39,   40,    0,
   41,   42,  168,    0,    0,    0,    0,    0,  168,  168,
  106,   43,    0,   44,    0,    0,    0,    0,    0,  168,
    0,  168,  106,    0,  168,    0,    0,    0,    0,  106,
    0,    0,   15,    0,    0,  106,    0,    0,   16,   17,
    0,    0,    0,    0,    0,    0,  168,    0,    0,   18,
    0,   19,    0,    0,   20,    0,    0,    0,    0,   45,
    0,   46,   47,   48,   49,   50,   51,   21,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   52,    0,    0,   54,    0,
   55,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  168,    0,  168,  168,    0,   53,    0,
    0,    0,    0,    0,    0,    0,  168,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  168,    0,   22,    0,   23,   24,    0,    0,    0,
    0,    0,    0,    0,    0,   25,   26,    0,    0,    0,
    0,    0,   15,    0,    0,    0,    0,    0,   16,   17,
    0,   27,    0,  168,  168,    0,  168,    0,    0,   18,
    0,   19,    0,    0,   20,    0,    0,    0,  168,    0,
    0,    0,    0,    0,  168,    0,    0,    0,  168,    0,
    0,    0,    0,   28,   29,    0,  168,  168,  168,  168,
  168,   58,  168,  168,  168,    0,  168,  168,   30,    0,
    0,    0,    0,    0,   31,    0,    0,  168,   32,  168,
  125,    0,  176,   54,    0,   55,   33,   34,   35,   36,
   37,    0,   38,   39,   40,    0,   41,   42,    0,    0,
    0,    0,   58,   53,    0,   58,    0,   43,    0,   44,
    0,    0,    0,   22,    0,   23,   24,    0,    0,    0,
    0,    0,    0,    0,    0,  168,   26,  168,  168,  168,
  168,  168,  168,    0,    0,    0,    0,    0,    0,    0,
    0,   27,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   45,    0,   46,   47,   48,
   49,   50,   51,    0,    0,    0,    0,    0,    0,    0,
   15,    0,    0,   28,   29,    0,   16,   17,    0,    0,
    0,    0,    0,    0,    0,    0,    0,   18,   30,   19,
    0,    0,   20,    0,   31,    0,    0,    0,   32,  125,
    0,    0,   54,    0,   55,   21,   33,   34,   35,   36,
   37,  469,   38,   39,   40,  241,   41,   42,    0,    0,
    0,    0,   53,    0,    0,    0,    0,   43,    0,   44,
    0,  470,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   22,    0,   23,   24,   45,    0,   46,   47,   48,
   49,   50,   51,   25,   26,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   27,
    0,    0,    0,    0,    0,   15,    0,    0,    0,    0,
    0,   16,   17,    0,    0,    0,  125,    0,    0,   54,
   58,   55,   18,    0,   19,    0,    0,   20,    0,    0,
    0,   28,   29,    0,    0,    0,    0,    0,    0,   53,
    0,    0,    0,    0,    0,    0,   30,    0,    0,    0,
    0,    0,   31,    0,    0,    0,   32,    0,    0,    0,
    0,    0,    0,    0,   33,   34,   35,   36,   37,    0,
   38,   39,   40,    0,   41,   42,    0,    0,   58,    0,
    0,    0,    0,    0,    0,   43,    0,   44,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   58,
   58,    0,    0,    0,    0,    0,   22,    0,   23,   24,
    0,    0,    0,   58,    0,    0,   58,  120,    0,   26,
    0,    0,    0,    0,   15,    0,    0,    0,    0,    0,
   16,   17,    0,   45,   27,   46,   47,   48,   49,   50,
   51,   18,    0,   19,   58,    0,   20,    0,   58,   58,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,   28,   29,  121,  175,
    0,    0,    0,    0,  125,    0,    0,   54,    0,   55,
    0,   30,    0,    0,    0,    0,    0,   31,    0,    0,
    0,   32,   58,    0,    0,    0,    0,   53,    0,   33,
   34,   35,   36,   37,   58,   38,   39,   40,    0,   41,
   42,   58,    0,    0,    0,    0,    0,   58,    0,    0,
   43,    0,   44,    0,    0,   22,    0,   23,   24,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   26,    0,
    0,   15,    0,    0,    0,    0,    0,   16,   17,    0,
    0,    0,    0,   27,    0,    0,    0,    0,   18,    0,
   19,    0,    0,   20,    0,    0,    0,    0,   45,    0,
   46,   47,   48,   49,   50,   51,   21,    0,    0,    0,
    0,    0,    0,    0,    0,   28,   29,    0,    0,  125,
    0,    0,   54,    0,   55,    0,    0,    0,    0,    0,
   30,    0,    0,    0,    0,    0,   31,    0,    0,    0,
   32,    0,   53,    0,    0,    0,    0,    0,   33,   34,
   35,   36,   37,    0,   38,   39,   40,    0,   41,   42,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   43,
    0,   44,   22,    0,   23,   24,    0,    0,    0,    0,
    0,    0,    0,    0,   25,   26,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   27,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,   45,    0,   46,
   47,   48,   49,   50,   51,    0,    0,    0,    0,   15,
    0,    0,   28,   29,  465,   16,   17,    0,  215,    0,
  215,  215,    0,  215,    0,    0,   18,   30,   19,    0,
    0,   20,    0,   31,    0,    0,    0,   32,    0,    0,
    0,  215,    0,    0,    0,   33,   34,   35,   36,   37,
    0,   38,   39,   40,    0,   41,   42,    0,    0,    0,
    0,    0,    0,    0,    0,    0,   43,    0,   44,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   22,    0,   23,   24,   45,    0,   46,   47,   48,   49,
   50,   51,    0,   26,   15,    0,    0,    0,    0,    0,
   16,   17,    0,    0,    0,    0,    0,  125,   27,    0,
   54,   18,   55,   19,    0,    0,   20,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   53,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   28,   29,    0,  175,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   30,    0,    0,    0,    0,
    0,   31,    0,    0,    0,   32,    0,    0,    0,    0,
    0,    0,    0,   33,   34,   35,   36,   37,    0,   38,
   39,   40,    0,   41,   42,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   43,   22,   44,   23,   24,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   26,    0,
    0,    0,    0,  215,    0,    0,    0,    0,    0,  215,
  215,    0,    0,   27,    0,    0,    0,    0,  125,    0,
  215,   54,  215,   55,    0,  215,    0,    0,    0,    0,
    0,    0,   45,    0,   46,   47,   48,   49,   50,   51,
    0,   53,    0,    0,    0,   28,   29,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   30,    0,    0,    0,    0,    0,   31,    0,    0,    0,
   32,    0,    0,    0,    0,    0,    0,    0,   33,   34,
   35,   36,   37,  469,   38,   39,   40,    0,   41,   42,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   43,
    0,   44,    0,  470,  215,    0,  215,  215,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  215,    0,    0,
    0,    0,   15,    0,    0,    0,    0,    0,   16,   17,
    0,    0,  215,  125,    0,    0,   54,    0,   55,   18,
    0,   19,    0,    0,   20,    0,    0,   45,    0,   46,
   47,   48,   49,   50,   51,    0,   53,    0,    0,    0,
    0,    0,    0,    0,  215,  215,    0,  215,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  215,
    0,    0,    0,    0,    0,  215,    0,    0,    0,  215,
    0,    0,    0,    0,    0,    0,    0,  215,  215,  215,
  215,  215,    0,  215,  215,  215,    0,  215,  215,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  215,    0,
  215,    0,    0,   22,    0,   23,   24,    0,    0,    0,
    0,    0,    0,    0,    0,    0,   26,    0,    0,    0,
    0,    0,    0,   15,    0,    0,    0,    0,    0,   16,
   17,   27,  125,    0,    0,   54,    0,   55,    0,    0,
   18,    0,   19,    0,    0,   20,  215,    0,  215,  215,
  215,  215,  215,  215,    0,   53,    0,    0,    0,    0,
    0,    0,    0,   28,   29,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   30,    0,
    0,    0,    0,    0,   31,    0,    0,    0,   32,    0,
    0,  124,    0,    0,    0,    0,   33,   34,   35,   36,
   37,    0,   38,   39,   40,    0,   41,   42,    0,    0,
    0,    0,    0,    0,    0,    0,    0,   43,    0,   44,
    0,    0,    0,    0,   22,    0,   23,   24,    0,    0,
    0,    0,    0,    0,    0,    0,  216,   26,   15,    0,
    0,    0,    0,    0,   16,   17,    0,    0,    0,    0,
    0,  125,   27,    0,   54,   18,   55,   19,    0,    0,
   20,    0,    0,    0,    0,   45,    0,   46,   47,   48,
   49,   50,   51,    0,   53,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   28,   29,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   30,
    0,    0,    0,    0,    0,   31,    0,    0,    0,   32,
    0,    0,    0,    0,    0,    0,    0,   33,   34,   35,
   36,   37,    0,   38,   39,   40,    0,   41,   42,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   43,   22,
   44,   23,   24,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   26,    0,    0,    0,    0,   15,    0,    0,
    0,    0,    0,   16,   17,    0,    0,   27,    0,    0,
    0,    0,  125,    0,   18,   54,   19,   55,    0,   20,
    0,    0,    0,    0,    0,    0,   45,    0,   46,   47,
   48,   49,   50,   51,    0,   53,    0,    0,    0,   28,
   29,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   30,    0,    0,    0,    0,    0,
   31,    0,    0,    0,   32,    0,    0,    0,    0,    0,
    0,    0,   33,   34,   35,   36,   37,    0,   38,   39,
   40,  241,   41,   42,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   43,    0,   44,    0,    0,   22,    0,
   23,   24,    0,    0,    0,    0,    0,    0,    0,    0,
   25,   26,    0,    0,    0,    0,   15,    0,    0,    0,
    0,    0,   16,   17,    0,    0,   27,  125,    0,    0,
   54,    0,   55,   18,    0,   19,    0,    0,   20,    0,
    0,   45,    0,   46,   47,   48,   49,   50,   51,    0,
   53,    0,    0,    0,    0,    0,    0,    0,   28,   29,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   30,    0,    0,    0,    0,    0,   31,
    0,    0,  339,   32,    0,    0,    0,    0,    0,    0,
    0,   33,   34,   35,   36,   37,    0,   38,   39,   40,
    0,   41,   42,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   43,    0,   44,    0,    0,   22,    0,   23,
   24,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   26,    0,    0,    0,    0,    0,    0,   15,    0,    0,
    0,    0,    0,   16,   17,   27,  125,    0,    0,   54,
    0,   55,    0,    0,   18,    0,   19,    0,    0,   20,
   45,    0,   46,   47,   48,   49,   50,   51,    0,   53,
    0,    0,    0,    0,    0,    0,    0,   28,   29,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   30,    0,    0,    0,    0,    0,   31,    0,
    0,    0,   32,    0,    0,    0,    0,    0,    0,    0,
   33,   34,   35,   36,   37,    0,   38,   39,   40,    0,
   41,   42,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   43,    0,   44,    0,    0,    0,    0,   22,    0,
   23,   24,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   26,   15,    0,    0,  125,    0,    0,   16,   17,
    0,    0,    0,    0,    0,    0,   27,    0,    0,   18,
    0,   19,    0,    0,   20,    0,    0,    0,   53,   45,
    0,   46,   47,   48,   49,   50,   51,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   28,   29,
    0,  175,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   30,    0,    0,    0,    0,    0,   31,
    0,    0,    0,   32,    0,    0,    0,    0,    0,    0,
    0,   33,   34,   35,   36,   37,    0,   38,   39,   40,
    0,   41,   42,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   43,   22,   44,   23,   24,    0,    0,    0,
    0,    0,    0,    0,    0,    0,   26,    0,    0,    0,
    0,   15,    0,    0,    0,    0,    0,   16,   17,    0,
    0,   27,    0,    0,    0,    0,    0,    0,    0,    0,
   19,    0,    0,   20,    0,    0,    0,    0,    0,    0,
   45,    0,   46,   47,   48,   49,   50,   51,    0,    0,
    0,    0,    0,   28,   29,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   30,    0,
    0,    0,    0,    0,   31,    0,    0,    0,   32,    0,
    0,    0,    0,    0,    0,    0,   33,   34,   35,   36,
   37,    0,   38,   39,   40,    0,   41,   42,    0,    0,
    0,    0,    0,    0,    0,    0,    0,   43,    0,   44,
    0,    0,    0,    0,   23,   24,    0,    0,    0,    0,
   15,    0,    0,    0,    0,   26,   16,   17,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   19,
   27,    0,   20,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   45,    0,   46,   47,   48,
   49,   50,   51,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   29,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,   32,    0,    0,
    0,    0,    0,    0,    0,   33,   34,   35,   36,   37,
    0,   38,   39,   40,    0,   41,   42,    0,    0,    0,
    0,    0,    0,   23,   24,    0,   43,    0,   44,    0,
    0,    0,    0,    0,   26,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   27,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   45,    0,   46,   47,   48,   49,
   50,   51,   29,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,   32,    0,    0,    0,
    0,    0,    0,    0,   33,   34,   35,   36,   37,    0,
   38,   39,   40,    0,   41,   42,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   43,    0,   44,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   45,    0,   46,   47,   48,   49,   50,
   51,
  };
  protected static  short [] yyCheck = {            16,
    6,  169,  123,    0,   41,   40,   40,   44,   40,  300,
   46,   41,   41,    0,  142,  334,   41,   40,   44,   41,
    0,   41,  143,  167,  357,   40,  343,  226,  344,  187,
  151,   52,   41,  389,  364,   52,  283,  331,  344,  467,
  141,  189,  324,  324,   41,   42,   43,   44,   45,   46,
   47,  317,  312,  212,   41,   91,  500,  395,  266,  313,
  314,   41,  450,  378,   44,  313,  314,  258,  248,  383,
  266,  471,  297,  332,  530,  531,    0,  332,  258,  492,
    0,  494,   42,  496,  536,  498,  530,   47,  289,  263,
  332,  346,  317,   43,   91,   45,   41,   42,   43,   44,
   45,   46,   47,    0,  346,  536,  123,  362,  125,    0,
  510,   41,  512,  130,   44,  132,  133,   41,  309,  374,
  362,   41,  388,  140,   44,  142,  143,  144,  145,  146,
  304,  152,  520,  234,  393,  343,  450,    0,  476,  338,
  339,   60,   61,   62,   41,  162,   91,  343,  504,  297,
   41,  168,  407,  312,  510,  511,  486,  500,  414,  165,
  442,  442,  422,  445,  445,  407,  187,  414,  289,  317,
  187,  292,  618,   41,  602,  472,   44,  443,   41,   42,
   43,   44,   45,   46,   47,  506,  287,  508,  389,  504,
  520,  534,  209,  414,  414,  212,  452,   60,   61,   62,
  454,   41,  414,  520,   44,  452,  454,  530,  329,  414,
  378,  502,  535,  659,   60,   61,   62,    0,  235,  352,
  388,  349,  528,  556,   41,  581,  520,   44,   91,  548,
   93,  452,  452,  488,  355,  490,  357,  553,  554,  260,
  452,   40,  400,  534,  388,  404,    0,  452,  265,   58,
   41,   41,   41,   44,   44,   44,  163,  164,   41,  458,
  616,   40,   40,  422,  279,  262,   41,   41,  389,   44,
   44,   60,   61,   62,   41,  292,   41,   44,   41,   44,
   41,   44,   41,   44,   40,   44,   41,   41,   41,   44,
   40,   44,  317,  299,   40,  312,   41,  332,    0,   44,
    0,  331,  308,  504,   41,   41,   40,   44,   44,  510,
  511,   40,   40,  330,  329,  332,   40,  262,   41,  328,
  317,   44,   41,   41,   40,   44,   44,  344,   41,   40,
  317,   44,  349,   41,  331,   40,   44,  317,  355,   41,
  357,   41,   41,   44,   44,   44,   40,  266,  369,   40,
   40,  331,  339,   40,   40,  352,   40,  520,  393,   60,
   61,   62,   40,  388,   40,  352,  388,  359,  360,  361,
   44,  270,  352,  258,  531,  454,  530,  530,    0,  534,
  581,  396,   41,  504,  530,  528,   44,  283,  530,  510,
  511,  388,  530,  317,   41,  429,  259,   40,  262,  262,
  524,  388,  408,  266,  331,  422,  429,   44,  388,  387,
  259,    0,  431,  355,  314,  616,   44,  585,  443,   41,
   42,   43,   44,   45,  343,   47,  317,  262,  586,  461,
  558,   41,  560,  331,   41,  556,  355,    0,   60,   61,
   62,   40,  459,  524,   44,   41,  443,   41,  367,   46,
  313,  314,   41,  316,  317,  492,  443,  494,  377,  496,
  581,  498,  563,  443,  388,  328,   93,   44,  331,  332,
  270,   93,  391,  456,   41,  338,  339,  266,   41,  647,
  343,  639,  499,  346,  510,  377,  512,  444,  431,  352,
  526,  520,  355,  530,  357,  616,  530,  388,  530,  362,
  520,  364,  536,  535,  367,  633,  314,  530,  331,  355,
   41,  374,  613,  530,  377,   41,  383,  383,  530,  443,
  383,  367,  530,  520,  387,  388,   41,   41,  391,  526,
  510,  377,  512,  530,  317,  656,  553,  554,   40,  556,
   44,  558,  338,  560,  407,  391,  378,  492,    0,  494,
  530,  496,  443,  498,  343,  338,  339,   40,  316,  259,
   40,  451,   41,  317,  316,  266,  355,   40,  431,  352,
   40,  328,   41,   41,   41,  520,   40,  331,  367,  534,
  443,  526,   41,  589,  378,  530,  357,  450,  377,   41,
  530,  454,  389,  456,  259,  514,  339,  516,  352,  518,
  270,   41,  391,  531,   41,  388,  531,  624,  531,   41,
   44,   41,   41,  357,   40,  317,  633,  317,   41,   41,
  530,   41,  531,  486,    0,  488,   44,  490,   41,  492,
   41,  494,   41,  496,  388,  498,  358,  259,  338,  339,
  262,  263,  343,  506,  266,  508,  358,  510,   41,  512,
  352,  514,  352,  516,  355,  518,    0,  520,    0,   41,
  443,  524,   41,  526,  378,   41,  367,  530,  514,  331,
  516,   41,  518,  456,   40,  530,  377,   41,   41,   41,
   41,   41,  304,  383,  357,  357,  388,  387,  388,  443,
  391,  313,  314,   41,  316,  317,   41,    0,   41,   41,
   42,   43,   44,   45,  153,   47,  328,  242,  159,  331,
  332,  210,   80,  508,  535,  162,  338,  339,   60,   61,
   62,  343,  280,  311,  346,  514,  647,  516,  317,  518,
  352,  431,  118,  355,  173,  357,  281,  158,  260,  128,
  362,  443,  364,  443,  314,  367,  190,  316,  292,  210,
  450,   93,  374,    0,  317,  377,  456,  378,  388,  233,
  151,  383,  349,  352,  355,  387,  388,  553,  354,  391,
  558,  564,   -1,   -1,   -1,  338,  339,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  407,   -1,   -1,   -1,  352,
   -1,   -1,   -1,   -1,   41,   -1,   -1,   44,   -1,  388,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  431,
   -1,   -1,   -1,  514,   -1,  516,   -1,  518,   -1,  124,
   -1,  443,   -1,   -1,   -1,  388,  131,   -1,  450,  134,
  135,  136,  454,   -1,  456,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  443,   -1,  161,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  486,  317,  488,   -1,  490,    0,
  492,   -1,  494,   -1,  496,   -1,  498,   -1,   -1,   -1,
  443,   -1,  187,   -1,  506,  190,  508,  339,  510,   -1,
  512,   -1,  514,   -1,  516,   -1,  518,   -1,  520,   -1,
  352,  273,  274,   -1,   -1,   -1,   -1,   -1,  530,   -1,
   41,   42,   43,   44,   45,   -1,   47,  259,   -1,   -1,
  262,  226,  294,   -1,  266,  297,  298,   -1,  233,   60,
   61,   62,   -1,   -1,   -1,   -1,  388,   -1,   -1,   -1,
   -1,  317,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  327,   -1,   -1,   -1,   -1,
   -1,   -1,   93,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  313,  314,   -1,  316,  317,  352,   -1,  350,  351,
   -1,   -1,   -1,   -1,   -1,   -1,  328,   -1,   -1,  331,
  332,  443,   -1,   -1,   -1,   -1,  338,  339,   -1,   -1,
   -1,  343,   -1,   -1,  346,  310,  311,   -1,  380,  314,
  352,  316,  388,  355,   -1,  357,   -1,   -1,   -1,   -1,
  362,   -1,  364,   -1,   -1,  367,   -1,   -1,  333,   -1,
  402,   -1,  374,  338,  339,  377,   -1,   -1,   -1,   -1,
   -1,  383,   -1,   -1,   -1,  387,  388,  419,   -1,  391,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  407,   -1,  443,   -1,   -1,
  317,   -1,   -1,   -1,   -1,   -1,   -1,   -1,    0,   -1,
   -1,  453,   -1,   -1,   -1,  332,   -1,   -1,    0,  431,
   -1,  338,  339,   -1,   -1,  400,   -1,   -1,   -1,  346,
   -1,  443,  474,   -1,  476,  352,  478,   -1,  450,   -1,
  357,   -1,  454,   -1,  456,  362,   -1,   -1,   -1,   41,
   -1,   43,   44,   45,   -1,   -1,   -1,  374,  259,   41,
   -1,  262,   44,   -1,   -1,  266,  383,   -1,   60,   61,
   62,  388,   -1,   -1,  486,   -1,  488,   -1,  490,   -1,
  492,   -1,  494,  458,  496,   -1,  498,   -1,  530,   -1,
  407,   -1,   -1,   -1,  506,   -1,  508,   -1,  510,   -1,
  512,   93,  514,   -1,  516,   -1,  518,   -1,  520,   -1,
   -1,   -1,  313,  314,   -1,  316,  317,   -1,  530,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  443,  328,   -1,   -1,
  331,   -1,   -1,  450,   -1,   -1,   -1,  338,  339,  456,
   -1,   -1,  343,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  352,   -1,   -1,  355,   -1,   -1,   -1,   -1,  534,
  535,   -1,   -1,  364,   -1,   -1,  367,   -1,   -1,   -1,
   -1,  488,   -1,  490,   -1,   -1,  377,   -1,   -1,   -1,
   -1,   -1,  383,   -1,   -1,   -1,  387,  388,   -1,   -1,
  391,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  586,   -1,  530,   -1,   -1,   -1,    0,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,    0,   -1,   -1,
  431,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  443,   -1,   -1,   -1,   -1,   -1,   -1,  450,
   -1,   -1,   -1,  454,   -1,  456,   -1,   -1,   41,   -1,
   43,   44,   45,   -1,  639,   -1,   -1,  259,   41,   -1,
  262,   44,   -1,   -1,  266,   -1,   -1,   60,   61,   62,
   -1,   -1,   -1,   -1,   -1,  486,   -1,   -1,   -1,   -1,
   -1,  492,   -1,  494,   -1,  496,   -1,  498,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  506,   -1,  508,   -1,  510,
   93,  512,   -1,  514,   -1,  516,   -1,  518,   -1,  520,
   -1,  313,  314,   -1,  316,  317,   -1,   -1,   -1,  530,
   -1,   -1,   -1,   -1,   -1,  317,  328,   -1,   -1,  331,
   -1,   -1,   -1,  175,   -1,   -1,  338,  339,   -1,   -1,
   -1,  343,  184,   -1,  186,   -1,  338,  339,   -1,   -1,
  352,   -1,   -1,  355,   -1,   -1,   -1,   -1,   -1,   -1,
  352,   -1,  364,   -1,   -1,  367,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  377,   -1,   -1,   -1,   -1,
   -1,  383,   -1,   -1,   -1,  387,  388,   -1,   -1,  391,
   -1,  383,   -1,   -1,   -1,  387,  388,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,    0,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  431,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  431,
   -1,  443,   -1,   -1,   -1,   -1,   -1,  289,  450,   -1,
   -1,  443,  454,   -1,  456,   -1,   -1,   41,  450,   -1,
   44,   -1,   -1,   -1,  456,   -1,  259,   -1,   -1,  262,
   -1,   -1,   -1,  266,   -1,   -1,   60,   61,   62,   -1,
   -1,   -1,   -1,   -1,  486,   -1,   -1,   -1,   -1,   -1,
  492,   -1,  494,   -1,  496,   -1,  498,   -1,    0,   -1,
    0,   -1,   -1,   -1,  506,   -1,  508,   -1,  510,   93,
  512,   -1,  514,   -1,  516,   -1,  518,   -1,  520,   -1,
  313,  314,   -1,  316,  317,   -1,   -1,   -1,  530,   -1,
   -1,   -1,   -1,   -1,  317,  328,   -1,   -1,  331,   41,
   -1,   41,   44,   -1,   44,  338,  339,  389,   -1,   -1,
  343,   -1,   -1,   -1,   -1,  338,  339,   -1,   -1,  352,
   -1,   -1,  355,   -1,   -1,   -1,   -1,  409,   -1,  352,
   -1,  364,  414,   -1,  367,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  377,   -1,   -1,   -1,   -1,   -1,
  383,   93,   -1,   -1,  387,  388,   -1,   -1,  391,   -1,
  383,   -1,   -1,   -1,  387,  388,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   40,   -1,   -1,   43,   -1,   45,  431,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  431,   -1,
  443,   -1,   -1,   -1,   -1,   63,   -1,  450,   -1,   -1,
  443,  454,  504,  456,   -1,   -1,   -1,  450,  510,  511,
   -1,   -1,   -1,  456,   -1,  259,   -1,   -1,  262,   -1,
   -1,   -1,  266,   -1,   -1,  527,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  486,   -1,   -1,   -1,   -1,   -1,  492,
   -1,  494,   -1,  496,   -1,  498,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  506,   -1,  508,   -1,  510,   -1,  512,
   -1,  514,   -1,  516,   -1,  518,   -1,  520,   -1,  313,
  314,   -1,  316,  317,   -1,   -1,   -1,  530,   -1,  581,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  331,   -1,   -1,
   -1,   -1,   -1,   40,  338,  339,   43,  259,   45,  343,
  262,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  352,   -1,
   -1,  355,   -1,   -1,  616,   -1,   63,   -1,    0,   -1,
  364,   -1,   -1,  367,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  377,   -1,   -1,   -1,   -1,   -1,  383,
   -1,   -1,   -1,  387,  388,   -1,   -1,  391,   -1,   -1,
   -1,  313,  314,   -1,   -1,  317,   -1,  317,   -1,   41,
   -1,   -1,   44,   -1,   -1,   -1,   -1,   -1,   -1,  331,
   -1,   -1,   -1,   -1,   -1,   -1,  338,  339,  338,  339,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  431,   -1,   -1,
  352,   -1,  352,  355,   -1,   -1,   -1,  265,   -1,  443,
   -1,   -1,   -1,  271,  272,   -1,  450,   -1,   40,   -1,
  454,   43,  456,   45,  282,   -1,  284,   -1,   -1,  287,
   -1,  383,   -1,  383,   -1,  387,  388,   -1,  388,   -1,
   -1,   63,  300,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  486,   -1,   -1,   -1,   -1,   -1,  492,   -1,
  494,   -1,  496,  321,  498,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  506,   -1,  508,   -1,  510,   -1,  512,  431,
  514,  431,  516,   -1,  518,   -1,  520,   -1,   -1,   -1,
   -1,  443,   -1,  443,   -1,   -1,  530,    0,  450,   -1,
  450,   -1,  454,   -1,  456,   -1,  456,   -1,  366,   -1,
  368,  369,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  377,
  378,  379,   -1,   -1,   -1,   -1,   -1,   -1,  265,   -1,
   -1,   -1,   -1,   -1,  271,  272,  394,   -1,   41,   -1,
  492,   44,  494,   -1,  496,  282,  498,  284,   -1,   -1,
  287,   -1,   -1,   -1,  506,   -1,  508,   -1,  510,   -1,
  512,   -1,   -1,  300,   -1,   -1,   -1,   -1,  426,  427,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  530,   -1,
   -1,   -1,   -1,  441,   -1,   -1,  444,  259,   -1,  447,
   -1,   -1,   -1,  451,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  459,  460,  461,  462,  463,   -1,  465,  466,  467,
   -1,  469,  470,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  480,   -1,  482,   -1,   -1,   -1,   -1,  366,
   -1,  368,  369,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  378,  379,  265,  316,  317,   -1,   -1,   -1,  271,
  272,    0,   -1,   -1,   -1,   -1,   40,  394,   -1,   43,
  282,   45,  284,   -1,   -1,  287,  338,  339,   -1,   -1,
  528,   -1,  530,  531,  532,  533,  534,  535,  536,   63,
  352,   -1,   -1,  355,   -1,   -1,   -1,   -1,   -1,  426,
  427,   -1,   41,   -1,   -1,   44,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  441,   -1,   -1,   -1,   -1,   -1,
  447,  383,   -1,   -1,  451,  387,  388,   -1,   -1,   -1,
   -1,   -1,  459,  460,  461,  462,  463,   -1,  465,  466,
  467,   -1,  469,  470,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  480,  366,  482,  368,  369,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  259,  379,   -1,  431,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  443,  394,   -1,    0,   -1,   -1,   -1,  450,   -1,
   40,   -1,   -1,   43,  456,   45,   -1,   -1,   -1,   -1,
   -1,  528,   -1,  530,  531,  532,  533,  534,  535,  536,
   -1,   -1,   -1,   63,  426,  427,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  317,   41,   -1,   -1,   44,  441,
   -1,   -1,   -1,   -1,   -1,  447,   -1,   -1,   -1,  451,
   -1,   -1,   -1,   -1,   -1,  338,  339,  459,  460,  461,
  462,  463,   -1,  465,  466,  467,   -1,  469,  470,  352,
   -1,   -1,  355,   -1,   -1,   -1,   -1,   -1,  480,   -1,
  482,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  383,  265,   -1,   -1,  387,  388,   -1,  271,  272,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  282,   -1,
  284,   -1,   -1,  287,   -1,   -1,  528,   -1,  530,  531,
  532,  533,  534,  535,  536,   -1,  300,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   40,  431,   -1,
   43,   -1,   45,   -1,   -1,   -1,   -1,  321,   -1,   -1,
  443,   -1,   -1,   -1,   -1,   -1,   -1,  450,   -1,   -1,
   63,   -1,   -1,  456,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  317,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  366,  332,  368,  369,   -1,   -1,   -1,  338,
  339,   -1,   -1,  377,  378,  379,   -1,  346,  258,   -1,
  260,   -1,   -1,  352,   -1,  265,   -1,   -1,  357,   -1,
  394,  271,  272,  362,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  282,   -1,  284,  374,   -1,  287,   -1,   -1,
   -1,   -1,   -1,   -1,  383,   -1,   -1,   -1,   -1,  388,
  300,   -1,  426,  427,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   40,   -1,   -1,   43,   -1,   45,  441,  407,    0,
  444,   -1,   -1,  447,   -1,   -1,   -1,  451,   -1,   -1,
   -1,   -1,   -1,   -1,   63,  459,  460,  461,  462,  463,
   -1,  465,  466,  467,   -1,  469,  470,   -1,   -1,   -1,
   -1,  317,   -1,   -1,  443,   -1,  480,   -1,  482,   -1,
   41,  450,   -1,   44,   -1,   -1,  366,  456,  368,  369,
   -1,   -1,  338,  339,   -1,   -1,   -1,   -1,  378,  379,
   -1,   -1,   -1,   -1,   -1,   -1,  352,   -1,   -1,   -1,
   -1,  357,   -1,   -1,  394,   -1,   -1,   -1,   -1,  488,
   -1,  490,   -1,   -1,  528,   -1,  530,  531,  532,  533,
  534,  535,  265,   -1,   -1,   -1,  269,  383,  271,  272,
  420,   -1,  388,   -1,   -1,   -1,  426,  427,   -1,  282,
   -1,  284,   -1,   -1,  287,   -1,   -1,   -1,   -1,   -1,
   -1,  441,   -1,   -1,   -1,   -1,   -1,  447,   -1,   -1,
   -1,  451,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  459,
  460,  461,  462,  463,   -1,  465,  466,  467,   -1,  469,
  470,   -1,   -1,   -1,   -1,   -1,   -1,  443,  331,   -1,
  480,   -1,  482,   -1,  450,   -1,   -1,   -1,   -1,   -1,
  456,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  361,   -1,
   -1,   -1,   -1,  366,   -1,  368,  369,   40,   -1,   42,
   43,   -1,   45,   -1,   -1,   -1,  379,   -1,  528,   -1,
  530,  531,  532,  533,  534,  535,  265,   -1,   -1,   -1,
   63,  394,  271,  272,   -1,   -1,   -1,   40,   41,   -1,
   43,   -1,   45,  282,   -1,  284,   -1,   -1,  287,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   63,  300,   -1,  426,  427,   -1,   -1,   -1,  259,   -1,
   -1,   -1,   -1,   -1,  437,   -1,   -1,   -1,  441,   -1,
   -1,   -1,  321,   -1,  447,   -1,   -1,   -1,  451,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  459,  460,  461,  462,
  463,   -1,  465,  466,  467,   -1,  469,  470,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  480,   -1,  482,
   -1,   -1,   -1,   -1,   -1,   -1,  317,  366,   -1,  368,
  369,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  378,
  379,   -1,   -1,   -1,   -1,   -1,   -1,  338,  339,   -1,
   -1,   -1,   -1,   -1,   -1,  394,   -1,   40,   -1,   -1,
   43,  352,   45,   -1,  355,  528,   -1,  530,  531,  532,
  533,  534,  535,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   63,   -1,   -1,   -1,   -1,   -1,   -1,  426,  427,   -1,
   -1,   -1,  383,   -1,   -1,   -1,  387,  388,   -1,   -1,
   -1,   -1,  441,   -1,   -1,  444,   -1,   -1,  447,   -1,
   -1,   -1,  451,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  459,  460,  461,  462,  463,  258,  465,  466,  467,   -1,
  469,  470,  265,   -1,   -1,   -1,   -1,   -1,  271,  272,
  431,  480,   -1,  482,   -1,   -1,   -1,   -1,   -1,  282,
   -1,  284,  443,   -1,  287,   -1,   -1,   -1,   -1,  450,
   -1,   -1,  265,   -1,   -1,  456,   -1,   -1,  271,  272,
   -1,   -1,   -1,   -1,   -1,   -1,  309,   -1,   -1,  282,
   -1,  284,   -1,   -1,  287,   -1,   -1,   -1,   -1,  528,
   -1,  530,  531,  532,  533,  534,  535,  300,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   40,   -1,   -1,   43,   -1,
   45,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  366,   -1,  368,  369,   -1,   63,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  379,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  394,   -1,  366,   -1,  368,  369,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  378,  379,   -1,   -1,   -1,
   -1,   -1,  265,   -1,   -1,   -1,   -1,   -1,  271,  272,
   -1,  394,   -1,  426,  427,   -1,  429,   -1,   -1,  282,
   -1,  284,   -1,   -1,  287,   -1,   -1,   -1,  441,   -1,
   -1,   -1,   -1,   -1,  447,   -1,   -1,   -1,  451,   -1,
   -1,   -1,   -1,  426,  427,   -1,  459,  460,  461,  462,
  463,    0,  465,  466,  467,   -1,  469,  470,  441,   -1,
   -1,   -1,   -1,   -1,  447,   -1,   -1,  480,  451,  482,
   40,   -1,   42,   43,   -1,   45,  459,  460,  461,  462,
  463,   -1,  465,  466,  467,   -1,  469,  470,   -1,   -1,
   -1,   -1,   41,   63,   -1,   44,   -1,  480,   -1,  482,
   -1,   -1,   -1,  366,   -1,  368,  369,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  528,  379,  530,  531,  532,
  533,  534,  535,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  394,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  528,   -1,  530,  531,  532,
  533,  534,  535,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  265,   -1,   -1,  426,  427,   -1,  271,  272,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  282,  441,  284,
   -1,   -1,  287,   -1,  447,   -1,   -1,   -1,  451,   40,
   -1,   -1,   43,   -1,   45,  300,  459,  460,  461,  462,
  463,  464,  465,  466,  467,  468,  469,  470,   -1,   -1,
   -1,   -1,   63,   -1,   -1,   -1,   -1,  480,   -1,  482,
   -1,  484,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  366,   -1,  368,  369,  528,   -1,  530,  531,  532,
  533,  534,  535,  378,  379,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  394,
   -1,   -1,   -1,   -1,   -1,  265,   -1,   -1,   -1,   -1,
   -1,  271,  272,   -1,   -1,   -1,   40,   -1,   -1,   43,
  259,   45,  282,   -1,  284,   -1,   -1,  287,   -1,   -1,
   -1,  426,  427,   -1,   -1,   -1,   -1,   -1,   -1,   63,
   -1,   -1,   -1,   -1,   -1,   -1,  441,   -1,   -1,   -1,
   -1,   -1,  447,   -1,   -1,   -1,  451,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  459,  460,  461,  462,  463,   -1,
  465,  466,  467,   -1,  469,  470,   -1,   -1,  317,   -1,
   -1,   -1,   -1,   -1,   -1,  480,   -1,  482,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  338,
  339,   -1,   -1,   -1,   -1,   -1,  366,   -1,  368,  369,
   -1,   -1,   -1,  352,   -1,   -1,  355,  258,   -1,  379,
   -1,   -1,   -1,   -1,  265,   -1,   -1,   -1,   -1,   -1,
  271,  272,   -1,  528,  394,  530,  531,  532,  533,  534,
  535,  282,   -1,  284,  383,   -1,  287,   -1,  387,  388,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  426,  427,  309,  429,
   -1,   -1,   -1,   -1,   40,   -1,   -1,   43,   -1,   45,
   -1,  441,   -1,   -1,   -1,   -1,   -1,  447,   -1,   -1,
   -1,  451,  431,   -1,   -1,   -1,   -1,   63,   -1,  459,
  460,  461,  462,  463,  443,  465,  466,  467,   -1,  469,
  470,  450,   -1,   -1,   -1,   -1,   -1,  456,   -1,   -1,
  480,   -1,  482,   -1,   -1,  366,   -1,  368,  369,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  379,   -1,
   -1,  265,   -1,   -1,   -1,   -1,   -1,  271,  272,   -1,
   -1,   -1,   -1,  394,   -1,   -1,   -1,   -1,  282,   -1,
  284,   -1,   -1,  287,   -1,   -1,   -1,   -1,  528,   -1,
  530,  531,  532,  533,  534,  535,  300,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  426,  427,   -1,   -1,   40,
   -1,   -1,   43,   -1,   45,   -1,   -1,   -1,   -1,   -1,
  441,   -1,   -1,   -1,   -1,   -1,  447,   -1,   -1,   -1,
  451,   -1,   63,   -1,   -1,   -1,   -1,   -1,  459,  460,
  461,  462,  463,   -1,  465,  466,  467,   -1,  469,  470,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  480,
   -1,  482,  366,   -1,  368,  369,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  378,  379,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  394,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  528,   -1,  530,
  531,  532,  533,  534,  535,   -1,   -1,   -1,   -1,  265,
   -1,   -1,  426,  427,  270,  271,  272,   -1,   40,   -1,
   42,   43,   -1,   45,   -1,   -1,  282,  441,  284,   -1,
   -1,  287,   -1,  447,   -1,   -1,   -1,  451,   -1,   -1,
   -1,   63,   -1,   -1,   -1,  459,  460,  461,  462,  463,
   -1,  465,  466,  467,   -1,  469,  470,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  480,   -1,  482,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  366,   -1,  368,  369,  528,   -1,  530,  531,  532,  533,
  534,  535,   -1,  379,  265,   -1,   -1,   -1,   -1,   -1,
  271,  272,   -1,   -1,   -1,   -1,   -1,   40,  394,   -1,
   43,  282,   45,  284,   -1,   -1,  287,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   63,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  426,  427,   -1,  429,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  441,   -1,   -1,   -1,   -1,
   -1,  447,   -1,   -1,   -1,  451,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  459,  460,  461,  462,  463,   -1,  465,
  466,  467,   -1,  469,  470,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  480,  366,  482,  368,  369,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  379,   -1,
   -1,   -1,   -1,  265,   -1,   -1,   -1,   -1,   -1,  271,
  272,   -1,   -1,  394,   -1,   -1,   -1,   -1,   40,   -1,
  282,   43,  284,   45,   -1,  287,   -1,   -1,   -1,   -1,
   -1,   -1,  528,   -1,  530,  531,  532,  533,  534,  535,
   -1,   63,   -1,   -1,   -1,  426,  427,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  441,   -1,   -1,   -1,   -1,   -1,  447,   -1,   -1,   -1,
  451,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  459,  460,
  461,  462,  463,  464,  465,  466,  467,   -1,  469,  470,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  480,
   -1,  482,   -1,  484,  366,   -1,  368,  369,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  379,   -1,   -1,
   -1,   -1,  265,   -1,   -1,   -1,   -1,   -1,  271,  272,
   -1,   -1,  394,   40,   -1,   -1,   43,   -1,   45,  282,
   -1,  284,   -1,   -1,  287,   -1,   -1,  528,   -1,  530,
  531,  532,  533,  534,  535,   -1,   63,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  426,  427,   -1,  429,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  441,
   -1,   -1,   -1,   -1,   -1,  447,   -1,   -1,   -1,  451,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  459,  460,  461,
  462,  463,   -1,  465,  466,  467,   -1,  469,  470,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  480,   -1,
  482,   -1,   -1,  366,   -1,  368,  369,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  379,   -1,   -1,   -1,
   -1,   -1,   -1,  265,   -1,   -1,   -1,   -1,   -1,  271,
  272,  394,   40,   -1,   -1,   43,   -1,   45,   -1,   -1,
  282,   -1,  284,   -1,   -1,  287,  528,   -1,  530,  531,
  532,  533,  534,  535,   -1,   63,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  426,  427,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  441,   -1,
   -1,   -1,   -1,   -1,  447,   -1,   -1,   -1,  451,   -1,
   -1,  454,   -1,   -1,   -1,   -1,  459,  460,  461,  462,
  463,   -1,  465,  466,  467,   -1,  469,  470,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  480,   -1,  482,
   -1,   -1,   -1,   -1,  366,   -1,  368,  369,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  378,  379,  265,   -1,
   -1,   -1,   -1,   -1,  271,  272,   -1,   -1,   -1,   -1,
   -1,   40,  394,   -1,   43,  282,   45,  284,   -1,   -1,
  287,   -1,   -1,   -1,   -1,  528,   -1,  530,  531,  532,
  533,  534,  535,   -1,   63,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  426,  427,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  441,
   -1,   -1,   -1,   -1,   -1,  447,   -1,   -1,   -1,  451,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  459,  460,  461,
  462,  463,   -1,  465,  466,  467,   -1,  469,  470,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  480,  366,
  482,  368,  369,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  379,   -1,   -1,   -1,   -1,  265,   -1,   -1,
   -1,   -1,   -1,  271,  272,   -1,   -1,  394,   -1,   -1,
   -1,   -1,   40,   -1,  282,   43,  284,   45,   -1,  287,
   -1,   -1,   -1,   -1,   -1,   -1,  528,   -1,  530,  531,
  532,  533,  534,  535,   -1,   63,   -1,   -1,   -1,  426,
  427,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  441,   -1,   -1,   -1,   -1,   -1,
  447,   -1,   -1,   -1,  451,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  459,  460,  461,  462,  463,   -1,  465,  466,
  467,  468,  469,  470,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  480,   -1,  482,   -1,   -1,  366,   -1,
  368,  369,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  378,  379,   -1,   -1,   -1,   -1,  265,   -1,   -1,   -1,
   -1,   -1,  271,  272,   -1,   -1,  394,   40,   -1,   -1,
   43,   -1,   45,  282,   -1,  284,   -1,   -1,  287,   -1,
   -1,  528,   -1,  530,  531,  532,  533,  534,  535,   -1,
   63,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  426,  427,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  441,   -1,   -1,   -1,   -1,   -1,  447,
   -1,   -1,  331,  451,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  459,  460,  461,  462,  463,   -1,  465,  466,  467,
   -1,  469,  470,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  480,   -1,  482,   -1,   -1,  366,   -1,  368,
  369,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  379,   -1,   -1,   -1,   -1,   -1,   -1,  265,   -1,   -1,
   -1,   -1,   -1,  271,  272,  394,   40,   -1,   -1,   43,
   -1,   45,   -1,   -1,  282,   -1,  284,   -1,   -1,  287,
  528,   -1,  530,  531,  532,  533,  534,  535,   -1,   63,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  426,  427,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  441,   -1,   -1,   -1,   -1,   -1,  447,   -1,
   -1,   -1,  451,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  459,  460,  461,  462,  463,   -1,  465,  466,  467,   -1,
  469,  470,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  480,   -1,  482,   -1,   -1,   -1,   -1,  366,   -1,
  368,  369,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  379,  265,   -1,   -1,   40,   -1,   -1,  271,  272,
   -1,   -1,   -1,   -1,   -1,   -1,  394,   -1,   -1,  282,
   -1,  284,   -1,   -1,  287,   -1,   -1,   -1,   63,  528,
   -1,  530,  531,  532,  533,  534,  535,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  426,  427,
   -1,  429,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  441,   -1,   -1,   -1,   -1,   -1,  447,
   -1,   -1,   -1,  451,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  459,  460,  461,  462,  463,   -1,  465,  466,  467,
   -1,  469,  470,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  480,  366,  482,  368,  369,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  379,   -1,   -1,   -1,
   -1,  265,   -1,   -1,   -1,   -1,   -1,  271,  272,   -1,
   -1,  394,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  284,   -1,   -1,  287,   -1,   -1,   -1,   -1,   -1,   -1,
  528,   -1,  530,  531,  532,  533,  534,  535,   -1,   -1,
   -1,   -1,   -1,  426,  427,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  441,   -1,
   -1,   -1,   -1,   -1,  447,   -1,   -1,   -1,  451,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  459,  460,  461,  462,
  463,   -1,  465,  466,  467,   -1,  469,  470,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  480,   -1,  482,
   -1,   -1,   -1,   -1,  368,  369,   -1,   -1,   -1,   -1,
  265,   -1,   -1,   -1,   -1,  379,  271,  272,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  284,
  394,   -1,  287,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  528,   -1,  530,  531,  532,
  533,  534,  535,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  427,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  451,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  459,  460,  461,  462,  463,
   -1,  465,  466,  467,   -1,  469,  470,   -1,   -1,   -1,
   -1,   -1,   -1,  368,  369,   -1,  480,   -1,  482,   -1,
   -1,   -1,   -1,   -1,  379,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  394,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  528,   -1,  530,  531,  532,  533,
  534,  535,  427,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  451,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  459,  460,  461,  462,  463,   -1,
  465,  466,  467,   -1,  469,  470,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  480,   -1,  482,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  528,   -1,  530,  531,  532,  533,  534,
  535,
  };

#line 2255 "SQL92-min.y"
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
  public const int _RS_START = 257;
  public const int ALL = 258;
  public const int AND = 259;
  public const int ANY = 260;
  public const int ARE = 261;
  public const int AS = 262;
  public const int ASC = 263;
  public const int AT = 264;
  public const int AVG = 265;
  public const int BETWEEN = 266;
  public const int BIT = 267;
  public const int BIT_LENGTH = 268;
  public const int BOTH = 269;
  public const int BY = 270;
  public const int CASE = 271;
  public const int CAST = 272;
  public const int CHAR = 273;
  public const int CHARACTER = 274;
  public const int CHAR_LENGTH = 275;
  public const int CHARACTER_LENGTH = 276;
  public const int CONNECT = 277;
  public const int CONNECTION = 278;
  public const int CONSTRAINT = 279;
  public const int CONSTRAINTS = 280;
  public const int CONTINUE = 281;
  public const int CONVERT = 282;
  public const int CORRESPONDING = 283;
  public const int COUNT = 284;
  public const int CREATE = 285;
  public const int CROSS = 286;
  public const int COALESCE = 287;
  public const int CURRENT = 288;
  public const int CURRENT_DATE = 289;
  public const int CURRENT_TIME = 290;
  public const int CURRENT_TIMESTAMP = 291;
  public const int CURRENT_USER = 292;
  public const int CURSOR = 293;
  public const int DATE = 294;
  public const int DAY = 295;
  public const int DEALLOCATE = 296;
  public const int DEC = 297;
  public const int DECIMAL = 298;
  public const int DECLARE = 299;
  public const int DEFAULT = 300;
  public const int DEFERRABLE = 301;
  public const int DEFERRED = 302;
  public const int DELETE = 303;
  public const int DESC = 304;
  public const int DESCRIBE = 305;
  public const int DESCRIPTOR = 306;
  public const int DIAGNOSTICS = 307;
  public const int DISCONNECT = 308;
  public const int DISTINCT = 309;
  public const int DOMAIN = 310;
  public const int DOUBLE = 311;
  public const int DROP = 312;
  public const int ELSE = 313;
  public const int END = 314;
  public const int END_EXEC = 315;
  public const int ESCAPE = 316;
  public const int EXCEPT = 317;
  public const int EXCEPTION = 318;
  public const int EXEC = 319;
  public const int EXECUTE = 320;
  public const int EXISTS = 321;
  public const int EXTERNAL = 322;
  public const int EXTRACT = 323;
  public const int FALSE = 324;
  public const int FETCH = 325;
  public const int FIRST = 326;
  public const int FLOAT = 327;
  public const int FOR = 328;
  public const int FOREIGN = 329;
  public const int FOUND = 330;
  public const int FROM = 331;
  public const int FULL = 332;
  public const int GET = 333;
  public const int GLOBAL = 334;
  public const int GO = 335;
  public const int GOTO = 336;
  public const int GRANT = 337;
  public const int GROUP = 338;
  public const int HAVING = 339;
  public const int HOUR = 340;
  public const int IDENTITY = 341;
  public const int IMMEDIATE = 342;
  public const int IN = 343;
  public const int INDICATOR = 344;
  public const int INITIALLY = 345;
  public const int INNER = 346;
  public const int INPUT = 347;
  public const int INSENSITIVE = 348;
  public const int INSERT = 349;
  public const int INT = 350;
  public const int INTEGER = 351;
  public const int INTERSECT = 352;
  public const int INTERVAL = 353;
  public const int INTO = 354;
  public const int IS = 355;
  public const int ISOLATION = 356;
  public const int JOIN = 357;
  public const int KEY = 358;
  public const int LANGUAGE = 359;
  public const int LAST = 360;
  public const int LEADING = 361;
  public const int LEFT = 362;
  public const int LEVEL = 363;
  public const int LIKE = 364;
  public const int LOCAL = 365;
  public const int LOWER = 366;
  public const int MATCH = 367;
  public const int MAX = 368;
  public const int MIN = 369;
  public const int MINUTE = 370;
  public const int MONTH = 371;
  public const int NAMES = 372;
  public const int NATIONAL = 373;
  public const int NATURAL = 374;
  public const int NCHAR = 375;
  public const int NEXT = 376;
  public const int NOT = 377;
  public const int NULL = 378;
  public const int NULLIF = 379;
  public const int NUMERIC = 380;
  public const int OCTET_LENGTH = 381;
  public const int OF = 382;
  public const int ON = 383;
  public const int ONLY = 384;
  public const int OPEN = 385;
  public const int OPTION = 386;
  public const int OR = 387;
  public const int ORDER = 388;
  public const int OUTER = 389;
  public const int OUTPUT = 390;
  public const int OVERLAPS = 391;
  public const int PAD = 392;
  public const int PARTIAL = 393;
  public const int POSITION = 394;
  public const int PREPARE = 395;
  public const int PRIMARY = 396;
  public const int PRIOR = 397;
  public const int PRIVILEGES = 398;
  public const int PROCEDURE = 399;
  public const int PUBLIC = 400;
  public const int READ = 401;
  public const int REAL = 402;
  public const int REFERENCES = 403;
  public const int RELATIVE = 404;
  public const int RESTRICT = 405;
  public const int REVOKE = 406;
  public const int RIGHT = 407;
  public const int ROLLBACK = 408;
  public const int ROWS = 409;
  public const int SCHEMA = 410;
  public const int SCROLL = 411;
  public const int SECOND = 412;
  public const int SECTION = 413;
  public const int SELECT = 414;
  public const int SESSION = 415;
  public const int SESSION_USER = 416;
  public const int SET = 417;
  public const int SIZE = 418;
  public const int SMALLINT = 419;
  public const int SOME = 420;
  public const int SPACE = 421;
  public const int SQL = 422;
  public const int SQLCODE = 423;
  public const int SQLERROR = 424;
  public const int SQLSTATE = 425;
  public const int SUBSTRING = 426;
  public const int SUM = 427;
  public const int SYSTEM_USER = 428;
  public const int TABLE = 429;
  public const int TEMPORARY = 430;
  public const int THEN = 431;
  public const int TIME = 432;
  public const int TIMESTAMP = 433;
  public const int TIMEZONE_HOUR = 434;
  public const int TIMEZONE_MINUTE = 435;
  public const int TO = 436;
  public const int TRAILING = 437;
  public const int TRANSACTION = 438;
  public const int TRANSLATE = 439;
  public const int TRANSLATION = 440;
  public const int TRIM = 441;
  public const int TRUE = 442;
  public const int UNION = 443;
  public const int UNIQUE = 444;
  public const int UNKNOWN = 445;
  public const int UPDATE = 446;
  public const int UPPER = 447;
  public const int USAGE = 448;
  public const int USER = 449;
  public const int USING = 450;
  public const int VALUE = 451;
  public const int VALUES = 452;
  public const int VARCHAR = 453;
  public const int WHEN = 454;
  public const int WHENEVER = 455;
  public const int WHERE = 456;
  public const int WITH = 457;
  public const int WRITE = 458;
  public const int XMLPI = 459;
  public const int XMLPARSE = 460;
  public const int XMLQUERY = 461;
  public const int XMLCOMMENT = 462;
  public const int XMLELEMENT = 463;
  public const int XMLATTRIBUTES = 464;
  public const int XMLCONCAT = 465;
  public const int XMLFOREST = 466;
  public const int XMLAGG = 467;
  public const int XMLNAMESPACES = 468;
  public const int XMLCDATA = 469;
  public const int XMLROOT = 470;
  public const int PASSING = 471;
  public const int TOP = 472;
  public const int _RS_END = 473;
  public const int CHARACTER_VARYING = 474;
  public const int CHAR_VARYING = 476;
  public const int DOUBLE_PRECISION = 478;
  public const int count_all_fct = 480;
  public const int xml_forest_all = 482;
  public const int xml_attributes_all = 484;
  public const int NOTLIKE = 486;
  public const int CROSSJOIN = 488;
  public const int UNIONJOIN = 490;
  public const int OPTION_NULL = 492;
  public const int OPTION_EMPTY = 494;
  public const int OPTION_ABSENT = 496;
  public const int OPTION_NIL = 498;
  public const int NO_VALUE = 500;
  public const int NO_DEFAULT = 502;
  public const int NO_CONTENT = 504;
  public const int PRESERVE_WHITESPACE = 506;
  public const int STRIP_WHITESPACE = 508;
  public const int RETURNING_CONTENT = 510;
  public const int RETURNING_SEQUENCE = 512;
  public const int not_equals_operator = 514;
  public const int greater_than_or_equals_operator = 516;
  public const int less_than_or_equals_operator = 518;
  public const int concatenation_operator = 520;
  public const int double_colon = 522;
  public const int asterisk_tag = 524;
  public const int double_slash = 526;
  public const int parameter_name = 528;
  public const int embdd_variable_name = 529;
  public const int id = 530;
  public const int unsigned_integer = 531;
  public const int unsigned_float = 532;
  public const int unsigned_double = 533;
  public const int string_literal = 534;
  public const int func = 535;
  public const int optimizer_hint = 536;
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
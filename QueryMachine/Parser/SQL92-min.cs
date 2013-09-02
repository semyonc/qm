
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
//t    "table_ref_spec : tuple_stream",
//t    "table_ref_spec : subquery",
//t    "dynamic_table : TABLE funcall",
//t    "dynamic_table : TABLE column_ref",
//t    "dynamic_table : TABLE '(' value_exp ')'",
//t    "dynamic_table : TABLE xml_query",
//t    "tuple_stream : ROW funcall",
//t    "tuple_stream : ROW column_ref",
//t    "tuple_stream : ROW '(' value_exp ')'",
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
//t    "column_ref : column_ref_primary",
//t    "column_ref_primary : qualified_name",
//t    "column_ref_primary : column_ref_primary ref_operator id",
//t    "column_ref_primary : column_ref_primary ref_operator funcall",
//t    "column_ref_primary : column_ref_primary '[' value_exp ']'",
//t    "column_ref_primary : column_ref_primary double_slash id",
//t    "qualified_name : column_name",
//t    "qualified_name : id '.' id",
//t    "unsigned_lit : unsigned_integer",
//t    "unsigned_lit : unsigned_float",
//t    "unsigned_lit : unsigned_double",
//t    "unsigned_lit : string_literal",
//t    "unsigned_lit : DATE string_literal",
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
    "XMLAGG","XMLNAMESPACES","XMLCDATA","XMLROOT","PASSING","ROW","TOP",
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
    "asterisk_tag","\".*\"","double_slash","\"//\"","ref_operator",
    "\"->\"","parameter_name","embdd_variable_name","id",
    "unsigned_integer","unsigned_float","unsigned_double",
    "string_literal","func","optimizer_hint",
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
#line 347 "SQL92-min.y"
  {            
            notation.ConfirmTag(Tag.Stmt, Descriptor.Root, yyVals[0+yyTop]);
            yyVal = notation.ResolveTag(Tag.Stmt);
            if (yyVals[-1+yyTop] != null)
                notation.Confirm((Symbol)yyVal, Descriptor.OptimizerHint, yyVals[-1+yyTop]);
      }
  break;
case 2:
#line 354 "SQL92-min.y"
  {      
            notation.ConfirmTag(Tag.Stmt, Descriptor.Root, yyVals[-1+yyTop]);
			notation.ConfirmTag(Tag.Stmt, Descriptor.Order, yyVals[0+yyTop]);						
			yyVal = notation.ResolveTag(Tag.Stmt);
            if (yyVals[-2+yyTop] != null)
                notation.Confirm((Symbol)yyVal, Descriptor.OptimizerHint, yyVals[-2+yyTop]);			
      }
  break;
case 3:
#line 365 "SQL92-min.y"
  {
         yyVal = yyVals[0+yyTop];
      }
  break;
case 4:
#line 373 "SQL92-min.y"
  {
         Symbol sym = new Symbol(Tag.Predicate);
         yyVal = notation.Confirm(sym, Descriptor.Between, yyVals[-5+yyTop], yyVals[-2+yyTop], yyVals[0+yyTop]);
         if (yyVals[-4+yyTop] != null)
           notation.Confirm(sym, Descriptor.Inverse);
      }
  break;
case 5:
#line 383 "SQL92-min.y"
  {
        yyVal = null;
      }
  break;
case 10:
#line 400 "SQL92-min.y"
  {
          yyVal = notation.Confirm(new Symbol(Tag.Expr), 
            Descriptor.NullIf, yyVals[-3+yyTop], yyVals[-1+yyTop]);
      }
  break;
case 11:
#line 408 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.Expr),
            Descriptor.Coalesce, yyVals[-1+yyTop]);
      }
  break;
case 12:
#line 416 "SQL92-min.y"
  {
		yyVal = Lisp.Cons(yyVals[0+yyTop]);
      }
  break;
case 13:
#line 420 "SQL92-min.y"
  {
		yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
	  }
  break;
case 22:
#line 451 "SQL92-min.y"
  {
         Symbol sym = new Symbol(Tag.CExpr);
         if (yyVals[-1+yyTop] == null)
			yyVal = notation.Confirm(sym, Descriptor.Substring, yyVals[-4+yyTop], yyVals[-2+yyTop]);
		 else
			yyVal = notation.Confirm(sym, Descriptor.Substring, yyVals[-4+yyTop], yyVals[-2+yyTop], yyVals[-1+yyTop]);
      }
  break;
case 23:
#line 462 "SQL92-min.y"
  {
         yyVal = null;
      }
  break;
case 24:
#line 466 "SQL92-min.y"
  {
         yyVal = yyVals[0+yyTop];
      }
  break;
case 26:
#line 474 "SQL92-min.y"
  {
           yyVal = notation.Confirm(new Symbol(Tag.CExpr), Descriptor.Concat, yyVals[-2+yyTop], yyVals[0+yyTop]);
      }
  break;
case 31:
#line 488 "SQL92-min.y"
  {
		yyVal = Lisp.Cons(yyVals[0+yyTop]);
      }
  break;
case 32:
#line 492 "SQL92-min.y"
  {
		yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
	  }
  break;
case 39:
#line 509 "SQL92-min.y"
  {
          yyVal = notation.Confirm(new Symbol(Tag.Predicate), Descriptor.Pred, yyVals[-2+yyTop], yyVals[-1+yyTop], yyVals[0+yyTop]);
      }
  break;
case 44:
#line 529 "SQL92-min.y"
  {
		yyVal = yyVals[-1+yyTop];
    }
  break;
case 45:
#line 536 "SQL92-min.y"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 46:
#line 543 "SQL92-min.y"
  {
		yyVal = yyVals[0+yyTop];
	}
  break;
case 47:
#line 550 "SQL92-min.y"
  {
		yyVal = yyVals[0+yyTop];
		if (yyVals[-2+yyTop] != null)
			notation.ConfirmTag(Tag.Join, Descriptor.JoinType, new TokenWrapper(yyVals[-2+yyTop]));
    }
  break;
case 50:
#line 564 "SQL92-min.y"
  {
          yyVal = yyVals[-1+yyTop];          
          notation.Confirm((Symbol)yyVal, Descriptor.Alias, yyVals[0+yyTop]);          
      }
  break;
case 52:
#line 573 "SQL92-min.y"
  {
          yyVal = yyVals[0+yyTop];
          notation.Confirm((Symbol)yyVal,  Descriptor.Dynatable);
      }
  break;
case 57:
#line 594 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.CaseExpr), Descriptor.ElseBranch, yyVals[0+yyTop]);
      }
  break;
case 59:
#line 605 "SQL92-min.y"
  {
      yyVal = notation.Confirm(new Symbol(Tag.Predicate), Descriptor.Exists, yyVals[0+yyTop]);
    }
  break;
case 61:
#line 620 "SQL92-min.y"
  {
         if (yyVals[-1+yyTop].Equals("-"))
			yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.UnaryMinus, yyVals[0+yyTop]);
		 else
			yyVal = 2;
       }
  break;
case 62:
#line 631 "SQL92-min.y"
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
#line 653 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.CExpr), 
			Descriptor.StringConvert,  yyVals[-3+yyTop], yyVals[-1+yyTop]);
      }
  break;
case 67:
#line 665 "SQL92-min.y"
  {
		notation.ConfirmTag(Tag.SQuery, Descriptor.From, yyVals[0+yyTop]);
      }
  break;
case 68:
#line 672 "SQL92-min.y"
  {
        yyVal = Lisp.Cons(yyVals[0+yyTop]);
      }
  break;
case 69:
#line 676 "SQL92-min.y"
  {
		yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
	  }
  break;
case 70:
#line 684 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.AggExpr), Descriptor.Aggregate, new TokenWrapper(yyVals[-3+yyTop]), yyVals[-1+yyTop]);
      }
  break;
case 71:
#line 689 "SQL92-min.y"
  {
         Symbol sym = new Symbol(Tag.AggExpr);
         yyVal = notation.Confirm(sym, Descriptor.Aggregate, new TokenWrapper(yyVals[-4+yyTop]), yyVals[-1+yyTop]);
         if ((int)yyVals[-2+yyTop] == Token.DISTINCT)
            notation.Confirm(sym, Descriptor.Distinct);
      }
  break;
case 72:
#line 699 "SQL92-min.y"
  {
         yyVal = new Parameter(yyVals[0+yyTop].ToString()); 
      }
  break;
case 76:
#line 712 "SQL92-min.y"
  {
        yyVal = yyVals[0+yyTop];
		notation.ConfirmTag(Tag.SQuery, Descriptor.GroupingColumnRef, yyVals[0+yyTop]);
      }
  break;
case 77:
#line 720 "SQL92-min.y"
  {
		yyVal = Lisp.Cons(yyVals[0+yyTop]);
      }
  break;
case 78:
#line 724 "SQL92-min.y"
  {
		yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
      }
  break;
case 80:
#line 733 "SQL92-min.y"
  {
		notation.ConfirmTag(Tag.SQuery, Descriptor.GroupBy, yyVals[0+yyTop]);
      }
  break;
case 82:
#line 741 "SQL92-min.y"
  {
		notation.ConfirmTag(Tag.SQuery, Descriptor.Having, yyVals[0+yyTop]);
      }
  break;
case 86:
#line 758 "SQL92-min.y"
  {         
         Symbol sym = new Symbol(Tag.Predicate);
		 yyVal = notation.Confirm(sym, Descriptor.InSet, yyVals[-3+yyTop], yyVals[0+yyTop]);            
		 if (yyVals[-2+yyTop] != null)
		   notation.Confirm(sym, Descriptor.Inverse);
      }
  break;
case 88:
#line 769 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.ValueList), Descriptor.ValueList, yyVals[-1+yyTop]);
      }
  break;
case 89:
#line 776 "SQL92-min.y"
  {
         yyVal = Lisp.Cons(yyVals[0+yyTop]);
      }
  break;
case 90:
#line 780 "SQL92-min.y"
  {
         yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
      }
  break;
case 91:
#line 787 "SQL92-min.y"
  {
		notation.ConfirmTag(Tag.Join, Descriptor.CrossJoin, yyVals[-1+yyTop], yyVals[0+yyTop]);
		yyVal = notation.ResolveTag(Tag.Join);
      }
  break;
case 92:
#line 792 "SQL92-min.y"
  {
		notation.ConfirmTag(Tag.Join, Descriptor.UnionJoin, yyVals[-1+yyTop], yyVals[0+yyTop]);
		yyVal = notation.ResolveTag(Tag.Join);      
      }
  break;
case 93:
#line 797 "SQL92-min.y"
  {
		notation.ConfirmTag(Tag.Join, Descriptor.NaturalJoin, yyVals[-1+yyTop], yyVals[0+yyTop]);
		yyVal = notation.ResolveTag(Tag.Join);      
      }
  break;
case 94:
#line 802 "SQL92-min.y"
  {
		notation.ConfirmTag(Tag.Join, Descriptor.QualifiedJoin, yyVals[-1+yyTop], yyVals[0+yyTop]);
		yyVal = notation.ResolveTag(Tag.Join);      
      }
  break;
case 95:
#line 807 "SQL92-min.y"
  {
		yyVal = notation.Confirm(new Symbol(Tag.Join), Descriptor.Branch, yyVals[-1+yyTop]);
      }
  break;
case 97:
#line 818 "SQL92-min.y"
  {
		yyVal = yyVals[0+yyTop];
      }
  break;
case 104:
#line 837 "SQL92-min.y"
  {
		  notation.ConfirmTag(Tag.Join, Descriptor.Outer);
	  }
  break;
case 105:
#line 844 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.Predicate), 
			Descriptor.Like, yyVals[-2+yyTop], yyVals[0+yyTop]);           
      }
  break;
case 106:
#line 849 "SQL92-min.y"
  {               
         Symbol sym = new Symbol(Tag.Predicate);
         yyVal = notation.Confirm(sym, Descriptor.Like, yyVals[-2+yyTop], yyVals[0+yyTop]);           
		 notation.Confirm(sym, Descriptor.Inverse);
      }
  break;
case 107:
#line 855 "SQL92-min.y"
  {
         Symbol sym = new Symbol(Tag.Predicate);
         yyVal = notation.Confirm(sym, Descriptor.Like, yyVals[-4+yyTop], yyVals[-2+yyTop]);           
		 notation.Confirm(sym, Descriptor.Escape, yyVals[0+yyTop]);
      }
  break;
case 108:
#line 861 "SQL92-min.y"
  {
         Symbol sym = new Symbol(Tag.Predicate);
         yyVal = notation.Confirm(sym, Descriptor.Like, yyVals[-4+yyTop], yyVals[-2+yyTop]);           
         notation.Confirm(sym, Descriptor.Inverse);
		 notation.Confirm(sym, Descriptor.Escape, yyVals[0+yyTop]);
      }
  break;
case 109:
#line 873 "SQL92-min.y"
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
#line 885 "SQL92-min.y"
  {
        yyVal = null;
      }
  break;
case 112:
#line 893 "SQL92-min.y"
  {
        yyVal = null;
      }
  break;
case 116:
#line 906 "SQL92-min.y"
  {
		yyVal = notation.Confirm(new Symbol(Tag.Join), Descriptor.Using, yyVals[-1+yyTop]);
      }
  break;
case 117:
#line 913 "SQL92-min.y"
  {
		yyVal = notation.Confirm(new Symbol(Tag.Join), Descriptor.Constraint, new TokenWrapper(yyVals[-1+yyTop]));
      }
  break;
case 118:
#line 917 "SQL92-min.y"
  {
        yyVal = notation.Confirm(new Symbol(Tag.Join), Descriptor.Constraint, new TokenWrapper(yyVals[-1+yyTop]));
      }
  break;
case 119:
#line 921 "SQL92-min.y"
  {
        yyVal = notation.Confirm(new Symbol(Tag.Join), Descriptor.Constraint, yyVals[0+yyTop]);
      }
  break;
case 121:
#line 929 "SQL92-min.y"
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
#line 953 "SQL92-min.y"
  {
        yyVal = null;
      }
  break;
case 126:
#line 961 "SQL92-min.y"
  {
		yyVal = null;
      }
  break;
case 129:
#line 970 "SQL92-min.y"
  {
         Symbol sym = new Symbol(Tag.QueryTerm);
         notation.Confirm(sym, Descriptor.Intersect, yyVals[-4+yyTop], yyVals[0+yyTop]); 
         if (yyVals[-2+yyTop] != null)
           notation.Confirm(sym, Descriptor.Distinct);
         yyVal = sym;
      }
  break;
case 130:
#line 982 "SQL92-min.y"
  {
        Symbol sym = new Symbol(Tag.Predicate);
        yyVal = notation.Confirm(sym, Descriptor.IsNull, yyVals[-3+yyTop]);
        if (yyVals[-1+yyTop] != null)
          notation.Confirm(sym, Descriptor.Inverse);
      }
  break;
case 136:
#line 1003 "SQL92-min.y"
  {
		  Symbol sym = new Symbol(Tag.Expr);
          if (yyVals[-1+yyTop].Equals("+"))
			yyVal = notation.Confirm(sym, Descriptor.Add, yyVals[-2+yyTop], yyVals[0+yyTop]);
		  else
		    yyVal = notation.Confirm(sym, Descriptor.Sub, yyVals[-2+yyTop], yyVals[0+yyTop]);
      }
  break;
case 141:
#line 1024 "SQL92-min.y"
  {
			yyVal  = yyVals[0+yyTop];
	  }
  break;
case 145:
#line 1038 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.Predicate), Descriptor.Overlaps, yyVals[-2+yyTop], yyVals[0+yyTop]);
      }
  break;
case 149:
#line 1055 "SQL92-min.y"
  {
           yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.PosString, yyVals[-1+yyTop], yyVals[-3+yyTop]);
       }
  break;
case 160:
#line 1075 "SQL92-min.y"
  {
		yyVal = yyVals[-1+yyTop];
		if (yyVals[-3+yyTop] != null)
			notation.ConfirmTag(Tag.Join, Descriptor.JoinType, new TokenWrapper(yyVals[-3+yyTop]));		
		notation.ConfirmTag(Tag.Join, Descriptor.JoinSpec, yyVals[0+yyTop]);		
	  }
  break;
case 161:
#line 1085 "SQL92-min.y"
  {
		yyVal = null;
	  }
  break;
case 163:
#line 1094 "SQL92-min.y"
  {
		  yyVal = notation.Confirm(new Symbol(Tag.Predicate), 
		    Descriptor.QuantifiedPred, yyVals[-3+yyTop], yyVals[-2+yyTop], new TokenWrapper(yyVals[-1+yyTop]), yyVals[0+yyTop]);	  
      }
  break;
case 167:
#line 1109 "SQL92-min.y"
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
#line 1121 "SQL92-min.y"
  {
         yyVal = null;
      }
  break;
case 169:
#line 1125 "SQL92-min.y"
  {
         yyVal = yyVals[-1+yyTop]; 
      }
  break;
case 174:
#line 1143 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.RowValue), 
            Descriptor.RowValue, Lisp.Append(Lisp.Cons(yyVals[-3+yyTop]), yyVals[-1+yyTop]));
      }
  break;
case 176:
#line 1153 "SQL92-min.y"
  { 
		yyVal = new TokenWrapper(yyVals[0+yyTop]);
	  }
  break;
case 177:
#line 1157 "SQL92-min.y"
  {
        yyVal = new TokenWrapper(yyVals[0+yyTop]);
      }
  break;
case 178:
#line 1164 "SQL92-min.y"
  {
        yyVal = Lisp.Cons(yyVals[0+yyTop]);
      }
  break;
case 179:
#line 1168 "SQL92-min.y"
  {
        yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
      }
  break;
case 180:
#line 1177 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.CaseExpr), Descriptor.Case, yyVals[-1+yyTop]);         
      }
  break;
case 181:
#line 1184 "SQL92-min.y"
  {
         object clause_list = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[-1+yyTop]));
         yyVal = notation.Confirm(new Symbol(Tag.CaseExpr), Descriptor.Case, clause_list);         
      }
  break;
case 182:
#line 1192 "SQL92-min.y"
  {
         yyVal = Lisp.Cons(yyVals[0+yyTop]);
      }
  break;
case 183:
#line 1196 "SQL92-min.y"
  {
         yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(yyVals[0+yyTop]));
      }
  break;
case 184:
#line 1203 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.CaseExpr), Descriptor.CaseBranch, yyVals[-2+yyTop], yyVals[0+yyTop]);
      }
  break;
case 187:
#line 1215 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.BooleanExpr), Descriptor.LogicalOR, yyVals[-2+yyTop], yyVals[0+yyTop]);
      }
  break;
case 190:
#line 1227 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.BooleanExpr), Descriptor.LogicalAND, yyVals[-2+yyTop], yyVals[0+yyTop]);
      }
  break;
case 193:
#line 1239 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.BooleanExpr), Descriptor.Inverse, yyVals[0+yyTop]);
      }
  break;
case 195:
#line 1247 "SQL92-min.y"
  {
         Symbol sym = new Symbol(Tag.BooleanExpr);
         yyVal = notation.Confirm(sym, Descriptor.BooleanTest, new TokenWrapper(yyVals[0+yyTop]), yyVals[-3+yyTop]);
		 if (yyVals[-1+yyTop] != null)
		   notation.Confirm(sym, Descriptor.Inverse);                  
      }
  break;
case 197:
#line 1258 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.BooleanExpr),
           Descriptor.Branch, yyVals[-1+yyTop]);
      }
  break;
case 201:
#line 1272 "SQL92-min.y"
  {
          notation.ConfirmTag(Tag.SQuery, Descriptor.Select, null); 
      }
  break;
case 202:
#line 1276 "SQL92-min.y"
  {
          notation.ConfirmTag(Tag.SQuery, Descriptor.Select, yyVals[0+yyTop]); 
      }
  break;
case 203:
#line 1283 "SQL92-min.y"
  {
          yyVal = Lisp.Cons(yyVals[0+yyTop]);
      }
  break;
case 204:
#line 1287 "SQL92-min.y"
  {
         yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
      }
  break;
case 205:
#line 1294 "SQL92-min.y"
  {		
		 yyVal = notation.Confirm(new Symbol(Tag.TableFields), Descriptor.TableFields, yyVals[-1+yyTop]);   
      }
  break;
case 207:
#line 1302 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.AggExpr), Descriptor.AggCount);
      }
  break;
case 215:
#line 1319 "SQL92-min.y"
  {
           notation.EnterContext();
           yyVal = Token.ALL;
        }
  break;
case 216:
#line 1324 "SQL92-min.y"
  {
           notation.EnterContext();
           yyVal = yyVals[0+yyTop];
        }
  break;
case 221:
#line 1344 "SQL92-min.y"
  {
          yyVal = notation.Confirm(new Symbol(Tag.CaseExpr), Descriptor.Case, yyVals[-2+yyTop], yyVals[-1+yyTop]);              
      }
  break;
case 222:
#line 1351 "SQL92-min.y"
  {
         object clause_list = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[-1+yyTop]));
         yyVal = notation.Confirm(new Symbol(Tag.CaseExpr), Descriptor.Case, yyVals[-3+yyTop], clause_list);
      }
  break;
case 223:
#line 1359 "SQL92-min.y"
  {
         yyVal = Lisp.Cons(yyVals[0+yyTop]);
      }
  break;
case 224:
#line 1363 "SQL92-min.y"
  {
         yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(yyVals[0+yyTop]));
      }
  break;
case 227:
#line 1376 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.CaseExpr), Descriptor.CaseBranch, yyVals[-2+yyTop], yyVals[0+yyTop]);
      }
  break;
case 229:
#line 1384 "SQL92-min.y"
  {
			yyVal = new IntegerValue(yyVals[0+yyTop]);
      }
  break;
case 230:
#line 1392 "SQL92-min.y"
  {
        yyVal = yyVals[-1+yyTop]; 
        if ((int)yyVals[0+yyTop] == Token.DESC) 
			notation.Confirm((Symbol)yyVals[-1+yyTop], Descriptor.Desc);
      }
  break;
case 231:
#line 1401 "SQL92-min.y"
  {
        yyVal = Token.ASC;
      }
  break;
case 233:
#line 1409 "SQL92-min.y"
  {
			yyVal = Lisp.Cons(yyVals[0+yyTop]);
      }
  break;
case 234:
#line 1413 "SQL92-min.y"
  {
            yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
      }
  break;
case 238:
#line 1432 "SQL92-min.y"
  {
         yyVal = yyVals[-1+yyTop];
         if (yyVals[-2+yyTop] != null)
           notation.Confirm((Symbol)yyVals[-1+yyTop], Descriptor.OptimizerHint, yyVals[-2+yyTop]);
      }
  break;
case 239:
#line 1438 "SQL92-min.y"
  {
         yyVal = yyVals[-2+yyTop];
         notation.Confirm((Symbol)yyVals[-2+yyTop], Descriptor.Order, yyVals[-1+yyTop]);  
         if (yyVals[-3+yyTop] != null)
           notation.Confirm((Symbol)yyVals[-2+yyTop], Descriptor.OptimizerHint, yyVals[-3+yyTop]);         
      }
  break;
case 245:
#line 1462 "SQL92-min.y"
  {
		yyVal = yyVals[-1+yyTop];
		notation.Confirm((Symbol)yyVal, Descriptor.Alias, yyVals[0+yyTop]);
    }
  break;
case 250:
#line 1477 "SQL92-min.y"
  {
        yyVal = notation.Confirm(new Symbol(Tag.Dynatable), Descriptor.Dynatable, yyVals[0+yyTop]);
    }
  break;
case 251:
#line 1481 "SQL92-min.y"
  {
        yyVal = notation.Confirm(new Symbol(Tag.Dynatable), Descriptor.Dynatable, yyVals[0+yyTop]);
    }
  break;
case 252:
#line 1485 "SQL92-min.y"
  {
        yyVal = notation.Confirm(new Symbol(Tag.Dynatable), Descriptor.Dynatable, yyVals[-2+yyTop]);
    }
  break;
case 253:
#line 1489 "SQL92-min.y"
  {
		yyVal = notation.Confirm(new Symbol(Tag.Dynatable), Descriptor.Dynatable, yyVals[0+yyTop]);
    }
  break;
case 254:
#line 1496 "SQL92-min.y"
  {
	    yyVal = notation.Confirm(new Symbol(Tag.Tuple), Descriptor.Tuple, yyVals[0+yyTop]);
	}
  break;
case 255:
#line 1500 "SQL92-min.y"
  {
	    yyVal = notation.Confirm(new Symbol(Tag.Tuple), Descriptor.Tuple, yyVals[0+yyTop]);
	}
  break;
case 256:
#line 1504 "SQL92-min.y"
  {
	    yyVal = notation.Confirm(new Symbol(Tag.Tuple), Descriptor.Tuple, yyVals[-2+yyTop]);
	}
  break;
case 257:
#line 1511 "SQL92-min.y"
  {
		yyVal = new Qname(yyVals[0+yyTop]);
    }
  break;
case 258:
#line 1516 "SQL92-min.y"
  {
		yyVal = new Qname(yyVals[-3+yyTop]);
		notation.Confirm((Symbol)yyVal, Descriptor.DerivedColumns, yyVals[-1+yyTop]);
    }
  break;
case 259:
#line 1524 "SQL92-min.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.TableConstructor), 
          Descriptor.TableValue, yyVals[0+yyTop]);
    }
  break;
case 260:
#line 1532 "SQL92-min.y"
  {
      yyVal = Lisp.Cons(yyVals[0+yyTop]);
    }
  break;
case 261:
#line 1536 "SQL92-min.y"
  {
      yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
    }
  break;
case 263:
#line 1544 "SQL92-min.y"
  {
        yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Mul, yyVals[-2+yyTop], yyVals[0+yyTop]);
    }
  break;
case 264:
#line 1548 "SQL92-min.y"
  {
        yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Div, yyVals[-2+yyTop], yyVals[0+yyTop]);
    }
  break;
case 265:
#line 1555 "SQL92-min.y"
  { 
        yyVal = yyVals[-1+yyTop];
      }
  break;
case 266:
#line 1562 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.CExpr), Descriptor.StringTrim, 
			new TokenWrapper(Token.BOTH), new Literal(" "), yyVals[0+yyTop]);
      }
  break;
case 267:
#line 1567 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.CExpr), Descriptor.StringTrim, 
            new TokenWrapper(Token.BOTH), new Literal(" "), yyVals[0+yyTop]);
      }
  break;
case 268:
#line 1572 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.CExpr), Descriptor.StringTrim, 
            new TokenWrapper(Token.BOTH), yyVals[-2+yyTop], yyVals[0+yyTop]);
      }
  break;
case 269:
#line 1577 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.CExpr), Descriptor.StringTrim, 
			new TokenWrapper(yyVals[-2+yyTop]), new Literal(" "), yyVals[0+yyTop]);
      }
  break;
case 270:
#line 1582 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.CExpr), Descriptor.StringTrim, 
			new TokenWrapper(yyVals[-3+yyTop]), yyVals[-2+yyTop], yyVals[0+yyTop]);
      }
  break;
case 276:
#line 1604 "SQL92-min.y"
  {
        yyVal = notation.Confirm(new Symbol(Tag.Predicate), Descriptor.Unique, yyVals[0+yyTop]);
      }
  break;
case 288:
#line 1628 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Branch, yyVals[-1+yyTop]);
      }
  break;
case 289:
#line 1635 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.Funcall), Descriptor.Funcall, new Qname(yyVals[-2+yyTop]), null);
      }
  break;
case 290:
#line 1639 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.Funcall), Descriptor.Funcall, new Qname(yyVals[-3+yyTop]), yyVals[-1+yyTop]);
      }
  break;
case 293:
#line 1651 "SQL92-min.y"
  {
        notation.ConfirmTag(Tag.SQuery, Descriptor.Where, yyVals[0+yyTop]);
    }
  break;
case 294:
#line 1658 "SQL92-min.y"
  {
       yyVal = null;
    }
  break;
case 296:
#line 1666 "SQL92-min.y"
  {
       yyVal = Lisp.Cons(yyVals[0+yyTop]);
    }
  break;
case 297:
#line 1670 "SQL92-min.y"
  {
       yyVal = Lisp.Append(yyVals[-1+yyTop], Lisp.Cons(yyVals[0+yyTop])); 
    }
  break;
case 298:
#line 1677 "SQL92-min.y"
  {
	   yyVal = new Qname(yyVals[0+yyTop]);
    }
  break;
case 299:
#line 1684 "SQL92-min.y"
  {
       yyVal = yyVals[0+yyTop];
       notation.ConfirmTag(Tag.SQuery, Descriptor.TableName, yyVals[0+yyTop]);
    }
  break;
case 300:
#line 1689 "SQL92-min.y"
  {      
       notation.ConfirmTag(Tag.SQuery, Descriptor.TableName, yyVals[0+yyTop]);
    }
  break;
case 301:
#line 1696 "SQL92-min.y"
  {
       yyVal = yyVals[0+yyTop];
       notation.Confirm((Symbol)yyVal, Descriptor.Prefix, new Literal(yyVals[-2+yyTop]));       
    }
  break;
case 303:
#line 1708 "SQL92-min.y"
  {
       yyVal = yyVals[0+yyTop];
       notation.Confirm((Symbol)yyVals[0+yyTop], Descriptor.ColumnRef);
    }
  break;
case 305:
#line 1717 "SQL92-min.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Dref), Descriptor.Dref, yyVals[-2+yyTop], new Literal((String)yyVals[0+yyTop]));
    }
  break;
case 306:
#line 1721 "SQL92-min.y"
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
case 307:
#line 1733 "SQL92-min.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Dref), Descriptor.At, yyVals[-3+yyTop], yyVals[-1+yyTop]);
    }
  break;
case 308:
#line 1737 "SQL92-min.y"
  {
       yyVal = notation.Confirm(new Symbol(Tag.Dref), Descriptor.Wref, yyVals[-2+yyTop], new Literal((String)yyVals[0+yyTop]));    
    }
  break;
case 310:
#line 1745 "SQL92-min.y"
  {
	   yyVal = new Qname(new String[] { (string)yyVals[-2+yyTop], (string)yyVals[0+yyTop] });
	}
  break;
case 311:
#line 1752 "SQL92-min.y"
  {
      yyVal = new IntegerValue(yyVals[0+yyTop]);
    }
  break;
case 312:
#line 1756 "SQL92-min.y"
  {
      yyVal = new DecimalValue(yyVals[0+yyTop]);
    }
  break;
case 313:
#line 1760 "SQL92-min.y"
  {
      yyVal = new DoubleValue(yyVals[0+yyTop]);
    }
  break;
case 314:
#line 1764 "SQL92-min.y"
  {
      yyVal = new Literal(yyVals[0+yyTop]);
    }
  break;
case 315:
#line 1768 "SQL92-min.y"
  {
	  yyVal = new DateTimeValue(DateTime.Parse((string)yyVals[0+yyTop])); 
	}
  break;
case 325:
#line 1788 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLQuery, yyVals[-2+yyTop], null, yyVals[-1+yyTop]);     
      }
  break;
case 326:
#line 1792 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLQuery, yyVals[-4+yyTop], yyVals[-2+yyTop], yyVals[-1+yyTop]);     
      }
  break;
case 327:
#line 1796 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLQuery, yyVals[-6+yyTop], yyVals[-2+yyTop], yyVals[-1+yyTop]);     
      }
  break;
case 328:
#line 1803 "SQL92-min.y"
  {
         yyVal = null;
      }
  break;
case 329:
#line 1807 "SQL92-min.y"
  {
         yyVal = new TokenWrapper(Token.RETURNING_CONTENT);
      }
  break;
case 330:
#line 1811 "SQL92-min.y"
  {
         yyVal = new TokenWrapper(Token.RETURNING_SEQUENCE);
      }
  break;
case 331:
#line 1818 "SQL92-min.y"
  {
         yyVal = Lisp.Cons(yyVals[0+yyTop]);
      }
  break;
case 332:
#line 1822 "SQL92-min.y"
  {
         yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
      }
  break;
case 333:
#line 1829 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.AggExpr), Descriptor.Aggregate, 
            new TokenWrapper(Token.XMLAGG), yyVals[-1+yyTop]);
      }
  break;
case 334:
#line 1834 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.AggExpr), Descriptor.Aggregate, 
            new TokenWrapper(Token.XMLAGG), yyVals[-2+yyTop]);
         notation.Confirm((Symbol)yyVal, Descriptor.Order, yyVals[-1+yyTop]);
      }
  break;
case 335:
#line 1843 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.SQLX), 
            Descriptor.XMLConcat, yyVals[-1+yyTop]); 
      }
  break;
case 336:
#line 1851 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLForestAll);               
      }
  break;
case 337:
#line 1855 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.SQLX), 
            Descriptor.XMLForest, yyVals[-1+yyTop]);         
      }
  break;
case 338:
#line 1860 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.SQLX), 
            Descriptor.XMLForest, yyVals[-1+yyTop]);  
         notation.Confirm((Symbol)yyVal, Descriptor.XMLNamespaces, yyVals[-3+yyTop]);
      }
  break;
case 339:
#line 1869 "SQL92-min.y"
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
case 340:
#line 1884 "SQL92-min.y"
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
case 341:
#line 1895 "SQL92-min.y"
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
case 342:
#line 1909 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLComment, yyVals[-1+yyTop]);
      }
  break;
case 343:
#line 1916 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLCDATA, yyVals[-1+yyTop]);
      }
  break;
case 344:
#line 1923 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLRoot, yyVals[-1+yyTop], null, null);
      }
  break;
case 345:
#line 1927 "SQL92-min.y"
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
case 346:
#line 1938 "SQL92-min.y"
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
case 348:
#line 1960 "SQL92-min.y"
  {
         yyVal = null;
      }
  break;
case 349:
#line 1967 "SQL92-min.y"
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
case 350:
#line 1977 "SQL92-min.y"
  {
         yyVal = null;
      }
  break;
case 351:
#line 1984 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLElement, yyVals[-1+yyTop], null);
      }
  break;
case 352:
#line 1988 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLElement, yyVals[-3+yyTop], null);
         notation.Confirm((Symbol)yyVal, Descriptor.XMLNamespaces, yyVals[-1+yyTop]);
      }
  break;
case 353:
#line 1993 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLElement, yyVals[-3+yyTop], null);
         notation.Confirm((Symbol)yyVal, Descriptor.XMLAttributes, yyVals[-1+yyTop]);
      }
  break;
case 354:
#line 1999 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLElement, yyVals[-5+yyTop], null);
         notation.Confirm((Symbol)yyVal, Descriptor.XMLNamespaces, yyVals[-3+yyTop]);
         notation.Confirm((Symbol)yyVal, Descriptor.XMLAttributes, yyVals[-1+yyTop]);
      }
  break;
case 355:
#line 2005 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLElement, yyVals[-3+yyTop], yyVals[-1+yyTop]);         
      }
  break;
case 356:
#line 2009 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLElement, yyVals[-5+yyTop], yyVals[-1+yyTop]);
         notation.Confirm((Symbol)yyVal, Descriptor.XMLNamespaces, yyVals[-3+yyTop]);
      }
  break;
case 357:
#line 2014 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLElement, yyVals[-5+yyTop], yyVals[-1+yyTop]);
         notation.Confirm((Symbol)yyVal, Descriptor.XMLAttributes, yyVals[-3+yyTop]);
      }
  break;
case 358:
#line 2020 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLElement, yyVals[-7+yyTop], yyVals[-1+yyTop]);
         notation.Confirm((Symbol)yyVal, Descriptor.XMLNamespaces, yyVals[-5+yyTop]);
         notation.Confirm((Symbol)yyVal, Descriptor.XMLAttributes, yyVals[-3+yyTop]);      
      }
  break;
case 359:
#line 2029 "SQL92-min.y"
  {
         yyVal = yyVals[-1+yyTop];
      }
  break;
case 360:
#line 2036 "SQL92-min.y"
  {
         yyVal = Lisp.Cons(yyVals[0+yyTop]);
      }
  break;
case 361:
#line 2040 "SQL92-min.y"
  {
         yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
      }
  break;
case 364:
#line 2052 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.SQLX),  
            Descriptor.DeclNamespace, new Literal(yyVals[-2+yyTop]), yyVals[0+yyTop]);  
      }
  break;
case 365:
#line 2060 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.SQLX), 
            Descriptor.DeclNamespace, new Literal(yyVals[0+yyTop]), null);        
      }
  break;
case 366:
#line 2065 "SQL92-min.y"
  {
         yyVal = null;
      }
  break;
case 367:
#line 2072 "SQL92-min.y"
  {
         yyVal = null;
      }
  break;
case 368:
#line 2076 "SQL92-min.y"
  {
         yyVal = yyVals[-1+yyTop];
      }
  break;
case 369:
#line 2083 "SQL92-min.y"
  {
          yyVal = Lisp.Cons(yyVals[0+yyTop]);
      }
  break;
case 370:
#line 2087 "SQL92-min.y"
  {
          yyVal = Lisp.Append(yyVals[-2+yyTop], Lisp.Cons(yyVals[0+yyTop]));
      }
  break;
case 372:
#line 2095 "SQL92-min.y"
  {
         yyVal = yyVals[-1+yyTop];
         notation.Confirm((Symbol)yyVal, Descriptor.ContentOption, yyVals[0+yyTop]);
      }
  break;
case 373:
#line 2100 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.TableFields), Descriptor.TableFields, yyVals[-1+yyTop]);   
      }
  break;
case 374:
#line 2104 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.TableFields), Descriptor.TableFields, yyVals[-2+yyTop]);   
         notation.Confirm((Symbol)yyVal, Descriptor.ContentOption, yyVals[0+yyTop]);
      }
  break;
case 376:
#line 2113 "SQL92-min.y"
  {
         yyVal = yyVals[-1+yyTop];
         notation.Confirm((Symbol)yyVal, Descriptor.Alias, yyVals[0+yyTop]);
      }
  break;
case 377:
#line 2121 "SQL92-min.y"
  { 
         yyVal = null;
      }
  break;
case 379:
#line 2129 "SQL92-min.y"
  {
          yyVal = new TokenWrapper(Token.PRESERVE_WHITESPACE);
      }
  break;
case 380:
#line 2133 "SQL92-min.y"
  {
          yyVal = new TokenWrapper(Token.STRIP_WHITESPACE);
      }
  break;
case 381:
#line 2140 "SQL92-min.y"
  {
         yyVal = new TokenWrapper(Token.OPTION_NULL);
      }
  break;
case 382:
#line 2144 "SQL92-min.y"
  {
         yyVal = new TokenWrapper(Token.OPTION_EMPTY);
      }
  break;
case 383:
#line 2148 "SQL92-min.y"
  {
         yyVal = new TokenWrapper(Token.OPTION_ABSENT);
      }
  break;
case 384:
#line 2152 "SQL92-min.y"
  {
         yyVal = new TokenWrapper(Token.OPTION_NIL);
      }
  break;
case 385:
#line 2156 "SQL92-min.y"
  {
         yyVal = new TokenWrapper(Token.NO_CONTENT);
      }
  break;
case 388:
#line 2168 "SQL92-min.y"
  {
         yyVal = notation.Confirm(new Symbol(Tag.Expr), 
            Descriptor.Cast, yyVals[-3+yyTop], yyVals[-1+yyTop]);                 
      }
  break;
case 389:
#line 2176 "SQL92-min.y"
  {
         yyVal = null;
      }
  break;
case 393:
#line 2186 "SQL92-min.y"
  {
         yyVal = new TokenWrapper(yyVals[0+yyTop]);
      }
  break;
case 395:
#line 2194 "SQL92-min.y"
  {
         yyVal = new TokenWrapper(yyVals[0+yyTop]);
      }
  break;
case 396:
#line 2198 "SQL92-min.y"
  {
         yyVal = new TokenWrapper(yyVals[-3+yyTop]);
         notation.Confirm((Symbol)yyVal, Descriptor.Typelen, yyVals[-1+yyTop]);
      }
  break;
case 404:
#line 2216 "SQL92-min.y"
  {
         yyVal = new TokenWrapper(yyVals[0+yyTop]);
      }
  break;
case 410:
#line 2231 "SQL92-min.y"
  {
         yyVal = new TokenWrapper(yyVals[0+yyTop]);
      }
  break;
case 411:
#line 2235 "SQL92-min.y"
  {
         yyVal = new TokenWrapper(yyVals[-3+yyTop]);
         notation.Confirm((Symbol)yyVal, Descriptor.Typeprec, yyVals[-1+yyTop]);         
      }
  break;
case 412:
#line 2240 "SQL92-min.y"
  {
         yyVal = new TokenWrapper(yyVals[-5+yyTop]);
         notation.Confirm((Symbol)yyVal, Descriptor.Typeprec, yyVals[-3+yyTop]);
         notation.Confirm((Symbol)yyVal, Descriptor.Typescale, yyVals[-1+yyTop]);         
      }
  break;
case 416:
#line 2255 "SQL92-min.y"
  {
         yyVal = new TokenWrapper(Token.FLOAT);
      }
  break;
case 417:
#line 2259 "SQL92-min.y"
  {
         yyVal = new TokenWrapper(Token.FLOAT);
         notation.Confirm((Symbol)yyVal, Descriptor.Typeprec, yyVals[-1+yyTop]);         
      }
  break;
case 418:
#line 2267 "SQL92-min.y"
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
  121,   42,   42,   82,   82,  147,  147,  147,  147,  150,
  150,  150,  150,  151,  151,  151,  148,  148,  141,  154,
  154,  108,  108,  108,   33,  155,  155,  155,  155,  155,
  157,  156,  158,  158,  158,  115,  159,  159,   14,  104,
  104,  104,  104,  104,  104,  104,  104,  104,  152,  152,
  142,  146,  146,    1,    1,  163,  163,    6,  149,  149,
  161,  164,   71,  165,  165,  165,  165,  165,   61,   61,
  160,  160,  160,  160,  160,  106,  106,  106,  106,  106,
  106,  106,  106,  106,  153,  153,  153,  174,  174,  174,
  175,  175,  138,  138,  168,  171,  171,  171,  170,  166,
  166,  167,  172,  173,  173,  173,  179,  179,  180,  180,
  169,  169,  169,  169,  169,  169,  169,  169,  177,  182,
  182,  183,  183,  184,  185,  185,  181,  181,  176,  176,
  186,  186,  186,  186,  187,  187,  178,  178,  189,  189,
  188,  188,  188,  188,  188,  190,  190,  162,  191,  191,
  192,  192,  192,  192,  193,  193,  196,  196,  196,  196,
  196,  194,  194,  194,  199,  199,  199,  199,  199,  197,
  197,  197,  200,  200,  200,  198,  198,  195,
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
    4,    1,    1,    1,    2,    1,    1,    1,    1,    2,
    2,    4,    2,    2,    2,    4,    2,    5,    2,    1,
    3,    1,    3,    3,    4,    1,    2,    3,    3,    4,
    1,    1,    1,    1,    1,    2,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    3,    3,    4,
    1,    0,    2,    0,    1,    1,    2,    1,    1,    1,
    3,    1,    1,    1,    3,    3,    4,    3,    1,    3,
    1,    1,    1,    1,    2,    1,    1,    1,    1,    1,
    1,    1,    1,    1,    5,    7,    9,    0,    1,    1,
    1,    3,    4,    5,    4,    1,    4,    6,    6,    5,
    7,    4,    4,    4,    7,   10,    1,    1,    1,    1,
    4,    6,    6,    8,    6,    8,    8,   10,    4,    1,
    3,    1,    1,    3,    2,    1,    1,    4,    1,    3,
    1,    2,    2,    3,    1,    2,    0,    1,    1,    1,
    2,    2,    2,    2,    3,    0,    2,    6,    1,    1,
    1,    1,    1,    1,    1,    4,    1,    1,    1,    1,
    1,    1,    1,    1,    1,    1,    1,    1,    1,    1,
    4,    6,    1,    1,    1,    1,    4,    1,
  };
   static  short [] yyDefRed = {            0,
  296,    0,    0,    0,    0,    0,    0,    0,  128,  225,
  226,  297,    0,    0,  210,    0,    0,    0,  214,    0,
    0,   48,   64,  211,  212,   19,    0,    0,    0,  213,
    0,   63,   75,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  207,  336,    0,    0,  311,  312,  313,
  314,    0,    0,   56,  219,  220,  309,  260,   14,    8,
    9,  175,  284,   15,   17,   18,  176,   25,    0,   21,
   27,    0,  237,   28,   29,   30,  177,  283,   73,  262,
   60,    0,    0,  304,  208,    0,  278,   72,   74,  282,
  131,  132,  133,  134,    0,  173,  281,  209,  285,  324,
    0,  280,  277,  286,  287,    0,  316,  317,  318,  319,
  320,  321,  322,  323,  123,    0,  122,    2,    0,    0,
    0,  218,  217,  216,    0,    0,    0,   16,    0,    0,
  182,    0,    0,    0,  315,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   85,
   83,  147,    0,    0,    0,    0,    0,    0,    0,  137,
  138,    0,    0,   61,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   40,  125,    0,    0,    0,    0,  201,
   51,  206,    0,    0,    0,    0,  203,    0,    7,    0,
    0,  151,    0,    0,    0,  150,  156,    0,  152,  153,
  158,  154,  159,  196,  155,  157,    0,    0,  189,  192,
    0,    0,    0,    0,  223,    0,  180,    0,  183,  389,
  390,    0,    0,   12,    0,    0,    0,    0,  275,    0,
  273,  274,    0,    0,  266,    0,    0,    0,    0,    0,
    0,  298,    0,    0,    0,    0,    0,    0,    0,  369,
    0,    0,    0,    0,   84,    0,  302,  301,  310,  289,
  178,    0,    0,  288,    0,    0,   26,    0,    0,    0,
  263,  264,  261,  308,  305,  306,    0,  229,  228,    0,
    0,  233,    0,  127,    0,    0,  169,    0,   52,   55,
   50,    0,  205,    0,    0,  167,    0,   59,  276,    0,
    0,    0,    0,    0,   34,   38,   37,   33,   35,   36,
    0,    6,    0,  193,    0,    0,    0,  188,    0,  191,
    0,    0,  291,    0,  221,    0,  224,  172,  171,   57,
  170,  181,    0,    0,    0,   11,    0,    0,    0,    0,
  267,  265,    0,    0,    0,    0,    0,    0,    0,  329,
  330,    0,  342,    0,  351,  335,    0,  376,    0,    0,
  337,    0,    0,    0,    0,    0,  372,  333,    0,  343,
    0,  344,    0,  290,  238,    0,    0,   62,   70,    0,
  307,    0,  139,  140,  232,  230,    0,   43,    0,  129,
    3,    0,    0,    0,   68,  249,    0,  243,    0,    0,
  246,  247,  248,  299,  300,    0,    0,  204,  197,    0,
  111,    0,  145,    0,    0,  164,  166,  165,   39,    0,
    0,    0,    0,  184,    0,  190,    0,    0,  222,  398,
  397,  393,  415,  414,    0,  406,  405,  413,  408,  407,
  401,  399,  400,  409,  418,    0,  391,  392,  394,    0,
  402,  403,  404,    0,    0,   66,   13,    0,    0,    0,
    0,  268,  269,    0,    0,  340,  379,  380,    0,  378,
    0,  331,    0,  325,    0,  367,    0,    0,    0,    0,
  366,    0,    0,  360,  362,  363,  374,  370,    0,    0,
  381,  382,  383,    0,  384,  334,    0,  179,  239,  174,
   71,  234,    0,    0,  251,  250,  253,    0,  255,  254,
    0,    0,  144,  101,  142,    0,  143,    0,    0,   91,
   92,   93,    0,   94,  162,    0,    0,  245,  293,    0,
    0,  130,  114,  115,    0,  113,    0,    0,   87,   86,
  163,    0,    0,  199,  198,  200,  195,  227,    0,  388,
    0,    0,   65,   10,  149,    0,    0,  270,    0,  339,
    0,    0,    0,    0,  355,    0,  352,    0,  353,  365,
    0,    0,  359,  338,  387,  385,  348,  347,    0,   31,
    0,    0,    0,    0,   95,   69,    0,   45,   46,    0,
  104,  102,    0,    0,    0,  241,  109,    0,   89,    0,
    0,  107,  108,    0,    0,    0,    0,   24,   22,  341,
    0,  332,  326,    0,    0,    0,    0,  364,  361,    0,
  345,    0,   44,  252,  256,    0,    0,    0,   77,   76,
    0,   82,    4,    0,   88,  417,  396,    0,  411,    0,
  368,  356,    0,  354,  357,    0,   32,   47,    0,    0,
   98,  160,   99,  100,    0,    0,    0,   90,    0,  327,
    0,  350,  349,    0,   97,    0,    0,    0,    0,  258,
   78,  412,  358,  346,  119,  118,  117,    0,    0,  116,
  };
  protected static  short [] yyDgoto  = {             2,
  157,    7,  118,  291,  292,   57,  192,  193,  311,  194,
   59,   60,   61,   62,  225,   63,   64,  129,   65,   66,
   67,   68,   69,   70,   71,   72,  461,  557,  608,   73,
   74,   75,   76,  581,  313,  196,  175,  582,  284,  388,
  520,  395,  521,  522,  523,   77,  182,  183,   78,  656,
   79,  218,  330,  602,  197,   80,   81,   82,   83,  455,
   84,  295,  397,   85,   86,  124,   87,   88,   89,  629,
   90,  631,  531,  596,  198,  152,  153,  199,  540,  600,
  398,  399,  524,  679,  651,  652,  653,  654,  525,  526,
  592,  200,  422,  201,  412,  535,  536,    8,  119,  176,
  285,    9,  202,   91,   92,   93,   94,   95,  162,  385,
  280,  203,  204,  205,  206,  420,   10,   14,  125,  185,
  296,  331,   96,  262,  130,  131,  207,  208,  319,  209,
  321,  210,  211,  547,  186,  187,   97,   98,  214,  215,
   11,  324,  281,  282,  386,  407,  400,  528,  401,  402,
  403,   99,  100,  101,  234,  235,  236,  237,  102,  103,
  104,  105,    4,  405,  106,  107,  108,  109,  110,  111,
  112,  113,  114,  352,  473,  248,  249,  469,  579,  664,
  479,  483,  484,  485,  486,  250,  251,  367,  470,  491,
  222,  446,  447,  448,  449,  450,  451,  452,  453,  454,
  };
  protected static  short [] yySindex = {         -443,
    0,    0, -328, -433, -360, 3472, -276, -176,    0,    0,
    0,    0,  143, -169,    0, 4500,  155,  163,    0,  172,
 -319,    0,    0,    0,    0,    0,  179,  190,  204,    0,
  213,    0,    0,  232,  240,  271,  288,  292,  297,  300,
  311,  314,  325,    0,    0, -302,   56,    0,    0,    0,
    0,  346, 2014,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  156,    0,
    0, -272,    0,    0,    0,    0,    0,    0,    0,    0,
    0, 5524,  348,    0,    0,  367,    0,    0,    0,    0,
    0,    0,    0,    0,  147,    0,    0,    0,    0,    0,
  234,    0,    0,    0,    0,  -79,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   53,    0,    0,  135,  135,
 -120,    0,    0,    0, 3596, 2333, 2130,    0,  -17, -248,
    0, 4616, 5289, 5289,    0, 5289, 5289, 5289, 2686, -115,
 -105,  -86, 5289,  -78, 5289, 4734, 5289, 5289, 5289,    0,
    0,    0,  -74,  -69,  -62, 3150, -328,  434,  442,    0,
    0, 5419, 5289,    0, 5289, 3730, 5419, 5419, 3472,  -32,
 -435, 5289, -368,    0,    0,  216,  216,  467,  477,    0,
    0,    0,  253,    5,  202,  495,    0,  477,    0,  477,
 1800,    0,  186, 2810, -300,    0,    0,  109,    0,    0,
    0,    0,    0,    0,    0,    0,  157,  290,    0,    0,
  199,  434, 5289, -241,    0, 4861,    0,  241,    0,    0,
    0,  316, -382,    0,  173,  514, -305, -301,    0, 5289,
    0,    0, -272,  531,    0,  242, 5009,  -78, 5289, -366,
  535,    0,  221,  222,  539,  253,   62,  229,  544,    0,
  -80,  -12,  551,  238,    0,  550,    0,    0,    0,    0,
    0,  248,   50,    0, 3933,  147,    0,  -36,  558, 5289,
    0,    0,    0,    0,    0,    0,  508,    0,    0,  562,
 -181,    0,  332,    0, -328, -328,    0, -443,    0,    0,
    0,  -78,    0,  -19,  152,    0, 5143,    0,    0,  580,
  442,  245,  181, 3472,    0,    0,    0,    0,    0,    0,
 -209,    0, 2463,    0, 5289, 5289, 4861,    0, 2333,    0,
 2333,  245,    0,  192,    0,  318,    0,    0,    0,    0,
    0,    0,  493,  -69, 5289,    0, 5289, 5289, 5419, -272,
    0,    0, 5289, 5289, -272,  307,  250, -273, 4049,    0,
    0,  587,    0, 3266,    0,    0, -268,    0,  -80, 5289,
    0, 5289,  265,  265,  265,  270,    0,    0,  592,    0,
  122,    0, 3933,    0,    0,  616,  256,    0,    0,  618,
    0, -368,    0,    0,    0,    0,  621,    0, -176,    0,
    0,  -26,  -20,  -29,    0,    0,  623,    0,  310,  253,
    0,    0,    0,    0,    0, 2333,  324,    0,    0,  291,
    0, -283,    0, 3472,  628,    0,    0,    0,    0,  477,
 -272,  355,  359,    0,  290,    0, -254, 4861,    0,    0,
    0,    0,    0,    0,  636,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  637,    0,    0,    0,  639,
    0,    0,    0,  640,  641,    0,    0,  644,  -24,  156,
  349,    0,    0, 5289, 5289,    0,    0,    0,  647,    0,
  230,    0,  -34,    0,  649,    0,  266,  268,  281,  154,
    0,  253,  283,    0,    0,    0,    0,    0,  301,  320,
    0,    0,    0, -332,    0,    0, -416,    0,    0,    0,
    0,    0,  -78, 5289,    0,    0,    0, 5289,    0,    0,
  651,  -19,    0,    0,    0,  -87,    0,  -19,  -19,    0,
    0,    0,  337,    0,    0,  315,  167,    0,    0,  426,
  368,    0,    0,    0,  477,    0,  447, 2130,    0,    0,
    0, 5289, 5289,    0,    0,    0,    0,    0,  175,    0,
  176,  177,    0,    0,    0, 5419,  671,    0,  672,    0,
 5143, 5143,  673, 5289,    0, 4167,    0, 5289,    0,    0,
  -78, -268,    0,    0,    0,    0,    0,    0,  336,    0,
  674,  675,  678,  680,    0,    0,  358,    0,    0,  -19,
    0,    0,  682,  -69, 2333,    0,    0, 3472,    0,  338,
 -272,    0,    0,  683,  684,  361,  156,    0,    0,    0,
  -34,    0,    0,  381,  385,  401,  403,    0,    0,  193,
    0,  -78,    0,    0,    0,  -19, -314,  -78,    0,    0,
  679,    0,    0, 5289,    0,    0,    0,  194,    0,  686,
    0,    0, 5289,    0,    0, -331,    0,    0, 2333,  -15,
    0,    0,    0,    0,  674,  688,  -69,    0,  690,    0,
  422,    0,    0,  692,    0,  -69,  377,  379,  -78,    0,
    0,    0,    0,    0,    0,    0,    0,  674,  698,    0,
  };
  protected static  short [] yyRindex = {         -324,
    0,    0,    0, -306, 2944,    0,  741,   81,    0,    0,
    0,    0,    0, 4382,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0, 1045,  523,    0,    0,    0,
    0,    0, -324,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0, 1620,    0,
    0, 1709,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0, 1130,    0,    0,    0,    0,    0,
   51,    0,    0,    0,    0,  765,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0, -228, -228,
    0,    0,    0,    0,    0,    0, -324,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  269,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0, -297, -297,    0,    0,    0,
    0,    0,   18,  137,  209,   33,    0,    0,    0,    0,
 -324,    0, -199,    0,   17,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  252,  797,    0,    0,
 1938,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  -33,    0,    0,    0,    0,    0,    0,  702,
    0,    0,    0,    0,    0,  -37,  116,    0,    0,    0,
  429,    0,    0,    0,    0,  243,    0,    0,    0,    0,
    0,    0,    0,    0,    0, 1394,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  100,
   60,    0, -262,    0,    0,    0,    0, -324,    0,    0,
    0,    0,    0,    0,   52,    0,    0,    0,    0,    0,
  296,  366,   31,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0, -249,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  705,
    0,    0,    0,    0,  416,    0,    0,  707,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  439,    0,
    0,    0,  475,  475,  475,  475,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   84,    0,
    0,    0,    0, -324,    0,    0,   35,    0,  603,    9,
    0,    0,    0,    0,    0,    0,   94,    0,    0,    0,
    0,  709,    0,    0,    0,    0,    0,    0,    0,    0,
  850, 1333, 1360,    0, 1418,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  710,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  712,
    0,    0,    0,  714,    0,    0,    0,    0,    0,  -22,
  716,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  702,    0,    0,    0,    0,    0,    0,    0,
    0,  226,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  404,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  406,    0,    0,    0,    0,
  277,    0,    0,    0,    0,    0,    0, -324,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  727,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0, 1250,    0,    0,    0,    0,    0,    0,    0,
 1479,    0,    0,    0,    0,    0,  728,    0,    0,    0,
  702,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  302,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  730,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  732,    0,    0,
  };
  protected static  short [] yyGindex = {            0,
  774,  620, -164,  529, -344, -141,    0,   -5, -207, -130,
    0,    0,    0,  -16, -132,    0,    0,    0,    0,    0,
 -168,  615, -315,    0,    0, -111,    0,    0,    0,    0,
    0,    0,    0, -521,    0,    0,    0,    0,    0,    0,
    0, -350,    0,    0,  263,    0, -304,    0,  805,    0,
    0,  566, -230,  249,    0,  353,  699,    0,    0,    0,
 -123,    0,    0,    0,    0,  617,    0,    0,    0,  136,
 -167,    0,    0,    0, -175,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  478,    0,    0,    0,    0,  510,    0,  676,
  625,  512,    0,    0,    0,    0,    0,  638,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  -14,  538,    0,  687,    0,  485,    0,  484,
    0,  622,    0,    0,    0,  517,    0,    0,    0,  604,
    0,    0,    0,  437,    0,    0,    0,    0,    0,    0,
    0, -137,  430,    0,    0, -190,  584,    0,    0,    0,
 -251,    0,    0,  669,    0,    0,    0,    0,    0,    0,
    0,    0,    0, -426,  267, -326,  470,    0,    0,    0,
  264,    0,  257,    0,    0,  471,    0,  473,    0,  -49,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  };
  protected static  short [] yyTable = {           128,
   58,  184,  243,  375,  378,  279,  375,  272,  244,  562,
  394,  172,  244,  504,  195,  300,  555,   49,  235,  508,
  394,  223,  247,  460,  669,  227,  228,  233,  368,  339,
  257,  480,  202,  276,   67,  489,  158,  338,  159,  341,
  115,  150,  404,  511,  472,  575,  563,  329,  533,  244,
  259,  292,  244,  268,  124,  527,  414,  279,   49,  231,
  279,   49,  312,  315,  216,  217,    5,  334,  649,  544,
  110,  216,  325,  202,    5,   67,  279,  279,  279,  195,
  120,  383,  195,  121,  577,    5,  424,  369,  122,  294,
  375,  259,  292,   79,  410,    1,  347,  275,  376,  141,
  231,  155,   52,  231,  349,   12,  655,  295,  181,  534,
  212,  116,   13,  154,  427,  221,  126,  224,  340,  226,
  578,  120,  384,    6,  121,  345,  241,  294,  224,  246,
  252,  253,  254,  415,   79,  650,  304,  571,  163,  123,
  141,  261,  404,    5,  350,  295,  351,  678,  329,  269,
  391,   42,  462,  463,  126,  277,  304,  304,  304,  304,
  304,  586,  304,  273,  256,  278,  117,  588,  589,  662,
  257,  312,  576,  184,  158,  120,  301,  304,  304,  304,
  304,  304,  121,  304,  640,  124,  316,  545,  167,   42,
  546,  312,    5,  168,  132,    5,  323,  548,  160,  328,
  161,  663,  133,  421,  421,  126,  304,  195,  240,  195,
  456,  134,  213,  336,  279,  163,  335,  135,  136,  163,
  163,  477,  348,  124,  505,  509,  459,  304,  151,  137,
  529,  340,  340,  467,  481,  468,  247,  614,  247,  627,
  607,  532,  298,  138,  513,  309,  308,  310,  163,  240,
  261,  185,  139,  380,  506,  510,  472,  612,  514,  329,
  404,  355,  356,  666,  354,  335,  404,  404,  482,  361,
  257,  140,  360,  558,  515,  648,   81,  169,  372,  141,
  181,  371,  279,  298,  298,  298,  298,  298,  374,  298,
  466,  373,  185,  465,  195,  185,  500,  271,  413,  373,
  328,   80,  298,  298,  298,  235,  565,  419,  567,  335,
  142,  566,  175,  667,  492,  493,  495,   81,  457,  517,
  458,  569,  173,  573,  568,  244,  572,  143,  175,  175,
  175,  144,  181,  298,   49,  298,  145,  224,  404,  146,
  244,  574,   80,  246,  360,  246,  244,  244,   49,  202,
  147,   67,  340,  148,  244,  173,  173,  173,  498,  279,
  244,  580,  110,  202,  149,  244,  115,  259,  292,   49,
  244,  279,   67,   67,  404,  116,  621,  304,  635,  620,
  668,  634,  244,  279,  202,  156,   67,  165,  257,  292,
  292,  244,  174,  279,  257,  257,  244,  120,  304,  392,
  121,  639,  259,  292,  638,   49,  166,  279,  537,  392,
   79,  328,  363,  178,  364,  244,  365,  238,  366,  632,
  202,  641,   67,  110,  360,  642,  630,  239,  335,  618,
  601,  601,   79,  615,   36,  617,  213,  116,  259,  292,
  247,  644,  393,  645,  643,   79,  335,  170,  559,  171,
  240,  244,  393,  304,  242,  375,  255,  375,  244,  375,
   49,  375,  673,  256,  244,  335,  257,  304,  120,  371,
  259,  121,  371,  665,  264,  202,  350,   67,  351,  373,
  647,   79,  373,  195,  163,  265,  580,  583,  304,  630,
   67,  584,  117,  259,  292,   54,  163,  244,  283,  244,
  274,  298,  257,   47,  298,  298,  256,  287,  298,    1,
  661,   52,  256,   47,  290,  386,  288,   52,  386,  271,
  272,  599,  298,  120,  304,  240,  121,  580,   49,  293,
   49,  279,  294,  279,  175,  279,   79,  195,  297,  317,
  302,   54,  675,  318,  181,  181,  298,  246,  320,  224,
   54,  224,  303,  322,  332,  298,  298,  337,  298,  298,
  240,  173,  189,  298,  298,  298,  298,  298,  185,  298,
  298,  342,  343,  298,  298,  353,  304,  333,  357,  304,
  298,  298,  298,  298,  298,  298,  359,  362,  298,  185,
  185,  370,  633,   81,  298,  155,  240,  298,  379,  298,
  381,  387,  242,  185,  298,  382,  298,  406,  304,  298,
  304,  175,  304,  298,  304,  298,  298,  658,   80,  298,
  409,  189,  428,  175,  411,  298,  224,  474,   81,  298,
  298,  429,  496,  298,  185,  175,  304,  464,  173,  185,
   80,  513,  304,  242,  304,  175,  242,  490,  304,  298,
  173,  240,  494,   80,  497,  514,  499,  304,  501,  175,
  503,  530,  173,  304,   81,  304,  512,  538,   26,  304,
  542,  515,  173,  298,  543,  549,  556,  550,  551,  552,
  561,  553,  185,  516,  554,  298,  173,  560,  564,   80,
  570,  585,  298,  590,  185,  594,  298,  575,  298,  593,
  305,  185,  306,  591,  307,  598,  595,  185,  604,  605,
  606,  609,  610,  613,  626,  623,  517,  622,  624,   81,
  625,  628,  657,  636,  637,  646,  660,  659,  670,  298,
  672,  298,  674,  298,  676,  298,  677,  298,  680,  298,
    1,  298,  328,    5,   80,  272,  271,  377,  112,  298,
  416,  298,  395,  298,  410,  298,   23,  298,   54,  298,
  161,  298,  103,  298,  303,  430,  431,   41,  236,  298,
   53,  298,   96,    3,  358,  298,  263,  267,  587,  326,
  164,  298,  270,  175,  298,  175,  432,  175,  298,  433,
  434,  603,  671,  423,  389,  177,  186,  390,  518,  266,
  519,  286,  377,  425,  426,  303,  303,  303,  303,  303,
  173,  303,  173,  408,  173,  314,  219,  327,  502,  435,
  346,  507,  258,  478,  303,  303,  303,  611,  619,  616,
  488,  487,    0,    0,    0,  298,  298,  186,  298,  298,
  186,    0,  436,  437,    0,    0,    0,    0,    0,  148,
  298,    0,    0,  298,  298,    0,    0,  303,    0,    0,
  298,  298,    0,    0,    0,  298,    0,    0,  298,    0,
    0,    0,  438,    0,  298,    0,    0,  298,    0,  298,
    0,    0,    0,    0,  298,    0,  298,    0,    0,  298,
  148,    0,    0,  148,  439,    0,  298,    0,    0,  298,
    0,    0,    0,    0,    0,  298,    0,    0,    0,  298,
  298,  440,    0,  298,    0,    0,    0,    0,    0,  242,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  298,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  242,  242,    0,    0,    0,  441,    0,    0,    0,    0,
    0,    0,    0,  298,  242,    0,    0,    0,    0,  161,
    0,    0,    0,    0,    0,  298,    0,  442,    0,  443,
    0,  444,  298,    0,    0,    0,  298,    0,  298,    0,
    0,    0,    0,  289,    0,  242,    0,    0,    0,    0,
  242,    0,  298,    0,  299,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  298,
    0,  298,    0,  298,    0,  298,    0,  298,    0,  298,
    0,  298,    0,  303,    0,  445,  303,  303,    0,  298,
  303,  298,    0,  298,    0,  298,    0,  298,    0,  298,
    0,  298,    0,  298,  146,  242,    0,  298,    0,  298,
    0,  298,  242,    0,    0,  298,    0,    0,  242,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  303,    0,
    0,    0,    0,    0,    0,    0,    0,  303,  303,    0,
  303,  303,    0,    0,    0,  146,  146,  146,  146,  146,
    0,  146,  303,    0,    0,  303,  303,    0,  396,    0,
    0,    0,  303,  303,  146,  146,  146,  303,  148,    0,
  303,    0,    0,  186,    0,    0,  303,    0,    0,  303,
    0,  303,    0,    0,    0,    0,  303,    0,  303,  135,
    0,  303,    0,    0,  186,  186,    0,  146,  303,    0,
    0,  303,    0,    0,    0,    0,    0,  303,  186,    0,
    0,  303,  303,    0,    0,  303,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  148,  148,    0,    0,    0,
  135,  303,  135,  135,  135,    0,    0,    0,    0,  186,
    0,    0,    0,  186,  186,    0,    0,  148,  148,  135,
  135,  135,    0,    0,    0,  303,    0,    0,  396,    0,
    0,  148,    0,    0,  148,    0,    0,  303,    0,    0,
    0,    0,    0,    0,  303,    0,    0,    0,  303,  539,
  303,    0,  135,    0,  541,    0,    0,  186,    0,    0,
    0,    0,  148,    0,    0,    0,  148,  148,    0,  186,
    0,    0,    0,    0,    0,    0,  186,    0,    0,  257,
    0,  303,  186,  303,    0,  303,    0,  303,    0,  303,
    0,  303,    0,  303,    0,    0,    0,    0,    0,    0,
    0,  303,    0,  303,    0,  303,    0,  303,    0,  303,
  148,  303,    0,  303,    0,  303,    0,    0,    0,    0,
  257,    0,  148,  257,    0,    0,    0,  303,    0,  148,
    0,    0,    0,  146,    0,  148,  146,    0,    0,    0,
  146,    0,    0,    0,    0,    0,  396,    0,    0,    0,
    0,    0,  396,  396,    0,    0,    0,    0,    0,    0,
    0,    0,  105,    0,    0,    0,    0,    0,    0,  597,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  146,  146,  106,
  146,  146,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  146,  105,    0,  146,  105,    0,    0,    0,
    0,    0,  146,  146,    0,    0,    0,  146,  135,    0,
    0,  135,    0,  136,  396,  135,  146,    0,    0,  146,
  106,    0,    0,  106,    0,    0,    0,    0,  146,    0,
    0,  146,    0,    0,    0,    0,    0,  187,    0,    0,
    0,  146,    0,    0,    0,    0,    0,  146,    0,    0,
  396,  146,  146,    0,  136,  146,  136,  136,  136,    0,
    0,    0,  135,  135,    0,  135,  135,    0,    0,    0,
    0,    0,    0,  136,  136,  136,    0,  135,  187,    0,
  135,  187,    0,    0,    0,    0,    0,  135,  135,    0,
    0,    0,  135,    0,    0,  146,    0,    0,   58,    0,
    0,  135,    0,    0,  135,    0,  136,  146,    0,    0,
    0,    0,    0,  135,  146,    0,  135,    0,  146,    0,
  146,    0,    0,    0,    0,    0,  135,    0,    0,    0,
    0,    0,  135,    0,    0,    0,  135,  135,    0,   58,
  135,    0,   58,    0,    0,    0,    0,    0,    0,    0,
    0,  146,    0,    0,    0,    0,    0,  146,    0,  146,
    0,  146,    0,  146,    0,    0,    0,    0,    0,    0,
    0,  146,    0,  146,    0,  146,    0,  146,    0,  146,
  135,  146,    0,  146,    0,  146,  257,    0,    0,    0,
    0,    0,  135,    0,    0,    0,    0,  146,    0,  135,
    0,  257,    0,  135,    0,  135,    0,  257,  257,    0,
    0,  105,    0,    0,    0,  257,    0,    0,    0,    0,
    0,  257,    0,    0,    0,    0,  257,    0,    0,    0,
    0,  257,    0,    0,    0,    0,  135,    0,  106,   20,
    0,    0,  135,  257,  135,    0,  135,    0,  135,    0,
    0,    0,  257,    0,    0,    0,  135,  257,  135,    0,
  135,    0,  135,    0,  135,    0,  135,    0,  135,  105,
  135,    0,  136,    0,    0,  136,  257,    0,    0,  136,
   20,    0,  135,   20,    0,    0,    0,    0,    0,    0,
  105,  105,    0,    0,    0,    0,  106,    0,    0,   20,
   20,   20,    0,    0,  105,    0,    0,  105,    0,    0,
    0,    0,  257,    0,    0,    0,    0,  106,  106,  257,
    0,    0,    0,    0,    0,  257,  136,  136,  279,  136,
  136,  106,   20,    0,  106,  105,    0,    0,    0,  105,
  105,  136,    0,    0,  136,    0,    0,    0,    0,    0,
    0,  136,  136,    0,  187,    0,  136,   58,  257,    0,
  257,    0,  106,    0,    0,  136,  106,  106,  136,  279,
    0,    0,  279,    0,    0,  187,  187,  136,    0,    0,
  136,    0,    0,  105,    0,    0,    0,    0,    0,  187,
  136,    0,    0,    0,    0,  105,  136,    0,    0,    0,
  136,  136,  105,    0,  136,    0,    0,    0,  105,    0,
  106,    0,    0,    0,    0,   58,    0,    0,    0,    0,
  187,  279,  106,    0,  187,  187,    0,    0,    0,  106,
    0,    0,    0,    0,    0,  106,   58,   58,    0,    0,
    0,    0,    0,    0,  136,    0,    0,    0,    0,    0,
   58,    0,    0,   58,    0,    0,  136,    0,    0,  191,
    0,    0,   55,  136,   56,    0,    0,  136,  187,  136,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  187,   58,   54,    0,    0,   58,   58,  187,    0,    0,
    0,    0,    0,  187,    0,    0,    0,    0,   20,    0,
  136,   20,    0,    0,    0,   20,  136,    0,  136,    0,
  136,    0,  136,    0,    0,    0,    0,    0,    0,    0,
  136,    0,  136,    0,  136,    0,  136,    0,  136,   58,
  136,    0,  136,    0,  136,    0,    0,    0,    0,    0,
    0,   58,    0,    0,    0,    0,  136,    0,   58,    0,
    0,    0,   20,   20,   58,   20,   20,  194,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   20,    0,    0,    0,    0,    0,    0,   20,   20,    0,
    0,    0,   20,    0,    0,    0,    0,  279,    0,    0,
  279,   20,    0,    0,   20,    0,    0,    0,  194,    0,
    0,  194,    0,   20,    0,    0,   20,    0,    0,    0,
    0,    0,    0,    0,    0,    0,   20,    0,    0,    0,
    0,    0,   20,    0,    0,    0,   20,   20,    0,    0,
   20,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  279,  279,    0,    0,  279,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  279,
    0,    0,    0,    0,    0,    0,  279,  279,    0,    0,
   20,    0,    0,  127,    0,    0,   55,    0,   56,    0,
  279,    0,   20,  279,   15,    0,    0,    0,    0,   20,
   16,   17,    0,   20,    0,   20,   54,    0,    0,    0,
    0,   18,    0,   19,    0,    0,   20,    0,    0,    0,
    0,  279,    0,   21,    0,  279,  279,    0,    0,   22,
    0,    0,    0,    0,    0,    0,   20,    0,    0,    0,
    0,    0,   20,    0,   20,    0,   20,    0,   20,    0,
  188,    0,    0,    0,    0,    0,   20,    0,   20,    0,
   20,    0,   20,    0,   20,    0,   20,    0,   20,  279,
   20,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,  279,   20,    0,    0,    0,    0,    0,  279,    0,
    0,    0,  279,    0,  279,   23,    0,   24,   25,  127,
    0,    0,   55,    0,   56,    0,  189,   26,   27,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   54,   28,    0,    0,  194,    0,    0,    0,
    0,  279,    0,  279,    0,  279,    0,  279,    0,    0,
    0,    0,    0,    0,    0,  279,    0,  279,    0,  279,
    0,  279,    0,    0,    0,   29,   30,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   31,  279,    0,  190,    0,    0,   32,    0,    0,    0,
   33,    0,    0,    0,  194,    0,    0,    0,   34,   35,
   36,   37,   38,    0,   39,   40,   41,    0,   42,   43,
    0,    0,    0,    0,    0,  194,  194,    0,   15,    0,
   44,    0,   45,    0,   16,   17,    0,    0,    0,  194,
    0,    0,    0,    0,    0,   18,    0,   19,    0,    0,
   20,    0,    0,    0,    0,    0,    0,   21,    0,    0,
    0,    0,    0,   22,    0,    0,    0,    0,    0,    0,
  194,    0,    0,    0,  194,  194,    0,    0,    0,    0,
   46,    0,   47,   48,   49,   50,   51,   52,    1,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  194,    0,
    0,    0,  191,    0,    0,   55,    0,   56,    0,   23,
  194,   24,   25,    0,    0,    0,    0,  194,    0,    0,
    0,   26,   27,  194,   15,   54,    0,    0,    0,    0,
   16,   17,    0,    0,    0,    0,    0,   28,    0,    0,
    0,   18,    0,   19,    0,    0,   20,    0,    0,    0,
    0,    0,    0,   21,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   29,
   30,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   31,    0,    0,    0,    0,    0,
   32,    0,    0,    0,   33,    0,    0,    0,    0,    0,
    0,    0,   34,   35,   36,   37,   38,    0,   39,   40,
   41,    0,   42,   43,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   44,   23,   45,   24,   25,    0,
    0,    0,   53,    0,    0,   55,    0,   56,   27,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   28,    0,   54,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   46,    0,   47,   48,   49,   50,
   51,   52,    1,    0,    0,   29,   30,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   31,    0,    0,    0,    0,    0,   32,    0,    0,    0,
   33,    0,    0,    0,    0,    0,    0,    0,   34,   35,
   36,   37,   38,    0,   39,   40,   41,   15,   42,   43,
    0,    0,    0,   16,   17,    0,    0,    0,    0,    0,
   44,    0,   45,    0,   18,    0,   19,    0,    0,   20,
    0,    0,    0,    0,    0,    0,   21,    0,    0,    0,
    0,    0,   22,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,  188,    0,    0,    0,    0,    0,    0,
   46,    0,   47,   48,   49,   50,   51,   52,    1,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   23,    0,
   24,   25,    0,    0,    0,    0,    0,    0,    0,  189,
   26,   27,    0,    0,    0,    0,    0,    0,    0,    0,
  416,    0,  417,    0,    0,  127,   28,   15,   55,    0,
   56,    0,    0,   16,   17,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   18,    0,   19,    0,   54,   20,
    0,    0,    0,    0,    0,    0,   21,    0,   29,   30,
    0,    0,   22,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   31,    0,    0,  190,    0,    0,   32,
    0,    0,    0,   33,    0,    0,    0,    0,    0,    0,
    0,   34,   35,   36,   37,   38,    0,   39,   40,   41,
    0,   42,   43,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   44,    0,   45,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   23,    0,
   24,   25,    0,    0,    0,    0,    0,    0,    0,    0,
   26,   27,    0,    0,    0,    0,    0,    0,    0,  191,
    0,    0,   55,    0,   56,    0,   28,    0,    0,    0,
    0,    0,    0,   46,    0,   47,   48,   49,   50,   51,
   52,    0,   54,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  418,    0,    0,    0,    0,    0,   29,   30,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   31,    0,    0,    0,    0,    0,   32,
    0,    0,    0,   33,    0,    0,    0,    0,    0,    0,
    0,   34,   35,   36,   37,   38,    0,   39,   40,   41,
    0,   42,   43,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   44,    0,   45,    0,    0,    0,    0,
   15,    0,    0,    0,  229,    0,   16,   17,    0,    0,
    0,    0,    0,    0,    0,    0,    0,   18,    0,   19,
    0,    0,   20,    0,    0,    0,    0,    0,    0,   21,
    0,    0,    0,  168,    0,  168,  168,    0,  168,    0,
    0,    0,    0,   46,    0,   47,   48,   49,   50,   51,
   52,    0,    0,    0,    0,    0,  168,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  230,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  231,    0,    0,    0,
    0,   23,    0,   24,   25,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   27,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   15,    0,    0,    0,    0,   28,
   16,   17,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   18,    0,   19,    0,    0,   20,    0,    0,    0,
    0,    0,    0,   21,    0,    0,    0,    0,    0,   22,
    0,   29,   30,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  232,    0,    0,    0,   31,    0,    0,    0,
  188,    0,   32,    0,    0,    0,   33,    0,    0,    0,
    0,    0,    0,    0,   34,   35,   36,   37,   38,    0,
   39,   40,   41,    0,   42,   43,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,   44,    0,   45,    0,
    0,    0,    0,    0,    0,   23,    0,   24,   25,    0,
    0,    0,    0,    0,    0,    0,    0,   26,   27,  127,
  260,    0,   55,    0,   56,    0,    0,    0,    0,    0,
    0,  168,    0,   28,    0,    0,    0,    0,  168,    0,
    0,    0,   54,    0,  168,  168,   46,    0,   47,   48,
   49,   50,   51,   52,    0,  168,    0,  168,    0,    0,
  168,    0,    0,    0,    0,   29,   30,  168,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   31,    0,  168,  190,    0,    0,   32,    0,    0,    0,
   33,    0,    0,    0,    0,    0,    0,    0,   34,   35,
   36,   37,   38,    0,   39,   40,   41,    0,   42,   43,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   44,    0,   45,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,  127,    0,    0,   55,  168,
   56,  168,  168,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  168,    0,    0,    0,    0,    0,   54,    0,
    0,    0,    0,    0,    0,    0,    0,  168,    0,    0,
   46,    0,   47,   48,   49,   50,   51,   52,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,  168,
  168,    0,  168,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  168,    0,    0,    0,    0,    0,
  168,    0,    0,    0,  168,    0,    0,    0,    0,    0,
    0,    0,  168,  168,  168,  168,  168,    0,  168,  168,
  168,    0,  168,  168,   15,    0,    0,    0,    0,    0,
   16,   17,    0,    0,  168,    0,  168,    0,    0,    0,
    0,   18,    0,   19,    0,    0,   20,    0,    0,    0,
    0,    0,    0,   21,    0,    0,    0,    0,    0,   22,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,  168,    0,  168,  168,  168,  168,
  168,  168,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   53,    0,    0,   55,   23,   56,   24,   25,    0,
    0,    0,    0,    0,    0,    0,    0,   26,   27,    0,
   15,    0,    0,    0,   54,    0,   16,   17,    0,    0,
    0,    0,    0,   28,    0,    0,    0,   18,    0,   19,
    0,    0,   20,    0,    0,    0,    0,    0,    0,   21,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   29,   30,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   31,    0,    0,    0,    0,    0,   32,    0,    0,    0,
   33,    0,    0,    0,    0,    0,    0,    0,   34,   35,
   36,   37,   38,    0,   39,   40,   41,    0,   42,   43,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   44,   23,   45,   24,   25,  127,    0,  180,   55,    0,
   56,    0,    0,    0,   27,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   54,   28,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   46,    0,   47,   48,   49,   50,   51,   52,    0,    0,
    0,   29,   30,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,   31,    0,    0,    0,
    0,    0,   32,    0,    0,    0,   33,    0,    0,    0,
    0,    0,    0,    0,   34,   35,   36,   37,   38,  475,
   39,   40,   41,  245,   42,   43,   15,    0,    0,    0,
    0,    0,   16,   17,    0,    0,   44,    0,   45,    0,
  476,    0,    0,   18,    0,   19,    0,    0,   20,    0,
    0,    0,    0,    0,    0,   21,    0,    0,    0,  127,
    0,   22,   55,    0,   56,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   54,    0,    0,    0,   46,    0,   47,   48,
   49,   50,   51,   52,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,   23,    0,   24,
   25,    0,    0,    0,    0,    0,    0,    0,    0,   26,
   27,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   15,    0,    0,    0,    0,   28,   16,   17,    0,    0,
    0,    0,    0,    0,    0,    0,    0,   18,    0,   19,
    0,    0,   20,    0,    0,    0,    0,    0,    0,   21,
    0,    0,    0,    0,    0,    0,    0,   29,   30,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   31,    0,    0,    0,    0,    0,   32,    0,
    0,    0,   33,    0,    0,    0,    0,    0,    0,    0,
   34,   35,   36,   37,   38,    0,   39,   40,   41,    0,
   42,   43,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   44,    0,   45,    0,    0,    0,    0,    0,
    0,   23,    0,   24,   25,    0,    0,    0,    0,    0,
    0,    0,  127,    0,   27,   55,    0,   56,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  122,    0,   28,
    0,    0,    0,    0,   15,   54,    0,    0,    0,    0,
   16,   17,   46,    0,   47,   48,   49,   50,   51,   52,
    0,   18,    0,   19,    0,    0,   20,    0,    0,    0,
    0,   29,   30,   21,  179,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,   31,    0,  123,    0,
    0,    0,   32,    0,    0,    0,   33,    0,    0,    0,
    0,    0,    0,    0,   34,   35,   36,   37,   38,    0,
   39,   40,   41,    0,   42,   43,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,   44,    0,   45,    0,
    0,    0,    0,    0,    0,    0,    0,    0,  127,    0,
    0,   55,    0,   56,    0,   23,    0,   24,   25,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   27,    0,
    0,   54,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   28,    0,    0,   46,    0,   47,   48,
   49,   50,   51,   52,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   29,   30,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   31,    0,    0,    0,    0,    0,   32,    0,    0,    0,
   33,    0,    0,    0,    0,    0,    0,    0,   34,   35,
   36,   37,   38,    0,   39,   40,   41,   15,   42,   43,
    0,    0,    0,   16,   17,    0,  127,    0,    0,   55,
   44,   56,   45,    0,   18,    0,   19,    0,    0,   20,
    0,    0,    0,    0,    0,    0,   21,    0,    0,   54,
    0,    0,   22,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   46,    0,   47,   48,   49,   50,   51,   52,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   23,    0,
   24,   25,    0,    0,    0,    0,    0,    0,    0,    0,
   26,   27,    0,   15,    0,    0,    0,    0,  471,   16,
   17,    0,    0,    0,    0,    0,   28,    0,    0,    0,
   18,    0,   19,    0,    0,   20,    0,    0,    0,    0,
    0,    0,   21,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   29,   30,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   31,    0,    0,    0,    0,    0,   32,
    0,    0,    0,   33,    0,    0,    0,    0,    0,    0,
    0,   34,   35,   36,   37,   38,    0,   39,   40,   41,
    0,   42,   43,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   44,   23,   45,   24,   25,    0,    0,
    0,  215,    0,  215,  215,    0,  215,   27,    0,    0,
    0,   15,    0,    0,    0,    0,    0,   16,   17,    0,
    0,    0,   28,    0,  215,    0,    0,    0,   18,    0,
   19,    0,    0,   20,    0,    0,    0,    0,    0,    0,
   21,    0,    0,   46,    0,   47,   48,   49,   50,   51,
   52,    0,    0,    0,   29,   30,    0,  179,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   31,
    0,    0,    0,    0,    0,   32,    0,    0,    0,   33,
    0,    0,    0,    0,    0,    0,    0,   34,   35,   36,
   37,   38,    0,   39,   40,   41,    0,   42,   43,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   44,
    0,   45,   23,    0,   24,   25,    0,    0,    0,  127,
    0,    0,   55,    0,   56,   27,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   28,    0,   54,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   46,
    0,   47,   48,   49,   50,   51,   52,    0,    0,    0,
    0,    0,   29,   30,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,   31,    0,    0,
    0,    0,    0,   32,    0,    0,    0,   33,    0,    0,
    0,    0,    0,    0,    0,   34,   35,   36,   37,   38,
  475,   39,   40,   41,    0,   42,   43,    0,    0,    0,
    0,    0,    0,    0,    0,    0,  215,   44,    0,   45,
    0,  476,  215,  215,    0,  127,    0,    0,   55,    0,
   56,    0,    0,  215,    0,  215,    0,    0,  215,    0,
    0,    0,    0,    0,    0,  215,    0,    0,   54,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,   46,    0,   47,
   48,   49,   50,   51,   52,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  215,    0,  215,
  215,    0,    0,    0,    0,    0,    0,    0,    0,    0,
  215,    0,    0,    0,   15,    0,    0,    0,    0,    0,
   16,   17,    0,  127,    0,  215,   55,    0,   56,    0,
    0,   18,    0,   19,    0,    0,   20,    0,    0,    0,
    0,    0,    0,   21,    0,    0,   54,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,  215,  215,    0,
  215,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  215,    0,    0,    0,    0,    0,  215,    0,
    0,    0,  215,    0,    0,    0,    0,    0,    0,    0,
  215,  215,  215,  215,  215,    0,  215,  215,  215,    0,
  215,  215,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,  215,    0,  215,   23,    0,   24,   25,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   27,    0,
   15,    0,    0,    0,    0,    0,   16,   17,    0,    0,
    0,    0,    0,   28,    0,    0,    0,   18,    0,   19,
  127,    0,   20,   55,    0,   56,    0,    0,    0,   21,
    0,    0,  215,    0,  215,  215,  215,  215,  215,  215,
    0,    0,    0,   54,    0,   29,   30,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   31,    0,    0,    0,    0,    0,   32,    0,    0,    0,
   33,    0,    0,  126,    0,    0,    0,    0,   34,   35,
   36,   37,   38,    0,   39,   40,   41,    0,   42,   43,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
   44,   23,   45,   24,   25,    0,    0,    0,    0,    0,
    0,    0,    0,  220,   27,    0,    0,    0,   15,    0,
    0,    0,    0,    0,   16,   17,    0,    0,    0,   28,
    0,    0,    0,    0,    0,   18,    0,   19,    0,    0,
   20,    0,    0,    0,    0,    0,    0,   21,    0,    0,
   46,    0,   47,   48,   49,   50,   51,   52,    0,    0,
    0,   29,   30,    0,    0,    0,    0,    0,  127,    0,
    0,   55,    0,   56,    0,    0,   31,    0,    0,    0,
    0,    0,   32,    0,    0,    0,   33,    0,    0,    0,
    0,   54,    0,    0,   34,   35,   36,   37,   38,    0,
   39,   40,   41,    0,   42,   43,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,   44,    0,   45,   23,
    0,   24,   25,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   27,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   15,    0,   28,    0,    0,
    0,   16,   17,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   18,    0,   19,    0,   46,   20,   47,   48,
   49,   50,   51,   52,   21,    0,    0,    0,    0,   29,
   30,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   31,    0,    0,    0,    0,    0,
   32,    0,  127,    0,   33,   55,    0,   56,    0,    0,
    0,    0,   34,   35,   36,   37,   38,    0,   39,   40,
   41,  245,   42,   43,    0,   54,    0,    0,    0,    0,
    0,    0,    0,    0,   44,    0,   45,    0,    0,    0,
    0,    0,    0,    0,    0,    0,   23,    0,   24,   25,
    0,    0,    0,    0,    0,    0,    0,    0,   26,   27,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   28,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   46,    0,   47,   48,   49,   50,
   51,   52,    0,   15,    0,    0,    0,    0,    0,   16,
   17,    0,    0,    0,    0,    0,   29,   30,    0,    0,
   18,    0,   19,    0,    0,   20,    0,    0,    0,    0,
    0,   31,   21,    0,    0,    0,    0,   32,    0,    0,
    0,   33,    0,    0,    0,    0,    0,    0,    0,   34,
   35,   36,   37,   38,    0,   39,   40,   41,  127,   42,
   43,   55,    0,   56,    0,    0,    0,    0,    0,  344,
    0,   44,    0,   45,    0,    0,    0,    0,    0,    0,
    0,   54,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   23,    0,   24,   25,    0,    0,
    0,    0,    0,    0,    0,    0,    0,   27,    0,    0,
    0,   46,    0,   47,   48,   49,   50,   51,   52,    0,
    0,    0,   28,    0,    0,    0,    0,   15,    0,    0,
    0,    0,    0,   16,   17,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   18,    0,   19,    0,    0,   20,
    0,    0,    0,    0,   29,   30,   21,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   31,
    0,    0,    0,    0,    0,   32,    0,    0,  127,   33,
    0,   55,    0,   56,    0,    0,    0,   34,   35,   36,
   37,   38,    0,   39,   40,   41,    0,   42,   43,    0,
    0,   54,    0,    0,    0,    0,    0,    0,    0,   44,
    0,   45,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,   23,    0,
   24,   25,    0,    0,    0,    0,    0,    0,    0,    0,
    0,   27,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,   28,    0,    0,   46,
    0,   47,   48,   49,   50,   51,   52,    0,    0,    0,
    0,    0,    0,   15,    0,    0,    0,    0,    0,   16,
   17,    0,    0,  127,    0,    0,    0,    0,   29,   30,
   18,  179,   19,    0,    0,   20,    0,    0,    0,    0,
    0,    0,   21,   31,    0,    0,   54,    0,    0,   32,
    0,    0,    0,   33,    0,    0,    0,    0,    0,    0,
    0,   34,   35,   36,   37,   38,    0,   39,   40,   41,
    0,   42,   43,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,   44,    0,   45,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   23,    0,   24,   25,    0,    0,
    0,    0,    0,    0,    0,    0,    0,   27,    0,    0,
    0,    0,    0,   46,    0,   47,   48,   49,   50,   51,
   52,    0,   28,   15,    0,    0,    0,    0,    0,   16,
   17,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,   19,    0,    0,   20,    0,    0,    0,    0,
    0,    0,   21,    0,   29,   30,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   31,
    0,    0,    0,    0,    0,   32,    0,    0,    0,   33,
    0,    0,    0,    0,    0,    0,    0,   34,   35,   36,
   37,   38,    0,   39,   40,   41,    0,   42,   43,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   44,
    0,   45,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,   24,   25,   15,    0,
    0,    0,    0,    0,   16,   17,    0,   27,    0,    0,
    0,    0,    0,    0,    0,    0,    0,   19,    0,    0,
   20,    0,   28,    0,    0,    0,    0,   21,    0,   46,
    0,   47,   48,   49,   50,   51,   52,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,   30,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   33,
    0,    0,    0,    0,    0,    0,    0,   34,   35,   36,
   37,   38,    0,   39,   40,   41,    0,   42,   43,    0,
    0,   24,   25,    0,    0,    0,    0,    0,    0,   44,
    0,   45,   27,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,   28,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,   46,
   30,   47,   48,   49,   50,   51,   52,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   33,    0,    0,    0,    0,    0,
    0,    0,   34,   35,   36,   37,   38,    0,   39,   40,
   41,    0,   42,   43,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   44,    0,   45,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,    0,    0,    0,    0,    0,    0,
    0,    0,    0,    0,   46,    0,   47,   48,   49,   50,
   51,   52,
  };
  protected static  short [] yyCheck = {            16,
    6,  125,  144,   41,   41,  173,   44,   41,    0,   44,
   40,   91,  145,   40,  126,  191,   41,    0,   41,   40,
   40,  133,  146,  339,   40,  137,  138,  139,   41,  331,
  154,  300,    0,  171,    0,  362,   53,  343,   53,  230,
  317,  344,  294,  394,  349,  378,  473,  216,  332,   41,
    0,    0,   44,  165,  283,  400,  266,   41,   41,    0,
   44,   44,  193,  364,  313,  314,  266,  450,  383,  324,
   40,  313,  314,   41,  324,   41,   60,   61,   62,  191,
    0,  263,  194,    0,  501,  414,  317,  252,  258,  414,
   41,   41,   41,    0,  302,  539,  238,  533,  263,    0,
   41,   46,  538,   44,  471,  539,  628,  414,  125,  393,
  127,  388,  473,   58,  322,  132,  414,  134,  230,  136,
  537,   41,  304,  452,   41,  237,  143,  452,  145,  146,
  147,  148,  149,  343,   41,  450,    0,  482,  521,  309,
   41,  156,  394,  343,  511,  452,  513,  669,  317,  166,
  292,  414,  343,  344,  452,  172,   41,   42,   43,   44,
   45,  512,   47,  169,  533,  534,  443,  518,  519,  501,
  294,  302,  505,  297,  191,  352,  191,   41,   42,   43,
   44,   45,   40,   47,  611,  414,  487,  442,   42,  452,
  445,  322,  442,   47,   40,  445,  213,  428,   43,  216,
   45,  533,   40,  315,  316,  454,   91,  319,    0,  321,
  334,   40,  454,   41,  382,  521,   44,  537,   40,  521,
  521,  354,  239,  452,  392,  393,  338,   91,  531,   40,
  406,  343,  344,  507,  503,  509,  360,  564,  362,  590,
  556,  410,    0,   40,  332,   60,   61,   62,  521,   41,
  265,    0,   40,  270,  392,  393,  561,  562,  346,  428,
  512,   41,   41,  279,   44,   44,  518,  519,  537,   41,
  394,   40,   44,  464,  362,  626,    0,   44,   41,   40,
  297,   44,  266,   41,   42,   43,   44,   45,   41,   47,
   41,   44,   41,   44,  406,   44,   41,  331,  304,   44,
  317,    0,   60,   61,   62,  328,   41,  313,   41,   44,
   40,   44,   44,  329,  364,  365,  366,   41,  335,  407,
  337,   41,  270,   41,   44,  317,   44,   40,   60,   61,
   62,   40,  349,   91,  317,   93,   40,  354,  590,   40,
  332,   41,   41,  360,   44,  362,  338,  339,  331,  317,
   40,  317,  464,   40,  346,   60,   61,   62,  373,  343,
  352,  503,  332,  331,   40,  357,  317,  317,  317,  352,
  362,  355,  338,  339,  626,  388,   41,  262,   41,   44,
  396,   44,  374,  367,  352,   40,  352,   40,  512,  338,
  339,  383,  258,  377,  518,  519,  388,  317,  262,  429,
  317,   41,  352,  352,   44,  388,   40,  391,  414,  429,
  317,  428,  493,  534,  495,  407,  497,  533,  499,  595,
  388,   41,  388,  393,   44,   41,  594,  533,   44,  571,
  542,  543,  339,  566,  461,  568,  454,  388,  388,  388,
  564,   41,  472,   41,   44,  352,   44,  527,  465,  529,
  537,  443,  472,  317,  533,  493,  531,  495,  450,  497,
  443,  499,   41,  533,  456,   44,  590,  331,  388,   41,
  533,  388,   44,  649,   41,  443,  511,  443,  513,   41,
  622,  388,   44,  595,  521,   44,  628,  504,  352,  657,
  456,  508,  443,  443,  443,  533,  521,  489,  283,  491,
  533,  259,  626,  533,  262,  263,  533,   41,  266,  539,
  643,  538,  533,  533,  262,   41,   40,  538,   44,  167,
  168,  538,    0,  443,  388,  317,  443,  669,  511,  525,
  513,  515,  331,  517,  266,  519,  443,  649,   44,  431,
  355,  533,  666,  387,  561,  562,  304,  564,  259,  566,
  533,  568,  367,  355,  314,  313,  314,   44,  316,  317,
  352,  266,  377,   41,   42,   43,   44,   45,  317,   47,
  328,   41,  331,  331,  332,   41,  391,  262,   40,  443,
  338,  339,   60,   61,   62,  343,  525,   44,  346,  338,
  339,   41,  598,  317,  352,   46,  388,  355,   41,  357,
   93,  270,    0,  352,  362,   44,  364,  456,  493,  367,
  495,  343,  497,   91,  499,   93,  374,  634,  317,  377,
   41,  377,  431,  355,  444,  383,  643,   41,  352,  387,
  388,  314,   41,  391,  383,  367,  521,  331,  343,  388,
  339,  332,  527,   41,  529,  377,   44,  383,  533,  407,
  355,  443,  383,  352,  533,  346,   41,  521,   41,  391,
   40,  338,  367,  527,  388,  529,   44,   40,  378,  533,
  316,  362,  377,  431,  316,   40,  328,   41,   40,   40,
  451,   41,  431,  374,   41,  443,  391,   41,   40,  388,
  537,   41,  450,  357,  443,  270,  454,  378,  456,  533,
  515,  450,  517,  389,  519,  259,  339,  456,  534,  534,
  534,   41,   41,   41,  357,   41,  407,   44,   41,  443,
   41,   40,   44,   41,   41,  533,   41,  534,   41,  487,
   41,  489,   41,  491,  358,  493,  358,  495,   41,  497,
    0,  499,   41,  378,  443,   41,  331,   41,   40,  507,
   41,  509,   41,  511,   41,  513,   41,  515,  533,  517,
  357,  519,  357,  521,    0,  273,  274,   41,   41,  527,
   41,  529,   41,    0,  246,  533,  157,  163,  516,  214,
   82,  259,  166,  515,  262,  517,  294,  519,  266,  297,
  298,  543,  657,  316,  285,  120,    0,  286,  489,  162,
  491,  177,  265,  319,  321,   41,   42,   43,   44,   45,
  515,   47,  517,  297,  519,  194,  130,  214,  382,  327,
  237,  392,  154,  354,   60,   61,   62,  561,  572,  566,
  360,  359,   -1,   -1,   -1,  313,  314,   41,  316,  317,
   44,   -1,  350,  351,   -1,   -1,   -1,   -1,   -1,    0,
  328,   -1,   -1,  331,  332,   -1,   -1,   93,   -1,   -1,
  338,  339,   -1,   -1,   -1,  343,   -1,   -1,  346,   -1,
   -1,   -1,  380,   -1,  352,   -1,   -1,  355,   -1,  357,
   -1,   -1,   -1,   -1,  362,   -1,  364,   -1,   -1,  367,
   41,   -1,   -1,   44,  402,   -1,  374,   -1,   -1,  377,
   -1,   -1,   -1,   -1,   -1,  383,   -1,   -1,   -1,  387,
  388,  419,   -1,  391,   -1,   -1,   -1,   -1,   -1,  317,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  407,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  338,  339,   -1,   -1,   -1,  453,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  431,  352,   -1,   -1,   -1,   -1,  357,
   -1,   -1,   -1,   -1,   -1,  443,   -1,  475,   -1,  477,
   -1,  479,  450,   -1,   -1,   -1,  454,   -1,  456,   -1,
   -1,   -1,   -1,  179,   -1,  383,   -1,   -1,   -1,   -1,
  388,   -1,  188,   -1,  190,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  487,
   -1,  489,   -1,  491,   -1,  493,   -1,  495,   -1,  497,
   -1,  499,   -1,  259,   -1,  533,  262,  263,   -1,  507,
  266,  509,   -1,  511,   -1,  513,   -1,  515,   -1,  517,
   -1,  519,   -1,  521,    0,  443,   -1,  525,   -1,  527,
   -1,  529,  450,   -1,   -1,  533,   -1,   -1,  456,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  304,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  313,  314,   -1,
  316,  317,   -1,   -1,   -1,   41,   42,   43,   44,   45,
   -1,   47,  328,   -1,   -1,  331,  332,   -1,  294,   -1,
   -1,   -1,  338,  339,   60,   61,   62,  343,  259,   -1,
  346,   -1,   -1,  317,   -1,   -1,  352,   -1,   -1,  355,
   -1,  357,   -1,   -1,   -1,   -1,  362,   -1,  364,    0,
   -1,  367,   -1,   -1,  338,  339,   -1,   93,  374,   -1,
   -1,  377,   -1,   -1,   -1,   -1,   -1,  383,  352,   -1,
   -1,  387,  388,   -1,   -1,  391,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  316,  317,   -1,   -1,   -1,
   41,  407,   43,   44,   45,   -1,   -1,   -1,   -1,  383,
   -1,   -1,   -1,  387,  388,   -1,   -1,  338,  339,   60,
   61,   62,   -1,   -1,   -1,  431,   -1,   -1,  394,   -1,
   -1,  352,   -1,   -1,  355,   -1,   -1,  443,   -1,   -1,
   -1,   -1,   -1,   -1,  450,   -1,   -1,   -1,  454,  415,
  456,   -1,   93,   -1,  420,   -1,   -1,  431,   -1,   -1,
   -1,   -1,  383,   -1,   -1,   -1,  387,  388,   -1,  443,
   -1,   -1,   -1,   -1,   -1,   -1,  450,   -1,   -1,    0,
   -1,  487,  456,  489,   -1,  491,   -1,  493,   -1,  495,
   -1,  497,   -1,  499,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  507,   -1,  509,   -1,  511,   -1,  513,   -1,  515,
  431,  517,   -1,  519,   -1,  521,   -1,   -1,   -1,   -1,
   41,   -1,  443,   44,   -1,   -1,   -1,  533,   -1,  450,
   -1,   -1,   -1,  259,   -1,  456,  262,   -1,   -1,   -1,
  266,   -1,   -1,   -1,   -1,   -1,  512,   -1,   -1,   -1,
   -1,   -1,  518,  519,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,    0,   -1,   -1,   -1,   -1,   -1,   -1,  535,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  313,  314,    0,
  316,  317,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  328,   41,   -1,  331,   44,   -1,   -1,   -1,
   -1,   -1,  338,  339,   -1,   -1,   -1,  343,  259,   -1,
   -1,  262,   -1,    0,  590,  266,  352,   -1,   -1,  355,
   41,   -1,   -1,   44,   -1,   -1,   -1,   -1,  364,   -1,
   -1,  367,   -1,   -1,   -1,   -1,   -1,    0,   -1,   -1,
   -1,  377,   -1,   -1,   -1,   -1,   -1,  383,   -1,   -1,
  626,  387,  388,   -1,   41,  391,   43,   44,   45,   -1,
   -1,   -1,  313,  314,   -1,  316,  317,   -1,   -1,   -1,
   -1,   -1,   -1,   60,   61,   62,   -1,  328,   41,   -1,
  331,   44,   -1,   -1,   -1,   -1,   -1,  338,  339,   -1,
   -1,   -1,  343,   -1,   -1,  431,   -1,   -1,    0,   -1,
   -1,  352,   -1,   -1,  355,   -1,   93,  443,   -1,   -1,
   -1,   -1,   -1,  364,  450,   -1,  367,   -1,  454,   -1,
  456,   -1,   -1,   -1,   -1,   -1,  377,   -1,   -1,   -1,
   -1,   -1,  383,   -1,   -1,   -1,  387,  388,   -1,   41,
  391,   -1,   44,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  487,   -1,   -1,   -1,   -1,   -1,  493,   -1,  495,
   -1,  497,   -1,  499,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  507,   -1,  509,   -1,  511,   -1,  513,   -1,  515,
  431,  517,   -1,  519,   -1,  521,  317,   -1,   -1,   -1,
   -1,   -1,  443,   -1,   -1,   -1,   -1,  533,   -1,  450,
   -1,  332,   -1,  454,   -1,  456,   -1,  338,  339,   -1,
   -1,  259,   -1,   -1,   -1,  346,   -1,   -1,   -1,   -1,
   -1,  352,   -1,   -1,   -1,   -1,  357,   -1,   -1,   -1,
   -1,  362,   -1,   -1,   -1,   -1,  487,   -1,  259,    0,
   -1,   -1,  493,  374,  495,   -1,  497,   -1,  499,   -1,
   -1,   -1,  383,   -1,   -1,   -1,  507,  388,  509,   -1,
  511,   -1,  513,   -1,  515,   -1,  517,   -1,  519,  317,
  521,   -1,  259,   -1,   -1,  262,  407,   -1,   -1,  266,
   41,   -1,  533,   44,   -1,   -1,   -1,   -1,   -1,   -1,
  338,  339,   -1,   -1,   -1,   -1,  317,   -1,   -1,   60,
   61,   62,   -1,   -1,  352,   -1,   -1,  355,   -1,   -1,
   -1,   -1,  443,   -1,   -1,   -1,   -1,  338,  339,  450,
   -1,   -1,   -1,   -1,   -1,  456,  313,  314,    0,  316,
  317,  352,   93,   -1,  355,  383,   -1,   -1,   -1,  387,
  388,  328,   -1,   -1,  331,   -1,   -1,   -1,   -1,   -1,
   -1,  338,  339,   -1,  317,   -1,  343,  259,  489,   -1,
  491,   -1,  383,   -1,   -1,  352,  387,  388,  355,   41,
   -1,   -1,   44,   -1,   -1,  338,  339,  364,   -1,   -1,
  367,   -1,   -1,  431,   -1,   -1,   -1,   -1,   -1,  352,
  377,   -1,   -1,   -1,   -1,  443,  383,   -1,   -1,   -1,
  387,  388,  450,   -1,  391,   -1,   -1,   -1,  456,   -1,
  431,   -1,   -1,   -1,   -1,  317,   -1,   -1,   -1,   -1,
  383,   93,  443,   -1,  387,  388,   -1,   -1,   -1,  450,
   -1,   -1,   -1,   -1,   -1,  456,  338,  339,   -1,   -1,
   -1,   -1,   -1,   -1,  431,   -1,   -1,   -1,   -1,   -1,
  352,   -1,   -1,  355,   -1,   -1,  443,   -1,   -1,   40,
   -1,   -1,   43,  450,   45,   -1,   -1,  454,  431,  456,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  443,  383,   63,   -1,   -1,  387,  388,  450,   -1,   -1,
   -1,   -1,   -1,  456,   -1,   -1,   -1,   -1,  259,   -1,
  487,  262,   -1,   -1,   -1,  266,  493,   -1,  495,   -1,
  497,   -1,  499,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  507,   -1,  509,   -1,  511,   -1,  513,   -1,  515,  431,
  517,   -1,  519,   -1,  521,   -1,   -1,   -1,   -1,   -1,
   -1,  443,   -1,   -1,   -1,   -1,  533,   -1,  450,   -1,
   -1,   -1,  313,  314,  456,  316,  317,    0,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  331,   -1,   -1,   -1,   -1,   -1,   -1,  338,  339,   -1,
   -1,   -1,  343,   -1,   -1,   -1,   -1,  259,   -1,   -1,
  262,  352,   -1,   -1,  355,   -1,   -1,   -1,   41,   -1,
   -1,   44,   -1,  364,   -1,   -1,  367,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  377,   -1,   -1,   -1,
   -1,   -1,  383,   -1,   -1,   -1,  387,  388,   -1,   -1,
  391,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  313,  314,   -1,   -1,  317,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  331,
   -1,   -1,   -1,   -1,   -1,   -1,  338,  339,   -1,   -1,
  431,   -1,   -1,   40,   -1,   -1,   43,   -1,   45,   -1,
  352,   -1,  443,  355,  265,   -1,   -1,   -1,   -1,  450,
  271,  272,   -1,  454,   -1,  456,   63,   -1,   -1,   -1,
   -1,  282,   -1,  284,   -1,   -1,  287,   -1,   -1,   -1,
   -1,  383,   -1,  294,   -1,  387,  388,   -1,   -1,  300,
   -1,   -1,   -1,   -1,   -1,   -1,  487,   -1,   -1,   -1,
   -1,   -1,  493,   -1,  495,   -1,  497,   -1,  499,   -1,
  321,   -1,   -1,   -1,   -1,   -1,  507,   -1,  509,   -1,
  511,   -1,  513,   -1,  515,   -1,  517,   -1,  519,  431,
  521,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  443,  533,   -1,   -1,   -1,   -1,   -1,  450,   -1,
   -1,   -1,  454,   -1,  456,  366,   -1,  368,  369,   40,
   -1,   -1,   43,   -1,   45,   -1,  377,  378,  379,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   63,  394,   -1,   -1,  259,   -1,   -1,   -1,
   -1,  493,   -1,  495,   -1,  497,   -1,  499,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  507,   -1,  509,   -1,  511,
   -1,  513,   -1,   -1,   -1,  426,  427,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  441,  533,   -1,  444,   -1,   -1,  447,   -1,   -1,   -1,
  451,   -1,   -1,   -1,  317,   -1,   -1,   -1,  459,  460,
  461,  462,  463,   -1,  465,  466,  467,   -1,  469,  470,
   -1,   -1,   -1,   -1,   -1,  338,  339,   -1,  265,   -1,
  481,   -1,  483,   -1,  271,  272,   -1,   -1,   -1,  352,
   -1,   -1,   -1,   -1,   -1,  282,   -1,  284,   -1,   -1,
  287,   -1,   -1,   -1,   -1,   -1,   -1,  294,   -1,   -1,
   -1,   -1,   -1,  300,   -1,   -1,   -1,   -1,   -1,   -1,
  383,   -1,   -1,   -1,  387,  388,   -1,   -1,   -1,   -1,
  531,   -1,  533,  534,  535,  536,  537,  538,  539,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  431,   -1,
   -1,   -1,   40,   -1,   -1,   43,   -1,   45,   -1,  366,
  443,  368,  369,   -1,   -1,   -1,   -1,  450,   -1,   -1,
   -1,  378,  379,  456,  265,   63,   -1,   -1,   -1,   -1,
  271,  272,   -1,   -1,   -1,   -1,   -1,  394,   -1,   -1,
   -1,  282,   -1,  284,   -1,   -1,  287,   -1,   -1,   -1,
   -1,   -1,   -1,  294,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  426,
  427,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  441,   -1,   -1,   -1,   -1,   -1,
  447,   -1,   -1,   -1,  451,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  459,  460,  461,  462,  463,   -1,  465,  466,
  467,   -1,  469,  470,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  481,  366,  483,  368,  369,   -1,
   -1,   -1,   40,   -1,   -1,   43,   -1,   45,  379,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  394,   -1,   63,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  531,   -1,  533,  534,  535,  536,
  537,  538,  539,   -1,   -1,  426,  427,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  441,   -1,   -1,   -1,   -1,   -1,  447,   -1,   -1,   -1,
  451,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  459,  460,
  461,  462,  463,   -1,  465,  466,  467,  265,  469,  470,
   -1,   -1,   -1,  271,  272,   -1,   -1,   -1,   -1,   -1,
  481,   -1,  483,   -1,  282,   -1,  284,   -1,   -1,  287,
   -1,   -1,   -1,   -1,   -1,   -1,  294,   -1,   -1,   -1,
   -1,   -1,  300,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  321,   -1,   -1,   -1,   -1,   -1,   -1,
  531,   -1,  533,  534,  535,  536,  537,  538,  539,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  366,   -1,
  368,  369,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  377,
  378,  379,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  258,   -1,  260,   -1,   -1,   40,  394,  265,   43,   -1,
   45,   -1,   -1,  271,  272,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  282,   -1,  284,   -1,   63,  287,
   -1,   -1,   -1,   -1,   -1,   -1,  294,   -1,  426,  427,
   -1,   -1,  300,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  441,   -1,   -1,  444,   -1,   -1,  447,
   -1,   -1,   -1,  451,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  459,  460,  461,  462,  463,   -1,  465,  466,  467,
   -1,  469,  470,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  481,   -1,  483,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  366,   -1,
  368,  369,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  378,  379,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   40,
   -1,   -1,   43,   -1,   45,   -1,  394,   -1,   -1,   -1,
   -1,   -1,   -1,  531,   -1,  533,  534,  535,  536,  537,
  538,   -1,   63,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  420,   -1,   -1,   -1,   -1,   -1,  426,  427,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  441,   -1,   -1,   -1,   -1,   -1,  447,
   -1,   -1,   -1,  451,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  459,  460,  461,  462,  463,   -1,  465,  466,  467,
   -1,  469,  470,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  481,   -1,  483,   -1,   -1,   -1,   -1,
  265,   -1,   -1,   -1,  269,   -1,  271,  272,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  282,   -1,  284,
   -1,   -1,  287,   -1,   -1,   -1,   -1,   -1,   -1,  294,
   -1,   -1,   -1,   40,   -1,   42,   43,   -1,   45,   -1,
   -1,   -1,   -1,  531,   -1,  533,  534,  535,  536,  537,
  538,   -1,   -1,   -1,   -1,   -1,   63,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  331,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  361,   -1,   -1,   -1,
   -1,  366,   -1,  368,  369,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  379,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  265,   -1,   -1,   -1,   -1,  394,
  271,  272,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  282,   -1,  284,   -1,   -1,  287,   -1,   -1,   -1,
   -1,   -1,   -1,  294,   -1,   -1,   -1,   -1,   -1,  300,
   -1,  426,  427,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  437,   -1,   -1,   -1,  441,   -1,   -1,   -1,
  321,   -1,  447,   -1,   -1,   -1,  451,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  459,  460,  461,  462,  463,   -1,
  465,  466,  467,   -1,  469,  470,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  481,   -1,  483,   -1,
   -1,   -1,   -1,   -1,   -1,  366,   -1,  368,  369,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  378,  379,   40,
   41,   -1,   43,   -1,   45,   -1,   -1,   -1,   -1,   -1,
   -1,  258,   -1,  394,   -1,   -1,   -1,   -1,  265,   -1,
   -1,   -1,   63,   -1,  271,  272,  531,   -1,  533,  534,
  535,  536,  537,  538,   -1,  282,   -1,  284,   -1,   -1,
  287,   -1,   -1,   -1,   -1,  426,  427,  294,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  441,   -1,  309,  444,   -1,   -1,  447,   -1,   -1,   -1,
  451,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  459,  460,
  461,  462,  463,   -1,  465,  466,  467,   -1,  469,  470,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  481,   -1,  483,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   40,   -1,   -1,   43,  366,
   45,  368,  369,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  379,   -1,   -1,   -1,   -1,   -1,   63,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  394,   -1,   -1,
  531,   -1,  533,  534,  535,  536,  537,  538,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  426,
  427,   -1,  429,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  441,   -1,   -1,   -1,   -1,   -1,
  447,   -1,   -1,   -1,  451,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  459,  460,  461,  462,  463,   -1,  465,  466,
  467,   -1,  469,  470,  265,   -1,   -1,   -1,   -1,   -1,
  271,  272,   -1,   -1,  481,   -1,  483,   -1,   -1,   -1,
   -1,  282,   -1,  284,   -1,   -1,  287,   -1,   -1,   -1,
   -1,   -1,   -1,  294,   -1,   -1,   -1,   -1,   -1,  300,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  531,   -1,  533,  534,  535,  536,
  537,  538,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   40,   -1,   -1,   43,  366,   45,  368,  369,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  378,  379,   -1,
  265,   -1,   -1,   -1,   63,   -1,  271,  272,   -1,   -1,
   -1,   -1,   -1,  394,   -1,   -1,   -1,  282,   -1,  284,
   -1,   -1,  287,   -1,   -1,   -1,   -1,   -1,   -1,  294,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  426,  427,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  441,   -1,   -1,   -1,   -1,   -1,  447,   -1,   -1,   -1,
  451,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  459,  460,
  461,  462,  463,   -1,  465,  466,  467,   -1,  469,  470,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  481,  366,  483,  368,  369,   40,   -1,   42,   43,   -1,
   45,   -1,   -1,   -1,  379,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   63,  394,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  531,   -1,  533,  534,  535,  536,  537,  538,   -1,   -1,
   -1,  426,  427,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  441,   -1,   -1,   -1,
   -1,   -1,  447,   -1,   -1,   -1,  451,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  459,  460,  461,  462,  463,  464,
  465,  466,  467,  468,  469,  470,  265,   -1,   -1,   -1,
   -1,   -1,  271,  272,   -1,   -1,  481,   -1,  483,   -1,
  485,   -1,   -1,  282,   -1,  284,   -1,   -1,  287,   -1,
   -1,   -1,   -1,   -1,   -1,  294,   -1,   -1,   -1,   40,
   -1,  300,   43,   -1,   45,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   63,   -1,   -1,   -1,  531,   -1,  533,  534,
  535,  536,  537,  538,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  366,   -1,  368,
  369,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  378,
  379,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  265,   -1,   -1,   -1,   -1,  394,  271,  272,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  282,   -1,  284,
   -1,   -1,  287,   -1,   -1,   -1,   -1,   -1,   -1,  294,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  426,  427,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  441,   -1,   -1,   -1,   -1,   -1,  447,   -1,
   -1,   -1,  451,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  459,  460,  461,  462,  463,   -1,  465,  466,  467,   -1,
  469,  470,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  481,   -1,  483,   -1,   -1,   -1,   -1,   -1,
   -1,  366,   -1,  368,  369,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   40,   -1,  379,   43,   -1,   45,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  258,   -1,  394,
   -1,   -1,   -1,   -1,  265,   63,   -1,   -1,   -1,   -1,
  271,  272,  531,   -1,  533,  534,  535,  536,  537,  538,
   -1,  282,   -1,  284,   -1,   -1,  287,   -1,   -1,   -1,
   -1,  426,  427,  294,  429,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  441,   -1,  309,   -1,
   -1,   -1,  447,   -1,   -1,   -1,  451,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  459,  460,  461,  462,  463,   -1,
  465,  466,  467,   -1,  469,  470,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  481,   -1,  483,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   40,   -1,
   -1,   43,   -1,   45,   -1,  366,   -1,  368,  369,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  379,   -1,
   -1,   63,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  394,   -1,   -1,  531,   -1,  533,  534,
  535,  536,  537,  538,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  426,  427,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  441,   -1,   -1,   -1,   -1,   -1,  447,   -1,   -1,   -1,
  451,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  459,  460,
  461,  462,  463,   -1,  465,  466,  467,  265,  469,  470,
   -1,   -1,   -1,  271,  272,   -1,   40,   -1,   -1,   43,
  481,   45,  483,   -1,  282,   -1,  284,   -1,   -1,  287,
   -1,   -1,   -1,   -1,   -1,   -1,  294,   -1,   -1,   63,
   -1,   -1,  300,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  531,   -1,  533,  534,  535,  536,  537,  538,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  366,   -1,
  368,  369,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  378,  379,   -1,  265,   -1,   -1,   -1,   -1,  270,  271,
  272,   -1,   -1,   -1,   -1,   -1,  394,   -1,   -1,   -1,
  282,   -1,  284,   -1,   -1,  287,   -1,   -1,   -1,   -1,
   -1,   -1,  294,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  426,  427,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  441,   -1,   -1,   -1,   -1,   -1,  447,
   -1,   -1,   -1,  451,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  459,  460,  461,  462,  463,   -1,  465,  466,  467,
   -1,  469,  470,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  481,  366,  483,  368,  369,   -1,   -1,
   -1,   40,   -1,   42,   43,   -1,   45,  379,   -1,   -1,
   -1,  265,   -1,   -1,   -1,   -1,   -1,  271,  272,   -1,
   -1,   -1,  394,   -1,   63,   -1,   -1,   -1,  282,   -1,
  284,   -1,   -1,  287,   -1,   -1,   -1,   -1,   -1,   -1,
  294,   -1,   -1,  531,   -1,  533,  534,  535,  536,  537,
  538,   -1,   -1,   -1,  426,  427,   -1,  429,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  441,
   -1,   -1,   -1,   -1,   -1,  447,   -1,   -1,   -1,  451,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  459,  460,  461,
  462,  463,   -1,  465,  466,  467,   -1,  469,  470,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  481,
   -1,  483,  366,   -1,  368,  369,   -1,   -1,   -1,   40,
   -1,   -1,   43,   -1,   45,  379,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  394,   -1,   63,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  531,
   -1,  533,  534,  535,  536,  537,  538,   -1,   -1,   -1,
   -1,   -1,  426,  427,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  441,   -1,   -1,
   -1,   -1,   -1,  447,   -1,   -1,   -1,  451,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  459,  460,  461,  462,  463,
  464,  465,  466,  467,   -1,  469,  470,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  265,  481,   -1,  483,
   -1,  485,  271,  272,   -1,   40,   -1,   -1,   43,   -1,
   45,   -1,   -1,  282,   -1,  284,   -1,   -1,  287,   -1,
   -1,   -1,   -1,   -1,   -1,  294,   -1,   -1,   63,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  531,   -1,  533,
  534,  535,  536,  537,  538,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  366,   -1,  368,
  369,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  379,   -1,   -1,   -1,  265,   -1,   -1,   -1,   -1,   -1,
  271,  272,   -1,   40,   -1,  394,   43,   -1,   45,   -1,
   -1,  282,   -1,  284,   -1,   -1,  287,   -1,   -1,   -1,
   -1,   -1,   -1,  294,   -1,   -1,   63,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  426,  427,   -1,
  429,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  441,   -1,   -1,   -1,   -1,   -1,  447,   -1,
   -1,   -1,  451,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  459,  460,  461,  462,  463,   -1,  465,  466,  467,   -1,
  469,  470,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  481,   -1,  483,  366,   -1,  368,  369,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  379,   -1,
  265,   -1,   -1,   -1,   -1,   -1,  271,  272,   -1,   -1,
   -1,   -1,   -1,  394,   -1,   -1,   -1,  282,   -1,  284,
   40,   -1,  287,   43,   -1,   45,   -1,   -1,   -1,  294,
   -1,   -1,  531,   -1,  533,  534,  535,  536,  537,  538,
   -1,   -1,   -1,   63,   -1,  426,  427,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  441,   -1,   -1,   -1,   -1,   -1,  447,   -1,   -1,   -1,
  451,   -1,   -1,  454,   -1,   -1,   -1,   -1,  459,  460,
  461,  462,  463,   -1,  465,  466,  467,   -1,  469,  470,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
  481,  366,  483,  368,  369,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  378,  379,   -1,   -1,   -1,  265,   -1,
   -1,   -1,   -1,   -1,  271,  272,   -1,   -1,   -1,  394,
   -1,   -1,   -1,   -1,   -1,  282,   -1,  284,   -1,   -1,
  287,   -1,   -1,   -1,   -1,   -1,   -1,  294,   -1,   -1,
  531,   -1,  533,  534,  535,  536,  537,  538,   -1,   -1,
   -1,  426,  427,   -1,   -1,   -1,   -1,   -1,   40,   -1,
   -1,   43,   -1,   45,   -1,   -1,  441,   -1,   -1,   -1,
   -1,   -1,  447,   -1,   -1,   -1,  451,   -1,   -1,   -1,
   -1,   63,   -1,   -1,  459,  460,  461,  462,  463,   -1,
  465,  466,  467,   -1,  469,  470,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  481,   -1,  483,  366,
   -1,  368,  369,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  379,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  265,   -1,  394,   -1,   -1,
   -1,  271,  272,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  282,   -1,  284,   -1,  531,  287,  533,  534,
  535,  536,  537,  538,  294,   -1,   -1,   -1,   -1,  426,
  427,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  441,   -1,   -1,   -1,   -1,   -1,
  447,   -1,   40,   -1,  451,   43,   -1,   45,   -1,   -1,
   -1,   -1,  459,  460,  461,  462,  463,   -1,  465,  466,
  467,  468,  469,  470,   -1,   63,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  481,   -1,  483,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  366,   -1,  368,  369,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  378,  379,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  394,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  531,   -1,  533,  534,  535,  536,
  537,  538,   -1,  265,   -1,   -1,   -1,   -1,   -1,  271,
  272,   -1,   -1,   -1,   -1,   -1,  426,  427,   -1,   -1,
  282,   -1,  284,   -1,   -1,  287,   -1,   -1,   -1,   -1,
   -1,  441,  294,   -1,   -1,   -1,   -1,  447,   -1,   -1,
   -1,  451,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  459,
  460,  461,  462,  463,   -1,  465,  466,  467,   40,  469,
  470,   43,   -1,   45,   -1,   -1,   -1,   -1,   -1,  331,
   -1,  481,   -1,  483,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   63,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  366,   -1,  368,  369,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  379,   -1,   -1,
   -1,  531,   -1,  533,  534,  535,  536,  537,  538,   -1,
   -1,   -1,  394,   -1,   -1,   -1,   -1,  265,   -1,   -1,
   -1,   -1,   -1,  271,  272,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  282,   -1,  284,   -1,   -1,  287,
   -1,   -1,   -1,   -1,  426,  427,  294,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  441,
   -1,   -1,   -1,   -1,   -1,  447,   -1,   -1,   40,  451,
   -1,   43,   -1,   45,   -1,   -1,   -1,  459,  460,  461,
  462,  463,   -1,  465,  466,  467,   -1,  469,  470,   -1,
   -1,   63,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  481,
   -1,  483,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  366,   -1,
  368,  369,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  379,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  394,   -1,   -1,  531,
   -1,  533,  534,  535,  536,  537,  538,   -1,   -1,   -1,
   -1,   -1,   -1,  265,   -1,   -1,   -1,   -1,   -1,  271,
  272,   -1,   -1,   40,   -1,   -1,   -1,   -1,  426,  427,
  282,  429,  284,   -1,   -1,  287,   -1,   -1,   -1,   -1,
   -1,   -1,  294,  441,   -1,   -1,   63,   -1,   -1,  447,
   -1,   -1,   -1,  451,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,  459,  460,  461,  462,  463,   -1,  465,  466,  467,
   -1,  469,  470,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,  481,   -1,  483,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  366,   -1,  368,  369,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  379,   -1,   -1,
   -1,   -1,   -1,  531,   -1,  533,  534,  535,  536,  537,
  538,   -1,  394,  265,   -1,   -1,   -1,   -1,   -1,  271,
  272,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  284,   -1,   -1,  287,   -1,   -1,   -1,   -1,
   -1,   -1,  294,   -1,  426,  427,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  441,
   -1,   -1,   -1,   -1,   -1,  447,   -1,   -1,   -1,  451,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  459,  460,  461,
  462,  463,   -1,  465,  466,  467,   -1,  469,  470,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  481,
   -1,  483,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,  368,  369,  265,   -1,
   -1,   -1,   -1,   -1,  271,  272,   -1,  379,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  284,   -1,   -1,
  287,   -1,  394,   -1,   -1,   -1,   -1,  294,   -1,  531,
   -1,  533,  534,  535,  536,  537,  538,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,  427,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  451,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  459,  460,  461,
  462,  463,   -1,  465,  466,  467,   -1,  469,  470,   -1,
   -1,  368,  369,   -1,   -1,   -1,   -1,   -1,   -1,  481,
   -1,  483,  379,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,  394,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,  531,
  427,  533,  534,  535,  536,  537,  538,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  451,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,  459,  460,  461,  462,  463,   -1,  465,  466,
  467,   -1,  469,  470,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  481,   -1,  483,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,   -1,
   -1,   -1,   -1,   -1,  531,   -1,  533,  534,  535,  536,
  537,  538,
  };

#line 2273 "SQL92-min.y"
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
  public const int ROW = 472;
  public const int TOP = 473;
  public const int _RS_END = 474;
  public const int CHARACTER_VARYING = 475;
  public const int CHAR_VARYING = 477;
  public const int DOUBLE_PRECISION = 479;
  public const int count_all_fct = 481;
  public const int xml_forest_all = 483;
  public const int xml_attributes_all = 485;
  public const int NOTLIKE = 487;
  public const int CROSSJOIN = 489;
  public const int UNIONJOIN = 491;
  public const int OPTION_NULL = 493;
  public const int OPTION_EMPTY = 495;
  public const int OPTION_ABSENT = 497;
  public const int OPTION_NIL = 499;
  public const int NO_VALUE = 501;
  public const int NO_DEFAULT = 503;
  public const int NO_CONTENT = 505;
  public const int PRESERVE_WHITESPACE = 507;
  public const int STRIP_WHITESPACE = 509;
  public const int RETURNING_CONTENT = 511;
  public const int RETURNING_SEQUENCE = 513;
  public const int not_equals_operator = 515;
  public const int greater_than_or_equals_operator = 517;
  public const int less_than_or_equals_operator = 519;
  public const int concatenation_operator = 521;
  public const int double_colon = 523;
  public const int asterisk_tag = 525;
  public const int double_slash = 527;
  public const int ref_operator = 529;
  public const int parameter_name = 531;
  public const int embdd_variable_name = 532;
  public const int id = 533;
  public const int unsigned_integer = 534;
  public const int unsigned_float = 535;
  public const int unsigned_double = 536;
  public const int string_literal = 537;
  public const int func = 538;
  public const int optimizer_hint = 539;
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
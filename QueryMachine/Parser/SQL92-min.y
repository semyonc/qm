/*    
    SQLXEngine - Implementation of ANSI-SQL specification and 
       SQL-engine for executing the SELECT SQL command across the different data sources.
    Copyright (C) 2008-2009  Semyon A. Chertkov (semyonc@gmail.com)

    This library is free software; you can redistribute it and/or
    modify it under the terms of the GNU Lesser General Public
    License as published by the Free Software Foundation; either
    version 2.1 of the License, or (at your option) any later version.

    This library is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public
    License along with this library; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
*/
/*
BNF for SQL92
This file contains a depth-first tree traversal of the BNF
for the  language done at about 27-AUG-1992 11:03:41.64.
The specific version of the BNF included here is:  ANSI-only, SQL2-only.
*/

%{

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
		
%}

/* Reserved words */


%token _RS_START
%token ALL
%token AND
%token ANY 
%token ARE
%token AS 
%token ASC
%token AT
%token AVG
%token BETWEEN 
%token BIT 
%token BIT_LENGTH
%token BOTH 
%token BY 
%token CASE 
%token CAST
%token CHAR 
%token CHARACTER 
%token CHAR_LENGTH
%token CHARACTER_LENGTH 
%token CONNECT
%token CONNECTION 
%token CONSTRAINT
%token CONSTRAINTS 
%token CONTINUE
%token CONVERT 
%token CORRESPONDING 
%token COUNT 
%token CREATE 
%token CROSS
%token COALESCE
%token CURRENT
%token CURRENT_DATE 
%token CURRENT_TIME
%token CURRENT_TIMESTAMP 
%token CURRENT_USER 
%token CURSOR
%token DATE 
%token DAY 
%token DEALLOCATE 
%token DEC
%token DECIMAL 
%token DECLARE 
%token DEFAULT 
%token DEFERRABLE
%token DEFERRED 
%token DELETE 
%token DESC 
%token DESCRIBE 
%token DESCRIPTOR
%token DIAGNOSTICS
%token DISCONNECT 
%token DISTINCT 
%token DOMAIN 
%token DOUBLE 
%token DROP
%token ELSE 
%token END 
%token END_EXEC 
%token ESCAPE
%token EXCEPT 
%token EXCEPTION
%token EXEC 
%token EXECUTE 
%token EXISTS
%token EXTERNAL 
%token EXTRACT
%token FALSE 
%token FETCH 
%token FIRST 
%token FLOAT 
%token FOR
%token FOREIGN 
%token FOUND 
%token FROM 
%token FULL
%token GET 
%token GLOBAL 
%token GO 
%token GOTO
%token GRANT 
%token GROUP
%token HAVING 
%token HOUR
%token IDENTITY 
%token IMMEDIATE 
%token IN 
%token INDICATOR
%token INITIALLY 
%token INNER 
%token INPUT
%token INSENSITIVE 
%token INSERT 
%token INT 
%token INTEGER 
%token INTERSECT
%token INTERVAL 
%token INTO 
%token IS
%token ISOLATION
%token JOIN
%token KEY
%token LANGUAGE 
%token LAST 
%token LEADING 
%token LEFT
%token LEVEL 
%token LIKE 
%token LOCAL 
%token LOWER
%token MATCH 
%token MAX 
%token MIN 
%token MINUTE 
%token MONTH
%token NAMES 
%token NATIONAL 
%token NATURAL 
%token NCHAR 
%token NEXT 
%token NOT 
%token NULL
%token NULLIF 
%token NUMERIC
%token OCTET_LENGTH 
%token OF
%token ON 
%token ONLY 
%token OPEN 
%token OPTION 
%token OR
%token ORDER 
%token OUTER
%token OUTPUT 
%token OVERLAPS
%token PAD 
%token PARTIAL 
%token POSITION 
%token PREPARE
%token PRIMARY
%token PRIOR 
%token PRIVILEGES 
%token PROCEDURE 
%token PUBLIC
%token READ 
%token REAL 
%token REFERENCES 
%token RELATIVE 
%token RESTRICT
%token REVOKE 
%token RIGHT
%token ROLLBACK 
%token ROWS
%token SCHEMA 
%token SCROLL 
%token SECOND 
%token SECTION
%token SELECT
%token SESSION 
%token SESSION_USER 
%token SET
%token SIZE 
%token SMALLINT 
%token SOME 
%token SPACE 
%token SQL 
%token SQLCODE
%token SQLERROR 
%token SQLSTATE
%token SUBSTRING 
%token SUM 
%token SYSTEM_USER
%token TABLE 
%token TEMPORARY
%token THEN 
%token TIME 
%token TIMESTAMP
%token TIMEZONE_HOUR 
%token TIMEZONE_MINUTE
%token TO 
%token TRAILING 
%token TRANSACTION
%token TRANSLATE 
%token TRANSLATION 
%token TRIM 
%token TRUE
%token UNION 
%token UNIQUE 
%token UNKNOWN 
%token UPDATE 
%token UPPER 
%token USAGE
%token USER 
%token USING
%token VALUE 
%token VALUES 
%token VARCHAR 
%token WHEN 
%token WHENEVER 
%token WHERE 
%token WITH 
%token WRITE

%token XMLPI
%token XMLPARSE
%token XMLQUERY
%token XMLCOMMENT
%token XMLELEMENT
%token XMLATTRIBUTES
%token XMLCONCAT
%token XMLFOREST
%token XMLAGG
%token XMLNAMESPACES
%token XMLCDATA
%token XMLROOT
%token PASSING

%token TOP

%token _RS_END

%token CHARACTER_VARYING					   "CHARACTER VARYING"	
%token CHAR_VARYING							   "CHAR VARYING"	
%token DOUBLE_PRECISION						   "DOUBLE PRECISION"

%token count_all_fct                           "COUNT(*)"
%token xml_forest_all                          "XMLFOREST(*)"
%token xml_attributes_all					   "XMLATTRIBUTES(*)"
%token NOTLIKE                                 "NOT LIKE"

%token CROSSJOIN                               "CROSS JOIN"
%token UNIONJOIN							   "UNION JOIN"	
%token OPTION_NULL                             "OPTION NULL"   
%token OPTION_EMPTY                            "OPTION EMPTY"  
%token OPTION_ABSENT                           "OPTION ABSENT"
%token OPTION_NIL                              "OPTION NIL"  
%token NO_VALUE                                "NO VALUE"
%token NO_DEFAULT                              "NO DEFAULT"
%token NO_CONTENT                              "NO CONTENT"
%token PRESERVE_WHITESPACE                     "PRESERVE WHITESPACE" 
%token STRIP_WHITESPACE                        "STRIP WHITESPACE"  
%token RETURNING_CONTENT					   "RETURNING CONTENT"
%token RETURNING_SEQUENCE					   "RETURNING SEQUENCE"

%token not_equals_operator                     "<>"
%token greater_than_or_equals_operator         ">="
%token less_than_or_equals_operator            "<="
%token concatenation_operator                  "||"
%token double_colon							   "::"
%token asterisk_tag                            ".*"
%token double_slash                            "//"

%token parameter_name
%token embdd_variable_name

%token id
%token unsigned_integer
%token unsigned_float
%token unsigned_double
%token string_literal
%token func
%token optimizer_hint

%start direct_select_stmt_n_rows

%%

direct_select_stmt_n_rows 
      : opt_optimizer_hint query_exp 
      {            
            notation.ConfirmTag(Tag.Stmt, Descriptor.Root, $2);
            $$ = notation.ResolveTag(Tag.Stmt);
            if ($1 != null)
                notation.Confirm((Symbol)$$, Descriptor.OptimizerHint, $1);
      }
      | opt_optimizer_hint query_exp order_by_clause 
      {      
            notation.ConfirmTag(Tag.Stmt, Descriptor.Root, $2);
			notation.ConfirmTag(Tag.Stmt, Descriptor.Order, $3);						
			$$ = notation.ResolveTag(Tag.Stmt);
            if ($1 != null)
                notation.Confirm((Symbol)$$, Descriptor.OptimizerHint, $1);			
      }
      ;
     
as_clause 
      :  opt_AS column_name
      {
         $$ = $2;
      }
      ;
       
between_predicate 
      : row_value_constructor 
           opt_not_tag BETWEEN row_value_constructor AND row_value_constructor 
      {
         Symbol sym = new Symbol(Tag.Predicate);
         $$ = notation.Confirm(sym, Descriptor.Between, $1, $4, $6);
         if ($2 != null)
           notation.Confirm(sym, Descriptor.Inverse);
      }
      ;
      
opt_not_tag
      : /* Empty */
      {
        $$ = null;
      }
      | not_tag
      ; 
      
not_tag
      : NOT
      ;

case_abbreviation 
      : nullif_case_abbreviation
      | coalesce_case_abbreviation
      ;

nullif_case_abbreviation 
      : NULLIF '(' value_exp ',' value_exp ')'  
      {
          $$ = notation.Confirm(new Symbol(Tag.Expr), 
            Descriptor.NullIf, $3, $5);
      }
      ;

coalesce_case_abbreviation 
      : COALESCE '(' value_exp_list ')' 
      {
         $$ = notation.Confirm(new Symbol(Tag.Expr),
            Descriptor.Coalesce, $3);
      }
      ;            
      
value_exp_list
      : value_exp
      {
		$$ = Lisp.Cons($1);
      }
      | value_exp_list ',' value_exp
      {
		$$ = Lisp.Append($1, Lisp.Cons($3));
	  }
      ;
         
case_exp 
      : case_abbreviation 
      | case_spec 
      ;
      
case_operand 
      : value_exp 
      ;
      
case_spec 
      : simple_case 
      | searched_case 
      ;
      
null_tag
      : NULL
      ;

char_factor 
      : num_value_exp 
      | string_value_fct 
      ;
         
char_substring_fct 
      : SUBSTRING '(' char_value_exp 
          FROM start_position for_string_length ')' 
      {
         Symbol sym = new Symbol(Tag.CExpr);
         if ($6 == null)
			$$ = notation.Confirm(sym, Descriptor.Substring, $3, $5);
		 else
			$$ = notation.Confirm(sym, Descriptor.Substring, $3, $5, $6);
      }
      ;

for_string_length 
      : /* Empty */
      {
         $$ = null;
      }
      | FOR string_length 
      {
         $$ = $2;
      }
      ;

char_value_exp 
      : char_factor
      | char_value_exp concatenation_operator char_factor
      {
           $$ = notation.Confirm(new Symbol(Tag.CExpr), Descriptor.Concat, $1, $3);
      }
      ;
           
char_value_fct 
      : char_substring_fct 
      | fold 
      | form_conversion 
      | trim_fct 
      ;

column_name_list 
      : column_name
      {
		$$ = Lisp.Cons($1);
      }
      | column_name_list ',' column_name
      {
		$$ = Lisp.Append($1, Lisp.Cons($3));
	  }
      ;

comp_op 
      : '='                                  
      | not_equals_operator                  
      | '<'                                  
      | '>'                                  
      | less_than_or_equals_operator         
      | greater_than_or_equals_operator      
      ;

comp_predicate 
      : row_value_constructor comp_op 
          row_value_constructor 
      {
          $$ = notation.Confirm(new Symbol(Tag.Predicate), Descriptor.Pred, $1, $2, $3);
      }
      ;

all_tag  
    : ALL
    ;

corresponding_column_list 
    : column_name_list 
    ;

corresponding_spec 
    : CORRESPONDING 
    | CORRESPONDING by_corresponding_column_list 
    ;

by_corresponding_column_list 
    : BY '(' corresponding_column_list ')' 
    {
		$$ = $3;
    }
    ;

cross_join 
    : CROSSJOIN table_ref 
    {
		$$ = $2;
	}    
    ;
    
union_join
    : UNIONJOIN table_ref
    {
		$$ = $2;
	}
    ;
    
natural_join
    : NATURAL opt_join_type JOIN table_ref 
    {
		$$ = $4;
		if ($2 != null)
			notation.ConfirmTag(Tag.Join, Descriptor.JoinType, new TokenWrapper($2));
    }    
    ;

default_spec 
      : DEFAULT
      ;

derived_column 
      : derived_value 
      | derived_value as_clause
      {
          $$ = $1;          
          notation.Confirm((Symbol)$$, Descriptor.Alias, $2);          
      }
      ;
      
derived_value
      : value_exp 
      | TABLE subquery
      {
          $$ = $2;
          notation.Confirm((Symbol)$$,  Descriptor.Dynatable);
      }
      ; 

derived_column_list 
      : column_name_list 
      ;
 
opt_AS
    : /* Empty */
    | AS
    ;      

dyn_parameter_spec 
    : '?'
    ;

else_clause 
    : ELSE result 
      {
         $$ = notation.Confirm(new Symbol(Tag.CaseExpr), Descriptor.ElseBranch, $2);
      }
    ;

escape_char 
    : char_value_exp 
    ;

exists_predicate 
    : EXISTS subquery 
    {
      $$ = notation.Confirm(new Symbol(Tag.Predicate), Descriptor.Exists, $2);
    }
    ;

// explicit_table 
//     : TABLE table_name 
//     {
//         $$ = new Symbol(Tag.ExplictTable);
//         notation.Confirm((Symbol)$$, Descriptor.Explicit, $2);
//     }
//     ;

factor : num_primary
       | sign num_primary 
       {
         if ($1.Equals("-"))
			$$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.UnaryMinus, $2);
		 else
			$$ = 2;
       }
       ;

fold 
      : upper_or_lower
         '(' char_value_exp ')'
      {
         Symbol sym = new Symbol(Tag.CExpr);
         switch((int)$1)
         {
			case Token.UPPER:
				$$ = notation.Confirm(sym, Descriptor.StringUpper, $3);
				break;
			case Token.LOWER:
				$$ = notation.Confirm(sym, Descriptor.StringLower, $3);
				break;
         }
      }
      ;

upper_or_lower 
       : UPPER   
       | LOWER   
       ;

form_conversion 
      : CONVERT '(' char_value_exp 
           USING form_conversion_name ')' 
      {
         $$ = notation.Confirm(new Symbol(Tag.CExpr), 
			Descriptor.StringConvert,  $3, $5);
      }
      ;

form_conversion_name 
      : qualified_name 
      ;

from_clause 
      : FROM table_ref_list
      {
		notation.ConfirmTag(Tag.SQuery, Descriptor.From, $2);
      }
      ;
      
table_ref_list
      : table_ref
      {
        $$ = Lisp.Cons($1);
      }
      | table_ref_list ',' table_ref
      {
		$$ = Lisp.Append($1, Lisp.Cons($3));
	  }
      ;     

general_set_fct
      :  set_fct_type 
          '(' value_exp ')' 
      {
         $$ = notation.Confirm(new Symbol(Tag.AggExpr), Descriptor.Aggregate, new TokenWrapper($1), $3);
      }
      |  set_fct_type 
          '(' set_quantifier value_exp ')' 
      {
         Symbol sym = new Symbol(Tag.AggExpr);
         $$ = notation.Confirm(sym, Descriptor.Aggregate, new TokenWrapper($1), $4);
         if ((int)$3 == Token.DISTINCT)
            notation.Confirm(sym, Descriptor.Distinct);
      }
      ;

general_value_spec 
      : parameter_spec
      {
         $$ = new Parameter($1.ToString()); 
      } 
      | dyn_parameter_spec 
      | value_tag 
      ;

value_tag 
      : VALUE 
      ;

grouping_column_ref 
      : column_ref  
      {
        $$ = $1;
		notation.ConfirmTag(Tag.SQuery, Descriptor.GroupingColumnRef, $1);
      }
      ;

grouping_column_ref_list 
      : grouping_column_ref
      {
		$$ = Lisp.Cons($1);
      } 
      | grouping_column_ref_list ',' grouping_column_ref
      {
		$$ = Lisp.Append($1, Lisp.Cons($3));
      }
      ; 

opt_group_by_clause 
      : /* Empty */
      | GROUP BY
          grouping_column_ref_list 
      {
		notation.ConfirmTag(Tag.SQuery, Descriptor.GroupBy, $3);
      }    
      ;

opt_having_clause 
      : /* Empty */
      | HAVING search_condition 
      {
		notation.ConfirmTag(Tag.SQuery, Descriptor.Having, $2);
      }
      ;

indicator_parameter 
      : parameter_name 
      | indicator_tag parameter_name 
      ;

indicator_tag 
      : INDICATOR 
      ;

in_predicate 
      : row_value_constructor 
           opt_not_tag IN in_predicate_value 
      {         
         Symbol sym = new Symbol(Tag.Predicate);
		 $$ = notation.Confirm(sym, Descriptor.InSet, $1, $4);            
		 if ($2 != null)
		   notation.Confirm(sym, Descriptor.Inverse);
      }
      ;

in_predicate_value 
      : subquery 
      | '(' in_value_list ')' 
      {
         $$ = notation.Confirm(new Symbol(Tag.ValueList), Descriptor.ValueList, $2);
      }
      ;

in_value_list 
      : value_exp 
      {
         $$ = Lisp.Cons($1);
      }
      | in_value_list  ',' value_exp 
      {
         $$ = Lisp.Append($1, Lisp.Cons($3));
      }
      ;

joined_table 
      : table_ref_simple cross_join 
      {
		notation.ConfirmTag(Tag.Join, Descriptor.CrossJoin, $1, $2);
		$$ = notation.ResolveTag(Tag.Join);
      }
      | table_ref_simple union_join
      {
		notation.ConfirmTag(Tag.Join, Descriptor.UnionJoin, $1, $2);
		$$ = notation.ResolveTag(Tag.Join);      
      }
      | table_ref_simple natural_join      
      {
		notation.ConfirmTag(Tag.Join, Descriptor.NaturalJoin, $1, $2);
		$$ = notation.ResolveTag(Tag.Join);      
      }            
      | table_ref_simple qualified_join
      {
		notation.ConfirmTag(Tag.Join, Descriptor.QualifiedJoin, $1, $2);
		$$ = notation.ResolveTag(Tag.Join);      
      }
      | '(' table_ref ')'
      {
		$$ = notation.Confirm(new Symbol(Tag.Join), Descriptor.Branch, $2);
      }
      ;

join_column_list 
      : column_name_list 
      ;

join_condition 
      : ON search_condition 
      {
		$$ = $2;
      }
      ;

join_spec       
      : join_condition 
      | named_columns_join 
      | constraint_join
      ;      

join_type 
      : INNER
      | outer_join_type opt_OUTER
      ;
      
opt_OUTER
      : /* Empty */            
      | OUTER                  
      {
		  notation.ConfirmTag(Tag.Join, Descriptor.Outer);
	  }
      ;      

like_predicate 
      : char_value_exp LIKE pattern
      {
         $$ = notation.Confirm(new Symbol(Tag.Predicate), 
			Descriptor.Like, $1, $3);           
      }
      | char_value_exp NOTLIKE pattern
      {               
         Symbol sym = new Symbol(Tag.Predicate);
         $$ = notation.Confirm(sym, Descriptor.Like, $1, $3);           
		 notation.Confirm(sym, Descriptor.Inverse);
      }      
      | char_value_exp LIKE pattern ESCAPE escape_char
      {
         Symbol sym = new Symbol(Tag.Predicate);
         $$ = notation.Confirm(sym, Descriptor.Like, $1, $3);           
		 notation.Confirm(sym, Descriptor.Escape, $5);
      }      
      | char_value_exp NOTLIKE pattern ESCAPE escape_char
      {
         Symbol sym = new Symbol(Tag.Predicate);
         $$ = notation.Confirm(sym, Descriptor.Like, $1, $3);           
         notation.Confirm(sym, Descriptor.Inverse);
		 notation.Confirm(sym, Descriptor.Escape, $5);
      }      
      ;
            
match_predicate 
      : row_value_constructor 
          MATCH opt_unique_tag
            opt_match_type subquery 
      {
          Symbol sym = new Symbol(Tag.Predicate);          
          $$ = notation.Confirm(sym, Descriptor.Match, $1, $5);
		  if ($3 != null)
			notation.Confirm(sym, Descriptor.Unique);          
		  if ($4 != null)
			notation.Confirm(sym, Descriptor.MatchType, new TokenWrapper($4));
      }
      ;

opt_unique_tag
      : /* Empty */
      {
        $$ = null;
      }
      | UNIQUE        
      ;
      
opt_match_type       
      : /* Empty */ 
      {
        $$ = null;
      }
      | match_type
      ;
      
match_type
      : FULL          
      | PARTIAL       
      ;

named_columns_join 
      : USING '(' join_column_list ')' 
      {
		$$ = notation.Confirm(new Symbol(Tag.Join), Descriptor.Using, $3);
      }
      ;
      
constraint_join
      : USING PRIMARY KEY
      {
		$$ = notation.Confirm(new Symbol(Tag.Join), Descriptor.Constraint, new TokenWrapper($2));
      }
      | USING FOREIGN KEY
      {
        $$ = notation.Confirm(new Symbol(Tag.Join), Descriptor.Constraint, new TokenWrapper($2));
      }
      | USING CONSTRAINT qualified_name
      {
        $$ = notation.Confirm(new Symbol(Tag.Join), Descriptor.Constraint, $3);
      }
      ;

query_exp
      : query_term
      | query_exp query_exp_tag opt_all_tag opt_corresponding_spec query_term
      {         
         Symbol sym = new Symbol(Tag.QueryExp);     
         switch((int)$2)
         {
            case Token.UNION:
               notation.Confirm(sym, Descriptor.Union, $1, $5);
               break;
            case Token.EXCEPT:
               notation.Confirm(sym, Descriptor.Except, $1, $5);
               break;
         }
         if ($3 != null)
           notation.Confirm(sym, Descriptor.Distinct);
         $$ = sym;
      }
      ;
      
query_exp_tag
      : UNION			
      | EXCEPT			
      ;      
      
opt_all_tag
      : /* Empty */
      {
        $$ = null;
      }
      | all_tag      
      ;      
      
opt_corresponding_spec
      : /* Empty */
      {
		$$ = null;
      }
      | corresponding_spec
      ;
      
query_term
      : simple_table
      | query_term INTERSECT opt_all_tag opt_corresponding_spec simple_table
      {
         Symbol sym = new Symbol(Tag.QueryTerm);
         notation.Confirm(sym, Descriptor.Intersect, $1, $5); 
         if ($3 != null)
           notation.Confirm(sym, Descriptor.Distinct);
         $$ = sym;
      }
      ;      

null_predicate 
      : row_value_constructor 
          IS opt_not_tag null_tag 
      {
        Symbol sym = new Symbol(Tag.Predicate);
        $$ = notation.Confirm(sym, Descriptor.IsNull, $1);
        if ($3 != null)
          notation.Confirm(sym, Descriptor.Inverse);
      }
      ;

num_primary 
      : value_exp_primary 
      | num_value_fct 
      | xml_value_func
      ;
      
num_value_fct 
      : position_exp 
      ;
      
num_value_exp 
      : term
      | num_value_exp term_op term
      {
		  Symbol sym = new Symbol(Tag.Expr);
          if ($2.Equals("+"))
			$$ = notation.Confirm(sym, Descriptor.Add, $1, $3);
		  else
		    $$ = notation.Confirm(sym, Descriptor.Sub, $1, $3);
      }
      ;

term_op 
      : '+' 
      | '-' 
      ;

ordering_spec 
      : ASC          
      | DESC         
      ;

order_by_clause 
      : ORDER BY sort_spec_list 
      {
			$$  = $3;
	  }
      ;

outer_join_type 
      : LEFT       
      | RIGHT      
      | FULL       
      ;

overlaps_predicate 
      : row_value_constructor 
           OVERLAPS row_value_constructor 
      {
         $$ = notation.Confirm(new Symbol(Tag.Predicate), Descriptor.Overlaps, $1, $3);
      }
      ;

parameter_spec 
      : parameter_name                
      | parameter_name indicator_parameter
      ;

pattern 
      : char_value_exp 
      ;

position_exp 
      : POSITION '(' char_value_exp 
           IN char_value_exp ')' 
      {
           $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.PosString, $5, $3);
       }
      ;

predicate 
      : comp_predicate 
      | between_predicate 
      | in_predicate 
      | like_predicate 
      | null_predicate 
      | quantified_comp_predicate 
      | exists_predicate 
      | unique_predicate 
      | match_predicate 
      | overlaps_predicate 
      ;

qualified_join 
	  : opt_join_type JOIN table_ref join_spec
	  {
		$$ = $3;
		if ($1 != null)
			notation.ConfirmTag(Tag.Join, Descriptor.JoinType, new TokenWrapper($1));		
		notation.ConfirmTag(Tag.Join, Descriptor.JoinSpec, $4);		
	  }
      ;
         
opt_join_type
      : /* Empty */
      {
		$$ = null;
	  }
      | join_type
      ;      
  
quantified_comp_predicate 
      : row_value_constructor comp_op 
          quantifier subquery 
      {
		  $$ = notation.Confirm(new Symbol(Tag.Predicate), 
		    Descriptor.QuantifiedPred, $1, $2, new TokenWrapper($3), $4);	  
      }
      ;

quantifier 
      : ALL                   
      | SOME                  
      | ANY                   
      ;

query_spec 
      : SELECT opt_top_quantifier opt_set_quantifier 
           select_list table_exp
      {
           if ((int)$3 == Token.DISTINCT)
              notation.ConfirmTag(Tag.SQuery, Descriptor.Distinct);      
           if ($2 != null)
              notation.ConfirmTag(Tag.SQuery, Descriptor.Top, $2);      
           $$ = notation.ResolveTag(Tag.SQuery);           
           notation.LeaveContext();
      }
      ;
      
opt_top_quantifier
      : /* null */
      {
         $$ = null;
      }      
      | TOP '(' unsigned_integer ')'
      {
         $$ = $3; 
      }
      ;      
   
result 
      : result_exp 
      | null_tag 
      ;

result_exp 
      : value_exp 
      ;


row_value_constructor 
      : row_value_constructor_elem 
      | '(' row_value_constructor_elem ',' row_value_const_list ')'  
      {
         $$ = notation.Confirm(new Symbol(Tag.RowValue), 
            Descriptor.RowValue, Lisp.Append(Lisp.Cons($2), $4));
      }
/*    | subquery  */
      ;
      
row_value_constructor_elem 
      : value_exp 
      | null_tag 
      { 
		$$ = new TokenWrapper($1);
	  }
      | default_spec 
      {
        $$ = new TokenWrapper($1);
      }
      ;

row_value_const_list 
      : row_value_constructor_elem
      {
        $$ = Lisp.Cons($1);
      }
      | row_value_const_list ',' row_value_constructor_elem
      {
        $$ = Lisp.Append($1, Lisp.Cons($3));
      }
      ;

searched_case 
      : CASE
        searched_when_clause_list
        END 
      {
         $$ = notation.Confirm(new Symbol(Tag.CaseExpr), Descriptor.Case, $2);         
      }
      | CASE
        searched_when_clause_list
        else_clause
        END 
      {
         object clause_list = Lisp.Append($2, Lisp.Cons($3));
         $$ = notation.Confirm(new Symbol(Tag.CaseExpr), Descriptor.Case, clause_list);         
      }
      ;

searched_when_clause_list
      : searched_when_clause
      {
         $$ = Lisp.Cons($1);
      }
      | searched_when_clause_list searched_when_clause
      {
         $$ = Lisp.Append($1, Lisp.Cons($2));
      }
      ;
      
searched_when_clause 
      : WHEN search_condition THEN result 
      {
         $$ = notation.Confirm(new Symbol(Tag.CaseExpr), Descriptor.CaseBranch, $2, $4);
      }
      ;

search_condition 
      : boolean_value_exp 
      ;

boolean_value_exp 
      : boolean_term
      | boolean_value_exp boolean_term_op boolean_term
      {
         $$ = notation.Confirm(new Symbol(Tag.BooleanExpr), Descriptor.LogicalOR, $1, $3);
      }
      ;

boolean_term_op 
      : OR
      ;
      
boolean_term 
      : boolean_factor
      | boolean_term boolean_factor_op boolean_factor
      {
         $$ = notation.Confirm(new Symbol(Tag.BooleanExpr), Descriptor.LogicalAND, $1, $3);
      }
      ;

boolean_factor_op 
      : AND 
      ;
      
boolean_factor 
      : boolean_test
      | not_tag boolean_test 
      {
         $$ = notation.Confirm(new Symbol(Tag.BooleanExpr), Descriptor.Inverse, $2);
      }
      ;
      
boolean_test 
      : boolean_primary 
      | boolean_primary IS opt_not_tag truth_value 
      {
         Symbol sym = new Symbol(Tag.BooleanExpr);
         $$ = notation.Confirm(sym, Descriptor.BooleanTest, new TokenWrapper($4), $1);
		 if ($3 != null)
		   notation.Confirm(sym, Descriptor.Inverse);                  
      }
      ;
     
boolean_primary 
      :  predicate 
      |  '(' search_condition ')' 
      {
         $$ = notation.Confirm(new Symbol(Tag.BooleanExpr),
           Descriptor.Branch, $2);
      }
      ;

truth_value 
      : TRUE            
      | FALSE           
      | UNKNOWN         
      ;

select_list 
      : '*'
      {
          notation.ConfirmTag(Tag.SQuery, Descriptor.Select, null); 
      }
      | select_sublist_list
      {
          notation.ConfirmTag(Tag.SQuery, Descriptor.Select, $1); 
      }
      ;
      
select_sublist_list
      : select_sublist     
      {
          $$ = Lisp.Cons($1);
      } 
      | select_sublist_list ',' select_sublist
      {
         $$ = Lisp.Append($1, Lisp.Cons($3));
      }
      ;

select_sublist 
      : qualified_name asterisk_tag
      {		
		 $$ = notation.Confirm(new Symbol(Tag.TableFields), Descriptor.TableFields, $1);   
      }
      | derived_column 
      ;

set_fct_spec 
      : count_all_fct
      {
         $$ = notation.Confirm(new Symbol(Tag.AggExpr), Descriptor.AggCount);
      }
      | general_set_fct
      | xml_agg_set_fct 
      ;

set_fct_type 
      : AVG			
      | MAX         
      | MIN         
      | SUM         
      | COUNT       
      ;

opt_set_quantifier 
      : /* Empty */
        {
           notation.EnterContext();
           $$ = Token.ALL;
        }
      | set_quantifier
        {
           notation.EnterContext();
           $$ = $1;
        }
      ;

set_quantifier 
      : DISTINCT 
      | ALL
      ;

sign 
      : '+'            
      | '-'			   
      ;

simple_case
      : CASE case_operand 
        simple_when_clause_list
        END
      {
          $$ = notation.Confirm(new Symbol(Tag.CaseExpr), Descriptor.Case, $2, $3);              
      }      
      | CASE case_operand 
        simple_when_clause_list
        else_clause 
        END
      {
         object clause_list = Lisp.Append($3, Lisp.Cons($4));
         $$ = notation.Confirm(new Symbol(Tag.CaseExpr), Descriptor.Case, $2, clause_list);
      }
      ;
      
simple_when_clause_list
      :  simple_when_clause
      {
         $$ = Lisp.Cons($1);
      }
      |  simple_when_clause_list simple_when_clause
      {
         $$ = Lisp.Append($1, Lisp.Cons($2));
      }
      ;   

simple_table 
      : query_spec 
      | table_value_constructor 
//    | explicit_table 
      ;

simple_when_clause 
      : WHEN when_operand THEN result 
      {
         $$ = notation.Confirm(new Symbol(Tag.CaseExpr), Descriptor.CaseBranch, $2, $4);
      }
      ;

sort_key 
      : column_ref 
      | unsigned_integer 
      {
			$$ = new IntegerValue($1);
      }
      ;

sort_spec 
      : sort_key          
        ordering_spec_opt
      {
        $$ = $1; 
        if ((int)$2 == Token.DESC) 
			notation.Confirm((Symbol)$1, Descriptor.Desc);
      }  
      ;  
      
ordering_spec_opt
      : /* Empty */
      {
        $$ = Token.ASC;
      } 
      | ordering_spec        
      ;      

sort_spec_list 
      : sort_spec
      {
			$$ = Lisp.Cons($1);
      }
      | sort_spec_list ','  sort_spec
      {
            $$ = Lisp.Append($1, Lisp.Cons($3));
      }
      ;

start_position 
      : num_value_exp 
      ;

string_length 
      : num_value_exp 
      ;

string_value_fct 
      : char_value_fct 
      ;

subquery 
      : '(' opt_optimizer_hint query_exp ')' 
      {
         $$ = $3;
         if ($2 != null)
           notation.Confirm((Symbol)$3, Descriptor.OptimizerHint, $2);
      }
      | '(' opt_optimizer_hint query_exp order_by_clause ')' 
      {
         $$ = $3;
         notation.Confirm((Symbol)$3, Descriptor.Order, $4);  
         if ($2 != null)
           notation.Confirm((Symbol)$3, Descriptor.OptimizerHint, $2);         
      }      
      ;
  
table_exp 
      : /* empty */  
      | from_clause 
        opt_where_clause 
        opt_group_by_clause 
        opt_having_clause 
      ;

table_ref 
    : table_ref_simple 
    | joined_table
    ;
    
table_ref_simple 
    : table_ref_spec 
    | table_ref_spec table_correlation
    {
		$$ = $1;
		notation.Confirm((Symbol)$$, Descriptor.Alias, $2);
    } 
    ;
    
table_ref_spec
    : table_name
    | dynamic_table
    | subquery
    ;   
    
dynamic_table
    : TABLE funcall
    {
        $$ = notation.Confirm(new Symbol(Tag.Dynatable), Descriptor.Dynatable, $2);
    }
    | TABLE column_ref
    {
        $$ = notation.Confirm(new Symbol(Tag.Dynatable), Descriptor.Dynatable, $2);
    }    
    | TABLE '(' value_exp ')'    
    {
        $$ = notation.Confirm(new Symbol(Tag.Dynatable), Descriptor.Dynatable, $2);
    }  
    | TABLE xml_query
    {
		$$ = notation.Confirm(new Symbol(Tag.Dynatable), Descriptor.Dynatable, $2);
    }       
    ;     
    
table_correlation
    :  opt_AS id
    {
		$$ = new Qname($2);
    }
    |  opt_AS id   
          '(' derived_column_list ')' 
    {
		$$ = new Qname($2);
		notation.Confirm((Symbol)$$, Descriptor.DerivedColumns, $4);
    }
    ;
       
table_value_constructor 
    : VALUES table_value_const_list 
    {
       $$ = notation.Confirm(new Symbol(Tag.TableConstructor), 
          Descriptor.TableValue, $2);
    }
    ;

table_value_const_list 
    : row_value_constructor
    {
      $$ = Lisp.Cons($1);
    }
    | table_value_const_list ',' row_value_constructor
    {
      $$ = Lisp.Append($1, Lisp.Cons($3));
    }
    ;

term 
    : factor
    | term '*' factor
    {
        $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Mul, $1, $3);
    }
    | term '/' factor
    {
        $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Div, $1, $3);
    }
    ;

trim_fct 
      : TRIM '(' trim_operands ')' 
      { 
        $$ = $3;
      }
      ;

trim_operands 
      : trim_source
      {
         $$ = notation.Confirm(new Symbol(Tag.CExpr), Descriptor.StringTrim, 
			new TokenWrapper(Token.BOTH), new Literal(" "), $1);
      }
      | FROM trim_source
      {
         $$ = notation.Confirm(new Symbol(Tag.CExpr), Descriptor.StringTrim, 
            new TokenWrapper(Token.BOTH), new Literal(" "), $2);
      }
      | trim_char FROM trim_source
      {
         $$ = notation.Confirm(new Symbol(Tag.CExpr), Descriptor.StringTrim, 
            new TokenWrapper(Token.BOTH), $1, $3);
      }
      | trim_spec FROM trim_source
      {
         $$ = notation.Confirm(new Symbol(Tag.CExpr), Descriptor.StringTrim, 
			new TokenWrapper($1), new Literal(" "), $3);
      }
      | trim_spec trim_char FROM trim_source      
      {
         $$ = notation.Confirm(new Symbol(Tag.CExpr), Descriptor.StringTrim, 
			new TokenWrapper($1), $2, $4);
      }
      ;

trim_char 
      : char_value_exp 
      ;

trim_source 
      : char_value_exp 
      ;

trim_spec       
      : LEADING
      | TRAILING
      | BOTH       
      ;

unique_predicate  
      : UNIQUE subquery 
      {
        $$ = notation.Confirm(new Symbol(Tag.Predicate), Descriptor.Unique, $2);
      }      
      ;

unsigned_value_spec 
      : unsigned_lit 
      | general_value_spec 
      ;

value_exp 
      : char_value_exp
      ;

value_exp_primary 
      : unsigned_value_spec 
      | set_fct_spec 
      | column_ref 
      | subquery 
      | case_exp 
      | funcall
      | prefixed_table_name
      | cast_specification
      | '(' value_exp ')'
      {
         $$ = notation.Confirm(new Symbol(Tag.Expr), Descriptor.Branch, $2);
      }
      ;
      
funcall
      : func '(' ')'
      {
         $$ = notation.Confirm(new Symbol(Tag.Funcall), Descriptor.Funcall, new Qname($1), null);
      }
      | func '(' row_value_const_list ')'
      {
         $$ = notation.Confirm(new Symbol(Tag.Funcall), Descriptor.Funcall, new Qname($1), $3);
      }     
     ;
                            
when_operand 
     : value_exp 
     ;

opt_where_clause 
    : /* Empty */
    | WHERE search_condition 
    {
        notation.ConfirmTag(Tag.SQuery, Descriptor.Where, $2);
    }
    ;
    
opt_optimizer_hint
    : /* Empty */
    {
       $$ = null;
    }
    | optimizer_hint_list
    ;
    
optimizer_hint_list
    : optimizer_hint
    {
       $$ = Lisp.Cons($1);
    }
    | optimizer_hint_list optimizer_hint
    {
       $$ = Lisp.Append($1, Lisp.Cons($2)); 
    }  
    ; 
          
column_name
    : id
    {
	   $$ = new Qname($1);
    }
    ;
    
table_name
    : prefixed_table_name      
    {
       $$ = $1;
       notation.ConfirmTag(Tag.SQuery, Descriptor.TableName, $1);
    }
    | implict_table_name
    {      
       notation.ConfirmTag(Tag.SQuery, Descriptor.TableName, $1);
    }      
    ;    
        
prefixed_table_name      
    : id ':' implict_table_name
    {
       $$ = $3;
       notation.Confirm((Symbol)$$, Descriptor.Prefix, new Literal($1));       
    }
    ;   
        
implict_table_name
    : qualified_name 
    | qualified_name '.' id
    {
       $$ = $1;
       ((Qname)$1).Append((String)$3);
    }
    ;
       
column_ref 
    : column_ref_primary 
    {
       $$ = $1;
       notation.Confirm((Symbol)$1, Descriptor.ColumnRef);
    }
    ;
        
column_ref_primary
    : qualified_name 
    | column_ref_primary '.' id
    {
       $$ = notation.Confirm(new Symbol(Tag.Dref), Descriptor.Dref, $1, new Literal((String)$3));
    }
    | column_ref_primary '.' funcall
    {       
       Notation.Record[] recs = notation.Select((Symbol)$3, Descriptor.Funcall, 2);       
       if (recs.Length > 0)
       {
          $$ = notation.Confirm(new Symbol(Tag.Funcall), Descriptor.Funcall, 
             recs[0].Arg0, Lisp.Append(Lisp.Cons($1), recs[0].args[1]));
          notation.Remove(recs);
       }
       else
          throw new InvalidOperationException();
    }
    | column_ref_primary '[' value_exp ']'
    {
       $$ = notation.Confirm(new Symbol(Tag.Dref), Descriptor.At, $1, $3);
    }
    | column_ref_primary double_slash id 
    {
       $$ = notation.Confirm(new Symbol(Tag.Dref), Descriptor.Wref, $1, new Literal((String)$3));    
    }
    ;
    
qualified_name 
    : id    
    {
      $$ = new Qname($1);
    }
	;        
                    
unsigned_lit
    : unsigned_integer
    {
      $$ = new IntegerValue($1);
    }
    | unsigned_float
    {
      $$ = new DecimalValue($1);
    }
    | unsigned_double
    {
      $$ = new DoubleValue($1);
    }
    | string_literal
    {
      $$ = new Literal($1);
    }
    ;	    

/* SQX Extension */    
xml_value_func
      : xml_pi 
      | xml_comment
      | xml_concatenation
      | xml_element
      | xml_parse
      | xml_forest
      | xml_cdata
      | xml_root
      | xml_query 
      ;
      
xml_query
      : XMLQUERY '(' string_literal xml_returning_clause ')'
      {
         $$ = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLQuery, $3, null, $4);     
      }
      | XMLQUERY '(' string_literal PASSING xml_passing_clause xml_returning_clause ')'
      {
         $$ = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLQuery, $3, $5, $6);     
      }      
      | XMLQUERY '(' string_literal PASSING BY VALUE xml_passing_clause xml_returning_clause ')'
      {
         $$ = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLQuery, $3, $7, $8);     
      }            
      ;      
      
xml_returning_clause
      : /* Empty */ 
      {
         $$ = null;
      }      
      | RETURNING_CONTENT
      {
         $$ = new TokenWrapper(Token.RETURNING_CONTENT);
      }
      | RETURNING_SEQUENCE
      {
         $$ = new TokenWrapper(Token.RETURNING_SEQUENCE);
      }
      ;     
      
xml_passing_clause
      : derived_column
      {
         $$ = Lisp.Cons($1);
      }
      | xml_passing_clause ',' derived_column
      {
         $$ = Lisp.Append($1, Lisp.Cons($3));
      }
      ;
           
xml_agg_set_fct 
      : XMLAGG '(' value_exp ')'
      {
         $$ = notation.Confirm(new Symbol(Tag.AggExpr), Descriptor.Aggregate, 
            new TokenWrapper(Token.XMLAGG), $3);
      }
      | XMLAGG '(' value_exp order_by_clause ')'
      {
         $$ = notation.Confirm(new Symbol(Tag.AggExpr), Descriptor.Aggregate, 
            new TokenWrapper(Token.XMLAGG), $3);
         notation.Confirm((Symbol)$$, Descriptor.Order, $4);
      }
      ;
      
xml_concatenation
      : XMLCONCAT '(' value_exp_list ')'
      {
         $$ = notation.Confirm(new Symbol(Tag.SQLX), 
            Descriptor.XMLConcat, $3); 
      }
      ;

xml_forest
      : xml_forest_all
      {
         $$ = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLForestAll);               
      }
      | XMLFOREST '(' content_value_list ')'
      {
         $$ = notation.Confirm(new Symbol(Tag.SQLX), 
            Descriptor.XMLForest, $3);         
      }
      | XMLFOREST '(' namespace_decl ',' content_value_list ')'    
      {
         $$ = notation.Confirm(new Symbol(Tag.SQLX), 
            Descriptor.XMLForest, $5);  
         notation.Confirm((Symbol)$$, Descriptor.XMLNamespaces, $3);
      }  
      ;      
      
xml_parse
      : XMLPARSE '(' id value_exp opt_xml_whilespace_clause ')'
      {
          String spec = $3.ToString().ToUpperInvariant();
          if (spec.Equals("DOCUMENT") || spec.Equals("CONTENT"))        
			 $$ = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLParse, new Literal(spec), $4, $5);
	      else
	      {
	         yyerror(String.Format(Properties.Resources.SyntaxError, 
	            String.Format("XMLPARSE({0}...", spec)));
	         throw new yyParser.yyException("Syntax error");
	      }      
      }
      ;      
      
xml_pi
      : XMLPI '(' id column_name ')'
      {
          String spec = (String)$3;  
          if (! spec.Equals("NAME", StringComparison.InvariantCultureIgnoreCase))
          {
	         yyerror(String.Format(Properties.Resources.SyntaxError, 
	            String.Format("XMLPI({0}...", spec)));          
             throw new yyParser.yyException("Syntax error");	      
          }
          $$ = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLPI, $4, null);
      }
      | XMLPI '(' id column_name ',' value_exp ')'
      {
          String spec = (String)$3;  
          if (! spec.Equals("NAME", StringComparison.InvariantCultureIgnoreCase))
          {
	         yyerror(String.Format(Properties.Resources.SyntaxError, 
	            String.Format("XMLPI({0}...", spec)));                    
             throw new yyParser.yyException("Syntax error");	            
          }
          $$ = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLPI, $4, $6);      
      }
      ;     
      
xml_comment
      : XMLCOMMENT '('  value_exp  ')'
      {
         $$ = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLComment, $3);
      }
      ;
      
xml_cdata
      : XMLCDATA '(' value_exp ')'
      {
         $$ = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLCDATA, $3);
      }
      ;
      
xml_root
      : XMLROOT '(' value_exp ')'
      {
         $$ = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLRoot, $3, null, null);
      }
      | XMLROOT '(' value_exp ',' id version_expr ')'
      {
         String tok = (String)$5;
         if (! tok.Equals("VERSION", StringComparison.InvariantCultureIgnoreCase))
         {
	         yyerror(String.Format(Properties.Resources.SyntaxError, 
	            String.Format("XMLRoot(... {0} ...", tok)));                             
              throw new yyParser.yyException("Syntax error");	                     
         }
         $$ = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLRoot, $3, new Literal($6), null);
      }
      | XMLROOT '(' value_exp ',' id version_expr ',' id standalone_expr ')'
      {
         String tok = (String)$5;
         if (! tok.Equals("VERSION", StringComparison.InvariantCultureIgnoreCase))
         {
	         yyerror(String.Format(Properties.Resources.SyntaxError, 
	            String.Format("XMLRoot(... {0} ...", tok)));                             
              throw new yyParser.yyException("Syntax error");	                     
         }
         tok = (String)$8;
         if (! tok.Equals("STANDALONE", StringComparison.InvariantCultureIgnoreCase))
         {
	         yyerror(String.Format(Properties.Resources.SyntaxError, 
	            String.Format("XMLRoot(... {0} ...", tok)));                             
              throw new yyParser.yyException("Syntax error");	                     
         }
         $$ = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLRoot, $3, new Literal($6), $9);
      }      
      ;
      
version_expr
      : string_literal
      | NO_VALUE
      {
         $$ = null;
      }
      ;
      
standalone_expr
      : id
      {
         String tok = (String)$1;
         if (tok.Equals("NO", StringComparison.InvariantCultureIgnoreCase) ||
             tok.Equals("YES", StringComparison.InvariantCultureIgnoreCase)) 
			$$ = new Qname(tok.ToUpperInvariant());
         else
            throw new yyParser.yyException(String.Format(Properties.Resources.SyntaxError, 
               "STANDALONE value must be YES|NO|NO VALUE"));			
      }
      | NO_VALUE
      {
         $$ = null;
      }            
      ;       
      
xml_element
      : XMLELEMENT '(' column_name  ')'
      {
         $$ = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLElement, $3, null);
      }
      | XMLELEMENT '(' column_name  ',' namespace_decl ')'
      {
         $$ = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLElement, $3, null);
         notation.Confirm((Symbol)$$, Descriptor.XMLNamespaces, $5);
      }
      | XMLELEMENT '(' column_name  ',' xml_attributes ')'
      {
         $$ = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLElement, $3, null);
         notation.Confirm((Symbol)$$, Descriptor.XMLAttributes, $5);
      }
      | XMLELEMENT '(' column_name  ',' namespace_decl ','
           xml_attributes ')'
      {
         $$ = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLElement, $3, null);
         notation.Confirm((Symbol)$$, Descriptor.XMLNamespaces, $5);
         notation.Confirm((Symbol)$$, Descriptor.XMLAttributes, $7);
      }
      | XMLELEMENT '(' column_name ',' value_exp_list ')'
      {
         $$ = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLElement, $3, $5);         
      }
      | XMLELEMENT '(' column_name ',' namespace_decl ',' value_exp_list ')'
      {
         $$ = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLElement, $3, $7);
         notation.Confirm((Symbol)$$, Descriptor.XMLNamespaces, $5);
      }
      | XMLELEMENT '(' column_name ',' xml_attributes ',' value_exp_list ')'
      {
         $$ = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLElement, $3, $7);
         notation.Confirm((Symbol)$$, Descriptor.XMLAttributes, $5);
      }
      | XMLELEMENT '(' column_name ',' namespace_decl ',' 
           xml_attributes ',' value_exp_list ')'           
      {
         $$ = notation.Confirm(new Symbol(Tag.SQLX), Descriptor.XMLElement, $3, $9);
         notation.Confirm((Symbol)$$, Descriptor.XMLNamespaces, $5);
         notation.Confirm((Symbol)$$, Descriptor.XMLAttributes, $7);      
      }
      ;
                       
namespace_decl      
      : XMLNAMESPACES '(' namespace_decl_list ')' 
      {
         $$ = $3;
      }           
      ;
            
namespace_decl_list
      : namespace_decl_item
      {
         $$ = Lisp.Cons($1);
      }
      | namespace_decl_list ',' namespace_decl_item
      {
         $$ = Lisp.Append($1, Lisp.Cons($3));
      }
      ;      
      
namespace_decl_item
      : regular_decl_item
      | default_decl_item
      ;
      
regular_decl_item
      : string_literal opt_AS column_name
      {
         $$ = notation.Confirm(new Symbol(Tag.SQLX), 
            Descriptor.DeclNamespace, new Literal($1), $3);  
      }
      ;
      
default_decl_item
      : DEFAULT string_literal
      {
         $$ = notation.Confirm(new Symbol(Tag.SQLX), 
            Descriptor.DeclNamespace, new Literal($2), null);        
      }
      | NO_DEFAULT
      {
         $$ = null;
      }
      ;      
                       
xml_attributes
      : xml_attributes_all
      {
         $$ = null;
      }
      | XMLATTRIBUTES '(' content_value_list ')'
      {
         $$ = $3;
      }
      ;      
      
content_value_list
      : content_value
      {
          $$ = Lisp.Cons($1);
      }
      | content_value_list ',' content_value
      {
          $$ = Lisp.Append($1, Lisp.Cons($3));
      }
      ;
      
content_value
      : named_value
      | named_value xml_content_option
      {
         $$ = $1;
         notation.Confirm((Symbol)$$, Descriptor.ContentOption, $2);
      }
      | qualified_name asterisk_tag
      {
         $$ = notation.Confirm(new Symbol(Tag.TableFields), Descriptor.TableFields, $1);   
      }
      | qualified_name asterisk_tag xml_content_option
      {
         $$ = notation.Confirm(new Symbol(Tag.TableFields), Descriptor.TableFields, $1);   
         notation.Confirm((Symbol)$$, Descriptor.ContentOption, $3);
      }      
      ;
      
named_value
      : value_exp 
      | value_exp as_clause
      {
         $$ = $1;
         notation.Confirm((Symbol)$$, Descriptor.Alias, $2);
      }
      ;                                         
                      
opt_xml_whilespace_clause
      : /* nothing */
      { 
         $$ = null;
      } 
      | xml_whilespace_clause
      ; 
      
xml_whilespace_clause
      : PRESERVE_WHITESPACE
      {
          $$ = new TokenWrapper(Token.PRESERVE_WHITESPACE);
      }
      | STRIP_WHITESPACE
      {
          $$ = new TokenWrapper(Token.STRIP_WHITESPACE);
      }
      ;
          
xml_content_option
      : OPTION_NULL opt_ON_NULL
      {
         $$ = new TokenWrapper(Token.OPTION_NULL);
      }
      | OPTION_EMPTY opt_ON_NULL
      {
         $$ = new TokenWrapper(Token.OPTION_EMPTY);
      }      
      | OPTION_ABSENT opt_ON_NULL
      {
         $$ = new TokenWrapper(Token.OPTION_ABSENT);
      }            
      | OPTION_NIL opt_ON_NULL
      {
         $$ = new TokenWrapper(Token.OPTION_NIL);
      }
      | OPTION_NIL ON NO_CONTENT
      {
         $$ = new TokenWrapper(Token.NO_CONTENT);
      }      
      ;
      
opt_ON_NULL
      : /* empty */
      | ON NULL
      ;         
      
cast_specification
      : CAST '(' cast_operand AS data_type ')'
      {
         $$ = notation.Confirm(new Symbol(Tag.Expr), 
            Descriptor.Cast, $3, $5);                 
      }
      ;
      
cast_operand
      : NULL
      {
         $$ = null;
      }
      | value_exp      
      ;
      
data_type
      : string_type
      | numeric_type
      | DATE
      {
         $$ = new TokenWrapper($1);
      }
      | domain_name
      ;
      
string_type
      : string_type_name
      {
         $$ = new TokenWrapper($1);
      }
      | string_type_name '(' unsigned_integer ')'
      {
         $$ = new TokenWrapper($1);
         notation.Confirm((Symbol)$$, Descriptor.Typelen, $3);
      }      
      ;     
      
string_type_name
      : CHARACTER
      | CHAR
      | CHARACTER_VARYING 
      | CHAR_VARYING 
      | VARCHAR
      ;       
      
numeric_type
      : decimal_type
      | float_type
      | other_numeric_type
      {
         $$ = new TokenWrapper($1);
      }
      ;
      
other_numeric_type      
      : INTEGER
      | INT
      | SMALLINT
      | REAL
      | DOUBLE_PRECISION
      ;     
      
decimal_type
      : decimal_type_name
      {
         $$ = new TokenWrapper($1);
      }
      | decimal_type_name '(' unsigned_integer ')'
      {
         $$ = new TokenWrapper($1);
         notation.Confirm((Symbol)$$, Descriptor.Typeprec, $3);         
      }      
      | decimal_type_name '(' unsigned_integer ',' unsigned_integer ')'      
      {
         $$ = new TokenWrapper($1);
         notation.Confirm((Symbol)$$, Descriptor.Typeprec, $3);
         notation.Confirm((Symbol)$$, Descriptor.Typescale, $5);         
      }      
      ;
      
decimal_type_name
      : NUMERIC
      | DECIMAL
      | DEC
      ;      
      
float_type
      : FLOAT
      {
         $$ = new TokenWrapper(Token.FLOAT);
      }      
      | FLOAT '(' unsigned_integer ')'
      {
         $$ = new TokenWrapper(Token.FLOAT);
         notation.Confirm((Symbol)$$, Descriptor.Typeprec, $3);         
      }            
      ;   
      
domain_name
      : id
      {
         $$ = new Qname($1);
      }
      ;          
     	
%%
}
    
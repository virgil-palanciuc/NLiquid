/********************************************************************************
Copyright (c) 2012 Adobe Systems Incorporated

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
********************************************************************************/

%using NLiquid.Runtime;

%namespace NLiquid.Analyzing

%{
	public StatementList program;
	private StatementFactory codegen;
	private Stack<IStatement> currentStatement;
%}

%start program

%union {
    public long Integer;
    public string String;
	public StringBuilder StringBuilder;
    public double Double;
	public bool Bool;
	public Expression expr;
	public StatementList statementList;
	public IStatement  statement;
}
// Defining Tokens
%token COMMENT
%token <String>	 IDENTIFIER
%token <Double>	 NUM_LITERAL
%token <Bool>	 BOOL_LITERAL
%token <String>	 STRING_LITERAL
%token <String>	 PLAIN

%token TAG_START         
%token TAG_END
%token OUTPUT_START         
%token OUTPUT_END

%token CAPTURE         
%token END_CAPTURE      
%token RAW				
%token END_RAW          
%token IF				
%token END_IF           
%token UNLESS			
%token END_UNLESS       
%token ELSE			
%token CYCLE			
%token WHEN			
%token IN				
%token AND				
%token OR				
%token TIMEOUT			
%token ASSIGN			
%token WITH			
%token INCLUDE         
%token NIL_LITERAL				
%token CASE            
%token END_CASE         
%token FOR				
%token END_FOR			
%token TABLE		
%token END_TABLE		



%token RIGHT_PAR
%token LEFT_PAR
%left OP_ASSIGN
%right DOT

%token COL
%token DOT_DOT
%token PIPE

%left COMMA
%left OP_AND
%left OP_OR
%left OP_NOT
%left OP_LT OP_GT OP_LE OP_GE OP_EQ OP_NE


// YACC Rules
%%
program			:	statementList {program = $1.statementList;}
				;

statementList	:	/*Empty*/	{
                                    if($$.statementList == null) $$.statementList = codegen.NewList();
								}

				|	statement	{	
				                   if($$.statementList == null)  $$.statementList = codegen.NewList();
								   if($1.statement != null)      $$.statementList.InsertFront($1.statement);
								}
								
				|	statementList statement	
				                { 
				                   if($2.statement != null) $1.statementList.Add($2.statement); 
								   $$.statementList = $1.statementList; 
								}
				;
			

statement	:	plainInst	{ $$.statement = $1.statement; }
            |   assignInst  { $$.statement = $1.statement; }
			|   forInst		{ $$.statement = $1.statement; }
			|   ifInst		{ $$.statement = $1.statement; }
			|   caseInst	{ $$.statement = $1.statement; }
			|   rawInst		{ $$.statement = $1.statement; }
			|   unlessInst	{ $$.statement = $1.statement; }
			|   tableInst	{ $$.statement = $1.statement; }
			|   captureInst	{ $$.statement = $1.statement; }
			|   includeInst	{ $$.statement = $1.statement; }
			|   timeoutInst	{ $$.statement = $1.statement; }
			|   cycleInst	{ $$.statement = $1.statement; }
			|   commentInst { $$.statement = $1.statement; }
			|   outputInst  { $$.statement = $1.statement; }
			;


expr		:	expr AND term			    { $$.expr = codegen.NewExpression(Operation.And,$1.expr,$3.expr); }		
			|	expr OR  term			    { $$.expr = codegen.NewExpression(Operation.Or,$1.expr,$3.expr); }		
			|   term                        { $$.expr = $1.expr; }
			;

term		:	fact OP_EQ  fact			{ $$.expr = codegen.NewExpression(Operation.Equ,$1.expr,$3.expr); }
			|	fact OP_NE  fact	     	{ $$.expr = codegen.NewExpression(Operation.NotEqu,$1.expr,$3.expr); }
			|	fact OP_LT  fact			{ $$.expr = codegen.NewExpression(Operation.Lt,$1.expr,$3.expr); }		
			|	fact OP_GT  fact			{ $$.expr = codegen.NewExpression(Operation.Gt,$1.expr,$3.expr); }		
			|	fact OP_GE  fact			{ $$.expr = codegen.NewExpression(Operation.GtEq,$1.expr,$3.expr); }	
			|	fact OP_LE  fact			{ $$.expr = codegen.NewExpression(Operation.LtEq,$1.expr,$3.expr); }	
			|   fact                        { $$.expr = $1.expr; }
			;

fact		:	LEFT_PAR expr RIGHT_PAR		{ $$.expr = $2.expr; }
     		|	STRING_LITERAL	            { $$.expr = codegen.NewExpression($1);}
			|	BOOL_LITERAL	            { $$.expr = codegen.NewExpression($1);}
			|	NUM_LITERAL                 { $$.expr = codegen.NewExpression($1);}
			|   NIL_LITERAL		            { $$.expr = codegen.NewExpression();}
			|   lookup                      { $$.expr = $1.expr; }
			;

lookup      :   lookup DOT IDENTIFIER       { $$.expr = codegen.NewExpression(Operation.Lookup, $1.expr, $3); }
			|	IDENTIFIER					{ $$.expr = codegen.NewExpression(Operation.Lookup, $1);}
			;


assignInst  :  TAG_START ASSIGN IDENTIFIER OP_ASSIGN expr TAG_END { $$.statement = codegen.NewAssignment($3, $5.expr);}
			;

commentInst : TAG_START COMMENT TAG_END    { $$.statement = null; } // will be ignored
            ;
plainInst   : PLAIN                        { $$.statement = codegen.Write($1); }
            ;
rawInst     : TAG_START RAW stringList END_RAW TAG_END { $$.statement = codegen.Write($3.StringBuilder.ToString() ); }
            ;
stringList  : /*Empty*/                    { $$.StringBuilder = new StringBuilder(); }
            | stringList PLAIN             { $1.StringBuilder.Append($2); $$.StringBuilder = $1.StringBuilder; }
			;


forInst     : TAG_START FOR IDENTIFIER IN lookup attributes TAG_END statementList TAG_START END_FOR TAG_END 
                                           { $$.statement = codegen.NewForLoop( $3, $5.expr, $6.statementList, $8.statementList);}
            | TAG_START FOR IDENTIFIER IN LEFT_PAR expr DOT_DOT expr RIGHT_PAR attributes TAG_END statementList TAG_START END_FOR TAG_END 
			                               { $$.statement = codegen.NewForLoop( $3, codegen.NewExpression(Operation.Range, $6.expr, $8.expr), $10.statementList, $12.statementList);}
		    ;

ifInst      : TAG_START IF expr TAG_END statementList TAG_START {  currentStatement.Push(codegen.NewIf($3.expr, $5.statementList)); }
                  optionalElse END_IF TAG_END   			    { $$.statement = currentStatement.Pop(); }
			;
optionalElse: /* empty */                  
            | ELSE TAG_END statementList TAG_START
			                               { codegen.AddElseClause(currentStatement.Peek(), $3.statementList); }
		    ;

caseInst    : TAG_START CASE expr TAG_END  { currentStatement.Push(codegen.NewCase($3.expr)); } 
                whenList TAG_START optionalElse END_CASE TAG_END
										   { $$.statement = currentStatement.Pop(); } 
			;
when        :  TAG_START WHEN expr TAG_END statementList   { codegen.AddWhenClause(currentStatement.Peek(), $3.expr, $5.statementList);}
		    ;
whenList    : whenList when
            | when                    
			;
unlessInst  : TAG_START UNLESS expr TAG_END statementList TAG_START {  currentStatement.Push(codegen.NewUnless($3.expr, $5.statementList)); }
				optionalElse END_UNLESS TAG_END						{ $$.statement = currentStatement.Pop(); }
			;
tableInst   : TAG_START TABLE IDENTIFIER IN lookup attributes TAG_END statementList TAG_START END_TABLE TAG_END 
                                           { $$.statement = codegen.NewTableRow( $3, $5.expr, $6.statementList, $8.statementList);}
			;

captureInst : TAG_START CAPTURE IDENTIFIER TAG_END statementList TAG_START END_CAPTURE TAG_END
										   {
											   $$.statement = codegen.NewCapture( $3, $5.statementList);
										   }
			;
includeInst :  TAG_START INCLUDE STRING_LITERAL optionalWith TAG_END { $$.statement = codegen.Include($3, $4.expr);}
			;
optionalWith: /* nothing */                { $$.expr = null; }
            | WITH expr                    { $$.expr = $2.expr; }
			;

timeoutInst :  TAG_START TIMEOUT NUM_LITERAL TAG_END { $$.statement = codegen.NewTimeout($3);}
			;
cycleInst   :  TAG_START CYCLE cycleGroup exprList TAG_END { $$.statement = codegen.NewCycle($3.String, $4.statementList);}
			;
cycleGroup  : STRING_LITERAL COL           { $$.String = $1; }
            | /* nothing */                { $$.String = "";        }
			;

exprList    : exprList COMMA expr          { $$.statementList = $1.statementList; $$.statementList.Add($3.expr); }
            | expr						   { $$.statementList = codegen.NewList(); $$.statementList.Add($1.expr); }
			;
attributes  : attributes attribute         { $$.statementList = $1.statementList; $$.statementList.Add($2.statement); }
            | /* nothing */                { $$.statementList = codegen.NewList(); } 
			;

attribute   : IDENTIFIER COL expr          { $$.statement = codegen.NewAttribute($1, $3.expr); }
            ; 

outputInst  : OUTPUT_START expr filterList OUTPUT_END
                                           { $$.statement = codegen.Write($2.expr, $3.statementList); }
			;
filterList  : filterList filter            { $$.statementList = $1.statementList; $$.statementList.Add($2.statement); }
            | /* nothing */                { $$.statementList = codegen.NewList(); }
			;
filter      : PIPE IDENTIFIER OptionalParams    { $$.statement = codegen.NewFilter($2, $3.statementList); }
            ;
OptionalParams
            : COL exprList                 { $$.statementList = $2.statementList; }
            | /* nothing */                { $$.statementList = null; }
			;
%%

// No argument CTOR. By deafult Parser's ctor requires scanner as param.
public Parser(Scanner scn, StatementFactory codegen) : base(scn) { this.codegen = codegen; program = codegen.NewList(); currentStatement = new Stack<IStatement>(); }
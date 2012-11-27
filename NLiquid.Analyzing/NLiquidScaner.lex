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

%using NLiquid.RunTime;

%namespace NLiquid.Analyzing

%option stack, minimize, parser, verbose, persistbuffer, unicode, compressNext, embedbuffers

%{
public override void yyerror(string format, params object[] args) 
{
	System.Console.Error.WriteLine("Error: line {0} - " + format, yyline);
}
%}
D               [0-9]
//Literals
Identifier		([a-zA-Z_]([a-zA-Z0-9_])*)
IntegerLiteral	{D}+
DoubleLiteral	{D}+(\.{D}+)?
BooleanLiteral	(true|false)
SStringLiteral	\"[^"]*\"
DStringLiteral	\'[^']*\'

DotDot  \.\.
Dot     \.
NEq     [!][=]
Eq      [=][=]
EqSign  [=]
GtEq    [>][=]
Gt      [>]
LtEq    [<][=]
Lt      [<]
Pipe    [|]
Col     [:]
Comma   [,]
OPar    [(]
CPar    [)]
Num     {DoubleLiteral}
Str     {SStringLiteral}|{DStringLiteral}


// Anyting that is not '{', followed a (possibly empty) repeating sequence of '{' immediately followed by something that is not '%' or '{'
AnyTextUpToTagOrOutputStart   [^{]*([{][^{%]+)*
AnyTextUpToTagStart           [^{]*([{][^%]+)*

//Spaces and End of Line
WhiteSpace		[ \t\r\n]+
OptionalWhiteSpace [ \t\r\n]*

TagStart		"{%"
TagEnd  		"%}"
OutputStart		"{{"
OutputEnd  		"}}"
CAPTURE         "capture"
ENDCAPTURE      "endcapture"
COMMENT		    "comment"
ENDCOMMENT      "endcomment"
RAW				"raw"
ENDRAW          "endraw"
IF				"if"
ENDIF           "endif"
UNLESS			"unless"
ENDUNLESS       "endunless"
ELSE			"else"
CYCLE			"cycle"
WHEN			"when"
IN				"in"
AND				"and"
OR				"or"
TIMEOUT			"timeout"
ASSIGN			"assign"
WITH			"with"
INCLUDE         "include"
NIL				"nil"
CASE            "case"
ENDCASE         "endcase"
FOR				"for"
ENDFOR			"endfor"
TABLEROW		"tablerow"
ENDTABLEROW		"endtablerow"

// The states into which this FSA can pass.
%x CMMT		// Inside a comment.
%x CMMT2		// Inside a comment.
%x CMMT3		// Inside a comment.

%x RAW		// Inside a 'raw' tag.
%x RAW2		// Inside a 'raw' tag.
%x RAW3		// Inside a comment.

%x INTAG
%x INOUTPUT


%%

//
// Start of Rules
//

<INITIAL>{
    {AnyTextUpToTagOrOutputStart}  { yylval.String = yytext; return (int) Tokens.PLAIN; }
    {TagStart}                     { BEGIN(INTAG); return (int) Tokens.TAG_START; }
    {OutputStart}	               { yy_push_state(INOUTPUT); return (int) Tokens.OUTPUT_START; }
}


<INOUTPUT>{
    {OutputEnd}		 { yy_pop_state(); return (int) Tokens.OUTPUT_END; }
    {WhiteSpace}+	 { ; }
	{Pipe}		     { return (int) Tokens.PIPE; }
	{Col}		     { return (int) Tokens.COL; }
	{Comma}		     { return (int) Tokens.COMMA; }
	{Dot}		     { return (int) Tokens.DOT; }
	{Str}			 {
 					   if (yytext.Length > 2) 
 					       yylval.String = yytext.Substring(1, yytext.Length - 2);
 					   else
 					       yylval.String = ""; 
 					   return (int) Tokens.STRING_LITERAL; 
 					 }
 
    {BooleanLiteral} {
                       bool.TryParse(yytext, out yylval.Bool);
 					   return (int) Tokens.BOOL_LITERAL; 
  					 }

	{Identifier}	 {
	                   yylval.String = yytext;
					   return (int) Tokens.IDENTIFIER; 
					 }

	{Num}			 {
	                   double.TryParse (yytext, NumberStyles.Float, CultureInfo.CurrentCulture, out yylval.Double); 
					   return (int) Tokens.NUM_LITERAL; 
					 }

}


<INTAG>{
    {WhiteSpace}+	 { ; }
    {COMMENT}        { BEGIN(CMMT); }
	{TagEnd}         { BEGIN(INITIAL); return (int) Tokens.TAG_END; }
	{CAPTURE}        { return (int) Tokens.CAPTURE; }  
	{ENDCAPTURE}     { return (int) Tokens.END_CAPTURE; }
	{RAW}		     { BEGIN(RAW); return (int) Tokens.END_RAW; }
	{IF}	         { return (int) Tokens.IF; }
	{ENDIF}      	 { return (int) Tokens.END_IF; }
	{UNLESS}         { return (int) Tokens.UNLESS; }
	{ENDUNLESS}		 { return (int) Tokens.END_UNLESS; }
	{CASE}           { return (int) Tokens.CASE; }
	{ENDCASE} 		 { return (int) Tokens.END_CASE; }
	{FOR}            { return (int) Tokens.FOR; }
	{ENDFOR}		 { return (int) Tokens.END_FOR; }
	{TABLEROW}       { return (int) Tokens.TABLE; }
	{ENDTABLEROW}	 { return (int) Tokens.END_TABLE; }
					 		  
	{ELSE}	         { return (int) Tokens.ELSE; }
	{CYCLE}		     { return (int) Tokens.CYCLE; }
	{WHEN}		     { return (int) Tokens.WHEN; }
	{IN}		     { return (int) Tokens.IN; }
	{AND}			 { return (int) Tokens.AND; }
	{OR}	         { return (int) Tokens.OR; }
	{ASSIGN}		 { return (int) Tokens.ASSIGN; }
	{WITH}	         { return (int) Tokens.WITH; }
	{INCLUDE}        { return (int) Tokens.INCLUDE; }
	{NIL}		     { return (int) Tokens.NIL_LITERAL; }
	{TIMEOUT}		 { return (int) Tokens.TIMEOUT; }
	{Str}			 {
 					   if (yytext.Length > 2) 
 					       yylval.String = yytext.Substring(1, yytext.Length - 2);
 					   else
 					       yylval.String = ""; 
 					   return (int) Tokens.STRING_LITERAL; 
 					 }
 
    {BooleanLiteral} {
                       bool.TryParse(yytext, out yylval.Bool);
 					   return (int) Tokens.BOOL_LITERAL; 
  					 }

	{Identifier}	 {
	                   yylval.String = yytext;
					   return (int) Tokens.IDENTIFIER; 
					 }

	{Num}			 {
	                   double.TryParse (yytext, NumberStyles.Float, CultureInfo.CurrentCulture, out yylval.Double); 
					   return (int) Tokens.NUM_LITERAL; 
					 }

	{DotDot}	     { return (int) Tokens.DOT_DOT; }
	{Dot}		     { return (int) Tokens.DOT; }
	{NEq}		     { return (int) Tokens.OP_NE; }
	{Eq}		     { return (int) Tokens.OP_EQ; }
	{GtEq}		     { return (int) Tokens.OP_GE; }
	{Gt}		     { return (int) Tokens.OP_GT; }
	{LtEq}		     { return (int) Tokens.OP_LE; }
	{Lt}		     { return (int) Tokens.OP_LT; }
	{Col}		     { return (int) Tokens.COL; }
	{Comma}		     { return (int) Tokens.COMMA; }
	{OPar}		     { return (int) Tokens.LEFT_PAR; }
	{CPar}		     { return (int) Tokens.RIGHT_PAR; }
	{EqSign}	     { return (int) Tokens.OP_ASSIGN; }
}

// remove comments at lexer level; kinda' complex, but should be fast.
<CMMT>{
    {AnyTextUpToTagStart} { ; } // just ignore anything
	{TagStart}    { BEGIN(CMMT2); }
}
<CMMT2>{
    {WhiteSpace} { ; } // ignore whitespace
	{ENDCOMMENT} { BEGIN(INTAG); return (int) Tokens.COMMENT; }
}

<RAW>{
    {WhiteSpace} { ; } // ignore whitespace
	{TagEnd}    { BEGIN(RAW2);}
}

<RAW2>{
    {AnyTextUpToTagStart} { yylval.String = yytext; return (int)Tokens.PLAIN; }
	{TagStart}    { BEGIN(RAW3); }
}
<RAW3>{
    {OptionalWhiteSpace}{ENDRAW} { BEGIN(INTAG); return (int)Tokens.END_RAW; }
	
	{OptionalWhiteSpace}         { yylval.String = "<%"+yytext; BEGIN(RAW2); return (int)Tokens.PLAIN; }
}

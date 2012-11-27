NLiquid - liquid implementation in .NET
=======================================

There are 3 parts in this solution:

+ Analyzer: contains the parser & lexer. Written for GPPG/GPLEX, which are used to generate a LALR(1) parser from a yacc-like grammar file & a lex-like scanner description file.
+ Runtime:  The actual classes that describe the Liquid "program", and are used by the scanner in order to build the AST.
+ Interpreter: Runs over the AST and interprets it (executes it). Note: we could do a translator from tree to linear IR, and interpret the LIR itself. But for now, we'll leave that as an exercise to the reader.

In addition, there are 2 more projects:
+ Console: command-line liquid interpreter (demo app)
+ WebService: WCF-based demo web service that serves pages with liquid content 
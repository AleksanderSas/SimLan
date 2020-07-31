grammar SimLan;


/*
 * Parser Rules
 */
program: (function | structDefinition)+ EOF; 
 
block: LBRAC e1+=expression* RBRAC | e2=expression;
loopControll: BREAK SEMICOLON | CONTINUE SEMICOLON;
expression: if_statement | for_statement | while_statement | assignment | return_statement | loopControll;

args: LPAR (a1=logical_statement_1 (COLON a2+=logical_statement_1)* )? RPAR;
args_def: LPAR (a1=ID (COLON a2+=ID)* )? RPAR;

function: ID args_def block;

if_statement: IF LPAR logical_statement_1 RPAR b1=block (ELSE b2=block)?;
for_statement: FOR LPAR a1=assignment logical_statement_1 SEMICOLON a2=assignment RPAR block;
while_statement: WHILE LPAR logical_statement_1 RPAR block;
assignment: ((VAR ID) | simpleValue) ASSIGN logical_statement_1 SEMICOLON;
return_statement: RETURN logical_statement_1 SEMICOLON;

structDefinition: DEF name=ID LBRAC (VAR id+=ID array? SEMICOLON)+ RBRAC;

//-------------------------
//      CALCULATIONS
//-------------------------

//this is common pattern to get rid of left recursion
logical_statement_1: logical_statement_2 logical_statement_1_2;
logical_statement_1_2: OR logical_statement_2 logical_statement_1_2 | ;

logical_statement_2: logical_value logical_statement_2_2;
logical_statement_2_2: AND logical_value logical_statement_2_2 | ;
logical_value: v1=arthmetic_statement_1 (CMP v2=arthmetic_statement_1)?;

arthmetic_statement_1: arthmetic_statement_2 arthmetic_statement_1_2;
arthmetic_statement_1_2: OPERATOR_1 arthmetic_statement_2 arthmetic_statement_1_2 | ;

arthmetic_statement_2: arthmetic_value arthmetic_statement_2_2;
arthmetic_statement_2_2: OPERATOR_2 arthmetic_value arthmetic_statement_2_2 | ;
arthmetic_value: simpleValue | LPAR logical_statement_1 RPAR;

array: LSQR_BRAC logical_statement_1 RSQR_BRAC;
referenceValueResolver: array | DOT ID;
simpleValue: OPERATOR_1? NUM | CHAR | STR | ID args* referenceValueResolver* | NEW (a2 = array | ID);

/*
 * Lexer Rules
 */

IF: 'if';
ELSE: 'else';
FOR: 'for';
WHILE: 'while';
RETURN: 'return';
BREAK: 'break';
CONTINUE: 'continue';
NEW: 'new';
VAR: 'var';
DEF: 'def';

SEMICOLON: ';';
COLON: ',';
DOT: '.';

OPERATOR_1: [+-];
OPERATOR_2: [/\\*];
CMP: [<>] | '<=' | '>=' | '<>' | '==' ;
ASSIGN: '=';
OR: '||';
AND: '&&';

LPAR: '(';
RPAR: ')';
LBRAC: '{';
RBRAC: '}';
LSQR_BRAC: '[';
RSQR_BRAC: ']';

NUM: [0-9]+;
STR: '"' ~('\r' | '\n' | '"')* '"' | '\'' ~('\'' | '\r' | '\n' | '\'')* '\'';
CHAR: '`' ~[`] '`';
ID: [_a-zA-Z][_a-zA-Z0-9]*;

COMMENT: '//' ~('\r' | '\n')* -> skip;
WS: [ \t]+ -> skip;
NL: [\r\n]+ -> skip;
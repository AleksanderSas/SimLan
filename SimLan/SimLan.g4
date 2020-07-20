grammar SimLan;


/*
 * Parser Rules
 */
program: function+ EOF; 
 
block: LBRAC e1+=expression* RBRAC | e2=expression;
expression: if_statement | for_statement | assignment | return_statement;

args: LPAR (a1=logical_statement_1 (COLON a2+=logical_statement_1)* )? RPAR;
args_def: LPAR (a1=ID (COLON a2+=ID)* )? RPAR;

function: ID args_def block;

if_statement: IF LPAR logical_statement_1 RPAR b1=block (ELSE b2=block)?;
for_statement: FOR LPAR a1=assignment logical_statement_1 SEMICOLON a2=assignment RPAR block;
assignment: ((VAR ID) | ID array*) ASSIGN logical_statement_1 SEMICOLON;
return_statement: RETURN logical_statement_1 SEMICOLON;

//-------------------------
//      CALCULATIONS
//-------------------------

//this is common pattern to get rid of left recursion
logical_statement_1: logical_statement_2 logical_statement_1_2;
logical_statement_1_2: OR logical_statement_2 logical_statement_1_2 | ;

logical_statement_2: logical_value logical_statement_2_2;
logical_statement_2_2: AND logical_value logical_statement_2_2 | ;
logical_value: v1=arthmetic_statement_1 (CMP v2=arthmetic_statement_1)? | LPAR logical_statement_1 RPAR;

arthmetic_statement_1: arthmetic_statement_2 arthmetic_statement_1_2;
arthmetic_statement_1_2: OPERATOR_1 arthmetic_statement_2 arthmetic_statement_1_2 | ;

arthmetic_statement_2: arthmetic_value arthmetic_statement_2_2;
arthmetic_statement_2_2: OPERATOR_2 arthmetic_value arthmetic_statement_2_2 | ;
arthmetic_value: simpleValue | LPAR logical_statement_1 RPAR;

array: LSQR_BRAC logical_statement_1 RSQR_BRAC;
simpleValue: NUM | ID (args+ | a1 += array+ | ) | NEW a2 = array | STR;

/*
 * Lexer Rules
 */

IF: 'if';
ELSE: 'else';
FOR: 'for';
RETURN: 'return';
NEW: 'new';
VAR: 'var';

SEMICOLON: ';';
COLON: ',';

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
ID: [_a-zA-Z][_a-zA-Z0-9]*;

WS: [ \t]+ -> skip;
NL: [\r\n]+ -> skip;

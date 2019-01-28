\ put string literal to Dictionary - nothing else
\ string," some string to literalize"
\ :  string," $22 parse dup IF string, ELSE 2drop THEN ;

\ just chars, you have to count bytes yourself
: mult-c,  ( c-addr len -- ) here swap dup allot move ;
: mult-c," $22 parse dup IF mult-c, ELSE 2drop THEN ;

\ with leading length byte
: string," $22 parse dup IF string, ELSE 2drop THEN ;

\ put literal bytes to Dictionary, depnding on current base
\ bytes," 00 11 AA BB 00 FF "
\ token           ( -- c-addr len ) Cuts one token out of input buffer
: bytes," BEGIN token over c@ $22 <>  WHILE  number 1 <> IF  quit THEN c,  REPEAT 2drop  ; 

\ don't forget to call calign afterwards


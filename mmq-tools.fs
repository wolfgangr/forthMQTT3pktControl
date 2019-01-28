: 460800baud 460800 baud USART1-BRR h! ;

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


\ align does not seem to work as intended???
\ try my own ... aligned seems to do the job...

: myalign here dup aligned swap - allot ; 
: align myalign ;

\ crude hack to display strings
: memstr-show ( addr -- ) dup cr hex. dup @ hex. cr $40 dump ; 


\ my favourite string format: 
\ 1 full cell length, then the bytes - no pad or delim, 
\ maybe alginments, but for that we don't care
\ convert to counted string stack format
: memstr-counted ( addr -- c-addr length ) dup 1 cells + swap @ ;


\ portable half words definition
\ : halfws [ 1 cells shr ] * ;
: halfws 2* ;


\ 128 stringbuffer constant mybuf ..... will this work?
\ allocates 1 cell + buf of size len chars
\ keep end at halfword 0, maxlen at halfword +2 aka

: stringbuf-allot  ( len -- addr )  here 2dup 1 halfws + h! swap 2 halfws + allot ;  
: stringbuf-len    ( addr -- len ) 1 halfws + h@ ;		\ len field
: stringbuf-dstart ( addr -- addr+x ) 2 halfws + ;		\ start  of data area
: stringbuf-rewind ( addr -- ) 0 swap h! ;			\ set write pointer to 0
: stringbuf-0fill  ( addr -- ) dup stringbuf-len swap  stringbuf-dstart swap  0 fill ;  \ clear data area
: stringbuf-clear  ( addr -- ) dup stringbuf-rewind stringbuf-0fill ;
: stringbuffer ( len -- addr ) stringbuf-allot dup stringbuf-clear ;






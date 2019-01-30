
compiletoflash

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
\ same for string with byte length count as made by string,"
: memstr-byte-cnt ( addr -- c-addr length ) dup 1  + swap c@ ;

\ portable half words definition
\ : halfws [ 1 cells shr ] * ;
: halfws 2* ;


: lsb16 ( value -- lsb ) $ff and ;
: hsb16 ( value -- hsb ) $ff00 and 8 rshift ;
\ cave! reverse stack order for further processing !
: littleendian16 ( value -- hsb lsb ) dup hsb16 swap lsb16 ;
: bigendian16 ( value -- lsb hsb ) dup lsb16 swap hsb16 ;


\ allocates 1 cell + buf of size len chars
\ keep end at halfword 0, maxlen at halfword +2 aka

: stringbuf-allot  ( len -- addr )  here 2dup 1 halfws + h! swap 2 halfws + allot ;  
: stringbuf-len    ( addr -- len ) 1 halfws + h@ ;		\ len field
: stringbuf-dstart ( addr -- addr+x ) 2 halfws + ;		\ start  of data area
: stringbuf-rewind ( addr -- ) 0 swap h! ;			\ set write pointer to 0
: stringbuf-0fill  ( addr -- ) dup stringbuf-len swap  stringbuf-dstart swap  0 fill ;  \ clear data area
: stringbuf-clear  ( addr -- ) dup stringbuf-rewind stringbuf-0fill ;

\ user space allocator
\ 128 stringbuffer constant mybuf 
: stringbuffer ( len -- addr ) stringbuf-allot dup stringbuf-clear ;

\ put string lib compatible address & length to stack for reading
: stringbuf-pos  ( addr -- pos ) h@ ;
: stringbuf-string ( addr -- addr+x len ) dup stringbuf-dstart swap stringbuf-pos ;

\ : stringbuf-full? ( addr --) dup  stringbuf-len swap stringbuf-pos <= ;
: stringbuf-freebytes ( addr --) dup  stringbuf-len swap stringbuf-pos - ;
: stringbuf-full? ( addr --) stringbuf-freebytes 0 <= ;

\ place write pointer - silentyl fix out of bounds condition
: stringbuf-seek ( addr newpos --) dup 0 max 2 pick stringbuf-len min swap drop swap h! ;
\ relative move write pointer
: stringbuf-shift ( addr shift) over stringbuf-pos + stringbuf-seek ;

\ get address for write operations
: stringbuf-wheretowrite ( addr -- addr-free-byte ) stringbuf-string + ; 

\ worker function for below
: stringbuf-write-unchecked ( s1-addr sb-addr do-len -- )
  over stringbuf-wheretowrite 
  -rot tuck
  stringbuf-shift
  move
;

\ safe call - protected agains overrun
: stringbuf-write ( s1-addr s1-len sb-addr -- )
  dup stringbuf-freebytes     \ do we have space?
  rot  min  ( s1-addr sb-addr do-len -- )
  stringbuf-write-unchecked
;

\ append one single byte
: stringbuf-byte-app ( byte addr -- ) 
    dup stringbuf-full? 
    IF drop drop 
    ELSE dup 1 stringbuf-shift  stringbuf-wheretowrite 1- c!
    THEN 
; 

\ =========== debug and output ==================

\ wrapper for hexdump including start marker
: stringbuf-dump ( addr --) 
  cr
  dup ." Start at " hex.
  cr
  dup #16 mod #3 * #11 + spaces
  ." |-pos-|-len-|-data-> "
  cr
  stringbuf-string
  swap 4 - swap
  4 +
  dump
;

\ print single byte to console
: hexbyte #16 /mod .digit emit .digit emit ;

\ niced loop over it
: memstr-hexbytes ( addr size -- )
  cr
  over ." Addr: " hex.
  dup ." - length: " hex.
  cr
  over + swap
  DO I space c@ hexbyte LOOP 
  cr
;


cornerstone <<<stringbuf-lib>>>
compiletoram

' nop variable SLIP-handler-ptr
$80 stringbuffer constant SLIP-message
false variable SLIP-reading


#1000 variable SLIP-timeout
: SLIP-timeout-error ." ERROR: SLIP timeout" cr quit exit ;

(
: sys-key-timed
    key? false = IF  
      SLIP-timeout @ ms key? false = IF
        SLIP-timeout-error
    THEN THEN  
  sys-key
; 
)

: SLIP-key-unescape ( char -- char )
  dup SLIP_ESC = sys-key? and IF 
      sys-key nip 
  THEN
;

: slip-dumper
  SLIP-message stringbuf-dump 
  SLIP-message stringbuf-clear
;

' slip-dumper SLIP-handler-ptr !




: SLIP-key ( -- char )
  BEGIN
    sys-key
    
    dup SLIP_END = IF
      drop
      SLIP-reading @ IF
        \ finish slip reader
        false SLIP-reading !
        \ process SLIP message
        SLIP-handler-ptr @ execute
        AGAIN
      ELSE
        \ sart slip reader
        true SLIP-reading !
        AGAIN
      THEN
    THEN
   
    \ SLIP_ESC if ....  
    SLIP-key-unescape 
    ( key-unescaped -- )     
    
    SLIP-reading @ IF 
      SLIP-message stringbuf-byte-app
      AGAIN
    THEN
  \ if not in slip mode and no specil char, return char as 'key'
  \   true UNTIL
  \ REPEAT
; 

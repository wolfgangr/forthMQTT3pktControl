' nop variable SLIP-handler-ptr
$80 stringbuffer constant SLIP-message
false variable SLIP-reading


#1000 variable SLIP-timeout
: SLIP-timeout-error ." ERROR: SLIP timeout" cr quit exit ;


: slip-dumper
  SLIP-message stringbuf-dump 
  SLIP-message stringbuf-clear
;

\ ' slip-dumper SLIP-handler-ptr !



: SLIP-key-unescape ( char -- char )
  dup SLIP_ESC = sys-key? and IF 
      sys-key nip 
  THEN
;


: SLIP-key ( -- char )
  sys-key
  BEGIN
    dup SLIP_END = IF
      drop
      SLIP-reading @ IF
        \ finish slip reader
        false SLIP-reading !
        \ process SLIP message
        SLIP-handler-ptr @ execute
      ELSE
        \ sart slip reader
        true SLIP-reading !
      THEN
      true \ AGAIN
    ELSE
   
      \ SLIP_ESC if ....  
      inline SLIP-key-unescape 
      ( key-unescaped -- )     
    
      SLIP-reading @ IF 
        SLIP-message stringbuf-byte-app
        true \ AGAIN
      ELSE
        \ if not in slip mode and no specil char, return char as 'key'
        false
      THEN
    THEN
    dup if sys-key swap \ make compiler's stack balance checker happy
  WHILE
; 

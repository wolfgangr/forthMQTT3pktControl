' nop variable SLIP-handler-ptr
$80 stringbuffer constant SLIP-message
false variable SLIP-process


#1000 variable SLIP-timeout
: SLIP-timeout-error ." ERROR: SLIP timeout" cr quit exit ;

: sys-key-timed
    key? false = IF  
      SLIP-timeout @ ms key? false = IF
        SLIP-timeout-error
    THEN THEN  
  sys-key
;

: slip-dumper
  SLIP-message stringbuf-dump 
  SLIP-message stringbuf-clear
;


: SLIP-read-message 
  BEGIN
    sys-key-timed
    dup SLIP_END = IF SLIP-handler-ptr @ execute exit THEN
    dup SLIP_ESC = IF drop sys-key-timed THEN   \ unescapes
    SLIP-message stringbuf-byte-app
  AGAIN
;


\ processes received char
\ unescapes SLIP_ESC
: SLIP-key### ( -- char )
  sys-key   \ this is not timeout protected since it resembles standard 'key'
  dup CASE
        \ read along and process until either EOT or SLIP_END
    SLIP_END OF SLIP-read-message  0 swap ENDOF 
        \ silently ignore SLIP_ESC and return exptected next char instead
    SLIP_ESC OF sys-key-timed swap ENDOF 
  ENDCASE
;


: SLIP-key-unescape ( char -- char )
  dup SLIP_ESC = sys-key? and IF 
      sys-key nip 
  THEN
;

: SLIP-eatup
  BEGIN sys-key? while
    sys-key 
    dup SLIP_END = IF
      SLIP-handler-ptr @ execute
      sys-key? IF sys-key
    THEN
    SLIP-key-unescape
    
  REPEAT
;

: SLIP-key ( -- char )
  sys-key
  SLIP-process @ IF 
    dup SLIP_END = IF
      false SLIP-process !
      SLIP-handler-ptr @ execute
      drop $0D 
    ELSE
      \ ############ eat up till C0 or 0D
      \ SLIP-message stringbuf-byte-app
      \ append ##### loop it
    THEN
  ELSE
    dup SLIP_END = IF 
      true SLIP-process !
      \ eat up until......
      drop $0D
    ELSE
      SLIP-key-unescape
    THEN
  
  THEN
; 

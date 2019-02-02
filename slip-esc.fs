
$C0 constant SLIP_END
$DB constant SLIP_ESC
$DC constant SLIP_ESC_END
$DD constant SLIP_ESC_ESC


hook-key @ variable sys-key-ptr
hook-emit @ variable sys-emit-ptr

' nop variable SLIP-handler-ptr
$80 stringbuffer constant SLIP-message
\ false variable SLIP-process

: sys-emit sys-emit-ptr @ execute ;
: sys-key sys-key-ptr  @ execute ;

#10 constant SLIP-timeout
: SLIP-timeout-error ." ERROR: SLIP timeout" cr quit exit ;

: sys-key-timed
    key? false = IF  
      SLIP-timeout ms key? false = IF
        SLIP-timeout-error
    THEN THEN  
  sys-key
;

\ does ordinary emit, but prepends SLIP_ESC to SLIP_END and SLIP_ESC
: SLIP-emit ( char -- )
  dup case
    SLIP_END of SLIP_ESC sys-emit endof
    SLIP_ESC of SLIP_ESC sys-emit endof
  endcase
  sys-emit
;



: SLIP-read-message 
  BEGIN
    sys-key-timed
    dup SLIP_END = IF SLIP-handler-ptr @ execute leave THEN
    dup SLIP_ESC = IF drop sys-key-timed THEN   \ unescapes
    SLIP-message stringbuf-byte-app
  AGAIN
;


\ processes received char
\ unescapes SLIP_ESC
: SLIP-key ( -- char )
  sys-key   \ this is not timeout protected since it resembles standard 'key'
  dup CASE
        \ read along and process until either EOT or SLIP_END
    SLIP_END OF SLIP-read-message  0 swap ENDOF 
        \ silently ignore SLIP_ESC and return exptected next char instead
    SLIP_ESC OF sys-key-timed swap ENDOF 
  ENDCASE
;






: slip-ESC-activate
  ['] SLIP-emit hook-emit !
  ['] SLIP-key  hook-key  !
;

: slip-ESC-deactivate
  sys-emit-ptr @ hook-emit !
  sys-key-ptr  @ hook-key  !
;


: slip-dumper
  SLIP-message stringbuf-dump 
  SLIP-message stringbuf-clear
;

' slip-dumper SLIP-handler-ptr !

  
\ =============================================

$80 stringbuffer constant mirror  
false variable SLIP-process

: ?mirrline mirror  stringbuf-wheretowrite 1- c@ $0d = IF mirror dup stringbuf-dump stringbuf-clear THEN ;
: mykey old-key execute dup mirror stringbuf-byte-app ?mirrline ; 
: shark ['] mykey hook-key ! ;
: unshark old-key hook-key ! ;



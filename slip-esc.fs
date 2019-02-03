
$C0 constant SLIP_END
$DB constant SLIP_ESC
\ $DC constant SLIP_ESC_END
\ $DD constant SLIP_ESC_ESC


hook-key @ variable sys-key-ptr
hook-key? @ variable sys-key?-ptr
hook-emit @ variable sys-emit-ptr
' nop variable SLIP-handler-ptr


: sys-emit sys-emit-ptr @ execute ;
: sys-key sys-key-ptr  @ execute ;
: sys-key? sys-key?-ptr  @ execute ;
: SLIP-handler SLIP-handler-ptr @ execute ;


\ does ordinary emit, but prepends SLIP_ESC to SLIP_END and SLIP_ESC
: SLIP-emit ( char -- )
  dup case
    SLIP_END of SLIP_ESC sys-emit endof
    SLIP_ESC of SLIP_ESC sys-emit endof
  endcase
  sys-emit
;



$80 stringbuffer constant SLIP-message
false variable SLIP-reading


#1000 variable SLIP-timeout

: SLIP-timeout-error 
  false SLIP-reading !
  ." ERROR: SLIP timeout" cr 
  quit exit 
;





: slip-dumper
  cr ." SLIP buffer content:" cr
  SLIP-message stringbuf-dump 
  SLIP-message stringbuf-clear
;

' slip-dumper SLIP-handler-ptr !







: slip-ESC-activate
  ['] SLIP-emit hook-emit !
 \ ['] SLIP-key  hook-key  !
;

: slip-ESC-deactivate
  sys-emit-ptr @ hook-emit !
  sys-key-ptr  @ hook-key  !
;




  



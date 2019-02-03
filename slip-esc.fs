
$C0 constant SLIP_END
$DB constant SLIP_ESC
\ $DC constant SLIP_ESC_END
\ $DD constant SLIP_ESC_ESC


hook-key @ variable sys-key-ptr
hook-key? @ variable sys-key?-ptr
hook-emit @ variable sys-emit-ptr



: sys-emit sys-emit-ptr @ execute ;
: sys-key sys-key-ptr  @ execute ;
: sys-key? sys-key?-ptr  @ execute ;



\ does ordinary emit, but prepends SLIP_ESC to SLIP_END and SLIP_ESC
: SLIP-emit ( char -- )
  dup case
    SLIP_END of SLIP_ESC sys-emit endof
    SLIP_ESC of SLIP_ESC sys-emit endof
  endcase
  sys-emit
;


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




: slip-ESC-activate
  ['] SLIP-emit hook-emit !
 \ ['] SLIP-key  hook-key  !
;

: slip-ESC-deactivate
  sys-emit-ptr @ hook-emit !
  sys-key-ptr  @ hook-key  !
;



\ ' slip-dumper SLIP-handler-ptr !

  



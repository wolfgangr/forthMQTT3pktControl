
$100 stringbuffer constant test-buf
\ $100 stringbuffer constant test-buf2

: debug test-buf stringbuf-byte-app ;

: debug~ 
  $7E debug
  dup debug
;

\ try to read another character
\ send 0x0D , wait 10 ms, maybe wait another configurable time and then call an error
: sys-key-timed-###
    sys-key? not IF  
       \ $11 sys-emit 
       100 ms
       sys-key? not IF 
          SLIP-timeout @ ms sys-key? not IF
            SLIP-timeout-error
    THEN THEN THEN
  sys-key
; 


: slip-key-timeout
  sys-key? IF exit THEN
  100 ms
  sys-key? IF exit THEN
  SLIP-timeout @ ms 
  sys-key? IF exit THEN
  SLIP-timeout-error
; 

: sys-key-timed
  slip-key-timeout
  sys-key
;

: SLIP-key-unescape ( char -- char )
  dup SLIP_ESC = sys-key? and IF 
      sys-key-timed nip 
  THEN
;


: SLIP-key \ ( -- char )
  sys-key
  BEGIN
    debug~   
    dup SLIP_END = IF
      $23 emit
      \ drop
      SLIP-reading @ IF
              \ finish slip reader
        $2d emit
        false SLIP-reading !
              \ process SLIP message
        SLIP-handler
      ELSE
              \ sart slip reader
        $2b emit
        true SLIP-reading !
      THEN
      \ false \ AGAIN
      dup $0d = IF true ELSE false THEN
    ELSE
   
      SLIP-key-unescape 
           ( key-unescaped -- )     
    
      SLIP-reading @ IF 
        $7C emit
        dup SLIP-message stringbuf-byte-app
        false \ AGAIN
      ELSE
        \ if not in slip mode and no specil char, return char as 'key'
        \ dup 0x0d = if false else true
        true
      THEN
      
    THEN
             ( char flag - )
    \ dup IF
    \ ELSE
    \   nip sys-key-timed swap 
    \ THEN
    over   debug
        \ hand over 0 as dummy to compiler and return in any case
    not if drop 0 then
    true
  UNTIL
  
; 


: Slip-key2
  sys-key
  SLIP-key-unescape
  dup SLIP-message stringbuf-byte-app
  dup $0D = IF SLIP-handler THEN
;

\ ' sys-key hook-key ! 
\ ' SLIP-key hook-key ! 
\ ESPL-sync memstr-counted slip-send 
\ SLIP-message stringbuf-dump  
\ test-buf stringbuf-dump 
\ test-buf stringbuf-clear  ok.
\ slip-message stringbuf-clear  ok.
\ 5000 slip-timeout ! 

: run-test
  ." =========================== running test ================="
  test-buf stringbuf-dump
  test-buf stringbuf-clear
  slip-message stringbuf-dump
  slip-message stringbuf-clear 
 
  ." ----------------------- start test -------------------------"

  ['] SLIP-key hook-key !
  ESPL-sync memstr-counted slip-send 
  ['] sys-key hook-key !

  ." ---------------------------finished test --------------------"

  test-buf stringbuf-dump
  slip-message stringbuf-dump
  
  ." =============================test finished =================="
;

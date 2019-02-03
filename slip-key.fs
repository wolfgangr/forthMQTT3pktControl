
\ $100 stringbuffer constant test-buf
\ $100 stringbuffer constant test-buf2

: debug test-buf stringbuf-byte-app ;

: debug~ 
  $7E debug
  dup debug
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
      false \ AGAIN
      
    ELSE
   
      SLIP-key-unescape 
           ( key-unescaped -- )     
    
      SLIP-reading @ IF 
        $7C emit
        dup SLIP-message stringbuf-byte-app
        false \ AGAIN
      ELSE
        \ if not in slip mode and no specil char, return char as 'key'
        true
      THEN
      
    THEN
             ( char flag - )
    dup IF
    ELSE
      nip sys-key-timed swap 
    THEN
    over   debug
  UNTIL
  
; 


\ ' sys-key hook-key ! 
\ ' SLIP-key hook-key ! 
\ ESPL-sync memstr-counted slip-send 
\ SLIP-message stringbuf-dump  
\ test-buf stringbuf-dump 
\ test-buf stringbuf-clear  ok.
\ slip-message stringbuf-clear  ok.
\ 5000 slip-timeout ! 

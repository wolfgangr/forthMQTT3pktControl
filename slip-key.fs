

: SLIP-key \ ( -- char )
  BEGIN
    sys-key
    \ dup $7b emit emit $7d emit 
    dup SLIP_END = IF
      $23 emit
      drop
      SLIP-reading @ IF
        \ finish slip reader
        $2b emit
        false SLIP-reading !
        \ process SLIP message
        SLIP-handler-ptr @ execute
      ELSE
        \ sart slip reader
        $2d emit
        true SLIP-reading !
      THEN
      false \ AGAIN
      
    ELSE
   
      \ SLIP_ESC if ....  
      inline SLIP-key-unescape 
      \ ( key-unescaped -- )     
    
      SLIP-reading @ IF 
        $7C emit
        SLIP-message stringbuf-byte-app
        false \ AGAIN
      ELSE
        \ if not in slip mode and no specil char, return char as 'key'
        true
      THEN
      
    THEN

  UNTIL
; 

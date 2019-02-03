




: SLIP-key \ ( -- char )
  sys-key
  BEGIN
    \ sys-key
    \ dup $7b emit emit $7d emit 
    dup SLIP_END = IF
      $23 emit
      \ drop
      SLIP-reading @ IF
        \ finish slip reader
        $2b emit
        false SLIP-reading !
        \ process SLIP message
        SLIP-handler
      ELSE
        \ sart slip reader
        $2d emit
        true SLIP-reading !
      THEN
      false \ AGAIN
      
    ELSE
   
      \ SLIP_ESC if ....  
      SLIP-key-unescape 
      \ ( key-unescaped -- )     
    
      SLIP-reading @ IF 
        $7C emit
        dup SLIP-message stringbuf-byte-app
        false \ AGAIN
      ELSE
        \ if not in slip mode and no specil char, return char as 'key'
        true
      THEN
      
    THEN

    dup IF
    ELSE
      drop sys-key-timed swap 
    THEN
    
  UNTIL
; 

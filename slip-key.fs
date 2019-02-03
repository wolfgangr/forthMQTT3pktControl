

: SLIP-key \ ( -- char )
  sys-key
  BEGIN
    dup SLIP_END = IF
    
      \ drop
      SLIP-reading @ IF
        \ finish slip reader
        false SLIP-reading !
        \ process SLIP message
        SLIP-handler-ptr @ execute
      ELSE
        \ sart slip reader
        true SLIP-reading !
      THEN
      false \ AGAIN
      
    ELSE
   
      \ SLIP_ESC if ....  
      inline SLIP-key-unescape 
      \ ( key-unescaped -- )     
    
      SLIP-reading @ IF 
        dup SLIP-message stringbuf-byte-app
        false \ AGAIN
      ELSE
        \ if not in slip mode and no specil char, return char as 'key'
        true
      THEN
      
    THEN
    
    dup IF
    ELSE
      drop sys-key swap \ make compiler's stack balance checker happy
    THEN
    
  UNTIL
; 

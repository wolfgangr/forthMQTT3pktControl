
$C0 constant SLIP_END
$DB constant SLIP_ESC
\ $DC constant SLIP_ESC_END
\ $DD constant SLIP_ESC_ESC


\ after nnn ms, message is marked as timed out and closed
#100 constant SLIP-timeout

\ after mmm chars, message is marked as overrun and closed
#128 constant SLIP-maxlen


#128 constant SLIP-buffer-size

\ allocated space for message description buffer
\ 16 LSB offset of start into buffer
\ 8 bit length
\ flags for overrun, timeout, closed
\ #8 constant SLIP-msg-num
\ ##### can we prepend that to the message?

\ ======= end of config =============00



SLIP-buffer-size 4 + buffer: SLIP-ring

0 variable SLIP-msg-count
0 variable SLIP-lastchar-millis
false variable SLIP-status 
false variable SLIP-escaped
false variable RX-overrun
false variable SLIP-overrun

\ hook into environment
: SLIP-lowlevel-emit uart1-irq-emit ;


\ does ordinary emit, but prepends SLIP_ESC to SLIP_END and SLIP_ESC
: SLIP-emit ( char -- )
  dup case
    SLIP_END of SLIP_ESC SLIP-lowlevel-emit endof
    SLIP_ESC of SLIP_ESC SLIP-lowlevel-emit endof
  endcase
  sys-emit
;

\ here we hook in
\ : uart1-RX-irq-handler ( -- )  \ handle the USART receive interrupt
\  sys-key  \ will drop input when there is no room left
\  uart1-RX-ring dup ring? if >ring else 2drop then ;
  
\  discard new input on overrun and keep a flag
: SLIP-write-to-RX
  uart1-RX-ring dup 
  ring? if  >ring  
  else  2drop true RX-overrun !  then
;

\ silently overwrite oldest buffer content
: SLIP-write-to-SLIP ( char -- )
  SLIP-ring dup ring? 0= if 
    dup ring> drop 
    true SLIP-overrun ! 
  then >ring ; 
;

: SLIP-RX-irq-handler ( -- )
  sys-key
  
  
  
  
;
  
  
\ ~~~~~~~~~~~~~~~~~~~8<--bleeding-edge----~~~~~~~~~~~~~~~~~~~~~~~~~~~



: slip-ESC-activate
  ['] SLIP-emit hook-emit !
 \ ['] SLIP-key  hook-key  !
;

: slip-ESC-deactivate
  sys-emit-ptr @ hook-emit !
  sys-key-ptr  @ hook-key  !
;




  



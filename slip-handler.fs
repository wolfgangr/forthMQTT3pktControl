
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
: SLIP-write-to-RX ( char -- )
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

\ toggle false<->true in address e.g variable
\ : ~! ( addr -- ) dup @ 0= swap ! ;

\ ######################
\ ( buffer offset -- addr ) get address relative to ring buffer write pos
\ ..... - len  .... len byte
\ ..... - len +1 ... flag byte
\ https://github.com/jeelabs/embello/blob/master/explore/1608-forth/flib/any/ring.fs#L4

: ring-poker ( ring-addr 1|2 offset -- byte-addr)
  swap 2 pick + @ + ( ring-addr raw-offset -- )
  over @ and  ( ring-addr real-offset -- )
  +
;

\ len is at start -2, flag is at start -1
: SLIP-current-len-addr  ( -- adr ) SLIP-ring 1 -2 SLIP-msg-count - ring-poker ;
: SLIP-current-flag-addr ( -- adr ) SLIP-ring 1 -1 SLIP-msg-count - ring-poker ;

: SLIP-RX-irq-handler ( -- )
  \  read right from bare metal
  sys-key     ( char --) 
  
  \ we had escape the last time, process it now
  SLIP-escaped if 
    SLIP-status if SLIP-write-to-SLIP
      else SLIP-write-to-RX 
      then  
    false SLIP-escaped !
    exit
  then
  
  \  check for new escape?
  dup SLIP_ESC = if 
    true SLIP-escaped !
    drop exit
  then 
  
  \ perform slip millis check ############
  \ perform slip length check #############
  
  \ SLIP_END toggles status but gets not recorded otherwise
  \ ###### to do .... error checking ad ehader
  dup SLIP_END = if
    SLIP-status if
        false SLIP-status !
        \ do the closing stuff
        \ #####################
      else
        true SLIP-status !
        0 SLIP-write-to-SLIP \ place for length
        0 SLIP-write-to-SLIP \ place for flags
        0 SLIP-msg-count !
        millis SLIP-lastchar-millis !
      then
    drop exit
  then
  
  \ nothing special - feed char to the current buffer
  SLIP-status if
    SLIP-write-to-SLIP
    1 SLIP-msg-count +!
    millis SLIP-lastchar-millis !
  else
    SLIP-write-to-RX
  then
  
  
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




  



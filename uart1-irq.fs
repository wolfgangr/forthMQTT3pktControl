\ interrupt-based USART1 with input ring buffer
\ needs ring.fs


\ [ wjr ] Sun Feb  3 21:45:39 CET 2019
\ derived from template:
\ https://github.com/jeelabs/embello/blob/master/explore/1608-forth/flib/stm32f1/uart2.fs


#128 constant UART1-RX-buffer-size
#128 constant UART1-TX-buffer-size

\ $40004400 constant USART2
\ https://www.st.com/en/microcontrollers/stm32f103c8.html
\ pg 38 
$40013800  USART1
   USART1 $00 + constant USART1-SR
   USART1 $04 + constant USART1-DR
   USART1 $08 + constant USART1-BRR
   USART1 $0C + constant USART1-CR1
   USART1 $10 + constant USART1-CR2
   USART1 $14 + constant USART1-CR3
   USART1 $18 + constant USART1-GPTR


: uart1. ( -- )
  cr ." SR " USART1-SR @ h.4
   ."  DR "  USART1-DR @ h.4
   ."  BRR " USART1-BRR @ h.4
   ."  CR1 " USART1-CR1 @ h.4 
   ."  CR2 " USART1-CR2 @ h.4
   ."  CR3 " USART1-CR3 @ h.4
   ."  GPTR " USART1-GPTR @ h.4 ;

   
: uart1-baud ( n -- )  \ set baud rate assuming PCLK1 = sysclk/2
  baud 2/ USART1-BRR ! ;

\ cave - uart1 is initialized at boot !
: uart1-init ( -- )
  OMODE-AF-PP OMODE-FAST + PA2 io-mode!
  IMODE-FLOAT PA3 io-mode!
  17 bit RCC-APB1ENR bis!  \ set USART2EN
  115200 uart-baud
  %0010000000001100 USART1-CR1 ! ;
  

\ keep them for test purposes
\ we have assembler routines in the core aka serial-key etc
\ and want to implement irq-based handlers in the end

: uart1-key? ( -- f ) pause 1 5 lshift USART1-SR bit@ ;
: uart1-key ( -- c ) begin uart-key? until  USART1-DR @ ;
: uart1-emit? ( -- f ) pause 1 7 lshift USART1-SR bit@ ;
: uart1-emit ( c -- ) begin uart-emit? until USART1-DR ! ; 


\ keep the old handlers - why? - do we ever need them?
hook-key   @ constant polling-key
hook-key?  @ constant polling-key?
hook-emit  @ constant polling-emit
hook-emit? @ constant polling-emit?

\ wrapper for portability
\ mecrisp-stellaris-2.4.8/mecrisp-stellaris-source/common/stm-terminal.s 
\ serial-emit serial-key serial-emit? serial-key?
: sys-key?  serial-key?  ;
: sys-key   serial-key   ;
: sys-emit? serial-emit? ;
: sys-emit  serial-emit  ; 


  
\ ##~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
\ derived from
\ https://github.com/jeelabs/embello/blob/master/explore/1608-forth/flib/stm32f1/uart2-irq.fs

UART1-RX-buffer-size 4 + buffer: uart1-RX-ring
UART1-TX-buffer-size 4 + buffer: uart1-TX-ring


\ restore saved interrupt flag  - companion to eint?
: eint! ( flag -- ) if eint else dint then ;


\ 5 bit USART1-CR1 bis!  \ set RXNEIE  
\ 7 bit USART1-CR1 bis!  \ set TXEIE
: uart1-RX-irq-enable  5 bit USART1-CR1 bis! ;
: uart1-RX-irq-disable 5 bit USART1-CR1 bic! ;
: uart1-TX-irq-enable  7 bit USART1-CR1 bis! ;
: uart1-TX-irq-disable 7 bit USART1-CR1 bic! ;


: uart1-RX-irq-handler ( -- )  \ handle the USART receive interrupt
  sys-key  \ will drop input when there is no room left
  uart1-RX-ring dup ring? if >ring else 2drop then ;

  
: uart1-TX-irq-handler
  \ dint     \ our buffer looks screwed, and sometimes wrong chars...
  uart1-TX-ring dup ring# 0<> if 
    ring> sys-emit 
  else 
    drop uart1-TX-irq-disable 
  then
  \ eint
;
  
  
\ combined RX / TX irq - check irq reason
: uart1-irq-handler
  sys-key?  IF uart1-RX-irq-handler THEN
  sys-emit? IF uart1-TX-irq-handler THEN
; 

$E000E104 constant NVIC-EN1R \ IRQ 32 to 63 Set Enable Register

: uart1-irq-init ( -- )  \ initialise the USART1 using a receive ring buffer
  uart1-init
  uart1-RX-ring UART1-RX-buffer-size init-ring
  uart1-TX-ring UART1-TX-buffer-size init-ring
  ['] uart1-irq-handler irq-usart1 !
  37 32 - bit NVIC-EN1R !  \ enable USART1 interrupt 37
  \ http://www.st.com/stonline/products/literature/rm/13902.pdf
  \ 27.3.3 pg 794/1137 ff  An interrupt is generated if the RXNEIE bit is set.

  uart1-RX-irq-enable
  uart1-TX-irq-enable 
;

: uart1-irq-key? ( -- f )  \ input check for interrupt-driven ring buffer
  pause uart1-RX-ring ring# 0<> 
;

: uart1-irq-key ( -- c )  \ input read from interrupt-driven ring buffer
  begin uart1-irq-key? until  uart1-RX-ring ring> 
;

: uart1-irq-emit? ( -- f )   
  pause uart1-TX-ring ring?   
;

: uart1-irq-emit-noblock ( c -- ) 
  \ silently discard if full, app might better check before
  eint? dint >r
  uart1-TX-ring dup ring? if >ring else 2drop then 
  r> eint!
  uart1-TX-irq-enable
; 

\ blocking variant mimics the standard behaviour
: uart1-irq-emit
  BEGIN uart1-irq-emit? UNTIL uart1-irq-emit-noblock 
;
  
\ switch running REPL to use irq buffered input queue
: uart1-irq_ulize  
  ['] uart1-irq-key?  hook-key?  !
  ['] uart1-irq-key   hook-key   !
  ['] uart1-irq-emit? hook-emit? !
  ['] uart1-irq-emit  hook-emit  !
  uart1-irq-init
;

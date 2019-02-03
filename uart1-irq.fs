\ interrupt-based USART1 with input ring buffer
\ needs ring.fs


\ [ wjr ] Sun Feb  3 21:45:39 CET 2019
\ derived from template:
\ https://github.com/jeelabs/embello/blob/master/explore/1608-forth/flib/stm32f1/uart2.fs


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
  cr ." SR " USART2-SR @ h.4
   ."  DR " USART2-DR @ h.4
   ."  BRR " USART2-BRR @ h.4
   ."  CR1 " USART2-CR1 @ h.4 
   ."  CR2 " USART2-CR2 @ h.4
   ."  CR3 " USART2-CR3 @ h.4
   ."  GPTR " USART2-GPTR @ h.4 ;

   
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

: uart1-key? ( -- f ) pause 1 5 lshift USART2-SR bit@ ;
: uart1-key ( -- c ) begin uart-key? until  USART2-DR @ ;
: uart1-emit? ( -- f ) pause 1 7 lshift USART2-SR bit@ ;
: uart1-emit ( c -- ) begin uart-emit? until USART2-DR ! ; 
  
  
\ ##~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
\ derived from
\ https://github.com/jeelabs/embello/blob/master/explore/1608-forth/flib/stm32f1/uart2-irq.fs

128 4 + buffer: uart1-ring

: uart1-irq-handler ( -- )  \ handle the USART receive interrupt
  USART1-DR @  \ will drop input when there is no room left
  uart1-ring dup ring? if >ring else 2drop then ;

$E000E104 constant NVIC-EN1R \ IRQ 32 to 63 Set Enable Register

: uart1-irq-init ( -- )  \ initialise the USART1 using a receive ring buffer
  uart1-init
  uart1-ring 128 init-ring
  ['] uart1-irq-handler irq-usart1 !
  37 32 - bit NVIC-EN1R !  \ enable USART1 interrupt 37
  \ http://www.st.com/stonline/products/literature/rm/13902.pdf
  \ 27.3.3 pg 794/1137 ff  An interrupt is generated if the RXNEIE bit is set.
  5 bit USART1-CR1 bis!  \ set RXNEIE
;

: uart1-irq-key? ( -- f )  \ input check for interrupt-driven ring buffer
  pause uart1-ring ring# 0<> ;
: uart1-irq-key ( -- c )  \ input read from interrupt-driven ring buffer
  begin uart-irq1-key? until  uart1-ring ring> ;

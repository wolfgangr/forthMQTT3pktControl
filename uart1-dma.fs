\ https://github.com/jeelabs/esp-link/issues/430#issuecomment-460806198
\ manual pg 50 - 3.3 MemoryMap


\   0x4002 0400 - 0x4002 07FF   DMA2
\   0x4002 0000 - 0x4002 03FF   DMA1
$40020400 constant DMA1_BASE 

DMA1_BASE $08 + #20 4 1- * + constant USART1_TX_DMA_BASE     \ channel 4 -> 40020444
DMA1_BASE $08 + #20 5 1- * + constant USART1_RX_DMA_BASE     \ channel 5 -> 40020458

USART1_TX_DMA_BASE constant USART1_TX_DMA_CCR
USART1_RX_DMA_BASE constant USART1_RX_DMA_CCR

USART1_TX_DMA_BASE $4 + constant USART1_TX_DMA_CNDTR
USART1_RX_DMA_BASE $4 + constant USART1_RX_DMA_CNDTR

USART1_TX_DMA_BASE $8 + constant USART1_TX_DMA_CPAR
USART1_RX_DMA_BASE $8 + constant USART1_RX_DMA_CPAR

USART1_TX_DMA_BASE $C + constant USART1_TX_DMA_CMAR
USART1_RX_DMA_BASE $C + constant USART1_RX_DMA_CMAR



\ usart definitions

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
   ."  GPTR " USART1-GPTR @ h.4 
;



   
\ lend from https://github.com/jeelabs/embello/blob/master/explore/1608-forth/flib/any/ring.fs
\ adapteted to match dma serving the other end

\ each ring needs 4 extra bytes for internal housekeeping:
\   addr+0 = ring mask, i.e. N-1
\   addr+1 = put index: 0..255 (needs to be masked before use)
\   addr+2 = get index: 0..255 (needs to be masked before use)
\   addr+3 = spare
\   addr+4..addr+4+N-1 = actual ring buffer, N bytes

128 constant USART1_TX_buf-len
USART1_TX_buf-len 4 + buffer: USART1_TX_buffer 

128 constant USART1_RX_buf-len
USART1_RX_buf-len 4 + buffer: USART1_RX_buffer 


\ P 278: Channel configuration procedure
\ 1. Set the peripheral register address in the DMA_CPARx register. 

USART1-DR USART1_TX_DMA_CPAR !
USART1-DR USART1_RX_DMA_CPAR !

\ 2. Set the memory address in the DMA_CMARx register. 

USART1_TX_buffer 4 + USART1_TX_DMA_CMAR !
USART1_RX_buffer 4 + USART1_RX_DMA_CMAR !

\ 3. Configure the total number of data to be transferred in the DMA_CNDTRx register.
USART1_TX_buf-len USART1_TX_DMA_CNDTR
USART1_RX_buf-len USART1_RX_DMA_CNDTR

\ 4.  Configure the channel priority using the PL[1:0] bits in the DMA_CCRx register
\ Medium: (Bits 13:12= 01) = 0x1???

\ 5a. Configure data transfer direction, 
\     RX: Bit 4=1:  0x??1?
\     TX: Bit 4=0:  0x??0?


\ 5b. circular mode, 
\     Bit 5=1 0x??2?

\ 5c. peripheral (Bit 6) & memory (Bit7) incremented mode, 
\     Bit 6 = 0, Bit 7 = 1 -> 0x??8?

\ 5d. peripheral & memory data size, 
\     Bits 8:9 = Bits 10:11 = 00 -> 0x?0??

\ 5e. and interrupt after half and/or full transfer in the DMA_CCRx register



\ 6. Activate the channel by setting the ENABLE bit in the DMA_CCRx register.



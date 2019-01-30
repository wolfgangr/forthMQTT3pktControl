\ include stringbuf.fs
\ cornerstone <<<stringbuf-lib>>>

\ resembles
\ https://github.com/hexagon5un/hackaday_esp-14_power_meter/blob/master/forth_system/messages.fs
\ line 59:
: decbyte #10 /mod #10 /mod .digit rot .digit rot .digit swap ; 

\ ah I see ... line 63
\     |-data-|
\ 3 0 49 53 57 0 2 0 3 0 0   0

create MQTT.msg.val.tplt
hex
	$0c c,
	bytes," 03 00 23 23 23 00 "		\ data
	bytes," 02 00 03 00 00 00 " 		\ len of data
calign
myalign

\ patch value as 3 dec digits into string, given by address
: patch-value ( value address -- )
  swap 
  #10 /mod swap .digit 2 pick 2+ c!
  #10 /mod swap .digit 2 pick 1+ c!
  .digit swap c!
;


\ https://github.com/hexagon5un/hackaday_esp-14_power_meter/blob/master/forth_system/crc.fs
\ CRC accumulator
\ this is our companion 
\ https://github.com/jeelabs/esp-link/blob/fe4f565fe83f05e402cc8d8ca3ceefbc39692a1f/serial/crc16.c
\ it uses shortint, so we mask intermediates to 16 bit
: crc+ ( old_running_sum new_byte -- new_running_sum ) 
  xor
  dup 8 rshift swap 8 lshift or
  $ffff and
  dup $ff00 and 4 lshift xor
  $ffff and
  dup 8 rshift 4 rshift xor 
  dup $ff00 and 5 rshift xor
;

\ does not belong here
: sb-byte-app  stringbuf-byte-app ;
\ : sb-byte-app ( byte addr -- ) 
\   dup stringbuf-full? 
\    IF drop drop 
\    ELSE dup 1 stringbuf-shift  stringbuf-wheretowrite 1- c!
\    THEN 
\ ; 



: crc-assemble ( first afterlast -- hsb lsb )
  0 -rot swap
  DO  I c@ crc+  LOOP 
  \ dup cr ." CRC: " hex. cr
  dup $00ff and swap $ff00 and 8 rshift 
  swap
; 

\ assemble MQTT message, enclosed in SLIP frame
: MQTT-assemble ( buf-addr top-adr top-len msg-adr msg-len --)
  4 pick 
    ESPL-sync memstr-counted 2 pick stringbuf-write
    SLIP_END over sb-byte-app
    dup stringbuf-wheretowrite >r
    MQTT-preamble memstr-counted 2 pick stringbuf-write

  4 roll 4 roll rot stringbuf-write
  2 pick stringbuf-write
  MQTT-qos.and.retain memstr-counted 2 pick stringbuf-write
  dup stringbuf-wheretowrite
  r>  swap       
  crc-assemble
  2 pick sb-byte-app
  over sb-byte-app
  SLIP_END swap sb-byte-app
;

\ enclose arbitrary string into slip frame ( <END> <message> <CRC> <END> )
: SLIP-message ( buf-addr msg-adr msg-len --)
  2 pick 
    ESPL-sync memstr-counted 2 pick stringbuf-write
    SLIP_END over sb-byte-app
    dup stringbuf-wheretowrite >r
    
  2 pick stringbuf-write
  dup stringbuf-wheretowrite
  r>  swap       
  crc-assemble
  2 pick sb-byte-app
  over sb-byte-app
  SLIP_END swap sb-byte-app
;



\ add a string in adr / len format to a buffer, prepending 16 bit little-endian counter and 16 bit padding

: MQTT-stringadd ( buf-addr string-adr string-len -- )
  dup >r
  rot >r
       \ ( string-adr string-len -- ... ) R: ( string-len buf-addr -- ... )
  \ append len in 16 bit little endian
  dup littleendian16
  r@ stringbuf-byte-app
  r@ stringbuf-byte-app
  \ append string itself
  r@ stringbuf-write
  r> r>
    \ ( buf-addr string-len -- ... ) R: ( -- )
  1 and IF 0 swap stringbuf-byte-app ELSE drop THEN
; 


\ we want to have a topic with 
\    16 bit litte endian header
\    topic and dest concatenated
\    0x00 padded to 16 bit
\    MQTT-topic ( buf-addr top-adr top-len msg-adr msg-len --)

: MQTT-topic ( buf-addr prefix-adr prefix-len subtop-adr subtop-len -- ) 
  2 pick over + 1+ >r
  4 roll >r
  2swap
       \ ( subtop-adr subtop-len prefix-adr prefix-len  -- ... ) R: ( string-len buf-addr -- ... )
  \ append len in 16 bit little endian
  1 rpick littleendian16
  r@ stringbuf-byte-app
  r@ stringbuf-byte-app
  \ append prefix/subtopic 
  r@ stringbuf-write
  $2F r@ stringbuf-byte-app
  r@ stringbuf-write
  \ pad to even and balance stacks
  r> r>
    \ ( buf-addr string-len -- ... ) R: ( -- )
  1 and IF 0 swap stringbuf-byte-app ELSE drop THEN
; 
  
\ clear buffer and add generic mqtt pramble
: MQTT-init ( buf-adr -- )
  dup stringbuf-clear 
  MQTT-preamble memstr-counted rot stringbuf-write
;

\ static string appenders ( buf-adr -- )
: MQTT-append-on      MQTT-msg.on         memstr-counted rot stringbuf-write ;
: MQTT-append-off     MQTT-msg.off        memstr-counted rot stringbuf-write ;
: MQTT-append-q.a.r   MQTT-qos.and.retain memstr-counted rot stringbuf-write ;

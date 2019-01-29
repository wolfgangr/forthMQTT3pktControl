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
	0c c,
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
: sb-byte-app ( byte addr -- ) 
    dup stringbuf-full? 
    IF drop drop 
    ELSE dup 1 stringbuf-shift  stringbuf-wheretowrite 1- c!
    THEN 
; 



: crc-assemble ( first afterlast -- hsb lsb )
  0 -rot swap
  DO  I crc+  LOOP 
  dup $00ff and swap $ff00 and 8 rshift 
  swap
; 

: SLIP-assemble ( buf-addr top-adr top-len msg-adr msg-len --)
  4 pick 
    ESPL-sync memstr-byte-cnt 2 pick stringbuf-write
    SLIP_END over sb-byte-app
    dup stringbuf-wheretowrite >r
    MQTT-preamble memstr-byte-cnt 2 pick stringbuf-write

  4 roll 4 roll rot stringbuf-write
  2 pick stringbuf-write
  MQTT-qos.and.retain memstr-byte-cnt 2 pick stringbuf-write
  dup stringbuf-wheretowrite
  r>  swap       
  crc-assemble
  2 pick sb-byte-app
  over sb-byte-app
  SLIP_END swap sb-byte-app
;

\ TEst data
\ $80 stringbuffer constant SLIP-message
\ SLIP-message
\ MQTT-washer.topic memstr-byte-cnt
\ MQTT-msg.on memstr-byte-cnt



\ prepare some test data
\ 40 stringbuffer constant mybuf  
\ MQTT.msg.val.tplt  memstr-byte-cnt mybuf stringbuf-write 
\ cr cr
\ mybuf stringbuf-dump 
\ cr cr

\ #186 mybuf stringbuf-dstart 2+ 
\ .s
\ patch-value
\ mybuf stringbuf-dump
\ MQTT.msg.val.tplt memstr-byte-cnt memstr-hexbytes
\ mybuf stringbuf-string memstr-hexbytes 

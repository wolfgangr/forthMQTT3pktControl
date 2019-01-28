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


\ prepare some test data
40 stringbuffer constant mybuf  
MQTT.msg.val.tplt  memstr-byte-cnt mybuf stringbuf-write 
cr cr
mybuf stringbuf-dump 
cr cr

#186 mybuf stringbuf-dstart 2+ 
.s
patch-value
mybuf stringbuf-dump
MQTT.msg.val.tplt memstr-byte-cnt memstr-hexbytes
mybuf stringbuf-string memstr-hexbytes 

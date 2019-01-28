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
	mult-c," 03 00 23 23 23 00 "		\ data
	mult-c," 02 00 03 00 00 00 " 		\ len of data
calign
myalign

\ patch value as 3 dec digits into string, given by address
: pathch-value ( value address -- )
  swap 
  #10 /mod .digit over c!
  #10 /mod .digit over 1+ c!
  .digit 2+ c!
;

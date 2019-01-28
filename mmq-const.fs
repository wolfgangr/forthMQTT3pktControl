include mmq-tools.fs
\ [ wr Sun Jan 27 22:22:27 CET 2019 ]
\  constants for mqtt gateway impleemntation against esp-link
\  https://github.com/jeelabs/el-client/blob/master/ELClient/ELClient.cpp
\  https://en.wikipedia.org/wiki/Serial_Line_Internet_Protocol
$C0 constant SLIP_END
$DB constant SLIP_ESC
$DC constant SLIP_ESC_END
$DD constant SLIP_ESC_ESC

\ https://github.com/hexagon5un/hackaday_esp-14_power_meter/blob/master/forth_system/messages.fs
hex

create ESPL:sync
        08 , \ length
	bytes," 01 00 00 00 89 02 00 00 "
calign

create MQTT:preamble
        $08 , \ length
	bytes," 0b 00 05 00 00 00 00 00 "
calign

\ home/basement/washer
create MQTT:washer.topic
  hex
  $22 , \ forth string length
  14 c, 00 c, \ mqtt length
  \        1234567890abcdef1234
  mult-c," home/basement/washer" ;   \ can't mix string and char literals?
calign

\ home/basement/dryer
create MQTT:dryer.topic
  hex
  $22 , \ forth string length
  13 c, 00 c, \ mqtt length
  \        1234567890abcdef1234
  mult-c," home/basement/dryer" ;   \ can't mix string and char literals?
  00 c,  \ padding
calign









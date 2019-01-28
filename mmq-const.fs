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
create ESPL:sync-old
	$08 , \ length
	1 c,
	0 c, 0 c, 0 c,
	$89 c,
	2 c,
	0 c, 0 c,
calign

create MQTT:preamble-old
	$08 , \ length
	$0B c, 0 c,
	$05 c, 0 c, 
	0 ,  \ full cell?
calign

hex

create ESPL:sync
        08 , \ length
	bytes," 01 00 00 00 89 02 00 00 "
calign

create MQTT:preamble-old
        $08 , \ length
	bytes," 0b 00 05 00 00 00 00 00 "
calign

create MQTT:washer.topic-old
	$22 , \ forth string length
	14 c, 00 c, \ mqtt length
	68 c, 6f c, 6d c, 65 c, \ can't use h, or , due to endianness
	2f c, 62 c, 61 c, 73 c,
	65 c, 6d c, 65 c, 6e c,
	74 c, 2f c, 77 c, 61 c,
	73 c, 68 c, 65 c, 72 c,
calign

\ decimal

\ home/basement/washer
\ home/basement/dryer


create MQTT:washer.topic
  $22 , \ forth string length
  14 c, 00 c, \ mqtt length
  \        1234567890abcdef1234
  mult-c," home/basement/washer" ;   \ can't mix string and char literals?
calign

\ : MQTT:dryer.topic_str  s" home/basement/dryer" ;


create MQTT:dryer.topic
  $22 , \ forth string length
  13 c, 00 c, \ mqtt length
  \        1234567890abcdef1234
  mult-c," home/basement/dryer" ;   \ can't mix string and char literals?
  00 c,  \ padding
calign





\ create :q






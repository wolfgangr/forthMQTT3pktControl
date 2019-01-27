\ [ wr Sun Jan 27 22:22:27 CET 2019 ]
\  constants for mqtt gateway impleemntation against esp-link
\  https://github.com/jeelabs/el-client/blob/master/ELClient/ELClient.cpp
\  https://en.wikipedia.org/wiki/Serial_Line_Internet_Protocol
$C0 constant SLIP_END
$DB constant SLIP_ESC
$DC constant SLIP_ESC_END
$DD constant SLIP_ESC_ESC

\ https://github.com/hexagon5un/hackaday_esp-14_power_meter/blob/master/forth_system/messages.fs
create ESPL:sync
	$08 , \ length
	1 c,
	0 c, 0 c, 0 c,
	$89 c,
	2 c,
	0 c, 0 c,
calign

create MQTT:preamble
	$08 , \ length
	$0B c, 0 c,
	$05 c, 0 c, 
	0 ,  \ full cell?
calign

hex

create MQTT:washer.topic
	$22 , \ forth string length
	14 c, 00 c, \ mqtt length
	68 c, 6f c, 6d c, 65 c, \ can't use h, or , due to endianness
	2f c, 62 c, 61 c, 73 c,
	65 c, 6d c, 65 c, 6e c,
	74 c, 2f c, 77 c, 61 c,
	73 c, 68 c, 65 c, 72 c,
calign

decimal

\ home/basement/washer
\ home/basement/dryer


: MQTT:washer.topic_str s" home/basement/washer" ;   \ can't mix string and char literals?
: MQTT:dryer.topic_str  s" home/basement/dryer" ;


\ create :q






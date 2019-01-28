\ include mmq-tools.fs
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

create ESPL-sync
  08 , \ length
  bytes," 01 00 00 00 89 02 00 00 "
calign
align

create MQTT-preamble
  $08 , \ length
  bytes," 0b 00 05 00 00 00 00 00 "
calign
align

\ home/basement/washer
create MQTT-washer.topic
  16 , \ forth string length
  14 c, 00 c, \ mqtt length
  \        1234567890abcdef1234
  mult-c," home/basement/washer"    
calign
align

\ home/basement/dryer
create MQTT-dryer.topic
  16 , \ forth string length
  13 c, 00 c, \ mqtt length
  \        1234567890abcdef1234
  mult-c," home/basement/dryer"    
  00 c,  \ padding
calign
align


create MQTT-msg.on 
  0c ,
  bytes," 02 00 6f 6e 00 00 "  \ "on"
  bytes," 02 00  02 00  00 00  "  \ len of data  
calign
align

create MQTT-msg.off 
  0c ,
  bytes," 03 00 6f 66 66 00 "  \ "off"
  bytes," 02 00  03 00  00 00  "  \ len of data  
calign
align

create MQTT-qos.and.retain 
  0c ,
  bytes," 01 00 00 00 00 00 "  \ qos = 0
  bytes," 01 00  00 00  00 00  "  \ retain = 0
calign
align




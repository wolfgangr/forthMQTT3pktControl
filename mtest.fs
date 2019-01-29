\ setup test stuff and palygrund to exlore mmqt stuff

forgetram

: stringbuf-type ( adr -- ) stringbuf-string type ; 

include mmq-const.fs
include mmq-tools.fs


 $80 stringbuffer constant MQTT-message  
  
MQTT-message  
MQTT-washer.topic memstr-counted  
MQTT-msg.on memstr-counted  
.s

\ Stack: [5 ] 2A 20000858 2000046D 16 200004E9  TOS: C  *>

MQTT-assemble
MQTT-message stringbuf-dump  


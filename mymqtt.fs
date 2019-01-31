include mmq-const.fs
include mmq-tools.fs

\ should go to lib
\ : my.stringbuf-type ( adr -- ) stringbuf-string type ; 




: mymq.prefix s" heating/pressure" ;

: mymq.valve  s" valve" ;
: mymq.pump   s" pump" ;
: mymq.press  s" press" ;
: mymq.status s" status" ;

\ domain specific concatenators
( mqtt-message -- )
: mymq.pump.on 
    dup MQTT-init 
    dup mymq.prefix mymq.pump MQTT-topic
    dup MQTT-append-on
    MQTT-append-q.a.r
;

: mymq.pump.off 
    dup MQTT-init 
    dup mymq.prefix mymq.pump MQTT-topic
    dup MQTT-append-off
    MQTT-append-q.a.r
;  

: mymq.valve.on 
    dup MQTT-init 
    dup mymq.prefix mymq.valve MQTT-topic
    dup MQTT-append-on
    MQTT-append-q.a.r
;

: mymq.valve.off 
    dup MQTT-init 
    dup mymq.prefix mymq.valve MQTT-topic
    dup MQTT-append-off
    MQTT-append-q.a.r
;
  
  
  
\ test rund
  
$40 stringbuffer constant mqtt-message
$80 stringbuffer constant slip1-message
$80 stringbuffer constant slip2-message 



\ mqtt-message MQTT-init 
\ mqtt-message mymq.prefix mymq.valve MQTT-topic 
\ mqtt-message MQTT-append-off    .... -on
\ mqtt-message MQTT-append-q.a.r 
\ mqtt-message stringbuf-dump 


\ $80 stringbuffer constant slip-message  
mqtt-message mymq.valve.off 
slip1-message mqtt-message  stringbuf-string SLIP-assemble

\ repeat old stuff:
slip2-message MQTT-washer.topic memstr-counted MQTT-msg.off memstr-counted MQTT-assemble

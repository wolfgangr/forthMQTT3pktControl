include mmq-const.fs
include mmq-tools.fs


$40 stringbuffer constant mqtt-message
\ mqtt-message MQTT-init 
\ mqtt-message mymq.prefix mymq.valve MQTT-topic 
\ mqtt-message MQTT-append-off    .... -on
\ mqtt-message MQTT-append-q.a.r 
\ mqtt-message stringbuf-dump 


$80 stringbuffer constant slip-message  
\ mqtt-message mymq.valve.off 
\ slip-message mqtt-message  stringbuf-string SLIP-assemble


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
  
  
  
  
  

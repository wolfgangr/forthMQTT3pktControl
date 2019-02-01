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
  
: test.prefix s" home/basement" ;
: test.wash   s" wasABCDher" ;

: test.msg dup MQTT-init dup test.prefix test.wash MQTT-topic dup MQTT-append-off MQTT-append-q.a.r ;
  
  
\ test rund
  
$60 stringbuffer constant mqtt-message
$80 stringbuffer constant slip1-message
$80 stringbuffer constant slip2-message 



\ mqtt-message MQTT-init 
\ mqtt-message mymq.prefix mymq.valve MQTT-topic 
\ mqtt-message MQTT-append-off    .... -on
\ mqtt-message MQTT-append-q.a.r 
\ mqtt-message stringbuf-dump 


\ $80 stringbuffer constant slip-message  
\ mqtt-message mymq.valve.off 
mqtt-message test.msg
slip1-message mqtt-message  stringbuf-string SLIP-assemble


: mqtt-send slip1-message >r stringbuf-string r@ -rot SLIP-assemble r> stringbuf-type ; 

slip1-message stringbuf-type 

mqtt-message mymq.valve.off  
mqtt-message mqtt-send 

mqtt-message mymq.valve.on 
mqtt-message mqtt-send 

mqtt-message mymq.pump.on  
mqtt-message mqtt-send 

mqtt-message mymq.pump.off  
mqtt-message mqtt-send 


\ =====================================================================

hook-key @ Constant old-key 
$80 stringbuffer constant mirror  

: ?mirrline mirror  stringbuf-wheretowrite 1- c@ $0d = IF mirror dup stringbuf-dump stringbuf-clear THEN ;
: mykey old-key execute dup mirror stringbuf-byte-app ?mirrline ; 
: shark ['] mykey hook-key ! ;
: unshark old-key hook-key ! ;



\ =====================================================================


\ test data
: foo s" foo bar tralala " ; 

mqtt-message foo MQTT-dataadd

mqtt-message 2 1 MQTT-numberadd  \ quos
mqtt-message 1 1 MQTT-numberadd  \ retain

\ test composed headers - craft a MQTT setup
mqtt-message stringbuf-clear  
mqtt-message stringbuf-dump   
mqtt-message CMD_MQTT_SETUP 4 0 MQTT-cmdadd  
mqtt-message stringbuf-dump   

mqtt-message caret1 4 MQTT-numberadd 
mqtt-message caret2 4 MQTT-numberadd 
mqtt-message caret3 4 MQTT-numberadd 
mqtt-message caret4 4 MQTT-numberadd 



\ dummy callback functions
: ^1 ." doing one "   ;  
: ^2 ." doing two "   ;  
: ^3 ." doing three " ;
: ^4 ." doing four "  ; 

shark
\ supposed to be processd as a cmd=MQTT_SETUP
mqtt-message mqtt-send


  unshark
  
  

\ =============================================0
\ subscribe
: subscribetopic s" foo/bar" ; 
mqtt-message stringbuf-clear 
mqtt-message CMD_MQTT_SUBSCRIBE 2 0 MQTT-cmdadd 
mqtt-message subscribetopic MQTT-stringadd
\ qos=1
mqtt-message 1 1 MQTT-numberadd 

shark
mqtt-message mqtt-send







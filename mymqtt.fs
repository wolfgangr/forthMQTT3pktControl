include mmq-const.fs
include mmq-tools.fs


\ $40 stringbuffer constant mqtt-message
\ mqtt-message mymq.prefix mymq.valve MQTT-topic 

: mymq.prefix s" heating/pressure" ;

: mymq.valve  s" valve" ;
: mymq.pump   s" pump" ;
: mymq.press  s" press" ;
: mymq.status s" status" ;




  
  
  
  
  
  

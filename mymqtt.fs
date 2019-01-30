\ maintopic
\ subtopic
\ subsubtopics


\ $40 stringbuffer constant mqtt-message
\ mqtt-message mymq.prefix mymq.valve MQTT-topic 

: mymq.prefix s" heating/pressure" ;

: mymq.valve  s" valve" ;
: mymq.pump   s" pump" ;
: mymq.press  s" press" ;
: mymq.status s" status" ;



\ add a string in adr / len format to a buffer, prepending 16 bit little-endian counter and 16 bit padding

: MQTT-stringadd ( buf-addr string-adr string-len -- )
  dup >r
  rot >r
       \ ( string-adr string-len -- ... ) R: ( string-len buf-addr -- ... )
  \ append len in 16 bit little endian
  dup littleendian16
  r@ stringbuf-byte-app
  r@ stringbuf-byte-app
  \ append string itself
  r@ stringbuf-write
  r> r>
    \ ( buf-addr string-len -- ... ) R: ( -- )
  1 and IF 0 swap stringbuf-byte-app ELSE drop THEN
; 


\ we want to have a topic with 
\    16 bit litte endian header
\    topic and dest concatenated
\    0x00 padded to 16 bit
\    MQTT-topic ( buf-addr top-adr top-len msg-adr msg-len --)

: MQTT-topic ( buf-addr prefix-adr prefix-len subtop-adr subtop-len -- ) 
  2 pick over + 1+ >r
  4 roll >r
  2swap
       \ ( subtop-adr subtop-len prefix-adr prefix-len  -- ... ) R: ( string-len buf-addr -- ... )
  \ append len in 16 bit little endian
  1 rpick littleendian16
  r@ stringbuf-byte-app
  r@ stringbuf-byte-app
  \ append prefix/subtopic 
  r@ stringbuf-write
  $2F r@ stringbuf-byte-app
  r@ stringbuf-write
  \ pad to even and balance stacks
  r> r>
    \ ( buf-addr string-len -- ... ) R: ( -- )
  1 and IF 0 swap stringbuf-byte-app ELSE drop THEN
; 
  
  
  
  
  
  
  

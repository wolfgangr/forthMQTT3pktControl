\ druck messen
include ad.fs   
\ Ausgänge steuern
include rel.fs 
\ schaltpunkte festlegen
3000 variable limit-v-on
2500 variable limit-v-off
1000 variable limit-p-on
1500 variable limit-p-off
100 variable pv-loop-delay

\ ===================
: pumpe? ( druck --) limit-p-on @ < IF pumpen THEN ;     \    limit-p-off @  > IF pv-stop THEN ;
: ventil? ( druck --) limit-v-on @ > IF ablassen THEN ;  \    limit-v-off @  < IF pv-stop THEN ;
\ : pvOK? ( druck --) dup limit-p-off @  >  swap limit-v-off @  <  and IF pv-stop THEN ;

: isbetween ( low high test -- flag) tuck > -rot < and ;
: pvOK? limit-p-off @  limit-v-off @ rot isbetween IF pv-stop THEN ;


: pv-check druck@ dup dup pumpe? ventil? pvOK? ;
: pv-loop BEGIN pv-check key? pv-loop-delay @ ms  UNTIL ;
: pv-test BEGIN druck@ dup . dup dup pumpe? ventil? pvOK? key? 1000 ms UNTIL ;


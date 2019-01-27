\ druck messen
include ad.fs   
\ Ausgänge steuern
include rel.fs 
\ schaltpunkte festlegen
3000 variable limit-v-on
2500 variable limit-v-off
1000 variable limit-p-on
1500 variable limit-p-off

\===================
: pumpe? ( druck --) dup limit-p-on @ < IF pumpen THEN limit-p-off @  > IF pv-stop THEN ;
: ventil? ( druck --) dup limit-v-on @ > IF ablassen THEN limit-v-off @  < IF pv-stop THEN ;
: pv-check druck@ dup pumpe? ventil? ;
: pv-loop BEGIN pv-check key? UNTIL ;
: pv-test BEGIN druck@ dup . dup pumpe? ventil? key? 1000 ms UNTIL ;


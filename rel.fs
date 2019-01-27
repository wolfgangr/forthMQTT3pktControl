\ pins definieren
2 15 io variable relais1  
2 14 io variable relais2  

\ initialisieren
omode-pp relais1 @ io-mode! 
omode-pp relais2 @ io-mode! 

\ relais schalten
: r1-ein -1 relais1 @ io! ;
: r2-ein -1 relais2 @ io! ;
: r1-aus  0 relais1 @ io! ;
: r2-aus  0 relais2 @ io! ;

\ app primitives Status setzen 
: pv-stop r1-aus r2-aus ;
: pumpen r1-ein r2-aus ;
: ablassen r2-ein r1-aus ;
: druck-OK pv-stop ;

\ .... und testen  0=aus , 1=ablassen 2=pumpen
: pv-status ( -- status) relais2 @ io@ 1 and relais1 @ io@ 2 and or ; 

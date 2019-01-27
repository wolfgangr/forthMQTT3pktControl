2 15 io variable relais1  
2 14 io variable relais2  
omode-pp relais1 @ io-mode! 
omode-pp relais2 @ io-mode! 
: r1-ein -1 relais1 @ io! ;
: r2-ein -1 relais2 @ io! ;
: r1-aus  0 relais1 @ io! ;
: r2-aus  0 relais2 @ io! ;
: pv-stop r1-aus r2-aus ;
: pumpen r1-ein r2-aus ;
: ablassen r2-ein r1-aus ;
: druck-OK pv-stop ;



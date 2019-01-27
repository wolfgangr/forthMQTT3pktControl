PB0 constant press-in
0 variable P_offset    \ nullpunk-readout des sensors
1000 variable P_scale  \ Skalierungsfaktor in 1000stel

: myADinit 
	IMODE-ADC press-in io-mode!
	adc-init 
	adc-calib 
;

myADinit 

: druck-raw@ press-in adc ;
: druck@ druck-raw@ P_offset @ - P_scale @ 1000 */ ;

 

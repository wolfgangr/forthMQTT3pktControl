\ compiletoflash

\ : flash-pagesize ( addr - u )  \ return size of flash page at given address
\   drop flash-kb 128 <= if 1024 else 2048 then ;



: cornerstone ( name ) ( -- )  \ define a flash memory cornerstone
  <builds begin here dup flash-pagesize 1- and while 0 h, repeat
  does>   begin dup  dup flash-pagesize 1- and while 2+   repeat  cr
  eraseflashfrom ;



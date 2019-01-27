include rnw/h
include rnw/cornerstone.fs
cornerstone <<embello-hal>>

\ https://github.com/jeelabs/embello/blob/master/docs/flib/timed.md#callback-timer-library
include ../flib/mecrisp/multi.fs
cornerstone <<multitasking-lib>>

include ../flib/any/timed.fs
cornerstone <<callback-timer-lib>>

include rnw/blink.fs
cornerstone <<blink-test>>

timed-init



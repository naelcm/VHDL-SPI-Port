# VHDL-SPI-Port
VHDL Data port and matching C# GUI for Lattice LCMX02-7000HE Dev Board

The Lattice LCMX02-7000HE Dev Board includes a dual channel FTDI USB-to-parallel device (FT2232H).  One channel of this is used as a JTAG programmer for the LCMX02-7000HE CPLD.  The other is not used but is connected to the FPGA I/O pins.  This project contains the VHDL to implement an SPI port in the CPLD which connects to two banks of eight 8-bit registers.  One bank can be read and the other bank can be written.  This project also contains a C# .NET project for a simple app to read and write the registers.

I imagine that this could be used either as a base for some learning and experimentation in VHDL (you could write VHDL code that takes the writeable registers as its inputs and uses the readable registers as outputs), or for a debug and control port on some other CPLD based project.

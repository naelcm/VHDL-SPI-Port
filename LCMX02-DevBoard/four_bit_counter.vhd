-- Four bit synchronous counter which counts on the rising edge of the clock
-- Has asynchronous clear input so counter can be reset when CS is high

LIBRARY ieee;
USE ieee.std_logic_1164.all;
USE ieee.numeric_std.all;

library MACHXO2;
use MACHXO2.components.all;

ENTITY four_bit_counter IS
	PORT(
		clk 	: IN STD_LOGIC;
		reset	: IN std_logic;
		qout	: OUT UNSIGNED(3 downto 0));
END four_bit_counter;

ARCHITECTURE behavior OF four_bit_counter IS

	SIGNAL q : UNSIGNED(3 downto 0);
	SIGNAL d0 : STD_LOGIC;
	SIGNAL d1 : STD_LOGIC;
	SIGNAL d2 : STD_LOGIC;
	SIGNAL d3 : STD_LOGIC;

	COMPONENT FD1P3DX
	   PORT(
			 D    : IN  STD_LOGIC;
			 SP   : IN  STD_LOGIC;
			 CK   : IN  STD_LOGIC;
			 CD   : IN  STD_LOGIC;
			 Q    : OUT STD_LOGIC);
	END COMPONENT;
	
BEGIN

	-- Intermediate signals are necessary, DO NOT put expressions inside PORT MAPs !
	d0 <= NOT q(0);
	d1 <= q(0) XOR q(1);
	d2 <= (q(0) AND q(1) AND NOT q(2)) OR (NOT q(1) AND q(2)) OR (NOT q(0) AND q(2));
	d3 <= (q(0) AND q(1) AND q(2) AND NOT q(3)) OR (NOT q(1) AND q(3)) OR (NOT q(0) AND q(3)) OR (NOT q(2) AND q(3));
	
	latch_1 : FD1P3DX PORT MAP (CK => clk, CD => reset, SP => '1', D => d0, Q => q(0));
	latch_2 : FD1P3DX PORT MAP (CK => clk, CD => reset, SP => '1', D => d1, Q => q(1));
	latch_3 : FD1P3DX PORT MAP (CK => clk, CD => reset, SP => '1', D => d2, Q => q(2));
	latch_4 : FD1P3DX PORT MAP (CK => clk, CD => reset, SP => '1', D => d3, Q => q(3));
	
	qout <= q;

END behavior;


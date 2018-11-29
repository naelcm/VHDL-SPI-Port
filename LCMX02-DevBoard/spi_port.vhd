-- SPI Serial Port
-- Rising edge of SCLK clocks data in and out, CPOL = 1
-- SDO is updated on the falling edge of SCLK
-- Usage is to assert CS low, then clock in eight bits of address data.  If b7 of address is set then the 
-- specified register is read from and the data from it is clocked out on SDO by the next eight clocks.
-- If b7 of address is low then the specified register is written with the subsequent eight bits.
-- Finally negate CS high.

LIBRARY ieee;
USE ieee.std_logic_1164.all;
USE ieee.numeric_std.all;

library MACHXO2;
use MACHXO2.components.all;

ENTITY spi_port IS
	PORT(
		clkin : IN STD_LOGIC;	    -- The master clock
		sclk : IN STD_LOGIC;      	-- Serial clock, rising edge clocks data in and out
		sdi : IN STD_LOGIC;       	-- Serial data in
		sdo : OUT STD_LOGIC;		-- Serial data out
		cs : IN STD_LOGIC;        	-- Chip select, active low
		wrReg0 : OUT STD_LOGIC_VECTOR(7 downto 0);
		wrReg1 : OUT STD_LOGIC_VECTOR(7 downto 0);
		wrReg2 : OUT STD_LOGIC_VECTOR(7 downto 0);
		wrReg3 : OUT STD_LOGIC_VECTOR(7 downto 0);
		wrReg4 : OUT STD_LOGIC_VECTOR(7 downto 0);
		wrReg5 : OUT STD_LOGIC_VECTOR(7 downto 0);
		wrReg6 : OUT STD_LOGIC_VECTOR(7 downto 0);
		wrReg7 : OUT STD_LOGIC_VECTOR(7 downto 0);
		rdReg0 : IN STD_LOGIC_VECTOR(7 downto 0);
		rdReg1 : IN STD_LOGIC_VECTOR(7 downto 0);
		rdReg2 : IN STD_LOGIC_VECTOR(7 downto 0);
		rdReg3 : IN STD_LOGIC_VECTOR(7 downto 0);
		rdReg4 : IN STD_LOGIC_VECTOR(7 downto 0);
		rdReg5 : IN STD_LOGIC_VECTOR(7 downto 0);
		rdReg6 : IN STD_LOGIC_VECTOR(7 downto 0);
		rdReg7 : IN STD_LOGIC_VECTOR(7 downto 0));
END spi_port;

ARCHITECTURE behavior OF spi_port IS
	SIGNAL shiftReg : STD_LOGIC_VECTOR(15 downto 0);	    -- 16 bit register which takes the 16 bits written in over the SPI bus
	SIGNAL bitCounter : UNSIGNED(3 downto 0);        		-- Counts number of bits received
	SIGNAL internalReg0 : STD_LOGIC_VECTOR(7 downto 0);
	SIGNAL outReg : STD_LOGIC_VECTOR(7 downto 0);
   
	COMPONENT four_bit_counter
		PORT(			clk 	: IN STD_LOGIC;
			reset	: IN std_logic;
			qout	: OUT UNSIGNED(3 downto 0));
	END COMPONENT;
   
BEGIN
	PROCESS(sclk)

	BEGIN
		-- Shift the data on the rising edge while CS is low
		IF(rising_edge(sclk) AND (cs = '0')) THEN
			shiftReg(0) <= sdi;
			shiftReg(15 downto 1) <= shiftReg(14 downto 0);
		END IF;
			
		-- Check for a read after the eighth clock
		-- The MSB of the read is written straight out to SDO
		IF(falling_edge(sclk) AND (cs = '0')) THEN
		    IF(bitCounter = 2#1000#) THEN
				CASE shiftReg(7 downto 0) IS
					WHEN "10000000" => outReg <= rdReg0; sdo <= rdReg0(7);
					WHEN "10000001" => outReg <= rdReg1; sdo <= rdReg1(7);
					WHEN "10000010" => outReg <= rdReg2; sdo <= rdReg2(7);
					WHEN "10000011" => outReg <= rdReg3; sdo <= rdReg3(7);
					WHEN "10000100" => outReg <= rdReg4; sdo <= rdReg4(7);
					WHEN "10000101" => outReg <= rdReg5; sdo <= rdReg5(7);
					WHEN "10000110" => outReg <= rdReg6; sdo <= rdReg6(7);
					WHEN "10000111" => outReg <= rdReg7; sdo <= rdReg7(7);
					WHEN OTHERS => NULL;
				END CASE;
			ELSIF(bitCounter > 2#1000#) THEN 
				sdo <= outReg(6);
				outReg(6 downto 1) <= outReg(5 downto 0);
			ELSE
				NULL;
			END IF;
		END IF;
		
	    -- Write to the addressed register on the rising edge of cs
		IF(rising_edge(cs)) THEN
			CASE shiftReg(15 downto 8) IS
				WHEN "00000000" => wrReg0 <= shiftReg(7 downto 0);
				WHEN "00000001" => wrReg1 <= shiftReg(7 downto 0);
				WHEN "00000010" => wrReg2 <= shiftReg(7 downto 0);
				WHEN "00000011" => wrReg3 <= shiftReg(7 downto 0);
				WHEN "00000100" => wrReg4 <= shiftReg(7 downto 0);
				WHEN "00000101" => wrReg5 <= shiftReg(7 downto 0);
				WHEN "00000110" => wrReg6 <= shiftReg(7 downto 0);
				WHEN "00000111" => wrReg7 <= shiftReg(7 downto 0);
				WHEN OTHERS => NULL;
			END CASE;
		END IF;


	END PROCESS;
	
	-- Four bit counter used to trigger logic to latch the address
	spi_counter : four_bit_counter PORT MAP (clk => sclk, reset => cs, qout => bitCounter);

	
END behavior;
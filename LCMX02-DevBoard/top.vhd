-- Project top level
-- Implements eight readable and eight writable registers
-- The hex switch is connected to one of the readable registers
-- The LEDs are connected to one of the writable registers
-- The other registers are connected to each other for readback

LIBRARY ieee;
USE ieee.std_logic_1164.all;
USE ieee.numeric_std.all;

library MACHXO2;
use MACHXO2.components.all;

ENTITY top IS
   PORT(
      hex_switch_nb0 : IN STD_LOGIC;    -- Connects to PT33A, pin 1, inverted
      hex_switch_nb1 : IN STD_LOGIC;    -- Connects to PT33A, pin 5, inverted
      hex_switch_nb2 : IN STD_LOGIC;    -- Connects to PT33A, pin 112, inverted
      hex_switch_nb3 : IN STD_LOGIC;    -- Connects to PT33A, pin 114, inverted
      led1  : INOUT  STD_LOGIC;			-- Connects to PR2C, pin 97, D1
      led2  : INOUT  STD_LOGIC;			-- Connects to PR2C, pin 98, D2
      led3  : INOUT  STD_LOGIC;			-- Connects to PR2C, pin 99, D3
      led4  : INOUT  STD_LOGIC;			-- Connects to PR2C, pin 100, D4
      led5  : INOUT  STD_LOGIC;			-- Connects to PR2C, pin 104, D5
      led6  : INOUT  STD_LOGIC;			-- Connects to PR2C, pin 105, D6
      led7  : INOUT  STD_LOGIC;			-- Connects to PR2B, pin 106, D7
      led8  : INOUT  STD_LOGIC;			-- Connects to PR2A, pin 107, D8
	  ft2232b_d0  : IN STD_LOGIC;		-- Connects to Port B D0 on the FT2232 (SCLK - rising edge active)
	  ft2232b_d1  : IN STD_LOGIC;		-- Connects to Port B D1 on the FT2232 (SDI - into the Lattice device)
	  ft2232b_d2  : OUT STD_LOGIC;		-- Connects to Port B D2 on the FT2232 (SDO - out of the Lattice device)
	  ft2232b_d3  : IN STD_LOGIC;		-- Connects to Port B D3 on the FT2232 (CS - active low)
	  ft2232b_d4  : IN STD_LOGIC;		-- Connects to Port B D4 on the FT2232
	  ft2232b_d5  : IN STD_LOGIC;		-- Connects to Port B D5 on the FT2232
	  ft2232b_d6  : IN STD_LOGIC);		-- Connects to Port B D6 on the FT2232
END top;

ARCHITECTURE behavior OF top IS
	SIGNAL clk  : STD_LOGIC;
	SIGNAL myWrReg0 : STD_LOGIC_VECTOR(7 downto 0);
	SIGNAL myWrReg1 : STD_LOGIC_VECTOR(7 downto 0);
	SIGNAL myWrReg2 : STD_LOGIC_VECTOR(7 downto 0);
	SIGNAL myWrReg3 : STD_LOGIC_VECTOR(7 downto 0);
	SIGNAL myWrReg4 : STD_LOGIC_VECTOR(7 downto 0);
	SIGNAL myWrReg5 : STD_LOGIC_VECTOR(7 downto 0);
	SIGNAL myWrReg6 : STD_LOGIC_VECTOR(7 downto 0);
	SIGNAL myWrReg7 : STD_LOGIC_VECTOR(7 downto 0);
	SIGNAL myRdReg0 : STD_LOGIC_VECTOR(7 downto 0);
	SIGNAL myRdReg1 : STD_LOGIC_VECTOR(7 downto 0);
	SIGNAL myRdReg2 : STD_LOGIC_VECTOR(7 downto 0);
	SIGNAL myRdReg3 : STD_LOGIC_VECTOR(7 downto 0);
	SIGNAL myRdReg4 : STD_LOGIC_VECTOR(7 downto 0);
	SIGNAL myRdReg5 : STD_LOGIC_VECTOR(7 downto 0);
	SIGNAL myRdReg6 : STD_LOGIC_VECTOR(7 downto 0);
	SIGNAL myRdReg7 : STD_LOGIC_VECTOR(7 downto 0);
	--internal oscillator
	COMPONENT OSCH
		GENERIC(
            NOM_FREQ: string := "53.20");
		PORT( 
            STDBY    : IN  STD_LOGIC;
            OSC      : OUT STD_LOGIC;
            SEDSTDBY : OUT STD_LOGIC);
	END COMPONENT;
   
	COMPONENT spi_port
		PORT(
			clkin : IN STD_LOGIC;	  					-- The master clock
			sclk : IN STD_LOGIC;      					-- Serial clock
			sdi : IN STD_LOGIC;       					-- Serial data in
			sdo : OUT STD_LOGIC;						-- Serial data out
			cs : IN STD_LOGIC;        					-- Chip select, active low
			wrReg0 : OUT STD_LOGIC_VECTOR(7 downto 0);	-- Registers that get written over the SPI bus
			wrReg1 : OUT STD_LOGIC_VECTOR(7 downto 0);
			wrReg2 : OUT STD_LOGIC_VECTOR(7 downto 0);
			wrReg3 : OUT STD_LOGIC_VECTOR(7 downto 0);
			wrReg4 : OUT STD_LOGIC_VECTOR(7 downto 0);	
			wrReg5 : OUT STD_LOGIC_VECTOR(7 downto 0);
			wrReg6 : OUT STD_LOGIC_VECTOR(7 downto 0);
			wrReg7 : OUT STD_LOGIC_VECTOR(7 downto 0);
			rdReg0 : IN STD_LOGIC_VECTOR(7 downto 0);	-- Registers that get read over the SPI bus
			rdReg1 : IN STD_LOGIC_VECTOR(7 downto 0);
			rdReg2 : IN STD_LOGIC_VECTOR(7 downto 0);
			rdReg3 : IN STD_LOGIC_VECTOR(7 downto 0);
			rdReg4 : IN STD_LOGIC_VECTOR(7 downto 0);
			rdReg5 : IN STD_LOGIC_VECTOR(7 downto 0);
			rdReg6 : IN STD_LOGIC_VECTOR(7 downto 0);
			rdReg7 : IN STD_LOGIC_VECTOR(7 downto 0));
	END COMPONENT;
	
BEGIN
	--internal oscillator
	OSCInst0: OSCH
    GENERIC MAP (NOM_FREQ  => "53.20")
    PORT MAP (STDBY => '0', OSC => clk, SEDSTDBY => OPEN);
	
	PROCESS(clk)
		VARIABLE count :   INTEGER RANGE 0 TO 25_000_000;
	BEGIN
		IF(clk'EVENT AND clk = '1') THEN
		    -- Flash led1 so we know something is working
			IF(count < 25_000_000) THEN
				count := count + 1;
			ELSE
				count := 0;
				led1 <= not led1;
			END IF;

			myRdReg0 <= myWrReg0;
			myRdReg1 <= myWrReg1;
			myRdReg2 <= myWrReg2;
			myRdReg3 <= myWrReg3;
			myRdReg4 <= myWrReg4;
			myRdReg5 <= myWrReg5;
			myRdReg6 <= myWrReg6;
			myRdReg7(0) <= hex_switch_nb0;
			myRdReg7(1) <= hex_switch_nb1;
			myRdReg7(2) <= hex_switch_nb2;
			myRdReg7(3) <= hex_switch_nb3;
			led5 <= not myWrReg0(0);
			led6 <= not myWrReg0(1);
			led7 <= not myWrReg0(2);
			led8 <= not myWrReg0(3);
		END IF;
	END PROCESS;
	  
	spi_port_1 : spi_port PORT MAP( clkin => clk, sclk => ft2232b_d0, sdi => ft2232b_d1, sdo => ft2232b_d2, cs => ft2232b_d3, wrReg0 => myWrReg0, wrReg1 => myWrReg1, wrReg2 => myWrReg2, wrReg3 => myWrReg3, wrReg4 => myWrReg4, wrReg5 => myWrReg5, wrReg6 => myWrReg6, wrReg7 => myWrReg7, rdReg0 => myRdReg0, rdReg1 => myRdReg1, rdReg2 => myRdReg2, rdReg3 => myRdReg3, rdReg4 => myRdReg4, rdReg5 => myRdReg5, rdReg6 => myRdReg6, rdReg7 => myRdReg7);
			
END behavior;

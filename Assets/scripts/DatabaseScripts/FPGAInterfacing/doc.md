# Documentation for FPGA-side

`firmware.c` contains the firmware running on the NIOS II/E present on the FPGA board.

`communication.py` contains the python script to communicate between the PC Host and the NIOS II/E on board.

`DE10_LITE_Golden_Top.sof` contains the bitstream to flash onto the DE10-Lite containing an SOC with the following:
- NIOS II/E softcore.
- 64kB of onchip SRAM.
- JTAG UART
- Accelerometer SPI controller.
- HEX0, HEX1, HEX2 powered by individual 4-bit PIOs configured for output, with hexto7seg converting the 4-bit signal into a 7 bit signal.
- Buttons powered by PIOs configured for input.
- LEDs powered by PIOs configured for output.

All of this runs @50Mhz.

Latency results for fixed vs float calculated relative to the interval timer ticks (which ticks at about every 2304 cycles)
Notes about accelerometer data:
- Accelerometer data is filtered locally on board with a LPF configured for 25Hz corner frequency.
- Fixed point prefered over floating point implementation:
    - Because the NIOS II/E already lacks proper bitshift and multiply making floating point extrememly slow compared to fixed point (avg of 760000 cycles vs 160000 cycles for the filter implemented).
    - As a result of the fixed-point implementation, it is slightly glitchier than the floating-point implementation, but in real terms due to the higher responsiveness is more suitable for a controller, (and the 'glitchy' values are within single-digit percentages from the correct value).
    - The fixed point implementation uses a signed 9.7 integer-fraction format. This is a balance between maintaining full accelerometer precision, and having a faster filter.
- If the game where to poll the fpga and every time the fpga was polled it read the accelerometer and then did the filtering this would've been incredibly slow. Hence the firmware sets up an interval of 200000 cycles, where every 200000 cycles it will read and filter the accelerometer data. This is very infrequent (as 200000 * 20 * 10^-9 means a sample rate of about 250Hz), hence does not effect responsiveness of the fpga compared to the round trip time (mentioned later).
- Every time a read command 'P' is sent to the FPGA it simply reads in the last filtered read of the accelerometer. This changed response times from a few seconds to milliseconds of the user rotating the FPGA.
- The round trip time between the computer and the FPGA using the tool given to the cohort in the InfoProc labs was **1.5 seconds**, (we were warned about this), hence I decided to ditch the script and write a new one based of off the **intel_jtag_uart** library. The new script resulted in a round-trip latency between the PC and FPGA of just **110 milliseconds**, a 10x improvement. However due to constraints with regards to the speed of the JTAG UART, it cannot be improved further.


Command summary:
- Command **'U'**:
    - Arguments: **3-digit index, followed by 2-digit number**
    - Purpose: **Modify filter coefficient located at index to the 2-digit number following it.**
    - Returns: **New Line followed by CTRL+D**
- Command **'R'**:
    - Arguments: **3-digit index**
    - Purpose: **Readback filter coefficient located at index**
    - Returns: **Filter coefficient followed by newline followed by CTRL+D**
- Command **'C'**:
    - Arguments: **'0' or '1'**
    - Purpose: **Enable filtering if not '0', else disable filtering**
    - Returns: **CTRL+D**
- Command **'P'**:
    - Arguments: **None**
    - Purpose: **Read (filtered) accelerometer data**
    - Returns: **Integer capped between -256(leftmost) and 255(rightmost) followed by CTRL+D**
- Command **'L'**:
    - Arguments: **None**
    - Purpose: **None**
    - Returns: **CTRL+D**
- Default:
    - Arguments: **None**
    - Purpose: **None**
    - Returns: **Unhandled character followed by CTRL+D**


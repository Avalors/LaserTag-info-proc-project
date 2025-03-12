#include "alt_types.h"
#include "altera_avalon_jtag_uart.h"
#include "altera_avalon_jtag_uart_regs.h"
#include "altera_avalon_pio_regs.h"
#include "altera_avalon_timer.h"
#include "altera_avalon_timer_regs.h"
#include "altera_up_avalon_accelerometer_spi.h"
#include "os/alt_syscall.h"
#include "stdint.h"
#include "sys/alt_alarm.h"
#include "sys/alt_errno.h"
#include "sys/alt_irq.h"
#include "sys/times.h"
#include "system.h"
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#define CHARLIM                                                                \
  256 // Maximum character length of what the user places in memory.  Increase
      // to allow longer sequences
#define LENLM 2
#define IDXLM 3
#define QUITLETTER '~' // Letter to kill all processing
#define OFFSET -32
#define PWM_PERIOD 16
#define NO_OF_TAPS 107

alt_8 pwm = 0;
alt_u8 led;
alt_u8 mode = 0;
int counter = 100;
int level;
alt_up_accelerometer_spi_dev *acc_dev;
int buf[NO_OF_TAPS];
alt_32 x_read;
int32_t array_of_coeffs[NO_OF_TAPS] = {
    0,  0,  0,  0,  0,  0,  0,  0,  0, 0,  0,  0,  0,  0,  0,  0,  0,  1,
    1,  0,  0,  0,  0,  0,  0,  0,  0, -1, -1, -1, -1, -1, -1, -1, -1, -1,
    -1, -1, 0,  0,  1,  2,  2,  3,  4, 5,  6,  6,  7,  8,  8,  8,  9,  9,
    9,  8,  8,  8,  7,  6,  6,  5,  4, 3,  2,  2,  1,  0,  0,  -1, -1, -1,
    -1, -1, -1, -1, -1, -1, -1, -1, 0, 0,  0,  0,  0,  0,  0,  0,  1,  1,
    0,  0,  0,  0,  0,  0,  0,  0,  0, 0,  0,  0,  0,  0,  0,  0,  0};

int cast_fxp_to_int(int fxp) {
  int integer = (fxp >> 7);
  return integer;
}

int cast_short_to_fxp(int integer) {
  int fxp = integer << 7;
  return fxp;
}

int lpf_fir_filter(alt_32 sampl_value) {
  static int x = 0;
  buf[x % NO_OF_TAPS] = sampl_value;
  x++;
  int32_t output = 0;
  for (int y = 0; y < NO_OF_TAPS; y++) {
    output += (int32_t)array_of_coeffs[y] * (int32_t)buf[(x + y) % NO_OF_TAPS];
  }
  int32_t integer = output >> 14;
  return (int)(integer);
}

void sys_timer_isr() {
  IOWR_ALTERA_AVALON_TIMER_STATUS(TIMER_BASE, 0);
  IOWR_ALTERA_AVALON_TIMER_CONTROL(TIMER_BASE, 0x0000);
  alt_up_accelerometer_spi_read_y_axis(acc_dev, &x_read);
  if (mode)
    x_read = lpf_fir_filter(cast_short_to_fxp(x_read));
  IOWR_ALTERA_AVALON_TIMER_CONTROL(TIMER_BASE, 0x0007);
}

void timer_init(void *isr) {
  IOWR_ALTERA_AVALON_TIMER_CONTROL(TIMER_BASE, 0x0003);
  IOWR_ALTERA_AVALON_TIMER_STATUS(TIMER_BASE, 0);
  IOWR_ALTERA_AVALON_TIMER_PERIODL(TIMER_BASE, 0x0D40);
  IOWR_ALTERA_AVALON_TIMER_PERIODH(TIMER_BASE, 0x0003);
  alt_irq_register(TIMER_IRQ, 0, isr);
  IOWR_ALTERA_AVALON_TIMER_CONTROL(TIMER_BASE, 0x0007);
}

void h2dcomm() {
  char buffer;
  buffer = alt_getchar();
  if (buffer == 'U') {
    // Coefficient update
    char idxbuf[IDXLM + 1];
    idxbuf[IDXLM] = '\0';
    for (int x = 0; x < IDXLM; x++) {
      idxbuf[x] = alt_getchar();
    }
    int index_u = atoi(&idxbuf);
    char cfbuf[LENLM + 1];
    cfbuf[LENLM] = '\0';
    for (int x = 0; x < LENLM; x++) {
      cfbuf[x] = alt_getchar();
    }
    int coeff = atoi(&cfbuf);
    array_of_coeffs[index_u] = coeff;
    printf("\n%c", 0x4);
  } else if (buffer == 'R') {
    char idxbuf[IDXLM + 1];
    for (int x = 0; x < IDXLM; x++) {
      idxbuf[x] = 0;
    }
    for (int x = 0; x < IDXLM; x++) {
      idxbuf[x] = alt_getchar();
    }
    int index_u = atoi(&idxbuf);
    printf("%d%c", array_of_coeffs[index_u], 0x4);
  } else if (buffer == 'C') {
    buffer = alt_getchar();
    mode = buffer - 48;
    printf("\n%c", 0x4);
  } else if (buffer == 'S') {
    counter--;
    int div100 = counter / 100;
    int div10 = (counter - (div100 * 100)) / 10;
    IOWR_ALTERA_AVALON_PIO_DATA(HEX2_BASE, div100);
    IOWR_ALTERA_AVALON_PIO_DATA(HEX1_BASE, div10);
    IOWR_ALTERA_AVALON_PIO_DATA(HEX0_BASE,
                                counter - (div100 * 100) - (div10 * 10));
    printf("%c", 0x4);
  } else if (buffer == 'P') {
    int ro = x_read;
    if (ro > 255)
      ro = 255;
    else if (ro < -256)
      ro = -256;

    printf("%d%c", ro, 0x4);
  } else if (buffer == 'L') {
    printf("%c", 0x4);
  } else {
    printf("%c", buffer);
  }
}

int main() {

  acc_dev = alt_up_accelerometer_spi_open_dev("/dev/accelerometer_spi");

  if (acc_dev ==
      NULL) { // if return 1, check if the spi ip name is "accelerometer_spi"
    return 1;
  }
  int div100 = counter / 100;
  int div10 = (counter - (div100 * 100)) / 10;
  IOWR_ALTERA_AVALON_PIO_DATA(HEX2_BASE, div100);
  IOWR_ALTERA_AVALON_PIO_DATA(HEX1_BASE, div10);
  IOWR_ALTERA_AVALON_PIO_DATA(HEX0_BASE,
                              counter - (div100 * 100) - (div10 * 10));
  timer_init(sys_timer_isr);
  while (1) {
    h2dcomm();
  }

  return 0;
}

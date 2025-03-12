
import time as time
import intel_jtag_uart as jtag

c = 0
j = None
timeout = 50
lpf_coeffs = 107


def global_init() -> None:
    global j
    j = jtag.intel_jtag_uart()


def pad(arg: str, padnum: int) -> str:
    res = ""
    if (len(arg) < 3):
        num = padnum - len(arg)
        for x in range(0, num):
            res += '0'
    res += arg
    return res


def read_decode() -> str:
    global j
    return j.read().decode("utf-8")


def write_decode(cmd: str) -> None:
    global j
    j.write(cmd.encode("utf-8"))


def send_cmd(opcode: str, *args) -> None:
    global j
    global lpf_coeffs
    match opcode:
        case "P":
            write_decode("P")
        case "C":
            if (int(args[0]) > 1 or int(args[0]) < 0):
                raise Exception("Opcode C has wrong arguments")
            cmd = opcode+args[0]
            write_decode(cmd)
        case "R":
            if (int(args[0]) > lpf_coeffs):
                raise Exception("Opcode R has wrong arguments")
            arg = pad(args[0], 3)
            cmd = opcode[0]+arg
            write_decode(cmd)
        case "U":
            if (int(args[0]) > lpf_coeffs or int(args[1]) > 99):
                raise Exception("Opcode U has wrong arguments")
            arg1 = pad(args[0], 3)
            arg2 = pad(args[1], 2)
            cmd = opcode[0]+arg1+arg2
            write_decode(cmd)
        case _:
            write_decode(opcode)


def turn_on_filter() -> None:
    global j
    send_cmd("C", "1")
    j.read()


def controller_check_connection() -> int:
    global timeout
    while timeout != 0:
        send_cmd("a")
        if j.bytes_available() > 0:
            if (read_decode() == "a"):
                return 1
        timeout -= 1
    return 0


def controller_init() -> int:
    global j
    if controller_check_connection() == 0:
        raise Exception("Controller is not connected")
    turn_on_filter()
    for x in range(0, 200):
        send_cmd("P")
        while True:
            if (j.bytes_available() > 0):
                j.read()
                break


def connection_test() -> float:
    global j
    start = time.time()
    for num in range(0, 10):
        send_cmd("a")
        j.read()
        num -= 1
    end = time.time()
    return (end-start)/10


def get_accelerometer_data() -> int:
    global j
    send_cmd("P")
    return int(read_decode()[:-1])


def send_on_jtag():
    global c
    global j
    string = ""
    send_cmd("P")
    while True:
        string += read_decode()
        break
    print(string[:-1])
    j.flush()


def console():
    while True:
        testlink = False
        print("fpga>", end='')
        cmd = input("")
        if cmd[0:15] == "test_connection":
            if (cmd[16:27] == 'continuous'):
                testlink = True
            if testlink:
                while True:
                    print(
                        f"Connection test: Latency (in milliseconds): {connection_test()*1000:.6f}")
            print(
                f"Connection test: Latency (in milliseconds): {connection_test()*1000:.6f}")
        elif cmd[0:10] == "read_accel":
            if (cmd[11:21] == 'continuous'):
                testlink = True
            if (testlink):
                while True:
                    print(get_accelerometer_data())
            print(get_accelerometer_data())
        elif cmd[0:13] == "update_coeffs":
            arg1 = cmd[14:17].strip()
            arg2 = cmd[17:19].strip()
            send_cmd("U", arg1, arg2)
            read_decode()
        elif cmd[0:11] == "read_coeffs":
            arg = cmd[12:15].strip()
            send_cmd("R", arg)
            print(read_decode()[:-1])
        elif cmd == "filter_on":
            turn_on_filter()
        elif cmd == "connection_test":
            testlink = True
        elif cmd.lower() == "exit":
            break


def main():
    global_init()
    controller_init()
    turn_on_filter()
    console()


if __name__ == "__main__":
    main()

import socket
import time
print("We're in tcp client...")
# the server name and port client wishes to access
server_name = '127.0.0.1'
# '52.205.252.164'
server_port = 11999
# create a TCP client socket

# Set up a TCP connection with the server
# connection_socket will be assigned to this client on the server side
while True:
    msg = input("fpga>")
    if (msg == "exit"):
        break
    start = time.time()
    client_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    client_socket.connect((server_name, server_port))
# send the message to the TCP server
    client_socket.send(msg.encode())
# return values from the server
    msg = client_socket.recv(1024)
    print(msg.decode())
    client_socket.close()
    end = time.time()
    print(end-start)

using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEditor.Scripting.Python;
using System.Threading;
public class FPGAController : MonoBehaviour
{
    private string serverIP = "127.0.0.1";
    private int port = 11999;
    private TcpClient client;
    private NetworkStream stream;
    public bool connected = false;
    public int accelerometer_x = 0;
    public int accolerometer_y = 0;
    public int shooting_data = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ConnectFPGA();
    }
    private void ConnectFPGA()
    {
        client = new TcpClient(serverIP, port);
        stream = client.GetStream();

    }

    private void SendMessage(string message)
    {
        byte[] data = System.Text.Encoding.ASCII.GetBytes(message);
        stream.Write(data, 0, data.Length);
    }

    private string ReceiveMessage()
    {
        byte[] response = new byte[1024];
        Int32 responseBytes = stream.Read(response, 0, response.Length);
        return System.Text.Encoding.ASCII.GetString(response, 0, responseBytes);
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            
            SendMessage("read_state");

            string response = ReceiveMessage();
            
            string[] words = response.Split(' ');
            accelerometer_x = int.Parse(words[0]);
            accolerometer_y = int.Parse(words[1]);
            shooting_data = int.Parse(words[2]);
            
            connected = true;
        }
        catch (Exception e)
        {
            connected = false;
            Debug.Log(e);
            ConnectFPGA();
        }
    }
}

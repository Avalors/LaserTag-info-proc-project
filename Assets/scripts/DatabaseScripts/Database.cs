using UnityEngine;
using System;
using System.Net.Sockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;


public class Database : MonoBehaviour
{
    public static Database Instance { get; private set; }
    private string username = "";
    private TcpClient socket;
    private NetworkStream stream;
    private const string serverIP = "51.21.192.69";
    private const int serverPort = 12000;
    private bool connected = false;


    private void Awake()
    {
        // Ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Make it persist across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Connect();
    }

    private void Connect()
    {
        try
        {
            socket = new TcpClient(serverIP, serverPort);
            stream = socket.GetStream();
            connected = true;
            Debug.Log("Connected to server");
        }
        catch (Exception e)
        {
            connected = false;
        }
    }

    private JObject SendRequest(object requestData)
    {
        if (connected)
        {
            string jsonString = JsonConvert.SerializeObject(requestData);
            byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonString + "\n");

            stream.Write(jsonBytes, 0, jsonBytes.Length);
            byte[] responseBuffer = new byte[1024];
            int bytesRead = stream.Read(responseBuffer, 0, responseBuffer.Length);
            string responseData = Encoding.UTF8.GetString(responseBuffer, 0, bytesRead);

            JObject jsonResponse = JObject.Parse(responseData);
            return jsonResponse;
        }
        else
        {
            Debug.Log("Not connected to server");
            return null;
        }
        
    }

    public int AttemptLogIn(string username, string password)
    {
        try
        {

        
        JObject requestData = new JObject();
        requestData["type"] = "check_user_credentials";
        requestData["username"] = username;
        requestData["password"] = password;

        
        
        JObject response = SendRequest(requestData);
        if (response["status"].ToString() == "success")
        {
            if (response["signin"].ToString() == "True")
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        
        
        else
        {
            return -1;
        }


        }
        catch (Exception e)
        {
            return -1;
        }
    }


    public int AttemptSignUp(string username, string password)
    {
        try
        {

        
        JObject requestData = new JObject();
        requestData["type"] = "signup_user";
        requestData["username"] = username;
        requestData["password"] = password;

        
        
        JObject response = SendRequest(requestData);
        if (response["status"].ToString() == "success")
        {
            if (response["signup"].ToString() == "True")
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        
        
        else
        {
            return -1;
        }


        }
        catch (Exception e)
        {
            return -1;
        }
    }


    public JObject GetUserStats(string username)
    {
        try
        {

        
        JObject requestData = new JObject();
        requestData["type"] = "request_user_data";
        requestData["username"] = username;

        
        
        JObject response = SendRequest(requestData);
        if (response["status"].ToString() == "success")
        {
            JObject obj = response["data"] as JObject;
            return obj;
        }
        
        
        else
        {
            return null;
        }


        }
        catch (Exception e)
        {
            return null;
        }
    }

    public int SubmitScore(string username, string password, int kills, int deaths)
    {
        try
        {

        
        JObject requestData = new JObject();
        requestData["type"] = "submit_score";
        requestData["username"] = username;
        requestData["password"] = password;
        requestData["kills"] = kills;
        requestData["deaths"] = deaths;

        
        
        JObject response = SendRequest(requestData);
        if (response["status"].ToString() == "success")
        {
            if (response["submit"].ToString() == "True")
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        
        
        else
        {
            return -1;
        }


        }
        catch (Exception e)
        {
            return -1;
        }
    }
}

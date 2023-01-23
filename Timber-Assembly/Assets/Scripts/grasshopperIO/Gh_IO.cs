using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Gh_IO : MonoBehaviour
{
    [SerializeField] private bool send;
    [SerializeField] private string IP = "127.0.0.1";
    [SerializeField] private int port = 6000;
    [SerializeField] private string message = "hi, this message is from unity";

    private IPEndPoint _endPoint;
    private UdpClient _client;


    // Start is called before the first frame update
    void Start()
    {
        _endPoint = new IPEndPoint(IPAddress.Parse(IP), port);
        _client = new UdpClient();

        // status
        print("Sending to " + IP + " : " + port);
    }

    // Update is called once per frame
    void Update()
    {
        if (send)
        {
            UdpServer.Sender(message,_endPoint, _client);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;

public class UdpServer : MonoBehaviour
{
    public static void UdpSender(string data, string address, int port)
    {
        using (var client = new UdpClient(port))
        {
            byte[] dataBytes = System.Text.Encoding.UTF8.GetBytes(data);
            int dataSize = dataBytes.Length;
            client.Send(dataBytes, dataSize, address, port);
        }
    }
}

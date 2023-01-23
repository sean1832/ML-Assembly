using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;

public class UdpServer : MonoBehaviour
{
    public static void Sender(string message, IPEndPoint endPoint, UdpClient client)
    {
        try
        {
            // encode string to UTF8-coded bytes
            byte[] data = Encoding.UTF8.GetBytes(message);

            // send the data
            client.Send(data, data.Length, endPoint);

        }
        catch (Exception err)
        {
            print($"UDP Send error: {err.ToString()}");
        }
    }

    public static (string, bool) Receiver(int port, bool changeStatus)
    {
        UdpClient client = new UdpClient(port);
        string lastReceivedMessage = "";
        string previousMessage = "";
        bool messageChanged = changeStatus;
        while (true)
        {
            try
            {
                // receive bytes
                IPEndPoint IP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref IP);

                // decode
                string message = Encoding.UTF8.GetString(data);

                // manage changes, only update if change happens, else do nothing
                previousMessage = lastReceivedMessage;
                lastReceivedMessage = message;
                if (lastReceivedMessage != previousMessage)
                {
                    messageChanged = true;

                    // show message
                    print($"received message: [{message}]");
                }

                return (lastReceivedMessage, messageChanged);
            }
            catch (Exception ex)
            {
                print($"UDP Receiver error: {ex.ToString()}");
            }
        }
    }
}

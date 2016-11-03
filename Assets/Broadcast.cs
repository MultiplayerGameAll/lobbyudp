using UnityEngine;
using System.Collections;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System;

public class Broadcast {

    private static bool active;

    private static TcpListener socketReceiver;


    private static int getAvailableUdpPort()
    {
        return 9999;
    }

    public static void startBroadcast (Action<string> callbackAfter) {
        if (!active)
        {
            Debug.Log("Broadcast started!");
            active = true;
            Request request = new Request();
            request.type = "FIND_SERVER";
            request.port = getAvailableUdpPort();
            string json = JsonUtility.ToJson(request);

            Thread threadEnvioBroadcast = new Thread(() => {
                while (active)
                {
                    UdpClient client = new UdpClient();
                    IPEndPoint ip = new IPEndPoint(IPAddress.Broadcast, 20162);
                    byte[] bytes = Encoding.ASCII.GetBytes(json);
                    client.Send(bytes, bytes.Length, ip);
                    client.Close();
                    Debug.Log("    send package...");
                    Thread.Sleep(1000);
                }
                Debug.Log("Thread Sender Broadcast stoped");
            });
            threadEnvioBroadcast.Start();

            Thread threadReceiver = new Thread(()=> {
                if(socketReceiver == null)
                {
                    socketReceiver = new TcpListener(IPAddress.Any, getAvailableUdpPort());
                }
                socketReceiver.Start();

                TcpClient client = socketReceiver.AcceptTcpClient();

                NetworkStream streamServer = client.GetStream();
                Debug.Log("    tcp socket openned...");

                string message = "";
                int BUFFER_SIZE = 1024;
                byte[] buffer = new byte[BUFFER_SIZE];
                while (true)
                {
                    int bytesReceived = streamServer.Read(buffer, 0, BUFFER_SIZE);
                    message += Encoding.ASCII.GetString(buffer, 0, bytesReceived);
                    Debug.Log(message);
                    if (message.Contains("###"))
                    {
                        int splitpoint = message.IndexOf("###");
                        message = message.Substring(0, splitpoint);
                        break;
                    }
                }
                Debug.Log("Message: " + message);
                Debug.Log("Data received:" + message + "-");
                string ip = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                Debug.Log(ip);
                active = false;
                streamServer.Close();
                callbackAfter(ip);
            });
            threadReceiver.Start();

        }

    }

    public static void stop()
    {
        active = false;
        if(socketReceiver != null)
        {
            socketReceiver.Stop();
        }
    }
	
}

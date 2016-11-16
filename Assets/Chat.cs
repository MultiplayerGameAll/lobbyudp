using UnityEngine;
using System.Collections;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Text;

public class Chat {

    private string ip;

    private int port;

    private string nick;

    private TcpClient client;

    private NetworkStream stream;

    private Action<string> callbackMessage;

    private const int BUFFER_SIZE = 1024;

    private bool active;

    public static Chat startChat(string nick, string ip, int port, Action<string> callbackMessage)
    {
        Chat chat = new Chat();
        chat.ip = ip;
        chat.port = port;
        chat.nick = nick;
        chat.callbackMessage = callbackMessage;
        chat.connect();
        chat.startReceiving();
        return chat;
    }

    private void startReceiving()
    {
        Thread thread = new Thread(receive);
        thread.Start();
    }

    private void receive()
    {
        byte[] buffer = new byte[BUFFER_SIZE];
        string message = "";
        active = true;
        while (active)
        {
            int bytesReceived = stream.Read(buffer, 0, BUFFER_SIZE);
            message += Encoding.ASCII.GetString(buffer, 0, bytesReceived);
            if (message.Contains("###"))
            {

                int splitpoint = message.IndexOf("###");
                string msg = message.Substring(0, splitpoint);
                message = message.Substring(splitpoint + 3);

                callbackMessage(msg);
            }
        }

    }

    public void send(string message)
    {
        byte[] bytesToSend = Encoding.ASCII.GetBytes(message + "###");
        stream.Write(bytesToSend, 0, bytesToSend.Length);
        stream.Flush();
        Debug.Log(message);
    }

    private void connect()
    {
        this.client = new TcpClient(ip, port);
        this.stream = client.GetStream();
    }

    public void deactivate()
    {
        active = false;
        stream.Close();
        client.Close();
    }

}

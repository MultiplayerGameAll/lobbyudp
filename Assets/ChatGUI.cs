using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Collections.Generic;

public class ChatGUI : MonoBehaviour
{

    private string nick = "";
    private string ip;
    private int port;

    private string mode;

    private const string INIT = "INIT";
    private const string CHAT = "CHAT";

    private string messages = "";

    private string message = "";

    private Chat chat;

    Queue<Request> messagesQueue = new Queue<Request>();

    // Use this for initialization
    void Start()
    {
        Broadcast.startBroadcast(onConnect);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void onConnect(Connection con)
    {
        this.ip = con.host;
        this.port = con.port;
        Debug.Log("IP: " + ip + ":" + port);
        mode = INIT;
    }

    void OnGUI()
    {
        if(mode == INIT)
        {
            string txt = "IP do servidor: " + ip + ":" + port;
            GUI.Label(new Rect(10, 10, 500, 20), txt);
            GUI.Label(new Rect(10, 30, 50, 20), "Nick");
            nick = GUI.TextField(new Rect(60, 30, 200, 20), nick, 25);

            if (GUI.Button(new Rect(60, 60, 90, 20), "Entrar"))
            {
                this.chat = Chat.startChat(nick, ip, port, updateMessage);
                this.mode = CHAT;
            }
        }
        if (mode == CHAT)
        {
            GUI.Label(new Rect(10, 30, 50, 20), "Mensagem:");
            message = GUI.TextField(new Rect(60, 30, 200, 20), message, 25);
            if (GUI.Button(new Rect(270, 30, 100, 20), "Enviar"))
            {
                Request request = new Request();
                request.nick = nick;
                request.type = "SEND_CHAT";
                request.body = message;
                string json = JsonUtility.ToJson(request);
                this.chat.send(json);
                message = "";
            }
            while(messagesQueue.Count > 0)
            {
                Request req = messagesQueue.Dequeue();
                messages += req.nick + ":" + req.body + "\n";
            }
            GUI.TextArea(new Rect(10, 50, 300, 200), messages);
        }
    }



    void OnApplicationQuit()
    {
        Broadcast.stop();
        chat.deactivate();
        mode = null;
    }

    public void updateMessage(string msg)
    {
        Request req = JsonUtility.FromJson<Request>(msg);
        messagesQueue.Enqueue(req);
        Debug.Log(msg);
    }
}

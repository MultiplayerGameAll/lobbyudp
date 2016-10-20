using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Collections.Generic;

public class ChatGUI : MonoBehaviour
{

    private string nick = "";
    private string ip = "localhost";
    private int port = 9000;

    private List<ThreadReaderMessage> readers = new List<ThreadReaderMessage>();

    private string mode = INIT;

    private const string INIT = "INIT";
    private const string SERVER_STARTED = "SERVER_STARTED";
    private const string CLIENT_STARTED = "CLIENT_STARTED";

    private string mensagens = "";

    private string mensagem = "";

    // Use this for initialization
    void Start()
    {
        Request req = new Request();
        req.body = "corpo";
        req.nick = "teste";
        req.type = "tipo";

        Debug.Log(JsonUtility.ToJson(req));

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnGUI()
    {
        if(mode == INIT)
        {
            GUI.Label(new Rect(10, 10, 50, 20), "Nick");
            nick = GUI.TextField(new Rect(60, 10, 200, 20), nick, 25);

            if (GUI.Button(new Rect(60, 60, 90, 20), "Entrar"))
            {
                Broadcast.startBroadcast(nick);
            }
        }

    }

    private void sendMessage(string message)
    {
        if (mode == SERVER_STARTED)
        {
            foreach (ThreadReaderMessage trm in readers)
            {
                trm.sendMessage(message);
            }
        }
        else if (mode == CLIENT_STARTED)
        {

        }
    }

    private void startServer()
    {
        TcpListener listener = null;
        Debug.Log("Server started.");

        try
        {
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            while (mode == SERVER_STARTED)
            {
                TcpClient client = client = listener.AcceptTcpClient();
                NetworkStream streamServer = client.GetStream();
                ThreadReaderMessage trm = new ThreadReaderMessage(nick, streamServer);
                readers.Add(trm);
                Thread thread = new Thread(trm.read);
                thread.Start();


            }
            listener.Stop();
        }
        catch (SocketException e)
        {
            Debug.Log("Error on open socket. " + e.Message);
        }
    }

    private void atualizarMensagens()
    {
        while(mode == SERVER_STARTED || mode == CLIENT_STARTED)
        {
            List<Request> mensagensList = new List<Request>();
            foreach(ThreadReaderMessage trm in readers)
            {
                mensagensList.AddRange(trm.getMensagens());
            }
            mensagensList.Sort(delegate (Request i1, Request i2)
            {
                return (int)(i1.instant - i2.instant);
            });

            string mensagensProv = "";
            foreach(Request msg in mensagensList)
            {
                mensagensProv += msg.nick + ": " + msg.body + "\n";
            }

            mensagens = mensagensProv;

            Thread.Sleep(500);
        }
    }

    void OnApplicationQuit()
    {
        Broadcast.stop();
        mode = INIT;
        foreach (ThreadReaderMessage trm in readers)
        {
            trm.deactive();
        }
    }
}

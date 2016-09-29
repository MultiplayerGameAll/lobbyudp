using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System;

public class ThreadReaderMessage {

    private NetworkStream stream;

    /**
     * Tamanho do buffer de leitura
     */
    private const int BUFFER_SIZE = 1024;

    private List<Mensagem> mensagens = new List<Mensagem>();

    private bool active = true;

    private string nick;


    public ThreadReaderMessage(NetworkStream stream)
    {
        this.stream = stream;
    }

    public void deactive()
    {
        active = false;
    }

    public void read()
    {
        byte[] buffer = new byte[BUFFER_SIZE];
        string message = "";
        while (active)
        {
            int bytesReceived = stream.Read(buffer, 0, BUFFER_SIZE);
            message += Encoding.ASCII.GetString(buffer, 0, bytesReceived);
            if (message.Contains("###"))
            {
                
                int splitpoint = message.IndexOf("###");
                string msg = message.Substring(0, splitpoint);
                Mensagem msgObj = new Mensagem();
                msgObj.message = msg;
                msgObj.nick = nick;
                message = message.Substring(splitpoint + 3);
                mensagens.Add(msgObj);
                Debug.Log(msg);
            }
        }
    }

    public List<Mensagem> getMensagens()
    {
        return mensagens;
    }
}

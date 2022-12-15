using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Net;
using System.IO;
using UnityEngine.Events;

public class Server : MonoBehaviour
{
    public UnityEvent<string> onSpeak;
    public UnityEvent<string> onCommand;

    public string port;
    IAsyncResult status;
    HttpListener server;

    private string speakQueue;
    private string commandQueue;
    private void Start()
    {
        Application.runInBackground = true;
        server = new HttpListener();
        server.Prefixes.Add("http://localhost:"+port+"/");
        server.Start();
        status = server.BeginGetContext(HandleRequest, server);
    }
    private void Update()
    {
        if (!String.IsNullOrEmpty(speakQueue))
        {
            onSpeak.Invoke(speakQueue);
            speakQueue = "";
        }
        if (!String.IsNullOrEmpty(commandQueue))
        {
            onCommand.Invoke(commandQueue);
            commandQueue = "";
        }
    }

    private void HandleRequest(IAsyncResult result)
    {
        Debug.Log("handling request");
        var server = (HttpListener)result.AsyncState;
        var context = server.EndGetContext(result);
        var request = context.Request;
        var response = context.Response;

        var requestBody = new StreamReader(request.InputStream).ReadToEnd();

        var json = JsonUtility.FromJson<CommandRequest>(requestBody);

        speakQueue = json.text;
        commandQueue = json.command;
        response.StatusCode = 200;
        response.ContentType = "text/plain";
        response.Close();
        Debug.Log("finished handling request");
        server.BeginGetContext(HandleRequest, server);
        return;
    }

    public class CommandRequest
    {
        public string command;
        public string text;
    }
}
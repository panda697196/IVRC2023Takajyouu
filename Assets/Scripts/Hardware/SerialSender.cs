using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerialSender : INexusRobotSend
{
    [SerializeField] private MySerialHandler _serialHandler;

    public void Send(byte[] msg)
    {
        _serialHandler.Write(msg);
    }

    public void Send(string msg)
    {
        _serialHandler.Write(msg);
    }

    public void Send(char msg)
    {
        _serialHandler.Write(msg);
    }

    // Start is called before the first frame update
    public SerialSender(MySerialHandler handler)
    {
        _serialHandler = handler;
        _serialHandler.OnRecieveMessage += (msg) => Debug.Log(msg);
    }
}

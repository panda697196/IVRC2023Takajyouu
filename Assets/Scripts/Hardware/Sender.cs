using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sender : MonoBehaviour
{
    [SerializeReference] private INexusRobotSend _sender;
    [SerializeField] private PullInspector _pullInspector;
    [SerializeField] private MySerialHandler mySerialHandler;

    // Start is called before the first frame update
    void Start()
    {
        _sender = new SerialSender(mySerialHandler);
    }

    // Update is called once per frame
    void Update()
    {
        if(mySerialHandler._isNewMessageReceived)
        {
            if (mySerialHandler._message == "AS")
                _pullInspector.DetectPull();
        }
    }

    public void DataSend(string command)
    {
        _sender?.Send(command);
    }
}

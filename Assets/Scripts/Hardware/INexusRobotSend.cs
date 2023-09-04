using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface INexusRobotSend
{
    void Send(byte[] msg);
    void Send(string msg);
    void Send(char msg);
}

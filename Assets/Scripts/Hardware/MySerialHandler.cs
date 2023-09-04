using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports; //エラーが出る場合は，unityでEdit>ProjectSettings>Player>OtherSettings>Configurationで.Netを4xにする
using UnityEngine;

public class MySerialHandler : MonoBehaviour
{
    public Action<string> OnRecieveMessage;

    /*private*/ public bool _isNewMessageReceived = false;
    /*private*/ public string _message;
    private char[] mess = { 'A' };

    private SerialPort _serialPort;
    [SerializeField] private int _bandRate = 115200;
    [SerializeField] private string _portName = "COM6";

    private void Awake()
    {
        Initalize();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isNewMessageReceived)
        {
            OnRecieveMessage?.Invoke(_message);
            //Debug.Log(_message);
            _isNewMessageReceived = false;
        }
    }

    private void OnDestroy()
    {
        Close();
    }

    private void Initalize()
    {
        _serialPort = new SerialPort(_portName, _bandRate, Parity.None, 8, StopBits.One);
        Open();
        Task.Run(Read);
    }

    private void Open()
    {
        _serialPort.Handshake = Handshake.None;
        _serialPort.ReadTimeout = 500;
        _serialPort.WriteTimeout = 500;
        _serialPort.Open();
    }

    private void Close()
    {
        if (_serialPort != null && _serialPort.IsOpen)
        {
            _serialPort.Close();
        }
    }

    /*private*/ public void Read()
    {
        while (_serialPort != null && _serialPort.IsOpen)
        {
            try
            {
                // if (serialPort_.BytesToRead > 0) {
                _message = _serialPort.ReadLine();
                _isNewMessageReceived = true;
                // }
            }
            catch (System.Exception e)
            {
                //Debug.LogWarning(e.Message);
            }
        }
    }

    public void Write(string message)
    {
        try
        {
            _serialPort.Write(message);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e.Message);
        }
    }

    public void Write(char mes)
    {
        mess[0] = mes;
        try
        {
            _serialPort.Write(mess, 0, 1);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e.Message);
        }
    }

    public void Writeln(string message)
    {
        try
        {
            _serialPort.Write(message + "\n");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e.Message);
        }
    }

    public void Write(byte[] b)
    {
        _serialPort.Write(b, 0, b.Length);
        //Debug.Log("Send");
    }
}

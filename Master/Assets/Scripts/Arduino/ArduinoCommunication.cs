using UnityEngine;
using System;
using System.Collections;
using System.IO.Ports;

public class ArduinoCommunication : MonoBehaviour {

    SerialPort sp = new SerialPort("COM5", 9600);
 
    void Start () {
        sp.Open ();
        sp.ReadTimeout = 1;
    }
 
    void Update () 
    {
        try{
            print (sp.ReadLine());
        }
        catch(Exception ex){
            Debug.Log(ex);
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEngine;

namespace ArduinoUnityConnection
{
  public class WifiConnection
  {
    public float CurrentValue;

    public void Begin(string ipAddress, int port)
    {
      // Give the network stuff its own special thread
      var thread = new Thread(() =>
      {

        //network stuff
        var client = new TcpClient();

        //Read IP and port from arduino terminal
        client.Connect(ipAddress, port);

        NetworkStream networkStream = client.GetStream();

        Byte[] data = Encoding.ASCII.GetBytes("Mathias");
        
        networkStream.Flush();
        networkStream.Write(data,0,data.Length);
        
        var stream = new StreamReader(networkStream);
        

        // We'll read values and buffer them up in here
        var buffer = new List<byte>();

        return;
        
        while (client.Connected)
        {
          // Read the next byte
          var read = stream.Read();
          
          // We split readings with a carriage return, so check for it 
          if (read == 13)
          {
            // Once we have a reading, convert our buffer to a string, since the values are coming as strings
            var str = Encoding.ASCII.GetString(buffer.ToArray());

            // We assume that they're floats
            var dist = float.Parse(str);

            CurrentValue = dist;
          
            // Clear the buffer ready for another reading
            buffer.Clear();
          }
          else
            // If this wasn't the end of a reading, then just add this new byte to our buffer
            buffer.Add((byte)read);
        }
      });
    
      thread.Start();
    }
  }
  
}
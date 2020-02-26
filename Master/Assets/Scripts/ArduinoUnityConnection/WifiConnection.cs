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

    // The range of inputs we expect. In this case we're saying it'll be between 0 and 30cm
    float _minInputY = 0f;
    float _maxInputY = 30;

    // The Y range that the plane can move within
    float _minFinalY = -4f;
    float _maxFinalY = 4;

    public void Begin(string ipAddress, int port)
    {
      // Give the network stuff its own special thread
      var thread = new Thread(() =>
      {

        // This class makes it super easy to do network stuff
        var client = new TcpClient();

        // Change this to your devices real address
        client.Connect(ipAddress, port);
        var stream = new StreamReader(client.GetStream());

        // We'll read values and buffer them up in here
        var buffer = new List<byte>();
        while (client.Connected)
        {
          // Read the next byte
          var read = stream.Read();
          
          
          
          // We split readings with a carriage return, so check for it 
          if (read == 13)
          {
            // Once we have a reading, convert our buffer to a string, since the values are coming as strings
            var str = Encoding.ASCII.GetString(buffer.ToArray());

            Debug.Log("str: " + str);
            
            // We assume that they're floats
            var dist = float.Parse(str);

            // Ignore any value outside of our expected input range
            dist = Mathf.Clamp(dist, _minInputY, _maxInputY);
          
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
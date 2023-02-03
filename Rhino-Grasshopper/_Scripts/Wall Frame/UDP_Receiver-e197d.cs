using System;
using System.Collections;
using System.Collections.Generic;

using Rhino;
using Rhino.Geometry;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using System.Drawing.Printing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;


/// <summary>
/// This class will be instantiated on demand by the Script component.
/// </summary>
public abstract class Script_Instance_e197d : GH_ScriptInstance
{
  #region Utility functions
  /// <summary>Print a String to the [Out] Parameter of the Script component.</summary>
  /// <param name="text">String to print.</param>
  private void Print(string text) { /* Implementation hidden. */ }
  /// <summary>Print a formatted String to the [Out] Parameter of the Script component.</summary>
  /// <param name="format">String format.</param>
  /// <param name="args">Formatting parameters.</param>
  private void Print(string format, params object[] args) { /* Implementation hidden. */ }
  /// <summary>Print useful information about an object instance to the [Out] Parameter of the Script component. </summary>
  /// <param name="obj">Object instance to parse.</param>
  private void Reflect(object obj) { /* Implementation hidden. */ }
  /// <summary>Print the signatures of all the overloads of a specific method to the [Out] Parameter of the Script component. </summary>
  /// <param name="obj">Object instance to parse.</param>
  private void Reflect(object obj, string method_name) { /* Implementation hidden. */ }
  #endregion

  #region Members
  /// <summary>Gets the current Rhino document.</summary>
  private readonly RhinoDoc RhinoDocument;
  /// <summary>Gets the Grasshopper document that owns this script.</summary>
  private readonly GH_Document GrasshopperDocument;
  /// <summary>Gets the Grasshopper script component that owns this script.</summary>
  private readonly IGH_Component Component;
  /// <summary>
  /// Gets the current iteration count. The first call to RunScript() is associated with Iteration==0.
  /// Any subsequent call within the same solution will increment the Iteration count.
  /// </summary>
  private readonly int Iteration;
  #endregion
  /// <summary>
  /// This procedure contains the user code. Input parameters are provided as regular arguments,
  /// Output parameters as ref arguments. You don't have to assign output parameters,
  /// they will have a default value.
  /// </summary>
  #region Runscript
  private void RunScript(bool run, bool receive, string IP, int port, ref object Result)
  {
    if (run)
    {
      // create a new IP end point
      IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
      using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
      {
        socket.Bind(ipEndPoint);

        Thread listenThread = new Thread(() =>
        {
          string outResult = ReceiveData(IP, port, socket, receive);
          Print(outResult);
        });
        listenThread.Start();

        if (receive == false)
        {
          listenThread.Abort();
          Print("Thread is Abort");
        }
      }


    }
  }
  #endregion
  #region Additional

  public static string ReceiveData(string IP, int port, Socket socket, bool receiveCtrl)
  {
    while (true)
    {
      if (!receiveCtrl)
      {
        break;
      }
      // receive message from server
      byte[] receiveBuffer = new byte[4096];
      int bytesReceived = socket.Receive(receiveBuffer);
      string message = Encoding.UTF8.GetString(receiveBuffer, 0, bytesReceived);

      return message;
    }
    return null;
  }
  #endregion
}
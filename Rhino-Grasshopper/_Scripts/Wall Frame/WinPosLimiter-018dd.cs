using System;
using System.Collections;
using System.Collections.Generic;

using Rhino;
using Rhino.Geometry;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;



/// <summary>
/// This class will be instantiated on demand by the Script component.
/// </summary>
public abstract class Script_Instance_018dd : GH_ScriptInstance
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
  private void RunScript(Point3d wallSize, Point3d winPos, Point3d winSize, ref object uv)
  {
    Point2d maxPosUv = new Point2d();
    double wallXSize = wallSize.X;
    double wallYSize = wallSize.Y;
    double winXPerPos = winPos.X;
    double winYPerPos = winPos.Y;
    double winXSize = winSize.X;
    double winYSize = winSize.Y;

    double winXPos = winXPerPos * wallXSize;
    double winYPos = winYPerPos * wallYSize;

    double WinXMinPos = winXSize * 0.5;
    double winYMinPos = winYSize * 0.5;

    double winXMaxPos = wallXSize - WinXMinPos;
    double winYMaxPos = wallYSize - winYMinPos;

    Print(winXPos.ToString());
    // Horizontal limiter (X)
    if (winXPos < WinXMinPos) winXPos = WinXMinPos; // minimum limiter
    else if (winXPos > winXMaxPos) winXPos = winXMaxPos; // maximum limiter


    // Vertical limiter (y)
    if (winYPos < winYMinPos) winYPos = winYMinPos; // minimum limiter
    else if (winYPos > winYMaxPos) winYPos = winYMaxPos; // maximum limiter

    maxPosUv.X = winYPos / wallYSize;
    maxPosUv.Y = winXPos / wallXSize;
    uv = maxPosUv;

  }
  #endregion
  #region Additional

  #endregion
}
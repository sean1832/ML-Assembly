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
public abstract class Script_Instance_f06f2 : GH_ScriptInstance
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
  private void RunScript(Rectangle3d wallBound, List<Point3d> center, List<Rectangle3d> rectangle, double offset, ref object A)
  {
    for (int i = 0; i < center.Count; i++)
    {
      // point A
      Point3d ptA = center[i];
      Rectangle3d recA = rectangle[i];
      for (int j = i+1; j < center.Count; j++)
      {
        // point B
        Point3d ptB = center[j];
        Rectangle3d recB = rectangle[j];

        // safe distance
        double winSafeDistanceX = recA.Height*0.5 + recB.Height*0.5 + offset;
        double winDistanceX = Math.Abs(ptA.X - ptB.X);


        if (!(winDistanceX <= winSafeDistanceX)) continue;
        // if distances is less than safe distance

        Point3d wallCenter = wallBound.Center;
        double wallSafeDistanceX;
        double wallDistanceX;
        center.Remove(ptB);
        double difference = winSafeDistanceX - winDistanceX;
        if (ptA.X > ptB.X) // ptB is left
        {
          //ptB.X = ptB.X - difference;
          wallSafeDistanceX = (wallBound.Width * 0.5) - (recB.Height * 0.5);
          wallDistanceX = Math.Abs(wallCenter.X - (ptB.X - difference));
          if (wallDistanceX <= wallSafeDistanceX)
          {
            ptB.X = ptB.X - difference;
          }
          else
          {
            center.Remove(ptA);
            ptA.X = ptA.X + difference;
            center.Insert(i, ptA);
          }
        }
        else // ptB is right
        {
          wallSafeDistanceX = (wallBound.Width * 0.5) - (recB.Height * 0.5);
          wallDistanceX = Math.Abs(wallCenter.X - (ptB.X + difference));
          if (wallDistanceX <= wallSafeDistanceX)
          {
            ptB.X = ptB.X + difference;
          }
          else
          {
            center.Remove(ptA);
            ptA.X = ptA.X - difference;
            center.Insert(i, ptA);
          }
        }
        center.Insert(j,ptB);
      }
    }

    A = center;
  }
  #endregion
  #region Additional
  
  #endregion
}
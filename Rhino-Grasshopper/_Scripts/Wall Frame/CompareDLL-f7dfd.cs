using System;
using System.Collections;
using System.Collections.Generic;

using Rhino;
using Rhino.Geometry;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using TimberAssembly;
using TimberAssembly.Component;


/// <summary>
/// This class will be instantiated on demand by the Script component.
/// </summary>
public abstract class Script_Instance_f7dfd : GH_ScriptInstance
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
  private void RunScript(List<string> salvageData, List<string> TargetFrameData, double tolerance, bool EnableSecondMatch, ref object pairDatas, ref object D1, ref object D2)
  {
    List<Agent> salvageAgents = Parser.ParseAgents(salvageData);
    List<Agent> targetAgents = Parser.ParseAgents(TargetFrameData);

    Match matchOperation = new Match(salvageAgents, targetAgents, tolerance);

    Remain remainFirst = new Remain();
    Remain remainSecond = new Remain();

    List<MatchPair> matchedPairs = matchOperation.ExactMatch(out remainFirst);
    matchedPairs.AddRange(matchOperation.SecondMatch(remainFirst, out remainSecond));

    List<Point3d> debugDimension1 = new List<Point3d>(); 

    List<Point3d> debugDimension2 = new List<Point3d>();

    List<Agent> targets = remainFirst.Targets;
    List<Agent> salvages = remainFirst.Subjects;

    foreach (Agent target in targets)
    {
      debugDimension1.Add(new Point3d(target.Dimension.Length, target.Dimension.Width, target.Dimension.Height));
    }

    foreach (Agent salvage in salvages)
    {
      debugDimension2.Add(new Point3d(salvage.Dimension.Length, salvage.Dimension.Width, salvage.Dimension.Height));
    }
    D1 = debugDimension1;
    D2 = debugDimension2;

    //matchedPairs.AddRange(matchOperation.RemainMatch(remainFirst));



  }
  #endregion
  #region Additional

  #endregion
}
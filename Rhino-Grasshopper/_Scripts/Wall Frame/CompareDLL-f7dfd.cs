using System;
using System.Collections;
using System.Collections.Generic;

using Rhino;
using Rhino.Geometry;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using System.Linq;
using System.Reflection;
using TimberAssembly;
using TimberAssembly.Entities;


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
  private void RunScript(List<string> salvageData, List<string> TargetFrameData, double tolerance, ref object pairDatas, ref object remains)
  {
    // version display
    Assembly assembly = Assembly.GetAssembly(typeof(TimberAssembly.Match));
    string version = assembly.GetName().Version.ToString();
    Component.Message = "Ver " + version;

    // deserialize
    List<Agent> salvageAgents = Parser.DeserializeToAgents(salvageData);
    List<Agent> targetAgents = Parser.DeserializeToAgents(TargetFrameData);


    // tolerance limit
    if (tolerance > smallestDimension(salvageAgents))
    {
      string message = "Tolerance is too large. Tolerance is set to " + smallestDimension(salvageAgents).ToString();
      Component.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, message);
      tolerance = smallestDimension(salvageAgents);
    }


    Match matchOperation = new Match(targetAgents, salvageAgents, tolerance);

    Remain remainFirst = new Remain();
    Remain remainSecond = new Remain();
    Remain remainThird = new Remain();

    List<Pair> matchedPairs = matchOperation.ExactMatch(out remainFirst);
    matchedPairs.AddRange(matchOperation.SecondMatchSlow(remainFirst, out remainSecond));
    matchedPairs.AddRange(matchOperation.CutToTarget(remainSecond, out remainThird));
    //matchedPairs.AddRange(matchOperation.RemainMatch(remainThird));
    matchedPairs.AddRange(matchOperation.ExtendToTarget(ref remainThird));

    //List<Pair> nonTrimmedPairs = new List<Pair>();

    //foreach (var pair in matchedPairs)
    //{
    //  foreach (var subject in pair.Subjects)
    //  {
    //    if (subject.Trimmed > 0) continue;
    //    nonTrimmedPairs.Add(pair);
    //    break;
    //  }
    //}

    pairDatas = matchedPairs;
    remains = remainThird;
  }
  #endregion
  #region Additional

  private double smallestDimension(List<Agent> agents)
  {
    double smallestDimension = double.MaxValue;
    foreach (var agent in agents)
    {
      if (agent.Dimension.ToList().Min() < smallestDimension)
      {
        smallestDimension = agent.Dimension.ToList().Min();
      }
    }
    return smallestDimension;
  }
  #endregion
}
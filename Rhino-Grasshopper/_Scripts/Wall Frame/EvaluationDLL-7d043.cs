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
using TimberAssembly;
using TimberAssembly.Entities;


/// <summary>
/// This class will be instantiated on demand by the Script component.
/// </summary>
public abstract class Script_Instance_7d043 : GH_ScriptInstance
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
  private void RunScript(List<object> pairsData, object remainsData, List<string> initSalvageTimber, ref object score, ref object cutCounts, ref object newSubjectVolume, ref object recycleRate, ref object wasteRate, ref object materialEfficiency, ref object laborEfficiency, ref object timeEfficiency)
  {
    List<Agent> initSalvage = Parser.DeserializeToAgents(initSalvageTimber);
    Remain remain = (Remain)remainsData;
    List<Pair> pairs = pairsData.OfType<Pair>().ToList();

    Evaluate evaluate = new Evaluate(pairs, remain, initSalvage);

    cutCounts = evaluate.GetCutCount();
    newSubjectVolume = evaluate.GetNewSubjectVolume();
    recycleRate = evaluate.GetRecycleRateVolume();
    wasteRate = evaluate.GetWasteRateByVolume();
    materialEfficiency = evaluate.EvaluateEfficiencyByVolume();
    laborEfficiency = evaluate.EvaluateEfficiencyByCutCount();
    timeEfficiency = evaluate.EvaluateEfficiencyByTime(0.2, 0.05);

    score = GetOverallScore(evaluate, 0.2, 0.05);
  }
  #endregion
  #region Additional

  private double GetOverallScore(Evaluate eval, double timePerSubject, double timePerCut)
  {
    // These weights should add up to 1
    double wasteRateWeight = 0.2;
    double recycleRateWeight = 0.25;
    double materialEfficiencyWeight = 0.25;
    double laborEfficiencyWeight = 0.15;
    double timeEfficiencyWeight = 0.15;

    // Calculate each component of the overall score
    double wasteRateScore = (1 - eval.GetWasteRateByVolume()) * wasteRateWeight;
    double recycleRateScore = eval.GetRecycleRateVolume() * recycleRateWeight;
    double materialEfficiencyScore = eval.EvaluateEfficiencyByVolume() * materialEfficiencyWeight;
    double laborEfficiencyScore = eval.EvaluateEfficiencyByCutCount() * laborEfficiencyWeight;
    double timeEfficiencyScore = eval.EvaluateEfficiencyByTime(timePerSubject, timePerCut) * timeEfficiencyWeight;

    // Sum up all the components to get the overall score
    double overallScore = wasteRateScore + recycleRateScore + materialEfficiencyScore + laborEfficiencyScore + timeEfficiencyScore;

    return overallScore;
  }
  #endregion
}
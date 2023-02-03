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
using System.Reflection.Emit;
using System.Threading.Tasks;
using System.Xml.Linq;
using Rhino.FileIO;


/// <summary>
/// This class will be instantiated on demand by the Script component.
/// </summary>
public abstract class Script_Instance_20889 : GH_ScriptInstance
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
  private void RunScript(List<string> salvageData, List<double> frameT, double tolerance, bool EnableDoubleMatch, ref object pairDatas)
  {
    // (frameLength)(<salvageName1, salvageLength1><...>)

    pairDatas = FindCombinationBinary(frameT, salvageData, tolerance, EnableDoubleMatch);
  }
  #endregion
  #region Additional

  private List<string> FindCombination(List<double> targetLengths, List<string>salData, double tolerance, bool doubleMatch)
  {
    List<string> matchedList = new List<string>();

    List<string> serializedData = new List<string>();

    // iterate through target frames
    for (int z = 0; z < targetLengths.Count; z++)
    {
      var targetLength = targetLengths[z];
      targetLength = Math.Round(targetLength, 2);
      var targetName = "Tar" + z;

      // iterate through first salvage timber
      for (int i = 0; i < salData.Count; i++)
      {
        string salAData = salData[i];
        string[] salADataSplit = salAData.Split(new string[] { "::" }, StringSplitOptions.None);
        string salAName = salADataSplit[0];
        double salAValue = double.Parse(salADataSplit[1]);

        // solo pair condition
        bool inMatchListA = matchedList.Contains(salAName);
        bool matchedSolo = MatchSolo(salAValue, targetLength, tolerance);


        //if (matchedSolo && !inMatchListA && !doubleMatch)
        if (matchedSolo && !inMatchListA)
        {
          matchedList.Add(salAName);

          string dataSolo = Serializer(targetName, targetLength, salAName, salAValue);
          dataSolo = "{MinCut}" + dataSolo;
          serializedData.Add(dataSolo);

          // break compare loop
          break;
        }

        // double match (heavy)
        if (!doubleMatch) continue;

        // iterate through second salvage timber
        for (int j = i + 1; j < salData.Count; j++)
        {
          string salBData = salData[j];
          string[] salBDataSplit = salBData.Split(new string[] { "::" }, StringSplitOptions.None);
          string salBName = salBDataSplit[0];
          double salBValue = double.Parse(salBDataSplit[1]);

          // conditions
          bool matched = MatchDouble(salAValue, salBValue, targetLength, tolerance);
          bool inMatchListB = matchedList.Contains(salBName);

          // if match
          if (!matched || inMatchListA || inMatchListB) continue;
          matchedList.Add(salAName);
          matchedList.Add(salBName);

          string dataDouble = Serializer(targetName, targetLength, salAName, salAValue, salBName, salBValue);
          dataDouble = "{MinCut}" + dataDouble;
          serializedData.Add(dataDouble);
          // break compare loop
          goto CompareExit;
        }

      }
      // exit compare loop
      CompareExit:
        ;
    }

    return serializedData;
  }

  private List<string> FindCombinationParallel(List<double> targetLengths, List<string> salData, double tolerance, bool doubleMatch)
  {

    List<string> matchedSal = new List<string>();
    List<string> matchedTarget = new List<string>();

    List<string> serializedData = new List<string>();
    
    // iterate through target frames
    for(int z=0; z<targetLengths.Count; z++)
    {
      var targetLength = targetLengths[z];
      targetLength = Math.Round(targetLength, 2);
      var targetName = "Tar" + z;

      bool breakInner = false;

      Parallel.For(0, salData.Count, (i, stateI) =>
      {
        if (breakInner)
        {
          stateI.Break();
        }

        string salAData = salData[i];
        string salAName;
        double salAValue;
        DeSerializer(salAData, out salAValue, out salAName);


        // solo pair condition
        bool inMatchListA = matchedSal.Contains(salAName);
        bool inMatchTarget = matchedTarget.Contains(targetName);
        bool matchedSolo = MatchSolo(salAValue, targetLength, tolerance);


        //if (matchedSolo && !inMatchListA && !doubleMatch)
        if (matchedSolo && !inMatchListA && !inMatchTarget)
        { 
          matchedSal.Add(salAName);
          matchedTarget.Add(targetName);

          string dataSolo = Serializer(targetName, targetLength, salAName, salAValue);
          dataSolo = "{MinCut}" + dataSolo;
          serializedData.Add(dataSolo);

          // break compare loop
          stateI.Break();
        }

        // double match (heavy) ** if not (double Match) continue
        if (!doubleMatch) return;

        Parallel.For(i + 1, salData.Count, (j, stateJ) =>
        {
          string salBData = salData[j];
          string salBName;
          double salBValue;
          DeSerializer(salBData, out salBValue, out salBName);


          // conditions
          bool matched = MatchDouble(salAValue, salBValue, targetLength, tolerance);
          bool inMatchListB = matchedSal.Contains(salBName);

          // if match
          if (!matched || inMatchListA || inMatchListB || inMatchTarget) return;
          matchedSal.Add(salAName);
          matchedSal.Add(salBName);

          string dataDouble = Serializer(targetName, targetLength, salAName, salAValue, salBName, salBValue);
          dataDouble = "{MinCut}" + dataDouble;
          serializedData.Add(dataDouble);

          // break compare loop
          breakInner = true;
          stateJ.Break();
        });

      });

    }

    return serializedData;
  }

  private List<string> FindCombinationBinary(List<double> targetLengths, List<string> salData, double tolerance, bool doubleMatch)
  {
    List<string> matchedList = new List<string>();

    List<string> serializedData = new List<string>();

    Dictionary<string, double> targetDict = DeSerializer(targetLengths);
    Dictionary<string, double> salvageDict = DeSerializer(salData);

    Dictionary<string, double> singleRemainTargets = new Dictionary<string, double>();
    Dictionary<string, double> singleRemainSal = new Dictionary<string, double>();

    Dictionary<string, double> doubleRemainTargets = new Dictionary<string, double>();
    Dictionary<string, double> doubleRemainSal = new Dictionary<string, double>();

    // single matching
    Dictionary<string, string> singleMatchedDict = singleMatch(
      targetDict,
      salvageDict,
      tolerance,
      out singleRemainTargets,
      out singleRemainSal);

    // serialization of data
    foreach (var match in singleMatchedDict)
    {
      string dataStr = Serializer(match.Key, targetDict[match.Key], match.Value, salvageDict[match.Value]);
      double offcuts = CalculateOffcuts(targetDict[match.Key], salvageDict[match.Value]);
      dataStr = "|" + offcuts + "|" + "{minCut}" + dataStr;
      serializedData.Add(dataStr);
    }


    // double matching
    if (!doubleMatch) return serializedData;
    {
      Dictionary<string, Tuple<string, string>> doubleMatchedDict = DoubleMatch(
        singleRemainTargets,
        singleRemainSal,
        tolerance,
        out doubleRemainTargets,
        out doubleRemainSal);

      // serialization of data
      foreach (var match in doubleMatchedDict)
      {
        string targetName = match.Key;
        double targetVal = targetDict[targetName];

        string salAName = match.Value.Item1;
        double salAVal = salvageDict[salAName];

        string salBName = match.Value.Item2;
        double salBVal = salvageDict[salBName];

        string dataStr = Serializer(targetName, targetVal, salAName, salAVal, salBName, salBVal);
        double offcuts = CalculateOffcuts(targetVal, salAVal + salBVal);
        dataStr = "|" + offcuts + "|" + "{minCut}" + dataStr;

        serializedData.Add(dataStr);
      }
    }

    Dictionary<string, string> restComb = FindRestComb(doubleRemainTargets, doubleRemainSal);
    foreach (var match in restComb)
    {
      string dataStr = Serializer(match.Key, targetDict[match.Key], match.Value, salvageDict[match.Value]);
      double offcuts = CalculateOffcuts(targetDict[match.Key], salvageDict[match.Value]);
      dataStr = "|" + offcuts + "|" + "{elseCut}" + dataStr;
      serializedData.Add(dataStr);
    }

    return serializedData;
  }

  private static double CalculateOffcuts(double target, double salvage)
  {
    double offcuts = Math.Abs(target - salvage);
    return Math.Round(offcuts, 2);
  }

  private static Dictionary<string, string> FindRestComb(Dictionary<string, double> targets, Dictionary<string, double> salvages)
  {
    Dictionary<string, string> foundPairsDict = new Dictionary<string, string>();

    List<string> matchList = new List<string>();

    // single matching
    foreach (var target in targets)
    {
      double targetValue = target.Value;
      string targetName = target.Key;

      Dictionary<string, double> pairs = new Dictionary<string, double>();

      foreach (var salvage in salvages)
      {
        double difference = Math.Abs(targetValue - salvage.Value);
        pairs.Add(salvage.Key, difference);
      }

      var sortedPairs = pairs.OrderBy(pair => pair.Value).ToDictionary(x => x.Key, x => x.Value);

      int elementIdx = 0;
      while (elementIdx < sortedPairs.Count && matchList.Contains(sortedPairs.ElementAt(elementIdx).Key))
      {
        elementIdx++;
      }
      var smallestPair = sortedPairs.ElementAt(elementIdx);

      matchList.Add(smallestPair.Key);
      foundPairsDict.Add(targetName, smallestPair.Key);
    }

    return foundPairsDict;
  }


  //private static Dictionary<string, Tuple<string, string>> DoubleMatch(Dictionary<string, double> targets, Dictionary<string, double> salvages, double tolerance)
  //{
  //  Dictionary<string, Tuple<string, string>> matchedID = new Dictionary<string, Tuple<string, string>>();

  //  List<string> matchList = new List<string>();

  //  //foreach (var target in targets)
  //  for (int z = 0; z < targets.Count; z++)
  //  {
  //    KeyValuePair<string, double> target = targets.ElementAt(z);

  //    for (int i = 0; i < salvages.Count; i++)
  //    {
  //      string timberA_Name = salvages.Keys.ElementAt(i);
  //      double timberA_Val = salvages.Values.ElementAt(i);

  //      for (int j = i + 1; j < salvages.Count; j++)
  //      {
  //        string timberB_Name = salvages.Keys.ElementAt(j);
  //        double timberB_Val = salvages.Values.ElementAt(j);

  //        bool matched = MatchDouble(timberA_Val, timberB_Val, target.Value, tolerance);
  //        bool inListA = matchList.Contains(timberA_Name);
  //        bool inListB = matchList.Contains(timberB_Name);


  //        if (!matched || inListA || inListB) continue;
  //        matchList.Add(timberA_Name);
  //        matchList.Add(timberB_Name);

  //        Tuple<string, string> pairs = Tuple.Create(timberA_Name, timberB_Name);
  //        matchedID.Add(target.Key, pairs);

  //        // break compare loop
  //        goto CompareExit;
  //      }
  //    }
  //  // exit compare loop
  //  CompareExit:
  //    ;
  //  }

  //  return matchedID;
  //}



  private static Dictionary<string, Tuple<string, string>> DoubleMatch(Dictionary<string, double> targets, Dictionary<string, double> salvages, double tolerance,
    out Dictionary<string, double> remainTargets, out Dictionary<string, double> remainSalvages)
  {
    Dictionary<string, Tuple<string, string>> matchedID = new Dictionary<string, Tuple<string, string>>();

    remainSalvages = salvages.ToDictionary(salvage => salvage.Key, salvage => salvage.Value);
    remainTargets = targets.ToDictionary(target => target.Key, target => target.Value);


    List<string> matchList = new List<string>();
    List<double> sortedSalvageValues = salvages.Values.ToList();
    sortedSalvageValues.Sort();

    foreach (var target in targets)
    {
      // simultaneously working with 2 index by starting timberA on 0 and timber B on last index
      // binary search algorithm
      int timberLeftIdx = 0;
      int timberRightIdx = sortedSalvageValues.Count - 1;

      while (timberLeftIdx < timberRightIdx)
      {
        double sum = sortedSalvageValues[timberLeftIdx] + sortedSalvageValues[timberRightIdx];

        // if sum of two timber is within tolerance of target value
        if (Math.Abs(sum - target.Value) <= tolerance)
        {
          string leftKey = salvages.FirstOrDefault(x => x.Value == sortedSalvageValues[timberLeftIdx]).Key;
          string rightKey = salvages.FirstOrDefault(x => x.Value == sortedSalvageValues[timberRightIdx]).Key;

          // skip if left or right key is contained in matchList
          if (matchList.Contains(leftKey) || matchList.Contains(rightKey))
          {
            timberLeftIdx++;
            continue;
          }
          // found match

          matchList.Add(leftKey);
          matchList.Add(rightKey);

          remainTargets.Remove(target.Key);
          remainSalvages.Remove(leftKey);
          remainSalvages.Remove(rightKey);

          Tuple<string, string> pairs = Tuple.Create(leftKey, rightKey);
          matchedID.Add(target.Key, pairs);
          break;
        }
        else if (sum < target.Value)
        {
          timberLeftIdx++;
        }
        else
        {
          timberRightIdx--;
        }
      }
    }

    return matchedID;
  }


  private static Dictionary<string, string> singleMatch(Dictionary<string, double> targets, Dictionary<string, double> salvages, double tolerance, out Dictionary<string, double> remainTargets, out Dictionary<string, double> remainSal)
  {
    remainTargets = new Dictionary<string, double>();
    remainSal = new Dictionary<string, double>();

    foreach (var target in targets)
    {
      remainTargets.Add(target.Key, target.Value);
    }

    foreach (var salvage in salvages)
    {
      remainSal.Add(salvage.Key, salvage.Value);
    }


    Dictionary<string, string> matchedID = new Dictionary<string, string>();

    foreach (var target in targets)
    {
      foreach (var salvage in salvages)
      {
        bool matched = MatchSolo(salvage.Value, target.Value, tolerance);

        if (!matched || matchedID.Values.Contains(salvage.Key)) continue;
        remainTargets.Remove(target.Key);
        remainSal.Remove(salvage.Key);

        matchedID.Add(target.Key, salvage.Key);
        break;
      }
    }

    return matchedID;
  }

  private static Dictionary<string, double> DeSerializer(List<double> data)
  {
    Dictionary<string, double> deSerializedData = new Dictionary<string, double>();
    for (int i = 0; i < data.Count; i++)
    {
      double value = Math.Round(data[i], 2);
      string name = "Tar" + i.ToString();

      deSerializedData.Add(name, value);
    }
    return deSerializedData;
  }

  private static Dictionary<string, double> DeSerializer(List<string> data)
  {
    Dictionary<string, double> deSerializedData = new Dictionary<string, double>();
    foreach (var d in data)
    {
      string[] salADataSplit = d.Split(new string[] { "::" }, StringSplitOptions.None);
      string Name = salADataSplit[0];
      double value = double.Parse(salADataSplit[1]);


      deSerializedData.Add(Name, value);
    }

    return deSerializedData;
  }

  private static void DeSerializer(string data, out double value, out string Name)
  {
    string[] salADataSplit = data.Split(new string[] { "::" }, StringSplitOptions.None);
    Name = salADataSplit[0];
    value = double.Parse(salADataSplit[1]);
  }

  private static bool MatchSolo(double a, double target, double tolerance)
  {
    double difference = Math.Abs(a - target);
    return difference <= tolerance;
  }

  private static bool MatchDouble(double a, double b, double target, double tolerance)
  {
    double sum = a + b;
    double difference = Math.Abs(sum - target);
    return difference < tolerance;
  }

  private static string Serializer(string tarName, double tarLen, string salAName, double salAVal)
  {
    string targetStr = "(" + tarName + "," + tarLen + ")";
    string salA = "<" + salAName + "," + salAVal + ">";
    string serialized = targetStr + salA;
    return serialized;
  }

  private static string Serializer(string tarName, double tarLen, string salAName, double salAVal, string salBName, double salBVal)
  {
    string targetStr = "(" + tarName + "," + tarLen + ")";
    string salA = "<" + salAName + "," + salAVal + ">";
    string salB = "<" + salBName + "," + salBVal + ">";
    string serialized = targetStr + salA + salB;
    return serialized;
  }
  #endregion
}
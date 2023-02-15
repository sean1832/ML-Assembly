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
    Dictionary<string, string> singleMatchedDict = SingleMatch(
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

    Dictionary<string, Tuple<string, double, bool>> restComb = FindRestComb(doubleRemainTargets, doubleRemainSal, tolerance);
    foreach (var match in restComb)
    {
      double originalSalLength = salvageDict[match.Value.Item1];
      double cutLength = originalSalLength - match.Value.Item2;
      string dataStr = Serializer(match.Key, targetDict[match.Key], match.Value.Item1, match.Value.Item2, match.Value.Item3, cutLength);
      double offcuts = CalculateOffcuts(targetDict[match.Key], match.Value.Item2);
      dataStr = "|" + offcuts + "|" + "{elseCut}" + dataStr;
      serializedData.Add(dataStr);
    }

    return serializedData;
  }

  private static string Serializer(string tarName, double tarLen, string salName, double salLen, bool isCut, double cutLength)
  {
    if (isCut)
    {
      string target = "(" + tarName + "," + tarLen + ")";
      string salvage = "<" + salName + "," + salLen + ">";
      string cut = "[" + Math.Round(cutLength, 2) + "]";
      return target + salvage + cut;
    }
    else
    {
      string target = "(" + tarName + "," + tarLen + ")";
      string salvage = "<" + salName + "," + salLen + ">";
      return target + salvage;
    }
  }

  private static double CalculateOffcuts(double target, double salvage)
  {
    double offcuts = salvage - target;
    return Math.Round(offcuts, 2);
  }

  private static Dictionary<string, Tuple<string, double, bool>> FindRestComb(Dictionary<string, double> targets, Dictionary<string, double> salvages, double tolerance)
  {
    // (targetName, <SalvageName, lengthLeft, isCut>)
    Dictionary<string, Tuple<string, double, bool>> resultsDictionary = new Dictionary<string, Tuple<string, double, bool>>();

    Dictionary<string, Tuple<string, double>> foundPairsDict = new Dictionary<string, Tuple<string, double>>();
    

    List<string> matchList = new List<string>();

    // single matching
    foreach (var target in targets)
    {
      double targetValue = target.Value;
      string targetName = target.Key;

      Dictionary<string, double> pairs = new Dictionary<string, double>();
      
      foreach (var salvage in salvages)
      {
        double difference = salvage.Value - targetValue;
        if (difference < 0) continue;
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
      foundPairsDict.Add(targetName, Tuple.Create(smallestPair.Key, smallestPair.Value));
    }


    foreach (var pair in foundPairsDict)
    {
      string targetName = pair.Key;
      double targetLength = targets[pair.Key];
      string salvageName = pair.Value.Item1;
      double salvageLength = pair.Value.Item2;
      double difference = targetLength - salvageLength;
      if (difference >= 0.3)
      {
        double cutLength = difference;
        salvageLength = targetLength;
        foreach (var newTarget in targets)
        {
          bool matched = MatchSolo(cutLength, newTarget.Value, tolerance);
          if (matched)
          {
            // Check if the key already exists in the dictionary before adding it
            if (resultsDictionary.ContainsKey(targetName)) continue;
            resultsDictionary.Add(targetName, Tuple.Create(salvageName, salvageLength, true));
            if (resultsDictionary.ContainsKey(newTarget.Key)) continue;
            resultsDictionary.Add(newTarget.Key, Tuple.Create(salvageName, cutLength, true));
            break;
          }
        }
      }

      else
      {
        // Check if the key already exists in the dictionary before adding it
        if (resultsDictionary.ContainsKey(targetName)) continue;
        resultsDictionary.Add(targetName, Tuple.Create(salvageName, salvageLength, false));
      }
    }

    return resultsDictionary;
  }



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


  private static Dictionary<string, string> SingleMatch(Dictionary<string, double> targets, Dictionary<string, double> salvages, double tolerance, out Dictionary<string, double> remainTargets, out Dictionary<string, double> remainSal)
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

  private static bool MatchSolo(double salvage, double target, double tolerance)
  {
    double difference = salvage - target;

    if (difference < 0)
    {
      return false;
    }
    else
    {
      return difference <= tolerance;
    }
  }

  private static bool MatchDouble(double a, double b, double target, double tolerance)
  {
    double sum = a + b;
    double difference = sum - target;

    if (difference < 0)
    {
      return false;
    }
    else
    {
      return difference < tolerance;
    }
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
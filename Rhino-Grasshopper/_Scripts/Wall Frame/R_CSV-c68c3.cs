using System;
using System.Collections;
using System.Collections.Generic;

using Rhino;
using Rhino.Geometry;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using System.IO;


/// <summary>
/// This class will be instantiated on demand by the Script component.
/// </summary>
public abstract class Script_Instance_c68c3 : GH_ScriptInstance
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
  private void RunScript(string path, string delimit, ref object values)
  {
    if (path != null && delimit != null)
    {
      List<string[]> datas = ReadCSV(path);
      values = ConvertTree(datas);
    }
  }
  #endregion
  #region Additional
  private static List<string[]> ReadCSV(string path)
  {
    try
    {
      List<string[]> datas = new List<string[]>();

      using (var reader = new StreamReader(path))
      {
        while (!reader.EndOfStream)
        {
          var line = reader.ReadLine();
          var data = line.Split(',');
          datas.Add(data);
        }
      }

      return datas;
    }
    catch(IOException ex)
    {
      Console.WriteLine("Error reading the file: " + ex.Message);
      return null;
    }
  }
  
  private static DataTree<string> ConvertTree(List<string[]> datas)
  {
    Grasshopper.DataTree<string> tree = new DataTree<string>();

    for (int i = 0; i < datas.Count; i++)
    {
      string[] data = datas[i];
      foreach (string text in data)
      {
        tree.Add(text, new GH_Path(i));
      }
    }
    return tree;
  }
  #endregion
}
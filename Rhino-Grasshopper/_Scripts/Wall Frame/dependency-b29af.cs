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
public abstract class Script_Instance_b29af : GH_ScriptInstance
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
  private void RunScript(object refresh, bool write, string dir, ref object data)
  {
    List<string> result = GetAssembly();

    data = result;

    if (write)
    {
      WriteToPath(result, dir, "plugin requirements");
    }

  }
  #endregion
  #region Additional
  private List<string> GetAssembly()
  {
    Dictionary<string, string> asms = new Dictionary<string, string>();
    List<string> results = new List<string>();
    foreach (IGH_DocumentObject obj in Grasshopper.Instances.ActiveCanvas.Document.Objects)
    {
      GH_AssemblyInfo asm = Grasshopper.Instances.ComponentServer.FindAssemblyByObject(obj.ComponentGuid);

      if (asm != null && !asm.IsCoreLibrary)
      {
        asms[asm.Name] = asm.Version;
      }
    }

    foreach (KeyValuePair<string, string> pair in asms)
    {
      if (!string.IsNullOrEmpty(pair.Key))
      {
        results.Add(pair.Key + " : " + pair.Value);
      }
    }
    return results;
  }

  private static void WriteToPath(List<string> contents ,string dir, string fileName, string extension = ".txt")
  {
    dir = Path.GetDirectoryName(dir);
    string path = dir + "/" + fileName + extension;
    using (StreamWriter writer = new StreamWriter(path))
    {
      foreach (var content in contents)
      {
        writer.WriteLine(content);
      }
    }
  }
  #endregion
}
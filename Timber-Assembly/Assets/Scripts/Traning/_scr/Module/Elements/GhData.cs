using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhData
{
    public float Score { get; set; }
    public float TotalOffcutsAmount { get; set; }
    public float TotalSalvageLength { get; set; }
    public float MaterialEfficiency { get; set; }
    public int OffcutsCount { get; set; }
    public float LaborEfficiency { get; set; }
    public int ReuseCount { get; set; }
    public WallScale WallScale { get; set; }
    public WindowPosition[] WindowPosition { get; set; }
    public WindowScale[] WindowScale { get; set; }
    public bool[] IsAtBounds { get; set; }
}

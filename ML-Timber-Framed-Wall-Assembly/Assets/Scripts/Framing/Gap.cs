using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[System.Serializable]
public class Gap
{
    public float XPos;
    public float Width;
    public int numStuds;
    public int oldNumStuds;
    public List<GameObject> Studs;
    public List<GameObject> Noggings;

    public Gap(float Xpos, float width)
    {
        this.XPos = Xpos;
        this.Width = width;
        this.Studs = new List<GameObject>();
        this.Noggings = new List<GameObject>();
        this.numStuds = 0;
        this.oldNumStuds = 0;
    }
}

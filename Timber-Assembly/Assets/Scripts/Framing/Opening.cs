using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Opening
{
    public float XPos;
    public float YPos;
    public float Width;
    public float Height;
    public string Type;
    public int studNum;
    public int numStuds;
    public int oldNumStuds;
    public List<GameObject> Vertical;
    public List<GameObject> Horizontal;

    public Opening(string type, float Xpos, float Ypos, float width, float height, int studs)
    {
        this.Type = type;
        this.XPos = Xpos;
        this.YPos = Ypos;
        this.Width = width;
        this.Height = height;
        this.studNum = studs;
        this.Vertical = new List<GameObject>();
        this.Horizontal = new List<GameObject>();
        this.numStuds = 0;
        this.oldNumStuds = 0;
    }
}

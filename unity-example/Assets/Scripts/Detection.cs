using System;
using System.Collections.Generic;
using UnityEngine;

// Simple detection data container
[Serializable]
public class Detection
{
    public Rect box; // x,y,w,h in pixels
    public int classId;
    public float score;
    public Detection(Rect box, int classId, float score)
    {
        this.box = box;
        this.classId = classId;
        this.score = score;
    }
}

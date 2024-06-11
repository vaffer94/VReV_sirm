using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
[System.Serializable]
public class CoordinateInfo
{
    public float pianoCoronale;
    public float pianoAssiale;
    public float pianoSagitale;
    public int idView;

}
*/

[Serializable]
public class SliceCoordinate
{
    public float Sagittal;
    public float Coronal;
    public float Axial;
}

[Serializable]
public class RootObject
{
    public List<SliceCoordinate> sliceCoordinates;
}

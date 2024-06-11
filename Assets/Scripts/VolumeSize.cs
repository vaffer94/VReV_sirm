using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeSize : MonoBehaviour
{
    public float SizeX { get; private set; }
    public float XMin { get; private set; }
    public float XMax { get; private set; }

    public float SizeY { get; private set; }
    public float YMin { get; private set; }
    public float YMax { get; private set; }

    public float SizeZ { get; private set; }
    public float ZMin { get; private set; }
    public float ZMax { get; private set; }

    void Start()
    {
        CalculateVolumeInfo();
    }

    void CalculateVolumeInfo()
    {
        Bounds bounds = new Bounds();

        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            bounds = renderer.bounds;
        }
        else
        {
            Collider collider = GetComponent<Collider>();
            if (collider != null)
            {
                bounds = collider.bounds;
            }
            else
            {
                Debug.LogWarning("No Renderer or Collider found on the object.");
                return;
            }
        }

        // Calculate and store the size and coordinates
        SizeX = bounds.size.x;
        XMin = bounds.min.x;
        XMax = bounds.max.x;

        SizeY = bounds.size.y;
        YMin = bounds.min.y;
        YMax = bounds.max.y;

        SizeZ = bounds.size.z;
        ZMin = bounds.min.z;
        ZMax = bounds.max.z;

        // Print the values to the console for verification
        Debug.Log("Size X: " + SizeX);
        Debug.Log("X Min: " + XMin);
        Debug.Log("X Max: " + XMax);

        Debug.Log("Size Y: " + SizeY);
        Debug.Log("Y Min: " + YMin);
        Debug.Log("Y Max: " + YMax);

        Debug.Log("Size Z: " + SizeZ);
        Debug.Log("Z Min: " + ZMin);
        Debug.Log("Z Max: " + ZMax);
    }
}

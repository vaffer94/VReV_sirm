using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementSyncronizer : MonoBehaviour
{
    public Transform objectToCopy; // Reference to the object whose movement we want to copy

    private Vector3 initialOffset;

    // Start is called before the first frame update
    void Start()
    {
        // Calculate the initial offset between the objects
        if (objectToCopy != null)
        {
            initialOffset = objectToCopy.position - transform.position;
        }
        else
        {
            Debug.LogError("Please assign an object to copy in the inspector.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the object to copy exists
        if (objectToCopy != null)
        {
            // Update the position of this object to match the position of the objectToCopy
            transform.position = objectToCopy.position - initialOffset;
        }
    }
}

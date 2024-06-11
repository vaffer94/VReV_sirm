using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class XAxisMovement : MonoBehaviour
{
    Vector3 currentPosition;
    private GameObject me;
    private Rigidbody rb; // Rigidbody component reference

    void Start()
    {
        // Find the GameObject named "Cube" and assign it to 'me'
        me = GameObject.Find("Cube");

        // Get the Rigidbody component attached to the 'me' GameObject
        rb = me.GetComponent<Rigidbody>();

        if (rb != null)
        {
            // Freeze rotation around the Y axis
            rb.constraints = RigidbodyConstraints.FreezeRotationY;
            rb.constraints = RigidbodyConstraints.FreezeRotationX;
            rb.constraints = RigidbodyConstraints.FreezeRotationZ;
            rb.constraints = RigidbodyConstraints.FreezePositionX;
            rb.constraints = RigidbodyConstraints.FreezePositionY;
        }
        else
        {
            // Log an error message if 'rb' is null
            Debug.LogError("Rigidbody component not found on the Cube GameObject.");
        }
    }
        public void Update()
    {


        currentPosition = transform.position;
        Debug.Log(currentPosition);
    }
}

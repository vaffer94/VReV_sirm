using UnityEngine;

namespace UnityVolumeRendering
{
    [ExecuteInEditMode]
    public class SlicingPlane : MonoBehaviour
    {
        public string SlicePlaneToMonitor;
        public VolumeRenderedObject targetObject;
        // public GameObject VolumeRenderedObject;
        private MeshRenderer meshRenderer;

        private void Start()
        {
            meshRenderer = GetComponent<MeshRenderer>(); // retrieves the MeshRenderer component attached to the same GameObject as this script and stores it in the meshRenderer field
        }

        private void Update()
        {
            switch(SlicePlaneToMonitor)
            {
                case "xy":
                    {
                        // Create a translation matrix to shift the slice 5 units to the left
                        Matrix4x4 translationMatrix = Matrix4x4.Translate(new Vector3(1f, 0f, 0f));

                        // Combine the translation matrix with the local-to-world matrix of the target object
                        //Matrix4x4 updatedLocalToWorldMatrix = translationMatrix * VolumeRenderedObject.transform.worldToLocalMatrix;
                        Matrix4x4 updatedLocalToWorldMatrix = translationMatrix * transform.parent.worldToLocalMatrix;

                        meshRenderer.sharedMaterial.SetMatrix("_parentInverseMat", updatedLocalToWorldMatrix);
                        meshRenderer.sharedMaterial.SetMatrix("_planeMat", transform.localToWorldMatrix); // TODO: allow changing scale
                        break;
                    }
                case "yz":
                    {
                        meshRenderer.sharedMaterial.SetMatrix("_parentInverseMat", transform.parent.worldToLocalMatrix);
                        meshRenderer.sharedMaterial.SetMatrix("_planeMat", transform.localToWorldMatrix);
                        break;
                    }
                case "xz":
                    {
                        Matrix4x4 translationMatrix = Matrix4x4.Translate(new Vector3(0f, 1f, 0f));
                        Matrix4x4 updatedLocalToWorldMatrix = translationMatrix * transform.parent.worldToLocalMatrix;

                        meshRenderer.sharedMaterial.SetMatrix("_parentInverseMat", updatedLocalToWorldMatrix);
                        meshRenderer.sharedMaterial.SetMatrix("_planeMat", transform.localToWorldMatrix);
                        break;
                    }
                default:
                    Debug.Log("Error in setting the slicing plane!");
                    break;
            }


        }
    }
}

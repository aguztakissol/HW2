using UnityEngine;

public class SimpleGrab : MonoBehaviour
{
    public Transform grabPoint;   // where the object snaps to
    private Transform grabbedObject;

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Grabbable") && Input.GetKey(KeyCode.G))
        {
            grabbedObject = other.transform;
            grabbedObject.SetParent(grabPoint);
            grabbedObject.localPosition = Vector3.zero;
            grabbedObject.localRotation = Quaternion.identity;
        }
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.G) && grabbedObject != null)
        {
            grabbedObject.SetParent(null);
            grabbedObject = null;
        }
    }
}

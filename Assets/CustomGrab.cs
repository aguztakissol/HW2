using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CustomGrab : MonoBehaviour
{
    // --- Existing fields ---
    public InputActionReference action;       // Input action for grab (Grip button)
    public List<Transform> nearObjects = new List<Transform>(); // Objects in range
    public Transform grabbedObject = null;    // Currently grabbed object
    public bool doubleRotation = false;       // Extra credit toggle

    // --- Internal ---
    CustomGrab otherHand = null;              // Reference to the other hand
    bool grabbing = false;                     // Is this hand holding something?
    Vector3 lastPosition;                      // Last frame position
    Quaternion lastRotation;                   // Last frame rotation

    void Start()
    {
        action.action.Enable();

        // Find the other hand in the same parent hierarchy
        foreach (CustomGrab c in transform.parent.GetComponentsInChildren<CustomGrab>())
        {
            if (c != this)
                otherHand = c;
        }

        // Initialize last position/rotation
        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }

    void Update()
    {
        grabbing = action.action.IsPressed();

        if (grabbing)
        {
            // Grab nearby object or object held by other hand
            if (!grabbedObject)
            {
                grabbedObject = nearObjects.Count > 0 ? nearObjects[0] :
                                (otherHand != null ? otherHand.grabbedObject : null);
            }

            if (grabbedObject)
            {
                // --- Step 1: Compute deltas for this hand ---
                Vector3 deltaPos = transform.position - lastPosition;
                Quaternion deltaRot = transform.rotation * Quaternion.Inverse(lastRotation);

                // --- Step 2: Check if other hand is also grabbing the same object ---
                if (otherHand != null && otherHand.grabbedObject == grabbedObject)
                {
                    // Other hand's deltas
                    Vector3 otherDeltaPos = otherHand.transform.position - otherHand.lastPosition;
                    Quaternion otherDeltaRot = otherHand.transform.rotation * Quaternion.Inverse(otherHand.lastRotation);

                    // Combine translations
                    Vector3 totalDeltaPos = deltaPos + otherDeltaPos;

                    // Apply combined translation
                    grabbedObject.position += totalDeltaPos;

                    // Apply rotations around each hand
                    foreach (var hand in new Transform[] { transform, otherHand.transform })
                    {
                        Vector3 dir = grabbedObject.position - hand.position;
                        Quaternion dRot = (hand == transform) ? deltaRot : otherDeltaRot;
                        dir = dRot * dir;
                        grabbedObject.position = hand.position + dir;
                    }

                    // Combine rotations
                    Quaternion totalDeltaRot = deltaRot * otherDeltaRot;

                    // Optional double rotation
                    if (doubleRotation)
                    {
                        totalDeltaRot.ToAngleAxis(out float angle, out Vector3 axis);
                        angle *= 2f;
                        totalDeltaRot = Quaternion.AngleAxis(angle, axis);
                    }

                    grabbedObject.rotation = totalDeltaRot * grabbedObject.rotation;
                }
                else
                {
                    // Only this hand is grabbing
                    grabbedObject.position += deltaPos;

                    Vector3 dir = grabbedObject.position - transform.position;
                    dir = deltaRot * dir;
                    grabbedObject.position = transform.position + dir;

                    // Optional double rotation
                    if (doubleRotation)
                    {
                        deltaRot.ToAngleAxis(out float angle, out Vector3 axis);
                        angle *= 2f;
                        deltaRot = Quaternion.AngleAxis(angle, axis);
                    }

                    grabbedObject.rotation = deltaRot * grabbedObject.rotation;
                }
            }
        }
        else if (grabbedObject)
        {
            // Release object
            grabbedObject = null;
        }

        // --- Step 3: Update last position and rotation for this hand ---
        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }

    // --- Trigger detection for nearby objects ---
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform && other.tag.ToLower() == "grabbable")
            nearObjects.Add(other.transform);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform && other.tag.ToLower() == "grabbable")
            nearObjects.Remove(other.transform);
    }
}


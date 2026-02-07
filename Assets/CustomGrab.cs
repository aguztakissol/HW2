using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CustomGrab : MonoBehaviour
{
    // --- Public fields ---
    public InputActionReference action;          // Input action for grab (Grip button)
    public bool doubleRotation = false;          // Extra credit toggle

    // --- Internal ---
    public List<Transform> nearObjects = new List<Transform>(); // Objects in range
    public Transform grabbedObject = null;      // Currently grabbed object
    private List<CustomGrab> grabbingHands = new List<CustomGrab>(); // All hands grabbing this object

    private bool grabbing = false;              // Is this hand holding something?
    private Vector3 lastPosition;               // Last frame position
    private Quaternion lastRotation;            // Last frame rotation

    private void Start()
    {
        action.action.Enable();
        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }

    private void Update()
    {
        grabbing = action.action.IsPressed();

        if (grabbing)
        {
            // --- Step 1: Grab object if not already ---
            if (!grabbedObject)
            {
                grabbedObject = GetClosestGrabbable();
                if (grabbedObject)
                    RegisterHand(this);
            }
            else if (!grabbingHands.Contains(this))
            {
                RegisterHand(this);
            }

            if (grabbedObject)
            {
                ApplyGrabTransform();
            }
        }
        else
        {
            // Release this hand
            if (grabbedObject)
            {
                grabbingHands.Remove(this);
                if (grabbingHands.Count == 0)
                    grabbedObject = null;
            }
        }

        // --- Step 2: Update last frame position/rotation ---
        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }

    // --- Register this hand as grabbing the object ---
    private void RegisterHand(CustomGrab hand)
    {
        if (!grabbingHands.Contains(hand))
            grabbingHands.Add(hand);
    }

    // --- Apply combined transformation to grabbed object ---
    private void ApplyGrabTransform()
    {
        Vector3 totalDeltaPos = Vector3.zero;
        Quaternion totalDeltaRot = Quaternion.identity;

        // Calculate deltas for all grabbing hands
        foreach (var hand in grabbingHands)
        {
            Vector3 deltaPos = hand.transform.position - hand.lastPosition;
            Quaternion deltaRot = hand.transform.rotation * Quaternion.Inverse(hand.lastRotation);

            totalDeltaPos += deltaPos;
            totalDeltaRot = deltaRot * totalDeltaRot;
        }

        // Optional: double rotation
        if (doubleRotation)
        {
            totalDeltaRot.ToAngleAxis(out float angle, out Vector3 axis);
            angle *= 2f;
            totalDeltaRot = Quaternion.AngleAxis(angle, axis);
        }

        // --- Apply translation ---
        grabbedObject.position += totalDeltaPos;

        // --- Apply rotation around pivot ---
        Vector3 pivot = Vector3.zero;
        foreach (var hand in grabbingHands)
            pivot += hand.transform.position;
        pivot /= grabbingHands.Count;

        Vector3 dir = grabbedObject.position - pivot;
        dir = totalDeltaRot * dir;
        grabbedObject.position = pivot + dir;

        // Apply rotation
        grabbedObject.rotation = totalDeltaRot * grabbedObject.rotation;
    }

    // --- Find closest grabbable object from nearObjects ---
    private Transform GetClosestGrabbable()
    {
        Transform closest = null;
        float minDist = float.MaxValue;

        foreach (var obj in nearObjects)
        {
            if (!obj) continue;
            float dist = Vector3.Distance(transform.position, obj.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = obj;
            }
        }

        return closest;
    }

    // --- Trigger detection for nearby objects ---
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform && other.CompareTag("Grabbable"))
            nearObjects.Add(other.transform);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform && other.CompareTag("Grabbable"))
            nearObjects.Remove(other.transform);
    }
}

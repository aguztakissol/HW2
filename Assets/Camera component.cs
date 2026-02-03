using UnityEngine;

public class LensSetup : MonoBehaviour
{
    [Header("Lens Settings")]
    public Transform lens;             // Drag your magnifying glass object here
    public RenderTexture lensTexture;  // Drag your RenderTexture here

    private Camera lensCamera;

    void Awake()
    {
        // Get the Camera component on this GameObject
        lensCamera = GetComponent<Camera>();

        if (lensCamera == null)
        {
            Debug.LogError("No Camera component found on this GameObject!");
        }

        // Assign the RenderTexture
        if (lensCamera != null && lensTexture != null)
        {
            lensCamera.targetTexture = lensTexture;
        }
        else
        {
            Debug.LogWarning("Lens Camera or Render Texture not assigned!");
        }
    }

    void LateUpdate()
    {
        // Make sure lens and main camera exist
        if (lens != null && Camera.main != null)
        {
            // Follow the lens position (hand)
            lensCamera.transform.position = lens.position;

            // Always look in the headset forward direction
            lensCamera.transform.forward = Camera.main.transform.forward;
        }
    }
}

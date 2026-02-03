using UnityEngine;

[RequireComponent(typeof(Camera))]
public class LensCameraController : MonoBehaviour
{
    public Transform vrHead;            // Drag Main Camera (VR headset) here
    public RenderTexture lensTexture;   // Drag LensRenderTexture here

    private Camera lensCamera;

    void Awake()
    {
        lensCamera = GetComponent<Camera>();

        // URP workaround: assign Render Texture in code
        if (lensTexture != null)
        {
            lensCamera.targetTexture = lensTexture;
        }
    }

    void LateUpdate()
    {
        if (vrHead != null)
        {
            // Follow headset position
            lensCamera.transform.position = vrHead.position;

            // Keep rotation fixed forward (does not rotate with lens)
            lensCamera.transform.rotation = Quaternion.identity;
        }
    }
}

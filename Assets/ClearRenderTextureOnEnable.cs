using UnityEngine;

public class ClearRenderTextureOnEnable : MonoBehaviour
{
    [SerializeField] private RenderTexture renderTexture;

    private void OnEnable()
    {
        if (renderTexture == null)
        {
            Debug.LogWarning("RenderTexture not assigned!");
            return;
        }

        RenderTexture activeRT = RenderTexture.active;

        RenderTexture.active = renderTexture;
        GL.Clear(true, true, Color.clear); // Change to Color.clear if needed
        RenderTexture.active = activeRT;
    }
}

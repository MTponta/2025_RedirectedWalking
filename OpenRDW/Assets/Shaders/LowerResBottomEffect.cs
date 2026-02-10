using UnityEngine;

[RequireComponent(typeof(Camera))]
public class LowerResBottomEffect : MonoBehaviour
{
    public Material effectMaterial;
    [Range(0.1f, 1.0f)]
    public float downscale = 0.5f;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (effectMaterial == null)
        {
            Graphics.Blit(src, dest);
            return;
        }

        effectMaterial.SetFloat("_Downscale", downscale);
        effectMaterial.SetFloat("_SrcHeight", src.height);

        Graphics.Blit(src, dest, effectMaterial);
    }
}
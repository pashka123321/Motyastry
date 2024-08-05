using UnityEngine;

[RequireComponent(typeof(Camera))]
public class BlurEffect : MonoBehaviour
{
    public Material blurMaterial;
    public float blurSize = 1.0f;
    public Shader depthShader;

    private Camera _camera;
    private RenderTexture _depthTexture;

    private void Start()
    {
        _camera = GetComponent<Camera>();
        _camera.depthTextureMode = DepthTextureMode.Depth;
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (blurMaterial != null)
        {
            if (_depthTexture == null)
            {
                _depthTexture = new RenderTexture(src.width, src.height, 0, RenderTextureFormat.Depth);
            }

            // Render depth texture
            RenderTexture temp = RenderTexture.GetTemporary(src.width, src.height, 0, src.format);
            Graphics.Blit(src, temp);
            blurMaterial.SetTexture("_DepthTex", _depthTexture);

            blurMaterial.SetFloat("_Size", blurSize);
            Graphics.Blit(temp, dest, blurMaterial);
            RenderTexture.ReleaseTemporary(temp);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}

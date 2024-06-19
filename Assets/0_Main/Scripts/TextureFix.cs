using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureFix : MonoBehaviour
{
    private Material groundMaterial;
    public float textureScale = 2f;



    [Button("RepeatTexture")]
    void RepeatTexture()
    {
        groundMaterial = GetComponent<MeshRenderer>().material;
        if (groundMaterial != null)
        {
            groundMaterial.mainTextureScale = new Vector2(transform.localScale.x * textureScale, transform.localScale.z * textureScale);
        }
        else
        {
            Debug.LogError("Please assign a material to the script.");
        }
    }
}

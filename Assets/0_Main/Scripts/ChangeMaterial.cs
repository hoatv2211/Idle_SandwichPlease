using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Sirenix.OdinInspector;

public class ChangeMaterial : MonoBehaviour
{
    public Material crMaterial;
    public Material newMaterial;

    public List<MeshRenderer> objsParents;

    [Button("GetMaterial")]
    public void GetMaterial()
    {
         objsParents.Clear();
         MeshRenderer[] allMeshRenderers = FindObjectsOfType<MeshRenderer>();

        foreach (MeshRenderer meshRenderer in allMeshRenderers)
        {
            //if (meshRenderer.sharedMaterials.Count() > 1)
            //{
            //    for (int i = 0; i < meshRenderer.sharedMaterials.Count(); i++)
            //    {
            //        if (meshRenderer.sharedMaterials[i] == crMaterial)
            //        {
            //            objsParents.Add(meshRenderer);
            //            break;
            //        }
            //    }

            //    continue;
            //}
           

            if (meshRenderer.sharedMaterial == crMaterial)
            {
                Debug.Log("Found MeshRenderer with target material: " + meshRenderer.gameObject.name);
                objsParents.Add(meshRenderer);
            }
        }

        Debug.LogError("Done");
    }

    [Button("SetMaterial")]
    public void SetMaterial()
    {
        foreach (MeshRenderer meshRenderer in objsParents)
        {
            for(int i =0;i< meshRenderer.sharedMaterials.Count();i++)
            {

                if (meshRenderer.sharedMaterials[i]==crMaterial)
                {
                    meshRenderer.sharedMaterials[i] = newMaterial;
                }
            }

            //if (meshRenderer.sharedMaterial == crMaterial)
            //{
            //    Debug.Log("Found MeshRenderer with target material: " + meshRenderer.gameObject.name);
            //    meshRenderer.sharedMaterial = newMaterial;
            //}
        }
    }
}

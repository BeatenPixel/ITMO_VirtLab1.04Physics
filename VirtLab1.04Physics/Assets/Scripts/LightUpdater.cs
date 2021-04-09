using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LightUpdater : MonoBehaviour {

    public Material[] materialsToUpdate;
    public Transform lightT;

    private void Update() {
        Vector3 lightDir = lightT.forward;
        foreach (var m in materialsToUpdate) {
            m.SetVector("_LightDir", lightDir);
        }
    }

}

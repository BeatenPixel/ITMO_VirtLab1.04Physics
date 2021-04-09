using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeviceController : MonoBehaviour {

    // d = 0,046m
    // 1.601 0.1601

    public float speed = 60f;
    [Range(0f,1f)]
    public float prog = 0.5f;
    [Range(0f,1f)]
    public float weightsNormalDst = 0.5f;
    public float d = 0.046f;

    public Transform mainAxis;
    public Transform[] weightsT;

    public Vector3 mainAxisStartRotation;
    public Vector2 minMaxWeightsPos;
    public float minMaxWeightsVisualDstMul;

    public Vector3[] ropePoints;

    [Header("Lineyka")]
    public Transform lineykaLinePrefab;
    public Transform lineykaStartPoint;
    public Transform lineykaEndPoint;
    public float lineykaStartEndDst;

    [Header("Weights")]
    public Transform weightStartPoint;
    public Transform weightEndPoint;
    public float weightStartEndDst;
    public Transform massWeightT;
    public TextMeshPro massText;

    [Header("Katushka")]
    public Transform katushka1;
    public Transform katushka2;
    public Vector3 katushkaHalfWorldSize;

    [Header("Rope")]
    public LineRenderer rope;
    public Material ropeMaterial;
    public float scrollX = 10f;

    [Header("Calculations")]
    public float r = 0.128f;
    public float m = 0.05f;
    public float h = 0.000f;
    public float h0 = 0.8f;
    public float q = 0.000f;
    public float time = 0.000f;
    public float R_stup = 0.023f;
    public float m_gr = 0.400f;
    public float I0 = 0.009f;
    public float Mtr = 0.010f;
    public float a = 0.000f;
    public float I = 0.000f;
    public float g = 9.819f;

    internal void InternalStart() {
        ResetDevice();
    }

    internal void InternalUpdate() {

        if (LabMain.inst.state == LabState.RUNNING) {
            if (h < 1) {
                I = I0 + 4 * m_gr * r * r;
                a = (R_stup * (R_stup * m * g - Mtr)) / (I + m * R_stup * R_stup);
                if (a < 0)
                    a = 0;
                h = (1 - h0) + a * time * time / 2;
                q = h * 360 * 6.9197f;

                time += (Time.deltaTime + Random.Range(0f, 1f) * 0.01f);
                UpdateGraphics(h);
            } else {
                LabMain.inst.EndLab();
            }            
        }
        
    }

    public void StartDevice() {
        time = 0;
        h = 0;
    }

    public void StopDevice() {

    }

    public void ResetDevice() {
        UpdateGraphics(1-h0);
    }

    public void UpdateGraphics(float progress) {
        int turnPointsCount = 4;
        rope.positionCount = 1 + turnPointsCount + 1;
        Vector3[] p = new Vector3[rope.positionCount];
        p[0] = katushka1.position + Vector3.up * katushkaHalfWorldSize.y;
        for (int i = 0; i <= turnPointsCount; i++) {
            p[1 + i] = katushka2.position + Quaternion.Euler(0, 0, -90f * i / turnPointsCount) * Vector3.up * katushkaHalfWorldSize.y;
        }
        p[5] = new Vector3(p[4].x, Mathf.Lerp(lineykaEndPoint.position.y, lineykaStartPoint.position.y, progress), p[4].z);
        rope.SetPositions(p);

        massWeightT.position = p[5];

        ropeMaterial.SetTextureOffset("_MainTex", new Vector2(-scrollX * progress, 0));

        float nRotations = progress / (d * Mathf.PI);
        mainAxis.rotation = Quaternion.Euler(mainAxisStartRotation) * Quaternion.Euler(0, 360f*nRotations, 0);

        UpdateWeightsPos();
    }

    public void UpdateHeight(float _h0) {
        h0 = _h0;
        UpdateGraphics(1-h0);
    }

    public void UpdateMass(float _m) {
        m = _m;
        massText.text = m.ToString("F2") + "кг";        
    }

    public void UpdateRadius(float _r, float normalized) {
        r = _r;
        weightsNormalDst = normalized;
        UpdateWeightsPos();
    }

    private void UpdateWeightsPos() {
        float weightDst = Mathf.Lerp(minMaxWeightsPos.x, minMaxWeightsPos.y, weightsNormalDst);
        for (int i = 0; i < weightsT.Length; i++) {
            weightsT[i].localPosition = new Vector3(0, weightDst * minMaxWeightsVisualDstMul, 0);
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(lineykaStartPoint.position, 0.02f);
        Gizmos.DrawWireSphere(lineykaEndPoint.position, 0.02f);
        lineykaStartEndDst = Vector3.Distance(lineykaStartPoint.position, lineykaEndPoint.position);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(weightStartPoint.position, 0.02f);
        Gizmos.DrawWireSphere(weightEndPoint.position, 0.02f);
        weightStartEndDst = Vector3.Distance(weightStartPoint.position, weightEndPoint.position);

        Gizmos.DrawLine(katushka1.position, katushka1.position + Vector3.right * katushkaHalfWorldSize.x);
        Gizmos.DrawLine(katushka1.position, katushka1.position + Vector3.up * katushkaHalfWorldSize.y);
        Gizmos.DrawLine(katushka1.position, katushka1.position + Vector3.forward * katushkaHalfWorldSize.z);
    }

}

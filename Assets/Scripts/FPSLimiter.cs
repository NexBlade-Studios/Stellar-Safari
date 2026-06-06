using UnityEngine;
public class FPSLimiter : MonoBehaviour
{
    public int fps = 60;
    void Start() {QualitySettings.vSyncCount = 0; Application.targetFrameRate = fps;}
}
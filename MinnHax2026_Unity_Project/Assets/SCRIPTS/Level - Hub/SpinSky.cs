using UnityEngine;

public class SpinSkybox : MonoBehaviour
{

    void Update()
    {
        
        float spin = Mathf.Repeat(Time.time, 360f);
        RenderSettings.skybox.SetFloat("_Rotation", spin);
    }
}
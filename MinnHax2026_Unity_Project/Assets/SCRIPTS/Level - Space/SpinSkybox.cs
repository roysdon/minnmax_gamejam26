using UnityEngine;
using TMPro;

public class SpinSkybox : MonoBehaviour
{

    public Camera mainCamera;
    public GameObject textGroup;
    public GameObject player;



    void Update()
    {
        
        float spin = Mathf.Repeat(Time.time, 360f);
        RenderSettings.skybox.SetFloat("_Rotation", spin);

        float newY = transform.position.y + Mathf.Sin(Time.time * 2);
        textGroup.transform.position = new Vector3(textGroup.transform.position.x, 300 + newY * 2, textGroup.transform.position.z);
        textGroup.transform.LookAt(textGroup.transform.position - (player.transform.position - textGroup.transform.position));
    }
}

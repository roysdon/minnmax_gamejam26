using UnityEngine;

public class Astronaut : MonoBehaviour
{

    public AudioClip[] requestSounds = new AudioClip[3];
    public int step = -1;
    public GameObject one;
    public GameObject two;
    public GameObject three;



    void Start()
    {

        step = -1;
        gameObject.GetComponent<AudioSource>().clip = null;
    }

    public void SwapRequest()
    {

        step += 1;

        if (step == 0)
        {

            one.SetActive(true);
            two.SetActive(true);
            three.SetActive(true);
        }

        if (step > 2)
        {

            step = 2;
        }

        if (step > -1)
        {
            gameObject.GetComponent<AudioSource>().mute = false;
            gameObject.GetComponent<AudioSource>().clip = requestSounds[step];
            gameObject.GetComponent<AudioSource>().Play();
        }
    }
}

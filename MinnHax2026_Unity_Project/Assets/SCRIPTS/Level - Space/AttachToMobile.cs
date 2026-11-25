using UnityEngine;

public class AttachToSlot : MonoBehaviour
{

    public AudioSource audioSource;
    public AudioClip audioSound;
    public string destination;


    public void Attach()
    {

        GameObject objectSlot = GameObject.Find(destination);  

        if (Vector3.Distance(objectSlot.transform.position, gameObject.transform.position) < 1.25f)
        {
            gameObject.tag = "Untagged";
            gameObject.layer = 0;
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
            gameObject.transform.position = objectSlot.transform.position;

            Destroy(objectSlot);

            audioSource.clip = audioSound;
            audioSource.Play();
        }
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

public class Interact : MonoBehaviour
{

    [Header("Pickup")]
    public Transform holdArea;
    private GameObject heldObj;
    private Rigidbody heldObjRB;
	public InputActionReference pickupActionReference;
    public GameObject CinemachineCameraTarget; //used by sound interactions as well
    public float pickupRange = 2.0f; //used by sound interactions as well
    public float move = 180.0f;
    public float damp = 10.0f;

    [Header("Interact w/ Sound")]
    public InputActionReference takeSoundActionReference;
    public InputActionReference playSoundActionReference;
    public Transform holdArea2;
    public AudioSource playerAudioSource;
    private AudioClip playerSoundClip;
    private GameObject heldWaveform;
    public GameObject cowPicTest;

    void Update()
    {

        if (pickupActionReference.action.triggered)
        {

            if (heldObj == null)
            {
                RaycastHit hit;
                if (Physics.Raycast(CinemachineCameraTarget.transform.position,CinemachineCameraTarget.transform.forward, out hit, pickupRange) && hit.transform.CompareTag("Pickup"))
                {
                    PickupObject(hit.transform.gameObject);
                }
            }
            else
            {
                DropObject();
            }
        }

        if (takeSoundActionReference.action.triggered)
        {

            RaycastHit hit;
            if (Physics.Raycast(CinemachineCameraTarget.transform.position,CinemachineCameraTarget.transform.forward, out hit, pickupRange) && hit.transform.CompareTag("Sound"))
            {
                TakeSound(hit.transform.gameObject);
            }
        }

        if (playSoundActionReference.action.triggered)
        {

            PlaySound();
        }
    }

    void FixedUpdate()
    {

        if (heldObj != null)
        {
            AdjustItem();
        } 

        if (heldWaveform != null)
        {
            AdjustPic();
        } 
    }

    void AdjustItem()
    {

        if(Vector3.Distance(heldObj.transform.position, holdArea.position) > 0.1f)
        {
            Vector3 moveDirection = (holdArea.position - heldObj.transform.position);
            //heldObjRB.AddForce(moveDirection * move, ForceMode.Force);
            //heldObjRB.MovePosition(heldObj.transform.position + moveDirection * Time.deltaTime * move);
            //heldObjRB.linearVelocity = moveDirection * move;
            //heldObjRB.linearVelocity = moveDirection / Mathf.Max((moveDirection.magnitude / move), Time.fixedDeltaTime);
            //heldObjRB.MovePosition(holdArea.position);
            Debug.Log($"linear velocity: {heldObjRB.linearVelocity}");
            Vector3 adjustForce = (moveDirection * move) - (heldObjRB.linearVelocity * damp);
            heldObjRB.AddForce(adjustForce, ForceMode.Force);
             
        }
    }

    void PickupObject(GameObject pickObj)
    {

        if (pickObj.GetComponent<Rigidbody>())
        {
            heldObjRB = pickObj.GetComponent<Rigidbody>();
            heldObjRB.useGravity = false;
            heldObjRB.linearDamping = 5.0f;
            //heldObjRB.angularDamping = 0;
            //heldObjRB.constraints = RigidbodyConstraints.FreezeRotation;
            heldObjRB.transform.parent = holdArea;
            heldObj = pickObj;
        }
    }   

    void DropObject()
    {

        heldObjRB.useGravity = true;
        heldObjRB.linearDamping = 0;
        //heldObjRB.angularDamping = 0.05f; 
        //heldObjRB.constraints = RigidbodyConstraints.None;
        heldObjRB.transform.parent = null;
        heldObj = null;
    }

    void AdjustPic()
    {

        heldWaveform.transform.SetParent(holdArea2.transform);
    }

    void TakeSound(GameObject newSound)
    {
        if (heldWaveform != null)
        {
            Destroy(heldWaveform);
        }

        if (newSound.name == "Cow")
        {
            heldWaveform = Instantiate(cowPicTest, holdArea2.position, holdArea2.rotation);
            AudioSource targetAudioSource = newSound.GetComponent<AudioSource>();
            AudioClip targetAudioClip = targetAudioSource.clip;
            playerSoundClip = targetAudioClip;
        }
    }

    void PlaySound()
    {

        if (playerSoundClip != null && !playerAudioSource.isPlaying)
        {
            Debug.Log("Mooooo");
            playerAudioSource.PlayOneShot(playerSoundClip, 1.0f);
            //create sphere collider around player
            //target will check for collision + check for the audioclip attached to hold area 2's audiosource
        }
    }
}

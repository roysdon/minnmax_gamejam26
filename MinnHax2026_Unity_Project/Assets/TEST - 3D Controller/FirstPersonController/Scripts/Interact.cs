using System.Collections.Generic;
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
    [SerializeField] private List<AudioClip> recordedSounds = new List<AudioClip>();
    [SerializeField] private int selectedSoundIndex = -1;
    private GameObject heldWaveform;

    private AudioClip SelectedSound => selectedSoundIndex >= 0 ? recordedSounds[selectedSoundIndex] : null;

    [Header("Sound Puzzle Progress")]
    public float clearDistance = 1.5f;

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
            if (Physics.Raycast(CinemachineCameraTarget.transform.position,CinemachineCameraTarget.transform.forward, out hit, pickupRange) && hit.transform.TryGetComponent(out SoundSource source) && source.IsActive)
            {
                TakeSound(source);
            }
        }

        if (playSoundActionReference.action.triggered)
        {
            PlaySound();
        }

        if (heldWaveform != null)
        {
            AdjustPic();
        }
    }

    void FixedUpdate()
    {

        if (heldObj != null)
        {
            AdjustItem();
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

    void TakeSound(SoundSource newSound)
    {
        if (heldWaveform != null)
        {
            Destroy(heldWaveform);
        }

        if (newSound.HeldSoundGraphic != null)
        {
            heldWaveform = Instantiate(newSound.HeldSoundGraphic, holdArea2.position, holdArea2.rotation);
        }

        if (!recordedSounds.Contains(newSound.Clip))
        {
            recordedSounds.Add(newSound.Clip);
        }

        selectedSoundIndex = recordedSounds.IndexOf(newSound.Clip);
        newSound.Capture();
    }

    void PlaySound()
    {

        if (SelectedSound != null && !playerAudioSource.isPlaying)
        {
            playerAudioSource.PlayOneShot(SelectedSound, 1.0f);

            //RaycastHit hit;
            // && Physics.Raycast(CinemachineCameraTarget.transform.position,CinemachineCameraTarget.transform.forward, out hit, pickupRange) && hit.transform.CompareTag("Sound")
            SoundReceiver receiver = FindReceiverInRange(SelectedSound);
            if (receiver != null)
            {
                receiver.Solve();
                Destroy(heldWaveform);
            }
        }
    }

    SoundReceiver FindReceiverInRange(AudioClip clip)
    {
        foreach (Collider nearby in Physics.OverlapSphere(holdArea2.position, clearDistance))
        {
            SoundReceiver receiver = nearby.GetComponentInParent<SoundReceiver>();
            if (receiver != null && receiver.Accepts(clip))
            {
                return receiver;
            }
        }
        return null;
    }
}

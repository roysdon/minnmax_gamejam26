using System.Collections.Generic;
using TMPro;
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
    public InputActionReference nextSoundActionReference;
    public Transform holdArea2;
    public AudioSource playerAudioSource;
    [SerializeField] private List<AudioClip> recordedSounds = new List<AudioClip>();
    [SerializeField] private int selectedSoundIndex = -1;
    private GameObject heldWaveform;
    [SerializeField] private TMP_Text soundListText = null;

    // Message shown on HUD for a set duration
    public TMP_Text messageText = null;
    private float messageHideTime;

    // Object interaction prompt text
    public TMP_Text promptText = null;

    private AudioClip SelectedSound => selectedSoundIndex >= 0 ? recordedSounds[selectedSoundIndex] : null;

    [Header("Sound Puzzle Progress")]
    public float clearDistance = 1.5f;

    void Start()
    {
        UpdateSoundListText();
    }

    void Update()
    {
        Interactable lookedAt = FindTargetInteractable();
        UpdatePromptText(lookedAt);

        if (pickupActionReference.action.triggered)
        {
            if (heldObj == null && lookedAt != null)
            {
                lookedAt.Use(this);
            }
            else if (heldObj == null)
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
            if (Physics.Raycast(CinemachineCameraTarget.transform.position,CinemachineCameraTarget.transform.forward, out hit, pickupRange) && hit.transform.TryGetComponent(out SoundSource source))
            {
                if (source.IsBlocked)
                {
                    ShowMessage("Sound is blocked", 2.0f);
                }
                else if (source.CanBeCaptured)
                {
                    TakeSound(source);
                }
            }
        }

        if (playSoundActionReference.action.triggered)
        {
            PlaySound();
        }

        if (nextSoundActionReference.action.triggered)
        {
            CycleSound(true);
        }

        if (heldWaveform != null)
        {
            AdjustPic();
        }

        if (messageText != null && Time.time >= messageHideTime)
        {
            messageText.text = "";
        }
    }

    Interactable FindTargetInteractable()
    {
        RaycastHit hit;
        if (Physics.Raycast(CinemachineCameraTarget.transform.position, CinemachineCameraTarget.transform.forward, out hit, pickupRange))
        {
            return hit.transform.GetComponentInParent<Interactable>();
        }
        return null;
    }

    void UpdatePromptText(Interactable lookedAt)
    {
        if (promptText == null)
        {
            return;
        }

        string prompt = lookedAt != null ? lookedAt.prompt : "";
        if (promptText.text != prompt)
        {
            promptText.text = prompt;
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

            if (heldObj.TryGetComponent(out Pickupable pickupable))
            {
                pickupable.PickedUp();
            }
        }
    }   

    void DropObject()
    {

        heldObjRB.useGravity = true;
        heldObjRB.linearDamping = 0;
        //heldObjRB.angularDamping = 0.05f; 
        //heldObjRB.constraints = RigidbodyConstraints.None;
        heldObjRB.transform.parent = null;

        if (heldObj.TryGetComponent(out Pickupable pickupable))
        {
            pickupable.Dropped();
        }

        heldObj = null;
    }

    void AdjustPic()
    {

        heldWaveform.transform.SetParent(holdArea2.transform);
    }

    void TakeSound(SoundSource newSound)
    {
        if (newSound.Clip == null)
        {
            Debug.LogWarning(newSound.name + " has a SoundSource but its AudioSource has no clip", newSound);
            return;
        }

        if (heldWaveform != null)
        {
            Destroy(heldWaveform);
        }

        if (newSound.heldSoundGraphic != null)
        {
            heldWaveform = Instantiate(newSound.heldSoundGraphic, holdArea2.position, holdArea2.rotation);
        }

        if (!recordedSounds.Contains(newSound.Clip))
        {
            recordedSounds.Add(newSound.Clip);
        }

        SelectSound(recordedSounds.IndexOf(newSound.Clip));
        newSound.Capture();
    }

    void CycleSound(bool forward)
    {
        if (recordedSounds.Count == 0)
        {
            return;
        }

        int step = forward ? 1 : -1;
        SelectSound((selectedSoundIndex + step + recordedSounds.Count) % recordedSounds.Count);
    }

    void SelectSound(int index)
    {
        if (index < 0 || index >= recordedSounds.Count)
        {
            return;
        }

        selectedSoundIndex = index;
        UpdateSoundListText();
    }

    void UpdateSoundListText()
    {
        if (soundListText == null)
        {
            return;
        }

        string text = "";
        for (int i = 0; i < recordedSounds.Count; i++)
        {
            string soundName = recordedSounds[i] != null ? recordedSounds[i].name : "<missing sound>";

            if (i == selectedSoundIndex)
            {
                text += "<b>> " + soundName + "</b>\n";
            }
            else
            {
                text += soundName + "\n";
            }
        }

        soundListText.text = text;
    }

    void ShowMessage(string message, float duration)
    {
        if (messageText == null)
        {
            Debug.Log(message);
            return;
        }

        messageText.text = message;
        messageHideTime = Time.time + duration;
    }

    void PlaySound()
    {

        if (SelectedSound != null)
        {
            playerAudioSource.Stop();
            playerAudioSource.clip = SelectedSound;
            playerAudioSource.Play();

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

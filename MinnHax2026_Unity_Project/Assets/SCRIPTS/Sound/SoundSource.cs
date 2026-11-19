using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundSource : MonoBehaviour
{
    // An object's sound can either be taken (object muted) or recorded (object sound keeps playing)
    public enum CaptureMode { Take, Record }

    public CaptureMode mode = CaptureMode.Take;

    [SerializeField] private bool isActive = true;

    public bool IsActive => isActive;

    // Visual representation of a held sound
    public GameObject heldSoundGraphic = null;

    // A louder sound source that prevents this sound from being used
    public SoundSource blockedBy = null;

    public AudioClip Clip => GetComponent<AudioSource>().clip;

    public bool IsAudible
    {
        get
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            return isActive && audioSource.isPlaying && !audioSource.mute;
        }
    }

    public bool IsBlocked => blockedBy != null && blockedBy.IsAudible;

    public bool CanBeCaptured => isActive && !IsBlocked;

    void Start()
    {
        if (!isActive)
        {
            GetComponent<AudioSource>().Stop();
        }
    }

    public void Activate()
    {
        if (isActive)
        {
            return;
        }

        isActive = true;
        GetComponent<AudioSource>().Play();
    }

    public void Deactivate()
    {
        if (!isActive)
        {
            return;
        }

        isActive = false;
        GetComponent<AudioSource>().Stop();
    }

    public void ChangeClip(AudioClip newClip)
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.clip = newClip;

        if (isActive)
        {
            audioSource.Play();
        }
    }

    public void Capture()
    {
        if (mode == CaptureMode.Take)
        {
            GetComponent<AudioSource>().mute = true;
        }
    }
}

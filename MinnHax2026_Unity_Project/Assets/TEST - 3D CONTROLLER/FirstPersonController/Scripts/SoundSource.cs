using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundSource : MonoBehaviour
{
    // An object's sound can either be taken (object muted) or recorded (object sound keeps playing)
    public enum CaptureMode { Take, Record }

    public CaptureMode mode = CaptureMode.Take;

    // Visual representation of a held sound
    [SerializeField] private GameObject heldSoundGraphic = null;

    public GameObject HeldSoundGraphic => heldSoundGraphic;

    public AudioClip Clip => GetComponent<AudioSource>().clip;

    public void Capture()
    {
        if (mode == CaptureMode.Take)
        {
            GetComponent<AudioSource>().mute = true;
        }
    }
}

using UnityEngine;
using UnityEngine.Events;

public class SoundReceiver : MonoBehaviour
{
    public AudioClip requiredSound = null;

    public UnityEvent onSolved = new UnityEvent();

    private bool isSolved;

    public bool Accepts(AudioClip clip)
    {
        return !isSolved && clip == requiredSound;
    }

    public void Solve()
    {
        isSolved = true;
        onSolved.Invoke();
    }
}

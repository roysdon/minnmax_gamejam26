using UnityEngine;
using UnityEngine.Events;

public class SoundReceiver : MonoBehaviour
{
    [SerializeField] private AudioClip requiredSound = null;

    [SerializeField] private UnityEvent onSolved = new UnityEvent();

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

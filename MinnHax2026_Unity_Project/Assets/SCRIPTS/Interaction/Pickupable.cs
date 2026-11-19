using UnityEngine;
using UnityEngine.Events;

// Allows held objects to trigger events on pickup/drop
public class Pickupable : MonoBehaviour
{
    public UnityEvent onPickedUp = new UnityEvent();

    public UnityEvent onDropped = new UnityEvent();

    public void PickedUp()
    {
        onPickedUp.Invoke();
    }

    public void Dropped()
    {
        onDropped.Invoke();
    }
}

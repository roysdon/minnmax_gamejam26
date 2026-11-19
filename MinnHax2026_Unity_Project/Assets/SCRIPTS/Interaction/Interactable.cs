using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public string prompt = "[E] Interact";

    // Takes player's interact component so objects can affect player
    public UnityEvent<Interact> onInteract = new UnityEvent<Interact>();

    public void Use(Interact player)
    {
        onInteract.Invoke(player);
    }
}

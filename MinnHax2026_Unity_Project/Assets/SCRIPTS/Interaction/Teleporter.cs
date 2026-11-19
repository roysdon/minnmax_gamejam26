using UnityEngine;

// Moves the player to a target location (top of a rope, through a door, down a hole...).
// Wire an Interactable's On Interact to Teleport.
public class Teleporter : MonoBehaviour
{
    // Where the player ends up: place it above the floor, clear of walls
    public Transform target = null;

    public void Teleport(Interact player)
    {
        if (target == null)
        {
            Debug.LogWarning(name + " has a Teleporter with no Target assigned", this);
            return;
        }

        // A CharacterController overrides transform changes while enabled, so turn it off for the move
        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false;
        }

        player.transform.position = target.position;

        if (controller != null)
        {
            controller.enabled = true;
        }
    }
}

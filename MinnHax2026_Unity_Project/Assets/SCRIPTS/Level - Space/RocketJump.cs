using UnityEngine;
using StarterAssets;

public class RocketJump : MonoBehaviour
{

	public GameObject player;



	public void JumpBuff()
	{

		FirstPersonController firstPersonController = player.GetComponent<FirstPersonController>();
        firstPersonController.JumpHeight = 15f;
	}

	public void JumpDebuff()
	{

		FirstPersonController firstPersonController = player.GetComponent<FirstPersonController>();
        firstPersonController.JumpHeight = 2.25f;
	}
}

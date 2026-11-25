using UnityEngine;

public class MoveObject : MonoBehaviour
{

    public Vector3 destination;
    public float moveSpeed;
    private bool moving = false;
    public GameObject newRocket;

    public void MoveToDestination()
    {

        gameObject.tag = "Untagged";
        gameObject.layer = 0;
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        gameObject.GetComponent<Collider>().enabled = false;
        gameObject.transform.eulerAngles = new Vector3(0,0,0);
        moving = true;
    }

    void Update()
    {

        if (moving == true)
        {
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, destination, moveSpeed * Time.deltaTime);
        }

        if (moving == true && gameObject.transform.position.y == 6.299f)
        {
            GameObject one = GameObject.Find("Thrusters");
            GameObject two = GameObject.Find("Body");
            GameObject three = GameObject.Find("Nose");
            Destroy(one);
            Destroy(two);
            newRocket.SetActive(true);
            Destroy(three);
        }
    }
}

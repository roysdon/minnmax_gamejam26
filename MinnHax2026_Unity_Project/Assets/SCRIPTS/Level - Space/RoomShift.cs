using UnityEngine;
using System.Collections;
using StarterAssets;

public class RoomShift : MonoBehaviour
{

    public GameObject toyBoxLid;
    public GameObject[] planets = new GameObject[11];
    [SerializeField] GameObject[] walls = new GameObject[6];
    public float yForce = 5f;
    public float newGravity = -5f;
    public GameObject player;
    public float newPlayerGravity = -5f;
    public float newJumpHeight = 2.5f;
    public Camera mainCamera;
    public Vector3 wallExplosionOrigin;
    public AudioSource music;
    public AudioSource activation;
    public AudioReverbZone spaceReverb;
    public GameObject padLock;



    public void StartShift()
    {

        music.Play();
        Invoke(nameof(Shift), 8);
    }

    public void Shift()
    {

        spaceReverb.GetComponent<AudioReverbZone>().enabled = true;
        activation.Play();
        toyBoxLid.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        toyBoxLid.GetComponent<Rigidbody>().AddForce((new Vector3(Random.Range(-1,1), yForce, Random.Range(-1,1))), ForceMode.Impulse);
        Physics.gravity = new Vector3(0,newGravity,0);
        StartCoroutine(SpawnPlanets());
        FirstPersonController firstPersonController = player.GetComponent<FirstPersonController>();
        firstPersonController.Gravity = newPlayerGravity;
        firstPersonController.JumpHeight = newJumpHeight;
        walls = GameObject.FindGameObjectsWithTag("Wall");
        StartCoroutine(MoveWalls());
        //mainCamera.GetComponent<AudioLowPassFilter>().enabled = true;
    }

    IEnumerator SpawnPlanets()
    {

        for (int i = 0; i < 11; i++)
        {
            this.GetComponent<AudioSource>().Play();
            GameObject planet = Instantiate(planets[i], transform.position, transform.rotation);
            planet.GetComponent<Rigidbody>().AddForce((new Vector3(Random.Range(-2f,2f), yForce, Random.Range(-2f,2f))), ForceMode.Impulse);
            yield return new WaitForSeconds(0.3f);

            if (i == 5)
            {
                toyBoxLid.GetComponent<Rigidbody>().useGravity = false;
                toyBoxLid.GetComponent<Collider>().enabled = false;
                Destroy(toyBoxLid, 3);
            }
        }
    }

    IEnumerator MoveWalls()
    {

        foreach (GameObject wall in walls)
        {
            Vector3 forceOrigin = wallExplosionOrigin;
            Vector3 forceDirection = (wall.transform.position - forceOrigin);
            wall.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            wall.GetComponent<Rigidbody>().AddForce(forceDirection, ForceMode.Impulse);
            wall.GetComponent<Rigidbody>().AddTorque(forceDirection, ForceMode.Impulse);
            Destroy(wall, 15);
            yield return 0;
        }
    }

    public void Win()
    {

        Debug.Log("YOU WIN!!!");
    }
}

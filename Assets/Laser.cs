using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public Rigidbody body; //player
    public GameObject target; //cursor target
    public Transform spawnPoint; //where user will be moved if they're hit - could probably have a more obvious name
    public float speed; //how fast the laser should move - recommend 5-20
    public Rigidbody bolt; //laser object prefab
    public float timer; //how often lasers should be generated - 0.1 is hard, 0.5 medium, 1 easy
    [HideInInspector] public GameObject instance; //gameobject used to instantiate laser
    public List<Transform> spawnPoints; //list of possible spawn points for laser
    public int laserCount; //number of lasers to be generated each time - can't be higher than spawnPoints length

    private Rigidbody laser; //object to like totally be like the new laser. omg that was like so obvious
    private System.Random random = new System.Random(); //woo we be random baby
    private Color shootColour = Color.black; //if your ring flashes black i recommend you see a doctor
    private Color normalColour = Color.red; //and if it flashes red you need to call an ambulance now.


    void Update()
    {
        //if it's time to generate lasers then proceed otherwise BACK OFF
        timer -= 1 * Time.deltaTime;
        if (timer <= 0)
        {
            //create number of new lasers wanted at a time
            for (int i = 0; i < laserCount; i++)
            {
                //random spawnpoint chosen
                int count = random.Next(0, spawnPoints.Count);
                //create laser and set the transform up using spawnPoint
                laser = Instantiate(bolt);
                laser.position = spawnPoints[count].position;
                laser.rotation = spawnPoints[count].rotation;
                laser.velocity = spawnPoints[count].transform.forward * speed;
                //have the laser ring (one at each spawnPoint) flash to signal it's creating a laser
                //could have the laser generation delayed to warn player a laser is about to be created
                StartCoroutine(ringFlash(spawnPoints[count]));

                //set up collision system for if laser hits player
                PlayerHit playerHit = laser.GetComponent<PlayerHit>();
                playerHit.body = body;
                playerHit.target = target;
                playerHit.spawnPoint = spawnPoint;

                //destroy the laser after 5 seconds - otherwise you end up with 50 million lasers
                Destroy(laser.gameObject, 5f);
            }
            //reset timer 
            timer = 1;
        }
    }


    IEnumerator ringFlash(Transform spawn)
    {
        //change colour for flash then change it back after x seconds
        spawn.GetComponentInChildren<Renderer>().material.SetColor("_Color", shootColour);
        yield return new WaitForSeconds(0.5f);
        spawn.GetComponentInChildren<Renderer>().material.SetColor("_Color", normalColour);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{

    public Rigidbody body; //player
    public GameObject target; //cursor target

    [HideInInspector] public GameObject instance; //new object instatiation
    public List<Transform> points; //potential spawn points
    public List<Transform> mPoints; //mandatory spawn points
    public Rigidbody obstacle; //woo new object 
    public int obstacleCount; //number of obstacles requested

    private System.Random random = new System.Random(); //we random baby
    private List<Transform> spawnPoints; //will be copy of points
    private List<Transform> mandatorySpawnPoints; //will be copy of mPoints


    void Start()
    {
        //Copy lists to allow removal - no duplicate spawn points
        spawnPoints = new List<Transform>(points);
        mandatorySpawnPoints = new List<Transform>(mPoints);

        //For each required obstacle create a new object
        for (int i = 0; i < obstacleCount; i++)
        {
            //create mandatory points first (block door and weird bug flicker in centre of room)
            Transform spawnPoint;
            if (mandatorySpawnPoints.Count > 0)
            {
                spawnPoint = mandatorySpawnPoints[random.Next(0, mandatorySpawnPoints.Count)];
                mandatorySpawnPoints.Remove(spawnPoint);
            }
            else
            {
                //create rest of points
                spawnPoint = spawnPoints[random.Next(0, spawnPoints.Count)];
                spawnPoints.Remove(spawnPoint);
            }
            //format obstacle
            createObstacle(spawnPoint);
        }
    }


    public void createObstacle(Transform spawnPoint)
    {
        Rigidbody newObstacle = Instantiate(obstacle);
        newObstacle.position = spawnPoint.position;

        //randomise rotation to make it less uniform
        Quaternion vector = newObstacle.rotation;
        vector.x = Random.Range(0f, 360f);
        vector.y = Random.Range(0f, 360f);
        vector.z = Random.Range(0f, 360f);
        vector = vector.normalized;
        newObstacle.rotation = vector;
        //randomise rendering of tiling - changes appearance of outside layer
        Renderer rend = newObstacle.GetComponent<Renderer>();
        float x = Random.Range(0.1f, 0.5f);
        float y = Random.Range(0.1f, 0.5f);
        rend.material.mainTextureScale = new Vector2(x, y);

        //set it to collider - allows player to 'stick' to it
        Collision coll = newObstacle.GetComponent<Collision>();
        coll.body = body;
        coll.target = target;
    }

    public void Reset()
    {
        //create all old objects when player is hit then regenerate them - means player doesn't get bored of room
        GameObject[] oldObjects = GameObject.FindGameObjectsWithTag("Obstacle");

        foreach (GameObject obstacle in oldObjects)
        {
            Destroy(obstacle);
        }

        Start();
    }

}

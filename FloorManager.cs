using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FloorManager : MonoBehaviour
{
    public GameObject finalFloor; //Floor prefab with the exit
    public int floors; //Amount of floor to generate
    public float floorDistance; //Distance between floors

    public static FloorManager Instance; //Creates and instance of the FloorManager to acces it everywhere

    public List<GameObject> floorInstances; //List with floor prefabs

    public List<Transform> enemyLocations; //Locations where enemies can spawn
    public List<GameObject> floorObjects; //List with the generated floors

    private float floorRotation = 180; //Rotations required to fit the floors correctly

    private List<NavMeshSurface> navMeshSurfaces; //Navmesh for the enemies
    private List<Vector3> waypoints; //List with waypoint triggers
    
    Vector3 curPos;

    void Awake()
    {
        if (FloorManager.Instance == null)
            FloorManager.Instance = this;
        else
            Destroy(this.gameObject);
    }

    void Start()
    {
        waypoints = new List<Vector3>();
        enemyLocations = new List<Transform>();
        floorObjects = new List<GameObject>();

        for (int i = 0; i < floors; i++) //Generates new floor and adds it as child
        {
            GameObject newFloor = Instantiate(floorInstances[Random.Range(0,floorInstances.Count)], curPos, Quaternion.identity);
            if (i%2==1)
            {
                newFloor.transform.Rotate(newFloor.transform.localPosition.x, floorRotation ,newFloor.transform.localPosition.z);
            }
            curPos.y -= floorDistance;
            newFloor.transform.parent = FloorManager.Instance.transform;
            floorObjects.Add(newFloor);            
        }        
        
        GameObject fFloor = Instantiate(finalFloor, curPos, Quaternion.identity);
        if (floors % 2 == 1) //Rotates every other floor to make the entrances link up
        {
            fFloor.transform.Rotate(fFloor.transform.localPosition.x, floorRotation, fFloor.transform.localPosition.z);
        }
        curPos.y -= floorDistance;
        fFloor.transform.parent = FloorManager.Instance.transform;
        floorObjects.Add(fFloor);



        for (int i = 0; i < floors; i++) //Adds all waypoints to check if player entered floor
        {
            Vector3 pos = GameObject.FindGameObjectsWithTag("Waypoint")[i].transform.localPosition;
            waypoints.Add(pos);
        }

        BakeNavmesh(); //Bakes the navmesh with all the floors for AI navigation
        SpawnManager.Instance.LoadFloors(floorObjects);
    }


    private void BakeNavmesh() //Bakes the mesh of all floors
    {
        for (int i = 0; i < FindObjectsOfType<NavMeshSurface>().Length; i++)
        {
            FindObjectsOfType<NavMeshSurface>()[i].BuildNavMesh();
        }
    }

    public Vector3 GetWaypoint()
    {
        return waypoints[0];   
    }

}

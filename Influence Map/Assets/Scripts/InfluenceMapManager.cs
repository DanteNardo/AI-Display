using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfluenceMapManager : MonoBehaviour {

    public GameObject influencer1;
    public GameObject influencer2;
    public GameObject influencer3;
    public GameObject influencer4;

    public GameObject GreenTeamParent;
    public GameObject RedTeamParent;

    public Camera overheadCamera;

    public GameObject terrain;
    public GameObject bridge;



    //size of the map
    public int x = 100;
    public int y = 100;

    //data for the map, true: location can be moved to, false: location can't be moved to
    public bool[,] mapData;

    //list of obstacles to get to mapData
    public GameObject[] obstacles;

    //block used to test the mapData
    public GameObject testBlock;

    //use to toggle the two teams, true = green, false = red
    private bool team = true;

    RaycastHit hit;

    Ray ray;

    // Use this for initialization
    void Start () {

        Cursor.visible = true;

        Cursor.lockState = CursorLockMode.None;
        
        //initialize the mapData
        mapData = new bool[x, y];

        //get the data for the map
        GetMapData();


    }
	
	// Update is called once per frame
	void Update () {

        //place the appropriate influencer
        if (Input.GetKeyDown("1"))
        {
            PlaceInfluencer(influencer1);
        }
        else if (Input.GetKeyDown("2"))
        {
            PlaceInfluencer(influencer2);
        }
        else if (Input.GetKeyDown("3"))
        {
            PlaceInfluencer(influencer3);
        }
        else if (Input.GetKeyDown("4"))
        {
            PlaceInfluencer(influencer4);
        }

        //swap the team
        else if (Input.GetKeyDown("t"))
        {
            team = !team;
        }

        //generate the influence map
        else if (Input.GetKeyDown("g"))
        {
            PlaceInfluencer(influencer1);
        }
    }

    //get the data for placing influencers in valid places
    private void GetMapData()
    {
        //initialize the data in mapData to true
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                mapData[i, j] = true;
            }
        }

        //get all of the obstacles
        obstacles = GameObject.FindGameObjectsWithTag("Obstacle");

        //Debug.Log(obstacles.Length);

        //values for the obstacles to calc invalid locations
        int sizeX;
        int sizeY;
        int locX;
        int locY;

        //for all of the obstacles
        for (int i = 0; i < obstacles.Length; i++)
        {
            //get the size of the obstacle
            sizeX = (int)obstacles[i].transform.localScale.x;
            sizeY = (int)obstacles[i].transform.localScale.z;

            //get the location of the obstacle in the grid
            locX = (int)obstacles[i].transform.position.x + x / 2 - sizeX / 2;
            locY = (int)(obstacles[i].transform.position.z - y / 2) * -1 - sizeY / 2;

            //for all values in the grid that are overlaped by the obstacle
            for (int tempX = locX; tempX < locX + sizeX; tempX++)
            {
                for (int tempY = locY; tempY < locY + sizeY; tempY++)
                {
                    //update the mapData at that location to be false
                    mapData[tempX, tempY] = false;
                }
            }
        }

        //display the mapData
        //TestMapData();
    }

    //test to display mapdata, red shows available spaces 
    private void TestMapData()
    {
        //for all of mapData
        for (int i = 0; i < y; i++)
        {
            for (int j = 0; j < x; j++)
            {
                //if mapData is true
                if (mapData[j, i])
                {
                    //display ground that can be moved on
                    Instantiate(testBlock, new Vector3(j - x / 2 + .5f, 10, -i + y / 2 - .5f), Quaternion.identity);
                }
            }
        }
    }

    private void PlaceInfluencer(GameObject influencer)
    {
        //make a ray from the mouse
        ray = overheadCamera.ScreenPointToRay(Input.mousePosition);

        //cast a ray and check if it is valid
        if(Physics.Raycast(ray, out hit) && (hit.transform == terrain.transform || hit.transform == bridge.transform))
        {
            //get location for the map array manipulation
            Vector2 hitLocation = new Vector2(hit.point.x, hit.point.z);

            //manipulate the coordinates into mapData coordinates
            int xLoc = (int)( hitLocation.x + (x / 2) );

            int yLoc = -1 * (int)( hitLocation.y - (y / 2) );

            //validate the coordinates
            if(xLoc >= 0 && xLoc < x && yLoc >= 0 && yLoc < y && mapData[xLoc, yLoc])
            {
                //create a world location from the mapData coordinates
                Vector3 placementLocation = new Vector3(xLoc - (x / 2) + .5f, hit.point.y, -yLoc + y / 2 - .5f);

                //create the new influencer
                GameObject tempInfluencer = Instantiate(influencer, placementLocation, Quaternion.identity);

                //set the influencer's stored coordinates to be used in the influence map creation
                tempInfluencer.GetComponent<Influencer>().SetMapLocation(xLoc, yLoc);

                //if it is set to green team
                if(team)
                {
                    tempInfluencer.transform.parent = GreenTeamParent.transform;
                }
                //else it is set to red team
                else
                {
                    tempInfluencer.transform.parent = RedTeamParent.transform;
                }
            }
            
        }
    }
}

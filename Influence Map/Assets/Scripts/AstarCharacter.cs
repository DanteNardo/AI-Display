using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class AstarCharacter : MonoBehaviour
{
    //reference to the mapManager to call functions
    public GameObject mapManager;

    //location in the grid
    public int xLoc;
    public int yLoc;

    //the path to follow
    public List<Vector2> path;

    //raycasting variables
    private RaycastHit hit;
    private Ray downRay;

    //bool to determine if the character is active
    private bool active;

    // Use this for initialization
    void Start()
    {
        active = true;
    }

    // Update is called once per frame
    void Update()
    {
        //if the character is active, Follow the path it has
        if(active)
        {
            FollowPath();
        }
    }

    //assign a new path, used in MapManager
    public void AssignPath(List<Vector2> newPath)
    {
        path = newPath;
    }

    //have the character follow the path it has
    private void FollowPath()
    {
        //if there is a path
        if(path != null && path.Count > 0)
        {
            // move in path direction
            this.transform.position += (new Vector3(path[0].x - this.transform.position.x, GetHeight(), path[0].y - this.transform.position.z)).normalized / 10;

            //if the character has reached the next node in the path
            if (Vector2.Distance(path[0], new Vector2(this.transform.position.x, this.transform.position.z)) < .05)
            {
                //update the grid location
                xLoc = (int)(path[0].x + mapManager.GetComponent<MapManager>().x / 2 - .5f);
                yLoc = (int)(-1 * path[0].y + mapManager.GetComponent<MapManager>().y / 2 - .5f);

                //remove this path node to now move toward the next node
                path.RemoveAt(0);
            }
        }

        //if there is no path or we have reached the end
        else
        {
            //create a new path to follow
            path = mapManager.GetComponent<MapManager>().CreateRandomPath(xLoc, yLoc);
        }
    }

    //get the height of the character so it will hover above the ground at a set height
    public float GetHeight()
    {
        //ray pointing down from location
        downRay = new Ray(this.transform.position, -1 * Vector3.up);

        //if the character is above the terrain
        if(Physics.Raycast(downRay, out hit))
        {
            return 1 - hit.distance;
        }

        //incase we are below the terrain
        else
        {
            downRay = new Ray(this.transform.position, Vector3.up);

            return 1 + hit.distance;
        }
    }
}
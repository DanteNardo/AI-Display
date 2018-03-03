using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class AstarCharacter : MonoBehaviour
{

    public GameObject mapManager;

    public int xLoc;

    public int yLoc;

    public List<Vector2> path;

    private RaycastHit hit;

    private Ray downRay;

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        FollowPath();
    }

    public void AssignPath(List<Vector2> newPath)
    {
        path = newPath;
    }

    private void FollowPath()
    {
        if(path != null && path.Count > 0)
        {
            // move in path direction


            this.transform.position += (new Vector3(path[0].x - this.transform.position.x, GetHeight(), path[0].y - this.transform.position.z)).normalized / 10;

            if (Vector2.Distance(path[0], new Vector2(this.transform.position.x, this.transform.position.z)) < .05)
            {
                xLoc = (int)(path[0].x + mapManager.GetComponent<MapManager>().x / 2 - .5f);

                yLoc = (int)(-1 * path[0].y + mapManager.GetComponent<MapManager>().y / 2 + .5f);

                path.RemoveAt(0);
            }
        }

        else
        {
            path = mapManager.GetComponent<MapManager>().CreateRandomPath(xLoc, yLoc - 1);
        }
    }

    public float GetHeight()
    {
        //ray pointing down from location
        downRay = new Ray(this.transform.position, -1 * Vector3.up);

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
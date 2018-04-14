using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using System;

public class SceneManager : MonoBehaviour
{
    public GameObject aStar;
    public List<GameObject> flockers;
    private List<Flocker> flockerComps;
    private Vector3[] originalPositions;
    private GameObject[] temp;

    public GameObject target;
    private Vector3 avgDirection;
    private Vector3 avgPosition;

    private bool debugMode;

    public Material blue;
    public Material green;

    private List<GameObject> obstacles;
    
    private Terrain terrain;
    private int cornerIndex;
    private GameObject endGoal;
    private NavMeshPath path;

    // Use this for initialization
    void Start()
    {

        flockers = new List<GameObject>();
        flockerComps = new List<Flocker>();
        temp = new GameObject[GameObject.FindGameObjectsWithTag("Flocker").Length];
        temp = GameObject.FindGameObjectsWithTag("Flocker");
        foreach (GameObject g in temp)
        {
            flockers.Add(g);
            flockerComps.Add(g.GetComponent<Flocker>());
        }

        originalPositions = new Vector3[flockers.Count];
        for( int index = 0; index < flockers.Count; index++)
        {
            originalPositions[index] = flockers[index].transform.position;
        }

        this.target = GameObject.FindGameObjectWithTag("Target");

        avgDirection = Vector3.zero;
        avgPosition = Vector3.zero;

        terrain = GameObject.Find("Terrain").GetComponent<Terrain>();
        endGoal = GameObject.Find("EndGoal");

        path = new NavMeshPath();


        NavMesh.CalculatePath(flockers[0].transform.position,endGoal.transform.position,1<<NavMesh.GetAreaFromName("Walkable"), path);

        cornerIndex = 0;
        target.transform.position = path.corners[cornerIndex];

        
        temp = new GameObject[ GameObject.FindGameObjectsWithTag( "Obstacle" ).Length ];
        temp = GameObject.FindGameObjectsWithTag( "Obstacle" );
        obstacles = new List<GameObject>();
        foreach( GameObject g in temp )
        {
            obstacles.Add( g );
        }
        foreach (GameObject g in flockers)
        {
            g.GetComponent<Flocker>().target = target;
        }

    }

    // Update is called once per frame
    void Update()
    {
        // Check key presses
        CheckKeys();


        avgDirection = Vector3.zero;
        avgPosition = Vector3.zero;
        //in case number of flockers changes

        //convert array to list


        //average the position and direction
        foreach( GameObject g in flockers )
        {
            //if( g.GetComponent<Flocker>().direction != Vector3.zero )
                avgDirection += g.GetComponent<Flocker>().transform.forward;
            //if( g.GetComponent<Flocker>().position != Vector3.zero )
                avgPosition += g.GetComponent<Flocker>().position;
        }

        avgDirection = avgDirection.normalized;



        avgDirection *= flockers[ 0 ].GetComponent<VehicleMovement>().maxSpeed;
        avgPosition /= flockers.Count;
        GameObject.Find( "AveragePosition" ).transform.position = avgPosition;
        if( avgDirection != Vector3.zero )
        {
            GameObject.Find( "AveragePosition" ).transform.rotation = Quaternion.LookRotation( avgDirection );
        }
        //send information to flockers
        foreach( GameObject g in flockers )
        {
            g.GetComponent<Flocker>().avgDirection = this.avgDirection;
            g.GetComponent<Flocker>().avgPosition = this.avgPosition;
        }

        //find closest flocker to every flocker
        //outer is the one we are comparing with
        for( int outer = 0; outer < flockers.Count; outer++ )
        {
            float distance = int.MaxValue;
            int indexOfClosest = int.MaxValue;
            //inner to
            for( int inner = 0; inner < flockers.Count; inner++ )
            {
                if( flockers[outer] != flockers[inner] && Vector3.Distance( flockers[ outer ].transform.position, flockers[ inner ].transform.position ) < distance )
                {
                    distance = Vector3.Distance( flockers[ outer ].transform.position, flockers[ inner ].transform.position );
                    indexOfClosest = inner;
                }
            }
            if( indexOfClosest != int.MaxValue )
            { 
                flockers[ outer ].GetComponent<Flocker>().nearestNeighbor = flockers[ indexOfClosest ];
            }
        }


        for( int outer = 0; outer < flockers.Count; outer++ )
        {
            float distance = int.MaxValue;
            int indexOfClosest = int.MaxValue;

            for( int inner = 0; inner < obstacles.Count; inner++ )
            {
                if( Vector3.Distance( ProjectToXZ( flockers[outer].transform.position), ProjectToXZ( obstacles[inner].transform.position) ) < distance )
                {
                    distance = Vector3.Distance( ProjectToXZ( flockers[ outer ].transform.position), ProjectToXZ( obstacles[ inner ].transform.position ) );
                    indexOfClosest = inner;
                }
            }
            if( indexOfClosest != int.MaxValue )
            {
                flockers[ outer ].GetComponent<Flocker>().nearestObstacle = obstacles[ indexOfClosest ];
            }
        }

        //update target
        for (int index = 0; index < flockers.Count; index++)
        {
            //if flocker and target are within .7 units, reset position of goal
            if (Vector3.Distance(ProjectToXZ(flockers[index].transform.position), ProjectToXZ(target.transform.position)) < 1f)
            {
                cornerIndex++;
                if(cornerIndex >= path.corners.Length)
                {
                    endGoal.transform.position = new Vector3(UnityEngine.Random.value * 80, 0.0f, UnityEngine.Random.value * 80);
                    endGoal.transform.position += new Vector3(0, GetHeight(endGoal.transform.position), 0);

                    while (!(NavMesh.CalculatePath(flockers[0].transform.position, endGoal.transform.position, 1 << NavMesh.GetAreaFromName("Walkable"), path)))
                    {
                        endGoal.transform.position = new Vector3(UnityEngine.Random.value * 80, 0.0f, UnityEngine.Random.value * 80);
                        endGoal.transform.position += new Vector3(0, GetHeight(endGoal.transform.position), 0);
                    }

                    cornerIndex = 0;

                    target.transform.position = path.corners[cornerIndex];
                    break;
                }

                target.transform.position = path.corners[cornerIndex];
            }
        }


    }
    Vector3 ProjectToXZ( Vector3 input )
    {
        return new Vector3(input.x, 0, input.z);
    }

    //void OnRenderObject()
    //{
    //    if( debugMode )
    //    {
    //        //forward
    //        blue.SetPass( 0 );
    //        GL.Begin( GL.LINES );
    //        GL.Vertex( GameObject.Find( "AveragePosition" ).transform.position );
    //        GL.Vertex( GameObject.Find( "AveragePosition" ).transform.position + GameObject.Find( "AveragePosition" ).transform.forward * 5 );
    //        GL.End();

    //        GameObject.Find( "AveragePosition" ).GetComponentInChildren<MeshRenderer>().enabled = true;
    //    }
    //    else
    //    {
    //        GameObject.Find( "AveragePosition" ).GetComponentInChildren<MeshRenderer>().enabled = false;
    //    }
    //}

    void OnDrawGizmosSelected()
    {
        if(path != null)
        { 
            Gizmos.color = Color.red;
        
            foreach( Vector3 v in path.corners)
            {
                Gizmos.DrawSphere(v, 1);
            }
        }
    }

    public float GetHeight(Vector3 input)
    {
        RaycastHit hit;

        Ray downRay;

        //ray pointing down from location
        downRay = new Ray(input, Vector3.down);

        if (Physics.Raycast(downRay, out hit))
        {
            return hit.point.y + .5f;
        }

        //incase we are below the terrain
        else
        {

            downRay = new Ray(input, Vector3.up);

            if (Physics.Raycast(downRay, out hit))
            {
                return hit.point.y + .5f;
            }
        }

        return 0.0f;
    }

    private void CheckKeys()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            debugMode = !debugMode;
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ToggleAStar();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleFlocking();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetPositions();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetStateDefault();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetStateTightKnit();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetStateSpreadApart();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SetStateFlee();
        }
    }

    private void ResetPositions()
    {
        for(int index = 0; index < flockers.Count; index++)
        {
            flockers[index].transform.position = originalPositions[index];
        }
    }

    private void ToggleAStar()
    {
        aStar.GetComponent<AstarCharacter>().enabled = !aStar.GetComponent<AstarCharacter>().enabled;
    }

    private void ToggleFlocking()
    {
        foreach (var f in flockerComps)
        {
            f.enabled = !f.enabled;
        }
    }

    public void SetStateDefault()
    {
        foreach (var flocker in flockerComps)
        {
            flocker.alignmentWeight  = 10;
            flocker.cohesionWeight   = 3;
            flocker.separationWeight = 5;
            flocker.targetWeight     = 3;
            flocker.safeDistance     = 2;
        }
    }

    public void SetStateTightKnit()
    {
        foreach (var flocker in flockerComps)
        {
            flocker.alignmentWeight  = 5;
            flocker.cohesionWeight   = 3;
            flocker.separationWeight = 5;
            flocker.targetWeight     = 3;
            flocker.safeDistance     = 1;
        }
    }

    public void SetStateSpreadApart()
    {
        foreach (var flocker in flockerComps)
        {
            flocker.alignmentWeight  = 2;
            flocker.cohesionWeight   = 1;
            flocker.separationWeight = 10;
            flocker.targetWeight     = 3;
            flocker.safeDistance     = 5;
        }
    }

    public void SetStateFlee()
    {
        foreach (var flocker in flockerComps)
        {
            flocker.alignmentWeight  = 0;
            flocker.cohesionWeight   = 0;
            flocker.separationWeight = 5;
            flocker.targetWeight     = 0;
            flocker.safeDistance     = 5;
        }
    }

}

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.AI;

public class Flocker : VehicleMovement
{
    public Vector3 avgDirection;
    public Vector3 avgPosition;
    private Vector3 alignSteeringForce;

    public float alignmentWeight;
    public float cohesionWeight;
    public float separationWeight;
    public float targetWeight;

    public float safeDistance;
    
    public GameObject nearestNeighbor;

    public GameObject nearestObstacle;
    private Terrain terrain;


    // Use this for initialization

    protected override void Start()
    {

        terrain = GameObject.Find("Terrain").GetComponent<Terrain>();

    }


    public override void CalcSteeringForces()
    {


        //seek target
        ApplyForce( Seek2D( target.transform.position ) * targetWeight );
        //cohere
        ApplyForce( Seek2D( avgPosition ) * cohesionWeight );
        
        //align
        alignSteeringForce = new Vector3(avgDirection.x,0,avgDirection.z) - velocity;
        ApplyForce( alignSteeringForce * alignmentWeight );
        
        //separate
        if( nearestNeighbor != null )
        {
            if( Vector3.Distance( new Vector3(this.transform.position.x,0,this.transform.position.z), new Vector3(nearestNeighbor.transform.position.x,0, nearestNeighbor.transform.position.z)) < safeDistance )
            {
                ApplyForce( Flee2D( nearestNeighbor.transform.position ) * separationWeight );
            }
        }

        

    }

    public override void SetPosition()
    {
        position.y = GetHeight();
    }

    public float GetTerrainHeight()
    {
        return terrain.SampleHeight(transform.position);
    }

    public float GetHeight()
    {
        RaycastHit hit;

        Ray downRay;
        
        //ray pointing down from location
        downRay = new Ray(this.transform.position, Vector3.down);

        if (Physics.Raycast(downRay, out hit))
        {
            return hit.point.y + .5f;
        }

        //incase we are below the terrain
        else
        {

            downRay = new Ray(this.transform.position, Vector3.up);

            if (Physics.Raycast(downRay, out hit))
            {
                return hit.point.y +.5f;
            }
        }

        return GetTerrainHeight() + .5f;
    }

}

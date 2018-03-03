using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class VehicleMovement : MonoBehaviour
{

    // Reference to this vehicle's target
    public GameObject target;

    // Vectors for force-based movement
    public Vector3 position;
    public Vector3 direction;
    public Vector3 acceleration;
    public Vector3 velocity;

    protected Vector3 ultimateForce;
    // UNNECESSARY FOR NOW
    // forces to be applied to this vehicle
    //public Vector3 wind;
    //public Vector3 gravity;

    // floats necessary for STEERING FORCES
    public float mass;
    public float maxSpeed;
    public float radius;

    // Update is called once per frame
    private void Update()
    {
        CalcSteeringForces();
        UpdatePosition();
        SetPosition();
        SetTransform();

    }
    public Vector3 Avoid2D( GameObject obj )
    {
        //obstacles have raidus of 1
        //zombies/humans have a radius of .15
        //1.15 is the sum of the radii
        Vector3 toCenter;
        Vector3 right = this.transform.right;
        Vector3 result = new Vector3( 0, 0, 0 );

        toCenter =  obj.transform.position - this.transform.position;
        toCenter.y = 0.0f;

        if( toCenter.magnitude - 3 > /*safedistance*/ 5 )
        {
            //no potential collsion
            return new Vector3( 0, 0, 0 ); ;
        }
        else
        {
            if( Vector3.Dot( toCenter, this.transform.forward ) < 0 )
            {
                return new Vector3( 0, 0, 0 ); ;
            }
            else
            {
                if( Vector3.Dot( toCenter, right ) > 3 )
                {
                    return new Vector3( 0, 0, 0 ); ;
                }
                else
                {

                    if( Vector3.Dot( toCenter, right ) > 0 ) // is to the right
                    {
                        result = -right * maxSpeed;
                        result -= this.velocity;
                    }
                    else if( Vector3.Dot( toCenter, right ) < 0 ) // is to the left
                    {
                        result = right * maxSpeed;
                        result -= this.velocity;
                    }
                }
            }

        }
        //calc steering force


        return result;


    }
    public Vector3 Avoid( GameObject obj )
    {
        //obstacles have raidus of 1
        //zombies/humans have a radius of .15
        //1.15 is the sum of the radii
        Vector3 toCenter;
        Vector3 right = this.transform.right;
        Vector3 result = new Vector3( 0, 0, 0 );

        toCenter =  obj.transform.position - this.transform.position;

        if( toCenter.magnitude - 1.15 > /*safedistance*/ 1.2 )
        {
            //no potential collsion
            return new Vector3( 0, 0, 0 ); ;
        }
        else
        {
            if( Vector3.Dot( toCenter, this.transform.forward ) < 0 )
            {
                return new Vector3( 0, 0, 0 ); ;
            }
            else
            {
                if( Vector3.Dot( toCenter, right ) > 1.15 )
                {
                    return new Vector3( 0, 0, 0 ); ;
                }
                else
                {

                    if( Vector3.Dot( toCenter, right ) > 0 ) // is to the right
                    {
                        result = -right * maxSpeed;
                        result -= this.velocity;
                    }
                    else if( Vector3.Dot( toCenter, right ) < 0 ) // is to the left
                    {
                        result = right * maxSpeed;
                        result -= this.velocity;
                    }
                }
            }

        }
        //calc steering force


        return result;


    }
    

    /// <summary>
    /// UpdatePosition
    /// Calculate a new position for this vehicle based on incoming forces
    /// </summary>
    void UpdatePosition()
    {
        // Grab the world position from the transform component
        position = gameObject.transform.position;

        // Step 1: Add accel to vel * time
        velocity += acceleration * Time.deltaTime;
        velocity = Vector3.ClampMagnitude( velocity, maxSpeed );
        // Step 2: Add velocity to position
        position += velocity * Time.deltaTime;

        // Step 3: Derive a direction (
        if( velocity != Vector3.zero )
        {
            direction = velocity.normalized;
        }
        else
        {
            transform.rotation = Quaternion.Euler( Vector3.zero );
        }
        // Step 4: Zero out acceleration
        // (start fresh with new forces every frame)
        acceleration = Vector3.zero;
    }

    /// <summary>
    /// Applies the force to the vehicle. 
    /// </summary>
    /// <param name="force">Force.</param>
    protected virtual void ApplyForce( Vector3 force )
    {
        acceleration += force / mass;
    }


    /// <summary>
    /// Apply friction to a vehicle 
    /// </summary>
    void ApplyFriction( float coeff )
    {
        // Step 1: Get the negative velocity
        Vector3 friction = velocity * -1;

        // Step 2: Normalize it (friction is not dependent on vel mag)
        friction.Normalize();

        // Step 3: Multiply by coefficient of friction
        friction = friction * coeff;

        // Step 4: Add to acceleration
        acceleration += friction;
    }


    public Vector3 Seek( Vector3 targetPosition )
    {
        // Step 1:  Calculate desired velocity
        //  This is the vector pointing from myself to my target
        Vector3 desiredVelocity = targetPosition - position;

        // Step 2: Scale desired to the maximum speed
        //   so that I move as quickly as possible
        //desiredVelocity = Vector3.ClampMagnitude (desiredVelocity, maxSpeed);

        desiredVelocity.Normalize();
        desiredVelocity *= maxSpeed;

        // Step 3: Calculate the steering force for seeking
        // Steering = desred - current
        Vector3 steeringForce = desiredVelocity - velocity;

        // Step 4: Return the steering force so this vehicle can 
        //   aply it to its acceleration vector
        return steeringForce;
    }

    public Vector3 Seek2D( Vector3 targetPosition )
    {
        // Step 1:  Calculate desired velocity
        //  This is the vector pointing from myself to my target
        Vector3 xzProjTarget = new Vector3(targetPosition.x,0.0f,targetPosition.z);

        Vector3 desiredVelocity = xzProjTarget - new Vector3( position.x,0.0f, position.z);

        // Step 2: Scale desired to the maximum speed
        //   so that I move as quickly as possible
        //desiredVelocity = Vector3.ClampMagnitude (desiredVelocity, maxSpeed);

        desiredVelocity.Normalize();
        desiredVelocity *= maxSpeed;

        // Step 3: Calculate the steering force for seeking
        // Steering = desred - current
        Vector3 steeringForce = desiredVelocity - velocity;

        // Step 4: Return the steering force so this vehicle can 
        //   aply it to its acceleration vector
        return steeringForce;
    }

    public Vector3 Flee2D( Vector3 target )
    {
        Vector3 xzProjTarget = new Vector3(target.x,0,target.z);
        Vector3 desiredVelocity = xzProjTarget - new Vector3(position.x,0,position.z);
        desiredVelocity.Normalize();
        desiredVelocity *= maxSpeed;
        Vector3 steeringForce = desiredVelocity - velocity;

        return -steeringForce;
    }

    public Vector3 Flee( Vector3 target )
    {
        Vector3 desiredVelocity = target - position;
        desiredVelocity.Normalize();
        desiredVelocity *= maxSpeed;
        Vector3 steeringForce = desiredVelocity - velocity;

        return -steeringForce;
    }

    /// <summary>
    /// Applies any fore as a gravity force, where the result is independent of the mass
    /// </summary>
    /// <param name="force">Force.</param>
    void ApplyGravity( Vector3 force )
    {
        acceleration += force;
    }

    /// <summary>
    /// Set the transform component to reflect the local position vector
    /// </summary>
    void SetTransform()
    {
        // Rotate this vehicle based on its up vector
        // Unity will rotate the game object to face the correct way
        if( direction != Vector3.zero )
        { 
            gameObject.transform.forward = direction;
        }
        gameObject.transform.position = position;

        
    }


    public abstract void CalcSteeringForces();

    public abstract void SetPosition();

    protected virtual void Start()
    {
        
    }
}

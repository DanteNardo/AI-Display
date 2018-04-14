using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PathHeap;

public class MapManager : MonoBehaviour {

    //list of obstacles to get to mapData
    public GameObject[] obstacles;

    //block used to test the mapData
    public GameObject testBlock;

    //block used to test the path and display the goal
    public GameObject pathBlock;

    //block to represent the character
    public GameObject character;

    //size of the map
    public int x = 100;
    public int y = 100;

    //data for the map, true: location can be moved to, false: location can't be oved to
    public bool[,] mapData;

    //value to represent the square root of two
    private float rt2 = Mathf.Sqrt(2);


	// Use this for initialization
	void Start () {
        //initialize the mapData
        mapData = new bool[x,y];

        //get the data for the map
        GetMapData();

        //setup the character
        SetCharacter();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void GetMapData()
    {
        //initialize the data in mapData to true
        for(int i = 0; i < x; i++)
        {
            for(int j = 0; j < y; j++)
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
        for(int i = 0; i < obstacles.Length; i++)
        {
            //get the size of the obstacle
            sizeX = (int)obstacles[i].transform.localScale.x;
            sizeY = (int)obstacles[i].transform.localScale.z;

            //get the location of the obstacle in the grid
            locX = (int)obstacles[i].transform.position.x + x / 2 - sizeX / 2;
            locY = (int)(obstacles[i].transform.position.z - y / 2) * -1 - sizeY / 2;

            //for all values in the grid that are overlaped by the obstacle
            for(int tempX = locX; tempX < locX + sizeX; tempX++)
            {
                for(int tempY = locY; tempY < locY + sizeY; tempY++)
                {
                    //update the mapData at that location to be false
                    mapData[tempX, tempY] = false;
                }
            }
        }

        //display the mapData
        //TestMapData();
    }

    //set up the A* character
    private void SetCharacter()
    {
        //values to store a random grid location
        int randX = 0;
        int randY = 0;

        //get random grid location that is in a valid position when compared to mapData
        do
        {
            randX = Random.Range(0, 10000) % 100;
            randY = Random.Range(0, 10000) % 100;
        }
        while (!mapData[randX, randY]);
        
        //set the characters grid location to the generated location
        character.GetComponent<AstarCharacter>().xLoc = randX;
        character.GetComponent<AstarCharacter>().yLoc = randY;

        //set the characters world position to the position found
        character.transform.position = new Vector3(randX - x / 2 + .5f, 11, -1 * randY + y/2 - .5f);

        //set the correct height for the character
        character.transform.position = new Vector3(character.transform.position.x, 
                                                    character.transform.position.y + character.GetComponent<AstarCharacter>().GetHeight(), 
                                                    character.transform.position.z);

        //get the character a path to follow
        character.GetComponent<AstarCharacter>().AssignPath(CreateRandomPath(randX, randY));
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
                if(mapData[j,i])
                {
                    //display ground that can be moved on
                    Instantiate(testBlock, new Vector3(j - x / 2 + .5f, 10, -i + y / 2 - .5f), Quaternion.identity);
                }
            }
        }
    }

    //create a random path that starts at a given location
    public List<Vector2> CreateRandomPath(int charX, int charY)
    {
        //destination values
        int destX;
        int destY;

        //generate destination values that are not the characters location and are valid to move to
        do
        {
            destX = (int)(Random.Range(0, 1000000) % x);
            destY = (int)(Random.Range(0, 1000000) % y);
        }
        while (destX == charX && destY == charY && mapData[destX, destY]);

        //generate and return the path
        return CreatePath(charX, charY, destX, destY);
    }

    //generate a path with a given location and given destination
    public List<Vector2> CreatePath(int startX, int startY, int destX, int destY)
    {
        //2D array that holds the data of the parent node to calculate the path later
        KeyValuePair<int, int>[,] pathParent = new KeyValuePair<int, int>[x, y];

        //2D array that keeps track of visited nodes
        bool[,] availableNodes = new bool[x,y];

        //distance moved so dar at a node
        float[,] distanceMoved = new float[x, y];


        //copy the data from mapData, get blocked nodes and valid nodes
        for(int i = 0; i < x; i++)
        {
            for(int j = 0; j < y; j++)
            {
                availableNodes[i, j] = mapData[i, j];
            }
        }

        //create a priority queue for node data
        Heap prq = new Heap();

        //double to keep track of the heuristic value
        float heur = 0;

        //value for calulating temp priority values
        float tempDistance;

        //values to hold data in the checking of locations
        int currentNodeX;
        int currentNodeY;

        //values to hold the location of the next node to check
        int nextNodeX;
        int nextNodeY;

        //insert the start location
        prq.Insert(0, new KeyValuePair<int, int>(startX, startY));

        distanceMoved[startX, startY] = 0;

        //place a signal value in the pathParents
        pathParent[startX, startY] = new KeyValuePair<int, int>(-1, -1);

        availableNodes[startX, startY] = false;

        //loop untill we reach the goal
        while (true)
        {
            //get the next location from the priority queue
            KeyValuePair<float, KeyValuePair<int, int>> currentNodeData = prq.Pop();

            //if we have reached the goal
            if(currentNodeData.Value.Key == destX && currentNodeData.Value.Value == destY)
            {
                //break out of the loop
                break;
            }


            //set values
            currentNodeX = currentNodeData.Value.Key;
            currentNodeY = currentNodeData.Value.Value;



            ///         CHECK ADJACENT LOCATIONS



            //get value of the node to the left

            nextNodeX = currentNodeX - 1;
            nextNodeY = currentNodeY;

            //calculate the heuristic
            heur = CalcHeuristic(destX, destY, nextNodeX, nextNodeY);

            //check that the new location is valid
            if (nextNodeX >= 0)
            {
                //check if the tile is available
                if(availableNodes[nextNodeX, nextNodeY])
                {
                    //store the distance moved
                    distanceMoved[nextNodeX, nextNodeY] = distanceMoved[currentNodeX, currentNodeY] + 1;

                    //add the new node to the heap with the weighted heuristic value
                    prq.Insert(distanceMoved[nextNodeX, nextNodeY] + heur, new KeyValuePair<int, int>(nextNodeX, nextNodeY));

                    //add the parent location of this new node
                    pathParent[nextNodeX, nextNodeY] = currentNodeData.Value;

                    //update the available node array
                    availableNodes[nextNodeX, nextNodeY] = false;
                }
                
                else
                {
                    //calculate the temporary distance for the node
                    tempDistance = distanceMoved[currentNodeX, currentNodeY] + 1;

                    //compare this tempDistance to the stored distance
                    if(tempDistance < distanceMoved[nextNodeX, nextNodeY])
                    {
                        //replace the distance moved
                        distanceMoved[nextNodeX, nextNodeY] = tempDistance;

                        //add the new node to the heap with the weighted heuristic value
                        prq.Insert(distanceMoved[nextNodeX, nextNodeY] + heur, new KeyValuePair<int, int>(nextNodeX, nextNodeY));

                        //add the parent location of this new node
                        pathParent[nextNodeX, nextNodeY] = currentNodeData.Value;
                    }
                }
                
            }


            //get value of the node to the right
            nextNodeX = currentNodeX + 1;
            nextNodeY = currentNodeY;

            //calculate the heuristic
            heur = CalcHeuristic(destX, destY, nextNodeX, nextNodeY);


            //check that the new location is valid
            if (nextNodeX < x)
            {
                //check if the tile is available
                if (availableNodes[nextNodeX, nextNodeY])
                {
                    //store the distance moved
                    distanceMoved[nextNodeX, nextNodeY] = distanceMoved[currentNodeX, currentNodeY] + 1;

                    //add the new node to the heap with the weighted heuristic value
                    prq.Insert(distanceMoved[nextNodeX, nextNodeY] + heur, new KeyValuePair<int, int>(nextNodeX, nextNodeY));

                    //add the parent location of this new node
                    pathParent[nextNodeX, nextNodeY] = currentNodeData.Value;

                    //update the available node array
                    availableNodes[nextNodeX, nextNodeY] = false;
                }

                else
                {
                    //calculate the temporary distance for the node
                    tempDistance = distanceMoved[currentNodeX, currentNodeY] + 1;

                    //compare this tempDistance to the stored distance
                    if (tempDistance < distanceMoved[nextNodeX, nextNodeY])
                    {
                        //replace the distance moved
                        distanceMoved[nextNodeX, nextNodeY] = tempDistance;

                        //add the new node to the heap with the weighted heuristic value
                        prq.Insert(distanceMoved[nextNodeX, nextNodeY] + heur, new KeyValuePair<int, int>(nextNodeX, nextNodeY));

                        //add the parent location of this new node
                        pathParent[nextNodeX, nextNodeY] = currentNodeData.Value;
                    }
                }
            }


            //get value of the node from above
            nextNodeX = currentNodeX;
            nextNodeY = currentNodeY - 1;

            //calculate the heuristic
            heur = CalcHeuristic(destX, destY, nextNodeX, nextNodeY);


            //check that the new location is valid
            if (nextNodeY >= 0)
            {
                //check if the tile is available
                if (availableNodes[nextNodeX, nextNodeY])
                {
                    //store the distance moved
                    distanceMoved[nextNodeX, nextNodeY] = distanceMoved[currentNodeX, currentNodeY] + 1;

                    //add the new node to the heap with the weighted heuristic value
                    prq.Insert(distanceMoved[nextNodeX, nextNodeY] + heur, new KeyValuePair<int, int>(nextNodeX, nextNodeY));

                    //add the parent location of this new node
                    pathParent[nextNodeX, nextNodeY] = currentNodeData.Value;

                    //update the available node array
                    availableNodes[nextNodeX, nextNodeY] = false;
                }

                else
                {
                    //calculate the temporary distance for the node
                    tempDistance = distanceMoved[currentNodeX, currentNodeY] + 1;

                    //compare this tempDistance to the stored distance
                    if (tempDistance < distanceMoved[nextNodeX, nextNodeY])
                    {
                        //replace the distance moved
                        distanceMoved[nextNodeX, nextNodeY] = tempDistance;

                        //add the new node to the heap with the weighted heuristic value
                        prq.Insert(distanceMoved[nextNodeX, nextNodeY] + heur, new KeyValuePair<int, int>(nextNodeX, nextNodeY));

                        //add the parent location of this new node
                        pathParent[nextNodeX, nextNodeY] = currentNodeData.Value;
                    }
                }
            }


            //get value of the node from below
            nextNodeX = currentNodeX;
            nextNodeY = currentNodeY + 1;

            //calculate the heuristic
            heur = CalcHeuristic(destX, destY, nextNodeX, nextNodeY);


            //check that the new location is valid
            if (nextNodeY < y)
            {
                //check if the tile is available
                if (availableNodes[nextNodeX, nextNodeY])
                {
                    //store the distance moved
                    distanceMoved[nextNodeX, nextNodeY] = distanceMoved[currentNodeX, currentNodeY] + 1;

                    //add the new node to the heap with the weighted heuristic value
                    prq.Insert(distanceMoved[nextNodeX, nextNodeY] + heur, new KeyValuePair<int, int>(nextNodeX, nextNodeY));

                    //add the parent location of this new node
                    pathParent[nextNodeX, nextNodeY] = currentNodeData.Value;

                    //update the available node array
                    availableNodes[nextNodeX, nextNodeY] = false;
                }

                else
                {
                    //calculate the temporary distance for the node
                    tempDistance = distanceMoved[currentNodeX, currentNodeY] + 1;

                    //compare this tempDistance to the stored distance
                    if (tempDistance < distanceMoved[nextNodeX, nextNodeY])
                    {
                        //replace the distance moved
                        distanceMoved[nextNodeX, nextNodeY] = tempDistance;

                        //add the new node to the heap with the weighted heuristic value
                        prq.Insert(distanceMoved[nextNodeX, nextNodeY] + heur, new KeyValuePair<int, int>(nextNodeX, nextNodeY));

                        //add the parent location of this new node
                        pathParent[nextNodeX, nextNodeY] = currentNodeData.Value;
                    }
                }
            }



            ///         CHECK DIAGONAL LOCATIONS


            

            //get value of the node from top left
            nextNodeX = currentNodeX - 1;
            nextNodeY = currentNodeY - 1;

            //calculate the heuristic
            heur = CalcHeuristic(destX, destY, nextNodeX, nextNodeY);

            //check that the new location is valid
            if (nextNodeX >= 0 && nextNodeY >= 0)
            {
                //check if the tile is available
                if (availableNodes[nextNodeX, nextNodeY])
                {
                    //check if the nodes that are adjacent to the next node and current node are not blocked
                    if(mapData[nextNodeX, currentNodeY] && mapData[currentNodeX, nextNodeY])
                    {
                        //store the distance moved
                        distanceMoved[nextNodeX, nextNodeY] = distanceMoved[currentNodeX, currentNodeY] + rt2;

                        //add the new node to the heap with the weighted heuristic value
                        prq.Insert(distanceMoved[nextNodeX, nextNodeY] + heur, new KeyValuePair<int, int>(nextNodeX, nextNodeY));

                        //add the parent location of this new node
                        pathParent[nextNodeX, nextNodeY] = currentNodeData.Value;

                        //update the available node array
                        availableNodes[nextNodeX, nextNodeY] = false;
                    }
                }

                else
                {
                    //calculate the temporary distance for the node
                    tempDistance = distanceMoved[currentNodeX, currentNodeY] + rt2;

                    //compare this tempDistance to the stored distance
                    if (tempDistance < distanceMoved[nextNodeX, nextNodeY])
                    {
                        //store the priority value to be compared later
                        distanceMoved[nextNodeX, nextNodeY] = tempDistance;

                        //add the new node to the heap with the weighted heuristic value
                        prq.Insert(distanceMoved[nextNodeX, nextNodeY] + heur, new KeyValuePair<int, int>(nextNodeX, nextNodeY));

                        //add the parent location of this new node
                        pathParent[nextNodeX, nextNodeY] = currentNodeData.Value;
                    }
                }
            }


            //get value of the node from top right
            nextNodeX = currentNodeX + 1;
            nextNodeY = currentNodeY - 1;

            //calculate the heuristic
            heur = CalcHeuristic(destX, destY, nextNodeX, nextNodeY);

            //check that the new location is valid
            if (nextNodeX < x && nextNodeY >= 0)
            {
                //check if the tile is available
                if (availableNodes[nextNodeX, nextNodeY])
                {
                    //check if the nodes that are adjacent to the next node and current node are not blocked
                    if (mapData[nextNodeX, currentNodeY] && mapData[currentNodeX, nextNodeY])
                    {
                        //store the distance moved
                        distanceMoved[nextNodeX, nextNodeY] = distanceMoved[currentNodeX, currentNodeY] + rt2;

                        //add the new node to the heap with the weighted heuristic value
                        prq.Insert(distanceMoved[nextNodeX, nextNodeY] + heur, new KeyValuePair<int, int>(nextNodeX, nextNodeY));

                        //add the parent location of this new node
                        pathParent[nextNodeX, nextNodeY] = currentNodeData.Value;

                        //update the available node array
                        availableNodes[nextNodeX, nextNodeY] = false;
                    }
                }

                else
                {
                    //calculate the temporary distance for the node
                    tempDistance = distanceMoved[currentNodeX, currentNodeY] + rt2;

                    //compare this tempDistance to the stored distance
                    if (tempDistance < distanceMoved[nextNodeX, nextNodeY])
                    {
                        //store the priority value to be compared later
                        distanceMoved[nextNodeX, nextNodeY] = tempDistance;

                        //add the new node to the heap with the weighted heuristic value
                        prq.Insert(distanceMoved[nextNodeX, nextNodeY] + heur, new KeyValuePair<int, int>(nextNodeX, nextNodeY));

                        //add the parent location of this new node
                        pathParent[nextNodeX, nextNodeY] = currentNodeData.Value;
                    }
                }
            }


            //get value of the node from bottom left
            nextNodeX = currentNodeX - 1;
            nextNodeY = currentNodeY + 1;

            //calculate the heuristic
            heur = CalcHeuristic(destX, destY, nextNodeX, nextNodeY);

            //check that the new location is valid
            if (nextNodeX >= 0 && nextNodeY < y)
            {
                //check if the tile is available
                if (availableNodes[nextNodeX, nextNodeY])
                {
                    //check if the nodes that are adjacent to the next node and current node are not blocked
                    if (mapData[nextNodeX, currentNodeY] && mapData[currentNodeX, nextNodeY])
                    {
                        //store the distance moved
                        distanceMoved[nextNodeX, nextNodeY] = distanceMoved[currentNodeX, currentNodeY] + rt2;

                        //add the new node to the heap with the weighted heuristic value
                        prq.Insert(distanceMoved[nextNodeX, nextNodeY] + heur, new KeyValuePair<int, int>(nextNodeX, nextNodeY));

                        //add the parent location of this new node
                        pathParent[nextNodeX, nextNodeY] = currentNodeData.Value;

                        //update the available node array
                        availableNodes[nextNodeX, nextNodeY] = false;
                    }
                }

                else
                {
                    //calculate the temporary distance for the node
                    tempDistance = distanceMoved[currentNodeX, currentNodeY] + rt2;

                    //compare this tempDistance to the stored distance
                    if (tempDistance < distanceMoved[nextNodeX, nextNodeY])
                    {
                        //store the priority value to be compared later
                        distanceMoved[nextNodeX, nextNodeY] = tempDistance;

                        //add the new node to the heap with the weighted heuristic value
                        prq.Insert(distanceMoved[nextNodeX, nextNodeY] + heur, new KeyValuePair<int, int>(nextNodeX, nextNodeY));

                        //add the parent location of this new node
                        pathParent[nextNodeX, nextNodeY] = currentNodeData.Value;
                    }
                }
            }


            //get value of the node from bottom right
            nextNodeX = currentNodeX + 1;
            nextNodeY = currentNodeY + 1;

            //calculate the heuristic
            heur = CalcHeuristic(destX, destY, nextNodeX, nextNodeY);

            //check that the new location is valid
            if (nextNodeX < x && nextNodeY < y)
            {
                //check if the tile is available
                if (availableNodes[nextNodeX, nextNodeY])
                {
                    //check if the nodes that are adjacent to the next node and current node are not blocked
                    if (mapData[nextNodeX, currentNodeY] && mapData[currentNodeX, nextNodeY])
                    {
                        //store the distance moved
                        distanceMoved[nextNodeX, nextNodeY] = distanceMoved[currentNodeX, currentNodeY] + rt2;

                        //add the new node to the heap with the weighted heuristic value
                        prq.Insert(distanceMoved[nextNodeX, nextNodeY] + heur, new KeyValuePair<int, int>(nextNodeX, nextNodeY));

                        //add the parent location of this new node
                        pathParent[nextNodeX, nextNodeY] = currentNodeData.Value;

                        //update the available node array
                        availableNodes[nextNodeX, nextNodeY] = false;
                    }
                }

                else
                {
                    //calculate the temporary distance for the node
                    tempDistance = distanceMoved[currentNodeX, currentNodeY] + rt2;

                    //compare this tempDistance to the stored distance
                    if (tempDistance < distanceMoved[nextNodeX, nextNodeY])
                    {
                        //store the priority value to be compared later
                        distanceMoved[nextNodeX, nextNodeY] = tempDistance;

                        //add the new node to the heap with the weighted heuristic value
                        prq.Insert(distanceMoved[nextNodeX, nextNodeY] + heur, new KeyValuePair<int, int>(nextNodeX, nextNodeY));

                        //add the parent location of this new node
                        pathParent[nextNodeX, nextNodeY] = currentNodeData.Value;
                    }
                }
            }


            //check the priority queue
            if (prq.GetSize() == 0)
            {
                return null;
            }
        }

        //hold location values for the path
        List<Vector2> path = new List<Vector2>();

        //the nest location to add to the path
        Vector2 locationToAdd;

        //set the current location to the destination and work backwards
        KeyValuePair<int, int> currentLocation = new KeyValuePair<int, int>(destX, destY);

        //while we have not hit the flag values
        while(currentLocation.Value != -1 && currentLocation.Key != -1)
        {
            //calculate the physical location
            locationToAdd = new Vector2(currentLocation.Key - (x / 2) + .5f, -1 * currentLocation.Value + (y / 2) - .5f);

            //if we are at the goal value
            if(currentLocation.Key == destX && currentLocation.Value == destY)
            {
                //Raycast to store Raycast data
                RaycastHit hit;

                //ray pointing down from the destination
                Ray downRay = new Ray(new Vector3(locationToAdd.x, 15f, locationToAdd.y), -1 * Vector3.up);

                //do the raycast
                Physics.Raycast(downRay, out hit);

                //destroy the old goal
                Destroy(GameObject.FindGameObjectWithTag("AstarGoal"));

                //add the goal at this location
                Instantiate(pathBlock, new Vector3(locationToAdd.x, 15f + 1 - hit.distance, locationToAdd.y), Quaternion.identity);

                
            }

            //displays the path
            //Instantiate(pathBlock, new Vector3(locationToAdd.x, 11f, locationToAdd.y), Quaternion.identity);


            //insert the new location into the beginning of the enemyPath
            path.Insert(0, locationToAdd);

            //get the parent of the current lcation
            currentLocation = pathParent[currentLocation.Key, currentLocation.Value];
        }

        //remove the first value of the path so it will not move toward the location it is currently at
        path.RemoveAt(0);

        //return the calculated path
        return path;
    }

    //calculate the heuristic of a position given a goal
    private float CalcHeuristic(int destX, int destY, int currentX, int currentY)
    {
        //get the value needed to move in the x and y direction
        int heurX = Mathf.Abs(destX - currentX);
        int heurY = Mathf.Abs(destY - currentY);

        //if the x movement is bigger
        if (heurX > heurY)
        {
            //return the difference of the x and y (distance needed to move in the x direction)
            //added with the value of the y (value needed to move in both the x and y aka the diagonal) and mult by root 2
            return heurX - heurY + (heurY * rt2);
        }
        else
        {
            //return the difference of the y and x (distance needed to move in the y direction)
            //added with the value of the x (value needed to move in both the x and y aka the diagonal) and mult by root 2
            return heurY - heurX + (heurX * rt2);
        }
    }
}

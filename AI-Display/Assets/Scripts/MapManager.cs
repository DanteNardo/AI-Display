﻿using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PathHeap;

public class MapManager : MonoBehaviour {

    public GameObject[] obstacles;

    public GameObject testBlock;

    public GameObject pathBlock;

    public GameObject character;

    public int x = 100;
    public int y = 100;

    //true: location can be moved to, false: location can't be oved to
    public bool[,] mapData;


	// Use this for initialization
	void Start () {
        mapData = new bool[x,y];

        GetMapData();

        SetCharacter();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void GetMapData()
    {
        for(int i = 0; i < x; i++)
        {
            for(int j = 0; j < y; j++)
            {
                mapData[i, j] = true;
            }
        }

        obstacles = GameObject.FindGameObjectsWithTag("Obstacle");

        //Debug.Log(obstacles.Length);

        int sizeX;
        int sizeY;
        int locX;
        int locY;

        for(int i = 0; i < obstacles.Length; i++)
        {
            sizeX = (int)obstacles[i].transform.localScale.x;

            sizeY = (int)obstacles[i].transform.localScale.z;

            locX = (int)obstacles[i].transform.position.x + x / 2 - sizeX / 2;

            locY = (int)(obstacles[i].transform.position.z - y / 2) * -1 - sizeY / 2;

            for(int tempX = locX; tempX < locX + sizeX; tempX++)
            {
                
                for(int tempY = locY; tempY < locY + sizeY; tempY++)
                {
                    mapData[tempX, tempY] = false;
                }
            }
        }

        //TestMapData();
    }


    private void SetCharacter()
    {
        int randX = 0;
        int randY = 0;

        do
        {
            randX = Random.Range(0, 10000) % 100;
            randY = Random.Range(0, 10000) % 100;
        }
        while (!mapData[randX, randY]);
        

        character.GetComponent<AstarCharacter>().xLoc = randX;
        character.GetComponent<AstarCharacter>().yLoc = randY;

        character.transform.position = new Vector3(randX - x / 2 + .5f, 11, -1 * randY + y/2 - .5f);

        character.transform.position = new Vector3(character.transform.position.x, 
                                                    character.transform.position.y + character.GetComponent<AstarCharacter>().GetHeight(), 
                                                    character.transform.position.z);

        character.GetComponent<AstarCharacter>().AssignPath(CreateRandomPath(randX, randY));
    }

    //test to display mapdata, red shows available spaces 
    private void TestMapData()
    {
        for (int i = 0; i < y; i++)
        {
            for (int j = 0; j < x; j++)
            {
                if(mapData[j,i])
                {
                    Instantiate(testBlock, new Vector3(j - x / 2 + .5f, 10, -i + y / 2 - .5f), Quaternion.identity);
                }
            }
        }
    }

    public List<Vector2> CreateRandomPath(int charX, int charY)
    {
        //destination values
        int destX;
        int destY;

        //generate destination values that area not the characters location
        do
        {
            destX = (int)(Random.Range(0, 1000000) % x);
            destY = (int)(Random.Range(0, 1000000) % y);
        }
        while (destX == charX && destY == charY && mapData[destX, destY]);

        return CreatePath(charX, charY, destX, destY);
    }

    public List<Vector2> CreatePath(int startX, int startY, int destX, int destY)
    {
        //2D array that holds the data of the parent node to calculate the path later
        KeyValuePair<int, int>[,] pathParent = new KeyValuePair<int, int>[x, y];

        //2D array that keeps track of visited nodes
        bool[,] availableNodes = new bool[x,y];

        float[,] distanceMoved = new float[x, y];


        //copy the data from the  map, get blocked nodes
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

        //ints to hold data in the checking of locations
        int currentNodeX;
        int currentNodeY;

        int nextNodeX;
        int nextNodeY;

        //insert the start location
        prq.Insert(0, new KeyValuePair<int, int>(startX, startY));

        heur = Mathf.Sqrt((destX - startX) * (destX - startX) + (destY - startY) * (destY - startY));

        distanceMoved[startX, startY] = 0;

        //place a signal value in the pathParents
        pathParent[startX, startY] = new KeyValuePair<int, int>(-1, -1);

        availableNodes[startX, startY] = false;

        while (true)
        {
            //get the next location from the priority queue
            KeyValuePair<float, KeyValuePair<int, int>> currentNodeData = prq.Pop();

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
                    tempDistance = distanceMoved[currentNodeX, currentNodeY] + 1;

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
                    tempDistance = distanceMoved[currentNodeX, currentNodeY] + 1;

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
                    tempDistance = distanceMoved[currentNodeX, currentNodeY] + 1;

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
                    tempDistance = distanceMoved[currentNodeX, currentNodeY] + 1;

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
                           distanceMoved[nextNodeX, nextNodeY] = distanceMoved[currentNodeX, currentNodeY] + Mathf.Sqrt(2);

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
                       tempDistance = distanceMoved[currentNodeX, currentNodeY] + Mathf.Sqrt(2);

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
                           distanceMoved[nextNodeX, nextNodeY] = distanceMoved[currentNodeX, currentNodeY] + Mathf.Sqrt(2);

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
                       tempDistance = distanceMoved[currentNodeX, currentNodeY] + Mathf.Sqrt(2);

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
                           distanceMoved[nextNodeX, nextNodeY] = distanceMoved[currentNodeX, currentNodeY] + Mathf.Sqrt(2);

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
                       tempDistance = distanceMoved[currentNodeX, currentNodeY] + Mathf.Sqrt(2);

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
                           distanceMoved[nextNodeX, nextNodeY] = distanceMoved[currentNodeX, currentNodeY] + Mathf.Sqrt(2);

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
                       tempDistance = distanceMoved[currentNodeX, currentNodeY] + Mathf.Sqrt(2);

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

        Vector2 locationToAdd;

        //set the current location to the destination and work backwards
        KeyValuePair<int, int> currentLocation = new KeyValuePair<int, int>(destX, destY);


        while(currentLocation.Value != -1 && currentLocation.Key != -1)
        {
            //calculate the physical location
            locationToAdd = new Vector2(currentLocation.Key - (x / 2) + .5f, -1 * currentLocation.Value + (y / 2) - .5f);

            if(currentLocation.Key == destX && currentLocation.Value == destY)
            {
                //Raycast to store Raycast data
                RaycastHit hit;

                //ray pointing down from the destination
                Ray downRay = new Ray(new Vector3(locationToAdd.x, 15f, locationToAdd.y), -1 * Vector3.up);


                Physics.Raycast(downRay, out hit);

                Destroy(GameObject.FindGameObjectWithTag("AstarGoal"));

                Instantiate(pathBlock, new Vector3(locationToAdd.x, 15f + 1 - hit.distance, locationToAdd.y), Quaternion.identity);

                
            }

            //displays the path
            //Instantiate(pathBlock, new Vector3(locationToAdd.x, 11f, locationToAdd.y), Quaternion.identity);


            //insert the new location into the beginning of the enemyPath
            path.Insert(0, locationToAdd);

            //get the parent of the current lcation
            currentLocation = pathParent[currentLocation.Key, currentLocation.Value];
        }

        path.RemoveAt(0);

        return path;
    }

    private float CalcHeuristic(int destX, int destY, int currentX, int currentY)
    {
        int heurX = Mathf.Abs(destX - currentX);
        int heurY = Mathf.Abs(destY - currentY);


        if (heurX > heurY)
        {
            return heurX - heurY + (heurY * Mathf.Sqrt(2));
        }
        else
        {
            return heurY - heurX + (heurX * Mathf.Sqrt(2));
        }
    }
}

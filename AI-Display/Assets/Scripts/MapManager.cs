using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PathHeap;

public class MapManager : MonoBehaviour {

    private int x = 100;
    private int y = 100;

    private int size = 1;

    //true: location can be moved to, false: location can't be oved to
    public bool[,] mapData;


	// Use this for initialization
	void Start () {
        GetMapData();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void GetMapData()
    {
        //get data from the map
    }

    public bool CreateRandomPath(int charX, int charY)
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
        while (destX == charX && destY == charY);

        KeyValuePair<int, int>[,] pathParent = CreatePathPoints(charX, charY, destX, destY);

        if(pathParent == null)
        {
            return false;
        }

        return true;
    }

    public KeyValuePair<int, int>[,] CreatePathPoints(int startX, int startY, int destX, int destY)
    {
        //2D array that holds the data of the parent node to calculate the path later
        KeyValuePair<int, int>[,] pathParent = new KeyValuePair<int, int>[x, y];

        //2D array that keeps track of visited nodes
        bool[,] availableNodes = new bool[x,y];


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

        //ints to hold data in the checking of locations
        int nextNodeX;
        int nextNodeY;

        //insert the start location
        prq.Insert(0, new KeyValuePair<int, int>(startX, startY));

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


            //get value of the node to the left
            nextNodeX = currentNodeData.Value.Key - 1;
            nextNodeY = currentNodeData.Value.Value;

            //check to the left
            if (nextNodeX >= 0)
            {
                //check if the tile is available
                if(availableNodes[nextNodeX, nextNodeY])
                {
                    //calculate the heuristic to the destination
                    heur = Mathf.Sqrt( (destX - nextNodeX) * (destX - nextNodeX) + (destY - nextNodeY) * (destY - nextNodeY));

                    //add the new node to the heap with the weighted heuristic value
                    prq.Insert(currentNodeData.Key + 1 + heur, new KeyValuePair<int, int>(nextNodeX, nextNodeY));

                    //add the parent location of this new node
                    pathParent[nextNodeX, nextNodeY] = currentNodeData.Value;

                    //update the available node array
                    availableNodes[nextNodeX, nextNodeY] = false;
                }
            }


            //get value of the node to the right
            nextNodeX = currentNodeData.Value.Key + 1;
            nextNodeY = currentNodeData.Value.Value;

            //check to the right
            if (nextNodeX < x)
            {
                //check if the tile is available
                if (availableNodes[nextNodeX, nextNodeY])
                {
                    //calculate the heuristic to the destination
                    heur = Mathf.Sqrt((destX - nextNodeX) * (destX - nextNodeX) + (destY - nextNodeY) * (destY - nextNodeY));

                    //add the new node to the heap with the weighted heuristic value
                    prq.Insert(currentNodeData.Key + 1 + heur, new KeyValuePair<int, int>(nextNodeX, nextNodeY));

                    //add the parent location of this new node
                    pathParent[nextNodeX, nextNodeY] = currentNodeData.Value;

                    //update the available node array
                    availableNodes[nextNodeX, nextNodeY] = false;
                }
            }


            //get value of the node from above
            nextNodeX = currentNodeData.Value.Key;
            nextNodeY = currentNodeData.Value.Value - 1;

            //check above
            if (nextNodeY >= 0)
            {
                //check if the tile is available
                if (availableNodes[nextNodeX, nextNodeY])
                {
                    //calculate the heuristic to the destination
                    heur = Mathf.Sqrt((destX - nextNodeX) * (destX - nextNodeX) + (destY - nextNodeY) * (destY - nextNodeY));

                    //add the new node to the heap with the weighted heuristic value
                    prq.Insert(currentNodeData.Key + 1 + heur, new KeyValuePair<int, int>(nextNodeX, nextNodeY));

                    //add the parent location of this new node
                    pathParent[nextNodeX, nextNodeY] = currentNodeData.Value;

                    //update the available node array
                    availableNodes[nextNodeX, nextNodeY] = false;
                }
            }


            //get value of the node from below
            nextNodeX = currentNodeData.Value.Key;
            nextNodeY = currentNodeData.Value.Value + 1;

            //check below
            if (nextNodeY < y)
            {
                //check if the tile is available
                if (availableNodes[nextNodeX, nextNodeY])
                {
                    //calculate the heuristic to the destination
                    heur = Mathf.Sqrt((destX - nextNodeX) * (destX - nextNodeX) + (destY - nextNodeY) * (destY - nextNodeY));

                    //add the new node to the heap with the weighted heuristic value
                    prq.Insert(currentNodeData.Key + 1 + heur, new KeyValuePair<int, int>(nextNodeX, nextNodeY));

                    //add the parent location of this new node
                    pathParent[nextNodeX, nextNodeY] = currentNodeData.Value;

                    //update the available node array
                    availableNodes[nextNodeX, nextNodeY] = false;
                }
            }


            if(prq.GetSize() == 0)
            {
                return null;
            }
        }

        return pathParent;


    }
}

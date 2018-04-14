using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PathHeap
{
    class Heap
    {
        //key is weighted distance
        //value is the location
        List<KeyValuePair<float, KeyValuePair<int, int>>> keys = new List<KeyValuePair<float, KeyValuePair<int, int>>>();

        //get the parent of the node
        static int GetParent(int i)
        {
            return (i - 1) / 2;
        }

        //get the left child of the node
        static int GetLeft(int i)
        {
            return 2 * i + 1;
        }

        //get the right child of the node
        static int GetRight(int i)
        {
            return 2 * i + 2;
        }

        //sort the node up, replace the parent if the current node is smaller than the parent
        private void HeapifyUp(int i)
        {
            if (i <= 0) return;

            //get the index of the parent in the list
            int j = GetParent(i);

            //check the values at the indexes
            if (keys[i].Key < keys[j].Key)
            {
                KeyValuePair<float, KeyValuePair<int, int>> temp = keys[i];
                keys[i] = keys[j];
                keys[j] = temp;
            }

            //continue to sort in the new node location
            HeapifyUp(j);
        }

        //sort the node down, used when we pop the heap
        private void HeapifyDn(int i)
        {
            int j;

            // If no children
            if (GetLeft(i) > keys.Count - 1) return;

            // If no right child
            if (GetRight(i) > keys.Count - 1)
            {
                //get the left child
                j = GetLeft(i);
            }
            else
            {
                // If both right and left children, get the smaller of the two
                j = (keys[GetLeft(i)].Key < keys[GetRight(i)].Key) ? (GetLeft(i)) : (GetRight(i));
            }

            //if the value to heapify is greater than the child
            if (keys[i].Key > keys[j].Key)
            {
                //swap the two values
                KeyValuePair<float, KeyValuePair<int, int>> temp = keys[i];
                keys[i] = keys[j];
                keys[j] = temp;
            }

            //contiunue to heapify down to fully sort the heap
            HeapifyDn(j);
        }

        //add a new value to the heap and then sort it
        public void Insert(float newKey, KeyValuePair<int, int> newValue)
        {
            keys.Add(new KeyValuePair<float, KeyValuePair<int, int>>(newKey, newValue));
            HeapifyUp(keys.Count - 1);
        }

        //get the top value in the heap
        public KeyValuePair<float, KeyValuePair<int, int>> Pop()
        {
            //get the first value
            KeyValuePair<float, KeyValuePair<int, int>> temp = keys[0];

            //take the last vaue added and replace the first value with it
            keys[0] = keys[keys.Count - 1];
            //Remove the last value, now in the first location
            keys.RemoveAt(keys.Count - 1);

            //sort the heap down to organize the new value in the first location
            HeapifyDn(0);

            //return the first value we stored
            return temp;
        }

        //get the size of the heap
        public int GetSize()
        {
            return keys.Count;
        }
    }
}

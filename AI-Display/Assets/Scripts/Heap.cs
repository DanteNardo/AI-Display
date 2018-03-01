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

        static int GetParent(int i)
        {
            return (i - 1) / 2;
        }

        static int GetLeft(int i)
        {
            return 2 * i + 1;
        }

        static int GetRight(int i)
        {
            return 2 * i + 2;
        }

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

            HeapifyUp(j);
        }

        private void HeapifyDn(int i)
        {
            int j;

            // If no children...
            if (GetLeft(i) > keys.Count - 1) return;

            // If no right child...
            if (GetRight(i) > keys.Count - 1)
            {
                j = GetLeft(i);
            }
            else
            {
                // If both right and left children
                j = (keys[GetLeft(i)].Key < keys[GetRight(i)].Key) ? (GetLeft(i)) : (GetRight(i));
            }

            if (keys[i].Key > keys[j].Key)
            {
                KeyValuePair<float, KeyValuePair<int, int>> temp = keys[i];
                keys[i] = keys[j];
                keys[j] = temp;
            }

            HeapifyDn(j);
        }

        public void Insert(float newKey, KeyValuePair<int, int> newValue)
        {
            keys.Add(new KeyValuePair<float, KeyValuePair<int, int>>(newKey, newValue));
            HeapifyUp(keys.Count - 1);
        }

        public KeyValuePair<float, KeyValuePair<int, int>> Pop()
        {
            KeyValuePair<float, KeyValuePair<int, int>> temp = keys[0];

            keys[0] = keys[keys.Count - 1];
            keys.RemoveAt(keys.Count - 1);

            HeapifyDn(0);

            return temp;
        }

        public int GetSize()
        {
            return keys.Count;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Heap<T> where T : IHeapItem<T>
{

    T[] items;
    int currentCount = 0;

    public Heap(int capacity) {
        this.items = new T[capacity];
    }

    public void Add(T item) {
        if(currentCount >= items.Length) {
            return;
        }
        item.HeapIndex = currentCount;
        items[currentCount] = item;
        SortUp(item);
        currentCount++;
    }

    public T Pop() {
        T popped = items[0];
        currentCount--;
        // When we lower the lenght we don't need to explicitly remove the last item
        items[0] = items[currentCount];
        items[0].HeapIndex = 0;
        SortDown(items[0]);
        return popped;
    }

    public void UpdateItem(T item) {
        SortUp(item);
    }

    public bool Contains(T item) {
        return Equals(items[item.HeapIndex], item);
    }

    public int Count {
        get {
            return currentCount;
        }
    }

    public void Print() {
        string s = "Printing Heap: ";
        for(int i = 0; i < currentCount; i++) {
            T item = items[i];
            if(item != null)
                s += item.Val + ", ";
        }
        Debug.Log(s);
    }

    private void SortDown(T item) {
        while(true) {
            int leftIndex = item.HeapIndex*2 + 1;
            int rightIndex = item.HeapIndex*2 + 2;
            int swapIndex = 0;

            if(leftIndex < currentCount) {
                swapIndex = leftIndex;

                if(rightIndex < currentCount) {
                    if(items[leftIndex].CompareTo(items[rightIndex]) < 0) {
                        swapIndex = rightIndex;
                    }
                }

                if(item.CompareTo(items[swapIndex]) < 0) {
                    Swap(item, items[swapIndex]);
                } else {
                    return;
                }

            } else {
                return;
            }
        }
    }

    private void SortUp(T item) {
        int parentIndex = (item.HeapIndex - 1)/2;

        while(true) {
            T parent = items[parentIndex];
            if(item.CompareTo(parent) > 0) {
                Swap(item, parent);
            } else {
                return;
            }

            parentIndex = (item.HeapIndex - 1) / 2;
        }
    }

    void Swap(T item1, T item2) {
        items[item1.HeapIndex] = item2;
        items[item2.HeapIndex] = item1;
        int item1Index = item1.HeapIndex;
        item1.HeapIndex = item2.HeapIndex;
        item2.HeapIndex = item1Index;
    }

}


public interface IHeapItem<T> : IComparable<T> {
    int HeapIndex {
        get;
        set;
    }

    int Val {
        get;
    }
    
}
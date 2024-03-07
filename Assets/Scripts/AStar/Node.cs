using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Node : IComparable<Node>
{
    public Vector2Int gridPosition;
    public int gCost = 0; //距离起始点的距离
    public int hCost = 0; //距离终点的距离
    public bool isObstacle = false;
    public int movementPenalty;
    public Node parentNode;

    public Node(Vector2Int gridPosition)
    {
        this.gridPosition = gridPosition;

        parentNode = null;
    }


    public int FCost
    {
        get => gCost + hCost;
    }

    public int CompareTo(Node nodeToCompare)
    {
        int compare = FCost.CompareTo(nodeToCompare.FCost);

        if(compare == 0) //0表示比较的值是相同的
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return compare;
    }
}

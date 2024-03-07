using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Node : IComparable<Node>
{
    public Vector2Int gridPosition;
    public int gCost = 0; //������ʼ��ľ���
    public int hCost = 0; //�����յ�ľ���
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

        if(compare == 0) //0��ʾ�Ƚϵ�ֵ����ͬ��
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return compare;
    }
}

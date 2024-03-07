using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNodes
{
    private int width;
    private int height;

    private Node[,] gridNode;

    public GridNodes(int width,int height)
    {
        this.width = width;
        this.height = height;

        gridNode = new Node[width,height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gridNode[x, y] = new Node(new Vector2Int(x, y));
            }
        }
    }

    public Node GetGridNode(int xPosition,int yPosition)
    {
        if(xPosition < width && yPosition < height)
        {
            return gridNode[xPosition, yPosition];
        }
        else
        {
            Debug.LogFormat("Requstted grid node is out of range");
            return null;
        }
    }
}

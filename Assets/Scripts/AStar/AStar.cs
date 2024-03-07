using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour
{
    [Header("Tiles & Tilemaps Reference")]
    [Header("Options")]
    [SerializeField] private bool observeMovementPenaltis = true;

    [Range(0, 20)] [SerializeField] private int partMovemntPenalty = 0;
    [Range(0,20)][SerializeField] private int defaultMovemntPenalty = 0;

    private GridNodes gridNodes;
    private Node startNode;
    private Node targetNode;
    private int gridWidth;
    private int gridHeight;
    private int originX;
    private int originY;

    private List<Node> openNodeList;
    private HashSet<Node> closedNodeList;

    private bool pathFound = false; 

    public bool BuildPath(SceneName sceneName,Vector2Int startGridPosition,Vector2Int endGirPositon,Stack<NPCMovementStep> npcMovementStepStack)
    {
       pathFound = false;

        if (PopulateGridNodesFromGirdPropertiesDictionary(sceneName, startGridPosition, endGirPositon))
        {
            if (FindShortsPath())
            {
                UpdatePathOnNPCMovementStepStack(sceneName, npcMovementStepStack);

                return true;
            }
        }
        return false;
    }

    private void UpdatePathOnNPCMovementStepStack(SceneName sceneName, Stack<NPCMovementStep> npcMovementStepStack)
    {
        Node nextNode = targetNode;

        while(nextNode != null)
        {
            NPCMovementStep npcMovementStep = new NPCMovementStep();

            npcMovementStep.sceneName = sceneName;
            npcMovementStep.gridCoordinate = new Vector2Int(nextNode.gridPosition.x + originX, nextNode.gridPosition.y + originY);

            npcMovementStepStack.Push(npcMovementStep);

            nextNode = nextNode.parentNode;
        }
    }

    private bool FindShortsPath()
    {
        openNodeList.Add(startNode);

        while(openNodeList.Count > 0)
        {
            openNodeList.Sort();

            Node currentNode = openNodeList[0];
            openNodeList.RemoveAt(0);

            closedNodeList.Add(currentNode);

            if(currentNode == targetNode)
            {
                pathFound = true;
                break;
            }

            EvaluateCurrentNodeNeighbours(currentNode);
        }

        if(pathFound)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void EvaluateCurrentNodeNeighbours(Node currentNode)
    {
        Vector2Int currentNdoeGirdPosition = currentNode.gridPosition;

        Node validNeighbourNode;

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                    continue;

                validNeighbourNode = GetValidNodeNieghbour(currentNdoeGirdPosition.x + i, currentNdoeGirdPosition.y + j);

                if(validNeighbourNode != null)
                {
                    int newCostToNeighbour;

                    if (observeMovementPenaltis)
                    {
                        newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, validNeighbourNode) + validNeighbourNode.movementPenalty;
                    }
                    else
                    {
                        newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, validNeighbourNode);
                    }

                    bool isValidNeighboureNodeInOpenList = openNodeList.Contains(validNeighbourNode);

                    if(newCostToNeighbour < validNeighbourNode.gCost || !isValidNeighboureNodeInOpenList)
                    {
                        validNeighbourNode.gCost = newCostToNeighbour;
                        validNeighbourNode.hCost = GetDistance(validNeighbourNode, targetNode);

                        validNeighbourNode.parentNode = currentNode;

                        if (!isValidNeighboureNodeInOpenList)
                        {
                            openNodeList.Add(validNeighbourNode);
                        }
                    }
                }
            }
        }
    }

    private int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridPosition.x -  nodeB.gridPosition.x);
        int dstY = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

        if(dstX > dstY)
        {
            return 14 * dstY + 10 * (dstX - dstY);
        }
        return 14 * dstX + 10 * (dstY - dstX);
    }

    private Node GetValidNodeNieghbour(int neighbourNodeXPosition, int neighbourNodeYPosition)
    {
        if (neighbourNodeXPosition >= gridWidth || neighbourNodeXPosition < 0 || neighbourNodeYPosition >= gridHeight || neighbourNodeYPosition < 0)
        {
            return null;
        }

        Node neighbourNode = gridNodes.GetGridNode(neighbourNodeXPosition, neighbourNodeYPosition);

        if(neighbourNode.isObstacle || closedNodeList.Contains(neighbourNode))
        {
            return null;
        }
        else
        {
            return neighbourNode;
        }
    }

    private bool PopulateGridNodesFromGirdPropertiesDictionary(SceneName sceneName, Vector2Int startGridPosition, Vector2Int endGirPositon)
    {
        SceneSave sceneSave;

        if(GridPropertiesManager.Instance.GameObjectSave.sceneData.TryGetValue(sceneName.ToString(),out sceneSave))
        {
            if(sceneSave.gridPropetyDeatilsDictionary != null)
            {
                if(GridPropertiesManager.Instance.GetGridDimensions(sceneName,out Vector2Int gridDimension,out Vector2Int gridOrigin))
                {
                    gridNodes = new GridNodes(gridDimension.x, gridDimension.y);
                    gridWidth = gridDimension.x;
                    gridHeight = gridDimension.y;
                    originX = gridOrigin.x;
                    originY = gridOrigin.y;

                    openNodeList = new List<Node>();
                    closedNodeList = new HashSet<Node>();

                }
                else
                {
                    return false;
                }

                startNode = gridNodes.GetGridNode(startGridPosition.x - gridOrigin.x, startGridPosition.y - gridOrigin.y);
                targetNode = gridNodes.GetGridNode(endGirPositon.x - gridOrigin.x, endGirPositon.y - gridOrigin.y);

                for (int x = 0; x < gridDimension.x; x++)
                {
                    for (int y = 0; y < gridDimension.y; y++)
                    {
                        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropetyDetails(x + gridOrigin.x, y + gridOrigin.y,
                            sceneSave.gridPropetyDeatilsDictionary);

                        if(gridPropertyDetails != null)
                        {
                            if (gridPropertyDetails.isNPCObstacle)
                            {
                                Node node = gridNodes.GetGridNode(x, y);
                                node.isObstacle = true;
                            }
                            else if (gridPropertyDetails.isPath)
                            {
                                Node node = gridNodes.GetGridNode(x, y);
                                node.movementPenalty = partMovemntPenalty;
                            }
                            else
                            {
                                Node node = gridNodes.GetGridNode(x, y);
                                node.movementPenalty = defaultMovemntPenalty;
                            }
                        }
                    }
                }
            }
            else
            {
                return false;
            }

        }
        else
        {
            return false;
        }

        return true;
    }
}

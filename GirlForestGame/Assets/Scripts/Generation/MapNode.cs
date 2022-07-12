using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNode
{
    MapNode parentNode;
    MapNode leftChild;
    MapNode rightChild;

    int distanceFromStart;
    int directionFromParent;

    NodeTypes nodeType;

    MapGenerator generator;

    public MapNode(MapNode parent, NodeTypes type, int direction = 2 /*0 = left child | 1 = right child*/)
    {
        generator = MapGenerator.Instance;
        nodeType = type;
        directionFromParent = direction;

        if (parent == null && type == NodeTypes.End)
        {
            distanceFromStart = generator.GetEndNodeDistance();
        }
        else if (parent == null)
        {
            distanceFromStart = 0;
        }
        else
        {
            parentNode = parent;

            distanceFromStart = parentNode.GetDistanceFromStart() + 1;

            if (direction == 0)
            {
                parentNode.SetLeftChild(this);
            }
            else if (direction == 1)
            {
                parentNode.SetRightChild(this);
            }
        }
    }

    public void SetLeftChild(MapNode child)
    {
        leftChild = child;
    }

    public void SetRightChild(MapNode child)
    {
        rightChild = child;
    }

    public MapNode GetLeftChild()
    {
        return leftChild;
    }

    public MapNode GetRightChild()
    {
        return rightChild;
    }

    public int GetDistanceFromStart()
    {
        return distanceFromStart;
    }

    public int GetDirectionFromParent()
    {
        return directionFromParent;
    }

    public MapNode GetParentNode()
    {
        return parentNode;
    }
}

public enum NodeTypes
{
    Default,
    Blessing,
    Shop,
    Challenge,
    End
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapNode : MonoBehaviour
{
    MapNode parentNode;
    MapNode leftChild;
    MapNode rightChild;

    int distanceFromStart;
    int directionFromParent;
    bool isSelectable;
    bool selected;

    public bool Selectable { get { return isSelectable; } set { isSelectable = value; } }

    NodeTypes nodeType;

    MapGenerator generator;

    Vector3 defaultSize;

    private void Start()
    {
        defaultSize = transform.localScale;
    }

    private void Update()
    {
        if (isSelectable)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                if (hit.collider.gameObject == this.gameObject)
                {
                    SetSelected(true);
                }
                else
                {
                    SetSelected(false);
                }
            }
        }

        if (selected && InputManager.Instance.SelectNode())
        {
            print("Gi");
            NodeMapManager.Instance.SetActiveNode(this);
        }
    }

    public void SetNode(MapNode parent, NodeTypes type, int direction = 2 /*0 = left child | 1 = right child*/)
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

    public void SetType(NodeTypes type)
    {
        nodeType = type;

        if (type == NodeTypes.Shop)
        {
            GetComponent<MeshRenderer>().material.color = Color.yellow;
        }
        else if( type == NodeTypes.Blessing)
        {
            GetComponent<MeshRenderer>().material.color = Color.blue;
        }

    }

    public NodeTypes GetNodeType()
    {
        return nodeType;
    }
    
    public void SetSelected(bool isSelected)
    {
        if (isSelected)
        {
            transform.localScale = defaultSize * 2;
            selected = true;

            return;
        }

        transform.localScale = defaultSize;
        selected = false;
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

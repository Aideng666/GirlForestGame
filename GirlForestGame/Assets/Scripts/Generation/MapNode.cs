using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MapNode : MonoBehaviour
{
    List<MapNode> parentNodes = new List<MapNode>();
    MapNode leftChild;
    MapNode rightChild;

    public int columnNum { get; private set; }
    bool hasChild = false;
    int distanceFromStart;
    int directionFromParent;
    bool isSelectable;
    bool selected;

    [SerializeField] Sprite[] roomSprites;
    [SerializeField] Image roomImage;

    public bool Selectable { get { return isSelectable; } set { isSelectable = value; } }

    NodeTypes nodeType;

    MapGenerator generator;

    public Vector3 defaultSize { get; private set; }

    private void Start()
    {
        defaultSize = transform.localScale;

        if (transform.localScale == Vector3.zero)
        {
            transform.localScale = new Vector3(2, 2, 2);

            defaultSize = transform.localScale;
        }
    }

    private void Update()
    {
        if (!isSelectable)
        {
            switch (nodeType)
            {
                case NodeTypes.Default:
                    roomImage.sprite = roomSprites[5];
                    break;
                case NodeTypes.Marking:
                    roomImage.sprite = roomSprites[4];
                    break;
                case NodeTypes.Shop:
                    roomImage.sprite = roomSprites[3];
                    break;
            }

            return;
        }

        switch (nodeType)
        {
            case NodeTypes.Default:
                roomImage.sprite = roomSprites[2];
                break;
            case NodeTypes.Marking:
                roomImage.sprite = roomSprites[1];
                break;
            case NodeTypes.Shop:
                roomImage.sprite = roomSprites[0];
                break;
        }
    }

    public void SetNode(MapNode parent, NodeTypes type, int column, int dirFromParent = 2 /*0 = left child | 1 = right child*/)
    {
        nodeType = type;
        //directionFromParent = dirFromParent;
        columnNum = column;
        generator = MapGenerator.Instance;

        if (parent == null && type == NodeTypes.Boss)
        {
            distanceFromStart = generator.GetEndNodeDistance();

            for (int i = 0; i < generator.Nodes.Count; i++)
            {
                if (generator.Nodes[i].GetComponent<MapNode>().GetDistanceFromStart() == generator.GetEndNodeDistance() - 1)
                {
                    parentNodes.Add(generator.Nodes[i].GetComponent<MapNode>());
                }
            }
        }
        else if (parent == null)
        {
            distanceFromStart = 0;
        }
        else
        {
            parentNodes.Add(parent);

            distanceFromStart = parentNodes[parentNodes.Count - 1].GetDistanceFromStart() + 1;

            if (dirFromParent == 0)
            {
                parentNodes[parentNodes.Count - 1].SetLeftChild(this);
            }
            else if (dirFromParent == 1)
            {
                parentNodes[parentNodes.Count - 1].SetRightChild(this);
            }
        }
    }

    public void SetNodeType(NodeTypes type)
    {
        nodeType = type;

        //if (type == NodeTypes.Shop)
        //{
        //    //GetComponent<MeshRenderer>().material.color = Color.yellow;
        //    roomImage.sprite = roomSprites[0];
        //}
        //else if( type == NodeTypes.Marking)
        //{
        //    //GetComponent<MeshRenderer>().material.color = Color.blue;
        //    roomImage.sprite = roomSprites[1];
        //}
    }

    public NodeTypes GetNodeType()
    {
        return nodeType;
    }
    
    //public void SetSelected(bool isSelected)
    //{
    //    if (isSelected)
    //    {
    //        transform.localScale = defaultSize * 2;
    //        selected = true;

    //        return;
    //    }

    //    transform.localScale = defaultSize;
    //    selected = false;
    //}

    public void SetLeftChild(MapNode child)
    {
        leftChild = child;

        hasChild = true;
    }

    public void SetRightChild(MapNode child)
    {
        rightChild = child;

        hasChild = true;
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

    public int GetColumnNum()
    {
        return columnNum;
    }

    public List<MapNode> GetParentNodes()
    {
        return parentNodes;
    }

    public bool GetHasChild()
    {
        return hasChild;
    }
}

public enum NodeTypes
{
    Default,
    Marking,
    Shop,
    Challenge,
    Boss
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] GameObject visualNodePrefab;
    [SerializeField] int endNodeDistance = 4; // How many levels the map has to reach the final node
    //[SerializeField] int initialNodeSpread = 5;
    [SerializeField] int numberOfNodeColumns = 5; // How many different spots can the node spawn in vertically

    [SerializeField] float blessingNodeChance = 0.2f;
    [SerializeField] float shopNodeChance = 0.2f;
    [SerializeField] float secondChildChance = 2; // This means it is a 1/value chance to spawn a second child (4 = 25%, 2 = 50%, 5 = 20%)

    [SerializeField] Material lineRendererMaterial;

    GameObject[,] nodeSlots; // The map which dictates which slots (row and column) have nodes in them
    float[] columnXPositions; // The X position to spawn each node in the world

    bool isFirstColumnEven;

    //List of all of the spawned nodes
    List<GameObject> visualNodes = new List<GameObject>();
    List<GameObject> lineRenderers = new List<GameObject>();

    //Property for the spawned nodes to access getter and setter
    public List<GameObject> Nodes { get { return visualNodes; } }

    public static MapGenerator Instance { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Sets the size of the arrays based on the inputted values in the inspector
        nodeSlots = new GameObject[endNodeDistance, numberOfNodeColumns];
        columnXPositions = new float[numberOfNodeColumns];

        //Sets the x position for each column to have nodes spawn in the correct positions
        for (int i = 1; i < numberOfNodeColumns + 1; i++)
        {
            float columnPercentage = ((float)i / ((float)numberOfNodeColumns + 1.0f));

            columnXPositions[i - 1] = Mathf.Lerp(-30, 30, columnPercentage);
        }

        CreateNodeMap();
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.Instance.RegenMap())
        {
            Regenerate();
        }
    }

    void Regenerate()
    {
        foreach(GameObject visualNode in visualNodes)
        {
            Destroy(visualNode);
        }

        nodeSlots = new GameObject[endNodeDistance, numberOfNodeColumns];

        visualNodes = new List<GameObject>();

        CreateNodeMap();
    }

    void CreateNodeMap()
    {
        int numberOfStartingNodes = Random.Range(2, (numberOfNodeColumns / 2));
        int currentLevel = 0;

        //Spawns in the initial starting nodes on level 0
        for (int i = 0; i < numberOfStartingNodes; i++)
        {
            int selectedColumn = Random.Range(0, numberOfNodeColumns);

            if (i == 0 && selectedColumn % 2 == 0)
            {
                isFirstColumnEven = true;
            }
            else if (i == 0)
            {
                isFirstColumnEven = false;
            }

            if (i == 0)
            {
                visualNodes.Add(Instantiate(visualNodePrefab, (Vector3.down * 100) + (new Vector3(columnXPositions[selectedColumn], 0, 0)), Quaternion.identity));
                visualNodes[visualNodes.Count - 1].GetComponent<MapNode>().SetNode(null, NodeTypes.Default, selectedColumn);

                nodeSlots[currentLevel, selectedColumn] = visualNodes[visualNodes.Count - 1];
            }
            else if ((isFirstColumnEven && selectedColumn % 2 != 0) || (!isFirstColumnEven && selectedColumn % 2 == 0))
            {
                i--;
            }
            else if (nodeSlots[currentLevel, selectedColumn] == null)
            {
                visualNodes.Add(Instantiate(visualNodePrefab, (Vector3.down * 100) + (new Vector3(columnXPositions[selectedColumn], 0, 0)), Quaternion.identity));
                visualNodes[visualNodes.Count - 1].GetComponent<MapNode>().SetNode(null, NodeTypes.Default, selectedColumn);

                nodeSlots[currentLevel, selectedColumn] = visualNodes[visualNodes.Count - 1];
            }
            else
            {
                i--;
            }
        }
        ///////////////////////////////////////////////////
        

        for (int i = 1; i < endNodeDistance; i++)
        {
            for (int j = 0; j < visualNodes.Count; j++)
            {
                MapNode currentNode = visualNodes[j].GetComponent<MapNode>();

                if (currentNode.GetDistanceFromStart() == i - 1)
                {
                    //If the current node is all the way on the left, it can only have a child on its right side
                    if (currentNode.GetColumnNum() == 0)
                    {
                        //Check the node slot where a child would spawn to see if its empty
                        // if it is empty, has a chance to spawn a child in that spot
                        if (nodeSlots[currentNode.GetDistanceFromStart() + 1, currentNode.GetColumnNum() + 1] == null)
                        {
                            //25% chance to spawn a second child, if the current node has no children it is guarunteed to spawn one
                            if (currentNode.GetHasChild() && Random.Range(0, secondChildChance) != 0)
                            {
                                continue;
                            }

                            visualNodes.Add(Instantiate(visualNodePrefab,
                                new Vector3(0, -100, visualNodes[j].transform.position.z) + new Vector3(columnXPositions[currentNode.GetColumnNum() + 1], 0, 10),
                                Quaternion.identity));

                            nodeSlots[currentNode.GetDistanceFromStart() + 1, currentNode.GetColumnNum() + 1] = visualNodes[visualNodes.Count - 1];
                            //visualNodes[visualNodes.Count - 1].GetComponent<MapNode>().SetNode(currentNode, NodeTypes.Default, currentNode.GetColumnNum() + 1, 1);
                        }
                        //else
                        //{
                        //Sets the child nodes variables
                        nodeSlots[currentNode.GetDistanceFromStart() + 1, currentNode.GetColumnNum() + 1]
                            .GetComponent<MapNode>().SetNode(currentNode, NodeTypes.Default, currentNode.GetColumnNum() + 1, 1);
                        //}
                    }
                    //If the current node is all the way on the right, it can only have a child on its left side
                    else if (currentNode.GetColumnNum() == numberOfNodeColumns - 1)
                    {
                        if (nodeSlots[currentNode.GetDistanceFromStart() + 1, currentNode.GetColumnNum() - 1] == null)
                        {
                            if (currentNode.GetHasChild() && Random.Range(0, secondChildChance) != 0)
                            {
                                continue;
                            }

                            visualNodes.Add(Instantiate(visualNodePrefab,
                                new Vector3(0, -100, visualNodes[j].transform.position.z) + new Vector3(columnXPositions[currentNode.GetColumnNum() - 1], 0, 10),
                                Quaternion.identity));

                            nodeSlots[currentNode.GetDistanceFromStart() + 1, currentNode.GetColumnNum() - 1] = visualNodes[visualNodes.Count - 1];
                        }

                        nodeSlots[currentNode.GetDistanceFromStart() + 1, currentNode.GetColumnNum() - 1]
                            .GetComponent<MapNode>().SetNode(currentNode, NodeTypes.Default, currentNode.GetColumnNum() - 1, 0);
                    }
                    else
                    {
                        int columnDirection; // Can only be 1 or -1

                        if (Random.Range(0, 2) == 0)
                        {
                            columnDirection = -1;
                        }
                        else
                        {
                            columnDirection = 1;
                        }

                        //Checks Randomly Selected Side first
                        if (nodeSlots[currentNode.GetDistanceFromStart() + 1, currentNode.GetColumnNum() + columnDirection] == null)
                        {
                            if (currentNode.GetHasChild() && Random.Range(0, secondChildChance) != 0)
                            {
                                continue;
                            }

                            visualNodes.Add(Instantiate(visualNodePrefab,
                                new Vector3(0, -100, visualNodes[j].transform.position.z) + new Vector3(columnXPositions[currentNode.GetColumnNum() + columnDirection], 0, 10),
                                Quaternion.identity));

                            nodeSlots[currentNode.GetDistanceFromStart() + 1, currentNode.GetColumnNum() + columnDirection] = visualNodes[visualNodes.Count - 1];
                        }

                        nodeSlots[currentNode.GetDistanceFromStart() + 1, currentNode.GetColumnNum() + columnDirection]
                            .GetComponent<MapNode>().SetNode(currentNode, NodeTypes.Default, currentNode.GetColumnNum() + columnDirection, 0);

                        //Then checks the other side
                        if (nodeSlots[currentNode.GetDistanceFromStart() + 1, currentNode.GetColumnNum() + -columnDirection] == null)
                        {
                            if (currentNode.GetHasChild() && Random.Range(0, secondChildChance) != 0)
                            {
                                continue;
                            }

                            visualNodes.Add(Instantiate(visualNodePrefab,
                                new Vector3(0, -100, visualNodes[j].transform.position.z) + new Vector3(columnXPositions[currentNode.GetColumnNum() + -columnDirection], 0, 10),
                                Quaternion.identity));

                            nodeSlots[currentNode.GetDistanceFromStart() + 1, currentNode.GetColumnNum() + -columnDirection] = visualNodes[visualNodes.Count - 1];
                        }

                        nodeSlots[currentNode.GetDistanceFromStart() + 1, currentNode.GetColumnNum() + -columnDirection]
                            .GetComponent<MapNode>().SetNode(currentNode, NodeTypes.Default, currentNode.GetColumnNum() + -columnDirection, 1);
                    }
                }
            }
        }

        visualNodes.Add(Instantiate(visualNodePrefab, new Vector3(0, 0, endNodeDistance * 10) + (Vector3.down * 100), Quaternion.identity));
        visualNodes[visualNodes.Count - 1].GetComponent<MapNode>().SetNode(null, NodeTypes.Boss, numberOfNodeColumns / 2);

        nodeSlots[endNodeDistance - 1, numberOfNodeColumns / 2] = visualNodes[visualNodes.Count - 1];

        for (int i = 0; i < visualNodes.Count; i++)
        {
            if (visualNodes[i].GetComponent<MapNode>().GetDistanceFromStart() == endNodeDistance - 1)
            {
                visualNodes[i].GetComponent<MapNode>().SetLeftChild(visualNodes[visualNodes.Count - 1].GetComponent<MapNode>());
                visualNodes[i].GetComponent<MapNode>().SetRightChild(visualNodes[visualNodes.Count - 1].GetComponent<MapNode>());
            }
        }

        FillNodes();
        DrawLines();
    }

    void FillNodes()
    {
        int numberOfFillableNodes = visualNodes.Count - 2;

        int numBlessingNodes = (int)(numberOfFillableNodes * blessingNodeChance);
        int numShopNodes = (int)(numberOfFillableNodes * shopNodeChance);

        for (int i = 0; i < numBlessingNodes; i++)
        {
            int nodeIndex = Random.Range(1, visualNodes.Count - 2);

            if (visualNodes[nodeIndex].GetComponent<MapNode>().GetNodeType() == NodeTypes.Default)
            {
                visualNodes[nodeIndex].GetComponent<MapNode>().SetNodeType(NodeTypes.Marking);
            }
            else
            {
                i--;
            }
        }

        for (int i = 0; i < numShopNodes; i++)
        {
            int nodeIndex = Random.Range(1, visualNodes.Count - 2);

            if (visualNodes[nodeIndex].GetComponent<MapNode>().GetNodeType() == NodeTypes.Default)
            {
                visualNodes[nodeIndex].GetComponent<MapNode>().SetNodeType(NodeTypes.Shop);
            }
            else
            {
                i--;
            }
        }
    }

    void DrawLines()
    {
        //Remove any old lines
        foreach (GameObject lineObject in lineRenderers)
        {
            Destroy(lineObject);
        }

        //Creates the lines in between each node in the map
        for (int i = 0; i < visualNodes.Count; i++)
        {
            GameObject lineObj = new GameObject();
            LineRenderer line = lineObj.AddComponent<LineRenderer>();

            line.positionCount = 3;
            line.startColor = Color.cyan;
            line.endColor = line.startColor;
            line.startWidth = 0.4f;
            line.endWidth = line.startWidth;
            line.material = lineRendererMaterial;
            line.SetPosition(0, visualNodes[i].transform.position);
            line.SetPosition(1, visualNodes[i].transform.position);
            line.SetPosition(2, visualNodes[i].transform.position);

            if (visualNodes[i].GetComponent<MapNode>().GetLeftChild() != null)
            {    
                line.SetPosition(0, visualNodes[visualNodes.IndexOf(visualNodes[i].GetComponent<MapNode>().GetLeftChild().gameObject)].transform.position);
            }
            if (visualNodes[i].GetComponent<MapNode>().GetRightChild() != null)
            {
                line.SetPosition(2, visualNodes[visualNodes.IndexOf(visualNodes[i].GetComponent<MapNode>().GetRightChild().gameObject)].transform.position);
            }

            lineRenderers.Add(lineObj);
        }
    }

    public int GetEndNodeDistance()
    {
        return endNodeDistance;
    }

    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.cyan;

        //for (int i = 0; i < visualNodes.Count; i++)
        //{
        //    if (visualNodes[i].GetComponent<MapNode>().GetLeftChild() != null)
        //    {
        //        Gizmos.DrawLine(visualNodes[i].transform.position, visualNodes[visualNodes.IndexOf(visualNodes[i].GetComponent<MapNode>().GetLeftChild().gameObject)].transform.position);
        //    }
        //    if (visualNodes[i].GetComponent<MapNode>().GetRightChild() != null)
        //    {
        //        Gizmos.DrawLine(visualNodes[i].transform.position, visualNodes[visualNodes.IndexOf(visualNodes[i].GetComponent<MapNode>().GetRightChild().gameObject)].transform.position);
        //    }
        //}
    }
}

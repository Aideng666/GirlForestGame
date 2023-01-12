using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NodeMapManager : MonoBehaviour
{
    [SerializeField] Camera dungeonCam;
    [SerializeField] Camera mapCam;
    [SerializeField] int totalMapCycles = 3;

    Vector3 mapCamStartPos = new Vector3(0, -70, 12);

    static int currentLevel = 0;
    static bool mapActive = true;
    static bool mapUpdated;

    bool nextLevelSet;

    int currentCycle = 1;

    MapNode activeNode;
    MapNode previousNode;

    List<MapNode> selectableNodes = new List<MapNode>();
    MapNode highlightedNode = null;

    public static NodeMapManager Instance { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        mapCam.enabled = true;
        dungeonCam.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (mapActive)
        {
            if (!mapUpdated)
            {
                UpdateMap();
            }

            //Detects Input for selecting a node in the map to enter
            Vector2 directionInput = InputManager.Instance.MoveSelection();

            if (directionInput.magnitude > 0)
            {
                if (directionInput.x > 0)
                {
                    List<MapNode> availableNodes = new List<MapNode>();

                    foreach (GameObject entry in MapGenerator.Instance.Nodes)
                    {
                        MapNode node = entry.GetComponent<MapNode>();

                        if (entry.transform.position.x > highlightedNode.transform.position.x && entry.transform.position.z == highlightedNode.transform.position.z)
                        {
                            availableNodes.Add(node);
                        }
                    }

                    if (availableNodes.Count < 1)
                    {
                        foreach (GameObject entry in MapGenerator.Instance.Nodes)
                        {
                            MapNode node = entry.GetComponent<MapNode>();

                            if (node.columnNum > highlightedNode.columnNum)
                            {
                                availableNodes.Add(node);
                            }
                        }
                    }

                    if (availableNodes.Count > 0)
                    {
                        MapNode closestNode = availableNodes[0];

                        foreach (MapNode entry in availableNodes)
                        {
                            if (Vector3.Distance(highlightedNode.transform.position, entry.transform.position) < Vector3.Distance(highlightedNode.transform.position, closestNode.transform.position))
                            {
                                closestNode = entry;
                            }
                        }

                        SetHighlighted(closestNode);
                    }
                }
                if (directionInput.x < 0)
                {
                    List<MapNode> availableNodes = new List<MapNode>();

                    foreach (GameObject entry in MapGenerator.Instance.Nodes)
                    {
                        MapNode node = entry.GetComponent<MapNode>();

                        if (entry.transform.position.x < highlightedNode.transform.position.x && entry.transform.position.z == highlightedNode.transform.position.z)
                        {
                            availableNodes.Add(node);
                        }
                    }

                    if (availableNodes.Count < 1)
                    {
                        foreach (GameObject entry in MapGenerator.Instance.Nodes)
                        {
                            MapNode node = entry.GetComponent<MapNode>();

                            if (node.columnNum < highlightedNode.columnNum)
                            {
                                availableNodes.Add(node);
                            }
                        }
                    }

                    if (availableNodes.Count > 0)
                    {
                        MapNode closestNode = availableNodes[0];

                        foreach (MapNode entry in availableNodes)
                        {
                            if (Vector3.Distance(highlightedNode.transform.position, entry.transform.position) < Vector3.Distance(highlightedNode.transform.position, closestNode.transform.position))
                            {
                                closestNode = entry;
                            }
                        }

                        SetHighlighted(closestNode);
                    }
                }
                if (directionInput.y > 0)
                {
                    if (highlightedNode.GetLeftChild() != null && highlightedNode.GetRightChild() != null)
                    {
                        int childToSelect = Random.Range(0, 2);

                        if (childToSelect == 0)
                        {
                            SetHighlighted(highlightedNode.GetLeftChild());
                        }
                        else if (childToSelect == 1)
                        {
                            SetHighlighted(highlightedNode.GetRightChild());
                        }
                    }
                    else if (highlightedNode.GetLeftChild() != null)
                    {
                        SetHighlighted(highlightedNode.GetLeftChild());
                    }
                    else if (highlightedNode.GetRightChild() != null)
                    {
                        SetHighlighted(highlightedNode.GetRightChild());
                    }
                }
                if (directionInput.y < 0)
                {
                    if (highlightedNode.GetParentNodes().Count > 0)
                    {
                        MapNode closestParent = highlightedNode.GetParentNodes()[0];

                        foreach (MapNode entry in highlightedNode.GetParentNodes())
                        {
                            if (Vector3.Distance(highlightedNode.transform.position, entry.transform.position) < Vector3.Distance(highlightedNode.transform.position, closestParent.transform.position))
                            {
                                closestParent = entry;
                            }
                        }

                        SetHighlighted(closestParent);
                    }
                }
            }

            if (PlayerController.Instance.MouseControlActive())
            {
                Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100))
                {

                    if (hit.collider.gameObject.TryGetComponent(out MapNode node))
                    {
                        SetHighlighted(node);
                    }
                }
            }

            //Detects Node Confirmation
            if (highlightedNode.Selectable && InputManager.Instance.SelectNode())
            {
                SetActiveNode(highlightedNode);
            }
        }
    }

    void ToggleNodeMap()
    {
        mapCam.enabled = !mapCam.enabled;
        dungeonCam.enabled = !dungeonCam.enabled;
    }

    void UpdateMap()
    {
        selectableNodes.Clear();

        //Loops through the existing nodes and sets the available nodes to be selectable
        //based on if the previous node was its parent and if it is on the correct level
        for (int i = 0; i < MapGenerator.Instance.Nodes.Count; i++)
        {
            if (MapGenerator.Instance.Nodes[i].GetComponent<MapNode>().GetDistanceFromStart() == currentLevel)
            {
                for (int j = 0; j < MapGenerator.Instance.Nodes[i].GetComponent<MapNode>().GetParentNodes().Count; j++)
                {
                    if (MapGenerator.Instance.Nodes[i].GetComponent<MapNode>().GetParentNodes()[j] == previousNode)
                    {
                        MapGenerator.Instance.Nodes[i].GetComponent<MapNode>().Selectable = true;
                        selectableNodes.Add(MapGenerator.Instance.Nodes[i].GetComponent<MapNode>());
                    }
                }

                //If the current node has no parents (meaning its in the first row), it will always be selectable on the first level
                if (MapGenerator.Instance.Nodes[i].GetComponent<MapNode>().GetParentNodes().Count == 0)
                {
                    MapGenerator.Instance.Nodes[i].GetComponent<MapNode>().Selectable = true;
                    selectableNodes.Add(MapGenerator.Instance.Nodes[i].GetComponent<MapNode>());
                }

                continue;
            }

            MapGenerator.Instance.Nodes[i].GetComponent<MapNode>().Selectable = false;
        }

        mapUpdated = true;
        SetHighlighted(selectableNodes[0]);
    }

    void SetHighlighted(MapNode node)
    {
        highlightedNode = node;
        node.transform.localScale = node.defaultSize * 2;

        for (int i = 0; i < MapGenerator.Instance.Nodes.Count; i++)
        {
            MapNode currentNode = MapGenerator.Instance.Nodes[i].GetComponent<MapNode>();

            if (node != currentNode)
            {
                currentNode.transform.localScale = currentNode.defaultSize;
            }
        }
    }

    public void SetNextLevel()
    {
        if (!nextLevelSet)
        {
            Minimap.Instance.ResetMap();

            if (currentLevel == MapGenerator.Instance.GetEndNodeDistance())
            {
                MapGenerator.Instance.Regenerate();

                currentLevel = 0;
                currentCycle++;

                SetPreviousNode(null);
                activeNode = null;

                mapCam.transform.position = mapCamStartPos;
            }
            else
            {
                currentLevel++;

                SetPreviousNode(activeNode);
                activeNode = null;
            }

            mapUpdated = false;

            ToggleNodeMap();

            mapActive = true;

            InputManager.Instance.SwapActionMap("NodeMap");

            nextLevelSet = true;
        }
    }

    public void SetActiveNode(MapNode node)
    {
        activeNode = node;

        nextLevelSet = false;

        DungeonGenerator.Instance.InitDungeon(node.GetNodeType());

        InputManager.Instance.SwapActionMap("Player");
        mapActive = false;

        ToggleNodeMap();
    }

    public void SetPreviousNode(MapNode node)
    {
        previousNode = node;
    }

    public int GetCurrentMapCycle()
    {
        return currentCycle;
    }
}

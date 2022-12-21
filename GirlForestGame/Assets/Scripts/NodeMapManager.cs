using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeMapManager : MonoBehaviour
{
    [SerializeField] Camera dungeonCam;
    [SerializeField] Camera mapCam;

    static int currentLevel = 0;

    static bool mapActive = true;
    static bool mapUpdated;

    MapNode activeNode;
    MapNode previousNode;

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
        if (mapActive && !mapUpdated)
        {
            UpdateMap();
        }
    }

    void ToggleNodeMap()
    {
        mapCam.enabled = !mapCam.enabled;
        dungeonCam.enabled = !dungeonCam.enabled;
    }

    void UpdateMap()
    {
        //Loops through the existing nodes and sets the available nodes to be selectable
        //based on if the previous node was its parent and if it is on the correct level
        for (int i = 0; i < MapGenerator.Instance.Nodes.Count; i++)
        {
            if (MapGenerator.Instance.Nodes[i].GetComponent<MapNode>().GetDistanceFromStart() == currentLevel)
            {

                for (int j = 0; j < MapGenerator.Instance.Nodes[i].GetComponent<MapNode>().GetParentNode().Count; j++)
                {
                    if (MapGenerator.Instance.Nodes[i].GetComponent<MapNode>().GetParentNode()[j] == previousNode)
                    {
                        MapGenerator.Instance.Nodes[i].GetComponent<MapNode>().Selectable = true;
                    }
                }

                //If the current node has no parents (meaning its in the first row), it will always be selectable on the first level
                if (MapGenerator.Instance.Nodes[i].GetComponent<MapNode>().GetParentNode().Count == 0)
                {
                    MapGenerator.Instance.Nodes[i].GetComponent<MapNode>().Selectable = true;
                }

                continue;
            }

            MapGenerator.Instance.Nodes[i].GetComponent<MapNode>().Selectable = false;
        }

        mapUpdated = true;
    }

    public void SetNextLevel()
    {
        Minimap.Instance.ResetMap();

        currentLevel++;

        SetPreviousNode(activeNode);
        activeNode = null;

        mapUpdated = false;

        ToggleNodeMap();

        mapActive = true;

        InputManager.Instance.SwapActionMap("NodeMap");
    }

    public void SetActiveNode(MapNode node)
    {
        activeNode = node;

        DungeonGenerator.Instance.InitDungeon(node.GetNodeType());

        InputManager.Instance.SwapActionMap("Player");
        mapActive = false;

        ToggleNodeMap();
    }

    public void SetPreviousNode(MapNode node)
    {
        previousNode = node;
    }
}

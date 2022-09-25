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

        //if (InputManager.Instance.BowAttack())
        //{
        //    SetNextLevel();

        //    print($"current level: {currentLevel}");
        //}

        //if (InputManager.Instance.SwapMapButton())
        //{
        //    if (mapActive)
        //    {
        //        InputManager.Instance.SwapActionMap("Player");
        //        mapActive = false;
        //    }
        //    else
        //    {
        //        InputManager.Instance.SwapActionMap("NodeMap");
        //        mapActive = true;
        //    }

        //    SwapCameras();
        //}
    }

    void ToggleNodeMap()
    {
        mapCam.enabled = !mapCam.enabled;
        dungeonCam.enabled = !dungeonCam.enabled;
    }

    void UpdateMap()
    {
        for (int i = 0; i < MapGenerator.Instance.Nodes.Count; i++)
        {
            if (MapGenerator.Instance.Nodes[i].GetComponent<MapNode>().GetDistanceFromStart() == currentLevel)
            {
                MapGenerator.Instance.Nodes[i].GetComponent<MapNode>().Selectable = true;

                continue;
            }

            MapGenerator.Instance.Nodes[i].GetComponent<MapNode>().Selectable = false;
        }

        mapUpdated = true;
    }

    public void SetNextLevel()
    {
        currentLevel++;

        mapUpdated = false;

        ToggleNodeMap();

        mapActive = true;

        InputManager.Instance.SwapActionMap("NodeMap");
    }

    public void SetActiveNode(MapNode node)
    {
        activeNode = node;

        DungeonGenerator.Instance.InitDungeon();

        InputManager.Instance.SwapActionMap("Player");
        mapActive = false;

        ToggleNodeMap();
    }
}

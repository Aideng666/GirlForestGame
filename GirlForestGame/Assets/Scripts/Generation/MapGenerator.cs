using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] GameObject visualNodePrefab;
    [SerializeField] int endNodeDistance = 4;
    [SerializeField] int initialNodeSpread = 5;

    List<MapNode> nodes = new List<MapNode>();
    List<GameObject> visualNodes = new List<GameObject>();

    public static MapGenerator Instance { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        CreateNodeMap();

        SpawnVisualNodes();
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.Instance.Dash())
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

        visualNodes = new List<GameObject>();
        nodes = new List<MapNode>();

        CreateNodeMap();

        SpawnVisualNodes();
    }

    void CreateNodeMap()
    {
        nodes.Add(new MapNode(null, NodeTypes.Default));

        nodes.Add(new MapNode(nodes[0], NodeTypes.Default, 0));
        nodes.Add(new MapNode(nodes[0], NodeTypes.Default, 1));

        //Loops through the nodes in order to create children properly and not too many, stops looping before the end node gets created
        for (int i = 1; i < endNodeDistance - 1; i++)
        {
            for (int j = 0; j < nodes.Count; j++)
            {
                if (nodes[j].GetDistanceFromStart() == i)
                {
                    int childrenCount = Random.Range(1, 4);

                    switch (childrenCount)
                    {
                        case 1:

                            nodes.Add(new MapNode(nodes[j], NodeTypes.Default, 0));

                            break;

                        case 2:

                            nodes.Add(new MapNode(nodes[j], NodeTypes.Default, 1));

                            break;

                        case 3:

                            nodes.Add(new MapNode(nodes[j], NodeTypes.Default, 0));
                            nodes.Add(new MapNode(nodes[j], NodeTypes.Default, 1));

                            break;
                    }
                }
            }
        }

        nodes.Add(new MapNode(null, NodeTypes.End));

        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].GetDistanceFromStart() == endNodeDistance - 1)
            {
                nodes[i].SetLeftChild(nodes[nodes.Count - 1]);
                nodes[i].SetRightChild(nodes[nodes.Count - 1]);
            }
        }

        print(nodes.Count);
    }

    void SpawnVisualNodes()
    {
        visualNodes.Add(Instantiate(visualNodePrefab, Vector3.zero, Quaternion.identity));

        for (int j = 1; j < nodes.Count - 1; j++)
        {
            if (nodes[j].GetDirectionFromParent() == 0)
            {
                visualNodes.Add(Instantiate(visualNodePrefab, visualNodes[nodes.IndexOf(nodes[j].GetParentNode())].transform.position + new Vector3(-initialNodeSpread / nodes[j].GetDistanceFromStart(), 0, 10), Quaternion.identity));
            }
            else if (nodes[j].GetDirectionFromParent() == 1)
            {
                visualNodes.Add(Instantiate(visualNodePrefab, visualNodes[nodes.IndexOf(nodes[j].GetParentNode())].transform.position + new Vector3(initialNodeSpread / nodes[j].GetDistanceFromStart(), 0, 10), Quaternion.identity));
            }
        }

        visualNodes.Add(Instantiate(visualNodePrefab, new Vector3(0, 0, endNodeDistance * 10), Quaternion.identity));
    }

    public int GetEndNodeDistance()
    {
        return endNodeDistance;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].GetLeftChild() != null)
            {
                Gizmos.DrawLine(visualNodes[i].transform.position, visualNodes[nodes.IndexOf(nodes[i].GetLeftChild())].transform.position);
            }
            if (nodes[i].GetRightChild() != null)
            {
                Gizmos.DrawLine(visualNodes[i].transform.position, visualNodes[nodes.IndexOf(nodes[i].GetRightChild())].transform.position);
            }
        }
    }
}

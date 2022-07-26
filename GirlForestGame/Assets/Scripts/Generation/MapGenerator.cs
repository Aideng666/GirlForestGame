using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] GameObject visualNodePrefab;
    [SerializeField] int endNodeDistance = 4;
    [SerializeField] int initialNodeSpread = 5;

    [SerializeField] float blessingNodeChance = 0.2f;
    [SerializeField] float shopNodeChance = 0.2f;

    List<GameObject> visualNodes = new List<GameObject>();

    public List<GameObject> Nodes { get { return visualNodes; } }

    public static MapGenerator Instance { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        CreateNodeMap();
    }

    // Update is called once per frame
    void Update()
    {
        //if (InputManager.Instance.Dash())
        //{
        //    Regenerate();
        //}
    }

    void Regenerate()
    {
        foreach(GameObject visualNode in visualNodes)
        {
            Destroy(visualNode);
        }

        visualNodes = new List<GameObject>();

        CreateNodeMap();
    }

    void CreateNodeMap()
    {
        visualNodes.Add(Instantiate(visualNodePrefab, Vector3.zero + (Vector3.down * 100), Quaternion.identity));
        visualNodes[0].GetComponent<MapNode>().SetNode(null, NodeTypes.Default);


        visualNodes.Add(Instantiate(visualNodePrefab, Vector3.zero, Quaternion.identity));
        visualNodes[1].GetComponent<MapNode>().SetNode(visualNodes[0].GetComponent<MapNode>(), NodeTypes.Default, 0);
        visualNodes.Add(Instantiate(visualNodePrefab, Vector3.zero, Quaternion.identity));
        visualNodes[2].GetComponent<MapNode>().SetNode(visualNodes[0].GetComponent<MapNode>(), NodeTypes.Default, 1);

        visualNodes[1].transform.position = visualNodes[0].transform.position + new Vector3(-initialNodeSpread / visualNodes[1].GetComponent<MapNode>().GetDistanceFromStart(), 0, 10);
        visualNodes[2].transform.position = visualNodes[0].transform.position + new Vector3(initialNodeSpread / visualNodes[2].GetComponent<MapNode>().GetDistanceFromStart(), 0, 10);

        //Loops through the nodes in order to create children properly and not too many, stops looping before the end node gets created
        for (int i = 1; i < endNodeDistance - 1; i++)
        {
            for (int j = 0; j < visualNodes.Count; j++)
            {
                if (visualNodes[j].GetComponent<MapNode>().GetDistanceFromStart() == i)
                {
                    int childrenCount = Random.Range(1, 4);

                    switch (childrenCount)
                    {
                        case 1:

                            visualNodes.Add(Instantiate(visualNodePrefab, Vector3.zero, Quaternion.identity));
                            visualNodes[visualNodes.Count - 1].GetComponent<MapNode>().SetNode(visualNodes[j].GetComponent<MapNode>(), NodeTypes.Default, 0);
                            visualNodes[visualNodes.Count - 1].transform.position = visualNodes[j].transform.position + new Vector3(-initialNodeSpread / visualNodes[visualNodes.Count - 1].GetComponent<MapNode>().GetDistanceFromStart(), 0, 10);

                            break;

                        case 2:

                            visualNodes.Add(Instantiate(visualNodePrefab, Vector3.zero, Quaternion.identity));
                            visualNodes[visualNodes.Count - 1].GetComponent<MapNode>().SetNode(visualNodes[j].GetComponent<MapNode>(), NodeTypes.Default, 1);
                            visualNodes[visualNodes.Count - 1].transform.position = visualNodes[j].transform.position + new Vector3(initialNodeSpread / visualNodes[visualNodes.Count - 1].GetComponent<MapNode>().GetDistanceFromStart(), 0, 10);

                            break;

                        case 3:

                            visualNodes.Add(Instantiate(visualNodePrefab, Vector3.zero, Quaternion.identity));
                            visualNodes[visualNodes.Count - 1].GetComponent<MapNode>().SetNode(visualNodes[j].GetComponent<MapNode>(), NodeTypes.Default, 0);
                            visualNodes[visualNodes.Count - 1].transform.position = visualNodes[j].transform.position + new Vector3(-initialNodeSpread / visualNodes[visualNodes.Count - 1].GetComponent<MapNode>().GetDistanceFromStart(), 0, 10);

                            visualNodes.Add(Instantiate(visualNodePrefab, Vector3.zero, Quaternion.identity));
                            visualNodes[visualNodes.Count - 1].GetComponent<MapNode>().SetNode(visualNodes[j].GetComponent<MapNode>(), NodeTypes.Default, 1);
                            visualNodes[visualNodes.Count - 1].transform.position = visualNodes[j].transform.position + new Vector3(initialNodeSpread / visualNodes[visualNodes.Count - 1].GetComponent<MapNode>().GetDistanceFromStart(), 0, 10);

                            break;
                    }
                }
            }
        }

        visualNodes.Add(Instantiate(visualNodePrefab, new Vector3(0, 0, endNodeDistance * 10) + (Vector3.down * 100), Quaternion.identity));
        visualNodes[visualNodes.Count - 1].GetComponent<MapNode>().SetNode(null, NodeTypes.End);

        for (int i = 0; i < visualNodes.Count; i++)
        {
            if (visualNodes[i].GetComponent<MapNode>().GetDistanceFromStart() == endNodeDistance - 1)
            {
                visualNodes[i].GetComponent<MapNode>().SetLeftChild(visualNodes[visualNodes.Count - 1].GetComponent<MapNode>());
                visualNodes[i].GetComponent<MapNode>().SetRightChild(visualNodes[visualNodes.Count - 1].GetComponent<MapNode>());
            }
        }

        FillNodes();
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
                visualNodes[nodeIndex].GetComponent<MapNode>().SetType(NodeTypes.Blessing);
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
                visualNodes[nodeIndex].GetComponent<MapNode>().SetType(NodeTypes.Shop);
            }
            else
            {
                i--;
            }
        }
    }

    public int GetEndNodeDistance()
    {
        return endNodeDistance;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        for (int i = 0; i < visualNodes.Count; i++)
        {
            if (visualNodes[i].GetComponent<MapNode>().GetLeftChild() != null)
            {
                Gizmos.DrawLine(visualNodes[i].transform.position, visualNodes[visualNodes.IndexOf(visualNodes[i].GetComponent<MapNode>().GetLeftChild().gameObject)].transform.position);
            }
            if (visualNodes[i].GetComponent<MapNode>().GetRightChild() != null)
            {
                Gizmos.DrawLine(visualNodes[i].transform.position, visualNodes[visualNodes.IndexOf(visualNodes[i].GetComponent<MapNode>().GetRightChild().gameObject)].transform.position);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
    [SerializeField] GameObject mapIconPrefab;
    [SerializeField] GameObject pathIconPrefab;
    [SerializeField] Vector2 iconSpacing = new Vector2(40, 40);
    [SerializeField] Vector2 pathwaySpacing = new Vector2(20, 20);
    [SerializeField] Color currentRoomColour;
    [SerializeField] Color nonCurrentRoomColour;

    List<GameObject> mapIcons = new List<GameObject>();
    List<GameObject> mapPathways = new List<GameObject>();

    Dictionary<Room, bool> visitedRooms;

    public static Minimap Instance { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetMap()
    {
        visitedRooms = new Dictionary<Room, bool>();

        foreach (Room room in DungeonGenerator.Instance.GetRooms())
        {
            visitedRooms.Add(room, false);
        }

        foreach (GameObject icon in mapIcons)
        {
            Destroy(icon);
        }

        foreach (GameObject icon in mapPathways)
        {
            Destroy(icon);
        }

        mapIcons.Clear();
        mapPathways.Clear();
    }

    public void VisitRoom(Room visitedRoom, Directions directionOfPreviousRoom)
    {
        bool roomPreviouslyVisited = false;

        //Sets the correct room to be visited in the dictionary
        if (visitedRooms.ContainsKey(visitedRoom))
        {
            if (visitedRooms[visitedRoom])
            {
                roomPreviouslyVisited = true;
            }
            else
            {
                roomPreviouslyVisited = false;
                visitedRooms[visitedRoom] = true;
            }
        }

        //based on the direction of the previous room, the map icons will be updated and moved to the correct position on the minimap
        switch (directionOfPreviousRoom)
        {
            case Directions.North:

                foreach (GameObject icon in mapIcons)
                {
                    icon.GetComponent<RectTransform>().position += (Vector3.up * iconSpacing.y) + (Vector3.left * iconSpacing.x);
                }

                break;

            case Directions.South:

                foreach (GameObject icon in mapIcons)
                {
                    icon.GetComponent<RectTransform>().position += (Vector3.down * iconSpacing.y) + (Vector3.right * iconSpacing.x);
                }

                break;

            case Directions.East:

                foreach (GameObject icon in mapIcons)
                {
                    icon.GetComponent<RectTransform>().position += (Vector3.up * iconSpacing.y) + (Vector3.right * iconSpacing.x);
                }

                break;

            case Directions.West:

                foreach (GameObject icon in mapIcons)
                {
                    icon.GetComponent<RectTransform>().position += (Vector3.down * iconSpacing.y) + (Vector3.left * iconSpacing.x);
                }

                break;
        }

        //Creates a new icon for a room and a pathway when visiting a new room for the first time
        if (!roomPreviouslyVisited)
        {
            mapIcons.Add(Instantiate(mapIconPrefab, this.transform));

            switch (directionOfPreviousRoom)
            {
                case Directions.North:

                    mapIcons.Add(Instantiate(pathIconPrefab, this.transform));

                    mapIcons[mapIcons.Count - 1].GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 45);
                    mapIcons[mapIcons.Count - 1].GetComponent<RectTransform>().position += (Vector3.up * pathwaySpacing.y) + (Vector3.left * pathwaySpacing.x);

                    break;

                case Directions.South:

                    mapIcons.Add(Instantiate(pathIconPrefab, this.transform));

                    mapIcons[mapIcons.Count - 1].GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 45);
                    mapIcons[mapIcons.Count - 1].GetComponent<RectTransform>().position += (Vector3.down * pathwaySpacing.y) + (Vector3.right * pathwaySpacing.x);

                    break;

                case Directions.East:

                    mapIcons.Add(Instantiate(pathIconPrefab, this.transform));

                    mapIcons[mapIcons.Count - 1].GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, -45);
                    mapIcons[mapIcons.Count - 1].GetComponent<RectTransform>().position += (Vector3.up * pathwaySpacing.y) + (Vector3.right * pathwaySpacing.x);

                    break;

                case Directions.West:

                    mapIcons.Add(Instantiate(pathIconPrefab, this.transform));

                    mapIcons[mapIcons.Count - 1].GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, -45);
                    mapIcons[mapIcons.Count - 1].GetComponent<RectTransform>().position += (Vector3.down * pathwaySpacing.y) + (Vector3.left * pathwaySpacing.x);

                    break;
            }
        }

        //Updates the color of the icons based on if they are in the room and what type of room it is
        foreach (GameObject icon in mapIcons)
        {
            if (icon.GetComponent<RectTransform>().localPosition == Vector3.zero)
            {
                icon.GetComponent<Image>().color = currentRoomColour;
            }
            else
            {
                icon.GetComponent<Image>().color = nonCurrentRoomColour;
            }
        }
    }
}

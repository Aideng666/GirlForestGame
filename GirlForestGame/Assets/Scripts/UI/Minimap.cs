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
    [SerializeField] Color defaultRoomColor;
    [SerializeField] Color totemRoomColor;

    List<GameObject> mapIcons = new List<GameObject>();
    List<GameObject> mapPathways = new List<GameObject>();
    List<Sprite> mapIconSprites = new List<Sprite>();

    Dictionary<Room, bool> visitedRooms;

    [SerializeField] Sprite[] roomIcons;

    bool mapUpdated = false;

    //public static Minimap Instance { get; set; }

    //private void Awake()
    //{
    //    Instance = this;
    //}

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

        foreach (Sprite icon in mapIconSprites)
        {
            Destroy(icon);
        }

        foreach (GameObject icon in mapPathways)
        {
            Destroy(icon);
        }

        mapIcons.Clear();
        mapPathways.Clear();
        mapIconSprites.Clear();
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
                foreach (GameObject path in mapPathways)
                {
                    path.GetComponent<RectTransform>().position += (Vector3.up * iconSpacing.y) + (Vector3.left * iconSpacing.x);
                }

                break;

            case Directions.South:

                foreach (GameObject icon in mapIcons)
                {
                    icon.GetComponent<RectTransform>().position += (Vector3.down * iconSpacing.y) + (Vector3.right * iconSpacing.x);
                }
                foreach (GameObject path in mapPathways)
                {
                    path.GetComponent<RectTransform>().position += (Vector3.down * iconSpacing.y) + (Vector3.right * iconSpacing.x);
                }

                break;

            case Directions.East:

                foreach (GameObject icon in mapIcons)
                {
                    icon.GetComponent<RectTransform>().position += (Vector3.up * iconSpacing.y) + (Vector3.right * iconSpacing.x);
                }
                foreach (GameObject path in mapPathways)
                {
                    path.GetComponent<RectTransform>().position += (Vector3.up * iconSpacing.y) + (Vector3.right * iconSpacing.x);
                }

                break;

            case Directions.West:

                foreach (GameObject icon in mapIcons)
                {
                    icon.GetComponent<RectTransform>().position += (Vector3.down * iconSpacing.y) + (Vector3.left * iconSpacing.x);
                }
                foreach (GameObject path in mapPathways)
                {
                    path.GetComponent<RectTransform>().position += (Vector3.down * iconSpacing.y) + (Vector3.left * iconSpacing.x);
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

                    mapPathways.Add(Instantiate(pathIconPrefab, this.transform));

                    mapPathways[mapPathways.Count - 1].GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 45);
                    mapPathways[mapPathways.Count - 1].GetComponent<RectTransform>().position += (Vector3.up * pathwaySpacing.y) + (Vector3.left * pathwaySpacing.x);

                    break;

                case Directions.South:

                    mapPathways.Add(Instantiate(pathIconPrefab, this.transform));

                    mapPathways[mapPathways.Count - 1].GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 45);
                    mapPathways[mapPathways.Count - 1].GetComponent<RectTransform>().position += (Vector3.down * pathwaySpacing.y) + (Vector3.right * pathwaySpacing.x);

                    break;

                case Directions.East:

                    mapPathways.Add(Instantiate(pathIconPrefab, this.transform));

                    mapPathways[mapPathways.Count - 1].GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, -45);
                    mapPathways[mapPathways.Count - 1].GetComponent<RectTransform>().position += (Vector3.up * pathwaySpacing.y) + (Vector3.right * pathwaySpacing.x);

                    break;

                case Directions.West:

                    mapPathways.Add(Instantiate(pathIconPrefab, this.transform));

                    mapPathways[mapPathways.Count - 1].GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, -45);
                    mapPathways[mapPathways.Count - 1].GetComponent<RectTransform>().position += (Vector3.down * pathwaySpacing.y) + (Vector3.left * pathwaySpacing.x);

                    break;
            }
            //adding nodes so that the minimaps shows the connected rooms
            for (int i = 0; i < 4; i++)
            {
                if (visitedRoom.connectedRooms[i] == null)
                {
                    continue;
                }
                switch ((Directions)i)
                {
                    case Directions.North:

                        mapPathways.Add(Instantiate(pathIconPrefab, this.transform));

                        mapPathways[mapPathways.Count - 1].GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 45);
                        mapPathways[mapPathways.Count - 1].GetComponent<RectTransform>().position += (Vector3.up * pathwaySpacing.y) + (Vector3.left * pathwaySpacing.x);

                        break;

                    case Directions.South:

                        mapPathways.Add(Instantiate(pathIconPrefab, this.transform));

                        mapPathways[mapPathways.Count - 1].GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 45);
                        mapPathways[mapPathways.Count - 1].GetComponent<RectTransform>().position += (Vector3.down * pathwaySpacing.y) + (Vector3.right * pathwaySpacing.x);

                        break;

                    case Directions.East:

                        mapPathways.Add(Instantiate(pathIconPrefab, this.transform));

                        mapPathways[mapPathways.Count - 1].GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, -45);
                        mapPathways[mapPathways.Count - 1].GetComponent<RectTransform>().position += (Vector3.up * pathwaySpacing.y) + (Vector3.right * pathwaySpacing.x);

                        break;

                    case Directions.West:

                        mapPathways.Add(Instantiate(pathIconPrefab, this.transform));

                        mapPathways[mapPathways.Count - 1].GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, -45);
                        mapPathways[mapPathways.Count - 1].GetComponent<RectTransform>().position += (Vector3.down * pathwaySpacing.y) + (Vector3.left * pathwaySpacing.x);

                        break;
                }
            }
        }

        //Updates the color of the icons based on if they are in the room and what type of room it is
        foreach (GameObject icon in mapIcons)
        {
            //Color iconColor = icon.GetComponent<Image>().color;

            if (icon.GetComponent<RectTransform>().localPosition == Vector3.zero)
            {
                if (!roomPreviouslyVisited)
                {
                    switch (visitedRoom.GetRoomType())
                    {
                        case RoomTypes.Fight:

                            //iconColor = defaultRoomColor;
                            icon.GetComponent<Image>().sprite = roomIcons[0];
                            mapIconSprites.Add(roomIcons[0]);

                            break;

                        case RoomTypes.Totem:

                            //iconColor = totemRoomColor;
                            icon.GetComponent<Image>().sprite = roomIcons[1];
                            mapIconSprites.Add(roomIcons[1]);

                            break;

                        case RoomTypes.End:

                            if (DungeonGenerator.Instance.GetCurrentFloorType() == NodeTypes.Marking)
                            {
                                icon.GetComponent<Image>().sprite = roomIcons[2];
                                mapIconSprites.Add(roomIcons[2]);
                            }
                            else if (DungeonGenerator.Instance.GetCurrentFloorType() == NodeTypes.Shop)
                            {
                                icon.GetComponent<Image>().sprite = roomIcons[3];
                                mapIconSprites.Add(roomIcons[3]);
                            }

                            break;
                    }
                }
                else
                {
                    //Sets the icon to be active and not greyed out
                    if (icon.GetComponent<Image>().sprite == roomIcons[4])
                    {
                        icon.GetComponent<Image>().sprite = roomIcons[0];
                    }
                    if (icon.GetComponent<Image>().sprite == roomIcons[5])
                    {
                        icon.GetComponent<Image>().sprite = roomIcons[1];
                    }
                    if (icon.GetComponent<Image>().sprite == roomIcons[6])
                    {
                        icon.GetComponent<Image>().sprite = roomIcons[2];
                    }
                    if (icon.GetComponent<Image>().sprite == roomIcons[7])
                    {
                        icon.GetComponent<Image>().sprite = roomIcons[3];
                    }
                }
                //iconColor.a = 1;
            }
            else
            {
                //iconColor.a = 0.25f;
                if (icon.GetComponent<Image>().sprite == roomIcons[0])
                {
                    icon.GetComponent<Image>().sprite = roomIcons[4];
                }
                if (icon.GetComponent<Image>().sprite == roomIcons[1])
                {
                    icon.GetComponent<Image>().sprite = roomIcons[5];
                }
                if (icon.GetComponent<Image>().sprite == roomIcons[2])
                {
                    icon.GetComponent<Image>().sprite = roomIcons[6];
                }
                if (icon.GetComponent<Image>().sprite == roomIcons[3])
                {
                    icon.GetComponent<Image>().sprite = roomIcons[7];
                }
            }

            //icon.GetComponent<Image>().color = iconColor;
            //icon.GetComponent<Image>().sprite = roomIcons[0];
        }
    }
}

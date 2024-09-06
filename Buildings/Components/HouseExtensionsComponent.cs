using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static BuildingData;
using static Utility.TilemapManager;
using static Utility.SpriteManager;
using static Utility.ClassManager;
using UnityEngine.UI;
using UnityEngine.U2D;

public class HouseExtensionsComponent : BuildingComponent {

    public enum HouseModifications {
        RemoveCrib,
        OpenBedroom,
        SouthernRoom,
        CornerRoom,
        ExpandedCornerRoom,
        Attic,
        Cubby,
        DiningRoom,
        OpenDiningRoom,
        Marriage
    }

    public enum MarriageCandidate {
        Emily,
        Haley,
        Leah,
        Maru,
        Penny,
        Abigail,
        Alex,
        Elliott,
        Harvey,
        Sam,
        Sebastian,
        Shane,
        Crobus
    }

    public MarriageCandidate spouse;
    public bool isMarried;
    private Sprite spouseRoomSprite;
    public GameObject ModificationMenu { get; private set; }
    Tilemap modificationsTilemap;
    SpriteAtlas spriteAtlas;
    readonly Dictionary<string, Sprite> checkbox = new(2);

    private Tilemap BuildingInteriorTilemap => GetComponent<EnterableBuildingComponent>().BuildingInterior.GetComponent<Tilemap>();

    // Start is called before the first frame update
    void Start() {
        GetComponent<InteractableBuildingComponent>().ButtonsCreated += CreateModificationMenu;
        GameObject modifications = new("Modifications");
        modificationsTilemap = modifications.AddComponent<Tilemap>();
        modifications.AddComponent<TilemapRenderer>().sortingOrder = 1;
        modifications.transform.SetParent(GetComponent<EnterableBuildingComponent>().BuildingInterior.transform);
        spouseRoomSprite = Resources.Load<Sprite>("BuildingInsides/House/SpouseRoom");
        checkbox.Add("On", Resources.Load<Sprite>("UI/CheckBoxOn"));
        checkbox.Add("Off", Resources.Load<Sprite>("UI/CheckBoxOff"));
        spriteAtlas = Resources.Load<SpriteAtlas>("BuildingInsides/House/InteriorModificationsAtlas");
        CreateModificationMenu();
    }

    void CreateModificationMenu() {
        ModificationMenu = Instantiate(Resources.Load<GameObject>("UI/HouseModifications"), GetComponent<EnterableBuildingComponent>().InteriorButtonsParent.transform.parent);
        ModificationMenu.transform.position = Vector3.zero;
        ModificationMenu.GetComponent<HouseModificationMenu>().GetMarriageToggle().onValueChanged.AddListener(ChangeMarriedStatus);
        ModificationMenu.GetComponent<HouseModificationMenu>().GetCornerRoomToggle().onValueChanged.AddListener((isOn) => RenovateHouse(HouseModifications.CornerRoom, isOn));
    }

    public void ToggleModificationMenu() {
        if (ModificationMenu == null) CreateModificationMenu();
        // modificationMenu
        ModificationMenu.GetComponent<MoveablePanel>().TogglePanel();
    }

    bool IsMarriageElligible() {
        return GetComponent<TieredBuildingComponent>().Tier > 1; //to marry you need to upgrade the house at least once
    }

    // Update is called once per frame
    void Update() {

    }

    public void ChangeMarriedStatus(bool isNowMarried) {
        if (!isMarried && !IsMarriageElligible()) {
            GetNotificationManager().SendNotification("You need to upgrade your house to at least tier 2 to get married", NotificationManager.Icons.ErrorIcon);
            return;
        }
        isMarried = isNowMarried;
        if (isMarried) {
            AddSpouseRoom();
            ModificationMenu.transform.Find("Marriage").Find("Checkbox").Find("Image").GetComponent<Image>().sprite = checkbox["On"];
        }
        else {
            RemoveSpouseRoom();
            ModificationMenu.transform.Find("Marriage").Find("Checkbox").Find("Image").GetComponent<Image>().sprite = checkbox["Off"];
        }


        //todo Destroy spouse room here
    }

    public void RemoveSpouseRoom() {
        Vector3Int spouseRoomOrigin = GetComponent<TieredBuildingComponent>().Tier switch {
            2 => new Vector3Int(29, 2, 0),
            3 => new Vector3Int(29, 2, 0),//todo put correct origin
            4 => new Vector3Int(29, 2, 0),
            _ => new Vector3Int(0, 0, 0),
        };
        var area = GetAreaAroundPosition(spouseRoomOrigin, (int)(spouseRoomSprite.textureRect.height / 16), (int)(spouseRoomSprite.textureRect.width / 16));
        foreach (Vector3Int tile in area) modificationsTilemap.SetTile(tile, null);
        modificationsTilemap.CompressBounds();
    }

    public void AddSpouseRoom() {
        Vector3Int spouseRoomOrigin = GetComponent<TieredBuildingComponent>().Tier switch {
            2 => new Vector3Int(29, 2, 0),
            3 => new Vector3Int(29, 2, 0),//todo put correct origin
            4 => new Vector3Int(29, 2, 0),
            _ => new Vector3Int(0, 0, 0),
        };
        var area = GetAreaAroundPosition(spouseRoomOrigin, (int)(spouseRoomSprite.textureRect.height / 16), (int)(spouseRoomSprite.textureRect.width / 16));
        modificationsTilemap.SetTiles(area.ToArray(), SplitSprite(spouseRoomSprite));
        modificationsTilemap.CompressBounds();
    }

    public void RenovateHouse(HouseModifications modification, bool isOn) {
        if (GetComponent<TieredBuildingComponent>().Tier < 3) {
            GetNotificationManager().SendNotification("You need to upgrade your house to at least tier 3 to renovate it", NotificationManager.Icons.ErrorIcon);
            return;
        }
        Vector3Int origin;
        Sprite sprite;
        switch (modification) {
            case HouseModifications.RemoveCrib:
                break;
            case HouseModifications.OpenBedroom:
                break;
            case HouseModifications.SouthernRoom:
                break;
            case HouseModifications.CornerRoom:
                origin = new Vector3Int(27, 14, 0);
                if (isOn) sprite = spriteAtlas.GetSprite("CornerRoomWalls");
                else sprite = spriteAtlas.GetSprite("CornerRoomRemovedWalls");
                BuildingInteriorTilemap.SetTiles(GetAreaAroundPosition(origin, (int)(sprite.textureRect.height / 16), (int)(sprite.textureRect.width / 16)).ToArray(), SplitSprite(sprite));
                ModificationMenu.transform.Find("Renovations").Find("CornerRoom").Find("Button").GetComponent<Image>().sprite = isOn ? checkbox["On"] : checkbox["Off"];
                break;
            case HouseModifications.ExpandedCornerRoom:
                break;
            case HouseModifications.Attic:
                break;
            case HouseModifications.Cubby:
                break;
            case HouseModifications.DiningRoom:
                break;
            case HouseModifications.OpenDiningRoom:
                break;
            default:
                break;
        }
    }



    public override void Load(ComponentData data) {
        return;
    }

    public override ComponentData Save() {
        return null;
    }
}

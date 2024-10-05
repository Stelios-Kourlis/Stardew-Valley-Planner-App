using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static BuildingData;
using static Utility.TilemapManager;
using static Utility.SpriteManager;
using static Utility.ClassManager;
using static Utility.InvalidTileLoader;
using UnityEngine.UI;
using UnityEngine.U2D;
using System.Linq;

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
    }

    public enum MarriageCandidate {
        Abigail,
        Alex,
        Elliot,
        Emily,
        Haley,
        Harvey,
        Leah,
        Maru,
        Penny,
        Sam,
        Sebastian,
        Shane,
        Krobus
    }

    public MarriageCandidate spouse;
    private Sprite spouseRoomSprite;
    public bool isMarried;
    public HouseModificationMenu ModificationMenu { get; private set; }
    Tilemap spouseRoomTilemap;
    SpriteAtlas spriteAtlas;
    readonly Dictionary<string, Sprite> checkbox = new(2);

    private Tilemap BuildingInteriorTilemap => GetComponent<EnterableBuildingComponent>().BuildingInterior.GetComponent<Tilemap>();

    // Start is called before the first frame update
    void Start() {
        GetComponent<InteractableBuildingComponent>().ButtonsCreated += CreateModificationMenu;
        GameObject spouseRoom = new("SpouseRoomTilemap");
        spouseRoomTilemap = spouseRoom.AddComponent<Tilemap>();
        spouseRoom.AddComponent<TilemapRenderer>().sortingOrder = -102;
        spouseRoom.transform.SetParent(GetComponent<EnterableBuildingComponent>().BuildingInterior.transform);
        spouseRoomSprite = Resources.Load<Sprite>("BuildingInsides/House/SpouseRoom");
        checkbox.Add("On", Resources.Load<Sprite>("UI/CheckBoxOn"));
        checkbox.Add("Off", Resources.Load<Sprite>("UI/CheckBoxOff"));
        spriteAtlas = Resources.Load<SpriteAtlas>("BuildingInsides/House/InteriorModificationsAtlas");
        CreateModificationMenu();
    }

    void CreateModificationMenu() {
        ModificationMenu = Instantiate(Resources.Load<GameObject>("UI/HouseModifications"), GetComponent<EnterableBuildingComponent>().InteriorButtonsParent.transform.parent).GetComponent<HouseModificationMenu>();
        ModificationMenu.transform.position = Vector3.zero;
        ModificationMenu.GetMarriageToggle().onValueChanged.AddListener(ChangeMarriedStatus);
        ModificationMenu.spouseChanged += SetSpouse;
        ModificationMenu.GetCornerRoomToggle().onValueChanged.AddListener((isOn) => RenovateHouse(HouseModifications.CornerRoom, isOn));
    }

    public void ToggleModificationMenu() {
        if (ModificationMenu == null) CreateModificationMenu();
        // modificationMenu
        ModificationMenu.GetComponent<MoveablePanel>().TogglePanel();
    }

    bool IsMarriageElligible() {
        return GetComponent<TieredBuildingComponent>().Tier > 1; //to marry you need to upgrade the house at least once
    }

    public void ChangeMarriedStatus(bool isNowMarried) {
        if (!IsMarriageElligible()) {
            NotificationManager.Instance.SendNotification("You need to upgrade your house to at least tier 2 to get married", NotificationManager.Icons.ErrorIcon);
            return;
        }

        isMarried = isNowMarried;

        if (isMarried) AddSpouseRoom();
        else RemoveSpouseRoom();

        ModificationMenu.GetMarriageToggle().transform.Find("Image").GetComponent<Image>().sprite = checkbox[isMarried ? "On" : "Off"];
        ModificationMenu.SetSpouseDropdownInteractability(isMarried);
    }

    public void RemoveSpouseRoom() {
        Vector3Int spouseRoomOrigin = GetSpouseRoomOrigin();
        Sprite spouseRoomRemoved = Resources.Load<Sprite>("BuildingInsides/House/SpouseRoomRemoved");
        Vector3Int[] area = GetRectAreaFromPoint(spouseRoomOrigin, (int)(spouseRoomRemoved.textureRect.height / 16), (int)(spouseRoomRemoved.textureRect.width / 16)).ToArray();
        BuildingInteriorTilemap.SetTiles(area, SplitSprite(spouseRoomRemoved));
        BuildingInteriorTilemap.CompressBounds();

        spouseRoomTilemap.ClearAllTiles();

        GetComponent<EnterableBuildingComponent>().InteriorSpecialTiles.RemoveSpecialTileSet("SpouseRoom");
        InvalidTilesManager.Instance.UpdateAllCoordinates();
    }

    public void AddSpouseRoom() {
        Vector3Int spouseRoomOrigin = GetSpouseRoomOrigin();
        var area = GetRectAreaFromPoint(spouseRoomOrigin, (int)(spouseRoomSprite.textureRect.height / 16), (int)(spouseRoomSprite.textureRect.width / 16));
        BuildingInteriorTilemap.SetTiles(area.ToArray(), SplitSprite(spouseRoomSprite));
        BuildingInteriorTilemap.CompressBounds();

        SpecialCoordinateRect spouseSpecialTileSet = GetSpecialCoordinateSet("SpouseRoom");
        spouseSpecialTileSet.offset = spouseRoomOrigin - new Vector3Int(0, 1, 0);
        GetComponent<EnterableBuildingComponent>().InteriorSpecialTiles.AddSpecialTileSet(spouseSpecialTileSet);

        SetSpouse((int)spouse); //Refresh spouse room
        InvalidTilesManager.Instance.UpdateAllCoordinates();
    }

    public void SetSpouse(int candidate) {
        Vector3Int spouseRoomOrigin = GetSpouseRoomOrigin() + new Vector3Int(1, 0, 0);

        //clear old spouse room invalid coords
        GetComponent<EnterableBuildingComponent>().InteriorSpecialTiles.RemoveSpecialTileSet(spouse.ToString());

        spouse = (MarriageCandidate)candidate;

        //Draw room
        Sprite spouseRoom = Resources.Load<SpriteAtlas>($"BuildingInsides/House/SpouseRoomAtlas").GetSprite(spouse.ToString());
        var area = GetRectAreaFromPoint(spouseRoomOrigin, (int)(spouseRoom.textureRect.height / 16), (int)(spouseRoom.textureRect.width / 16));
        spouseRoomTilemap.SetTiles(area.ToArray(), SplitSprite(spouseRoom));

        SpecialCoordinateRect spouseSpecialTileSet = GetSpecialCoordinateSet(spouse.ToString());
        spouseSpecialTileSet.offset = spouseRoomOrigin;
        GetComponent<EnterableBuildingComponent>().InteriorSpecialTiles.AddSpecialTileSet(spouseSpecialTileSet);
    }

    private Vector3Int GetSpouseRoomOrigin() {
        return GetComponent<TieredBuildingComponent>().Tier switch {
            2 => new Vector3Int(29, 2, 0),
            3 => new Vector3Int(34, 5, 0),
            4 => new Vector3Int(34, 5, 0),
            _ => new Vector3Int(0, 0, 0),
        };
    }

    public void RenovateHouse(HouseModifications modification, bool isOn) {
        if (GetComponent<TieredBuildingComponent>().Tier < 3) {
            NotificationManager.Instance.SendNotification("You need to upgrade your house to at least tier 3 to renovate it", NotificationManager.Icons.ErrorIcon);
            return;
        }
        Vector3Int origin = new();
        string modificationName = "";
        switch (modification) {
            case HouseModifications.RemoveCrib:
                break;
            case HouseModifications.OpenBedroom:
                break;
            case HouseModifications.SouthernRoom:
                break;
            case HouseModifications.CornerRoom:
                origin = new Vector3Int(27, 11, 0);
                modificationName = "CornerRoom";
                ModificationMenu.transform.Find("TabContent").Find("Modifications").Find("Renovations").Find("CornerRoom").Find("Button").GetComponent<Image>().sprite = checkbox[isOn ? "On" : "Off"];
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

        Sprite sprite;
        if (isOn) {
            sprite = spriteAtlas.GetSprite(modificationName);
            SpecialCoordinateRect cornerRoomSpecialTileSet = GetSpecialCoordinateSet(modificationName);
            cornerRoomSpecialTileSet.offset = origin;
            GetComponent<EnterableBuildingComponent>().InteriorSpecialTiles.AddSpecialTileSet(cornerRoomSpecialTileSet);
        }
        else {
            sprite = spriteAtlas.GetSprite($"{modificationName}Removed");
            GetComponent<EnterableBuildingComponent>().InteriorSpecialTiles.RemoveSpecialTileSet(modificationName);
        }
        BuildingInteriorTilemap.SetTiles(GetRectAreaFromPoint(origin, (int)(sprite.textureRect.height / 16), (int)(sprite.textureRect.width / 16)).ToArray(), SplitSprite(sprite));
    }

    public override void Load(ComponentData data) {
        return;
    }

    public override ComponentData Save() {
        return null;
    }
}

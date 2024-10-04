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
    // void Start()
    // {
    //     GetComponent<InteractableBuildingComponent>().ButtonsCreated += CreateModificationMenu;
    //     GameObject spouseRoom = new("SpouseRoomTilemap");
    //     spouseRoomTilemap = spouseRoom.AddComponent<Tilemap>();
    //     spouseRoom.AddComponent<TilemapRenderer>().sortingOrder = -102;
    //     spouseRoom.transform.SetParent(GetComponent<EnterableBuildingComponent>().BuildingInterior.transform);
    //     spouseRoomSprite = Resources.Load<Sprite>("BuildingInsides/House/SpouseRoom");
    //     checkbox.Add("On", Resources.Load<Sprite>("UI/CheckBoxOn"));
    //     checkbox.Add("Off", Resources.Load<Sprite>("UI/CheckBoxOff"));
    //     spriteAtlas = Resources.Load<SpriteAtlas>("BuildingInsides/House/InteriorModificationsAtlas");
    //     CreateModificationMenu();
    // }

    // void CreateModificationMenu()
    // {
    //     ModificationMenu = Instantiate(Resources.Load<GameObject>("UI/HouseModifications"), GetComponent<EnterableBuildingComponent>().InteriorButtonsParent.transform.parent).GetComponent<HouseModificationMenu>();
    //     ModificationMenu.transform.position = Vector3.zero;
    //     ModificationMenu.GetMarriageToggle().onValueChanged.AddListener(ChangeMarriedStatus);
    //     ModificationMenu.spouseChanged += SetSpouse;
    //     ModificationMenu.GetCornerRoomToggle().onValueChanged.AddListener((isOn) => RenovateHouse(HouseModifications.CornerRoom, isOn));
    // }

    // public void ToggleModificationMenu()
    // {
    //     if (ModificationMenu == null) CreateModificationMenu();
    //     // modificationMenu
    //     ModificationMenu.GetComponent<MoveablePanel>().TogglePanel();
    // }

    // bool IsMarriageElligible()
    // {
    //     return GetComponent<TieredBuildingComponent>().Tier > 1; //to marry you need to upgrade the house at least once
    // }

    // public void ChangeMarriedStatus(bool isNowMarried)
    // {
    //     if (!IsMarriageElligible())
    //     {
    //         NotificationManager.Instance.SendNotification("You need to upgrade your house to at least tier 2 to get married", NotificationManager.Icons.ErrorIcon);
    //         return;
    //     }

    //     isMarried = isNowMarried;

    //     if (isMarried) AddSpouseRoom();
    //     else RemoveSpouseRoom();

    //     ModificationMenu.GetMarriageToggle().transform.Find("Image").GetComponent<Image>().sprite = checkbox[isMarried ? "On" : "Off"];
    //     ModificationMenu.SetSpouseDropdownInteractability(isMarried);
    // }

    // public void RemoveSpouseRoom()
    // {
    //     Vector3Int spouseRoomOrigin = GetSpouseRoomOrigin();
    //     Sprite spouseRoomRemoved = Resources.Load<Sprite>("BuildingInsides/House/SpouseRoomRemoved");
    //     Vector3Int[] area = GetRectAreaFromPoint(spouseRoomOrigin, (int)(spouseRoomRemoved.textureRect.height / 16), (int)(spouseRoomRemoved.textureRect.width / 16)).ToArray();
    //     BuildingInteriorTilemap.SetTiles(area, SplitSprite(spouseRoomRemoved));
    //     BuildingInteriorTilemap.CompressBounds();

    //     spouseRoomTilemap.ClearAllTiles();

    //     SpecialCoordinateSet newInvalidTiles = GetInsideUnavailableCoordinates("SpouseRoomRemoved");
    //     newInvalidTiles = new(newInvalidTiles.identifier, newInvalidTiles.Select(tile => tile + spouseRoomOrigin - new Vector3Int(0, 1, 0)), newInvalidTiles.type);
    //     GetComponent<EnterableBuildingComponent>().InteriorSpecialTiles.AddSpecialTileSet(newInvalidTiles);
    //     // HashSet<Vector3Int> newNeutralTiles = GetInsideNeutralCoordinates("SpouseRoomRemoved");
    //     // newNeutralTiles = newNeutralTiles.Select(tile => tile + spouseRoomOrigin - new Vector3Int(0, 1, 0)).ToHashSet();
    //     // GetComponent<EnterableBuildingComponent>().RemoveFromInteriorUnavailableCoordinates(newNeutralTiles);
    //     InvalidTilesManager.Instance.UpdateAllCoordinates();
    // }

    // public void AddSpouseRoom()
    // {
    //     Vector3Int spouseRoomOrigin = GetSpouseRoomOrigin();
    //     var area = GetRectAreaFromPoint(spouseRoomOrigin, (int)(spouseRoomSprite.textureRect.height / 16), (int)(spouseRoomSprite.textureRect.width / 16));
    //     BuildingInteriorTilemap.SetTiles(area.ToArray(), SplitSprite(spouseRoomSprite));
    //     BuildingInteriorTilemap.CompressBounds();

    //     // HashSet<Vector3Int> newNeutralTiles = GetInsideNeutralCoordinates("SpouseRoom");
    //     // newNeutralTiles = newNeutralTiles.Select(tile => tile + spouseRoomOrigin - new Vector3Int(0, 1, 0)).ToHashSet();
    //     // GetComponent<EnterableBuildingComponent>().RemoveFromInteriorUnavailableCoordinates(newNeutralTiles);

    //     SpecialCoordinateSet newInvalidTiles = GetInsideUnavailableCoordinates("SpouseRoom");
    //     newInvalidTiles = newInvalidTiles.Select(tile => tile + spouseRoomOrigin - new Vector3Int(0, 1, 0)).ToHashSet();
    //     GetComponent<EnterableBuildingComponent>().AddToInteriorUnavailableCoordinates(newInvalidTiles);

    //     SetSpouse((int)spouse); //Refresh spouse room
    //     InvalidTilesManager.Instance.UpdateAllCoordinates();
    // }

    // public void SetSpouse(int candidate)
    // {
    //     Vector3Int spouseRoomOrigin = GetSpouseRoomOrigin() + new Vector3Int(1, 0, 0);

    //     HashSet<Vector3Int> oldSpouseSpecificInvalidTiles = GetInsideUnavailableCoordinates(spouse.ToString()); //clear old spouse room invalid coords
    //     oldSpouseSpecificInvalidTiles = oldSpouseSpecificInvalidTiles.Select(tile => tile + spouseRoomOrigin).ToHashSet();
    //     GetComponent<EnterableBuildingComponent>().RemoveFromInteriorUnavailableCoordinates(oldSpouseSpecificInvalidTiles);

    //     spouse = (MarriageCandidate)candidate;

    //     //Draw room
    //     Sprite spouseRoom = Resources.Load<SpriteAtlas>($"BuildingInsides/House/SpouseRoomAtlas").GetSprite(spouse.ToString());
    //     var area = GetRectAreaFromPoint(spouseRoomOrigin, (int)(spouseRoom.textureRect.height / 16), (int)(spouseRoom.textureRect.width / 16));
    //     spouseRoomTilemap.SetTiles(area.ToArray(), SplitSprite(spouseRoom));

    //     HashSet<Vector3Int> newSpouseSpecificInvalidTiles = GetInsideUnavailableCoordinates(spouse.ToString());
    //     newSpouseSpecificInvalidTiles = newSpouseSpecificInvalidTiles.Select(tile => tile + spouseRoomOrigin).ToHashSet();
    //     GetComponent<EnterableBuildingComponent>().AddToInteriorUnavailableCoordinates(newSpouseSpecificInvalidTiles);
    // }

    // private Vector3Int GetSpouseRoomOrigin()
    // {
    //     return GetComponent<TieredBuildingComponent>().Tier switch
    //     {
    //         2 => new Vector3Int(29, 2, 0),
    //         3 => new Vector3Int(34, 5, 0),
    //         4 => new Vector3Int(34, 5, 0),
    //         _ => new Vector3Int(0, 0, 0),
    //     };
    // }

    // public void RenovateHouse(HouseModifications modification, bool isOn)
    // {
    //     if (GetComponent<TieredBuildingComponent>().Tier < 3)
    //     {
    //         NotificationManager.Instance.SendNotification("You need to upgrade your house to at least tier 3 to renovate it", NotificationManager.Icons.ErrorIcon);
    //         return;
    //     }
    //     Vector3Int origin = new();
    //     Sprite sprite;
    //     string name = "";
    //     switch (modification)
    //     {
    //         case HouseModifications.RemoveCrib:
    //             break;
    //         case HouseModifications.OpenBedroom:
    //             break;
    //         case HouseModifications.SouthernRoom:
    //             break;
    //         case HouseModifications.CornerRoom:
    //             origin = new Vector3Int(27, 11, 0);
    //             name = "CornerRoom";

    //             ModificationMenu.transform.Find("TabContent").Find("Modifications").Find("Renovations").Find("CornerRoom").Find("Button").GetComponent<Image>().sprite = checkbox[isOn ? "On" : "Off"];
    //             break;
    //         case HouseModifications.ExpandedCornerRoom:
    //             break;
    //         case HouseModifications.Attic:
    //             break;
    //         case HouseModifications.Cubby:
    //             break;
    //         case HouseModifications.DiningRoom:
    //             break;
    //         case HouseModifications.OpenDiningRoom:
    //             break;
    //         default:
    //             break;
    //     }

    //     if (isOn)
    //     {
    //         sprite = spriteAtlas.GetSprite(name);
    //         RemoveOldTiles($"{name}Removed", origin);
    //         AddNewTiles(name, origin);
    //     }
    //     else
    //     {
    //         sprite = spriteAtlas.GetSprite($"{name}Removed");
    //         RemoveOldTiles(name, origin);
    //         AddNewTiles($"{name}Removed", origin);
    //     }
    //     BuildingInteriorTilemap.SetTiles(GetRectAreaFromPoint(origin, (int)(sprite.textureRect.height / 16), (int)(sprite.textureRect.width / 16)).ToArray(), SplitSprite(sprite));
    // }

    // // private void RemoveOldTiles(string name, Vector3Int offset) {
    // //     HashSet<Vector3Int> oldInvalidTiles = GetInsideUnavailableCoordinates(name);
    // //     oldInvalidTiles = oldInvalidTiles.Select(tile => tile + offset).ToHashSet();
    // //     GetComponent<EnterableBuildingComponent>().RemoveFromInteriorUnavailableCoordinates(oldInvalidTiles);
    // // }

    // // private void AddNewTiles(string name, Vector3Int offset) {
    // //     HashSet<Vector3Int> newInvalidTiles = GetInsideUnavailableCoordinates(name);
    // //     newInvalidTiles = newInvalidTiles.Select(tile => tile + offset).ToHashSet();
    // //     GetComponent<EnterableBuildingComponent>().AddToInteriorUnavailableCoordinates(newInvalidTiles);

    // //     HashSet<Vector3Int> newNeutralTiles = GetInsideNeutralCoordinates(name);
    // //     newNeutralTiles = newNeutralTiles.Select(tile => tile + offset).ToHashSet();
    // //     GetComponent<EnterableBuildingComponent>().RemoveFromInteriorUnavailableCoordinates(newNeutralTiles);
    // // }



    public override void Load(ComponentData data) {
        return;
    }

    public override ComponentData Save() {
        return null;
    }
}

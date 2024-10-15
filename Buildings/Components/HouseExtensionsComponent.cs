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
using Utility;
using static WallsComponent;
using static FlooringComponent;

public class HouseExtensionsComponent : BuildingComponent {

    public enum HouseModifications {
        Crib,
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
    public GameObject modificationWarning;
    private Sprite spouseRoomSprite;
    public bool isMarried;
    public HouseModificationMenu ModificationMenu { get; private set; }
    Tilemap spouseRoomTilemap, frontTilemap;
    // Tilemap spouseRoomWallsTilemap;
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

        GameObject frontTilemapGameObj = new("SpouseRoomWallsTilemap");
        frontTilemap = frontTilemapGameObj.AddComponent<Tilemap>();
        frontTilemapGameObj.AddComponent<TilemapRenderer>().sortingOrder = BuildingInteriorTilemap.GetComponent<TilemapRenderer>().sortingOrder + 1;
        frontTilemapGameObj.transform.SetParent(GetComponent<EnterableBuildingComponent>().BuildingInterior.transform);

        spouseRoomSprite = Resources.Load<Sprite>("BuildingInsides/House/SpouseRoom");
        checkbox.Add("On", Resources.Load<Sprite>("UI/CheckBoxOn"));
        checkbox.Add("Off", Resources.Load<Sprite>("UI/CheckBoxOff"));
        spriteAtlas = Resources.Load<SpriteAtlas>("BuildingInsides/House/InteriorModificationsAtlas");
        modificationWarning = Resources.Load<GameObject>("UI/ModificationWarning");
        CreateModificationMenu();

        GetComponent<TieredBuildingComponent>().tierChanged += newTier => BuildingTierChange(newTier);
    }

    void CreateModificationMenu() {
        ModificationMenu = Instantiate(Resources.Load<GameObject>("UI/HouseModifications"), GetComponent<EnterableBuildingComponent>().InteriorButtonsParent.transform.parent).GetComponent<HouseModificationMenu>();
        ModificationMenu.transform.position = Vector3.zero;
        ModificationMenu.GetMarriageToggle().onValueChanged.AddListener(ChangeMarriedStatus);
        ModificationMenu.spouseChanged += SetSpouse;



        ModificationMenu.GetModificationToggle("CornerRoom").onValueChanged.AddListener((isOn) => RenovateHouse(HouseModifications.CornerRoom, isOn));
        ModificationMenu.GetModificationToggle("Attic").onValueChanged.AddListener((isOn) => RenovateHouse(HouseModifications.Attic, isOn));
        ModificationMenu.GetModificationToggle("Crib").onValueChanged.AddListener((isOn) => RenovateHouse(HouseModifications.Crib, isOn));
        ModificationMenu.GetModificationToggle("Cubby").onValueChanged.AddListener((isOn) => RenovateHouse(HouseModifications.Cubby, isOn));
    }

    private void BuildingTierChange(int newTier) {
        if (newTier != 3) return;
        RenovateHouse(HouseModifications.Crib, true);
        ModificationMenu.GetModificationToggle("Crib").isOn = true;
        ModificationMenu.SetAllToglesToOff();
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
        GetComponent<EnterableBuildingComponent>().InteriorSpecialTiles.RemoveSpecialTileSet(spouse.ToString());
        InvalidTilesManager.Instance.UpdateAllCoordinates();
    }

    public void AddSpouseRoom() {
        Vector3Int spouseRoomOrigin = GetSpouseRoomOrigin();
        var area = GetRectAreaFromPoint(spouseRoomOrigin, (int)(spouseRoomSprite.textureRect.height / 16), (int)(spouseRoomSprite.textureRect.width / 16));
        BuildingInteriorTilemap.SetTiles(area.ToArray(), SplitSprite(spouseRoomSprite));
        BuildingInteriorTilemap.CompressBounds();

        SpecialCoordinateRect spouseSpecialTileSet = GetSpecialCoordinateSet("SpouseRoom");
        spouseSpecialTileSet.AddOffset(spouseRoomOrigin - new Vector3Int(0, 1, 0));
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
        spouseSpecialTileSet.AddOffset(spouseRoomOrigin);
        GetComponent<EnterableBuildingComponent>().InteriorSpecialTiles.AddSpecialTileSet(spouseSpecialTileSet);
        InvalidTilesManager.Instance.UpdateAllCoordinates();
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

        Debug.Log($"BuildingInsides/House/{modification}");
        HouseModificationScriptableObject values = (HouseModificationScriptableObject)Resources.Load<ScriptableObject>($"BuildingInsides/House/{modification}");
        List<Vector3Int> positions = GetRectAreaFromPoint(values.spriteOrigin, (int)(values.backSprite.textureRect.height / 16), (int)(values.backSprite.textureRect.width / 16));

        SpecialCoordinateRect modificationSpecialTiles = GetSpecialCoordinateSet(values.type.ToString());
        modificationSpecialTiles.AddOffset(values.spriteOrigin);
        List<Vector3Int> newInvalidPositions = modificationSpecialTiles.GetSpecialCoordinates().ToList().Where(position => position.type == TileType.Invalid).ToList().ConvertAll(position => position.position);

        bool buildingPreventsModificationPlacement = BuildingController.buildings.Where(building => building.BaseCoordinates.Intersect(positions).Any()).Any() && isOn;
        bool buildingPreventsModificationRemoval = BuildingController.buildings.Where(building => building.BaseCoordinates.Intersect(positions.Except(newInvalidPositions)).Any()).Any() && !isOn;
        if (isOn) {
            if (BuildingController.buildings.Where(building => building.BaseCoordinates.Intersect(positions).Any()).Any()) {
                GameObject warning = Instantiate(modificationWarning, GetCanvasGameObject().transform);
                warning.SetActive(true);
                warning.transform.GetChild(1).Find("Yes").GetComponent<Button>().onClick.AddListener(() => {
                    List<Building> intersectingBuildings = BuildingController.buildings.Where(building => building.BaseCoordinates.Intersect(positions).Any()).ToList();
                    foreach (Building building in intersectingBuildings) building.DeleteBuilding();
                    ApplyRenovation(modification, isOn, values, positions);
                    Destroy(warning);
                });
                warning.transform.GetChild(1).Find("Cancel").GetComponent<Button>().onClick.AddListener(() => { Destroy(warning); });
            }
            else ApplyRenovation(modification, isOn, values, positions);
        }
        else {
            // List<Vector3Int> intersectingBuildings = BuildingController.buildings.Where(building => building.BaseCoordinates.Intersect(positions).Any()).ToList();

            if (BuildingController.buildings.Where(building => building.BaseCoordinates.Intersect(positions.Except(newInvalidPositions)).Any()).Any()) {
                GameObject warning = Instantiate(modificationWarning, GetCanvasGameObject().transform);
                warning.SetActive(true);
                warning.transform.GetChild(1).Find("Yes").GetComponent<Button>().onClick.AddListener(() => {
                    List<Building> intersectingBuildings = BuildingController.buildings.Where(building => building.BaseCoordinates.Intersect(positions.Except(newInvalidPositions)).Any()).ToList();
                    foreach (Building building in intersectingBuildings) building.DeleteBuilding();
                    ApplyRenovation(modification, isOn, values, positions);
                    Destroy(warning);
                });
                warning.transform.GetChild(1).Find("Cancel").GetComponent<Button>().onClick.AddListener(() => { Destroy(warning); });
            }
            else ApplyRenovation(modification, isOn, values, positions);

            // foreach (Vector3Int pos in positions.Except(newInvalidPositions)) {
            //     BuildingInteriorTilemap.SetTile(pos, LoadTile(Tiles.Green));
            // }
        }

    }



    private void ApplyRenovation(HouseModifications modification, bool isOn, HouseModificationScriptableObject values, List<Vector3Int> positions) {
        if (isOn) {
            SpecialCoordinateRect modificationSpecialTiles = GetSpecialCoordinateSet(values.type.ToString());
            modificationSpecialTiles.AddOffset(values.spriteOrigin);
            GetComponent<EnterableBuildingComponent>().InteriorSpecialTiles.AddSpecialTileSet(modificationSpecialTiles);
            Tile[] removedTiles = SplitSprite(values.backRemoved);
            for (int i = 0; i < positions.Count; i++) {
                if (removedTiles[i] != null) {
                    BuildingInteriorTilemap.SetTile(positions[i], null);
                }
            }
            SetTilesOnlyNonNull(SplitSprite(values.backSprite), positions.ToArray(), BuildingInteriorTilemap);
            SetTilesOnlyNonNull(SplitSprite(values.frontSprite), positions.ToArray(), frontTilemap);
            foreach (WallOrigin origin in values.wallOrigins) GetComponent<WallsComponent>().AddWall(origin);
            foreach (FlooringOrigin origin in values.floorOrigins) GetComponent<FlooringComponent>().AddFloor(origin);
            foreach (WallMove wallMover in values.wallModifications) GetComponent<WallsComponent>().MoveWall(wallMover.oldWallPoint, wallMover.newWall);
        }
        else {
            GetComponent<EnterableBuildingComponent>().InteriorSpecialTiles.RemoveSpecialTileSet(values.type.ToString());
            SetTilesOnlyNonNull(SplitSprite(values.backRemoved), positions.ToArray(), BuildingInteriorTilemap);
            foreach (Vector3Int position in positions) frontTilemap.SetTile(position, null);
            foreach (WallOrigin origin in values.wallOrigins) GetComponent<WallsComponent>().RemoveWall(origin.lowerLeftCorner);
            foreach (FlooringOrigin origin in values.floorOrigins) GetComponent<FlooringComponent>().RemoveFloor(origin.lowerLeftCorner);
            foreach (WallMove wallMover in values.wallModifications) GetComponent<WallsComponent>().MoveWall(wallMover.oldWallPoint, wallMover.GetReverseModification());
        }

        ModificationMenu.transform.Find("TabContent").Find("Modifications").Find("Renovations").Find($"{modification}").Find("Button").GetComponent<Image>().sprite = checkbox[isOn ? "On" : "Off"];
        ResolveConflicts();
        InvalidTilesManager.Instance.UpdateAllCoordinates();
    }

    //
    private void ResolveConflicts() {

    }

    private void CreateWarning(IEnumerable<Vector3Int> positions) {
        // GameObject warning = Instantiate(modificationWarning, GetCanvasGameObject().transform);
        // warning.SetActive(true);
        // warning.transform.GetChild(1).Find("Yes").GetComponent<Button>().onClick.AddListener(() => {
        //     List<Building> intersectingBuildings = BuildingController.buildings.Where(building => building.BaseCoordinates.Intersect(positions).Any()).ToList();
        //     foreach (Building building in intersectingBuildings) building.DeleteBuilding();
        //     ApplyRenovation(modification, isOn, values, positions);
        //     Destroy(warning);
        // });
        // warning.transform.GetChild(1).Find("Cancel").GetComponent<Button>().onClick.AddListener(() => { Destroy(warning); });
    }

    public override void Load(ComponentData data) {
        return;
    }

    public override ComponentData Save() {
        return null;
    }
}

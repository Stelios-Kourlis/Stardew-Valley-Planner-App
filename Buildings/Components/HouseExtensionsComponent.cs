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
using System;
using System.IO;
using TMPro;
using Newtonsoft.Json.Linq;

public class HouseExtensionsComponent : BuildingComponent {

    public enum HouseModifications {
        Null,
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
    public bool isMarried;
    public HouseModificationMenu ModificationMenu { get; private set; }
    private bool isLoadingDefaultRenovations;
    private Dictionary<HouseModifications, bool?> houseModificationsActive;
    private static Dictionary<HouseModifications, HouseModificationScriptableObject> scriptableObjects = new();
    Tilemap spouseRoomTilemap, frontTilemap, overlapTilemap;
    private bool hasExtensions;
    private bool hasFlooringOrWallpapers;
    readonly Dictionary<string, Sprite> checkbox = new(2);

    private Tilemap BuildingInteriorTilemap => GetComponent<EnterableBuildingComponent>().BuildingInterior.GetComponent<Tilemap>();

    private void AddTilemaps() {
        GetComponent<EnterableBuildingComponent>().InteriorUpdated -= AddTilemaps;

        GameObject spouseRoom = new("SpouseRoomTilemap");
        spouseRoomTilemap = spouseRoom.AddComponent<Tilemap>();
        spouseRoom.AddComponent<TilemapRenderer>().sortingOrder = -102;
        spouseRoom.transform.SetParent(GetComponent<EnterableBuildingComponent>().BuildingInterior.transform);

        GameObject frontTilemapGameObj = new("frontTilemap");
        frontTilemap = frontTilemapGameObj.AddComponent<Tilemap>();
        frontTilemapGameObj.AddComponent<TilemapRenderer>().sortingOrder = BuildingInteriorTilemap.GetComponent<TilemapRenderer>().sortingOrder + 1;
        frontTilemapGameObj.transform.SetParent(GetComponent<EnterableBuildingComponent>().BuildingInterior.transform);

        GameObject overlapTilemapGameObj = new("overlapTilemap");
        overlapTilemap = overlapTilemapGameObj.AddComponent<Tilemap>();
        overlapTilemapGameObj.AddComponent<TilemapRenderer>().sortingOrder = BuildingInteriorTilemap.GetComponent<TilemapRenderer>().sortingOrder + 1000;
        overlapTilemapGameObj.transform.SetParent(GetComponent<EnterableBuildingComponent>().BuildingInterior.transform);
    }

    void CreateModificationMenu() {
        GetComponent<EnterableBuildingComponent>().InteriorUpdated -= CreateModificationMenu;

        ModificationMenu = Instantiate(Resources.Load<GameObject>("UI/HouseModifications"), GetComponent<EnterableBuildingComponent>().GetInteriorButton(ButtonTypes.ENTER).transform.parent).GetComponent<HouseModificationMenu>();
        ModificationMenu.transform.position = Vector3.zero;

        if (!hasExtensions) {
            ModificationMenu.DisableModificationTab();
            return;
        }

        ModificationMenu.GetMarriageToggle().onValueChanged.AddListener(ChangeMarriedStatus);
        ModificationMenu.spouseChanged += SetSpouse;

        foreach (HouseModifications modification in Enum.GetValues(typeof(HouseModifications)))
            if (modification != HouseModifications.Null && modification != HouseModifications.Marriage)
                ModificationMenu.GetModificationToggle(modification).onValueChanged.AddListener((isOn) => CheckRenovationPreconditionsAndApply(modification, isOn));
    }

    private void BuildingTierChange() {
        if (GetComponent<TieredBuildingComponent>().Tier < 3) {
            frontTilemap.ClearAllTiles();
            // Debug.Log("Clearing front tilemap");
            return;
        }

        // RenovateHouse(HouseModifications.RemoveCrib, true);
        if (ModificationMenu == null) CreateModificationMenu();
        isLoadingDefaultRenovations = true;
        ModificationMenu.GetModificationToggle(HouseModifications.RemoveCrib).isOn = true;
        ModificationMenu.GetModificationToggle(HouseModifications.OpenBedroom).isOn = true;
        isLoadingDefaultRenovations = false;
        ModificationMenu.SetAllToglesSpritesToOff();
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
        UndoRedoController.ignoreAction = true;
        if (isMarried) AddSpouseRoom();
        else RemoveSpouseRoom();
        UndoRedoController.ignoreAction = false;

        UndoRedoController.AddActionToLog(new HouseRenovationRecord(ModificationMenu, HouseModifications.Marriage, isMarried));

        // ResolveConflicts(HouseModifications.Marriage, isMarried);

        // ModificationMenu.GetMarriageToggle().transform.Find("Image").GetComponent<Image>().sprite = checkbox[isMarried ? "On" : "Off"];
        ModificationMenu.SetSpouseDropdownInteractability(isMarried);
    }

    public void RemoveSpouseRoom() {
        HouseModificationScriptableObject values = scriptableObjects[HouseModifications.Marriage];
        values.spriteOrigin = GetSpouseRoomOrigin();
        ApplyRenovation(HouseModifications.Marriage, false);

        spouseRoomTilemap.ClearAllTiles(); //clear spouse specific tiles
        GetComponent<EnterableBuildingComponent>().InteriorSpecialTiles.RemoveSpecialTileSet(spouse.ToString());
        InvalidTilesManager.Instance.UpdateAllCoordinates();
    }

    public void AddSpouseRoom() {
        HouseModificationScriptableObject values = scriptableObjects[HouseModifications.Marriage];
        values.spriteOrigin = GetSpouseRoomOrigin();
        ApplyRenovation(HouseModifications.Marriage, true);

        SetSpouse((int)spouse); //Refresh spouse room
    }

    public void SetSpouse(int candidate) {
        Vector3Int spouseRoomOrigin = GetSpouseRoomOrigin() + new Vector3Int(1, 1, 0);

        //clear old spouse room invalid coords
        GetComponent<EnterableBuildingComponent>().InteriorSpecialTiles.RemoveSpecialTileSet(spouse.ToString());
        MarriageCandidate oldSpouse = spouse;

        spouse = (MarriageCandidate)candidate;

        //Draw room
        Sprite spouseRoom = Resources.Load<SpriteAtlas>($"BuildingInsides/House/SpouseRoomAtlas").GetSprite(spouse.ToString());
        List<Vector3Int> area = GetRectAreaFromPoint(spouseRoomOrigin, (int)(spouseRoom.textureRect.height / 16), (int)(spouseRoom.textureRect.width / 16));

        //Delete all building in spouse room
        List<Building> overlappingBuildings = GetComponent<EnterableBuildingComponent>().GetInteriorBuildings().Where(building => building.BaseCoordinates.Intersect(area).Any()).ToList();
        List<BuildingData> buildingsDeleted = new();
        UndoRedoController.ignoreAction = true;
        foreach (Building building in overlappingBuildings) {
            BuildingData buildingData = BuildingSaverLoader.Instance.SaveBuilding(building);
            buildingsDeleted.Add(buildingData);
            building.DeleteBuilding();
        }
        UndoRedoController.ignoreAction = false;
        spouseRoomTilemap.SetTiles(area.ToArray(), SplitSprite(spouseRoom));

        SpecialCoordinateRect spouseSpecialTileSet = GetSpecialCoordinateSet(spouse.ToString());
        spouseSpecialTileSet.AddOffset(spouseRoomOrigin);
        GetComponent<EnterableBuildingComponent>().InteriorSpecialTiles.AddSpecialTileSet(spouseSpecialTileSet);
        InvalidTilesManager.Instance.UpdateAllCoordinates();
        UndoRedoController.AddActionToLog(new SpouseChangeRecord(ModificationMenu, (oldSpouse, spouse), buildingsDeleted));
    }

    private Vector3Int GetSpouseRoomOrigin() {
        return GetComponent<TieredBuildingComponent>().Tier switch {
            2 => new Vector3Int(29, 1, 0),
            3 => new Vector3Int(34, 4, 0),
            4 => new Vector3Int(34, 4, 0),
            _ => new Vector3Int(0, 0, 0),
        };
    }

    public void CheckRenovationPreconditionsAndApply(HouseModifications modification, bool isOn) {
        if (GetComponent<TieredBuildingComponent>().Tier < 3 && isOn) {
            NotificationManager.Instance.SendNotification("You need to upgrade your house to at least tier 3 to renovate it", NotificationManager.Icons.ErrorIcon);
            return;
        }

        // Debug.Log($"BuildingInsides/House/{modification}");
        HouseModificationScriptableObject renovationAttributes = scriptableObjects[modification];

        if (renovationAttributes == null) return; //renovation not found

        //Ensure any required preexisting modifications are active
        if (renovationAttributes.preexistingModification != HouseModifications.Null && (!houseModificationsActive[renovationAttributes.preexistingModification] ?? false)) {
            NotificationManager.Instance.SendNotification($"You need to have {System.Text.RegularExpressions.Regex.Replace(renovationAttributes.preexistingModification.ToString(), "(?<!^)([A-Z])", " $1")} to apply {System.Text.RegularExpressions.Regex.Replace(modification.ToString(), "(?<!^)([A-Z])", " $1")}", NotificationManager.Icons.ErrorIcon);
            Debug.Log($"You need to have {System.Text.RegularExpressions.Regex.Replace(renovationAttributes.preexistingModification.ToString(), "(?<!^)([A-Z])", " $1")} to apply {System.Text.RegularExpressions.Regex.Replace(modification.ToString(), "(?<!^)([A-Z])", " $1")}");
            return;
        }

        //Check no buildings interfere
        List<Vector3Int> coordinatesAffectedByRenovation = GetRectAreaFromPoint(renovationAttributes.spriteOrigin, (int)(renovationAttributes.backSprite.textureRect.height / 16), (int)(renovationAttributes.backSprite.textureRect.width / 16));

        SpecialCoordinateRect modificationSpecialTiles = GetSpecialCoordinateSet(renovationAttributes.type.ToString());
        modificationSpecialTiles.AddOffset(renovationAttributes.spriteOrigin);
        List<Vector3Int> coordinatesThatWillNowBeInvalid = modificationSpecialTiles.GetSpecialCoordinates().ToList().Where(position => position.type == TileType.Invalid).ToList().ConvertAll(position => position.position);

        List<Building> overlappingBuildings;
        if (isOn) overlappingBuildings = GetComponent<EnterableBuildingComponent>().GetInteriorBuildings().Where(building => building.BaseCoordinates.Intersect(coordinatesAffectedByRenovation).Any()).ToList();
        else overlappingBuildings = GetComponent<EnterableBuildingComponent>().GetInteriorBuildings().Where(building => building.BaseCoordinates.Intersect(coordinatesAffectedByRenovation.Except(coordinatesThatWillNowBeInvalid)).Any()).ToList();

        if (overlappingBuildings.Any())
            WarnForBuildingsInterferingWithRenovation(modification, overlappingBuildings, isOn);
        else {
            UndoRedoController.ignoreAction = true; //to ignore any wall changes
            ApplyRenovation(modification, isOn);
            UndoRedoController.ignoreAction = false;
            if (!isLoadingDefaultRenovations) UndoRedoController.AddActionToLog(new HouseRenovationRecord(ModificationMenu, modification, isOn));
        }
    }

    private void WarnForBuildingsInterferingWithRenovation(HouseModifications modification, IEnumerable<Building> overlappingBuildings, bool isOn) {
        GameObject warning = Instantiate(modificationWarning, GetCanvasGameObject().transform);
        warning.SetActive(true);
        warning.transform.GetChild(0).GetComponent<TMP_Text>().text = $"It seems {System.Text.RegularExpressions.Regex.Replace(modification.ToString(), "(?<!^)([A - Z])", " $1")} interferes with some buildings, delete all interfering buildings?";
        warning.transform.GetChild(1).Find("Yes").GetComponent<Button>().onClick.AddListener(() => {
            UndoRedoController.ignoreAction = true;
            List<BuildingData> buildingsDeleted = new();
            foreach (Building building in overlappingBuildings) {
                buildingsDeleted.Add(BuildingSaverLoader.Instance.SaveBuilding(building));
                building.DeleteBuilding();
            }
            overlapTilemap.ClearAllTiles();
            ApplyRenovation(modification, isOn);
            UndoRedoController.ignoreAction = false;
            if (!isLoadingDefaultRenovations) UndoRedoController.AddActionToLog(new HouseRenovationRecord(ModificationMenu, modification, isOn, buildingsDeleted));
            Destroy(warning);
        });
        warning.transform.GetChild(1).Find("Cancel").GetComponent<Button>().onClick.AddListener(() => {
            overlapTilemap.ClearAllTiles();
            Destroy(warning);
        });
        warning.transform.GetChild(1).Find("Highlight").GetComponent<Button>().onClick.AddListener(() => {
            Tile redTile = InvalidTilesManager.Instance.RedTileSprite;
            warning.transform.position = new Vector3(Screen.width / 2, warning.GetComponent<RectTransform>().rect.height / 2 + 50, 0);
            ModificationMenu.GetComponent<MoveablePanel>().SetPanelToClosedPosition();
            HouseModificationScriptableObject renovationAttributes = scriptableObjects[modification];
            List<Vector3Int> coordinatesAffectedByRenovation = GetRectAreaFromPoint(renovationAttributes.spriteOrigin, (int)(renovationAttributes.backSprite.textureRect.height / 16), (int)(renovationAttributes.backSprite.textureRect.width / 16));
            GetCamera().GetComponent<CameraController>().SetPosition(GetMiddleOfCoordinates(coordinatesAffectedByRenovation.ToArray()));
            foreach (Building building in overlappingBuildings) foreach (Vector3Int position in building.BaseCoordinates) overlapTilemap.SetTile(position, redTile);
        });
    }

    private void ApplyRenovation(HouseModifications modification, bool isOn) { //dining room off 
        HouseModificationScriptableObject values = scriptableObjects[modification];

        //Disable renovations that are dependent on this one
        foreach (HouseModifications houseModification in Enum.GetValues(typeof(HouseModifications))) {
            if (houseModification == HouseModifications.Null || houseModification == HouseModifications.Marriage) continue;
            HouseModificationScriptableObject otherModification = scriptableObjects[houseModification];
            if (otherModification.preexistingModification == modification && !isOn) { // if you turn off a modification that is a preexisting modification of another, turn off that other modification
                ModificationMenu.GetModificationToggle(houseModification).isOn = false;
                ModificationMenu.SetSpriteToOff(houseModification);
                houseModificationsActive[houseModification] = null;
            }
        }

        List<Vector3Int> positions = GetRectAreaFromPoint(values.spriteOrigin, (int)(values.backSprite.textureRect.height / 16), (int)(values.backSprite.textureRect.width / 16));
        // UndoRedoController.ignoreAction = true;
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
            if (values.frontSprite != null) SetTilesOnlyNonNull(SplitSprite(values.frontSprite), positions.ToArray(), frontTilemap);
            foreach (WallMove wallMover in values.wallModifications) GetComponent<WallsComponent>().MoveWall(wallMover.oldWallPoint, wallMover.newWall);
            foreach (FlooringMove flooringMove in values.floorModifications) GetComponent<FlooringComponent>().MoveFloor(flooringMove.oldFlooringPoint, flooringMove.newFlooring);
            foreach (WallOrigin origin in values.wallOrigins) GetComponent<WallsComponent>().AddWall(origin);
            foreach (FlooringOrigin origin in values.floorOrigins) GetComponent<FlooringComponent>().AddFloor(origin);
        }
        else {
            GetComponent<EnterableBuildingComponent>().InteriorSpecialTiles.RemoveSpecialTileSet(values.type.ToString());
            SetTilesOnlyNonNull(SplitSprite(values.backRemoved), positions.ToArray(), BuildingInteriorTilemap);
            foreach (Vector3Int position in positions) frontTilemap.SetTile(position, null);
            foreach (WallOrigin origin in values.wallOrigins) GetComponent<WallsComponent>().RemoveWall(origin.lowerLeftCorner);
            foreach (FlooringOrigin origin in values.floorOrigins) GetComponent<FlooringComponent>().RemoveFloor(origin.lowerLeftCorner);
            foreach (WallMove wallMover in values.wallModifications) GetComponent<WallsComponent>().MoveWall(wallMover.oldWallPoint, wallMover.GetReverseModification());
            foreach (FlooringMove flooringMove in values.floorModifications) GetComponent<FlooringComponent>().MoveFloor(flooringMove.oldFlooringPoint, flooringMove.GetReverseModification());
        }

        GetComponent<EnterableBuildingComponent>().BuildingInterior.GetComponent<Tilemap>().CompressBounds();
        if (values.reverseActivation) ModificationMenu.GetModificationSprite(modification).sprite = checkbox[isOn ? "Off" : "On"];
        else ModificationMenu.GetModificationSprite(modification).sprite = checkbox[isOn ? "On" : "Off"];
        houseModificationsActive[values.type] = isOn;
        ResolveTileConflicts(values.type, isOn);
        InvalidTilesManager.Instance.UpdateAllCoordinates();
        if (BuildingController.CurrentTilemapTransform == GetComponent<EnterableBuildingComponent>().BuildingInterior.transform) BuildingController.SetCurrentTilemapTransform(GetComponent<EnterableBuildingComponent>().BuildingInterior.transform); //if user inside the buildiing update the camera bounds
    }

    //Sometimes special tile are needed to bridge renovations together
    private void ResolveTileConflicts(HouseModifications changedMofification, bool isOn) {
        if (changedMofification == HouseModifications.DiningRoom && isOn) {
            if (houseModificationsActive[HouseModifications.OpenDiningRoom] == null) {
                ModificationMenu.GetModificationToggle(HouseModifications.OpenDiningRoom).isOn = true; //if you have dining room, this need to reactivate
            }
        }


        bool marriedStatuesOrCornerRoomChanged = changedMofification == HouseModifications.Marriage || changedMofification == HouseModifications.CornerRoom;

        if (marriedStatuesOrCornerRoomChanged) {
            string path;
            if ((houseModificationsActive[HouseModifications.CornerRoom] ?? false) && isMarried) path = "BuildingInsides/House/CornerRoomMarriageConflict";
            else if (isMarried) path = "BuildingInsides/House/CornerRoomRemovedMarriageOn";
            else if (houseModificationsActive[HouseModifications.CornerRoom] ?? false) path = "BuildingInsides/House/CornerRoomMarriageOff";
            else return;
            Sprite tileTexture = Resources.Load<Sprite>(path);
            Debug.Assert(tileTexture != null, "Conflict tile is null");
            Tile tile = SpriteToTile(tileTexture);
            BuildingInteriorTilemap.SetTile(new Vector3Int(34, 14, 0), tile);
            Resources.UnloadAsset(tileTexture);
        }

        if (changedMofification == HouseModifications.Attic && !isOn) {
            if (houseModificationsActive[HouseModifications.RemoveCrib] ?? false) {
                //Removing the attic clips the crib a bit so just re-add it to fix it
                ModificationMenu.GetModificationToggle(HouseModifications.RemoveCrib).isOn = false;
                ModificationMenu.GetModificationToggle(HouseModifications.RemoveCrib).isOn = true;
            }
        }



    }


    public List<MaterialCostEntry> GetMaterialsNeeded() {
        List<MaterialCostEntry> totalMaterials = new();
        foreach (HouseModifications modification in Enum.GetValues(typeof(HouseModifications))) {
            if (!houseModificationsActive[modification] ?? true) continue;
            switch (modification) {
                case HouseModifications.RemoveCrib:
                    break;
                case HouseModifications.OpenBedroom:
                    totalMaterials.Add(new(10_000, Materials.Coins));
                    break;
                case HouseModifications.SouthernRoom:
                    totalMaterials.Add(new(30_000, Materials.Coins));
                    break;
                case HouseModifications.CornerRoom:
                    totalMaterials.Add(new(20_000, Materials.Coins));
                    break;
                case HouseModifications.ExpandedCornerRoom:
                    totalMaterials.Add(new(100_000, Materials.Coins));
                    break;
                case HouseModifications.Attic:
                    totalMaterials.Add(new(60_000, Materials.Coins));
                    break;
                case HouseModifications.Cubby:
                    totalMaterials.Add(new(10_000, Materials.Coins));
                    break;
                case HouseModifications.DiningRoom:
                    totalMaterials.Add(new(150_000, Materials.Coins));
                    break;
                case HouseModifications.OpenDiningRoom:
                    totalMaterials.Add(new(10_000, Materials.Coins));
                    break;
            }
        }
        return totalMaterials;
    }

    public override void Load(BuildingScriptableObject bso) {
        hasExtensions = bso.hasInteriorExtensions;
        hasFlooringOrWallpapers = bso.interiorFlooring.Length > 0 || bso.interiorWalls.Length > 0;

        if (hasExtensions || hasFlooringOrWallpapers)
            GetComponent<EnterableBuildingComponent>().InteriorUpdated += CreateModificationMenu;


        checkbox.Add("On", Resources.Load<Sprite>("UI/CheckBoxOn"));
        checkbox.Add("Off", Resources.Load<Sprite>("UI/CheckBoxOff"));
        houseModificationsActive = new();
        scriptableObjects = new();
        foreach (HouseModifications modification in Enum.GetValues(typeof(HouseModifications)))
            houseModificationsActive.Add(modification, null);

        if (scriptableObjects.Count == 0)
            foreach (HouseModifications modification in Enum.GetValues(typeof(HouseModifications)))
                if (modification != HouseModifications.Null) scriptableObjects.Add(modification, Resources.Load<HouseModificationScriptableObject>($"BuildingInsides/House/{modification}"));

        GetComponent<EnterableBuildingComponent>().InteriorUpdated += AddTilemaps;
        GetComponent<EnterableBuildingComponent>().InteriorUpdated += BuildingTierChange;

        modificationWarning = Resources.Load<GameObject>("UI/ModificationWarning");
    }

    public override void Load(ComponentData data) {
        foreach (JProperty property in data.GetAllComponentDataProperties()) {
            HouseModifications type = Enum.Parse<HouseModifications>(property.Name);
            bool? status = property.Value.Value<bool?>();
            if (status ?? false) ApplyRenovation(type, status ?? false);
        }
    }

    public override ComponentData Save() {
        ComponentData data = new(typeof(HouseExtensionsComponent));
        foreach (HouseModifications modification in Enum.GetValues(typeof(HouseModifications))) {
            if (modification == HouseModifications.Null) continue;
            data.AddProperty(new JProperty(modification.ToString(), houseModificationsActive[modification]));
        }
        return data;

    }
}

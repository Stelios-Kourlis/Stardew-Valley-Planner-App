using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Utility.ClassManager;
using static Utility.BuildingManager;
using static Utility.TilemapManager;
using static Utility.SpriteManager;
using UnityEngine.Tilemaps;
using System.Linq;
using UnityEngine.UI;
using System;
using System.Threading.Tasks;
using UnityEngine.U2D;

[Serializable]
public class Building : TooltipableGameObject {
    public static readonly Color SEMI_TRANSPARENT = new(1, 1, 1, 0.5f);
    public static readonly Color SEMI_TRANSPARENT_INVALID = new(1, 0.5f, 0.5f, 0.5f);
    public static readonly Color OPAQUE = new(1, 1, 1, 1);
    public override string TooltipMessage {
        get {
            string tooltip = BuildingName;
            if (TryGetComponent(out InteractableBuildingComponent _)) tooltip += "\nRight Click to Interact";
            return tooltip;
        }
    }

    public enum BuildingState {
        PLACED,
        NOT_PLACED,
        PICKED_UP,
    }

    [field: SerializeField] public BuildingState CurrentBuildingState { get; protected set; } = BuildingState.NOT_PLACED;

    [field: SerializeField] public string BuildingName { get; protected set; }
    public string FullName {
        get {
            string fullName = "";
            if (TryGetComponent(out TieredBuildingComponent tieredBuildingComponent)) fullName += "Tier " + tieredBuildingComponent.Tier + " ";
            if (TryGetComponent(out MultipleTypeBuildingComponent multipleTypeBuildingComponent)) fullName += multipleTypeBuildingComponent.CurrentType + " ";
            fullName += BuildingName;
            return fullName;
        }
    }
    public BuildingType type;
    public List<MaterialCostEntry> materialsNeeded;

    [field: SerializeField] public Vector3Int Base { get; protected set; }
    public Vector3Int[] SpriteCoordinates => GetRectAreaFromPoint(Base, Height, Width).ToArray();
    public Vector3Int[] BaseCoordinates {
        get {
            if (BaseHeight == 0) return new Vector3Int[] { Base };
            return GetRectAreaFromPoint(Base, BaseHeight, BaseWidth == 0 ? Width : BaseWidth).ToArray();
        }
    }

    public int Height => Sprite != null ? (int)Sprite.textureRect.height / 16 : -1;
    public int Width => Sprite != null ? (int)Sprite.textureRect.width / 16 : -1;


    public int BaseWidth { get; protected set; } = 0;
    public int BaseHeight { get; protected set; } = 0;

    public Sprite Sprite { get; protected set; } = null;
    public SpriteAtlas Atlas { get; private set; }
    public Sprite DefaultSprite { get; private set; }
    public bool CanBeMassPlaced { get; protected set; } = false;
    public BuildingData buildingData;
    private BuildingBehaviourExtension behaviourExtension;

    public Tilemap Tilemap => gameObject.GetComponent<Tilemap>();
    public TilemapRenderer TilemapRenderer => gameObject.GetComponent<TilemapRenderer>();
    public Transform Transform => gameObject.transform;
    public GameObject BuildingGameObject => gameObject;

    private bool canBeDeleted, canBePickedUp;


    public Action<Vector3Int> BuildingPlaced { get; set; }
    public Action BuildingRemoved { get; set; }
    public Action BuildingPickedUp { get; set; }

    public override bool ShowTooltipCondition() {
        if (gameObject == null) return false;
        return IsMouseCurrentlyOverBuilding(this);
    }

    public void OnDestroy() {
        Destroy(gameObject.GetComponent<InteractableBuildingComponent>());
        behaviourExtension?.OnDestroy();
    }

    public void DeleteBuilding(bool force = false) {
        if (!canBeDeleted && !force) return; //Greenhouse and House shouldnt be deleted except loading a new farm

        behaviourExtension?.OnDelete();

        if (CurrentBuildingState == BuildingState.PLACED) {
            BuildingController.buildingGameObjects.Remove(gameObject);
            BuildingController.buildings.Remove(this);
            InvalidTilesManager.Instance.CurrentCoordinateSet.RemoveSpecialTileSet($"{BuildingName}{Base}");
            UndoRedoController.AddActionToLog(new BuildingDeleteRecord(BuildingSaverLoader.Instance.SaveBuilding(this)));
            if (TryGetComponent(out InteractableBuildingComponent component)) Destroy(component.ButtonParentGameObject);
        }
        BuildingRemoved?.Invoke();
        BuildingController.anyBuildingPositionChanged?.Invoke();

        Destroy(TooltipGameObject);
        Destroy(gameObject);
    }

    public void DoBuildingPreview() {
        switch (BuildingController.CurrentAction) {
            // case Actions.PLACE: //Place is handled diffrently
            //     PlaceBuildingPreview(GetMousePositionInTilemap());
            //     break;
            case Actions.PICKUP:
                PickupBuildingPreview();
                break;
            case Actions.DELETE:
                DeleteBuildingPreview();
                break;
        }
        behaviourExtension?.OnMouseEnter();
    }


    public List<MaterialCostEntry> GetMaterialsNeeded() {
        if (behaviourExtension?.DiffrentMaterialCost(out List<MaterialCostEntry> alternativeMaterials) ?? false) return alternativeMaterials;
        List<MaterialCostEntry> totalCost = new();
        if (materialsNeeded != null) totalCost.AddRange(materialsNeeded);
        if (TryGetComponent(out TieredBuildingComponent tieredBuildingComponent)) totalCost.AddRange(tieredBuildingComponent.GetMaterialsNeeded());
        if (TryGetComponent(out MultipleTypeBuildingComponent multipleTypeBuildingComponent)) totalCost.AddRange(multipleTypeBuildingComponent.GetMaterialsNeeded());
        if (TryGetComponent(out AnimalHouseComponent animalHouseComponent)) totalCost.AddRange(animalHouseComponent.GetMaterialsNeeded());
        if (TryGetComponent(out HouseExtensionsComponent houseExtensionsComponent)) totalCost.AddRange(houseExtensionsComponent.GetMaterialsNeeded());
        return CompressList(totalCost);
    }

    private List<MaterialCostEntry> CompressList(List<MaterialCostEntry> list) {
        List<MaterialCostEntry> compressedList = new();
        List<Materials> compressedItems = new();
        foreach (MaterialCostEntry entry in list) {
            if (compressedItems.Contains(entry.materialType)) continue;
            if (entry.IsSpecial) {
                compressedList.Add(entry);
                continue;
            }
            int totalAmount = list.Where(item => item.materialType == entry.materialType).Select(item => item.amount).Sum();
            compressedList.Add(new(totalAmount, entry.materialType));
            compressedItems.Add(entry.materialType);
        }
        return compressedList;
    }

    public void NoPreview() {
        // Debug.Log($"NoPreview on {BuildingName} (State = {CurrentBuildingState})");
        if (CurrentBuildingState == BuildingState.PLACED) Tilemap.color = OPAQUE;
        else Tilemap.ClearAllTiles();
        behaviourExtension?.NoPreview();
        behaviourExtension?.OnMouseExit();
    }

    public bool PickupBuilding() {
        if (!canBePickedUp) return false;
        Tilemap.ClearAllTiles();
        BuildingController.CurrentBuildingBeingPlaced = this;
        BuildingController.SetCurrentAction(Actions.PLACE);
        UndoRedoController.AddActionToLog(new BuildingPickupRecord(BuildingSaverLoader.Instance.SaveBuilding(this)));
        InvalidTilesManager.Instance.CurrentCoordinateSet.RemoveSpecialTileSet($"{BuildingName}{Base}");
        buildingData = BuildingSaverLoader.Instance.SaveBuilding(this);
        CurrentBuildingState = BuildingState.PICKED_UP;
        BuildingPickedUp?.Invoke();
        BuildingController.anyBuildingPositionChanged?.Invoke();
        behaviourExtension?.OnPickup();
        PlaceBuildingPreview(GetMousePositionInTilemap());
        return true;
    }

    public void PickupBuildingPreview() {
        if (CurrentBuildingState != BuildingState.PLACED) {
            Tilemap.ClearAllTiles();
            return;
        }
        Tilemap.color = SEMI_TRANSPARENT;
        behaviourExtension?.OnPickupPreview();
    }

    /// <summary>
    /// Attempts to place the building at the given position
    /// </summary>
    /// <returns> Whether the placement succeded or not </returns>
    public bool PlaceBuilding(Vector3Int position) {
        Debug.Assert(Sprite != null, $"Sprite is null for {BuildingName}");
        Base = position;
        bool canBePlacedAtPosition = BuildingCanBePlacedAtPosition(Base, this, out string errorMessage);
        if (!canBePlacedAtPosition) {
            NotificationManager.Instance.SendNotification(errorMessage, NotificationManager.Icons.ErrorIcon);
            Debug.Log($"Failed to place {BuildingName}: {errorMessage}");
            return false;
        }
        PlaceBuildingPreview(position);
        Tilemap.color = OPAQUE;

        behaviourExtension?.OnPlace(position);

        PlayParticleEffect(this, true);
        BuildingController.buildingGameObjects.Add(gameObject);
        bool wasPickedUp = BuildingState.PICKED_UP == CurrentBuildingState; //kind of ghetto but change need to happen before setting the action
        CurrentBuildingState = BuildingState.PLACED;
        if (wasPickedUp) {
            BuildingSaverLoader.Instance.LoadSavedComponents(this, buildingData);
            BuildingController.SetCurrentAction(Actions.PICKUP);
        }
        else {
            BuildingController.buildingGameObjects.Add(gameObject);
            BuildingController.buildings.Add(this);
        }

        InvalidTilesManager.Instance.CurrentCoordinateSet.AddSpecialTileSet(new($"{BuildingName}{Base}", BaseCoordinates.ToHashSet(), TileType.Invalid));
        BuildingController.LastBuildingPlaced = this;
        if (!wasPickedUp) BuildingController.CreateNewBuilding();
        UndoRedoController.AddActionToLog(new BuildingPlaceRecord(BuildingSaverLoader.Instance.SaveBuilding(this)));
        BuildingController.anyBuildingPositionChanged?.Invoke();
        BuildingPlaced?.Invoke(Base);
        return true;
    }

    /// <summary>
    /// Show a preview of the building Placement
    /// </summary
    public void PlaceBuildingPreview(Vector3Int position) {
        if (CurrentBuildingState == BuildingState.PLACED) return;
        position -= new Vector3Int(BaseWidth, 0, 0);
        TilemapRenderer.sortingOrder = -position.y + 50;

        if (BuildingCanBePlacedAtPosition(position, this, out _)) Tilemap.color = SEMI_TRANSPARENT;
        else Tilemap.color = SEMI_TRANSPARENT_INVALID;

        Tilemap.ClearAllTiles();
        Vector3Int[] mouseoverEffectArea = GetRectAreaFromPoint(position, Height, Width).ToArray();
        Tilemap.SetTiles(mouseoverEffectArea, SplitSprite(Sprite));

        behaviourExtension?.OnPlacePreview(position);
    }

    public void DeleteBuildingPreview() {
        if (CurrentBuildingState != BuildingState.PLACED) {
            Tilemap.ClearAllTiles();
            return;
        }
        Tilemap.color = SEMI_TRANSPARENT_INVALID;
        behaviourExtension?.OnDeletePreview();
    }

    public bool BuildingSpecificPlacementPreconditionsAreMet(Vector3Int position, out string errorMessage) {
        errorMessage = "";
        return behaviourExtension?.BuildingSpecificPlacementPreconditionsAreMet(position, out errorMessage) ?? true;
    }

    // public void 

    /// <summary>
    /// Update the sprite of the building
    /// </summary>
    public void UpdateTexture(Sprite newSprite) {
        Debug.Assert(newSprite != null, $"UpdateTexture called for {BuildingName} with null sprite/Called from: {new System.Diagnostics.StackTrace()}");
        // Debug.Log($"Updating texture to {newSprite.name} for {BuildingName}");
        Sprite = newSprite;
        if (CurrentBuildingState != BuildingState.PLACED) return;
        Tile[] buildingTiles = SplitSprite(Sprite);
        Tilemap.SetTiles(SpriteCoordinates.ToArray(), buildingTiles);
    }


    public static GameObject CreateBuildingButton(BuildingScriptableObject bso) {
        GameObject button = new($"{bso.BuildingName}");
        button.AddComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        button.AddComponent<Image>().sprite = bso.defaultSprite;
        button.GetComponent<Image>().preserveAspect = true;
        button.AddComponent<UIElement>();
        button.GetComponent<UIElement>().tooltipMessage = bso.BuildingName;
        button.GetComponent<UIElement>().playSounds = true;
        button.GetComponent<UIElement>().ExpandOnHover = true;

        button.AddComponent<Button>().onClick.AddListener(() => {
            BuildingController.SetCurrentBuildingType(bso.typeName);
            BuildingController.SetCurrentAction(Actions.PLACE);
        });
        return button;
    }

    public static GameObject CreateBuildingButton(BuildingType type) {
        BuildingScriptableObject bso = Resources.Load<BuildingScriptableObject>($"BuildingScriptableObjects/{type}");
        if (bso == null) {
            Debug.LogWarning($"BSO for {type} not made yet");
            return null;
        }
        GameObject button = CreateBuildingButton(bso);
        Resources.UnloadAsset(bso);
        return button;
    }

    public Building LoadFromScriptableObject(BuildingScriptableObject bso) {
        Debug.Assert(bso != null, $"BuildingScriptableObject is null");

        CurrentBuildingState = BuildingState.NOT_PLACED;
        if (!gameObject.TryGetComponent<Tilemap>(out _)) gameObject.AddComponent<Tilemap>();
        if (!gameObject.TryGetComponent<TilemapRenderer>(out _)) gameObject.AddComponent<TilemapRenderer>();

        BuildingName = bso.BuildingName;
        type = bso.typeName;
        BaseHeight = bso.baseHeight;
        BaseWidth = bso.baseWidth;
        CanBeMassPlaced = bso.canBeMassPlaced;
        materialsNeeded = bso.materialsNeeded;
        DefaultSprite = bso.defaultSprite;
        Sprite = bso.defaultSprite;
        Atlas = bso.atlas;
        canBeDeleted = bso.canBeDeleted;
        canBePickedUp = bso.canBePickedUp;

        //todo make it so maybe building has isInteractable bool to Add InteractableBuildingComponent?
        gameObject.AddComponent<InteractableBuildingComponent>().Load(bso);

        if (bso.isMultipleType) gameObject.AddComponent<MultipleTypeBuildingComponent>().Load(bso);

        if (bso.isTiered) gameObject.AddComponent<TieredBuildingComponent>().Load(bso);

        if (bso.isConnecting) gameObject.AddComponent<ConnectingBuildingComponent>().Load(bso);

        if (bso.isFishPond) gameObject.AddComponent<FishPondComponent>().Load(bso);

        if (bso.isCave) gameObject.AddComponent<CaveComponent>().Load(bso);

        if (bso.isPaintable) gameObject.AddComponent<PaintableBuildingComponent>().Load(bso);

        if (bso.isEnterable) gameObject.AddComponent<EnterableBuildingComponent>().Load(bso);

        if (bso.isAnimalHouse) gameObject.AddComponent<AnimalHouseComponent>().Load(bso);

        if (bso.hasInteriorExtensions || bso.interiorFlooring.Length > 0 || bso.interiorWalls.Length > 0)
            gameObject.AddComponent<HouseExtensionsComponent>().Load(bso);

        if (bso.extraBehaviourType.Type != null) {
            behaviourExtension = (BuildingBehaviourExtension)Activator.CreateInstance(bso.extraBehaviourType.Type);
            behaviourExtension?.OnStart(this);
        }

        return this;
    }
}

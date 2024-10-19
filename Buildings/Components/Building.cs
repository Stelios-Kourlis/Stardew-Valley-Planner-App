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
    protected readonly Color SEMI_TRANSPARENT = new(1, 1, 1, 0.5f);
    protected readonly Color SEMI_TRANSPARENT_INVALID = new(1, 0.5f, 0.5f, 0.5f);
    protected readonly Color OPAQUE = new(1, 1, 1, 1);
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
    public Vector3Int[] BaseCoordinates => GetRectAreaFromPoint(Base, BaseHeight, Width).ToArray();

    public int Height => Sprite != null ? (int)Sprite.textureRect.height / 16 : -1;

    public virtual int Width => Sprite != null ? (int)Sprite.textureRect.width / 16 : -1; //virtual because Trees have a width of 1

    public int BaseHeight { get; protected set; } = 0;

    [field: SerializeField] public Sprite Sprite { get; protected set; } = null;
    public SpriteAtlas Atlas { get; private set; }
    public Sprite DefaultSprite { get; private set; }
    [field: SerializeField] public bool CanBeMassPlaced { get; protected set; } = false;
    private BuildingBehaviourExtension behaviourExtension;

    public Tilemap Tilemap => gameObject.GetComponent<Tilemap>();
    public TilemapRenderer TilemapRenderer => gameObject.GetComponent<TilemapRenderer>();
    public Transform Transform => gameObject.transform;
    public GameObject BuildingGameObject => gameObject;


    public Action<Vector3Int> BuildingPlaced { get; set; }
    public Action BuildingRemoved { get; set; }
    public Action BuildingPickedUp { get; set; }
    protected Action HidBuildingPreview { get; set; }

    public override void OnAwake() {
        CurrentBuildingState = BuildingState.NOT_PLACED;
        if (!gameObject.TryGetComponent<Tilemap>(out _)) gameObject.AddComponent<Tilemap>();
        if (!gameObject.TryGetComponent<TilemapRenderer>(out _)) gameObject.AddComponent<TilemapRenderer>();
        if (!gameObject.TryGetComponent<BuildingSaverLoader>(out _)) gameObject.AddComponent<BuildingSaverLoader>();
        // Sprite = Resources.Load<Sprite>($"Buildings/{GetType()}");
        Sprite = DefaultSprite;

    }

    public void OnDestroy() {
        Destroy(gameObject.GetComponent<InteractableBuildingComponent>());
        behaviourExtension?.OnDestroy();
    }

    public void DeleteBuilding(bool force = false) {
        if ((type is BuildingType.Greenhouse || type is BuildingType.House) && !force) return; //Greenhouse and House shouldnt be deleted except loading a new farm

        if (this is IExtraActionBuilding extraActionBuilding) extraActionBuilding.PerformExtraActionsOnDelete();

        if (CurrentBuildingState == BuildingState.PLACED) {
            BuildingController.buildingGameObjects.Remove(gameObject);
            BuildingController.buildings.Remove(this);
            BuildingController.specialCoordinates.RemoveSpecialTileSet($"{BuildingName}{BaseCoordinates[0]}");
            UndoRedoController.AddActionToLog(new UserAction(Actions.DELETE, GetComponent<BuildingSaverLoader>().SaveBuilding()));
            if (TryGetComponent(out InteractableBuildingComponent component)) Destroy(component.ButtonParentGameObject);
        }
        BuildingRemoved?.Invoke();
        BuildingController.anyBuildingPositionChanged?.Invoke();
        behaviourExtension?.OnDelete();
        Destroy(TooltipGameObject);
        Destroy(gameObject);
    }

    public void DoBuildingPreview() {
        switch (BuildingController.CurrentAction) {
            // case Actions.PLACE: //Place is handled diffrently
            //     PlaceBuildingPreview(GetMousePositionInTilemap());
            //     break;
            case Actions.EDIT:
                PickupBuildingPreview();
                break;
            case Actions.DELETE:
                DeleteBuildingPreview();
                break;
        }
    }


    public List<MaterialCostEntry> GetMaterialsNeeded() {
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

    public override void OnUpdate() {
        // if (this is Shed) Debug.Log($"{IsPickedUp.Item1} ({BaseCoordinates?[0].x}, {BaseCoordinates?[0].y})");
    }

    public void NoPreview() {
        // Debug.Log($"NoPreview on {BuildingName} (State = {CurrentBuildingState})");
        if (CurrentBuildingState == BuildingState.PLACED) Tilemap.color = OPAQUE;
        else Tilemap.ClearAllTiles();
        HidBuildingPreview?.Invoke();
        behaviourExtension?.NoPreview();
    }

    public bool PickupBuilding() {
        Tilemap.ClearAllTiles();
        BuildingController.CurrentBuildingBeingPlaced = this;
        BuildingController.SetCurrentAction(Actions.PLACE);
        UndoRedoController.AddActionToLog(new UserAction(Actions.EDIT, GetComponent<BuildingSaverLoader>().SaveBuilding()));
        BuildingController.specialCoordinates.RemoveSpecialTileSet($"{BuildingName}{BaseCoordinates[0]}");
        CurrentBuildingState = BuildingState.PICKED_UP;
        if (this is IExtraActionBuilding extraActionBuilding) extraActionBuilding.PerformExtraActionsOnPickup();
        BuildingPickedUp?.Invoke();
        BuildingController.anyBuildingPositionChanged?.Invoke();
        behaviourExtension?.OnPickup();
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
        (bool canBePlacedAtPosition, string errorMessage) = BuildingCanBePlacedAtPosition(position, this);
        if (!canBePlacedAtPosition) {
            NotificationManager.Instance.SendNotification(errorMessage, NotificationManager.Icons.ErrorIcon);
            Debug.Log($"Failed to place {BuildingName}: {errorMessage}");
            return false;
        }
        PlaceBuildingPreview(position);
        Tilemap.color = OPAQUE;
        Base = position;

        behaviourExtension?.OnPlace(position);

        PlayParticleEffect(this, true);
        BuildingController.buildingGameObjects.Add(gameObject);
        // BuildingController.buildings.Add(this);
        bool wasPickedUp = BuildingState.PICKED_UP == CurrentBuildingState; //kind of ghetto but change need to happen before setting the action
        CurrentBuildingState = BuildingState.PLACED;
        if (wasPickedUp) {
            GetComponent<BuildingSaverLoader>().LoadSavedComponents();
            BuildingController.SetCurrentAction(Actions.EDIT); //todo add BuildingData loading
        }
        else {
            BuildingController.buildingGameObjects.Add(gameObject);
            BuildingController.buildings.Add(this);
        }

        // Debug.Log($"Adding special coords for {BuildingName}, width: {Width}, BHeight: {BaseHeight}");
        BuildingController.specialCoordinates.AddSpecialTileSet(new($"{BuildingName}{BaseCoordinates[0]}", BaseCoordinates.ToHashSet(), TileType.Invalid));
        // Debug.Log($"added {BaseCoordinates.Length} coordinates to unavailable coordinates");
        if (!wasPickedUp) BuildingController.CreateNewBuilding();
        UndoRedoController.AddActionToLog(new UserAction(Actions.PLACE, GetComponent<BuildingSaverLoader>().SaveBuilding()));
        BuildingController.anyBuildingPositionChanged?.Invoke();
        BuildingPlaced?.Invoke(BaseCoordinates[0]);
        // if (BuildingPlaced != null) {
        //     foreach (Func<Vector3Int, Task> handler in BuildingPlaced.GetInvocationList().Cast<Func<Vector3Int, Task>>()) {
        //         await handler(position);
        //     }
        // }
        // Debug.Log($"Placed buildng {BuildingName}");
        return true;
    }

    /// <summary>
    /// Show a preview of the building Placement
    /// </summary
    public void PlaceBuildingPreview(Vector3Int position) {
        if (CurrentBuildingState == BuildingState.PLACED) return;
        TilemapRenderer.sortingOrder = -position.y + 50;

        if (BuildingCanBePlacedAtPosition(position, this).Item1) Tilemap.color = SEMI_TRANSPARENT;
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
        GameObject button = new($"{bso.buildingName}");
        button.AddComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        button.AddComponent<Image>().sprite = bso.defaultSprite;
        button.GetComponent<Image>().preserveAspect = true;
        button.AddComponent<UIElement>();
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
        BuildingName = bso.buildingName;
        type = bso.typeName;
        BaseHeight = bso.baseHeight;
        CanBeMassPlaced = bso.canBeMassPlaced;
        materialsNeeded = bso.materialsNeeded;
        DefaultSprite = bso.defaultSprite;
        Sprite = bso.defaultSprite;
        Atlas = bso.atlas;

        if (bso.isMultipleType) gameObject.AddComponent<MultipleTypeBuildingComponent>().Load(bso);

        if (bso.isTiered) gameObject.AddComponent<TieredBuildingComponent>().Load(bso);

        if (bso.isAnimalHouse) gameObject.AddComponent<AnimalHouseComponent>().Load(bso);

        if (bso.isConnecting) gameObject.AddComponent<ConnectingBuildingComponent>().Load(bso);

        if (bso.isFishPond) gameObject.AddComponent<FishPondComponent>();

        if (bso.isEnterable) gameObject.AddComponent<EnterableBuildingComponent>().Load(bso);

        if (bso.hasInteriorExtensions) gameObject.AddComponent<HouseExtensionsComponent>();

        if (bso.extraBehaviourScript != null) {
            behaviourExtension = (BuildingBehaviourExtension)Activator.CreateInstance(bso.extraBehaviourScript.GetClass());
            behaviourExtension?.OnStart(this);
        }

        return this;
    }
}

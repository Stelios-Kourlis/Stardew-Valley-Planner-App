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

public abstract class Building : TooltipableGameObject {
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

    [field: SerializeField] public Vector3Int[] SpriteCoordinates { get; private set; }

    [field: SerializeField] public Vector3Int[] BaseCoordinates { get; protected set; }

    public int Height => Sprite != null ? (int)Sprite.textureRect.height / 16 : -1;

    public virtual int Width => Sprite != null ? (int)Sprite.textureRect.width / 16 : -1; //virtual because Trees have a width of 1

    public int BaseHeight { get; protected set; } = 0;

    [field: SerializeField] public Sprite Sprite { get; protected set; } = null;

    public Tilemap Tilemap => gameObject.GetComponent<Tilemap>();

    public TilemapRenderer TilemapRenderer => gameObject.GetComponent<TilemapRenderer>();

    public Transform Transform => gameObject.transform;

    public GameObject BuildingGameObject => gameObject;

    public Action BuildingPlaced { get; set; }
    public Action BuildingRemoved { get; set; }
    [field: SerializeField] public bool CanBeMassPlaced { get; protected set; } = false;

    public override void OnAwake() {
        CurrentBuildingState = BuildingState.NOT_PLACED;
        gameObject.AddComponent<Tilemap>();
        gameObject.AddComponent<TilemapRenderer>();
        gameObject.AddComponent<BuildingSaverLoader>();
        Sprite = Resources.Load<Sprite>($"Buildings/{GetType()}");
        // Debug.Assert(Sprite != null, $"Sprite 'Buildings/{GetType()}' not found");
    }

    public void OnDestroy() {
        Destroy(gameObject.GetComponent<InteractableBuildingComponent>());
    }

    public void DeleteBuilding(bool force = false) {
        if ((this is Greenhouse || this is House) && !force) return; //Greenhouse and House can't be deleted

        if (this is IExtraActionBuilding extraActionBuilding) extraActionBuilding.PerformExtraActionsOnDelete();

        if (CurrentBuildingState == BuildingState.PLACED) {
            BuildingController.buildingGameObjects.Remove(gameObject);
            BuildingController.buildings.Remove(this);
            BuildingController.GetUnavailableCoordinates().RemoveWhere(x => BaseCoordinates.Contains(x));
            UndoRedoController.AddActionToLog(new UserAction(Actions.DELETE, GetComponent<BuildingSaverLoader>().SaveBuilding()));
            if (TryGetComponent(out InteractableBuildingComponent component)) Destroy(component.ButtonParentGameObject);
        }
        BuildingRemoved?.Invoke();

        // Debug.Log("Destryong building " + BuildingName);
        BuildingController.anyBuildingPositionChanged?.Invoke();
        Destroy(TooltipGameObject);
        Destroy(gameObject);
    }

    public void DoBuildingPreview() {
        switch (BuildingController.CurrentAction) {
            case Actions.EDIT:
                PickupBuildingPreview();
                break;
            case Actions.DELETE:
                DeleteBuildingPreview();
                break;
        }
    }

    public void StopBuildingPreview() {
        Tilemap.color = OPAQUE;
    }

    public abstract List<MaterialCostEntry> GetMaterialsNeeded();

    public override void OnUpdate() {
        // if (this is Shed) Debug.Log($"{IsPickedUp.Item1} ({BaseCoordinates?[0].x}, {BaseCoordinates?[0].y})");
    }

    public void NoPreview() {
        Debug.Log("NoPreview");
        if (CurrentBuildingState == BuildingState.PLACED) Tilemap.color = OPAQUE;
        else Tilemap.ClearAllTiles();
    }

    public bool PickupBuilding() {
        Tilemap.ClearAllTiles();
        BuildingController.CurrentBuildingBeingPlaced = this;
        BuildingController.SetCurrentAction(Actions.PLACE);
        UndoRedoController.AddActionToLog(new UserAction(Actions.EDIT, GetComponent<BuildingSaverLoader>().SaveBuilding()));
        BuildingController.GetUnavailableCoordinates().RemoveWhere(x => BaseCoordinates.Contains(x));
        CurrentBuildingState = BuildingState.PICKED_UP;
        BuildingController.anyBuildingPositionChanged?.Invoke();
        return true;
    }

    public void PickupBuildingPreview() {
        if (CurrentBuildingState != BuildingState.PLACED) {
            Tilemap.ClearAllTiles();
            return;
        }
        Tilemap.color = SEMI_TRANSPARENT;
        if (this is IExtraActionBuilding extraActionBuilding) extraActionBuilding.PerformExtraActionsOnPickupPreview();
    }

    /// <summary>
    /// Attempts to place the building at the given position
    /// </summary>
    /// <returns> Whether the placement succeded or not </returns>
    public bool PlaceBuilding(Vector3Int position) {
        Debug.Assert(Sprite != null, $"Sprite is null for {BuildingName}");
        (bool canBePlacedAtPosition, string errorMessage) = BuildingCanBePlacedAtPosition(position, this);
        if (!canBePlacedAtPosition) { GetNotificationManager().SendNotification(errorMessage, NotificationManager.Icons.ErrorIcon); return false; }
        PlaceBuildingPreview(position);
        Tilemap.color = OPAQUE;

        BaseCoordinates = GetAreaAroundPosition(position, BaseHeight, Width).ToArray();
        SpriteCoordinates = GetAreaAroundPosition(position, Height, Width).ToArray();
        if (this is IExtraActionBuilding extraActionBuilding) {
            extraActionBuilding.PerformExtraActionsOnPlace(position);
            // if (IsPickedUp.Item1) extraActionBuilding.LoadExtraBuildingData(IsPickedUp.Item2.extraData);
        }

        PlayParticleEffect(this, true);
        BuildingController.buildingGameObjects.Add(gameObject);
        BuildingController.buildings.Add(this);
        if (CurrentBuildingState == BuildingState.PICKED_UP) BuildingController.SetCurrentAction(Actions.EDIT); //todo add BuildingData loading
        CurrentBuildingState = BuildingState.PLACED;
        BuildingController.buildingGameObjects.Add(gameObject);
        BuildingController.buildings.Add(this);
        BuildingController.GetUnavailableCoordinates().UnionWith(BaseCoordinates);
        // Debug.Log($"added {BaseCoordinates.Length} coordinates to unavailable coordinates");
        BuildingController.CreateNewBuilding();
        UndoRedoController.AddActionToLog(new UserAction(Actions.PLACE, GetComponent<BuildingSaverLoader>().SaveBuilding()));
        BuildingController.anyBuildingPositionChanged?.Invoke();
        BuildingPlaced?.Invoke();
        return true;
    }

    /// <summary>
    /// Show a preview of the building Placement
    /// </summary
    public void PlaceBuildingPreview(Vector3Int position) {
        // return;
        TilemapRenderer.sortingOrder = -position.y + 50;

        if (BuildingCanBePlacedAtPosition(position, this).Item1) Tilemap.color = SEMI_TRANSPARENT;
        else Tilemap.color = SEMI_TRANSPARENT_INVALID;

        Tilemap.ClearAllTiles();
        Vector3Int[] mouseoverEffectArea = GetAreaAroundPosition(position, Height, Width).ToArray();
        Tilemap.SetTiles(mouseoverEffectArea, SplitSprite(Sprite));

        if (this is IExtraActionBuilding extraActionBuilding) extraActionBuilding.PerformExtraActionsOnPlacePreview(position);
    }

    public void DeleteBuildingPreview() {
        if (CurrentBuildingState != BuildingState.PLACED) {
            Tilemap.ClearAllTiles();
            return;
        }
        Tilemap.color = SEMI_TRANSPARENT_INVALID;
        if (this is IExtraActionBuilding extraActionBuilding) extraActionBuilding.PerformExtraActionsOnDeletePreview();
    }

    /// <summary>
    /// Update the sprite of the building
    /// </summary>
    public void UpdateTexture(Sprite newSprite) {
        Debug.Assert(newSprite != null, $"UpdateTexture called for {GetType()} with null sprite/Called from: {new System.Diagnostics.StackTrace()}");
        // Debug.Log($"Updating texture to {newSprite.name} for {BuildingName}");
        Sprite = newSprite;
        if (CurrentBuildingState != BuildingState.PLACED) return;
        Tile[] buildingTiles = SplitSprite(Sprite);
        Tilemap.SetTiles(SpriteCoordinates.ToArray(), buildingTiles);
    }

    // public void HidePreview() {
    //     Tilemap.ClearAllTiles();
    // }

    /// <summary>
    /// Create a button that sets the current building to this building
    /// </summary>
    /// <returns>The game object of the button, with no parent, caller should use transform.SetParent()</returns>
    public virtual GameObject CreateBuildingButton() {
        GameObject button = Instantiate(Resources.Load<GameObject>("UI/BuildingButton"));
        button.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        button.name = $"{BuildingName}";
        button.GetComponent<Image>().sprite = Sprite;

        System.Type buildingType = GetType();
        button.GetComponent<Button>().onClick.AddListener(() => {
            Debug.Log($"Setting current building to {buildingType}");
            BuildingController.SetCurrentBuildingType(buildingType);
            BuildingController.SetCurrentAction(Actions.PLACE);
        });
        return button;
    }
}

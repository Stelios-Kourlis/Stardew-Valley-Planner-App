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

public abstract class Building : TooltipableGameObject, IBuilding {
    protected readonly Color SEMI_TRANSPARENT = new(1, 1, 1, 0.5f);
    protected readonly Color SEMI_TRANSPARENT_INVALID = new(1, 0.5f, 0.5f, 0.5f);
    protected readonly Color OPAQUE = new(1, 1, 1, 1);
    public override string TooltipMessage {
        get {
            string tooltip = BuildingName;
            if (this is IInteractableBuilding) tooltip += "\nRight Click to Interact";
            return tooltip;
        }
    }

    public string BuildingName { get; protected set; }

    public int UID { get; private set; } = 0;

    public Vector3Int[] SpriteCoordinates { get; private set; }

    public Vector3Int[] BaseCoordinates { get; private set; }

    public bool IsPlaced { get; private set; } = false;

    public (bool, string) IsPickedUp { get; private set; }

    public int Height => Sprite != null ? (int)Sprite.textureRect.height / 16 : 0;

    public int Width => Sprite != null ? (int)Sprite.textureRect.width / 16 : 0;

    public int BaseHeight { get; protected set; } = 0;

    public Sprite Sprite { get; protected set; } = null;

    public Tilemap Tilemap => gameObject.GetComponent<Tilemap>();

    public TilemapRenderer TilemapRenderer => gameObject.GetComponent<TilemapRenderer>();

    public Transform Transform => gameObject.transform;

    // public string[] BuildingData {
    //     get {
    //         string data = $"{GetType()}|{BaseCoordinates[0].x}|{BaseCoordinates[0].y}";
    //         if (this is IExtraActionBuilding extraActionBuilding) data += "|" + extraActionBuilding.AddToBuildingData();
    //         return data.Split('|');
    //     }
    // }

    public void DeleteBuilding(bool force = false) {
        if ((this is Greenhouse || this is House) && !force) return; //Greenhouse and House can't be deleted
        IsPlaced = false;
        Tilemap.ClearAllTiles();

        if (this is IExtraActionBuilding extraActionBuilding) extraActionBuilding.PerformExtraActionsOnDelete();

        BuildingController.buildingGameObjects.Remove(gameObject);
        BuildingController.buildings.Remove(this);
        BuildingController.GetUnavailableCoordinates().RemoveWhere(x => BaseCoordinates.Contains(x));
        BuildingController.AddActionToLog(new UserAction(Actions.DELETE, UID, GetBuildingData()));
        if (this is IInteractableBuilding interactableBuilding) Destroy(interactableBuilding.ButtonParentGameObject);
        Destroy(TooltipGameObject);
        Destroy(gameObject);
    }

    public string GetBuildingData() {
        string data = $"{GetType()}|{BaseCoordinates[0].x}|{BaseCoordinates[0].y}";
        if (this is IExtraActionBuilding extraActionBuilding) data += "|" + extraActionBuilding.AddToBuildingData();
        return data;
    }

    public abstract List<MaterialInfo> GetMaterialsNeeded();

    public bool LoadBuildingFromData(string[] data) {
        int x = int.Parse(data[0]);
        int y = int.Parse(data[1]);
        PlaceBuilding(new Vector3Int(x, y, 0));

        if (this is IExtraActionBuilding extraActionBuilding) extraActionBuilding.LoadExtraBuildingData(data.Skip(2).ToArray());

        return true; //make it so it might return false
    }

    public override void OnAwake() {
        IsPlaced = false;
        gameObject.AddComponent<Tilemap>();
        gameObject.AddComponent<TilemapRenderer>();
    }

    public override void OnUpdate() {
        return;
    }

    public bool PickupBuilding() {
        IsPlaced = false;
        IsPickedUp = (true, GetBuildingData());
        BuildingController.SetCurrentAction(Actions.PLACE);
        return true;
    }

    public void PickupBuildingPreview() {
        if (!IsPlaced) {
            Tilemap.ClearAllTiles();
            return;
        }
        if (BaseCoordinates.Contains(GetMousePositionInTilemap())) Tilemap.color = SEMI_TRANSPARENT;
        else Tilemap.color = OPAQUE;
    }

    /// <summary>
    /// Attempts to place the building at the given position
    /// </summary>
    /// <returns> Whether the placement succeded or not </returns>
    public bool PlaceBuilding(Vector3Int position) {
        (bool canBePlacedAtPosition, string errorMessage) = BuildingCanBePlacedAtPosition(position, this);
        if (!canBePlacedAtPosition) { GetNotificationManager().SendNotification(errorMessage, NotificationManager.Icons.ErrorIcon); return false; }
        Tilemap.color = OPAQUE;

        BaseCoordinates = GetAreaAroundPosition(position, BaseHeight, Width).ToArray();
        SpriteCoordinates = GetAreaAroundPosition(position, Height, Width).ToArray();
        if (this is IExtraActionBuilding extraActionBuilding) {
            extraActionBuilding.PerformExtraActionsOnPlace(position);
            if (IsPickedUp.Item1) extraActionBuilding.LoadExtraBuildingData(IsPickedUp.Item2.Split('|').Skip(3).ToArray());
        }

        PlayParticleEffect(this, true);
        IsPlaced = true;
        BuildingController.buildingGameObjects.Add(gameObject);
        BuildingController.buildings.Add(this);
        UID = (name + BaseCoordinates[0].x + BaseCoordinates[0].y).GetHashCode();
        BuildingController.AddActionToLog(new UserAction(Actions.PLACE, UID, GetBuildingData()));
        if (BuildingController.CurrentAction == Actions.PLACE) BuildingController.OnBuildingPlaced();
        return true;
    }

    /// <summary>
    /// Show a preview of the building Placement
    /// </summary
    public void PlaceBuildingPreview(Vector3Int position) {
        TilemapRenderer.sortingOrder = -position.y + 50;

        if (BuildingCanBePlacedAtPosition(position, this).Item1) Tilemap.color = SEMI_TRANSPARENT;
        else Tilemap.color = SEMI_TRANSPARENT_INVALID;

        Tilemap.ClearAllTiles();
        Vector3Int[] mouseoverEffectArea = GetAreaAroundPosition(position, Height, Width).ToArray();
        Tilemap.SetTiles(mouseoverEffectArea, SplitSprite(Sprite));

        if (this is IExtraActionBuilding extraActionBuilding) extraActionBuilding.PerformExtraActionsOnPlacePreview(position);
    }

    public void DeleteBuildingPreview() {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Update the sprite of the building
    /// </summary>
    public void UpdateTexture(Sprite newSprite) {
        // UnityEngine.Debug.Assert(newSprite != null, $"UpdateTexture called for {GetType()} with null sprite/Called from: {new StackTrace()}");
        Sprite = newSprite;
        if (!IsPlaced) return;
        Tile[] buildingTiles = SplitSprite(Sprite);
        Tilemap.SetTiles(SpriteCoordinates.ToArray(), buildingTiles);
    }

    /// <summary>
    /// Create a button that sets the current building to this building
    /// </summary>
    /// <returns>The game object of the button, with no parent, caller should use transform.SetParent()</returns>
    public virtual GameObject CreateBuildingButton() {
        GameObject button = Instantiate(Resources.Load<GameObject>("UI/BuildingButton"));
        button.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        button.name = $"{GetType()}Button";
        button.GetComponent<Image>().sprite = Sprite;

        System.Type buildingType = GetType();
        button.GetComponent<Button>().onClick.AddListener(() => {
            // Debug.Log($"Setting current building to {buildingType}");
            BuildingController.SetCurrentBuildingType(buildingType);
            BuildingController.SetCurrentAction(Actions.PLACE);
        });
        return button;
    }
}

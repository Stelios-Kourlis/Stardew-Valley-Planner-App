using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using UnityEngine.UI;
using static Utility.TilemapManager;
using static Utility.SpriteManager;
using static Utility.ClassManager;
using static Utility.BuildingManager;
using System.Linq;


public class FishPond : Building, IInteractableBuilding, IExtraActionBuilding {
    private InteractableBuildingComponent interactableBuildingComponent;
    private SpriteAtlas atlas;
    private SpriteAtlas fishAtlas;
    public Fish fish;
    private Vector3Int[] decoCoordinates;
    private GameObject decoTilemapObject;
    private int decoIndex = 0;
    private GameObject waterTilemapObject;

    public HashSet<ButtonTypes> BuildingInteractions => interactableBuildingComponent.BuildingInteractions;

    public GameObject ButtonParentGameObject => interactableBuildingComponent.ButtonParentGameObject;

    public override void OnAwake() {
        BuildingName = "Fish Pond";
        BaseHeight = 5;
        atlas = Resources.Load<SpriteAtlas>("Buildings/FishPondAtlas");
        Sprite = atlas.GetSprite("FishPond");
        base.OnAwake();
        fishAtlas = Resources.Load<SpriteAtlas>("Fish/FishAtlas");
        decoTilemapObject = CreateTilemapObject(transform, 0, "Deco");
        waterTilemapObject = CreateTilemapObject(transform, 0, "Water");
    }

    public override List<MaterialCostEntry> GetMaterialsNeeded() {
        return new List<MaterialCostEntry> {
            new(5_000, Materials.Coins),
            new(200, Materials.Wood),
            new(5, Materials.Seaweed),
            new(5, Materials.GreenAlgae)
        };
    }

    public void PerformExtraActionsOnPlace(Vector3Int position) {
        Vector3Int topRightCorner = position + new Vector3Int(0, 4, 0);
        decoCoordinates = GetAreaAroundPosition(topRightCorner, 3, 5).ToArray();
        decoTilemapObject.GetComponent<Tilemap>().ClearAllTiles();
        decoTilemapObject.GetComponent<Tilemap>().color = OPAQUE;
        decoTilemapObject.GetComponent<Tilemap>().SetTiles(decoCoordinates, SplitSprite(atlas.GetSprite($"FishDeco_{decoIndex}")));
        decoTilemapObject.GetComponent<TilemapRenderer>().sortingOrder = gameObject.GetComponent<TilemapRenderer>().sortingOrder + 1;
        waterTilemapObject.GetComponent<Tilemap>().ClearAllTiles();
        waterTilemapObject.GetComponent<Tilemap>().color = OPAQUE;
        waterTilemapObject.GetComponent<Tilemap>().SetTiles(BaseCoordinates, SplitSprite(atlas.GetSprite("FishPondBottom")));
        waterTilemapObject.GetComponent<TilemapRenderer>().sortingOrder = gameObject.GetComponent<TilemapRenderer>().sortingOrder - 1;
    }

    private void PerformExtraActionsOnPickup() {
        decoTilemapObject.GetComponent<Tilemap>().ClearAllTiles();
        waterTilemapObject.GetComponent<Tilemap>().ClearAllTiles();
    }

    protected void PerformExtraActionsOnPlacePreview(Vector3Int position) {
        Vector3Int topRightCorner = position + new Vector3Int(0, 4, 0);
        decoCoordinates = GetAreaAroundPosition(topRightCorner, 3, 5).ToArray();
        decoTilemapObject.GetComponent<Tilemap>().ClearAllTiles();
        Vector3Int[] unavailableCoordinates = BuildingController.GetUnavailableCoordinates().ToArray();
        Vector3Int[] buildingBaseCoordinates = GetAreaAroundPosition(position, BaseHeight, Width).ToArray();
        if (unavailableCoordinates.Intersect(buildingBaseCoordinates).Count() != 0) decoTilemapObject.GetComponent<Tilemap>().color = SEMI_TRANSPARENT_INVALID;
        else decoTilemapObject.GetComponent<Tilemap>().color = SEMI_TRANSPARENT;
        decoTilemapObject.GetComponent<Tilemap>().SetTiles(decoCoordinates, SplitSprite(atlas.GetSprite($"FishDeco_{decoIndex}")));
        decoTilemapObject.GetComponent<TilemapRenderer>().sortingOrder = gameObject.GetComponent<TilemapRenderer>().sortingOrder + 1;
        waterTilemapObject.GetComponent<Tilemap>().ClearAllTiles();
        if (unavailableCoordinates.Intersect(buildingBaseCoordinates).Count() != 0) waterTilemapObject.GetComponent<Tilemap>().color = SEMI_TRANSPARENT_INVALID;
        else waterTilemapObject.GetComponent<Tilemap>().color = SEMI_TRANSPARENT;
        waterTilemapObject.GetComponent<Tilemap>().SetTiles(GetAreaAroundPosition(position, Height, Width).ToArray(), SplitSprite(atlas.GetSprite("FishPondBottom")));
        waterTilemapObject.GetComponent<TilemapRenderer>().sortingOrder = gameObject.GetComponent<TilemapRenderer>().sortingOrder - 1;
    }

    protected void PerformExtraActionsOnPickupPreview() {
        Vector3Int currentCell = GetMousePositionInTilemap();
        if (BaseCoordinates.Contains(currentCell)) {
            decoTilemapObject.GetComponent<Tilemap>().color = SEMI_TRANSPARENT;
            waterTilemapObject.GetComponent<Tilemap>().color = SEMI_TRANSPARENT;
        }
        else {
            decoTilemapObject.GetComponent<Tilemap>().color = OPAQUE;
            waterTilemapObject.GetComponent<Tilemap>().color = OPAQUE;
        }
    }

    protected void PerformExtraActionsOnDeletePreview() {
        Vector3Int currentCell = GetMousePositionInTilemap();
        if (BaseCoordinates.Contains(currentCell)) {
            decoTilemapObject.GetComponent<Tilemap>().color = SEMI_TRANSPARENT_INVALID;
            waterTilemapObject.GetComponent<Tilemap>().color = SEMI_TRANSPARENT_INVALID;
        }
        else {
            decoTilemapObject.GetComponent<Tilemap>().color = OPAQUE;
            waterTilemapObject.GetComponent<Tilemap>().color = OPAQUE;
        }
    }

    /// <summary>
    ///Set the fish image and the pond color to a fish of your choosing
    /// </summary>
    /// <param name="fishType"> The fish</param>
    public void SetFishImage(Fish fishType) {
        ButtonParentGameObject.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = fishAtlas.GetSprite(fishType.ToString());
        Color color = fishType switch { // RGB 0-255 dont work so these are the values normalized to 0-1
            Fish.LavaEel => new Color(0.7490196f, 0.1137255f, 0.1333333f, 1),
            Fish.SuperCucumber => new Color(0.4117647f, 0.3294118f, 0.7490196f, 1),
            Fish.Slimejack => new Color(0.08886068f, 0.7490196f, 0.003921576f, 1),
            Fish.VoidSalmon => new Color(0.5764706f, 0.1176471f, 0.7490196f, 1),
            _ => new Color(0.2039216f, 0.5254902f, 0.7490196f, 1),
        };
        waterTilemapObject.GetComponent<Tilemap>().color = color;
        fish = fishType;
    }

    public void CreateFishMenu() {
        GameObject fishMenuPrefab = Resources.Load<GameObject>("UI/FishMenu");
        GameObject fishMenu = Instantiate(fishMenuPrefab);
        fishMenu.transform.SetParent(ButtonParentGameObject.transform.GetChild(0));
        Vector3 fishMenuPositionWorld = new(Tilemap.CellToWorld(BaseCoordinates[0] + new Vector3Int(1, 0, 0)).x, GetMiddleOfBuildingWorld(this).y);
        fishMenu.transform.position = Camera.main.WorldToScreenPoint(fishMenuPositionWorld);
        fishMenu.GetComponent<RectTransform>().localScale = new Vector2(1, 1);
        fishMenu.SetActive(false);
        GameObject fishMenuContent = fishMenu.transform.GetChild(0).GetChild(0).gameObject;
        for (int childIndex = 0; childIndex < fishMenuContent.transform.childCount; childIndex++) {
            Button fishButton = fishMenuContent.transform.GetChild(childIndex).GetComponent<Button>();
            AddHoverEffect(fishButton);
            fishButton.onClick.AddListener(() => {
                Fish fishType = (Fish)Enum.Parse(typeof(Fish), fishButton.GetComponent<Image>().sprite.name);
                SetFishImage(fishType);
                Debug.Log($"Set fish to {fishType}");
            });
        }
    }

    public void UpdateFishImage() {
        SetFishImage(fish);
    }

    public void CycleFishPondDeco() {
        decoIndex++;
        if (decoIndex > 3) decoIndex = 0;
        decoTilemapObject.GetComponent<Tilemap>().SetTiles(decoCoordinates, SplitSprite(atlas.GetSprite($"FishDeco_{decoIndex}")));
    }

    public string GetExtraData() {
        return $"{decoIndex}|{(int)fish}";
    }

    public void LoadExtraBuildingData(string[] data) {
        SetFishImage((Fish)int.Parse(data[1]));
        decoTilemapObject.GetComponent<Tilemap>().SetTiles(decoCoordinates, SplitSprite(atlas.GetSprite($"FishDeco_{data[0]}")));
    }

    public override GameObject CreateBuildingButton() {
        GameObject button = Instantiate(Resources.Load<GameObject>("UI/BuildingButton"));
        button.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        button.name = $"{BuildingName}";
        button.GetComponent<Image>().sprite = atlas.GetSprite("FishPondImage");

        Type buildingType = GetType();
        button.GetComponent<Button>().onClick.AddListener(() => {
            // Debug.Log($"Setting current building to {buildingType}");
            BuildingController.SetCurrentBuildingType(buildingType);
            BuildingController.SetCurrentAction(Actions.PLACE);
        });
        return button;
    }
}

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
using static BuildingData;
using Newtonsoft.Json.Linq;

public class FishPondComponent : BuildingComponent {
    protected readonly Color SEMI_TRANSPARENT = new(1, 1, 1, 0.5f);
    protected readonly Color SEMI_TRANSPARENT_INVALID = new(1, 0.5f, 0.5f, 0.5f);
    protected readonly Color OPAQUE = new(1, 1, 1, 1);


    private SpriteAtlas fishAtlas;
    private GameObject waterTilemapObject;
    private GameObject decoTilemapObject;
    private int decoIndex = 0;
    private Vector3Int[] decoCoordinates;
    private SpriteAtlas atlas;
    private GameObject fishMenu;
    public Fish fish;

    public void Awake() {
        if (!gameObject.GetComponent<InteractableBuildingComponent>()) gameObject.AddComponent<InteractableBuildingComponent>();
        fishAtlas = Resources.Load<SpriteAtlas>("Fish/FishAtlas");
        atlas = Resources.Load<SpriteAtlas>("Buildings/FishPondAtlas");
        decoTilemapObject = CreateTilemapObject(transform, 0, "Deco");
        waterTilemapObject = CreateTilemapObject(transform, 0, "Water");
        gameObject.GetComponent<InteractableBuildingComponent>().AddInteractionToBuilding(ButtonTypes.PLACE_FISH);
        gameObject.GetComponent<InteractableBuildingComponent>().AddInteractionToBuilding(ButtonTypes.CHANGE_FISH_POND_DECO);
        gameObject.GetComponent<InteractableBuildingComponent>().ButtonsCreated += CreateFishMenu;
        Building.BuildingPlaced += SetWaterTilemapLocation;
        Building.BuildingPlaced += SetDecoTilemapLocation;
    }

    /// <summary>
    ///Set the fish image and the pond color to a fish of your choosing
    /// </summary>
    /// <param name="fishType"> The fish</param>
    public void SetFishImage(Fish fishType) {
        Debug.Log($"Setting fish image to {fishType}");
        Building.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = fishAtlas.GetSprite(fishType.ToString());
        Color color = fishType switch { // RGB 0-255 dont work so these are the values normalized to 0-1
            Fish.LavaEel => new Color(0.7490196f, 0.1137255f, 0.1333333f, 1),
            Fish.SuperCucumber => new Color(0.4117647f, 0.3294118f, 0.7490196f, 1),
            Fish.Slimejack => new Color(0.08886068f, 0.7490196f, 0.003921576f, 1),
            Fish.VoidSalmon => new Color(0.5764706f, 0.1176471f, 0.7490196f, 1),
            Fish.PLACE_FISH => new Color(1, 1, 1, 1),
            _ => new Color(0.2039216f, 0.5254902f, 0.7490196f, 1),
        };
        waterTilemapObject.GetComponent<Tilemap>().color = color;
        fish = fishType;
    }

    public void RemoveFish() {
        SetFishImage(Fish.PLACE_FISH);
    }

    public void CreateFishMenu() {
        GameObject fishMenuPrefab = Resources.Load<GameObject>("UI/FishMenu");
        fishMenu = Instantiate(fishMenuPrefab);
        fishMenu.transform.SetParent(Building.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject.transform.GetChild(0));
        fishMenu.GetComponent<RectTransform>().localPosition = Building.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject.transform.GetChild(0).position - new Vector3(75, 0, 0);
        fishMenu.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        fishMenu.SetActive(false);
        GameObject fishMenuContent = fishMenu.transform.GetChild(0).GetChild(0).gameObject;
        for (int childIndex = 0; childIndex < fishMenuContent.transform.childCount; childIndex++) {
            Button fishButton = fishMenuContent.transform.GetChild(childIndex).GetComponent<Button>();
            fishButton.gameObject.AddComponent<ButtonSelfHandler>();
            fishButton.onClick.AddListener(() => {
                Fish fishType = (Fish)Enum.Parse(typeof(Fish), fishButton.GetComponent<Image>().sprite.name);
                SetFishImage(fishType);
                // Debug.Log($"Set fish to {fishType}");
            });
        }

        fishMenu.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => RemoveFish());
    }

    public void ToggleFishMenu() {
        fishMenu.SetActive(!fishMenu.activeSelf);
    }

    public void SetWaterTilemapLocation(Vector3Int lowerLeftCorner) {
        Vector3Int[] baseCoordinates = GetAreaAroundPosition(lowerLeftCorner, Building.BaseHeight, Building.Width).ToArray();
        waterTilemapObject.GetComponent<Tilemap>().ClearAllTiles();
        waterTilemapObject.GetComponent<Tilemap>().color = OPAQUE;
        waterTilemapObject.GetComponent<Tilemap>().SetTiles(baseCoordinates, SplitSprite(atlas.GetSprite("FishPondBottom")));
        waterTilemapObject.GetComponent<TilemapRenderer>().sortingOrder = gameObject.GetComponent<TilemapRenderer>().sortingOrder - 1;
    }

    public void SetWaterTilemapAlpha(float alpha) {
        var color = waterTilemapObject.GetComponent<Tilemap>().color;
        color.a = alpha;
        waterTilemapObject.GetComponent<Tilemap>().color = color;
    }

    public void ClearWaterTilemap() {
        waterTilemapObject.GetComponent<Tilemap>().ClearAllTiles();
    }

    public void SetDecoTilemapLocation(Vector3Int lowerLeftCorner) {
        Vector3Int topRightCorner = lowerLeftCorner + new Vector3Int(0, 4, 0);
        decoCoordinates = GetAreaAroundPosition(topRightCorner, 3, 5).ToArray();
        decoTilemapObject.GetComponent<Tilemap>().ClearAllTiles();
        decoTilemapObject.GetComponent<Tilemap>().color = OPAQUE;
        decoTilemapObject.GetComponent<Tilemap>().SetTiles(decoCoordinates, SplitSprite(atlas.GetSprite($"FishDeco_{decoIndex}")));
        decoTilemapObject.GetComponent<TilemapRenderer>().sortingOrder = gameObject.GetComponent<TilemapRenderer>().sortingOrder + 1;
    }

    public void SetDecoTilemapAlpha(float alpha) {
        var color = decoTilemapObject.GetComponent<Tilemap>().color;
        color.a = alpha;
        decoTilemapObject.GetComponent<Tilemap>().color = color;
    }

    public void ClearDecoTilemap() {
        // Debug.Log("Cleared Deco");
        decoTilemapObject.GetComponent<Tilemap>().ClearAllTiles();
    }

    public void CycleFishPondDeco() {
        decoIndex++;
        if (decoIndex > 3) decoIndex = 0;
        decoTilemapObject.GetComponent<Tilemap>().SetTiles(decoCoordinates, SplitSprite(atlas.GetSprite($"FishDeco_{decoIndex}")));
    }

    public override ComponentData Save() {
        return new(typeof(FishPondComponent),
                new() {
                    new JProperty("Fish", fish.ToString()),
                    new JProperty("Deco Index", decoIndex.ToString())
                });
    }

    public override void Load(ComponentData data) {
        SetFishImage((Fish)Enum.Parse(typeof(Fish), data.componentData.First(x => x.Name == "Fish").Value.ToString()));
        decoIndex = 0;
        for (int index = 0; index != int.Parse(data.componentData.First(x => x.Name == "Deco Index").Value.ToString()); index++) {
            CycleFishPondDeco();
        }
    }
}

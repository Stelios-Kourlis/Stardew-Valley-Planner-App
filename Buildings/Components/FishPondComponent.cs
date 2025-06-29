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


    private static SpriteAtlas fishAtlas;
    private GameObject waterTilemapObject;
    private GameObject decoTilemapObject;
    private int decoIndex = 0;
    private Vector3Int[] decoCoordinates;
    private static GameObject fishMenu;
    public Fish fish = Fish.PLACE_FISH;

    private static GameObject fishMenuPrefab;

    /// <summary>
    ///Set the fish image and the pond color to a fish of your choosing
    /// </summary>
    /// <param name="newFish"> The fish</param>
    public void SetFish(Fish newFish) {
        Building.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = fishAtlas.GetSprite(newFish.ToString());
        Color color = newFish switch { // RGB 0-255 dont work so these are the values normalized to 0-1
            Fish.LavaEel => new Color(0.7490196f, 0.1137255f, 0.1333333f, 1),
            Fish.SuperCucumber => new Color(0.4117647f, 0.3294118f, 0.7490196f, 1),
            Fish.Slimejack => new Color(0.08886068f, 0.7490196f, 0.003921576f, 1),
            Fish.VoidSalmon => new Color(0.5764706f, 0.1176471f, 0.7490196f, 1),
            Fish.PLACE_FISH => new Color(1, 1, 1, 1),
            _ => new Color(0.2039216f, 0.5254902f, 0.7490196f, 1),
        };
        waterTilemapObject.GetComponent<Tilemap>().color = color;
        UndoRedoController.AddActionToLog(new FishChangeRecord(this, (fish, newFish)));
        fish = newFish;
    }

    public void CreateFishMenu() {

        fishMenu = HUDButtonCotroller.CreatePanelNextToButton(fishMenuPrefab, GetComponent<InteractableBuildingComponent>().GetInteractionButtonTransform(ButtonTypes.PLACE_FISH));
        GameObject fishMenuContent = fishMenu.transform.Find("Fish").Find("ScrollArea").Find("Content").gameObject;
        for (int childIndex = 0; childIndex < fishMenuContent.transform.childCount; childIndex++) {
            Button fishButton = fishMenuContent.transform.GetChild(childIndex).GetComponent<Button>();
            fishButton.gameObject.AddComponent<UIElement>().ExpandOnHover = true;
            fishButton.gameObject.GetComponent<UIElement>().playSounds = true;
            fishButton.gameObject.GetComponent<UIElement>().SetActionToNothingOnEnter = false;
            fishButton.onClick.AddListener(() => {
                Fish fishType = (Fish)Enum.Parse(typeof(Fish), fishButton.GetComponent<Image>().sprite.name);
                SetFish(fishType);
            });
        }

        fishMenu.transform.Find("RemoveFish").GetComponent<Button>().onClick.AddListener(() => SetFish(Fish.PLACE_FISH));
    }

    public void ToggleFishMenu() {
        fishMenu.SetActive(!fishMenu.activeSelf);
    }

    public void SetWaterTilemapLocation(Vector3Int lowerLeftCorner) {
        Vector3Int[] baseCoordinates = GetRectAreaFromPoint(lowerLeftCorner, Building.BaseHeight, Building.Width).ToArray();
        waterTilemapObject.GetComponent<Tilemap>().ClearAllTiles();
        waterTilemapObject.GetComponent<Tilemap>().color = OPAQUE;
        waterTilemapObject.GetComponent<Tilemap>().SetTiles(baseCoordinates, SplitSprite(Building.Atlas.GetSprite("FishPondBottom")));
        waterTilemapObject.GetComponent<TilemapRenderer>().sortingOrder = gameObject.GetComponent<TilemapRenderer>().sortingOrder - 1;
    }

    public void SetWaterTilemapColop(float alpha) {
        var color = waterTilemapObject.GetComponent<Tilemap>().color;
        color.a = alpha;
        waterTilemapObject.GetComponent<Tilemap>().color = color;
    }

    public void ClearWaterTilemap() {
        waterTilemapObject.GetComponent<Tilemap>().ClearAllTiles();
    }

    public void SetDecoTilemapLocation(Vector3Int lowerLeftCorner) {
        Vector3Int topRightCorner = lowerLeftCorner + new Vector3Int(0, 4, 0);
        decoCoordinates = GetRectAreaFromPoint(topRightCorner, 3, 5).ToArray();
        decoTilemapObject.GetComponent<Tilemap>().ClearAllTiles();
        decoTilemapObject.GetComponent<Tilemap>().color = OPAQUE;
        decoTilemapObject.GetComponent<Tilemap>().SetTiles(decoCoordinates, SplitSprite(Building.Atlas.GetSprite($"FishDeco_{decoIndex}")));
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

    public void UpdateTilemapColors() {
        SetDecoTilemapAlpha(Building.Tilemap.color.a);
        SetWaterTilemapColop(Building.Tilemap.color.a);
    }

    public void CycleFishPondDeco() {
        decoIndex++;
        if (decoIndex > 3) decoIndex = 0;
        decoTilemapObject.GetComponent<Tilemap>().SetTiles(decoCoordinates, SplitSprite(Building.Atlas.GetSprite($"FishDeco_{decoIndex}")));
    }

    private void SetFishPondDeco(int index) {
        decoIndex = index;
        decoTilemapObject.GetComponent<Tilemap>().SetTiles(decoCoordinates, SplitSprite(Building.Atlas.GetSprite($"FishDeco_{decoIndex}")));
    }

    public override ComponentData Save() {
        return new(typeof(FishPondComponent),
                new() {
                    new JProperty("Fish", fish.ToString()),
                    new JProperty("Deco Index", decoIndex.ToString())
                });
    }

    public override void Load(ComponentData data) {
        SetFish(Enum.Parse<Fish>(data.GetComponentDataPropertyValue<string>("Fish")));
        SetFishPondDeco(data.GetComponentDataPropertyValue<int>("Deco Index"));
    }

    public override void Load(BuildingScriptableObject bso) {
        if (fishMenuPrefab == null) fishMenuPrefab = Resources.Load<GameObject>("UI/FishMenu");
        if (fishAtlas == null) fishAtlas = Resources.Load<SpriteAtlas>("Fish/FishAtlas");
        Building.UpdateTexture(Building.Atlas.GetSprite("FishPond"));
        decoTilemapObject = CreateTilemapObject(transform, 0, "Deco");
        waterTilemapObject = CreateTilemapObject(transform, 0, "Water");
        gameObject.GetComponent<InteractableBuildingComponent>().AddInteractionToBuilding(ButtonTypes.PLACE_FISH);
        gameObject.GetComponent<InteractableBuildingComponent>().AddInteractionToBuilding(ButtonTypes.CHANGE_FISH_POND_DECO);
        gameObject.GetComponent<InteractableBuildingComponent>().ButtonsCreated += CreateFishMenu;
    }
}

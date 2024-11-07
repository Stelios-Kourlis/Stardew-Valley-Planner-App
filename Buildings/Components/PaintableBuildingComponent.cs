using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using static Utility.TilemapManager;
using static Utility.SpriteManager;

public enum PaintableLayerType {
    Building,
    Trim,
    Roof
}

[Serializable]
public class PaintableLayer {
    public PaintableLayerType type;
    public Sprite sprite;
    [HideInInspector] public Color color;
    [HideInInspector] public Tilemap tilemap;
}

[RequireComponent(typeof(Building))]
public class PaintableBuildingComponent : BuildingComponent {

    public PaintableLayer[] layers;
    private static GameObject paintMenuPrefab;
    private GameObject paintMenu;
    private PaintableLayer currentLayer;

    public void Start() {
        Building.BuildingPlaced += _ => PlaceLayers();
    }

    private void PlaceLayers() {
        // GameObject paintMenuPrefab = Resources.Load<GameObject>("UI/PaintMenu");
        paintMenu = HUDButtonCotroller.CreatePanelNextToButton(paintMenuPrefab, GetComponent<InteractableBuildingComponent>().GetInteractionButtonTransform(ButtonTypes.PAINT));
        // Resources.UnloadAsset(paintMenuPrefab);

        Transform sliders = paintMenu.transform.Find("Sliders");
        sliders.Find("H").GetComponent<Slider>().onValueChanged.AddListener(value => {
            Color.RGBToHSV(currentLayer.color, out float H, out float S, out float V);
            H = Normalize((int)value, 0, 360);
            currentLayer.color = Color.HSVToRGB(H, S, V);
            currentLayer.tilemap.color = currentLayer.color;
        });

        sliders.Find("S").GetComponent<Slider>().onValueChanged.AddListener(value => {
            Color.RGBToHSV(currentLayer.color, out float H, out float S, out float V);
            S = Normalize((int)value, 0, 100);
            currentLayer.color = Color.HSVToRGB(H, S, V);
            currentLayer.tilemap.color = currentLayer.color;
        });

        sliders.Find("V").GetComponent<Slider>().onValueChanged.AddListener(value => {
            Color.RGBToHSV(currentLayer.color, out float H, out float S, out float V);
            V = Normalize((int)value, 0, 100);
            currentLayer.color = Color.HSVToRGB(H, S, V);
            currentLayer.tilemap.color = currentLayer.color;
        });

        foreach (PaintableLayer layer in layers) {
            if (layer.sprite == null) continue;
            GameObject layerObject = CreateTilemapObject(Building.transform, Building.TilemapRenderer.sortingOrder, layer.type.ToString());
            layerObject.GetComponent<Tilemap>().SetTiles(Building.SpriteCoordinates, SplitSprite(layer.sprite));
            layer.tilemap = layerObject.GetComponent<Tilemap>();
        }
    }



    private float Normalize(int value, int min, int max) {
        return (float)(value - min) / (max - min);
    }

    public void TogglePaintMenu() {
        paintMenu.SetActive(!paintMenu.activeSelf);
    }

    public override void Load(BuildingData.ComponentData data) {
        return;
    }

    public override void Load(BuildingScriptableObject bso) {
        layers = bso.paintableLayers;
        if (paintMenuPrefab == null) paintMenuPrefab = Resources.Load<GameObject>("UI/PaintMenu");

        GetComponent<InteractableBuildingComponent>().AddInteractionToBuilding(ButtonTypes.PAINT);
    }

    public override BuildingData.ComponentData Save() {
        throw new System.NotImplementedException();
    }
}

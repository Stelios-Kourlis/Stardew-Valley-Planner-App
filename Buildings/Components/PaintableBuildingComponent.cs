using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using static Utility.TilemapManager;
using static Utility.SpriteManager;

public enum PaintableParts {
    ROOF,
    TRIM,
    BUILDING
}


[RequireComponent(typeof(Building))]
public class PaintableBuildingComponent : BuildingComponent {

    private Color ROOF = Color.green;
    private Color TRIM = Color.blue;
    private Color BUILDING = Color.red;


    [SerializeField] private Sprite paintMask;
    private static GameObject paintMenuPrefab;
    private GameObject paintMenu;
    private PaintableParts selectedPart = PaintableParts.ROOF;

    private String selectedPartRefrence {
        get {
            return selectedPart switch {
                PaintableParts.ROOF => "_Roof",
                PaintableParts.TRIM => "_Trim",
                PaintableParts.BUILDING => "_Building",
                _ => throw new NotImplementedException(),
            };
        }
    }

    public void Start() {
        Building.BuildingPlaced += _ => PlaceLayers();
    }

    private void PlaceLayers() {
        // GameObject paintMenuPrefab = Resources.Load<GameObject>("UI/PaintMenu");
        paintMenu = HUDButtonCotroller.CreatePanelNextToButton(paintMenuPrefab, GetComponent<InteractableBuildingComponent>().GetInteractionButtonTransform(ButtonTypes.PAINT));

        foreach (Transform buttonTransform in paintMenu.transform.Find("LayerChoose")) {
            Button button = buttonTransform.GetComponent<Button>();
            button.onClick.AddListener(() => {
                selectedPart = buttonTransform.name switch {
                    "Roof" => PaintableParts.ROOF,
                    "Trim" => PaintableParts.TRIM,
                    "Building" => PaintableParts.BUILDING,
                    _ => throw new NotImplementedException(),
                };
            });
        }

        Material material = new(Shader.Find("Shader Graphs/colorChange"));
        gameObject.GetComponent<TilemapRenderer>().material = material;
        material.SetTexture("_MainTex", Building.Sprite.texture);
        material.SetTexture("_PaintMask", paintMask.texture);
        // Resources.UnloadAsset(paintMenuPrefab);

        Transform sliders = paintMenu.transform.Find("Sliders");
        sliders.Find("H").GetComponent<Slider>().onValueChanged.AddListener(value => {
            Color layerColor = material.GetColor(selectedPartRefrence);
            Color.RGBToHSV(layerColor, out float H, out float S, out float V);
            H = Normalize((int)value, 0, 360);
            material.SetColor(selectedPartRefrence, Color.HSVToRGB(H, S, V));
        });

        sliders.Find("S").GetComponent<Slider>().onValueChanged.AddListener(value => {
            Color layerColor = material.GetColor(selectedPartRefrence);
            Color.RGBToHSV(layerColor, out float H, out float S, out float V);
            S = Normalize((int)value, 0, 86);
            material.SetColor(selectedPartRefrence, Color.HSVToRGB(H, S, V));
        });

        sliders.Find("V").GetComponent<Slider>().onValueChanged.AddListener(value => {
            Color layerColor = material.GetColor(selectedPartRefrence);
            Color.RGBToHSV(layerColor, out float H, out float S, out float V);
            V = Normalize((int)value, 25, 86);
            material.SetColor(selectedPartRefrence, Color.HSVToRGB(H, S, V));
        });

        paintMenu.SetActive(false);
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
        paintMask = bso.paintMask;
        if (paintMenuPrefab == null) paintMenuPrefab = Resources.Load<GameObject>("UI/PaintMenu");

        GetComponent<InteractableBuildingComponent>().AddInteractionToBuilding(ButtonTypes.PAINT);
    }

    public override BuildingData.ComponentData Save() {
        return null;
    }
}

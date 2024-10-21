using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using static Utility.ClassManager;

public class TotalMaterialsCalculator : MonoBehaviour {

    private enum Mode {
        ByType,
        ByBuilding
    }

    private List<MaterialCostEntry> materialsNeeded;
    [SerializeField] private SpriteAtlas materialAtlas;
    [SerializeField] private GameObject buildingCostPrefab;
    [SerializeField] private GameObject materialEntryPrefab;
    private readonly List<BuildingType> collapsableTypes = new() { BuildingType.Crop, BuildingType.Craftables, BuildingType.Fence, BuildingType.Floor, BuildingType.Sprinkler, BuildingType.Scarecrow, };
    private Mode currentMode;
    private Transform ContentPanelTransform => transform.Find("ScrollArea").Find("Content");

    public void Start() {
        GetComponent<MoveablePanel>().panelOpened += RecalculateMaterials;

        // foreach (Materials material in Enum.GetValues(typeof(Materials))) { // This is a debug to ensure all materials have a sprite in the atlas
        //     if (material == Materials.DummyMaterial) continue;
        //     if (materialAtlas.GetSprite(material.ToString()) == null) Debug.LogError($"Material {material} does not have a sprite in the atlas");
        // }
    }

    private void RecalculateMaterials() {
        if (currentMode == Mode.ByType) ShowMaterialsByType();
        else ShowMaterialsByBuilding();
    }

    public void ShowMaterialsByType() {
        ClearPanel();
        currentMode = Mode.ByType;
        SetButtonsColor();
        materialsNeeded = new();
        foreach (Building building in BuildingController.GetBuildings()) {
            foreach (MaterialCostEntry material in building.GetMaterialsNeeded()) {
                MaterialCostEntry prexeistingEntry = materialsNeeded.FirstOrDefault(item => item.EntryText == material.EntryText);
                if (prexeistingEntry != null) prexeistingEntry.amount += material.amount;
                else materialsNeeded.Add((MaterialCostEntry)material.Clone());
            }
        }
        foreach (MaterialCostEntry material in materialsNeeded) CreateTextGameObject(material, ContentPanelTransform);
    }

    public void ShowMaterialsByBuilding() {
        ClearPanel();
        currentMode = Mode.ByBuilding;
        SetButtonsColor();
        List<Building> allBuildingsCompressed = CompressBuildingsList(BuildingController.GetBuildings());
        Dictionary<string, GameObject> addedBuildings = new();
        foreach (Building building in BuildingController.GetBuildings()) {
            // if (building.type == BuildingType.Crop) continue;

            if (addedBuildings.ContainsKey(building.FullName) && collapsableTypes.Contains(building.type)) {
                GameObject cost = addedBuildings[building.FullName];
                string oldText = cost.transform.Find("Text").Find("Materials").GetChild(0).GetChild(1).GetComponent<Text>().text;
                string newText = Regex.Replace(oldText, @"\d+", match => (int.Parse(match.Value) + 1).ToString());
                cost.transform.Find("Text").Find("Materials").GetChild(0).GetChild(1).GetComponent<Text>().text = newText;

                string oldTitle = cost.transform.Find("Text").Find("BuildingName").Find("name").GetComponent<Text>().text;
                oldTitle.Replace("Crop", "");
                oldTitle.Replace("Craftables", "");
                var regex = new Regex(@"^(\d+)x\s");
                string newTitle;
                if (regex.IsMatch(oldTitle)) newTitle = regex.Replace(oldTitle, match => $"{int.Parse(match.Groups[1].Value) + 1}x ");
                else newTitle = $"1x {oldTitle}";
                cost.transform.Find("Text").Find("BuildingName").Find("name").GetComponent<Text>().text = newTitle;
                continue;
            }//todo this whole if is ugly, fix later

            GameObject buildingCost = Instantiate(buildingCostPrefab, ContentPanelTransform);
            buildingCost.transform.Find("BuildingImage").GetComponent<Image>().sprite = building.Sprite;
            buildingCost.transform.Find("Text").Find("BuildingName").Find("name").GetComponent<TMP_Text>().text = building.FullName;
            buildingCost.transform.Find("Text").Find("BuildingName").Find("amount").GetComponent<TMP_Text>().text = "";
            Transform parent = buildingCost.transform.Find("Text").Find("Materials");
            foreach (MaterialCostEntry material in building.GetMaterialsNeeded()) CreateTextGameObject(material, parent);
            if (!addedBuildings.ContainsKey(building.FullName)) addedBuildings.Add(building.FullName, buildingCost);
        }

        // Dictionary<int, int> amountPerCrop = new();
        // foreach (Building building in BuildingController.GetBuildings().Where(b => b.type == BuildingType.Crop)) {
        //     int cropVariant = building.gameObject.GetComponent<MultipleTypeBuildingComponent>().CurrentVariantIndex;
        //     if (amountPerCrop.ContainsKey(cropVariant)) amountPerCrop[cropVariant] += 1;
        //     else amountPerCrop.Add(cropVariant, 1);
        // }

        // foreach (int variant in amountPerCrop.Keys) {
        //     MaterialCostEntry entry = new(amountPerCrop[variant], );

        // }


        RectTransform layoutGroupRectTransform = ContentPanelTransform.GetComponent<RectTransform>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroupRectTransform);
    }

    private List<Building> CompressBuildingsList(List<Building> buildings) {
        List<Building> compressedList = new();
        foreach (Building building in buildings) {
            if (building.type == BuildingType.Crop) {
                if (compressedList.Where(b => b.FullName == building.FullName).Any()) {
                    compressedList.First(b => b.FullName == building.FullName);
                }
            }
            else compressedList.Add(building);
        }
        return compressedList;
    }

    private void SetButtonsColor() {
        GameObject buttons = gameObject.transform.Find("Buttons").gameObject;
        GameObject byTypeButton = buttons.transform.Find("PerType").gameObject;
        GameObject byBuildingButton = buttons.transform.Find("PerBuilding").gameObject;
        ColorBlock normalColors = new() {
            normalColor = new Color(1, 1, 1),
            highlightedColor = new Color(0.9607843f, 0.9607843f, 0.9607843f),
            pressedColor = new Color(0.7843137f, 0.7843137f, 0.7843137f),
            selectedColor = new Color(0.9607843f, 0.9607843f, 0.9607843f),
            disabledColor = new Color(0.7843137f, 0.7843137f, 0.7843137f),
            colorMultiplier = 1,
            fadeDuration = 0.1f
        };

        ColorBlock disabledColors = new() {
            normalColor = new Color(0.5f, 0.5f, 0.5f),
            highlightedColor = new Color(0.9607843f, 0.9607843f, 0.9607843f),
            pressedColor = new Color(0.7843137f, 0.7843137f, 0.7843137f),
            selectedColor = new Color(0.9607843f, 0.9607843f, 0.9607843f),
            disabledColor = new Color(0.7843137f, 0.7843137f, 0.7843137f),
            colorMultiplier = 1,
            fadeDuration = 0.1f
        };

        if (currentMode == Mode.ByType) {
            byTypeButton.GetComponent<Button>().colors = normalColors;
            byBuildingButton.GetComponent<Button>().colors = disabledColors;
        }
        else {
            byTypeButton.GetComponent<Button>().colors = disabledColors;
            byBuildingButton.GetComponent<Button>().colors = normalColors;
        }

    }

    private void CreateTextGameObject(MaterialCostEntry material, Transform parent) {
        GameObject entryGameObject = Instantiate(materialEntryPrefab);
        entryGameObject.transform.SetParent(parent);
        GameObject textGameObject = entryGameObject.transform.Find("Text").gameObject;
        textGameObject.transform.Find("amount").GetComponent<TMP_Text>().text = $"{material.amount:N0}x";
        textGameObject.transform.Find("name").GetComponent<TMP_Text>().text = $"{material.EntryText}";
        AddImage(material, entryGameObject);
    }

    private void AddImage(MaterialCostEntry material, GameObject entryObject) {
        GameObject imageGameObject = entryObject.transform.Find("Image").gameObject;
        if (material.IsSpecial) {
            imageGameObject.GetComponent<Image>().color = new(0, 0, 0, 0); //special material has no image
            return;
        }
        Sprite sprite = materialAtlas.GetSprite(material.materialType.ToString());
        imageGameObject.GetComponent<Image>().sprite = sprite;
    }

    private void ClearPanel() {
        Debug.Log(transform.name);
        Debug.Log(transform.Find("ScrollArea"));
        foreach (Transform child in ContentPanelTransform) Destroy(child.gameObject);
    }


}

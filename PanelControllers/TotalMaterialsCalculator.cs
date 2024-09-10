using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private Mode currentMode;
    private Transform ContentPanelTransform => transform.Find("ScrollArea").Find("Content");

    public void Start() {
        GetComponent<MoveablePanel>().panelOpened += RecalculateMaterials;
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
                else materialsNeeded.Add(material);
            }
        }
        foreach (MaterialCostEntry material in materialsNeeded) CreateTextGameObject(material, ContentPanelTransform);
    }

    public void ShowMaterialsByBuilding() {
        ClearPanel();
        currentMode = Mode.ByBuilding;
        SetButtonsColor();
        foreach (Building building in BuildingController.GetBuildings()) {
            GameObject buildingCost = Instantiate(buildingCostPrefab, ContentPanelTransform);
            buildingCost.transform.Find("BuildingImage").GetComponent<Image>().sprite = building.Sprite;
            buildingCost.transform.Find("Text").Find("BuildingName").GetComponent<Text>().text = building.BuildingName;
            Transform parent = buildingCost.transform.Find("Text").Find("Materials");
            foreach (MaterialCostEntry material in building.GetMaterialsNeeded()) CreateTextGameObject(material, parent);
        }
        RectTransform layoutGroupRectTransform = ContentPanelTransform.GetComponent<RectTransform>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroupRectTransform);
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
        textGameObject.GetComponent<Text>().text = $"{material.amount:N0}x {material.EntryText}";
        AddImage(material, entryGameObject);
    }

    private void AddImage(MaterialCostEntry material, GameObject entryObject) {
        GameObject imageGameObject = entryObject.transform.Find("Image").gameObject;
        if (material.IsSpecial) {
            imageGameObject.GetComponent<Image>().color = new(0, 0, 0, 0); //special material has no image
            return;
        }
        Sprite sprite = materialAtlas.GetSprite(material.name.ToString());
        imageGameObject.GetComponent<Image>().sprite = sprite;
    }

    private void ClearPanel() {
        Debug.Log(transform.name);
        Debug.Log(transform.Find("ScrollArea"));
        foreach (Transform child in ContentPanelTransform) Destroy(child.gameObject);
    }


}

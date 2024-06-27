using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using static Utility.ClassManager;

public class TotalMaterialsCalculator : MonoBehaviour, IToggleablePanel {

    const int HEIGHT = 50;
    const int WIDTH = 1200;

    private List<MaterialCostEntry> materialsNeeded;
    private SpriteAtlas materialAtlas;
    private GameObject buildingCostPrefab;
    private GameObject materialEntry;

    public bool IsMoving { get; private set; }

    public bool IsOpen { get; private set; }
    private Transform ContentPanelTransform => gameObject.transform.GetChild(1).GetChild(0);

    public void Start() {
        materialAtlas = Resources.Load<SpriteAtlas>("Materials/MaterialAtlas");
        buildingCostPrefab = Resources.Load<GameObject>("UI/BuildingCost");
        materialEntry = Resources.Load<GameObject>("UI/MaterialEntry");
        gameObject.transform.position = new Vector3(Screen.width / 2, Screen.height + 500, 0);
    }

    public void ShowMaterialsByType() {
        ClearPanel();
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


    private void CreateTextGameObject(MaterialCostEntry material, Transform parent) {
        GameObject entryGameObject = Instantiate(materialEntry);
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
        foreach (Transform child in ContentPanelTransform) Destroy(child.gameObject);
    }

    public void CallClosePanelCoroutine() {
        StartCoroutine(ClosePanel());
    }

    public void CallOpenPanelCoroutine() {
        StartCoroutine(OpenPanel());
    }
    public void TogglePanel() {
        if (IsMoving) return;
        if (IsOpen) StartCoroutine(ClosePanel());
        else {
            StartCoroutine(OpenPanel());
        }
    }
    public IEnumerator OpenPanel() {
        if (IsOpen) yield break;
        IsMoving = true;
        IsOpen = true;
        IToggleablePanel.PanelsCurrentlyOpen++;
        StartCoroutine(GetSettingsModalController().ClosePanel());
        if (BuildingController.CurrentAction != Actions.DO_NOTHING) {
            IToggleablePanel.ActionBeforeEnteringSettings = BuildingController.CurrentAction;
            BuildingController.SetCurrentAction(Actions.DO_NOTHING);
        }
        GetInputHandler().SetCursor(InputHandler.CursorType.Default);
        // BuildingController.LastBuildingObjectCreated.GetComponent<Building>().HidePreview();
        StartCoroutine(GetCamera().GetComponent<CameraController>().BlurBasedOnDistance(gameObject, new Vector3(Screen.width / 2, Screen.height / 2, 0)));// the close to 0,0 the more blur we want
        while (gameObject.transform.position.y > Screen.height / 2) {
            gameObject.transform.position -= new Vector3(0, IToggleablePanel.MOVE_SCALE * Time.deltaTime, 0);
            yield return null;
        }
        gameObject.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        IsMoving = false;

    }
    public IEnumerator ClosePanel() {
        if (!IsOpen) yield break;
        IsMoving = true;
        IsOpen = false;
        IToggleablePanel.PanelsCurrentlyOpen--;
        StartCoroutine(GetCamera().GetComponent<CameraController>().BlurBasedOnDistance(gameObject, new Vector3(Screen.width / 2, Screen.height / 2, 0)));// the close to 0,0 the more blur we want
        while (gameObject.transform.position.y < Screen.height + 500) {
            gameObject.transform.position += new Vector3(0, IToggleablePanel.MOVE_SCALE * Time.deltaTime, 0);
            yield return null;
        }
        IsMoving = false;
        gameObject.transform.position = new Vector3(Screen.width / 2, Screen.height + 500, 0);
        if (IToggleablePanel.PanelsCurrentlyOpen == 0) BuildingController.SetCurrentAction(IToggleablePanel.ActionBeforeEnteringSettings);
    }
}

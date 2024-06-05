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

    private readonly List<MaterialInfo> materialsNeeded = new();
    private SpriteAtlas materialAtlas;

    public bool IsMoving { get; private set; }

    public bool IsOpen { get; private set; }

    public void Start() {
        materialAtlas = Resources.Load<SpriteAtlas>("Materials/MaterialAtlas");
        gameObject.transform.position = new Vector3(Screen.width / 2, Screen.height + 500, 0);
    }

    public void SumTotalMaterialsNeeded() {
        materialsNeeded.Clear();
        foreach (Building building in BuildingController.GetBuildings()) {
            foreach (MaterialInfo material in building.GetMaterialsNeeded()) {
                if (material.IsSpecial) {
                    MaterialInfo specialMaterial = materialsNeeded.FirstOrDefault(item => item.howToGet == material.howToGet);
                    if (specialMaterial != null) specialMaterial.amount += material.amount;
                    else materialsNeeded.Add(material);
                    continue;
                }
                else {
                    MaterialInfo materialInfo = materialsNeeded.FirstOrDefault(item => item.name == material.name);
                    if (materialInfo != null) materialInfo.amount += material.amount;
                    else materialsNeeded.Add(material);
                }
            }
        }
        DisplayMaterials();
    }

    private void DisplayMaterials() {
        GameObject materialsNeededPanel = GameObject.FindGameObjectWithTag("TotalMaterialsNeededPanel");
        // StartCoroutine(OpenPanel());
        GameObject scrollContent = materialsNeededPanel.transform.GetChild(1).GetChild(0).gameObject;
        int counter = 0;

        for (int childIndex = 0; childIndex < scrollContent.transform.childCount; childIndex++) {
            Destroy(scrollContent.transform.GetChild(childIndex).gameObject);
        }

        foreach (MaterialInfo material in materialsNeeded) {
            GameObject entry = new($"{material}Entry");
            entry.transform.SetParent(scrollContent.transform);
            entry.AddComponent<RectTransform>().sizeDelta = new Vector2(WIDTH, HEIGHT);
            entry.transform.position = new Vector3(50, -50 + counter * HEIGHT, 0);
            if (!material.IsSpecial) {
                AddTextGameObject(material, entry.transform);
                AddImage(material, entry.transform);
            }
            else AddSpecialTextGameObject(material, entry.transform);
            counter++;
        }
        // materialsNeededPanel.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
    }

    private void AddTextGameObject(MaterialInfo material, Transform parent) {
        if (material.IsSpecial) throw new ArgumentException("Material is special, use CreateSpecialTextGameObject() instead.");
        GameObject textGameObject = new(material.name.ToString());
        textGameObject.AddComponent<Text>().text = $"{material.amount:N0}x {material.name}";
        textGameObject.GetComponent<Text>().fontSize = 50;
        textGameObject.GetComponent<Text>().color = Color.black;
        textGameObject.GetComponent<Text>().font = Resources.Load<Font>("StardewValley");
        textGameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(WIDTH, HEIGHT);
        textGameObject.GetComponent<RectTransform>().pivot = new Vector2(0, 0.5f);
        textGameObject.transform.SetParent(parent);
    }

    private void AddSpecialTextGameObject(MaterialInfo material, Transform parent) {
        if (!material.IsSpecial) throw new ArgumentException("Material is not special, use CreateTextGameObject() instead.");
        GameObject textGameObject = new(material.howToGet);
        textGameObject.AddComponent<Text>().text = $"{material.amount:N0}x {material.howToGet}";
        textGameObject.GetComponent<Text>().fontSize = 50;
        textGameObject.GetComponent<Text>().color = Color.black;
        textGameObject.GetComponent<Text>().font = Resources.Load<Font>("StardewValley");
        textGameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(WIDTH, HEIGHT);
        textGameObject.GetComponent<RectTransform>().pivot = new Vector2(0, 0.5f);
        textGameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        textGameObject.transform.SetParent(parent);
    }

    private void AddImage(MaterialInfo material, Transform parent) {
        if (material.IsSpecial) throw new ArgumentException("Special Material do not have images");
        GameObject imageGameObject = new(material.name.ToString());
        Sprite sprite = materialAtlas.GetSprite(material.name.ToString());
        imageGameObject.AddComponent<Image>().sprite = sprite;
        imageGameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(HEIGHT, HEIGHT);
        imageGameObject.GetComponent<RectTransform>().pivot = new Vector2(1, 0.5f);
        imageGameObject.transform.SetParent(parent);
    }

    public void CallClosePanelCoroutine() {
        StartCoroutine(ClosePanel());
    }
    public void TogglePanel() {
        if (IsMoving) return;
        if (IsOpen) StartCoroutine(ClosePanel());
        else {
            SumTotalMaterialsNeeded();
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

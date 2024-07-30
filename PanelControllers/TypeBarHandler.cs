using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Utility.ClassManager;
using static Utility.BuildingManager;
using System.Reflection;
using Utility;

public class TypeBarHandler : MonoBehaviour {
    private readonly Sprite[] arrowButtons = new Sprite[2];
    private Type lastType;
    private Type currentBuildingType;
    private GameObject searchBar;

    // Start is called before the first frame update
    void Start() {
        gameObject.transform.position = new Vector3(Screen.width + 300, 5, 0);
        arrowButtons[0] = Sprite.Create(Resources.Load("UI/TypeBarUnhide") as Texture2D, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
        arrowButtons[1] = Sprite.Create(Resources.Load("UI/TypeBarHide") as Texture2D, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);

        lastType = BuildingController.GetCurrentBuildingType();

        searchBar = GameObject.FindGameObjectWithTag("TypeSearchBar");
    }

    void Update() {
        lastType = currentBuildingType;
        currentBuildingType = BuildingController.GetCurrentBuildingType();
        if (currentBuildingType == lastType) return;
        bool isCurrentlyBuildingMultipleTypeBuilding = IsMultipleTypeBuilding(currentBuildingType);
        if (isCurrentlyBuildingMultipleTypeBuilding) {
            if (!GetComponent<MoveablePanel>().IsPanelOpen()) {
                GetComponent<MoveablePanel>().SetPanelToOpenPosition();
                searchBar.GetComponent<MoveablePanel>().SetPanelToClosedPosition();
            }
            Transform typeBarContent = transform.GetChild(0).GetChild(0);
            GameObject temp = new();
            dynamic buildingTemp = temp.AddComponent(currentBuildingType);
            GameObject[] buttons = buildingTemp.CreateButtonsForAllTypes();
            Destroy(temp);
            for (int i = 0; i < typeBarContent.childCount; i++) Destroy(typeBarContent.GetChild(i).gameObject);
            foreach (GameObject button in buttons) {
                button.transform.SetParent(typeBarContent);
                button.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            }
        }
        else if (!isCurrentlyBuildingMultipleTypeBuilding && GetComponent<MoveablePanel>().IsPanelOpen()) {
            GetComponent<MoveablePanel>().SetPanelToHiddenPosition();
            searchBar.GetComponent<MoveablePanel>().SetPanelToHiddenPosition();
        }
    }

    public bool IsMultipleTypeBuilding(Type type) {
        if (type == typeof(Crop)) return false;
        if (type == typeof(Craftables)) return false;
        GameObject temp = new();
        temp.AddComponent(type);
        bool isMultipleTypeBuilding = temp.TryGetComponent(out MultipleTypeBuildingComponent _);
        Destroy(temp);
        return isMultipleTypeBuilding;
    }
}

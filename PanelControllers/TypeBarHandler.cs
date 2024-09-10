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
    [SerializeField] private GameObject typeBar;

    // Start is called before the first frame update
    void Start() {
        BuildingController.currentBuildingTypeChanged += EvalueateIfTypeBarShouldBeShown;
        EvalueateIfTypeBarShouldBeShown(BuildingController.currentBuildingType);
    }

    private void EvalueateIfTypeBarShouldBeShown(Type newBuildingType) {
        bool isCurrentlyBuildingMultipleTypeBuilding = IsMultipleTypeBuilding(newBuildingType);
        if (isCurrentlyBuildingMultipleTypeBuilding) {
            typeBar.GetComponent<MoveablePanel>().SetPanelToOpenPosition();

            Transform typeBarContent = typeBar.transform.GetChild(0).GetChild(0);
            GameObject temp = new();
            Building buildingTemp = temp.AddComponent(newBuildingType) as Building;
            GameObject[] buttons = buildingTemp.GetComponent<MultipleTypeBuildingComponent>().CreateButtonsForAllTypes();
            Destroy(temp);
            for (int i = 0; i < typeBarContent.childCount; i++) Destroy(typeBarContent.GetChild(i).gameObject);
            foreach (GameObject button in buttons) {
                button.transform.SetParent(typeBarContent);
                button.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            }
        }
        else typeBar.GetComponent<MoveablePanel>().SetPanelToClosedPosition();
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

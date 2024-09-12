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
    private readonly Type[] typesThatShouldBeInCraftables = { typeof(Sprinkler), typeof(Floor), typeof(Fence), typeof(Scarecrow), typeof(Craftables) };
    [SerializeField] private GameObject typeBar;

    // Start is called before the first frame update
    void Start() {
        Transform categoryButtons = transform.Find("Rect").Find("CategoryButtons");
        Transform content = transform.Find("Rect").Find("ScrollAreaBuildings").Find("Content");

        categoryButtons.Find("BuildingsButton").GetComponent<Button>().onClick.AddListener(() => {
            ClearAllChildrenInTransform(content);
            CreateButtonsForBuildings(content);
        });

        categoryButtons.Find("PlaceablesButton").GetComponent<Button>().onClick.AddListener(() => {
            ClearAllChildrenInTransform(content);
            CreateButtonsForCraftables(content);
        });

        categoryButtons.Find("CropsButton").GetComponent<Button>().onClick.AddListener(() => {
            ClearAllChildrenInTransform(content);
            CreateButtonsForCrops(content);
        });

        CreateButtonsForBuildings(content);

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


    private void ClearAllChildrenInTransform(Transform transform) {
        for (int i = 0; i < transform.childCount; i++) Destroy(transform.GetChild(i).gameObject);
    }
    private void CreateButtonsForBuildings(Transform buildingSelectContent) {
        var buildingType = typeof(Building);
        var allTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => buildingType.IsAssignableFrom(p) && p.IsClass && !p.IsAbstract);
        foreach (var type in allTypes) {
            if (type == typeof(House)) continue; //these 2 should not be available to build
            if (type == typeof(Greenhouse)) continue;
            if (type == typeof(Crop)) continue; //later category
            if (typesThatShouldBeInCraftables.Contains(type)) continue; //if its not any of these dont add it
            GameObject temp = new();
            Building buildingTemp = (Building)temp.AddComponent(type);
            GameObject button = buildingTemp.CreateBuildingButton();
            button.transform.SetParent(buildingSelectContent);
            button.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            Destroy(temp);
        }
    }

    private void CreateButtonsForCraftables(Transform buildingSelectContent) {
        var buildingType = typeof(Building);
        var allTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => buildingType.IsAssignableFrom(p) && p.IsClass && !p.IsAbstract);
        foreach (var type in allTypes) {
            if (type == typeof(House)) continue; //these 2 should not be available to build
            if (type == typeof(Greenhouse)) continue;
            if (!typesThatShouldBeInCraftables.Contains(type)) continue; //only add craftables
            if (type == typeof(Craftables)) {
                GameObject tempCraftables = new("CraftablesTypes");
                GameObject[] buttons = tempCraftables.AddComponent<Craftables>().GetComponent<MultipleTypeBuildingComponent>().CreateButtonsForAllTypes();
                foreach (GameObject craftableButton in buttons) {
                    craftableButton.transform.SetParent(buildingSelectContent);
                    craftableButton.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                }
                Destroy(tempCraftables);
            }
            GameObject temp = new();
            Building buildingTemp = (Building)temp.AddComponent(type);
            GameObject button = buildingTemp.CreateBuildingButton();//todo Floor/Fence are missing icons
            button.transform.SetParent(buildingSelectContent);
            button.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            Destroy(temp);
        }
    }

    private void CreateButtonsForCrops(Transform buildingSelectContent) {
        GameObject temp = new();
        GameObject[] buttons = temp.AddComponent<Crop>().GetComponent<MultipleTypeBuildingComponent>().CreateButtonsForAllTypes();
        foreach (GameObject button in buttons) {
            button.transform.SetParent(buildingSelectContent.transform);
            button.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        }
        Destroy(temp);
    }
}

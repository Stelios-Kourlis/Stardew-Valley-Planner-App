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
    private readonly BuildingType[] typesThatShouldBeInCraftables = { BuildingType.Sprinkler, BuildingType.Floor, BuildingType.Fence, BuildingType.Scarecrow, BuildingType.Craftables };
    [SerializeField] private GameObject typeBar;
    // [SerializeField] private List<BuildingScriptableObject> buildingsInBuildingPanel;

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

    private void EvalueateIfTypeBarShouldBeShown(BuildingType newBuildingType) {
        BuildingScriptableObject bso = Resources.Load<BuildingScriptableObject>($"BuildingScriptableObjects/{newBuildingType}");
        Debug.Log($"EvalueateIfTypeBarShouldBeShown for {newBuildingType}");
        bool isCurrentlyBuildingMultipleTypeBuilding = IsMultipleTypeBuilding(bso);
        if (isCurrentlyBuildingMultipleTypeBuilding) {
            typeBar.GetComponent<MoveablePanel>().SetPanelToOpenPosition();

            Transform typeBarContent = typeBar.transform.GetChild(0).GetChild(0);

            GameObject[] buttons = MultipleTypeBuildingComponent.CreateButtonsForAllTypes(newBuildingType);
            for (int i = 0; i < typeBarContent.childCount; i++) Destroy(typeBarContent.GetChild(i).gameObject);
            foreach (GameObject button in buttons) {
                button.transform.SetParent(typeBarContent);
                button.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            }

        }
        else typeBar.GetComponent<MoveablePanel>().SetPanelToClosedPosition();
        Resources.UnloadAsset(bso);

    }

    public bool IsMultipleTypeBuilding(BuildingScriptableObject bso) {
        if (bso.buildingName == "Crop") return false;
        if (bso.buildingName == "Craftables") return false;
        return bso.isMultipleType;
    }


    private void ClearAllChildrenInTransform(Transform transform) {
        for (int i = 0; i < transform.childCount; i++) Destroy(transform.GetChild(i).gameObject);
    }
    private void CreateButtonsForBuildings(Transform buildingSelectContent) {
        foreach (BuildingType type in Enum.GetValues(typeof(BuildingType))) {
            if (type == BuildingType.House) continue; //these 2 should not be available to build
            if (type == BuildingType.Greenhouse) continue;
            if (type == BuildingType.Crop) continue; //later category
            if (typesThatShouldBeInCraftables.Contains(type)) continue; //if its not any of these dont add it
            GameObject button = Building.CreateBuildingButton(type);
            if (button == null) continue;
            button.transform.SetParent(buildingSelectContent);
            button.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        }
    }

    private void CreateButtonsForCraftables(Transform buildingSelectContent) {
        foreach (BuildingType type in Enum.GetValues(typeof(BuildingType))) {
            if (type == BuildingType.House) continue; //these 2 should not be available to build
            if (type == BuildingType.Greenhouse) continue;
            if (!typesThatShouldBeInCraftables.Contains(type)) continue; //only add craftables
            if (type == BuildingType.Craftables) {
                GameObject[] buttons = MultipleTypeBuildingComponent.CreateButtonsForAllTypes(type);
                foreach (GameObject craftableButton in buttons) {
                    craftableButton.transform.SetParent(buildingSelectContent);
                    craftableButton.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                }
            }
            GameObject button = Building.CreateBuildingButton(type);
            button.transform.SetParent(buildingSelectContent);
            button.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        }
    }

    private void CreateButtonsForCrops(Transform buildingSelectContent) {
        GameObject[] buttons = MultipleTypeBuildingComponent.CreateButtonsForAllTypes(BuildingType.Crop);
        foreach (GameObject button in buttons) {
            button.transform.SetParent(buildingSelectContent.transform);
            button.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        }
    }
}

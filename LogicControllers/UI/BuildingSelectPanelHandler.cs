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

public enum Categories {
    Buildings,
    Craftables,
    Crops,
    Decorations,
    None
}
public class BuildingSelectPanelHandler : MonoBehaviour {
    [SerializeField] private GameObject typeBar;
    private BuildingType currentBuildingType;
    void Start() {
        GameObject.Find("NoBuilding").GetComponent<Button>().onClick.AddListener(() => BuildingController.SetCurrentAction(Actions.DO_NOTHING));

        Transform categoryButtons = transform.Find("Rect").Find("CategoryButtons");
        Transform content = transform.Find("Rect").Find("ScrollAreaBuildings").Find("Content");

        foreach (Categories category in Enum.GetValues(typeof(Categories))) {
            if (category == Categories.None || category == Categories.Decorations) continue; //todo remove after adding decorations
            categoryButtons.Find($"{category}Button").GetComponent<Button>().onClick.AddListener(() => {
                ClearAllChildrenInTransform(content);
                CreateButtonsForCategory(category, content);
            });
        }

        CreateButtonsForCategory(Categories.Buildings, content);

        BuildingController.currentBuildingTypeChanged += EvalueateIfTypeBarShouldBeShown;
        EvalueateIfTypeBarShouldBeShown(BuildingController.currentBuildingType);
    }

    private void EvalueateIfTypeBarShouldBeShown(BuildingType newBuildingType) {
        if (currentBuildingType == newBuildingType) return;
        currentBuildingType = newBuildingType;
        BuildingScriptableObject bso = Resources.Load<BuildingScriptableObject>($"BuildingScriptableObjects/{newBuildingType}");
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
        return bso.isMultipleType && !bso.isExpanded;
    }


    private void ClearAllChildrenInTransform(Transform transform) {
        for (int i = 0; i < transform.childCount; i++) Destroy(transform.GetChild(i).gameObject);
    }

    private void CreateButtonsForCategory(Categories category, Transform content) {
        foreach (BuildingType type in Enum.GetValues(typeof(BuildingType))) {
            BuildingScriptableObject bso = Resources.Load<BuildingScriptableObject>($"BuildingScriptableObjects/{type}");
            if (bso.category != category) {
                Resources.UnloadAsset(bso);
                continue;
            }

            if (bso.isExpanded) {
                GameObject[] buttons = MultipleTypeBuildingComponent.CreateButtonsForAllTypes(type);
                foreach (GameObject craftableButton in buttons) {
                    craftableButton.transform.SetParent(content);
                    craftableButton.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                }
            }
            else {
                GameObject button = Building.CreateBuildingButton(type);
                button.transform.SetParent(content);
                button.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            }

            Resources.UnloadAsset(bso);
        }
    }
}

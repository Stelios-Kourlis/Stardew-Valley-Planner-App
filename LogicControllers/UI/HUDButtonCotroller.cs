using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HUDButtonCotroller : MonoBehaviour {
    private readonly Type[] typesThatShouldBeInCraftables = { typeof(Sprinkler), typeof(Floor), typeof(Fence), typeof(Scarecrow), typeof(Craftables) };
    public void Awake() {
        //Action Buttons
        GameObject.FindWithTag("PlaceButton").GetComponent<Button>().onClick.AddListener(() => { ActionButtonPressed(GameObject.FindWithTag("PlaceButton"), Actions.PLACE); });
        GameObject.FindWithTag("DeleteButton").GetComponent<Button>().onClick.AddListener(() => { ActionButtonPressed(GameObject.FindWithTag("DeleteButton"), Actions.DELETE); });
        GameObject.FindWithTag("PickupButton").GetComponent<Button>().onClick.AddListener(() => { ActionButtonPressed(GameObject.FindWithTag("PickupButton"), Actions.EDIT); });
        GameObject.Find("CloseMenuButton").GetComponent<Button>().onClick.AddListener(() => { GameObject.Find("ActionButtons").GetComponent<FoldingMenuGroup>().ToggleMenu(); });

        //Tab Buttons
        // GameObject panelGameObject = GameObject.FindWithTag("Panel");
        GameObject panelGameObject = GameObject.Find("BuildingSelect").transform.Find("Rect").gameObject;
        // GameObject content = panelGameObject.transform.GetChild(0).GetChild(0).gameObject;
        GameObject content = panelGameObject.transform.Find("ScrollAreaBuildings").Find("Content").gameObject;

        panelGameObject.transform.Find("CategoryButtons").Find("BuildingsButton").gameObject.GetComponent<Button>().onClick.AddListener(() => {
            for (int i = 0; i < content.transform.childCount; i++) Destroy(content.transform.GetChild(i).gameObject);
            AddBuildingButtonsForPanel(content.transform);
            // GameObject.Find("BuildingSelect").transform.Find("Search").GetComponent<SearchBar>().OnValueChanged(GameObject.Find("BuildingSelect").transform.Find("Search").GetComponent<InputField>().text);
        });


        panelGameObject.transform.Find("CategoryButtons").Find("PlaceablesButton").gameObject.GetComponent<Button>().onClick.AddListener(() => {
            for (int i = 0; i < content.transform.childCount; i++) Destroy(content.transform.GetChild(i).gameObject);
            AddCraftablesButtonsForPanel(content.transform);
            // GameObject.Find("BuildingSelect").transform.Find("Search").GetComponent<SearchBar>().OnValueChanged(GameObject.Find("BuildingSelect").transform.Find("Search").GetComponent<InputField>().text);
        });

        panelGameObject.transform.Find("CategoryButtons").Find("CropsButton").gameObject.GetComponent<Button>().onClick.AddListener(() => {
            for (int i = 0; i < content.transform.childCount; i++) Destroy(content.transform.GetChild(i).gameObject);
            GameObject temp = new();
            GameObject[] buttons = temp.AddComponent<Crop>().CreateButtonsForAllTypes();
            foreach (GameObject button in buttons) {
                button.transform.SetParent(content.transform);
                button.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            }
            Destroy(temp);
            // GameObject.Find("BuildingSelect").transform.Find("Search").GetComponent<SearchBar>().OnValueChanged(GameObject.Find("BuildingSelect").transform.Find("Search").GetComponent<InputField>().text);
        });

        panelGameObject.transform.Find("CategoryButtons").Find("BuildingsButton").gameObject.GetComponent<Button>().onClick.Invoke();
        panelGameObject.transform.Find("CategoryButtons").Find("BuildingsButton").gameObject.GetComponent<Button>().Select();
    }




    private void ActionButtonPressed(GameObject button, Actions action) {
        if (GameObject.Find("ActionButtons").GetComponent<FoldingMenuGroup>().isOpen) {
            // Debug.Log("g");
            button.transform.SetAsLastSibling();
            BuildingController.SetCurrentAction(action);
        }
        GameObject.Find("ActionButtons").GetComponent<FoldingMenuGroup>().ToggleMenu();

    }

    /// <summary>
    /// Add buttons for the buildings tab to the panel
    /// </summary>
    private void AddBuildingButtonsForPanel(Transform BuildingContentTransform) {
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
            button.transform.SetParent(BuildingContentTransform);
            button.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            Destroy(temp);
        }
    }

    /// <summary>
    /// Add buttons for the craftables tab to the panel
    /// </summary>
    private void AddCraftablesButtonsForPanel(Transform BuildingContentTransform) {
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
                    craftableButton.transform.SetParent(BuildingContentTransform);
                    craftableButton.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                }
                Destroy(tempCraftables);
            }
            GameObject temp = new();
            Building buildingTemp = (Building)temp.AddComponent(type);
            GameObject button = buildingTemp.CreateBuildingButton();//todo Floor/Fence are missing icons
            button.transform.SetParent(BuildingContentTransform);
            button.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            Destroy(temp);
        }
    }
}

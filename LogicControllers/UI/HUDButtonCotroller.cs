using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HUDButtonCotroller : MonoBehaviour {
    private bool ActionButtonMenuIsOpen = false;
    public float SPEED = 500.0f; // Speed of the movement

    private GameObject[] ActionButtons = new GameObject[3];
    private readonly Type[] typesThatShouldBeInCraftables = { typeof(Sprinkler), typeof(Floor), typeof(Fence), typeof(Scarecrow), typeof(Craftables), typeof(Crop) };
    public void Awake() {
        //Action Buttons
        GameObject.FindWithTag("PlaceButton").GetComponent<Button>().onClick.AddListener(() => { ActionButtonPressed(GameObject.FindWithTag("PlaceButton"), Actions.PLACE); });
        GameObject.FindWithTag("DeleteButton").GetComponent<Button>().onClick.AddListener(() => { ActionButtonPressed(GameObject.FindWithTag("DeleteButton"), Actions.DELETE); });
        GameObject.FindWithTag("PickupButton").GetComponent<Button>().onClick.AddListener(() => { ActionButtonPressed(GameObject.FindWithTag("PickupButton"), Actions.EDIT); });

        ActionButtons[0] = GameObject.FindWithTag("PickupButton");
        ActionButtons[1] = GameObject.FindWithTag("DeleteButton");
        ActionButtons[2] = GameObject.FindWithTag("PlaceButton");

        //Tab Buttons
        GameObject panelGameObject = GameObject.FindWithTag("Panel");
        GameObject content = panelGameObject.transform.GetChild(0).GetChild(0).gameObject;

        panelGameObject.transform.Find("BuildingsButton").gameObject.GetComponent<Button>().onClick.AddListener(() => {
            for (int i = 0; i < content.transform.childCount; i++) Destroy(content.transform.GetChild(i).gameObject);
            AddBuildingButtonsForPanel(content.transform);
            panelGameObject.transform.Find("Search").GetComponent<SearchBar>().OnValueChanged(panelGameObject.transform.Find("Search").GetComponent<InputField>().text);
        });


        panelGameObject.transform.Find("PlaceablesButton").gameObject.GetComponent<Button>().onClick.AddListener(() => {
            for (int i = 0; i < content.transform.childCount; i++) Destroy(content.transform.GetChild(i).gameObject);
            AddCraftablesButtonsForPanel(content.transform);
            panelGameObject.transform.Find("Search").GetComponent<SearchBar>().OnValueChanged(panelGameObject.transform.Find("Search").GetComponent<InputField>().text);
        });

        panelGameObject.transform.Find("CropsButton").gameObject.GetComponent<Button>().onClick.AddListener(() => {
            for (int i = 0; i < content.transform.childCount; i++) Destroy(content.transform.GetChild(i).gameObject);
            GameObject temp = new();
            GameObject[] buttons = temp.AddComponent<Crop>().CreateButtonsForAllTypes();
            foreach (GameObject button in buttons) {
                button.transform.SetParent(content.transform);
                button.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            }
            Destroy(temp);
            panelGameObject.transform.Find("Search").GetComponent<SearchBar>().OnValueChanged(panelGameObject.transform.Find("Search").GetComponent<InputField>().text);
        });

        AddBuildingButtonsForPanel(content.transform);
    }

    public void ToggleActionButtonMenu() {
        if (ActionButtonMenuIsOpen) CloseActionButtonMenu();
        else OpenActionButtonMenu();
    }

    public void OpenActionButtonMenu() {
        Vector3 startPosition = GameObject.Find("ActionButtons").transform.Find("CloseMenuButton").position;
        float buttonWidth = GameObject.Find("ActionButtons").transform.Find("CloseMenuButton").GetComponent<RectTransform>().rect.width;
        for (int childIndex = 0; childIndex < ActionButtons.Count(); childIndex++) {
            Vector3 endPosition = startPosition - new Vector3((buttonWidth + 10) * (childIndex + 1), 0, 0);
            StartCoroutine(UIObjectMover.MoveObjectInConstantTime(ActionButtons[childIndex].transform, startPosition, endPosition, 0.5f));
        }

        ActionButtonMenuIsOpen = true;
    }

    public void CloseActionButtonMenu() {
        Vector3 endPosition = GameObject.Find("ActionButtons").transform.Find("CloseMenuButton").position;
        for (int childIndex = 0; childIndex < ActionButtons.Count(); childIndex++) {
            Vector3 startPosition = ActionButtons[childIndex].transform.position;
            StartCoroutine(UIObjectMover.MoveObjectInConstantTime(ActionButtons[childIndex].transform, startPosition, endPosition, 0.5f));
        }
        ActionButtonMenuIsOpen = false;
    }



    private void ActionButtonPressed(GameObject button, Actions action) {
        if (!ActionButtonMenuIsOpen) {
            button.transform.SetAsLastSibling();
            BuildingController.SetCurrentAction(action);
            CloseActionButtonMenu();
        }
        else OpenActionButtonMenu();

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
            GameObject button = buildingTemp.CreateBuildingButton();
            button.transform.SetParent(BuildingContentTransform);
            button.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            Destroy(temp);
        }
    }
}

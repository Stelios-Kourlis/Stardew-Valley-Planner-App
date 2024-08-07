using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Utility.ClassManager;
using static Utility.BuildingManager;
using static Utility.TilemapManager;
using UnityEngine.EventSystems;
using System.Configuration;
using System.Linq;
using UnityEngine.U2D;


public class BuildingButtonController : MonoBehaviour {

    readonly float BUTTON_SIZE = 75;

    private GameObject buttonPrefab;

    void Awake() {
        buttonPrefab = Resources.Load<GameObject>("UI/Button");
        // Debug.Log(buttonPrefab == null);
    }

    /// <summary>
    /// Create the buttons for a building
    /// </summary>
    /// <returns>The parent game object of the buttons</returns>
    public GameObject CreateButtonsForBuilding(Building building) {
        HashSet<ButtonTypes> buttonTypes = building.GetComponent<InteractableBuildingComponent>().BuildingInteractions;
        int numberOfButtons = buttonTypes.Count;
        if (numberOfButtons == 0) return null;

        //Create parent object for buttons
        GameObject buttonParent = new(building.BuildingName + "buttons");
        buttonParent.transform.parent = GetCanvasGameObject().transform;
        buttonParent.SetActive(false);
        buttonParent.transform.SetAsFirstSibling();

        //Create a button for each interaction
        int buttonNumber = 1;
        Vector3 middleOfBuildingScreen = Camera.main.WorldToScreenPoint(GetMiddleOfBuildingWorld(building));
        foreach (ButtonTypes buttonType in buttonTypes) {
            Vector3 buttonPositionScreen = CalculatePositionOfButton(numberOfButtons, buttonNumber, middleOfBuildingScreen);
            GameObject button = CreateInteractionButton(buttonType, building);
            button.transform.SetParent(buttonParent.transform);
            buttonNumber++;
        }
        // UpdateButtonPositionsAndScaleForBuilding(building);
        return buttonParent;
    }

    public void UpdateButtonPositionsAndScaleForBuilding(Building building) {
        if (BuildingController.isInsideBuilding.Key) return;
        float buttonScale = 10f / GetCamera().GetComponent<Camera>().orthographicSize;
        GameObject buttonParent = building.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject;
        buttonParent.transform.position = Camera.main.WorldToScreenPoint(GetMiddleOfBuildingWorld(building));
        for (int buttonIndex = 0; buttonIndex < buttonParent.transform.childCount; buttonIndex++) {
            buttonParent.transform.GetChild(buttonIndex).position = CalculatePositionOfButton(building.GetComponent<InteractableBuildingComponent>().BuildingInteractions.Count, buttonIndex + 1, Camera.main.WorldToScreenPoint(GetMiddleOfBuildingWorld(building)));
            buttonParent.transform.GetChild(buttonIndex).transform.localScale = new Vector3(buttonScale, buttonScale);
        }
    }

    private GameObject CreateInteractionButton(ButtonTypes type, Building building) {
        GameObject button = Instantiate(buttonPrefab);
        button.name = type.ToString();
        button.GetComponent<Image>().sprite = Resources.Load<Sprite>($"UI/{type}");
        // button.transform.position = buttonPositionScreen;

        if (type == ButtonTypes.PLACE_FISH) { // Add the fish icon to the button
            GameObject fishIcon = new("FishIcon");
            fishIcon.transform.SetParent(button.transform);
            fishIcon.transform.position = new Vector3(0, 0, 0);
            fishIcon.transform.localScale = new Vector3(0.4f, 0.4f);
            fishIcon.AddComponent<Image>().sprite = Resources.Load<SpriteAtlas>("Fish/FishAtlas").GetSprite("NO_FISH_OLD");//i prefer the old
        }

        button.GetComponent<RectTransform>().sizeDelta = new Vector2(BUTTON_SIZE, BUTTON_SIZE);
        AddButtonListener(type, button.GetComponent<Button>(), building);
        return button;
    }

    private void AddButtonListener(ButtonTypes type, Button button, Building building) {
        switch (type) {
            case ButtonTypes.TIER_ONE:
                button.onClick.AddListener(() => {
                    if (building.BuildingGameObject.TryGetComponent(out TieredBuildingComponent TieredBuildingComponent)) TieredBuildingComponent.SetTier(1);
                });
                break;
            case ButtonTypes.TIER_TWO:
                button.onClick.AddListener(() => {
                    if (building.BuildingGameObject.TryGetComponent(out TieredBuildingComponent TieredBuildingComponent)) TieredBuildingComponent.SetTier(2);
                });
                break;
            case ButtonTypes.TIER_THREE:
                button.onClick.AddListener(() => {
                    if (building.BuildingGameObject.TryGetComponent(out TieredBuildingComponent TieredBuildingComponent)) TieredBuildingComponent.SetTier(3);
                });
                break;
            case ButtonTypes.ENTER:
                button.onClick.AddListener(() => {
                    if (building.BuildingGameObject.TryGetComponent(out EnterableBuildingComponent enterableBuildingComponent)) enterableBuildingComponent.ToggleEditBuildingInterior();
                    else GetNotificationManager().SendNotification("WIP", NotificationManager.Icons.ErrorIcon);
                });
                break;
            case ButtonTypes.PAINT:
                button.onClick.AddListener(() => { GetNotificationManager().SendNotification("Not Implemented yet", NotificationManager.Icons.ErrorIcon); });//todo add building painting support
                break;
            case ButtonTypes.PLACE_FISH:
                if (building is FishPond fishPond) fishPond.CreateFishMenu();
                else throw new ArgumentException("Building MUST be fish pond to have this button type");
                button.onClick.AddListener(() => {
                    GameObject fishMenu = button.transform.GetChild(1).gameObject;
                    fishMenu.SetActive(!fishMenu.activeInHierarchy);
                });
                break;
            case ButtonTypes.CHANGE_FISH_POND_DECO:
                button.onClick.AddListener(() => {
                    if (building is FishPond fishPond) fishPond.CycleFishPondDeco();
                });
                break;
            case ButtonTypes.ADD_ANIMAL:
                button.onClick.AddListener(() => {
                    if (building.BuildingGameObject.TryGetComponent(out AnimalHouseComponent animalHouseComponent)) animalHouseComponent.ToggleAnimalMenu();
                });
                break;
            default:
                throw new ArgumentException("This should never happen");
        }
    }

    /// <summary>
    /// Calculate the position of a button based on the number of buttons and the number of the button
    /// </summary>
    private Vector3 CalculatePositionOfButton(int numberOfButtons, int buttonNumber, Vector3 middleOfBuildingScreen) {

        float BUTTON_OFFSET = 5 * BUTTON_SIZE / 4 * (7.5f / GetCamera().GetComponent<Camera>().orthographicSize);
        //Debug.Log("Finding position of button "+buttonNumber+"/"+numberOfButtons + ", offset = "+BUTTON_OFFSET + ", middleOfBuildingScreen = "+middleOfBuildingScreen);
        if (numberOfButtons <= 0) throw new ArgumentException("numberOfButtons must be greater than 0");
        if (numberOfButtons > Enum.GetValues(typeof(ButtonTypes)).Length) throw new ArgumentException($"numberOfButtons must be less than {Enum.GetValues(typeof(ButtonTypes)).Length} (got {numberOfButtons})");

        return numberOfButtons switch {
            1 => middleOfBuildingScreen,
            2 => buttonNumber switch {
                1 => new Vector3(middleOfBuildingScreen.x - BUTTON_OFFSET, middleOfBuildingScreen.y),
                2 => new Vector3(middleOfBuildingScreen.x + BUTTON_OFFSET, middleOfBuildingScreen.y),
                _ => throw new ArgumentException("This should never happen")
                // _ => middleOfBuildingScreen
            },
            3 => buttonNumber switch {
                1 => new Vector3(middleOfBuildingScreen.x, middleOfBuildingScreen.y + BUTTON_OFFSET),
                2 => new Vector3(middleOfBuildingScreen.x + BUTTON_OFFSET, middleOfBuildingScreen.y - BUTTON_OFFSET),
                3 => new Vector3(middleOfBuildingScreen.x - BUTTON_OFFSET, middleOfBuildingScreen.y - BUTTON_OFFSET),
                _ => throw new ArgumentException("This should never happen")
            },
            4 => buttonNumber switch {
                1 => new Vector3(middleOfBuildingScreen.x, middleOfBuildingScreen.y + BUTTON_OFFSET),
                2 => new Vector3(middleOfBuildingScreen.x + BUTTON_OFFSET, middleOfBuildingScreen.y),
                3 => new Vector3(middleOfBuildingScreen.x, middleOfBuildingScreen.y - BUTTON_OFFSET),
                4 => new Vector3(middleOfBuildingScreen.x - BUTTON_OFFSET, middleOfBuildingScreen.y),
                _ => throw new ArgumentException("This should never happen")
            },
            5 => buttonNumber switch {
                1 => new Vector3(middleOfBuildingScreen.x, middleOfBuildingScreen.y + BUTTON_OFFSET),
                2 => new Vector3(middleOfBuildingScreen.x + BUTTON_OFFSET, middleOfBuildingScreen.y),
                3 => new Vector3(middleOfBuildingScreen.x, middleOfBuildingScreen.y - BUTTON_OFFSET),
                4 => new Vector3(middleOfBuildingScreen.x - BUTTON_OFFSET, middleOfBuildingScreen.y),
                5 => middleOfBuildingScreen,
                _ => throw new ArgumentException("This should never happen")
            },
            6 => buttonNumber switch {
                6 => new Vector3(middleOfBuildingScreen.x - BUTTON_OFFSET, middleOfBuildingScreen.y - BUTTON_OFFSET / 2),
                4 => new Vector3(middleOfBuildingScreen.x - BUTTON_OFFSET, middleOfBuildingScreen.y + BUTTON_OFFSET / 2),
                5 => new Vector3(middleOfBuildingScreen.x, middleOfBuildingScreen.y + BUTTON_OFFSET),
                1 => new Vector3(middleOfBuildingScreen.x + BUTTON_OFFSET, middleOfBuildingScreen.y + BUTTON_OFFSET / 2),
                2 => new Vector3(middleOfBuildingScreen.x + BUTTON_OFFSET, middleOfBuildingScreen.y - BUTTON_OFFSET / 2),
                3 => new Vector3(middleOfBuildingScreen.x, middleOfBuildingScreen.y - BUTTON_OFFSET),
                _ => throw new ArgumentException("This should never happen")
            },
            _ => throw new ArgumentException("This should never happen")
        };

    }

}

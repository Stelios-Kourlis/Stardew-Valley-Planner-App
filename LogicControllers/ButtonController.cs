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


public class ButtonController : MonoBehaviour{

    readonly float BUTTON_SIZE = 75;
    private readonly Type[] typesThatShouldBeInCraftables = {typeof(Sprinkler), typeof(Floor), typeof(Fence), typeof(Scarecrow), typeof(Craftables), typeof(Crop)};
    
    void Start(){
        //Tab Buttons
        GameObject panelGameObject = GameObject.FindWithTag("Panel");
        GameObject content = panelGameObject.transform.GetChild(0).GetChild(0).gameObject;
        panelGameObject.transform.Find("BuildingsButton").gameObject.GetComponent<Button>().onClick.AddListener(() => {
            for(int i = 0; i < content.transform.childCount; i++) Destroy(content.transform.GetChild(i).gameObject);
            AddBuildingButtonsForPanel(content.transform);
            panelGameObject.transform.Find("Search").GetComponent<SearchBar>().OnValueChanged(panelGameObject.transform.Find("Search").GetComponent<InputField>().text);
        });

        
        panelGameObject.transform.Find("PlaceablesButton").gameObject.GetComponent<Button>().onClick.AddListener(() => {
            for(int i = 0; i < content.transform.childCount; i++) Destroy(content.transform.GetChild(i).gameObject);
            AddCraftablesButtonsForPanel(content.transform);
            panelGameObject.transform.Find("Search").GetComponent<SearchBar>().OnValueChanged(panelGameObject.transform.Find("Search").GetComponent<InputField>().text);
        });

        panelGameObject.transform.Find("CropsButton").gameObject.GetComponent<Button>().onClick.AddListener(() => {
            for(int i = 0; i < content.transform.childCount; i++) Destroy(content.transform.GetChild(i).gameObject);
            GameObject temp = new();
            GameObject[] buttons = temp.AddComponent<Crop>().CreateButtonsForAllTypes();
            foreach (GameObject button in buttons){
                    button.transform.SetParent(content.transform);
                    button.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
            }
            Destroy(temp);
            panelGameObject.transform.Find("Search").GetComponent<SearchBar>().OnValueChanged(panelGameObject.transform.Find("Search").GetComponent<InputField>().text);
        });

        AddBuildingButtonsForPanel(content.transform);

        //Action Buttons
        GameObject.FindWithTag("PlaceButton").GetComponent<Button>().onClick.AddListener(() => { Building.CurrentAction =Actions.PLACE; });
        GameObject.FindWithTag("DeleteButton").GetComponent<Button>().onClick.AddListener(() => { Building.CurrentAction =Actions.DELETE; });
        GameObject.FindWithTag("PickupButton").GetComponent<Button>().onClick.AddListener(() => { Building.CurrentAction =Actions.EDIT; });
        
    }

    public void CreateButtonsForBuilding(Building building){
        ButtonTypes[] buttonTypes = building.BuildingInteractions;
        int numberOfButtons = buttonTypes.Length;
        if (numberOfButtons == 0) return;
        Vector3 middleOfBuildingWorld = GetMiddleOfBuildingWorld(building);
        Vector3 middleOfBuildingScreen = Camera.main.WorldToScreenPoint(middleOfBuildingWorld);
        GameObject buttonParent = new(building.name+"buttons");
        buttonParent.transform.parent = GetCanvasGameObject().transform;
        buttonParent.SetActive(false);
        buttonParent.transform.SetAsFirstSibling();
        building.buttonParent = buttonParent;

        int buttonNumber = 1;
        foreach (ButtonTypes buttonType in buttonTypes){
            Vector3 buttonPositionScreen = CalculatePositionOfButton(numberOfButtons, buttonNumber, middleOfBuildingScreen);
            CreateInteractionButton(buttonType, middleOfBuildingScreen, buttonParent, building);
            buttonNumber++;
        } 
        
    }

    public void UpdateButtonPositionsAndScaleForBuilding(Building building){
        float buttonScale = 10f/GetCamera().GetComponent<Camera>().orthographicSize;
        GameObject buttonParent = building.buttonParent;
        for (int buttonIndex = 0; buttonIndex < buttonParent.transform.childCount; buttonIndex++){
            buttonParent.transform.GetChild(buttonIndex).position = CalculatePositionOfButton(building.BuildingInteractions.Length, buttonIndex+1, Camera.main.WorldToScreenPoint(GetMiddleOfBuildingWorld(building)));
            buttonParent.transform.GetChild(buttonIndex).transform.localScale = new Vector3(buttonScale,buttonScale);
        } 
    }

    private void CreateInteractionButton(ButtonTypes type, Vector3 buttonPositionScreen, GameObject buttonParent, Building building){
        GameObject button = new(type.ToString());
        button.transform.parent = buttonParent.transform;
        button.transform.position = buttonPositionScreen;
        button.AddComponent<Image>().sprite = Resources.Load<Sprite>("UI/"+type.ToString());
        if (type == ButtonTypes.PLACE_FISH){
            GameObject fishIcon = new("FishIcon");
            fishIcon.transform.parent = button.transform;
            fishIcon.transform.position = buttonPositionScreen;
            fishIcon.transform.localScale = new Vector3(0.4f,0.4f);
            fishIcon.AddComponent<Image>().sprite = Resources.Load<SpriteAtlas>("Fish/FishAtlas").GetSprite("NO_FISH_OLD");//i prefer the old
        }
        button.GetComponent<RectTransform>().sizeDelta = new Vector2(BUTTON_SIZE, BUTTON_SIZE);
        float buttonScale = 10f/GetCamera().GetComponent<Camera>().orthographicSize;
        button.transform.localScale = new Vector3(buttonScale,buttonScale);
        Button buttonComponent = button.AddComponent<Button>();
        AddButtonListener(type, buttonComponent, building);
    }

    private void AddButtonListener(ButtonTypes type, Button button, Building building){
        switch(type){
            case ButtonTypes.TIER_ONE:
                button.onClick.AddListener(() => {
                    if (building is ITieredBuilding tieredBuilding) tieredBuilding.SetTier(1);
                 });
                break;
            case ButtonTypes.TIER_TWO:
                button.onClick.AddListener(() => {
                    if (building is ITieredBuilding tieredBuilding) tieredBuilding.SetTier(2);
                });
                break;
            case ButtonTypes.TIER_THREE:
                button.onClick.AddListener(() => {
                    if (building is ITieredBuilding tieredBuilding) tieredBuilding.SetTier(3);
                 });
                break;
            case ButtonTypes.ENTER:
                button.onClick.AddListener(() => { 
                    if (building is IEnterableBuilding enterableBuilding) enterableBuilding.ToggleBuildingInterior();  
                    });
                break;
            case ButtonTypes.PAINT:
                button.onClick.AddListener(() => { GetNotificationManager().SendNotification("Not Implemented yet"); });//todo add building painting support
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
                    if (building is FishPond fishPond){
                        fishPond.CycleFishPondDeco();
                    }
                 });
                break;
            case ButtonTypes.ADD_ANIMAL:
                button.onClick.AddListener(() => { 
                    if (building is IAnimalHouse animalBuilding){
                        animalBuilding.ToggleAnimalMenu();
                    }
                 });
                break;
            default:
                throw new ArgumentException("This should never happen");
        }
    }

    private void AddBuildingButtonsForPanel(Transform BuildingContentTransform){
        var buildingType = typeof(Building);
        var allTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => buildingType.IsAssignableFrom(p) && p.IsClass && !p.IsAbstract);
        foreach (var type in allTypes){
            if (type == typeof(House)) continue; //these 2 should not be available to build
            if (type == typeof(Greenhouse)) continue;
            if (typesThatShouldBeInCraftables.Contains(type)) continue; //if its not any of these dont add it
            GameObject temp = new();
            Building buildingTemp = (Building) temp.AddComponent(type);
            GameObject button = buildingTemp.CreateButton();
            button.transform.SetParent(BuildingContentTransform);
            button.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
            Destroy(temp);
        }
    }

    private void AddCraftablesButtonsForPanel(Transform BuildingContentTransform){
        var buildingType = typeof(Building);
        var allTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => buildingType.IsAssignableFrom(p) && p.IsClass && !p.IsAbstract);
        foreach (var type in allTypes){
            if (type == typeof(House)) continue; //these 2 should not be available to build
            if (type == typeof(Greenhouse)) continue;
            if (!typesThatShouldBeInCraftables.Contains(type)) continue; //only add craftables
            if (type == typeof(Craftables)){
                GameObject tempCraftables = new("CraftablesTypes");
                GameObject[] buttons = tempCraftables.AddComponent<Craftables>().CreateButtonsForAllTypes();
                foreach (GameObject craftableButton in buttons){
                    craftableButton.transform.SetParent(BuildingContentTransform);
                    craftableButton.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
                }
                Destroy(tempCraftables);
            }
            GameObject temp = new();
            Building buildingTemp = (Building) temp.AddComponent(type);
            GameObject button = buildingTemp.CreateButton();
            button.transform.SetParent(BuildingContentTransform);
            button.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
            Destroy(temp);
        }
    }

    // private void AddFishMenuObject(Button button, Building building){
    //     GameObject fishMenuPrefab = Resources.Load<GameObject>("UI/FishMenu");
    //     GameObject fishMenu = Instantiate(fishMenuPrefab);
    //     fishMenu.transform.SetParent(button.transform);
    //     Vector3 fishMenuPositionWorld = new(building.Tilemap.CellToWorld(building.BaseCoordinates[0] + new Vector3Int(1,0,0)).x, GetMiddleOfBuildingWorld(building).y);
    //     fishMenu.transform.position = Camera.main.WorldToScreenPoint(fishMenuPositionWorld);
    //     fishMenu.GetComponent<RectTransform>().localScale = new Vector2(1, 1);
    //     fishMenu.SetActive(false);
    //     GameObject fishMenuContent = fishMenu.transform.GetChild(0).GetChild(0).gameObject;
    //     //FishPond fishPond = building as FishPond;
    //     for (int childIndex = 0; childIndex < fishMenuContent.transform.childCount; childIndex++){
    //         Button fishButton = fishMenuContent.transform.GetChild(childIndex).GetComponent<Button>();
    //         AddHoverEffect(fishButton);
    //         fishButton.onClick.AddListener(() => {
    //             Fish fishType = (Fish)Enum.Parse(typeof(Fish), fishButton.GetComponent<Image>().sprite.name);
    //             // fishPond.SetFishImage(fishType);
    //         });
    //     }
    // }

    private Vector3 CalculatePositionOfButton(int numberOfButtons, int buttonNumber, Vector3 middleOfBuildingScreen){
        
        float BUTTON_OFFSET = 5 * BUTTON_SIZE / 4 * (7.5f/GetCamera().GetComponent<Camera>().orthographicSize);
        //Debug.Log("Finding position of button "+buttonNumber+"/"+numberOfButtons + ", offset = "+BUTTON_OFFSET + ", middleOfBuildingScreen = "+middleOfBuildingScreen);
        if (numberOfButtons <= 0) throw new ArgumentException("numberOfButtons must be greater than 0");
        if (numberOfButtons > Enum.GetValues(typeof(ButtonTypes)).Length) throw new ArgumentException("numberOfButtons must be less than 5");
        
        return numberOfButtons switch{
            1 => middleOfBuildingScreen,
            2 => buttonNumber switch{
                1 => new Vector3(middleOfBuildingScreen.x - BUTTON_OFFSET, middleOfBuildingScreen.y),
                2 => new Vector3(middleOfBuildingScreen.x + BUTTON_OFFSET, middleOfBuildingScreen.y),
                _ => throw new ArgumentException("This should never happen")
                // _ => middleOfBuildingScreen
                },
            3 => buttonNumber switch{
                1 => new Vector3(middleOfBuildingScreen.x, middleOfBuildingScreen.y + BUTTON_OFFSET),
                2 => new Vector3(middleOfBuildingScreen.x + BUTTON_OFFSET, middleOfBuildingScreen.y - BUTTON_OFFSET),
                3 => new Vector3(middleOfBuildingScreen.x - BUTTON_OFFSET, middleOfBuildingScreen.y - BUTTON_OFFSET),
                _ => throw new ArgumentException("This should never happen")
                },
            4 => buttonNumber switch{
                1 => new Vector3(middleOfBuildingScreen.x, middleOfBuildingScreen.y + BUTTON_OFFSET),
                2 => new Vector3(middleOfBuildingScreen.x + BUTTON_OFFSET, middleOfBuildingScreen.y),
                3 => new Vector3(middleOfBuildingScreen.x, middleOfBuildingScreen.y - BUTTON_OFFSET),
                4 => new Vector3(middleOfBuildingScreen.x - BUTTON_OFFSET, middleOfBuildingScreen.y),
                _ => throw new ArgumentException("This should never happen")
                },
            5 => buttonNumber switch{
                1 => new Vector3(middleOfBuildingScreen.x, middleOfBuildingScreen.y + BUTTON_OFFSET),
                2 => new Vector3(middleOfBuildingScreen.x + BUTTON_OFFSET, middleOfBuildingScreen.y),
                3 => new Vector3(middleOfBuildingScreen.x, middleOfBuildingScreen.y - BUTTON_OFFSET),
                4 => new Vector3(middleOfBuildingScreen.x - BUTTON_OFFSET, middleOfBuildingScreen.y),
                5 => middleOfBuildingScreen,
                _ => throw new ArgumentException("This should never happen")
            },
            6 => buttonNumber switch{
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Utility.ClassManager;
using static Utility.BuildingManager;
using static Utility.TilemapManager;

public class ButtonController : MonoBehaviour{

    readonly float BUTTON_SIZE = 75;
    
    // Start is called before the first frame update
    void Start(){

        Transform buildingPanelTransform = GameObject.FindWithTag("Panel").transform.GetChild(0).GetChild(0);
        CreateButton("logCabin", "Buildings/LogCabinT1", typeof(LogCabinT1), buildingPanelTransform);
        CreateButton("stoneCabin", "Buildings/StoneCabinT1", typeof(StoneCabinT1), buildingPanelTransform);
        CreateButton("plankCabin", "Buildings/PlankCabinT1", typeof(PlankCabinT1), buildingPanelTransform);
        CreateButton("coop", "Buildings/CoopT1", typeof(Coop), buildingPanelTransform);
        CreateButton("barn", "Buildings/BarnT1", typeof(Barn), buildingPanelTransform);
        CreateButton("goldClock", "Buildings/GoldClock", typeof(GoldClock), buildingPanelTransform);
        CreateButton("silo", "Buildings/Silo", typeof(Silo), buildingPanelTransform);
        CreateButton("slimeHutch", "Buildings/SlimeHutch", typeof(SlimeHutch), buildingPanelTransform);
        CreateButton("shippingBin", "Buildings/ShippingBin", typeof(ShippingBin), buildingPanelTransform);
        CreateButton("well", "Buildings/Well", typeof(Well), buildingPanelTransform);
        CreateButton("earthObelisk", "Buildings/EarthObelisk", typeof(EarthObelisk), buildingPanelTransform);
        CreateButton("desertObelisk", "Buildings/DesertObelisk", typeof(DesertObelisk), buildingPanelTransform);
        CreateButton("stable", "Buildings/Stable", typeof(Stable), buildingPanelTransform);
        CreateButton("islandObelisk", "Buildings/IslandObelisk", typeof(IslandObelisk), buildingPanelTransform);
        CreateButton("waterObelisk", "Buildings/WaterObelisk", typeof(WaterObelisk), buildingPanelTransform);
        CreateButton("junimoHunt", "Buildings/JunimoHut", typeof(JunimoHut), buildingPanelTransform);
        CreateButton("greenhouse", "Buildings/Greenhouse", typeof(Greenhouse), buildingPanelTransform);
        CreateButton("mill", "Buildings/Mill", typeof(Mill), buildingPanelTransform);
        CreateButton("fishPond", "Buildings/FishPond", typeof(FishPond), buildingPanelTransform);
        CreateButton("floor", "Buildings/WoodFloor", typeof(Floor), buildingPanelTransform);

        GameObject.FindWithTag("PlaceButton").GetComponent<Button>().onClick.AddListener(() => { GetBuildingController().SetCurrentAction(Actions.PLACE); });
        GameObject.FindWithTag("DeleteButton").GetComponent<Button>().onClick.AddListener(() => { GetBuildingController().SetCurrentAction(Actions.DELETE); });
        GameObject.FindWithTag("PickupButton").GetComponent<Button>().onClick.AddListener(() => { GetBuildingController().SetCurrentAction(Actions.EDIT); });

        Transform floorBarTransform = GameObject.FindWithTag("FloorSelectBar").transform.GetChild(0).GetChild(0);
        CreateButton("WoodFloor", "Floors/WoodFloor", typeof(Floor), floorBarTransform, FloorType.WOOD_FLOOR);
        CreateButton("RusticPlankFloor", "Floors/RusticPlankFloor", typeof(Floor), floorBarTransform, FloorType.RUSTIC_PLANK_FLOOR);
        CreateButton("StrawFloor", "Floors/StrawFloor", typeof(Floor), floorBarTransform, FloorType.STRAW_FLOOR);
        CreateButton("WeatheredFloor", "Floors/WeatheredFloor", typeof(Floor), floorBarTransform, FloorType.WEATHERED_FLOOR);
        CreateButton("CrystalFloor", "Floors/CrystalFloor", typeof(Floor), floorBarTransform, FloorType.CRYSTAL_FLOOR);
        CreateButton("StoneFloor", "Floors/StoneFloor", typeof(Floor), floorBarTransform, FloorType.STONE_FLOOR);
        CreateButton("StoneWalkwayFloor", "Floors/StoneWalkwayFloor", typeof(Floor), floorBarTransform, FloorType.STONE_WALKWAY_FLOOR);
        CreateButton("BrickFloor", "Floors/BrickFloor", typeof(Floor), floorBarTransform, FloorType.BRICK_FLOOR);
        CreateButton("WoodPath", "Floors/WoodPath", typeof(Floor), floorBarTransform, FloorType.WOOD_PATH);
        CreateButton("GravelPath", "Floors/GravelPath", typeof(Floor), floorBarTransform, FloorType.GRAVEL_PATH);
        CreateButton("CobblestonePath", "Floors/CobblestonePath", typeof(Floor), floorBarTransform, FloorType.COBBLESTONE_PATH);
        CreateButton("SteppingStonePath", "Floors/SteppingStonePath", typeof(Floor), floorBarTransform, FloorType.STEPPING_STONE_PATH);
        CreateButton("CrystalPath", "Floors/CrystalPath", typeof(Floor), floorBarTransform, FloorType.CRYSTAL_PATH);
        
    }

    public void CreateButtonsForBuilding(Building building){
        GameObject parentGameObject = building.gameObject;//todo fix this
        ButtonTypes[] buttonTypes = building.buildingInteractions;
        int numberOfButtons = buttonTypes.Length;
        if (numberOfButtons == 0) return;
        Vector3 middleOfBuildingWorld = GetMiddleOfBuildingWorld(building);
        Vector3 middleOfBuildingScreen = Camera.main.WorldToScreenPoint(middleOfBuildingWorld);
        GameObject buttonParent = new GameObject(building.name+"buttons");
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

    public void UpdateButtonPositionsAndScaleForBuilding(Building building){//todo fix this
        float buttonScale = 10f/GetCamera().GetComponent<Camera>().orthographicSize;
        GameObject buttonParent = building.buttonParent;
        for (int buttonIndex = 0; buttonIndex < buttonParent.transform.childCount; buttonIndex++){
            buttonParent.transform.GetChild(buttonIndex).position = CalculatePositionOfButton(building.buildingInteractions.Length, buttonIndex+1, Camera.main.WorldToScreenPoint(GetMiddleOfBuildingWorld(building)));
            buttonParent.transform.GetChild(buttonIndex).transform.localScale = new Vector3(buttonScale,buttonScale);
        } 
    }

    private void CreateInteractionButton(ButtonTypes type, Vector3 buttonPositionScreen, GameObject buttonParent, Building building){
        GameObject button = new GameObject(type.ToString());
        button.transform.parent = buttonParent.transform;
        button.transform.position = buttonPositionScreen;
        button.AddComponent<Image>().sprite = Resources.Load<Sprite>("UI/"+type.ToString());
        if (type == ButtonTypes.PLACE_FISH){
            GameObject fishIcon = new GameObject("FishIcon");
            fishIcon.transform.parent = button.transform;
            fishIcon.transform.position = buttonPositionScreen;
            fishIcon.transform.localScale = new Vector3(0.4f,0.4f);
            fishIcon.AddComponent<Image>().sprite = Resources.Load<Sprite>("Fish/NO_FISH");
        }
        button.GetComponent<RectTransform>().sizeDelta = new Vector2(BUTTON_SIZE, BUTTON_SIZE);
        float buttonScale = 10f/GetCamera().GetComponent<Camera>().orthographicSize;
        button.transform.localScale = new Vector3(buttonScale,buttonScale);
        Button buttonComponent = button.AddComponent<Button>();
        AddButtonListener(type, buttonComponent, building);
    }

    private void AddButtonListener(ButtonTypes type, Button button, Building building){
        if (type == ButtonTypes.PLACE_FISH){
            GameObject fishMenuPrefab = Resources.Load<GameObject>("UI/FishMenu");
            GameObject fishMenu = Instantiate(fishMenuPrefab);
            fishMenu.transform.SetParent(button.transform);
            Vector3 fishMenuPositionWorld = new Vector3(building.tilemap.CellToWorld(building.baseCoordinates[0] + new Vector3Int(1,0,0)).x, GetMiddleOfBuildingWorld(building).y);
            fishMenu.transform.position = Camera.main.WorldToScreenPoint(fishMenuPositionWorld);
            fishMenu.GetComponent<RectTransform>().localScale = new Vector2(1, 1);
            fishMenu.SetActive(false);
            GameObject fishMenuContent = fishMenu.transform.GetChild(0).GetChild(0).gameObject;
            for (int childIndex = 0; childIndex < fishMenuContent.transform.childCount; childIndex++){
                fishMenuContent.transform.GetChild(childIndex).GetComponent<FishImageRetriever>().SetBuilding(building);
            }
        }
        switch(type){
            case ButtonTypes.TIER_ONE:
                if (building is House)button.onClick.AddListener(() => {GetBuildingController().PlaceHouse(1); });
                else button.onClick.AddListener(() => { AddTierChangeListener(1, building); });
                break;
            case ButtonTypes.TIER_TWO:
                if (building is House) button.onClick.AddListener(() => {GetBuildingController().PlaceHouse(2); });
                else button.onClick.AddListener(() => { AddTierChangeListener(2, building); });
                break;
            case ButtonTypes.TIER_THREE:
                if (building is House) button.onClick.AddListener(() => {GetBuildingController().PlaceHouse(3); });
                else button.onClick.AddListener(() => { AddTierChangeListener(3, building); });
                break;
            case ButtonTypes.ENTER:
                button.onClick.AddListener(() => { /* Add valid statement here */ });//todo add building insides
                break;
            case ButtonTypes.PAINT:
                button.onClick.AddListener(() => { /* Add valid statement here */ });//todo add building painting support
                break;
            case ButtonTypes.PLACE_FISH:
                button.onClick.AddListener(() => { 
                    bool isObjectActive =  button.transform.GetChild(1).gameObject.activeInHierarchy;
                    button.transform.GetChild(1).gameObject.SetActive(!isObjectActive);
                 });
                break;
            case ButtonTypes.CHANGE_FISH_POND_DECO:
                button.onClick.AddListener(() => { 
                    GetBuildingController().CycleFishPondDeco(building);
                 });
                break;
            default:
                throw new ArgumentException("This should never happen");
        }
    }

    private void AddTierChangeListener(int tier, Building building){
        GetBuildingController().DeleteBuilding(building.baseCoordinates[0]);
        string buildingType = building.GetType().ToString();
        string tieredBuildingType = buildingType.Substring(0, buildingType.Length - 1) + tier;
        Building tieredBuilding = Activator.CreateInstance(Type.GetType(tieredBuildingType), null, null, null) as Building;
        //GetBuildingController().PlaceBuilding(tieredBuilding, building.baseCoordinates[0]);//todo fix this
    }

    private Vector3 CalculatePositionOfButton(int numberOfButtons, int buttonNumber, Vector3 middleOfBuildingScreen){
        
        float BUTTON_OFFSET = 5 * BUTTON_SIZE / 4 * (7.5f/GetCamera().GetComponent<Camera>().orthographicSize);
        //Debug.Log("Finding position of button "+buttonNumber+"/"+numberOfButtons + ", offset = "+BUTTON_OFFSET + ", middleOfBuildingScreen = "+middleOfBuildingScreen);
        if (numberOfButtons <= 0) throw new ArgumentException("numberOfButtons must be greater than 0");
        if (numberOfButtons > 5) throw new ArgumentException("numberOfButtons must be less than 5");
        
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
            _ => throw new ArgumentException("This should never happen")
        };

    }
}

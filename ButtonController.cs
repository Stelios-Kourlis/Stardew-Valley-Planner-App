using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Utility.ClassManager;
using static Utility.BuildingManager;
using static Utility.TilemapManager;
using UnityEngine.EventSystems;


public class ButtonController : MonoBehaviour{

    readonly float BUTTON_SIZE = 75;
    
    void Start(){
        //Tab Buttons
        GameObject panelGameObject = GameObject.FindWithTag("Panel");
        panelGameObject.transform.Find("BuildingsButton").gameObject.GetComponent<Button>().onClick.AddListener(() => {
            panelGameObject.transform.Find("ScrollAreaBuildings").gameObject.SetActive(true);
            panelGameObject.transform.Find("ScrollAreaPlaceables").gameObject.SetActive(false);
            panelGameObject.transform.Find("Search").GetComponent<SearchBar>().OnValueChanged(panelGameObject.transform.Find("Search").GetComponent<InputField>().text);
        });
        panelGameObject.transform.Find("PlaceablesButton").gameObject.GetComponent<Button>().onClick.AddListener(() => {
            panelGameObject.transform.Find("ScrollAreaBuildings").gameObject.SetActive(false);
            panelGameObject.transform.Find("ScrollAreaPlaceables").gameObject.SetActive(true);
            panelGameObject.transform.Find("Search").GetComponent<SearchBar>().OnValueChanged(panelGameObject.transform.Find("Search").GetComponent<InputField>().text);
        });

        //Building Buttons
        Transform buildingPanelTransform = GameObject.FindWithTag("Panel").transform.GetChild(0).GetChild(0);
        CreateButton("logCabin", "Buildings/LogCabinT1", typeof(Cabin), buildingPanelTransform);
        CreateButton("stoneCabin", "Buildings/StoneCabinT1", typeof(Cabin), buildingPanelTransform);
        CreateButton("plankCabin", "Buildings/PlankCabinT1", typeof(Cabin), buildingPanelTransform);
        CreateButton("coop", "Buildings/Coop", typeof(Coop), buildingPanelTransform);
        CreateButton("barn", "Buildings/Barn", typeof(Barn), buildingPanelTransform);
        CreateButton("shed", "Buildings/ShedT1", typeof(Shed), buildingPanelTransform);
        CreateButton("goldClock", "Buildings/GoldClock", typeof(GoldClock), buildingPanelTransform);
        CreateButton("silo", "Buildings/Silo", typeof(Silo), buildingPanelTransform);
        CreateButton("slimeHutch", "Buildings/SlimeHutch", typeof(SlimeHutch), buildingPanelTransform);
        CreateButton("shippingBin", "Buildings/ShippingBin", typeof(ShippingBin), buildingPanelTransform);
        CreateButton("well", "Buildings/Well", typeof(Well), buildingPanelTransform);
        CreateButton("earthObelisk", "Buildings/EarthObelisk", typeof(Obelisk), buildingPanelTransform);
        CreateButton("stable", "Buildings/Stable", typeof(Stable), buildingPanelTransform);
        CreateButton("junimoHunt", "Buildings/JunimoHut", typeof(JunimoHut), buildingPanelTransform);
        CreateButton("greenhouse", "Buildings/Greenhouse", typeof(Greenhouse), buildingPanelTransform);
        CreateButton("mill", "Buildings/Mill", typeof(Mill), buildingPanelTransform);
        CreateButton("fishPond", "Buildings/FishPond", typeof(FishPond), buildingPanelTransform);
        CreateButton("sprinkler", "Buildings/SprinklerT1", typeof(Sprinkler), buildingPanelTransform);
        CreateButton("scarecrow", "Buildings/Scarecrows", typeof(Scarecrow), buildingPanelTransform);
        CreateButton("floor", "Buildings/WoodFloor", typeof(Floor), buildingPanelTransform);

        //Placeables Buttons
        buildingPanelTransform = GameObject.FindWithTag("Panel").transform.GetChild(1).GetChild(0);
        CreateButton("Beehouse", "Buildings/Placeables/Beehouse", buildingPanelTransform, CraftableUtility.Beehouse);
        CreateButton("Bonemill", "Buildings/Placeables/Bonemill", buildingPanelTransform, CraftableUtility.BoneMill);
        CreateButton("Cask", "Buildings/Placeables/Cask", buildingPanelTransform, CraftableUtility.Cask);
        CreateButton("CharcoalKiln", "Buildings/Placeables/CharcoalKiln", buildingPanelTransform, CraftableUtility.CharcoalKiln);
        CreateButton("CheesePress", "Buildings/Placeables/CheesePress", buildingPanelTransform, CraftableUtility.CheesePress);
        CreateButton("Crystalarium", "Buildings/Placeables/Crystalarium", buildingPanelTransform, CraftableUtility.Crystalarium);
        CreateButton("FarmComputer", "Buildings/Placeables/FarmComputer", buildingPanelTransform, CraftableUtility.FarmComputer);
        CreateButton("Furnace", "Buildings/Placeables/Furnace", buildingPanelTransform, CraftableUtility.Furnace);
        CreateButton("GeodeCrusher", "Buildings/Placeables/GeodeCrusher", buildingPanelTransform, CraftableUtility.GeodeCrusher);
        CreateButton("Hopper", "Buildings/Placeables/Hopper", buildingPanelTransform, CraftableUtility.Hopper);
        CreateButton("Keg", "Buildings/Placeables/Keg", buildingPanelTransform, CraftableUtility.Keg);
        CreateButton("LightningRod", "Buildings/Placeables/LightningRod", buildingPanelTransform, CraftableUtility.LightningRod);
        CreateButton("Loom", "Buildings/Placeables/Loom", buildingPanelTransform, CraftableUtility.Loom);
        CreateButton("MayonnaiseMachine", "Buildings/Placeables/MayonnaiseMachine", buildingPanelTransform, CraftableUtility.MayonnaiseMachine);
        CreateButton("MiniObelisk", "Buildings/Placeables/MiniObelisk", buildingPanelTransform, CraftableUtility.MiniObelisk);
        CreateButton("OilMaker", "Buildings/Placeables/OilMaker", buildingPanelTransform, CraftableUtility.OilMaker);
        CreateButton("PreservesJar", "Buildings/Placeables/PreservesJar", buildingPanelTransform, CraftableUtility.PreservesJar);
        CreateButton("RecyclingMachine", "Buildings/Placeables/RecyclingMachine", buildingPanelTransform, CraftableUtility.RecyclingMachine);
        CreateButton("SeedMaker", "Buildings/Placeables/SeedMaker", buildingPanelTransform, CraftableUtility.SeedMaker);
        CreateButton("SlimeEggPress", "Buildings/Placeables/SlimeEggPress", buildingPanelTransform, CraftableUtility.SlimeEggPress);
        CreateButton("SlimeIncubator", "Buildings/Placeables/SlimeIncubator", buildingPanelTransform, CraftableUtility.SlimeIncubator);
        CreateButton("SolarPanel", "Buildings/Placeables/SolarPanel", buildingPanelTransform, CraftableUtility.SolarPanel);
        CreateButton("WormBin", "Buildings/Placeables/WormBin", buildingPanelTransform, CraftableUtility.WormBin);
        GameObject.FindWithTag("Panel").transform.GetChild(1).gameObject.SetActive(false);

        //Action Buttons
        GameObject.FindWithTag("PlaceButton").GetComponent<Button>().onClick.AddListener(() => { GetBuildingController().SetCurrentAction(Actions.PLACE); });
        GameObject.FindWithTag("DeleteButton").GetComponent<Button>().onClick.AddListener(() => { GetBuildingController().SetCurrentAction(Actions.DELETE); });
        GameObject.FindWithTag("PickupButton").GetComponent<Button>().onClick.AddListener(() => { GetBuildingController().SetCurrentAction(Actions.EDIT); });

        //Floor Type Buttons
        Transform floorBarTransform = GameObject.FindWithTag("FloorSelectBar").transform.GetChild(0).GetChild(0);
        CreateButton("WoodFloor", "Floors/WoodFloor", floorBarTransform, FloorType.WOOD_FLOOR);
        CreateButton("RusticPlankFloor", "Floors/RusticPlankFloor", floorBarTransform, FloorType.RUSTIC_PLANK_FLOOR);
        CreateButton("StrawFloor", "Floors/StrawFloor", floorBarTransform, FloorType.STRAW_FLOOR);
        CreateButton("WeatheredFloor", "Floors/WeatheredFloor", floorBarTransform, FloorType.WEATHERED_FLOOR);
        CreateButton("CrystalFloor", "Floors/CrystalFloor", floorBarTransform, FloorType.CRYSTAL_FLOOR);
        CreateButton("StoneFloor", "Floors/StoneFloor", floorBarTransform, FloorType.STONE_FLOOR);
        CreateButton("StoneWalkwayFloor", "Floors/StoneWalkwayFloor", floorBarTransform, FloorType.STONE_WALKWAY_FLOOR);
        CreateButton("BrickFloor", "Floors/BrickFloor", floorBarTransform, FloorType.BRICK_FLOOR);
        CreateButton("WoodPath", "Floors/WoodPath", floorBarTransform, FloorType.WOOD_PATH);
        CreateButton("GravelPath", "Floors/GravelPath", floorBarTransform, FloorType.GRAVEL_PATH);
        CreateButton("CobblestonePath", "Floors/CobblestonePath", floorBarTransform, FloorType.COBBLESTONE_PATH);
        CreateButton("SteppingStonePath", "Floors/SteppingStonePath", floorBarTransform, FloorType.STEPPING_STONE_PATH);
        CreateButton("CrystalPath", "Floors/CrystalPath", floorBarTransform, FloorType.CRYSTAL_PATH);
        
    }

    public void CreateButtonsForBuilding(Building building){
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

    public void UpdateButtonPositionsAndScaleForBuilding(Building building){
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
        if (type == ButtonTypes.PLACE_FISH) AddFishMenuObject(button, building);
        if (type == ButtonTypes.ADD_ANIMAL) AddAnimalMenuObject(button, building);
        switch(type){
            case ButtonTypes.TIER_ONE:
                button.onClick.AddListener(() => {
                    if (building is ITieredBuilding tieredBuilding){
                        tieredBuilding.ChangeTier(1);
                    }
                 });
                break;
            case ButtonTypes.TIER_TWO:
                button.onClick.AddListener(() => {
                    if (building is ITieredBuilding tieredBuilding){
                        tieredBuilding.ChangeTier(2);
                    }
                });
                break;
            case ButtonTypes.TIER_THREE:
                button.onClick.AddListener(() => {
                    if (building is ITieredBuilding tieredBuilding){
                        tieredBuilding.ChangeTier(3);
                    }
                 });
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
                    if (building is FishPond fishPond){
                        fishPond.CycleFishPondDeco();
                    }
                 });
                break;
            case ButtonTypes.ADD_ANIMAL:
                button.onClick.AddListener(() => { 
                    bool isObjectActive =  button.transform.GetChild(0).gameObject.activeInHierarchy;
                    button.transform.GetChild(0).gameObject.SetActive(!isObjectActive);
                    button.transform.GetChild(1).gameObject.SetActive(!isObjectActive);
                 });
                break;
            default:
                throw new ArgumentException("This should never happen");
        }
    }

    private void AddAnimalMenuObject(Button button, Building building){
        //Animal Add
        GameObject animalMenuPrefab = building.GetType() switch{
            Type t when t == typeof(Coop) => Resources.Load<GameObject>("UI/CoopAnimalMenu"),
            Type t when t == typeof(Barn) => Resources.Load<GameObject>("UI/BarnAnimalMenu"),
            _ => throw new ArgumentException("This should never happen")
        };
        //GameObject animalMenuPrefab = Resources.Load<GameObject>("UI/BarnAnimalMenu");
        GameObject animalMenu = Instantiate(animalMenuPrefab);
        animalMenu.transform.SetParent(button.transform);
        Vector3 animalMenuPositionWorld = new Vector3(building.tilemap.CellToWorld(building.baseCoordinates[0] + new Vector3Int(1,0,0)).x, GetMiddleOfBuildingWorld(building).y + 4);
        animalMenu.transform.position = Camera.main.WorldToScreenPoint(animalMenuPositionWorld);
        animalMenu.GetComponent<RectTransform>().localScale = new Vector2(1, 1);
        animalMenu.SetActive(false);
        GameObject animalMenuContent = animalMenu.transform.GetChild(0).gameObject;
        IAnimalHouse animalHouse = building as IAnimalHouse;
        for (int childIndex = 0; childIndex < animalMenuContent.transform.childCount; childIndex++){
            Button addAnimalButton = animalMenuContent.transform.GetChild(childIndex).GetComponent<Button>();
            AddHoverEffect(addAnimalButton);
            addAnimalButton.onClick.AddListener(() => {
                //Debug.Log($"Adding animal {addAnimalButton.gameObject.name} to {building.name}");
                animalHouse.AddAnimal((Animals)Enum.Parse(typeof(Animals), addAnimalButton.gameObject.name));
            });
        }

        //Animal Remove
        GameObject animalInBuildingMenuPrefab = Resources.Load<GameObject>("UI/AnimalsInBuilding");
        GameObject animalInBuilding = Instantiate(animalInBuildingMenuPrefab);
        animalInBuilding.transform.SetParent(button.transform);
        Vector3 animalInBuildingMenuPositionWorld = new Vector3(building.tilemap.CellToWorld(building.baseCoordinates[0] + new Vector3Int(1,0,0)).x, GetMiddleOfBuildingWorld(building).y + 1);
        animalInBuilding.transform.position = Camera.main.WorldToScreenPoint(animalInBuildingMenuPositionWorld);
        animalInBuilding.GetComponent<RectTransform>().localScale = new Vector2(1, 1);
        animalInBuilding.SetActive(false);
        
    }

    private void AddFishMenuObject(Button button, Building building){
        GameObject fishMenuPrefab = Resources.Load<GameObject>("UI/FishMenu");
        GameObject fishMenu = Instantiate(fishMenuPrefab);
        fishMenu.transform.SetParent(button.transform);
        Vector3 fishMenuPositionWorld = new Vector3(building.tilemap.CellToWorld(building.baseCoordinates[0] + new Vector3Int(1,0,0)).x, GetMiddleOfBuildingWorld(building).y);
        fishMenu.transform.position = Camera.main.WorldToScreenPoint(fishMenuPositionWorld);
        fishMenu.GetComponent<RectTransform>().localScale = new Vector2(1, 1);
        fishMenu.SetActive(false);
        GameObject fishMenuContent = fishMenu.transform.GetChild(0).GetChild(0).gameObject;
        FishPond fishPond = building as FishPond;
        for (int childIndex = 0; childIndex < fishMenuContent.transform.childCount; childIndex++){
            Button fishButton = fishMenuContent.transform.GetChild(childIndex).GetComponent<Button>();
            AddHoverEffect(fishButton);
            fishButton.onClick.AddListener(() => {
                Fish fishType = (Fish)Enum.Parse(typeof(Fish), fishButton.GetComponent<Image>().sprite.name);
                fishPond.SetFishImage(fishType);
            });
            // fishMenuContent.transform.GetChild(childIndex).GetComponent<FishImageRetriever>().SetBuilding(fishPond);
            
        }
    }

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

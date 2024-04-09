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
        // CreateButton("logCabin", "Buildings/LogCabin1", buildingPanelTransform, Cabin.CabinTypes.Wood);
        // CreateButton("stoneCabin", "Buildings/StoneCabin1", buildingPanelTransform, Cabin.CabinTypes.Stone);
        // CreateButton("plankCabin", "Buildings/PlankCabin1", buildingPanelTransform, Cabin.CabinTypes.Plank);
        // CreateButton("coop", "Buildings/Coop", buildingPanelTransform, typeof(Coop));
        // CreateButton("barn", "Buildings/Barn", buildingPanelTransform, typeof(Barn));
        // CreateButton("shed", "Buildings/Shed1", buildingPanelTransform, typeof(Shed));
        // CreateButton("goldClock", "Buildings/GoldClock", buildingPanelTransform, typeof(GoldClock));
        // CreateButton("silo", "Buildings/Silo", buildingPanelTransform, typeof(Silo));
        // CreateButton("slimeHutch", "Buildings/SlimeHutch", buildingPanelTransform, typeof(SlimeHutch));
        // CreateButton("shippingBin", "Buildings/ShippingBin", buildingPanelTransform, typeof(ShippingBin));
        // CreateButton("well", "Buildings/Well", buildingPanelTransform, typeof(Well));
        // CreateButton("earthObelisk", "Buildings/EarthObelisk", buildingPanelTransform, typeof(Obelisk));
        // CreateButton("stable", "Buildings/Stable", buildingPanelTransform, typeof(Stable));
        // CreateButton("junimoHunt", "Buildings/JunimoHut", buildingPanelTransform, typeof(JunimoHut));
        // CreateButton("greenhouse", "Buildings/Greenhouse", buildingPanelTransform, typeof(Greenhouse));
        // CreateButton("mill", "Buildings/Mill", buildingPanelTransform, typeof(Mill));
        // CreateButton("fishPond", "Buildings/FishPond", buildingPanelTransform, typeof(FishPond));
        // CreateButton("floor", "Buildings/WoodFloor", buildingPanelTransform, typeof(Floor));
        // CreateButton("Fence", "Fences/WoodFence", buildingPanelTransform, typeof(Fence));
        // CreateButton("Crop", "Buildings/Crop", buildingPanelTransform, typeof(Crop));

        // Debug.Log("Getting all building types");
        var buildingType = typeof(Building);
        var allTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => buildingType.IsAssignableFrom(p) && p.IsClass && !p.IsAbstract);

        Transform buildingPanelContent = GameObject.FindGameObjectWithTag("Panel").transform.GetChild(0).GetChild(0);
        foreach (var type in allTypes){//todo problem with silo
            if (type == typeof(House)) continue;
            if (type == typeof(Greenhouse)) continue;
            // Debug.Log(type);
            GameObject temp = new GameObject();
            Building buildingTemp = (Building) temp.AddComponent(type);
            GameObject button = buildingTemp.CreateButton();
            button.transform.SetParent(buildingPanelContent);
            button.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
            Destroy(temp);
            // StartCoroutine(RemoveComponentNextFrame(buildingTemp)); //Components cant be removed the same frame they are added
        }

        //Placeables Buttons
        buildingPanelTransform = GameObject.FindWithTag("Panel").transform.GetChild(1).GetChild(0);
        // CreateButton("Scarecrow", "Buildings/Scarecrow", buildingPanelTransform, false);
        // CreateButton("DeluxeScarecrow", "Buildings/DeluxeScarecrow",buildingPanelTransform, true);
        CreateButton("sprinkler", "Buildings/SprinklerT1", buildingPanelTransform, typeof(Sprinkler));
        CreateButton("Beehouse", "Buildings/Placeables/Beehouse", buildingPanelTransform, Craftables.Type.Beehouse);
        CreateButton("Bonemill", "Buildings/Placeables/Bonemill", buildingPanelTransform, Craftables.Type.BoneMill);
        CreateButton("Cask", "Buildings/Placeables/Cask", buildingPanelTransform, Craftables.Type.Cask);
        CreateButton("CharcoalKiln", "Buildings/Placeables/CharcoalKiln", buildingPanelTransform, Craftables.Type.CharcoalKiln);
        CreateButton("CheesePress", "Buildings/Placeables/CheesePress", buildingPanelTransform, Craftables.Type.CheesePress);
        CreateButton("Crystalarium", "Buildings/Placeables/Crystalarium", buildingPanelTransform, Craftables.Type.Crystalarium);
        CreateButton("FarmComputer", "Buildings/Placeables/FarmComputer", buildingPanelTransform, Craftables.Type.FarmComputer);
        CreateButton("Furnace", "Buildings/Placeables/Furnace", buildingPanelTransform, Craftables.Type.Furnace);
        CreateButton("GeodeCrusher", "Buildings/Placeables/GeodeCrusher", buildingPanelTransform, Craftables.Type.GeodeCrusher);
        CreateButton("Hopper", "Buildings/Placeables/Hopper", buildingPanelTransform, Craftables.Type.Hopper);
        CreateButton("Keg", "Buildings/Placeables/Keg", buildingPanelTransform, Craftables.Type.Keg);
        CreateButton("LightningRod", "Buildings/Placeables/LightningRod", buildingPanelTransform, Craftables.Type.LightningRod);
        CreateButton("Loom", "Buildings/Placeables/Loom", buildingPanelTransform, Craftables.Type.Loom);
        CreateButton("MayonnaiseMachine", "Buildings/Placeables/MayonnaiseMachine", buildingPanelTransform, Craftables.Type.MayonnaiseMachine);
        CreateButton("MiniObelisk", "Buildings/Placeables/MiniObelisk", buildingPanelTransform, Craftables.Type.MiniObelisk);
        CreateButton("OilMaker", "Buildings/Placeables/OilMaker", buildingPanelTransform, Craftables.Type.OilMaker);
        CreateButton("PreservesJar", "Buildings/Placeables/PreservesJar", buildingPanelTransform, Craftables.Type.PreservesJar);
        CreateButton("RecyclingMachine", "Buildings/Placeables/RecyclingMachine", buildingPanelTransform, Craftables.Type.RecyclingMachine);
        CreateButton("SeedMaker", "Buildings/Placeables/SeedMaker", buildingPanelTransform, Craftables.Type.SeedMaker);
        CreateButton("SlimeEggPress", "Buildings/Placeables/SlimeEggPress", buildingPanelTransform, Craftables.Type.SlimeEggPress);
        CreateButton("SlimeIncubator", "Buildings/Placeables/SlimeIncubator", buildingPanelTransform, Craftables.Type.SlimeIncubator);
        CreateButton("SolarPanel", "Buildings/Placeables/SolarPanel", buildingPanelTransform, Craftables.Type.SolarPanel);
        CreateButton("WormBin", "Buildings/Placeables/WormBin", buildingPanelTransform, Craftables.Type.WormBin);
        GameObject.FindWithTag("Panel").transform.GetChild(1).gameObject.SetActive(false);

        //Action Buttons
        GameObject.FindWithTag("PlaceButton").GetComponent<Button>().onClick.AddListener(() => { GetBuildingController().SetCurrentAction(Actions.PLACE); });
        GameObject.FindWithTag("DeleteButton").GetComponent<Button>().onClick.AddListener(() => { GetBuildingController().SetCurrentAction(Actions.DELETE); });
        GameObject.FindWithTag("PickupButton").GetComponent<Button>().onClick.AddListener(() => { GetBuildingController().SetCurrentAction(Actions.EDIT); });

        //Floor Type Buttons
        Transform floorBarTransform = GameObject.FindWithTag("FloorSelectBar").transform.GetChild(0).GetChild(0);
        // CreateButton("WoodFloor", "Floors/WoodFloor", floorBarTransform, Floor.Types.WOOD_FLOOR);
        // CreateButton("RusticPlankFloor", "Floors/RusticPlankFloor", floorBarTransform, Floor.Types.RUSTIC_PLANK_FLOOR);
        // CreateButton("StrawFloor", "Floors/StrawFloor", floorBarTransform, Floor.Types.STRAW_FLOOR);
        // CreateButton("WeatheredFloor", "Floors/WeatheredFloor", floorBarTransform, Floor.Types.WEATHERED_FLOOR);
        // CreateButton("CrystalFloor", "Floors/CrystalFloor", floorBarTransform, Floor.Types.CRYSTAL_FLOOR);
        // CreateButton("StoneFloor", "Floors/StoneFloor", floorBarTransform, Floor.Types.STONE_FLOOR);
        // CreateButton("StoneWalkwayFloor", "Floors/StoneWalkwayFloor", floorBarTransform, Floor.Types.STONE_WALKWAY_FLOOR);
        // CreateButton("BrickFloor", "Floors/BrickFloor", floorBarTransform, Floor.Types.BRICK_FLOOR);
        // CreateButton("WoodPath", "Floors/WoodPath", floorBarTransform, Floor.Types.WOOD_PATH);
        // CreateButton("GravelPath", "Floors/GravelPath", floorBarTransform, Floor.Types.GRAVEL_PATH);
        // CreateButton("CobblestonePath", "Floors/CobblestonePath", floorBarTransform, Floor.Types.COBBLESTONE_PATH);
        // CreateButton("SteppingStonePath", "Floors/SteppingStonePath", floorBarTransform, Floor.Types.STEPPING_STONE_PATH);
        // CreateButton("CrystalPath", "Floors/CrystalPath", floorBarTransform, Floor.Types.CRYSTAL_PATH);
        // CreateButton("WoodFence", "Fences/WoodFence", floorBarTransform, Fence.Types.Wood);
        // CreateButton("HardwoodFence", "Fences/HardwoodFence", floorBarTransform, Fence.Types.Hardwood);
        // CreateButton("IronFence", "Fences/IronFence", floorBarTransform, Fence.Types.Iron);
        // CreateButton("StoneFence", "Fences/StoneFence", floorBarTransform, Fence.Types.Stone);
        // CreateButton("Parsnip", "Fences/StoneFence", floorBarTransform, Crop.Types.Parsnip);
        // CreateButton("Pumpkin", "Fences/StoneFence", floorBarTransform, Crop.Types.Pumpkin);
        
    }

    private IEnumerator RemoveComponentNextFrame(Component component){
        yield return null; // wait until the next frame
        Destroy(component);
    }

    // private 

    public void CreateButtonsForBuilding(Building building){
        ButtonTypes[] buttonTypes = building.BuildingInteractions;
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
            buttonParent.transform.GetChild(buttonIndex).position = CalculatePositionOfButton(building.BuildingInteractions.Length, buttonIndex+1, Camera.main.WorldToScreenPoint(GetMiddleOfBuildingWorld(building)));
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
                        tieredBuilding.SetTier(1);
                    }
                 });
                break;
            case ButtonTypes.TIER_TWO:
                button.onClick.AddListener(() => {
                    if (building is ITieredBuilding tieredBuilding){
                        tieredBuilding.SetTier(2);
                    }
                });
                break;
            case ButtonTypes.TIER_THREE:
                button.onClick.AddListener(() => {
                    if (building is ITieredBuilding tieredBuilding){
                        tieredBuilding.SetTier(3);
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
            // case ButtonTypes.CHANGE_FISH_POND_DECO:
            //     button.onClick.AddListener(() => { 
            //         if (building is FishPond fishPond){
            //             fishPond.CycleFishPondDeco();
            //         }
            //      });
            //     break;
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
        Vector3 animalMenuPositionWorld = new Vector3(building.Tilemap.CellToWorld(building.BaseCoordinates[0] + new Vector3Int(1,0,0)).x, GetMiddleOfBuildingWorld(building).y + 4);
        animalMenu.transform.position = Camera.main.WorldToScreenPoint(animalMenuPositionWorld);
        animalMenu.GetComponent<RectTransform>().localScale = new Vector2(1, 1);
        animalMenu.SetActive(false);
        GameObject animalMenuContent = animalMenu.transform.GetChild(0).gameObject;
        IAnimalHouse animalHouse = building as IAnimalHouse;
        for (int childIndex = 0; childIndex < animalMenuContent.transform.childCount; childIndex++){
            Button addAnimalButton = animalMenuContent.transform.GetChild(childIndex).GetComponent<Button>();
            AddHoverEffect(addAnimalButton);
            addAnimalButton.onClick.AddListener(() => {
                animalHouse.AddAnimal((Animals)Enum.Parse(typeof(Animals), addAnimalButton.gameObject.name));
            });
        }

        //Animal Remove
        GameObject animalInBuildingMenuPrefab = Resources.Load<GameObject>("UI/AnimalsInBuilding");
        GameObject animalInBuilding = Instantiate(animalInBuildingMenuPrefab);
        animalInBuilding.transform.SetParent(button.transform);
        Vector3 animalInBuildingMenuPositionWorld = new Vector3(building.Tilemap.CellToWorld(building.BaseCoordinates[0] + new Vector3Int(1,0,0)).x, GetMiddleOfBuildingWorld(building).y + 1);
        animalInBuilding.transform.position = Camera.main.WorldToScreenPoint(animalInBuildingMenuPositionWorld);
        animalInBuilding.GetComponent<RectTransform>().localScale = new Vector2(1, 1);
        animalInBuilding.SetActive(false);
        
    }

    private void AddFishMenuObject(Button button, Building building){
        GameObject fishMenuPrefab = Resources.Load<GameObject>("UI/FishMenu");
        GameObject fishMenu = Instantiate(fishMenuPrefab);
        fishMenu.transform.SetParent(button.transform);
        Vector3 fishMenuPositionWorld = new Vector3(building.Tilemap.CellToWorld(building.BaseCoordinates[0] + new Vector3Int(1,0,0)).x, GetMiddleOfBuildingWorld(building).y);
        fishMenu.transform.position = Camera.main.WorldToScreenPoint(fishMenuPositionWorld);
        fishMenu.GetComponent<RectTransform>().localScale = new Vector2(1, 1);
        fishMenu.SetActive(false);
        GameObject fishMenuContent = fishMenu.transform.GetChild(0).GetChild(0).gameObject;
        //FishPond fishPond = building as FishPond;
        for (int childIndex = 0; childIndex < fishMenuContent.transform.childCount; childIndex++){
            Button fishButton = fishMenuContent.transform.GetChild(childIndex).GetComponent<Button>();
            AddHoverEffect(fishButton);
            fishButton.onClick.AddListener(() => {
                Fish fishType = (Fish)Enum.Parse(typeof(Fish), fishButton.GetComponent<Image>().sprite.name);
                //fishPond.SetFishImage(fishType);
            });
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

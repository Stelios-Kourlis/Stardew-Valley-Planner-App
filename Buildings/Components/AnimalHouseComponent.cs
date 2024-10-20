using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using static Utility.TilemapManager;
using static Utility.BuildingManager;
using static Utility.ClassManager;
using static BuildingData;
using Newtonsoft.Json.Linq;

[Serializable]
public class AnimalTiers {
    public int tier;
    public Animals[] animalsAllowed;
}

[RequireComponent(typeof(Building))]
[Serializable]
public class AnimalHouseComponent : BuildingComponent {
    public static SpriteAtlas AnimalAtlas { get; private set; }
    private readonly GameObject[] UIElements = new GameObject[2];
    [field: SerializeField] public List<Animals> AnimalsInBuilding { get; private set; }
    public int MaxAnimalCapacity => gameObject.GetComponent<TieredBuildingComponent>().Tier * 4; //capacity is based on tier
    public AnimalTiers[] animalsPerTier;
    private int Tier => GetComponent<TieredBuildingComponent>().Tier;

    // [SerializeField] private GameObject animalMenuPrefab;

    public void Awake() {
        if (AnimalAtlas == null) AnimalAtlas = Resources.Load("AnimalsAtlas") as SpriteAtlas;
        AnimalsInBuilding = new List<Animals>();
        if (!gameObject.GetComponent<InteractableBuildingComponent>()) gameObject.AddComponent<InteractableBuildingComponent>();
        gameObject.GetComponent<InteractableBuildingComponent>().ButtonsCreated += AddAnimalMenuObject;
        gameObject.GetComponent<InteractableBuildingComponent>().AddInteractionToBuilding(ButtonTypes.ADD_ANIMAL);

        gameObject.GetComponent<TieredBuildingComponent>().tierChanged += UpdateMaxAnimalCapacity;
    }

    public AnimalHouseComponent SetAllowedAnimalsPerTier(AnimalTiers[] animalsPerTier) {
        this.animalsPerTier = animalsPerTier;
        return this;
    }

    public bool AddAnimal(Animals animal) {
        if (AnimalsInBuilding.Count >= MaxAnimalCapacity) {
            NotificationManager.Instance.SendNotification($"{Building.GetType()} is full ({AnimalsInBuilding.Count}/{MaxAnimalCapacity}), cannot add {animal}", NotificationManager.Icons.ErrorIcon);
            return false;
        }
        if (!animalsPerTier.First(apt => apt.tier == Tier).animalsAllowed.Contains(animal)) { NotificationManager.Instance.SendNotification($"Cannot add {animal} to {Building.BuildingName} tier {GetComponent<TieredBuildingComponent>().Tier}", NotificationManager.Icons.ErrorIcon); return false; }
        AnimalsInBuilding.Add(animal);
        AddAnimalButton(animal);
        return true;
    }

    public void UpdateMaxAnimalCapacity(int tier) {
        var animalsToRemove = AnimalsInBuilding.Where(animal => !animalsPerTier.First(apt => apt.tier == Tier).animalsAllowed.Contains(animal)).ToList();

        AnimalsInBuilding.RemoveAll(animal => animalsToRemove.Contains(animal));

        if (AnimalsInBuilding.Count > MaxAnimalCapacity) NotificationManager.Instance.SendNotification($"Removed {AnimalsInBuilding.Count - MaxAnimalCapacity} animals that exceed the new capacity of {Building.BuildingName}", NotificationManager.Icons.InfoIcon);
        while (AnimalsInBuilding.Count > MaxAnimalCapacity) {
            AnimalsInBuilding.Remove(AnimalsInBuilding.Last());
        }

        UpdateAnimalInBuildingButtons();
        UpdateAddAnimalButtonsBasedOnTier(UIElements[0]);
    }

    public void UpdateAnimalInBuildingButtons() {
        GameObject targetGameObject = gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject.transform.Find("ADD_ANIMAL").GetChild(1).GetChild(0).gameObject;

        foreach (Transform child in targetGameObject.transform) {
            Destroy(child.gameObject);
        }

        foreach (var animal in AnimalsInBuilding) {
            AddAnimalButton(animal);
        }
    }

    public void AddAnimalMenuObject() {
        UIElements[0] = CreateAnimalChoiceMenu();
        UIElements[1] = CreateAnimalsInBuildingMenu();
    }

    public List<MaterialCostEntry> GetMaterialsNeeded() {
        List<MaterialCostEntry> animalCosts = new();
        foreach (Animals animal in AnimalsInBuilding) {
            switch (animal) {
                case Animals.Chicken:
                    animalCosts.Add(new(800, Materials.Coins));
                    break;
                case Animals.Cow:
                    animalCosts.Add(new(1500, Materials.Coins));
                    break;
                case Animals.Goat:
                    animalCosts.Add(new(4000, Materials.Coins));
                    break;
                case Animals.Duck:
                    animalCosts.Add(new(1200, Materials.Coins));
                    break;
                case Animals.Sheep:
                    animalCosts.Add(new(8000, Materials.Coins));
                    break;
                case Animals.Rabbit:
                    animalCosts.Add(new(8000, Materials.Coins));
                    break;
                case Animals.Pig:
                    animalCosts.Add(new(16000, Materials.Coins));
                    break;
                case Animals.Dinosaur:
                    animalCosts.Add(new(800, Materials.DinosaurEgg));
                    break;
                case Animals.GoldenChicken:
                    animalCosts.Add(new(800, Materials.GoldenEgg));
                    break;
                case Animals.VoidChicken:
                    animalCosts.Add(new(800, Materials.VoidEgg));
                    break;
                case Animals.Ostrich:
                    animalCosts.Add(new(1, Materials.OstrichEgg));
                    break;
            }
        }
        return animalCosts;
    }

    private GameObject CreateAnimalChoiceMenu() {
        GameObject animalMenuPrefab = Resources.Load<GameObject>($"UI/AddAnimalMenu");
        GameObject animalMenu = Instantiate(animalMenuPrefab);
        CreateAddAnimalButtons(animalMenu);
        animalMenu.transform.SetParent(gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject.transform.Find("ADD_ANIMAL").transform);//this is the button to toggle the animal menu
        animalMenu.GetComponent<RectTransform>().position = new(gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject.transform.position.x - 100, gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject.transform.position.y + 25);
        animalMenu.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        animalMenu.SetActive(false);
        // GameObject animalMenuContent = animalMenu.transform.gameObject;
        // for (int childIndex = 0; childIndex < animalMenuContent.transform.childCount; childIndex++) {
        //     Button addAnimalButton = animalMenuContent.transform.GetChild(childIndex).GetComponent<Button>();
        //     // AddHoverEffect(addAnimalButton);
        //     // addAnimalButton.gameObject.AddComponent<UIElement>().SetActionToNothingOnEnter = false;
        //     // addAnimalButton.onClick.AddListener(() => {
        //     //     Debug.Log($"Clicked {addAnimalButton.gameObject.name}");
        //     //     AddAnimal((Animals)Enum.Parse(typeof(Animals), addAnimalButton.gameObject.name));
        //     // });
        // }

        UpdateAddAnimalButtonsBasedOnTier(animalMenu);
        return animalMenu;
    }

    private GameObject CreateAnimalsInBuildingMenu() {
        GameObject animalsInBuildingMenuPrefab = Resources.Load<GameObject>("UI/AnimalsInBuilding");
        GameObject animalsInBuilding = Instantiate(animalsInBuildingMenuPrefab);
        // Resources.UnloadAsset(animalsInBuildingMenuPrefab);
        animalsInBuilding.transform.SetParent(gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject.transform.Find("ADD_ANIMAL").transform);
        animalsInBuilding.GetComponent<RectTransform>().position = new(gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject.transform.position.x - 100, gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject.transform.position.y - 25);
        animalsInBuilding.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        animalsInBuilding.SetActive(false);
        return animalsInBuilding;
    }

    private void CreateAddAnimalButtons(GameObject animalMenu) {
        // for (int tier = 1; tier < GetComponent<TieredBuildingComponent>().MaxTier; tier++) {
        foreach (Animals animal in animalsPerTier.First(apt => apt.tier == Tier).animalsAllowed) {
            GameObject button = new(animal.ToString());
            button.transform.SetParent(animalMenu.transform);
            button.AddComponent<RectTransform>().sizeDelta = new Vector3(100, 100, 1);
            button.gameObject.AddComponent<UIElement>().SetActionToNothingOnEnter = false;
            button.AddComponent<Image>().sprite = AnimalAtlas.GetSprite(animal.ToString());
            Animals typeOfAnimal = animal; //Capture it outside of lamda expression
            button.AddComponent<Button>().onClick.AddListener(() => {
                AddAnimal(animal);
            });
        }
        // }
    }
    private void UpdateAddAnimalButtonsBasedOnTier(GameObject animalMenu) {
        foreach (Transform animalButton in animalMenu.transform) {
            Button button = animalButton.GetComponent<Button>();
            UIElement uiElement = animalButton.GetComponent<UIElement>();

            bool isAnimalAllowed = animalsPerTier.First(apt => apt.tier == Tier).animalsAllowed.Contains((Animals)Enum.Parse(typeof(Animals), button.gameObject.name));

            button.interactable = isAnimalAllowed;
            uiElement.ExpandOnHover = isAnimalAllowed;
            uiElement.playSounds = isAnimalAllowed;

        }
    }
    private void AddAnimalButton(Animals animal) {
        GameObject button = new(animal.ToString());
        button.transform.SetParent(gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject.transform.Find("ADD_ANIMAL").GetChild(1).GetChild(0));
        button.AddComponent<Image>().sprite = AnimalAtlas.GetSprite(animal.ToString());
        button.AddComponent<Button>().onClick.AddListener(() => {
            AnimalsInBuilding.Remove(animal);
            Destroy(button);
        });
        button.AddComponent<UIElement>();
        button.transform.localScale = new Vector3(1, 1, 1);
    }

    public void ToggleAnimalMenu() {
        UIElements[0].SetActive(!UIElements[0].activeSelf);
        UIElements[1].SetActive(!UIElements[1].activeSelf);
    }

    public void Load(BuildingScriptableObject bso) {
        animalsPerTier = bso.animalsPerTier;
    }

    public override ComponentData Save() {
        ComponentData animalHouseData = new(typeof(AnimalHouseComponent), new());
        foreach (var animal in AnimalsInBuilding) {
            var animalAlreadyExists = animalHouseData.componentData.FirstOrDefault(x => x.Name == animal.ToString());
            if (animalAlreadyExists != null) {
                int index = animalHouseData.componentData.IndexOf(animalAlreadyExists);
                animalHouseData.componentData[index] = new JProperty(animal.ToString(), int.Parse(animalAlreadyExists.Value.ToString()) + 1);
            }
            else animalHouseData.componentData.Add(new JProperty(animal.ToString(), 1));
        }
        return animalHouseData;
    }

    public override void Load(ComponentData data) {
        foreach (var property in data.componentData) {
            for (int count = 0; count < int.Parse((string)property.Value); count++) AddAnimal(Enum.Parse<Animals>(property.Name));
        }
    }
}

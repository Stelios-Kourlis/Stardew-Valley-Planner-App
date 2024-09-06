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

[RequireComponent(typeof(Building))]
[Serializable]
public class AnimalHouseComponent : BuildingComponent {
    public SpriteAtlas AnimalAtlas { get; private set; }
    private readonly GameObject[] UIElements = new GameObject[2];
    private SpriteAtlas animalsInBuildingPanelBackgroundAtlas;
    [field: SerializeField] public List<Animals> AnimalsInBuilding { get; private set; }
    public int MaxAnimalCapacity => gameObject.GetComponent<TieredBuildingComponent>().Tier * 4; //capacity is based on tier
    public Dictionary<int, HashSet<Animals>> animalsPerTier;

    public void Awake() {
        AnimalAtlas = Resources.Load($"{Building.GetType()}AnimalsAtlas") as SpriteAtlas;
        animalsInBuildingPanelBackgroundAtlas = Resources.Load("UI/AnimalsInBuildingAtlas") as SpriteAtlas;
        AnimalsInBuilding = new List<Animals>();
        if (!gameObject.GetComponent<InteractableBuildingComponent>()) gameObject.AddComponent<InteractableBuildingComponent>();
        gameObject.GetComponent<InteractableBuildingComponent>().ButtonsCreated += AddAnimalMenuObject;
        gameObject.GetComponent<InteractableBuildingComponent>().AddInteractionToBuilding(ButtonTypes.ADD_ANIMAL);

        gameObject.GetComponent<TieredBuildingComponent>().tierChanged += UpdateMaxAnimalCapacity;
    }

    public AnimalHouseComponent SetAllowedAnimalsPerTier(Dictionary<int, HashSet<Animals>> allowedAnimalsPerTier) {
        animalsPerTier = allowedAnimalsPerTier;
        return this;
    }

    public bool AddAnimal(Animals animal) {
        if (AnimalsInBuilding.Count >= MaxAnimalCapacity) {
            GetNotificationManager().SendNotification($"{Building.GetType()} is full ({AnimalsInBuilding.Count}/{MaxAnimalCapacity}), cannot add {animal}", NotificationManager.Icons.ErrorIcon);
            return false;
        }
        if (!animalsPerTier[GetComponent<TieredBuildingComponent>().Tier].Contains(animal)) { GetNotificationManager().SendNotification($"Cannot add {animal} to Barn tier {GetComponent<TieredBuildingComponent>().Tier}", NotificationManager.Icons.ErrorIcon); return false; }
        AnimalsInBuilding.Add(animal);
        AddAnimalButton(animal);
        return true;
    }

    public void UpdateMaxAnimalCapacity(int tier) {
        if (UIElements[1] != null) {
            UIElements[1].GetComponent<Image>().sprite = animalsInBuildingPanelBackgroundAtlas.GetSprite($"AnimalsInBuilding{MaxAnimalCapacity}");
            UIElements[1].GetComponent<RectTransform>().sizeDelta = new Vector2(620, 100 * tier + 100);
            UIElements[1].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(580, 100 * tier + 100 - 50);
        }

        var animalsToRemove = AnimalsInBuilding.Where(animal => !animalsPerTier[tier].Contains(animal)).ToList();

        AnimalsInBuilding.RemoveAll(animal => animalsToRemove.Contains(animal));

        if (AnimalsInBuilding.Count > MaxAnimalCapacity) GetNotificationManager().SendNotification($"Removed {AnimalsInBuilding.Count - MaxAnimalCapacity} animals that exceed the new capacity of {Building.BuildingName}", NotificationManager.Icons.InfoIcon);
        while (AnimalsInBuilding.Count > MaxAnimalCapacity) {
            AnimalsInBuilding.Remove(AnimalsInBuilding.Last());
        }

        UpdateAnimals();
    }

    public void UpdateAnimals() {

        GameObject targetGameObject = gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject.transform.Find("ADD_ANIMAL").GetChild(1).GetChild(0).gameObject;

        // Step 2: Iterate through all its children
        foreach (Transform child in targetGameObject.transform) {
            // Step 3: Destroy each child GameObject
            Destroy(child.gameObject);
        }

        foreach (var animal in AnimalsInBuilding) {
            AddAnimalButton(animal);
        }
    }

    public void AddAnimalMenuObject() {
        //Animal Add Panel (Upper)
        GameObject animalMenuPrefab = Resources.Load<GameObject>($"UI/{Building.GetType()}AnimalMenu");
        GameObject animalMenu = Instantiate(animalMenuPrefab);
        animalMenu.transform.SetParent(gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject.transform.Find("ADD_ANIMAL").transform);//this is the button to toggle the animal menu
        animalMenu.GetComponent<RectTransform>().position = new(gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject.transform.position.x - 100, gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject.transform.position.y + 25);
        animalMenu.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        animalMenu.SetActive(false);
        GameObject animalMenuContent = animalMenu.transform.GetChild(0).gameObject;
        for (int childIndex = 0; childIndex < animalMenuContent.transform.childCount; childIndex++) {
            Button addAnimalButton = animalMenuContent.transform.GetChild(childIndex).GetComponent<Button>();
            // AddHoverEffect(addAnimalButton);
            addAnimalButton.gameObject.AddComponent<UIElement>();
            addAnimalButton.onClick.AddListener(() => {
                AddAnimal((Animals)Enum.Parse(typeof(Animals), addAnimalButton.gameObject.name));
            });
        }

        //Animals In Building Panel (Lower)
        GameObject animalInBuildingMenuPrefab = Resources.Load<GameObject>("UI/AnimalsInBuilding");
        GameObject animalInBuilding = Instantiate(animalInBuildingMenuPrefab);
        animalInBuilding.transform.SetParent(gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject.transform.Find("ADD_ANIMAL").transform);
        animalInBuilding.GetComponent<RectTransform>().position = new(gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject.transform.position.x - 100, gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject.transform.position.y - 25);
        animalInBuilding.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        animalInBuilding.SetActive(false);

        animalInBuilding.GetComponent<Image>().sprite = animalsInBuildingPanelBackgroundAtlas.GetSprite($"AnimalsInBuilding{MaxAnimalCapacity}");

        UIElements[0] = animalMenu;
        UIElements[1] = animalInBuilding;
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
        // AddHoverEffect(button.GetComponent<Button>());
        button.transform.localScale = new Vector3(1, 1, 1);
    }

    public void ToggleAnimalMenu() {
        UIElements[0].SetActive(!UIElements[0].activeSelf);
        UIElements[1].SetActive(!UIElements[1].activeSelf);
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

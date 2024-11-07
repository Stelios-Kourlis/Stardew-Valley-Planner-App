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
    private static SpriteAtlas animalAtlas;
    private static GameObject animalMenuPrefab;
    private GameObject AnimalMenu;
    private GameObject AddAnimalsPanel => AnimalMenu != null ? AnimalMenu.transform.GetChild(0).gameObject : null;
    private GameObject AnimalsInBuildingPanel => AnimalMenu != null ? AnimalMenu.transform.GetChild(1).gameObject : null;
    [field: SerializeField] public List<Animals> AnimalsInBuilding { get; private set; }
    public int MaxAnimalCapacity => gameObject.GetComponent<TieredBuildingComponent>().Tier * 4; //capacity is based on tier
    public AnimalTiers[] animalsPerTier;
    private int Tier => GetComponent<TieredBuildingComponent>().Tier;

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
        UpdateAddAnimalButtonsBasedOnTier(AddAnimalsPanel);
    }

    public void UpdateAnimalInBuildingButtons() {
        if (AnimalsInBuildingPanel == null) return;
        foreach (Transform child in AnimalsInBuildingPanel.transform.GetChild(0)) {
            Destroy(child.gameObject);
        }

        foreach (var animal in AnimalsInBuilding) {
            AddAnimalButton(animal);
        }
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

    private void CreateAnimalChoiceMenu() {
        // GameObject animalMenuPrefab = Resources.Load<GameObject>($"UI/AddAnimalMenu");
        AnimalMenu = Instantiate(animalMenuPrefab);
        CreateAddAnimalButtons(AddAnimalsPanel);
        SetAnimalMenuPosition();
        AnimalMenu.SetActive(false);

        UpdateAddAnimalButtonsBasedOnTier(AddAnimalsPanel);
    }

    private void CreateAddAnimalButtons(GameObject animalMenu) {
        foreach (Animals animal in animalsPerTier.First(apt => apt.tier == gameObject.GetComponent<TieredBuildingComponent>().MaxTier).animalsAllowed) {
            GameObject button = new(animal.ToString());
            button.transform.SetParent(animalMenu.transform);
            button.AddComponent<RectTransform>().sizeDelta = new Vector3(100, 100, 1);
            button.AddComponent<UIElement>().SetActionToNothingOnEnter = false;
            button.AddComponent<Image>().sprite = animalAtlas.GetSprite(animal.ToString());
            Animals typeOfAnimal = animal; //Capture it outside of lamda expression
            button.AddComponent<Button>().onClick.AddListener(() => {
                AddAnimal(animal);
            });
        }
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
        button.transform.SetParent(AnimalsInBuildingPanel.transform.GetChild(0));
        button.AddComponent<Image>().sprite = animalAtlas.GetSprite(animal.ToString());
        button.AddComponent<Button>().onClick.AddListener(() => {
            AnimalsInBuilding.Remove(animal);
            Destroy(button);
        });
        button.AddComponent<UIElement>();
        button.transform.localScale = new Vector3(1, 1, 1);
    }

    public void SetAnimalMenuPosition() {
        if (AnimalMenu == null) return;
        // GameObject animalMenu = UIElements[0];
        // GameObject animalsInBuilding = UIElements[1];

        if (BuildingController.isInsideBuilding.Key) {
            EnterableBuildingComponent enterableBuildingComponent = BuildingController.isInsideBuilding.Value;

            AnimalMenu.transform.SetParent(enterableBuildingComponent.interiorSceneCanvas.transform);
            AnimalMenu.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            AnimalMenu.GetComponent<RectTransform>().localScale = new Vector3(1.3f, 1.3f, 1.3f);
            AnimalMenu.GetComponent<RectTransform>().localPosition = new Vector3(0, 0);

            // animalsInBuilding.transform.SetParent(enterableBuildingComponent.interiorSceneCanvas.transform);
            // animalsInBuilding.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1);
            // animalsInBuilding.GetComponent<RectTransform>().localScale = new Vector3(1.3f, 1.3f, 1.3f);
            // animalsInBuilding.GetComponent<RectTransform>().localPosition = new Vector3(0, 50);
        }
        else {
            AnimalMenu.transform.SetParent(gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject.transform.Find("ADD_ANIMAL").transform);
            AnimalMenu.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            AnimalMenu.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            AnimalMenu.GetComponent<RectTransform>().localPosition = Vector3.zero - new Vector3(AnimalMenu.GetComponent<RectTransform>().rect.width / 2 + 150, 0, 0);

            // animalsInBuilding.transform.SetParent(gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject.transform.Find("ADD_ANIMAL").transform);
            // animalsInBuilding.GetComponent<RectTransform>().pivot = new Vector2(1, 1);
            // animalsInBuilding.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            // animalsInBuilding.GetComponent<RectTransform>().position = new(gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject.transform.position.x - 100, gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject.transform.position.y - 25);
        }
    }

    public void ToggleAnimalMenu() {
        // SetAnimalMenuPosition();
        AnimalMenu.SetActive(!AnimalMenu.activeInHierarchy);
        // UIElements[1].SetActive(!UIElements[1].activeSelf);
    }

    private void CloseAnimalMenu() {
        Debug.Log("Close animal menu");
        if (AnimalMenu.activeSelf) ToggleAnimalMenu(); //if any is on close
    }

    public override void Load(BuildingScriptableObject bso) {
        animalsPerTier = bso.animalsPerTier;

        if (animalAtlas == null) animalAtlas = Resources.Load("AnimalsAtlas") as SpriteAtlas;
        if (animalMenuPrefab == null) animalMenuPrefab = Resources.Load<GameObject>($"UI/AddAnimalMenu");

        AnimalsInBuilding = new List<Animals>();
        gameObject.GetComponent<InteractableBuildingComponent>().ButtonsCreated += CreateAnimalChoiceMenu;
        gameObject.GetComponent<InteractableBuildingComponent>().AddInteractionToBuilding(ButtonTypes.ADD_ANIMAL);

        gameObject.GetComponent<TieredBuildingComponent>().tierChanged += UpdateMaxAnimalCapacity;

        gameObject.GetComponent<EnterableBuildingComponent>().EnteredOrExitedBuilding += SetAnimalMenuPosition;
        gameObject.GetComponent<EnterableBuildingComponent>().EnteredOrExitedBuilding += CloseAnimalMenu;
    }

    public override ComponentData Save() {
        ComponentData animalHouseData = new(typeof(AnimalHouseComponent));
        foreach (var animal in AnimalsInBuilding) {
            if (animalHouseData.TryGetComponentDataProperty(animal.ToString(), out JProperty property)) {
                property.Value = property.Value.Value<int>() + 1;
            }
            else animalHouseData.AddProperty(new JProperty(animal.ToString(), 1));
        }
        return animalHouseData;
    }

    public override void Load(ComponentData data) {
        foreach (JProperty property in data.GetAllComponentDataProperties()) {
            for (int count = 0; count < property.Value.Value<int>(); count++) AddAnimal(Enum.Parse<Animals>(property.Name));
        }
    }
}

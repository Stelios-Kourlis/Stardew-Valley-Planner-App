using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Utility.ClassManager;
using static Utility.TilemapManager;

public class OnboradingFlow : MonoBehaviour {

    [SerializeField] private GameObject tutorialText;
    [SerializeField] private GameObject interactionLegend;
    public bool IsInTutorial { get; private set; }

    void Start() {
        if (PlayerPrefs.GetInt("HasDoneIntro") == 0) StartOnboardingFlow();
        IsInTutorial = false;
    }

    public void StartOnboardingFlow() {

        IEnumerator OnboardingFlow() {
            BuildingController.DeleteAllBuildings(true);
            float cameraMoveTime = 2f;
            float delay = 0.25f;

            CameraController.Instance.SetPositionSmooth(GetGridTilemap().CellToWorld(MapController.Instance.GetGreenhousePosition() + new Vector3Int(3, 3, 0)), cameraMoveTime);
            yield return new WaitForSecondsRealtime(cameraMoveTime);
            BuildingController.PlaceGreenhouse(out _);
            yield return new WaitForSecondsRealtime(delay);

            CameraController.Instance.SetPositionSmooth(GetGridTilemap().CellToWorld(MapController.Instance.GetShippingBinPosition() + new Vector3Int(1, 0, 0)), cameraMoveTime);
            yield return new WaitForSecondsRealtime(cameraMoveTime);
            BuildingController.PlaceBin(out _);
            yield return new WaitForSecondsRealtime(delay);

            CameraController.Instance.SetPositionSmooth(GetGridTilemap().CellToWorld(MapController.Instance.GetHousePosition() + new Vector3Int(4, 5, 0)), cameraMoveTime);
            yield return new WaitForSecondsRealtime(cameraMoveTime);
            BuildingController.PlaceHouse(out _);
            yield return new WaitForSecondsRealtime(delay);

            GameObject text = Instantiate(tutorialText, GameObject.FindGameObjectWithTag("Canvas").transform);
            text.GetComponentInChildren<TMP_Text>().text = "When creating a new farm, the\n-Greenhouse\n-House\n-Shipping Bin\nare always placed automatically.";
            text.GetComponentInChildren<Button>().onClick.AddListener(() => {
                Destroy(text);
                ShowHowToMoveCamera();
            });
        }

        MoveablePanel.CloseAllPanels?.Invoke();
        IsInTutorial = true;
        BuildingController.SetCurrentAction(Actions.DO_NOTHING);
        GetCanvasGameObject().transform.Find("TopRightButtons").gameObject.SetActive(false);
        GetCanvasGameObject().transform.Find("ToggleBuildingSelectButton").gameObject.SetActive(false);
        StartCoroutine(OnboardingFlow());
    }

    public void ShowHowToMoveCamera() {

        IEnumerator CheckCameraMoved() {
            Vector3 cameraPosition = CameraController.Instance.GetPosition();
            float cameraZoom = CameraController.Instance.GetZoom();

            GameObject text = Instantiate(tutorialText, GameObject.FindGameObjectWithTag("Canvas").transform);
            text.GetComponentInChildren<TMP_Text>().text = "Move the camera by moving the mouse and holding middle mouse button.\nZoom in and out by scrolling.\nTry it out!";
            text.transform.GetChild(1).gameObject.SetActive(false);

            bool movedCamera = false, changedZoom = false;
            while (true) {
                if (Vector3.Distance(cameraPosition, CameraController.Instance.GetPosition()) >= 10) movedCamera = true;
                if (Mathf.Abs(cameraZoom - CameraController.Instance.GetZoom()) >= 3) changedZoom = true;

                if (movedCamera && changedZoom) break;
                yield return null;
            }

            text.transform.GetChild(1).gameObject.SetActive(true);
            text.GetComponentInChildren<Button>().onClick.AddListener(() => {
                Destroy(text);
                ShowHowToPlaceBuilding();
            });

        }

        StartCoroutine(CheckCameraMoved());
    }

    public void ShowHowToPlaceBuilding() {
        GetCanvasGameObject().transform.Find("ToggleBuildingSelectButton").gameObject.SetActive(true);
        GameObject text = Instantiate(tutorialText, GameObject.FindGameObjectWithTag("Canvas").transform);
        text.GetComponentInChildren<TMP_Text>().text = "To select a building to place press the button on the top left corner. Try placing a building!";
        text.transform.GetChild(1).gameObject.SetActive(false);

        void onBuildingSelectOpen() {
            Destroy(text);
            GetCanvasGameObject().transform.Find("ToggleBuildingSelectButton").GetComponent<Button>().onClick.RemoveListener(onBuildingSelectOpen);
        }
        GetCanvasGameObject().transform.Find("ToggleBuildingSelectButton").GetComponent<Button>().onClick.AddListener(onBuildingSelectOpen);
        BuildingController.anyBuildingPositionChanged += ShowActions;
    }

    public void ShowActions() {
        BuildingController.anyBuildingPositionChanged -= ShowActions;

        GameObject text = Instantiate(tutorialText, GameObject.FindGameObjectWithTag("Canvas").transform);
        GameObject topRightButtons = GetCanvasGameObject().transform.Find("TopRightButtons").gameObject;
        topRightButtons.SetActive(true);
        topRightButtons.transform.Find("settingsButton").gameObject.SetActive(false);
        topRightButtons.transform.Find("ShowTotalMaterials").gameObject.SetActive(false);
        text.GetComponentInChildren<TMP_Text>().text = "Click the top right arrow to open the actions menu.\nFrom there you can select different actions to perform.";
        text.transform.GetChild(1).gameObject.SetActive(false);

        void onActionButtonClick() {
            Destroy(text);
            ShowWhatActionsDo();
            topRightButtons.transform.Find("ActionButtons").Find("CloseMenuButton").GetComponent<Button>().onClick.RemoveListener(onActionButtonClick);
        }

        topRightButtons.transform.Find("ActionButtons").Find("CloseMenuButton").GetComponent<Button>().onClick.AddListener(onActionButtonClick);
    }

    public void ShowWhatActionsDo() {
        GameObject text = Instantiate(tutorialText, GameObject.FindGameObjectWithTag("Canvas").transform);
        text.GetComponentInChildren<TMP_Text>().text = "The Actions you can do are:\n-Place\n-Pick Up\n-Delete\nTry one of them on the building you just placed!";
        text.transform.GetChild(1).gameObject.SetActive(false);

        void onBuildingPlaced() {
            Destroy(text);
            BuildingController.anyBuildingPositionChanged -= onBuildingPlaced;
            ShowHowToInteractWithBuildings();
        }

        BuildingController.anyBuildingPositionChanged += onBuildingPlaced;
    }

    public void ShowHowToInteractWithBuildings() {
        BuildingController.anyBuildingPositionChanged -= ShowHowToInteractWithBuildings;
        CameraController.Instance.SetPositionSmooth(GetGridTilemap().CellToWorld(MapController.Instance.GetHousePosition() + new Vector3Int(4, 5, 0)), 1f);
        GameObject text = Instantiate(tutorialText, GameObject.FindGameObjectWithTag("Canvas").transform);
        text.GetComponentInChildren<TMP_Text>().text = "Some buildings can be interacted with.\nRight Click on the House to see what you can do with it!";

        void onBuildingRightClick() {
            Destroy(text);
            InteractableBuildingComponent.BuildingWasRightClicked -= onBuildingRightClick;
            ShowWhatInteractionsDo();
        }

        InteractableBuildingComponent.BuildingWasRightClicked += onBuildingRightClick;
    }

    public void ShowWhatInteractionsDo() {
        GameObject legend = Instantiate(interactionLegend, GameObject.FindGameObjectWithTag("Canvas").transform);
        GameObject text = Instantiate(tutorialText, GameObject.FindGameObjectWithTag("Canvas").transform);
        text.GetComponentInChildren<TMP_Text>().text = "These are the all available interactions, different buildings have different interactions.";
        text.transform.GetChild(1).gameObject.SetActive(false);

        void onBuildingInteraction() {
            Destroy(text);
            Destroy(legend);
            BuildingButtonController.anyActionWasClicked -= onBuildingInteraction;
            ShowSettingsAndTotalMaterialCost();
        }
        BuildingButtonController.anyActionWasClicked += onBuildingInteraction;
    }

    public void ShowSettingsAndTotalMaterialCost() {
        GameObject topRightButtons = GetCanvasGameObject().transform.Find("TopRightButtons").gameObject;
        topRightButtons.transform.Find("settingsButton").gameObject.SetActive(true);
        topRightButtons.transform.Find("ShowTotalMaterials").gameObject.SetActive(true);

        GameObject text = Instantiate(tutorialText, GameObject.FindGameObjectWithTag("Canvas").transform);
        text.GetComponentInChildren<TMP_Text>().text = "Lastly you can open the settings or see the total cost of your farm by pressing the buttons on the top right corner.";
        text.GetComponentInChildren<Button>().onClick.AddListener(() => {
            Destroy(text);
            EndOnboardingFlow();
        });
    }

    public void EndOnboardingFlow() {
        GameObject topRightButtons = GetCanvasGameObject().transform.Find("TopRightButtons").gameObject;
        topRightButtons.SetActive(true);
        topRightButtons.transform.Find("settingsButton").gameObject.SetActive(true);
        topRightButtons.transform.Find("ShowTotalMaterials").gameObject.SetActive(true);
        GetCanvasGameObject().transform.Find("ToggleBuildingSelectButton").gameObject.SetActive(true);

        GameObject text = Instantiate(tutorialText, GameObject.FindGameObjectWithTag("Canvas").transform);
        text.GetComponentInChildren<TMP_Text>().text = "That's all! Start building and experimenting with your farm! You can always redo the tutorial through the settings.";
        text.GetComponentInChildren<Button>().onClick.AddListener(() => {
            Destroy(text);
        });
    }

}

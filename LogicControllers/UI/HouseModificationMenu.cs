using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static HouseExtensionsComponent;

public class HouseModificationMenu : MonoBehaviour {

    private Transform Content => transform.Find("TabContent");
    public Action<int> spouseChanged;
    [SerializeField] private GameObject wallpaperButtonPrefab;
    [SerializeField] private GameObject flooringButtonPrefab;

    public void Start() {
        var dropdown = Content.Find("Modifications").Find("Marriage").Find("spouse").Find("Dropdown").GetComponent<TMP_Dropdown>();
        dropdown.onValueChanged.AddListener((int value) => {
            spouseChanged?.Invoke(value);
        });

        for (int wallpaperId = 0; wallpaperId < WallsComponent.TotalWallpaperTextures - 1; wallpaperId++) {
            GameObject wallpaperButton = Instantiate(wallpaperButtonPrefab, GetWallpaperContent());
            wallpaperButton.transform.Find("Mask").Find("WallpaperImage").GetComponent<Image>().sprite = WallsComponent.GetWallpaperSprite(wallpaperId);
            int id = wallpaperId; //Capture id outside lambda
            wallpaperButton.GetComponent<Button>().onClick.AddListener(() => {
                WallsComponent.SetSelectedWallpaper(id);
                BuildingController.SetCurrentAction(Actions.PLACE_WALLPAPER);
            });
        }

        for (int floorId = 0; floorId < FlooringComponent.TotalFloorTextures - 1; floorId++) {
            GameObject flooringButton = Instantiate(flooringButtonPrefab, GetFlooringContent());
            flooringButton.transform.Find("Mask").Find("FlooringImage").GetComponent<Image>().sprite = FlooringComponent.GetFloorSprite(floorId);
            int id = floorId; //Capture id outside lambda
            flooringButton.GetComponent<Button>().onClick.AddListener(() => {
                FlooringComponent.SetSelectedFloor(id);
                BuildingController.SetCurrentAction(Actions.PLACE_FLOORING);
            });
        }

    }

    public void DisableModificationTab() {
        transform.Find("Tabs").Find("Modifications").gameObject.SetActive(false);
    }

    public void SetSpouseDropdownInteractability(bool interactable) {
        Content.Find("Modifications").Find("Marriage").Find("spouse").Find("Dropdown").GetComponent<TMP_Dropdown>().interactable = interactable;
    }

    public Toggle GetMarriageToggle() {
        return Content.Find("Modifications").Find("Marriage").Find("Checkbox").GetComponent<Toggle>();
    }

    public void SetSpouseDropdownValue(int value) {
        Content.Find("Modifications").Find("Marriage").Find("spouse").Find("Dropdown").GetComponent<TMP_Dropdown>().value = value;
    }

    public Toggle GetModificationToggle(HouseModifications modificationName) {
        if (modificationName == HouseModifications.Marriage) return Content.Find("Modifications").Find("Marriage").Find("Checkbox").GetComponent<Toggle>();
        return Content.Find("Modifications").Find("Renovations").Find(modificationName.ToString()).GetComponent<Toggle>();
    }

    public Image GetModificationSprite(HouseModifications modificationName) {
        if (modificationName == HouseModifications.Marriage) return Content.Find("Modifications").Find("Marriage").Find("Checkbox").Find("Image").GetComponent<Image>();
        return Content.Find("Modifications").Find("Renovations").Find(modificationName.ToString()).Find("Button").GetComponent<Image>();
    }

    public void SetAllToglesSpritesToOff() {
        Sprite offSprite = Resources.Load<Sprite>("UI/CheckBoxOff");
        foreach (Transform toggle in Content.Find("Modifications").Find("Renovations")) {
            if (toggle.TryGetComponent(out Toggle _)) {
                toggle.Find("Button").GetComponent<Image>().sprite = offSprite;
            }
        }
        Resources.UnloadAsset(offSprite);
    }

    public void SetSpriteToOff(HouseModifications modificationName) {
        Sprite offSprite = Resources.Load<Sprite>("UI/CheckBoxOff");
        Content.Find("Modifications").Find("Renovations").Find(modificationName.ToString()).Find("Button").GetComponent<Image>().sprite = offSprite;
    }

    private Transform GetWallpaperContent() {
        return Content.Find("Wallpaper").Find("Content");
    }

    private Transform GetFlooringContent() {
        return Content.Find("Flooring").Find("Content");
    }


}

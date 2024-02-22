using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class FishImageRetriever : MonoBehaviour {
    // Start is called before the first frame update
    Building building;

    void Start() {
        gameObject.GetComponent<Button>().onClick.AddListener(SetFishImage);
    }

    public void SetBuilding(Building building) {
        this.building = building;
    }

    public void SetFishImage() {
        //GameObject fishButton = gameObject.transform.parent.parent.parent.parent.transform.GetChild(0).GetChild(0).gameObject;
        GameObject fishButton = gameObject.transform.parent.parent.parent.parent.GetChild(0).gameObject;
        fishButton.GetComponent<Image>().sprite = gameObject.GetComponent<Image>().sprite;
        if (fishButton.GetComponent<Image>().color.a == 0) fishButton.GetComponent<Image>().color = new Color(255, 255, 255, 255);
        Fish fishType = (Fish)Enum.Parse(typeof(Fish), gameObject.GetComponent<Image>().sprite.name);
        Debug.Log(fishType);
        Color color = fishType switch
        { // RGB 0-255 dont work so these are the values normalized to 0-1
            Fish.LavaEel => new Color(0.7490196f, 0.1137255f, 0.1333333f, 1),
            Fish.SuperCucumber => new Color(0.4117647f, 0.3294118f, 0.7490196f, 1),
            Fish.Slimejack => new Color(0.08886068f, 0.7490196f, 0.003921576f, 1),
            Fish.VoidSalmon => new Color(0.5764706f, 0.1176471f, 0.7490196f, 1),
            _ => new Color(0.2039216f, 0.5254902f, 0.7490196f, 1),
        };
        building.tilemap.gameObject.transform.GetChild(0).GetComponent<Tilemap>().color = color;
    }

    
}

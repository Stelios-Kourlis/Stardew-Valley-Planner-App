using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class FishImageRetriever : MonoBehaviour {
    // Start is called before the first frame update
    FishPond fishPond;

    void Start() {
        gameObject.GetComponent<Button>().onClick.AddListener(SetFishImage);
    }

    public void SetBuilding(FishPond fishPond) {
        this.fishPond = fishPond;
    }

    public void SetFishImage() {
        Fish fishType = (Fish)Enum.Parse(typeof(Fish), gameObject.GetComponent<Image>().sprite.name);
        fishPond.SetFishImage(fishType);
    }

    
}

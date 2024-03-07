using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.U2D;

public class Craftables : Building{
    
    private SpriteAtlas atlas;
    public CraftableUtility? CraftableType {get; private set;} = null;

    public void Awake(){
        baseHeight = 1;
        base.Start();
        atlas = Resources.Load<SpriteAtlas>("Buildings/Placeables/PlaceablesAtlas");
        if (CraftableType == null) SetCraftable(CraftableUtility.Beehouse);
    } 

    public new void Update(){
        //Debug.Log($"Craftable Type is {CraftableType}");
        if (sprite == null){
            Debug.LogWarning("Sprite is null");
            SetCraftable((CraftableUtility)CraftableType);
        }//todo for some reason when switchng craftables, CraftableType is Beehouse and sprite is null
        base.Update();
    }

    public void SetCraftable(CraftableUtility craftable){
        CraftableType = craftable;
        UpdateTexture(atlas.GetSprite(craftable.ToString()));
        Debug.Log($"Set Craftable to {craftable} in Craftables (UID: {gameObject.GetInstanceID()}), Sprite = null: {sprite == null} " + (atlas.GetSprite(craftable.ToString()) == null));
    }

    public override List<MaterialInfo> GetMaterialsNeeded(){
        return CraftableType switch{//todo implement
            _ => new List<MaterialInfo>()
        };
    }

    public override void RecreateBuildingForData(int x, int y, params string[] data){
        Place(new Vector3Int(x,y,0));
        CraftableType = (CraftableUtility)int.Parse(data[4]);
    }

    public override string GetBuildingData(){
        return base.GetBuildingData() + $"|{(int)CraftableType}";
    }

}

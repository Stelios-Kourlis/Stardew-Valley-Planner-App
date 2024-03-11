using UnityEngine;
using UnityEngine.U2D;

public class MaterialInfo{

    private static SpriteAtlas spriteAtlas;
    public int amount;
    public readonly Materials? name;
    public bool IsSpecial { get {return name == null;} }
    // public readonly Sprite sprite;

    /// <summary> in case a player need to perform an action rather than give a material, ex. obtaining rarecrows, Greenhouse write a 1 sentence instruction here </summary>
    public readonly string howToGet;


    public MaterialInfo(int amount, Materials name){
        this.amount = amount;
        howToGet = null;
        this.name = name;
        spriteAtlas = Resources.Load<SpriteAtlas>("Materials/MaterialAtlas");
    }    

    public MaterialInfo(string howToGet){
        amount = 1;
        name = null;
        this.howToGet = howToGet;
        spriteAtlas = Resources.Load<SpriteAtlas>("Materials/MaterialAtlas");
    } 

    public override string ToString(){
        if (IsSpecial) return "SpecialMat";
        else return name.ToString();
    }
}

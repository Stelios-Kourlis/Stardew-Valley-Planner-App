using UnityEngine;

public class MaterialInfo{

    public readonly int amount;
    public readonly Materials? name;
    //public readonly Sprite sprite;

    /// <summary>
    /// in case a player need to perform an action rather than give a material, ex. obtaining rarecrows, Greenhouse write a 1 sentence instruction here
    /// </summary>
    public readonly string howToGet;

    // public MaterialInfo(int amount, Materials name, Sprite sprite, string howToGet = null){
    //     this.amount = amount;
    //     this.sprite = sprite;
    //     this.howToGet = null;
    //     this.name = null;
    //     if (howToGet != null) this.howToGet = howToGet;
    //     else this.name = name;
    // }

    public MaterialInfo(int amount, Materials name){
        this.amount = amount;
        howToGet = null;
        this.name = name;
    }    

    public MaterialInfo(string howToGet){
        amount = 0;
        name = null;
        this.howToGet = howToGet;
    } 
}

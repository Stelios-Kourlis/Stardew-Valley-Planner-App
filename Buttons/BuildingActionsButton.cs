using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BuildingActionsButton : MonoBehaviour{

    protected Texture2D icon;
    protected float sizeScale;
    protected Vector3Int position;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public abstract void Initialize();
}

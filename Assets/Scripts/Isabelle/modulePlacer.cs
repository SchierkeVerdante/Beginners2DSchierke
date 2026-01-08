using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class modulePlacer : MonoBehaviour
{
    [SerializeField]
    public GameObject _module;

    [SerializeField]
    private Grid _tilegrid;

    [SerializeField]
    private Tilemap _tilemap;

    // [SerializeField]
    // private int moduleCount;

    [SerializeField]
    private Sprite _module1;

    [SerializeField]
    private Sprite _module2;
    
    [SerializeField]
    private Sprite _module3;
    
    [SerializeField]
    private Sprite _module4;
    
    [SerializeField]
    private Sprite _module5;
    
    [SerializeField]
    private Sprite _module6;

    [SerializeField]
    private Sprite _module7;
    
    [SerializeField]
    private Sprite _module8;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    bool CheckSpot(){
        //Module count is the # of modules that spawn on the map.
        //Module count, unlike enemies and obstacles, is not randomized;
        //Once all objects have been placed, we stop placing them.

        //Give each spot a small chance of spawning. If not enough spawns, place function will just cycle back through
        int moduleChance = Random.Range(1,301); 
        if(moduleChance<=1){
            Debug.Log("Placed a module");
            return true;
        }
        return false;
    }


    public void MakeModules(List<string> moduleOptions, int _borderSizeX, int _borderSizeY, int moduleCount){
        BoundsInt bounds = _tilemap.cellBounds;
        int width = bounds.size.x;
        int height = bounds.size.y;
        int lowX = bounds.xMin+_borderSizeX;
        int lowY = bounds.yMin+_borderSizeY;
        int hiX = bounds.xMax-_borderSizeX;
        int hiY = bounds.yMax-_borderSizeY;
        int modulesPlaced = 0;

        while(modulesPlaced<moduleCount-1){
            for(int i=lowX; i<hiX+1;i++){
                for(int j=lowY; j<hiY+1;j++){
                    bool hasModule = CheckSpot();
                    if(hasModule){
                        //Create prefab
                        GameObject moduleInstance = Instantiate(_module);

                        //Set prefab location
                        moduleInstance.transform.position=new Vector3(i,j,0);
                        // obstacleInstance.transform.position.x=i;
                        // obstacleInstance.transform.position.y=j;

                        //Set prefab image based on terrainOption
                        SpriteRenderer spriteRending= moduleInstance.GetComponent<SpriteRenderer>();
                        switch(moduleOptions[modulesPlaced]){
                            case "1":
                                spriteRending.sprite=_module1;
                                break;
                            case "2":
                                spriteRending.sprite=_module2;
                                break;
                            case "3":
                                spriteRending.sprite=_module3;
                                break;
                            case "4":
                                spriteRending.sprite=_module4;
                                break;
                            case "5":
                                spriteRending.sprite=_module5;
                                break;
                            case "6":
                                spriteRending.sprite=_module6;
                                break;
                            case "7":
                                spriteRending.sprite=_module7;
                                break;
                            case "8":
                                spriteRending.sprite=_module8;
                                break;
                            default:
                                spriteRending.sprite=_module8;
                                break;
                        }
                        Debug.Log("Modules placed was "+modulesPlaced);
                        modulesPlaced = modulesPlaced+1;
                        Debug.Log("now "+modulesPlaced);
                        if(modulesPlaced==moduleCount){
                            return;
                        }
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

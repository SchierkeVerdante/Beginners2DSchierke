using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class OilPlacer : MonoBehaviour
{
    [SerializeField]
    public GameObject _oil;

    [SerializeField]
    private Grid _tilegrid;

    [SerializeField]
    private Tilemap _tilemap;

    // [SerializeField]
    // private int oilCount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    bool CheckSpot(){
        //Oil count is the # of oil tanks that spawn on the map.
        //Oil count, unlike enemies and obstacles, is not randomized;
        //Once all objects have been placed, we stop placing them.

        //Give each spot a small chance of spawning. If not enough spawns, place function will just cycle back through
        int oilChance = Random.Range(1,301); 
        if(oilChance<=1){
            Debug.Log("Placed an oil tank");
            return true;
        }
        return false;
    }


    public void MakeOil(int _borderSizeXlw, int _borderSizeYlw,int _borderSizeXhi, int _borderSizeYhi,int oilCount){
        BoundsInt bounds = _tilemap.cellBounds;
        int width = bounds.size.x;
        int height = bounds.size.y;
        int lowX = bounds.xMin+_borderSizeXlw;
        int lowY = bounds.yMin+_borderSizeYlw;
        int hiX = bounds.xMax-_borderSizeXhi;
        int hiY = bounds.yMax-_borderSizeYhi;
        int oilPlaced = 0;

        while(oilPlaced<oilCount-1){
            for(int i=lowX; i<hiX+1;i++){
                for(int j=lowY; j<hiY+1;j++){
                    bool hasOil = CheckSpot();
                    if(hasOil){
                        //Create prefab
                        GameObject oilInstance = Instantiate(_oil);

                        //Set prefab location
                        oilInstance.transform.position=new Vector3(i,j,0);
                        // obstacleInstance.transform.position.x=i;
                        // obstacleInstance.transform.position.y=j;

                        
                        Debug.Log("Oil placed was "+oilPlaced);
                        oilPlaced = oilPlaced+1;
                        Debug.Log("now "+oilPlaced);
                        if(oilPlaced==oilCount){
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

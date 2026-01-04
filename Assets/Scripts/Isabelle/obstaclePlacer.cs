using UnityEngine;
using UnityEngine.Tilemaps;

public class obstaclePlacer : MonoBehaviour
{
    [SerializeField]
    public GameObject _obstacle;

    [SerializeField]
    private Grid _tilegrid;

    [SerializeField]
    private Tilemap _tilemap;

    [SerializeField]
    private Sprite _jungleObstacle;

    [SerializeField]
    private Sprite _rockObstacle;
    
    [SerializeField]
    private Sprite _craterObstacle;
    
    [SerializeField]
    private Sprite _islandObstacle;
    
    [SerializeField]
    private Sprite _snowObstacle;
    
    [SerializeField]
    private Sprite _sandObstacle;

    [SerializeField]
    private Sprite _waterObstacle;
    
    [SerializeField]
    private Sprite _lavaObstacle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    bool CheckSpot(int obstacleDensity){
        //Obstacle density is value 1 to 20 (1% to 20% of grid space)
        int obstacleChance = Random.Range(1,101);
        if(obstacleChance<=obstacleDensity){
            return true;
        }
        return false;
    }


    public void MakeObstacles(string terrainOption,int obstacleDensity){
        BoundsInt bounds = _tilemap.cellBounds;
        int width = bounds.size.x;
        int height = bounds.size.y;
        int lowX = bounds.xMin;
        int lowY = bounds.yMin;
        int hiX = bounds.xMax;
        int hiY = bounds.yMax;

        for(int i=lowX; i<hiX+1;i++){
            for(int j=lowY; j<hiY+1;j++){
                bool hasObstacle = CheckSpot(obstacleDensity);
                if(hasObstacle){
                    //Create prefab
                    GameObject obstacleInstance = Instantiate(_obstacle);

                    //Set prefab location
                    obstacleInstance.transform.position=new Vector3(i,j,0);
                    // obstacleInstance.transform.position.x=i;
                    // obstacleInstance.transform.position.y=j;

                    //Set prefab image based on terrainOption
                    SpriteRenderer spriteRending= obstacleInstance.GetComponent<SpriteRenderer>();
                    switch(terrainOption){
                        case "crater":
                            spriteRending.sprite=_craterObstacle;
                            break;
                        case "sand":
                            spriteRending.sprite=_sandObstacle;
                            break;
                        case "rock":
                            spriteRending.sprite=_rockObstacle;
                            break;
                        case "lava":
                            spriteRending.sprite=_lavaObstacle;
                            break;
                        case "jungle":
                            spriteRending.sprite=_jungleObstacle;
                            break;
                        case "island":
                            spriteRending.sprite=_islandObstacle;
                            break;
                        case "snow":
                            spriteRending.sprite=_snowObstacle;
                            break;
                        case "water":
                            spriteRending.sprite=_waterObstacle;
                            break;
                        default:
                            spriteRending.sprite=_craterObstacle;
                            break;
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

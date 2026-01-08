using UnityEngine;
using UnityEngine.Tilemaps;
// using System;

public class obstaclePlacer : MonoBehaviour
{
    [SerializeField]
    public int _playerSpawnX;
    [SerializeField]
    public int _playerSpawnY;
    [SerializeField]
    public int _noSpawnRadius=2;

    [SerializeField]
    public GameObject _obstacle;

    [SerializeField]
    public GameObject _obstacle2x2;

    [SerializeField]
    public GameObject _obstacle2x1;

    [SerializeField]
    public GameObject _obstacle1x2;

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

    [SerializeField]
    private Sprite _lavaFlow;

    [SerializeField]
    private Sprite _storm;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    bool CheckSpot(int obstacleDensity){
        //Obstacle density is value 1 to 20 (1% to 20% of grid space)
        int obstacleChance = Random.Range(1,201);
        if(obstacleChance<=obstacleDensity){
            return true;
        }
        return false;
    }


    public void MakeObstacles(string terrainOption,int obstacleDensity, int _borderSizeX, int _borderSizeY){
        BoundsInt bounds = _tilemap.cellBounds;
        int width = bounds.size.x;
        int height = bounds.size.y;
        int lowX = bounds.xMin+_borderSizeX;
        int lowY = bounds.yMin+_borderSizeY;
        int hiX = bounds.xMax-_borderSizeX;
        int hiY = bounds.yMax-_borderSizeY;

        for(int j=lowY; j<hiY+1;j=j+2){
            for(int i=lowX; i<hiX+1;i++){
                bool hasObstacle = CheckSpot(obstacleDensity);
                if(Mathf.Abs(i)<_noSpawnRadius&Mathf.Abs(j)<_noSpawnRadius){
                // if(i==_playerSpawnX&j==_playerSpawnY){
                    hasObstacle=false;
                }
                if(hasObstacle){
                GameObject obstacleInstance;
                switch(terrainOption){
                    case "crater":
                        obstacleInstance = Instantiate(_obstacle2x2);
                        break;
                    case "rock":
                        obstacleInstance = Instantiate(_obstacle1x2);
                        break;
                    case "lava":
                        obstacleInstance = Instantiate(_obstacle2x1);
                        break;
                    case "jungle":
                        obstacleInstance = Instantiate(_obstacle1x2);
                        break;
                    case "island":
                        obstacleInstance = Instantiate(_obstacle2x1);
                        break;
                    case "snow":
                        obstacleInstance = Instantiate(_obstacle2x2);
                        break;
                    case "water":
                        obstacleInstance = Instantiate(_obstacle2x1);
                        break;
                    default:
                        obstacleInstance = Instantiate(_obstacle2x2);
                        break;
                }

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
                            spriteRending.sprite=_lavaFlow;
                            // spriteRending.sprite=_lavaObstacle;
                            break;
                        case "jungle":
                            spriteRending.sprite=_jungleObstacle;
                            break;
                        case "island":
                            spriteRending.sprite=_islandObstacle;
                            break;
                        case "snow":
                            spriteRending.sprite=_storm;
                            // spriteRending.sprite=_snowObstacle;
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

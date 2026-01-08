using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class enemyPlacer : MonoBehaviour
{
    [SerializeField]
    public GameObject _enemy;

    [SerializeField]
    private Grid _tilegrid;

    [SerializeField]
    private Tilemap _tilemap;

    [SerializeField]
    public int _noSpawnRadius=2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    bool CheckSpot(int spawnRate){
        //Spawn rate is value 1 to 10 (1 to 10 enemies for a 20x20 grid)
        int enemyChance = Random.Range(1,401);
        if(enemyChance<=spawnRate){
            return true;
        }
        return false;
    }


    public void MakeEnemies(int spawnRate, int _borderSizeX, int _borderSizeY,List<GameObject> enemyOptions){
        BoundsInt bounds = _tilemap.cellBounds;
        int width = bounds.size.x;
        int height = bounds.size.y;
        int lowX = bounds.xMin+_borderSizeX;
        int lowY = bounds.yMin+_borderSizeY;
        int hiX = bounds.xMax-_borderSizeX;
        int hiY = bounds.yMax-_borderSizeY;

        int numEnemyOptions = enemyOptions.Count;

        for(int i=lowX; i<hiX+1;i++){
            for(int j=lowY; j<hiY+1;j++){
                bool hasEnemy = CheckSpot(spawnRate);
                if(Mathf.Abs(i)<=_noSpawnRadius&Mathf.Abs(j)<=_noSpawnRadius){
                // if(i==_playerSpawnX&j==_playerSpawnY){
                    hasEnemy=false;
                }
                if(hasEnemy){
                    int enemyChoice = Random.Range( 0, numEnemyOptions);
                    //Create prefab
                    GameObject enemyInstance = Instantiate(enemyOptions[enemyChoice]);
                    // GameObject enemyInstance = Instantiate(_enemy);

                    //Set prefab location
                    enemyInstance.transform.position=new Vector3(i,j,0);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

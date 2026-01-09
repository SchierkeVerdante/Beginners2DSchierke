using ModestTree;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyPlacer : MonoBehaviour
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
        int enemyChance = Random.Range(1,801);
        if(enemyChance<=spawnRate){
            return true;
        }
        return false;
    }


    public void MakeEnemies(int spawnRate, int _borderSizeXlw, int _borderSizeYlw,int _borderSizeXhi, int _borderSizeYhi,List<GameObject> enemyOptions){
        BoundsInt bounds = _tilemap.cellBounds;
        int width = bounds.size.x;
        int height = bounds.size.y;
        int lowX = bounds.xMin+_borderSizeXlw;
        int lowY = bounds.yMin+_borderSizeYlw;
        int hiX = bounds.xMax-_borderSizeXhi;
        int hiY = bounds.yMax-_borderSizeYhi;

        int numEnemyOptions = enemyOptions.Count;

        for(int i=lowX; i<hiX+1;i++){
            for(int j=lowY; j<hiY+1;j++){
                bool hasEnemy = CheckSpot(spawnRate);
                if(Mathf.Abs(i)<=_noSpawnRadius&Mathf.Abs(j)<=_noSpawnRadius){
                // if(i==_playerSpawnX&j==_playerSpawnY){
                    hasEnemy=false;
                }
                if(hasEnemy){
                    if (enemyOptions.IsEmpty()) return;
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

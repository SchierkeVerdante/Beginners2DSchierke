using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TerrainPicker : MonoBehaviour
{ 

    [SerializeField]
    private int _spawnRate=3; //Set from 1-10: # of enemies you'd expect to spawn in a 20x20 grid

    [SerializeField]
    private int _obstacleDensity=5; //Set from 1-20: % of tiles to be covered by obstacles.

    [SerializeField]
    private List<string> _modulesList;

    [SerializeField]
    private int _borderSizeX=15;

    [SerializeField]
    private int _borderSizeY=5;

    [SerializeField]
    private obstaclePlacer _obstaclePlacer;
    
    [SerializeField]
    private enemyPlacer _enemyPlacer;

    [SerializeField]
    private modulePlacer _modulePlacer;

    [SerializeField]
    private Tilemap _tilemap;

    [SerializeField]
    private Grid _tilegrid;

    [SerializeField]
    private TileBase _tilebase;

    [SerializeField]
    private Tile _cave1;
    [SerializeField]
    private Tile _cave2;
    [SerializeField]
    private Tile _cave3;
    [SerializeField]
    private Tile _cave4;

    [SerializeField]
    private Tile _rocks1;
    [SerializeField]
    private Tile _rocks2;
    [SerializeField]
    private Tile _rocks3;
    [SerializeField]
    private Tile _rocks4;

    [SerializeField]
    private Tile _water1;
    [SerializeField]
    private Tile _water2;
    [SerializeField]
    private Tile _water3;
    [SerializeField]
    private Tile _water4;

    [SerializeField]
    private Tile _snow1;
    [SerializeField]
    private Tile _snow2;
    [SerializeField]
    private Tile _snow3;
    [SerializeField]
    private Tile _snow4;

    [SerializeField]
    private Tile _ice1;
    [SerializeField]
    private Tile _ice2;
    [SerializeField]
    private Tile _ice3;
    [SerializeField]
    private Tile _ice4;

    [SerializeField]
    private Tile _crater1;
    [SerializeField]
    private Tile _crater2;
    [SerializeField]
    private Tile _crater3;
    [SerializeField]
    private Tile _crater4;

    [SerializeField]
    private Tile _lava1;
    [SerializeField]
    private Tile _lava2;
    [SerializeField]
    private Tile _lava3;
    [SerializeField]
    private Tile _lava4;

    [SerializeField]
    private Tile _sand1;
    [SerializeField]
    private Tile _sand2;
    [SerializeField]
    private Tile _sand3;
    [SerializeField]
    private Tile _sand4;

    // private Tile TerrainBase;

    private Tile _opt1;
    private Tile _opt2;
    private Tile _opt3;
    private Tile _opt4;

    public string _terrainLabel = "crater";

    void Start(){
        setTiles();
    }

    private void setTiles(string selectedTerrain="random"){
        BoundsInt bounds = _tilemap.cellBounds;
        Debug.Log("Size:");
        int width = bounds.size.x;
        Debug.Log("width:"+width);
        int height = bounds.size.y;
        Debug.Log("height:"+height);
        int lowX = bounds.xMin;
        int lowY = bounds.yMin;
        int hiX = bounds.xMax;
        int hiY = bounds.yMax;
        Debug.Log("bounds:"+hiX+"-"+lowX+","+hiY+"-"+lowY);
        


        switch(selectedTerrain) 
        {
            case "crater":
            _opt1 = _crater1;
            _opt2 = _crater2;
            _opt3 = _crater3;
            _opt4 = _crater4;
            _terrainLabel = "crater";
            break;
        case "sand":
            _opt1 = _sand1;
            _opt2 = _sand2;
            _opt3 = _sand3;
            _opt4 = _sand4;
            _terrainLabel = "sand";
            break;
        case "rock":
            _opt1 = _rocks1;
            _opt2 = _rocks2;
            _opt3 = _rocks3;
            _opt4 = _rocks4;
            _terrainLabel = "rock";
            break;
        case "lava":
            _opt1 = _lava1;
            _opt2 = _lava2;
            _opt3 = _lava3;
            _opt4 = _lava4;
            _terrainLabel = "lava";
            break;
        case "cave":
            _opt1 = _cave1;
            _opt2 = _cave2;
            _opt3 = _cave3;
            _opt4 = _cave4;
            _terrainLabel = "cave";
            break;
        case "ice":
            _opt1 = _ice1;
            _opt2 = _ice2;
            _opt3 = _ice3;
            _opt4 = _ice4;
            _terrainLabel = "ice";
            break;
        case "snow":
            _opt1 = _snow1;
            _opt2 = _snow2;
            _opt3 = _snow3;
            _opt4 = _snow4;
            _terrainLabel = "snow";
            break;
        case "water":
            _opt1 = _water1;
            _opt2 = _water2;
            _opt3 = _water3;
            _opt4 = _water4;
            _terrainLabel = "water";
            break;
        default:
            pickRandomTerrain();
            break;
        }

        // TerrainBase = pickRandomTerrain();
        // pickRandomTerrain();

        for(int i=lowX; i<hiX+1;i++){
            for(int j=lowY; j<hiY+1;j++){
                int tervers = pickRandomTile();
                Tile TerrainOption;
                switch(tervers){
                    case 1:
                        TerrainOption = _opt1;
                        break;
                    case 2:
                        TerrainOption = _opt2;
                        break;
                    case 3:
                        TerrainOption = _opt3;
                        break;
                    case 4:
                        TerrainOption = _opt4;
                        break;
                    default:
                        TerrainOption = _opt1;
                        break;
                }
                setTileColor(i,j,TerrainOption);
            }
        }

        Debug.Log("Making Modules");
        _modulePlacer.MakeModules(_modulesList,_borderSizeX,_borderSizeY);

        Debug.Log("Making obstacles");
        _obstaclePlacer.MakeObstacles(_terrainLabel,_obstacleDensity);//set input to % of tiles having obstacles, 1-20
        
        Debug.Log("Making Enemies");
        _enemyPlacer.MakeEnemies(_spawnRate,_borderSizeX,_borderSizeY);//set input to spawn rate (avg # of enemies to spawn in a 20x20 grid)

    }

    private void pickRandomTerrain(){
        int terrainnum  = Random.Range(1, 9); 
        switch(terrainnum) 
        {
        case 1:
            _opt1 = _crater1;
            _opt2 = _crater2;
            _opt3 = _crater3;
            _opt4 = _crater4;
            _terrainLabel = "crater";
            return;
        case 2:
            _opt1 = _sand1;
            _opt2 = _sand2;
            _opt3 = _sand3;
            _opt4 = _sand4;
            _terrainLabel = "sand";
            return;
        case 3:
            _opt1 = _rocks1;
            _opt2 = _rocks2;
            _opt3 = _rocks3;
            _opt4 = _rocks4;
            _terrainLabel = "rock";
            return;
        case 4:
            _opt1 = _lava1;
            _opt2 = _lava2;
            _opt3 = _lava3;
            _opt4 = _lava4;
            _terrainLabel = "lava";
            return;
        case 5:
            _opt1 = _cave1;
            _opt2 = _cave2;
            _opt3 = _cave3;
            _opt4 = _cave4;
            _terrainLabel = "cave";
            return;
        case 6:
            _opt1 = _ice1;
            _opt2 = _ice2;
            _opt3 = _ice3;
            _opt4 = _ice4;
            _terrainLabel = "ice";
            return;
        case 7:
            _opt1 = _snow1;
            _opt2 = _snow2;
            _opt3 = _snow3;
            _opt4 = _snow4;
            _terrainLabel = "snow";
            return;
        case 8:
            _opt1 = _water1;
            _opt2 = _water2;
            _opt3 = _water3;
            _opt4 = _water4;
            _terrainLabel = "water";
            return;
        default:
            return;
        }

    }

    private int pickRandomTile(){
        int tilenum  = Random.Range(1, 5); 
        return tilenum;
    }

    private void setTileColor(int x, int y,Tile terraintile){
        Vector3Int position = new Vector3Int(x,y,0);
        _tilemap.SetTile(position,terraintile);
        return;
    }
}

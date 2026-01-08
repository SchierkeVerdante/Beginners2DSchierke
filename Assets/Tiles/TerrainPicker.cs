using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class TerrainPicker : MonoBehaviour
{ 

    // [SerializeField]
    // private int _spawnRate=3; //Set from 1-10: # of enemies you'd expect to spawn in a 20x20 grid

    // [SerializeField]
    // private int _obstacleDensity=5; //Set from 1-20: % of tiles to be covered by obstacles.

    // [SerializeField]
    // private List<string> _modulesList;

    [SerializeField]
    private int _borderSizeX=17;

    [SerializeField]
    private int _borderSizeY=7;

    [SerializeField]
    private obstaclePlacer _obstaclePlacer;
    
    [SerializeField]
    private enemyPlacer _enemyPlacer;

    [SerializeField]
    private modulePlacer _modulePlacer;

    [SerializeField]
    private GameObject _background;

    [SerializeField]
    private Tilemap _tilemap;

    [SerializeField]
    private Grid _tilegrid;

    [SerializeField]
    private TileBase _tilebase;

    [SerializeField]
    private Tile _jungle1;
    [SerializeField]
    private Tile _jungle2;
    [SerializeField]
    private Tile _jungle3;
    [SerializeField]
    private Tile _jungle4;

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
    private Tile _island1;
    [SerializeField]
    private Tile _island2;
    [SerializeField]
    private Tile _island3;
    [SerializeField]
    private Tile _island4;

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

    [SerializeField]
    private Color waterColor;
    [SerializeField]
    private Color sandColor;
    [SerializeField]
    private Color lavaColor;
    [SerializeField]
    private Color snowColor;
    [SerializeField]
    private Color rockColor;
    [SerializeField]
    private Color craterColor;
    [SerializeField]
    private Color jungleColor;
    [SerializeField]
    private Color islandColor;


    // private Tile TerrainBase;

    private Tile _opt1;
    private Tile _opt2;
    private Tile _opt3;
    private Tile _opt4;

    public string _terrainLabel = "rock";

    void Start(){
        // setTiles(_terrainLabel);
    }    
    
    private void OnSceneLoaded(Scene level, object data)
    {
        if (level.name == "Level_1" && data is TerrainSpawnStageConfig terrain)
        {
            setTiles(terrain.terrainType,terrain.spawnRate,terrain.obstacleDensity);
        }
    }
// Isabelle to add oil count and module count, oil spawners, enemy list for each land (serialize this)
    private void setTiles(string selectedTerrain="random", int enemyRate=1,int obsDensity=1){
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
        case "jungle":
            _opt1 = _jungle1;
            _opt2 = _jungle2;
            _opt3 = _jungle3;
            _opt4 = _jungle4;
            _terrainLabel = "jungle";
            break;
        case "island":
            _opt1 = _island1;
            _opt2 = _island2;
            _opt3 = _island3;
            _opt4 = _island4;
            _terrainLabel = "island";
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

        //Set the Color to the values gained from the Sliders
        Color background_color;

        //enums? (ess ints)
        switch(_terrainLabel){
            case "crater":
                background_color = craterColor;
                break;
            case "sand":
                background_color = sandColor;
                break;
            case "rock":
                background_color = rockColor;
                break;
            case "lava":
                background_color = lavaColor;
                break;
            case "jungle":
                background_color = jungleColor;
                break;
            case "island":
                background_color = islandColor;
                break;
            case "snow":
                background_color = snowColor;
                break;
            case "water":
                background_color = waterColor;
                break;
            default:
                background_color = new Color(255,255,255);
                break;
        }

        
        SpriteRenderer spriteRending= _background.GetComponent<SpriteRenderer>();
        //Set the SpriteRenderer to the Color defined by the Sliders
        spriteRending.color = background_color;

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

        // Debug.Log("Making Modules");
        // _modulePlacer.MakeModules(_modulesList,_borderSizeX,_borderSizeY);

        Debug.Log("Making obstacles");
        // _obstaclePlacer.MakeObstacles(_terrainLabel,_obstacleDensity);//set input to % of tiles having obstacles, 1-20
    
        _obstaclePlacer.MakeObstacles(_terrainLabel,obsDensity,_borderSizeX,_borderSizeY);//set input to % of tiles having obstacles, 1-20
        
        Debug.Log("Making Enemies"); 
        // _enemyPlacer.MakeEnemies(_spawnRate,_borderSizeX,_borderSizeY);//set input to spawn rate (avg # of enemies to spawn in a 20x20 grid)
        _enemyPlacer.MakeEnemies(enemyRate,_borderSizeX,_borderSizeY);//set input to spawn rate (avg # of enemies to spawn in a 20x20 grid)

    }

    private void pickRandomTerrain(){
        int terrainnum  = UnityEngine.Random.Range(1, 9); 
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
            _opt1 = _jungle1;
            _opt2 = _jungle2;
            _opt3 = _jungle3;
            _opt4 = _jungle4;
            _terrainLabel = "jungle";
            return;
        case 6:
            _opt1 = _island1;
            _opt2 = _island2;
            _opt3 = _island3;
            _opt4 = _island4;
            _terrainLabel = "island";
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
        int tilenum  = UnityEngine.Random.Range(1, 5); 
        return tilenum;
    }

    private void setTileColor(int x, int y,Tile terraintile){
        Vector3Int position = new Vector3Int(x,y,0);
        _tilemap.SetTile(position,terraintile);
        return;
    }
}


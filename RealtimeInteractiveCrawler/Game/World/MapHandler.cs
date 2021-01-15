using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtimeInteractiveCrawler
{


    public class MapHandler
    {
        public TileType[,] Map;

        public int MapWidth { get; set; }
        public int MapHeight { get; set; }
        public int PercentAreWalls { get; set; }

        public MapHandler(int mapWidth, int mapHeight, int percentAreWalls, int seed)
        {
            MapWidth = mapWidth;
            MapHeight = mapHeight;
            PercentAreWalls = percentAreWalls;

            AwesomeGame.Rand = new Random(seed);

            RandomFillMap();
        }

        public MapHandler(int mapWidth, int mapHeight, TileType[,] map, int percentWalls = 40)
        {
            this.MapWidth = mapWidth;
            this.MapHeight = mapHeight;
            this.PercentAreWalls = percentWalls;
            this.Map = new TileType[this.MapWidth, this.MapHeight];
            this.Map = map;
        }

        public void MakeCaverns()
        {
            // By initilizing column in the outter loop, its only created ONCE
            for (int column = 0, row = 0; row <= MapHeight - 1; row++)
            {
                for (column = 0; column <= MapWidth - 1; column++)
                {
                    Map[column, row] = PlaceWallLogic(column, row);
                }
            }
        }

        public TileType PlaceWallLogic(int x, int y)
        {
            int numWalls = GetAdjacentWalls(x, y, 1, 1);


            if (Map[x, y] == TileType.GRASS)
            {
                if (numWalls >= 4)
                {
                    return TileType.GRASS;
                }
                if (numWalls < 2)
                {
                    return TileType.GROUND;
                }

            }
            else
            {
                if (numWalls >= 5)
                {
                    return TileType.GRASS;
                }
            }
            return TileType.GROUND;
        }

        public int GetAdjacentWalls(int x, int y, int scopeX, int scopeY)
        {
            int startX = x - scopeX;
            int startY = y - scopeY;
            int endX = x + scopeX;
            int endY = y + scopeY;

            int iX = startX;
            int iY = startY;

            int wallCounter = 0;

            for (iY = startY; iY <= endY; iY++)
            {
                for (iX = startX; iX <= endX; iX++)
                {
                    if (!(iX == x && iY == y))
                    {
                        if (IsWall(iX, iY))
                        {
                            wallCounter += 1;
                        }
                    }
                }
            }
            return wallCounter;
        }

        bool IsWall(int x, int y)
        {
            // Consider out-of-bound a wall
            if (IsOutOfBounds(x, y))
            {
                return true;
            }

            if (Map[x, y] == TileType.GRASS)
            {
                return true;
            }

            if (Map[x, y] == TileType.GROUND)
            {
                return false;
            }
            return false;
        }

        bool IsOutOfBounds(int x, int y)
        {
            if (x < 0 || y < 0)
            {
                return true;
            }
            else if (x > MapWidth - 1 || y > MapHeight - 1)
            {
                return true;
            }
            return false;
        }

        public void PrintMap()
        {
            Console.Clear();
            Console.Write(MapToString());
        }

        string MapToString()
        {
            string returnString = string.Join(" ", // Seperator between each element
                                              "Width:",
                                              MapWidth.ToString(),
                                              "\tHeight:",
                                              MapHeight.ToString(),
                                              "\t% Walls:",
                                              PercentAreWalls.ToString(),
                                              Environment.NewLine
                                             );

            List<string> mapSymbols = new List<string>();
            mapSymbols.Add(".");
            mapSymbols.Add("#");
            mapSymbols.Add("+");

            for (int column = 0, row = 0; row < MapHeight; row++)
            {
                for (column = 0; column < MapWidth; column++)
                {
                    returnString += mapSymbols[(int)Map[column, row]];
                }
                returnString += Environment.NewLine;
            }
            return returnString;
        }

        public void BlankMap()
        {
            for (int column = 0, row = 0; row < MapHeight; row++)
            {
                for (column = 0; column < MapWidth; column++)
                {
                    Map[column, row] = 0;
                }
            }
        }

        public void RandomFillMap()
        {
            // New, empty map
            Map = new TileType[MapWidth, MapHeight];

            int mapMiddle = 0; // Temp variable
            for (int column = 0, row = 0; row < MapHeight; row++)
            {
                for (column = 0; column < MapWidth; column++)
                {
                    // If coordinants lie on the the edge of the map (creates a border)
                    if (column == 0)
                    {
                        Map[column, row] = TileType.GRASS;
                    }
                    else if (row == 0)
                    {
                        Map[column, row] = TileType.GRASS;
                    }
                    else if (column == MapWidth - 1)
                    {
                        Map[column, row] = TileType.GRASS;
                    }
                    else if (row == MapHeight - 1)
                    {
                        Map[column, row] = TileType.GRASS;
                    }
                    // Else, fill with a wall a random percent of the time
                    else
                    {
                        mapMiddle = (MapHeight / 2);

                        if (row == mapMiddle)
                        {
                            Map[column, row] = TileType.GROUND;
                        }
                        else
                        {
                            Map[column, row] = RandomPercent(PercentAreWalls);
                        }
                    }
                }
            }
        }

        TileType RandomPercent(int percent)
        {
            if (percent >= AwesomeGame.Rand.Next(1, 101))
            {
                return TileType.GRASS;
            }
            return TileType.GROUND;
        }

        public void DoSimulationStep()
        {
            TileType[,] oldMap = Map;
            TileType[,] newMap = new TileType[MapWidth, MapHeight];
            int deathLimit = 3;
            int birthLimit = 3;
            //Loop over each row and column of the map
            for (int x = 0; x < oldMap.GetLength(0); x++)
            {
                for (int y = 0; y < oldMap.GetLength(1); y++)
                {
                    int nbs = CountAliveNeighbours(x, y);

                    //The new value is based on our simulation rules
                    //First, if a cell is alive but has too few neighbours, kill it.
                    if (oldMap[x, y] == TileType.GRASS)
                    {
                        if (nbs < deathLimit)
                        {
                            newMap[x, y] = TileType.GROUND;
                        }
                        else
                        {
                            newMap[x, y] = TileType.GRASS;
                        }
                    } //Otherwise, if the cell is dead now, check if it has the right number of neighbours to be 'born'
                    else
                    {
                        if (nbs > birthLimit)
                        {
                            newMap[x, y] = TileType.GRASS;
                        }
                        else
                        {
                            newMap[x, y] = TileType.GROUND;
                        }
                    }
                }
            }

            Map = newMap;
        }

        //Returns the number of cells in a ring around (x,y) that are alive.
        public int CountAliveNeighbours(int x, int y)
        {
            int count = 0;
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    int neighbour_x = x + i;
                    int neighbour_y = y + j;
                    //If we're looking at the middle point
                    if (i == 0 && j == 0)
                    {
                        //Do nothing, we don't want to add ourselves in!
                    }
                    //In case the index we're looking at it off the edge of the map
                    else if (neighbour_x < 0 || neighbour_y < 0 || neighbour_x >= Map.GetLength(0) || neighbour_y >= Map.GetLength(1))
                    {
                        count = count + 1;
                    }
                    //Otherwise, a normal check of the neighbour
                    else if (Map[neighbour_x, neighbour_y] == TileType.GRASS)
                    {
                        count = count + 1;
                    }
                }
            }

            return count;
        }


        public List<Vector2i> PlaceTreasure()
        {
            List<Vector2i> allSlimes = new List<Vector2i>();
            //How hidden does a spot need to be for treasure?
            //I find 5 or 6 is good. 6 for very rare treasure.
            int treasureHiddenLimit = 5;
            for (int x = 0; x < MapWidth; x++)
            {
                for (int y = 0; y < MapHeight; y++)
                {
                    if (Map[x,y] != TileType.GRASS && Map[x, y] != TileType.SLIME && Map[x, y] != TileType.ENEMY)
                    {
                        int nbs = CountAliveNeighbours(x, y);
                        if (nbs >= treasureHiddenLimit)
                        {
                            allSlimes.Add(new Vector2i(x, y));
                        }
                    }
                }
            }

            return allSlimes;
        }

        public List<Vector2i> PlaceEnemies()
        {
            List<Vector2i> enemies = new List<Vector2i>();
            //How hidden does a spot need to be for treasure?
            //I find 5 or 6 is good. 6 for very rare treasure.
            int enemyHiddenLimit = 4;
            for (int x = 0; x < MapWidth; x++)
            {
                for (int y = 0; y < MapHeight; y++)
                {
                    if (Map[x, y] != TileType.GRASS && Map[x, y] != TileType.SLIME && Map[x, y] != TileType.ENEMY)
                    {
                        int nbs = CountAliveNeighbours(x, y);
                        if (nbs >= enemyHiddenLimit)
                        {
                            enemies.Add(new Vector2i(x, y));
                        }
                    }
                }
            }

            return enemies;
        }

    }
}

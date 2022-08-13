using Godot;
using System;
using System.Collections.Generic;
public class Generator : TileMap
{
    enum tileType : int { empty = -1, floor = 0, wall = 1};
    [Export]
    int width = 16, height = 16;
    [Export]
    int iterations = 1000;
    class walker
    {
        public Vector2 dir;
        public Vector2 pos;

        public void RandomDirection()
        {
            Random r = new Random();
            int choice = r.Next(0, 4);
            switch (choice)
            {
                case 0:
                    dir = Vector2.Down;
                    break;
                case 1:
                    dir = Vector2.Left;
                    break;
                case 2:
                    dir =  Vector2.Up;
                    break;
                case 3:
                    dir = Vector2.Right;
                    break;
            }
        }
    }
    List<walker> walkers = new List<walker>();
    [Export]
    int maxWalkers = 10;
    [Export(PropertyHint.Range, "0,1,0.05")]
    float walkerDirChange = 0.5f, walkerSpawn = 0.05f;
    [Export(PropertyHint.Range, "0,1,0.05")]
    float walkerDestroy = 0.05f;
    [Export(PropertyHint.Range, "0,1,0.05")]
    float percentFill = 0.2f;
    [Export]
    bool generateWalls = true;

    public override void _Ready()
    {
        Initialize();
        GenerateFloor();
        if(generateWalls) GenerateWalls();
    }

    void Initialize()
    {
        walker newWalker = new walker();
        newWalker.RandomDirection();
        newWalker.pos = new Vector2(width / 2, height / 2);
        walkers.Add(newWalker);
    }

    void GenerateFloor()
    {
        int iterationsCount = 0;
        do
        {
            foreach(walker w in walkers)
            {
                SetCellv(w.pos, (int)tileType.floor);
            }

            Random r = new Random();
            for (int i = 0; i < walkers.Count; i++)
            {
                if (r.NextDouble() < walkerDestroy && walkers.Count > 1)
                {
                    walkers.RemoveAt(i);
                    break;
                }
            }
            for (int i = 0; i < walkers.Count; i++)
            {
                if (r.NextDouble() < walkerDirChange)
                {
                    walkers[i].RandomDirection();
                }
            }
            for (int i = 0; i < walkers.Count; i++)
            {
                if (r.NextDouble() < walkerSpawn && walkers.Count < maxWalkers)
                {
                    walker newWalker = new walker();
                    newWalker.RandomDirection();
                    newWalker.pos = walkers[i].pos;
                    walkers.Add(newWalker);
                }
            }
            // Walker movement
            for (int i = 0; i < walkers.Count; i++)
            {
                walker thisWalker = walkers[i];
                thisWalker.pos += thisWalker.dir;
                walkers[i] = thisWalker;
            }
            for (int i = 0; i < walkers.Count; i++)
            {
                walkers[i].pos.x = Mathf.Clamp(walkers[i].pos.x, 1, width - 1);
                walkers[i].pos.y = Mathf.Clamp(walkers[i].pos.y, 1, height - 1);
            }

            if (GetUsedCellsById((int)tileType.floor).Count / (float)(width*height) > percentFill)
            {
                break;
            }
            iterationsCount++;
        } while (iterationsCount < iterations);
    }

    void GenerateWalls()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (GetCell(x,y) == (int)tileType.floor)
                {
                    SetCell(x, y + 1, GetCell(x, y + 1)==(int)tileType.empty ? (int)tileType.wall : GetCell(x, y + 1));
                    SetCell(x, y - 1, GetCell(x, y - 1)==(int)tileType.empty ? (int)tileType.wall : GetCell(x, y - 1));
                    SetCell(x + 1, y, GetCell(x + 1, y)==(int)tileType.empty ? (int)tileType.wall : GetCell(x + 1, y));
                    SetCell(x - 1, y, GetCell(x - 1, y)==(int)tileType.empty ? (int)tileType.wall : GetCell(x - 1, y));
                }
            }
        }
    }
}

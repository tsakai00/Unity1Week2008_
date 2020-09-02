using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// オブジェクトをセル分割
/// </summary>
public class ObjectMap<T> where T : Component
{
    public CacheList<T>[,] map { get; private set; }
    public int numX { get; private set; }
    public int numY { get; private set; }

    public ObjectMap(int x, int y)
    {
        numX = Mathf.Max(1, x);
        numY = Mathf.Max(1, y);
        map = new CacheList<T>[numY, numX];
        for(int yy = 0; yy < numY; yy++)
        {
            for(int xx = 0; xx < numX; xx++)
            {
                var list = new CacheList<T>(4);
                map[yy, xx] = list;
            }
        }
    }

    public void Add(T obj, float x, float y)
    {
        // とりあえずクランプ
        int xx = Mathf.Clamp(Mathf.FloorToInt(x), 0, numX - 1);
        int yy = Mathf.Clamp(Mathf.FloorToInt(y), 0, numY - 1);

        map[yy, xx].Add(obj);
    }

    public void ClearAll()
    {
        for(int yy = 0; yy < numY; yy++)
        {
            for(int xx = 0; xx < numX; xx++)
            {
                map[yy, xx].Clear();
            }
        }
    }
}

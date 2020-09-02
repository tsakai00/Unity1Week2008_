using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// きっとリストそのままだと重くなると妄想した
/// </summary>
public class CacheList<T> where T : class
{
    public int      count   { get; private set; }
    public List<T>  list    { get; private set; }

    public CacheList(int capacity)
    {
        count   = 0;
        list    = new List<T>(capacity);
    }

    public void Add(T item)
    {
        if(list.Count <= count) { list.Add(default); }
        list[count++] = item;
    }

    public void Remove(T item)
    {
        if(count <= 0) { return; }
        for(int i = 0; i < count; i++)
        {
            if(list[i] == item)
            {
                list[i] = list[--count];
                break;
            }
        }
    }

    public T GetAndRemoveLast()
    {
        if(count <= 0) { return default; }
        return list[--count];
    }

    public void Clear()
    {
        count = 0;
    }
}

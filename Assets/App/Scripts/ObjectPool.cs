using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPool<T> where T : Component
{
    private Transform       _parent;
    private T               _prefab;
    private Stack<T>        _stack;
    private List<T>         _activeList;

    public List<T> activeList { get { return _activeList; } }

    public ObjectPool(Transform parent, T prefab, int capacity)
    {
        _parent = parent;
        _prefab = prefab;
        _stack = new Stack<T>(capacity);
        _activeList = new List<T>(capacity);
        for(int i = 0; i < capacity; i++)
        {
            var obj = Object.Instantiate(prefab, parent);
            obj.gameObject.SetActive(false);
            _stack.Push(obj);
        }
    }

    public void Add(T item)
    {
        _activeList.Add(item);
    }

    public T Rent()
    {
        T ret = null;
        if(_stack.Count <= 0)
        {
            ret = Object.Instantiate(_prefab, _parent);
        }
        else
        {
            ret = _stack.Pop();
        }

        ret.gameObject.SetActive(true);
        _activeList.Add(ret);
        return ret;
    }

    public void Return(T obj)
    {
        obj.gameObject.SetActive(false);
        _stack.Push(obj);
        _activeList.Remove(obj);
    }
}

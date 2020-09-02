using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    [SerializeField] private Transform _wallL;
    [SerializeField] private Transform _wallR;
    [SerializeField] private Transform _wallT;
    [SerializeField] private Transform _wallB;

    void Awake()
    {
        float size = WallConfig.WALL_MAX - WallConfig.WALL_MIN;

        _wallL.localPosition = new Vector3(-WallConfig.W_HALF, 0.0f, 0.0f);
        _wallL.localScale    = new Vector3(size, 0.1f, 0.1f);
        _wallR.localPosition = new Vector3(WallConfig.W_HALF, 0.0f, 0.0f);
        _wallR.localScale    = new Vector3(size, 0.1f, 0.1f);
        _wallT.localPosition = new Vector3(0.0f, WallConfig.W_HALF, 0.0f);
        _wallT.localScale    = new Vector3(size, 0.1f, 0.1f);
        _wallB.localPosition = new Vector3(0.0f, -WallConfig.W_HALF, 0.0f);
        _wallB.localScale    = new Vector3(size, 0.1f, 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

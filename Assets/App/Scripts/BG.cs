using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BG : MonoBehaviour
{
    [SerializeField] Camera _cam;
    [SerializeField] List<Transform> _bgList;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector2 v = _cam.transform.position;
        v -= new Vector2(100.0f, 100.0f);
        float a = 1.0f;
        float z = 0.0f;
        foreach(var bg in _bgList)
        {
            bg.transform.localPosition =  new Vector3(-v.x * a, -v.y * a, z);
            a *= 0.5f;
            z += 1.0f;
        }
    }
}

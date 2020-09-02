using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private BoidUnit _player;

    private Vector2 _target;
    private Vector2 _shakeVel = Vector2.zero;
    private Vector2 _shakeOfs = Vector2.zero;

    public void SetTarget(BoidUnit t)
    {
        _player = t;
    }

    // Start is called before the first frame update
    void Start()
    {
        _target = _player.pos;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateShake();

        Vector2 pos = transform.position;
        pos += (_player.pos - pos) * 0.1f;

        float a = Screen.width / (float)Screen.height;

        var min = WallConfig.WALL_MIN;
        var max = WallConfig.WALL_MAX;
        pos.x = Mathf.Clamp(pos.x, min, max);
        pos.y = Mathf.Clamp(pos.y, min, max);

        transform.position = new Vector3(pos.x + _shakeOfs.x, pos.y + _shakeOfs.y, transform.position.z);
    }

    private void UpdateShake()
    {
        _shakeVel *= 0.8f;
        _shakeVel += (Vector2.zero - _shakeOfs) * 0.8f;
        _shakeOfs += _shakeVel;
    }

    /// <summary>
    /// ダメージ受けたときのシェイク
    /// </summary>
    public void Shake(Vector2 dir, float power = 0.5f)
    {
        _shakeVel = -dir.normalized * power;
    }
}

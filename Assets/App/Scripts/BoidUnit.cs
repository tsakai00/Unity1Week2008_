using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// たぶん増えるやつ
/// </summary>
public class BoidUnit : MonoBehaviour
{
    public enum Type
    {
        Player,
        PlayerBullet,
        Enemy,
        PlayerBulletSleep
    }

    public Type    type { get; set; } = Type.PlayerBullet;
    public Vector2 pos  { get; set; } = Vector2.zero;
    public Vector2 vel  { get; set; } = Vector2.zero;
    public Vector2 dir  { get; set; } = Vector2.zero;
    public float   dec  { get; set; } = 0.985f;
    public float   invMass    { get; set; } = 1.0f;

    [SerializeField] protected float            _radius = 1;
    [SerializeField] protected SpriteRenderer _view;
    [SerializeField] protected TrailRenderer  _trail;

    public float radius { get { return _radius; } }

    protected Vector2 _dir = Vector2.zero;

    /// <summary>
    /// 初期化
    /// </summary>
    public virtual void Init(Vector2 tgt)
    {

    }

    /// <summary>
    /// プレイヤーを狙う
    /// </summary>
    public virtual void ChaseTarget(Vector2 tgt)
    {
    }

    /// <summary>
    /// 他のボイドに当たった
    /// </summary>
    public virtual void Hit(BoidUnit boid, BoidManager boidManager)
    {
    }

    /// <summary>
    /// 座標更新
    /// </summary>
    public virtual void UpdatePosition(BoidManager boidManager)
    {
        vel *= dec;
        pos += vel;
        _dir += (dir - _dir) * 0.1f;

        float s = Mathf.Clamp(vel.sqrMagnitude * 5 + 1.0f, 1.0f, 2.0f);
        float r = Mathf.Atan2(-dir.x, dir.y) * Mathf.Rad2Deg;

        transform.position = pos;
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, r);
        transform.localScale = new Vector3(1.0f / s, s, 1.0f);
    }
}

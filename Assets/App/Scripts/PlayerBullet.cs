using Lib.Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : BoidUnit
{
    [SerializeField] private Gradient _gradient;


    private Color _color;
    private float _deadSec;

    public override void Init(Vector2 p)
    {
        float r = Random.Range(0.0f, 2 * Mathf.PI);
        float d = Random.Range(0.1f, 0.2f);
        Vector2 v = new Vector2(Mathf.Cos(r) * d, Mathf.Sin(r) * d);
        
        _color = _gradient.Evaluate(Random.Range(0.0f, 1.0f));
        SetColor(_color);

        type = BoidUnit.Type.PlayerBullet;
        pos = p;
        vel = v;
        dec = 0.985f;
        _dir = dir = v.normalized;
        _deadSec = 3.0f;
        for(int i = 0; i < _trail.positionCount; i++)
        {
            _trail.SetPosition(i, new Vector3(pos.x, pos.y, _trail.transform.position.z));
        }

        transform.position = pos;
    }

    public void AddStartOffset()
    {
        pos += vel.normalized;
    }

    private void SetColor(Color color)
    {
        _view.color = color;
        _trail.startColor = color;
        color.a = 0.0f;
        _trail.endColor = color;
    }

    public override void ChaseTarget(Vector2 tgt)
    {
        if(type == BoidUnit.Type.PlayerBulletSleep) { return; }
        
        Vector2 v = (tgt - pos) * 0.1f;
        float d = v.magnitude;
        if(d < 0.0001f) { return; }

        v = v / d * Mathf.Min(d, 0.01f);
        vel += v;
        dir = v;
    }

    public override void Hit(BoidUnit boid, BoidManager boidManager)
    {
        if(type == BoidUnit.Type.PlayerBullet)
        {
            if(boid.type == BoidUnit.Type.Enemy)
            {
                var enemy = boid as Enemy;
                if(enemy.hp >= 0)
                {
                    Dead(pos - boid.pos);
                }
            }
        }
    }

    private void Dead(Vector2 dir)
    {
        SetColor(Color.white * 0.8f);
        type = BoidUnit.Type.PlayerBulletSleep;
        dec = 0.95f;
        _deadSec = 3.0f;

        float r = Random.Range(0.0f, 2 * Mathf.PI);
        float d = Random.Range(0.8f, 1.2f);
        //Vector2 v = new Vector2(Mathf.Cos(r) * d, Mathf.Sin(r) * d);
        Vector2 v = dir.normalized * d;
        vel += v;
    }

    public override void UpdatePosition(BoidManager boidManager)
    {
        if(type == BoidUnit.Type.PlayerBulletSleep)
        {
            _deadSec -= Time.deltaTime;
            if(_deadSec <= 0.0f)
            {
                boidManager.DeletePlayerBullet(this);
                return;
            }
        }

        base.UpdatePosition(boidManager);
    }
}

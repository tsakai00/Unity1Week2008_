using Lib.Sound;
using Lib.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : BoidUnit
{
    const int ADD_CNT = 10;

    [SerializeField] private int _hp        = 1;
    [SerializeField] private int _addNum    = 2;
    [SerializeField] private bool _isBoss   = false;

    public int hp { get { return _hp; } }
    public int addNum { get { return _addNum; } }

    private float _sclSec;
    private float _dmgSclVel = 0.0f;
    private float _dmgScl = 0.0f;
    private int   _addCnt = ADD_CNT;

    private SimpleTimer _timer = new SimpleTimer();

    public override void Init(Vector2 pos)
    {
        invMass = 0.0001f;
        _sclSec = Random.Range(0, 360.0f);

        type = BoidUnit.Type.Enemy;
        this.pos = pos;
        this.vel = Vector2.zero;
        transform.position = pos;
    }

	public override void Hit(BoidUnit boid, BoidManager boidManager)
	{
		if(boid.type == Type.PlayerBullet || boid.type == Type.Player)
        {
            _hp--;
            _dmgSclVel = 0.25f * (1.0f / radius);
            if(_hp > 0)
            {
                var rnd = Random.Range(10, 30) * 0.1f;
                SoundManager.Instance.PlaySE(SoundPath_SE._HIT_HURT45, rnd);
            }
            else
            {
                var renderer = gameObject.GetComponentsInChildren<Renderer>();
                foreach(var r in renderer)
                {
                    r.enabled = false;
                }
            }
        }
	}

	public override void UpdatePosition(BoidManager boidManager)
    {
        if(_hp <= 0)
        {
            _radius = 0.0f;
            if(_isBoss)
            {
                if(_timer.Update())
                {
                    float r = Random.Range(0.0f, 2 * Mathf.PI);
                    float d = Random.Range(0.0f, radius);
                    Vector2 v = new Vector2(Mathf.Cos(r) * d, Mathf.Sin(r) * d) + pos;

                    int nnn = _addNum / ADD_CNT;
                    for(int i = 0; i < nnn; i++)
                    {
                        boidManager.CreatePlayerBullet(v);
                    }

                    _addCnt--;
                    if(_addCnt <= 0)
                    {
                        boidManager.DeleteEnemy(this);
                    }

                    _timer.Init(0.1f);
                    SoundManager.Instance.PlaySE(SoundPath_SE._EXPLOSION8, 2.0f);
                }
            }
            else
            {
                for(int i = 0; i < _addNum; i++)
                {
                    boidManager.CreatePlayerBullet(pos);
                    boidManager.DeleteEnemy(this);
                }
                SoundManager.Instance.PlaySE(SoundPath_SE._EXPLOSION8, 2.0f);
            }

            return;
        }

        base.UpdatePosition(boidManager);

        _dmgSclVel *= 0.8f;
        _dmgSclVel += (0.0f - _dmgScl) * 0.8f;
        _dmgScl += _dmgSclVel;

        _sclSec += Time.deltaTime * 90.0f;
        if(_sclSec > 360.0f) { _sclSec -= 360.0f; }
        transform.localScale = Vector3.one + (Vector3.one * Mathf.Sin(_sclSec * Mathf.Deg2Rad) * 0.05f) + (Vector3.one * _dmgScl);
    }

    public void Exec(BoidManager boidMgr, Player player)
    {
        // _sec -= Time.deltaTime;
        // if(_sec <= 0.0f)
        // {
        //     var bullet = boidMgr.CreateEnemyBullet(transform.position);

        //     Vector2 p = transform.position;
        //     Vector2 v = player.pos - p;
        //     v.Normalize();
        //     bullet.pos = p;
        //     bullet.vel = v * 0.1f;
        //     bullet.dir = v;

        //     _sec += 5.0f;
        // }
    }
}

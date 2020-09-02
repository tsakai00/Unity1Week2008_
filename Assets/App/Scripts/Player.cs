using Lib.Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// とりあえずプレイヤー
/// </summary>
public class Player : BoidUnit
{
    private const float SPEED = 0.05f;
    private const float MUTEKI_SEC = 0.5f;

    private int _hp = 3;
    public int hp { get { return _hp; } }

    private float _mutekiSec = 0.0f;

    void Awake()
    {
        this._hp = 3;
        this.invMass = 0.05f;
        this.dec = 0.8f;
        this.type = BoidUnit.Type.Player;
    }

    public override void Hit(BoidUnit boid, BoidManager boidManager)
    {
        if(_mutekiSec > 0) { return; }
        if(boid.type == BoidUnit.Type.Enemy)
        {
            SoundManager.Instance.PlaySE(SoundPath_SE._LASER_SHOOT23);
            _hp--;
            boidManager.playerCamera.Shake(boid.pos - pos);
            if(hp > 0)
            {
                _mutekiSec = MUTEKI_SEC;
            }
            else
            {
                Dead(pos - boid.pos);
            }
        }
    }

    private void Dead(Vector2 dir)
    {
        SetColor(new Color(0.5f, 0.0f, 0.0f));
        type = BoidUnit.Type.PlayerBulletSleep;
        dec = 0.95f;

        float r = Random.Range(0.0f, 2 * Mathf.PI);
        float d = Random.Range(0.8f, 1.2f);
        //Vector2 v = new Vector2(Mathf.Cos(r) * d, Mathf.Sin(r) * d);
        Vector2 v = dir.normalized * d;
        vel += v;
    }

    private void SetColor(Color color)
    {
        _view.color = color;
        _trail.startColor = color;
        color.a = 0.0f;
        _trail.endColor = color;
    }

    public override void UpdatePosition(BoidManager boidManager)
    {
        base.UpdatePosition(boidManager);


        _mutekiSec -= Time.deltaTime;
        if(_mutekiSec <= 0) { _mutekiSec = 0.0f; }

        if(_mutekiSec > 0)
        {
            _view.enabled = !_view.enabled;
            _trail.enabled = !_trail.enabled;
        }
        else
        {
            _view.enabled = true;
            _trail.enabled = true;
        }

        // 壁
        {
            const float min = WallConfig.WALL_MIN;
            const float max = WallConfig.WALL_MAX;

            var pos = this.pos;
            var vel = this.vel;

            if(pos.x < min)
            {
                pos.x = min;
                vel.x = 0.0f;
            }
            else if(pos.x > max)
            {
                pos.x = max;
                vel.x = 0.0f;
            }
            if(pos.y < min)
            {
                pos.y = min;
                vel.y = 0.0f;
            }
            else if(pos.y >  max)
            {
                pos.y = max;
                vel.y = 0.0f;
            }

            this.pos = pos;
            this.vel = vel;
        }
    }

    public void UpdateInput(Camera cam, BoidManager boidMgr)
    {
        if(hp <= 0) { return; }

        Vector2 v = Vector2.zero;
        if(Input.GetMouseButton(0))
        {
            Vector2 vv = Util.GetMousePosition(cam);
            v = vv - pos;
        }
        else
        {
            if(Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                v.x = -1;
            }
            else if(Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                v.x =  1;
            }
            if(Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            {
                v.y =  1;
            }
            else if(Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            {
                v.y = -1;
            }
        }

        /*
        if(Input.GetKeyDown(KeyCode.Space))
        {
            // 増やすテスト！！！
            int n = Mathf.Max(boidMgr.plBulletPool.activeList.Count, 1);
            for(int i = 0; i < n; i++)
            {
                boidMgr.CreatePlayerBullet(pos);
            }
        }*/

        v.Normalize();
        v *= SPEED;

        this.vel += v;
        this.dir = vel;
        this.dir.Normalize();
    }
}

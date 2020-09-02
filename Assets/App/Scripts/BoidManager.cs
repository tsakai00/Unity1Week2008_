using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ボイド管理
/// </summary>
public class BoidManager : MonoBehaviour
{
    [SerializeField] private PlayerBullet   _plBulletPrefab;
    [SerializeField] private Enemy          _enemyPrefab;
    [SerializeField] private Effect         _effectPrefab;

    private ObjectMap<BoidUnit>         _objMap;
    private ObjectPool<PlayerBullet>    _plBulletPool;
    // private ObjectPool<Enemy>           _enemyPool;
    private ObjectPool<Effect>          _effectPool;
    private List<BoidUnit>              _activeList;
    private List<BoidUnit>              _inactiveList;
    private List<Enemy>                 _enemyList;

    private PlayerCamera    _cam;   // 無理やり置いておく

    public ObjectPool<PlayerBullet> plBulletPool { get { return _plBulletPool; } }
    // public ObjectPool<Enemy>        enemyPool    { get { return _enemyPool; } }
    public ObjectMap<BoidUnit>      objMap       { get { return _objMap; } }
    public List<Enemy>  enemyList { get { return _enemyList; } }

    public PlayerCamera playerCamera { get { return _cam; } }


    /// <summary>
    /// 初期化
    /// </summary>
    public void Init(float w, float h, Player player, PlayerCamera cam)
    {
        _objMap  = new ObjectMap<BoidUnit>(Mathf.CeilToInt(w), Mathf.CeilToInt(h));

        _plBulletPool = new ObjectPool<PlayerBullet>(transform, _plBulletPrefab, 1024);
        // _enemyPool    = new ObjectPool<Enemy>(transform, _enemyPrefab, 64);
        _effectPool   = new ObjectPool<Effect>(transform, _effectPrefab, 8);

        _activeList   = new List<BoidUnit>(2048);
        _inactiveList = new List<BoidUnit>(1024);
        _enemyList    = new List<Enemy>(128);

        _activeList.Add(player);  // プレイヤーをボイド計算に組み込む
        _cam = cam; // ボイド処理と全く関係ないがここに置かせてもらう
    }

    /// <summary>
    /// ボイド更新
    /// </summary>
    public void Calc(Player player)
    {
        if(_inactiveList.Count > 0)
        {
            // Deleteされたオブジェクトはアクティブリストから破棄
            int nn = _inactiveList.Count;
            for(int i = 0; i < nn; i++)
            {
                var item = _inactiveList[i];
                _activeList.Remove(item);
                _enemyList.Remove(item as Enemy);
                _inactiveList[i].gameObject.SetActive(false);
            }
            _inactiveList.Clear();
        }

        int num  = _activeList.Count;
        var list = _activeList;

        // マップに追加
        _objMap.ClearAll();
        for(int i = 0; i < num; i++)
        {
            var obj = list[i];
            _objMap.Add(obj, Mathf.FloorToInt(obj.pos.x), Mathf.FloorToInt(obj.pos.y));
        }

        // 中心に向かって移動
        var center = player.pos;
        for(int i = 0; i < num; i++)
        {
            list[i].ChaseTarget(center);
        }

        // 当たり判定
        // 先に敵から行う。ダメージ判定を正確に行うのと、半径が大きいのでセルサイズから漏れるため
        {
            int nn = _enemyList.Count;
            for(int i = 0; i < nn; i++)
            {
                var boid = _enemyList[i];
                int xx = Mathf.FloorToInt(boid.pos.x);
                int yy = Mathf.FloorToInt(boid.pos.y);
                CollisionBoid(boid, xx, yy);
                _objMap.map[yy, xx].Remove(boid);
            }
        }
        int nx = _objMap.numX;
        int ny = _objMap.numY;
        // 近すぎたら引き離す
        for(int i = 0; i < ny; i++)
        {
            for(int j = 0; j < nx; j++)
            {
                while(true)
                {
                    var boid = _objMap.map[i, j].GetAndRemoveLast();
                    if(boid == null) { break; }
                    CollisionBoid(boid, j, i);
                }
            }
        }

        // transform更新
        for(int i = 0; i < num; i++)
        {
            list[i].UpdatePosition(this);
        }
    }

    /// <summary>
    /// プレイヤー弾生成
    /// </summary>
    /// <param name="pos"></param>
    public PlayerBullet CreatePlayerBullet(Vector2 pos)
    {
        var obj = _plBulletPool.Rent();
        obj.Init(pos);
        _activeList.Add(obj);
        return obj;
    }

    /// <summary>
    /// プレイヤー弾消滅
    /// </summary>
    /// <param name="item"></param>
    public void DeletePlayerBullet(BoidUnit item)
    {
        if(item is PlayerBullet == false) { return; }
        _plBulletPool.Return(item as PlayerBullet);
        _inactiveList.Add(item);
    }

    /// <summary>
    /// 敵生成
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public Enemy AddEnemy(Enemy enemy, Vector2 pos)
    {
        var obj = enemy;
        obj.Init(pos);
        _activeList.Add(obj);
        _enemyList.Add(obj);
        return obj;
    }

    /// <summary>
    /// 敵消滅
    /// </summary>
    /// <param name="item"></param>
    public void DeleteEnemy(BoidUnit item)
    {
        if(item is Enemy == false) { return; }
        _inactiveList.Add(item);
    }

    /// <summary>
    /// ボイド同士の当たり判定
    /// </summary>
    private void CollisionBoid(BoidUnit a, int x, int y)
    {
        int ari = Mathf.CeilToInt(a.radius);

        // 半径分の近隣セルをチェック
        for(int yy = y - ari; yy <= y + ari; yy++)
        {
            if(yy < 0 || yy >= _objMap.numY) { continue; }
            for(int xx = x - ari; xx <= x + ari; xx++)
            {
                if(xx < 0 || xx >= _objMap.numX) { continue; }

                var list = _objMap.map[yy, xx];
                int num = list.count;
                for(int k = 0; k < num; k++)
                {
                    var b = list.list[k];
                    if(b == a) { continue; }

                    var v = (b.pos) - (a.pos);
                    var d = v.sqrMagnitude;
                    var r = a.radius + b.radius;
                    if(d < r * r && d > 0.0001f)
                    {
                        // 粘性
                        if(a.type == BoidUnit.Type.PlayerBullet && b.type == BoidUnit.Type.PlayerBullet)
                        {
                            var vv = (a.vel + b.vel) * 0.5f;
                            a.vel = (a.vel + vv) * 0.5f;
                            b.vel = (b.vel + vv) * 0.5f;
                        }

                        // 反発
                        //if(a.type != BoidUnit.Type.EnemyBullet || b.type != BoidUnit.Type.EnemyBullet)
                        {
                            d = Mathf.Sqrt(d);
                            v = v * (1.0f - d / r) / (d * (a.invMass + b.invMass)) * 0.4f;
                            a.vel -= v * a.invMass;
                            b.vel += v * b.invMass;
                        }

                        a.Hit(b, this);
                        b.Hit(a, this);
                    }
                }
            }
        }
    }

    private void CollisionSleepBullet(BoidUnit a, int x, int y)
    {
        const int rad = 2;
        const float sqrRad = rad * rad;   // 当たり判定用

        // とりあえず近隣9セルをチェック
        for(int yy = y - rad; yy <= y + rad; yy++)
        {
            if(yy < 0 || yy >= _objMap.numY) { continue; }
            for(int xx = x - rad; xx <= x + rad; xx++)
            {
                if(xx < 0 || xx >= _objMap.numX) { continue; }

                var list = _objMap.map[yy, xx];
                int num = list.count;
                for(int k = 0; k < num; k++)
                {
                    var b = list.list[k];
                    if(b.type != BoidUnit.Type.PlayerBulletSleep) { continue; }

                    var v = (b.pos) - (a.pos);
                    var d = v.sqrMagnitude;
                    if(d < sqrRad && d > 0.0001f)
                    {
                        b.Hit(a, this);
                    }
                }
            }
        }
    }

/*
    void OnDrawGizmos()
    {
        if(_objMap == null) { return; }
        for(int i = 0; i < _objMap.numY; i++)
        {
            for(int j = 0; j < _objMap.numX; j++)
            {
                Gizmos.DrawWireCube(new Vector3(j + 0.5f, i + 0.5f, 0.0f), Vector3.one);
            }
        }
    }
*/
}

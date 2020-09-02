using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public class EnemyParam
    {
        public int   num;
        public Enemy prefab;
    }

    [SerializeField] List<EnemyParam>    _enemyList;
    [SerializeField] Enemy               _boss;

    public Enemy boss { get; private set; }

            List<Enemy> list = new List<Enemy>();

    public bool CreateWave(BoidManager boidMgr, Vector2 playerPos)
    {
        Wave_001(boidMgr, playerPos);
        return true;
    }

    public void Exec(BoidManager boidMgr)
    {
    }



    private void Wave_001(BoidManager boidMgr, Vector2 playerPos)
    {
        // 敵を生成

        foreach(var enemy in _enemyList)
        {
            for(int i = 0; i < enemy.num; i++)
            {
                var obj = Instantiate<Enemy>(enemy.prefab, transform);
                var pos = new Vector2(Random.Range(WallConfig.WALL_MIN, WallConfig.WALL_MAX), Random.Range(WallConfig.WALL_MIN, WallConfig.WALL_MAX));
                boidMgr.AddEnemy(obj, pos);

                list.Add(obj);
            }
        }

        Vector2 bossPos = new Vector2(WallConfig.CENTER, WallConfig.CENTER + WallConfig.W_HALF * 0.25f);
        {
            var obj = Instantiate<Enemy>(_boss, transform);
            var pos = bossPos;
            boidMgr.AddEnemy(obj, pos);

            boss = obj;
            //list.Add(obj);
        }

        // 敵を壁の中に収めつつ、重ならないように配置する
        int num = list.Count;
        for(int kk = 0; kk < 8; kk++)
        {
            for(int i = 0; i < num; i++)
            {
                var a = list[i];

                float min = WallConfig.WALL_MIN + a.radius;
                float max = WallConfig.WALL_MAX - a.radius;

                var pos = a.pos;

                if(pos.x < min)
                {
                    pos.x = min;
                }
                else if(pos.x > max)
                {
                    pos.x = max;
                }
                if(pos.y < min)
                {
                    pos.y = min;
                }
                else if(pos.y >  max)
                {
                    pos.y = max;
                }

                {
                    var vv = pos - bossPos;
                    var rr = (10.0f + a.radius);
                    if(vv.sqrMagnitude < rr * rr)
                    {
                        pos = bossPos + vv.normalized * rr;
                    }
                }
                {
                    var vv = pos - playerPos;
                    var rr = (5.0f + a.radius);
                    if(vv.sqrMagnitude < rr * rr)
                    {
                        pos = playerPos + vv.normalized * rr;
                    }
                }

                a.pos = pos;


                for(int j = i + 1; j < num; j++)
                {
                    var b = list[j];
                    
                    var v = (b.pos) - (a.pos);
                    var d = v.sqrMagnitude;
                    var r = a.radius + b.radius;
                    if(d < r * r && d > 0.0001f)
                    {
                        {
                            d = Mathf.Sqrt(d);
                            v = (v / d) * (r - d) * 0.4f;
                            a.pos = a.pos - v * 0.5f;
                            b.pos = b.pos + v * 0.5f;
                        }
                    }
                }
            }
        }
    }

    void Update()
    {

    }
}

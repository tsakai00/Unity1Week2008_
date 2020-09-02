using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using Lib.Util;
using Lib.Sound;
using UnityEngine.SceneManagement;

/// <summary>
/// なんかエントリーポイント
/// </summary>
public class GameMainController : MonoBehaviour
{
    [SerializeField] private PlayerCamera _cam;
    [SerializeField] private BoidManager _boidManager;
    [SerializeField] private WaveManager _waveManager;
    [SerializeField] private Player      _player;
    [SerializeField] private TextMeshProUGUI _playerCountText;
    [SerializeField] private PlayerHPView    _playerHPView;
    [SerializeField] private TextMeshProUGUI _readyText;
    [SerializeField] private TextMeshProUGUI _gameOverText;
    [SerializeField] private TextMeshProUGUI _clearText;
    [SerializeField] private GameObject      _button; 

    private Camera _cam2;
    private StateMachine _state;
    private SimpleTimer  _timer = new SimpleTimer();
    private bool        _isInput = false;
    
    void Start()
    {
        _button.SetActive(false);
        Application.targetFrameRate = 60;

        _cam2 = _cam.GetComponent<Camera>();
        var startPos = new Vector2(WallConfig.CENTER, WallConfig.WALL_MIN + WallConfig.W_HALF * 0.25f);
        _cam.transform.position = new Vector3(startPos.x, startPos.y, _cam.transform.position.z);
        _player.pos = startPos;
        _boidManager.Init(200, 200, _player, _cam);

        for(int i = 0; i < 8; i++)
        {
            var obj = _boidManager.CreatePlayerBullet(_player.pos);
            obj.AddStartOffset();
        }
        
        _waveManager.CreateWave(_boidManager, _player.pos);

        _state = new StateMachine(State_Ready);
    }

    void Update()
    {
        _state.Update();
    }

    private void UpdateAll()
    {
         // 入力
        _player.UpdateInput(_cam2, _boidManager);

        // ボイド更新
        _boidManager.Calc(_player);

        // UI更新
        _playerCountText.text = _boidManager.plBulletPool.activeList.Count(x => x.type == BoidUnit.Type.PlayerBullet).ToString();
        _playerHPView.UpdateHP(_player.hp);
    }

    private void State_Ready(StateMachineCase c)
    {
        switch(c)
        {
            case StateMachineCase.Enter:
                {
                    _timer.Init(3.0f);
                    _readyText.gameObject.SetActive(true);
                    SoundManager.Instance.PlayBGM(SoundPath_BGM._CHESS);
                }
                break;
            case StateMachineCase.Exec:
                {
                    UpdateAll();
                    if(_timer.Update())
                    {
                        _state.ChangeState(State_Main);
                    }
                }
                break;
            case StateMachineCase.Exit:
                {
                    _readyText.gameObject.SetActive(false);
                }
                break;
        }
    }

    private void State_Main(StateMachineCase c)
    {
        switch(c)
        {
            case StateMachineCase.Enter:
                {
                }
                break;
            case StateMachineCase.Exec:
                {
                    UpdateAll();
                    if(_player.hp <= 0)
                    {
                        _state.ChangeState(State_GameOver);
                    }
                    else if(IsAllEnemyDead())
                    {
                        _state.ChangeState(State_ClearCam);
                    }
                }
                break;
            case StateMachineCase.Exit:
                break;
        }
    }

    private void State_GameOver(StateMachineCase c)
    {
        switch(c)
        {
            case StateMachineCase.Enter:
                {
                    _timer.Init(2.0f);
                    _gameOverText.gameObject.SetActive(true);
                    SoundManager.Instance.StopBGM(2.0f);
                }
                break;
            case StateMachineCase.Exec:
                {
                    if(_timer.Update())
                    {
                        _state.ChangeState(State_InputWait);
                    }
                    UpdateAll();
                }
                break;
            case StateMachineCase.Exit:
                break;
        }
    }

    private void State_ClearCam(StateMachineCase c)
    {
        switch(c)
        {
            case StateMachineCase.Enter:
                {
                    _timer.Init(3.0f);
                    _cam.SetTarget(_waveManager.boss);
                    SoundManager.Instance.StopBGM(2.0f);
                }
                break;
            case StateMachineCase.Exec:
                {
                    if(_timer.Update())
                    {
                        _state.ChangeState(State_Clear);
                    }
                    UpdateAll();
                }
                break;
            case StateMachineCase.Exit:
                {
                    _cam.SetTarget(_player);
                }
                break;
        }
    }

    private void State_Clear(StateMachineCase c)
    {
        switch(c)
        {
            case StateMachineCase.Enter:
                {
                    _timer.Init(2.0f);
                    _clearText.gameObject.SetActive(true);
                    SoundManager.Instance.PlayBGM(SoundPath_BGM._NEONPURPLE);
                }
                break;
            case StateMachineCase.Exec:
                {
                    if(_timer.Update())
                    {
                        _state.ChangeState(State_InputWait);
                    }
                    UpdateAll();
                }
                break;
            case StateMachineCase.Exit:
                break;
        }
    }

    private void State_InputWait(StateMachineCase c)
    {
        switch(c)
        {
            case StateMachineCase.Enter:
                {
                    _button.SetActive(true);
                    _isInput = false;
                }
                break;
            case StateMachineCase.Exec:
                {
                    if(Input.GetKeyDown(KeyCode.Space))
                    {
                        _isInput = true;
                    }
                    if(_isInput)
                    {
                        SceneManager.LoadScene("Title");
                    }

                    UpdateAll();
                }
                break;
            case StateMachineCase.Exit:
                break;
        }
    }

    public void OnClick()
    {
        _isInput = true;
    }

    private bool IsAllEnemyDead()
    {
        return _waveManager.boss.hp <= 0;

        // int num = _boidManager.enemyList.Count;
        // for(int i = 0; i < num; i++)
        // {
        //     var oo = _boidManager.enemyList[i];
        //     if(oo.hp > 0) { return false; }
        // }

        // return true;
    }
}

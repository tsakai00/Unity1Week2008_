using System.Collections;
using System.Collections.Generic;
using Lib.Sound;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleController : MonoBehaviour
{
    private bool _isStart = false;
    [SerializeField] private Slider _slider;

    private float _volMax;

    // Start is called before the first frame update
    void Start()
    {
        _volMax = SoundManager.Instance.GetVolumeMaster();
        // SoundManager.Instance.PlayBGM(SoundPath_BGM._MIST);
        SoundManager.Instance.StopBGM(2.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            _isStart = true;
        }

        if(_isStart)
        {
            SceneManager.LoadScene("GameMain");
        }
    }

    public void OnClick()
    {
        _isStart = true;
    }

    public void OnVolumeChange(float vol)
    {
        SoundManager.Instance.SetVolumeMaster(vol);
    }
}

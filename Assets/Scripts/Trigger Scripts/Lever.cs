using UnityEngine;
using System;

public class Lever : MonoBehaviour
{
    [SerializeField] private bool _IsActivated = false;
    bool _IsActive = false;

    [SerializeField] private AudioSource _clickSound;

    [SerializeField] private ScreenUnlockConfig _screenUnlockConfig;

    public Action<string> onResultingTag;

    public string ObjectTag
    {
        get;
        private set;
    }

    public bool LeverPosition
    {
        get;
        private set;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerController2d player))
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (!_IsActive)
                {
                    _IsActivated = true;
                    _IsActive = true;
                }
                else
                {
                    _IsActivated = false;
                    _IsActive = false;
                }
                SendInfoAboutPress(player.gameObject.tag);
                _screenUnlockConfig.CheckMatchInfo();
                _clickSound.Play();
                LeverPosition = _IsActivated;
            }

        }
    }

    private void SendInfoAboutPress(string tag)
    {
        ObjectTag = tag;
        onResultingTag?.Invoke(tag);
    }
}

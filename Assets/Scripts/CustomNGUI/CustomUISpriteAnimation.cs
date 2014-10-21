using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CustomUISpriteAnimation : UISpriteAnimation
{
    #region Variables (private)

    [SerializeField]
    [HideInInspector]
    protected bool _keepSize;
    [SerializeField]
    protected List<CustomUISpriteAnimationData> _spritesPlaylist;

    protected IntVector3 _initialSize;

    #endregion

    #region Propiedades (public)

    public List<CustomUISpriteAnimationData> SpritesPlaylist
    {
        get { return _spritesPlaylist; }
    }

    public bool KeepSize
    {
        get { return _keepSize; }
        set
        {
            if (value)
            {
                ResetInitialSize();
            }
            _keepSize = value;
        }
    }

    #endregion

    #region Funciones de evento de Unity

    /// <summary>
    /// Llamado al inicializar el componente, si MonoBehaviour esta habilitado.
    /// </summary>
    protected override void Start()
    {
        ResetInitialSize();
    }

    /// <summary>
    /// Update es llamado una vez por frame, si MonoBehaviour esta habilitado.
    /// </summary>
    protected override void Update()
    {
        if (mActive && _spritesPlaylist.Count > 1 && Application.isPlaying)
        {
            mDelta += RealTime.deltaTime;

            if (mDelta >= _spritesPlaylist[mIndex].ShowTime)
            {

                mDelta = mDelta - _spritesPlaylist[mIndex].ShowTime;

                if (++mIndex >= _spritesPlaylist.Count)
                {
                    mIndex = 0;
                    mActive = mLoop;
                }

                if (mActive)
                {
                    mSprite.spriteName = _spritesPlaylist[mIndex].SpriteName;
                    if (mSnap)
                    {
                        mSprite.MakePixelPerfect();
                    }
                    if (_keepSize)
                    {
                        mSprite.width = _initialSize.x;
                        mSprite.height = _initialSize.y;
                    }
                }
            }
        }
    }

    #endregion

    #region Metodos

    public void ResetInitialSize()
    {
        if (mSprite == null)
            mSprite = GetComponent<UISprite>();

        _initialSize = new IntVector3(mSprite.width, mSprite.height);
    }

    #endregion
}

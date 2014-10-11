using UnityEngine;
using System.Collections;

[System.Serializable]
public class ActionDataV2
{
    #region Variables (private)

    [SerializeField]
    private string _name;
    [SerializeField]
    public float _duration;
    [SerializeField]
    private float _deltaX;
    [SerializeField]
    private float _deltaY;
    [SerializeField]
    private float _deltaZ;
    [SerializeField]
    private float _deltaR;

    #endregion

    #region Properties (public)

    public string Name { get { return _name; } }
    public float Duration { get { return _duration; } }
    public float DeltaX { get { return _deltaX; } }
    public float DeltaY { get { return _deltaY; } }
    public float DeltaZ { get { return _deltaZ; } }
    public float DeltaR { get { return _deltaR; } }
    
    #endregion

    #region Metodos privados

    private AnimationCurve EmptyCurve()
    {
        var curve = AnimationCurve.Linear(0f, 0f, 1f, 0f);
        curve.RemoveKey(1);
        curve.RemoveKey(0);
        return curve;
    }

    #endregion

    #region Metodos publicos

    public ActionDataV2()
    {
        this._duration = 0f;
        this._deltaX = 0f;
        this._deltaY = 0f;
        this._deltaZ = 0f;
        this._deltaR = 0f;
    }

    public ActionDataV2(float duration, float deltaX, float deltaY, float deltaZ, float deltaR)
    {
        this._duration = duration;
        this._deltaX = deltaX;
        this._deltaY = deltaY;
        this._deltaZ = deltaZ;
        this._deltaR = deltaR;
    }
    
    #endregion
}